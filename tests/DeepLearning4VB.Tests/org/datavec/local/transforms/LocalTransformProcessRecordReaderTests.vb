Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports CategoricalColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.CategoricalColumnCondition
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource
Imports CollectionRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports ConditionOp = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports InMemorySequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.inmemory.InMemorySequenceRecordReader
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LocalTransformProcessRecordReaderT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformProcessRecordReader
Imports LocalTransformProcessSequenceRecordReaderT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformProcessSequenceRecordReader
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports RecordReaderT = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports RecordT = DeepLearning4VB.org.datavec.api.records.Record
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports SplitMaxLengthSequence = DeepLearning4VB.org.datavec.api.transform.sequence.split.SplitMaxLengthSequence
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms

    ''' <summary>
    ''' Port of the DataVec <c>LocalTransformProcessRecordReaderTests</c> JUnit class (the iris data is
    ''' <c>datavec-api/iris.dat</c>), plus a test of the sequence reader with a sequence split.
    ''' </summary>
    <TestClass>
    Public Class LocalTransformProcessRecordReaderTests

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        <TestMethod>
        Public Sub simpleTransformTest()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("0").addColumnDouble("1").addColumnDouble("2").addColumnDouble("3").addColumnDouble("4").build()
            Dim transformProcess As TransformProcessT = (New TransformProcessT.Builder(schema)).removeColumns("0").build()
            Dim csvRecordReader As New CSVRecordReader()
            csvRecordReader.initialize(New FileSplit((New ClassPathResource("datavec-api/iris.dat")).File))
            Dim transformProcessRecordReader As New LocalTransformProcessRecordReaderT(csvRecordReader, transformProcess)
            assertEquals(4, transformProcessRecordReader.next().Count)
        End Sub

        <TestMethod>
        Public Sub simpleTransformTestSequence()
            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            'First window:
            sequence.Add(row(New LongWritable(1451606400000L), New IntWritable(0), New IntWritable(0)))
            sequence.Add(row(New LongWritable(1451606400000L + 100L), New IntWritable(1), New IntWritable(0)))
            sequence.Add(row(New LongWritable(1451606400000L + 200L), New IntWritable(2), New IntWritable(0)))

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").addColumnInteger("intcolumn2").build()
            Dim transformProcess As TransformProcessT = (New TransformProcessT.Builder(schema)).removeColumns("intcolumn2").build()
            Dim inMemorySequenceRecordReader As New InMemorySequenceRecordReader(New List(Of IList(Of IList(Of WritableT))) From {sequence})
            Dim transformProcessSequenceRecordReader As New LocalTransformProcessSequenceRecordReaderT(inMemorySequenceRecordReader, transformProcess)
            Dim [next] As IList(Of IList(Of WritableT)) = transformProcessSequenceRecordReader.sequenceRecord()
            assertEquals(2, [next](0).Count)

            'Not in the Java test: the values themselves (time and integer columns keep their writable types)
            Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New LongWritable(1451606400000L), New IntWritable(0)),
                row(New LongWritable(1451606400100L), New IntWritable(1)),
                row(New LongWritable(1451606400200L), New IntWritable(2))
            }
            assertEquals(expected, [next])
        End Sub

        <TestMethod>
        Public Sub testLocalFilter()
            Dim [in] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            [in].Add(row(New Text("Keep"), New IntWritable(0)))
            [in].Add(row(New Text("Remove"), New IntWritable(1)))
            [in].Add(row(New Text("Keep"), New IntWritable(2)))
            [in].Add(row(New Text("Remove"), New IntWritable(3)))

            Dim s As SchemaT = (New SchemaT.Builder()).addColumnCategorical("cat", "Keep", "Remove").addColumnInteger("int").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(s)).filter(New CategoricalColumnCondition("cat", ConditionOp.Equal, "Remove")).build()

            Dim rr As RecordReaderT = New CollectionRecordReader([in])
            Dim ltprr As New LocalTransformProcessRecordReaderT(rr, tp)

            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Do While ltprr.hasNext()
                [out].Add(ltprr.next())
            Loop

            Dim exp As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {[in](0), [in](2)}
            assertEquals(exp, [out])

            'Check reset:
            ltprr.reset()
            [out].Clear()
            Do While ltprr.hasNext()
                [out].Add(ltprr.next())
            Loop
            assertEquals(exp, [out])

            'Also test Record method:
            Dim rl As IList(Of RecordT) = New List(Of RecordT)()
            rr.reset()
            Do While rr.hasNext()
                rl.Add(rr.nextRecord())
            Loop
            Dim exp2 As IList(Of RecordT) = New List(Of RecordT) From {rl(0), rl(2)}

            Dim act As IList(Of RecordT) = New List(Of RecordT)()
            ltprr.reset()
            Do While ltprr.hasNext()
                act.Add(ltprr.nextRecord())
            Loop
            'The Java test stopped here; check the values of the records as well
            assertEquals(exp2.Select(Function(r) r.getRecord()).ToList(), act.Select(Function(r) r.getRecord()).ToList())
        End Sub

        ''' <summary>Not in the Java tests: the sequence reader runs the whole executor, so sequence splits work too.</summary>
        <TestMethod>
        Public Sub testSequenceReaderWithSequenceSplit()
            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For i As Integer = 0 To 4
                sequence.Add(row(New IntWritable(i), New IntWritable(10 * i)))
            Next i
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnInteger("a").addColumnInteger("b").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).removeColumns("b").splitSequence(New SplitMaxLengthSequence(2, False)).build()
            Dim reader As New LocalTransformProcessSequenceRecordReaderT(New InMemorySequenceRecordReader(New List(Of IList(Of IList(Of WritableT))) From {sequence}), tp)

            'The reader returns the first of the split sequences
            Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New IntWritable(0)), row(New IntWritable(1))}
            assertEquals(expected, reader.sequenceRecord())
            assertFalse(reader.hasNext())
        End Sub

    End Class

End Namespace
