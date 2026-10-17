Imports System
Imports System.Collections.Generic
Imports System.Text.Json.Serialization
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' This method maps all String values, except those is the specified list, to a single String  value
    ''' Equality compares the class, the column name, the exceptions and the new value.
    ''' </summary>
    ' Java: @Data @EqualsAndHashCode
    <Serializable>
    Public Class MapAllStringsExceptListTransform
        Inherits BaseStringTransform

        Private ReadOnly exceptions As ISet(Of String)
        Private ReadOnly newValue As String

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal newValue As String, ByVal exceptions As IList(Of String))
            MyBase.New(columnName)
            Me.newValue = newValue
            Me.exceptions = New HashSet(Of String)(exceptions)
        End Sub

        ''' <summary>Java <c>getExceptions()</c> (Lombok).</summary>
        Public Function getExceptions() As ISet(Of String)
            Return exceptions
        End Function

        ''' <summary>Java <c>getNewValue()</c> (Lombok).</summary>
        Public Function getNewValue() As String
            Return newValue
        End Function

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Dim str As String = writable.ToString()
            If exceptions.Contains(str) Then
                Return New Text(str)
            Else
                Return New Text(newValue)
            End If
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim str As String = input.ToString()
            If exceptions.Contains(str) Then
                Return str
            Else
                Return newValue
            End If
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "MapAllStringsExceptListTransform(exceptions=" & javaToString(exceptions) & ", newValue=" & newValue & ")"
        End Function

        ''' <summary>Equal when the class, the column name, the exceptions and the new value are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If Not MyBase.Equals(obj) Then Return False
            Dim o As MapAllStringsExceptListTransform = DirectCast(obj, MapAllStringsExceptListTransform)
            Return javaEquals(exceptions, o.exceptions) AndAlso String.Equals(newValue, o.newValue)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), exceptions, newValue)
        End Function

    End Class

End Namespace
