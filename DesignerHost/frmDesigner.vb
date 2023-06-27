Imports System.ComponentModel.Design.Serialization
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports CodingCool.DeveloperCore.WinForms.Designer.Base
Imports CodingCool.DeveloperCore.WinForms.Designer.Load

Public Class frmDesigner
    Dim ReadOnly _client As New PipeClient

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub MessageReceived(message As Byte())
        Dim s As String = Encoding.UTF8.GetString(message)
        If s = "Disconnect" Then
            Application.Exit()
        End If
    End Sub

    Private Sub frmDesigner_Load(sender As Object, e As EventArgs) Handles Me.Load
        _client.Connect(Environment.GetCommandLineArgs()(1))
        AddHandler _client.MessageReceived, AddressOf MessageReceived
    End Sub

    Private Sub frmDesigner_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Hide()
        Dim designer As DesignSurfaceExt = CreateDesignSurface()
        designer.UseSnapLines()
        Try
            Dim rootComponent As UserControl
            rootComponent = CType(designer.CreateRootComponent(GetLoader(), New Size(400, 400)), UserControl)
            rootComponent.BackColor = Color.Yellow
            Dim view As Control = designer.GetView()
            If view Is Nothing Then Return
            'designer.EnableDragAndDrop()
        Catch ex As Exception
            Return
        End Try
        _client.SendMessage($"Connect:{designer.GetView().Handle}")
    End Sub
    
    Private Function CreateDesignSurface() As DesignSurfaceExt
        Dim designer As New DesignSurfaceExt()
        designer.GetUndoEngine().Enabled = True
'        Dim selectionService As ISelectionService = CType((designer.GetDesignerHost().GetService(GetType(ISelectionService))), ISelectionService)
'        If selectionService IsNot Nothing Then AddHandler selectionService.SelectionChanged, AddressOf OnSelectionChanged
'        Dim tbox As ToolboxService = designer.GetToolboxService()
'        If tbox IsNot Nothing Then
'            tbox.Toolbox = lstToolbox
'            tbox.AddItems(GetType(Form).Assembly)
'        End If
        Return designer
    End Function
    
    Private Shared Function GetLoader() As BasicDesignerLoader
        Dim ofd As New OpenFileDialog With {
                .Filter = "XML Files|*.xml"
                }
        Return If(ofd.ShowDialog() = DialogResult.OK, New XmlDesignerLoader(File.ReadAllText(ofd.FileName)), New XmlDesignerLoader(GetType(UserControl)))
    End Function
End Class