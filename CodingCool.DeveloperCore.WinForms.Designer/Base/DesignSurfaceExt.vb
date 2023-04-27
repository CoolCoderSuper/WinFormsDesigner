Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Windows.Forms
Imports CodingCool.DeveloperCore.WinForms.Designer.Core
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CSharp

'this class adds to a .NET
'   DesignSurface instance
'the following facilities:
'    * TabOrder
'    * UndoEngine
'    * Cut/Copy/Paste/Delete commands
' DesignSurfaceExt
'    |
'    +--|DesignSurface|
'    |
'    +--|TabOrder|
'    |
'    +--|UndoEngine|
'    |
'    +--|Cut/Copy/Paste/Delete commands|
Namespace Base

    Public Class DesignSurfaceExt
        Inherits DesignSurface
        Implements IDesignSurfaceExt

        Private Const _Name_ As String = "DesignSurfaceExt"

#Region "IDesignSurfaceExt Members"

        Private _menuCommandService As MenuCommandServiceExt = Nothing

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
            Dim opsService2 As DesignerOptionService = New DesignerOptionServiceExt4SnapLines()
            serviceProvider.AddService(GetType(DesignerOptionService), opsService2)
        End Sub

        Public Sub UseGrid(gridSize As Size) Implements IDesignSurfaceExt.UseGrid
            Dim serviceProvider As IServiceContainer = TryCast(GetService(GetType(IServiceContainer)), IServiceContainer)
            Dim opsService As DesignerOptionService = TryCast(serviceProvider.GetService(GetType(DesignerOptionService)), DesignerOptionService)
            If opsService IsNot Nothing Then serviceProvider.RemoveService(GetType(DesignerOptionService))
            Dim opsService2 As DesignerOptionService = New DesignerOptionServiceExt4Grid(gridSize)
            serviceProvider.AddService(GetType(DesignerOptionService), opsService2)
        End Sub

        Public Sub UseGridWithoutSnapping(gridSize As Size) Implements IDesignSurfaceExt.UseGridWithoutSnapping
            Dim serviceProvider As IServiceContainer = TryCast(GetService(GetType(IServiceContainer)), IServiceContainer)
            Dim opsService As DesignerOptionService = TryCast(serviceProvider.GetService(GetType(DesignerOptionService)), DesignerOptionService)
            If opsService IsNot Nothing Then serviceProvider.RemoveService(GetType(DesignerOptionService))
            Dim opsService2 As DesignerOptionService = New DesignerOptionServiceExt4GridWithoutSnapping(gridSize)
            serviceProvider.AddService(GetType(DesignerOptionService), opsService2)
        End Sub

        Public Sub UseNoGuides() Implements IDesignSurfaceExt.UseNoGuides
            Dim serviceProvider As IServiceContainer = TryCast(GetService(GetType(IServiceContainer)), IServiceContainer)
            Dim opsService As DesignerOptionService = TryCast(serviceProvider.GetService(GetType(DesignerOptionService)), DesignerOptionService)
            If opsService IsNot Nothing Then serviceProvider.RemoveService(GetType(DesignerOptionService))
            Dim opsService2 As DesignerOptionService = New DesignerOptionServiceExt4NoGuides()
            serviceProvider.AddService(GetType(DesignerOptionService), opsService2)
        End Sub

        Public Function GetUndoEngineExt() As UndoEngineExt Implements IDesignSurfaceExt.GetUndoEngineExt
            Return _undoEngine
        End Function

        Private Function CreateRootComponentCore(controlType As Type, controlSize As Size, loader As DesignerLoader) As IComponent
            Const _signature_ As String = _Name_ & "::CreateRootComponentCore()"

            Try
                'step.1
                'get the IDesignerHost
                'if we are not not able to get it
                'then rollback (return without do nothing)
                'check if the root component has already been set
                'if so then rollback (return without do nothing)
                'step.2
                'create a new root component and initialize it via its designer
                'if the component has not a designer
                'then rollback (return without do nothing)
                'else do the initialization
                Dim host As IDesignerHost = GetIDesignerHost()

                If host Is Nothing Then Return Nothing

                'check If the root component has already been set
                'if so then rollback (return without do nothing)
                If host.RootComponent IsNot Nothing Then Return Nothing

                'step.2
                'create a new root component and initialize it via its designer
                'if the component has not a designer
                'then rollback (return without do nothing)
                'else do the initialization
                If loader IsNot Nothing Then
                    BeginLoad(loader)
                    If LoadErrors.Count > 0 Then Throw New Exception($"{_signature_} - Exception: the BeginLoad(loader) failed!")
                Else
                    BeginLoad(controlType)
                    If LoadErrors.Count > 0 Then Throw New Exception($"{_signature_} - Exception: the BeginLoad(Type) failed! Some error during {controlType} loading")
                End If
                'step.3
                'try to modify the Size of the object just created
                Dim ihost As IDesignerHost = GetIDesignerHost()
                'Set the backcolor and the Size
                Dim ctrl As Control = Nothing

                If TypeOf host.RootComponent Is Form Then
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.LightGray
                    'set the Size
                    Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(ctrl)
                    'Sets a PropertyDescriptor to the specific property
                    Dim pdS As PropertyDescriptor = pdc.Find("Size", False)
                    If pdS IsNot Nothing Then pdS.SetValue(ihost.RootComponent, controlSize)
                ElseIf TypeOf host.RootComponent Is UserControl Then
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.Gray
                    'set the Size
                    Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(ctrl)
                    'Sets a PropertyDescriptor to the specific property
                    Dim pdS As PropertyDescriptor = pdc.Find("Size", False)
                    If pdS IsNot Nothing Then pdS.SetValue(ihost.RootComponent, controlSize)
                ElseIf TypeOf host.RootComponent Is Control Then
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.LightGray
                    'set the Size
                    Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(ctrl)
                    'Sets a PropertyDescriptor to the specific property
                    Dim pdS As PropertyDescriptor = pdc.Find("Size", False)
                    If pdS IsNot Nothing Then pdS.SetValue(ihost.RootComponent, controlSize)
                ElseIf TypeOf host.RootComponent Is Component Then
                    ctrl = TryCast(View, Control)
                    'don't set the Size
                    ctrl.BackColor = Color.White
                Else
                    'Undefined Host Type
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.Red
                End If

                Return ihost.RootComponent
            Catch exx As Exception
                Debug.WriteLine(exx.Message)
                If exx.InnerException IsNot Nothing Then Debug.WriteLine(exx.InnerException.Message)
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
                'step.1
                'get the IDesignerHost
                'if we are not able to get it
                'then rollback (return without do nothing)
                'check if the root component has already been set
                'if not so then rollback (return without do nothing)
                Dim host As IDesignerHost = GetIDesignerHost()
                If host Is Nothing Then Return Nothing
                'check if the root component has already been set
                'if not so then rollback (return without do nothing)
                If host.RootComponent Is Nothing Then Return Nothing
                'step.2
                'create a new component and initialize it via its designer
                'if the component has not a designer
                'then rollback (return without do nothing)
                'else do the initialization
                Dim newComp As IComponent = host.CreateComponent(controlType)
                If newComp Is Nothing Then Return Nothing
                Dim designer As IDesigner = host.GetDesigner(newComp)
                If designer Is Nothing Then Return Nothing
                If TypeOf designer Is IComponentInitializer Then CType(designer, IComponentInitializer).InitializeNewComponent(Nothing)
                'step.3
                'try to modify the Size/Location of the object just created
                Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(newComp)
                'Sets a PropertyDescriptor to the specific property.
                Dim pdS As PropertyDescriptor = pdc.Find("Size", False)
                If pdS IsNot Nothing Then pdS.SetValue(newComp, controlSize)
                Dim pdL As PropertyDescriptor = pdc.Find("Location", False)
                If pdL IsNot Nothing Then pdL.SetValue(newComp, controlLocation)
                'step.4
                'commit the Creation Operation
                'adding the control to the DesignSurface's root component
                'and return the control just created to let further initializations
                CType(newComp, Control).Parent = TryCast(host.RootComponent, Control)
                Return TryCast(newComp, Control)
            Catch exx As Exception
                Debug.WriteLine(exx.Message)
                If exx.InnerException IsNot Nothing Then Debug.WriteLine(exx.InnerException.Message)
                Throw
            End Try
        End Function

        Public Function GetIDesignerHost() As IDesignerHost Implements IDesignSurfaceExt.GetIDesignerHost
            Return CType(GetService(GetType(IDesignerHost)), IDesignerHost)
        End Function

        Public Function GetView() As Control Implements IDesignSurfaceExt.GetView
            Dim ctrl As Control = TryCast(View, Control)
            If ctrl Is Nothing Then Return Nothing
            ctrl.Dock = DockStyle.Fill
            Return ctrl
        End Function

