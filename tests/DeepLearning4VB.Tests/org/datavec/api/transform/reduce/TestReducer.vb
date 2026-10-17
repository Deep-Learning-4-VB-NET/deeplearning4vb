Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.ops
Imports AggregableReductionUtils = DeepLearning4VB.org.datavec.api.transform.reduce.AggregableReductionUtils
Imports ByteWritable = DeepLearning4VB.org.datavec.api.writable.ByteWritable
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConditionOp = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports DateTimeZone = DeepLearning4VB.org.joda.time.DateTimeZone
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports IAssociativeReducer = DeepLearning4VB.org.datavec.api.transform.reduce.IAssociativeReducer
Imports IntegerColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.IntegerColumnCondition
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongMetaData = DeepLearning4VB.org.datavec.api.transform.metadata.LongMetaData
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports Reducer = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports ReduceOp = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports StringColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.StringColumnCondition
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.reduce

    ''' <summary>
    ''' Tests of the <see cref="Reducer"/> builder ops, output schemas, errors, combining and equality, and of
    ''' <see cref="AggregableReductionUtils"/> for the time and bytes columns.
    ''' </summary>
    <TestClass>
    Public Class TestReducer

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        Private Shared Function wideSchema() As SchemaT
            Return (New SchemaT.Builder()).addColumnString("key").addColumnInteger("i").addColumnLong("l").addColumnDouble("d").addColumnFloat("f").addColumnTime("t", DateTimeZone.UTC).addColumnString("s").addColumnCategorical("c", "x", "y").build()
        End Function

        Private Shared Function wideReducer() As Reducer
            Return (New Reducer.Builder(ReduceOp.TakeFirst)).keyColumns("key").sumColumns("i").prodColumns("l").meanColumns("d").stdevColumns("d").minColumns("f").maxColumn("f").rangeColumns("t").countUniqueColumns("s").countColumns("c").build()
        End Function

        Private Shared Function reduceAll(ByVal reducer As IAssociativeReducer, ByVal rows As IList(Of IList(Of WritableT))) As IList(Of WritableT)
            Dim accu As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()
            For Each r As IList(Of WritableT) In rows
                accu.accept(r)
            Next r
            Return accu.get()
        End Function

        <TestMethod>
        Public Sub testOutputSchemaPerOp()
            Dim schema As SchemaT = wideSchema()
            Dim reducer As Reducer = wideReducer()

            Dim outSchema As SchemaT = reducer.transform(schema)
            assertEquals(New List(Of String) From {"key", "sum(i)", "prod(l)", "mean(d)", "stdev(d)", "min(f)", "max(f)", "range(t)", "countunique(s)", "count(c)"}, outSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.String, ColumnType.Integer, ColumnType.Long, ColumnType.Double, ColumnType.Double, ColumnType.Float, ColumnType.Float, ColumnType.Time, ColumnType.Long, ColumnType.Long}, outSchema.getColumnTypes())
            'Counts are longs with minimum 0
            assertEquals(New LongMetaData("count(c)", 0L, Nothing), outSchema.getMetaData("count(c)"))
            'The input metadata is not renamed
            assertEquals(New List(Of String) From {"key", "i", "l", "d", "f", "t", "s", "c"}, schema.getColumnNames())
            'A sequence schema stays a sequence schema
            Dim seqSchema As SchemaT = (New SequenceSchemaT.Builder()).addColumnInteger("i").build()
            assertTrue(TypeOf (New Reducer.Builder(ReduceOp.Sum)).build().transform(seqSchema) Is SequenceSchemaT)
        End Sub

        <TestMethod>
        Public Sub testValuesPerOp()
            Dim reducer As Reducer = wideReducer()
            reducer.InputSchema = wideSchema()
            Dim rows As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("k"), New IntWritable(1), New LongWritable(3), New DoubleWritable(1.0), New FloatWritable(1.5F), New LongWritable(1000), New Text("a"), New Text("x")),
                row(New Text("k"), New IntWritable(2), New LongWritable(4), New DoubleWritable(3.0), New FloatWritable(-0.5F), New LongWritable(4000), New Text("a"), New Text("y"))}
            Dim [out] As IList(Of WritableT) = reduceAll(reducer, rows)
            assertEquals(10, [out].Count)
            assertEquals(New Text("k"), [out](0))
            assertEquals(New IntWritable(3), [out](1))
            assertEquals(New LongWritable(12), [out](2))
            assertEquals(New DoubleWritable(2.0), [out](3))
            assertEquals(Math.Sqrt(2.0), [out](4).toDouble(), 0.000000001)
            assertEquals(New FloatWritable(-0.5F), [out](5))
            assertEquals(New FloatWritable(1.5F), [out](6))
            assertEquals(New LongWritable(3000), [out](7))
            assertEquals(New LongWritable(1), [out](8))
            assertEquals(New LongWritable(2), [out](9))
        End Sub

        <TestMethod>
        Public Sub testOtherBuilderOps()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("a").addColumnDouble("b").addColumnDouble("c").addColumnString("s").addColumnString("p").addColumnInteger("z").build()
            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).uncorrectedStdevColumns("a").variance("b").populationVariance("c").appendColumns("s").prependColumns("p").takeLastColumns("z").multipleOpColmumns(New List(Of ReduceOp) From {ReduceOp.Min, ReduceOp.Max}, "a").build()
            reducer.InputSchema = schema
            Dim outSchema As SchemaT = reducer.transform(schema)
            assertEquals(New List(Of String) From {"uncorrectedstddev(a)", "min(a)", "max(a)", "variance(b)", "populationvariance(c)", "append(s)", "prepend(p)", "takelast(z)"}, outSchema.getColumnNames())

            Dim rows As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New DoubleWritable(1), New DoubleWritable(1), New DoubleWritable(1), New Text("x"), New Text("x"), New IntWritable(1)),
                row(New DoubleWritable(3), New DoubleWritable(3), New DoubleWritable(3), New Text("y"), New Text("y"), New IntWritable(2))}
            Dim [out] As IList(Of WritableT) = reduceAll(reducer, rows)
            assertEquals(8, [out].Count)
            assertEquals(1.0, [out](0).toDouble(), 0.000000001) 'uncorrected stdev of {1, 3}
            assertEquals(1.0, [out](1).toDouble(), 0.0)
            assertEquals(3.0, [out](2).toDouble(), 0.0)
            assertEquals(2.0, [out](3).toDouble(), 0.000000001) 'variance (Bessel) of {1, 3}
            assertEquals(1.0, [out](4).toDouble(), 0.000000001) 'population variance of {1, 3}
            assertEquals("xy", [out](5).ToString())
            assertEquals("yx", [out](6).ToString())
            assertEquals(New IntWritable(2), [out](7))
        End Sub

        <TestMethod>
        Public Sub testConditionalReductionWithSeveralOps()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("v").build()
            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.TakeFirst)).keyColumns("key").conditionalReduction("v", New List(Of String) From {"minPos", "countPos"}, New List(Of ReduceOp) From {ReduceOp.Min, ReduceOp.Count}, New IntegerColumnCondition("v", ConditionOp.GreaterThan, 0)).build()
            reducer.InputSchema = schema
            Dim outSchema As SchemaT = reducer.transform(schema)
            assertEquals(New List(Of String) From {"key", "minPos", "countPos"}, outSchema.getColumnNames())
            assertEquals(New List(Of ColumnType) From {ColumnType.String, ColumnType.Integer, ColumnType.Long}, outSchema.getColumnTypes())

            Dim rows As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("k"), New IntWritable(-5)), row(New Text("k"), New IntWritable(7)), row(New Text("k"), New IntWritable(3)), row(New Text("k"), New IntWritable(0))}
            assertEquals(row(New Text("k"), New IntWritable(3), New LongWritable(2)), reduceAll(reducer, rows))

            'Names and reductions must match
            assertThrows(Of ArgumentException)(Sub() Call (New Reducer.Builder(ReduceOp.Sum)).conditionalReduction("v", New List(Of String) From {"a"}, New List(Of ReduceOp) From {ReduceOp.Min, ReduceOp.Max}, New IntegerColumnCondition("v", ConditionOp.GreaterThan, 0)))
        End Sub

        <TestMethod>
        Public Sub testCombine()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("a").addColumnDouble("b").addColumnLong("c").addColumnString("d").build()
            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).keyColumns("key").meanColumns("b").countColumns("c").takeLastColumns("d").conditionalReduction("a", "maxA", ReduceOp.Max, New StringColumnCondition("d", ConditionOp.NotEqual, "skip")).build()
            reducer.InputSchema = schema

            Dim rows As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {
                row(New Text("k"), New IntWritable(4), New DoubleWritable(1), New LongWritable(1), New Text("p")),
                row(New Text("k"), New IntWritable(9), New DoubleWritable(2), New LongWritable(1), New Text("skip")),
                row(New Text("k"), New IntWritable(2), New DoubleWritable(3), New LongWritable(1), New Text("q")),
                row(New Text("k"), New IntWritable(6), New DoubleWritable(6), New LongWritable(1), New Text("r"))}

            Dim all As IList(Of WritableT) = reduceAll(reducer, rows)
            assertEquals(row(New Text("k"), New IntWritable(6), New DoubleWritable(3), New LongWritable(4), New Text("r")), all)

            Dim first As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()
            first.accept(rows(0))
            first.accept(rows(1))
            Dim second As IAggregableReduceOp(Of IList(Of WritableT), IList(Of WritableT)) = reducer.aggregableReducer()
            second.accept(rows(2))
            second.accept(rows(3))
            first.combine(second)
            assertEquals(all, first.get())
        End Sub

        <TestMethod>
        Public Sub testErrors()
            'No schema
            Dim noSchema As Reducer = (New Reducer.Builder(ReduceOp.Sum)).build()
            assertThrows(Of InvalidOperationException)(Sub() noSchema.aggregableReducer())

            'Op not supported by the column type
            Dim unsupported As New List(Of Tuple(Of SchemaT, ReduceOp))() From {
                Tuple.Create((New SchemaT.Builder()).addColumnString("s").build(), ReduceOp.Sum),
                Tuple.Create((New SchemaT.Builder()).addColumnInteger("i").build(), ReduceOp.Append),
                Tuple.Create((New SchemaT.Builder()).addColumnTime("t", DateTimeZone.UTC).build(), ReduceOp.Sum),
                Tuple.Create((New SchemaT.Builder()).addColumnBoolean("b").build(), ReduceOp.TakeFirst)}
            For Each c As Tuple(Of SchemaT, ReduceOp) In unsupported
                Dim reducer As Reducer = (New Reducer.Builder(c.Item2)).build()
                reducer.InputSchema = c.Item1
                assertThrows(Of NotSupportedException)(Sub() reducer.aggregableReducer())
            Next c
        End Sub

        <TestMethod>
        Public Sub testTimeAndBytesColumns()
            Dim time As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = AggregableReductionUtils.reduceColumn(New List(Of ReduceOp) From {ReduceOp.Min, ReduceOp.Max, ReduceOp.Mean, ReduceOp.Count, ReduceOp.TakeFirst}, ColumnType.Time, False, Nothing)
            time.accept(New LongWritable(3000))
            time.accept(New LongWritable(1000))
            Dim timeOut As IList(Of WritableT) = time.get()
            assertEquals(New LongWritable(1000), timeOut(0))
            assertEquals(New LongWritable(3000), timeOut(1))
            assertEquals(2000.0, timeOut(2).toDouble(), 0.0)
            assertEquals(New LongWritable(2), timeOut(3))
            assertEquals(New LongWritable(3000), timeOut(4))

            Dim bytes As IAggregableReduceOp(Of WritableT, IList(Of WritableT)) = AggregableReductionUtils.reduceColumn(New List(Of ReduceOp) From {ReduceOp.TakeFirst, ReduceOp.TakeLast}, ColumnType.Bytes, False, Nothing)
            bytes.accept(New ByteWritable(CSByte(5)))
            bytes.accept(New ByteWritable(CSByte(-7)))
            assertEquals(New List(Of WritableT) From {New ByteWritable(CSByte(5)), New ByteWritable(CSByte(-7))}, bytes.get())

            assertThrows(Of NotSupportedException)(Sub() AggregableReductionUtils.reduceColumn(New List(Of ReduceOp) From {ReduceOp.Sum}, ColumnType.Bytes, False, Nothing))
            assertThrows(Of NotSupportedException)(Sub() AggregableReductionUtils.reduceColumn(New List(Of ReduceOp) From {ReduceOp.Prod}, ColumnType.Time, False, Nothing))
            assertThrows(Of NotSupportedException)(Sub() AggregableReductionUtils.reduceColumn(New List(Of ReduceOp) From {ReduceOp.Mean}, ColumnType.Categorical, False, Nothing))
        End Sub

        <TestMethod>
        Public Sub testEmptyGroup()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("v").build()
            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).keyColumns("key").countColumns("v").build()
            reducer.InputSchema = schema
            Dim [out] As IList(Of WritableT) = reducer.aggregableReducer().get()
            assertEquals(row(NullWritable.INSTANCE, New LongWritable(0)), [out])
        End Sub

        <TestMethod>
        Public Sub testAccessorsEqualityAndToString()
            Dim a As Reducer = (New Reducer.Builder(ReduceOp.Sum)).keyColumns("key").meanColumns("x").setIgnoreInvalid("x").build()
            Dim b As Reducer = (New Reducer.Builder(ReduceOp.Sum)).keyColumns("key").meanColumns("x").setIgnoreInvalid("x").build()
            Dim c As Reducer = (New Reducer.Builder(ReduceOp.Sum)).keyColumns("key").meanColumns("x").build()

            assertEquals(New List(Of String) From {"key"}, a.KeyColumns)
            assertEquals(New List(Of String) From {"key"}, a.getKeyColumns())
            assertEquals(ReduceOp.Sum, a.getDefaultOp())
            assertEquals(New List(Of ReduceOp) From {ReduceOp.Mean}, a.getOpMap()("x"))
            assertTrue(a.getIgnoreInvalidInColumns().Contains("x"))
            assertTrue(a.getKeyColumnsSet().Contains("key"))

            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertFalse(a.Equals(c))
            'The schema is not part of the equality
            a.InputSchema = (New SchemaT.Builder()).addColumnString("key").addColumnDouble("x").build()
            assertTrue(a.getSchema() IsNot Nothing)
            assertEquals(a, b)

            assertEquals("Reducer(keyColumns=[key],defaultOp=Sum,opMap={x=[Mean]},customReductions={},conditionalReductions={},ignoreInvalidInColumns=[x])", a.ToString())
        End Sub

        <TestMethod>
        Public Sub testConditionalReductionAccessors()
            Dim cond As New IntegerColumnCondition("v", ConditionOp.GreaterThan, 0)
            Dim cr As New Reducer.ConditionalReduction("v", New List(Of String) From {"out"}, New List(Of ReduceOp) From {ReduceOp.Max}, cond)
            assertEquals("v", cr.getColumnName())
            assertEquals(New List(Of String) From {"out"}, cr.getOutputNames())
            assertEquals(New List(Of ReduceOp) From {ReduceOp.Max}, cr.getReductions())
            assertTrue(cond Is cr.getCondition())
            Dim same As New Reducer.ConditionalReduction("v", New List(Of String) From {"out"}, New List(Of ReduceOp) From {ReduceOp.Max}, New IntegerColumnCondition("v", ConditionOp.GreaterThan, 0))
            assertEquals(same, cr)
            assertEquals(same.GetHashCode(), cr.GetHashCode())
            assertFalse(cr.Equals(New Reducer.ConditionalReduction("v", New List(Of String) From {"out"}, New List(Of ReduceOp) From {ReduceOp.Min}, cond)))

            'Setting the reducer's input schema sets the condition's input schema
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("v").build()
            Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).conditionalReduction("v", "out", ReduceOp.Max, cond).build()
            reducer.InputSchema = schema
            assertTrue(schema Is cond.InputSchema)
            assertTrue(reducer.getConditionalReductions().ContainsKey("v"))
        End Sub

    End Class

End Namespace
