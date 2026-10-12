Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports org.datavec.api.transform.condition.column
Imports SequenceLengthCondition = org.datavec.api.transform.condition.sequence.SequenceLengthCondition
Imports StringRegexColumnCondition = org.datavec.api.transform.condition.string.StringRegexColumnCondition
Imports Schema = org.datavec.api.transform.schema.Schema
Imports TestTransforms = org.datavec.api.transform.transform.TestTransforms
Imports org.datavec.api.writable
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertTrue

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.datavec.api.transform.condition


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestConditions extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestConditions
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIntegerCondition()
		Public Overridable Sub testIntegerCondition()
			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Integer)

			Dim condition As Condition = New IntegerColumnCondition("column", SequenceConditionMode.Or, ConditionOp.LessThan, 0)
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New IntWritable(-1), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New IntWritable(-2), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))

			Dim set As ISet(Of Integer) = New HashSet(Of Integer)()
			set.Add(0)
			set.Add(3)
			condition = New IntegerColumnCondition("column", SequenceConditionMode.Or, ConditionOp.InSet, set)
			condition.InputSchema = schema
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New IntWritable(3), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(2), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLongCondition()
		Public Overridable Sub testLongCondition()
			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Long)

			Dim condition As Condition = New LongColumnCondition("column", SequenceConditionMode.Or, ConditionOp.NotEqual, 5L)
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New LongWritable(0), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New LongWritable(1), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New LongWritable(5), Writable))))

			Dim set As ISet(Of Long) = New HashSet(Of Long)()
			set.Add(0L)
			set.Add(3L)
			condition = New LongColumnCondition("column", SequenceConditionMode.Or, ConditionOp.NotInSet, set)
			condition.InputSchema = schema
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New LongWritable(5), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New LongWritable(10), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New LongWritable(0), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New LongWritable(3), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDoubleCondition()
		Public Overridable Sub testDoubleCondition()
			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Double)

			Dim condition As Condition = New DoubleColumnCondition("column", SequenceConditionMode.Or, ConditionOp.GreaterOrEqual, 0)
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New DoubleWritable(0.0), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New DoubleWritable(0.5), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New DoubleWritable(-0.5), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New DoubleWritable(-1), Writable))))

			Dim set As ISet(Of Double) = New HashSet(Of Double)()
			set.Add(0.0)
			set.Add(3.0)
			condition = New DoubleColumnCondition("column", SequenceConditionMode.Or, ConditionOp.InSet, set)
			condition.InputSchema = schema
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New DoubleWritable(0.0), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New DoubleWritable(3.0), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New DoubleWritable(1.0), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New DoubleWritable(2.0), Writable))))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFloatCondition()
		Public Overridable Sub testFloatCondition()
			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Float)

			Dim condition As Condition = New FloatColumnCondition("column", SequenceConditionMode.Or, ConditionOp.GreaterOrEqual, 0)
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New FloatWritable(0.0f), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New FloatWritable(0.5f), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New FloatWritable(-0.5f), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New FloatWritable(-1f), Writable))))

			Dim set As ISet(Of Single) = New HashSet(Of Single)()
			set.Add(0.0f)
			set.Add(3.0f)
			condition = New FloatColumnCondition("column", SequenceConditionMode.Or, ConditionOp.InSet, set)
			condition.InputSchema = schema
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New FloatWritable(0.0f), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New FloatWritable(3.0f), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New FloatWritable(1.0f), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New FloatWritable(2.0f), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStringCondition()
		Public Overridable Sub testStringCondition()
			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Integer)

			Dim condition As Condition = New StringColumnCondition("column", SequenceConditionMode.Or, ConditionOp.Equal, "value")
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("value"), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text("not_value"), Writable))))

			Dim set As ISet(Of String) = New HashSet(Of String)()
			set.Add("in set")
			set.Add("also in set")
			condition = New StringColumnCondition("column", SequenceConditionMode.Or, ConditionOp.InSet, set)
			condition.InputSchema = schema
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("in set"), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("also in set"), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text("not in the set"), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text(":)"), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCategoricalCondition()
		Public Overridable Sub testCategoricalCondition()
			Dim schema As Schema = (New Schema.Builder()).addColumnCategorical("column", "alpha", "beta", "gamma").build()

			Dim condition As Condition = New CategoricalColumnCondition("column", SequenceConditionMode.Or, ConditionOp.Equal, "alpha")
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("alpha"), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text("beta"), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text("gamma"), Writable))))

			Dim set As ISet(Of String) = New HashSet(Of String)()
			set.Add("alpha")
			set.Add("beta")
			condition = New StringColumnCondition("column", SequenceConditionMode.Or, ConditionOp.InSet, set)
			condition.InputSchema = schema
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("alpha"), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("beta"), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text("gamma"), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTimeCondition()
		Public Overridable Sub testTimeCondition()
			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Time)

			'1451606400000 = 01/01/2016 00:00:00 GMT
			Dim condition As Condition = New TimeColumnCondition("column", SequenceConditionMode.Or, ConditionOp.LessOrEqual, 1451606400000L)
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New LongWritable(1451606400000L), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New LongWritable(1451606400000L - 1L), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New LongWritable(1451606400000L + 1L), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New LongWritable(1451606400000L + 1000L), Writable))))

			Dim set As ISet(Of Long) = New HashSet(Of Long)()
			set.Add(1451606400000L)
			condition = New TimeColumnCondition("column", SequenceConditionMode.Or, ConditionOp.InSet, set)
			condition.InputSchema = schema
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New LongWritable(1451606400000L), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New LongWritable(1451606400000L + 1L), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStringRegexCondition()
		Public Overridable Sub testStringRegexCondition()

			Dim schema As Schema = TestTransforms.getSchema(ColumnType.String)

			'Condition: String value starts with "abc"
			Dim condition As Condition = New StringRegexColumnCondition("column", "abc.*")
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("abc"), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("abcdefghijk"), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("abc more text " & vbTab & "etc"), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text("ab"), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text("also doesn't match"), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text(" abc"), Writable))))

			'Check application on non-String columns
			schema = TestTransforms.getSchema(ColumnType.Integer)
			condition = New StringRegexColumnCondition("column", "123\d*")
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New IntWritable(123), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New IntWritable(123456), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(-123), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(456789), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNullWritableColumnCondition()
		Public Overridable Sub testNullWritableColumnCondition()
			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Time)

			Dim condition As Condition = New NullWritableColumnCondition("column")
			condition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(NullWritable.INSTANCE, Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New NullWritable(), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text("1"), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBooleanConditionNot()
		Public Overridable Sub testBooleanConditionNot()

			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Integer)

			Dim condition As Condition = New IntegerColumnCondition("column", SequenceConditionMode.Or, ConditionOp.LessThan, 0)
			condition.InputSchema = schema

			Dim notCondition As Condition = BooleanCondition.NOT(condition)
			notCondition.InputSchema = schema

			assertTrue(condition.condition(Collections.singletonList(DirectCast(New IntWritable(-1), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New IntWritable(-2), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))

			'Expect opposite for not condition:
			assertFalse(notCondition.condition(Collections.singletonList(DirectCast(New IntWritable(-1), Writable))))
			assertFalse(notCondition.condition(Collections.singletonList(DirectCast(New IntWritable(-2), Writable))))
			assertTrue(notCondition.condition(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertTrue(notCondition.condition(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBooleanConditionAnd()
		Public Overridable Sub testBooleanConditionAnd()

			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Integer)

			Dim condition1 As Condition = New IntegerColumnCondition("column", SequenceConditionMode.Or, ConditionOp.LessThan, 0)
			condition1.InputSchema = schema

			Dim condition2 As Condition = New IntegerColumnCondition("column", SequenceConditionMode.Or, ConditionOp.LessThan, -1)
			condition2.InputSchema = schema

			Dim andCondition As Condition = BooleanCondition.AND(condition1, condition2)
			andCondition.InputSchema = schema

			assertFalse(andCondition.condition(Collections.singletonList(DirectCast(New IntWritable(-1), Writable))))
			assertTrue(andCondition.condition(Collections.singletonList(DirectCast(New IntWritable(-2), Writable))))
			assertFalse(andCondition.condition(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertFalse(andCondition.condition(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testInvalidValueColumnConditionCondition()
		Public Overridable Sub testInvalidValueColumnConditionCondition()
			Dim schema As Schema = TestTransforms.getSchema(ColumnType.Integer)

			Dim condition As Condition = New InvalidValueColumnCondition("column")
			condition.InputSchema = schema

			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(-1), Writable)))) 'Not invalid -> condition does not apply
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New IntWritable(-2), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New LongWritable(1000), Writable))))
			assertFalse(condition.condition(Collections.singletonList(DirectCast(New Text("1000"), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("text"), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New Text("NaN"), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New LongWritable(1L + CLng(Math.Truncate(Integer.MaxValue))), Writable))))
			assertTrue(condition.condition(Collections.singletonList(DirectCast(New DoubleWritable(3.14159), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceLengthCondition()
		Public Overridable Sub testSequenceLengthCondition()

			Dim c As Condition = New SequenceLengthCondition(ConditionOp.LessThan, 2)

			Dim l1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Collections.singletonList(Of Writable)(NullWritable.INSTANCE)}

			Dim l2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Collections.singletonList(Of Writable)(NullWritable.INSTANCE), Collections.singletonList(Of Writable)(NullWritable.INSTANCE)}

			Dim l3 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Collections.singletonList(Of Writable)(NullWritable.INSTANCE), Collections.singletonList(Of Writable)(NullWritable.INSTANCE), Collections.singletonList(Of Writable)(NullWritable.INSTANCE)}

			assertTrue(c.conditionSequence(l1))
			assertFalse(c.conditionSequence(l2))
			assertFalse(c.conditionSequence(l3))

			Dim set As ISet(Of Integer) = New HashSet(Of Integer)()
			set.Add(2)
			c = New SequenceLengthCondition(ConditionOp.InSet, set)
			assertFalse(c.conditionSequence(l1))
			assertTrue(c.conditionSequence(l2))
			assertFalse(c.conditionSequence(l3))

		End Sub
	End Class

End Namespace