Imports System.Collections.Generic
Imports Distance = org.datavec.api.transform.Distance
Imports MathFunction = org.datavec.api.transform.MathFunction
Imports MathOp = org.datavec.api.transform.MathOp
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports Schema = org.datavec.api.transform.schema.Schema
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.datavec.api.transform.transform.ndarray


	Public Class TestNDArrayWritableTransforms
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNDArrayWritableBasic()
		Public Overridable Sub testNDArrayWritableBasic()

			Dim s As Schema = (New Schema.Builder()).addColumnDouble("col0").addColumnNDArray("col1", New Long() {1, 10}).addColumnString("col2").build()


			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).ndArrayScalarOpTransform("col1", MathOp.Add, 100).build()

			Dim [in] As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim [out] As IList(Of Writable) = tp.execute([in])

			Dim exp As IList(Of Writable) = New List(Of Writable) From {Of Writable}

			assertEquals(exp, [out])

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNDArrayColumnsMathOpTransform()
		Public Overridable Sub testNDArrayColumnsMathOpTransform()

			Dim s As Schema = (New Schema.Builder()).addColumnDouble("col0").addColumnNDArray("col1", New Long() {1, 10}).addColumnNDArray("col2", New Long() {1, 10}).build()


			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).ndArrayColumnsMathOpTransform("myCol", MathOp.Add, "col1", "col2").build()

			Dim expColNames As IList(Of String) = New List(Of String) From {"col0", "col1", "col2", "myCol"}
			assertEquals(expColNames, tp.FinalSchema.getColumnNames())


			Dim [in] As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim [out] As IList(Of Writable) = tp.execute([in])

			Dim exp As IList(Of Writable) = New List(Of Writable) From {Of Writable}

			assertEquals(exp, [out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNDArrayMathFunctionTransform()
		Public Overridable Sub testNDArrayMathFunctionTransform()

			Dim s As Schema = (New Schema.Builder()).addColumnDouble("col0").addColumnNDArray("col1", New Long() {1, 10}).addColumnNDArray("col2", New Long() {1, 10}).build()


			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).ndArrayMathFunctionTransform("col1", MathFunction.SIN).ndArrayMathFunctionTransform("col2", MathFunction.SQRT).build()



			Dim expColNames As IList(Of String) = New List(Of String) From {"col0", "col1", "col2"}
			assertEquals(expColNames, tp.FinalSchema.getColumnNames())


			Dim [in] As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim [out] As IList(Of Writable) = tp.execute([in])

			Dim exp As IList(Of Writable) = New List(Of Writable) From {Of Writable}

			assertEquals(exp, [out])
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testNDArrayDistanceTransform()
		Public Overridable Sub testNDArrayDistanceTransform()

			Dim s As Schema = (New Schema.Builder()).addColumnDouble("col0").addColumnNDArray("col1", New Long() {1, 10}).addColumnNDArray("col2", New Long() {1, 10}).build()


			Dim tp As TransformProcess = (New TransformProcess.Builder(s)).ndArrayDistanceTransform("dist", Distance.COSINE, "col1", "col2").build()



			Dim expColNames As IList(Of String) = New List(Of String) From {"col0", "col1", "col2", "dist"}
			assertEquals(expColNames, tp.FinalSchema.getColumnNames())

			Nd4j.Random.setSeed(12345)
			Dim arr1 As INDArray = Nd4j.rand(1, 10)
			Dim arr2 As INDArray = Nd4j.rand(1, 10)
			Dim cosine As Double = Transforms.cosineSim(arr1, arr2)

			Dim [in] As IList(Of Writable) = New List(Of Writable) From {Of Writable}
			Dim [out] As IList(Of Writable) = tp.execute([in])

			Dim exp As IList(Of Writable) = New List(Of Writable) From {Of Writable}

			assertEquals(exp, [out])
		End Sub

	End Class

End Namespace