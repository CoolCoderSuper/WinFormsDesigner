Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.Windows.Forms.Design

Namespace Core
    Friend Class DesignerOptionServiceGridWithoutSnapping
        Inherits DesignerOptionService

        Private ReadOnly _gridSize As Size

        Public Sub New(gridSize As Size)
            MyBase.New()
            _gridSize = gridSize
        End Sub

        Protected Overrides Sub PopulateOptionCollection(options As DesignerOptionCollection)
            If options.Parent IsNot Nothing Then Return
            Dim ops As New DesignerOptions() With {
                    .GridSize = _gridSize,
                    .SnapToGrid = False,
                    .ShowGrid = True,
                    .UseSnapLines = False,
                    .UseSmartTags = True
                    }
            Dim wfd As DesignerOptionCollection = CreateOptionCollection(options, "WindowsFormsDesigner", Nothing)
            CreateOptionCollection(wfd, "General", ops)
        End Sub

    End Class
End NameSpace