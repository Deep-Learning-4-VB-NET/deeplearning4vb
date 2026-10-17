Imports System
Imports System.Collections.Generic
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms.parse

    ''' <summary>
    ''' Convert string writables to doubles: every <c>Text</c> value is parsed (<c>Writable.toDouble()</c>) and every
    ''' string column of the schema becomes a double column. The transform has no configuration, so (as with Lombok
    ''' <c>@Data</c> on a class without fields) all instances are equal.
    ''' </summary>
    ' Java: @Data
    <Serializable>
    Public Class ParseDoubleTransform
        Inherits BaseTransform

        ''' <summary>As in Java: the full class name.</summary>
        Public Overrides Function ToString() As String
            Return Me.GetType().FullName
        End Function

        ''' <summary>
        ''' Get the output schema for this transformation, given an input schema
        ''' </summary>
        ''' <param name="inputSchema"> </param>
        Public Overrides Function transform(ByVal inputSchema As SchemaT) As SchemaT
            Dim newSchema As New SchemaT.Builder()
            For i As Integer = 0 To inputSchema.numColumns() - 1
                If inputSchema.getType(i) = ColumnType.String Then
                    newSchema.addColumnDouble(inputSchema.getMetaData(i).Name)
                Else
                    newSchema.addColumn(inputSchema.getMetaData(i))
                End If
            Next i
            Return newSchema.build()
        End Function

        Public Overrides Function map(ByVal writables As IList(Of WritableT)) As IList(Of WritableT)
            Dim transformed As IList(Of WritableT) = New List(Of WritableT)()
            For Each w As WritableT In writables
                If TypeOf w Is Text Then
                    transformed.Add(New DoubleWritable(w.toDouble()))
                Else
                    transformed.Add(w)
                End If
            Next w
            Return transformed
        End Function

        ''' <summary>
        ''' Transform an object
        ''' in to another object: the string form of the input, parsed as a double
        ''' </summary>
        ''' <param name="input"> the record to transform </param>
        ''' <returns> the transformed writable </returns>
        Public Overrides Function map(ByVal input As Object) As Object
            Return JavaNumbers.parseDouble(input.ToString())
        End Function

        ''' <summary>
        ''' Transform a sequence
        ''' </summary>
        ''' <param name="sequence"> </param>
        Public Overrides Function mapSequence(ByVal sequence As Object) As Object
            Dim list As System.Collections.IList = DirectCast(sequence, System.Collections.IList)
            Dim ret As IList(Of Object) = New List(Of Object)()
            For Each o As Object In list
                ret.Add(map(o))
            Next o
            Return ret
        End Function

        ''' <summary>
        ''' The output column name
        ''' after the operation has been applied
        ''' </summary>
        ''' <returns> the output column name </returns>
        Public Overrides Function outputColumnName() As String
            Return InputSchema.getColumnNames()(0)
        End Function

        ''' <summary>
        ''' The output column names
        ''' This will often be the same as the input
        ''' </summary>
        ''' <returns> the output column names </returns>
        Public Overrides Function outputColumnNames() As String()
            Return New List(Of String)(inputSchemaField.getColumnNames()).ToArray()
        End Function

        ''' <summary>
        ''' Returns column names
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnNames() As String()
            Return New List(Of String)(inputSchemaField.getColumnNames()).ToArray()
        End Function

        ''' <summary>
        ''' Returns a singular column name
        ''' this op is meant to run on
        ''' </summary>
        Public Overrides Function columnName() As String
            Return outputColumnName()
        End Function

        ''' <summary>Lombok <c>@Data</c> equality of a class without fields: any two instances are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return obj IsNot Nothing AndAlso obj.GetType() Is Me.GetType()
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 1
        End Function

    End Class

End Namespace
