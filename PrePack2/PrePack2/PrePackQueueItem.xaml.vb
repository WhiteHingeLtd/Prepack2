Imports WHLClasses
Imports WHLClasses.Orders

Public Class PrePackQueueItem
    Public SkuNum As String = ""
    Public OrderNum As String = ""

    Public Sub Ivebeenclicked() Handles Me.TouchUp, Me.MouseUp
        If OrderNum = "" Or SkuNum = "" Then
            'Do nu'in'.
        Else
            Dim loader As New GenericDataController
            Dim theOrder As Linnworks.Orders.ExtendedOrder = loader.LoadOrdex("T:\AppData\Orders\" + OrderNum + ".ordex")

            If theOrder.Status = OrderStatus._Withdrawn Then

            Else
                Dim edited As Boolean = False
                For Each foundIssue As IssueData In theOrder.issues
                    If foundIssue.DodgySku = SkuNum Then 'If they're dealing with one issue in the order with this sku number, they're effectively dealing with them all
                        If Not foundIssue.Prepack_WorkingOnIt Then
                            foundIssue.Prepack_WorkingOnIt = True
                            UglyOrangePanel.Visible = True 'This needs to turn on IMMEDIATELY
                            edited = True
                        Else
                            MainWindow.EMsgbox("Someone is currently working on this one. It may take a moment to update the status.")
                            UglyOrangePanel.Visible = True 'This wasn't fast enough
                            Exit For
                        End If
                    End If
                Next
                If edited Then
                    theOrder.SaveToDisk()
                End If
            End If
        End If
    End Sub
End Class