#End Region

#Region "TabOrder"

        Private _tabOrder As TabOrderHooker = Nothing
        Private _tabOrderMode As Boolean = False

        Public ReadOnly Property IsTabOrderMode As Boolean
            Get
                Return _tabOrderMode
            End Get
        End Property

        Public Property TabOrder As TabOrderHooker
            Get
                If _tabOrder Is Nothing Then _tabOrder = New TabOrderHooker()
                Return _tabOrder
            End Get
            Set(value As TabOrderHooker)
                _tabOrder = value
            End Set
        End Property

        Public Sub InvokeTabOrder()
            TabOrder.HookTabOrder(GetIDesignerHost())
            _tabOrderMode = True
        End Sub

        Public Sub DisposeTabOrder()
            TabOrder.HookTabOrder(GetIDesignerHost())
            _tabOrderMode = False
        End Sub

#End Region

#Region "UndoEngine"

        Private _undoEngine As UndoEngineExt = Nothing
        Private _nameCreationService As NameCreationServiceImp = Nothing
        Private _designerSerializationService As DesignerSerializationServiceImpl = Nothing
        Private _codeDomComponentSerializationService As CodeDomComponentSerializationService = Nothing

#End Region

#Region "IToolboxService"

        Private _toolboxService As ToolboxServiceImp = Nothing

        Public Function GetIToolboxService() As ToolboxServiceImp Implements IDesignSurfaceExt.GetIToolboxService
            Return CType(GetService(GetType(IToolboxService)), ToolboxServiceImp)
        End Function

