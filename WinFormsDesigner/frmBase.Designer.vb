﻿Imports CodingCool.DeveloperCore.WinForms.Designer.Base

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBase
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
        Me.lstToolbox = New DesignerToolBox()
        Me.toolStripMenuItemTabOrder = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripMenuItemTools = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripMenuItemDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemPaste = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemCut = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripMenuItemReDo = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemUnDo = New System.Windows.Forms.ToolStripMenuItem()
        Me.editToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.newFormAlignControlByhandMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.newFormUseGridMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.newFormUseGridandSnapMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.newFormUseSnapLinesMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.newToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.pgProperties = New System.Windows.Forms.PropertyGrid()
        Me.tcDesigners = New System.Windows.Forms.TabControl()
        Me.pnl4Toolbox = New System.Windows.Forms.Panel()
        Me.splitContainer = New System.Windows.Forms.SplitContainer()
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuStrip1.SuspendLayout()
        Me.pnl4Toolbox.SuspendLayout()
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer.Panel1.SuspendLayout()
        Me.splitContainer.Panel2.SuspendLayout()
        Me.splitContainer.SuspendLayout()
        Me.SuspendLayout()
        '
        'lstToolbox
        '
        Me.lstToolbox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstToolbox.FormattingEnabled = True
        Me.lstToolbox.Location = New System.Drawing.Point(0, 0)
        Me.lstToolbox.Margin = New System.Windows.Forms.Padding(2)
        Me.lstToolbox.Name = "lstToolbox"
        Me.lstToolbox.Size = New System.Drawing.Size(119, 535)
        Me.lstToolbox.TabIndex = 0
        '
        'toolStripMenuItemTabOrder
        '
        Me.toolStripMenuItemTabOrder.Name = "toolStripMenuItemTabOrder"
        Me.toolStripMenuItemTabOrder.Size = New System.Drawing.Size(180, 22)
        Me.toolStripMenuItemTabOrder.Text = "Tab Order"
        '
        'toolStripMenuItemTools
        '
        Me.toolStripMenuItemTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripMenuItemTabOrder})
        Me.toolStripMenuItemTools.Name = "toolStripMenuItemTools"
        Me.toolStripMenuItemTools.Size = New System.Drawing.Size(46, 20)
        Me.toolStripMenuItemTools.Text = "&Tools"
        '
        'toolStripSeparator4
        '
        Me.toolStripSeparator4.Name = "toolStripSeparator4"
        Me.toolStripSeparator4.Size = New System.Drawing.Size(177, 6)
        '
        'ToolStripMenuItemDelete
        '
        Me.ToolStripMenuItemDelete.Name = "ToolStripMenuItemDelete"
        Me.ToolStripMenuItemDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete
        Me.ToolStripMenuItemDelete.Size = New System.Drawing.Size(180, 22)
        Me.ToolStripMenuItemDelete.Text = "Delete"
        '
        'ToolStripMenuItemPaste
        '
        Me.ToolStripMenuItemPaste.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripMenuItemPaste.Name = "ToolStripMenuItemPaste"
        Me.ToolStripMenuItemPaste.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.V), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemPaste.Size = New System.Drawing.Size(180, 22)
        Me.ToolStripMenuItemPaste.Text = "Paste"
        '
        'ToolStripMenuItemCopy
        '
        Me.ToolStripMenuItemCopy.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripMenuItemCopy.Name = "ToolStripMenuItemCopy"
        Me.ToolStripMenuItemCopy.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemCopy.Size = New System.Drawing.Size(180, 22)
        Me.ToolStripMenuItemCopy.Text = "Copy"
        '
        'ToolStripMenuItemCut
        '
        Me.ToolStripMenuItemCut.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripMenuItemCut.Name = "ToolStripMenuItemCut"
        Me.ToolStripMenuItemCut.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.X), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemCut.Size = New System.Drawing.Size(180, 22)
        Me.ToolStripMenuItemCut.Text = "Cut"
        '
        'toolStripSeparator3
        '
        Me.toolStripSeparator3.Name = "toolStripSeparator3"
        Me.toolStripSeparator3.Size = New System.Drawing.Size(177, 6)
        '
        'ToolStripMenuItemReDo
        '
        Me.ToolStripMenuItemReDo.Name = "ToolStripMenuItemReDo"
        Me.ToolStripMenuItemReDo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Y), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemReDo.Size = New System.Drawing.Size(180, 22)
        Me.ToolStripMenuItemReDo.Text = "Redo"
        '
        'ToolStripMenuItemUnDo
        '
        Me.ToolStripMenuItemUnDo.Name = "ToolStripMenuItemUnDo"
        Me.ToolStripMenuItemUnDo.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Z), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemUnDo.Size = New System.Drawing.Size(180, 22)
        Me.ToolStripMenuItemUnDo.Text = "Undo"
        '
        'editToolStripMenuItem
        '
        Me.editToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemUnDo, Me.ToolStripMenuItemReDo, Me.toolStripSeparator3, Me.ToolStripMenuItemCut, Me.ToolStripMenuItemCopy, Me.ToolStripMenuItemPaste, Me.ToolStripMenuItemDelete, Me.toolStripSeparator4})
        Me.editToolStripMenuItem.Name = "editToolStripMenuItem"
        Me.editToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.editToolStripMenuItem.Text = "&Edit"
        '
        'newFormAlignControlByhandMenuItem
        '
        Me.newFormAlignControlByhandMenuItem.Name = "newFormAlignControlByhandMenuItem"
        Me.newFormAlignControlByhandMenuItem.Size = New System.Drawing.Size(227, 22)
        Me.newFormAlignControlByhandMenuItem.Text = "Align control by &hand"
        '
        'newFormUseGridMenuItem
        '
        Me.newFormUseGridMenuItem.Name = "newFormUseGridMenuItem"
        Me.newFormUseGridMenuItem.Size = New System.Drawing.Size(227, 22)
        Me.newFormUseGridMenuItem.Text = "Use Gri&d"
        '
        'newFormUseGridandSnapMenuItem
        '
        Me.newFormUseGridandSnapMenuItem.Name = "newFormUseGridandSnapMenuItem"
        Me.newFormUseGridandSnapMenuItem.Size = New System.Drawing.Size(227, 22)
        Me.newFormUseGridandSnapMenuItem.Text = "Use &Grid and snap to the grid"
        '
        'newFormUseSnapLinesMenuItem
        '
        Me.newFormUseSnapLinesMenuItem.Name = "newFormUseSnapLinesMenuItem"
        Me.newFormUseSnapLinesMenuItem.Size = New System.Drawing.Size(227, 22)
        Me.newFormUseSnapLinesMenuItem.Text = "Use &SnapLines"
        '
        'newToolStripMenuItem
        '
        Me.newToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.newFormUseSnapLinesMenuItem, Me.newFormUseGridandSnapMenuItem, Me.newFormUseGridMenuItem, Me.newFormAlignControlByhandMenuItem})
        Me.newToolStripMenuItem.Name = "newToolStripMenuItem"
        Me.newToolStripMenuItem.Size = New System.Drawing.Size(43, 20)
        Me.newToolStripMenuItem.Text = "&New"
        '
        'menuStrip1
        '
        Me.menuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.newToolStripMenuItem, Me.editToolStripMenuItem, Me.toolStripMenuItemTools, Me.SaveToolStripMenuItem, Me.CloseToolStripMenuItem})
        Me.menuStrip1.Location = New System.Drawing.Point(123, 0)
        Me.menuStrip1.Name = "menuStrip1"
        Me.menuStrip1.Size = New System.Drawing.Size(773, 24)
        Me.menuStrip1.TabIndex = 4
        Me.menuStrip1.Text = "menuStrip1"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(43, 20)
        Me.SaveToolStripMenuItem.Text = "Save"
        '
        'pgProperties
        '
        Me.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pgProperties.Location = New System.Drawing.Point(0, 0)
        Me.pgProperties.Name = "pgProperties"
        Me.pgProperties.Size = New System.Drawing.Size(191, 511)
        Me.pgProperties.TabIndex = 0
        '
        'tcDesigners
        '
        Me.tcDesigners.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcDesigners.Location = New System.Drawing.Point(0, 0)
        Me.tcDesigners.Margin = New System.Windows.Forms.Padding(2)
        Me.tcDesigners.Name = "tcDesigners"
        Me.tcDesigners.SelectedIndex = 0
        Me.tcDesigners.Size = New System.Drawing.Size(561, 511)
        Me.tcDesigners.TabIndex = 0
        '
        'pnl4Toolbox
        '
        Me.pnl4Toolbox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.pnl4Toolbox.Controls.Add(Me.lstToolbox)
        Me.pnl4Toolbox.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnl4Toolbox.Location = New System.Drawing.Point(0, 0)
        Me.pnl4Toolbox.Margin = New System.Windows.Forms.Padding(2)
        Me.pnl4Toolbox.Name = "pnl4Toolbox"
        Me.pnl4Toolbox.Size = New System.Drawing.Size(123, 539)
        Me.pnl4Toolbox.TabIndex = 5
        '
        'splitContainer
        '
        Me.splitContainer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.splitContainer.Location = New System.Drawing.Point(128, 26)
        Me.splitContainer.Name = "splitContainer"
        '
        'splitContainer.Panel1
        '
        Me.splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Window
        Me.splitContainer.Panel1.Controls.Add(Me.tcDesigners)
        '
        'splitContainer.Panel2
        '
        Me.splitContainer.Panel2.Controls.Add(Me.pgProperties)
        Me.splitContainer.Size = New System.Drawing.Size(756, 511)
        Me.splitContainer.SplitterDistance = 561
        Me.splitContainer.TabIndex = 3
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.CloseToolStripMenuItem.Text = "Close"
        '
        'frmBase
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(896, 539)
        Me.Controls.Add(Me.menuStrip1)
        Me.Controls.Add(Me.pnl4Toolbox)
        Me.Controls.Add(Me.splitContainer)
        Me.Name = "frmBase"
        Me.Text = "Base WinForms Designer"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.menuStrip1.ResumeLayout(False)
        Me.menuStrip1.PerformLayout
        Me.pnl4Toolbox.ResumeLayout(false)
        Me.splitContainer.Panel1.ResumeLayout(false)
        Me.splitContainer.Panel2.ResumeLayout(false)
        CType(Me.splitContainer,System.ComponentModel.ISupportInitialize).EndInit
        Me.splitContainer.ResumeLayout(false)
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Private WithEvents lstToolbox As DesignerToolBox
    Private WithEvents toolStripMenuItemTabOrder As ToolStripMenuItem
    Private WithEvents toolStripMenuItemTools As ToolStripMenuItem
    Private WithEvents toolStripSeparator4 As ToolStripSeparator
    Private WithEvents ToolStripMenuItemDelete As ToolStripMenuItem
    Private WithEvents ToolStripMenuItemPaste As ToolStripMenuItem
    Private WithEvents ToolStripMenuItemCopy As ToolStripMenuItem
    Private WithEvents ToolStripMenuItemCut As ToolStripMenuItem
    Private WithEvents toolStripSeparator3 As ToolStripSeparator
    Private WithEvents ToolStripMenuItemReDo As ToolStripMenuItem
    Private WithEvents ToolStripMenuItemUnDo As ToolStripMenuItem
    Private WithEvents editToolStripMenuItem As ToolStripMenuItem
    Private WithEvents newFormAlignControlByhandMenuItem As ToolStripMenuItem
    Private WithEvents newFormUseGridMenuItem As ToolStripMenuItem
    Private WithEvents newFormUseGridandSnapMenuItem As ToolStripMenuItem
    Private WithEvents newFormUseSnapLinesMenuItem As ToolStripMenuItem
    Private WithEvents newToolStripMenuItem As ToolStripMenuItem
    Private WithEvents menuStrip1 As MenuStrip
    Private WithEvents pgProperties As PropertyGrid
    Private WithEvents tcDesigners As TabControl
    Private WithEvents pnl4Toolbox As Panel
    Private WithEvents splitContainer As SplitContainer
    Friend WithEvents SaveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CloseToolStripMenuItem As ToolStripMenuItem
End Class
