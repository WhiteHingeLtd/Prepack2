Public Class SkuCacheDownloadMask
    Private Sub TimerMain_Tick(sender As Object, e As EventArgs) Handles TimerMain.Tick
        Try
            LoadTextc.Text = Main.Skusfull.CountDone
        Catch ex As Exception

        End Try
    End Sub
End Class