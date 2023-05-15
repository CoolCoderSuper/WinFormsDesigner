Imports System.Drawing.Design
Imports CodingCool.DeveloperCore.WinForms.Designer.Core

Public Class DesignerToolBox
    Inherits ListBox
    Implements IToolbox

    Public Sub AddItem(toolboxItem As ToolboxItem) Implements IToolbox.AddItem
        Items.Add(toolboxItem)
    End Sub
    
    Public Sub Refresh() Implements IToolbox.Refresh
        MyBase.Refresh()
    End Sub

    Public Sub RemoveItem(toolboxItem As ToolboxItem) Implements IToolbox.RemoveItem
        Items.Remove(toolboxItem)
    End Sub

    Public Property SelectedItem As ToolboxItem Implements IToolbox.SelectedItem
        Get
            Return MyBase.SelectedItem
        End Get
        Set
            MyBase.SelectedItem = Value
        End Set
    End Property

    Public Sub DoDragDrop(toolboxItem As ToolboxItem, dragDropEffects As DragDropEffects) Implements IToolbox.DoDragDrop
        MyBase.DoDragDrop(toolboxItem, dragDropEffects)
    End Sub

    Public Function GetItems() As ToolboxItem() Implements IToolbox.GetItems
        Dim arr = New ToolboxItem(Items.Count - 1) {}
        Items.CopyTo(arr, 0)
        Return arr
    End Function

    Public Event MouseDown(sender As Object, e As MouseEventArgs) Implements IToolbox.MouseDown
    
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        RaiseEvent MouseDown(Me, e)
        MyBase.OnMouseDown(e)
    End Sub
End Class