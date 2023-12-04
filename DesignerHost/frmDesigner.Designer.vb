Imports CodingCool.DeveloperCore.WinForms.Designer.Base

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        lstToolbox = New DesignerToolBox()
        pgDesigner = New System.Windows.Forms.PropertyGrid()
        pnlDesigner = New System.Windows.Forms.Panel()
        pnlMain = New System.Windows.Forms.Panel()
        SplitContainer1 = New System.Windows.Forms.SplitContainer()
        pnlMain.SuspendLayout()
        CType(SplitContainer1, ComponentModel.ISupportInitialize).BeginInit()
        SplitContainer1.Panel1.SuspendLayout()
        SplitContainer1.Panel2.SuspendLayout()
        SplitContainer1.SuspendLayout()
        SuspendLayout()
        ' 
        ' lstToolbox
        ' 
        lstToolbox.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left
        lstToolbox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        lstToolbox.FormattingEnabled = True
        lstToolbox.Location = New System.Drawing.Point(0, 0)
        lstToolbox.Margin = New System.Windows.Forms.Padding(2)
        lstToolbox.Name = "lstToolbox"
        lstToolbox.SelectedItem = Nothing
        lstToolbox.Size = New System.Drawing.Size(168, 452)
        lstToolbox.TabIndex = 0
        ' 
        ' pgDesigner
        ' 
        pgDesigner.Dock = System.Windows.Forms.DockStyle.Fill
        pgDesigner.Location = New System.Drawing.Point(0, 0)
        pgDesigner.Name = "pgDesigner"
        pgDesigner.Size = New System.Drawing.Size(158, 450)
        pgDesigner.TabIndex = 0
        ' 
        ' pnlDesigner
        ' 
        pnlDesigner.Dock = System.Windows.Forms.DockStyle.Fill
        pnlDesigner.Location = New System.Drawing.Point(0, 0)
        pnlDesigner.Name = "pnlDesigner"
        pnlDesigner.Size = New System.Drawing.Size(465, 450)
        pnlDesigner.TabIndex = 1
        ' 
        ' pnlMain
        ' 
        pnlMain.Controls.Add(SplitContainer1)
        pnlMain.Controls.Add(lstToolbox)
        pnlMain.Location = New System.Drawing.Point(0, 0)
        pnlMain.Name = "pnlMain"
        pnlMain.Size = New System.Drawing.Size(800, 450)
        pnlMain.TabIndex = 2
        ' 
        ' SplitContainer1
        ' 
        SplitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        SplitContainer1.Location = New System.Drawing.Point(173, 0)
        SplitContainer1.Name = "SplitContainer1"
        ' 
        ' SplitContainer1.Panel1
        ' 
        SplitContainer1.Panel1.Controls.Add(pnlDesigner)
        ' 
        ' SplitContainer1.Panel2
        ' 
        SplitContainer1.Panel2.Controls.Add(pgDesigner)
        SplitContainer1.Size = New System.Drawing.Size(627, 450)
        SplitContainer1.SplitterDistance = 465
        SplitContainer1.TabIndex = 3
        ' 
        ' frmDesigner
        ' 
        AutoScaleDimensions = New System.Drawing.SizeF(7.0F, 15.0F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        ClientSize = New System.Drawing.Size(800, 450)
        Controls.Add(pnlMain)
        Name = "frmDesigner"
        Text = "DesignerHost"
        pnlMain.ResumeLayout(False)
        SplitContainer1.Panel1.ResumeLayout(False)
        SplitContainer1.Panel2.ResumeLayout(False)
        CType(SplitContainer1, ComponentModel.ISupportInitialize).EndInit()
        SplitContainer1.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents lstToolbox As DesignerToolBox
    Friend WithEvents pgDesigner As System.Windows.Forms.PropertyGrid
    Friend WithEvents pnlDesigner As System.Windows.Forms.Panel
    Friend WithEvents pnlMain As System.Windows.Forms.Panel
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
End Class
