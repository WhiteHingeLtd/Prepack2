﻿Imports LoginModule
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices

Imports Spire.Barcode
Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Data
Imports System.Deployment.Internal
Imports System.Deployment.Application
Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Speech.Synthesis
Imports System.Windows.Threading
Imports System.Windows.Forms
Imports System.IO
Imports WHLClasses
Imports WHLClasses.Orders
Imports LinnworksAPI

Class MainWindow
    Public bundlepacks As String = ""
    Private CheckUpdate As Object = True
    Private HasAutobagHistory As Boolean = False
    Public logins As FullscreenLogin = New FullscreenLogin
    Private NewScan As Boolean = False
    Public NoBox As Boolean = False
    Public PrepackInfo As ArrayList = New ArrayList
    Public PrinterName As String = ""
    Private pritinglabel As String
    Private PritingLabelID As Integer = 1
    Public SelectedSKU As WhlSKU
    Private Synthesizer As SpeechSynthesizer
    Private Emps As New EmployeeCollection
    Friend PrepackQueueWorker As New BackgroundWorker

    Friend Skusfull As New WHLClasses.SkuCollection(True)
    Private Skus As New WHLClasses.SkuCollection(True)
    Private labels As New WHLClasses.LabelMaker

    '27/08/16
    Dim CurrentEmployeeAnalytic As WarehouseAnalytics.AnalyticBase
    Dim CurrentAnalyticSession As WarehouseAnalytics.SessionAnalytic

    Private Sub Keypad(ByVal Key As String)
        If (Key = "Clear") Then
            Keypaddisp.Text = ""
        Else
            Keypaddisp.Text += Key
        End If
        ScanFocusBox.Focus()
    End Sub

    Private Sub keyPrint_BtnClick(ByVal sender As Object, ByVal e As EventArgs) Handles keyPrint.Click

        'Print quantity. 
        Dim printquantity As Integer = 1
        Dim CheckLength As String = Keypaddisp.Text
        If CheckLength.Length > 0 Then
            printquantity = Convert.ToInt32(Keypaddisp.Text)
        End If


        ' COMPLETELY REDO PRINTING CODE TO USE LABELGENERATOR
        If Me.PritingLabelID = 1 Then               'Prepack
            If IsNothing(SelectedSKU) Then
                EMsgbox("You need to choose a packsize.", Nothing, Nothing)
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
                    EMsgbox("You can't print a label for this pack. It doesn't seem to have a GS1. Speak to whoever is responsible to fix this. ", Nothing, Nothing)
                End Try

            End If
        ElseIf Me.PritingLabelID = 2 Then          'Shelf
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
        ElseIf Me.PritingLabelID = 3 Then           'Magnet
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
        ElseIf Me.PritingLabelID = 5 Then           'PPready
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
        ElseIf Me.PritingLabelID = 4 Then           'Autobag
            If IsNothing(SelectedSKU) Then
                EMsgbox("You need to choose a packsize.", Nothing, Nothing)
            Else
                EMsgbox("Make sure you clear the previous job first!", MsgBoxStyle.ApplicationModal, "Autobag")
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
                    EMsgbox("You can't print a label for this pack. It doesn't seem to have a GS1. Speak to whoever is responsible to fix this. ", Nothing, Nothing)
                End Try
            End If
        End If

        Keypaddisp.Clear()
        ScanFocusBox.Focus()
        'MySql.insertupdate("INSERT INTO whldata.log_prepack (UserId, UserFullName, WorkStationName, Time, PP_Sku, PP_Label, PP_Quantity, PP_ShortTitle, PP_Binrack, DateA) VALUES (" + logins.AuthenticatedUser.PayrollId.ToString + ",'" + logins.AuthenticatedUser.FullName + "','" + My.Computer.Name + "','" + Now.ToString("dd/MM/yyyy HH:mm") + "','" + ActiveItem.SKU + "','" + PritingLabelID.ToString + "','" + printquantity.ToString + "','" + ActiveItem.Title.Label + "','" + ActiveItem.GetLocation(SKULocation.SKULocationType.Pickable).LocalLocationName + "','" + Now.ToString("yyyy-MM-dd") + "');")
        ChooseLabel(1)

    End Sub

    Private Sub Label5_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PrePackLabel.Click, PrepackLabelButton.Click
        Me.ChooseLabel(1)
    End Sub
    Private Sub Label6_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ShelfLabelButton.Click, ShelfLabelBG.Click
        Me.ChooseLabel(2)
    End Sub

    Private Sub Label7_Click(ByVal sender As Object, ByVal e As EventArgs) Handles MagnetLabelButton.Click, MagnetLabelBG.Click
        Me.ChooseLabel(3)
    End Sub

    Private Sub Label8_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AutobagButton.Click
        Me.ChooseLabel(4)
    End Sub

    Private Sub PPReadyText_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PPReadyText.Click
        Me.ChooseLabel(5)
    End Sub

    Private Sub Main_Activated(ByVal sender As Object, ByVal e As EventArgs) Handles Me.ContentRendered
        UpdateLoginInfo()
        ScanFocusBox.Focus()

        Try
            If logins.AuthenticatedUser.Permissions.PrepackAdmin Then
                PrepackAdminPanel.Visibility = True
            Else
                PrepackAdminPanel.Visibility = False
            End If
        Catch ex As Exception

        End Try


    End Sub

    Private Sub Main_FormClosing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        'e.Cancel = True
        Me.logins.LogoutUser(RuntimeHelpers.GetObjectValue(sender))
        UnregisterAnalyticUser() '27/08/16
        'End
    End Sub

    Private Sub Main_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Loaded

        BarcodeSettings.ApplyKey("LJG05B1M4RS-FOTV9-JSHEF-CQOHO")
        Dim response As Object = WHLClasses.MySQL.SelectData("SELECT * FROM whldata.prepacklist")
        If response.GetType Is "".GetType Then
            EMsgbox("Couldn't load prepack info. ", Nothing, Nothing)
        Else
            Me.PrepackInfo = response
        End If


        LoadTimers()
        Me.Synthesizer = New SpeechSynthesizer
        Me.Synthesizer.SetOutputToDefaultAudioDevice()


    End Sub

    Private Sub Main_Shown(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Activated
        ScanFocusBox.Focus()
        Me.logins.Standalone = False
        Me.logins.InitAuth()
        AddHandler logins.Shown, AddressOf UnregisterAnalyticUser
        Dim enumerator As IEnumerator
        Try
            enumerator = PrinterSettings.InstalledPrinters.GetEnumerator
            Do While enumerator.MoveNext
                Dim str As String = Conversions.ToString(enumerator.Current)
                Dim printerbutton As New Printerbutton With {
                    .PrinterText = str
                }
                PrinterButtons.Children.Add(printerbutton)
                PrinterName = str
            Loop
        Finally
            If TypeOf enumerator Is IDisposable Then
                TryCast(enumerator, IDisposable).Dispose()
            End If
        End Try

        Try
            Me.logins.AppVerStr = "2.0.0.0"
        Catch exception1 As Exception
            ProjectData.SetProjectError(exception1)
            Dim exception As Exception = exception1
            Me.logins.AppVerStr = "DEBUG"
            ProjectData.ClearProjectError()
        End Try
        MyBase.WindowState = FormWindowState.Maximized
        Me.ChooseLabel(1)
    End Sub

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
            ScanFocusBox.Focus()
            Me.GetHistory()
        End If

    End Sub

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
                Screws.Visibility = SelectedSKU.ExtendedProperties.HasScrews
                Pair.Visibility = SelectedSKU.ExtendedProperties.IsPair
                EnvelopeBox.Text = SelectedSKU.ExtendedProperties.Envelope
            End If
        Next
        'Now that's out the way, I don't actually think there's anything else to do other than print. Cool. 

    End Sub

    Private Sub ButtonLogout(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox1.MouseUp, UserName.MouseUp, UserTime.MouseUp, PictureBox1.TouchUp, UserName.TouchUp, UserTime.TouchUp
        Me.logins.LogoutUser(sender)
        UnregisterAnalyticUser() '27/08/16
    End Sub

    Private Sub FocusScanBox(ByVal sender As Object, ByVal e As EventArgs)
        ScanFocusBox.Focus()
    End Sub

    Private Sub ResetDisp()
        Me.Screws.Visibility = False
        Me.Pair.Visibility = False
        Me.Bag.Text = ""
        Me.NoteInfo.Text = ""
        Me.keypaddisp.Text = ""
    End Sub

    Public Sub ScanFocusBox_KeyDown(sender As Object, e As KeyEventArgs)
        If (e.KeyCode = Keys.Enter) And logins.Visible = False Then
            e.SuppressKeyPress = True
            My.Computer.Audio.Stop()
            Me.ExecuteSearch()
        End If
    End Sub

    Private Sub Shelf_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Shelf.Click
        Me.Close()
    End Sub

    Private Sub ShortSku_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ShortSku.Click
        If (MyBase.WindowState = FormWindowState.Normal) Then
            MyBase.WindowState = FormWindowState.Maximized
        Else
            MyBase.WindowState = FormWindowState.Normal
        End If
    End Sub

    'Private Sub ShortSku_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ShortSku.
    '    Me.Packsizes.Items.Clear()
    '    Me.SalesInfo.Items.Clear()

    '    ScanFocusBox.Focus()

    'End Sub

    Private Sub UpdateLoginInfo()
        Try
            Me.UserName.Text = Me.logins.AuthenticatedUser.FullName
            Me.UserTime.Text = ("Logged in: " & Me.logins.AuthdLoginTime.ToString)

            '27/08/16
            Try
                UnregisterAnalyticUser()
            Catch ex As Exception

            End Try
            RegisterAnalyticUser(logins.AuthenticatedUser)
            RegisterSession()
        Catch exception1 As Exception
            ProjectData.SetProjectError(exception1)
            Dim exception As Exception = exception1
            ProjectData.ClearProjectError()
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
            If Not Me.HasAutobagHistory Then
                Me.PrepackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
                Me.ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.AutoBagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.PritingLabelID = 1
                Me.pritinglabel = "Prepack"
            End If
        ElseIf (labelid = 2) Then
            If Not Me.HasAutobagHistory Then
                Me.PrepackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
                Me.MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.AutoBagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.PritingLabelID = 2
                Me.pritinglabel = "Shelf"
            End If
        ElseIf (labelid = 3) Then
            If Not Me.HasAutobagHistory Then
                Me.PrepackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
                Me.AutoBagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.PritingLabelID = 3
                Me.pritinglabel = "Magnet"
            End If
        ElseIf (labelid = 5) Then
            If Not Me.HasAutobagHistory Then
                Me.PrepackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.AutoBagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
                Me.PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
                Me.PritingLabelID = 5
                Me.pritinglabel = "PPReady"
            End If
        ElseIf (labelid = 4) Then
            Me.PrepackHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
            Me.ShelfLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
            Me.MagnetLabelHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
            Me.AutoBagHighlight.BorderBrush = New SolidColorBrush(Colors.DarkRed)
            Me.PPReadyHighlight.BorderBrush = New SolidColorBrush(Colors.DarkBlue)
            Me.PritingLabelID = 4
            Me.pritinglabel = "Autobag"
            Me.HasAutobagHistory = True
            'Me.AutobagModeHider.Visibility = True
            Me.PrepackHighlight.Visibility = False
            Me.ShelfLabelHighlight.Visibility = False
            Me.MagnetLabelHighlight.Visibility = False
            Me.PPReadyHighlight.Visibility = False
        End If
    End Sub

    Private Sub Clock_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Me.TimeBox.Text = DateAndTime.Now.ToLongTimeString
    End Sub

    Private Sub DataRefresher_Tick(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Me.PrepackInfo = DirectCast(WHLClasses.MySQL.SelectData("SELECT * FROM whldata.prepacklist"), ArrayList)
        Catch exception1 As Exception
            ProjectData.SetProjectError(exception1)
            Dim exception As Exception = exception1
            ProjectData.ClearProjectError()
        End Try
        If Conversions.ToBoolean(Me.CheckUpdate) Then
            Me.CheckUpdate = False
            Try
                Dim info As UpdateCheckInfo = ApplicationDeployment.CurrentDeployment.CheckForDetailedUpdate
                If (info.UpdateAvailable And (info.AvailableVersion > ApplicationDeployment.CurrentDeployment.CurrentVersion)) Then
                    Dim textArray1 As String() = New String() {"There is an update available: " & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10), info.AvailableVersion.ToString, " (", Math.Round(CDbl((CDbl(info.UpdateSizeBytes) / 1024))).ToString, "kB) " & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & "Do you want to install it?"}
                    If (Conversions.ToInteger(Me.EMsgbox(String.Concat(textArray1), MsgBoxStyle.YesNo, "Update Available")) = 6) Then
                        If ApplicationDeployment.CurrentDeployment.Update Then
                            Me.EMsgbox("The update was installed. The application will now restart to apply the update.", MsgBoxStyle.ApplicationModal, "Alert")

                            ProjectData.EndApp()
                        Else
                            Me.EMsgbox("The update failed to install.", MsgBoxStyle.ApplicationModal, "Alert")
                            Me.CheckUpdate = True
                        End If
                    End If
                End If
            Catch exception3 As Exception
                ProjectData.SetProjectError(exception3)
                Dim exception2 As Exception = exception3
                Me.CheckUpdate = True
                ProjectData.ClearProjectError()
            End Try
        End If
        ' Me.UpdateScores()
    End Sub


    Friend ActiveItem As WhlSKU
    Friend ActiveChildren As SkuCollection

    Private Sub ExecuteSearch()

        Dim text As String = ScanFocusBox.Text
        If [text].StartsWith("qzu") Then
            Me.logins.replaceUser([text])
        ElseIf ([text].Length > 0) Then

            If ([text].StartsWith("10") And ([text].Length = 11)) Then
                [text] = [text].Remove(7)
            End If
            Me.NewScan = True
            '28/01/2016     Swapped out the old raw code for the class host searching methods. 
            Try

                Dim SearchResults As SkuCollection = Skusfull.ExcludeStatus("Dead").SearchSKUS([text])
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
                    Else
                        'Got a choice here kid. 
                        Me.Synthesizer.SpeakAsync("Choose the correct item from the list.")
                        BundleDialog.bundleoptionsal = SearchResults
                        BundleDialog.ShowDialog()
                        ActiveItem = BundleDialog.chosenshsku
                        PopulateData()
                    End If

                ElseIf SearchResults.Count = 0 Then
                    'No results. RIP. 
                    Me.Synthesizer.SpeakAsync("Nothing found.")
                    Me.ResetDisp()
                Else
                    'Continue!
                    ActiveItem = SearchResults(0)
                    PopulateData()

                End If
            Catch ex As Exception
                EMsgbox("Couldn't search because the system is too busy, please try again in a few minutes.", Nothing, Nothing)
            End Try


        Else
            EMsgbox("Something weird happened. Try again.", Nothing, Nothing)
        End If
        ScanFocusBox.Text = ""
        ScanFocusBox.Focus()

    End Sub

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
            BundleDetailbutton.Visibility = True
        Else
            BundleDetailbutton.Visibility = False
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
                        BagNoteEditorWPF.ActiveSku = Child
                        BagNoteEditorWPF.ShowDialog()
                    End If
                End If
            Catch ex As Exception
                Reporting.ReportException(ex, False)
                EMsgbox("The system was unable to display data for the pack of " + Child.PackSize.ToString, Nothing, Nothing)
            End Try
        Next
        'If ActiveChildren.Count = 1 Then       '19/04/16 - Change made after "Value of 0" error.

        '13/05/2016     Added scanning to prepack queue
        ScanToPrepackQueue(ActiveItem.SKU)

        If Packsizes.Items.Count = 1 Then
            Packsizes.SelectedIndex = 0
        End If
    End Sub

    Private Sub BundleDetailbutton_Click(sender As Object, e As EventArgs) Handles BundleDetailbutton.Click
        If Not IsNothing(ActiveItem) Then
            Dim itemCompString As String = ""
            For Each item As WhlSKU In ActiveItem.Composition
                itemCompString += item.GetLocation(SKULocation.SKULocationType.Pickable).LocationText + " - " + item.Title.Label + "." + vbNewLine
            Next

            EMsgbox(itemCompString, MsgBoxStyle.OkOnly, "Items in bundle:")
        End If
    End Sub

    Private Sub GetHistory()
        HistoryLabel.Text = ("SKU Prepack History (" & SelectedSKU.SKU & ")")
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
            Me.HistoryBody.Text = (Me.HistoryBody.Text & "Anything done before 16/10/2015 has not been recorded and will not show. ")
        End If
    End Sub

    Private Sub ImageButton1_BtnClick(ByVal sender As Object, ByVal e As EventArgs) Handles ChangeButton.Click
        If Not IsNothing(SelectedSKU) Then
            BagNoteEditorWPF.ActiveSku = SelectedSKU
            BagNoteEditorWPF.ShowDialog()

            ScanFocusBox.Focus()
        End If

    End Sub

    Private Sub ImageButton3_Click(ByVal sender As Object, ByVal e As EventArgs)
        EMsgbox("This feature has disappeared. Tell me if you actually miss it. ", Nothing, Nothing)
    End Sub

    Private Sub KeypadPress(sender As Button, e As EventArgs) Handles KeyClear.Click, Key8.Click, Key7.Click, Key6.Click, Key5.Click, Key4.Click, Key3.Click, Key2.Click, Key1.Click, key0.Click, AutobagBG.Click
        Keypad(sender.Text)
        ScanFocusBox.Focus()
    End Sub

    Dim SkusFullLoaded As Boolean = False
    Private Sub SkuCacheInitLoader_Tick(sender As Object, e As EventArgs)
        SkuCacheINitLoader.Stop()
        SkusFullLoaded = False


        Dim Loader2 As New GenericDataController
        Try

            If Not ForceUpdate Then
                Try
                    'If it's less than 12 hours old. 
                    Skusfull = Loader2.LoadSkuColl(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData + "\SkusCache.bin", True, False)
                    'Skusfull = Skusfull.ExcludeStatus("Dead")
                    Skus = Skusfull.MakeMixdown
                    Skusfull.SetUpWorker()
                    Skusfull.StartThreadedWorker()
                    SkusFullLoaded = True
                Catch ex2 As Exception

                    Skusfull = Loader2.SmartSkuCollLoad(True, "", False)
                    'Skusfull = Skusfull.ExcludeStatus("Dead")
                    Skus = Skusfull.MakeMixdown
                    Loader2.SaveDataToFile("Skuscache.bin", Skusfull, My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)
                    Skusfull.SetUpWorker()
                    Skusfull.StartThreadedWorker()
                    SkusFullLoaded = True
                End Try
            Else
                Skusfull = Loader2.SmartSkuCollLoad(True, "", False)
                'Skusfull = Skusfull.ExcludeStatus("Dead")
                Skus = Skusfull.MakeMixdown
                Loader2.SaveDataToFile("Skuscache.bin", Skusfull, My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData)
                Skusfull.SetUpWorker()
                Skusfull.StartThreadedWorker()
                SkusFullLoaded = True
            End If
        Catch ex As Exception
            EMsgbox(ex.ToString, Nothing, Nothing)
        End Try
        SkuCacheDownloadMask.Close()
        ForceUpdate = False

        'skuRefreshTime = Now.AddHours(1)
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

    Private Sub KeypadPress(sender As Object, e As EventArgs) Handles keyClear.Click, key8.Click, key7.Click, key6.Click, key5.Click, key4.Click, key3.Click, key2.Click, key1.Click, key0.Click, AutobagBG.Click
        Keypad(sender.ButtonText)
        ScanFocusBox.Focus()
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


        'Open the highlighted order
        Dim ordex As Linnworks.Orders.ExtendedOrder = loader.LoadOrdex(HighlightedOrders.Filename)
        'For Each item As PickingStrictness In ordex.PickingStrictness
        '    item = PickingStrictness.BestJudgement
        'Next

        If ordex.Status = OrderStatus._Withdrawn Then

        Else

            Dim StrictnessCount As Integer = ordex.PickingStrictness.Count - 1
            ordex.PickingStrictness.Clear()
            For i = 0 To StrictnessCount
                ordex.PickingStrictness.Add(PickingStrictness.BestJudgement)
            Next
            For Each issue As IssueData In ordex.issues
                If issue.IssueItemIndex = HighlightedOrderItems.IssueItemIndex Then
                    If issue.PreferredResolutionType = Orders.ResolutionType.ResetOrderToNew Then
                        issue.Resolved = True
                        ordex.SetStatus(OrderStatus._New, logins.AuthenticatedUser)
                    Else
                        'issue.Resolved = False
                        issue.Prepack_IssuePartlyResolveFor = True
                    End If

                End If
            Next
            loader.SaveDataToFile(ordex.LinnOpenOrder.NumOrderId.ToString + ".ordex", ordex, "T:\AppData\Orders")
        End If


        ActivePPQTitle.Text = "-------: Not on order"
        ActivePPQLabel1.Text = "-"
        ActivePPQLabel2.Text = "-"
        ActivePPQCompleteButton.Visibility = False
        UpdatePrepackOrders_Tick(Nothing, Nothing)
        ScanFocusBox.Text = ""
        ScanFocusBox.Focus()
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

    Dim Shits As New List(Of PrePackQueueItem)
    Dim Goods As New List(Of PrePackQueueItem)

    '10/03/2016     Populates the prepack queue list.
    Private Sub PopulatePPQ()
        If SkusFullLoaded Then

            Shits.Clear()
            Goods.Clear()
            Dim tooMuchGood As Boolean = False
            Dim tooMuchShit As Boolean = False

            For Each order As IssueDataAndOrder In PrePackQueue
                'Limit to 20
                If Goods.Count >= 20 Then
                    'Priority - Stop gathering completely.
                    tooMuchGood = True
                ElseIf Shits.Count >= 20 Then
                    'Lower priority - Stop gathering shits.
                    tooMuchShit = True
                    'ElseIf (Goods.Count + Shits.Count) > 20 Then 'Superfluous. We won't collect more than 40 now.
                End If


                Try
                    Dim newSkuColl As New SkuCollection(True)
                    newSkuColl.AddRange(Skusfull)
                    Dim sku As WhlSKU = newSkuColl.SearchBarcodes(order.Issue.DodgySku)(0)
                    Dim NewQueueItem As New PrePackQueueItem
                    NewQueueItem.SkuNum = order.Issue.DodgySku '10/09/16
                    NewQueueItem.OrderNum = order.Order.OrderId '10/09/16
                    NewQueueItem.ItemBG.Background = System.Windows.Media.Brushes.OliveDrab
                    NewQueueItem.Shelf.Text = sku.GetLocation(SKULocation.SKULocationType.Pickable).LocationText
                    NewQueueItem.Sku.Text = sku.Title.Label
                    NewQueueItem.Info.Text = order.Order.BetterItems(order.Issue.IssueItemIndex).OrderQuantity.ToString + "x " + sku.PackSize.ToString + "-pack"

                    AddHandler NewQueueItem.MouseUp, AddressOf ClickPPQI
                    AddHandler NewQueueItem.TouchUp, AddressOf ClickPPQI

                    If order.Order.PicklistType = ItemPicklistType.MultiMixedFirst Or order.Order.PicklistType = ItemPicklistType.MultiMixedSecond Or order.Order.PicklistType = ItemPicklistType.Courier Then
                        'Mixed multi - Priority or something
                        If Not tooMuchGood Then
                            NewQueueItem.ItemBG.Background = System.Windows.Media.Brushes.Red
                            Goods.Add(NewQueueItem)
                        End If

                    Else
                        'Not mixed multi, lower priority.
                        If Not tooMuchShit And Not tooMuchGood Then

                            NewQueueItem.ItemBG.Background = System.Windows.Media.Brushes.Black
                            Shits.Add(NewQueueItem)
                        End If
                    End If
                    'PPQPanel.Controls.Add(NewQueueItem)
                Catch ex As Exception

                End Try
            Next

        End If
    End Sub

    'If I click a PPQI
    Private Sub ClickPPQI(sender As Object, e As EventArgs)
        Dim theOrder As Linnworks.Orders.ExtendedOrder = loader.LoadOrdex("T:\AppData\Orders\" + sender.OrderNum + ".ordex")

        If theOrder.Status = OrderStatus._Withdrawn Then
            MsgBox("This order has been withdrawn. It may take a moment to update the status.")
        Else
            Dim edited As Boolean = False
            For Each foundIssue As IssueData In theOrder.issues
                If foundIssue.DodgySku = sender.SkuNum Then 'If they're dealing with one issue in the order with this sku number, they're effectively dealing with them all
                    If Not foundIssue.Prepack_WorkingOnIt Then
                        foundIssue.Prepack_WorkingOnIt = True
                        sender.ChangePanelVisibility()
                        edited = True
                    Else
                        MsgBox("Someone is currently working on this one. It may take a moment to update the status.")
                    End If
                End If
            Next
            If edited Then
                theOrder.SaveToDisk()
            End If
        End If

    End Sub

    Dim workerAttempt As Integer = 0
    Dim workerWorked As Boolean = True
    Dim workerEx As New Exception

    '10/03/2016     Updates the PPQ, and tells it to populate. Will be force run when a PPQ item is "Completed"
    Private Sub UpdatePrepackOrders_Tick(sender As Object, e As EventArgs)

        'If Now > skuRefreshTime Then

        '    AdminUpdate.PerformClick()
        'Else
        'Dim timeToGo As TimeSpan = skuRefreshTime - Now
        'AdminUpdate.Text = "Update Cache - Full Reset in " + (Math.Floor(timeToGo.TotalMinutes)).ToString + "mins"

        UpdatePrepackOrders.Start()
        If Not PrepackQueueWorker.IsBusy Then
            PPQPanel.Visibility = False
            For Each control As Control In PPQPanel.Children
                control.Dispose()
            Next
            PPQPanel.Children.Clear()

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
            PopulatePPQ()
        Catch ex As Exception
            workerWorked = False
            workerEx = ex
        End Try

        PrepackQueueWorker.ReportProgress(0)
    End Sub

    Private Sub displayError()
        Dim ErrorLabel As New TextBlock
        PPQPanel.Children.Clear()
        ErrorLabel.Text = workerEx.ToString
        ErrorLabel.Background = New SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 0, 0, 0))
        ErrorLabel.Foreground = System.Windows.Media.Brushes.White
        ErrorLabel.FontSize = 11.0


        ErrorLabel.Width = 443
        ErrorLabel.Height = 188
        PPQPanel.Children.Add(ErrorLabel)
    End Sub

    Private Sub PPQWorkerReport()
        If workerWorked Then
            For Each goodOrder As PrePackQueueItem In Goods
                If PPQPanel.Children.Count >= 20 Then
                    Exit For
                Else
                    PPQPanel.Children.Add(goodOrder)
                End If
            Next
            For Each shitOrder As PrePackQueueItem In Shits
                If PPQPanel.Children.Count >= 20 Then
                    Exit For
                Else
                    PPQPanel.Children.Add(shitOrder)
                End If
            Next
        Else
            displayError()
        End If

        'Lets make panels appear
        For Each order As IssueDataAndOrder In PrePackQueue
            If order.Issue.Prepack_WorkingOnIt Then
                'find the control
                For Each PPQItem As PrePackQueueItem In PPQPanel.Children
                    If PPQItem.OrderNum = order.Order.OrderId Then
                        'PPQItem.UglyOrangePanel.Visible = True
                    End If
                Next
            End If
        Next

        PPQPanel.Visibility = True
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

        '03/06/2016     What the fuck is changed? Why did I make this? Also added the list clearing jsut above.
        Dim Changed As Boolean = False
        For Each order As IssueDataAndOrder In PrePackQueue
            For Each item As IssueData In order.Order.issues
                If item.DodgySku.Substring(0, 7) = Sku.Substring(0, 7) Then
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
            My.Computer.Audio.Play("bloop.wav", AudioPlayMode.Background)
            ActivePPQCompleteButton.Visibility = True

        Else
            ActivePPQTitle.Text = "-------: Not on order"
            ActivePPQLabel1.Text = "-"
            ActivePPQLabel2.Text = "-"
            ActivePPQCompleteButton.Visibility = False
        End If
    End Sub


    'WPF Stuff
    Public Sub PerformClick(Button As System.Windows.Controls.Button)
        Button.RaiseEvent(New RoutedEventArgs(System.Windows.Controls.Button.ClickEvent))
    End Sub
    Public Function EMsgbox(ByVal Prompt As String, Style As MsgBoxStyle, Title As String) As Object
        If IsNothing(Title) Then
            Title = "Alert"
        End If
        If IsNothing(Style) Then
            Style = 0
        End If
        Dim dialog As New MsgBoxDialog
        dialog.SetData(Prompt, Style, Title)
        Return dialog.ShowDialog
    End Function
    'Public Function EMsgbox(ByVal Prompt As String, ByVal Optional Style As MsgBoxStyle = 0, ByVal Optional Title As String = "Alert") As Object
    '    Dim dialog As New MsgBoxDialog
    '    dialog.SetData(Prompt, Style, Title)
    '    Return dialog.ShowDialog
    'End Function
    Friend DataRefresher As New System.Windows.Threading.DispatcherTimer
    Friend Clock As New System.Windows.Threading.DispatcherTimer
    Friend SkuCacheINitLoader As New System.Windows.Threading.DispatcherTimer
    Friend AdminStatusUpdater As New System.Windows.Threading.DispatcherTimer
    Friend UpdatePrepackOrders As New System.Windows.Threading.DispatcherTimer
    Friend Sub LoadTimers()
        AddHandler ScanFocusBox.KeyDown, AddressOf ScanFocusBox_KeyDown
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

