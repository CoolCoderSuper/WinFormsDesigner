Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Globalization
Imports System.IO
Imports System.Threading
Imports System.Windows.Forms
Imports System.Xml
Imports KGySoft.Serialization.Binary
'TODO: Read references
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
                If _rootComponentType.IsSubclassOf(GetType(Component)) Then
                    _host.CreateComponent(_rootComponentType)
                    baseClassName = $"{_rootComponentType.Name}1"
                Else
                    Throw New Exception("Undefined Host Type: " & _rootComponentType.ToString())
                End If
            Else
                baseClassName = New XmlParser(New DesignerComponentProvider(_host)).Parse(_code, errors, _xmlDocument)
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
                'Dim formatter As BinaryFormatter = New BinaryFormatter()
                'Dim stream As MemoryStream = New MemoryStream()
                'formatter.Serialize(stream, value)
                Dim binaryNode As XmlNode = WriteBinary(document, BinarySerializer.Serialize(value))'WriteBinary(document, stream.ToArray())
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
            'Dim formatter As BinaryFormatter = New BinaryFormatter()
            'Dim stream As MemoryStream = New MemoryStream()
            'formatter.Serialize(stream, desc.MemberInfo)
            Dim memberAttr As XmlAttribute = document.CreateAttribute("member")
            memberAttr.Value = Convert.ToBase64String(BinarySerializer.Serialize(desc.MemberInfo))'Convert.ToBase64String(stream.ToArray())
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