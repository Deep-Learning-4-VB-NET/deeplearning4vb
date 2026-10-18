Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.local.transforms.functions
Imports DeepLearning4VB.org.datavec.local.transforms.functions.data
Imports DeepLearning4VB.org.datavec.local.transforms.misc
Imports DeepLearning4VB.org.datavec.local.transforms.misc.comparator
Imports DeepLearning4VB.org.datavec.local.transforms.rank
Imports DeepLearning4VB.org.datavec.local.transforms.reduce
Imports DeepLearning4VB.org.datavec.local.transforms.sequence
Imports DeepLearning4VB.org.datavec.local.transforms.transform
Imports DeepLearning4VB.org.datavec.local.transforms.transform.filter
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports AnalysisCombineFunctionT = DeepLearning4VB.org.datavec.local.transforms.analysis.aggregate.AnalysisCombineFunction
Imports AnalysisCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.AnalysisCounter
Imports AnalysisAddFunctionT = DeepLearning4VB.org.datavec.local.transforms.analysis.aggregate.AnalysisAddFunction
Imports BaseFlatMapFunctionAdapteeT = DeepLearning4VB.org.datavec.local.transforms.BaseFlatMapFunctionAdaptee(Of Integer, Integer)
Imports BytesWritable = DeepLearning4VB.org.datavec.api.writable.BytesWritable
Imports CategoricalAnalysisCounter = DeepLearning4VB.org.datavec.api.transform.analysis.counter.CategoricalAnalysisCounter
Imports ConditionOpT = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports DoubleHistogramCounter = DeepLearning4VB.org.datavec.api.transform.analysis.histogram.DoubleHistogramCounter
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports ExecuteJoinFromCoGroupFlatMapFunctionT = DeepLearning4VB.org.datavec.local.transforms.join.ExecuteJoinFromCoGroupFlatMapFunction
Imports ExecuteJoinFromCoGroupFlatMapFunctionAdapterT = DeepLearning4VB.org.datavec.local.transforms.join.ExecuteJoinFromCoGroupFlatMapFunctionAdapter
Imports ExtractKeysFunctionT = DeepLearning4VB.org.datavec.local.transforms.join.ExtractKeysFunction
Imports FilterAndFlattenJoinedValuesAdapterT = DeepLearning4VB.org.datavec.local.transforms.join.FilterAndFlattenJoinedValuesAdapter
Imports FilterAndFlattenJoinedValuesT = DeepLearning4VB.org.datavec.local.transforms.join.FilterAndFlattenJoinedValues
Imports HistogramCombineFunctionT = DeepLearning4VB.org.datavec.local.transforms.analysis.histogram.HistogramCombineFunction
Imports HistogramCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.histogram.HistogramCounter
Imports IntegerColumnConditionT = DeepLearning4VB.org.datavec.api.transform.condition.column.IntegerColumnCondition
Imports IntegerMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.IntegerMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports JoinedValueT = DeepLearning4VB.org.datavec.local.transforms.join.JoinedValue
Imports JoinT = DeepLearning4VB.org.datavec.api.transform.join.Join
Imports LocalTransformProcessRecordReaderT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformProcessRecordReader
Imports LocalTransformProcessSequenceRecordReaderT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformProcessSequenceRecordReader
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports NumericalColumnComparator = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports ReduceOp = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports ReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceEmptyRecordFunctionT = DeepLearning4VB.org.datavec.local.transforms.SequenceEmptyRecordFunction
Imports SequenceMergeT = DeepLearning4VB.org.datavec.api.transform.sequence.merge.SequenceMerge
Imports SplitMaxLengthSequence = DeepLearning4VB.org.datavec.api.transform.sequence.split.SplitMaxLengthSequence
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms

    ''' <summary>Tests of the function classes of the local executor (not covered by the DataVec tests).</summary>
    <TestClass>
    Public Class LocalFunctionsTest

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        Private Shared Function rows(ByVal ParamArray values() As IList(Of WritableT)) As IList(Of IList(Of WritableT))
            Return New List(Of IList(Of WritableT))(values)
        End Function

        Private Shared Function utf8Stream(ByVal s As String) As Stream
            Return New MemoryStream(Encoding.UTF8.GetBytes(s))
        End Function

        <TestMethod>
        Public Sub testEmptyRecordFunctions()
            assertTrue((New EmptyRecordFunction()).apply(New List(Of WritableT)()))
            assertFalse((New EmptyRecordFunction()).apply(row(New IntWritable(1))))
            assertTrue((New SequenceEmptyRecordFunctionT()).apply(New List(Of IList(Of WritableT))()))
            assertFalse((New SequenceEmptyRecordFunctionT()).apply(rows(row())))
        End Sub

        <TestMethod>
        Public Sub testKeyFunctions()
            Dim record As IList(Of WritableT) = row(New Text("a"), New IntWritable(1), New DoubleWritable(2.5))

            Dim singleKey As Pair(Of IList(Of WritableT), IList(Of WritableT)) = (New ExtractKeysFunctionT(New Integer() {1})).apply(record)
            assertEquals(row(New IntWritable(1)), singleKey.First)
            assertTrue(singleKey.Second Is record)
            Dim multi As Pair(Of IList(Of WritableT), IList(Of WritableT)) = (New ExtractKeysFunctionT(New Integer() {2, 0})).apply(record)
            assertEquals(row(New DoubleWritable(2.5), New Text("a")), multi.First)

            assertEquals(row(New Text("a"), New DoubleWritable(2.5)), (New LocalMapToPairByMultipleColumnsFunction(New Integer() {0, 2})).apply(record).First)
            assertEquals(New IntWritable(1), (New LocalMapToPairByColumnFunction(1)).apply(record).First)
            assertEquals(New DoubleWritable(2.5), (New ColumnAsKeyPairFunction(2)).apply(record).First)

            Dim counted As Pair(Of WritableT, Long) = (New ColumnToKeyPairTransform(0)).apply(record)
            assertEquals(New Text("a"), counted.First)
            assertEquals(1L, counted.Second)
            assertEquals(5L, (New SumLongsFunction2()).apply(Pair.of(2L, 3L)))
        End Sub

        <TestMethod>
        Public Sub testTuple2Comparator()
            Dim pairs As IList(Of Pair(Of String, Long)) = New List(Of Pair(Of String, Long)) From {Pair.of("a", 3L), Pair.of("b", 1L), Pair.of("c", 2L)}
            assertEquals(New List(Of String) From {"b", "c", "a"}, pairs.OrderBy(Function(p) p, New Tuple2Comparator(Of String)(True)).Select(Function(p) p.First).ToList())
            assertEquals(New List(Of String) From {"a", "c", "b"}, pairs.OrderBy(Function(p) p, New Tuple2Comparator(Of String)(False)).Select(Function(p) p.First).ToList())
        End Sub

        <TestMethod>
        Public Sub testMapToPairForReducerFunction()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("k1").addColumnInteger("k2").addColumnDouble("v").build()
            Dim record As IList(Of WritableT) = row(New Text("a"), New IntWritable(7), New DoubleWritable(1.0))

            Dim globalReducer As ReducerT = (New ReducerT.Builder(ReduceOp.Sum)).build()
            globalReducer.InputSchema = schema
            assertEquals(MapToPairForReducerFunction.GLOBAL_KEY, (New MapToPairForReducerFunction(globalReducer)).apply(record).First)

            Dim oneKey As ReducerT = (New ReducerT.Builder(ReduceOp.Sum)).keyColumns("k2").build()
            oneKey.InputSchema = schema
            assertEquals("7", (New MapToPairForReducerFunction(oneKey)).apply(record).First)

            Dim twoKeys As ReducerT = (New ReducerT.Builder(ReduceOp.Sum)).keyColumns("k2", "k1").build()
            twoKeys.InputSchema = schema
            Dim keyed As Pair(Of String, IList(Of WritableT)) = (New MapToPairForReducerFunction(twoKeys)).apply(record)
            assertEquals("7_a", keyed.First)
            assertTrue(keyed.Second Is record)
        End Sub

        ''' <summary>The Java ReducerFunction fed each record to a new aggregation; the port reduces all the records.</summary>
        <TestMethod>
        Public Sub testReducerFunction()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("k").addColumnDouble("v").build()
            Dim reducer As ReducerT = (New ReducerT.Builder(ReduceOp.Sum)).keyColumns("k").build()
            reducer.InputSchema = schema
            Dim reduced As IList(Of WritableT) = (New ReducerFunction(reducer)).apply(rows(row(New Text("a"), New DoubleWritable(1.5)), row(New Text("a"), New DoubleWritable(2.0))))
            assertEquals(2, reduced.Count)
            assertEquals(New Text("a"), reduced(0))
            assertEquals(3.5, reduced(1).toDouble(), 0.0)
        End Sub

        <TestMethod>
        Public Sub testSequenceFunctions()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("k").addColumnLong("t").build()
            Dim comparator As New NumericalColumnComparator("t")
            comparator.Schema = schema

            'Stable sort by time; without a comparator the order is kept
            Dim group As IList(Of IList(Of WritableT)) = rows(row(New Text("x"), New LongWritable(3)), row(New Text("y"), New LongWritable(1)), row(New Text("z"), New LongWritable(3)))
            Dim sorted As IList(Of IList(Of WritableT)) = (New LocalGroupToSequenceFunction(comparator)).apply(group)
            assertEquals(New List(Of String) From {"y", "x", "z"}, sorted.Select(Function(r) r(0).ToString()).ToList())
            assertEquals(group, (New LocalGroupToSequenceFunction(Nothing)).apply(group))
            assertEquals(3, group.Count)

            Dim one As IList(Of IList(Of WritableT)) = (New ConvertToSequenceLengthOne()).apply(group(0))
            assertEquals(1, one.Count)
            assertTrue(one(0) Is group(0))

            'Merge two sequences by time
            Dim merge As New SequenceMergeFunction(Of String)(New SequenceMergeT(comparator))
            Dim seqA As IList(Of IList(Of WritableT)) = rows(row(New Text("a"), New LongWritable(1)), row(New Text("a"), New LongWritable(4)))
            Dim seqB As IList(Of IList(Of WritableT)) = rows(row(New Text("b"), New LongWritable(2)))
            Dim merged As IList(Of IList(Of WritableT)) = merge.apply(Pair.of(Of String, IEnumerable(Of IList(Of IList(Of WritableT))))("key", New List(Of IList(Of IList(Of WritableT))) From {seqA, seqB}))
            assertEquals(New List(Of Long) From {1L, 2L, 4L}, merged.Select(Function(r) r(1).toLong()).ToList())
        End Sub

        <TestMethod>
        Public Sub testTransformAndSplitFunctions()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("k").addColumnInteger("v").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).filter(New IntegerColumnConditionT("v", ConditionOpT.LessThan, 0)).removeColumns("k").build()

            Dim fn As New LocalTransformProcessFunction(tp)
            assertEquals(rows(row(New IntWritable(5))), fn.call(row(New Text("a"), New IntWritable(5))))
            assertEquals(0, fn.call(row(New Text("a"), New IntWritable(-5))).Count)

            Dim filterFn As New LocalFilterFunction(tp.getActionList()(0).getFilter())
            assertTrue(filterFn.apply(row(New Text("a"), New IntWritable(5))))
            assertFalse(filterFn.apply(row(New Text("a"), New IntWritable(-5))))

            Dim transformFn As New LocalTransformFunction(tp.getActionList()(1).getTransform())
            assertEquals(row(New IntWritable(5)), transformFn.apply(row(New Text("a"), New IntWritable(5))))

            Dim split As New SequenceSplitFunction(New SplitMaxLengthSequence(2, False))
            Dim parts As IList(Of IList(Of IList(Of WritableT))) = split.call(rows(row(New IntWritable(1)), row(New IntWritable(2)), row(New IntWritable(3))))
            assertEquals(2, parts.Count)
            assertEquals(rows(row(New IntWritable(3))), parts(1))

            Dim unzip As IList(Of WritableT) = (New UnzipForCalculateSortedRankFunction()).apply(Pair.of(Pair.of(Of WritableT, IList(Of WritableT))(New IntWritable(9), row(New IntWritable(9))), 4L))
            assertEquals(row(New IntWritable(9), New LongWritable(4)), unzip)
        End Sub

        <TestMethod>
        Public Sub testAdapteeWrapsExceptions()
            Dim fn As New BaseFlatMapFunctionAdapteeT(New FailingAdapter())
            assertEquals(New List(Of Integer) From {2, 2}, fn.call(2))
            Dim e As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() fn.call(-1))
            assertTrue(TypeOf e.InnerException Is ArgumentException)
        End Sub

        ''' <summary>Returns n copies of n; throws for a negative n.</summary>
        Private NotInheritable Class FailingAdapter
            Implements FlatMapFunctionAdapter(Of Integer, Integer)

            Public Function [call](ByVal t As Integer) As IList(Of Integer) Implements FlatMapFunctionAdapter(Of Integer, Integer).call
                If t < 0 Then
                    Throw New ArgumentException("negative")
                End If
                Return Enumerable.Repeat(t, t).ToList()
            End Function

        End Class

        <TestMethod>
        Public Sub testFilterWritablesBySchemaFunction()
            Dim meta As New IntegerMetaData("i", 0, 10)
            Dim keepValid As New FilterWritablesBySchemaFunction(meta, True)
            Dim keepInvalid As New FilterWritablesBySchemaFunction(meta, False)
            Dim keepInvalidNotMissing As New FilterWritablesBySchemaFunction(meta, False, True)

            assertTrue(keepValid.apply(New IntWritable(5)))
            assertFalse(keepValid.apply(New IntWritable(50)))
            assertTrue(keepInvalid.apply(New IntWritable(50)))
            assertTrue(keepInvalid.apply(NullWritable.INSTANCE))
            assertTrue(keepInvalid.apply(New Text("")))
            assertFalse(keepInvalidNotMissing.apply(NullWritable.INSTANCE))
            assertFalse(keepInvalidNotMissing.apply(New Text("")))
            assertTrue(keepInvalidNotMissing.apply(New Text("abc")))
        End Sub

        <TestMethod>
        Public Sub testJoinFunctions()
            Dim leftSchema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("left").build()
            Dim rightSchema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("right").build()
            Dim key As IList(Of WritableT) = row(New Text("k"))
            Dim lefts As IList(Of IList(Of WritableT)) = rows(row(New Text("k"), New IntWritable(1)), row(New Text("k"), New IntWritable(2)))
            Dim rights As IList(Of IList(Of WritableT)) = rows(row(New Text("k"), New IntWritable(10)))
            Dim none As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()

            For Each jt As JoinT.JoinType In [Enum].GetValues(GetType(JoinT.JoinType))
                Dim join As JoinT = (New JoinT.Builder(jt)).setJoinColumns("key").setSchemas(leftSchema, rightSchema).build()
                Dim fn As New ExecuteJoinFromCoGroupFlatMapFunctionT(join)

                'Both sides present: the cross product, left records in the outer loop
                assertEquals(rows(row(New Text("k"), New IntWritable(1), New IntWritable(10)), row(New Text("k"), New IntWritable(2), New IntWritable(10))), fn.call(Pair.of(key, Pair.of(lefts, rights))))

                Dim leftOnly As IList(Of IList(Of WritableT)) = fn.call(Pair.of(key, Pair.of(lefts, none)))
                Dim rightOnly As IList(Of IList(Of WritableT)) = fn.call(Pair.of(key, Pair.of(none, rights)))
                Select Case jt
                    Case JoinT.JoinType.Inner
                        assertEquals(0, leftOnly.Count)
                        assertEquals(0, rightOnly.Count)
                    Case JoinT.JoinType.LeftOuter
                        assertEquals(rows(row(New Text("k"), New IntWritable(1), NullWritable.INSTANCE), row(New Text("k"), New IntWritable(2), NullWritable.INSTANCE)), leftOnly)
                        assertEquals(0, rightOnly.Count)
                    Case JoinT.JoinType.RightOuter
                        assertEquals(0, leftOnly.Count)
                        assertEquals(rows(row(New Text("k"), NullWritable.INSTANCE, New IntWritable(10))), rightOnly)
                    Case JoinT.JoinType.FullOuter
                        assertEquals(2, leftOnly.Count)
                        assertEquals(rows(row(New Text("k"), NullWritable.INSTANCE, New IntWritable(10))), rightOnly)
                End Select

                'Filtering of joined values by join type
                Dim flatten As New FilterAndFlattenJoinedValuesT(jt)
                Dim values As IList(Of WritableT) = row(New IntWritable(1))
                Dim both As IList(Of IList(Of WritableT)) = flatten.call(New JoinedValueT(True, True, values))
                Dim leftValue As IList(Of IList(Of WritableT)) = flatten.call(New JoinedValueT(True, False, values))
                Dim rightValue As IList(Of IList(Of WritableT)) = flatten.call(New JoinedValueT(False, True, values))
                assertEquals(1, both.Count)
                assertEquals(If(jt = JoinT.JoinType.LeftOuter OrElse jt = JoinT.JoinType.FullOuter, 1, 0), leftValue.Count)
                assertEquals(If(jt = JoinT.JoinType.RightOuter OrElse jt = JoinT.JoinType.FullOuter, 1, 0), rightValue.Count)
            Next jt
        End Sub

        <TestMethod>
        Public Sub testJoinedValue()
            Dim a As New JoinedValueT(True, False, row(New IntWritable(1), NullWritable.INSTANCE))
            Dim b As New JoinedValueT(True, False, row(New IntWritable(1), NullWritable.INSTANCE))
            Dim c As New JoinedValueT(True, True, row(New IntWritable(1), NullWritable.INSTANCE))
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertNotEquals(a, c)
            assertTrue(a.isHaveLeft())
            assertFalse(a.isHaveRight())
            assertTrue(a.HaveLeft)
            assertEquals(2, a.getValues().Count)
            assertTrue(a.ToString().StartsWith("JoinedValue(haveLeft=true, haveRight=false, values=[", StringComparison.Ordinal))
        End Sub

        <TestMethod>
        Public Sub testRecordReaderFunctions()
            Dim recordFn As New RecordReaderFunction(New CSVRecordReader())
            assertEquals(row(New Text("1"), New Text("2.5"), New Text("a")), recordFn.apply(Pair.of("data/file.csv", utf8Stream("1,2.5,a" & vbLf & "ignored"))))

            Dim sequenceFn As New SequenceRecordReaderFunction(New CSVSequenceRecordReader())
            Dim sequence As IList(Of IList(Of WritableT)) = sequenceFn.apply(Pair.of("file:///C:/data/seq.csv", utf8Stream("1,a" & vbLf & "2,b" & vbLf)))
            assertEquals(rows(row(New Text("1"), New Text("a")), row(New Text("2"), New Text("b"))), sequence)

            'Files as bytes, then read back
            Dim bytes As Pair(Of Text, BytesWritable) = (New FilesAsBytesFunction()).apply(Pair.of("seq.csv", utf8Stream("3,c" & vbLf & "4,d")))
            assertEquals(New Text("seq.csv"), bytes.First)
            assertEquals(Encoding.UTF8.GetBytes("3,c" & vbLf & "4,d"), bytes.Second.getContent())
            assertEquals(row(New Text("3"), New Text("c")), (New RecordReaderBytesFunction(New CSVRecordReader())).apply(bytes))
            assertEquals(rows(row(New Text("3"), New Text("c")), row(New Text("4"), New Text("d"))), (New SequenceRecordReaderBytesFunction(New CSVSequenceRecordReader())).apply(bytes))
        End Sub

        <TestMethod>
        Public Sub testStringToWritablesFunction()
            Dim fn As New StringToWritablesFunction(New CSVRecordReader())
            assertEquals(row(New Text("1"), New Text("x")), fn.apply("1,x"))
            assertEquals(row(New Text("2"), New Text("y")), fn.apply("2,y"))
            Dim lineFn As New LineRecordReaderFunction(New CSVRecordReader())
            assertEquals(row(New Text("3"), New Text("z")), lineFn.apply("3,z"))
        End Sub

        <TestMethod>
        Public Sub testAnalysisCombineFunctions()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnCategorical("c", "x", "y").build()
            Dim add As New AnalysisAddFunctionT(schema)
            Dim part1 As IList(Of AnalysisCounterT) = add.apply(Nothing, row(New Text("x")))
            Dim part2 As IList(Of AnalysisCounterT) = add.apply(Nothing, row(New Text("y")))
            part2 = add.apply(part2, row(New Text("y")))

            Dim combine As New AnalysisCombineFunctionT()
            assertTrue(combine.apply(Nothing, part1) Is part1)
            assertTrue(combine.apply(part1, Nothing) Is part1)
            Dim merged As CategoricalAnalysisCounter = DirectCast(combine.apply(part1, part2)(0), CategoricalAnalysisCounter)
            assertEquals(3L, merged.getCountTotal())
            assertEquals(2L, merged.getCounts()("y"))
            assertThrows(Of InvalidOperationException)(Sub() combine.apply(part1, New List(Of AnalysisCounterT)()))
            assertThrows(Of InvalidOperationException)(Sub() add.apply(Nothing, row(New Text("x"), New Text("y"))))

            Dim h1 As IList(Of HistogramCounterT) = New List(Of HistogramCounterT) From {(New DoubleHistogramCounter(0, 10, 2)).add(New DoubleWritable(1)), Nothing}
            Dim h2 As IList(Of HistogramCounterT) = New List(Of HistogramCounterT) From {(New DoubleHistogramCounter(0, 10, 2)).add(New DoubleWritable(9)), (New DoubleHistogramCounter(0, 1, 1)).add(New DoubleWritable(0.5))}
            Dim hMerged As IList(Of HistogramCounterT) = (New HistogramCombineFunctionT()).apply(h1, h2)
            assertEquals(New Long() {1L, 1L}, hMerged(0).Counts)
            assertTrue(hMerged(1) Is h2(1))
            assertTrue((New HistogramCombineFunctionT()).apply(Nothing, h2) Is h2)
        End Sub

        <TestMethod>
        Public Sub testSerializableClasses()
            For Each t As Type In New Type() {GetType(JoinedValueT), GetType(Tuple2Comparator(Of String)), GetType(LocalTransformProcessRecordReaderT), GetType(LocalTransformProcessSequenceRecordReaderT), GetType(LocalTransformProcessFunctionAdapter), GetType(SequenceSplitFunctionAdapter), GetType(ExecuteJoinFromCoGroupFlatMapFunctionAdapterT), GetType(FilterAndFlattenJoinedValuesAdapterT)}
                assertTrue(isSerializableType(t), t.Name & " should be serializable")
            Next t
        End Sub

    End Class

End Namespace
