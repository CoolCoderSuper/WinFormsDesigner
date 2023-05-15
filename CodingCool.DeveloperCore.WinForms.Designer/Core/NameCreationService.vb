Imports System.ComponentModel
Imports System.ComponentModel.Design.Serialization

Namespace Core

    Friend Class NameCreationService
        Implements INameCreationService

        Public Function CreateName(container As IContainer, type As Type) As String Implements INameCreationService.CreateName
            If container Is Nothing Then Return String.Empty
            Dim cc As ComponentCollection = container.Components
            Dim min As Integer = Int32.MaxValue
            Dim max As Integer = Int32.MinValue
            Dim count As Integer = 0
            Dim i As Integer = 0

            While i < cc.Count
                Dim comp As Component = TryCast(cc(i), Component)
                If comp.[GetType]() Is type Then
                    count += 1
                    Dim name As String = comp.Site.Name

                    If name.StartsWith(type.Name) Then
                        Try
                            Dim value As Integer = Int32.Parse(name.Substring(type.Name.Length))
                            If value < min Then min = value
                            If value > max Then max = value
                        Catch ex As Exception
                        End Try
                    End If
                End If
                i += 1
            End While
            If count = 0 Then
                Return $"{type.Name}1"
            ElseIf min > 1 Then
                Dim j As Integer = min - 1
                Return $"{type.Name}{j}"
            Else
                Dim j As Integer = max + 1
                Return $"{type.Name}{j}"
            End If
        End Function

        Public Function IsValidName(name As String) As Boolean Implements INameCreationService.IsValidName
            If String.IsNullOrEmpty(name) Then Return False
            If Not Char.IsLetter(name, 0) Then Return False
            If name.StartsWith("_") Then Return False
            Return True
        End Function

        Public Sub ValidateName(name As String) Implements INameCreationService.ValidateName
            If Not IsValidName(name) Then Throw New ArgumentException($"Exception: Invalid name: {name}")
        End Sub

    End Class

End Namespace