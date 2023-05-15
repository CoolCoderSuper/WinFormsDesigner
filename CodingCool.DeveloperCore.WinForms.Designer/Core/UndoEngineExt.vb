Imports System.ComponentModel.Design

Namespace Core

    Public Class UndoEngineExt
        Inherits UndoEngine

        Private _Name_ As String = "UndoEngineExt"
        Private undoStack As Stack(Of UndoUnit) = New Stack(Of UndoUnit)()
        Private redoStack As Stack(Of UndoUnit) = New Stack(Of UndoUnit)()

        Public Sub New(provider As IServiceProvider)
            MyBase.New(provider)
        End Sub

        Public ReadOnly Property EnableUndo As Boolean
            Get
                Return undoStack.Count > 0
            End Get
        End Property

        Public ReadOnly Property EnableRedo As Boolean
            Get
                Return redoStack.Count > 0
            End Get
        End Property

        Public Sub Undo()
            If undoStack.Count > 0 Then
                Try
                    Dim unit As UndoUnit = undoStack.Pop()
                    unit.Undo()
                    redoStack.Push(unit)
                Catch ex As Exception
                    Debug.WriteLine($"{_Name_}{ex.Message}")
                End Try
            End If
        End Sub

        Public Sub Redo()
            If redoStack.Count > 0 Then
                Try
                    Dim unit As UndoUnit = redoStack.Pop()
                    unit.Undo()
                    undoStack.Push(unit)
                Catch ex As Exception
                    Debug.WriteLine($"{_Name_}{ex.Message}")
                End Try
            Else
            End If
        End Sub

        Protected Overrides Sub AddUndoUnit(unit As UndoEngine.UndoUnit)
            undoStack.Push(unit)
        End Sub

    End Class

End Namespace