Public Class BagNoteEditor
    Dim IsNew As Boolean
    Dim boxtext As String
    Public ActiveSku As WHLClasses.WhlSKU

    Private Sub BagNoteEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Loaded
        If MainWindow.NoBox = True Then
            IsNew = True
        Else
            IsNew = False
        End If
        NotesBox.Text = MainWindow.NoteInfo.Text
        newbox.Text = MainWindow.Bag.Text
        Packsize.Text = "Current pack size: " + ActiveSku.PackSize.ToString

    End Sub
    Private Sub box(box As String)
        boxtext = box
        newbox.Text = boxtext
    End Sub

    Private Sub Button30_Click(sender As Object, e As EventArgs) Handles Button30.Click
        If newbox.Text = "NEVER BAGGED" Or newbox.Text.Length < 2 Then
            Main.EMsgbox("You need to set a bag for this pack.")
        Else
            Dim query As String = "REPLACE INTO whldata.prepacklist (Sku,Bag,Notes) VALUES ('" + ActiveSku.SKU + "','" + boxtext + "','" + NotesBox.Text + "');"
            WHLClasses.MySql.insertUpdate(query)
            ActiveSku.PrepackInfo.Bag = boxtext
            ActiveSku.PrepackInfo.Notes = NotesBox.Text
            Main.Bag.Text = boxtext
            Main.NoteInfo.Text = NotesBox.Text
            Main.PrepackInfo = WHLClasses.MySql.SelectData("SELECT * FROM whldata.prepacklist")
            Me.Close()
            Main.Activate()
            Main.ScanFocusBox.Focus()
            Me.Dispose()
        End If

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        box("SEAL & LABEL")
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        box("LABEL ONLY")
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        box("PACK DOWN")
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        box("HAND COUNT")
    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        box("OWN BOX")

    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        box("OWN BAG")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        box("BAG A")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        box("BAG B")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        box("BAG C")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        box("BAG D")
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        box("BAG E")
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        box("BAG F")
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        box("BAG G")
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        box("BAG H")
    End Sub

    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        box("LIL A1")
    End Sub

    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click
        box("LIL A194")
    End Sub

    Private Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        box("LIL A3")
    End Sub

    Private Sub Button24_Click(sender As Object, e As EventArgs) Handles Button24.Click
        box("LIL A5")
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        box("BOX AA")
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        box("BOX A")
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        box("BOX B")
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        box("BOX C")
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        box("BOX D")
    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        box("BOX E")
    End Sub

    Private Sub Button25_Click(sender As Object, e As EventArgs) Handles Button25.Click
        box("BOX F")
    End Sub

    Private Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        box("BOX G")
    End Sub

    Private Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click
        box("BOX H")
    End Sub

    Private Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click
        box("BOX HOBBIT")
    End Sub

    Private Sub Button29_Click(sender As Object, e As EventArgs) Handles Button29.Click
        box("BOX CUTE")
    End Sub

    Private Sub Button31_Click(sender As Object, e As EventArgs) Handles Button31.Click
        box("AUTOBAG SMALL")
    End Sub

    Private Sub Button32_Click(sender As Object, e As EventArgs) Handles Button32.Click
        box("AUTOBAG LARGE")
    End Sub

    Private Sub Button33_Click(sender As Object, e As EventArgs) Handles Button33.Click
        box("L SEALER")
    End Sub

    Private Sub Button34_Click(sender As Object, e As EventArgs) Handles Button34.Click
        box("NO PREPACK")
    End Sub
End Class