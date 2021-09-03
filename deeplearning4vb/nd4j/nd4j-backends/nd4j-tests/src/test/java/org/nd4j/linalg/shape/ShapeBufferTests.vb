Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
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

Namespace org.nd4j.linalg.shape

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class ShapeBufferTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ShapeBufferTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRank(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRank(ByVal backend As Nd4jBackend)
			Dim shape() As Long = {2, 4}
			Dim stride() As Long = {1, 2}
			Dim shapeInfoBuffer As val = Shape.createShapeInformation(shape, stride, 1, "c"c, DataType.DOUBLE, False)
			Dim buff As val = shapeInfoBuffer.asNioLong()
			assertEquals(2, Shape.rank(buff))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testArrCreationShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testArrCreationShape(ByVal backend As Nd4jBackend)
			Dim arr As val = Nd4j.linspace(1, 4, 4, DataType.DOUBLE).reshape(ChrW(2), 2)
			For i As Integer = 0 To 1
				assertEquals(2, arr.size(i))
			Next i
			Dim stride() As Integer = ArrayUtil.calcStrides(New Integer() {2, 2})
			For i As Integer = 0 To stride.Length - 1
				assertEquals(stride(i), arr.stride(i))
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShape(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShape(ByVal backend As Nd4jBackend)
			Dim shape() As Long = {2, 4}
			Dim stride() As Long = {1, 2}
			Dim shapeInfoBuffer As val = Shape.createShapeInformation(shape, stride, 1, "c"c, DataType.DOUBLE, False)
			Dim buff As val = shapeInfoBuffer.asNioLong()
			Dim shapeView As val = Shape.shapeOf(buff)
			assertTrue(Shape.contentEquals(shape, shapeView))
			Dim strideView As val = Shape.stride(buff)
			assertTrue(Shape.contentEquals(stride, strideView))
			assertEquals("c"c, Shape.order(buff))
			assertEquals(1, Shape.elementWiseStride(buff))
			assertFalse(Shape.isVector(buff))
			assertTrue(Shape.contentEquals(shape, Shape.shapeOf(buff)))
			assertTrue(Shape.contentEquals(stride, Shape.stride(buff)))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBuff(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBuff(ByVal backend As Nd4jBackend)
			Dim shape() As Long = {1, 2}
			Dim stride() As Long = {1, 2}
			Dim buff As val = Shape.createShapeInformation(shape, stride, 1, "c"c, DataType.DOUBLE, False).asNioLong()
			assertTrue(Shape.isVector(buff))
		End Sub


	End Class

End Namespace