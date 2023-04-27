Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.Windows.Forms.Design

Namespace Core

    Friend Class DesignerOptionServiceExt4SnapLines
        Inherits DesignerOptionService

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub PopulateOptionCollection(options As DesignerOptionCollection)
            If options.Parent IsNot Nothing Then Return
            Dim ops As DesignerOptions = New DesignerOptions()
            ops.UseSnapLines = True
            ops.UseSmartTags = True
            Dim wfd As DesignerOptionCollection = CreateOptionCollection(options, "WindowsFormsDesigner", Nothing)
            CreateOptionCollection(wfd, "General", ops)
        End Sub

    End Class

    Friend Class DesignerOptionServiceExt4Grid
        Inherits DesignerOptionService

        Private _gridSize As Size

        Public Sub New(gridSize As Size)
            MyBase.New()
            _gridSize = gridSize
        End Sub

        Protected Overrides Sub PopulateOptionCollection(options As DesignerOptionCollection)
            If options.Parent IsNot Nothing Then Return
            Dim ops As DesignerOptions = New DesignerOptions()
            ops.GridSize = _gridSize
            ops.SnapToGrid = True
            ops.ShowGrid = True
            ops.UseSnapLines = False
            ops.UseSmartTags = True
            Dim wfd As DesignerOptionCollection = CreateOptionCollection(options, "WindowsFormsDesigner", Nothing)
            CreateOptionCollection(wfd, "General", ops)
        End Sub

    End Class

    Friend Class DesignerOptionServiceExt4GridWithoutSnapping
        Inherits DesignerOptionService

        Private _gridSize As Size

        Public Sub New(gridSize As Size)
            MyBase.New()
            _gridSize = gridSize
        End Sub

        Protected Overrides Sub PopulateOptionCollection(options As DesignerOptionCollection)
            If options.Parent IsNot Nothing Then Return
            Dim ops As DesignerOptions = New DesignerOptions()
            ops.GridSize = _gridSize
            ops.SnapToGrid = False
            ops.ShowGrid = True
            ops.UseSnapLines = False
            ops.UseSmartTags = True
            Dim wfd As DesignerOptionCollection = CreateOptionCollection(options, "WindowsFormsDesigner", Nothing)
            CreateOptionCollection(wfd, "General", ops)
        End Sub

    End Class

    Friend Class DesignerOptionServiceExt4NoGuides
        Inherits DesignerOptionService

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub PopulateOptionCollection(options As DesignerOptionCollection)
            If options.Parent IsNot Nothing Then Return
            Dim ops As DesignerOptions = New DesignerOptions()
            ops.GridSize = New Size(8, 8)
            ops.SnapToGrid = False
            ops.ShowGrid = False
            ops.UseSnapLines = False
            ops.UseSmartTags = True
            Dim wfd As DesignerOptionCollection = CreateOptionCollection(options, "WindowsFormsDesigner", Nothing)
            CreateOptionCollection(wfd, "General", ops)
        End Sub

    End Class

End Namespace