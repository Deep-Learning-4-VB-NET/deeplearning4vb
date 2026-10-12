Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports LongPointer = org.bytedeco.javacpp.LongPointer
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.fail

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
'ORIGINAL LINE: @Slf4j public class RavelIndexTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class RavelIndexTest
		Inherits BaseNd4jTestWithBackends

		Friend initialType As DataType = Nd4j.dataType()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			Nd4j.DataType = DataType.FLOAT
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void setDown()
		Public Overridable Sub setDown()
			Nd4j.DataType = initialType
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void ravelIndexesTest(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub ravelIndexesTest(ByVal backend As Nd4jBackend)
			' FIXME: we don't want this test running on cuda for now
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			If Nd4j.Executioner.GetType().FullName.ToLower().contains("cuda") Then
				Return
			End If

			Dim multiIdxArray() As Long = {0, 2, 7, 2, 36, 35, 3, 30, 17, 5, 12, 22, 5, 43, 45, 6, 32, 11, 8, 8, 32, 9, 29, 11, 5, 11, 22, 15, 26, 16, 17, 48, 49, 24, 28, 31, 26, 6, 23, 31, 21, 31, 35, 46, 45, 37, 13, 14, 6, 38, 18, 7, 28, 20, 8, 29, 39, 8, 32, 30, 9, 42, 43, 11, 15, 18, 13, 18, 45, 29, 26, 39, 30, 8, 25, 42, 31, 24, 28, 33, 5, 31, 27, 1, 35, 43, 26, 36, 8, 37, 39, 22, 14, 39, 24, 42, 42, 48, 2, 43, 26, 48, 44, 23, 49, 45, 18, 34, 46, 28, 5, 46, 32, 17, 48, 34, 44, 49, 38, 39}

			Dim flatIdxArray() As Long = { 147, 10955, 14717, 21862, 24055, 27451, 34192, 39841, 21792, 64836, 74809, 102791, 109643, 131701, 150265, 156324, 27878, 31380, 35669, 35870, 40783, 47268, 55905, 123659, 126585, 178594, 119915, 132091, 150036, 151797, 165354, 165522, 179762, 182468, 186459, 190294, 195165, 195457, 204024, 208499 }



			Dim clipMode As Integer = 0


			Dim [DIM] As Long = 3
			Dim length As Long = multiIdxArray.Length \ [DIM]
			Dim shape() As Long = {50, 60, 70}


			Dim multiIdxDB As DataBuffer = Nd4j.DataBufferFactory.createLong(multiIdxArray)
			Dim flatIdxDB As DataBuffer = Nd4j.DataBufferFactory.createLong(flatIdxArray)
			Dim shapeInfo As DataBuffer = Nd4j.ShapeInfoProvider.createShapeInformation(shape, DataType.FLOAT).First

			Dim resultMulti As DataBuffer = Nd4j.DataBufferFactory.createLong(length*[DIM])
			Dim resultFlat As DataBuffer = Nd4j.DataBufferFactory.createLong(length)

			NativeOpsHolder.Instance.getDeviceNativeOps().ravelMultiIndex(Nothing, CType(multiIdxDB.addressPointer(), LongPointer), CType(resultFlat.addressPointer(), LongPointer), length, CType(shapeInfo.addressPointer(), LongPointer),clipMode)

			assertArrayEquals(flatIdxArray, resultFlat.asLong())

			NativeOpsHolder.Instance.getDeviceNativeOps().unravelIndex(Nothing, CType(resultMulti.addressPointer(), LongPointer), CType(flatIdxDB.addressPointer(), LongPointer), length, CType(shapeInfo.addressPointer(), LongPointer))

			assertArrayEquals(multiIdxArray, resultMulti.asLong())


			'testing various clipMode cases
			' clipMode = 0: throw an exception
			Try
				shape(2) = 10
				shapeInfo = Nd4j.ShapeInfoProvider.createShapeInformation(shape, DataType.FLOAT).First
				NativeOpsHolder.Instance.getDeviceNativeOps().ravelMultiIndex(Nothing, CType(multiIdxDB.addressPointer(), LongPointer), CType(resultFlat.addressPointer(), LongPointer), length, CType(shapeInfo.addressPointer(), LongPointer),clipMode)
				fail("No exception thrown while using CLIP_MODE_THROW.")

			Catch e As Exception
				'OK
			End Try
			' clipMode = 1: wrap around shape
			clipMode = 1
			multiIdxDB = Nd4j.DataBufferFactory.createLong(New Long() {3, 4, 6, 5, 6, 9})
			resultFlat = Nd4j.DataBufferFactory.createLong(3)
			shapeInfo = Nd4j.ShapeInfoProvider.createShapeInformation(New Long() {4, 6}, DataType.FLOAT).First
			length = 3

			NativeOpsHolder.Instance.getDeviceNativeOps().ravelMultiIndex(Nothing, CType(multiIdxDB.addressPointer(), LongPointer), CType(resultFlat.addressPointer(), LongPointer), length, CType(shapeInfo.addressPointer(), LongPointer), clipMode)
			assertArrayEquals(New Long() {22, 17, 15}, resultFlat.asLong())

			' clipMode = 2: clip to shape
			clipMode = 2
			multiIdxDB = Nd4j.DataBufferFactory.createLong(New Long() {3, 4, 6, 5, 6, 9})
			resultFlat = Nd4j.DataBufferFactory.createLong(3)
			shapeInfo = Nd4j.ShapeInfoProvider.createShapeInformation(New Long() {4, 6}, DataType.FLOAT).First
			length = 3

			NativeOpsHolder.Instance.getDeviceNativeOps().ravelMultiIndex(Nothing, CType(multiIdxDB.addressPointer(), LongPointer), CType(resultFlat.addressPointer(), LongPointer), length, CType(shapeInfo.addressPointer(), LongPointer), clipMode)

			assertArrayEquals(New Long() {22, 23, 23}, resultFlat.asLong())



		End Sub

	End Class

End Namespace