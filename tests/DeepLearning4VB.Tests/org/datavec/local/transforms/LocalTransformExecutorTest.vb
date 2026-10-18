Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BooleanWritable = DeepLearning4VB.org.datavec.api.writable.BooleanWritable
Imports ConditionOp = DeepLearning4VB.org.datavec.api.transform.condition.ConditionOp
Imports DoubleColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.DoubleColumnCondition
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports DoubleWritableComparator = DeepLearning4VB.org.datavec.api.writable.comparator.DoubleWritableComparator
Imports FloatWritable = DeepLearning4VB.org.datavec.api.writable.FloatWritable
Imports IntegerColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.column.IntegerColumnCondition
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports JoinT = DeepLearning4VB.org.datavec.api.transform.join.Join
Imports LocalTransformExecutorT = DeepLearning4VB.org.datavec.local.transforms.LocalTransformExecutor
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports MathOp = DeepLearning4VB.org.datavec.api.transform.MathOp
Imports NullWritable = DeepLearning4VB.org.datavec.api.writable.NullWritable
Imports NumericalColumnComparator = DeepLearning4VB.org.datavec.api.transform.sequence.comparator.NumericalColumnComparator
Imports ReduceOp = DeepLearning4VB.org.datavec.api.transform.ReduceOp
Imports ReducerT = DeepLearning4VB.org.datavec.api.transform.reduce.Reducer
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceConditionMode = DeepLearning4VB.org.datavec.api.transform.condition.SequenceConditionMode
Imports SequenceSchemaT = DeepLearning4VB.org.datavec.api.transform.schema.SequenceSchema
Imports SplitMaxLengthSequence = DeepLearning4VB.org.datavec.api.transform.sequence.split.SplitMaxLengthSequence
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports TransformProcessT = DeepLearning4VB.org.datavec.api.transform.TransformProcess
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms

    ''' <summary>
    ''' Tests of the local executor that the DataVec tests do not cover: several kinds of steps in one process, empty
    ''' input, the order of the output, the conversion of the output to the schema's writable types, try/catch mode,
    ''' argument checks and the string conversion helpers.
    ''' </summary>
    <TestClass>
    Public Class LocalTransformExecutorTest

        Private Shared Function row(ByVal ParamArray values() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(values)
        End Function

        Private Shared Function rows(ByVal ParamArray values() As IList(Of WritableT)) As IList(Of IList(Of WritableT))
            Return New List(Of IList(Of WritableT))(values)
        End Function

        Private Shared Function keyTimeValueSchema() As SchemaT
            Return (New SchemaT.Builder()).addColumnString("key").addColumnLong("time").addColumnDouble("value").build()
        End Function

        ''' <summary>Filter, conversion to sequences by key, a sequence split, a transform and the conversion back, in one process.</summary>
        <TestMethod>
        Public Sub testFiltersAndSequenceConversionsInOneProcess()
            Dim input As IList(Of IList(Of WritableT)) = rows(
                row(New Text("b"), New LongWritable(3), New DoubleWritable(30)),
                row(New Text("a"), New LongWritable(2), New DoubleWritable(-1)),
                row(New Text("a"), New LongWritable(1), New DoubleWritable(10)),
                row(New Text("b"), New LongWritable(1), New DoubleWritable(10)),
                row(New Text("a"), New LongWritable(3), New DoubleWritable(30)),
                row(New Text("b"), New LongWritable(2), New DoubleWritable(20)),
                row(New Text("a"), New LongWritable(0), New DoubleWritable(0)))

            Dim tp As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())) _
                .filter(New DoubleColumnCondition("value", ConditionOp.LessThan, 0.0)) _
                .convertToSequence("key", New NumericalColumnComparator("time")) _
                .splitSequence(New SplitMaxLengthSequence(2, False)) _
                .doubleMathOp("value", MathOp.Multiply, 2.0) _
                .convertFromSequence() _
                .build()

            Dim out As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(input, tp)

            'Key "b" first (first in the input), each group sorted by time, split in two-step sequences, then flattened
            Dim expected As IList(Of IList(Of WritableT)) = rows(
                row(New Text("b"), New LongWritable(1), New DoubleWritable(20)),
                row(New Text("b"), New LongWritable(2), New DoubleWritable(40)),
                row(New Text("b"), New LongWritable(3), New DoubleWritable(60)),
                row(New Text("a"), New LongWritable(0), New DoubleWritable(0)),
                row(New Text("a"), New LongWritable(1), New DoubleWritable(20)),
                row(New Text("a"), New LongWritable(3), New DoubleWritable(60)))
            assertEquals(expected, out)
        End Sub

        <TestMethod>
        Public Sub testSequenceSplitAndFilterOnSequences()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())) _
                .convertToSequence("key", New NumericalColumnComparator("time")) _
                .splitSequence(New SplitMaxLengthSequence(2, False)) _
                .filter(New DoubleColumnCondition("value", SequenceConditionMode.And, ConditionOp.GreaterThan, 15.0)) _
                .build()

            Dim input As IList(Of IList(Of WritableT)) = rows(
                row(New Text("a"), New LongWritable(1), New DoubleWritable(10)),
                row(New Text("a"), New LongWritable(2), New DoubleWritable(20)),
                row(New Text("a"), New LongWritable(3), New DoubleWritable(30)),
                row(New Text("a"), New LongWritable(4), New DoubleWritable(40)),
                row(New Text("b"), New LongWritable(1), New DoubleWritable(50)))

            Dim out As IList(Of IList(Of IList(Of WritableT))) = LocalTransformExecutorT.executeToSequence(input, tp)

            'Sequences after the split: (a: 10, 20), (a: 30, 40), (b: 50). The filter removes the sequences in which
            'every value is greater than 15 (SequenceConditionMode.And), so only (a: 10, 20) is left.
            Dim expected As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT))) From {
                rows(row(New Text("a"), New LongWritable(1), New DoubleWritable(10)), row(New Text("a"), New LongWritable(2), New DoubleWritable(20)))
            }
            assertEquals(expected, out)
        End Sub

        <TestMethod>
        Public Sub testEmptyInput()
            Dim empty As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim emptySequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()

            Dim plain As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).doubleMathOp("value", MathOp.Add, 1.0).build()
            assertEquals(0, LocalTransformExecutorT.execute(empty, plain).Count)

            Dim toSequence As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).convertToSequence("key", New NumericalColumnComparator("time")).build()
            assertEquals(0, LocalTransformExecutorT.executeToSequence(empty, toSequence).Count)

            Dim reduce As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).reduce((New ReducerT.Builder(ReduceOp.Sum)).keyColumns("key").build()).build()
            assertEquals(0, LocalTransformExecutorT.execute(empty, reduce).Count)

            Dim rank As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).calculateSortedRank("rank", "value", New DoubleWritableComparator()).build()
            assertEquals(0, LocalTransformExecutorT.execute(empty, rank).Count)

            Dim sequenceSchema As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("key").addColumnLong("time").addColumnDouble("value").build()
            Dim sequenceTp As TransformProcessT = (New TransformProcessT.Builder(sequenceSchema)).doubleMathOp("value", MathOp.Add, 1.0).build()
            assertEquals(0, LocalTransformExecutorT.executeSequenceToSequence(emptySequences, sequenceTp).Count)

            Dim fromSequence As TransformProcessT = (New TransformProcessT.Builder(sequenceSchema)).convertFromSequence().build()
            assertEquals(0, LocalTransformExecutorT.executeSequenceToSeparate(emptySequences, fromSequence).Count)

            'An empty sequence stays an empty sequence
            Dim oneEmptySequence As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT))) From {New List(Of IList(Of WritableT))()}
            Dim result As IList(Of IList(Of IList(Of WritableT))) = LocalTransformExecutorT.executeSequenceToSequence(oneEmptySequence, sequenceTp)
            assertEquals(1, result.Count)
            assertEquals(0, result(0).Count)

            Dim leftSchema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("left").build()
            Dim rightSchema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("right").build()
            Dim join As JoinT = (New JoinT.Builder(JoinT.JoinType.FullOuter)).setJoinColumns("key").setSchemas(leftSchema, rightSchema).build()
            assertEquals(0, LocalTransformExecutorT.executeJoin(join, empty, empty).Count)
            Dim joined As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.executeJoin(join, rows(row(New Text("k"), New IntWritable(1))), empty)
            assertEquals(rows(row(New Text("k"), New IntWritable(1), NullWritable.INSTANCE)), joined)
        End Sub

        ''' <summary>Records keep their input order; groups come out in the order of the first occurrence of their key.</summary>
        <TestMethod>
        Public Sub testOutputOrder()
            Dim input As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim keys() As String = {"k3", "k1", "k2", "k1", "k3", "k4", "k2"}
            For i As Integer = 0 To keys.Length - 1
                input.Add(row(New Text(keys(i)), New LongWritable(100 - i), New DoubleWritable(i)))
            Next i

            'Transforms and filters keep the input order
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).doubleMathOp("value", MathOp.Add, 0.5).filter(New DoubleColumnCondition("value", ConditionOp.Equal, 3.5)).build()
            Dim out As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(input, tp)
            assertEquals(New List(Of Double) From {0.5, 1.5, 2.5, 4.5, 5.5, 6.5}, out.Select(Function(r) r(2).toDouble()).ToList())

            'Sequences: in the order of the first occurrence of each key; within a key, sorted by time
            Dim toSeq As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).convertToSequence("key", New NumericalColumnComparator("time")).build()
            Dim sequences As IList(Of IList(Of IList(Of WritableT))) = LocalTransformExecutorT.executeToSequence(input, toSeq)
            assertEquals(New List(Of String) From {"k3", "k1", "k2", "k4"}, sequences.Select(Function(s) s(0)(0).ToString()).ToList())
            assertEquals(New List(Of Double) From {4.0, 0.0}, sequences(0).Select(Function(r) r(2).toDouble()).ToList())

            'Reductions: one record per key, in the order of the first occurrence of each key
            Dim reduce As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).reduce((New ReducerT.Builder(ReduceOp.Sum)).keyColumns("key").build()).build()
            Dim reduced As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(input, reduce)
            assertEquals(New List(Of String) From {"k3", "k1", "k2", "k4"}, reduced.Select(Function(r) r(0).ToString()).ToList())
            assertEquals(New List(Of Double) From {4.0, 4.0, 8.0, 5.0}, reduced.Select(Function(r) r(2).toDouble()).ToList())

            'Joins: keys in the order of their first occurrence on the left, then the keys only on the right
            Dim leftSchema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("left").build()
            Dim rightSchema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("right").build()
            Dim join As JoinT = (New JoinT.Builder(JoinT.JoinType.FullOuter)).setJoinColumns("key").setSchemas(leftSchema, rightSchema).build()
            Dim left As IList(Of IList(Of WritableT)) = rows(row(New Text("b"), New IntWritable(1)), row(New Text("a"), New IntWritable(2)), row(New Text("b"), New IntWritable(3)))
            Dim right As IList(Of IList(Of WritableT)) = rows(row(New Text("c"), New IntWritable(10)), row(New Text("a"), New IntWritable(20)), row(New Text("b"), New IntWritable(30)), row(New Text("b"), New IntWritable(40)))
            Dim expected As IList(Of IList(Of WritableT)) = rows(
                row(New Text("b"), New IntWritable(1), New IntWritable(30)),
                row(New Text("b"), New IntWritable(1), New IntWritable(40)),
                row(New Text("b"), New IntWritable(3), New IntWritable(30)),
                row(New Text("b"), New IntWritable(3), New IntWritable(40)),
                row(New Text("a"), New IntWritable(2), New IntWritable(20)),
                row(New Text("c"), NullWritable.INSTANCE, New IntWritable(10)))
            assertEquals(expected, LocalTransformExecutorT.executeJoin(join, left, right))
        End Sub

        ''' <summary>The output values have the writable type of their column, as after Java's Arrow conversion.</summary>
        <TestMethod>
        Public Sub testOutputHasSchemaTypes()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("d").addColumnInteger("i").addColumnLong("l").addColumnFloat("f").addColumnString("s").addColumnBoolean("b").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).build()

            Dim input As IList(Of IList(Of WritableT)) = rows(
                row(New IntWritable(1), New LongWritable(2), New IntWritable(3), New DoubleWritable(4.5), New IntWritable(5), New IntWritable(1)),
                row(NullWritable.INSTANCE, New Text("7"), New Text("notALong"), New Text("2.5"), New Text("x"), New BooleanWritable(False)))
            Dim out As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(input, tp)

            Dim expected As IList(Of IList(Of WritableT)) = rows(
                row(New DoubleWritable(1.0), New IntWritable(2), New LongWritable(3), New FloatWritable(4.5F), New Text("5"), New BooleanWritable(True)),
                row(NullWritable.INSTANCE, New IntWritable(7), NullWritable.INSTANCE, New FloatWritable(2.5F), New Text("x"), New BooleanWritable(False)))
            assertEquals(expected, out)
            assertTrue(TypeOf out(0)(0) Is DoubleWritable)
            assertTrue(TypeOf out(0)(4) Is Text)
        End Sub

        ''' <summary>Java's executor dropped every record with as many values as join columns; the port keeps them.</summary>
        <TestMethod>
        Public Sub testJoinWithKeyOnlySide()
            Dim keysSchema As SchemaT = (New SchemaT.Builder()).addColumnString("key").build()
            Dim valuesSchema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("value").build()
            Dim join As JoinT = (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumns("key").setSchemas(keysSchema, valuesSchema).build()

            Dim keys As IList(Of IList(Of WritableT)) = rows(row(New Text("a")), row(New Text("c")))
            Dim values As IList(Of IList(Of WritableT)) = rows(row(New Text("a"), New IntWritable(1)), row(New Text("b"), New IntWritable(2)), row(New Text("c"), New IntWritable(3)))

            Dim expected As IList(Of IList(Of WritableT)) = rows(row(New Text("a"), New IntWritable(1)), row(New Text("c"), New IntWritable(3)))
            assertEquals(expected, LocalTransformExecutorT.executeJoin(join, keys, values))
        End Sub

        <TestMethod>
        Public Sub testJoinOnTwoColumns()
            Dim leftSchema As SchemaT = (New SchemaT.Builder()).addColumnString("k1").addColumnInteger("k2").addColumnDouble("left").build()
            Dim rightSchema As SchemaT = (New SchemaT.Builder()).addColumnInteger("k2").addColumnString("k1").addColumnDouble("right").build()
            Dim join As JoinT = (New JoinT.Builder(JoinT.JoinType.Inner)).setJoinColumns("k1", "k2").setSchemas(leftSchema, rightSchema).build()

            Dim left As IList(Of IList(Of WritableT)) = rows(row(New Text("a"), New IntWritable(1), New DoubleWritable(0.1)), row(New Text("a"), New IntWritable(2), New DoubleWritable(0.2)))
            Dim right As IList(Of IList(Of WritableT)) = rows(row(New IntWritable(2), New Text("a"), New DoubleWritable(2.0)), row(New IntWritable(1), New Text("b"), New DoubleWritable(1.0)))

            Dim expected As IList(Of IList(Of WritableT)) = rows(row(New Text("a"), New IntWritable(2), New DoubleWritable(0.2), New DoubleWritable(2.0)))
            assertEquals(expected, LocalTransformExecutorT.executeJoin(join, left, right))
        End Sub

        <TestMethod>
        Public Sub testTryCatchModeDropsFailedRecords()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("s").addColumnInteger("i").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).convertToInteger("s").build()
            Dim input As IList(Of IList(Of WritableT)) = rows(row(New Text("1"), New IntWritable(10)), row(New Text("notAnInt"), New IntWritable(20)), row(New Text("3"), New IntWritable(30)))

            assertFalse(LocalTransformExecutorT.isTryCatch())
            assertThrows(Of Exception)(Sub() LocalTransformExecutorT.execute(input, tp))

            JavaSystem.setProperty(LocalTransformExecutorT.LOG_ERROR_PROPERTY, "TRUE")
            Try
                assertTrue(LocalTransformExecutorT.isTryCatch())
                assertTrue(LocalTransformExecutorT.TryCatch)
                Dim out As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.execute(input, tp)
                assertEquals(rows(row(New IntWritable(1), New IntWritable(10)), row(New IntWritable(3), New IntWritable(30))), out)
            Finally
                JavaSystem.clearProperty(LocalTransformExecutorT.LOG_ERROR_PROPERTY)
            End Try
            assertFalse(LocalTransformExecutorT.isTryCatch())
        End Sub

        <TestMethod>
        Public Sub testColumnCountChecks()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).doubleMathOp("value", MathOp.Add, 1.0).build()
            'execute() drops the records with the wrong number of values
            Dim input As IList(Of IList(Of WritableT)) = rows(row(New Text("a"), New LongWritable(1)), row(New Text("b"), New LongWritable(2), New DoubleWritable(3.0)))
            assertEquals(rows(row(New Text("b"), New LongWritable(2), New DoubleWritable(4.0))), LocalTransformExecutorT.execute(input, tp))

            'executeToSequence() checks the first record
            Dim toSeq As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).convertToSequence("key", New NumericalColumnComparator("time")).build()
            assertThrows(Of InvalidOperationException)(Sub() LocalTransformExecutorT.executeToSequence(input, toSeq))

            Dim sequenceSchema As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("key").addColumnLong("time").addColumnDouble("value").build()
            Dim sequenceTp As TransformProcessT = (New TransformProcessT.Builder(sequenceSchema)).build()
            Dim badSequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT))) From {rows(row(New Text("a")))}
            assertThrows(Of InvalidOperationException)(Sub() LocalTransformExecutorT.executeSequenceToSequence(badSequences, sequenceTp))
        End Sub

        <TestMethod>
        Public Sub testWrongMethodForSchemaThrows()
            Dim empty As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim toSeq As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).convertToSequence("key", New NumericalColumnComparator("time")).build()
            assertThrows(Of InvalidOperationException)(Sub() LocalTransformExecutorT.execute(empty, toSeq))

            Dim plain As TransformProcessT = (New TransformProcessT.Builder(keyTimeValueSchema())).build()
            assertThrows(Of InvalidOperationException)(Sub() LocalTransformExecutorT.executeToSequence(empty, plain))

            Dim sequenceSchema As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("key").build()
            Dim sequenceTp As TransformProcessT = (New TransformProcessT.Builder(sequenceSchema)).build()
            assertThrows(Of InvalidOperationException)(Sub() LocalTransformExecutorT.executeSequenceToSeparate(New List(Of IList(Of IList(Of WritableT)))(), sequenceTp))
        End Sub

        <TestMethod>
        Public Sub testExecuteSequenceToSeparate()
            Dim sequenceSchema As SchemaT = (New SequenceSchemaT.Builder()).addColumnString("key").addColumnInteger("value").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(sequenceSchema)).convertFromSequence().filter(New IntegerColumnCondition("value", ConditionOp.Equal, 2)).build()
            Dim sequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT))) From {
                rows(row(New Text("a"), New IntWritable(1)), row(New Text("a"), New IntWritable(2))),
                rows(row(New Text("b"), New IntWritable(3)))
            }
            Dim expected As IList(Of IList(Of WritableT)) = rows(row(New Text("a"), New IntWritable(1)), row(New Text("b"), New IntWritable(3)))
            assertEquals(expected, LocalTransformExecutorT.executeSequenceToSeparate(sequences, tp))
        End Sub

        ''' <summary>With a non-sequence final schema, executeSequenceToSequence processes each sequence as a list of records.</summary>
        <TestMethod>
        Public Sub testExecuteSequenceToSequenceWithRecordSchema()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnString("key").addColumnInteger("value").build()
            Dim tp As TransformProcessT = (New TransformProcessT.Builder(schema)).filter(New IntegerColumnCondition("value", ConditionOp.Equal, 2)).build()
            Dim sequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT))) From {
                rows(row(New Text("a"), New IntWritable(1)), row(New Text("a"), New IntWritable(2))),
                rows(row(New Text("b"), New IntWritable(3)))
            }
            Dim expected As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT))) From {
                rows(row(New Text("a"), New IntWritable(1))),
                rows(row(New Text("b"), New IntWritable(3)))
            }
            assertEquals(expected, LocalTransformExecutorT.executeSequenceToSequence(sequences, tp))
        End Sub

        <TestMethod>
        Public Sub testStringConversions()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnDouble("d").addColumnFloat("f").addColumnInteger("i").addColumnLong("l").addColumnString("s").addColumnCategorical("c", "x", "y").addColumnBoolean("b").build()
            Dim strings As IList(Of IList(Of String)) = New List(Of IList(Of String)) From {
                New List(Of String) From {"1.5", "2.5", "-3", "4", "text", "y", "TRUE"},
                New List(Of String) From {"0", "0", "0", "0", "", "x", "no"}
            }
            Dim expected As IList(Of IList(Of WritableT)) = rows(
                row(New DoubleWritable(1.5), New FloatWritable(2.5F), New IntWritable(-3), New LongWritable(4), New Text("text"), New Text("y"), New BooleanWritable(True)),
                row(New DoubleWritable(0), New FloatWritable(0), New IntWritable(0), New LongWritable(0), New Text(""), New Text("x"), New BooleanWritable(False)))

            Dim records As IList(Of IList(Of WritableT)) = LocalTransformExecutorT.convertStringInput(strings, schema)
            assertEquals(expected, records)
            assertEquals(New List(Of IList(Of String)) From {
                New List(Of String) From {"1.5", "2.5", "-3", "4", "text", "y", "true"},
                New List(Of String) From {"0.0", "0.0", "0", "0", "", "x", "false"}
            }, LocalTransformExecutorT.convertWritableInputToString(records, schema))

            Dim timeSeries As IList(Of IList(Of IList(Of String))) = New List(Of IList(Of IList(Of String))) From {strings, New List(Of IList(Of String))()}
            Dim sequences As IList(Of IList(Of IList(Of WritableT))) = LocalTransformExecutorT.convertStringInputTimeSeries(timeSeries, schema)
            assertEquals(New List(Of IList(Of IList(Of WritableT))) From {expected, New List(Of IList(Of WritableT))()}, sequences)
            assertEquals(LocalTransformExecutorT.convertWritableInputToString(records, schema), LocalTransformExecutorT.convertWritableInputToStringTimeSeries(sequences, schema)(0))

            'Malformed numbers throw (Java: NumberFormatException)
            Dim bad As IList(Of IList(Of String)) = New List(Of IList(Of String)) From {New List(Of String) From {"x", "0", "0", "0", "", "x", "true"}}
            assertThrows(Of Exception)(Sub() LocalTransformExecutorT.convertStringInput(bad, schema))
        End Sub

    End Class

End Namespace
