Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
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

Namespace org.nd4j.linalg.util

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_INDEXING) @NativeTag public class ShapeTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ShapeTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToOffsetZero(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToOffsetZero(ByVal backend As Nd4jBackend)
			Dim matrix As INDArray = Nd4j.rand(3, 5)
			Dim rowOne As INDArray = matrix.getRow(1)
			Dim row1Copy As INDArray = Shape.toOffsetZero(rowOne)
			assertEquals(rowOne, row1Copy)
			Dim rows As INDArray = matrix.getRows(1, 2)
			Dim rowsOffsetZero As INDArray = Shape.toOffsetZero(rows)
			assertEquals(rows, rowsOffsetZero)

			Dim tensor As INDArray = Nd4j.rand(New Integer() {3, 3, 3})
			Dim getTensor As INDArray = tensor.slice(1).slice(1)
			Dim getTensorZero As INDArray = Shape.toOffsetZero(getTensor)
			assertEquals(getTensor, getTensorZero)



		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDupLeadingTrailingZeros(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDupLeadingTrailingZeros(ByVal backend As Nd4jBackend)
			testDupHelper(1, 10)
			testDupHelper(10, 1)
			testDupHelper(1, 10, 1)
			testDupHelper(1, 10, 1, 1)
			testDupHelper(1, 10, 2)
			testDupHelper(2, 10, 1, 1)
			testDupHelper(1, 1, 1, 10)
			testDupHelper(10, 1, 1, 1)
			testDupHelper(1, 1)

		End Sub

		Private Sub testDupHelper(ParamArray ByVal shape() As Integer)
			Dim arr As INDArray = Nd4j.ones(shape)
			Dim arr2 As INDArray = arr.dup()
			assertArrayEquals(arr.shape(), arr2.shape())
			assertTrue(arr.Equals(arr2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testLeadingOnes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testLeadingOnes(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(1, 5, 5)
			assertEquals(1, arr.LeadingOnes)
			Dim arr2 As INDArray = Nd4j.create(2, 2)
			assertEquals(0, arr2.LeadingOnes)
			Dim arr4 As INDArray = Nd4j.create(1, 1, 5, 5)
			assertEquals(2, arr4.LeadingOnes)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTrailingOnes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTrailingOnes(ByVal backend As Nd4jBackend)
			Dim arr2 As INDArray = Nd4j.create(5, 5, 1)
			assertEquals(1, arr2.TrailingOnes)
			Dim arr4 As INDArray = Nd4j.create(5, 5, 1, 1)
			assertEquals(2, arr4.TrailingOnes)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testElementWiseCompareOnesInMiddle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testElementWiseCompareOnesInMiddle(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 6, 6).reshape(ChrW(2), 3)
			Dim onesInMiddle As INDArray = Nd4j.linspace(1, 6, 6).reshape(ChrW(2), 1, 3)
			For i As Integer = 0 To arr.length() - 1
				Dim val As Double = arr.getDouble(i)
				Dim middleVal As Double = onesInMiddle.getDouble(i)
				assertEquals(val, middleVal, 1e-1)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSumLeadingTrailingZeros(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSumLeadingTrailingZeros(ByVal backend As Nd4jBackend)
			testSumHelper(1, 5, 5)
			testSumHelper(5, 5, 1)
			testSumHelper(1, 5, 1)

			testSumHelper(1, 5, 5, 5)
			testSumHelper(5, 5, 5, 1)
			testSumHelper(1, 5, 5, 1)

			testSumHelper(1, 5, 5, 5, 5)
			testSumHelper(5, 5, 5, 5, 1)
			testSumHelper(1, 5, 5, 5, 1)

			testSumHelper(1, 5, 5, 5, 5, 5)
			testSumHelper(5, 5, 5, 5, 5, 1)
			testSumHelper(1, 5, 5, 5, 5, 1)
		End Sub

		Private Sub testSumHelper(ParamArray ByVal shape() As Integer)
			Dim array As INDArray = Nd4j.ones(shape)
			For i As Integer = 0 To shape.Length - 1
				Dim j As Integer = 0
				Do While j < array.vectorsAlongDimension(i)
					Dim vec As INDArray = array.vectorAlongDimension(j, i)
					j += 1
				Loop
				array.sum(i)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEqualsWithSqueeze()
		Public Overridable Sub testEqualsWithSqueeze()

			assertTrue(Shape.shapeEqualWithSqueeze(Nothing, Nothing))
			assertTrue(Shape.shapeEqualWithSqueeze(New Long(){}, New Long(){}))
			assertTrue(Shape.shapeEqualWithSqueeze(New Long(){}, New Long(){1}))
			assertFalse(Shape.shapeEqualWithSqueeze(New Long(){}, New Long(){1, 2}))
			assertFalse(Shape.shapeEqualWithSqueeze(New Long(){}, New Long(){2, 1}))
			assertTrue(Shape.shapeEqualWithSqueeze(New Long(){1}, New Long(){}))
			assertTrue(Shape.shapeEqualWithSqueeze(New Long(){}, New Long(){1, 1, 1, 1, 1}))
			assertTrue(Shape.shapeEqualWithSqueeze(New Long(){1, 1, 1, 1, 1}, New Long(){}))
			assertTrue(Shape.shapeEqualWithSqueeze(New Long(){1}, New Long(){1, 1, 1}))

			assertTrue(Shape.shapeEqualWithSqueeze(New Long(){2, 3}, New Long(){2, 3}))
			assertFalse(Shape.shapeEqualWithSqueeze(New Long(){2, 3}, New Long(){3, 2}))
			assertTrue(Shape.shapeEqualWithSqueeze(New Long(){1, 2, 2}, New Long(){2, 2}))
			assertTrue(Shape.shapeEqualWithSqueeze(New Long(){1, 2, 3}, New Long(){2, 1, 1, 3}))
			assertFalse(Shape.shapeEqualWithSqueeze(New Long(){1, 2, 3}, New Long(){2, 1, 1, 4}))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapeOrder()
		Public Overridable Sub testShapeOrder()
			Dim shape() As Long = {2, 2}
			Dim stride() As Long = {1, 8} 'Ascending strides -> F order

			Dim order As Char = Shape.getOrder(shape, stride, 1)
			assertEquals("f"c, order)
		End Sub

		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace