Imports System.ComponentModel.Design
Imports System.Drawing.Design
Imports System.Windows.Forms

Namespace Core

    'It is the gateway between the toolbox user interface
    'in the development environment and the designers.
    'The designers constantly query the toolbox when the
    'cursor is  over them to get feedback about the selected control
    'NOTE:
    'this is a lightweight class!
    'this class implements the interface IToolboxService
    'it does NOT create a ListBox, it merely links one
    'which is created by user and then referenced by
    'the ToolboxServiceImp::Toolbox property

    Public Class ToolboxServiceImp
        Implements IToolboxService

        Private _DesignerHost As IDesignerHost

        Public Property DesignerHost As IDesignerHost
            Get
                Return _DesignerHost
            End Get
            Private Set(value As IDesignerHost)
                _DesignerHost = value
            End Set
        End Property

        'our real Toolbox
        Public Property Toolbox As ListBox

        'ctor
        Public Sub New(host As IDesignerHost)
            DesignerHost = host
            'Our MainForm adds our ToolboxPane to the DesignerHost's services.
            Toolbox = Nothing
        End Sub

#Region "IToolboxService Members"

        'Add a creator that will convert non-standard tools in the specified format into ToolboxItems, to be associated with a host.
        Private Sub AddCreator(creator As ToolboxItemCreatorCallback, format As String, host As IDesignerHost) Implements IToolboxService.AddCreator
            ' UNIMPLEMENTED - We aren't handling any non-standard tools here. Our toolset is constant.
            'throw new NotImplementedException();
        End Sub

        'Add a creator that will convert non-standard tools in the specified format into ToolboxItems.
        Private Sub AddCreator(creator As ToolboxItemCreatorCallback, format As String) Implements IToolboxService.AddCreator
            ' UNIMPLEMENTED - We aren't handling any non-standard tools here. Our toolset is constant.
            'throw new NotImplementedException();
        End Sub

        'Add a ToolboxItem to our toolbox, in a specific category, bound to a certain host.
        Private Sub AddLinkedToolboxItem(toolboxItem As ToolboxItem, category As String, host As IDesignerHost) Implements IToolboxService.AddLinkedToolboxItem
            ' UNIMPLEMENTED - We didn't end up doing a project system, so there's no need
            ' to add custom tools (despite that we do have a tab for such tools).
            'throw new NotImplementedException();
        End Sub

        'Add a ToolboxItem to our toolbox, bound to a certain host.
        Private Sub AddLinkedToolboxItem(toolboxItem As ToolboxItem, host As IDesignerHost) Implements IToolboxService.AddLinkedToolboxItem
            ' UNIMPLEMENTED - We didn't end up doing a project system, so there's no need
            ' to add custom tools (despite that we do have a tab for such tools).
            'throw new NotImplementedException();
        End Sub

        'Add a ToolboxItem to our toolbox under the specified category.
        Private Sub AddToolboxItem(toolboxItem As ToolboxItem, category As String) Implements IToolboxService.AddToolboxItem
            'we have no category
            CType(Me, IToolboxService).AddToolboxItem(toolboxItem)
        End Sub

        'Add a ToolboxItem to our Toolbox.
        Private Sub AddToolboxItem(toolboxItem As ToolboxItem) Implements IToolboxService.AddToolboxItem
            Toolbox.Items.Add(toolboxItem)
        End Sub

        'Our toolbox has categories akin to those of Visual Studio, but you
        'could group them any which way. Just make sure your IToolboxService knows.
        Private ReadOnly Property CategoryNames As CategoryNameCollection Implements IToolboxService.CategoryNames
            Get
                Return Nothing
            End Get
        End Property

        'necessary for the Drag&Drop
        'We deserialize a ToolboxItem when we drop it onto our design surface.
        'The ToolboxItem comes packaged in a DataObject. We're just working
        'with standard tools and one host, so the host parameter is ignored.
        Private Function DeserializeToolboxItem(serializedObject As Object, host As IDesignerHost) As ToolboxItem Implements IToolboxService.DeserializeToolboxItem
            Return CType(Me, IToolboxService).DeserializeToolboxItem(serializedObject)
        End Function

        'We deserialize a ToolboxItem when we drop it onto our design surface.
        'The ToolboxItem comes packaged in a DataObject.
        Private Function DeserializeToolboxItem(serializedObject As Object) As ToolboxItem Implements IToolboxService.DeserializeToolboxItem
            Return CType(CType(serializedObject, DataObject).GetData(GetType(ToolboxItem)), ToolboxItem)
        End Function

        'Return the selected ToolboxItem in our toolbox if it is associated with this host.
        'Since all of our tools are associated with our only host, the host parameter
        'is ignored.
        Private Function GetSelectedToolboxItem(host As IDesignerHost) As ToolboxItem Implements IToolboxService.GetSelectedToolboxItem
            Return CType(Me, IToolboxService).GetSelectedToolboxItem()
        End Function

        'Return the selected ToolboxItem in our Toolbox
        Private Function GetSelectedToolboxItem() As ToolboxItem Implements IToolboxService.GetSelectedToolboxItem
            If Toolbox Is Nothing OrElse Toolbox.SelectedItem Is Nothing Then Return Nothing
            Dim tbItem As ToolboxItem = CType(Toolbox.SelectedItem, ToolboxItem)
            If tbItem.DisplayName.ToUpper().Contains("POINTER") Then Return Nothing
            Return tbItem
        End Function

        ' Get all the tools in a category
        Private Function GetToolboxItems(category As String, host As IDesignerHost) As ToolboxItemCollection Implements IToolboxService.GetToolboxItems
            'we have no category
            Return CType(Me, IToolboxService).GetToolboxItems()
        End Function

        'Get all of the tools.
        Private Function GetToolboxItems(category As String) As ToolboxItemCollection Implements IToolboxService.GetToolboxItems
            'we have no category
            Return CType(Me, IToolboxService).GetToolboxItems()
        End Function

        'Get all of the tools. We're always using our current host though.
        Private Function GetToolboxItems(host As IDesignerHost) As ToolboxItemCollection Implements IToolboxService.GetToolboxItems
            Return CType(Me, IToolboxService).GetToolboxItems()
        End Function

        'Get all of the tools
        Private Function GetToolboxItems() As ToolboxItemCollection Implements IToolboxService.GetToolboxItems
            If Toolbox Is Nothing Then Return Nothing
            Dim arr = New ToolboxItem(Toolbox.Items.Count - 1) {}
            Toolbox.Items.CopyTo(arr, 0)
            Return New ToolboxItemCollection(arr)
        End Function

        'We are always using standard ToolboxItems, so they are always supported
        Private Function IsSupported(serializedObject As Object, filterAttributes As ICollection) As Boolean Implements IToolboxService.IsSupported
            Return True
        End Function

        'We are always using standard ToolboxItems, so they are always supported
        Private Function IsSupported(serializedObject As Object, host As IDesignerHost) As Boolean Implements IToolboxService.IsSupported
            Return True
        End Function

        'Check if a serialized object is a ToolboxItem. In our case, all of our tools
        'are standard and from a constant set, and they all extend ToolboxItem, so if
        'we can deserialize it in our standard-way, then it is indeed a ToolboxItem
        'The host is ignored
        Private Function IsToolboxItem(serializedObject As Object, host As IDesignerHost) As Boolean Implements IToolboxService.IsToolboxItem
            Return CType(Me, IToolboxService).IsToolboxItem(serializedObject)
        End Function

        'Check if a serialized object is a ToolboxItem. In our case, all of our tools
        'are standard and from a constant set, and they all extend ToolboxItem, so if
        'we can deserialize it in our standard-way, then it is indeed a ToolboxItem
        Private Function IsToolboxItem(serializedObject As Object) As Boolean Implements IToolboxService.IsToolboxItem
            '- If we can deserialize it, it's a ToolboxItem.
            If CType(Me, IToolboxService).DeserializeToolboxItem(serializedObject) IsNot Nothing Then Return True
            Return False
        End Function

        'Refreshes the Toolbox
        Private Sub Refresh() Implements IToolboxService.Refresh
            Toolbox.Refresh()
        End Sub

        'Remove the creator for the specified format, associated with a particular host.
        Private Sub RemoveCreator(format As String, host As IDesignerHost) Implements IToolboxService.RemoveCreator
            ' UNIMPLEMENTED - We aren't handling any non-standard tools here. Our toolset is constant.
            'throw new NotImplementedException();
        End Sub

        'Remove the creator for the specified format.
        Private Sub RemoveCreator(format As String) Implements IToolboxService.RemoveCreator
            ' UNIMPLEMENTED - We aren't handling any non-standard tools here. Our toolset is constant.
            'throw new NotImplementedException();
        End Sub

        'Remove a ToolboxItem from the specified category in our Toolbox.
        Private Sub RemoveToolboxItem(toolboxItem As ToolboxItem, category As String) Implements IToolboxService.RemoveToolboxItem
            CType(Me, IToolboxService).RemoveToolboxItem(toolboxItem)
        End Sub

        'Remove a ToolboxItem from our Toolbox.
        Private Sub RemoveToolboxItem(toolboxItem As ToolboxItem) Implements IToolboxService.RemoveToolboxItem
            If Toolbox Is Nothing Then Return
            Toolbox.SelectedItem = Nothing
            Toolbox.Items.Remove(toolboxItem)
        End Sub

        'If your toolbox is categorized, then it's good for others to know
        'which category is selected.
        Private Property SelectedCategory As String Implements IToolboxService.SelectedCategory
            Get
                Return Nothing
            End Get
            Set(value As String)
                ' UNIMPLEMENTED
            End Set
        End Property

        'This gets called after our IToolboxUser (the designer) ToolPicked method is called.
        'In our case, we select the pointer.
        Private Sub SelectedToolboxItemUsed() Implements IToolboxService.SelectedToolboxItemUsed
            If Toolbox Is Nothing Then Return
            Toolbox.SelectedItem = Nothing
        End Sub

        'Serialize the toolboxItem necessary for the Drag&Drop
        'We serialize a toolbox by packaging it in a DataObject
        Private Function SerializeToolboxItem(toolboxItem As ToolboxItem) As Object Implements IToolboxService.SerializeToolboxItem
            'return new DataObject(typeof( ToolboxItem), toolboxItem );
            Dim dataObject As DataObject = New DataObject()
            dataObject.SetData(GetType(ToolboxItem), toolboxItem)
            Return dataObject
        End Function

        'If we've got a tool selected, then perhaps we want to set our cursor to do
        'something interesting when its over the design surface. If we do, then
        'we do it here and return true. Otherwise we return false so the caller
        'can set the cursor in some default manor.
        Private Function SetCursor() As Boolean Implements IToolboxService.SetCursor
            If Toolbox Is Nothing OrElse Toolbox.SelectedItem Is Nothing Then Return False
            '<Pointer> is not a tool
            Dim tbItem As ToolboxItem = CType(Toolbox.SelectedItem, ToolboxItem)
            If tbItem.DisplayName.ToUpper().Contains("POINTER") Then Return False

            If Toolbox.SelectedItem IsNot Nothing Then
                Cursor.Current = Cursors.Cross
                Return True
            End If
            Return False
        End Function

        'Set the selected ToolboxItem in our Toolbox.
        Private Sub SetSelectedToolboxItem(toolboxItem As ToolboxItem) Implements IToolboxService.SetSelectedToolboxItem
            If Toolbox Is Nothing Then Return
            Toolbox.SelectedItem = toolboxItem
        End Sub

#End Region

    End Class

End Namespace