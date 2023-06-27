Imports System.Text
Imports System.Windows.Forms

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
        _client.SendMessage($"Connect:{Label1.Handle}")
    End Sub
End Class