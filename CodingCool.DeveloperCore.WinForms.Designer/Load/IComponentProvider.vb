Imports System.ComponentModel
Imports System.ComponentModel.Design

Namespace Load

    Public Interface IComponentProvider
        Function CreateComponent(type As Type) As IComponent
        Function CreateComponent(type As Type, name As String) As IComponent
        Function GetEventService() As IEventBindingService
    End Interface
End Namespace