Imports WHLClasses
Public Class WPFLoginScreen
    Private RequiresPin As Boolean = False
    ReadOnly empcol As New EmployeeCollection
    Private CurrentEmployee As New Employee
    Private Sub WPFLoginScreenLoad() Handles Me.Loaded
        Focus()
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
            If IsNumeric(Data) Then
                LoginTitle.Text = empcol.FindEmployeeByID(Convert.ToInt32(Data)).FullName + " Please enter your Pin"
                CurrentEmployee = empcol.FindEmployeeByID(Convert.ToInt32(Data))

                RequiresPin = True
                LoginPasswordBox.Visibility = Visibility.Visible
                LoginScanBox.Visibility = Visibility.Collapsed

                LoginPasswordBox.Focus()

            End If

        ElseIf RequiresPin = True Then
            If CurrentEmployee.CheckPin(Data) Then
                MainWindow.authd = CurrentEmployee
                RequiresPin = False
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
        LoginPasswordBox.Password += sender.content.ToString
        LoginScanBox.Text += sender.Content.ToString
    End Sub
    Private Sub ScanBox_KeyDown(sender As Object, e As KeyEventArgs) Handles LoginScanBox.KeyDown
        If (e.Key = Key.Return) Then
            e.Handled = True


            If RequiresPin Then
                ProcessData(LoginPasswordBox.Password)
                LoginPasswordBox.Password = ""

            Else
                ProcessData(LoginScanBox.Text)
                LoginScanBox.Text = ""
            End If

        End If
    End Sub
    Private Sub KeypadEnter_Click(Sender As Object, e As RoutedEventArgs) Handles KeypadEnter.Click
        If RequiresPin Then
            ProcessData(LoginPasswordBox.Password)
            LoginPasswordBox.Password = ""
            LoginPasswordBox.Focus()
        Else
            ProcessData(LoginScanBox.Text)
            LoginScanBox.Text = ""
        End If
    End Sub

    Private Sub ClearButton_Click(sender As Object, e As RoutedEventArgs) Handles ClearButton.Click
        If RequiresPin Then
            LoginPasswordBox.Password = ""
        Else
            LoginScanBox.Text = ""
        End If
    End Sub
End Class
