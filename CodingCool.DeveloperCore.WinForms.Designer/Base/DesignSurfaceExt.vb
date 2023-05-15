Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Windows.Forms
Imports CodingCool.DeveloperCore.WinForms.Designer.Core

Namespace Base

    Public Class DesignSurfaceExt
        Inherits DesignSurface
        Implements IDesignSurfaceExt

        Private _menuCommandService As Core.MenuCommandService = Nothing

        Public Sub SwitchTabOrder() Implements IDesignSurfaceExt.SwitchTabOrder
            If IsTabOrderMode = False Then
                InvokeTabOrder()
            Else
                DisposeTabOrder()
            End If
        End Sub

        Public Sub UseSnapLines() Implements IDesignSurfaceExt.UseSnapLines
            Dim serviceProvider As IServiceContainer = TryCast(GetService(GetType(IServiceContainer)), IServiceContainer)
            Dim opsService As DesignerOptionService = TryCast(serviceProvider.GetService(GetType(DesignerOptionService)), DesignerOptionService)
            If opsService IsNot Nothing Then serviceProvider.RemoveService(GetType(DesignerOptionService))
            Dim snapLinesService As DesignerOptionService = New DesignerOptionServiceSnapLines()
            serviceProvider.AddService(GetType(DesignerOptionService), snapLinesService)
        End Sub

        Public Sub UseGrid(gridSize As Size) Implements IDesignSurfaceExt.UseGrid
            Dim serviceProvider As IServiceContainer = TryCast(GetService(GetType(IServiceContainer)), IServiceContainer)
            Dim opsService As DesignerOptionService = TryCast(serviceProvider.GetService(GetType(DesignerOptionService)), DesignerOptionService)
            If opsService IsNot Nothing Then serviceProvider.RemoveService(GetType(DesignerOptionService))
            Dim gridService As DesignerOptionService = New DesignerOptionServiceGrid(gridSize)
            serviceProvider.AddService(GetType(DesignerOptionService), gridService)
        End Sub

        Public Sub UseGridWithoutSnapping(gridSize As Size) Implements IDesignSurfaceExt.UseGridWithoutSnapping
            Dim serviceProvider As IServiceContainer = TryCast(GetService(GetType(IServiceContainer)), IServiceContainer)
            Dim opsService As DesignerOptionService = TryCast(serviceProvider.GetService(GetType(DesignerOptionService)), DesignerOptionService)
            If opsService IsNot Nothing Then serviceProvider.RemoveService(GetType(DesignerOptionService))
            Dim noSnapService As DesignerOptionService = New DesignerOptionServiceGridWithoutSnapping(gridSize)
            serviceProvider.AddService(GetType(DesignerOptionService), noSnapService)
        End Sub

        Public Sub UseNoGuides() Implements IDesignSurfaceExt.UseNoGuides
            Dim serviceProvider As IServiceContainer = TryCast(GetService(GetType(IServiceContainer)), IServiceContainer)
            Dim opsService As DesignerOptionService = TryCast(serviceProvider.GetService(GetType(DesignerOptionService)), DesignerOptionService)
            If opsService IsNot Nothing Then serviceProvider.RemoveService(GetType(DesignerOptionService))
            Dim noGuidsService As DesignerOptionService = New DesignerOptionServiceNoGuides()
            serviceProvider.AddService(GetType(DesignerOptionService), noGuidsService)
        End Sub

        Public Function GetUndoEngine() As UndoEngineExt Implements IDesignSurfaceExt.GetUndoEngine
            Return _undoEngine
        End Function

        Private Function CreateRootComponentCore(controlType As Type, controlSize As Size, loader As DesignerLoader) As IComponent
            Try
                Dim host As IDesignerHost = GetDesignerHost()
                If host Is Nothing Then Return Nothing
                If host.RootComponent IsNot Nothing Then Return Nothing
                If loader IsNot Nothing Then
                    _loader = loader
                    BeginLoad(loader)
                    If LoadErrors.Count > 0 Then Throw New Exception($"Exception: the BeginLoad(loader) failed!")
                Else
                    BeginLoad(controlType)
                    If LoadErrors.Count > 0 Then Throw New Exception($"Exception: the BeginLoad(Type) failed! Some error during {controlType} loading")
                End If
                Dim ctrl As Control
                If TypeOf host.RootComponent Is Form Then
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.LightGray
                    Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(ctrl)
                    Dim pdS As PropertyDescriptor = pdc.Find("Size", False)
                    If pdS IsNot Nothing Then pdS.SetValue(host.RootComponent, controlSize)
                ElseIf TypeOf host.RootComponent Is UserControl Then
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.Gray
                    Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(ctrl)
                    Dim pdS As PropertyDescriptor = pdc.Find("Size", False)
                    If pdS IsNot Nothing Then pdS.SetValue(host.RootComponent, controlSize)
                ElseIf TypeOf host.RootComponent Is Control Then
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.LightGray
                    Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(ctrl)
                    Dim pdS As PropertyDescriptor = pdc.Find("Size", False)
                    If pdS IsNot Nothing Then pdS.SetValue(host.RootComponent, controlSize)
                ElseIf TypeOf host.RootComponent Is Component Then
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.White
                Else
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.Red
                End If
                Return host.RootComponent
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                If ex.InnerException IsNot Nothing Then Debug.WriteLine(ex.InnerException.Message)
                Throw
            End Try
        End Function

        Public Function CreateRootComponent(controlType As Type, controlSize As Size) As IComponent Implements IDesignSurfaceExt.CreateRootComponent
            Return CreateRootComponentCore(controlType, controlSize, Nothing)
        End Function

        Public Function CreateRootComponent(loader As DesignerLoader, controlSize As Size) As IComponent Implements IDesignSurfaceExt.CreateRootComponent
            Return CreateRootComponentCore(Nothing, controlSize, loader)
        End Function

        Public Function CreateControl(controlType As Type, controlSize As Size, controlLocation As Point) As Control Implements IDesignSurfaceExt.CreateControl
            Try
                Dim host As IDesignerHost = GetDesignerHost()
                If host Is Nothing Then Return Nothing
                If host.RootComponent Is Nothing Then Return Nothing
                Dim newComp As IComponent = host.CreateComponent(controlType)
                If newComp Is Nothing Then Return Nothing
                Dim designer As IDesigner = host.GetDesigner(newComp)
                If designer Is Nothing Then Return Nothing
                If TypeOf designer Is IComponentInitializer Then CType(designer, IComponentInitializer).InitializeNewComponent(Nothing)
                Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(newComp)
                Dim pdS As PropertyDescriptor = pdc.Find("Size", False)
                If pdS IsNot Nothing Then pdS.SetValue(newComp, controlSize)
                Dim pdL As PropertyDescriptor = pdc.Find("Location", False)
                If pdL IsNot Nothing Then pdL.SetValue(newComp, controlLocation)
                CType(newComp, Control).Parent = TryCast(host.RootComponent, Control)
                Return TryCast(newComp, Control)
            Catch exx As Exception
                Debug.WriteLine(exx.Message)
                If exx.InnerException IsNot Nothing Then Debug.WriteLine(exx.InnerException.Message)
                Throw
            End Try
        End Function

        Public Function GetDesignerHost() As IDesignerHost Implements IDesignSurfaceExt.GetDesignerHost
            Return CType(GetService(GetType(IDesignerHost)), IDesignerHost)
        End Function

        Public Function GetView() As Control Implements IDesignSurfaceExt.GetView
            Dim ctrl As Control = TryCast(View, Control)
            If ctrl Is Nothing Then Return Nothing
            ctrl.Dock = DockStyle.Fill
            Return ctrl
        End Function

