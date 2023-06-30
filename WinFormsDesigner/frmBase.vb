Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing.Design
Imports System.IO
Imports CodingCool.DeveloperCore.WinForms.Designer.Base
Imports CodingCool.DeveloperCore.WinForms.Designer.Core
Imports CodingCool.DeveloperCore.WinForms.Designer.Load

Public Class frmBase
    Private ReadOnly _designers As New List(Of IDesignSurfaceExt)
    
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim toolPointer As New ToolboxItem With {
            .DisplayName = "<Pointer>",
            .Bitmap = New Bitmap(16, 16)
        }
        lstToolbox.Items.Add(toolPointer)
        AddHandler tcDesigners.Selected, AddressOf OnTabPageSelected
    End Sub

    Private Sub OnTabPageSelected(sender As Object, e As TabControlEventArgs)
        SelectRootComponent()
    End Sub

    Private Function GetCurrentDesigner() As IDesignSurfaceExt
        If _designers Is Nothing Then Return Nothing
        If _designers.Count = 0 Then Return Nothing
        Return _designers(tcDesigners.SelectedIndex)
    End Function

    Private Sub OnSelectionChanged(sender As Object, e As EventArgs)
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        If designer IsNot Nothing Then
            Dim selectionService As ISelectionService = designer.GetDesignerHost().GetService(GetType(ISelectionService))
            pgProperties.SelectedObject = selectionService.PrimarySelection
        End If
    End Sub

    Private Sub SelectRootComponent()
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        If designer IsNot Nothing Then pgProperties.SelectedObject = designer.GetDesignerHost().RootComponent
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
        tcDesigners.TabPages.Add(tp)
        Dim tabPageSelectedIndex As Integer = tcDesigners.SelectedIndex
        Dim designer As DesignSurfaceExt = CreateDesignSurface()
        designer.UseSnapLines()
        Try
            Dim rootComponent As UserControl
            rootComponent = CType(designer.CreateRootComponent(GetLoader(), New Size(400, 400)), UserControl)
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
        tcDesigners.TabPages.Add(tp)
        Dim tabPageSelectedIndex As Integer = tcDesigners.SelectedIndex
        Dim designer As DesignSurfaceExt = CreateDesignSurface()
        designer.UseGrid(New Size(32, 32))
        Try
            Dim rootComponent As UserControl
            rootComponent = CType(designer.CreateRootComponent(GetLoader(), New Size(400, 400)), UserControl)
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
        tcDesigners.TabPages.Add(tp)
        Dim tabPageSelectedIndex As Integer = tcDesigners.SelectedIndex
        Dim designer As DesignSurfaceExt = CreateDesignSurface()
        designer.UseNoGuides()
        Try
            Dim rootComponent As UserControl
            rootComponent = CType(designer.CreateRootComponent(GetLoader, New Size(400, 400)), UserControl)
            rootComponent.Text = $"Root Component hosted by the DesignSurface N.{tabPageSelectedIndex}"
            rootComponent.BackColor = Color.LightGray
            Dim view As Control = designer.GetView()
            If view Is Nothing Then Return
            view.Text = $"Test Form N. {tabPageSelectedIndex}"
            view.Dock = DockStyle.Fill
            view.Parent = tp
            view.Focus()
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
        If tbox IsNot Nothing Then
            tbox.Toolbox = lstToolbox
            tbox.AddItems(GetType(Form).Assembly)
        End If
        Return designer
    End Function

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        Dim designer As IDesignSurfaceExt = GetCurrentDesigner()
        Dim loader As XmlDesignerLoader = designer.GetLoader()
        Try
            Dim fileName As String
            Dim filterIndex As Integer = 3
                Dim dlg As SaveFileDialog = New SaveFileDialog()
                dlg.DefaultExt = "xml"
                dlg.Filter = "XML Files|*.xml"
                If dlg.ShowDialog() = DialogResult.OK Then
                    fileName = dlg.FileName
                    filterIndex = dlg.FilterIndex
                End If
            If fileName IsNot Nothing Then
                Select Case filterIndex
                    Case 1
                        Dim file As StreamWriter = New StreamWriter(fileName)
                        file.Write(loader.GetCode())
                        file.Close()
                End Select
            End If
        Catch ex As Exception
            MessageBox.Show("Error during save: " & ex.ToString())
        End Try
    End Sub
    
    Private Shared Function GetLoader() As BasicDesignerLoader
        Dim ofd As New OpenFileDialog With {
            .Filter = "XML Files|*.xml"
        }
        Return If(ofd.ShowDialog() = DialogResult.OK, New XmlDesignerLoader(File.ReadAllText(ofd.FileName)), New XmlDesignerLoader(GetType(UserControl)))
    End Function

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        If tcDesigners.TabCount > 0 Then
            _designers.RemoveAt(tcDesigners.SelectedIndex)
            tcDesigners.TabPages.RemoveAt(tcDesigners.SelectedIndex)
        End If
    End Sub
End Class