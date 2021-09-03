Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg.api.blas

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class Level1Test extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class Level1Test
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDot(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDot(ByVal backend As Nd4jBackend)
			Dim vec1 As INDArray = Nd4j.create(New Single() {1, 2, 3, 4})
			Dim vec2 As INDArray = Nd4j.create(New Single() {1, 2, 3, 4})
			assertEquals(30, Nd4j.BlasWrapper.dot(vec1, vec2), 1e-1)

			Dim matrix As INDArray = Nd4j.linspace(1, 4, 4, DataType.FLOAT).reshape(ChrW(2), 2)
			Dim row As INDArray = matrix.getRow(1)
			Dim dot As Double = Nd4j.BlasWrapper.dot(row, row)
			assertEquals(20, dot, 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAxpy(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAxpy(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			Dim row As INDArray = matrix.getRow(1)
			Nd4j.BlasWrapper.level1().axpy(row.length(), 1.0, row, row)
			assertEquals(Nd4j.create(New Double() {4, 8}), row,getFailureMessage(backend))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAxpy2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAxpy2(ByVal backend As Nd4jBackend)
			Dim rowX As val = Nd4j.create(New Double(){1, 2, 3, 4})
			Dim rowY As val = Nd4j.create(New Double(){1, 2, 3, 4})
			Dim exp As val = Nd4j.create(New Double(){3, 6, 9, 12})

			Dim z As val = Nd4j.BlasWrapper.axpy(2.0, rowX, rowY)
			assertEquals(exp, z)
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace