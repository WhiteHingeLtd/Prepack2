Imports LoginModule
Imports Spire.Barcode
Imports System.ComponentModel
Imports System.Drawing.Printing
Imports System.Runtime.CompilerServices
Imports System.Speech.Synthesis
Imports WHLClasses
Imports WHLClasses.Orders

Class MainWindow
    Public bundlepacks As String = ""
    Private HasAutobagHistory As Boolean = False
    Public logins As FullscreenLogin = New FullscreenLogin
    Private NewScan As Boolean = False
    Public NoBox As Boolean = False
    Public Shared PrepackInfo As ArrayList = New ArrayList
    Public Shared PrinterName As String = ""

    Private pritinglabel As String
    Private PritingLabelID As Integer = 1
    Public SelectedSKU As WhlSKU
    Private Synthesizer As SpeechSynthesizer
    Private Emps As New EmployeeCollection
    Friend PrepackQueueWorker As New BackgroundWorker

    Friend Skusfull As New WHLClasses.SkuCollection(True)
    Private Skus As New WHLClasses.SkuCollection(True)
    Private labels As New LabelMaker

    Dim client As Services.OrderServer.iOSClientChannel
    '27/08/16
    Dim CurrentEmployeeAnalytic As WarehouseAnalytics.AnalyticBase
    Dim CurrentAnalyticSession As WarehouseAnalytics.SessionAnalytic

    Private Sub Keypad(ByVal Key As String)
        If (Key = "Clear") Then
            ScanBox.Text = ""
        Else
            ScanBox.Text += Key
        End If

    End Sub

    Private Sub keyPrint_BtnClick(ByVal sender As Object, ByVal e As EventArgs) Handles keyPrint.Click
        Dim PrinterSettings As New PrinterSettings
        PrinterName = PrinterSettings.PrinterName
        'Print quantity. 
        Dim printquantity As Integer = 1
        Dim CheckLength As String = ScanBox.Text
        If CheckLength.Length > 0 Then
            printquantity = Convert.ToInt32(ScanBox.Text)
        End If


        ' COMPLETELY REDO PRINTING CODE TO USE LABELGENERATOR
        If PritingLabelID = 1 Then               'Prepack
            If IsNothing(SelectedSKU) Then
                Dim EMsgBox As New WPFMsgBoxDialog

                EMsgBox.Body.Text = "You need to choose a packsize."
                EMsgBox.ShowDialog()
            Else
                'It's all good. 
                If printquantity > 50 Then
                    printquantity = 50
                End If
                Try
                    '29/02/2016     Moved the logging code to before label generation so the batch code can be added. 
                    If IsNumeric(WHLClasses.MySQL.insertUpdate("INSERT INTO whldata.log_prepack (UserId, UserFullName, WorkStationName, Time, PP_Sku, PP_Label, PP_Quantity, PP_ShortTitle, PP_Binrack, DateA) VALUES (" + logins.AuthenticatedUser.PayrollId.ToString + ",'" + logins.AuthenticatedUser.FullName + "','" + My.Computer.Name + "','" + Now.ToString("dd/MM/yyyy HH:mm") + "','" + SelectedSKU.SKU + "','" + PritingLabelID.ToString + "','" + printquantity.ToString + "','" + SelectedSKU.Title.Label + "','" + SelectedSKU.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + "','" + Now.ToString("yyyy-MM-dd") + "');")) Then
                        Dim batch As String = WHLClasses.MySQL.SelectData("SELECT LAST_INSERT_ID();")(0)(0).ToString
                        labels.PrepackLabel(SelectedSKU, PrinterName, printquantity, batch)

                        '27/08/16
                        For i As Integer = 0 To printquantity
                            RegisterOrderAnalytic()
                        Next
                    Else
                        Dim exString As String
                        Try
                            exString = "Local location name returned the following: " + SelectedSKU.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + vbNewLine + "Please tell IT."
                        Catch ex As Exception
                            exString = "Local location name broke - Please tell IT"
                        End Try
                        MsgBox("Something went wrong." + vbNewLine + exString)
                    End If


                Catch ex As ArgumentException
                    Dim EMsgBox As New WPFMsgBoxDialog

                    EMsgBox.Body.Text = "This item doesn't appear to have a GS1. Please speak to IT"
                    EMsgBox.ShowDialog()
                End Try

            End If
        ElseIf PritingLabelID = 2 Then          'Shelf
            If printquantity > 5 Then
                printquantity = 5
            End If
            If Not IsNothing(ActiveItem) Then
                '29/02/2016     Moved the logging code to before label generation so the batch code can be added. 
                If IsNumeric(WHLClasses.MySQL.insertUpdate("INSERT INTO whldata.log_prepack (UserId, UserFullName, WorkStationName, Time, PP_Sku, PP_Label, PP_Quantity, PP_ShortTitle, PP_Binrack, DateA) VALUES (" + logins.AuthenticatedUser.PayrollId.ToString + ",'" + logins.AuthenticatedUser.FullName + "','" + My.Computer.Name + "','" + Now.ToString("dd/MM/yyyy HH:mm") + "','" + ActiveItem.SKU + "','" + PritingLabelID.ToString + "','" + printquantity.ToString + "','" + ActiveItem.Title.Label + "','" + ActiveItem.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + "','" + Now.ToString("yyyy-MM-dd") + "');")) Then
                    labels.ShelfLabel(ActiveItem, ActiveChildren, PrinterName, printquantity)

                    '27/08/16
                    For i As Integer = 0 To printquantity
                        RegisterOrderAnalytic()
                    Next

                Else
                    Dim exString As String
                    Try
                        exString = "Local location name returned the following: " + SelectedSKU.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + vbNewLine + "Please tell IT."
                    Catch ex As Exception
                        exString = "Local location name broke - Please tell IT"
                    End Try
                    MsgBox("Something went wrong." + vbNewLine + exString)
                End If
            End If
        ElseIf PritingLabelID = 3 Then           'Magnet
            If printquantity > 5 Then
                printquantity = 5
            End If
            If Not IsNothing(ActiveItem) Then
                '29/02/2016     Moved the logging code to before label generation so the batch code can be added. 
                If IsNumeric(WHLClasses.MySQL.insertUpdate("INSERT INTO whldata.log_prepack (UserId, UserFullName, WorkStationName, Time, PP_Sku, PP_Label, PP_Quantity, PP_ShortTitle, PP_Binrack, DateA) VALUES (" + logins.AuthenticatedUser.PayrollId.ToString + ",'" + logins.AuthenticatedUser.FullName + "','" + My.Computer.Name + "','" + Now.ToString("dd/MM/yyyy HH:mm") + "','" + ActiveItem.SKU + "','" + PritingLabelID.ToString + "','" + printquantity.ToString + "','" + ActiveItem.Title.Label + "','" + ActiveItem.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + "','" + Now.ToString("yyyy-MM-dd") + "');")) Then
                    labels.MagnetLabel(ActiveItem, PrinterName, printquantity)

                    '27/08/16
                    For i As Integer = 0 To printquantity
                        RegisterOrderAnalytic()
                    Next
                Else
                    Dim exString As String
                    Try
                        exString = "Local location name returned the following: " + SelectedSKU.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + vbNewLine + "Please tell IT."
                    Catch ex As Exception
                        exString = "Local location name broke - Please tell IT"
                    End Try
                    MsgBox("Something went wrong." + vbNewLine + exString)
                End If
            End If
        ElseIf PritingLabelID = 5 Then           'PPready
            If printquantity > 20 Then
                printquantity = 20
            End If
            If Not IsNothing(ActiveItem) Then
                '29/02/2016     Moved the logging code to before label generation so the batch code can be added. 
                If IsNumeric(WHLClasses.MySQL.insertUpdate("INSERT INTO whldata.log_prepack (UserId, UserFullName, WorkStationName, Time, PP_Sku, PP_Label, PP_Quantity, PP_ShortTitle, PP_Binrack, DateA) VALUES (" + logins.AuthenticatedUser.PayrollId.ToString + ",'" + logins.AuthenticatedUser.FullName + "','" + My.Computer.Name + "','" + Now.ToString("dd/MM/yyyy HH:mm") + "','" + ActiveItem.SKU + "','" + PritingLabelID.ToString + "','" + printquantity.ToString + "','" + ActiveItem.Title.Label + "','" + ActiveItem.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + "','" + Now.ToString("yyyy-MM-dd") + "');")) Then
                    labels.PPReadyLabel(ActiveItem, PrinterName, printquantity)

                    '27/08/16
                    For i As Integer = 0 To printquantity
                        RegisterOrderAnalytic()
                    Next
                Else
                    Dim exString As String
                    Try
                        exString = "Local location name returned the following: " + SelectedSKU.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + vbNewLine + "Please tell IT."
                    Catch ex As Exception
                        exString = "Local location name broke - Please tell IT"
                    End Try
                    MsgBox("Something went wrong." + vbNewLine + exString)
                End If
            End If
        ElseIf PritingLabelID = 4 Then           'Autobag
            If IsNothing(SelectedSKU) Then
                Dim EMsgBox As New WPFMsgBoxDialog

                EMsgBox.Body.Text = "You need to choose a packsize."
                EMsgBox.ShowDialog()
            Else
                Dim EMsgBox As New WPFMsgBoxDialog
                EMsgBox.DialogTitle.Text = "Autobag"
                EMsgBox.Body.Text = "Make sure you clear the previous job first."
                EMsgBox.ShowDialog()
                'It's all good. 
                If printquantity > 50 Then
                    printquantity = 50
                End If
                Try
                    '29/02/2016     Moved the logging code to before label generation so the batch code can be added. 
                    If IsNumeric(WHLClasses.MySQL.insertUpdate("INSERT INTO whldata.log_prepack (UserId, UserFullName, WorkStationName, Time, PP_Sku, PP_Label, PP_Quantity, PP_ShortTitle, PP_Binrack, DateA) VALUES (" + logins.AuthenticatedUser.PayrollId.ToString + ",'" + logins.AuthenticatedUser.FullName + "','" + My.Computer.Name + "','" + Now.ToString("dd/MM/yyyy HH:mm") + "','" + SelectedSKU.SKU + "','" + PritingLabelID.ToString + "','" + printquantity.ToString + "','" + SelectedSKU.Title.Label + "','" + SelectedSKU.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + "','" + Now.ToString("yyyy-MM-dd") + "');")) Then
                        Dim batch As String = WHLClasses.MySQL.SelectData("SELECT LAST_INSERT_ID();")(0)(0).ToString
                        labels.PrepackLabel(SelectedSKU, PrinterName, printquantity, batch)

                        '27/08/16
                        For i As Integer = 0 To printquantity
                            RegisterOrderAnalytic()
                        Next
                    Else
                        Dim exString As String
                        Try
                            exString = "Local location name returned the following: " + SelectedSKU.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + vbNewLine + "Please tell IT."
                        Catch ex As Exception
                            exString = "Local location name broke - Please tell IT"
                        End Try
                        MsgBox("Something went wrong." + vbNewLine + exString)
                    End If
                Catch ex As ArgumentException
                    Dim EMsgBox1 As New WPFMsgBoxDialog

                    EMsgBox1.Body.Text = "This Item doesn't appear to have a GS1. Please speak to IT"
                    EMsgBox1.ShowDialog()
                End Try
            End If
        End If

        ScanBox.Clear()
        ScanBox.Focus()
        'MySql.insertupdate("INSERT INTO whldata.log_prepack (UserId, UserFullName, WorkStationName, Time, PP_Sku, PP_Label, PP_Quantity, PP_ShortTitle, PP_Binrack, DateA) VALUES (" + logins.AuthenticatedUser.PayrollId.ToString + ",'" + logins.AuthenticatedUser.FullName + "','" + My.Computer.Name + "','" + Now.ToString("dd/MM/yyyy HH:mm") + "','" + ActiveItem.SKU + "','" + PritingLabelID.ToString + "','" + printquantity.ToString + "','" + ActiveItem.Title.Label + "','" + ActiveItem.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + "','" + Now.ToString("yyyy-MM-dd") + "');")
        ChooseLabel(1)

    End Sub
