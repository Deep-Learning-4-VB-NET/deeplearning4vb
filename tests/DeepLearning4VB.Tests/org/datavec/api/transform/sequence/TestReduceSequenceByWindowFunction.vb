Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports IAssociativeReducer = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.LongMetaData
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports Reducer = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports ReduceOp = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports ReduceSequenceByWindowTransform = DeepLearning4VB.org.datavec.api.transform.sequence.window.ReduceSequenceByWindowTransform
Imports ReduceSequenceTransformT = DeepLearning4VB.org.datavec.api.transform.sequence.ReduceSequenceTransform
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports TimeUnit = DeepLearning4VB.JavaCompat.TimeUnit
Imports TimeWindowFunction = DeepLearning4VB.org.datavec.api.transform.sequence.window.TimeWindowFunction
Imports TransformT = DeepLearning4VB.org.datavec.api.transform.Transform
Imports WindowFunction = DeepLearning4VB.org.datavec.api.transform.sequence.window.WindowFunction
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence

    ''' <summary>
    ''' Port of the DataVec <c>TestReduceSequenceByWindowFunction</c> JUnit class, plus tests of
    ''' <see cref="ReduceSequenceTransformT"/> (from <c>TestTransforms.testReduceSequenceTransform</c>) and the rest of
    ''' <see cref="ReduceSequenceByWindowTransform"/>.
    ''' </summary>
    <TestClass>
    Public Class TestReduceSequenceByWindowFunction

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        <TestMethod>
        Public Sub testReduceSequenceByWindowFunction()
            'Time windowing: 1 second (1000 milliseconds) window

            'Create some data.
            Dim sequence As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            'First window:
            sequence.Add(row(New LongWritable(1451606400000L), New IntWritable(0)))
            sequence.Add(row(New LongWritable(1451606400000L + 100L), New IntWritable(1)))
            sequence.Add(row(New LongWritable(1451606400000L + 200L), New IntWritable(2)))
            'Second window:
            sequence.Add(row(New LongWritable(1451606400000L + 1000L), New IntWritable(3)))
            'Third window: empty
            'Fourth window:
            sequence.Add(row(New LongWritable(1451606400000L + 3000L), New IntWritable(4)))
            sequence.Add(row(New LongWritable(1451606400000L + 3100L), New IntWritable(5)))

            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").build()

            Dim wf As WindowFunction = New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS)
            wf.InputSchema = schema

            'Now: reduce by summing...
            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).takeFirstColumns("timecolumn").build()

            Dim transform As TransformT = New ReduceSequenceByWindowTransform(reducer, wf)
            transform.InputSchema = schema

            Dim postApply As IList(Of IList(Of WritableT)) = transform.mapSequence(sequence)

            assertEquals(4, postApply.Count)

            Dim exp0 As IList(Of WritableT) = row(New LongWritable(1451606400000L), New IntWritable(0 + 1 + 2))
            assertEquals(exp0, postApply(0))

            Dim exp1 As IList(Of WritableT) = row(New LongWritable(1451606400000L + 1000L), New IntWritable(3))
            assertEquals(exp1, postApply(1))

            ' here, takefirst of an empty window -> nullwritable makes more sense
            Dim exp2 As IList(Of WritableT) = row(NullWritable.INSTANCE, NullWritable.INSTANCE)
            assertEquals(exp2, postApply(2))

            Dim exp3 As IList(Of WritableT) = row(New LongWritable(1451606400000L + 3000L), New IntWritable(9))
            assertEquals(exp3, postApply(3))
        End Sub

        ' ---- additional tests ----

        <TestMethod>
        Public Sub testReduceSequenceByWindowTransformSchema()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnTime("timecolumn", DateTimeZone.UTC).addColumnInteger("intcolumn").build()
            Dim wf As New TimeWindowFunction("timecolumn", 1, TimeUnit.SECONDS, True, False)
            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).takeFirstColumns("timecolumn", "windowStartTime").build()
            Dim t As New ReduceSequenceByWindowTransform(reducer, wf)
            t.InputSchema = schema
            'The reducer sees the window function's output schema
            assertEquals(wf.transform(schema), reducer.InputSchema)

            Dim outSchema As SchemaT = t.transform(schema)
            assertTrue(TypeOf outSchema Is SequenceSchemaT)
            assertEquals(New List(Of String) From {"takefirst(timecolumn)", "sum(intcolumn)", "takefirst(windowStartTime)"}, outSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.Time, ColumnType.Integer, ColumnType.Time}, outSchema.getColumnTypes())

            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New LongWritable(1100L), New IntWritable(1)),
                row(New LongWritable(1900L), New IntWritable(2)),
                row(New LongWritable(2500L), New IntWritable(4))}
            Dim result As IList(Of IList(Of WritableT)) = t.mapSequence(seq)
            assertEquals(2, result.Count)
            assertEquals(row(New LongWritable(1100L), New IntWritable(3), New LongWritable(1000L)), result(0))
            assertEquals(row(New LongWritable(2500L), New IntWritable(4), New LongWritable(2000L)), result(1))

            assertEquals(New String() {"timecolumn", "intcolumn"}, t.columnNames())
            assertEquals("timecolumn", t.columnName())
            assertEquals("timecolumn", t.outputColumnName())

            'Errors
            Dim nonSequence As SchemaT = (New SchemaT.Builder()).addColumnInteger("intcolumn").build()
            assertThrows(Of ArgumentException)(Sub() t.transform(nonSequence))
            assertThrows(Of NotSupportedException)(Sub() t.map(row(New IntWritable(1))))
            assertThrows(Of NotSupportedException)(Sub() t.map(CObj("x")))
            assertThrows(Of NotSupportedException)(Sub() t.mapSequence(CObj("x")))

            assertEquals(New ReduceSequenceByWindowTransform(reducer, wf), t)
            assertTrue(t.ToString().StartsWith("ReduceSequenceByWindowTransform(reducer=Reducer(", StringComparison.Ordinal))
        End Sub

        <TestMethod>
        Public Sub testReduceSequenceTransform()
            ' Java: TestTransforms.testReduceSequenceTransform
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnsDouble("col%d", 0, 2).build()

            Dim reducer As IAssociativeReducer = (New Reducer.Builder(ReduceOp.Mean)).countColumns("col1").maxColumn("col2").build()

            Dim t As New ReduceSequenceTransformT(reducer)
            t.InputSchema = schema

            Dim seq As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)),
                row(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)),
                row(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8))}

            Dim exp As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(New DoubleWritable(3), New LongWritable(3L), New DoubleWritable(8))}
            Dim act As IList(Of IList(Of WritableT)) = t.mapSequence(seq)
            assertEquals(exp, act)

            Dim expOutSchema As SchemaT = (New SequenceSchemaT.Builder()).addColumnDouble("mean(col0)").addColumn(New LongMetaData("count(col1)", 0L, Nothing)).addColumnDouble("max(col2)").build()

            assertEquals(expOutSchema, t.transform(schema))
        End Sub

        <TestMethod>
        Public Sub testReduceSequenceTransformOtherMembers()
            Dim schema As SchemaT = (New SequenceSchemaT.Builder()).addColumnsDouble("col%d", 0, 1).build()
            Dim reducer As IAssociativeReducer = (New Reducer.Builder(ReduceOp.Sum)).build()
            Dim t As New ReduceSequenceTransformT(reducer)
            t.InputSchema = schema
            assertTrue(schema Is reducer.InputSchema)
            assertTrue(reducer Is t.getReducer())
            assertEquals(New String() {"col0", "col1"}, t.columnNames())
            assertEquals(New String() {"col0", "col1"}, t.outputColumnNames())
            assertEquals("col0", t.columnName())

            'An empty sequence reduces to one time step of null values
            Dim act As IList(Of IList(Of WritableT)) = t.mapSequence(New List(Of IList(Of WritableT))())
            assertEquals(New List(Of IList(Of WritableT)) From {row(NullWritable.INSTANCE, NullWritable.INSTANCE)}, act)

            assertThrows(Of ArgumentException)(Sub() t.transform((New SchemaT.Builder()).addColumnDouble("col0").build()))
            assertThrows(Of NotSupportedException)(Sub() t.map(row(New DoubleWritable(1))))
            assertThrows(Of NotSupportedException)(Sub() t.map(CObj("x")))
            assertThrows(Of NotSupportedException)(Sub() t.mapSequence(CObj("x")))

            assertEquals(New ReduceSequenceTransformT((New Reducer.Builder(ReduceOp.Sum)).build()), t)
            assertFalse(t.Equals(New ReduceSequenceTransformT((New Reducer.Builder(ReduceOp.Mean)).build())))
            assertEquals("ReduceSequenceTransform(reducer=Reducer(defaultOp=Sum,opMap={},customReductions={},conditionalReductions={},ignoreInvalidInColumns=[]))", t.ToString())
        End Sub

    End Class

End Namespace
