Public Class BagNoteEditor
    Dim boxtext As String
    Public ActiveSku As WHLClasses.WhlSKU
    Private Sub GetButtonContent(sender As Object, E As EventArgs)

    End Sub

    Private Sub box(box As String)
        boxtext = box
        newbox.Text = boxtext
    End Sub
    Private Sub SaveBag(sender As Object, e As EventArgs) Handles SaveClose.Click

    End Sub
End Class
