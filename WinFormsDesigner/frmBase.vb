Imports System.ComponentModel.Design
Imports System.Drawing.Design
Imports CodingCool.DeveloperCore.WinForms.Designer.Base
Imports CodingCool.DeveloperCore.WinForms.Designer.Core

'TODO: Cleanup code across the project
Public Class frmBase
    Private _listOfDesignSurface As New List(Of IDesignSurfaceExt)
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Add the toolboxItems to the future toolbox
        'the pointer
        Dim toolPointer As ToolboxItem = New ToolboxItem()
        toolPointer.DisplayName = "<Pointer>"
        toolPointer.Bitmap = New Bitmap(16, 16)
        listBox1.Items.Add(toolPointer)
        'the control
        listBox1.Items.Add(New ToolboxItem(GetType(Button)))
        listBox1.Items.Add(New ToolboxItem(GetType(ListView)))
        listBox1.Items.Add(New ToolboxItem(GetType(TreeView)))
        listBox1.Items.Add(New ToolboxItem(GetType(TextBox)))
        listBox1.Items.Add(New ToolboxItem(GetType(Label)))
        listBox1.Items.Add(New ToolboxItem(GetType(TabControl)))
        listBox1.Items.Add(New ToolboxItem(GetType(OpenFileDialog)))
        listBox1.Items.Add(New ToolboxItem(GetType(CheckBox)))
        listBox1.Items.Add(New ToolboxItem(GetType(ComboBox)))
        listBox1.Items.Add(New ToolboxItem(GetType(GroupBox)))
        listBox1.Items.Add(New ToolboxItem(GetType(ImageList)))
        listBox1.Items.Add(New ToolboxItem(GetType(Panel)))
        listBox1.Items.Add(New ToolboxItem(GetType(ProgressBar)))
        listBox1.Items.Add(New ToolboxItem(GetType(ToolStrip)))
        listBox1.Items.Add(New ToolboxItem(GetType(ToolTip)))
        AddHandler tabControl1.Selected, AddressOf OnTabPageSelected
    End Sub

    Private Sub OnTabPageSelected(ByVal sender As Object, ByVal e As TabControlEventArgs)
        'select into the propertygrid the current Form
        SelectRootComponent()
    End Sub

    Private Function GetCurrentIDesignSurface() As IDesignSurfaceExt
        If _listOfDesignSurface Is Nothing Then Return Nothing
        If _listOfDesignSurface.Count = 0 Then Return Nothing
        Return _listOfDesignSurface(tabControl1.SelectedIndex)
    End Function

    Private Sub OnSelectionChanged(sender As Object, e As System.EventArgs)
        Dim isurf As IDesignSurfaceExt = GetCurrentIDesignSurface()
        If isurf IsNot Nothing Then
            Dim SelectionService As ISelectionService = isurf.GetIDesignerHost().GetService(GetType(ISelectionService))
            propertyGrid.SelectedObject = SelectionService.PrimarySelection
        End If
    End Sub

    Private Sub SelectRootComponent()
        'find out the DesignSurfaceExt control hosted by the TabPage
        Dim isurf As IDesignSurfaceExt = GetCurrentIDesignSurface()
        If isurf IsNot Nothing Then propertyGrid.SelectedObject = isurf.GetIDesignerHost().RootComponent
    End Sub

    Private Sub undoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemUnDo.Click
        Dim isurf As IDesignSurfaceExt = GetCurrentIDesignSurface()
        If isurf IsNot Nothing Then isurf.GetUndoEngineExt().Undo()
    End Sub

    Private Sub redoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemReDo.Click
        Dim isurf As IDesignSurfaceExt = GetCurrentIDesignSurface()
        If isurf IsNot Nothing Then isurf.GetUndoEngineExt().Redo()
    End Sub

    Private Sub toolStripMenuItemTabOrder_Click(sender As Object, e As EventArgs) Handles toolStripMenuItemTabOrder.Click
        Dim isurf As IDesignSurfaceExt = GetCurrentIDesignSurface()
        If isurf IsNot Nothing Then isurf.SwitchTabOrder()
    End Sub

    Private Sub OnMenuClick(sender As Object, e As EventArgs) Handles ToolStripMenuItemCopy.Click, ToolStripMenuItemCut.Click, ToolStripMenuItemPaste.Click, ToolStripMenuItemDelete.Click
        Dim isurf As IDesignSurfaceExt = GetCurrentIDesignSurface()
        If isurf IsNot Nothing Then isurf.DoAction(sender.Text)
    End Sub

    Private Sub newFormUseSnapLinesMenuItem_Click(sender As Object, e As EventArgs) Handles newFormUseSnapLinesMenuItem.Click
        Dim tp As TabPage = New TabPage("Use SnapLines")
        tabControl1.TabPages.Add(tp)
        Dim iTabPageSelectedIndex As Integer = tabControl1.SelectedIndex
        'steps.1.2.3
        Dim surface As DesignSurfaceExt = CreateDesignSurface()
        surface.UseSnapLines()
        'step.5
        'create the Root compoment, in these cases a Form
        Try
            Dim rootComponent As Form = Nothing
            rootComponent = CType(surface.CreateRootComponent(GetType(Form), New Size(400, 400)), Form)
            rootComponent.Text = $"Root Component hosted by the DesignSurface N.{iTabPageSelectedIndex}"
            rootComponent.BackColor = Color.Yellow
            'step.4
            'display the DesignSurface
            Dim view As Control = surface.GetView()
            If view Is Nothing Then Return
            'change some properties
            view.Text = $"Test Form N. {iTabPageSelectedIndex}"
            view.Dock = DockStyle.Fill
            'Note these assignments
            view.Parent = tp
            surface.EnableDragandDrop()
        Catch ex As Exception
            Console.WriteLine($"{Name} the DesignSurface N. {iTabPageSelectedIndex} has generated errors during loading!Exception: {ex.Message}")
            Return
        End Try
    End Sub

    Private Sub newFormUseGridandSnapMenuItem_Click(sender As Object, e As EventArgs) Handles newFormUseGridandSnapMenuItem.Click
        Dim tp As TabPage = New TabPage("Use Grid (Snap to the grid)")
        tabControl1.TabPages.Add(tp)
        Dim iTabPageSelectedIndex As Integer = tabControl1.SelectedIndex
        'steps.1.2.3
        Dim surface As DesignSurfaceExt = CreateDesignSurface()
        surface.UseGrid(New System.Drawing.Size(32, 32))
        'step.5
        'create the Root compoment, in these cases a Form
        Try
            Dim rootComponent As Form = Nothing
            rootComponent = CType(surface.CreateRootComponent(GetType(Form), New Size(400, 400)), Form)
            rootComponent.Text = $"Root Component hosted by the DesignSurface N.{iTabPageSelectedIndex}"
            rootComponent.BackColor = Color.YellowGreen
            'step.4
            'display the DesignSurface
            Dim view As Control = surface.GetView()
            If view Is Nothing Then Return
            'change some properties
            view.Text = $"Test Form N. {iTabPageSelectedIndex}"
            view.Dock = DockStyle.Fill
            'Note these assignments
            view.Parent = tp
        Catch ex As Exception
            Console.WriteLine($"{Name} the DesignSurface N. {iTabPageSelectedIndex} has generated errors during loading!Exception: {ex.Message}")
            Return
        End Try
    End Sub

    Private Sub newFormAlignControlByhandMenuItem_Click(sender As Object, e As EventArgs) Handles newFormAlignControlByhandMenuItem.Click
        Dim tp As TabPage = New TabPage("Align control by hand")
        tabControl1.TabPages.Add(tp)
        Dim iTabPageSelectedIndex As Integer = tabControl1.SelectedIndex
        'steps.1.2.3
        Dim surface As DesignSurfaceExt = CreateDesignSurface()
        surface.UseNoGuides()
        'step.5
        'create the Root compoment, in these cases a Form
        Try
            Dim rootComponent As Form = Nothing
            rootComponent = CType(surface.CreateRootComponent(GetType(Form), New Size(400, 400)), Form)
            rootComponent.Text = $"Root Component hosted by the DesignSurface N.{iTabPageSelectedIndex}"
            rootComponent.BackColor = Color.LightGray
            'step.4
            'display the DesignSurface
            Dim view As Control = surface.GetView()
            If view Is Nothing Then Return
            'change some properties
            view.Text = $"Test Form N. {iTabPageSelectedIndex}"
            view.Dock = DockStyle.Fill
            'Note these assignments
            view.Parent = tp
        Catch ex As Exception
            Console.WriteLine($"{Name} the DesignSurface N. {iTabPageSelectedIndex} has generated errors during loading!Exception: {ex.Message}")
            Return
        End Try
    End Sub

    Private Function CreateDesignSurface() As DesignSurfaceExt
        'step.0
        'create a DesignSurface and put it inside a Form in DesignTime
        Dim surface As DesignSurfaceExt = New DesignSurfaceExt()
        Dim isurf As IDesignSurfaceExt = CType(surface, IDesignSurfaceExt)
        _listOfDesignSurface.Add(isurf)
        'step.1
        'enable the UndoEngines
        isurf.GetUndoEngineExt().Enabled = True
        'step.2
        'try to get a ptr to ISelectionService interface
        'if we obtain it then hook the SelectionChanged event
        Dim selectionService As ISelectionService = CType((isurf.GetIDesignerHost().GetService(GetType(ISelectionService))), ISelectionService)
        If selectionService IsNot Nothing Then AddHandler selectionService.SelectionChanged, AddressOf OnSelectionChanged
        'step.3
        'Select the service IToolboxService
        'and hook it to our ListBox
        Dim tbox As ToolboxServiceImp = isurf.GetIToolboxService()
        If tbox IsNot Nothing Then tbox.Toolbox = listBox1
        'finally return the Designsurface
        Return surface
    End Function

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        Dim isurf As IDesignSurfaceExt = GetCurrentIDesignSurface()
        If isurf IsNot Nothing Then
            IO.File.WriteAllText("C:\CodingCool\Code\Projects\test.vb", isurf.GetCodeBehind(DesignLanguage.VB))
        End If
    End Sub

    Private Sub LoadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadToolStripMenuItem.Click
        Dim isurf As IDesignSurfaceExt = GetCurrentIDesignSurface()
        isurf.LoadCodeBehind(IO.File.ReadAllText("C:\CodingCool\Code\Projects\test.vb"), DesignLanguage.VB)
    End Sub

End Class