Imports System.Collections.Generic
Imports MathFunction = org.datavec.api.transform.MathFunction
Imports MathOp = org.datavec.api.transform.MathOp
Imports Transform = org.datavec.api.transform.Transform
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports NDArrayColumnsMathOpTransform = org.datavec.api.transform.ndarray.NDArrayColumnsMathOpTransform
Imports NDArrayMathFunctionTransform = org.datavec.api.transform.ndarray.NDArrayMathFunctionTransform
Imports NDArrayScalarOpTransform = org.datavec.api.transform.ndarray.NDArrayScalarOpTransform
Imports Schema = org.datavec.api.transform.schema.Schema
Imports JsonSerializer = org.datavec.api.transform.serde.JsonSerializer
Imports YamlSerializer = org.datavec.api.transform.serde.YamlSerializer
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
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


	Public Class TestYamlJsonSerde
		Inherits BaseND4JTest

		Public Shared y As New YamlSerializer()
		Public Shared j As New JsonSerializer()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransforms()
		Public Overridable Sub testTransforms()


			Dim transforms() As Transform = {
				New NDArrayColumnsMathOpTransform("newCol", MathOp.Divide, "in1", "in2"),
				New NDArrayMathFunctionTransform("inCol", MathFunction.SQRT),
				New NDArrayScalarOpTransform("inCol", MathOp.ScalarMax, 3.0)
			}

			For Each t As Transform In transforms
				Dim yaml As String = y.serialize(t)
				Dim json As String = j.serialize(t)

				'            System.out.println(yaml);
				'            System.out.println(json);
				'            System.out.println();

				Dim t2 As Transform = y.deserializeTransform(yaml)
				Dim t3 As Transform = j.deserializeTransform(json)
				assertEquals(t, t2)
				assertEquals(t, t3)
			Next t


			Dim tArrAsYaml As String = y.serialize(transforms)
			Dim tArrAsJson As String = j.serialize(transforms)
			Dim tListAsYaml As String = y.serializeTransformList(New List(Of Transform) From {transforms})
			Dim tListAsJson As String = j.serializeTransformList(New List(Of Transform) From {transforms})

			'        System.out.println("\n\n\n\n");
			'        System.out.println(tListAsYaml);

			Dim lFromYaml As IList(Of Transform) = y.deserializeTransformList(tListAsYaml)
			Dim lFromJson As IList(Of Transform) = j.deserializeTransformList(tListAsJson)

			assertEquals(Arrays.asList(transforms), y.deserializeTransformList(tArrAsYaml))
			assertEquals(Arrays.asList(transforms), j.deserializeTransformList(tArrAsJson))
			assertEquals(Arrays.asList(transforms), lFromYaml)
			assertEquals(Arrays.asList(transforms), lFromJson)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testTransformProcessAndSchema()
		Public Overridable Sub testTransformProcessAndSchema()

			Dim schema As Schema = (New Schema.Builder()).addColumnInteger("firstCol").addColumnNDArray("nd1a", New Long() {1, 10}).addColumnNDArray("nd1b", New Long() {1, 10}).addColumnNDArray("nd2", New Long() {1, 100}).addColumnNDArray("nd3", New Long() {-1, -1}).build()

			Dim tp As TransformProcess = (New TransformProcess.Builder(schema)).integerMathOp("firstCol", MathOp.Add, 1).ndArrayColumnsMathOpTransform("added", MathOp.Add, "nd1a", "nd1b").ndArrayMathFunctionTransform("nd2", MathFunction.SQRT).ndArrayScalarOpTransform("nd3", MathOp.Multiply, 2.0).build()

			Dim asJson As String = tp.toJson()
			Dim asYaml As String = tp.toYaml()

			Dim fromJson As TransformProcess = TransformProcess.fromJson(asJson)
			Dim fromYaml As TransformProcess = TransformProcess.fromYaml(asYaml)

			assertEquals(tp, fromJson)
			assertEquals(tp, fromYaml)
		End Sub

	End Class

End Namespace