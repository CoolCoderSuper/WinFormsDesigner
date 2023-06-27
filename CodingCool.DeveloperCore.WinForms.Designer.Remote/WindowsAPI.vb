Imports System.Runtime.InteropServices

Public Module WindowsAPI
    <DllImport("user32.dll", SetLastError:=True)>
    Public Function SetParent(hWndChild As IntPtr, hWndNewParent As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Function SetWindowPos(hWnd As IntPtr, hWndInsertAfter As IntPtr, X As Integer, Y As Integer, cx As Integer, cy As Integer, uFlags As SetWindowPosFlags) As Boolean
    End Function
End Module