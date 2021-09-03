Imports System.Collections.Generic
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports OpValidationSuite = org.nd4j.OpValidationSuite
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.SAMEDIFF) public class FailingSameDiffTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class FailingSameDiffTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEyeShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEyeShape(ByVal backend As Nd4jBackend)
			Dim dco As val = DynamicCustomOp.builder("eye").addIntegerArguments(3,3).build()

			Dim list As val = Nd4j.Executioner.calculateOutputShape(dco)
			assertEquals(1, list.size()) 'Fails here - empty list
			assertArrayEquals(New Long(){3, 3}, list.get(0).getShape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExecutionDifferentShapesTransform(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExecutionDifferentShapesTransform(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.linspace(1,12,12, DataType.DOUBLE).reshape(ChrW(3), 4))

			Dim tanh As SDVariable = sd.math().tanh([in])
			Dim exp As INDArray = Transforms.tanh([in].Arr, True)

			Dim [out] As INDArray = tanh.eval()
			assertEquals(exp, [out])

			'Now, replace with minibatch 5:
			[in].Array = Nd4j.linspace(1,20,20, DataType.DOUBLE).reshape(ChrW(5), 4)
			Dim out2 As INDArray = tanh.eval()
			assertArrayEquals(New Long(){5, 4}, out2.shape())

			exp = Transforms.tanh([in].Arr, True)
			assertEquals(exp, out2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDropout(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDropout(ByVal backend As Nd4jBackend)
			Dim sd As SameDiff = SameDiff.create()
			Dim p As Double = 0.5
			Dim ia As INDArray = Nd4j.create(New Long(){2, 2})

			Dim input As SDVariable = sd.var("input", ia)

			Dim res As SDVariable = sd.nn().dropout(input, p)
			Dim output As IDictionary(Of String, INDArray) = sd.outputAll(Collections.emptyMap())
			assertTrue(output.Count > 0)

		   ' assertArrayEquals(new long[]{2, 2}, res.eval().shape());
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testExecutionDifferentShapesDynamicCustom(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testExecutionDifferentShapesDynamicCustom(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim [in] As SDVariable = sd.var("in", Nd4j.linspace(1,12,12, DataType.DOUBLE).reshape(ChrW(3), 4))
			Dim w As SDVariable = sd.var("w", Nd4j.linspace(1,20,20, DataType.DOUBLE).reshape(ChrW(4), 5))
			Dim b As SDVariable = sd.var("b", Nd4j.linspace(1,5,5, DataType.DOUBLE).reshape(ChrW(1), 5))

			Dim mmul As SDVariable = sd.mmul([in],w).add(b)
			Dim exp As INDArray = [in].Arr.mmul(w.Arr).addiRowVector(b.Arr)

			Dim [out] As INDArray = mmul.eval()
			assertEquals(exp, [out])

			'Now, replace with minibatch 5:
			[in].Array = Nd4j.linspace(1,20,20, DataType.DOUBLE).reshape(ChrW(5), 4)
			Dim out2 As INDArray = mmul.eval()
			assertArrayEquals(New Long(){5, 5}, out2.shape())

			exp = [in].Arr.mmul(w.Arr).addiRowVector(b.Arr)
			assertEquals(exp, out2)

			'Generate gradient function, and exec
			Dim loss As SDVariable = mmul.std(True)
			sd.calculateGradients(Collections.emptyMap(), sd.getVariables().keySet())

			[in].Array = Nd4j.linspace(1,12,12, DataType.DOUBLE).reshape(ChrW(3), 4)
			out2 = mmul.eval()
			assertArrayEquals(New Long(){3, 5}, out2.shape())
		End Sub

	End Class

End Namespace