#Region "drag&Drop"

        Public Sub EnableDragandDrop() Implements IDesignSurfaceExt.EnableDragAndDrop
            ' For the management of the drag and drop of the toolboxItems
            Dim ctrl As Control = GetView()
            If ctrl Is Nothing Then Return
            ctrl.AllowDrop = True
            AddHandler ctrl.DragDrop, AddressOf OnDragDrop
            'enable the Dragitem inside the our Toolbox
            Dim tbs As ToolboxServiceImp = GetIToolboxService()
            If tbs Is Nothing Then Return
            AddHandler tbs.Toolbox.MouseDown, AddressOf OnListboxMouseDown
        End Sub

        'Management of the Drag&Drop of the toolboxItems contained inside our Toolbox
        Private Sub OnListboxMouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
            Dim tbs As ToolboxServiceImp = GetIToolboxService()
            If tbs Is Nothing Then Return
            If tbs.Toolbox Is Nothing Then Return
            If tbs.Toolbox.SelectedItem Is Nothing Then Return
            tbs.Toolbox.DoDragDrop(tbs.Toolbox.SelectedItem, DragDropEffects.Copy Or DragDropEffects.Move)
        End Sub

        'Management of the drag and drop of the toolboxItems
        Public Sub OnDragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
            'if the user don't drag a ToolboxItem
            'then do nothing
            If Not e.Data.GetDataPresent(GetType(ToolboxItem)) Then
                e.Effect = DragDropEffects.None
                Return
            End If
            'now retrieve the data node
            Dim item As ToolboxItem = TryCast(e.Data.GetData(GetType(ToolboxItem)), ToolboxItem)
            e.Effect = DragDropEffects.Copy
            item.CreateComponents(GetIDesignerHost())
        End Sub

#End Region

#End Region

