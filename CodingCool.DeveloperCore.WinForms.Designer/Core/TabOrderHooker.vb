Imports System.ComponentModel.Design
Imports System.Reflection

Namespace Core

    Public Class TabOrderHooker
        Private Const _Name_ As String = "TabOrderHooker"
        Private _tabOrder As Object = Nothing

        'Enables/Disables visual TabOrder on the view.
        'internal override
        Public Sub HookTabOrder(ByVal host As IDesignerHost)
            'the TabOrder must be called AFTER the DesignSurface has been loaded
            'therefore we do a little check
            Const _signature_ As String = _Name_ & "::ctor()"
            'the TabOrder must be called AFTER the DesignSurface has been loaded
            'therefore we do a little check
            If host.RootComponent Is Nothing Then Throw New Exception($"{_signature_} - Exception: the TabOrder must be invoked after the DesignSurface has been loaded!")
            Try
                Dim designAssembly As System.Reflection.Assembly = System.Reflection.Assembly.Load("System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
                Dim tabOrderType As Type = designAssembly.[GetType]("System.Windows.Forms.Design.TabOrder")

                If _tabOrder Is Nothing Then
                    'call the ctor passing the IDesignerHost taget object
                    _tabOrder = Activator.CreateInstance(tabOrderType, New Object() {host})
                Else
                    DisposeTabOrder()
                End If
            Catch exx As Exception
                Debug.WriteLine(exx.Message)
                If exx.InnerException IsNot Nothing Then Debug.WriteLine(exx.InnerException.Message)
                Throw
            End Try
        End Sub

        'Disposes the tab order
        Public Sub DisposeTabOrder()
            If _tabOrder Is Nothing Then Return
            Try
                Dim designAssembly As System.Reflection.Assembly = System.Reflection.Assembly.Load("System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
                Dim tabOrderType As Type = designAssembly.GetType("System.Windows.Forms.Design.TabOrder")
                tabOrderType.InvokeMember("Dispose", BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.InvokeMethod, Nothing, _tabOrder, New Object() {True})
                _tabOrder = Nothing
            Catch exx As Exception
                Debug.WriteLine(exx.Message)
                If exx.InnerException IsNot Nothing Then Debug.WriteLine(exx.InnerException.Message)
                Throw
            End Try
        End Sub

    End Class

End Namespace