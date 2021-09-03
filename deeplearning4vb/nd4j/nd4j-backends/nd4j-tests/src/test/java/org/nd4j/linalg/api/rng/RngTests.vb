Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg.api.rng

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.RNG) public class RngTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class RngTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRngConstitency(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRngConstitency(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(123)
			Dim arr As INDArray = Nd4j.rand(1, 5)
			Nd4j.Random.setSeed(123)
			Dim arr2 As INDArray = Nd4j.rand(1, 5)
			assertEquals(arr, arr2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomWithOrder(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandomWithOrder(ByVal backend As Nd4jBackend)

			Nd4j.Random.setSeed(12345)

			Dim rows As Integer = 20
			Dim cols As Integer = 20
			Dim dim2 As Integer = 70

			Dim arr As INDArray = Nd4j.rand("c"c, rows, cols)
			assertArrayEquals(New Long() {rows, cols}, arr.shape())
			assertEquals("c"c, arr.ordering())
			assertTrue(arr.minNumber().doubleValue() >= 0.0)
			assertTrue(arr.maxNumber().doubleValue() <= 1.0)

			Dim arr2 As INDArray = Nd4j.rand("f"c, rows, cols)
			assertArrayEquals(New Long() {rows, cols}, arr2.shape())
			assertEquals("f"c, arr2.ordering())
			assertTrue(arr2.minNumber().doubleValue() >= 0.0)
			assertTrue(arr2.maxNumber().doubleValue() <= 1.0)

			Dim arr3 As INDArray = Nd4j.rand("c"c, New Integer() {rows, cols, dim2})
			assertArrayEquals(New Long() {rows, cols, dim2}, arr3.shape())
			assertEquals("c"c, arr3.ordering())
			assertTrue(arr3.minNumber().doubleValue() >= 0.0)
			assertTrue(arr3.maxNumber().doubleValue() <= 1.0)

			Dim arr4 As INDArray = Nd4j.rand("f"c, New Integer() {rows, cols, dim2})
			assertArrayEquals(New Long() {rows, cols, dim2}, arr4.shape())
			assertEquals("f"c, arr4.ordering())
			assertTrue(arr4.minNumber().doubleValue() >= 0.0)
			assertTrue(arr4.maxNumber().doubleValue() <= 1.0)


			Dim narr As INDArray = Nd4j.randn("c"c, rows, cols)
			assertArrayEquals(New Long() {rows, cols}, narr.shape())
			assertEquals("c"c, narr.ordering())
			assertEquals(0.0, narr.meanNumber().doubleValue(), 0.05)

			Dim narr2 As INDArray = Nd4j.randn("f"c, rows, cols)
			assertArrayEquals(New Long() {rows, cols}, narr2.shape())
			assertEquals("f"c, narr2.ordering())
			assertEquals(0.0, narr2.meanNumber().doubleValue(), 0.05)

			Dim narr3 As INDArray = Nd4j.randn("c"c, New Integer() {rows, cols, dim2})
			assertArrayEquals(New Long() {rows, cols, dim2}, narr3.shape())
			assertEquals("c"c, narr3.ordering())
			assertEquals(0.0, narr3.meanNumber().doubleValue(), 0.05)

			Dim narr4 As INDArray = Nd4j.randn("f"c, New Integer() {rows, cols, dim2})
			assertArrayEquals(New Long() {rows, cols, dim2}, narr4.shape())
			assertEquals("f"c, narr4.ordering())
			assertEquals(0.0, narr4.meanNumber().doubleValue(), 0.05)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRandomBinomial(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRandomBinomial(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(12345)
			'silly tests. Just increasing the usage for randomBinomial to stop compiler warnings.
			Dim x As INDArray = Nd4j.randomBinomial(10, 0.5, 3,3)
			assertTrue(x.sum().getDouble(0) > 0.0) 'silly test. Just increasing th usage for randomBinomial

			x = Nd4j.randomBinomial(10, 0.5, x)
			assertTrue(x.sum().getDouble(0) > 0.0)

			x = Nd4j.randomExponential(0.5, 3,3)
			assertTrue(x.sum().getDouble(0) > 0.0)

			x = Nd4j.randomExponential(0.5, x)
			assertTrue(x.sum().getDouble(0) > 0.0)
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function

	End Class

End Namespace