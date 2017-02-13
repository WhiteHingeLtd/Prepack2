Public Class WPFMsgBoxDialog
    Private Sub HandleLoad() Handles Me.Loaded
        Me.Focus()
    End Sub


    Private Sub CloseButton_Click(sender As Object, e As RoutedEventArgs) Handles CloseButton.Click
        Me.Close()
    End Sub
End Class
