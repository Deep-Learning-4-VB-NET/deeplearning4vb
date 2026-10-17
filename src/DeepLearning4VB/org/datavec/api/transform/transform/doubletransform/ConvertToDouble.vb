Imports System
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports DoubleMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.DoubleMetaData
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports WritableType = DeepLearning4VB.org.datavec.api.writable.WritableType

Namespace org.datavec.api.transform.transforms.doubletransform

    ''' <summary>
    ''' Convert any value to a Double (using <c>Writable.toDouble()</c>).
    ''' Equality compares the class and the column name.
    ''' </summary>
    ' Java: @NoArgsConstructor @Data
    <Serializable>
    Public Class ConvertToDouble
        Inherits BaseDoubleTransform

        ''' <summary>Lombok <c>@NoArgsConstructor</c>.</summary>
        Public Sub New()
        End Sub

        ''' <param name="column"> Name of the column to convert to a Double column </param>
        Public Sub New(ByVal column As String)
            MyBase.New(column)
        End Sub

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            If writable.getType() = WritableType.Double Then
                Return DirectCast(writable, DoubleWritable)
            End If
            Return New DoubleWritable(writable.toDouble())
        End Function

        ''' <summary>A number becomes its double value; anything else is parsed (Java <c>Double.parseDouble</c>).</summary>
        Public Overrides Function map(ByVal input As Object) As Object
            If JavaNumbers.isNumber(input) Then
                Return JavaNumbers.doubleValue(input)
            End If
            Return JavaNumbers.parseDouble(input.ToString())
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            Return New DoubleMetaData(newColumnName)
        End Function

        Public Overrides Function ToString() As String
            Return "ConvertToDouble(columnName=" & columnNameField & ")"
        End Function

    End Class

End Namespace
