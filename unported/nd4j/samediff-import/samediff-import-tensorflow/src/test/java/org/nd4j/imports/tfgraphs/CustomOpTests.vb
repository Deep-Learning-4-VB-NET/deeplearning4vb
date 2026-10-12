Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertArrayEquals
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

Namespace org.nd4j.imports.tfgraphs

	Public Class CustomOpTests
		Inherits BaseNd4jTestWithBackends


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPad(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPad(ByVal backend As Nd4jBackend)

			Dim [in] As INDArray = Nd4j.create(DataType.FLOAT, 1, 28, 28, 264)
			Dim pad As INDArray = Nd4j.createFromArray(New Integer()(){
				New Integer() {0, 0},
				New Integer() {0, 1},
				New Integer() {0, 1},
				New Integer() {0, 0}
			})
			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, 1, 29, 29, 264)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("pad").addInputs([in], pad).addOutputs([out]).addIntegerArguments(0).build()

			Dim outShape As val = Nd4j.Executioner.calculateOutputShape(op)
			assertEquals(1, outShape.size())
			assertArrayEquals(New Long(){1, 29, 29, 264}, outShape.get(0).getShape())

			Nd4j.Executioner.exec(op) 'Crash here
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testResizeBilinearEdgeCase(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testResizeBilinearEdgeCase(ByVal backend As Nd4jBackend)
			Dim [in] As INDArray = Nd4j.ones(DataType.FLOAT, 1, 1, 1, 3)
			Dim size As INDArray = Nd4j.createFromArray(8, 8)
			Dim [out] As INDArray = Nd4j.create(DataType.FLOAT, 1, 8, 8, 3)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("resize_bilinear").addInputs([in], size).addOutputs([out]).addIntegerArguments(1).build()

			Nd4j.Executioner.exec(op)

			Dim exp As INDArray = Nd4j.ones(DataType.FLOAT, 1, 8, 8, 3)
			assertEquals(exp, [out])
		End Sub
	End Class

End Namespace