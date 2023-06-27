Imports System.Runtime.InteropServices

Public Module WindowsAPI
    <DllImport("user32.dll", SetLastError:=True)>
    Public Function SetParent(hWndChild As IntPtr, hWndNewParent As IntPtr) As IntPtr
    End Function
End Module