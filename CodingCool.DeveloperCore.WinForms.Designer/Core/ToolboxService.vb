Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Reflection
Imports System.Windows.Forms

Namespace Core
    
    Public Class ToolboxService
        Implements IToolboxService

        Public Property DesignerHost As IDesignerHost

        Public Property Toolbox As IToolbox
        
        Public Sub New(host As IDesignerHost)
            DesignerHost = host
            Toolbox = Nothing
        End Sub
        
        Private Sub AddCreator(creator As ToolboxItemCreatorCallback, format As String, host As IDesignerHost) Implements IToolboxService.AddCreator
            ' UNIMPLEMENTED - We aren't handling any non-standard tools here. Our toolset is constant.
        End Sub

        Private Sub AddCreator(creator As ToolboxItemCreatorCallback, format As String) Implements IToolboxService.AddCreator
            ' UNIMPLEMENTED - We aren't handling any non-standard tools here. Our toolset is constant.
        End Sub
        
        Private Sub AddLinkedToolboxItem(toolboxItem As ToolboxItem, category As String, host As IDesignerHost) Implements IToolboxService.AddLinkedToolboxItem
            ' UNIMPLEMENTED - We didn't end up doing a project system, so there's no need
            ' to add custom tools (despite that we do have a tab for such tools).
        End Sub
        
        Private Sub AddLinkedToolboxItem(toolboxItem As ToolboxItem, host As IDesignerHost) Implements IToolboxService.AddLinkedToolboxItem
            ' UNIMPLEMENTED - We didn't end up doing a project system, so there's no need
            ' to add custom tools (despite that we do have a tab for such tools).
        End Sub
        
        Private Sub AddToolboxItem(toolboxItem As ToolboxItem, category As String) Implements IToolboxService.AddToolboxItem
            CType(Me, IToolboxService).AddToolboxItem(toolboxItem)
        End Sub
        
        Private Sub AddToolboxItem(toolboxItem As ToolboxItem) Implements IToolboxService.AddToolboxItem
            Toolbox.AddItem(toolboxItem)
        End Sub
        
        Private ReadOnly Property CategoryNames As CategoryNameCollection Implements IToolboxService.CategoryNames
            Get
                Return Nothing
            End Get
        End Property
        
        Private Function DeserializeToolboxItem(serializedObject As Object, host As IDesignerHost) As ToolboxItem Implements IToolboxService.DeserializeToolboxItem
            Return CType(Me, IToolboxService).DeserializeToolboxItem(serializedObject)
        End Function
        
        Private Function DeserializeToolboxItem(serializedObject As Object) As ToolboxItem Implements IToolboxService.DeserializeToolboxItem
            Return CType(CType(serializedObject, DataObject).GetData(GetType(ToolboxItem)), ToolboxItem)
        End Function
        
        Private Function GetSelectedToolboxItem(host As IDesignerHost) As ToolboxItem Implements IToolboxService.GetSelectedToolboxItem
            Return CType(Me, IToolboxService).GetSelectedToolboxItem()
        End Function
        
        Private Function GetSelectedToolboxItem() As ToolboxItem Implements IToolboxService.GetSelectedToolboxItem
            If Toolbox Is Nothing OrElse Toolbox.SelectedItem Is Nothing Then Return Nothing
            Dim tbItem As ToolboxItem = Toolbox.SelectedItem
            If tbItem.DisplayName.ToUpper().Contains("POINTER") Then Return Nothing
            Return tbItem
        End Function
        
        Private Function GetToolboxItems(category As String, host As IDesignerHost) As ToolboxItemCollection Implements IToolboxService.GetToolboxItems
            Return CType(Me, IToolboxService).GetToolboxItems()
        End Function
        
        Private Function GetToolboxItems(category As String) As ToolboxItemCollection Implements IToolboxService.GetToolboxItems
            Return CType(Me, IToolboxService).GetToolboxItems()
        End Function
        
        Private Function GetToolboxItems(host As IDesignerHost) As ToolboxItemCollection Implements IToolboxService.GetToolboxItems
            Return CType(Me, IToolboxService).GetToolboxItems()
        End Function
        
        Private Function GetToolboxItems() As ToolboxItemCollection Implements IToolboxService.GetToolboxItems
            If Toolbox Is Nothing Then Return Nothing
            Return New ToolboxItemCollection(Toolbox.GetItems())
        End Function
        
        Private Function IsSupported(serializedObject As Object, filterAttributes As ICollection) As Boolean Implements IToolboxService.IsSupported
            Return True
        End Function
        
        Private Function IsSupported(serializedObject As Object, host As IDesignerHost) As Boolean Implements IToolboxService.IsSupported
            Return True
        End Function

        Private Function IsToolboxItem(serializedObject As Object, host As IDesignerHost) As Boolean Implements IToolboxService.IsToolboxItem
            Return CType(Me, IToolboxService).IsToolboxItem(serializedObject)
        End Function
        
        Private Function IsToolboxItem(serializedObject As Object) As Boolean Implements IToolboxService.IsToolboxItem
            If CType(Me, IToolboxService).DeserializeToolboxItem(serializedObject) IsNot Nothing Then Return True
            Return False
        End Function
        
        Private Sub Refresh() Implements IToolboxService.Refresh
            Toolbox.Refresh()
        End Sub
        
        Private Sub RemoveCreator(format As String, host As IDesignerHost) Implements IToolboxService.RemoveCreator
            ' UNIMPLEMENTED - We aren't handling any non-standard tools here. Our toolset is constant.
        End Sub
        
        Private Sub RemoveCreator(format As String) Implements IToolboxService.RemoveCreator
            ' UNIMPLEMENTED - We aren't handling any non-standard tools here. Our toolset is constant.
        End Sub
        
        Private Sub RemoveToolboxItem(toolboxItem As ToolboxItem, category As String) Implements IToolboxService.RemoveToolboxItem
            CType(Me, IToolboxService).RemoveToolboxItem(toolboxItem)
        End Sub
        
        Private Sub RemoveToolboxItem(toolboxItem As ToolboxItem) Implements IToolboxService.RemoveToolboxItem
            If Toolbox Is Nothing Then Return
            Toolbox.SelectedItem = Nothing
            Toolbox.RemoveItem(toolboxItem)
        End Sub
        
        Private Property SelectedCategory As String Implements IToolboxService.SelectedCategory
            Get
                Return Nothing
            End Get
            Set
                ' UNIMPLEMENTED
            End Set
        End Property
        
        Private Sub SelectedToolboxItemUsed() Implements IToolboxService.SelectedToolboxItemUsed
            If Toolbox Is Nothing Then Return
            Toolbox.SelectedItem = Nothing
        End Sub
        
        Private Function SerializeToolboxItem(toolboxItem As ToolboxItem) As Object Implements IToolboxService.SerializeToolboxItem
            Dim dataObject As DataObject = New DataObject()
            dataObject.SetData(GetType(ToolboxItem), toolboxItem)
            Return dataObject
        End Function
        
        Private Function SetCursor() As Boolean Implements IToolboxService.SetCursor
            If Toolbox Is Nothing OrElse Toolbox.SelectedItem Is Nothing Then Return False
            Dim tbItem As ToolboxItem = Toolbox.SelectedItem
            If tbItem.DisplayName.ToUpper().Contains("POINTER") Then Return False
            If Toolbox.SelectedItem IsNot Nothing Then
                Cursor.Current = Cursors.Cross
                Return True
            End If
            Return False
        End Function
        
        Private Sub SetSelectedToolboxItem(toolboxItem As ToolboxItem) Implements IToolboxService.SetSelectedToolboxItem
            If Toolbox Is Nothing Then Return
            Toolbox.SelectedItem = toolboxItem
        End Sub
        

        Public Sub AddItems(ParamArray assemblies As Assembly())
            For Each t As Type In assemblies.Select(Function(x) x.GetTypes().Where(Function(y) y.IsSubclassOf(GetType(Component)))).SelectMany(Function(x) x).Where(Function(x) Not Toolbox.GetItems().Select(Function(y) y.TypeName).Contains(x.FullName)).Where(Function(x) x.GetConstructors().Any(Function(y) Not y.GetParameters().Any()) AndAlso Not x.IsAbstract AndAlso x.IsPublic)
                Dim item As New ToolboxItem(t)
                Dim imageAttr As ToolboxBitmapAttribute = TypeDescriptor.GetAttributes(t)(GetType(ToolboxBitmapAttribute))
                If imageAttr IsNot Nothing Then
                    item.Bitmap = imageAttr.GetImage(t)
                    item.OriginalBitmap = imageAttr.GetImage(t)
                End If
                AddToolboxItem(item)
            Next
        End Sub
    End Class
End Namespace