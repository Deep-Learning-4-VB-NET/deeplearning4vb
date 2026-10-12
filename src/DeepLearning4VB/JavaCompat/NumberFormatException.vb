Imports System

Namespace JavaCompat

    ''' <summary>
    ''' Thrown when a string cannot be parsed as a number. Like Java's
    ''' <c>NumberFormatException</c> it is an <see cref="ArgumentException"/>, so code ported
    ''' from Java that catches <c>IllegalArgumentException</c> keeps working.
    ''' </summary>
    <Serializable>
    Public Class NumberFormatException
        Inherits ArgumentException

        Public Sub New()
            MyBase.New("Number format error")
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub

        ''' <summary>The message Java produces for a malformed input string.</summary>
        Public Shared Function ForInputString(ByVal s As String, ByVal cause As Exception) As NumberFormatException
            Return New NumberFormatException("For input string: """ & If(s, "null") & """", cause)
        End Function
    End Class

End Namespace
