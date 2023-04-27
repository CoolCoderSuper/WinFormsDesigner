Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.Windows.Forms

Namespace Core

    Friend Class MenuCommandServiceExt
        Implements IMenuCommandService

        'this ServiceProvider is the DesignsurfaceExt2 instance
        'passed as paramter inside the ctor
        Private _serviceProvider As IServiceProvider = Nothing

        Private _menuCommandService As MenuCommandService = Nothing

        Public Sub New(serviceProvider As IServiceProvider)
            _serviceProvider = serviceProvider
            _menuCommandService = New MenuCommandService(serviceProvider)
        End Sub

        Public Sub ShowContextMenu(menuID As CommandID, x As Integer, y As Integer) Implements IMenuCommandService.ShowContextMenu
            Dim menu As ContextMenuStrip = New ContextMenuStrip()

            'Add the standard commands CUT/COPY/PASTE/DELETE
            Dim command As MenuCommand = FindCommand(StandardCommands.Cut)

            If command IsNot Nothing Then
                Dim menuItem As ToolStripMenuItem = New ToolStripMenuItem("Cut")
                AddHandler menuItem.Click, AddressOf OnMenuClicked
                menuItem.Tag = command
                menu.Items.Add(menuItem)
            End If

            command = FindCommand(StandardCommands.Copy)

            If command IsNot Nothing Then
                Dim menuItem As ToolStripMenuItem = New ToolStripMenuItem("Copy")
                AddHandler menuItem.Click, AddressOf OnMenuClicked
                menuItem.Tag = command
                menu.Items.Add(menuItem)
            End If

            command = FindCommand(StandardCommands.Paste)

            If command IsNot Nothing Then
                Dim menuItem As ToolStripMenuItem = New ToolStripMenuItem("Paste")
                AddHandler menuItem.Click, AddressOf OnMenuClicked
                menuItem.Tag = command
                menu.Items.Add(menuItem)
            End If

            command = FindCommand(StandardCommands.Delete)

            If command IsNot Nothing Then
                Dim menuItem As ToolStripMenuItem = New ToolStripMenuItem("Delete")
                AddHandler menuItem.Click, AddressOf OnMenuClicked
                menuItem.Tag = command
                menu.Items.Add(menuItem)
            End If

            'Show the contexteMenu
            Dim surface As DesignSurface = CType(_serviceProvider, DesignSurface)
            Dim viewService As Control = CType(surface.View, Control)

            If viewService IsNot Nothing Then
                menu.Show(viewService, viewService.PointToClient(New Point(x, y)))
            End If
        End Sub

        'Management of the selections of the contexteMenu
        Private Sub OnMenuClicked(sender As Object, e As EventArgs)
            Dim menuItem As ToolStripMenuItem = TryCast(sender, ToolStripMenuItem)

            If menuItem IsNot Nothing AndAlso TypeOf menuItem.Tag Is MenuCommand Then
                Dim command As MenuCommand = TryCast(menuItem.Tag, MenuCommand)
                command.Invoke()
            End If
        End Sub

        Public Sub AddCommand(command As MenuCommand) Implements IMenuCommandService.AddCommand
            _menuCommandService.AddCommand(command)
        End Sub

        Public Sub AddVerb(verb As DesignerVerb) Implements IMenuCommandService.AddVerb
            _menuCommandService.AddVerb(verb)
        End Sub

        Public Function FindCommand(commandID As CommandID) As MenuCommand Implements IMenuCommandService.FindCommand
            Return _menuCommandService.FindCommand(commandID)
        End Function

        Public Function GlobalInvoke(commandID As CommandID) As Boolean Implements IMenuCommandService.GlobalInvoke
            Return _menuCommandService.GlobalInvoke(commandID)
        End Function

        Public Sub RemoveCommand(command As MenuCommand) Implements IMenuCommandService.RemoveCommand
            _menuCommandService.RemoveCommand(command)
        End Sub

        Public Sub RemoveVerb(verb As DesignerVerb) Implements IMenuCommandService.RemoveVerb
            _menuCommandService.RemoveVerb(verb)
        End Sub

        Public ReadOnly Property Verbs As DesignerVerbCollection Implements IMenuCommandService.Verbs
            Get
                Return _menuCommandService.Verbs
            End Get
        End Property

    End Class

End Namespace