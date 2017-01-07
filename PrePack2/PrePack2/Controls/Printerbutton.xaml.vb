﻿Public Class Printerbutton
    Public Sub Highlight(state As Integer)
        If state = 1 Then
            Me.Background = Brushes.DarkRed
        Else
            Me.Background = Brushes.DarkBlue
        End If
    End Sub
    Public Property PrinterText() As String
        ' Retrieves the value of the private variable colBColor.
        Get
            Return PrinterLabel.Text
        End Get
        ' Stores the selected value in the private variable colBColor, and 
        ' updates the background color of the label control lblDisplay.
        Set(ByVal NewText As String)
            PrinterLabel.Text = NewText
        End Set
    End Property
    Public Sub BigClick()
        For Each buttonthing As Printerbutton In MainWindow.printerbuttons.Controls
            buttonthing.Highlight(0)
        Next
        Me.Highlight(1)
        MainWindow.PrinterName = printername.Text
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles ImageButton1.Click
        BigClick()
    End Sub

    Private Sub printername_Click(sender As Object, e As EventArgs)
        BigClick()
    End Sub
End Class