#Region "Choose Label"
    Private Sub Label5_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PrePackLabel.Click, PrepackLabelButton.Click
        ChooseLabel(1)
    End Sub
    Private Sub Label6_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ShelfLabelButton.Click, ShelfLabelBG.Click
        ChooseLabel(2)
    End Sub

    Private Sub Label7_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MagnetLabelButton.Click, MagnetLabelBG.Click
        ChooseLabel(3)
    End Sub

    Private Sub Label8_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AutobagButton.Click
        ChooseLabel(4)
    End Sub

    Private Sub PPReadyText_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PPReadyText.Click
        ChooseLabel(5)
    End Sub
#End Region
#Region "Application Startup Events"
    Private Sub Main_Activated(ByVal sender As Object, ByVal e As EventArgs) Handles Me.ContentRendered
        UpdateLoginInfo()
        ScanBox.Focus()

        Try
            If logins.AuthenticatedUser.Permissions.PrepackAdmin Then
                PrepackAdminPanel.Visibility = Visibility.Visible
            Else
                PrepackAdminPanel.Visibility = Visibility.Hidden
            End If
        Catch ex As Exception

        End Try


    End Sub

    Private Sub Main_FormClosing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        'e.Cancel = True
        logins.LogoutUser(RuntimeHelpers.GetObjectValue(sender))
        UnregisterAnalyticUser() '27/08/16
        'End
    End Sub

    Private Sub Main_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Loaded

        BarcodeSettings.ApplyKey("LJG05B1M4RS-FOTV9-JSHEF-CQOHO")
        Dim response As Object = WHLClasses.MySQL.SelectData("SELECT * FROM whldata.prepacklist")
        If response.GetType Is "".GetType Then
            Dim EMsgBox As New WPFMsgBoxDialog
            EMsgBox.Body.Text = "Couldn't load Prepack Info"
            EMsgBox.ShowDialog()

        Else
            PrepackInfo = response
        End If


        LoadTimers()
        Synthesizer = New SpeechSynthesizer
        Synthesizer.SetOutputToDefaultAudioDevice()

        Try
            client = Services.OrderServer.ConnectChannel("net.tcp://orderserver.ad.whitehinge.com:801/OrderServer/1/")
        Catch ex As Exception
            client = Nothing
        End Try


    End Sub

    Private Sub Main_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Loaded
        ScanBox.Focus()
        logins.Standalone = False
        logins.InitAuth()
        AddHandler logins.Shown, AddressOf UnregisterAnalyticUser

        Try
            logins.AppVerStr = "2.0.0.0"
        Catch exception1 As Exception
            logins.AppVerStr = "DEBUG"
        End Try
        ChooseLabel(1)
    End Sub
