Imports System.Windows.Threading

Class Application

    Private Sub Application_DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        WHLClasses.Reporting.ReportException(e.Exception)
        e.Handled = True
    End Sub
End Class
