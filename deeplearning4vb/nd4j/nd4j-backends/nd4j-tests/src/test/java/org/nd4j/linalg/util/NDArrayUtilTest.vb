Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.nd4j.linalg.util

	''' <summary>
	''' @author Hamdi Douss
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class NDArrayUtilTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NDArrayUtilTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMatrixConversion(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMatrixConversion(ByVal backend As Nd4jBackend)
			Dim nums()() As Integer = {
				New Integer() {1, 2},
				New Integer() {3, 4},
				New Integer() {5, 6}
			}
			Dim result As INDArray = NDArrayUtil.toNDArray(nums)
			assertArrayEquals(New Long(){2, 3}, result.shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testVectorConversion(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testVectorConversion(ByVal backend As Nd4jBackend)
			Dim nums() As Integer = {1, 2, 3, 4}
			Dim result As INDArray = NDArrayUtil.toNDArray(nums)
			assertArrayEquals(New Long(){1, 4}, result.shape())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlattenArray1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlattenArray1(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim arrX[][][] As Single = new Single[2][2][2]
			Dim arrX()()() As Single = RectangularArrays.RectangularSingleArray(2, 2, 2)

			Dim arrZ() As Single = ArrayUtil.flatten(arrX)

			assertEquals(8, arrZ.Length)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlattenArray2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlattenArray2(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim arrX[][][] As Single = new Single[5][4][3]
			Dim arrX()()() As Single = RectangularArrays.RectangularSingleArray(5, 4, 3)

			Dim arrZ() As Single = ArrayUtil.flatten(arrX)

			assertEquals(60, arrZ.Length)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlattenArray3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlattenArray3(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim arrX[][][] As Single = new Single[5][2][3]
			Dim arrX()()() As Single = RectangularArrays.RectangularSingleArray(5, 2, 3)

			Dim arrZ() As Single = ArrayUtil.flatten(arrX)

			assertEquals(30, arrZ.Length)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testFlattenArray4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testFlattenArray4(ByVal backend As Nd4jBackend)
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim arrX[][][][] As Single = new Single[5][2][3][3]
			Dim arrX()()()() As Single = RectangularArrays.RectangularSingleArray(5, 2, 3, 3)

			Dim arrZ() As Single = ArrayUtil.flatten(arrX)

			assertEquals(90, arrZ.Length)
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace