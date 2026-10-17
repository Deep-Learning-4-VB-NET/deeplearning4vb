Imports System
Imports ColumnMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.ColumnMetaData
Imports FloatMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.FloatMetaData
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports WritableType = DeepLearning4VB.org.datavec.api.writable.WritableType

Namespace org.datavec.api.transform.transforms.floattransform

    ''' <summary>
    ''' Convert any value to a Float (using <c>Writable.toFloat()</c>).
    ''' Equality compares the class and the column name.
    ''' <para>A <c>FloatWritable</c> is returned unchanged. (Java tested for <c>WritableType.Double</c> and then cast to
    ''' <c>FloatWritable</c>, so every double input failed with a ClassCastException.)</para>
    ''' </summary>
    ' Java: @NoArgsConstructor @Data
    <Serializable>
    Public Class ConvertToFloat
        Inherits BaseFloatTransform

        ''' <summary>Lombok <c>@NoArgsConstructor</c>.</summary>
        Public Sub New()
        End Sub

        ''' <param name="column"> Name of the column to convert to a Float column </param>
        Public Sub New(ByVal column As String)
            MyBase.New(column)
        End Sub

        Public Overrides Function map(ByVal writable As WritableT) As WritableT
            If writable.getType() = WritableType.Float Then
                Return DirectCast(writable, FloatWritable)
            End If
            Return New FloatWritable(writable.toFloat())
        End Function

        ''' <summary>A number becomes its float value; anything else is parsed (Java <c>Float.parseFloat</c>).</summary>
        Public Overrides Function map(ByVal input As Object) As Object
            If JavaNumbers.isNumber(input) Then
                Return JavaNumbers.floatValue(input)
            End If
            Return JavaNumbers.parseFloat(input.ToString())
        End Function

        Public Overrides Function getNewColumnMetaData(ByVal newColumnName As String, ByVal oldColumnType As ColumnMetaData) As ColumnMetaData
            Return New FloatMetaData(newColumnName)
        End Function

        Public Overrides Function ToString() As String
            Return "ConvertToFloat(columnName=" & columnNameField & ")"
        End Function

    End Class

End Namespace
