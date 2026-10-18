Imports System

Namespace org.nd4vb.linalg.workspace

    ''' <summary>
    ''' An exception to specify than a workspace-related error has occurred (Java <c>ND4JWorkspaceException</c>, a
    ''' <c>RuntimeException</c>).
    ''' </summary>
    Public Class ND4VBWorkspaceException
        Inherits System.Exception

        Public Sub New()
        End Sub

        Public Sub New(ByVal cause As System.Exception)
            ' Java: the message of new RuntimeException(cause) is cause.toString()
            MyBase.New(If(cause Is Nothing, Nothing, cause.GetType().FullName & ": " & cause.Message), cause)
        End Sub

        Public Sub New(ByVal msg As String)
            MyBase.New(msg)
        End Sub

        Public Sub New(ByVal msg As String, ByVal cause As System.Exception)
            MyBase.New(msg, cause)
        End Sub

    End Class

End Namespace
