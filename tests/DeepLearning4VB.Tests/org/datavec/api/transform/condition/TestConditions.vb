Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.condition
Imports DeepLearning4VB.org.datavec.api.transform.condition.column
Imports DeepLearning4VB.org.datavec.api.writable
Imports ColumnType = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports ConditionT = DeepLearning4VB.org.datavec.api.transform.condition.Condition
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceLengthCondition = DeepLearning4VB.org.datavec.api.transform.condition.sequence.SequenceLengthCondition
Imports StringRegexColumnCondition = DeepLearning4VB.org.datavec.api.transform.condition.string.StringRegexColumnCondition
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.condition

    ''' <summary>Port of the DataVec <c>TestConditions</c> JUnit class.</summary>
    <TestClass>
    Public Class TestConditions

        ''' <summary>Java <c>Collections.singletonList((Writable) w)</c>.</summary>
        Private Shared Function row(ByVal w As WritableT) As IList(Of WritableT)
            Return JavaCollections.singletonList(Of WritableT)(w)
        End Function

        <TestMethod>
        Public Sub testIntegerCondition()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Integer])

            Dim condition As ConditionT = New IntegerColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.LessThan, 0)
            condition.InputSchema = schema

            assertTrue(condition.condition(row(New IntWritable(-1))))
            assertTrue(condition.condition(row(New IntWritable(-2))))
            assertFalse(condition.condition(row(New IntWritable(0))))
            assertFalse(condition.condition(row(New IntWritable(1))))

            Dim [set] As ISet(Of Integer) = New HashSet(Of Integer)()
            [set].Add(0)
            [set].Add(3)
            condition = New IntegerColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.InSet, [set])
            condition.InputSchema = schema
            assertTrue(condition.condition(row(New IntWritable(0))))
            assertTrue(condition.condition(row(New IntWritable(3))))
            assertFalse(condition.condition(row(New IntWritable(1))))
            assertFalse(condition.condition(row(New IntWritable(2))))
        End Sub

        <TestMethod>
        Public Sub testLongCondition()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Long])

            Dim condition As ConditionT = New LongColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.NotEqual, 5L)
            condition.InputSchema = schema

            assertTrue(condition.condition(row(New LongWritable(0))))
            assertTrue(condition.condition(row(New LongWritable(1))))
            assertFalse(condition.condition(row(New LongWritable(5))))

            Dim [set] As ISet(Of Long) = New HashSet(Of Long)()
            [set].Add(0L)
            [set].Add(3L)
            condition = New LongColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.NotInSet, [set])
            condition.InputSchema = schema
            assertTrue(condition.condition(row(New LongWritable(5))))
            assertTrue(condition.condition(row(New LongWritable(10))))
            assertFalse(condition.condition(row(New LongWritable(0))))
            assertFalse(condition.condition(row(New LongWritable(3))))
        End Sub

        <TestMethod>
        Public Sub testDoubleCondition()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Double])

            Dim condition As ConditionT = New DoubleColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.GreaterOrEqual, 0)
            condition.InputSchema = schema

            assertTrue(condition.condition(row(New DoubleWritable(0.0))))
            assertTrue(condition.condition(row(New DoubleWritable(0.5))))
            assertFalse(condition.condition(row(New DoubleWritable(-0.5))))
            assertFalse(condition.condition(row(New DoubleWritable(-1))))

            Dim [set] As ISet(Of Double) = New HashSet(Of Double)()
            [set].Add(0.0)
            [set].Add(3.0)
            condition = New DoubleColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.InSet, [set])
            condition.InputSchema = schema
            assertTrue(condition.condition(row(New DoubleWritable(0.0))))
            assertTrue(condition.condition(row(New DoubleWritable(3.0))))
            assertFalse(condition.condition(row(New DoubleWritable(1.0))))
            assertFalse(condition.condition(row(New DoubleWritable(2.0))))
        End Sub

        <TestMethod>
        Public Sub testFloatCondition()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.Float)

            Dim condition As ConditionT = New FloatColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.GreaterOrEqual, 0)
            condition.InputSchema = schema

            assertTrue(condition.condition(row(New FloatWritable(0.0F))))
            assertTrue(condition.condition(row(New FloatWritable(0.5F))))
            assertFalse(condition.condition(row(New FloatWritable(-0.5F))))
            assertFalse(condition.condition(row(New FloatWritable(-1.0F))))

            Dim [set] As ISet(Of Single) = New HashSet(Of Single)()
            [set].Add(0.0F)
            [set].Add(3.0F)
            condition = New FloatColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.InSet, [set])
            condition.InputSchema = schema
            assertTrue(condition.condition(row(New FloatWritable(0.0F))))
            assertTrue(condition.condition(row(New FloatWritable(3.0F))))
            assertFalse(condition.condition(row(New FloatWritable(1.0F))))
            assertFalse(condition.condition(row(New FloatWritable(2.0F))))
        End Sub

        <TestMethod>
        Public Sub testStringCondition()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Integer])

            Dim condition As ConditionT = New StringColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.Equal, "value")
            condition.InputSchema = schema

            assertTrue(condition.condition(row(New Text("value"))))
            assertFalse(condition.condition(row(New Text("not_value"))))

            Dim [set] As ISet(Of String) = New HashSet(Of String)()
            [set].Add("in set")
            [set].Add("also in set")
            condition = New StringColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.InSet, [set])
            condition.InputSchema = schema
            assertTrue(condition.condition(row(New Text("in set"))))
            assertTrue(condition.condition(row(New Text("also in set"))))
            assertFalse(condition.condition(row(New Text("not in the set"))))
            assertFalse(condition.condition(row(New Text(":)"))))
        End Sub

        <TestMethod>
        Public Sub testCategoricalCondition()
            Dim schema As SchemaT = (New SchemaT.Builder()).addColumnCategorical("column", "alpha", "beta", "gamma").build()

            Dim condition As ConditionT = New CategoricalColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.Equal, "alpha")
            condition.InputSchema = schema

            assertTrue(condition.condition(row(New Text("alpha"))))
            assertFalse(condition.condition(row(New Text("beta"))))
            assertFalse(condition.condition(row(New Text("gamma"))))

            Dim [set] As ISet(Of String) = New HashSet(Of String)()
            [set].Add("alpha")
            [set].Add("beta")
            condition = New StringColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.InSet, [set])
            condition.InputSchema = schema
            assertTrue(condition.condition(row(New Text("alpha"))))
            assertTrue(condition.condition(row(New Text("beta"))))
            assertFalse(condition.condition(row(New Text("gamma"))))
        End Sub

        <TestMethod>
        Public Sub testTimeCondition()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.Time)

            '1451606400000 = 01/01/2016 00:00:00 GMT
            Dim condition As ConditionT = New TimeColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.LessOrEqual, 1451606400000L)
            condition.InputSchema = schema

            assertTrue(condition.condition(row(New LongWritable(1451606400000L))))
            assertTrue(condition.condition(row(New LongWritable(1451606400000L - 1L))))
            assertFalse(condition.condition(row(New LongWritable(1451606400000L + 1L))))
            assertFalse(condition.condition(row(New LongWritable(1451606400000L + 1000L))))

            Dim [set] As ISet(Of Long) = New HashSet(Of Long)()
            [set].Add(1451606400000L)
            condition = New TimeColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.InSet, [set])
            condition.InputSchema = schema
            assertTrue(condition.condition(row(New LongWritable(1451606400000L))))
            assertFalse(condition.condition(row(New LongWritable(1451606400000L + 1L))))
        End Sub

        <TestMethod>
        Public Sub testStringRegexCondition()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[String])

            'Condition: String value starts with "abc"
            Dim condition As ConditionT = New StringRegexColumnCondition("column", "abc.*")
            condition.InputSchema = schema

            assertTrue(condition.condition(row(New Text("abc"))))
            assertTrue(condition.condition(row(New Text("abcdefghijk"))))
            assertTrue(condition.condition(row(New Text("abc more text " & vbTab & "etc"))))
            assertFalse(condition.condition(row(New Text("ab"))))
            assertFalse(condition.condition(row(New Text("also doesn't match"))))
            assertFalse(condition.condition(row(New Text(" abc"))))

            'Check application on non-String columns
            schema = TransformTestSupport.getSchema(ColumnType.[Integer])
            condition = New StringRegexColumnCondition("column", "123\d*")
            condition.InputSchema = schema

            assertTrue(condition.condition(row(New IntWritable(123))))
            assertTrue(condition.condition(row(New IntWritable(123456))))
            assertFalse(condition.condition(row(New IntWritable(-123))))
            assertFalse(condition.condition(row(New IntWritable(456789))))
        End Sub

        <TestMethod>
        Public Sub testNullWritableColumnCondition()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.Time)

            Dim condition As ConditionT = New NullWritableColumnCondition("column")
            condition.InputSchema = schema

            assertTrue(condition.condition(row(NullWritable.INSTANCE)))
            assertTrue(condition.condition(row(New NullWritable())))
            assertFalse(condition.condition(row(New IntWritable(0))))
            assertFalse(condition.condition(row(New Text("1"))))
        End Sub

        <TestMethod>
        Public Sub testBooleanConditionNot()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Integer])

            Dim condition As ConditionT = New IntegerColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.LessThan, 0)
            condition.InputSchema = schema

            Dim notCondition As ConditionT = BooleanCondition.[NOT](condition)
            notCondition.InputSchema = schema

            assertTrue(condition.condition(row(New IntWritable(-1))))
            assertTrue(condition.condition(row(New IntWritable(-2))))
            assertFalse(condition.condition(row(New IntWritable(0))))
            assertFalse(condition.condition(row(New IntWritable(1))))

            'Expect opposite for not condition:
            assertFalse(notCondition.condition(row(New IntWritable(-1))))
            assertFalse(notCondition.condition(row(New IntWritable(-2))))
            assertTrue(notCondition.condition(row(New IntWritable(0))))
            assertTrue(notCondition.condition(row(New IntWritable(1))))
        End Sub

        <TestMethod>
        Public Sub testBooleanConditionAnd()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Integer])

            Dim condition1 As ConditionT = New IntegerColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.LessThan, 0)
            condition1.InputSchema = schema

            Dim condition2 As ConditionT = New IntegerColumnCondition("column", SequenceConditionMode.[Or], ConditionOp.LessThan, -1)
            condition2.InputSchema = schema

            Dim andCondition As ConditionT = BooleanCondition.[AND](condition1, condition2)
            andCondition.InputSchema = schema

            assertFalse(andCondition.condition(row(New IntWritable(-1))))
            assertTrue(andCondition.condition(row(New IntWritable(-2))))
            assertFalse(andCondition.condition(row(New IntWritable(0))))
            assertFalse(andCondition.condition(row(New IntWritable(1))))
        End Sub

        <TestMethod>
        Public Sub testInvalidValueColumnConditionCondition()
            Dim schema As SchemaT = TransformTestSupport.getSchema(ColumnType.[Integer])

            Dim condition As ConditionT = New InvalidValueColumnCondition("column")
            condition.InputSchema = schema

            assertFalse(condition.condition(row(New IntWritable(-1)))) 'Not invalid -> condition does not apply
            assertFalse(condition.condition(row(New IntWritable(-2))))
            assertFalse(condition.condition(row(New LongWritable(1000))))
            assertFalse(condition.condition(row(New Text("1000"))))
            assertTrue(condition.condition(row(New Text("text"))))
            assertTrue(condition.condition(row(New Text("NaN"))))
            assertTrue(condition.condition(row(New LongWritable(1L + CLng(Integer.MaxValue)))))
            assertTrue(condition.condition(row(New DoubleWritable(3.14159))))
        End Sub

        <TestMethod>
        Public Sub testSequenceLengthCondition()
            Dim c As ConditionT = New SequenceLengthCondition(ConditionOp.LessThan, 2)

            Dim l1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(NullWritable.INSTANCE)}

            Dim l2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(NullWritable.INSTANCE), row(NullWritable.INSTANCE)}

            Dim l3 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT)) From {row(NullWritable.INSTANCE), row(NullWritable.INSTANCE), row(NullWritable.INSTANCE)}

            assertTrue(c.conditionSequence(l1))
            assertFalse(c.conditionSequence(l2))
            assertFalse(c.conditionSequence(l3))

            Dim [set] As ISet(Of Integer) = New HashSet(Of Integer)()
            [set].Add(2)
            c = New SequenceLengthCondition(ConditionOp.InSet, [set])
            assertFalse(c.conditionSequence(l1))
            assertTrue(c.conditionSequence(l2))
            assertFalse(c.conditionSequence(l3))
        End Sub

    End Class

End Namespace
