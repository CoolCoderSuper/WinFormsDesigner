Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports Microsoft.Win32.SafeHandles

''' <summary>
''' Allow pipe communication between a server and a client
''' </summary>
Public Class PipeClient

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function CreateFile(pipeName As String, dwDesiredAccess As UInteger, dwShareMode As UInteger, lpSecurityAttributes As IntPtr, dwCreationDisposition As UInteger, dwFlagsAndAttributes As UInteger, hTemplate As IntPtr) As SafeFileHandle
    End Function

    ''' <summary>
    ''' Handles messages received from a server pipe
    ''' </summary>
    ''' <paramname="message">The byte message received</param>
    Public Delegate Sub MessageReceivedHandler(message As Byte())

    ''' <summary>
    ''' Event is called whenever a message is received from the server pipe
    ''' </summary>
    Public Event MessageReceived As MessageReceivedHandler

    ''' <summary>
    ''' Handles server disconnected messages
    ''' </summary>
    Public Delegate Sub ServerDisconnectedHandler()

    ''' <summary>
    ''' Event is called when the server pipe is severed.
    ''' </summary>
    Public Event ServerDisconnected As ServerDisconnectedHandler

    Const BUFFER_SIZE As Integer = 4096

    Private stream As FileStream
    Private handle As SafeFileHandle
    Private readThread As Thread

    ''' <summary>
    ''' Is this client connected to a server pipe
    ''' </summary>
    Public Property Connected As Boolean

    ''' <summary>
    ''' The pipe this client is connected to.
    ''' </summary>
    Public Property PipeName As String

    ''' <summary>
    ''' Connects to the server with a pipename.
    ''' </summary>
    ''' <param name="pipename">The name of the pipe to connect to.</param>
    Public Sub Connect(pipename As String)
        If Connected Then Throw New Exception("Already connected to pipe server.")

        Me.PipeName = pipename

        handle = CreateFile(Me.PipeName, &HC0000000UI, 0, IntPtr.Zero, 3, &H40000000, IntPtr.Zero) ' GENERIC_READ | GENERIC_WRITE = 0x80000000 | 0x40000000
        ' OPEN_EXISTING
        ' FILE_FLAG_OVERLAPPED

        'could not create handle - server probably not running
        If handle.IsInvalid Then Return

        Connected = True

        'start listening for messages
        readThread = New Thread(AddressOf Read) With {
.IsBackground = True
}
        readThread.Start()
    End Sub

    ''' <summary>
    ''' Disconnects from the server.
    ''' </summary>
    Public Sub Disconnect()
        If Not Connected Then Return

        ' we're no longer connected to the server
        Connected = False
        PipeName = Nothing

        'clean up resource
        If stream IsNot Nothing Then stream.Close()
        handle.Close()

        stream = Nothing
        handle = Nothing
    End Sub

    Private Sub Read()
        stream = New FileStream(handle, FileAccess.ReadWrite, BUFFER_SIZE, True)
        Dim readBuffer = New Byte(4095) {}

        While True
            Dim bytesRead = 0

            Using ms As MemoryStream = New MemoryStream()
                Try
                    ' read the total stream length
                    Dim totalSize = stream.Read(readBuffer, 0, 4)

                    ' client has disconnected
                    If totalSize = 0 Then Exit While

                    totalSize = BitConverter.ToInt32(readBuffer, 0)

                    Do
                        Dim numBytes = stream.Read(readBuffer, 0, Math.Min(totalSize - bytesRead, BUFFER_SIZE))

                        ms.Write(readBuffer, 0, numBytes)

                        bytesRead += numBytes

                    Loop While bytesRead < totalSize

                Catch
                    'read error has occurred
                    Exit While
                End Try

                'client has disconnected
                If bytesRead = 0 Then Exit While

                'fire message received event
                RaiseEvent MessageReceived(ms.ToArray())
            End Using
        End While

        ' if connected, then the disconnection was
        ' caused by a server terminating, otherwise it was from
        ' a call to Disconnect()
        If Connected Then
            'clean up resource
            stream.Close()
            handle.Close()

            stream = Nothing
            handle = Nothing

            ' we're no longer connected to the server
            Connected = False
            PipeName = Nothing

            RaiseEvent ServerDisconnected()
        End If
    End Sub

    ''' <summary>
    ''' Sends a message to all connected clients.
    ''' </summary>
    ''' <param name="messageString">The message to send.</param>
    Public Sub SendMessage(messageString As String)
        Dim message As Byte() = Encoding.UTF8.GetBytes(messageString)
        SendMessage(message)
    End Sub

    ''' <summary>
    ''' Sends a message to the server.
    ''' </summary>
    ''' <param name="message">The message to send.</param>
    ''' <returns>True if the message is sent successfully - false otherwise.</returns>
    Public Function SendMessage(message As Byte()) As Boolean
        Try
            ' write the entire stream length
            stream.Write(BitConverter.GetBytes(message.Length), 0, 4)

            stream.Write(message, 0, message.Length)
            stream.Flush()
            Return True
        Catch
            Return False
        End Try
    End Function
End Class