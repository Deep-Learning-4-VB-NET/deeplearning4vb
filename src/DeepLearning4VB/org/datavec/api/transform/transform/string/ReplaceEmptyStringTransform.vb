Imports System
Imports System.Text.Json.Serialization
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.string

    ''' <summary>
    ''' Replace empty String values with the specified String
    ''' Equality (Lombok <c>callSuper = true</c>) compares the class, the column name and the value.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    <Serializable>
    Public Class ReplaceEmptyStringTransform
        Inherits BaseStringTransform

        Private valueField As String

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal value As String)
            MyBase.New(columnName)
            Me.valueField = value
        End Sub

        ''' <summary>Java <c>getValue()</c> (Lombok).</summary>
        Public Function getValue() As String
            Return valueField
        End Function

        ''' <summary>Java <c>setValue()</c> (Lombok).</summary>
        Public Sub setValue(ByVal value As String)
            Me.valueField = value
        End Sub

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Dim s As String = writable.ToString()
            If s Is Nothing OrElse s.Length = 0 Then
                Return New Text(valueField)
            ElseIf TypeOf writable Is Text Then
                Return DirectCast(writable, Text)
            Else
                Return New Text(writable.ToString())
            End If
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Dim s As String = input.ToString()
            If s Is Nothing OrElse s.Length = 0 Then
                Return valueField
            End If
            Return s
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "ReplaceEmptyStringTransform(value=" & valueField & ")"
        End Function

        ''' <summary>Equal when the class, the column name and the value are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso String.Equals(valueField, DirectCast(obj, ReplaceEmptyStringTransform).valueField)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), valueField)
        End Function

    End Class

End Namespace
