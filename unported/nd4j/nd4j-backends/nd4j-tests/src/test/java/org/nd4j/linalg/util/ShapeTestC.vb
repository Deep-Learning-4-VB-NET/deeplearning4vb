Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Tile = org.nd4j.linalg.api.ops.impl.shape.Tile
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
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
'ORIGINAL LINE: @Slf4j @Tag(TagNames.NDARRAY_INDEXING) @NativeTag public class ShapeTestC extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ShapeTestC
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
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTile(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTile(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.scalar(DataType.DOUBLE, 1.0).reshape(ChrW(1), 1)
			'INDArray[] inputs, INDArray[] outputs, int[] axis
			Dim result As INDArray = Nd4j.createUninitialized(DataType.DOUBLE, 2,2)
			Dim tile As New Tile(New INDArray(){arr},New INDArray(){result},New Integer() {2, 2})
			Nd4j.Executioner.execAndReturn(tile)
			Dim tiled As INDArray = Nd4j.tile(arr,2,2).castTo(DataType.DOUBLE)
			assertEquals(tiled,result)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testElementWiseCompareOnesInMiddle(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testElementWiseCompareOnesInMiddle(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.linspace(1, 6, 6).reshape(ChrW(2), 3)
			Dim onesInMiddle As INDArray = Nd4j.linspace(1, 6, 6).reshape(ChrW(2), 1, 3)
			For i As Integer = 0 To arr.length() - 1
				assertEquals(arr.getDouble(i), onesInMiddle.getDouble(i), 1e-3)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testKeepDimsShape_1_T(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testKeepDimsShape_1_T(ByVal backend As Nd4jBackend)
			Dim shape As val = New Integer(){5, 5}
			Dim axis As val = New Integer(){1, 0, 1}

			Dim result As val = Shape.getReducedShape(shape, axis, True, True)

			assertArrayEquals(New Long(){1, 1}, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testKeepDimsShape_1_F(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testKeepDimsShape_1_F(ByVal backend As Nd4jBackend)
			Dim shape As val = New Integer(){5, 5}
			Dim axis As val = New Integer(){0, 0, 1}

			Dim result As val = Shape.getReducedShape(shape, axis, False, True)

			assertArrayEquals(New Long(){}, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testKeepDimsShape_2_T(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testKeepDimsShape_2_T(ByVal backend As Nd4jBackend)
			Dim shape As val = New Integer(){5, 5, 5}
			Dim axis As val = New Integer(){1, 0, 1}

			Dim result As val = Shape.getReducedShape(shape, axis, True, True)

			assertArrayEquals(New Long(){1, 1, 5}, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testKeepDimsShape_2_F(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testKeepDimsShape_2_F(ByVal backend As Nd4jBackend)
			Dim shape As val = New Integer(){5, 5, 5}
			Dim axis As val = New Integer(){0, 0, 1}

			Dim result As val = Shape.getReducedShape(shape, axis, False, True)

			assertArrayEquals(New Long(){5}, result)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testKeepDimsShape_3_T(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testKeepDimsShape_3_T(ByVal backend As Nd4jBackend)
			Dim shape As val = New Integer(){1, 1}
			Dim axis As val = New Integer(){1, 0, 1}

			Dim result As val = Shape.getReducedShape(shape, axis, True, True)

			assertArrayEquals(New Long(){1, 1}, result)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testKeepDimsShape_3_F(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testKeepDimsShape_3_F(ByVal backend As Nd4jBackend)
			Dim shape As val = New Integer(){1, 1}
			Dim axis As val = New Integer(){0, 0}

			Dim result As val = Shape.getReducedShape(shape, axis, False, True)

			log.info("Result: {}", result)

			assertArrayEquals(New Long(){1}, result)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testKeepDimsShape_4_F(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testKeepDimsShape_4_F(ByVal backend As Nd4jBackend)
			Dim shape As val = New Integer(){4, 4}
			Dim axis As val = New Integer(){0, 0}

			Dim result As val = Shape.getReducedShape(shape, axis, False, True)

			log.info("Result: {}", result)

			assertArrayEquals(New Long(){4}, result)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAxisNormalization_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAxisNormalization_1(ByVal backend As Nd4jBackend)
			Dim axis As val = New Integer() {1, -2}
			Dim rank As val = 2
			Dim exp As val = New Integer() {0, 1}

			Dim norm As val = Shape.normalizeAxis(rank, axis)
			assertArrayEquals(exp, norm)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAxisNormalization_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAxisNormalization_2(ByVal backend As Nd4jBackend)
			Dim axis As val = New Integer() {1, -2, 0}
			Dim rank As val = 2
			Dim exp As val = New Integer() {0, 1}

			Dim norm As val = Shape.normalizeAxis(rank, axis)
			assertArrayEquals(exp, norm)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAxisNormalization_3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAxisNormalization_3(ByVal backend As Nd4jBackend)
			assertThrows(GetType(ND4JIllegalStateException),Sub()
			Dim axis As val = New Integer() {1, -2, 2}
			Dim rank As val = 2
			Dim exp As val = New Integer() {0, 1}
			Dim norm As val = Shape.normalizeAxis(rank, axis)
			assertArrayEquals(exp, norm)
			End Sub)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAxisNormalization_4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAxisNormalization_4(ByVal backend As Nd4jBackend)
			Dim axis As val = New Integer() {1, 2, 0}
			Dim rank As val = 3
			Dim exp As val = New Integer() {0, 1, 2}

			Dim norm As val = Shape.normalizeAxis(rank, axis)
			assertArrayEquals(exp, norm)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace