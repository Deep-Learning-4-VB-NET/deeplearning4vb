Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.primitives
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class TADTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TADTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStall(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStall(ByVal backend As Nd4jBackend)
			'[4, 3, 3, 4, 5, 60, 20, 5, 1, 0, 1, 99], dimensions: [1, 2, 3]
			Dim arr As INDArray = Nd4j.create(3, 3, 4, 5)
			arr.tensorAlongDimension(0, 1, 2, 3)
		End Sub



		''' <summary>
		''' This test checks for TADs equality between Java & native
		''' </summary>
		''' <exception cref="Exception"> </exception>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEquality1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEquality1(ByVal backend As Nd4jBackend)

			Dim order() As Char = {"c"c, "f"c}
			Dim dim_e() As Integer = {0, 2}
			Dim dim_x() As Integer = {1, 3}
			Dim dim_3 As IList(Of Integer()) = New List(Of Integer()) From {
				New Integer() {0, 2, 3},
				New Integer() {0, 1, 2},
				New Integer() {1, 2, 3},
				New Integer() {0, 1, 3}
			}


			For Each o As Char In order
				Dim array As INDArray = Nd4j.create(New Integer() {3, 5, 7, 9}, o)
				For Each e As Integer In dim_e
					For Each x As Integer In dim_x

						Dim shape() As Integer = {e, x}
						Array.Sort(shape)
						Dim assertion As INDArray = array.tensorAlongDimension(0, shape)
						Dim test As INDArray = array.tensorAlongDimension(0, shape)

						assertEquals(assertion, test)
						'assertEquals(assertion.shapeInfoDataBuffer(), test.shapeInfoDataBuffer());
	'                    DataBuffer tadShape_N = Nd4j.getExecutioner().getTADManager().getTADOnlyShapeInfo(array, shape).getFirst();
	'                    DataBuffer tadShape_J = array.tensorAlongDimension(0, shape).shapeInfoDataBuffer();
	'                    log.info("Original order: {}; Dimensions: {}; Original shape: {};", o, Arrays.toString(shape), Arrays.toString(array.shapeInfoDataBuffer().asInt()));
	'                    log.info("Java shape: {}; Native shape: {}", Arrays.toString(tadShape_J.asInt()), Arrays.toString(tadShape_N.asInt()));
	'                    System.out.println();
	'                    assertEquals("TAD asertadShape_J,tadShape_N);
					Next x
				Next e
			Next o

	'        log.info("3D TADs:");
			For Each o As Char In order
				Dim array As INDArray = Nd4j.create(New Integer() {9, 7, 5, 3}, o)
				For Each shape As Integer() In dim_3
					Array.Sort(shape)
	'                log.info("About to do shape: " + Arrays.toString(shape) + " for array of shape "
	'                                + array.shapeInfoToString());
					Dim assertion As INDArray = array.tensorAlongDimension(0, shape)
					Dim test As INDArray = array.tensorAlongDimension(0, shape)
					assertEquals(assertion, test)
					'assertEquals(assertion.shapeInfoDataBuffer(), test.shapeInfoDataBuffer());

	'                
	'                
	'                
	'                log.info("Original order: {}; Dimensions: {}; Original shape: {};", o, Arrays.toString(shape), Arrays.toString(array.shapeInfoDataBuffer().asInt()));
	'                log.info("Java shape: {}; Native shape: {}", Arrays.toString(tadShape_J.asInt()), Arrays.toString(tadShape_N.asInt()));
	'                System.out.println();
	'                assertEquals(true, compareShapes(tadShape_N, tadShape_J));
				Next shape
			Next o
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMysteriousCrash(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testMysteriousCrash(ByVal backend As Nd4jBackend)
			Dim arrayF As INDArray = Nd4j.create(New Integer() {1, 1, 4, 4}, "f"c)
			Dim arrayC As INDArray = Nd4j.create(New Integer() {1, 1, 4, 4}, "c"c)
			Dim javaCTad As INDArray = arrayC.tensorAlongDimension(0, 2, 3)
			Dim javaFTad As INDArray = arrayF.tensorAlongDimension(0, 2, 3)
			Dim tadBuffersF As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(arrayF, 2, 3)
			Dim tadBuffersC As Pair(Of DataBuffer, DataBuffer) = Nd4j.Executioner.TADManager.getTADOnlyShapeInfo(arrayC, 2, 3)

	'        log.info("Got TADShapeF: {}", Arrays.toString(tadBuffersF.getFirst().asInt()) + " with java "
	'                        + javaFTad.shapeInfoDataBuffer());
	'        log.info("Got TADShapeC: {}", Arrays.toString(tadBuffersC.getFirst().asInt()) + " with java "
	'                        + javaCTad.shapeInfoDataBuffer());
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTADEWSStride()
		Public Overridable Sub testTADEWSStride()
			Dim orig As INDArray = Nd4j.linspace(1, 600, 600).reshape("f"c, 10, 1, 60)

			For i As Integer = 0 To 59
				Dim tad As INDArray = orig.tensorAlongDimension(i, 0, 1)
				'TAD: should be equivalent to get(all, all, point(i))
				Dim get As INDArray = orig.get(all(), all(), point(i))

				Dim str As String = i.ToString()
				assertEquals(get, tad,str)
				assertEquals(get.data().offset(), tad.data().offset(),str)
				assertEquals(get.elementWiseStride(), tad.elementWiseStride(),str)

				Dim orderTad As Char = Shape.getOrder(tad.shape(), tad.stride(), 1)
				Dim orderGet As Char = Shape.getOrder(get.shape(), get.stride(), 1)

				assertEquals("f"c, orderTad)
				assertEquals("f"c, orderGet)

				Dim ewsTad As Long = Shape.elementWiseStride(tad.shape(), tad.stride(), tad.ordering() = "f"c)
				Dim ewsGet As Long = Shape.elementWiseStride(get.shape(), get.stride(), get.ordering() = "f"c)

				assertEquals(1, ewsTad)
				assertEquals(1, ewsGet)
			Next i
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		''' <summary>
		''' this method compares rank, shape and stride for two given shapeBuffers </summary>
		''' <param name="shapeA"> </param>
		''' <param name="shapeB">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected boolean compareShapes(@NonNull DataBuffer shapeA, @NonNull DataBuffer shapeB)
		Protected Friend Overridable Function compareShapes(ByVal shapeA As DataBuffer, ByVal shapeB As DataBuffer) As Boolean
			If shapeA.dataType() <> DataType.INT Then
				Throw New System.InvalidOperationException("ShapeBuffer should have dataType of INT")
			End If

			If shapeA.dataType() <> shapeB.dataType() Then
				Return False
			End If

			Dim rank As Integer = shapeA.getInt(0)
			If rank <> shapeB.getInt(0) Then
				Return False
			End If

			Dim e As Integer = 1
			Do While e <= rank * 2
				If shapeA.getInt(e) <> shapeB.getInt(e) Then
					Return False
				End If
				e += 1
			Loop

			Return True
		End Function
	End Class

End Namespace