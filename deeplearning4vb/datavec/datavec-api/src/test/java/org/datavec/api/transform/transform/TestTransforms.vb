Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports TestCase = junit.framework.TestCase
Imports org.datavec.api.transform
Imports Condition = org.datavec.api.transform.condition.Condition
Imports ConditionOp = org.datavec.api.transform.condition.ConditionOp
Imports IntegerColumnCondition = org.datavec.api.transform.condition.column.IntegerColumnCondition
Imports StringColumnCondition = org.datavec.api.transform.condition.column.StringColumnCondition
Imports CategoricalMetaData = org.datavec.api.transform.metadata.CategoricalMetaData
Imports DoubleMetaData = org.datavec.api.transform.metadata.DoubleMetaData
Imports IntegerMetaData = org.datavec.api.transform.metadata.IntegerMetaData
Imports LongMetaData = org.datavec.api.transform.metadata.LongMetaData
Imports IAssociativeReducer = org.datavec.api.transform.reduce.IAssociativeReducer
Imports Reducer = org.datavec.api.transform.reduce.Reducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports ReduceSequenceTransform = org.datavec.api.transform.sequence.ReduceSequenceTransform
Imports SequenceTrimTransform = org.datavec.api.transform.sequence.trim.SequenceTrimTransform
Imports JsonMappers = org.datavec.api.transform.serde.JsonMappers
Imports org.datavec.api.transform.transform.categorical
Imports org.datavec.api.transform.transform.column
Imports ConditionalCopyValueTransform = org.datavec.api.transform.transform.condition.ConditionalCopyValueTransform
Imports ConditionalReplaceValueTransform = org.datavec.api.transform.transform.condition.ConditionalReplaceValueTransform
Imports ConditionalReplaceValueTransformWithDefault = org.datavec.api.transform.transform.condition.ConditionalReplaceValueTransformWithDefault
Imports org.datavec.api.transform.transform.doubletransform
Imports org.datavec.api.transform.transform.integer
Imports LongColumnsMathOpTransform = org.datavec.api.transform.transform.longtransform.LongColumnsMathOpTransform
Imports LongMathOpTransform = org.datavec.api.transform.transform.longtransform.LongMathOpTransform
Imports TextToCharacterIndexTransform = org.datavec.api.transform.transform.nlp.TextToCharacterIndexTransform
Imports TextToTermIndexSequenceTransform = org.datavec.api.transform.transform.nlp.TextToTermIndexSequenceTransform
Imports SequenceDifferenceTransform = org.datavec.api.transform.transform.sequence.SequenceDifferenceTransform
Imports SequenceMovingWindowReduceTransform = org.datavec.api.transform.transform.sequence.SequenceMovingWindowReduceTransform
Imports SequenceOffsetTransform = org.datavec.api.transform.transform.sequence.SequenceOffsetTransform
Imports org.datavec.api.transform.transform.string
Imports DeriveColumnsFromTimeTransform = org.datavec.api.transform.transform.time.DeriveColumnsFromTimeTransform
Imports StringToTimeTransform = org.datavec.api.transform.transform.time.StringToTimeTransform
Imports TimeMathOpTransform = org.datavec.api.transform.transform.time.TimeMathOpTransform
Imports org.datavec.api.writable
Imports DateTimeFieldType = org.joda.time.DateTimeFieldType
Imports DateTimeZone = org.joda.time.DateTimeZone
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions

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

