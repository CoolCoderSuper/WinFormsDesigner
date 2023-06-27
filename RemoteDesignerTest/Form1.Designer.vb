<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        RemoteDesigner1 = New CodingCool.DeveloperCore.WinForms.Designer.Remote.RemoteDesigner()
        SuspendLayout()
        ' 
        ' RemoteDesigner1
        ' 
        RemoteDesigner1.Dock = DockStyle.Fill
        RemoteDesigner1.Location = New Point(0, 0)
        RemoteDesigner1.Name = "RemoteDesigner1"
        RemoteDesigner1.Size = New Size(800, 450)
        RemoteDesigner1.TabIndex = 0
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(RemoteDesigner1)
        Name = "Form1"
        Text = "Form1"
        ResumeLayout(False)
    End Sub

    Friend WithEvents RemoteDesigner1 As CodingCool.DeveloperCore.WinForms.Designer.Remote.RemoteDesigner
End Class
