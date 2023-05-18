Imports System.Drawing.Design
Imports System.Windows.Forms

Namespace Core
    Public Interface IToolbox
        Sub AddItem(toolboxItem As ToolboxItem)
        Sub Refresh()
        Sub RemoveItem(toolboxItem As ToolboxItem)
        Property SelectedItem As ToolboxItem
        Sub DoDragDrop(toolboxItem As ToolboxItem, dragDropEffects As DragDropEffects)
        Function GetItems() As ToolboxItem()
        Event MouseDown(sender As Object, e As MouseEventArgs)
        Event MouseMove(sender As Object, e As MouseEventArgs)
    End Interface
End NameSpace