Imports WHLClasses
Class BundleDialog
    Public BundleOptions As SkuCollection = New SkuCollection(True)
    Public ChosenSku as WhlSKU = New WhlSKU()
    Public ReturnSkuColl as SkuCollection = New SkuCollection(True)

    Private Sub BundleDialogWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles BundleDialogWindow.Loaded
        Focus()
        For each Sku As WhlSKU in BundleOptions
            If Sku.SKU.Contains("xxxx") 
                Continue For               
            End If
            ReturnSkuColl.Add(Sku)
            SkuBox.Items.Add(Sku.Title.Label)
        Next

    End Sub

    Private Sub SubmitButton_Click(sender As Object, e As RoutedEventArgs) Handles SubmitButton.Click
        
        ChosenSku = ReturnSkuColl.SearchSKUSReturningSingleSku(SkuBox.SelectedItem)
        ReturnSkuColl.Clear()
        BundleOptions.Clear()
        Close()
    End Sub
End Class
