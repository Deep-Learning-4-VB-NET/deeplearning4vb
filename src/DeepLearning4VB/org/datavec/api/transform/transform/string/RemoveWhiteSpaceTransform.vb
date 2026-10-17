Imports System
Imports System.Text.Json.Serialization
Imports System.Text.RegularExpressions
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' String transform that removes all whitespace characters (Java <c>\s</c>: space, tab, newline, vertical tab,
    ''' form feed and carriage return).
    ''' Equality compares the class and the column name.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    <Serializable>
    Public Class RemoveWhiteSpaceTransform
        Inherits BaseStringTransform

        ''' <summary>Java's <c>\s</c> (ASCII whitespace only; .NET's <c>\s</c> also matches Unicode spaces).</summary>
        Private Const JavaWhitespace As String = "[ \t\n\x0B\f\r]"

        <JsonConstructor>
        Public Sub New(ByVal columnName As String)
            MyBase.New(columnName)
        End Sub

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Dim value As String = Regex.Replace(writable.ToString(), JavaWhitespace, "")
            Return New Text(value)
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return Regex.Replace(input.ToString(), JavaWhitespace, "")
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "RemoveWhiteSpaceTransform()"
        End Function

    End Class

End Namespace