#Region "CodeBehind"

        Public Function GetCodeBehind(lang As DesignLanguage) As String Implements IDesignSurfaceExt.GetCodeBehind
            Dim codeType As CodeTypeDeclaration
            Dim host As IDesignerHost = GetIDesignerHost()
            Dim root As IComponent = host.RootComponent
            Dim mngr As New DesignerSerializationManager(host)
            Using mngr.CreateSession
                Dim serializer As TypeCodeDomSerializer = mngr.GetSerializer(root.GetType, GetType(TypeCodeDomSerializer))
                codeType = serializer.Serialize(mngr, root, host.Container.Components)
                codeType.IsPartial = True
                codeType.Members.OfType(Of CodeConstructor).FirstOrDefault.Attributes = MemberAttributes.Public
            End Using
            Dim builder As New StringBuilder
            Dim options As New CodeGeneratorOptions With {
                .BracingStyle = "C",
                .BlankLinesBetweenMembers = False
            }
            Using writer = New StringWriter(builder, CultureInfo.InvariantCulture)
                Select Case lang
                    Case DesignLanguage.CS
                        Using csProv = New CSharpCodeProvider
                            csProv.GenerateCodeFromType(codeType, writer, options)
                        End Using
                    Case DesignLanguage.VB
                        Using vbProv = New VBCodeProvider
                            vbProv.GenerateCodeFromType(codeType, writer, options)
                        End Using
                    Case Else
                        Throw New Exception("Invalid language")
                End Select
            End Using
            Return builder.ToString
        End Function

        'Public Sub LoadCodeBehind(code As String, lang As DesignLanguage) Implements IDesignSurfaceExt.LoadCodeBehind
        '    Dim host As IDesignerHost = GetIDesignerHost()
        '    Dim root As IComponent = host.RootComponent
        '    Dim mngr As New DesignerSerializationManager(host)
        '    Using mngr.CreateSession
        '        Dim parser As IParser = ParserFactory.CreateParser(If(lang = DesignLanguage.VB, SupportedLanguage.VBNet, SupportedLanguage.CSharp), New StringReader(code))
        '        parser.Parse()
        '        Dim visit As New CodeDomVisitor
        '        visit.VisitCompilationUnit(parser.CompilationUnit, Nothing)
        '        Dim codeType As CodeTypeDeclaration = visit.codeCompileUnit.Namespaces(0).Types(0)
        '        Dim serializer As TypeCodeDomSerializer = mngr.GetSerializer(root.GetType, GetType(TypeCodeDomSerializer))
        '        serializer.Deserialize(mngr, codeType)
        '    End Using
        'End Sub

        'TODO: Support referencing other object to set value
        'TODO: Support call methods to set values
        'TODO: Support more expressions
        'TODO: Support resources
        'TODO: Arrays
        'TODO: Not fully qualified types
        'TODO: Not using Me keyword
        'TODO: More controls
        Public Sub LoadCodeBehind(code As String, lang As DesignLanguage) Implements IDesignSurfaceExt.LoadCodeBehind
            Dim tree As VisualBasicSyntaxTree = VisualBasicSyntaxTree.ParseText(code)
            Dim host As IDesignerHost = GetIDesignerHost()
            Dim root As IComponent = host.RootComponent
            Dim namespaces As New List(Of String)
            For Each import As ImportsStatementSyntax In tree.GetRoot.ChildNodes.OfType(Of ImportsStatementSyntax)

            Next
            Dim cls As ClassBlockSyntax = tree.GetRoot.ChildNodes.OfType(Of ClassBlockSyntax).FirstOrDefault
            If cls Is Nothing Then Throw New Exception("No class to design.")
            If cls.Inherits.Count <> 1 Then Throw New Exception("Cannot design more than one base type.")
            Dim baseType As Type = GetTypeFromString(cls.Inherits.First().Types.First().ToString)
            If Not GetType(Component).IsAssignableFrom(baseType) Then Throw New Exception($"The type {baseType.Name} is not supported.")
            Dim components As New Dictionary(Of String, Component)
            For Each f As FieldDeclarationSyntax In cls.ChildNodes.OfType(Of FieldDeclarationSyntax)
                If f.Declarators.Count <> 1 Then Throw New Exception("Invalid declaration.")
                Dim name As String = f.Declarators.First.Names.First.ToString
                If f.Declarators.First.AsClause Is Nothing Then Throw New Exception("Invalid declaration. Please specify an as clause.")
                If TypeOf f.Declarators.First.AsClause IsNot SimpleAsClauseSyntax Then Throw New Exception("Invalid declaration. Invalid as clause.")
                Dim clause As SimpleAsClauseSyntax = f.Declarators.First.AsClause
                Dim ctrlType As Type = GetTypeFromString(clause.Type.ToString)
                If GetType(Component).IsAssignableFrom(ctrlType) Then
                    components.Add(name, Nothing)
                Else
                    Throw New Exception($"The type {ctrlType.Name} is not supported.")
                End If
            Next
            Dim init As MethodBlockSyntax = cls.ChildNodes.OfType(Of MethodBlockSyntax).FirstOrDefault(Function(x) x.SubOrFunctionStatement.Identifier.ToString.ToLower = "initializecomponent")
            If init Is Nothing Then Throw New Exception("Could not find initialize component method.")
            For Each s As StatementSyntax In init.Statements
                If TypeOf s Is AssignmentStatementSyntax Then
                    Dim ss As AssignmentStatementSyntax = s
                    If ss.OperatorToken.ToString <> "=" Then Throw New Exception("Invalid assignment operator.")
                    Dim mem As MemberAccessExpressionSyntax = ss.Left
                    Dim name As String = mem.Name.Identifier.ToString
                    If TypeOf mem.Expression Is MeExpressionSyntax Then
                        'process the component creation
                        If components.ContainsKey(name) Then
                            If TypeOf ss.Right Is ObjectCreationExpressionSyntax Then
                                Dim oc As ObjectCreationExpressionSyntax = ss.Right
                                If oc.Initializer IsNot Nothing Then Throw New Exception("Invalid assignment, object initializers not supported.")
                                components(name) = host.CreateComponent(GetTypeFromString(oc.Type.ToString))
                            Else
                                Throw New Exception("Invalid assignment, expected object creation.")
                            End If
                        Else
                            'process parent properties
                            SetPropertyValueFromExpression(ss.Right, name, root)
                        End If
                    ElseIf TypeOf mem.Expression Is MemberAccessExpressionSyntax Then
                        'process components properties
                        Dim compName As String = DirectCast(mem.Expression, MemberAccessExpressionSyntax).Name.Identifier.ToString
                        If Not components.ContainsKey(compName) Then Throw New Exception($"Invalid assignment, component '{compName}' not found")
                        Dim obj As Object = components(compName)
                        SetPropertyValueFromExpression(ss.Right, name, obj)
                    End If
                ElseIf TypeOf s Is ExpressionStatementSyntax Then
                    Dim ss As ExpressionStatementSyntax = s
                    If TypeOf ss.Expression Is InvocationExpressionSyntax Then
                        Dim invoke As InvocationExpressionSyntax = ss.Expression
                        If TypeOf invoke.Expression Is MemberAccessExpressionSyntax Then
                            Dim method As MemberAccessExpressionSyntax = invoke.Expression
                            Dim name As String = method.Name.Identifier.ToString
                            If TypeOf method.Expression Is MeExpressionSyntax Then
                                InvokeMethod(name, root, If(invoke.ArgumentList Is Nothing, Array.Empty(Of Object), GetArrayFromArgsList(invoke.ArgumentList)))
                            ElseIf TypeOf method.Expression Is MemberAccessExpressionSyntax Then
                                Dim memAccess As MemberAccessExpressionSyntax = DirectCast(method.Expression, MemberAccessExpressionSyntax)
                                Dim ctrlName As String = memAccess.Name.Identifier.ToString
                                Dim memExp As MemberAccessExpressionSyntax = TryCast(memAccess.Expression, MemberAccessExpressionSyntax)
                                Dim otherName As String = memExp?.Name.Identifier.ToString
                                otherName = If(otherName, "")
                                Dim methodName As String
                                Dim methodParent As Object
                                If components.ContainsKey(ctrlName) Then
                                    methodName = name
                                    methodParent = components(ctrlName)
                                ElseIf components.ContainsKey(otherName) Then
                                    methodName = $"{ctrlName}.{name}"
                                    methodParent = components(otherName)
                                Else
                                    methodName = $"{ctrlName}.{name}"
                                    methodParent = root
                                End If
                                InvokeMethod(methodName, methodParent, If(invoke.ArgumentList Is Nothing, Array.Empty(Of Object), GetArrayFromArgsList(invoke.ArgumentList, components)))
                            End If
                        End If
                    End If
                End If
            Next
        End Sub

        Private Sub InvokeMethod(methodName As String, root As Object, ParamArray args As Object())
            Dim lProperties As String() = methodName.Split(".")
            Dim lastValue As Object = root
            Dim lastType As Type = root.GetType
            For i As Integer = 0 To lProperties.Length - 2
                Dim prop As PropertyInfo = lastType.GetProperty(lProperties(i))
                lastValue = prop.GetValue(lastValue)
                lastType = prop.PropertyType
            Next
            Dim method As MethodInfo = lastType.GetMethod(lProperties.Last, args.Select(Function(x) x.GetType).ToArray)
            method.Invoke(lastValue, args)
        End Sub

        Private Sub SetPropertyValueFromExpression(exp As ExpressionSyntax, name As String, root As Object)
            If TypeOf exp Is ObjectCreationExpressionSyntax Then
                Dim oc As ObjectCreationExpressionSyntax = exp
                If oc.Initializer IsNot Nothing Then Throw New Exception("Invalid assignment, object initializers not supported.")
                SetPropertyValue(GetValueFromObjectCreation(oc), name, root)
            ElseIf TypeOf exp Is LiteralExpressionSyntax Then
                SetPropertyValue(DirectCast(exp, LiteralExpressionSyntax).Token.Value, name, root)
            Else
                Throw New Exception("Invalid assignment, expected object creation.")
            End If
        End Sub

        Private Sub SetPropertyValue(value As Object, propName As String, root As Object)
            Dim lProperties As String() = propName.Split(".")
            Dim lastValue As Object = root
            Dim lastType As Type = root.GetType
            For i As Integer = 0 To lProperties.Length - 1
                Dim prop As PropertyInfo = lastType.GetProperty(lProperties(i))
                If lProperties.Length = i + 1 Then
                    prop.SetValue(lastValue, value)
                Else
                    lastValue = prop.GetValue(lastValue)
                    lastType = prop.PropertyType
                End If
            Next
        End Sub

        Private Function GetValueFromObjectCreation(init As ObjectCreationExpressionSyntax, Optional components As Dictionary(Of String, Component) = Nothing) As Object
            Dim vType As Type = GetTypeFromString(init.Type.ToString)
            Dim args As ArgumentListSyntax = init.ArgumentList
            Return If(args Is Nothing, Activator.CreateInstance(vType), Activator.CreateInstance(vType, GetArrayFromArgsList(args, components)))
        End Function

        Private Function GetArrayFromArgsList(args As ArgumentListSyntax, Optional components As Dictionary(Of String, Component) = Nothing) As Object()
            Dim vArgs As New List(Of Object)
            For Each arg As SimpleArgumentSyntax In args.ChildNodes
                Dim exp As ExpressionSyntax = arg.Expression
                If TypeOf exp Is LiteralExpressionSyntax Then
                    vArgs.Add(DirectCast(exp, LiteralExpressionSyntax).Token.Value)
                ElseIf TypeOf exp Is ObjectCreationExpressionSyntax Then
                    vArgs.Add(GetValueFromObjectCreation(exp, components))
                ElseIf TypeOf exp Is MemberAccessExpressionSyntax Then
                    Dim name As String = DirectCast(exp, MemberAccessExpressionSyntax).Name.Identifier.ToString
                    If components?.ContainsKey(name) Then
                        vArgs.Add(components(name))
                    Else
                        Throw New Exception($"Invalid reference, could find variable '{name}'.")
                    End If
                End If
            Next
            Return vArgs.ToArray
        End Function

        Public Function GetTypeFromString(name As String) As Type
            For Each assembly In AppDomain.CurrentDomain.GetAssemblies().Reverse()
                Dim tt As Type = assembly.[GetType](name)
                If tt IsNot Nothing Then
                    Return tt
                End If
            Next
            Return Nothing
        End Function

