Imports Microsoft.Win32.SafeHandles
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

''' <summary>
''' Allow pipe communication between a server and a client
''' </summary>
Public Class PipeServer

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function CreateNamedPipe(pipeName As String, dwOpenMode As UInteger, dwPipeMode As UInteger, nMaxInstances As UInteger, nOutBufferSize As UInteger, nInBufferSize As UInteger, nDefaultTimeOut As UInteger, lpSecurityAttributes As IntPtr) As SafeFileHandle
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function ConnectNamedPipe(hNamedPipe As SafeFileHandle, lpOverlapped As IntPtr) As Integer
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function DisconnectNamedPipe(hHandle As SafeFileHandle) As Boolean
    End Function

    <StructLayoutAttribute(LayoutKind.Sequential)>
    Friend Structure SECURITY_DESCRIPTOR
        Public revision As Byte
        Public size As Byte
        Public control As Short
        Public owner As IntPtr
        Public group As IntPtr
        Public sacl As IntPtr
        Public dacl As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure SECURITY_ATTRIBUTES
        Public nLength As Integer
        Public lpSecurityDescriptor As IntPtr
        Public bInheritHandle As Integer
    End Structure

    Private Const SECURITY_DESCRIPTOR_REVISION As UInteger = 1

    <DllImport("advapi32.dll", SetLastError:=True)>
    Private Shared Function InitializeSecurityDescriptor(ByRef sd As SECURITY_DESCRIPTOR, dwRevision As UInteger) As Boolean
    End Function

    <DllImport("advapi32.dll", SetLastError:=True)>
    Private Shared Function SetSecurityDescriptorDacl(ByRef sd As SECURITY_DESCRIPTOR, daclPresent As Boolean, dacl As IntPtr, daclDefaulted As Boolean) As Boolean
    End Function

    Public Class Client
        Public handle As SafeFileHandle
        Public stream As FileStream
    End Class

    ''' <summary>
    ''' Handles messages received from a client pipe
    ''' </summary>
    ''' <param name="message">The byte message received</param>
    Public Delegate Sub MessageReceivedHandler(message As Byte())

    ''' <summary>
    ''' Event is called whenever a message is received from a client pipe
    ''' </summary>
    Public Event MessageReceived As MessageReceivedHandler


    ''' <summary>
    ''' Handles client disconnected messages
    ''' </summary>
    Public Delegate Sub ClientDisconnectedHandler()

    ''' <summary>
    ''' Event is called when a client pipe is severed.
    ''' </summary>
    Public Event ClientDisconnected As ClientDisconnectedHandler

    Const BUFFER_SIZE As Integer = 4096

    Private listenThread As Thread
    Private ReadOnly clients As List(Of Client) = New List(Of Client)()

    ''' <summary>
    ''' The total number of PipeClients connected to this server
    ''' </summary>
    Public ReadOnly Property TotalConnectedClients As Integer
        Get
            SyncLock clients
                Return clients.Count
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' The name of the pipe this server is connected to
    ''' </summary>
    Public Property PipeName As String

    ''' <summary>
    ''' Is the server currently running
    ''' </summary>
    Public Property Running As Boolean

    ''' <summary>
    ''' Starts the pipe server on a particular name.
    ''' </summary>
    ''' <param name="pipename">The name of the pipe.</param>
    Public Sub Start(pipename As String)
        Me.PipeName = pipename

        'start the listening thread
        listenThread = New Thread(AddressOf ListenForClients) With {
    .IsBackground = True
}

        listenThread.Start()

        Running = True
    End Sub

    Private Sub ListenForClients()
        Dim sd As SECURITY_DESCRIPTOR = New SECURITY_DESCRIPTOR()

        ' set the Security Descriptor to be completely permissive
        InitializeSecurityDescriptor(sd, SECURITY_DESCRIPTOR_REVISION)
        SetSecurityDescriptorDacl(sd, True, IntPtr.Zero, False)

        Dim ptrSD = Marshal.AllocCoTaskMem(Marshal.SizeOf(sd))
        Marshal.StructureToPtr(sd, ptrSD, False)

        Dim sa As SECURITY_ATTRIBUTES = New SECURITY_ATTRIBUTES With {
    .nLength = Marshal.SizeOf(sd),
    .lpSecurityDescriptor = ptrSD,
    .bInheritHandle = 1
}

        Dim ptrSA = Marshal.AllocCoTaskMem(Marshal.SizeOf(sa))
        Marshal.StructureToPtr(sa, ptrSA, False)


        While True
            ' Creates an instance of a named pipe for one client

            ' DUPLEX | FILE_FLAG_OVERLAPPED = 0x00000003 | 0x40000000;
            Dim clientHandle = CreateNamedPipe(PipeName, &H40000003, 0, 255, BUFFER_SIZE, BUFFER_SIZE, 0, ptrSA)

            'could not create named pipe instance
            If clientHandle.IsInvalid Then Continue While

            Dim success = ConnectNamedPipe(clientHandle, IntPtr.Zero)

            'could not connect client
            If success = 0 Then
                ' close the handle, and wait for the next client
                clientHandle.Close()
                Continue While
            End If

            Dim client As Client = New Client With {
    .handle = clientHandle
}

            SyncLock clients
                clients.Add(client)
            End SyncLock

            Dim readThread As Thread = New Thread(AddressOf Read) With {
    .IsBackground = True
}
            readThread.Start(client)
        End While

        ' free up the ptrs (never reached due to infinite loop)
        Marshal.FreeCoTaskMem(ptrSD)
        Marshal.FreeCoTaskMem(ptrSA)
    End Sub

    Private Sub Read(clientObj As Object)
        Dim client = CType(clientObj, Client)
        client.stream = New FileStream(client.handle, FileAccess.ReadWrite, BUFFER_SIZE, True)
        Dim buffer = New Byte(4095) {}

        While True
            Dim bytesRead = 0

            Using ms As MemoryStream = New MemoryStream()
                Try
                    ' read the total stream length
                    Dim totalSize = client.stream.Read(buffer, 0, 4)

                    ' client has disconnected
                    If totalSize = 0 Then Exit While

                    totalSize = BitConverter.ToInt32(buffer, 0)

                    Do
                        Dim numBytes = client.stream.Read(buffer, 0, Math.Min(totalSize - bytesRead, BUFFER_SIZE))

                        ms.Write(buffer, 0, numBytes)

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

        ' the clients must be locked - otherwise "stream.Close()"
        ' could be called while SendMessage(byte[]) is being called on another thread.
        ' This leads to an IO error & several wasted days.
        SyncLock clients
            'clean up resources
            DisconnectNamedPipe(client.handle)
            client.stream.Close()
            client.handle.Close()

            clients.Remove(client)
        End SyncLock

        ' invoke the event, a client disconnected
        RaiseEvent ClientDisconnected()
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
    ''' Sends a message to all connected clients.
    ''' </summary>
    ''' <param name="message">The message to send.</param>
    Public Sub SendMessage(message As Byte())
        SyncLock clients
            'get the entire stream length
            Dim messageLength = BitConverter.GetBytes(message.Length)

            For Each client In clients
                ' length
                client.stream.Write(messageLength, 0, 4)

                ' data
                client.stream.Write(message, 0, message.Length)
                client.stream.Flush()
            Next
        End SyncLock
    End Sub
End Class