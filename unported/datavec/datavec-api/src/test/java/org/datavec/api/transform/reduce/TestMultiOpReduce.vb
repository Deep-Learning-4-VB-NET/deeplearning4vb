Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports ColumnType = org.datavec.api.transform.ColumnType
Imports ReduceOp = org.datavec.api.transform.ReduceOp
Imports Condition = org.datavec.api.transform.condition.Condition
Imports ConditionOp = org.datavec.api.transform.condition.ConditionOp
Imports StringColumnCondition = org.datavec.api.transform.condition.column.StringColumnCondition
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports StringMetaData = org.datavec.api.transform.metadata.StringMetaData
Imports org.datavec.api.transform.ops
Imports org.datavec.api.transform.ops
Imports Schema = org.datavec.api.transform.schema.Schema
Imports org.datavec.api.writable
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.fail

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

Namespace org.datavec.api.transform.reduce


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) public class TestMultiOpReduce extends org.nd4j.common.tests.BaseND4JTest
	Public Class TestMultiOpReduce
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultiOpReducerDouble()
		Public Overridable Sub testMultiOpReducerDouble()

			Dim inputs As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New DoubleWritable(0)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New DoubleWritable(1)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New DoubleWritable(2)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New DoubleWritable(2)
			})

			Dim exp As IDictionary(Of ReduceOp, Double) = New LinkedHashMap(Of ReduceOp, Double)()
			exp(ReduceOp.Min) = 0.0
			exp(ReduceOp.Max) = 2.0
			exp(ReduceOp.Range) = 2.0
			exp(ReduceOp.Sum) = 5.0
			exp(ReduceOp.Mean) = 1.25
			exp(ReduceOp.Stdev) = 0.957427108
			exp(ReduceOp.Count) = 4.0
			exp(ReduceOp.CountUnique) = 3.0
			exp(ReduceOp.TakeFirst) = 0.0
			exp(ReduceOp.TakeLast) = 2.0

			For Each op As ReduceOp In exp.Keys

				Dim schema As Schema = (New Schema.Builder()).addColumnString("key").addColumnDouble("column").build()

				Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").build()

				reducer.InputSchema = schema
				Dim accumulator As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()

				For i As Integer = 0 To inputs.Count - 1
					accumulator.accept(inputs(i))
				Next i
				Dim [out] As IList(Of Writable) = accumulator.get()

				assertEquals(2, [out].Count)

				assertEquals([out](0), New Text("someKey"))

				Dim msg As String = op.ToString()
				assertEquals(exp(op), [out](1).toDouble(), 1e-5,msg)
			Next op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReducerInteger()
		Public Overridable Sub testReducerInteger()

			Dim inputs As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(0)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(1)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(2)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(2)
			})

			Dim exp As IDictionary(Of ReduceOp, Double) = New LinkedHashMap(Of ReduceOp, Double)()
			exp(ReduceOp.Min) = 0.0
			exp(ReduceOp.Max) = 2.0
			exp(ReduceOp.Range) = 2.0
			exp(ReduceOp.Sum) = 5.0
			exp(ReduceOp.Mean) = 1.25
			exp(ReduceOp.Stdev) = 0.957427108
			exp(ReduceOp.Count) = 4.0
			exp(ReduceOp.CountUnique) = 3.0
			exp(ReduceOp.TakeFirst) = 0.0
			exp(ReduceOp.TakeLast) = 2.0

			For Each op As ReduceOp In exp.Keys

				Dim schema As Schema = (New Schema.Builder()).addColumnString("key").addColumnInteger("column").build()

				Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").build()

				reducer.InputSchema = schema
				Dim accumulator As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()

				For i As Integer = 0 To inputs.Count - 1
					accumulator.accept(inputs(i))
				Next i
				Dim [out] As IList(Of Writable) = accumulator.get()

				assertEquals(2, [out].Count)

				assertEquals([out](0), New Text("someKey"))

				Dim msg As String = op.ToString()
				assertEquals(exp(op), [out](1).toDouble(), 1e-5,msg)
			Next op
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReduceString()
		Public Overridable Sub testReduceString()
			Dim inputs As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New Text("1")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New Text("2")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New Text("3")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New Text("4")
			})

			Dim exp As IDictionary(Of ReduceOp, String) = New LinkedHashMap(Of ReduceOp, String)()
			exp(ReduceOp.Append) = "1234"
			exp(ReduceOp.Prepend) = "4321"

			For Each op As ReduceOp In exp.Keys

				Dim schema As Schema = (New Schema.Builder()).addColumnString("key").addColumnsString("column").build()

				Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").build()

				reducer.InputSchema = schema
				Dim accumulator As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()

				For i As Integer = 0 To inputs.Count - 1
					accumulator.accept(inputs(i))
				Next i
				Dim [out] As IList(Of Writable) = accumulator.get()

				assertEquals(2, [out].Count)

				assertEquals([out](0), New Text("someKey"))

				Dim msg As String = op.ToString()
				assertEquals(exp(op), [out](1).ToString(),msg)
			Next op
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testReduceIntegerIgnoreInvalidValues()
		Public Overridable Sub testReduceIntegerIgnoreInvalidValues()

			Dim inputs As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New Text("0")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New Text("1")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(2)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New Text("ignore me")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New Text("also ignore me")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New Text("2")
			})


			Dim exp As IDictionary(Of ReduceOp, Double) = New LinkedHashMap(Of ReduceOp, Double)()
			exp(ReduceOp.Min) = 0.0
			exp(ReduceOp.Max) = 2.0
			exp(ReduceOp.Range) = 2.0
			exp(ReduceOp.Sum) = 5.0
			exp(ReduceOp.Mean) = 1.25
			exp(ReduceOp.Stdev) = 0.957427108
			exp(ReduceOp.Count) = 4.0
			exp(ReduceOp.CountUnique) = 3.0
			exp(ReduceOp.TakeFirst) = 0.0
			exp(ReduceOp.TakeLast) = 2.0

			For Each op As ReduceOp In exp.Keys
				Dim schema As Schema = (New Schema.Builder()).addColumnString("key").addColumnInteger("column").build()

				Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").setIgnoreInvalid("column").build()

				reducer.InputSchema = schema

				Dim accumulator As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()

				For i As Integer = 0 To inputs.Count - 1
					accumulator.accept(inputs(i))
				Next i
				Dim [out] As IList(Of Writable) = accumulator.get()

				assertEquals(2, [out].Count)

				assertEquals([out](0), New Text("someKey"))

				Dim msg As String = op.ToString()
				assertEquals(exp(op), [out](1).toDouble(), 1e-5,msg)
			Next op

			For Each op As ReduceOp In java.util.Arrays.asList(ReduceOp.Min, ReduceOp.Max, ReduceOp.Range, ReduceOp.Sum, ReduceOp.Mean, ReduceOp.Stdev)
				'Try the same thing WITHOUT setIgnoreInvalid -> expect exception

				Dim schema As Schema = (New Schema.Builder()).addColumnString("key").addColumnInteger("column").build()

				Dim reducer As Reducer = (New Reducer.Builder(op)).keyColumns("key").build()
				reducer.InputSchema = schema
				Dim accu As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()

				Try
					For Each i As IList(Of Writable) In inputs
						accu.accept(i)
					Next i
					fail("No exception thrown for invalid input: op=" & op)
				Catch e As System.FormatException
					'ok
				End Try
			Next op
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomReductions()
		Public Overridable Sub testCustomReductions()

			Dim inputs As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(1),
				New Text("zero"),
				New DoubleWritable(0)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(2),
				New Text("one"),
				New DoubleWritable(1)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(3),
				New Text("two"),
				New DoubleWritable(2)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(4),
				New Text("three"),
				New DoubleWritable(3)
			})

			Dim expected As IList(Of Writable) = New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(10),
				New Text("one"),
				New DoubleWritable(1)
			}


			Dim schema As Schema = (New Schema.Builder()).addColumnString("key").addColumnInteger("intCol").addColumnString("textCol").addColumnString("doubleCol").build()

			Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).keyColumns("key").customReduction("textCol", New CustomReduceTakeSecond()).customReduction("doubleCol", New CustomReduceTakeSecond()).build()

			reducer.InputSchema = schema


			Dim accumulator As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()

			For i As Integer = 0 To inputs.Count - 1
				accumulator.accept(inputs(i))
			Next i
			Dim [out] As IList(Of Writable) = accumulator.get()

			assertEquals(4, [out].Count)
			assertEquals(expected, [out])

			'Check schema:
			Dim expNames() As String = {"key", "sum(intCol)", "myCustomReduce(textCol)", "myCustomReduce(doubleCol)"}
			Dim expTypes() As ColumnType = {ColumnType.String, ColumnType.Integer, ColumnType.String, ColumnType.String}
			Dim outSchema As Schema = reducer.transform(schema)

			assertEquals(4, outSchema.numColumns())
			For i As Integer = 0 To 3
				assertEquals(expNames(i), outSchema.getName(i))
				assertEquals(expTypes(i), outSchema.getType(i))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomReductionsWithCondition()
		Public Overridable Sub testCustomReductionsWithCondition()

			Dim inputs As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(1),
				New Text("zero"),
				New DoubleWritable(0)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(2),
				New Text("one"),
				New DoubleWritable(1)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(3),
				New Text("two"),
				New DoubleWritable(2)
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(4),
				New Text("three"),
				New DoubleWritable(3)
			})

			Dim expected As IList(Of Writable) = New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(10),
				New IntWritable(3),
				New DoubleWritable(1)
			}


			Dim schema As Schema = (New Schema.Builder()).addColumnString("key").addColumnInteger("intCol").addColumnString("textCol").addColumnString("doubleCol").build()

			Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Sum)).keyColumns("key").conditionalReduction("textCol", "condTextCol", ReduceOp.Count, New StringColumnCondition("textCol", ConditionOp.NotEqual, "three")).customReduction("doubleCol", New CustomReduceTakeSecond()).build()

			reducer.InputSchema = schema


			Dim accumulator As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()

			For i As Integer = 0 To inputs.Count - 1
				accumulator.accept(inputs(i))
			Next i
			Dim [out] As IList(Of Writable) = accumulator.get()

			assertEquals(4, [out].Count)
			assertEquals(expected, [out])

			'Check schema:
			Dim expNames() As String = {"key", "sum(intCol)", "condTextCol", "myCustomReduce(doubleCol)"}
			Dim expTypes() As ColumnType = {ColumnType.String, ColumnType.Integer, ColumnType.Long, ColumnType.String}
			Dim outSchema As Schema = reducer.transform(schema)

			assertEquals(4, outSchema.numColumns())
			For i As Integer = 0 To 3
				assertEquals(expNames(i), outSchema.getName(i))
				assertEquals(expTypes(i), outSchema.getType(i))
			Next i
		End Sub

		<Serializable>
		Private Class CustomReduceTakeSecond
			Implements AggregableColumnReduction

			Public Overridable Function reduceOp() As IAggregableReduceOp(Of Writable, IList(Of Writable)) Implements AggregableColumnReduction.reduceOp
				'For testing: let's take the second value
				Return New AggregableMultiOp(Of Writable, IList(Of Writable))(Collections.singletonList(Of IAggregableReduceOp(Of Writable, Writable))(New AggregableSecond(Of Writable, IList(Of Writable))()))
			End Function

			Public Overridable Function getColumnsOutputName(ByVal columnInputName As String) As IList(Of String)
				Return Collections.singletonList("myCustomReduce(" & columnInputName & ")")
			End Function

			Public Overridable Function getColumnOutputMetaData(ByVal newColumnName As IList(Of String), ByVal columnInputMeta As ColumnMetaData) As IList(Of ColumnMetaData)
				Dim thiscolumnMeta As ColumnMetaData = New StringMetaData(newColumnName(0))
				Return Collections.singletonList(thiscolumnMeta)
			End Function

			<Serializable>
			Public Class AggregableSecond(Of T)
				Implements IAggregableReduceOp(Of T, Writable)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private T firstMet = null;
				Friend firstMet As T = Nothing
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private T elem = null;
				Friend elem As T = Nothing

				Public Overridable Sub accept(ByVal element As T)
					If firstMet Is Nothing Then
						firstMet = element
					Else
						If elem Is Nothing Then
							elem = element
						End If
					End If
				End Sub

				Public Overridable Sub combine(Of W As IAggregableReduceOp(Of T, Writable))(ByVal accu As W) Implements IAggregableReduceOp(Of T, Writable).combine
					If TypeOf accu Is AggregableSecond AndAlso elem Is Nothing Then
						If firstMet Is Nothing Then ' this accumulator is empty, import accu
							Dim accumulator As AggregableSecond(Of T) = CType(accu, AggregableSecond)
							Dim otherFirst As T = accumulator.getFirstMet()
							Dim otherElement As T = accumulator.getElem()
							If otherFirst IsNot Nothing Then
								firstMet = otherFirst
							End If
							If otherElement IsNot Nothing Then
								elem = otherElement
							End If
						Else ' we have the first element, they may have the rest
							Dim accumulator As AggregableSecond(Of T) = CType(accu, AggregableSecond)
							Dim otherFirst As T = accumulator.getFirstMet()
							If otherFirst IsNot Nothing Then
								elem = otherFirst
							End If
						End If
					End If
				End Sub

				Public Overridable Function get() As Writable
					Return UnsafeWritableInjector.inject(elem)
				End Function
			End Class

			''' <summary>
			''' Get the output schema for this transformation, given an input schema
			''' </summary>
			''' <param name="inputSchema"> </param>
			Public Overrides Function transform(ByVal inputSchema As Schema) As Schema
				Return Nothing
			End Function

			''' <summary>
			''' Set the input schema.
			''' </summary>
			''' <param name="inputSchema"> </param>
			Public Overridable Property InputSchema As Schema
				Set(ByVal inputSchema As Schema)
    
				End Set
				Get
					Return Nothing
				End Get
			End Property


			''' <summary>
			''' The output column name
			''' after the operation has been applied
			''' </summary>
			''' <returns> the output column name </returns>
			Public Overridable Function outputColumnName() As String
				Return Nothing
			End Function

			''' <summary>
			''' The output column names
			''' This will often be the same as the input
			''' </summary>
			''' <returns> the output column names </returns>
			Public Overridable Function outputColumnNames() As String()
				Return New String(){}
			End Function

			''' <summary>
			''' Returns column names
			''' this op is meant to run on
			''' 
			''' @return
			''' </summary>
			Public Overridable Function columnNames() As String()
				Return New String(){}
			End Function

			''' <summary>
			''' Returns a singular column name
			''' this op is meant to run on
			''' 
			''' @return
			''' </summary>
			Public Overridable Function columnName() As String
				Return Nothing
			End Function
		End Class



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testConditionalReduction()
		Public Overridable Sub testConditionalReduction()

			Dim schema As Schema = (New Schema.Builder()).addColumnString("key").addColumnInteger("intCol").addColumnString("filterCol").addColumnString("textCol").build()

			Dim inputs As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(1),
				New Text("a"),
				New Text("zero")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(2),
				New Text("b"),
				New Text("one")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(3),
				New Text("a"),
				New Text("two")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(4),
				New Text("b"),
				New Text("three")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(5),
				New Text("a"),
				New Text("three")
			})
			inputs.Add(New List(Of Writable) From {
				New Text("someKey"),
				New IntWritable(6),
				New Text("b"),
				New Text("three")
			})

			Dim condition As Condition = New StringColumnCondition("filterCol", ConditionOp.Equal, "a")

			Dim reducer As Reducer = (New Reducer.Builder(ReduceOp.Stdev)).keyColumns("key").conditionalReduction("intCol", "sumOfAs", ReduceOp.Sum, condition).countUniqueColumns("filterCol", "textCol").build()

			reducer.InputSchema = schema

			Dim accumulator As IAggregableReduceOp(Of IList(Of Writable), IList(Of Writable)) = reducer.aggregableReducer()

			For i As Integer = 0 To inputs.Count - 1
				accumulator.accept(inputs(i))
			Next i
			Dim [out] As IList(Of Writable) = accumulator.get()
			Dim expected As IList(Of Writable) = New List(Of Writable) From {Of Writable}

			assertEquals(4, [out].Count)
			assertEquals(expected, [out])

			Dim outSchema As Schema = reducer.transform(schema)
			assertEquals(4, outSchema.numColumns())
			assertEquals(java.util.Arrays.asList("key", "sumOfAs", "countunique(filterCol)", "countunique(textCol)"), outSchema.getColumnNames())
			assertEquals(java.util.Arrays.asList(ColumnType.String, ColumnType.Integer, ColumnType.Long, ColumnType.Long), outSchema.getColumnTypes())
		End Sub
	End Class

End Namespace