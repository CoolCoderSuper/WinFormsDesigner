Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.Windows.Forms.Design

Namespace Core
    Friend Class DesignerOptionServiceNoGuides
        Inherits DesignerOptionService

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub PopulateOptionCollection(options As DesignerOptionCollection)
            If options.Parent IsNot Nothing Then Return
            Dim ops As New DesignerOptions() With {
                    .GridSize = New Size(8, 8),
                    .SnapToGrid = False,
                    .ShowGrid = False,
                    .UseSnapLines = False,
                    .UseSmartTags = True
                    }
            Dim wfd As DesignerOptionCollection = CreateOptionCollection(options, "WindowsFormsDesigner", Nothing)
            CreateOptionCollection(wfd, "General", ops)
        End Sub

    End Class
End NameSpace