Imports System.Windows.Window
Imports WHLClasses

Public Class BundleDialog

    Public bundleoptionsal As SkuCollection
    Public chosenshsku As WhlSKU

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If BundleOptions.SelectedIndex > -1 Then
            Me.DialogResult = System.Windows.Window.DialogResult.OK
            Me.Close()
        End If

    End Sub

    Private Sub BundleDialog_Shown(sender As Object, e As EventArgs) Handles MyBase.Load
        'Get the options
        BundleOptions.Items.Clear()
        For Each item As WhlSKU In bundleoptionsal
            BundleOptions.Items.Add(item.SKU + " - " + item.Title.Invoice)
        Next

    End Sub

    Private Sub BundleOptions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles BundleOptions.SelectedIndexChanged
        chosenshsku = bundleoptionsal.SearchSKUS(BundleOptions.SelectedItem.ToString.Split(" - ")(0).ToString)(0)
    End Sub
End Class
