Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports CodingCool.DeveloperCore.WinForms.Designer.Core
Imports Microsoft.CSharp

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

        Public Function GetUndoEngine() As CustomUndoEngine Implements IDesignSurfaceExt.GetUndoEngine
            Return _customUndoEngine
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
                If host.RootComponent.GetType().IsSubclassOf(GetType(Form)) Then
                    ctrl = TryCast(View, Control)
                    ctrl.BackColor = Color.LightGray
                    Dim pdc As PropertyDescriptorCollection = TypeDescriptor.GetProperties(ctrl)
                    Dim pdS As PropertyDescriptor = pdc.Find("Size", False)
                    If pdS IsNot Nothing Then pdS.SetValue(host.RootComponent, controlSize)
                ElseIf host.RootComponent.GetType().IsSubclassOf(GetType(UserControl)) Then
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

        Private _customUndoEngine As CustomUndoEngine
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
            AddHandler tbs.Toolbox.MouseMove, AddressOf OnListBoxMouseMove
        End Sub

        Dim _dragPoint As Point = Point.Empty
        
        Private Sub OnListBoxMouseDown(sender As Object, e As MouseEventArgs)
            Dim tbs As ToolboxService = GetToolboxService()
            If tbs Is Nothing Then Return
            If tbs.Toolbox Is Nothing Then Return
            If tbs.Toolbox.SelectedItem Is Nothing Then Return
            _dragPoint = New Point(e.X, e.Y)
        End Sub
        
        Private Sub OnListBoxMouseMove(sender As Object, e As MouseEventArgs)
            Dim tbs As ToolboxService = GetToolboxService()
            If tbs Is Nothing Then Return
            If tbs.Toolbox Is Nothing Then Return
            If tbs.Toolbox.SelectedItem Is Nothing Then Return
            If e.Button = MouseButtons.Left Then
                If _dragPoint <> Point.Empty AndAlso (Math.Abs(e.X - _dragPoint.X) > SystemInformation.DragSize.Width OrElse Math.Abs(e.Y - _dragPoint.Y) > SystemInformation.DragSize.Height) Then 
                    tbs.Toolbox.DoDragDrop(tbs.Toolbox.SelectedItem, DragDropEffects.Copy Or DragDropEffects.Move)
                End If
            End If
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
            _customUndoEngine = New CustomUndoEngine(ServiceContainer) With {
                .Enabled = False
                }
            If _customUndoEngine IsNot Nothing Then
                ServiceContainer.RemoveService(GetType(UndoEngine), False)
                ServiceContainer.AddService(GetType(UndoEngine), _customUndoEngine)
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
        
        Public Sub Rename(component As IComponent, name As String) Implements IDesignSurfaceExt.Rename
            Dim objDescriptor As PropertyDescriptorCollection = TypeDescriptor.GetProperties(component)
            Dim objName As PropertyDescriptor = objDescriptor.Find("Name", False)
            objName.SetValue(component, name)
        End Sub
        
    End Class
End Namespace