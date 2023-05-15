Imports System.ComponentModel.Design.Serialization
Imports System.Windows.Forms

Namespace Core

    Friend Class DesignerSerializationService
        Implements IDesignerSerializationService

        Private ReadOnly _serviceProvider As IServiceProvider

        Public Sub New(serviceProvider As IServiceProvider)
            _serviceProvider = serviceProvider
        End Sub

        Public Function Deserialize(serializationData As Object) As ICollection Implements IDesignerSerializationService.Deserialize
            Dim serializationStore As SerializationStore = TryCast(serializationData, SerializationStore)
            If serializationStore IsNot Nothing Then
                Dim componentSerializationService As ComponentSerializationService = TryCast(_serviceProvider.GetService(GetType(ComponentSerializationService)), ComponentSerializationService)
                Dim collection As ICollection = componentSerializationService.Deserialize(serializationStore)
                Return collection
            End If
            Return Array.Empty(Of Object)
        End Function

        Public Function Serialize(objects As ICollection) As Object Implements IDesignerSerializationService.Serialize
            Dim componentSerializationService As ComponentSerializationService = TryCast(_serviceProvider.GetService(GetType(ComponentSerializationService)), ComponentSerializationService)
            Dim returnObject As SerializationStore
            Using serializationStore As SerializationStore = componentSerializationService.CreateStore()
                For Each obj As Object In objects
                    If TypeOf obj Is Control Then componentSerializationService.Serialize(serializationStore, obj)
                Next
                returnObject = serializationStore
            End Using
            Return returnObject
        End Function

    End Class

End Namespace