#End Region

#Region "ctors"

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

        'The DesignSurface class provides several design-time services automatically.
        'The DesignSurface class adds all of its services in its constructor.
        'Most of these services can be overridden by replacing them in the
        'protected ServiceContainer property.To replace a service, override the constructor,
        'call base, and make any changes through the protected ServiceContainer property.
        Private Sub InitServices()
            'each DesignSurface has its own default services
            'We can leave the default services in their present state,
            'or we can remove them and replace them with our own.
            'Now add our own services using IServiceContainer
            'Note
            'before loading the root control in the design surface
            'we must add an instance of naming service to the service container.
            'otherwise the root component did not have a name and this caused
            'troubles when we try to use the UndoEngine
            'NameCreationService
            _nameCreationService = New NameCreationServiceImp()

            If _nameCreationService IsNot Nothing Then
                ServiceContainer.RemoveService(GetType(INameCreationService), False)
                ServiceContainer.AddService(GetType(INameCreationService), _nameCreationService)
            End If
            'CodeDomComponentSerializationService
            _codeDomComponentSerializationService = New CodeDomComponentSerializationService(ServiceContainer)

            If _codeDomComponentSerializationService IsNot Nothing Then
                'the CodeDomComponentSerializationService is ready to be replaced
                ServiceContainer.RemoveService(GetType(ComponentSerializationService), False)
                ServiceContainer.AddService(GetType(ComponentSerializationService), _codeDomComponentSerializationService)
            End If
            '3. IDesignerSerializationService
            _designerSerializationService = New DesignerSerializationServiceImpl(ServiceContainer)

            If _designerSerializationService IsNot Nothing Then
                '- the IDesignerSerializationService is ready to be replaced
                ServiceContainer.RemoveService(GetType(IDesignerSerializationService), False)
                ServiceContainer.AddService(GetType(IDesignerSerializationService), _designerSerializationService)
            End If
            '4. UndoEngine
            _undoEngine = New UndoEngineExt(ServiceContainer)
            'disable the UndoEngine
            _undoEngine.Enabled = False

            If _undoEngine IsNot Nothing Then
                'the UndoEngine is ready to be replaced
                ServiceContainer.RemoveService(GetType(UndoEngine), False)
                ServiceContainer.AddService(GetType(UndoEngine), _undoEngine)
            End If
            '5. IMenuCommandService
            _menuCommandService = New MenuCommandServiceExt(Me)

            If _menuCommandService IsNot Nothing Then
                'remove the old Service, i.e. the DesignsurfaceExt service
                ServiceContainer.RemoveService(GetType(IMenuCommandService), False)
                'add the new IMenuCommandService
                ServiceContainer.AddService(GetType(IMenuCommandService), _menuCommandService)
            End If
            '6. IToolboxService
            _toolboxService = New ToolboxServiceImp(GetIDesignerHost())

            If _toolboxService IsNot Nothing Then
                ServiceContainer.RemoveService(GetType(IToolboxService), False)
                ServiceContainer.AddService(GetType(IToolboxService), _toolboxService)
            End If
        End Sub

