Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports CodingCool.DeveloperCore.WinForms.Designer.Base
Imports CodingCool.DeveloperCore.WinForms.Designer.Core
Imports CodingCool.DeveloperCore.WinForms.Designer.Load

Public Class frmDesigner
    ReadOnly _client As New PipeClient
    Dim designer As DesignSurfaceExt

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub MessageReceived(message As Byte())
        Dim s As String = Encoding.UTF8.GetString(message)
        If s = "Disconnect" Then
            Application.Exit()
        End If
    End Sub

    Private Sub frmDesigner_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _client.Connect(Environment.GetCommandLineArgs()(1))
        AddHandler _client.MessageReceived, AddressOf MessageReceived
        designer = CreateDesignSurface()
        designer.UseSnapLines()
        Try
            Dim rootComponent As UserControl
            rootComponent = CType(designer.CreateRootComponent(GetLoader, New Size(400, 400)), UserControl)
            rootComponent.BackColor = Color.Yellow
            Dim view = designer.GetView
            If view Is Nothing Then Return
            view.Dock = DockStyle.Fill
            pnlDesigner.Controls.Add(view)
            designer.EnableDragAndDrop()
        Catch ex As Exception
            Return
        End Try
    End Sub

    Private Sub frmDesigner_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        _client.SendMessage($"Connect:{pnlMain.Handle}")
    End Sub

    Private Function CreateDesignSurface() As DesignSurfaceExt
        Dim designer As New DesignSurfaceExt()
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

    Private Sub OnSelectionChanged(sender As Object, e As EventArgs)
        If designer IsNot Nothing Then
            Dim selectionService As ISelectionService = designer.GetDesignerHost().GetService(GetType(ISelectionService))
            pgDesigner.SelectedObject = selectionService.PrimarySelection
        End If
    End Sub

    Private Shared Function GetLoader() As BasicDesignerLoader
        Dim ofd As New OpenFileDialog With {
                .Filter = "XML Files|*.xml"
                }
        Return If(ofd.ShowDialog() = DialogResult.OK, New XmlDesignerLoader(File.ReadAllText(ofd.FileName)), New XmlDesignerLoader(GetType(UserControl)))
    End Function
End Class