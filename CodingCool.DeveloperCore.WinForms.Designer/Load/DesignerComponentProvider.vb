Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization

Namespace Load

    Public Class DesignerComponentProvider
        Implements IComponentProvider
        Private ReadOnly _host As IDesignerLoaderHost

        Public Sub New(host As IDesignerLoaderHost)
            _host = host
        End Sub
    
        Public Function CreateComponent(type As Type) As IComponent Implements IComponentProvider.CreateComponent
            Return _host.CreateComponent(type)
        End Function
    
        Public Function CreateComponent(type As Type, name As String) As IComponent Implements IComponentProvider.CreateComponent
            Return _host.CreateComponent(type, name)
        End Function
    
        Public Function GetEventService() As IEventBindingService Implements IComponentProvider.GetEventService
            Return TryCast(_host.GetService(GetType(IEventBindingService)), IEventBindingService)
        End Function
    End Class
End NameSpace