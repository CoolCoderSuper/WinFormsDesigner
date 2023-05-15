Imports System.ComponentModel.Design
Imports System.Windows.Forms.Design

Namespace Core

    Friend Class DesignerOptionServiceSnapLines
        Inherits DesignerOptionService

        Protected Overrides Sub PopulateOptionCollection(options As DesignerOptionCollection)
            If options.Parent IsNot Nothing Then Return
            Dim ops As New DesignerOptions() With {.UseSnapLines = True, .UseSmartTags = True}
            Dim wfd As DesignerOptionCollection = CreateOptionCollection(options, "WindowsFormsDesigner", Nothing)
            CreateOptionCollection(wfd, "General", ops)
        End Sub

    End Class
End Namespace