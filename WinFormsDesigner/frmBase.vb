Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing.Design
Imports System.IO
Imports System.Xml
Imports CodingCool.DeveloperCore.WinForms.Designer.Base
Imports CodingCool.DeveloperCore.WinForms.Designer.Core
Imports CodingCool.DeveloperCore.WinForms.Designer.Load
Imports Microsoft.VisualBasic.ApplicationServices

Public Class frmBase
    Private ReadOnly _designers As New List(Of IDesignSurfaceExt)
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim toolPointer As ToolboxItem = New ToolboxItem()
        toolPointer.DisplayName = "<Pointer>"
        toolPointer.Bitmap = New Bitmap(16, 16)
        listBox1.Items.Add(toolPointer)
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

    Private Sub OnTabPageSelected(sender As Object, e As TabControlEventArgs)
        SelectRootComponent()
    End Sub

    Private Function GetCurrentDesigner() As IDesignSurfaceExt
        If _designers Is Nothing Then Return Nothing
        If _designers.Count = 0 Then Return Nothing
        Return _designers(tabControl1.SelectedIndex)
    End Function

    Private Sub OnSelectionChanged(sender As Object, e As EventArgs)
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        If designer IsNot Nothing Then
            Dim selectionService As ISelectionService = designer.GetDesignerHost().GetService(GetType(ISelectionService))
            propertyGrid.SelectedObject = selectionService.PrimarySelection
        End If
    End Sub

    Private Sub SelectRootComponent()
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        If designer IsNot Nothing Then propertyGrid.SelectedObject = designer.GetDesignerHost().RootComponent
    End Sub

    Private Sub undoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemUnDo.Click
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        If designer IsNot Nothing Then designer.GetUndoEngine().Undo()
    End Sub

    Private Sub redoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemReDo.Click
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        If designer IsNot Nothing Then designer.GetUndoEngine().Redo()
    End Sub

    Private Sub toolStripMenuItemTabOrder_Click(sender As Object, e As EventArgs) Handles toolStripMenuItemTabOrder.Click
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        If designer IsNot Nothing Then designer.SwitchTabOrder()
    End Sub

    Private Sub OnMenuClick(sender As Object, e As EventArgs) Handles ToolStripMenuItemCopy.Click, ToolStripMenuItemCut.Click, ToolStripMenuItemPaste.Click, ToolStripMenuItemDelete.Click
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        If designer IsNot Nothing Then designer.DoAction(sender.Text)
    End Sub

    Private Sub newFormUseSnapLinesMenuItem_Click(sender As Object, e As EventArgs) Handles newFormUseSnapLinesMenuItem.Click
        Dim tp As New TabPage("Use SnapLines")
        tabControl1.TabPages.Add(tp)
        Dim tabPageSelectedIndex As Integer = tabControl1.SelectedIndex
        Dim designer As DesignSurfaceExt = CreateDesignSurface()
        designer.UseSnapLines()
        Try
            Dim rootComponent As UserControl
            rootComponent = CType(designer.CreateRootComponent(New XmlDesignerLoader("C:\CodingCool\Code\Projects\test.xml"), New Size(400, 400)), UserControl)
            rootComponent.Text = $"Root Component hosted by the DesignSurface N.{tabPageSelectedIndex}"
            rootComponent.BackColor = Color.Yellow
            Dim view As Control = designer.GetView()
            If view Is Nothing Then Return
            view.Text = $"Test Form N. {tabPageSelectedIndex}"
            view.Dock = DockStyle.Fill
            view.Parent = tp
            designer.EnableDragAndDrop()
        Catch ex As Exception
            Console.WriteLine($"{Name} the DesignSurface N. {tabPageSelectedIndex} has generated errors during loading!Exception: {ex.Message}")
            Return
        End Try
    End Sub

    Private Sub newFormUseGridandSnapMenuItem_Click(sender As Object, e As EventArgs) Handles newFormUseGridandSnapMenuItem.Click
        Dim tp As New TabPage("Use Grid (Snap to the grid)")
        tabControl1.TabPages.Add(tp)
        Dim tabPageSelectedIndex As Integer = tabControl1.SelectedIndex
        Dim designer As DesignSurfaceExt = CreateDesignSurface()
        designer.UseGrid(New Size(32, 32))
        Try
            Dim rootComponent As UserControl
            rootComponent = CType(designer.CreateRootComponent(GetType(UserControl), New Size(400, 400)), UserControl)
            rootComponent.Text = $"Root Component hosted by the DesignSurface N.{tabPageSelectedIndex}"
            rootComponent.BackColor = Color.YellowGreen
            Dim view As Control = designer.GetView()
            If view Is Nothing Then Return
            view.Text = $"Test Form N. {tabPageSelectedIndex}"
            view.Dock = DockStyle.Fill
            view.Parent = tp
        Catch ex As Exception
            Console.WriteLine($"{Name} the DesignSurface N. {tabPageSelectedIndex} has generated errors during loading!Exception: {ex.Message}")
            Return
        End Try
    End Sub

    Private Sub newFormAlignControlByhandMenuItem_Click(sender As Object, e As EventArgs) Handles newFormAlignControlByhandMenuItem.Click
        Dim tp As New TabPage("Align control by hand")
        tabControl1.TabPages.Add(tp)
        Dim tabPageSelectedIndex As Integer = tabControl1.SelectedIndex
        Dim designer As DesignSurfaceExt = CreateDesignSurface()
        designer.UseNoGuides()
        Try
            Dim rootComponent As UserControl
            rootComponent = CType(designer.CreateRootComponent(GetType(UserControl), New Size(400, 400)), UserControl)
            rootComponent.Text = $"Root Component hosted by the DesignSurface N.{tabPageSelectedIndex}"
            rootComponent.BackColor = Color.LightGray
            Dim view As Control = designer.GetView()
            If view Is Nothing Then Return
            view.Text = $"Test Form N. {tabPageSelectedIndex}"
            view.Dock = DockStyle.Fill
            view.Parent = tp
        Catch ex As Exception
            Console.WriteLine($"{Name} the DesignSurface N. {tabPageSelectedIndex} has generated errors during loading!Exception: {ex.Message}")
            Return
        End Try
    End Sub

    Private Function CreateDesignSurface() As DesignSurfaceExt
        Dim designer As New DesignSurfaceExt()
        _designers.Add(designer)
        designer.GetUndoEngine().Enabled = True
        Dim selectionService As ISelectionService = CType((designer.GetDesignerHost().GetService(GetType(ISelectionService))), ISelectionService)
        If selectionService IsNot Nothing Then AddHandler selectionService.SelectionChanged, AddressOf OnSelectionChanged
        Dim tbox As ToolboxService = designer.GetToolboxService()
        If tbox IsNot Nothing Then tbox.Toolbox = listBox1
        Return designer
    End Function

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        Dim loader As XmlDesignerLoader = designer.GetLoader()
        loader.Flush()
        Dim sw As StringWriter = New StringWriter()
        Dim xtw As XmlTextWriter = New XmlTextWriter(sw)
        xtw.Formatting = Formatting.Indented
        loader.XmlDocument.WriteTo(xtw)
        Dim cleanup As String = sw.ToString().Replace("<DOCUMENT_ELEMENT>", "")
        cleanup = cleanup.Replace("</DOCUMENT_ELEMENT>", "")
        xtw.Close()
        Dim file As StreamWriter = New StreamWriter("C:\CodingCool\Code\Projects\test.xml")
        file.Write(cleanup)
        file.Close()
        'IO.File.WriteAllText("C:\CodingCool\Code\Projects\test.vb", designer.GetCodeBehind(DesignLanguage.VB))
    End Sub

    Private Sub LoadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadToolStripMenuItem.Click
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        
        'designer.LoadCodeBehind(IO.File.ReadAllText("C:\CodingCool\Code\Projects\test.vb"), DesignLanguage.VB)
    End Sub

End Class