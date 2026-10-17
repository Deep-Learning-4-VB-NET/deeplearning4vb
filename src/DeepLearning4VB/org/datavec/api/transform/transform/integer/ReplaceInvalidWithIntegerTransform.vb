Imports System
Imports System.Text.Json.Serialization
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.integer

    ''' <summary>
    ''' Replace an invalid (non-integer) value in a column with a specified integer (validity according to the
    ''' column's metadata in the input schema).
    ''' Equality compares the class, the column name and the value.
    ''' </summary>
    ' Java: @Data
    <Serializable>
    Public Class ReplaceInvalidWithIntegerTransform
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
            If inputSchemaField.getMetaData(columnNumber).isValid(writable) Then
                Return writable
            Else
                Return New IntWritable(valueField)
            End If
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the input if it is a valid integer for the column, otherwise the replacement value
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            If inputSchemaField.getMetaData(columnNumber).isValid(New IntWritable(JavaNumbers.intValue(input))) Then
                Return input
            Else
                Return valueField
            End If
        End Function

        ''' <summary>Lombok <c>toString()</c>.</summary>
        Public Overrides Function ToString() As String
            Return "ReplaceInvalidWithIntegerTransform(value=" & valueField & ")"
        End Function

        ''' <summary>Equal when the class, the column name and the value are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj) AndAlso valueField = DirectCast(obj, ReplaceInvalidWithIntegerTransform).valueField
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return lombokHash(MyBase.GetHashCode(), valueField)
        End Function

    End Class

End Namespace
