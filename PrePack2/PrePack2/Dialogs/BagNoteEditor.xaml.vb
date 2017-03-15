Imports WHLClasses

Public Class BagNoteEditor
    Dim boxtext As String
    Public ActiveSku As WhlSKU
    Private Sub GetButtonContent(sender As Object, e As EventArgs) Handles SealLabel.Click, LabelOnly.Click, PackDown.Click, HandCount.Click, OwnBox.Click, OwnBag.Click, NoPrepack.Click, BAGA.Click, BAGB.Click, BAGC.Click, BAGD.Click, BAGE.Click, BAGF.Click, BAGG.Click, BAGH.Click, BAGH.Click, LILA1.Click, LILA194.Click, LILA3.Click, LILA5.Click, AUTOSMALL.Click, AUTOLARGE.Click, LSEALER.Click, BOXAA.Click, BOXA.Click, BOXB.Click, BOXC.Click, BOXD.Click, BOXE.Click, BOXF.Click, BOXG.Click, BOXH.Click, BOXHOBBIT.Click, BOXCUTE.Click
        box(sender.content.ToString)

    End Sub

    Private Sub box(box As String)
        boxtext = box
        newbox.Text = boxtext
    End Sub
    Private Sub SaveBag(sender As Object, e As EventArgs) Handles SaveClose.Click
        If newbox.Text = "NEVER BAGGED" Or newbox.Text.Length < 2 Then
            Dim EMsgBox2 As New WPFMsgBoxDialog
            EMsgBox2.Body.Text = "You need to set a bag for this pack."
            EMsgBox2.InitializeComponent()
            EMsgBox2.ShowDialog()

        Else
            MSSQLPublic.insertUpdate("DELETE FROM whldata.prepacklist WHERE sku='"+ActiveSku.SKU+"';")
            Dim query As String = "INSERT INTO whldata.prepacklist (Sku,Bag,Notes) VALUES ('" + ActiveSku.SKU + "','" + boxtext + "','" + NotesBox.Text + "');"
            MSSQLPublic.insertUpdate(query)
            ActiveSku.PrepackInfo.Bag = boxtext
            ActiveSku.PrepackInfo.Notes = NotesBox.Text

            MainWindow.BagShared = boxtext
            MainWindow.NoteInfoShared = NotesBox.Text
            MainWindow.PrepackInfo = MSSQLPublic.SelectData("SELECT * FROM whldata.prepacklist")
            Me.Close()
            Activate()

        End If
    End Sub
End Class