#Region "TabOrder"

        Private _tabOrder As TabOrderHooker = Nothing

        Public Property IsTabOrderMode As Boolean = False

        Public Property TabOrder As TabOrderHooker
            Get
                If _tabOrder Is Nothing Then _tabOrder = New TabOrderHooker()
                Return _tabOrder
            End Get
            Set
                _tabOrder = Value
            End Set
        End Property

        Public Sub InvokeTabOrder()
            TabOrder.HookTabOrder(GetDesignerHost())
            IsTabOrderMode = True
        End Sub

        Public Sub DisposeTabOrder()
            TabOrder.HookTabOrder(GetDesignerHost())
            IsTabOrderMode = False
        End Sub

#End Region

        Private _undoEngine As UndoEngineExt
        Private _nameCreationService As NameCreationService
        Private _designerSerializationService As DesignerSerializationService
        Private _codeDomComponentSerializationService As CodeDomComponentSerializationService
        Private _toolboxService As ToolboxService
        Private _loader As BasicDesignerLoader

        Public Function GetToolboxService() As ToolboxService Implements IDesignSurfaceExt.GetToolboxService
            Return CType(GetService(GetType(IToolboxService)), ToolboxService)
        End Function
        
        Public Function GetLoader() As BasicDesignerLoader Implements IDesignSurfaceExt.GetLoader
            Return _loader
        End Function

