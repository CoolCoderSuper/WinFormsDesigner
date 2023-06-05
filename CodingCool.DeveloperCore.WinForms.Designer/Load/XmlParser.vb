Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Xml
Imports KGySoft.Serialization.Binary

Namespace Load
    Public Class XmlParser
        Private ReadOnly _provider As IComponentProvider

        Public Sub New(provider As IComponentProvider)
            _provider = provider
        End Sub
        
        Public Function Parse(fileName As String, errors As ArrayList, <Out> ByRef document As XmlDocument) As String
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
                        ParseObject(node, errors)
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

        Private Sub ParseEvent(childNode As XmlNode, instance As Object, errors As ArrayList)
            Dim bindings As IEventBindingService = _provider.GetEventService()
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
                errors.Add(String.Format("Event {0} has no method bound to it", nameAttr.Value))
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
                errors.Add(ex)
            End Try
        End Sub

        Private Function ParseInstanceDescriptor(node As XmlNode, errors As ArrayList) As Object
            Dim memberAttr As XmlAttribute = node.Attributes("member")
            If memberAttr Is Nothing Then
                errors.Add("No member attribute on instance descriptor")
                Return Nothing
            End If
            Dim data As Byte() = Convert.FromBase64String(memberAttr.Value)
            Dim mi As MemberInfo = BinarySerializer.Deserialize(data)
            Dim args As Object() = Nothing
            Dim memberInfo As MethodBase = TryCast(mi, MethodBase)
            If memberInfo IsNot Nothing
                Dim paramInfos As ParameterInfo() = (memberInfo).GetParameters()
                args = New Object(paramInfos.Length - 1) {}
                Dim idx As Integer = 0
                For Each child As XmlNode In node.ChildNodes
                    If child.Name.Equals("Argument") Then
                        Dim value As Object
                        If Not ParseValue(child, TypeDescriptor.GetConverter(paramInfos(idx).ParameterType), errors, value) Then
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
                    ParseProperty(prop, instance, errors)
                End If
            Next
            Return instance
        End Function

        Private Function ParseObject(node As XmlNode, errors As ArrayList) As Object
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
                    instance = _provider.CreateComponent(type)
                Else
                    instance = _provider.CreateComponent(type, nameAttr.Value)
                End If
            Else
                instance = Activator.CreateInstance(type)
            End If
            Dim childAttr As XmlAttribute = node.Attributes("children")
            Dim childList As IList = Nothing
            Dim tempList As New List(Of Object)
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
                        Dim childInstance As Object = ParseObject(childNode, errors)
                        tempList.Add(childInstance)
                    End If
                ElseIf childNode.Name.Equals("Property") Then
                    ParseProperty(childNode, instance, errors)
                ElseIf childNode.Name.Equals("Event") Then
                    ParseEvent(childNode, instance, errors)
                End If
            Next
            If childList IsNot Nothing Then
                For Each obj As Object In tempList
                    childList.Add(obj)
                Next
            End If
            Return instance
        End Function

        Private Sub ParseProperty(node As XmlNode, instance As Object, errors As ArrayList)
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
                            If ParseValue(child, TypeDescriptor.GetConverter(type), errors, item) Then
                                Try
                                    CType(value, IList).Add(item)
                                Catch ex As Exception
                                    errors.Add(ex)
                                End Try
                            End If
                        Else
                            errors.Add(String.Format("Only Item elements are allowed in collections, not {0} elements.", child.Name))
                        End If
                    Next
                Else
                    For Each child As XmlNode In node.ChildNodes
                        If child.Name.Equals("Property") Then
                            ParseProperty(child, value, errors)
                        Else
                            errors.Add(String.Format("Only Property elements are allowed in content properties, not {0} elements.", child.Name))
                        End If
                    Next
                End If
            Else
                Dim value As Object
                If ParseValue(node, prop.Converter, errors, value) Then
                    Try
                        prop.SetValue(instance, value)
                    Catch ex As Exception
                        errors.Add(ex)
                    End Try
                End If
            End If
        End Sub

        Private Function ParseValue(node As XmlNode, converter As TypeConverter, errors As ArrayList, <Out> ByRef value As Object) As Boolean
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
                            value = BinarySerializer.Deserialize(data)
                            Return True
                        End If
                    ElseIf child.Name.Equals("InstanceDescriptor") Then
                        value = ParseInstanceDescriptor(child, errors)
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
                errors.Add(ex)
                value = Nothing
                Return False
            End Try
        End Function

        Private Shared Function GetConversionSupported(converter As TypeConverter, conversionType As Type) As Boolean
            Return (converter.CanConvertFrom(conversionType) AndAlso converter.CanConvertTo(conversionType))
        End Function
    End Class
End NameSpace