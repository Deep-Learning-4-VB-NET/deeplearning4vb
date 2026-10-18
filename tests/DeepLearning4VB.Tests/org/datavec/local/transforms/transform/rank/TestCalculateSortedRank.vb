Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports DoubleWritableComparator = DeepLearning4VB.org.datavec.api.writable.comparator.DoubleWritableComparator
Imports LocalTransformExecutorT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformExecutor
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform.rank

    ''' <summary>Port of the DataVec (datavec-local) <c>TestCalculateSortedRank</c> JUnit class, plus descending order and ties.</summary>
    <TestClass>
    Public Class TestCalculateSortedRank

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        <TestMethod>
        Public Sub testCalculateSortedRank()

            Dim data As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            data.Add(row(New Text("0"), New DoubleWritable(0.0)))
            data.Add(row(New Text("3"), New DoubleWritable(0.3)))
            data.Add(row(New Text("2"), New DoubleWritable(0.2)))
            data.Add(row(New Text("1"), New DoubleWritable(0.1)))

            Dim rdd As IList(Of IList(Of WritableT)) = data

            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnsString("TextCol").addColumnDouble("DoubleCol").build()

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).calculateSortedRank("rank", "DoubleCol", New DoubleWritableComparator()).build()

            Dim outSchema As SchemaT = tp.FinalSchema
            assertEquals(3, outSchema.numColumns())
            assertEquals(New List(Of String) From {"TextCol", "DoubleCol", "rank"}, outSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.String, ColumnType.Double, ColumnType.Long}, outSchema.getColumnTypes())

            Dim [out] As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(rdd, tp)

            Dim collected As IList(Of IList(Of WritableT)) = [out]
            assertEquals(4, collected.Count)
            For i As Integer = 0 To 3
                assertEquals(3, collected(i).Count)
            Next i

            For Each example As IList(Of WritableT) In collected
                Dim exampleNum As Integer = example(0).toInt()
                Dim rank As Integer = example(2).toInt()
                assertEquals(exampleNum, rank)
            Next example
        End Sub

        ''' <summary>
        ''' Not in the Java test: the output is in rank order, the rank is a <c>LongWritable</c>, a descending rank reverses
        ''' the order, and ties keep their input order (stable sort).
        ''' </summary>
        <TestMethod>
        Public Sub testCalculateSortedRankOrderAndTies()
            Dim data As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("a"), New DoubleWritable(2.0)),
                row(New Text("b"), New DoubleWritable(1.0)),
                row(New Text("c"), New DoubleWritable(2.0)),
                row(New Text("d"), New DoubleWritable(0.5))
            }
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("TextCol").addColumnDouble("DoubleCol").build()

            Dim ascending As TransformProcessT = (New TransformProcessT.Builder(schema)).calculateSortedRank("rank", "DoubleCol", New DoubleWritableComparator()).build()
            Dim expAscending As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("d"), New DoubleWritable(0.5), New LongWritable(0)),
                row(New Text("b"), New DoubleWritable(1.0), New LongWritable(1)),
                row(New Text("a"), New DoubleWritable(2.0), New LongWritable(2)),
                row(New Text("c"), New DoubleWritable(2.0), New LongWritable(3))
            }
            assertEquals(expAscending, LocalTransformExecutorT.execute(data, ascending))

            Dim descending As TransformProcessT = (New TransformProcessT.Builder(schema)).calculateSortedRank("rank", "DoubleCol", New DoubleWritableComparator(), False).build()
            Dim expDescending As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("a"), New DoubleWritable(2.0), New LongWritable(0)),
                row(New Text("c"), New DoubleWritable(2.0), New LongWritable(1)),
                row(New Text("b"), New DoubleWritable(1.0), New LongWritable(2)),
                row(New Text("d"), New DoubleWritable(0.5), New LongWritable(3))
            }
            assertEquals(expDescending, LocalTransformExecutorT.execute(data, descending))
        End Sub

    End Class

End Namespace