#Region "DragDrop"

        Public Sub EnableDragAndDrop() Implements IDesignSurfaceExt.EnableDragAndDrop
            Dim ctrl As Control = GetView()
            If ctrl Is Nothing Then Return
            ctrl.AllowDrop = True
            AddHandler ctrl.DragDrop, AddressOf OnDragDrop
            Dim tbs As ToolboxService = GetToolboxService()
            If tbs Is Nothing Then Return
            AddHandler tbs.Toolbox.MouseDown, AddressOf OnListBoxMouseDown
        End Sub

        Private Sub OnListBoxMouseDown(sender As Object, e As MouseEventArgs)
            Dim tbs As ToolboxService = GetToolboxService()
            If tbs Is Nothing Then Return
            If tbs.Toolbox Is Nothing Then Return
            If tbs.Toolbox.SelectedItem Is Nothing Then Return
            tbs.Toolbox.DoDragDrop(tbs.Toolbox.SelectedItem, DragDropEffects.Copy Or DragDropEffects.Move)
        End Sub

        Public Sub OnDragDrop(sender As Object, e As DragEventArgs)
            If Not e.Data.GetDataPresent(GetType(ToolboxItem)) Then
                e.Effect = DragDropEffects.None
                Return
            End If
            Dim item As ToolboxItem = TryCast(e.Data.GetData(GetType(ToolboxItem)), ToolboxItem)
            e.Effect = DragDropEffects.Copy
            item.CreateComponents(GetDesignerHost())
        End Sub

