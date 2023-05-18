Imports System.Drawing.Design
Imports CodingCool.DeveloperCore.WinForms.Designer.Core

Public Class DesignerToolBox
    Inherits ListBox
    Implements IToolbox

    Public Sub New()
        DrawMode = DrawMode.OwnerDrawFixed
    End Sub
    
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
    
    Protected Overrides Sub OnDrawItem(e As DrawItemEventArgs)
        e.DrawBackground()
        Dim objItem As ToolboxItem = Items(e.Index)
        If objItem.OriginalBitmap IsNot Nothing Then e.Graphics.DrawImage(DirectCast(objItem.OriginalBitmap, Image), GetImagePoints(e.Bounds))
        e.Graphics.DrawString(objItem.DisplayName, e.Font, New SolidBrush(e.ForeColor), GetTextRect(e.Bounds))
        e.DrawFocusRectangle()
    End Sub
    
    Private Shared Function GetImagePoints(rectangle As Rectangle) As Point()
        Return New Point(2) {New Point(rectangle.Left, rectangle.Top), New Point(16, rectangle.Top), New Point(rectangle.Left, rectangle.Bottom)}
    End Function
    
    Private Shared Function GetTextRect(rectangle As Rectangle) As Rectangle
        Return New Rectangle(16, rectangle.Top, 0 ,0)
    End Function

End Class