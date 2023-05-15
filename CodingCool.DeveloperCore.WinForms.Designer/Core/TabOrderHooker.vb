Imports System.ComponentModel.Design
Imports System.Reflection

Namespace Core

    Public Class TabOrderHooker
        Private _tabOrder As Object = Nothing
        
        Public Sub HookTabOrder(host As IDesignerHost)
            If host.RootComponent Is Nothing Then Throw New Exception($"Exception: the TabOrder must be invoked after the DesignSurface has been loaded!")
            Try
                Dim designAssembly As Assembly = Assembly.Load("System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
                Dim tabOrderType As Type = designAssembly.[GetType]("System.Windows.Forms.Design.TabOrder")
                If _tabOrder Is Nothing Then
                    _tabOrder = Activator.CreateInstance(tabOrderType, New Object() {host})
                Else
                    DisposeTabOrder()
                End If
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                If ex.InnerException IsNot Nothing Then Debug.WriteLine(ex.InnerException.Message)
                Throw
            End Try
        End Sub

        Public Sub DisposeTabOrder()
            If _tabOrder Is Nothing Then Return
            Try
                Dim designAssembly As Assembly = Assembly.Load("System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
                Dim tabOrderType As Type = designAssembly.GetType("System.Windows.Forms.Design.TabOrder")
                tabOrderType.InvokeMember("Dispose", BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.InvokeMethod, Nothing, _tabOrder, New Object() {True})
                _tabOrder = Nothing
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
                If ex.InnerException IsNot Nothing Then Debug.WriteLine(ex.InnerException.Message)
                Throw
            End Try
        End Sub

    End Class

End Namespace