#End Region


    Private Sub Packsizes_SelectedIndexChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs) Handles Packsizes.SelectionChanged
        If Packsizes.SelectedItems.Count > 0 Then
            ResetDisp()
            'Choose the ActiveMini and set it. 
            For Each Child As WhlSKU In ActiveChildren
                If Child.PackSize.ToString = Packsizes.SelectedItem.ToString Then
                    SelectedSKU = Child
                    UpdateFromPacksize()
                End If
            Next
            ScanBox.Focus()
            GetHistory()
        End If

    End Sub
    Public Shared NoteInfoShared As String = ""

    Private Sub UpdateFromPacksize()
        'Updates packs-sensitive stuff from the child SKU. 
        'Firstly, go through the prepackinfo shitlist and find the info for this one. 
        For Each SKUList As ArrayList In PrepackInfo
            'I hate this shit so much. 
            If SKUList(0).ToString = SelectedSKU.SKU Then
                'This is the droid you're looking for. Loljk I haven't seen star wars. 
                NoteInfo.Text = SKUList(2).ToString
                Bag.Text = SKUList(1).ToString
                If Bag.Text.Length < 2 Or Bag.Text = "Never Bagged" Then
                    ChangeButton.RaiseEvent(New RoutedEventArgs(System.Windows.Controls.Button.ClickEvent, ChangeButton))
                End If
                If SelectedSKU.ExtendedProperties.HasScrews Then
                    Screws.Visibility = Visibility.Visible
                Else
                    Screws.Visibility = Visibility.Hidden
                End If

                If SelectedSKU.ExtendedProperties.IsPair Then
                    Pair.Visibility = Visibility.Visible
                Else

                    Pair.Visibility = Visibility.Hidden
                End If
                EnvelopeBox.Text = SelectedSKU.ExtendedProperties.Envelope

            End If
        Next
        'Now that's out the way, I don't actually think there's anything else to do other than print. Cool. 

    End Sub

    Private Sub ButtonLogout(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox1.MouseUp, UserName.MouseUp, UserTime.MouseUp, PictureBox1.TouchUp, UserName.TouchUp, UserTime.TouchUp
        logins.LogoutUser(sender)
        UnregisterAnalyticUser() '27/08/16
    End Sub


    Private Sub ResetDisp()
        Screws.Visibility = Visibility.Hidden
        Pair.Visibility = Visibility.Hidden
        Bag.Text = ""
        NoteInfo.Text = ""
        ScanBox.Text = ""
    End Sub

    Private Sub ScanBox_KeyDown(sender As Object, e As KeyEventArgs) Handles ScanBox.KeyDown
        If (e.Key = Key.Return) And logins.Visible = False Then
            e.Handled = True
            My.Computer.Audio.Stop()
            ExecuteSearch()
        End If
    End Sub

    Private Sub Shelf_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Shelf.Click
        Me.Close()
    End Sub

    Private Sub ShortSku_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ShortSku.Click
        If (Me.WindowState = WindowState.Normal) Then
            WindowState = WindowState.Maximized
        Else
            WindowState = WindowState.Normal
        End If
    End Sub
#Region "Analytics"
    Private Sub UpdateLoginInfo()
        Try
            UserName.Text = logins.AuthenticatedUser.FullName
            UserTime.Text = ("Logged in: " & logins.AuthdLoginTime.ToString)

            '27/08/16
            Try
                UnregisterAnalyticUser()
            Catch ex As Exception

            End Try
            RegisterAnalyticUser(logins.AuthenticatedUser)
            RegisterSession()
        Catch ex As Exception

        End Try
    End Sub

    '27/08/16
    Private Sub RegisterAnalyticUser(User As Employee)
        Dim Filename As String = "T:\AppData\Analytics\" + User.PayrollId.ToString + ".anal"
        If My.Computer.FileSystem.FileExists(Filename) Then
            CurrentEmployeeAnalytic = loader.LoadAnything(Filename).Value
        Else
            CurrentEmployeeAnalytic = New WarehouseAnalytics.AnalyticBase(User)
        End If
    End Sub
    '27/08/16
    Private Sub UnregisterAnalyticUser()
        UnregisterSession()
        If Not IsNothing(CurrentEmployeeAnalytic) Then
            CurrentEmployeeAnalytic.Save()
            CurrentEmployeeAnalytic = Nothing
        End If
    End Sub
    '27/08/16
    Private Sub RegisterSession()
        CurrentAnalyticSession = (New WarehouseAnalytics.SessionAnalytic(Nothing, WarehouseAnalytics.SessionType.Prepack, Nothing))
    End Sub
    '27/08/16
    Private Sub UnregisterSession()
        If Not CurrentAnalyticSession Is Nothing Then
            CurrentAnalyticSession.EndSession(CurrentEmployeeAnalytic)
            CurrentAnalyticSession = Nothing
        End If
    End Sub
    '27/08/16
    Private Sub RegisterOrderAnalytic()
        Try
            CurrentAnalyticSession.AddOrder()
        Catch ex As Exception
            If IsNothing(CurrentAnalyticSession) Then
                'Why aren't you here?
                RegisterSession()
                CurrentAnalyticSession.AddOrder()
            Else
                'No more errors on screen >:/
                WHLClasses.Reporting.ReportException(ex)
            End If
        End Try
    End Sub
#End Region

    'Private Sub UpdateScores()
    '    Dim enumerator As IEnumerator
    '    Dim enumerator2 As IEnumerator
    '    Dim enumerator3 As IEnumerator
    '    Dim enumerator4 As IEnumerator
    '    Me.LabelsNames.Text = ""
    '    Me.LabelsAmounts.Text = ""
    '    Dim left As Integer = 0
    '    Dim list As New ArrayList
    '    Dim list2 As New ArrayList
    '    Dim textArray1 As String() = New String() {"SELECT UserFullName, Sum(PP_Quantity) AS Amount FROM whldata.log_prepack WHERE DateA > '", DateAndTime.Now.AddDays(-7).ToString("yyyy-MM-dd"), "' AND DateA < '", DateAndTime.Now.AddDays(1).ToString("yyyy-MM-dd"), "' GROUP BY UserId ORDER BY Amount DESC LIMIT 10"}
    '    Try
    '        list = DirectCast(WHLClasses.MySQL.SelectData(String.Concat(textArray1)), ArrayList)

    '        Try
    '            enumerator = list.GetEnumerator
    '            Do While enumerator.MoveNext
    '                Dim current As ArrayList = DirectCast(enumerator.Current, ArrayList)
    '                Me.LabelsNames.Text = Me.LabelsNames.Text + current.Item(0) + vbNewLine
    '                Me.LabelsAmounts.Text = (Me.LabelsAmounts.Text & current.Item(1).ToString & ChrW(13) & ChrW(10))
    '                left = Conversions.ToInteger(Operators.AddObject(left, current.Item(1)))
    '            Loop
    '        Finally
    '            If TypeOf enumerator Is IDisposable Then
    '                TryCast(enumerator, IDisposable).Dispose()
    '            End If
    '        End Try
    '        Me.TotalLabelsLab.Text = Conversions.ToString(left)
    '        Me.SkusNames.Text = ""
    '        Me.SkusAmounts.Text = ""
    '        Dim num2 As Integer = 0
    '        Dim list3 As New ArrayList
    '        Dim list4 As New ArrayList
    '        Dim textArray2 As String() = New String() {"SELECT UserFullName, COUNT(PP_Sku) AS count FROM whldata.log_prepack WHERE DateA > '", DateAndTime.Now.AddDays(-7).ToString("yyyy-MM-dd"), "' AND DateA < '", DateAndTime.Now.AddDays(1).ToString("yyyy-MM-dd"), "' GROUP BY UserId ORDER BY count DESC LIMIT 10"}
    '        list3 = DirectCast(WHLClasses.MySQL.SelectData(String.Concat(textArray2)), ArrayList)
    '        Try
    '            enumerator2 = list3.GetEnumerator
    '            Do While enumerator2.MoveNext
    '                Dim list7 As ArrayList = DirectCast(enumerator2.Current, ArrayList)
    '                Me.SkusNames.Text = Conversions.ToString(Operators.AddObject(Operators.AddObject(Me.SkusNames.Text, list7.Item(0)), ChrW(13) & ChrW(10)))
    '                Me.SkusAmounts.Text = (Me.SkusAmounts.Text & list7.Item(1).ToString & ChrW(13) & ChrW(10))
    '                num2 = Conversions.ToInteger(Operators.AddObject(num2, list7.Item(1)))
    '            Loop
    '        Finally
    '            If TypeOf enumerator2 Is IDisposable Then
    '                TryCast(enumerator2, IDisposable).Dispose()
    '            End If
    '        End Try
    '        Me.SkusTotal.Text = Conversions.ToString(num2)
    '        Dim flag As Boolean = True
    '        Me.RecordDay.Text = ""
    '        Dim list5 As New ArrayList
    '        list5 = DirectCast(WHLClasses.MySQL.SelectData("SELECT SUM(PP_Quantity) as Recs, userFUllName, dateA FROM whldata.log_prepack  GROUP BY UserId, datea ORDER BY Recs ASC;"), ArrayList)
    '        Try
    '            enumerator3 = list5.GetEnumerator
    '            Do While enumerator3.MoveNext
    '                Dim list8 As ArrayList = DirectCast(enumerator3.Current, ArrayList)
    '                If (list8.Item(2).ToString.Length > 1) Then
    '                    Me.RecordDay.Text = Conversions.ToString(Operators.AddObject((list8.Item(0).ToString & " labels on" & ChrW(13) & ChrW(10) & list8.Item(2).ToString & " by" & ChrW(13) & ChrW(10)), list8.Item(1)))
    '                End If
    '            Loop
    '        Finally
    '            If TypeOf enumerator3 Is IDisposable Then
    '                TryCast(enumerator3, IDisposable).Dispose()
    '            End If
    '        End Try
    '        list5 = DirectCast(WHLClasses.MySQL.SelectData("SELECT SUM(PP_Quantity) as Recs, userFUllName, dateA FROM whldata.log_prepack  GROUP BY datea ORDER BY Recs DESC;"), ArrayList)
    '        Try
    '            enumerator4 = list5.GetEnumerator
    '            Do While enumerator4.MoveNext
    '                Dim list9 As ArrayList = DirectCast(enumerator4.Current, ArrayList)
    '                If ((list9.Item(2).ToString.Length > 1) AndAlso flag) Then
    '                    Dim textArray3 As String() = New String() {Me.RecordDay.Text, ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10), list9.Item(0).ToString, " total on" & ChrW(13) & ChrW(10), list9.Item(2).ToString}
    '                    Me.RecordDay.Text = String.Concat(textArray3)
    '                    flag = False
    '                End If
    '            Loop
    '        Finally
    '            If TypeOf enumerator4 Is IDisposable Then
    '                TryCast(enumerator4, IDisposable).Dispose()
    '            End If
    '        End Try
    '    Catch ex As Exception
    '        'Something failed 

    '    End Try
    'End Sub

    'Private Sub UpdateSpecialValues()
    '    Me.Codes.Text = (Me.PrimarysuppnameTextBox.Text.ToUpper & " - " & Me.PrimarysuppcodeTextBox.Text)
    '    If (Me.AltsuppnameTextBox.TextLength > 0) Then
    '        Dim textArray1 As String() = New String() {Me.Codes.Text, ", ", Me.AltsuppnameTextBox.Text.ToUpper, " - ", Me.AltsuppcodeTextBox.Text}
    '        Me.Codes.Text = String.Concat(textArray1)
    '    End If
    '    If (Me.Supp3nameTextBox.TextLength > 0) Then
    '        Dim textArray2 As String() = New String() {Me.Codes.Text, ", ", Me.Supp3nameTextBox.Text.ToUpper, " - ", Me.Supp3codeTextBox.Text}
    '        Me.Codes.Text = String.Concat(textArray2)
    '    End If
    'End Sub

    Private Sub WhlnewBindingSource_ListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
        'Me.UpdateSpecialValues()
    End Sub

    Private Sub ChooseLabel(ByVal labelid As Integer)
        If (labelid = 1) Then
            If Not HasAutobagHistory Then
                PrePackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
                ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                AutobagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                PritingLabelID = 1
                pritinglabel = "Prepack"
            End If
        ElseIf (labelid = 2) Then
            If Not HasAutobagHistory Then
                PrePackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
                MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                AutobagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                PritingLabelID = 2
                pritinglabel = "Shelf"
            End If
        ElseIf (labelid = 3) Then
            If Not HasAutobagHistory Then
                PrepackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
                AutoBagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                PritingLabelID = 3
                pritinglabel = "Magnet"
            End If
        ElseIf (labelid = 5) Then
            If Not HasAutobagHistory Then
                PrepackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                AutoBagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
                PritingLabelID = 5
                pritinglabel = "PPReady"
            End If
        ElseIf (labelid = 4) Then
            PrepackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
            ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
            MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
            AutoBagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
            PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
            PritingLabelID = 4
            pritinglabel = "Autobag"
            HasAutobagHistory = True

            PrePackHighlight.Visibility = Visibility.Hidden
            ShelfLabelHighlight.Visibility = Visibility.Hidden
            MagnetLabelHighlight.Visibility = Visibility.Hidden
            PPReadyHighlight.Visibility = Visibility.Hidden
        End If
    End Sub

    Private Sub Clock_Tick(ByVal sender As Object, ByVal e As EventArgs)
        TimeBox.Text = DateAndTime.Now.ToLongTimeString
    End Sub

    Private Sub DataRefresher_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            PrepackInfo = WHLClasses.MySQL.SelectData("SELECT * FROM whldata.prepacklist")
        Catch exception1 As Exception

        End Try

    End Sub


    Friend ActiveItem As WhlSKU
    Friend ActiveChildren As SkuCollection
    Private Sub ProcessSearch(Data As String)

        If Data.StartsWith("qzu") Then
            logins.replaceUser(Data)
        ElseIf (Data.Length < 2) Then


        ElseIf (Data.Length > 2) Then
            If Data.StartsWith("10") And Data.Length = 11 Then
                Data = Data.Remove(7)
            End If
            NewScan = True
            '28/01/2016     Swapped out the old raw code for the class host searching methods. 

            Try
                Dim SearchResults As SkuCollection = Skusfull.ExcludeStatus("Dead").SearchSKUS(Data)
                If SearchResults.Count > 1 Then

                    Dim AllSame As Boolean = True
                    Dim SameVal As String = SearchResults(0).ShortSku
                    For Each SearchRes As WhlSKU In SearchResults
                        If SearchRes.ShortSku = SameVal Then
                        Else
                            AllSame = False
                        End If

                    Next
                    If AllSame = True Then
                        'Continue!
                        ActiveItem = SearchResults(0)
                        PopulateData()
                        Try
                            For Each Location As SKULocation In ActiveItem.GetLocationsByType(SKULocation.SKULocationType.PrepackInstant)
                                Try
                                    ActiveItem.RemoveLocation(Location.LocationID, logins.AuthenticatedUser)
                                Catch ex As Exception
                                End Try

                            Next
                        Catch ex As Exception
                            Dim EMsgBox As New WPFMsgBoxDialog

                            EMsgBox.Body.Text = ex.Message.ToString
                            EMsgBox.ShowDialog()
                        End Try
                    Else
                        'Got a choice here kid. 
                        Synthesizer.SpeakAsync("Choose the correct item from the list.")
                        'BundleDialog.bundleoptionsal = SearchResults
                        'BundleDialog.ShowDialog()
                        'ActiveItem = BundleDialog.chosenshsku
                        PopulateData()
                        Try
                            For Each Location As SKULocation In ActiveItem.GetLocationsByType(SKULocation.SKULocationType.PrepackInstant)
                                Try
                                    ActiveItem.RemoveLocation(Location.LocationID, logins.AuthenticatedUser)
                                Catch ex As Exception
                                End Try

                            Next
                        Catch ex As Exception
                            Dim EMsgBox As New WPFMsgBoxDialog

                            EMsgBox.Body.Text = ex.Message.ToString
                            EMsgBox.ShowDialog()
                        End Try
                    End If

                ElseIf SearchResults.Count = 0 Then
                    'No results. RIP. 
                    Synthesizer.SpeakAsync("Nothing found.")
                    ResetDisp()
                Else
                    'Continue!
                    ActiveItem = SearchResults(0)
                    PopulateData()
                    Try
                        For Each Location As SKULocation In ActiveItem.GetLocationsByType(SKULocation.SKULocationType.PrepackInstant)
                            Try
                                ActiveItem.RemoveLocation(Location.LocationID, logins.AuthenticatedUser)
                            Catch ex As Exception
                            End Try

                        Next
                    Catch ex As Exception
                        MsgBox(ex.Message.ToString)
                    End Try

                End If
            Catch ex As Exception
                Dim EMsgBox As New WPFMsgBoxDialog

                EMsgBox.Body.Text = "Couldn't search because the system is too busy, please try again in a few minutes."
                EMsgBox.ShowDialog()

            End Try


        Else
            Dim EMsgBox As New WPFMsgBoxDialog

            EMsgBox.Body.Text = "We couldn't recognise that barcode, Please try again."
            EMsgBox.ShowDialog()
        End If
        ScanBox.Text = ""
        ScanBox.Focus()
    End Sub
    Private Sub ExecuteSearch()
        Dim text As String = ScanBox.Text
        If text.StartsWith("qzu") Then
            logins.replaceUser(text)
        ElseIf (text.Length > 0) Then
            If text.StartsWith("10") And text.Length = 11 Then
                text = text.Remove(7)
            End If
            NewScan = True
            '28/01/2016     Swapped out the old raw code for the class host searching methods. 

            Try
                Dim SearchResults As SkuCollection = Skusfull.ExcludeStatus("Dead").SearchSKUS(text)
                If SearchResults.Count > 1 Then

                    Dim AllSame As Boolean = True
                    Dim SameVal As String = SearchResults(0).ShortSku
                    For Each SearchRes As WhlSKU In SearchResults
                        If SearchRes.ShortSku = SameVal Then
                        Else
                            AllSame = False
                        End If

                    Next
                    If AllSame = True Then
                        'Continue!
                        ActiveItem = SearchResults(0)
                        PopulateData()
                        Try
                            For Each Location As SKULocation In ActiveItem.GetLocationsByType(SKULocation.SKULocationType.PrepackInstant)
                                Try
                                    'ActiveItem.RemoveLocation(Location.LocationID, logins.AuthenticatedUser)
                                Catch ex As Exception
                                End Try

                            Next
                        Catch ex As Exception
                            MsgBox(ex.Message.ToString)
                        End Try
                    Else
                        'Got a choice here kid. 
                        Synthesizer.SpeakAsync("Choose the correct item from the list.")
                        'BundleDialog.bundleoptionsal = SearchResults
                        'BundleDialog.ShowDialog()
                        'ActiveItem = BundleDialog.chosenshsku
                        PopulateData()
                        Try
                            For Each Location As SKULocation In ActiveItem.GetLocationsByType(SKULocation.SKULocationType.PrepackInstant)
                                Try
                                    'ActiveItem.RemoveLocation(Location.LocationID, logins.AuthenticatedUser)
                                Catch ex As Exception
                                End Try

                            Next
                        Catch ex As Exception
                            MsgBox(ex.Message.ToString)
                        End Try
                    End If

                ElseIf SearchResults.Count = 0 Then
                    'No results. RIP. 
                    Synthesizer.SpeakAsync("Nothing found.")
                    ResetDisp()
                Else
                    'Continue!
                    ActiveItem = SearchResults(0)
                    PopulateData()
                    Try
                        For Each Location As SKULocation In ActiveItem.GetLocationsByType(SKULocation.SKULocationType.PrepackInstant)
                            Try
                                'ActiveItem.RemoveLocation(Location.LocationID, logins.AuthenticatedUser)
                            Catch ex As Exception
                            End Try

                        Next
                    Catch ex As Exception
                        MsgBox(ex.Message.ToString)
                    End Try

                End If
            Catch ex As Exception
                Dim EMsgBox As New WPFMsgBoxDialog

                EMsgBox.Body.Text = "Couldn't search because the system is too busy, please try again in a few minutes."
                EMsgBox.ShowDialog()

            End Try


        Else
            Dim EMsgBox As New WPFMsgBoxDialog

            EMsgBox.Body.Text = "We couldn't recognise that barcode, Please try again."
            EMsgBox.ShowDialog()
        End If
        ScanBox.Text = ""
        ScanBox.Focus()

    End Sub
    Public Shared BagShared As String = ""
    Private Sub PopulateData()
        For Each Location As SKULocation In ActiveItem.Locations
            If Location.LocationType = 2 Then
                LocLbl.Text = LocLbl.Text + Location.LocalLocationName

            End If

        Next
        'This is where we display all the information we have. 
        LabelShortTitle.Text = ActiveItem.Title.Label
        Shelf.Content = ActiveItem.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName
        ShortSku.Content = ActiveItem.ShortSku

        If ActiveItem.isBundle Then
            BundleDetailbutton.Visibility = Visibility.Visible
        Else
            BundleDetailbutton.Visibility = Visibility.Hidden
        End If

        '22/03/2016     Added minimum and level to stock.   
        '29/02/2016     Added stock level (total) to the screen.
        Stock.Text = ActiveItem.Stock.Total.ToString + "(" + ActiveItem.Stock.Level.ToString + " + " + ActiveItem.Stock.Minimum.ToString + " min.)"

        Packsizes.Items.Clear()
        SalesInfo.Items.Clear()
        'Find the children and fill those in the packs list.
        ActiveChildren = Skusfull.GatherChildren(ActiveItem.ShortSku)
        For Each Child As WhlSKU In ActiveChildren
            Try
                If Child.NewItem.IsListed Then
                    Packsizes.Items.Add(Child.PackSize.ToString)
                    SalesInfo.Items.Add(Child.SalesData.WeightedAverage.ToString)

                    '06/05/2016     Checks to see if the prepackbag is none or a blank string
                    If Child.PrepackInfo.Bag = Nothing Or Child.PrepackInfo.Bag = "NEVER BAGGED" Or Child.PrepackInfo.Bag.Length < 2 Then
                        Dim BagNoteEditorNew As New BagNoteEditor
                        BagNoteEditorNew.ActiveSku = Child
                        BagNoteEditorNew.ShowDialog()
                        NoteInfo.Text = NoteInfoShared
                        Bag.Text = BagShared

                    End If
                End If
            Catch ex As Exception
                Reporting.ReportException(ex, False)
                Dim EMsgBox As New WPFMsgBoxDialog
                EMsgBox.Body.Text = "The system was unable to display data for the pack of " + Child.PackSize.ToString
                EMsgBox.ShowDialog()

            End Try
        Next
        'If ActiveChildren.Count = 1 Then       '19/04/16 - Change made after "Value of 0" error.

        '13/05/2016     Added scanning to prepack queue
        ScanToPrepackQueue(ActiveItem.SKU)

        If Packsizes.Items.Count = 1 Then
            Packsizes.SelectedIndex = 0
        Else
            EnvelopeBox.Text = ""
            Screws.Text = ""


        End If
    End Sub

    Private Sub BundleDetailbutton_Click(sender As Object, e As EventArgs) Handles BundleDetailbutton.Click
        If Not IsNothing(ActiveItem) Then
            Dim itemCompString As String = ""
            For Each item As WhlSKU In ActiveItem.Composition
                itemCompString += item.GetLocation(SKULocation.SKULocationType.Pickable).LocationText + " - " + item.Title.Label + "." + vbNewLine
            Next
            Dim EMsgBox As New WPFMsgBoxDialog
            EMsgBox.Body.Text = itemCompString
            EMsgBox.DialogTitle.Text = "Items in Bundle:"
            EMsgBox.ShowDialog()

        End If
    End Sub

    Private Sub GetHistory()
        Historylabel.Text = ("SKU Prepack History (" & SelectedSKU.SKU & ")")
        Dim list As New ArrayList
        list = WHLClasses.MySQL.SelectData("Select UserFullName, Sum(PP_Quantity) As Amount, DateA FROM whldata.log_prepack WHERE PP_Sku='" & SelectedSKU.SKU.ToString & "' GROUP BY UserId, DateA ORDER BY DateA DESC LIMIT 10;")
        If (list.Count = 0) Then
            HistoryBody.Text = "No history found for this SKU."
        Else
            Dim enumerator As IEnumerator
            HistoryBody.Text = ""
            Try
                enumerator = list.GetEnumerator
                Do While enumerator.MoveNext
                    Dim current As ArrayList = DirectCast(enumerator.Current, ArrayList)
                    If (current.Item(2).ToString.Length = 0) Then
                        Dim textArray1 As String = HistoryBody.Text + "Between 16/10 and 22/10: " + current.Item(0).ToString + " packed " + current.Item(1).ToString + " packs." + vbNewLine
                        HistoryBody.Text = textArray1
                    Else
                        Dim textArray2 As String = HistoryBody.Text + current.Item(2).ToString + ": " + current.Item(0).ToString + " packed " + current.Item(1).ToString + " packs." + vbNewLine
                        HistoryBody.Text = textArray2
                    End If
                Loop
            Finally
                If TypeOf enumerator Is IDisposable Then
                    TryCast(enumerator, IDisposable).Dispose()
                End If
            End Try
            HistoryBody.Text = (HistoryBody.Text & "Anything done before 16/10/2015 has not been recorded and will not show. ")
        End If
    End Sub

    Private Sub ImageButton1_BtnClick(ByVal sender As Object, ByVal e As EventArgs) Handles ChangeButton.Click
        Dim Dialog As New BagNoteEditor

        If Not IsNothing(SelectedSKU) Then
            Dialog.ActiveSku = SelectedSKU
            Dialog.ShowDialog()


            ScanBox.Focus()
        End If

    End Sub


    Private Sub KeypadPress(sender As Button, e As EventArgs) Handles KeyClear.Click, Key8.Click, Key7.Click, Key6.Click, Key5.Click, Key4.Click, Key3.Click, Key2.Click, Key1.Click, key0.Click, AutobagBG.Click
        Keypad(sender.Content.ToString)
        ScanBox.Focus()
    End Sub

    Dim SkusFullLoaded As Boolean = False
    Private Sub SkuCacheInitLoader_Tick(sender As Object, e As EventArgs)
        SkuCacheINitLoader.Stop()
        SkusFullLoaded = False


        Dim Loader2 As New GenericDataController
        Try

            Skusfull = Loader2.SmartSkuCollLoad(True, "", False)
            Skus = Skusfull.MakeMixdown

        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try

    End Sub

    Dim ForceUpdate As Boolean = False

    'Dim skuRefreshTime As New DateTime
    Private Sub CoolButton1_Click(sender As Object, e As EventArgs) Handles AdminUpdate.Click
        ForceUpdate = True
        SkusFullLoaded = False
        SkuCacheINitLoader.Start()
    End Sub

    Private Sub AdminStatusUpdater_Tick(sender As Object, e As EventArgs)
        Try
            If Skusfull.RefreshStatus.Length < 2 Then
                AdminStatus.Content = "RStatus- " + Skusfull.CountDone
            Else
                AdminStatus.Content = "Status - " + Skusfull.RefreshStatus
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub Main_FormClosed(sender As Object, e As EventArgs) Handles MyBase.Closed
        Skusfull.CAncelThreadedWorker()
        SkusFullLoaded = False
        logins.HidePanel()

        Skusfull.DisposeThreadedWorker()
        Dim Loader As New WHLClasses.SkusDataController
        Loader.SaveData(Skusfull)
    End Sub

    Private Sub RefreshCurrent_Click(sender As Object, e As EventArgs) Handles RefreshCurrent.Click
        For Each item As WhlSKU In ActiveChildren
            item.FullRefresh()
        Next
    End Sub

    Private Sub KillDataRefresher_Click(sender As Object, e As EventArgs) Handles KillDataRefresher.Click
        DataRefresher.Stop()
    End Sub

    Private Sub CoolButton1_Click_1(sender As Object, e As EventArgs) Handles ActivePPQCompleteButton.Click
        '03/06/2016     Modified to implement the listyness of the highlightedorder. Basically just wrapped in a for loop ayylamow


        If Not IsNothing(client) Then
            PrepackQueueBase = client.StreamIOOrderDefinition.GetByStatus(OrderStatus._Prepack)
        Else
            PrepackQueueBase = loader.LoadOrddef("T:\AppData\Orders\io.orddef", False, True).GetByStatus(OrderStatus._Prepack)
        End If
        Try
            For Each order As Order In PrepackQueueBase

                For Each issue As IssueData In order.issues
                    If (Not issue.Resolved) And (Not issue.Prepack_IssuePartlyResolveFor) Then
                        If issue.DodgySku = HighlightedOrderItems.DodgySku Then
                            Try
                                Dim ResponseInsert As Object
                                ResponseInsert = WHLClasses.MySQL.insertUpdate("INSERT INTO whldata.prepack_test (orderid,iscomplete,pplocationname,DateCleared) VALUES ('" + order.OrderId.ToString + "', '1' , '" + System.Environment.MachineName.ToString + "','" + Now.ToShortDateString + "')")

                                For Each Location As SKULocation In ActiveItem.GetLocationsByType(SKULocation.SKULocationType.PrepackInstant)
                                    Try
                                        ActiveItem.RemoveLocation(Location.LocationID, logins.AuthenticatedUser)
                                    Catch ex As Exception
                                    End Try

                                Next
                            Catch ex As Exception
                                MsgBox(ex.Message.ToString)
                            End Try
                        End If
                    End If
                Next
            Next

        Catch ex As Exception

        End Try


        ActivePPQTitle.Text = "-------: Not on order"
        ActivePPQLabel1.Text = "-"
        ActivePPQLabel2.Text = "-"
        ActivePPQCompleteButton.Visibility = Visibility.Hidden


        UpdatePrepackOrders_Tick()
        ScanBox.Text = ""
        ScanBox.Focus()
    End Sub

    'Stupid Classes yay.
    Public Class IssueDataAndOrderCollection
        Inherits List(Of IssueDataAndOrder)

    End Class
    Public Class IssueDataAndOrder
        Public Order As Order
        Public Issue As IssueData

        Public Sub New(Aorder As Order, Aissue As IssueData)
            Order = Aorder
            Issue = Aissue
        End Sub
    End Class

    '10/03/2016     Adding the prepack queue
    Dim PrepackQueueBase As OrderDefinition

    Dim PrePackQueue As New IssueDataAndOrderCollection

    Dim loader As New GenericDataController


    Dim workerAttempt As Integer = 0
    Dim workerWorked As Boolean = True
    Dim workerEx As New Exception

    '10/03/2016     Updates the PPQ, and tells it to populate. Will be force run when a PPQ item is "Completed"
    Private Sub UpdatePrepackOrders_Tick()

        UpdatePrepackOrders.Start()
        If Not PrepackQueueWorker.IsBusy Then


            PrepackQueueWorker.RunWorkerAsync()
            workerAttempt = 0
        Else
            'It's busy...
        End If
        'End If
    End Sub

    Private Sub PPQWorkerWork()
        Try
            workerWorked = True
            PrepackQueueBase = loader.LoadOrddef("T:\AppData\Orders\io.orddef", False, True).GetByStatus(OrderStatus._Prepack)
            PrePackQueue.Clear()
            '03/06/2016     We need to filter the orders from the PrepackQueueBase into the new dictionary which supports issues.
            For Each order As Order In PrepackQueueBase
                For Each issue As IssueData In order.issues
                    If (Not issue.Resolved) And (Not issue.Prepack_IssuePartlyResolveFor) Then
                        PrePackQueue.Add(New IssueDataAndOrder(order, issue))
                    End If
                Next
            Next
        Catch ex As Exception
            workerWorked = False
            workerEx = ex
        End Try

        PrepackQueueWorker.ReportProgress(0)
    End Sub

    Private Sub displayError()

    End Sub

    Private Sub PPQWorkerReport()
        If workerWorked Then

        Else
            displayError()
        End If
        If UpdatePrepackOrders.IsEnabled = False Then
            UpdatePrepackOrders.Start()
        End If
    End Sub

    '03/06/2016     Changed these to lists o we can smash them all if they're the same item. 
    Dim HighlightedOrders As Order
    Dim HighlightedOrderItems As IssueData


    Private Sub ScanToPrepackQueue(Sku As String)
        HighlightedOrders = Nothing
        HighlightedOrderItems = Nothing
        PrePackQueue.Clear()
        If Not IsNothing(client) Then
            PrepackQueueBase = client.StreamIOOrderDefinition.GetByStatus(OrderStatus._Prepack)
        Else
            PrepackQueueBase = loader.LoadOrddef("T:\AppData\Orders\io.orddef", False, True).GetByStatus(OrderStatus._Prepack)
        End If
        For Each order As Order In PrepackQueueBase
            For Each issue As IssueData In order.issues
                If (Not issue.Resolved) And (Not issue.Prepack_IssuePartlyResolveFor) Then
                    PrePackQueue.Add(New IssueDataAndOrder(order, issue))
                End If
            Next
        Next
        '03/06/2016     What the fuck is changed? Why did I make this? Also added the list clearing jsut above.
        Dim Changed As Boolean = False
        For Each order As IssueDataAndOrder In PrePackQueue
            For Each item As IssueData In order.Order.issues
                If item.DodgySku.Substring(0, 7) = Sku.Substring(0, 7) Then 'The question here is what happens if an order for two pack sizes of the same item goes to prepack...
                    Changed = True
                    HighlightedOrders = order.Order
                    HighlightedOrderItems = order.Issue
                End If
            Next
        Next
        If Changed Then
            Dim HighlightedSku As WhlSKU = Skusfull.SearchSKUS(HighlightedOrderItems.DodgySku)(0)
            ActivePPQTitle.Text = HighlightedOrders.OrderId + ": " + HighlightedSku.PackSize.ToString + " pack " + HighlightedSku.Title.Label
            ActivePPQLabel1.Text = "Sent by: " + Emps.FindEmployeeByID(HighlightedOrders.StateUser).FullName
            ActivePPQLabel2.Text = "Ordered: " + loader.LoadOrdex(HighlightedOrders.Filename).LinnOpenOrder.GeneralInfo.ReceivedDate.ToString
            ActivePPQCompleteButton.Visibility = Visibility.Visible

        Else
            ActivePPQTitle.Text = "-------: Not on order"
            ActivePPQLabel1.Text = "-"
            ActivePPQLabel2.Text = "-"
            ActivePPQCompleteButton.Visibility = Visibility.Hidden
        End If
    End Sub


    'WPF Stuff
    Public Sub PerformClick(Button As System.Windows.Controls.Button)
        Button.RaiseEvent(New RoutedEventArgs(System.Windows.Controls.Button.ClickEvent))
    End Sub

    Friend DataRefresher As New System.Windows.Threading.DispatcherTimer
    Friend Clock As New System.Windows.Threading.DispatcherTimer
    Friend SkuCacheINitLoader As New System.Windows.Threading.DispatcherTimer
    Friend AdminStatusUpdater As New System.Windows.Threading.DispatcherTimer
    Friend UpdatePrepackOrders As New System.Windows.Threading.DispatcherTimer
    Friend Sub LoadTimers()

        AddHandler DataRefresher.Tick, AddressOf DataRefresher_Tick
        AddHandler SkuCacheINitLoader.Tick, AddressOf SkuCacheInitLoader_Tick
        AddHandler Clock.Tick, AddressOf Clock_Tick
        AddHandler AdminStatusUpdater.Tick, AddressOf AdminStatusUpdater_Tick
        AddHandler UpdatePrepackOrders.Tick, AddressOf UpdatePrepackOrders_Tick

        DataRefresher.Interval = New TimeSpan(0, 0, 0, 0, 5000)
        Clock.Interval = New TimeSpan(0, 0, 0, 0, 100)
        SkuCacheINitLoader.Interval = New TimeSpan(0, 0, 0, 0, 100)
        AdminStatusUpdater.Interval = New TimeSpan(0, 0, 0, 0, 50)
        UpdatePrepackOrders.Interval = New TimeSpan(0, 0, 0, 0, 15000)
        DataRefresher.Start()
        Clock.Start()
        SkuCacheINitLoader.Start()
        AdminStatusUpdater.Start()
        UpdatePrepackOrders.Start()
    End Sub

End Class


