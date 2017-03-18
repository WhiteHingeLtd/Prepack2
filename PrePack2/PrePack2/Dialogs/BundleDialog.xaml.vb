Imports WHLClasses
Class BundleDialog
    Public BundleOptions As SkuCollection = New SkuCollection(True)
    Public ChosenSku as WhlSKU = New WhlSKU()

    Private Sub BundleDialogWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles BundleDialogWindow.Loaded
        Focus()
        For each Sku As WhlSKU in BundleOptions
            SkuBox.Items.Add(Sku)
        Next

    End Sub

    Private Sub SubmitButton_Click(sender As Object, e As RoutedEventArgs) Handles SubmitButton.Click
        MainWindow.SelectedBundleSku = Skubox.SelectedItem
    End Sub
End Class
