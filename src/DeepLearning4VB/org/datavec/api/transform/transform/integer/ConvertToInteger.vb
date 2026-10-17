Imports System
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports WritableType = DeepLearning4VB.org.datavec.api.writable.WritableType

Namespace org.datavec.api.transform.transforms.integer

    ''' <summary>
    ''' Convert any value to an Integer (using <c>Writable.toInt()</c>).
    ''' Equality compares the class and the column name.
    ''' </summary>
    ' Java: @Data @NoArgsConstructor
    <Serializable>
    Public Class ConvertToInteger
        Inherits BaseIntegerTransform

        ''' <summary>Lombok <c>@NoArgsConstructor</c>.</summary>
        Public Sub New()
        End Sub

        ''' <param name="column"> Name of the column to convert to an integer </param>
        Public Sub New(ByVal column As String)
            MyBase.New(column)
        End Sub

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            If writable.getType() = WritableType.Int Then
                Return DirectCast(writable, IntWritable)
            End If
            Return New IntWritable(writable.toInt())
        End Function

        ''' <summary>A number becomes its int value (truncated); anything else is parsed (Java <c>Integer.parseInt</c>).</summary>
        Public Overrides Function map(ByVal input As Object) As Object
            If JavaNumbers.isNumber(input) Then
                Return JavaNumbers.intValue(input)
            End If
            Return JavaNumbers.parseInt(input.ToString())
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            Return New IntegerMetaData(newColumnName)
        End Function

        Public Overrides Function ToString() As String
            Return "ConvertToInteger(columnName=" & columnNameField & ")"
        End Function

    End Class

End Namespace
