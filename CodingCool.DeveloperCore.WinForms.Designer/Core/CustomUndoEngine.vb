Imports System.ComponentModel.Design

Namespace Core

    Public Class CustomUndoEngine
        Inherits UndoEngine
        
        Private ReadOnly _undoStack As Stack(Of UndoUnit) = New Stack(Of UndoUnit)()
        Private ReadOnly _redoStack As Stack(Of UndoUnit) = New Stack(Of UndoUnit)()

        Public Sub New(provider As IServiceProvider)
            MyBase.New(provider)
        End Sub

        Public ReadOnly Property EnableUndo As Boolean
            Get
                Return _undoStack.Count > 0
            End Get
        End Property

        Public ReadOnly Property EnableRedo As Boolean
            Get
                Return _redoStack.Count > 0
            End Get
        End Property

        Public Sub Undo()
            If _undoStack.Count > 0 Then
                Try
                    Dim unit As UndoUnit = _undoStack.Pop()
                    unit.Undo()
                    _redoStack.Push(unit)
                Catch ex As Exception
                    Debug.WriteLine($"{ex.Message}")
                End Try
            End If
        End Sub

        Public Sub Redo()
            If _redoStack.Count > 0 Then
                Try
                    Dim unit As UndoUnit = _redoStack.Pop()
                    unit.Undo()
                    _undoStack.Push(unit)
                Catch ex As Exception
                    Debug.WriteLine($"{ex.Message}")
                End Try
            Else
            End If
        End Sub

        Protected Overrides Sub AddUndoUnit(unit As UndoUnit)
            _undoStack.Push(unit)
        End Sub

    End Class

End Namespace