<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BundleDialog
    Inherits System.Windows.Controls.UserControl

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
        Me.Label1 = New System.Windows.Controls.Label()
        Me.Label2 = New System.Windows.Controls.Label()
        Me.BundleOptions = New System.Windows.Controls.ListBox()
        Me.Button1 = New System.Windows.Controls.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(522, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "This item has links to other items, which may be more what you're looking for."
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(13, 50)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(660, 20)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "The first one is usually the one you actually scanned, and each one after should " &
    "be the related ones"
        '
        'BundleOptions
        '
        Me.BundleOptions.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BundleOptions.FormattingEnabled = True
        Me.BundleOptions.ItemHeight = 25
        Me.BundleOptions.Location = New System.Drawing.Point(12, 91)
        Me.BundleOptions.Name = "BundleOptions"
        Me.BundleOptions.Size = New System.Drawing.Size(765, 204)
        Me.BundleOptions.TabIndex = 2
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Segoe UI", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(141, 302)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(490, 65)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Choose"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'BundleDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(789, 379)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.BundleOptions)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Segoe UI", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "BundleDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Option Dialog"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents BundleOptions As ListBox
    Friend WithEvents Button1 As Button
End Class
