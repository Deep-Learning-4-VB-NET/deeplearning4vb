Imports System
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend

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

Namespace org.nd4j.imports

	Public Class TestReverse
		Inherits BaseNd4jTestWithBackends


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse(ByVal backend As Nd4jBackend)

			Dim [in] As INDArray = Nd4j.createFromArray(New Double(){1, 2, 3, 4, 5, 6})
			Dim [out] As INDArray = Nd4j.create(DataType.DOUBLE, 6)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("reverse").addInputs([in]).addOutputs([out]).addIntegerArguments(0).build()

			Nd4j.Executioner.exec(op)

			Console.WriteLine([out])
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReverse2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReverse2(ByVal backend As Nd4jBackend)

			Dim [in] As INDArray = Nd4j.createFromArray(New Double(){1, 2, 3, 4, 5, 6})
			Dim axis As INDArray = Nd4j.scalar(0)
			Dim [out] As INDArray = Nd4j.create(DataType.DOUBLE, 6)

			Dim op As DynamicCustomOp = DynamicCustomOp.builder("reverse").addInputs([in], axis).addOutputs([out]).build()

			Nd4j.Executioner.exec(op)

			Console.WriteLine([out])
		End Sub
	End Class

End Namespace