Imports System.Linq
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports LongPointer = org.bytedeco.javacpp.LongPointer
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
import static org.junit.jupiter.api.Assertions.assertArrayEquals

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

Namespace org.nd4j.linalg.specials


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SortCooTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class SortCooTests
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()
		Friend initialDefaultType As DataType = Nd4j.defaultFloatingPointType()



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			Nd4j.setDefaultDataTypes(DataType.FLOAT, DataType.FLOAT)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void setDown()
		Public Overridable Sub setDown()
			Nd4j.setDefaultDataTypes(initialType, Nd4j.defaultFloatingPointType())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void sortSparseCooIndicesSort1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub sortSparseCooIndicesSort1(ByVal backend As Nd4jBackend)
			' FIXME: we don't want this test running on cuda for now
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			If Nd4j.Executioner.GetType().FullName.ToLower().contains("cuda") Then
				Return
			End If

			Dim indices As val = New Long() { 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1}

			' we don't care about
			Dim values() As Double = {2, 1, 0, 3}
			Dim expIndices As val = New Long() { 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1}
			Dim expValues() As Double = {0, 1, 2, 3}

			For Each dataType As DataType In New DataType(){DataType.FLOAT, DataType.DOUBLE, DataType.FLOAT16, DataType.INT64, DataType.INT32, DataType.INT16, DataType.INT8}
				Dim idx As DataBuffer = Nd4j.DataBufferFactory.createLong(indices)
				Dim val As DataBuffer = Nd4j.createTypedBuffer(values, dataType)
				Dim shapeInfo As DataBuffer = Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){2, 2, 2}, val.dataType()).First
				NativeOpsHolder.Instance.getDeviceNativeOps().sortCooIndices(Nothing, CType(idx.addressPointer(), LongPointer), val.addressPointer(), 4, CType(shapeInfo.addressPointer(), LongPointer))

				assertArrayEquals(expIndices, idx.asLong())
				assertArrayEquals(expValues, val.asDouble(), 1e-5)

			Next dataType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void sortSparseCooIndicesSort2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub sortSparseCooIndicesSort2(ByVal backend As Nd4jBackend)
			' FIXME: we don't want this test running on cuda for now
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			If Nd4j.Executioner.GetType().FullName.ToLower().contains("cuda") Then
				Return
			End If

			Dim indices As val = New Long() { 0, 0, 0, 2, 2, 2, 1, 1, 1}

			' we don't care about
			Dim values() As Double = {2, 1, 3}
			Dim expIndices As val = New Long() { 0, 0, 0, 1, 1, 1, 2, 2, 2}
			Dim expValues() As Double = {2, 3, 1}


			For Each dataType As DataType In New DataType(){DataType.FLOAT, DataType.DOUBLE, DataType.FLOAT16, DataType.INT64, DataType.INT32, DataType.INT16, DataType.INT8}
				Dim idx As DataBuffer = Nd4j.DataBufferFactory.createLong(indices)
				Dim val As DataBuffer = Nd4j.createTypedBuffer(values, dataType)
				Dim shapeInfo As DataBuffer = Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){2, 2, 2}, val.dataType()).First
				NativeOpsHolder.Instance.getDeviceNativeOps().sortCooIndices(Nothing, CType(idx.addressPointer(), LongPointer), val.addressPointer(), 3, CType(shapeInfo.addressPointer(), LongPointer))

				assertArrayEquals(expIndices, idx.asLong())
				assertArrayEquals(expValues, val.asDouble(), 1e-5)

			Next dataType


		End Sub

		''' <summary>
		''' Workaround for missing method in DataBuffer interface:
		'''      long[] DataBuffer.GetLongsAt(long i, long length)
		''' 
		''' 
		''' When method is added to interface, this workaround should be deleted!
		''' @return
		''' </summary>
		Friend Shared Function getLongsAt(ByVal buffer As DataBuffer, ByVal i As Long, ByVal length As Long) As Long()
			Return Enumerable.Range(i, length).Select(AddressOf buffer.getLong).ToArray()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void sortSparseCooIndicesSort3(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub sortSparseCooIndicesSort3(ByVal backend As Nd4jBackend)
			' FIXME: we don't want this test running on cuda for now
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			If Nd4j.Executioner.GetType().FullName.ToLower().contains("cuda") Then
				Return
			End If

			Dim rng As Random = Nd4j.Random
			rng.Seed = 12040483421383L
			Dim shape() As Long = {50, 50, 50}
			Dim nnz As Integer = 100
			Dim indices As val = Nd4j.rand(New Integer(){nnz, shape.Length}, rng).muli(50).ravel().toLongVector()
			Dim values As val = Nd4j.rand(New Long(){nnz}).ravel().toDoubleVector()


			Dim indiceBuffer As DataBuffer = Nd4j.DataBufferFactory.createLong(indices)
			Dim valueBuffer As DataBuffer = Nd4j.createBuffer(values)
			Dim shapeInfo As DataBuffer = Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){3, 3, 3}, valueBuffer.dataType()).First
			Dim indMatrix As INDArray = Nd4j.create(indiceBuffer).reshape(New Long(){nnz, shape.Length})

			NativeOpsHolder.Instance.getDeviceNativeOps().sortCooIndices(Nothing, CType(indiceBuffer.addressPointer(), LongPointer), valueBuffer.addressPointer(), nnz, CType(shapeInfo.addressPointer(), LongPointer))

			For i As Long = 1 To nnz - 1
				For j As Long = 0 To shape.Length - 1
					Dim prev As Long = indiceBuffer.getLong(((i - 1) * shape.Length + j))
					Dim current As Long = indiceBuffer.getLong((i * shape.Length + j))
					If prev < current Then
						Exit For
					ElseIf prev > current Then
						Dim prevRow() As Long = getLongsAt(indiceBuffer, (i - 1) * shape.Length, shape.Length)
						Dim currentRow() As Long = getLongsAt(indiceBuffer, i * shape.Length, shape.Length)
						Throw New AssertionError(String.Format("indices are not correctly sorted between element {0:D} and {1:D}. {2} > {3}", i - 1, i, Arrays.toString(prevRow), Arrays.toString(currentRow)))
					End If
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void sortSparseCooIndicesSort4(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub sortSparseCooIndicesSort4(ByVal backend As Nd4jBackend)
			' FIXME: we don't want this test running on cuda for now
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			If Nd4j.Executioner.GetType().FullName.ToLower().contains("cuda") Then
				Return
			End If

			Dim indices As val = New Long() {0, 2, 7, 2, 36, 35, 3, 30, 17, 5, 12, 22, 5, 43, 45, 6, 32, 11, 8, 8, 32, 9, 29, 11, 5, 11, 22, 15, 26, 16, 17, 48, 49, 24, 28, 31, 26, 6, 23, 31, 21, 31, 35, 46, 45, 37, 13, 14, 6, 38, 18, 7, 28, 20, 8, 29, 39, 8, 32, 30, 9, 42, 43, 11, 15, 18, 13, 18, 45, 29, 26, 39, 30, 8, 25, 42, 31, 24, 28, 33, 5, 31, 27, 1, 35, 43, 26, 36, 8, 37, 39, 22, 14, 39, 24, 42, 42, 48, 2, 43, 26, 48, 44, 23, 49, 45, 18, 34, 46, 28, 5, 46, 32, 17, 48, 34, 44, 49, 38, 39}

			Dim expIndices As val = New Long() { 0, 2, 7, 2, 36, 35, 3, 30, 17, 5, 11, 22, 5, 12, 22, 5, 43, 45, 6, 32, 11, 6, 38, 18, 7, 28, 20, 8, 8, 32, 8, 29, 39, 8, 32, 30, 9, 29, 11, 9, 42, 43, 11, 15, 18, 13, 18, 45, 15, 26, 16, 17, 48, 49, 24, 28, 31, 26, 6, 23, 28, 33, 5, 29, 26, 39, 30, 8, 25, 31, 21, 31, 31, 27, 1, 35, 43, 26, 35, 46, 45, 36, 8, 37, 37, 13, 14, 39, 22, 14, 39, 24, 42, 42, 31, 24, 42, 48, 2, 43, 26, 48, 44, 23, 49, 45, 18, 34, 46, 28, 5, 46, 32, 17, 48, 34, 44, 49, 38, 39}

			Dim values() As Double = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39}

			Dim idx As DataBuffer = Nd4j.DataBufferFactory.createLong(indices)
			Dim val As DataBuffer = Nd4j.createBuffer(values)
			Dim shapeInfo As DataBuffer = Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){3, 3, 3}, val.dataType()).First

			NativeOpsHolder.Instance.getDeviceNativeOps().sortCooIndices(Nothing, CType(idx.addressPointer(), LongPointer), val.addressPointer(), 40, CType(shapeInfo.addressPointer(), LongPointer))

			' just check the indices. sortSparseCooIndicesSort1 and sortSparseCooIndicesSort2 checks that
			' indices and values are both swapped. This test just makes sure index sort works for larger arrays.
			assertArrayEquals(expIndices, idx.asLong())
		End Sub
		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace