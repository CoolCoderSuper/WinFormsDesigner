Imports System.ComponentModel
Imports System.ComponentModel.Design.Serialization

'NameCreationServiceImp - Implementing INameCreationService
'The INameCreationService interface is used to supply a name to the control just created
'In the CreateName() we use the same naming algorithm used by Visual Studio: just
'increment an integer counter until we find a name that isn't already in use.
Namespace Core

    Friend Class NameCreationServiceImp
        Implements INameCreationService

        Private Const _Name_ As String = "NameCreationServiceImp"

        Public Sub New()
        End Sub

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
                        Catch __unusedException1__ As Exception
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
            'Check that name is "something" and that is a string with at least one char
            If [String].IsNullOrEmpty(name) Then Return False

            'then the first character must be a letter
            If Not Char.IsLetter(name, 0) Then Return False

            'then don't allow a leading underscore
            If name.StartsWith("_") Then Return False

            'ok, it's a valid name
            Return True
        End Function

        Public Sub ValidateName(name As String) Implements INameCreationService.ValidateName
            Const _signature_ As String = _Name_ & "::ValidateName()"
            'Use our existing method to check, if it's invalid throw an exception
            If Not IsValidName(name) Then Throw New ArgumentException($"{_signature_} - Exception: Invalid name: {name}")
        End Sub

    End Class

End Namespace