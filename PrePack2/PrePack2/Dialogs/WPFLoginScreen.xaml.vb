﻿Imports WHLClasses
Public Class WPFLoginScreen
    Private RequiresPin As Boolean = False
    Private RequiresLogin As Boolean = False
    Public empcol As New EmployeeCollection
    Private CurrentEmployee As New Employee
    Private Sub WPFLoginScreenLoad() Handles Me.Loaded
        LoginScanBox.Focus()

    End Sub
    Private Sub ProcessData(Data As String)
        If Data.StartsWith("qzu") Then
            Try
                MainWindow.authd = empcol.FindEmployeeByID(Convert.ToInt32(Data.Replace("qzu", "")))
                Me.Close()

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        ElseIf Data.Length > 0 And Data.Length < 3 Then
            LoginTitle.Text = empcol.FindEmployeeByID(Convert.ToInt32(Data)).FullName + " Please enter your Pin"
            CurrentEmployee = empcol.FindEmployeeByID(Convert.ToInt32(Data))

            RequiresPin = True
        ElseIf Data.Length = 4 And RequiresPin = True Then
            If CurrentEmployee.CheckPin(Data) Then
                MainWindow.authd = CurrentEmployee
                Me.Close()
            Else
                Dim Msg As New WPFMsgBoxDialog
                Msg.Body.Text = "That is the wrong pin, please try again"
                Msg.ShowDialog()
            End If

        Else
            Dim Msg As New WPFMsgBoxDialog
            Msg.Body.Text = "We were unable to find your user, please try again"
            Msg.ShowDialog()
        End If

    End Sub
    Private Sub Keypad_Handle(sender As Object, e As RoutedEventArgs) Handles Keypad0.Click, Keypad1.Click, Keypad2.Click, Keypad3.Click, Keypad4.Click, Keypad5.Click, Keypad6.Click, Keypad7.Click, Keypad8.Click, Keypad9.Click
        LoginScanBox.Text += sender.Content.ToString
    End Sub
    Private Sub ScanBox_KeyDown(sender As Object, e As KeyEventArgs) Handles LoginScanBox.KeyDown
        If (e.Key = Key.Return) Then
            e.Handled = True
            ProcessData(LoginScanBox.Text)

            LoginScanBox.Text = ""
            LoginScanBox.Focus()
        End If
    End Sub
    Private Sub KeypadEnter_Click(Sender As Object, e As RoutedEventArgs) Handles KeypadEnter.Click
        ProcessData(LoginScanBox.Text)
        LoginScanBox.Text = ""
        LoginScanBox.Focus()
    End Sub

    Private Sub ClearButton_Click(sender As Object, e As RoutedEventArgs) Handles ClearButton.Click
        LoginScanBox.Text = ""
    End Sub
End Class
