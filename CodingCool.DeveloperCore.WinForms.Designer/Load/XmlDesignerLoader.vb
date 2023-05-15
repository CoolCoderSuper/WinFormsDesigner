Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Threading
Imports System.Windows.Forms
Imports System.Xml
'stolen with glee from a random msdn article that i cant find
Namespace Load
    Public Class XmlDesignerLoader
        Inherits BasicDesignerLoader

        Private _root As IComponent
        Private _dirty As Boolean = True
        Private _unsaved As Boolean
        Private ReadOnly _code As String
        Private _host As IDesignerLoaderHost
        Private _xmlDocument As XmlDocument
        Private Shared ReadOnly _propertyAttributes As Attribute() = New Attribute() {DesignOnlyAttribute.No}
        Private ReadOnly _rootComponentType As Type

        Public Sub New(rootComponentType As Type)
            _rootComponentType = rootComponentType
            Modified = True
        End Sub

        Public Sub New(code As String)
            If code Is Nothing Then
                Throw New ArgumentNullException(NameOf(code))
            End If
            _code = code
        End Sub

        Protected Overrides Sub PerformLoad(designerSerializationManager As IDesignerSerializationManager)
            _host = LoaderHost
            If _host Is Nothing Then
                Throw New ArgumentNullException("BasicHostLoader.BeginLoad: Invalid designerLoaderHost.")
            End If
            Dim errors As ArrayList = New ArrayList()
            Dim successful As Boolean = True
            Dim baseClassName As String
            If _code Is Nothing Then
                If _rootComponentType = GetType(Form) Then
                    _host.CreateComponent(GetType(Form))
                    baseClassName = "Form1"
                ElseIf _rootComponentType = GetType(UserControl) Then
                    _host.CreateComponent(GetType(UserControl))
                    baseClassName = "UserControl1"
                ElseIf _rootComponentType = GetType(Component) Then
                    _host.CreateComponent(GetType(Component))
                    baseClassName = "Component1"
                Else
                    Throw New Exception("Undefined Host Type: " & _rootComponentType.ToString())
                End If
            Else
                baseClassName = ReadCode(_code, errors, _xmlDocument)
            End If
            Dim cs As IComponentChangeService = TryCast(_host.GetService(GetType(IComponentChangeService)), IComponentChangeService)
            If cs IsNot Nothing Then
                AddHandler cs.ComponentAdded, AddressOf OnComponentAddedRemoved
                AddHandler cs.ComponentRemoved, AddressOf OnComponentAddedRemoved
                AddHandler cs.ComponentChanged, AddressOf OnComponentChanged
            End If
            _host.EndLoad(baseClassName, successful, errors)
            _dirty = True
            _unsaved = False
        End Sub

        Protected Overrides Sub PerformFlush(designerSerializationManager As IDesignerSerializationManager)
            If Not _dirty Then
                Return
            End If
            PerformFlushWorker()
        End Sub

        Public Overrides Sub Dispose()
            Dim cs As IComponentChangeService = TryCast(_host.GetService(GetType(IComponentChangeService)), IComponentChangeService)

            If cs IsNot Nothing Then
                RemoveHandler cs.ComponentAdded, AddressOf OnComponentAddedRemoved
                RemoveHandler cs.ComponentRemoved, AddressOf OnComponentAddedRemoved
                RemoveHandler cs.ComponentChanged, AddressOf OnComponentChanged
            End If
        End Sub

        Private Function GetConversionSupported(converter As TypeConverter, conversionType As Type) As Boolean
            Return (converter.CanConvertFrom(conversionType) AndAlso converter.CanConvertTo(conversionType))
        End Function

        Private Sub OnComponentChanged(sender As Object, ce As ComponentChangedEventArgs)
            _dirty = True
            _unsaved = True
        End Sub

        Private Sub OnComponentAddedRemoved(sender As Object, ce As ComponentEventArgs)
            _dirty = True
            _unsaved = True
        End Sub

        Private Sub PerformFlushWorker()
            Dim document As XmlDocument = New XmlDocument()
            document.AppendChild(document.CreateElement("DOCUMENT_ELEMENT"))
            Dim idh As IDesignerHost = CType(Me._host.GetService(GetType(IDesignerHost)), IDesignerHost)
            _root = idh.RootComponent
            Dim nametable As Hashtable = New Hashtable(idh.Container.Components.Count)
            Dim manager As IDesignerSerializationManager = TryCast(_host.GetService(GetType(IDesignerSerializationManager)), IDesignerSerializationManager)
            document.DocumentElement.AppendChild(WriteObject(document, nametable, _root))

            For Each comp As IComponent In idh.Container.Components
                If comp IsNot _root AndAlso Not nametable.ContainsKey(comp) Then
                    document.DocumentElement.AppendChild(WriteObject(document, nametable, comp))
                End If
            Next
            _xmlDocument = document
        End Sub

        Private Function WriteObject(document As XmlDocument, nametable As IDictionary, value As Object) As XmlNode
            Dim idh As IDesignerHost = CType(Me._host.GetService(GetType(IDesignerHost)), IDesignerHost)
            Debug.Assert(value IsNot Nothing, "Should not invoke WriteObject with a null value")
            Dim node As XmlNode = document.CreateElement("Object")
            Dim typeAttr As XmlAttribute = document.CreateAttribute("type")
            typeAttr.Value = value.[GetType]().AssemblyQualifiedName
            node.Attributes.Append(typeAttr)
            Dim component As IComponent = TryCast(value, IComponent)

            If component IsNot Nothing AndAlso component.Site IsNot Nothing AndAlso component.Site.Name IsNot Nothing Then
                Dim nameAttr As XmlAttribute = document.CreateAttribute("name")
                nameAttr.Value = component.Site.Name
                node.Attributes.Append(nameAttr)
                Debug.Assert(nametable(component) Is Nothing, "WriteObject should not be called more than once for the same object.  Use WriteReference instead")
                nametable(value) = component.Site.Name
            End If

            Dim isControl As Boolean = (TypeOf value Is Control)

            If isControl Then
                Dim childAttr As XmlAttribute = document.CreateAttribute("children")
                childAttr.Value = "Controls"
                node.Attributes.Append(childAttr)
            End If

            If component IsNot Nothing Then

                If isControl Then

                    For Each child As Control In (CType(value, Control)).Controls

                        If child.Site IsNot Nothing AndAlso child.Site.Container Is idh.Container Then
                            node.AppendChild(WriteObject(document, nametable, child))
                        End If
                    Next
                End If

                Dim properties As PropertyDescriptorCollection = TypeDescriptor.GetProperties(value, _propertyAttributes)

                If isControl Then
                    Dim controlProp As PropertyDescriptor = properties("Controls")

                    If controlProp IsNot Nothing Then
                        Dim propArray As PropertyDescriptor() = New PropertyDescriptor(properties.Count - 1 - 1) {}
                        Dim idx As Integer = 0

                        For Each p As PropertyDescriptor In properties

                            If p IsNot controlProp Then
                                propArray(Math.Min(Interlocked.Increment(idx), idx - 1)) = p
                            End If
                        Next

                        properties = New PropertyDescriptorCollection(propArray)
                    End If
                End If

                WriteProperties(document, properties, value, node, "Property")
                Dim events As EventDescriptorCollection = TypeDescriptor.GetEvents(value, _propertyAttributes)
                Dim bindings As IEventBindingService = TryCast(_host.GetService(GetType(IEventBindingService)), IEventBindingService)

                If bindings IsNot Nothing Then
                    properties = bindings.GetEventProperties(events)
                    WriteProperties(document, properties, value, node, "Event")
                End If
            Else
                WriteValue(document, value, node)
            End If

            Return node
        End Function

        Private Sub WriteProperties(document As XmlDocument, properties As PropertyDescriptorCollection, value As Object, parent As XmlNode, elementName As String)
            For Each prop As PropertyDescriptor In properties
                If prop.ShouldSerializeValue(value) Then
                    Dim node As XmlNode = document.CreateElement(elementName)
                    Dim attr As XmlAttribute = document.CreateAttribute("name")
                    attr.Value = prop.Name
                    node.Attributes.Append(attr)
                    Dim visibility As DesignerSerializationVisibilityAttribute = CType(prop.Attributes(GetType(DesignerSerializationVisibilityAttribute)), DesignerSerializationVisibilityAttribute)

                    Select Case visibility.Visibility
                        Case DesignerSerializationVisibility.Visible

                            If Not prop.IsReadOnly AndAlso WriteValue(document, prop.GetValue(value), node) Then
                                parent.AppendChild(node)
                            End If

                        Case DesignerSerializationVisibility.Content
                            Dim propValue As Object = prop.GetValue(value)

                            If GetType(IList).IsAssignableFrom(prop.PropertyType) Then
                                WriteCollection(document, CType(propValue, IList), node)
                            Else
                                Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(propValue, _propertyAttributes)
                                WriteProperties(document, props, propValue, node, elementName)
                            End If

                            If node.ChildNodes.Count > 0 Then
                                parent.AppendChild(node)
                            End If
                    End Select
                End If
            Next
        End Sub

        Private Function WriteReference(document As XmlDocument, value As IComponent) As XmlNode
            Dim idh As IDesignerHost = CType(_host.GetService(GetType(IDesignerHost)), IDesignerHost)
            Debug.Assert(value IsNot Nothing AndAlso value.Site IsNot Nothing AndAlso value.Site.Container Is idh.Container, "Invalid component passed to WriteReference")
            Dim node As XmlNode = document.CreateElement("Reference")
            Dim attr As XmlAttribute = document.CreateAttribute("name")
            attr.Value = value.Site.Name
            node.Attributes.Append(attr)
            Return node
        End Function

        Private Function WriteValue(document As XmlDocument, value As Object, parent As XmlNode) As Boolean
            Dim idh As IDesignerHost = CType(_host.GetService(GetType(IDesignerHost)), IDesignerHost)

            If value Is Nothing Then
                Return True
            End If

            Dim converter As TypeConverter = TypeDescriptor.GetConverter(value)

            If GetConversionSupported(converter, GetType(String)) Then
                parent.InnerText = CStr(converter.ConvertTo(Nothing, CultureInfo.InvariantCulture, value, GetType(String)))
            ElseIf GetConversionSupported(converter, GetType(Byte())) Then
                Dim data As Byte() = CType(converter.ConvertTo(Nothing, CultureInfo.InvariantCulture, value, GetType(Byte())), Byte())
                parent.AppendChild(WriteBinary(document, data))
            ElseIf GetConversionSupported(converter, GetType(InstanceDescriptor)) Then
                Dim id As InstanceDescriptor = CType(converter.ConvertTo(Nothing, CultureInfo.InvariantCulture, value, GetType(InstanceDescriptor)), InstanceDescriptor)
                parent.AppendChild(WriteInstanceDescriptor(document, id, value))
            ElseIf TypeOf value Is IComponent AndAlso (CType(value, IComponent)).Site IsNot Nothing AndAlso CType(value, IComponent).Site.Container Is idh.Container Then
                parent.AppendChild(WriteReference(document, CType(value, IComponent)))
            ElseIf value.[GetType]().IsSerializable Then
                Dim formatter As BinaryFormatter = New BinaryFormatter()
                Dim stream As MemoryStream = New MemoryStream()
                formatter.Serialize(stream, value)
                Dim binaryNode As XmlNode = WriteBinary(document, stream.ToArray())
                parent.AppendChild(binaryNode)
            Else
                Return False
            End If

            Return True
        End Function

        Private Sub WriteCollection(document As XmlDocument, list As IList, parent As XmlNode)
            For Each obj As Object In list
                Dim node As XmlNode = document.CreateElement("Item")
                Dim typeAttr As XmlAttribute = document.CreateAttribute("type")
                typeAttr.Value = obj.[GetType]().AssemblyQualifiedName
                node.Attributes.Append(typeAttr)
                WriteValue(document, obj, node)
                parent.AppendChild(node)
            Next
        End Sub

        Private Function WriteBinary(document As XmlDocument, value As Byte()) As XmlNode
            Dim node As XmlNode = document.CreateElement("Binary")
            node.InnerText = Convert.ToBase64String(value)
            Return node
        End Function

        Private Function WriteInstanceDescriptor(document As XmlDocument, desc As InstanceDescriptor, value As Object) As XmlNode
            Dim node As XmlNode = document.CreateElement("InstanceDescriptor")
            Dim formatter As BinaryFormatter = New BinaryFormatter()
            Dim stream As MemoryStream = New MemoryStream()
            formatter.Serialize(stream, desc.MemberInfo)
            Dim memberAttr As XmlAttribute = document.CreateAttribute("member")
            memberAttr.Value = Convert.ToBase64String(stream.ToArray())
            node.Attributes.Append(memberAttr)

            For Each arg As Object In desc.Arguments
                Dim argNode As XmlNode = document.CreateElement("Argument")

                If WriteValue(document, arg, argNode) Then
                    node.AppendChild(argNode)
                End If
            Next

            If Not desc.IsComplete Then
                Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(value, _propertyAttributes)
                WriteProperties(document, props, value, node, "Property")
            End If

            Return node
        End Function

        Private Function ReadCode(fileName As String, errors As ArrayList, <Out> ByRef document As XmlDocument) As String
            Dim baseClass As String = Nothing

            Try
                Dim sr As New StringReader(fileName)
                Dim cleandown As String = sr.ReadToEnd()
                cleandown = "<DOCUMENT_ELEMENT>" & cleandown & "</DOCUMENT_ELEMENT>"
                Dim doc As XmlDocument = New XmlDocument()
                doc.LoadXml(cleandown)

                For Each node As XmlNode In doc.DocumentElement.ChildNodes

                    If baseClass Is Nothing Then
                        baseClass = node.Attributes("name").Value
                    End If

                    If node.Name.Equals("Object") Then
                        ReadObject(node, errors)
                    Else
                        errors.Add(String.Format("Node type {0} is not allowed here.", node.Name))
                    End If
                Next

                document = doc
            Catch ex As Exception
                document = Nothing
                errors.Add(ex)
            End Try

            Return baseClass
        End Function

        Private Sub ReadEvent(childNode As XmlNode, instance As Object, errors As ArrayList)
            Dim bindings As IEventBindingService = TryCast(_host.GetService(GetType(IEventBindingService)), IEventBindingService)

            If bindings Is Nothing Then
                errors.Add("Unable to contact event binding service so we can't bind any events")
                Return
            End If

            Dim nameAttr As XmlAttribute = childNode.Attributes("name")

            If nameAttr Is Nothing Then
                errors.Add("No event name")
                Return
            End If

            Dim methodAttr As XmlAttribute = childNode.Attributes("method")

            If methodAttr Is Nothing OrElse methodAttr.Value Is Nothing OrElse methodAttr.Value.Length = 0 Then
                errors.Add(String.Format("Event {0} has no method bound to it"))
                Return
            End If

            Dim evt As EventDescriptor = TypeDescriptor.GetEvents(instance)(nameAttr.Value)

            If evt Is Nothing Then
                errors.Add(String.Format("Event {0} does not exist on {1}", nameAttr.Value, instance.[GetType]().FullName))
                Return
            End If

            Dim prop As PropertyDescriptor = bindings.GetEventProperty(evt)
            Debug.Assert(prop IsNot Nothing, "Bad event binding service")

            Try
                prop.SetValue(instance, methodAttr.Value)
            Catch ex As Exception
                errors.Add(ex.Message)
            End Try
        End Sub

        Private Function ReadInstanceDescriptor(node As XmlNode, errors As ArrayList) As Object
            Dim memberAttr As XmlAttribute = node.Attributes("member")

            If memberAttr Is Nothing Then
                errors.Add("No member attribute on instance descriptor")
                Return Nothing
            End If

            Dim data As Byte() = Convert.FromBase64String(memberAttr.Value)
            Dim formatter As BinaryFormatter = New BinaryFormatter()
            Dim stream As MemoryStream = New MemoryStream(data)
            Dim mi As MemberInfo = CType(formatter.Deserialize(stream), MemberInfo)
            Dim args As Object() = Nothing
            Dim memberInfo As MethodBase = TryCast(mi, MethodBase)
            If memberInfo IsNot Nothing
                Dim paramInfos As ParameterInfo() = (memberInfo).GetParameters()
                args = New Object(paramInfos.Length - 1) {}
                Dim idx As Integer = 0

                For Each child As XmlNode In node.ChildNodes

                    If child.Name.Equals("Argument") Then
                        Dim value As Object

                        If Not ReadValue(child, TypeDescriptor.GetConverter(paramInfos(idx).ParameterType), errors, value) Then
                            Return Nothing
                        End If

                        args(Math.Min(Interlocked.Increment(idx), idx - 1)) = value
                    End If
                Next

                If idx <> paramInfos.Length Then
                    errors.Add(String.Format("Member {0} requires {1} arguments, not {2}.", mi.Name, args.Length, idx))
                    Return Nothing
                End If
            End If

            Dim id As InstanceDescriptor = New InstanceDescriptor(mi, args)
            Dim instance As Object = id.Invoke()

            For Each prop As XmlNode In node.ChildNodes

                If prop.Name.Equals("Property") Then
                    ReadProperty(prop, instance, errors)
                End If
            Next

            Return instance
        End Function

        Private Function ReadObject(node As XmlNode, errors As ArrayList) As Object
            Dim typeAttr As XmlAttribute = node.Attributes("type")

            If typeAttr Is Nothing Then
                errors.Add("<Object> tag is missing required type attribute")
                Return Nothing
            End If

            Dim type As Type = Type.[GetType](typeAttr.Value)

            If type Is Nothing Then
                errors.Add(String.Format("Type {0} could not be loaded.", typeAttr.Value))
                Return Nothing
            End If

            Dim nameAttr As XmlAttribute = node.Attributes("name")
            Dim instance As Object

            If GetType(IComponent).IsAssignableFrom(type) Then

                If nameAttr Is Nothing Then
                    instance = _host.CreateComponent(type)
                Else
                    instance = _host.CreateComponent(type, nameAttr.Value)
                End If
            Else
                instance = Activator.CreateInstance(type)
            End If

            Dim childAttr As XmlAttribute = node.Attributes("children")
            Dim childList As IList = Nothing

            If childAttr IsNot Nothing Then
                Dim childProp As PropertyDescriptor = TypeDescriptor.GetProperties(instance)(childAttr.Value)

                If childProp Is Nothing Then
                    errors.Add(String.Format("The children attribute lists {0} as the child collection but this is not a property on {1}", childAttr.Value, instance.[GetType]().FullName))
                Else
                    childList = TryCast(childProp.GetValue(instance), IList)

                    If childList Is Nothing Then
                        errors.Add(String.Format("The property {0} was found but did not return a valid IList", childProp.Name))
                    End If
                End If
            End If

            For Each childNode As XmlNode In node.ChildNodes

                If childNode.Name.Equals("Object") Then

                    If childAttr Is Nothing Then
                        errors.Add("Child object found but there is no children attribute")
                        Continue For
                    End If

                    If childList IsNot Nothing Then
                        Dim childInstance As Object = ReadObject(childNode, errors)
                        childList.Add(childInstance)
                    End If
                ElseIf childNode.Name.Equals("Property") Then
                    ReadProperty(childNode, instance, errors)
                ElseIf childNode.Name.Equals("Event") Then
                    ReadEvent(childNode, instance, errors)
                End If
            Next

            Return instance
        End Function

        Private Sub ReadProperty(node As XmlNode, instance As Object, errors As ArrayList)
            Dim nameAttr As XmlAttribute = node.Attributes("name")

            If nameAttr Is Nothing Then
                errors.Add("Property has no name")
                Return
            End If

            Dim prop As PropertyDescriptor = TypeDescriptor.GetProperties(instance)(nameAttr.Value)

            If prop Is Nothing Then
                errors.Add(String.Format("Property {0} does not exist on {1}", nameAttr.Value, instance.[GetType]().FullName))
                Return
            End If

            Dim isContent As Boolean = prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content)

            If isContent Then
                Dim value As Object = prop.GetValue(instance)

                If TypeOf value Is IList Then

                    For Each child As XmlNode In node.ChildNodes

                        If child.Name.Equals("Item") Then
                            Dim item As Object
                            Dim typeAttr As XmlAttribute = child.Attributes("type")

                            If typeAttr Is Nothing Then
                                errors.Add("Item has no type attribute")
                                Continue For
                            End If

                            Dim type As Type = Type.[GetType](typeAttr.Value)

                            If type Is Nothing Then
                                errors.Add(String.Format("Item type {0} could not be found.", typeAttr.Value))
                                Continue For
                            End If

                            If ReadValue(child, TypeDescriptor.GetConverter(type), errors, item) Then

                                Try
                                    CType(value, IList).Add(item)
                                Catch ex As Exception
                                    errors.Add(ex.Message)
                                End Try
                            End If
                        Else
                            errors.Add(String.Format("Only Item elements are allowed in collections, not {0} elements.", child.Name))
                        End If
                    Next
                Else

                    For Each child As XmlNode In node.ChildNodes

                        If child.Name.Equals("Property") Then
                            ReadProperty(child, value, errors)
                        Else
                            errors.Add(String.Format("Only Property elements are allowed in content properties, not {0} elements.", child.Name))
                        End If
                    Next
                End If
            Else
                Dim value As Object

                If ReadValue(node, prop.Converter, errors, value) Then

                    Try
                        prop.SetValue(instance, value)
                    Catch ex As Exception
                        errors.Add(ex.Message)
                    End Try
                End If
            End If
        End Sub

        Private Function ReadValue(node As XmlNode, converter As TypeConverter, errors As ArrayList, <Out> ByRef value As Object) As Boolean
            Try

                For Each child As XmlNode In node.ChildNodes

                    If child.NodeType = XmlNodeType.Text Then
                        value = converter.ConvertFromInvariantString(node.InnerText)
                        Return True
                    ElseIf child.Name.Equals("Binary") Then
                        Dim data As Byte() = Convert.FromBase64String(child.InnerText)

                        If GetConversionSupported(converter, GetType(Byte())) Then
                            value = converter.ConvertFrom(Nothing, CultureInfo.InvariantCulture, data)
                            Return True
                        Else
                            Dim formatter As BinaryFormatter = New BinaryFormatter()
                            Dim stream As MemoryStream = New MemoryStream(data)
                            value = formatter.Deserialize(stream)
                            Return True
                        End If
                    ElseIf child.Name.Equals("InstanceDescriptor") Then
                        value = ReadInstanceDescriptor(child, errors)
                        Return (value IsNot Nothing)
                    Else
                        errors.Add(String.Format("Unexpected element type {0}", child.Name))
                        value = Nothing
                        Return False
                    End If
                Next

                value = Nothing
                Return True
            Catch ex As Exception
                errors.Add(ex.Message)
                value = Nothing
                Return False
            End Try
        End Function

        Public Function GetCode() As String
            Flush()
            Dim sw As StringWriter
            sw = New StringWriter()
            Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
            xtw.Formatting = Formatting.Indented
            _xmlDocument.WriteTo(xtw)
            Dim cleanup As String = sw.ToString().Replace("<DOCUMENT_ELEMENT>", "")
            cleanup = cleanup.Replace("</DOCUMENT_ELEMENT>", "")
            sw.Close()
            Return cleanup
        End Function
    End Class
End Namespace