Imports System
Imports System.Collections.Generic
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports MathOp = org.datavec.api.transform.MathOp
Imports ReduceOp = org.datavec.api.transform.ReduceOp
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports Reducer = org.datavec.api.transform.reduce.Reducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports FirstDigitTransform = org.datavec.api.transform.transform.categorical.FirstDigitTransform
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports BaseSparkTest = org.datavec.spark.BaseSparkTest
Imports Disabled = org.junit.jupiter.api.Disabled
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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
Namespace org.datavec.spark.transform


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Execution Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) class ExecutionTest extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Friend Class ExecutionTest
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Execution Simple") void testExecutionSimple()
		Friend Overridable Sub testExecutionSimple()
			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("col0").addColumnCategorical("col1", "state0", "state1", "state2").addColumnDouble("col2").build()
			Dim tp As TransformProcess = (New TransformProcess.Builder(schema)).categoricalToInteger("col1").doubleMathOp("col2", MathOp.Add, 10.0).build()
			Dim inputData As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputData.Add(New List(Of Writable) From {
				New IntWritable(0),
				New Text("state2"),
				New DoubleWritable(0.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(1),
				New Text("state1"),
				New DoubleWritable(1.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(2),
				New Text("state0"),
				New DoubleWritable(2.1)
			})
			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(inputData)
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(SparkTransformExecutor.execute(rdd, tp).collect())
			[out].Sort(System.Collections.IComparer.comparingInt(Function(o) o.get(0).toInt()))
			Dim expected As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expected.Add(New List(Of Writable) From {
				New IntWritable(0),
				New IntWritable(2),
				New DoubleWritable(10.1)
			})
			expected.Add(New List(Of Writable) From {
				New IntWritable(1),
				New IntWritable(1),
				New DoubleWritable(11.1)
			})
			expected.Add(New List(Of Writable) From {
				New IntWritable(2),
				New IntWritable(0),
				New DoubleWritable(12.1)
			})
			assertEquals(expected, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Execution Sequence") void testExecutionSequence()
		Friend Overridable Sub testExecutionSequence()
			Dim schema As Schema = (New SequenceSchema.Builder()).addColumnInteger("col0").addColumnCategorical("col1", "state0", "state1", "state2").addColumnDouble("col2").build()
			Dim tp As TransformProcess = (New TransformProcess.Builder(schema)).categoricalToInteger("col1").doubleMathOp("col2", MathOp.Add, 10.0).build()
			Dim inputSequences As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Dim seq1 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			seq1.Add(New List(Of Writable) From {
				New IntWritable(0),
				New Text("state2"),
				New DoubleWritable(0.1)
			})
			seq1.Add(New List(Of Writable) From {
				New IntWritable(1),
				New Text("state1"),
				New DoubleWritable(1.1)
			})
			seq1.Add(New List(Of Writable) From {
				New IntWritable(2),
				New Text("state0"),
				New DoubleWritable(2.1)
			})
			Dim seq2 As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			seq2.Add(New List(Of Writable) From {
				New IntWritable(4),
				New Text("state1"),
				New DoubleWritable(4.1)
			})
			seq2.Add(New List(Of Writable) From {
				New IntWritable(3),
				New Text("state0"),
				New DoubleWritable(3.1)
			})
			inputSequences.Add(seq1)
			inputSequences.Add(seq2)
			Dim rdd As JavaRDD(Of IList(Of IList(Of Writable))) = sc.parallelize(inputSequences)
			Dim [out] As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))(SparkTransformExecutor.executeSequenceToSequence(rdd, tp).collect())
			[out].Sort(Function(o1, o2) -Integer.compare(o1.size(), o2.size()))
			Dim expectedSequence As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			Dim seq1e As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			seq1e.Add(New List(Of Writable) From {
				New IntWritable(0),
				New IntWritable(2),
				New DoubleWritable(10.1)
			})
			seq1e.Add(New List(Of Writable) From {
				New IntWritable(1),
				New IntWritable(1),
				New DoubleWritable(11.1)
			})
			seq1e.Add(New List(Of Writable) From {
				New IntWritable(2),
				New IntWritable(0),
				New DoubleWritable(12.1)
			})
			Dim seq2e As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			seq2e.Add(New List(Of Writable) From {
				New IntWritable(4),
				New IntWritable(1),
				New DoubleWritable(14.1)
			})
			seq2e.Add(New List(Of Writable) From {
				New IntWritable(3),
				New IntWritable(0),
				New DoubleWritable(13.1)
			})
			expectedSequence.Add(seq1e)
			expectedSequence.Add(seq2e)
			assertEquals(expectedSequence, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reduction Global") void testReductionGlobal()
		Friend Overridable Sub testReductionGlobal()
			Dim [in] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New Text("first"), New DoubleWritable(3.0)), Arrays.asList(Of Writable)(New Text("second"), New DoubleWritable(5.0))}
			Dim inData As JavaRDD(Of IList(Of Writable)) = sc.parallelize([in])
			Dim s As Schema = (New Schema.Builder()).addColumnString("textCol").addColumnDouble("doubleCol").build()
			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).reduce((New Reducer.Builder(ReduceOp.TakeFirst)).takeFirstColumns("textCol").meanColumns("doubleCol").build()).build()
			Dim outRdd As JavaRDD(Of IList(Of Writable)) = SparkTransformExecutor.execute(inData, tp)
			Dim [out] As IList(Of IList(Of Writable)) = outRdd.collect()
			Dim expOut As IList(Of IList(Of Writable)) = Collections.singletonList(java.util.Arrays.asList(New Text("first"), New DoubleWritable(4.0)))
			assertEquals(expOut, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reduction By Key") void testReductionByKey()
		Friend Overridable Sub testReductionByKey()
			Dim [in] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New IntWritable(0), New Text("first"), New DoubleWritable(3.0)), Arrays.asList(Of Writable)(New IntWritable(0), New Text("second"), New DoubleWritable(5.0)), Arrays.asList(Of Writable)(New IntWritable(1), New Text("f"), New DoubleWritable(30.0)), Arrays.asList(Of Writable)(New IntWritable(1), New Text("s"), New DoubleWritable(50.0))}
			Dim inData As JavaRDD(Of IList(Of Writable)) = sc.parallelize([in])
			Dim s As Schema = (New Schema.Builder()).addColumnInteger("intCol").addColumnString("textCol").addColumnDouble("doubleCol").build()
			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).reduce((New Reducer.Builder(ReduceOp.TakeFirst)).keyColumns("intCol").takeFirstColumns("textCol").meanColumns("doubleCol").build()).build()
			Dim outRdd As JavaRDD(Of IList(Of Writable)) = SparkTransformExecutor.execute(inData, tp)
			Dim [out] As IList(Of IList(Of Writable)) = outRdd.collect()
			Dim expOut As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New IntWritable(0), New Text("first"), New DoubleWritable(4.0)), Arrays.asList(Of Writable)(New IntWritable(1), New Text("f"), New DoubleWritable(40.0))}
			[out] = New List(Of IList(Of Writable))([out])
			[out].Sort(System.Collections.IComparer.comparingInt(Function(o) o.get(0).toInt()))
			assertEquals(expOut, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Unique Multi Col") void testUniqueMultiCol()
		Friend Overridable Sub testUniqueMultiCol()
			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("col0").addColumnCategorical("col1", "state0", "state1", "state2").addColumnDouble("col2").build()
			Dim inputData As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputData.Add(New List(Of Writable) From {
				New IntWritable(0),
				New Text("state2"),
				New DoubleWritable(0.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(1),
				New Text("state1"),
				New DoubleWritable(1.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(2),
				New Text("state0"),
				New DoubleWritable(2.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(0),
				New Text("state2"),
				New DoubleWritable(0.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(1),
				New Text("state1"),
				New DoubleWritable(1.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(2),
				New Text("state0"),
				New DoubleWritable(2.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(0),
				New Text("state2"),
				New DoubleWritable(0.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(1),
				New Text("state1"),
				New DoubleWritable(1.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(2),
				New Text("state0"),
				New DoubleWritable(2.1)
			})
			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize(inputData)
			Dim l As IDictionary(Of String, IList(Of Writable)) = AnalyzeSpark.getUnique(java.util.Arrays.asList("col0", "col1"), schema, rdd)
			assertEquals(2, l.Count)
			Dim c0 As IList(Of Writable) = l("col0")
			assertEquals(3, c0.Count)
			assertTrue(c0.Contains(New IntWritable(0)) AndAlso c0.Contains(New IntWritable(1)) AndAlso c0.Contains(New IntWritable(2)))
			Dim c1 As IList(Of Writable) = l("col1")
			assertEquals(3, c1.Count)
			assertTrue(c1.Contains(New Text("state0")) AndAlso c1.Contains(New Text("state1")) AndAlso c1.Contains(New Text("state2")))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test First Digit Transform Benfords Law") void testFirstDigitTransformBenfordsLaw()
		Friend Overridable Sub testFirstDigitTransformBenfordsLaw()
			Dim s As Schema = (New Schema.Builder()).addColumnString("data").addColumnDouble("double").addColumnString("stringNumber").build()
			Dim [in] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(Of Writable)(New Text("a"), New DoubleWritable(3.14159), New Text("8e-4")), Arrays.asList(Of Writable)(New Text("a2"), New DoubleWritable(3.14159), New Text("7e-4")), Arrays.asList(Of Writable)(New Text("b"), New DoubleWritable(2.71828), New Text("7e2")), Arrays.asList(Of Writable)(New Text("c"), New DoubleWritable(1.61803), New Text("6e8")), Arrays.asList(Of Writable)(New Text("c"), New DoubleWritable(1.61803), New Text("2.0")), Arrays.asList(Of Writable)(New Text("c"), New DoubleWritable(1.61803), New Text("2.1")), Arrays.asList(Of Writable)(New Text("c"), New DoubleWritable(1.61803), New Text("2.2")), Arrays.asList(Of Writable)(New Text("c"), New DoubleWritable(-2), New Text("non numerical"))}
			' Test Benfords law use case:
			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).firstDigitTransform("double", "fdDouble", FirstDigitTransform.Mode.EXCEPTION_ON_INVALID).firstDigitTransform("stringNumber", "stringNumber", FirstDigitTransform.Mode.INCLUDE_OTHER_CATEGORY).removeAllColumnsExceptFor("stringNumber").categoricalToOneHot("stringNumber").reduce((New Reducer.Builder(ReduceOp.Sum)).build()).build()
			Dim rdd As JavaRDD(Of IList(Of Writable)) = sc.parallelize([in])
			Dim [out] As IList(Of IList(Of Writable)) = SparkTransformExecutor.execute(rdd, tp).collect()
			assertEquals(1, [out].Count)
			Dim l As IList(Of Writable) = [out](0)
			Dim exp As IList(Of Writable) = New List(Of Writable) From {
				New IntWritable(0),
				New IntWritable(0),
				New IntWritable(3),
				New IntWritable(0),
				New IntWritable(0),
				New IntWritable(0),
				New IntWritable(1),
				New IntWritable(2),
				New IntWritable(1),
				New IntWritable(0),
				New IntWritable(1)
			}
			assertEquals(exp, l)
		End Sub
	End Class

End Namespace