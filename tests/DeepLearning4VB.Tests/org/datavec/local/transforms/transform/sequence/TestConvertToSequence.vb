Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports LocalTransformExecutorT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformExecutor
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NumericalColumnComparator = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.transform.sequence

    ''' <summary>Port of the DataVec (datavec-local) <c>TestConvertToSequence</c> JUnit class.</summary>
    <TestClass>
    Public Class TestConvertToSequence

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        <TestMethod>
        Public Sub testConvertToSequenceCompoundKey()

            Dim s As SchemaT = (New SchemaT.Builder()).addColumnsString("key1", "key2").addColumnLong("time").build()

            Dim allExamples As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("k1a"), New Text("k2a"), New LongWritable(10)),
                row(New Text("k1b"), New Text("k2b"), New LongWritable(10)),
                row(New Text("k1a"), New Text("k2a"), New LongWritable(-10)),
                row(New Text("k1b"), New Text("k2b"), New LongWritable(5)),
                row(New Text("k1a"), New Text("k2a"), New LongWritable(0))
            }

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(s)).convertToSequence(New List(Of String) From {"key1", "key2"}, New NumericalColumnComparator("time")).build()

            Dim rdd As IList(Of IList(Of WritableT)) = allExamples

            Dim [out] As IList(Of IList(Of IList(Of WritableT))) = LocalTransformExecutorT.executeToSequence(rdd, tp)

            assertEquals(2, [out].Count)
            Dim seq0 As IList(Of IList(Of WritableT))
            Dim seq1 As IList(Of IList(Of WritableT))
            If [out](0).Count = 3 Then
                seq0 = [out](0)
                seq1 = [out](1)
            Else
                seq0 = [out](1)
                seq1 = [out](0)
            End If

            Dim expSeq0 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("k1a"), New Text("k2a"), New LongWritable(-10)),
                row(New Text("k1a"), New Text("k2a"), New LongWritable(0)),
                row(New Text("k1a"), New Text("k2a"), New LongWritable(10))
            }

            Dim expSeq1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("k1b"), New Text("k2b"), New LongWritable(5)),
                row(New Text("k1b"), New Text("k2b"), New LongWritable(10))
            }

            assertEquals(expSeq0, seq0)
            assertEquals(expSeq1, seq1)
        End Sub

        <TestMethod>
        Public Sub testConvertToSequenceLength1()

            Dim s As SchemaT = (New SchemaT.Builder()).addColumnsString("string").addColumnLong("long").build()

            Dim allExamples As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("a"), New LongWritable(0)),
                row(New Text("b"), New LongWritable(1)),
                row(New Text("c"), New LongWritable(2))
            }

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(s)).convertToSequence().build()

            Dim rdd As IList(Of IList(Of WritableT)) = allExamples

            'Java cast the result to an ArrowWritableRecordTimeSeriesBatch and called toArrayList(); the port returns a plain list
            Dim out2 As IList(Of IList(Of IList(Of WritableT))) = LocalTransformExecutorT.executeToSequence(rdd, tp)

            assertEquals(3, out2.Count)

            For i As Integer = 0 To 2
                Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {allExamples(i)}
                assertTrue(out2.Any(Function(x) DeepEquals(x, expected)))
            Next i
        End Sub

    End Class

End Namespace