#End Region

        'do some Edit menu command using the MenuCommandServiceImp
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
                    Case Else
                End Select
            Catch exx As Exception
                Debug.WriteLine(exx.Message)
                If exx.InnerException IsNot Nothing Then Debug.WriteLine(exx.InnerException.Message)
                Throw
            End Try
        End Sub

    End Class

    Public Enum DesignLanguage
        CS = 0
        VB = 1
        XML = 2
    End Enum

    Public Interface IDesignSurfaceExt

        'perform Cut/Copy/Paste/Delete commands
        Sub DoAction(command As String)

        'de/activate the TabOrder facility
        Sub SwitchTabOrder()

        'select the controls alignement mode
        Sub UseSnapLines()

        Sub UseGrid(gridSize As Size)

        Sub UseGridWithoutSnapping(gridSize As Size)

        Sub UseNoGuides()

        'method usefull to create control without the ToolBox facility
        Function CreateRootComponent(controlType As Type, controlSize As Size) As IComponent

        Function CreateRootComponent(loader As DesignerLoader, controlSize As Size) As IComponent

        Function CreateControl(controlType As Type, controlSize As Size, controlLocation As Point) As Control

        'Get the UndoEngineExtended object
        Function GetUndoEngineExt() As UndoEngineExt

        'Get the IDesignerHost of the DesignSurface
        Function GetIDesignerHost() As IDesignerHost

        'the HostControl of the DesignSurface is just a normal Control
        ' Get the HostControl
        Function GetView() As Control

        Function GetIToolboxService() As ToolboxServiceImp

        Sub EnableDragAndDrop()

        Function GetCodeBehind(lang As DesignLanguage) As String

        Sub LoadCodeBehind(code As String, lang As DesignLanguage)

    End Interface

End Namespace