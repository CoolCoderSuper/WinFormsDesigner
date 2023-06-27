Imports System.Text
Imports System.Windows.Forms

Public Class RemoteDesigner
    Inherits Panel
    
    Dim ReadOnly _server As New PipeServer
    Dim _designerHandle As IntPtr

    Public Sub Connect()
        Dim pipeName As String = $"\\.\pipe\{Guid.NewGuid()}"
        _server.Start(pipeName)
        AddHandler _server.MessageReceived, AddressOf MessageReceived
        Dim p As Process = Process.Start("C:\CodingCool\Code\Projects\WinFormsDesigner\DesignerHost\bin\Debug\net472\DesignerHost.exe", pipeName)
        p.WaitForInputIdle()
    End Sub

    Public Sub Disconnect()
        _server.SendMessage("Disconnect")
    End Sub

    Private Sub MessageReceived(message As Byte())
        Dim s As String = Encoding.UTF8.GetString(message)
        If s.StartsWith("Connect") Then
            _designerHandle = New IntPtr(CInt(s.Split(":")(1)))
            Invoke(Sub() SetParent(_designerHandle, Handle))
            SetWindowPos(_designerHandle, New IntPtr(SpecialWindowHandles.HWND_TOP), 0, 0, Width, Height, SetWindowPosFlags.SWP_SHOWWINDOW)
        End If
    End Sub

    Private Sub RemoteDesigner_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        SetWindowPos(_designerHandle, New IntPtr(SpecialWindowHandles.HWND_TOP), 0, 0, Width, Height, SetWindowPosFlags.SWP_SHOWWINDOW)
    End Sub
End Class