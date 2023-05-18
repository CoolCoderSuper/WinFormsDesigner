Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.ComponentModel.Design.Serialization
Imports System.Drawing
Imports System.Windows.Forms
Imports CodingCool.DeveloperCore.WinForms.Designer.Core

Namespace Base
    Public Interface IDesignSurfaceExt
        
        Sub DoAction(command As String)
        
        Sub SwitchTabOrder()
        
        Sub UseSnapLines()

        Sub UseGrid(gridSize As Size)

        Sub UseGridWithoutSnapping(gridSize As Size)

        Sub UseNoGuides()
        
        Function CreateRootComponent(controlType As Type, controlSize As Size) As IComponent

        Function CreateRootComponent(loader As DesignerLoader, controlSize As Size) As IComponent

        Function CreateControl(controlType As Type, controlSize As Size, controlLocation As Point) As Control
        
        Function GetUndoEngine() As CustomUndoEngine
        
        Function GetDesignerHost() As IDesignerHost
        
        Function GetView() As Control

        Function GetToolboxService() As ToolboxService
        
        Function GetLoader() As BasicDesignerLoader

        Sub EnableDragAndDrop()
        
        Sub Rename(component As IComponent, name As String)

    End Interface
End NameSpace