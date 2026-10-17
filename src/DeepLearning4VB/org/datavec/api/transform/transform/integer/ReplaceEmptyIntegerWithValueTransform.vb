Imports System
Imports System.Text.Json.Serialization
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.integer

    ''' <summary>
    ''' Replace an empty/missing integer with a certain value.
    ''' Equality (Lombok <c>callSuper = true</c>) compares the class, the column name and the value.
    ''' </summary>
    ' Java: @EqualsAndHashCode(callSuper = true) @Data
    <Serializable>
    Public Class ReplaceEmptyIntegerWithValueTransform
        Inherits BaseIntegerTransform

        Private ReadOnly valueField As Integer

        <JsonConstructor>
        Public Sub New(ByVal columnName As String, ByVal value As Integer)
            MyBase.New(columnName)
            Me.valueField = value
        End Sub

        ''' <summary>Java <c>getValue()</c> (Lombok).</summary>
        Public Function getValue() As Integer
            Return valueField
        End Function

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            Dim s As String = writable.ToString()
            If s Is Nothing OrElse s.Length = 0 Then
                Return New IntWritable(valueField)
            End If
            Return writable
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object (as in Java: always the replacement value)
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return valueField
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "ReplaceEmptyIntegerWithValueTransform(value=" & valueField & ")"
        End Function

        ''' <summary>Equal when the class, the column name and the value are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso valueField = DirectCast(obj, ReplaceEmptyIntegerWithValueTransform).valueField
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), valueField)
        End Function

    End Class

End Namespace
