<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDesigner
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Label1 = New Windows.Forms.Label()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Drawing.Point(303, 177)
        Label1.Name = "Label1"
        Label1.Size = New Drawing.Size(80, 15)
        Label1.TabIndex = 0
        Label1.Text = "Other process"
        ' 
        ' frmDesigner
        ' 
        AutoScaleDimensions = New Drawing.SizeF(7F, 15F)
        AutoScaleMode = Windows.Forms.AutoScaleMode.Font
        ClientSize = New Drawing.Size(800, 450)
        Controls.Add(Label1)
        Name = "frmDesigner"
        Text = "DesignerHost"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Windows.Forms.Label
End Class
