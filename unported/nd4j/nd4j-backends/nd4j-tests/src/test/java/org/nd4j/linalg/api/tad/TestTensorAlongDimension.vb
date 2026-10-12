Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StopWatch = org.apache.commons.lang3.time.StopWatch
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDArrayCreationUtil = org.nd4j.linalg.checkutil.NDArrayCreationUtil
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.linalg.api.tad


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.NDARRAY_INDEXING) public class TestTensorAlongDimension extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestTensorAlongDimension
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJavaVsNative(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testJavaVsNative(ByVal backend As Nd4jBackend)
			Dim totalJavaTime As Long = 0
			Dim totalCTime As Long = 0
			Dim n As Long = 10
			Dim row As INDArray = Nd4j.create(1, 100)

			For i As Integer = 0 To n - 1
				Dim javaTiming As New StopWatch()
				javaTiming.start()
				row.tensorAlongDimension(0, 0)
				javaTiming.stop()
				Dim cTiming As New StopWatch()
				cTiming.start()
				row.tensorAlongDimension(0, 0)
				cTiming.stop()
				totalJavaTime += javaTiming.getNanoTime()
				totalCTime += cTiming.getNanoTime()
			Next i

			Console.WriteLine("Java timing " & (totalJavaTime \ n) & " C time " & (totalCTime \ n))

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadShapesEdgeCases(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadShapesEdgeCases(ByVal backend As Nd4jBackend)
			Dim row As INDArray = Nd4j.create(DataType.DOUBLE, 1, 5)
			Dim col As INDArray = Nd4j.create(DataType.DOUBLE, 5, 1)

			assertArrayEquals(New Long() {1, 5}, row.tensorAlongDimension(0, 1).shape())
			assertArrayEquals(New Long() {1, 5}, col.tensorAlongDimension(0, 0).shape())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadShapes1d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadShapes1d(ByVal backend As Nd4jBackend)
			'Ensure TAD returns the correct/expected shapes, and values don't depend on underlying array layout/order etc
			''' <summary>
			''' NEED TO WORK ON ELEMENT WISE STRIDE NOW.
			''' </summary>
			'From a 2d array:
			Dim rows As Integer = 3
			Dim cols As Integer = 4
			Dim testValues As INDArray = Nd4j.linspace(1, rows * cols, rows * cols, DataType.DOUBLE).reshape("c"c, rows, cols)
			Dim list As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAllTestMatricesWithShape("c"c, rows, cols, 12345, DataType.DOUBLE)
			For Each p As Pair(Of INDArray, String) In list
				Dim arr As INDArray = p.First.assign(testValues)

				'Along dimension 0: expect row vector with length 'rows'
				assertEquals(cols, arr.tensorsAlongDimension(0))
				For i As Integer = 0 To cols - 1
					Dim tad As INDArray = arr.tensorAlongDimension(i, 0)
					Dim javaTad As INDArray = arr.tensorAlongDimension(i, 0)
					assertEquals(javaTad, tad)
					assertArrayEquals(New Long() {rows}, tad.shape())
					'assertEquals(testValues.javaTensorAlongDimension(i, 0), tad);
				Next i

				'Along dimension 1: expect row vector with length 'cols'
				assertEquals(rows, arr.tensorsAlongDimension(1))
				For i As Integer = 0 To rows - 1
					Dim tad As INDArray = arr.tensorAlongDimension(i, 1)
					assertArrayEquals(New Long() {cols}, tad.shape())
					'assertEquals(testValues.javaTensorAlongDimension(i, 1), tad);
				Next i
			Next p

			'From a 3d array:
			Dim dim2 As Integer = 5
			log.info("AF")
			testValues = Nd4j.linspace(1, rows * cols * dim2, rows * cols * dim2, DataType.DOUBLE).reshape("c"c, rows, cols, dim2)
			list = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){rows, cols, dim2}, DataType.DOUBLE)
			For Each p As Pair(Of INDArray, String) In list
				Dim arr As INDArray = p.First.assign(testValues)
				Dim javaTad As INDArray = arr.tensorAlongDimension(0, 0)
				Dim tadTest As INDArray = arr.tensorAlongDimension(0, 0)
				assertEquals(javaTad, tadTest)
				'Along dimension 0: expect row vector with length 'rows'
				assertEquals(cols * dim2, arr.tensorsAlongDimension(0),"Failed on " & p.getValue())
				Dim i As Integer = 0
				Do While i < cols * dim2
					Dim tad As INDArray = arr.tensorAlongDimension(i, 0)
					assertArrayEquals(New Long() {rows}, tad.shape())
					'assertEquals(testValues.javaTensorAlongDimension(i, 0), tad);
					i += 1
				Loop

				'Along dimension 1: expect row vector with length 'cols'
				assertEquals(rows * dim2, arr.tensorsAlongDimension(1))
				i = 0
				Do While i < rows * dim2
					Dim tad As INDArray = arr.tensorAlongDimension(i, 1)
					assertArrayEquals(New Long() {cols}, tad.shape())
					'assertEquals(testValues.javaTensorAlongDimension(i, 1), tad);
					i += 1
				Loop

				'Along dimension 2: expect row vector with length 'dim2'
				assertEquals(rows * cols, arr.tensorsAlongDimension(2))
				i = 0
				Do While i < rows * cols
					Dim tad As INDArray = arr.tensorAlongDimension(i, 2)
					assertArrayEquals(New Long() {dim2}, tad.shape())
					'assertEquals(testValues.javaTensorAlongDimension(i, 2), tad);
					i += 1
				Loop
			Next p
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadShapes2d(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadShapes2d(ByVal backend As Nd4jBackend)
			'Ensure TAD returns the correct/expected shapes, and values don't depend on underlying array layout/order etc

			'From a 3d array:
			Dim rows As Integer = 3
			Dim cols As Integer = 4
			Dim dim2 As Integer = 5
			Dim testValues As INDArray = Nd4j.linspace(1, rows * cols * dim2, rows * cols * dim2, DataType.DOUBLE).reshape("c"c, rows, cols, dim2)
			Dim list As IList(Of Pair(Of INDArray, String)) = NDArrayCreationUtil.getAll3dTestArraysWithShape(12345, New Integer(){rows, cols, dim2}, DataType.DOUBLE)
			For Each p As Pair(Of INDArray, String) In list
				Dim arr As INDArray = p.First.assign(testValues)

				'Along dimension 0,1: expect matrix with shape [rows,cols]
				assertEquals(dim2, arr.tensorsAlongDimension(0, 1))
				For i As Integer = 0 To dim2 - 1
					Dim javaTad As INDArray = arr.tensorAlongDimension(i, 0, 1)
					Dim tad As INDArray = arr.tensorAlongDimension(i, 0, 1)
					Dim javaEleStride As Integer = javaTad.elementWiseStride()
					Dim testTad As Integer = tad.elementWiseStride()
					'assertEquals(javaEleStride, testTad);
					assertEquals(javaTad, tad)
					assertArrayEquals(New Long() {rows, cols}, tad.shape())
					assertEquals(testValues.tensorAlongDimension(i, 0, 1), tad)
				Next i

				'Along dimension 0,2: expect matrix with shape [rows,dim2]
				assertEquals(cols, arr.tensorsAlongDimension(0, 2))
				For i As Integer = 0 To cols - 1
					Dim javaTad As INDArray = arr.tensorAlongDimension(i, 0, 2)
					Dim tad As INDArray = arr.tensorAlongDimension(i, 0, 2)
					assertEquals(javaTad, tad)
					assertArrayEquals(New Long() {rows, dim2}, tad.shape())
					assertEquals(testValues.tensorAlongDimension(i, 0, 2), tad)
				Next i

				'Along dimension 1,2: expect matrix with shape [cols,dim2]
				assertEquals(rows, arr.tensorsAlongDimension(1, 2))
				For i As Integer = 0 To rows - 1
					Dim tad As INDArray = arr.tensorAlongDimension(i, 1, 2)
					assertArrayEquals(New Long() {cols, dim2}, tad.shape())
					assertEquals(testValues.tensorAlongDimension(i, 1, 2), tad)
				Next i
			Next p

			'From a 4d array:
			Dim dim3 As Integer = 6
			testValues = Nd4j.linspace(1, rows * cols * dim2 * dim3, rows * cols * dim2 * dim3, DataType.DOUBLE).reshape("c"c, rows, cols, dim2, dim3)
			list = NDArrayCreationUtil.getAll4dTestArraysWithShape(12345, New Integer(){rows, cols, dim2, dim3}, DataType.DOUBLE)
			For Each p As Pair(Of INDArray, String) In list
				Dim arr As INDArray = p.First.assign(testValues)

				'Along dimension 0,1: expect matrix with shape [rows,cols]
				assertEquals(dim2 * dim3, arr.tensorsAlongDimension(0, 1))
				Dim i As Integer = 0
				Do While i < dim2 * dim3
					Dim tad As INDArray = arr.tensorAlongDimension(i, 0, 1)
					assertArrayEquals(New Long() {rows, cols}, tad.shape())
					assertEquals(testValues.tensorAlongDimension(i, 0, 1), tad)
					i += 1
				Loop

				'Along dimension 0,2: expect matrix with shape [rows,dim2]
				assertEquals(cols * dim3, arr.tensorsAlongDimension(0, 2))
				i = 0
				Do While i < cols * dim3
					Dim tad As INDArray = arr.tensorAlongDimension(i, 0, 2)
					assertArrayEquals(New Long() {rows, dim2}, tad.shape())
					assertEquals(testValues.tensorAlongDimension(i, 0, 2), tad)
					i += 1
				Loop

				'Along dimension 0,3: expect matrix with shape [rows,dim3]
				assertEquals(cols * dim2, arr.tensorsAlongDimension(0, 3))
				i = 0
				Do While i < cols * dim2
					Dim tad As INDArray = arr.tensorAlongDimension(i, 0, 3)
					assertArrayEquals(New Long() {rows, dim3}, tad.shape())
					assertEquals(testValues.tensorAlongDimension(i, 0, 3), tad)
					i += 1
				Loop


				'Along dimension 1,2: expect matrix with shape [cols,dim2]
				assertEquals(rows * dim3, arr.tensorsAlongDimension(1, 2))
				i = 0
				Do While i < rows * dim3
					Dim tad As INDArray = arr.tensorAlongDimension(i, 1, 2)
					assertArrayEquals(New Long() {cols, dim2}, tad.shape())
					assertEquals(testValues.tensorAlongDimension(i, 1, 2), tad)
					i += 1
				Loop

				'Along dimension 1,3: expect matrix with shape [cols,dim3]
				assertEquals(rows * dim2, arr.tensorsAlongDimension(1, 3))
				i = 0
				Do While i < rows * dim2
					Dim tad As INDArray = arr.tensorAlongDimension(i, 1, 3)
					assertArrayEquals(New Long() {cols, dim3}, tad.shape())
					assertEquals(testValues.tensorAlongDimension(i, 1, 3), tad)
					i += 1
				Loop

				'Along dimension 2,3: expect matrix with shape [dim2,dim3]
				assertEquals(rows * cols, arr.tensorsAlongDimension(2, 3))
				i = 0
				Do While i < rows * cols
					Dim tad As INDArray = arr.tensorAlongDimension(i, 2, 3)
					assertArrayEquals(New Long() {dim2, dim3}, tad.shape())
					assertEquals(testValues.tensorAlongDimension(i, 2, 3), tad)
					i += 1
				Loop
			Next p
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTadKnownValues(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTadKnownValues(ByVal backend As Nd4jBackend)
			Dim shape() As Long = {2, 3, 4}

			Dim arr As INDArray = Nd4j.create(DataType.DOUBLE, shape)
			Dim i As Integer = 0
			Do While i < shape(0)
				Dim j As Integer = 0
				Do While j < shape(1)
					Dim k As Integer = 0
					Do While k < shape(2)
						Dim d As Double = 100 * i + 10 * j + k
						arr.putScalar(i, j, k, d)
						k += 1
					Loop
					j += 1
				Loop
				i += 1
			Loop

			Dim exp01_0 As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 10, 20},
				New Double() {100, 110, 120}
			})
			Dim exp01_1 As INDArray = Nd4j.create(New Double()() {
				New Double() {1, 11, 21},
				New Double() {101, 111, 121}
			})

			Dim exp02_0 As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1, 2, 3},
				New Double() {100, 101, 102, 103}
			})
			Dim exp02_1 As INDArray = Nd4j.create(New Double()() {
				New Double() {10, 11, 12, 13},
				New Double() {110, 111, 112, 113}
			})

			Dim exp12_0 As INDArray = Nd4j.create(New Double()() {
				New Double() {0, 1, 2, 3},
				New Double() {10, 11, 12, 13},
				New Double() {20, 21, 22, 23}
			})
			Dim exp12_1 As INDArray = Nd4j.create(New Double()() {
				New Double() {100, 101, 102, 103},
				New Double() {110, 111, 112, 113},
				New Double() {120, 121, 122, 123}
			})

			assertEquals(exp01_0, arr.tensorAlongDimension(0, 0, 1))
			assertEquals(exp01_0, arr.tensorAlongDimension(0, 1, 0))
			assertEquals(exp01_1, arr.tensorAlongDimension(1, 0, 1))
			assertEquals(exp01_1, arr.tensorAlongDimension(1, 1, 0))

			assertEquals(exp02_0, arr.tensorAlongDimension(0, 0, 2))
			assertEquals(exp02_0, arr.tensorAlongDimension(0, 2, 0))
			assertEquals(exp02_1, arr.tensorAlongDimension(1, 0, 2))
			assertEquals(exp02_1, arr.tensorAlongDimension(1, 2, 0))

			assertEquals(exp12_0, arr.tensorAlongDimension(0, 1, 2))
			assertEquals(exp12_0, arr.tensorAlongDimension(0, 2, 1))
			assertEquals(exp12_1, arr.tensorAlongDimension(1, 1, 2))
			assertEquals(exp12_1, arr.tensorAlongDimension(1, 2, 1))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStalled(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testStalled(ByVal backend As Nd4jBackend)
			Dim shape() As Integer = {3, 3, 4, 5}
			Dim orig2 As INDArray = Nd4j.create(shape, "c"c)
			Console.WriteLine("Shape: " & Arrays.toString(orig2.shapeInfoDataBuffer().asInt()))
			Dim tad2 As INDArray = orig2.tensorAlongDimension(1, 1, 2, 3)

			log.info("You'll never see this message")
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace