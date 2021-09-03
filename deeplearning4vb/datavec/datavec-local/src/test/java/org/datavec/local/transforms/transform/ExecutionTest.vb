Imports System.Collections.Generic
Imports MathFunction = org.datavec.api.transform.MathFunction
Imports MathOp = org.datavec.api.transform.MathOp
Imports ReduceOp = org.datavec.api.transform.ReduceOp
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports ConditionOp = org.datavec.api.transform.condition.ConditionOp
Imports DoubleColumnCondition = org.datavec.api.transform.condition.column.DoubleColumnCondition
Imports Reducer = org.datavec.api.transform.reduce.Reducer
Imports Schema = org.datavec.api.transform.schema.Schema
Imports SequenceSchema = org.datavec.api.transform.schema.SequenceSchema
Imports org.datavec.api.writable
Imports LocalTransformExecutor = org.datavec.local.transforms.LocalTransformExecutor
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.junit.jupiter.api.Assertions.assertEquals
Imports DisplayName = org.junit.jupiter.api.DisplayName
import static org.junit.jupiter.api.Assertions.assertTimeout

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
Namespace org.datavec.local.transforms.transform

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Execution Test") @Tag(TagNames.FILE_IO) @NativeTag class ExecutionTest
	Friend Class ExecutionTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Execution Ndarray") void testExecutionNdarray()
		Friend Overridable Sub testExecutionNdarray()
			Dim schema As Schema = (New Schema.Builder()).addColumnNDArray("first", New Long() { 1, 32577 }).addColumnNDArray("second", New Long() { 1, 32577 }).build()
			Dim transformProcess As TransformProcess = (New TransformProcess.Builder(schema)).ndArrayMathFunctionTransform("first", MathFunction.SIN).ndArrayMathFunctionTransform("second", MathFunction.COS).build()
			Dim functions As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			Dim firstRow As IList(Of Writable) = New List(Of Writable)()
			Dim firstArr As INDArray = Nd4j.linspace(1, 4, 4)
			Dim secondArr As INDArray = Nd4j.linspace(1, 4, 4)
			firstRow.Add(New NDArrayWritable(firstArr))
			firstRow.Add(New NDArrayWritable(secondArr))
			functions.Add(firstRow)
			Dim execute As IList(Of IList(Of Writable)) = LocalTransformExecutor.execute(functions, transformProcess)
			Dim firstResult As INDArray = CType(execute(0)(0), NDArrayWritable).get()
			Dim secondResult As INDArray = CType(execute(0)(1), NDArrayWritable).get()
			Dim expected As INDArray = Transforms.sin(firstArr)
			Dim secondExpected As INDArray = Transforms.cos(secondArr)
			assertEquals(expected, firstResult)
			assertEquals(secondExpected, secondResult)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Execution Simple") void testExecutionSimple()
		Friend Overridable Sub testExecutionSimple()
			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("col0").addColumnCategorical("col1", "state0", "state1", "state2").addColumnDouble("col2").addColumnFloat("col3").build()
			Dim tp As TransformProcess = (New TransformProcess.Builder(schema)).categoricalToInteger("col1").doubleMathOp("col2", MathOp.Add, 10.0).floatMathOp("col3", MathOp.Add, 5f).build()
			Dim inputData As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputData.Add(New List(Of Writable) From {
				New IntWritable(0),
				New Text("state2"),
				New DoubleWritable(0.1),
				New FloatWritable(0.3f)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(1),
				New Text("state1"),
				New DoubleWritable(1.1),
				New FloatWritable(1.7f)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(2),
				New Text("state0"),
				New DoubleWritable(2.1),
				New FloatWritable(3.6f)
			})
			Dim rdd As IList(Of IList(Of Writable)) = (inputData)
			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))(LocalTransformExecutor.execute(rdd, tp))
			[out].Sort(Function(o1, o2) Integer.compare(o1.get(0).toInt(), o2.get(0).toInt()))
			Dim expected As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			expected.Add(New List(Of Writable) From {Of Writable})
			expected.Add(New List(Of Writable) From {Of Writable})
			expected.Add(New List(Of Writable) From {Of Writable})
			assertEquals(expected, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Filter") void testFilter()
		Friend Overridable Sub testFilter()
			Dim filterSchema As Schema = (New Schema.Builder()).addColumnDouble("col1").addColumnDouble("col2").addColumnDouble("col3").build()
			Dim inputData As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			inputData.Add(New List(Of Writable) From {
				New IntWritable(0),
				New DoubleWritable(1),
				New DoubleWritable(0.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(1),
				New DoubleWritable(3),
				New DoubleWritable(1.1)
			})
			inputData.Add(New List(Of Writable) From {
				New IntWritable(2),
				New DoubleWritable(3),
				New DoubleWritable(2.1)
			})
			Dim transformProcess As TransformProcess = (New TransformProcess.Builder(filterSchema)).filter(New DoubleColumnCondition("col1", ConditionOp.LessThan, 1)).build()
			Dim execute As IList(Of IList(Of Writable)) = LocalTransformExecutor.execute(inputData, transformProcess)
			assertEquals(2, execute.Count)
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
				New IntWritable(3),
				New Text("state0"),
				New DoubleWritable(3.1)
			})
			seq2.Add(New List(Of Writable) From {
				New IntWritable(4),
				New Text("state1"),
				New DoubleWritable(4.1)
			})
			inputSequences.Add(seq1)
			inputSequences.Add(seq2)
			Dim rdd As IList(Of IList(Of IList(Of Writable))) = (inputSequences)
			Dim [out] As IList(Of IList(Of IList(Of Writable))) = LocalTransformExecutor.executeSequenceToSequence(rdd, tp)
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
				New IntWritable(3),
				New IntWritable(0),
				New DoubleWritable(13.1)
			})
			seq2e.Add(New List(Of Writable) From {
				New IntWritable(4),
				New IntWritable(1),
				New DoubleWritable(14.1)
			})
			expectedSequence.Add(seq1e)
			expectedSequence.Add(seq2e)
			assertEquals(expectedSequence, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reduction Global") void testReductionGlobal()
		Friend Overridable Sub testReductionGlobal()
			Dim [in] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New Text("first"), New DoubleWritable(3.0)), Arrays.asList(Of Writable)(New Text("second"), New DoubleWritable(5.0))}
			Dim inData As IList(Of IList(Of Writable)) = [in]
			Dim s As Schema = (New Schema.Builder()).addColumnString("textCol").addColumnDouble("doubleCol").build()
			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).reduce((New Reducer.Builder(ReduceOp.TakeFirst)).takeFirstColumns("textCol").meanColumns("doubleCol").build()).build()
			Dim outRdd As IList(Of IList(Of Writable)) = LocalTransformExecutor.execute(inData, tp)
			Dim [out] As IList(Of IList(Of Writable)) = outRdd
			Dim expOut As IList(Of IList(Of Writable)) = Collections.singletonList(java.util.Arrays.asList(New Text("first"), New DoubleWritable(4.0)))
			assertEquals(expOut, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reduction By Key") void testReductionByKey()
		Friend Overridable Sub testReductionByKey()
			Dim [in] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New IntWritable(0), New Text("first"), New DoubleWritable(3.0)), Arrays.asList(Of Writable)(New IntWritable(0), New Text("second"), New DoubleWritable(5.0)), Arrays.asList(Of Writable)(New IntWritable(1), New Text("f"), New DoubleWritable(30.0)), Arrays.asList(Of Writable)(New IntWritable(1), New Text("s"), New DoubleWritable(50.0))}
			Dim inData As IList(Of IList(Of Writable)) = [in]
			Dim s As Schema = (New Schema.Builder()).addColumnInteger("intCol").addColumnString("textCol").addColumnDouble("doubleCol").build()
			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).reduce((New Reducer.Builder(ReduceOp.TakeFirst)).keyColumns("intCol").takeFirstColumns("textCol").meanColumns("doubleCol").build()).build()
			Dim outRdd As IList(Of IList(Of Writable)) = LocalTransformExecutor.execute(inData, tp)
			Dim [out] As IList(Of IList(Of Writable)) = outRdd
			Dim expOut As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable)) From {Arrays.asList(New IntWritable(0), New Text("first"), New DoubleWritable(4.0)), Arrays.asList(Of Writable)(New IntWritable(1), New Text("f"), New DoubleWritable(40.0))}
			[out] = New List(Of IList(Of Writable))([out])
			[out].Sort(System.Collections.IComparer.comparingInt(Function(o) o.get(0).toInt()))
			assertEquals(expOut, [out])
		End Sub

	End Class

End Namespace