Namespace org.datavec.api.transform.transform



	Public Class TestTransforms
		Inherits BaseND4JTest

		Public Shared Function getSchema(ByVal type As ColumnType, ParamArray ByVal colNames() As String) As Schema

			Dim schema As New Schema.Builder()

			Select Case type.innerEnumValue
				Case org.datavec.api.transform.ColumnType.InnerEnum.String
					schema.addColumnString("column")
				Case Integer?
					schema.addColumnInteger("column")
				Case Long?
					schema.addColumnLong("column")
				Case Double?
					schema.addColumnDouble("column")
				Case Single?
					schema.addColumnFloat("column")
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case org.datavec.api.transform.ColumnType.InnerEnum.Categorical
					schema.addColumnCategorical("column", colNames)
				Case org.datavec.api.transform.ColumnType.InnerEnum.Time
					schema.addColumnTime("column", DateTimeZone.UTC)
				Case Else
					Throw New Exception()
			End Select
			Return schema.build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCategoricalToInteger()
		Public Overridable Sub testCategoricalToInteger()
			Dim schema As Schema = getSchema(ColumnType.Categorical, "zero", "one", "two")

			Dim transform As Transform = New CategoricalToIntegerTransform("column")
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)


			TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(0).ColumnType)
			Dim meta As IntegerMetaData = DirectCast([out].getMetaData(0), IntegerMetaData)
			assertNotNull(meta.getMinAllowedValue())
			assertEquals(0, CInt(Math.Truncate(meta.getMinAllowedValue())))

			assertNotNull(meta.getMaxAllowedValue())
			assertEquals(2, CInt(Math.Truncate(meta.getMaxAllowedValue())))

			assertEquals(0, transform.map(Collections.singletonList(DirectCast(New Text("zero"), Writable))).get(0).toInt())
			assertEquals(1, transform.map(Collections.singletonList(DirectCast(New Text("one"), Writable))).get(0).toInt())
			assertEquals(2, transform.map(Collections.singletonList(DirectCast(New Text("two"), Writable))).get(0).toInt())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCategoricalToOneHotTransform()
		Public Overridable Sub testCategoricalToOneHotTransform()
			Dim schema As Schema = getSchema(ColumnType.Categorical, "zero", "one", "two")

			Dim transform As Transform = New CategoricalToOneHotTransform("column")
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(3, [out].getColumnMetaData().Count)
			For i As Integer = 0 To 2
				TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(i).ColumnType)
				Dim meta As IntegerMetaData = DirectCast([out].getMetaData(i), IntegerMetaData)
				assertNotNull(meta.getMinAllowedValue())
				assertEquals(0, CInt(Math.Truncate(meta.getMinAllowedValue())))

				assertNotNull(meta.getMaxAllowedValue())
				assertEquals(1, CInt(Math.Truncate(meta.getMaxAllowedValue())))
			Next i

			assertEquals(java.util.Arrays.asList(New IntWritable(1), New IntWritable(0), New IntWritable(0)), transform.map(Collections.singletonList(DirectCast(New Text("zero"), Writable))))
			assertEquals(java.util.Arrays.asList(New IntWritable(0), New IntWritable(1), New IntWritable(0)), transform.map(Collections.singletonList(DirectCast(New Text("one"), Writable))))
			assertEquals(java.util.Arrays.asList(New IntWritable(0), New IntWritable(0), New IntWritable(1)), transform.map(Collections.singletonList(DirectCast(New Text("two"), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testPivotTransform()
		Public Overridable Sub testPivotTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnString("otherCol").addColumnCategorical("key", java.util.Arrays.asList("first","second","third")).addColumnDouble("value").addColumnDouble("otherCol2").build()

			Dim t As Transform = New PivotTransform("key","value")
			t.InputSchema = schema
			Dim [out] As Schema = t.transform(schema)

			Dim expNames As IList(Of String) = New List(Of String) From {"otherCol", "key[first]", "key[second]", "key[third]", "otherCol2"}
			Dim actNames As IList(Of String) = [out].getColumnNames()

			assertEquals(expNames, actNames)

			Dim columnTypesExp As IList(Of ColumnType) = New List(Of ColumnType) From {ColumnType.String, ColumnType.Double, ColumnType.Double, ColumnType.Double, ColumnType.Double}
			assertEquals(columnTypesExp, [out].getColumnTypes())

			'Expand (second,100) into (0,100,0). Leave the remaining columns as is
			Dim e1 As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim a1 As IList(Of Writable) = t.map(java.util.Arrays.asList(Of Writable)(New DoubleWritable(1), New Text("second"), New DoubleWritable(100), New DoubleWritable(-1)))
			assertEquals(e1,a1)

			'Expand (third,200) into (0,0,200). Leave the remaining columns as is
			Dim e2 As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim a2 As IList(Of Writable) = t.map(java.util.Arrays.asList(Of Writable)(New DoubleWritable(1), New Text("third"), New DoubleWritable(200), New DoubleWritable(-1)))
			assertEquals(e2,a2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIntegerToCategoricalTransform()
		Public Overridable Sub testIntegerToCategoricalTransform()
			Dim schema As Schema = getSchema(ColumnType.Integer)

			Dim transform As Transform = New IntegerToCategoricalTransform("column", java.util.Arrays.asList("zero", "one", "two"))
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			assertEquals(ColumnType.Categorical, [out].getMetaData(0).ColumnType)
			Dim meta As CategoricalMetaData = DirectCast([out].getMetaData(0), CategoricalMetaData)
			assertEquals(java.util.Arrays.asList("zero", "one", "two"), meta.getStateNames())

			assertEquals(Collections.singletonList(DirectCast(New Text("zero"), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("one"), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("two"), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(2), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIntegerToOneHotTransform()
		Public Overridable Sub testIntegerToOneHotTransform()
			Dim schema As Schema = getSchema(ColumnType.Integer)

			Dim transform As Transform = New IntegerToOneHotTransform("column", 3, 5)
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(3, [out].getColumnMetaData().Count)
			assertEquals(ColumnType.Integer, [out].getMetaData(0).ColumnType)
			assertEquals(ColumnType.Integer, [out].getMetaData(1).ColumnType)
			assertEquals(ColumnType.Integer, [out].getMetaData(2).ColumnType)

			assertEquals(java.util.Arrays.asList("column[3]", "column[4]", "column[5]"), [out].getColumnNames())

			assertEquals(java.util.Arrays.asList(Of Writable)(New IntWritable(1), New IntWritable(0), New IntWritable(0)), transform.map(Collections.singletonList(DirectCast(New IntWritable(3), Writable))))
			assertEquals(java.util.Arrays.asList(Of Writable)(New IntWritable(0), New IntWritable(1), New IntWritable(0)), transform.map(Collections.singletonList(DirectCast(New IntWritable(4), Writable))))
			assertEquals(java.util.Arrays.asList(Of Writable)(New IntWritable(0), New IntWritable(0), New IntWritable(1)), transform.map(Collections.singletonList(DirectCast(New IntWritable(5), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStringToCategoricalTransform()
		Public Overridable Sub testStringToCategoricalTransform()
			Dim schema As Schema = getSchema(ColumnType.String)

			Dim transform As Transform = New StringToCategoricalTransform("column", java.util.Arrays.asList("zero", "one", "two"))
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Categorical, [out].getMetaData(0).ColumnType)
			Dim meta As CategoricalMetaData = DirectCast([out].getMetaData(0), CategoricalMetaData)
			assertEquals(java.util.Arrays.asList("zero", "one", "two"), meta.getStateNames())

			assertEquals(Collections.singletonList(DirectCast(New Text("zero"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("zero"), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("one"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("one"), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("two"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("two"), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConcatenateStringColumnsTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testConcatenateStringColumnsTransform()
			Const DELIMITER As String = " "
			Const NEW_COLUMN As String = "NewColumn"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<String> CONCAT_COLUMNS = Arrays.asList("ConcatenatedColumn1", "ConcatenatedColumn2", "ConcatenatedColumn3");
			Dim CONCAT_COLUMNS As IList(Of String) = New List(Of String) From {"ConcatenatedColumn1", "ConcatenatedColumn2", "ConcatenatedColumn3"}
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<String> ALL_COLUMNS = Arrays.asList("ConcatenatedColumn1", "OtherColumn4", "ConcatenatedColumn2", "OtherColumn5", "ConcatenatedColumn3", "OtherColumn6");
			Dim ALL_COLUMNS As IList(Of String) = New List(Of String) From {"ConcatenatedColumn1", "OtherColumn4", "ConcatenatedColumn2", "OtherColumn5", "ConcatenatedColumn3", "OtherColumn6"}
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<Text> COLUMN_VALUES = Arrays.asList(new Text("string1"), new Text("other4"), new Text("string2"), new Text("other5"), new Text("string3"), new Text("other6"));
			Dim COLUMN_VALUES As IList(Of Text) = New List(Of Text) From {
				New Text("string1"),
				New Text("other4"),
				New Text("string2"),
				New Text("other5"),
				New Text("string3"),
				New Text("other6")
			}
			Const NEW_COLUMN_VALUE As String = "string1 string2 string3"

			Dim transform As Transform = New ConcatenateStringColumns(NEW_COLUMN, DELIMITER, CONCAT_COLUMNS)
			Dim allColumns() As String = CType(ALL_COLUMNS, List(Of String)).ToArray()
			Dim schema As Schema = (New Schema.Builder()).addColumnsString(allColumns).build()

			Dim outputColumns As IList(Of String) = New List(Of String)(ALL_COLUMNS)
			outputColumns.Add(NEW_COLUMN)
			Dim newSchema As Schema = transform.transform(schema)
			assertEquals(outputColumns, newSchema.getColumnNames())

			Dim input As IList(Of Writable) = New List(Of Writable)()
			CType(input, List(Of Writable)).AddRange(COLUMN_VALUES)

			transform.InputSchema = schema
			Dim transformed As IList(Of Writable) = transform.map(input)
			assertEquals(NEW_COLUMN_VALUE, transformed(transformed.Count - 1).ToString())

			Dim outputColumnValues As IList(Of Text) = New List(Of Text)(COLUMN_VALUES)
			outputColumnValues.Add(New Text(NEW_COLUMN_VALUE))
			assertEquals(outputColumnValues, transformed)

			Dim s As String = JsonMappers.Mapper.writeValueAsString(transform)
			Dim transform2 As Transform = JsonMappers.Mapper.readValue(s, GetType(ConcatenateStringColumns))
			assertEquals(transform, transform2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testChangeCaseStringTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testChangeCaseStringTransform()
			Const STRING_COLUMN As String = "StringColumn"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final List<String> ALL_COLUMNS = Arrays.asList(STRING_COLUMN, "OtherColumn");
			Dim ALL_COLUMNS As IList(Of String) = New List(Of String) From {STRING_COLUMN, "OtherColumn"}
			Const TEXT_MIXED_CASE As String = "UPPER lower MiXeD"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String TEXT_UPPER_CASE = TEXT_MIXED_CASE.toUpperCase();
			Dim TEXT_UPPER_CASE As String = TEXT_MIXED_CASE.ToUpper()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String TEXT_LOWER_CASE = TEXT_MIXED_CASE.toLowerCase();
			Dim TEXT_LOWER_CASE As String = TEXT_MIXED_CASE.ToLower()

			Dim transform As Transform = New ChangeCaseStringTransform(STRING_COLUMN)
			Dim allColumns() As String = CType(ALL_COLUMNS, List(Of String)).ToArray()
			Dim schema As Schema = (New Schema.Builder()).addColumnsString(allColumns).build()
			transform.InputSchema = schema
			Dim newSchema As Schema = transform.transform(schema)
			Dim outputColumns As IList(Of String) = New List(Of String)(ALL_COLUMNS)
			assertEquals(outputColumns, newSchema.getColumnNames())

			transform = New ChangeCaseStringTransform(STRING_COLUMN, ChangeCaseStringTransform.CaseType.LOWER)
			transform.InputSchema = schema
			Dim input As IList(Of Writable) = New List(Of Writable)()
			input.Add(New Text(TEXT_MIXED_CASE))
			input.Add(New Text(TEXT_MIXED_CASE))
			Dim output As IList(Of Writable) = New List(Of Writable)()
			output.Add(New Text(TEXT_LOWER_CASE))
			output.Add(New Text(TEXT_MIXED_CASE))
			Dim transformed As IList(Of Writable) = transform.map(input)
			assertEquals(transformed(0).ToString(), TEXT_LOWER_CASE)
			assertEquals(transformed, output)

			transform = New ChangeCaseStringTransform(STRING_COLUMN, ChangeCaseStringTransform.CaseType.UPPER)
			transform.InputSchema = schema
			output.Clear()
			output.Add(New Text(TEXT_UPPER_CASE))
			output.Add(New Text(TEXT_MIXED_CASE))
			transformed = transform.map(input)
			assertEquals(transformed(0).ToString(), TEXT_UPPER_CASE)
			assertEquals(transformed, output)

			Dim s As String = JsonMappers.Mapper.writeValueAsString(transform)
			Dim transform2 As Transform = JsonMappers.Mapper.readValue(s, GetType(ChangeCaseStringTransform))
			assertEquals(transform, transform2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRemoveColumnsTransform()
		Public Overridable Sub testRemoveColumnsTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnDouble("first").addColumnString("second").addColumnInteger("third").addColumnLong("fourth").build()

			Dim transform As Transform = New RemoveColumnsTransform("first", "fourth")
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)

			assertEquals(2, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)
			TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(1).ColumnType)

			assertEquals(java.util.Arrays.asList(New Text("one"), New IntWritable(1)), transform.map(java.util.Arrays.asList(DirectCast(New DoubleWritable(1.0), Writable), New Text("one"), New IntWritable(1), New LongWritable(1L))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRemoveAllColumnsExceptForTransform()
		Public Overridable Sub testRemoveAllColumnsExceptForTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnDouble("first").addColumnString("second").addColumnInteger("third").addColumnLong("fourth").build()

			Dim transform As Transform = New RemoveAllColumnsExceptForTransform("second", "third")
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)

			assertEquals(2, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)
			TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(1).ColumnType)

			assertEquals(java.util.Arrays.asList(New Text("one"), New IntWritable(1)), transform.map(java.util.Arrays.asList(DirectCast(New DoubleWritable(1.0), Writable), New Text("one"), New IntWritable(1), New LongWritable(1L))))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReplaceEmptyIntegerWithValueTransform()
		Public Overridable Sub testReplaceEmptyIntegerWithValueTransform()
			Dim schema As Schema = getSchema(ColumnType.Integer)

			Dim transform As Transform = New ReplaceEmptyIntegerWithValueTransform("column", 1000)
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(0).ColumnType)

			assertEquals(Collections.singletonList(DirectCast(New IntWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(1), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(1000), Writable)), transform.map(Collections.singletonList(DirectCast(New Text(""), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReplaceInvalidWithIntegerTransform()
		Public Overridable Sub testReplaceInvalidWithIntegerTransform()
			Dim schema As Schema = getSchema(ColumnType.Integer)

			Dim transform As Transform = New ReplaceInvalidWithIntegerTransform("column", 1000)
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(0).ColumnType)

			assertEquals(Collections.singletonList(DirectCast(New IntWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(1), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(1000), Writable)), transform.map(Collections.singletonList(DirectCast(New Text(""), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLog2Normalizer()
		Public Overridable Sub testLog2Normalizer()
			Dim schema As Schema = getSchema(ColumnType.Double)

			Dim mu As Double = 2.0
			Dim min As Double = 1.0
			Dim scale As Double = 0.5

			Dim transform As Transform = New Log2Normalizer("column", mu, min, scale)
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
			Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
			assertNotNull(meta.getMinAllowedValue())
			assertEquals(0, meta.getMinAllowedValue(), 1e-6)
			assertNull(meta.getMaxAllowedValue())

			Dim loge2 As Double = Math.Log(2)
			assertEquals(0.0, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(min), Writable))).get(0).toDouble(), 1e-6)
			Dim d As Double = scale * Math.Log((10 - min) / (mu - min) + 1) / loge2
			assertEquals(d, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(10), Writable))).get(0).toDouble(), 1e-6)
			d = scale * Math.Log((3 - min) / (mu - min) + 1) / loge2
			assertEquals(d, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(3), Writable))).get(0).toDouble(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDoubleMinMaxNormalizerTransform()
		Public Overridable Sub testDoubleMinMaxNormalizerTransform()
			Dim schema As Schema = getSchema(ColumnType.Double)

			Dim transform As Transform = New MinMaxNormalizer("column", 0, 100)
			Dim transform2 As Transform = New MinMaxNormalizer("column", 0, 100, -1, 1)
			transform.InputSchema = schema
			transform2.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			Dim out2 As Schema = transform2.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
			Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
			Dim meta2 As DoubleMetaData = DirectCast(out2.getMetaData(0), DoubleMetaData)
			assertEquals(0, meta.getMinAllowedValue(), 1e-6)
			assertEquals(1, meta.getMaxAllowedValue(), 1e-6)
			assertEquals(-1, meta2.getMinAllowedValue(), 1e-6)
			assertEquals(1, meta2.getMaxAllowedValue(), 1e-6)


			assertEquals(0.0, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(0), Writable))).get(0).toDouble(), 1e-6)
			assertEquals(1.0, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(100), Writable))).get(0).toDouble(), 1e-6)
			assertEquals(0.5, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(50), Writable))).get(0).toDouble(), 1e-6)

			assertEquals(-1.0, transform2.map(Collections.singletonList(DirectCast(New DoubleWritable(0), Writable))).get(0).toDouble(), 1e-6)
			assertEquals(1.0, transform2.map(Collections.singletonList(DirectCast(New DoubleWritable(100), Writable))).get(0).toDouble(), 1e-6)
			assertEquals(0.0, transform2.map(Collections.singletonList(DirectCast(New DoubleWritable(50), Writable))).get(0).toDouble(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStandardizeNormalizer()
		Public Overridable Sub testStandardizeNormalizer()
			Dim schema As Schema = getSchema(ColumnType.Double)

			Dim mu As Double = 1.0
			Dim sigma As Double = 2.0

			Dim transform As Transform = New StandardizeNormalizer("column", mu, sigma)
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
			Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
			assertNull(meta.getMinAllowedValue())
			assertNull(meta.getMaxAllowedValue())


			assertEquals(0.0, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(mu), Writable))).get(0).toDouble(), 1e-6)
			Dim d As Double = (10 - mu) / sigma
			assertEquals(d, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(10), Writable))).get(0).toDouble(), 1e-6)
			d = (-2 - mu) / sigma
			assertEquals(d, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(-2), Writable))).get(0).toDouble(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSubtractMeanNormalizer()
		Public Overridable Sub testSubtractMeanNormalizer()
			Dim schema As Schema = getSchema(ColumnType.Double)

			Dim mu As Double = 1.0

			Dim transform As Transform = New SubtractMeanNormalizer("column", mu)
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
			Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
			assertNull(meta.getMinAllowedValue())
			assertNull(meta.getMaxAllowedValue())


			assertEquals(0.0, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(mu), Writable))).get(0).toDouble(), 1e-6)
			assertEquals(10 - mu, transform.map(Collections.singletonList(DirectCast(New DoubleWritable(10), Writable))).get(0).toDouble(), 1e-6)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMapAllStringsExceptListTransform()
		Public Overridable Sub testMapAllStringsExceptListTransform()
			Dim schema As Schema = getSchema(ColumnType.String)

			Dim transform As Transform = New MapAllStringsExceptListTransform("column", "replacement", New List(Of String) From {"one", "two", "three"})
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

			assertEquals(Collections.singletonList(DirectCast(New Text("one"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("one"), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("two"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("two"), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("replacement"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("this should be replaced"), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRemoveWhitespaceTransform()
		Public Overridable Sub testRemoveWhitespaceTransform()
			Dim schema As Schema = getSchema(ColumnType.String)

			Dim transform As Transform = New RemoveWhiteSpaceTransform("column")
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

			assertEquals(Collections.singletonList(DirectCast(New Text("one"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("one "), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("two"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("two" & vbTab), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("three"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("three" & vbLf), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("one"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text(" o n e" & vbTab), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReplaceEmptyStringTransform()
		Public Overridable Sub testReplaceEmptyStringTransform()
			Dim schema As Schema = getSchema(ColumnType.String)

			Dim transform As Transform = New ReplaceEmptyStringTransform("column", "newvalue")
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

			assertEquals(Collections.singletonList(DirectCast(New Text("one"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("one"), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("newvalue"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text(""), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("three"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("three"), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAppendStringColumnTransform()
		Public Overridable Sub testAppendStringColumnTransform()
			Dim schema As Schema = getSchema(ColumnType.String)

			Dim transform As Transform = New AppendStringColumnTransform("column", "_AppendThis")
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

			assertEquals(Collections.singletonList(DirectCast(New Text("one_AppendThis"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("one"), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("two_AppendThis"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("two"), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("three_AppendThis"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("three"), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStringListToCategoricalSetTransform()
		Public Overridable Sub testStringListToCategoricalSetTransform()
			'Idea: String list to a set of categories... "a,c" for categories {a,b,c} -> "true","false","true"

			Dim schema As Schema = getSchema(ColumnType.String)

			Dim transform As Transform = New StringListToCategoricalSetTransform("column", New List(Of String) From {"a", "b", "c"}, New List(Of String) From {"a", "b", "c"}, ",")
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(3, [out].getColumnMetaData().Count)
			For i As Integer = 0 To 2
				TestCase.assertEquals(ColumnType.Categorical, [out].getType(i))
				Dim meta As CategoricalMetaData = DirectCast([out].getMetaData(i), CategoricalMetaData)
				assertEquals(java.util.Arrays.asList("true", "false"), meta.getStateNames())
			Next i

			assertEquals(java.util.Arrays.asList(New Text("false"), New Text("false"), New Text("false")), transform.map(Collections.singletonList(DirectCast(New Text(""), Writable))))
			assertEquals(java.util.Arrays.asList(New Text("true"), New Text("false"), New Text("false")), transform.map(Collections.singletonList(DirectCast(New Text("a"), Writable))))
			assertEquals(java.util.Arrays.asList(New Text("false"), New Text("true"), New Text("false")), transform.map(Collections.singletonList(DirectCast(New Text("b"), Writable))))
			assertEquals(java.util.Arrays.asList(New Text("false"), New Text("false"), New Text("true")), transform.map(Collections.singletonList(DirectCast(New Text("c"), Writable))))
			assertEquals(java.util.Arrays.asList(New Text("true"), New Text("false"), New Text("true")), transform.map(Collections.singletonList(DirectCast(New Text("a,c"), Writable))))
			assertEquals(java.util.Arrays.asList(New Text("true"), New Text("true"), New Text("true")), transform.map(Collections.singletonList(DirectCast(New Text("a,b,c"), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStringMapTransform()
		Public Overridable Sub testStringMapTransform()
			Dim schema As Schema = getSchema(ColumnType.String)

			Dim map As IDictionary(Of String, String) = New Dictionary(Of String, String)()
			map("one") = "ONE"
			map("two") = "TWO"
			Dim transform As Transform = New StringMapTransform("column", map)
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

			assertEquals(Collections.singletonList(DirectCast(New Text("ONE"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("one"), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("TWO"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("two"), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New Text("three"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("three"), Writable))))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStringToTimeTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStringToTimeTransform()
			testStringToDateTime("YYYY-MM-dd HH:mm:ss")
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStringToTimeTransformNoDateTime() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStringToTimeTransformNoDateTime()

			Dim schema As Schema = getSchema(ColumnType.String)
			Dim dateTime As String = "2017-09-21T17:06:29.064687"
			Dim dateTime2 As String = "2007-12-30"
			Dim dateTime3 As String = "12/1/2010 11:21"

			'http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html
			Dim transform As New StringToTimeTransform("column", Nothing, DateTimeZone.forID("UTC"))
			transform.InputSchema = schema
			transform.map(New Text(dateTime3))
			transform.map(New Text(dateTime))
			transform.map(New Text(dateTime2))
			testStringToDateTime(Nothing)




		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void testStringToDateTime(String timeFormat) throws Exception
		Private Sub testStringToDateTime(ByVal timeFormat As String)
			Dim schema As Schema = getSchema(ColumnType.String)

			'http://www.joda.org/joda-time/apidocs/org/joda/time/format/DateTimeFormat.html
			Dim transform As Transform = New StringToTimeTransform("column", timeFormat, DateTimeZone.forID("UTC"))
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Time, [out].getMetaData(0).ColumnType)

			Dim in1 As String = "2016-01-01 12:30:45"
			Dim out1 As Long = 1451651445000L

			Dim in2 As String = "2015-06-30 23:59:59"
			Dim out2 As Long = 1435708799000L

			assertEquals(Collections.singletonList(DirectCast(New LongWritable(out1), Writable)), transform.map(Collections.singletonList(DirectCast(New Text(in1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New LongWritable(out2), Writable)), transform.map(Collections.singletonList(DirectCast(New Text(in2), Writable))))

			'Check serialization: things like DateTimeFormatter etc aren't serializable, hence we need custom serialization :/
			Dim baos As New MemoryStream()
			Dim oos As New ObjectOutputStream(baos)
			oos.writeObject(transform)

			Dim bytes() As SByte = baos.toByteArray()

			Dim bais As New MemoryStream(bytes)
			Dim ois As New ObjectInputStream(bais)

			Dim deserialized As Transform = DirectCast(ois.readObject(), Transform)
			assertEquals(Collections.singletonList(DirectCast(New LongWritable(out1), Writable)), deserialized.map(Collections.singletonList(DirectCast(New Text(in1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New LongWritable(out2), Writable)), deserialized.map(Collections.singletonList(DirectCast(New Text(in2), Writable))))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDeriveColumnsFromTimeTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeriveColumnsFromTimeTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnTime("column", DateTimeZone.forID("UTC")).addColumnString("otherColumn").build()

			Dim transform As Transform = (New DeriveColumnsFromTimeTransform.Builder("column")).insertAfter("otherColumn").addIntegerDerivedColumn("hour", DateTimeFieldType.hourOfDay()).addIntegerDerivedColumn("day", DateTimeFieldType.dayOfMonth()).addIntegerDerivedColumn("second", DateTimeFieldType.secondOfMinute()).addStringDerivedColumn("humanReadable", "YYYY-MM-dd HH:mm:ss", DateTimeZone.UTC).build()

			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(6, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Time, [out].getMetaData(0).ColumnType)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(1).ColumnType)
			TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(2).ColumnType)
			TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(3).ColumnType)
			TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(4).ColumnType)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(5).ColumnType)

			assertEquals("column", [out].getName(0))
			assertEquals("otherColumn", [out].getName(1))
			assertEquals("hour", [out].getName(2))
			assertEquals("day", [out].getName(3))
			assertEquals("second", [out].getName(4))
			assertEquals("humanReadable", [out].getName(5))

			Dim in1 As Long = 1451651445000L '"2016-01-01 12:30:45" GMT

			Dim out1 As IList(Of Writable) = New List(Of Writable)()
			out1.Add(New LongWritable(in1))
			out1.Add(New Text("otherColumnValue"))
			out1.Add(New IntWritable(12)) 'hour
			out1.Add(New IntWritable(1)) 'day
			out1.Add(New IntWritable(45)) 'second
			out1.Add(New Text("2016-01-01 12:30:45"))

			Dim in2 As Long = 1435708799000L '"2015-06-30 23:59:59" GMT
			Dim out2 As IList(Of Writable) = New List(Of Writable)()
			out2.Add(New LongWritable(in2))
			out2.Add(New Text("otherColumnValue"))
			out2.Add(New IntWritable(23)) 'hour
			out2.Add(New IntWritable(30)) 'day
			out2.Add(New IntWritable(59)) 'second
			out2.Add(New Text("2015-06-30 23:59:59"))

			assertEquals(out1, transform.map(java.util.Arrays.asList(DirectCast(New LongWritable(in1), Writable), New Text("otherColumnValue"))))
			assertEquals(out2, transform.map(java.util.Arrays.asList(DirectCast(New LongWritable(in2), Writable), New Text("otherColumnValue"))))



			'Check serialization: things like DateTimeFormatter etc aren't serializable, hence we need custom serialization :/
			Dim baos As New MemoryStream()
			Dim oos As New ObjectOutputStream(baos)
			oos.writeObject(transform)

			Dim bytes() As SByte = baos.toByteArray()

			Dim bais As New MemoryStream(bytes)
			Dim ois As New ObjectInputStream(bais)

			Dim deserialized As Transform = DirectCast(ois.readObject(), Transform)
			assertEquals(out1, deserialized.map(java.util.Arrays.asList(DirectCast(New LongWritable(in1), Writable), New Text("otherColumnValue"))))
			assertEquals(out2, deserialized.map(java.util.Arrays.asList(DirectCast(New LongWritable(in2), Writable), New Text("otherColumnValue"))))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDuplicateColumnsTransform()
		Public Overridable Sub testDuplicateColumnsTransform()

			Dim schema As Schema = (New Schema.Builder()).addColumnString("stringCol").addColumnInteger("intCol").addColumnLong("longCol").build()

			Dim toDup As IList(Of String) = New List(Of String) From {"intCol", "longCol"}
			Dim newNames As IList(Of String) = New List(Of String) From {"dup_intCol", "dup_longCol"}

			Dim transform As Transform = New DuplicateColumnsTransform(toDup, newNames)
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(5, [out].getColumnMetaData().Count)

			Dim expOutNames As IList(Of String) = New List(Of String) From {"stringCol", "intCol", "dup_intCol", "longCol", "dup_longCol"}
			Dim expOutTypes As IList(Of ColumnType) = New List(Of ColumnType) From {ColumnType.String, ColumnType.Integer, ColumnType.Integer, ColumnType.Long, ColumnType.Long}
			For i As Integer = 0 To 4
				assertEquals(expOutNames(i), [out].getName(i))
				TestCase.assertEquals(expOutTypes(i), [out].getType(i))
			Next i

			Dim inList As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("one"), Writable), New IntWritable(2), New LongWritable(3L)}
			Dim outList As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("one"), Writable), New IntWritable(2), New IntWritable(2), New LongWritable(3L), New LongWritable(3L)}

			assertEquals(outList, transform.map(inList))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIntegerMathOpTransform()
		Public Overridable Sub testIntegerMathOpTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("column", -1, 1).build()

			Dim transform As Transform = New IntegerMathOpTransform("column", MathOp.Multiply, 5)
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Integer, [out].getType(0))
			Dim meta As IntegerMetaData = DirectCast([out].getMetaData(0), IntegerMetaData)
			assertEquals(-5, CInt(Math.Truncate(meta.getMinAllowedValue())))
			assertEquals(5, CInt(Math.Truncate(meta.getMaxAllowedValue())))

			assertEquals(Collections.singletonList(DirectCast(New IntWritable(-5), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(-1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(5), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testIntegerColumnsMathOpTransform()
		Public Overridable Sub testIntegerColumnsMathOpTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("first").addColumnString("second").addColumnInteger("third").build()

			Dim transform As Transform = New IntegerColumnsMathOpTransform("out", MathOp.Add, "first", "third")
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(4, [out].numColumns())
			assertEquals(java.util.Arrays.asList("first", "second", "third", "out"), [out].getColumnNames())
			assertEquals(java.util.Arrays.asList(ColumnType.Integer, ColumnType.String, ColumnType.Integer, ColumnType.Integer), [out].getColumnTypes())


			assertEquals(java.util.Arrays.asList(DirectCast(New IntWritable(1), Writable), New Text("something"), New IntWritable(2), New IntWritable(3)), transform.map(java.util.Arrays.asList(DirectCast(New IntWritable(1), Writable), New Text("something"), New IntWritable(2))))
			assertEquals(java.util.Arrays.asList(DirectCast(New IntWritable(100), Writable), New Text("something2"), New IntWritable(21), New IntWritable(121)), transform.map(java.util.Arrays.asList(DirectCast(New IntWritable(100), Writable), New Text("something2"), New IntWritable(21))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLongMathOpTransform()
		Public Overridable Sub testLongMathOpTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnLong("column", -1L, 1L).build()

			Dim transform As Transform = New LongMathOpTransform("column", MathOp.Multiply, 5)
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Long, [out].getType(0))
			Dim meta As LongMetaData = DirectCast([out].getMetaData(0), LongMetaData)
			assertEquals(-5, CLng(Math.Truncate(meta.getMinAllowedValue())))
			assertEquals(5, CLng(Math.Truncate(meta.getMaxAllowedValue())))

			assertEquals(Collections.singletonList(DirectCast(New LongWritable(-5), Writable)), transform.map(Collections.singletonList(DirectCast(New LongWritable(-1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New LongWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New LongWritable(0), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New LongWritable(5), Writable)), transform.map(Collections.singletonList(DirectCast(New LongWritable(1), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testLongColumnsMathOpTransform()
		Public Overridable Sub testLongColumnsMathOpTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnLong("first").addColumnString("second").addColumnLong("third").build()

			Dim transform As Transform = New LongColumnsMathOpTransform("out", MathOp.Add, "first", "third")
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(4, [out].numColumns())
			assertEquals(java.util.Arrays.asList("first", "second", "third", "out"), [out].getColumnNames())
			assertEquals(java.util.Arrays.asList(ColumnType.Long, ColumnType.String, ColumnType.Long, ColumnType.Long), [out].getColumnTypes())


			assertEquals(java.util.Arrays.asList(DirectCast(New LongWritable(1), Writable), New Text("something"), New LongWritable(2), New LongWritable(3)), transform.map(java.util.Arrays.asList(DirectCast(New LongWritable(1), Writable), New Text("something"), New LongWritable(2))))
			assertEquals(java.util.Arrays.asList(DirectCast(New LongWritable(100), Writable), New Text("something2"), New LongWritable(21), New LongWritable(121)), transform.map(java.util.Arrays.asList(DirectCast(New LongWritable(100), Writable), New Text("something2"), New LongWritable(21))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTimeMathOpTransform()
		Public Overridable Sub testTimeMathOpTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnTime("column", DateTimeZone.UTC).build()

			Dim transform As Transform = New TimeMathOpTransform("column", MathOp.Add, 12, TimeUnit.HOURS) '12 hours: 43200000 milliseconds
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Time, [out].getType(0))

			assertEquals(Collections.singletonList(DirectCast(New LongWritable(1000 + 43200000), Writable)), transform.map(Collections.singletonList(DirectCast(New LongWritable(1000), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New LongWritable(1452441600000L + 43200000), Writable)), transform.map(Collections.singletonList(DirectCast(New LongWritable(1452441600000L), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDoubleMathOpTransform()
		Public Overridable Sub testDoubleMathOpTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnDouble("column", -1.0, 1.0).build()

			Dim transform As Transform = New DoubleMathOpTransform("column", MathOp.Multiply, 5.0)
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Double, [out].getType(0))
			Dim meta As DoubleMetaData = DirectCast([out].getMetaData(0), DoubleMetaData)
			assertEquals(-5.0, meta.getMinAllowedValue(), 1e-6)
			assertEquals(5.0, meta.getMaxAllowedValue(), 1e-6)

			assertEquals(Collections.singletonList(DirectCast(New DoubleWritable(-5), Writable)), transform.map(Collections.singletonList(DirectCast(New DoubleWritable(-1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New DoubleWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New DoubleWritable(0), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New DoubleWritable(5), Writable)), transform.map(Collections.singletonList(DirectCast(New DoubleWritable(1), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDoubleMathFunctionTransform()
		Public Overridable Sub testDoubleMathFunctionTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnDouble("column").addColumnString("strCol").build()

			Dim transform As Transform = New DoubleMathFunctionTransform("column", MathFunction.SIN)
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(2, [out].getColumnMetaData().Count)
			assertEquals(ColumnType.Double, [out].getType(0))
			assertEquals(ColumnType.String, [out].getType(1))

			assertEquals(java.util.Arrays.asList(Of Writable)(New DoubleWritable(Math.Sin(1)), New Text("0")), transform.map(java.util.Arrays.asList(Of Writable)(New DoubleWritable(1), New Text("0"))))
			assertEquals(java.util.Arrays.asList(Of Writable)(New DoubleWritable(Math.Sin(2)), New Text("1")), transform.map(java.util.Arrays.asList(Of Writable)(New DoubleWritable(2), New Text("1"))))
			assertEquals(java.util.Arrays.asList(Of Writable)(New DoubleWritable(Math.Sin(3)), New Text("2")), transform.map(java.util.Arrays.asList(Of Writable)(New DoubleWritable(3), New Text("2"))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDoubleColumnsMathOpTransform()
		Public Overridable Sub testDoubleColumnsMathOpTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnString("first").addColumnDouble("second").addColumnDouble("third").build()

			Dim transform As Transform = New DoubleColumnsMathOpTransform("out", MathOp.Add, "second", "third")
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(4, [out].numColumns())
			assertEquals(java.util.Arrays.asList("first", "second", "third", "out"), [out].getColumnNames())
			assertEquals(java.util.Arrays.asList(ColumnType.String, ColumnType.Double, ColumnType.Double, ColumnType.Double), [out].getColumnTypes())


			assertEquals(java.util.Arrays.asList(DirectCast(New Text("something"), Writable), New DoubleWritable(1.0), New DoubleWritable(2.1), New DoubleWritable(3.1)), transform.map(java.util.Arrays.asList(DirectCast(New Text("something"), Writable), New DoubleWritable(1.0), New DoubleWritable(2.1))))
			assertEquals(java.util.Arrays.asList(DirectCast(New Text("something2"), Writable), New DoubleWritable(100.0), New DoubleWritable(21.1), New DoubleWritable(121.1)), transform.map(java.util.Arrays.asList(DirectCast(New Text("something2"), Writable), New DoubleWritable(100.0), New DoubleWritable(21.1))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testRenameColumnsTransform()
		Public Overridable Sub testRenameColumnsTransform()

			Dim schema As Schema = (New Schema.Builder()).addColumnDouble("col1").addColumnString("col2").addColumnInteger("col3").build()

			Dim transform As Transform = New RenameColumnsTransform(java.util.Arrays.asList("col1", "col3"), java.util.Arrays.asList("column1", "column3"))
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)

			assertEquals(3, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.Double, [out].getMetaData(0).ColumnType)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(1).ColumnType)
			TestCase.assertEquals(ColumnType.Integer, [out].getMetaData(2).ColumnType)

			assertEquals("column1", [out].getName(0))
			assertEquals("col2", [out].getName(1))
			assertEquals("column3", [out].getName(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReorderColumnsTransform()
		Public Overridable Sub testReorderColumnsTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnDouble("col1").addColumnString("col2").addColumnInteger("col3").build()

			Dim transform As Transform = New ReorderColumnsTransform("col3", "col2")
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)

			assertEquals(3, [out].numColumns())
			assertEquals(java.util.Arrays.asList("col3", "col2", "col1"), [out].getColumnNames())
			assertEquals(java.util.Arrays.asList(ColumnType.Integer, ColumnType.String, ColumnType.Double), [out].getColumnTypes())

			assertEquals(java.util.Arrays.asList(DirectCast(New IntWritable(1), Writable), New Text("one"), New DoubleWritable(1.1)), transform.map(java.util.Arrays.asList(DirectCast(New DoubleWritable(1.1), Writable), New Text("one"), New IntWritable(1))))

			assertEquals(java.util.Arrays.asList(DirectCast(New IntWritable(2), Writable), New Text("two"), New DoubleWritable(200.2)), transform.map(java.util.Arrays.asList(DirectCast(New DoubleWritable(200.2), Writable), New Text("two"), New IntWritable(2))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConditionalReplaceValueTransform()
		Public Overridable Sub testConditionalReplaceValueTransform()
			Dim schema As Schema = getSchema(ColumnType.Integer)

			Dim condition As Condition = New IntegerColumnCondition("column", ConditionOp.LessThan, 0)
			condition.InputSchema = schema

			Dim transform As Transform = New ConditionalReplaceValueTransform("column", New IntWritable(0), condition)
			transform.InputSchema = schema

			assertEquals(Collections.singletonList(DirectCast(New IntWritable(10), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(10), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(1), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(-1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(-10), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConditionalReplaceValueTransformWithDefault()
		Public Overridable Sub testConditionalReplaceValueTransformWithDefault()
			Dim schema As Schema = getSchema(ColumnType.Integer)

			Dim condition As Condition = New IntegerColumnCondition("column", ConditionOp.LessThan, 0)
			condition.InputSchema = schema

			Dim transform As Transform = New ConditionalReplaceValueTransformWithDefault("column", New IntWritable(0), New IntWritable(1), condition)
			transform.InputSchema = schema

			assertEquals(Collections.singletonList(DirectCast(New IntWritable(1), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(10), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(1), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(1), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(0), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(-1), Writable))))
			assertEquals(Collections.singletonList(DirectCast(New IntWritable(0), Writable)), transform.map(Collections.singletonList(DirectCast(New IntWritable(-10), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConditionalCopyValueTransform()
		Public Overridable Sub testConditionalCopyValueTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnsString("first", "second", "third").build()

			Dim condition As Condition = New StringColumnCondition("third", ConditionOp.Equal, "")
			Dim transform As Transform = New ConditionalCopyValueTransform("third", "second", condition)
			transform.InputSchema = schema

			Dim list As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("first"), Writable), New Text("second"), New Text("third")}
			assertEquals(list, transform.map(list))

			list = New List(Of Writable) From {DirectCast(New Text("first"), Writable), New Text("second"), New Text("")}
			Dim exp As IList(Of Writable) = New List(Of Writable) From {DirectCast(New Text("first"), Writable), New Text("second"), New Text("second")}
			assertEquals(exp, transform.map(list))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceDifferenceTransform()
		Public Overridable Sub testSequenceDifferenceTransform()
			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnString("firstCol").addColumnInteger("secondCol").addColumnDouble("thirdCol").build()

			Dim sequence As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			sequence.Add(New List(Of Writable) From {Of Writable})
			sequence.Add(New List(Of Writable) From {Of Writable})
			sequence.Add(New List(Of Writable) From {Of Writable})
			sequence.Add(New List(Of Writable) From {Of Writable})

			Dim t As Transform = New SequenceDifferenceTransform("secondCol")
			t.InputSchema = schema

			Dim [out] As IList(Of IList(Of Writable)) = t.mapSequence(sequence)

			Dim expected As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expected.Add(New List(Of Writable) From {Of Writable})
			expected.Add(New List(Of Writable) From {Of Writable})
			expected.Add(New List(Of Writable) From {Of Writable})
			expected.Add(New List(Of Writable) From {Of Writable})

			assertEquals(expected, [out])



			t = New SequenceDifferenceTransform("thirdCol", "newThirdColName", 2, SequenceDifferenceTransform.FirstStepMode.SpecifiedValue, NullWritable.INSTANCE)
			Dim outputSchema As Schema = t.transform(schema)
			assertTrue(TypeOf outputSchema Is SequenceSchema)
			assertEquals(outputSchema.getColumnNames(), java.util.Arrays.asList("firstCol", "secondCol", "newThirdColName"))

			expected = New List(Of IList(Of Writable))()
			expected.Add(New List(Of Writable) From {Of Writable})
			expected.Add(New List(Of Writable) From {Of Writable})
			expected.Add(New List(Of Writable) From {Of Writable})
			expected.Add(New List(Of Writable) From {Of Writable})
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAddConstantColumnTransform()
		Public Overridable Sub testAddConstantColumnTransform()
			Dim schema As Schema = (New Schema.Builder()).addColumnString("first").addColumnDouble("second").build()

			Dim transform As Transform = New AddConstantColumnTransform("newCol", ColumnType.Integer, New IntWritable(10))
			transform.InputSchema = schema

			Dim [out] As Schema = transform.transform(schema)
			assertEquals(3, [out].numColumns())
			assertEquals(java.util.Arrays.asList("first", "second", "newCol"), [out].getColumnNames())
			assertEquals(java.util.Arrays.asList(ColumnType.String, ColumnType.Double, ColumnType.Integer), [out].getColumnTypes())


			assertEquals(java.util.Arrays.asList(DirectCast(New Text("something"), Writable), New DoubleWritable(1.0), New IntWritable(10)), transform.map(java.util.Arrays.asList(DirectCast(New Text("something"), Writable), New DoubleWritable(1.0))))
			assertEquals(java.util.Arrays.asList(DirectCast(New Text("something2"), Writable), New DoubleWritable(100.0), New IntWritable(10)), transform.map(java.util.Arrays.asList(DirectCast(New Text("something2"), Writable), New DoubleWritable(100.0))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReplaceStringTransform()
		Public Overridable Sub testReplaceStringTransform()
			Dim schema As Schema = getSchema(ColumnType.String)

			' Linked
			Dim map As IDictionary(Of String, String) = New LinkedHashMap(Of String, String)()
			map("mid") = "C2"
			map("\d") = "one"
			Dim transform As Transform = New ReplaceStringTransform("column", map)
			transform.InputSchema = schema
			Dim [out] As Schema = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

			assertEquals(Collections.singletonList(DirectCast(New Text("BoneConeTone"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("B1midT3"), Writable))))

			' No link
			map = New Dictionary(Of String, String)()
			map("^\s+|\s+$") = ""
			transform = New ReplaceStringTransform("column", map)
			transform.InputSchema = schema
			[out] = transform.transform(schema)

			assertEquals(1, [out].getColumnMetaData().Count)
			TestCase.assertEquals(ColumnType.String, [out].getMetaData(0).ColumnType)

			assertEquals(Collections.singletonList(DirectCast(New Text("4.25"), Writable)), transform.map(Collections.singletonList(DirectCast(New Text("  4.25 "), Writable))))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReduceSequenceTransform()
		Public Overridable Sub testReduceSequenceTransform()

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnsDouble("col%d",0,2).build()

			Dim reducer As IAssociativeReducer = (New Reducer.Builder(ReduceOp.Mean)).countColumns("col1").maxColumn("col2").build()

			Dim t As New ReduceSequenceTransform(reducer)
			t.InputSchema = schema

			Dim seq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8))}

			Dim exp As IList(Of IList(Of Writable)) = Collections.singletonList(java.util.Arrays.asList(Of Writable)(New DoubleWritable(3), New LongWritable(3L), New DoubleWritable(8)))
			Dim act As IList(Of IList(Of Writable)) = t.mapSequence(seq)
			assertEquals(exp, act)

			Dim expOutSchema As Schema = (New SequenceSchema.Builder()).addColumnDouble("mean(col0)").addColumn(New LongMetaData("count(col1)",0L,Nothing)).addColumnDouble("max(col2)").build()

			assertEquals(expOutSchema, t.transform(schema))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceMovingWindowReduceTransform()
		Public Overridable Sub testSequenceMovingWindowReduceTransform()
			Dim seq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11))}

			Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5), New DoubleWritable((2+5)/2.0)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8), New DoubleWritable((2+5+8)/3.0)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11), New DoubleWritable((5+8+11)/3.0))}

			Dim exp2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2), NullWritable.INSTANCE), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5), NullWritable.INSTANCE), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8), New DoubleWritable((2+5+8)/3.0)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11), New DoubleWritable((5+8+11)/3.0))}

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnsDouble("col%d",0,2).build()
			Dim expOutSchema1 As Schema = (New SequenceSchema.Builder()).addColumnsDouble("col%d",0,2).addColumnDouble("mean(3,col2)").build()
			Dim expOutSchema2 As Schema = (New SequenceSchema.Builder()).addColumnsDouble("col%d",0,2).addColumnDouble("newCol").build()

			Dim t1 As New SequenceMovingWindowReduceTransform("col2",3,ReduceOp.Mean)
			Dim t2 As New SequenceMovingWindowReduceTransform("col2","newCol", 3,ReduceOp.Mean, SequenceMovingWindowReduceTransform.EdgeCaseHandling.SpecifiedValue, NullWritable.INSTANCE)

			t1.InputSchema = schema
			assertEquals(expOutSchema1, t1.transform(schema))

			t2.InputSchema = schema
			assertEquals(expOutSchema2, t2.transform(schema))

			Dim act1 As IList(Of IList(Of Writable)) = t1.mapSequence(seq)
			Dim act2 As IList(Of IList(Of Writable)) = t2.mapSequence(seq)

			assertEquals(exp1, act1)
			assertEquals(exp2, act2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTrimSequenceTransform()
		Public Overridable Sub testTrimSequenceTransform()
			Dim seq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11))}

			Dim expTrimFirst As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11))}

			Dim expTrimLast As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5))}

			Dim tFirst As New org.datavec.api.transform.sequence.Trim.SequenceTrimTransform(2, True)
			Dim tLast As New org.datavec.api.transform.sequence.Trim.SequenceTrimTransform(2, False)

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnsDouble("col%d",0,2).build()
			tFirst.InputSchema = schema
			tLast.InputSchema = schema

			assertEquals(expTrimFirst, tFirst.mapSequence(seq))
			assertEquals(expTrimLast, tLast.mapSequence(seq))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceTrimToLengthTransform()
		Public Overridable Sub testSequenceTrimToLengthTransform()
			Dim seq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11))}

			Dim expTrimLength3 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8))}

			Dim s As Schema = (New Schema.Builder()).addColumnsDouble("first", "second", "third").build()

			Dim p As TransformProcess = (New TransformProcess.Builder(s)).trimSequenceToLength(3).build()

			Dim [out] As IList(Of IList(Of Writable)) = p.executeSequence(seq)
			assertEquals(expTrimLength3, [out])


			Dim seq2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5))}

			[out] = p.executeSequence(seq2)
			assertEquals(seq2, [out])

			Dim json As String = p.toJson()
			Dim tp2 As TransformProcess = TransformProcess.fromJson(json)
			assertEquals(expTrimLength3, tp2.executeSequence(seq))
			assertEquals(seq2, tp2.executeSequence(seq2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceTrimToLengthTransformTrimOrPad()
		Public Overridable Sub testSequenceTrimToLengthTransformTrimOrPad()
			Dim seq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11)), Arrays.asList(Of Writable)(New DoubleWritable(12), New DoubleWritable(13), New DoubleWritable(14))}

			Dim seq2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5))}

			Dim expTrimLength4 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11))}

			Dim s As Schema = (New Schema.Builder()).addColumnsDouble("first", "second", "third").build()

			Dim p As TransformProcess = (New TransformProcess.Builder(s)).trimOrPadSequenceToLength(4, New List(Of Writable) From {Of Writable}).build()

			Dim [out] As IList(Of IList(Of Writable)) = p.executeSequence(seq)
			assertEquals(expTrimLength4, [out])


			Dim exp2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(900), New DoubleWritable(901), New DoubleWritable(902)), Arrays.asList(Of Writable)(New DoubleWritable(900), New DoubleWritable(901), New DoubleWritable(902))}

			[out] = p.executeSequence(seq2)
			assertEquals(exp2, [out])


			Dim json As String = p.toJson()
			Dim tp2 As TransformProcess = TransformProcess.fromJson(json)
			assertEquals(expTrimLength4, tp2.executeSequence(seq))
			assertEquals(exp2, tp2.executeSequence(seq2))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSequenceOffsetTransform()
		Public Overridable Sub testSequenceOffsetTransform()

			Dim seq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(11))}

			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnsDouble("col%d",0,2).build()

			'First: test InPlace
			Dim exp1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(1), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(4), New DoubleWritable(11))}

			Dim exp2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(7), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(10), New DoubleWritable(5))}

			'In-place + trim
			Dim t_inplace_trim_p2 As New SequenceOffsetTransform(Collections.singletonList("col1"), 2, SequenceOffsetTransform.OperationType.InPlace, SequenceOffsetTransform.EdgeHandling.TrimSequence, Nothing)
			Dim t_inplace_trim_m2 As New SequenceOffsetTransform(Collections.singletonList("col1"), -2, SequenceOffsetTransform.OperationType.InPlace, SequenceOffsetTransform.EdgeHandling.TrimSequence, Nothing)
			t_inplace_trim_p2.InputSchema = schema
			t_inplace_trim_m2.InputSchema = schema

			assertEquals(exp1, t_inplace_trim_p2.mapSequence(seq))
			assertEquals(exp2, t_inplace_trim_m2.mapSequence(seq))


			'In-place + specified
			Dim t_inplace_specified_p2 As New SequenceOffsetTransform(Collections.singletonList("col1"), 2, SequenceOffsetTransform.OperationType.InPlace, SequenceOffsetTransform.EdgeHandling.SpecifiedValue, NullWritable.INSTANCE)
			Dim t_inplace_specified_m2 As New SequenceOffsetTransform(Collections.singletonList("col1"), -2, SequenceOffsetTransform.OperationType.InPlace, SequenceOffsetTransform.EdgeHandling.SpecifiedValue, NullWritable.INSTANCE)
			t_inplace_specified_p2.InputSchema = schema
			t_inplace_specified_m2.InputSchema = schema

			Dim exp3 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), NullWritable.INSTANCE, New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), NullWritable.INSTANCE, New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(1), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(4), New DoubleWritable(11))}
			Dim exp4 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(7), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(10), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), NullWritable.INSTANCE, New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), NullWritable.INSTANCE, New DoubleWritable(11))}

			assertEquals(exp3, t_inplace_specified_p2.mapSequence(seq))
			assertEquals(exp4, t_inplace_specified_m2.mapSequence(seq))




			'Second: test NewColumn
			Dim exp1a As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(1), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(4), New DoubleWritable(11))}

			Dim exp2a As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(7), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(10), New DoubleWritable(5))}
			Dim t_newcol_trim_p2 As New SequenceOffsetTransform(Collections.singletonList("col1"), 2, SequenceOffsetTransform.OperationType.NewColumn, SequenceOffsetTransform.EdgeHandling.TrimSequence, Nothing)
			Dim t_newcol_trim_m2 As New SequenceOffsetTransform(Collections.singletonList("col1"), -2, SequenceOffsetTransform.OperationType.NewColumn, SequenceOffsetTransform.EdgeHandling.TrimSequence, Nothing)
			t_newcol_trim_p2.InputSchema = schema
			t_newcol_trim_m2.InputSchema = schema

			assertEquals(exp1a, t_newcol_trim_p2.mapSequence(seq))
			assertEquals(exp2a, t_newcol_trim_m2.mapSequence(seq))

			Dim exp3a As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), NullWritable.INSTANCE, New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), NullWritable.INSTANCE, New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), New DoubleWritable(1), New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), New DoubleWritable(4), New DoubleWritable(11))}
			Dim exp4a As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New DoubleWritable(0), New DoubleWritable(1), New DoubleWritable(7), New DoubleWritable(2)), Arrays.asList(Of Writable)(New DoubleWritable(3), New DoubleWritable(4), New DoubleWritable(10), New DoubleWritable(5)), Arrays.asList(Of Writable)(New DoubleWritable(6), New DoubleWritable(7), NullWritable.INSTANCE, New DoubleWritable(8)), Arrays.asList(Of Writable)(New DoubleWritable(9), New DoubleWritable(10), NullWritable.INSTANCE, New DoubleWritable(11))}

			Dim t_newcol_specified_p2 As New SequenceOffsetTransform(Collections.singletonList("col1"), 2, SequenceOffsetTransform.OperationType.NewColumn, SequenceOffsetTransform.EdgeHandling.SpecifiedValue, NullWritable.INSTANCE)
			Dim t_newcol_specified_m2 As New SequenceOffsetTransform(Collections.singletonList("col1"), -2, SequenceOffsetTransform.OperationType.NewColumn, SequenceOffsetTransform.EdgeHandling.SpecifiedValue, NullWritable.INSTANCE)
			t_newcol_specified_p2.InputSchema = schema
			t_newcol_specified_m2.InputSchema = schema

			assertEquals(exp3a, t_newcol_specified_p2.mapSequence(seq))
			assertEquals(exp4a, t_newcol_specified_m2.mapSequence(seq))


			'Finally: check edge case
			assertEquals(java.util.Collections.emptyList(), t_inplace_trim_p2.mapSequence(exp1))
			assertEquals(java.util.Collections.emptyList(), t_inplace_trim_m2.mapSequence(exp1))
			assertEquals(java.util.Collections.emptyList(), t_newcol_trim_p2.mapSequence(exp1))
			assertEquals(java.util.Collections.emptyList(), t_newcol_trim_m2.mapSequence(exp1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStringListToCountsNDArrayTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStringListToCountsNDArrayTransform()

			Dim t As New StringListToCountsNDArrayTransform("inCol", "outCol", New List(Of String) From {"cat","dog","horse"}, ",", False, True)

			Dim s As Schema = (New Schema.Builder()).addColumnString("inCol").build()
			t.InputSchema = s

			Dim l As IList(Of Writable) = Collections.singletonList(Of Writable)(New Text("cat,cat,dog,dog,dog,unknown"))

			Dim [out] As IList(Of Writable) = t.map(l)

			Dim exp As INDArray = Nd4j.create(New Double(){2, 3, 0}, New Long(){1, 3}, Nd4j.dataType())
			assertEquals(Collections.singletonList(New NDArrayWritable(exp)), [out])

			Dim json As String = JsonMappers.Mapper.writeValueAsString(t)
			Dim transform2 As Transform = JsonMappers.Mapper.readValue(json, GetType(StringListToCountsNDArrayTransform))
			assertEquals(t, transform2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testStringListToIndicesNDArrayTransform() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStringListToIndicesNDArrayTransform()

			Dim t As New StringListToIndicesNDArrayTransform("inCol", "outCol", New List(Of String) From {"apple", "cat","dog","horse"}, ",", False, True)

			Dim s As Schema = (New Schema.Builder()).addColumnString("inCol").build()
			t.InputSchema = s

			Dim l As IList(Of Writable) = Collections.singletonList(Of Writable)(New Text("cat,dog,dog,dog,unknown"))

			Dim [out] As IList(Of Writable) = t.map(l)

			assertEquals(Collections.singletonList(New NDArrayWritable(Nd4j.create(New Double(){1, 2, 2, 2}, New Long(){1, 4}, Nd4j.dataType()))), [out])

			Dim json As String = JsonMappers.Mapper.writeValueAsString(t)
			Dim transform2 As Transform = JsonMappers.Mapper.readValue(json, GetType(StringListToIndicesNDArrayTransform))
			assertEquals(t, transform2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTextToCharacterIndexTransform()
		Public Overridable Sub testTextToCharacterIndexTransform()

			Dim s As Schema = (New Schema.Builder()).addColumnString("col").addColumnDouble("d").build()

			Dim inSeq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("text"), New DoubleWritable(1.0)), Arrays.asList(Of Writable)(New Text("ab"), New DoubleWritable(2.0))}

			Dim map As IDictionary(Of Char, Integer) = New Dictionary(Of Char, Integer)()
			map("a"c) = 0
			map("b"c) = 1
			map("e"c) = 2
			map("t"c) = 3
			map("x"c) = 4

			Dim exp As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New IntWritable(3), New DoubleWritable(1.0)), Arrays.asList(Of Writable)(New IntWritable(2), New DoubleWritable(1.0)), Arrays.asList(Of Writable)(New IntWritable(4), New DoubleWritable(1.0)), Arrays.asList(Of Writable)(New IntWritable(3), New DoubleWritable(1.0)), Arrays.asList(Of Writable)(New IntWritable(0), New DoubleWritable(2.0)), Arrays.asList(Of Writable)(New IntWritable(1), New DoubleWritable(2.0))}

			Dim t As Transform = New TextToCharacterIndexTransform("col", "newName", map, False)
			t.InputSchema = s

			Dim outputSchema As Schema = t.transform(s)
			assertEquals(2, outputSchema.getColumnNames().Count)
			assertEquals(ColumnType.Integer, outputSchema.getType(0))
			assertEquals(ColumnType.Double, outputSchema.getType(1))

			Dim intMetadata As IntegerMetaData = DirectCast(outputSchema.getMetaData(0), IntegerMetaData)
			assertEquals(0, CInt(Math.Truncate(intMetadata.getMinAllowedValue())))
			assertEquals(4, CInt(Math.Truncate(intMetadata.getMaxAllowedValue())))

			Dim [out] As IList(Of IList(Of Writable)) = t.mapSequence(inSeq)
			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTextToTermIndexSequenceTransform()
		Public Overridable Sub testTextToTermIndexSequenceTransform()

			Dim schema As Schema = (New Schema.Builder()).addColumnString("ID").addColumnString("TEXT").addColumnDouble("FEATURE").build()
			Dim vocab As IList(Of String) = New List(Of String) From {"zero", "one", "two", "three"}
			Dim inSeq As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("a"), New Text("zero four two"), New DoubleWritable(4.2)), Arrays.asList(Of Writable)(New Text("b"), New Text("six one two four three five"), New DoubleWritable(87.9))}

			Dim expSchema As Schema = (New Schema.Builder()).addColumnString("ID").addColumnInteger("INDEXSEQ", 0, 3).addColumnDouble("FEATURE").build()
			Dim exp As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("a"), New IntWritable(0), New DoubleWritable(4.2)), Arrays.asList(Of Writable)(New Text("a"), New IntWritable(2), New DoubleWritable(4.2)), Arrays.asList(Of Writable)(New Text("b"), New IntWritable(1), New DoubleWritable(87.9)), Arrays.asList(Of Writable)(New Text("b"), New IntWritable(2), New DoubleWritable(87.9)), Arrays.asList(Of Writable)(New Text("b"), New IntWritable(3), New DoubleWritable(87.9))}

			Dim t As Transform = New TextToTermIndexSequenceTransform("TEXT", "INDEXSEQ", vocab, " ", False)
			t.InputSchema = schema

			Dim outputSchema As Schema = t.transform(schema)
			assertEquals(expSchema.getColumnNames(), outputSchema.getColumnNames())
			assertEquals(expSchema.getColumnTypes(), outputSchema.getColumnTypes())
			assertEquals(expSchema, outputSchema)

			assertEquals(3, outputSchema.getColumnNames().Count)
			assertEquals(ColumnType.String, outputSchema.getType(0))
			assertEquals(ColumnType.Integer, outputSchema.getType(1))
			assertEquals(ColumnType.Double, outputSchema.getType(2))

			Dim intMetadata As IntegerMetaData = DirectCast(outputSchema.getMetaData(1), IntegerMetaData)
			assertEquals(0, CInt(Math.Truncate(intMetadata.getMinAllowedValue())))
			assertEquals(3, CInt(Math.Truncate(intMetadata.getMaxAllowedValue())))

			Dim [out] As IList(Of IList(Of Writable)) = t.mapSequence(inSeq)
			assertEquals(exp, [out])

			Dim tp As TransformProcess = (New TransformProcess.Builder(schema)).transform(t).build()
			Dim json As String = tp.toJson()
			Dim tp2 As TransformProcess = TransformProcess.fromJson(json)
			assertEquals(tp, tp2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testFirstDigitTransform()
		Public Overridable Sub testFirstDigitTransform()
			Dim s As Schema = (New Schema.Builder()).addColumnString("data").addColumnDouble("double").addColumnString("stringNumber").build()

			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).firstDigitTransform("double", "fdDouble", FirstDigitTransform.Mode.EXCEPTION_ON_INVALID).firstDigitTransform("stringNumber", "stringNumber", FirstDigitTransform.Mode.INCLUDE_OTHER_CATEGORY).build()

			Dim s2 As Schema = tp.FinalSchema
			assertEquals(java.util.Arrays.asList("data","double", "fdDouble", "stringNumber"), s2.getColumnNames())

			assertEquals(java.util.Arrays.asList(ColumnType.String, ColumnType.Double, ColumnType.Categorical, ColumnType.Categorical), s2.getColumnTypes())

			Dim [in] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("a"), New DoubleWritable(3.14159), New Text("8e-4")), Arrays.asList(Of Writable)(New Text("b"), New DoubleWritable(2.71828), New Text("7e2")), Arrays.asList(Of Writable)(New Text("c"), New DoubleWritable(1.61803), New Text("6e8")), Arrays.asList(Of Writable)(New Text("c"), New DoubleWritable(-2), New Text("non numerical"))}

			Dim expected As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("a"), New DoubleWritable(3.14159), New Text("3"), New Text("8")), Arrays.asList(Of Writable)(New Text("b"), New DoubleWritable(2.71828), New Text("2"), New Text("7")), Arrays.asList(Of Writable)(New Text("c"), New DoubleWritable(1.61803), New Text("1"), New Text("6")), Arrays.asList(Of Writable)(New Text("c"), New DoubleWritable(-2), New Text("2"), New Text("Other"))}

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For Each i As IList(Of Writable) In [in]
				[out].Add(tp.execute(i))
			Next i
			assertEquals(expected, [out])

			'Test Benfords law use case:
			Dim tp2 As TransformProcess = (New TransformProcess.Builder(s)).firstDigitTransform("double", "fdDouble", FirstDigitTransform.Mode.EXCEPTION_ON_INVALID).firstDigitTransform("stringNumber", "stringNumber", FirstDigitTransform.Mode.INCLUDE_OTHER_CATEGORY).removeColumns("data", "double").categoricalToOneHot("fdDouble", "stringNumber").reduce((New Reducer.Builder(ReduceOp.Sum)).build()).build()
		End Sub
	End Class

End Namespace