Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.primitives
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

Namespace org.nd4j.linalg.shape


	''' <summary>
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class StaticShapeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class StaticShapeTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testShapeInd2Sub(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testShapeInd2Sub(ByVal backend As Nd4jBackend)
			Dim normalTotal As Long = 0
			Dim n As Long = 1000
			For i As Integer = 0 To n - 1
				Dim start As Long = System.nanoTime()
				Shape.ind2subC(New Integer() {2, 2}, 1)
				Dim [end] As Long = System.nanoTime()
				normalTotal += Math.Abs([end] - start)
			Next i

			normalTotal \= n
			Console.WriteLine(normalTotal)

			Console.WriteLine("C " & Arrays.toString(Shape.ind2subC(New Integer() {2, 2}, 1)))
			Console.WriteLine("F " & Arrays.toString(Shape.ind2sub(New Integer() {2, 2}, 1)))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBufferToIntShapeStrideMethods(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBufferToIntShapeStrideMethods(ByVal backend As Nd4jBackend)
			'Specifically: Shape.shape(IntBuffer), Shape.shape(DataBuffer)
			'.isRowVectorShape(DataBuffer), .isRowVectorShape(IntBuffer)
			'Shape.size(DataBuffer,int), Shape.size(IntBuffer,int)
			'Also: Shape.stride(IntBuffer), Shape.stride(DataBuffer)
			'Shape.stride(DataBuffer,int), Shape.stride(IntBuffer,int)

			Dim lists As IList(Of IList(Of Pair(Of INDArray, String))) = New List(Of IList(Of Pair(Of INDArray, String)))()
			lists.Add(NDArrayCreationUtil.getAllTestMatricesWithShape(3, 4, 12345, DataType.DOUBLE))
			lists.Add(NDArrayCreationUtil.getAllTestMatricesWithShape(1, 4, 12345, DataType.DOUBLE))
			lists.Add(NDArrayCreationUtil.getAllTestMatricesWithShape(3, 1, 12345, DataType.DOUBLE))
			lists.Add(NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Long(){3, 4, 5}, DataType.DOUBLE))
			lists.Add(NDArrayCreationUtil.getAll4dTestArraysWithShape(12345, New Integer(){3, 4, 5, 6}, DataType.DOUBLE))
			lists.Add(NDArrayCreationUtil.getAll4dTestArraysWithShape(12345, New Integer(){3, 1, 5, 1}, DataType.DOUBLE))
			lists.Add(NDArrayCreationUtil.getAll5dTestArraysWithShape(12345, New Integer(){3, 4, 5, 6, 7}, DataType.DOUBLE))
			lists.Add(NDArrayCreationUtil.getAll6dTestArraysWithShape(12345, New Integer(){3, 4, 5, 6, 7, 8}, DataType.DOUBLE))

			Dim shapes As val = New Long()() {
				New Long() {3, 4},
				New Long() {1, 4},
				New Long() {3, 1},
				New Long() {3, 4, 5},
				New Long() {3, 4, 5, 6},
				New Long() {3, 1, 5, 1},
				New Long() {3, 4, 5, 6, 7},
				New Long() {3, 4, 5, 6, 7, 8}
			}

			For i As Integer = 0 To shapes.length - 1
				Dim list As IList(Of Pair(Of INDArray, String)) = lists(i)
				Dim shape As val = shapes(i)

				For Each p As Pair(Of INDArray, String) In list
					Dim arr As INDArray = p.First

					assertArrayEquals(shape, arr.shape())

					Dim thisStride As val = arr.stride()

					Dim ib As val = arr.shapeInfo()
					Dim db As DataBuffer = arr.shapeInfoDataBuffer()

					'Check shape calculation
					assertEquals(shape.length, Shape.rank(ib))
					assertEquals(shape.length, Shape.rank(db))

					assertArrayEquals(shape, Shape.shape(ib))
					assertArrayEquals(shape, Shape.shape(db))

					For j As Integer = 0 To shape.length - 1
						assertEquals(shape(j), Shape.size(ib, j))
						assertEquals(shape(j), Shape.size(db, j))

						assertEquals(thisStride(j), Shape.stride(ib, j))
						assertEquals(thisStride(j), Shape.stride(db, j))
					Next j

					'Check base offset
					assertEquals(Shape.offset(ib), Shape.offset(db))

					'Check offset calculation:
					Dim iter As New NdIndexIterator(shape)
					Do While iter.MoveNext()
						Dim [next] As val = iter.Current
						Dim offset1 As Long = Shape.getOffset(ib, [next])

						assertEquals(offset1, Shape.getOffset(db, [next]))

						Select Case shape.length
							Case 2
								assertEquals(offset1, Shape.getOffset(ib, [next](0), [next](1)))
								assertEquals(offset1, Shape.getOffset(db, [next](0), [next](1)))
							Case 3
								assertEquals(offset1, Shape.getOffset(ib, [next](0), [next](1), [next](2)))
								assertEquals(offset1, Shape.getOffset(db, [next](0), [next](1), [next](2)))
							Case 4
								assertEquals(offset1, Shape.getOffset(ib, [next](0), [next](1), [next](2), [next](3)))
								assertEquals(offset1, Shape.getOffset(db, [next](0), [next](1), [next](2), [next](3)))
							Case 5, 6
								'No 5 and 6d getOffset overloads
							Case Else
								Throw New Exception()
						End Select
					Loop
				Next p
			Next i
		End Sub


		Public Overrides Function ordering() As Char
			Return "f"c
		End Function
	End Class

End Namespace