#End Region

        Public Sub New()
            MyBase.New()
            InitServices()
        End Sub

        Public Sub New(parentProvider As IServiceProvider)
            MyBase.New(parentProvider)
            InitServices()
        End Sub

        Public Sub New(rootComponentType As Type)
            MyBase.New(rootComponentType)
            InitServices()
        End Sub

        Public Sub New(parentProvider As IServiceProvider, rootComponentType As Type)
            MyBase.New(parentProvider, rootComponentType)
            InitServices()
        End Sub

        Private Sub InitServices()
            _nameCreationService = New NameCreationService()
            If _nameCreationService IsNot Nothing Then
                ServiceContainer.RemoveService(GetType(INameCreationService), False)
                ServiceContainer.AddService(GetType(INameCreationService), _nameCreationService)
            End If
            _codeDomComponentSerializationService = New CodeDomComponentSerializationService(ServiceContainer)
            If _codeDomComponentSerializationService IsNot Nothing Then
                ServiceContainer.RemoveService(GetType(ComponentSerializationService), False)
                ServiceContainer.AddService(GetType(ComponentSerializationService), _codeDomComponentSerializationService)
            End If
            _designerSerializationService = New DesignerSerializationService(ServiceContainer)
            If _designerSerializationService IsNot Nothing Then
                ServiceContainer.RemoveService(GetType(IDesignerSerializationService), False)
                ServiceContainer.AddService(GetType(IDesignerSerializationService), _designerSerializationService)
            End If
            _undoEngine = New UndoEngineExt(ServiceContainer) With {
                .Enabled = False
            }
            If _undoEngine IsNot Nothing Then
                ServiceContainer.RemoveService(GetType(UndoEngine), False)
                ServiceContainer.AddService(GetType(UndoEngine), _undoEngine)
            End If
            _menuCommandService = New Core.MenuCommandService(Me)
            If _menuCommandService IsNot Nothing Then
                ServiceContainer.RemoveService(GetType(IMenuCommandService), False)
                ServiceContainer.AddService(GetType(IMenuCommandService), _menuCommandService)
            End If
            _toolboxService = New ToolboxService(GetDesignerHost())
            If _toolboxService IsNot Nothing Then
                ServiceContainer.RemoveService(GetType(IToolboxService), False)
                ServiceContainer.AddService(GetType(IToolboxService), _toolboxService)
            End If
        End Sub

        Public Sub DoAction(command As String) Implements IDesignSurfaceExt.DoAction
            If String.IsNullOrEmpty(command) Then Return
            Dim ims As IMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), IMenuCommandService)
            If ims Is Nothing Then Return
            Try
                Select Case command.ToUpper()
                    Case "CUT"
                        ims.GlobalInvoke(StandardCommands.Cut)
                    Case "COPY"
                        ims.GlobalInvoke(StandardCommands.Copy)
                    Case "PASTE"
                        ims.GlobalInvoke(StandardCommands.Paste)
                    Case "DELETE"
                        ims.GlobalInvoke(StandardCommands.Delete)
                End Select
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                If ex.InnerException IsNot Nothing Then Debug.WriteLine(ex.InnerException.Message)
                Throw
            End Try
        End Sub

        '#Region "CodeBehind"
        '        'TODO: Convert to DesignerLoader

        '        Public Function GetCodeBehind(lang As DesignLanguage) As String Implements IDesignSurfaceExt.GetCodeBehind
        '            Dim codeType As CodeTypeDeclaration
        '            Dim host As IDesignerHost = GetDesignerHost()
        '            Dim root As IComponent = host.RootComponent
        '            Dim mngr As New DesignerSerializationManager(host)
        '            Using mngr.CreateSession
        '                Dim serializer As TypeCodeDomSerializer = mngr.GetSerializer(root.GetType, GetType(TypeCodeDomSerializer))
        '                codeType = serializer.Serialize(mngr, root, host.Container.Components)
        '                codeType.IsPartial = True
        '                codeType.Members.OfType(Of CodeConstructor).FirstOrDefault.Attributes = MemberAttributes.Public
        '            End Using
        '            Dim builder As New StringBuilder
        '            Dim options As New CodeGeneratorOptions With {
        '                .BracingStyle = "C",
        '                .BlankLinesBetweenMembers = False
        '            }
        '            Using writer = New StringWriter(builder, CultureInfo.InvariantCulture)
        '                Select Case lang
        '                    Case DesignLanguage.CS
        '                        Using csProv = New CSharpCodeProvider
        '                            csProv.GenerateCodeFromType(codeType, writer, options)
        '                        End Using
        '                    Case DesignLanguage.VB
        '                        Using vbProv = New VBCodeProvider
        '                            vbProv.GenerateCodeFromType(codeType, writer, options)
        '                        End Using
        '                    Case Else
        '                        Throw New Exception("Invalid language")
        '                End Select
        '            End Using
        '            Return builder.ToString
        '        End Function

        '        'Public Sub LoadCodeBehind(code As String, lang As DesignLanguage) Implements IDesignSurfaceExt.LoadCodeBehind
        '        '    Dim host As IDesignerHost = GetIDesignerHost()
        '        '    Dim root As IComponent = host.RootComponent
        '        '    Dim mngr As New DesignerSerializationManager(host)
        '        '    Using mngr.CreateSession
        '        '        Dim parser As IParser = ParserFactory.CreateParser(If(lang = DesignLanguage.VB, SupportedLanguage.VBNet, SupportedLanguage.CSharp), New StringReader(code))
        '        '        parser.Parse()
        '        '        Dim visit As New CodeDomVisitor
        '        '        visit.VisitCompilationUnit(parser.CompilationUnit, Nothing)
        '        '        Dim codeType As CodeTypeDeclaration = visit.codeCompileUnit.Namespaces(0).Types(0)
        '        '        Dim serializer As TypeCodeDomSerializer = mngr.GetSerializer(root.GetType, GetType(TypeCodeDomSerializer))
        '        '        serializer.Deserialize(mngr, codeType)
        '        '    End Using
        '        'End Sub

        '        'TODO: Support referencing other object to set value
        '        'TODO: Support call methods to set values
        '        'TODO: Support more expressions
        '        'TODO: Support resources
        '        'TODO: Arrays
        '        'TODO: Not fully qualified types
        '        'TODO: Not using Me keyword
        '        'TODO: More controls
        '        Public Sub LoadCodeBehind(code As String, lang As DesignLanguage) Implements IDesignSurfaceExt.LoadCodeBehind
        '            Dim tree As VisualBasicSyntaxTree = VisualBasicSyntaxTree.ParseText(code)
        '            Dim host As IDesignerHost = GetDesignerHost()
        '            Dim root As IComponent = host.RootComponent
        '            Dim namespaces As New List(Of String)
        '            For Each import As ImportsStatementSyntax In tree.GetRoot.ChildNodes.OfType(Of ImportsStatementSyntax)

        '            Next
        '            Dim cls As ClassBlockSyntax = tree.GetRoot.ChildNodes.OfType(Of ClassBlockSyntax).FirstOrDefault
        '            If cls Is Nothing Then Throw New Exception("No class to design.")
        '            If cls.Inherits.Count <> 1 Then Throw New Exception("Cannot design more than one base type.")
        '            Dim baseType As Type = GetTypeFromString(cls.Inherits.First().Types.First().ToString)
        '            If Not GetType(Component).IsAssignableFrom(baseType) Then Throw New Exception($"The type {baseType.Name} is not supported.")
        '            Dim components As New Dictionary(Of String, Component)
        '            For Each f As FieldDeclarationSyntax In cls.ChildNodes.OfType(Of FieldDeclarationSyntax)
        '                If f.Declarators.Count <> 1 Then Throw New Exception("Invalid declaration.")
        '                Dim name As String = f.Declarators.First.Names.First.ToString
        '                If f.Declarators.First.AsClause Is Nothing Then Throw New Exception("Invalid declaration. Please specify an as clause.")
        '                If TypeOf f.Declarators.First.AsClause IsNot SimpleAsClauseSyntax Then Throw New Exception("Invalid declaration. Invalid as clause.")
        '                Dim clause As SimpleAsClauseSyntax = f.Declarators.First.AsClause
        '                Dim ctrlType As Type = GetTypeFromString(clause.Type.ToString)
        '                If GetType(Component).IsAssignableFrom(ctrlType) Then
        '                    components.Add(name, Nothing)
        '                Else
        '                    Throw New Exception($"The type {ctrlType.Name} is not supported.")
        '                End If
        '            Next
        '            Dim init As MethodBlockSyntax = cls.ChildNodes.OfType(Of MethodBlockSyntax).FirstOrDefault(Function(x) x.SubOrFunctionStatement.Identifier.ToString.ToLower = "initializecomponent")
        '            If init Is Nothing Then Throw New Exception("Could not find initialize component method.")
        '            For Each s As StatementSyntax In init.Statements
        '                If TypeOf s Is AssignmentStatementSyntax Then
        '                    Dim ss As AssignmentStatementSyntax = s
        '                    If ss.OperatorToken.ToString <> "=" Then Throw New Exception("Invalid assignment operator.")
        '                    Dim mem As MemberAccessExpressionSyntax = ss.Left
        '                    Dim name As String = mem.Name.Identifier.ToString
        '                    If TypeOf mem.Expression Is MeExpressionSyntax Then
        '                        'process the component creation
        '                        If components.ContainsKey(name) Then
        '                            If TypeOf ss.Right Is ObjectCreationExpressionSyntax Then
        '                                Dim oc As ObjectCreationExpressionSyntax = ss.Right
        '                                If oc.Initializer IsNot Nothing Then Throw New Exception("Invalid assignment, object initializers not supported.")
        '                                components(name) = host.CreateComponent(GetTypeFromString(oc.Type.ToString))
        '                            Else
        '                                Throw New Exception("Invalid assignment, expected object creation.")
        '                            End If
        '                        Else
        '                            'process parent properties
        '                            SetPropertyValueFromExpression(ss.Right, name, root)
        '                        End If
        '                    ElseIf TypeOf mem.Expression Is MemberAccessExpressionSyntax Then
        '                        'process components properties
        '                        Dim compName As String = DirectCast(mem.Expression, MemberAccessExpressionSyntax).Name.Identifier.ToString
        '                        If Not components.ContainsKey(compName) Then Throw New Exception($"Invalid assignment, component '{compName}' not found")
        '                        Dim obj As Object = components(compName)
        '                        SetPropertyValueFromExpression(ss.Right, name, obj)
        '                    End If
        '                ElseIf TypeOf s Is ExpressionStatementSyntax Then
        '                    Dim ss As ExpressionStatementSyntax = s
        '                    If TypeOf ss.Expression Is InvocationExpressionSyntax Then
        '                        Dim invoke As InvocationExpressionSyntax = ss.Expression
        '                        If TypeOf invoke.Expression Is MemberAccessExpressionSyntax Then
        '                            Dim method As MemberAccessExpressionSyntax = invoke.Expression
        '                            Dim name As String = method.Name.Identifier.ToString
        '                            If TypeOf method.Expression Is MeExpressionSyntax Then
        '                                InvokeMethod(name, root, If(invoke.ArgumentList Is Nothing, Array.Empty(Of Object), GetArrayFromArgsList(invoke.ArgumentList)))
        '                            ElseIf TypeOf method.Expression Is MemberAccessExpressionSyntax Then
        '                                Dim memAccess As MemberAccessExpressionSyntax = DirectCast(method.Expression, MemberAccessExpressionSyntax)
        '                                Dim ctrlName As String = memAccess.Name.Identifier.ToString
        '                                Dim memExp As MemberAccessExpressionSyntax = TryCast(memAccess.Expression, MemberAccessExpressionSyntax)
        '                                Dim otherName As String = memExp?.Name.Identifier.ToString
        '                                otherName = If(otherName, "")
        '                                Dim methodName As String
        '                                Dim methodParent As Object
        '                                If components.ContainsKey(ctrlName) Then
        '                                    methodName = name
        '                                    methodParent = components(ctrlName)
        '                                ElseIf components.ContainsKey(otherName) Then
        '                                    methodName = $"{ctrlName}.{name}"
        '                                    methodParent = components(otherName)
        '                                Else
        '                                    methodName = $"{ctrlName}.{name}"
        '                                    methodParent = root
        '                                End If
        '                                InvokeMethod(methodName, methodParent, If(invoke.ArgumentList Is Nothing, Array.Empty(Of Object), GetArrayFromArgsList(invoke.ArgumentList, components)))
        '                            End If
        '                        End If
        '                    End If
        '                End If
        '            Next
        '        End Sub

        '        Private Sub InvokeMethod(methodName As String, root As Object, ParamArray args As Object())
        '            Dim lProperties As String() = methodName.Split(".")
        '            Dim lastValue As Object = root
        '            Dim lastType As Type = root.GetType
        '            For i As Integer = 0 To lProperties.Length - 2
        '                Dim prop As PropertyInfo = lastType.GetProperty(lProperties(i))
        '                lastValue = prop.GetValue(lastValue)
        '                lastType = prop.PropertyType
        '            Next
        '            Dim method As MethodInfo = lastType.GetMethod(lProperties.Last, args.Select(Function(x) x.GetType).ToArray)
        '            method.Invoke(lastValue, args)
        '        End Sub

        '        Private Sub SetPropertyValueFromExpression(exp As ExpressionSyntax, name As String, root As Object)
        '            If TypeOf exp Is ObjectCreationExpressionSyntax Then
        '                Dim oc As ObjectCreationExpressionSyntax = exp
        '                If oc.Initializer IsNot Nothing Then Throw New Exception("Invalid assignment, object initializers not supported.")
        '                SetPropertyValue(GetValueFromObjectCreation(oc), name, root)
        '            ElseIf TypeOf exp Is LiteralExpressionSyntax Then
        '                SetPropertyValue(DirectCast(exp, LiteralExpressionSyntax).Token.Value, name, root)
        '            Else
        '                Throw New Exception("Invalid assignment, expected object creation.")
        '            End If
        '        End Sub

        '        Private Sub SetPropertyValue(value As Object, propName As String, root As Object)
        '            Dim lProperties As String() = propName.Split(".")
        '            Dim lastValue As Object = root
        '            Dim lastType As Type = root.GetType
        '            For i As Integer = 0 To lProperties.Length - 1
        '                Dim prop As PropertyInfo = lastType.GetProperty(lProperties(i))
        '                If lProperties.Length = i + 1 Then
        '                    prop.SetValue(lastValue, value)
        '                Else
        '                    lastValue = prop.GetValue(lastValue)
        '                    lastType = prop.PropertyType
        '                End If
        '            Next
        '        End Sub

        '        Private Function GetValueFromObjectCreation(init As ObjectCreationExpressionSyntax, Optional components As Dictionary(Of String, Component) = Nothing) As Object
        '            Dim vType As Type = GetTypeFromString(init.Type.ToString)
        '            Dim args As ArgumentListSyntax = init.ArgumentList
        '            Return If(args Is Nothing, Activator.CreateInstance(vType), Activator.CreateInstance(vType, GetArrayFromArgsList(args, components)))
        '        End Function

        '        Private Function GetArrayFromArgsList(args As ArgumentListSyntax, Optional components As Dictionary(Of String, Component) = Nothing) As Object()
        '            Dim vArgs As New List(Of Object)
        '            For Each arg As SimpleArgumentSyntax In args.ChildNodes
        '                Dim exp As ExpressionSyntax = arg.Expression
        '                If TypeOf exp Is LiteralExpressionSyntax Then
        '                    vArgs.Add(DirectCast(exp, LiteralExpressionSyntax).Token.Value)
        '                ElseIf TypeOf exp Is ObjectCreationExpressionSyntax Then
        '                    vArgs.Add(GetValueFromObjectCreation(exp, components))
        '                ElseIf TypeOf exp Is MemberAccessExpressionSyntax Then
        '                    Dim name As String = DirectCast(exp, MemberAccessExpressionSyntax).Name.Identifier.ToString
        '                    If components?.ContainsKey(name) Then
        '                        vArgs.Add(components(name))
        '                    Else
        '                        Throw New Exception($"Invalid reference, could find variable '{name}'.")
        '                    End If
        '                End If
        '            Next
        '            Return vArgs.ToArray
        '        End Function

        '        Public Function GetTypeFromString(name As String) As Type
        '            For Each assembly In AppDomain.CurrentDomain.GetAssemblies().Reverse()
        '                Dim tt As Type = assembly.[GetType](name)
        '                If tt IsNot Nothing Then
        '                    Return tt
        '                End If
        '            Next
        '            Return Nothing
        '        End Function

        '#End Region

    End Class
End Namespace