#If _MyType <> "Empty" Then

Namespace My
    ''' <summary>
    ''' Module used to define the properties that are available in the My Namespace for WPF
    ''' </summary>
    ''' <remarks></remarks>
    <HideModuleName()>
    Module MyWpfExtension
        Private s_Computer As New ThreadSafeObjectProvider(Of Devices.Computer)
        Private s_User As New ThreadSafeObjectProvider(Of ApplicationServices.User)
        Private s_Windows As New ThreadSafeObjectProvider(Of MyWindows)
        Private s_Log As New ThreadSafeObjectProvider(Of Logging.Log)
        ''' <summary>
        ''' Returns the application object for the running application
        ''' </summary>
        <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>
        Friend ReadOnly Property Application() As Application
            Get
                Return CType(System.Windows.Application.Current, Application)
            End Get
        End Property
        ''' <summary>
        ''' Returns information about the host computer.
        ''' </summary>
        <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>
        Friend ReadOnly Property Computer() As Devices.Computer
            Get
                Return s_Computer.GetInstance()
            End Get
        End Property
        ''' <summary>
        ''' Returns information for the current user.  If you wish to run the application with the current 
        ''' Windows user credentials, call My.User.InitializeWithWindowsUser().
        ''' </summary>
        <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>
        Friend ReadOnly Property User() As ApplicationServices.User
            Get
                Return s_User.GetInstance()
            End Get
        End Property
        ''' <summary>
        ''' Returns the application log. The listeners can be configured by the application's configuration file.
        ''' </summary>
        <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>
        Friend ReadOnly Property Log() As Logging.Log
            Get
                Return s_Log.GetInstance()
            End Get
        End Property

        ''' <summary>
        ''' Returns the collection of Windows defined in the project.
        ''' </summary>
        <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>
        Friend ReadOnly Property Windows() As MyWindows
            <DebuggerHidden()>
            Get
                Return s_Windows.GetInstance()
            End Get
        End Property
        <ComponentModel.EditorBrowsableAttribute(ComponentModel.EditorBrowsableState.Never)>
        <MyGroupCollection("System.Windows.Window", "Create__Instance__", "Dispose__Instance__", "My.MyWpfExtenstionModule.Windows")>
        Friend NotInheritable Class MyWindows
            <DebuggerHidden()>
            Private Shared Function Create__Instance__(Of T As {New, Window})(ByVal Instance As T) As T
                If Instance Is Nothing Then
                    If s_WindowBeingCreated IsNot Nothing Then
                        If s_WindowBeingCreated.ContainsKey(GetType(T)) = True Then
                            Throw New InvalidOperationException("The window cannot be accessed via My.Windows from the Window constructor.")
                        End If
                    Else
                        s_WindowBeingCreated = New Hashtable()
                    End If
                    s_WindowBeingCreated.Add(GetType(T), Nothing)
                    Return New T()
                    s_WindowBeingCreated.Remove(GetType(T))
                Else
                    Return Instance
                End If
            End Function
            <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")>
            <DebuggerHidden()>
            Private Sub Dispose__Instance__(Of T As Window)(ByRef instance As T)
                instance = Nothing
            End Sub
            <DebuggerHidden()>
            <ComponentModel.EditorBrowsableAttribute(ComponentModel.EditorBrowsableState.Never)>
            Public Sub New()
                MyBase.New()
            End Sub
            <Global.System.ThreadStatic()> Private Shared s_WindowBeingCreated As Hashtable
            <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Overrides Function Equals(ByVal o As Object) As Boolean
                Return MyBase.Equals(o)
            End Function
            <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Overrides Function GetHashCode() As Integer
                Return MyBase.GetHashCode
            End Function
            <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")>
            <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
            Friend Overloads Function [GetType]() As Type
                Return GetType(MyWindows)
            End Function
            <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Overrides Function ToString() As String
                Return MyBase.ToString
            End Function
        End Class
    End Module
End Namespace
Partial Class Application
    Inherits System.Windows.Application
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>
    <CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")>
    Friend ReadOnly Property Info() As ApplicationServices.AssemblyInfo
        <DebuggerHidden()>
        Get
            Return New ApplicationServices.AssemblyInfo(Reflection.Assembly.GetExecutingAssembly())
        End Get
    End Property
End Class
#End If