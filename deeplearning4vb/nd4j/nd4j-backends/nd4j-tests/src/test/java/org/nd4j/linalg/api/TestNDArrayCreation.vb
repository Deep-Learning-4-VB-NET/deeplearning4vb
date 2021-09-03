Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Disabled = org.junit.jupiter.api.Disabled
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
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
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

Namespace org.nd4j.linalg.api


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class TestNDArrayCreation extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestNDArrayCreation
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBufferCreation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBufferCreation(ByVal backend As Nd4jBackend)
			Dim dataBuffer As DataBuffer = Nd4j.createBuffer(New Single() {1, 2})
			Dim pointer As Pointer = dataBuffer.pointer()
			Dim floatPointer As New FloatPointer(pointer)
			Dim dataBuffer1 As DataBuffer = Nd4j.createBuffer(floatPointer, 2, DataType.FLOAT)

			assertEquals(2, dataBuffer.length())
			assertEquals(1.0, dataBuffer.getDouble(0), 1e-1)
			assertEquals(2.0, dataBuffer.getDouble(1), 1e-1)

			assertEquals(2, dataBuffer1.length())
			assertEquals(1.0, dataBuffer1.getDouble(0), 1e-1)
			assertEquals(2.0, dataBuffer1.getDouble(1), 1e-1)
			Dim arr As INDArray = Nd4j.create(dataBuffer1)
			Console.WriteLine(arr)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateNpy() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCreateNpy()
			Dim arrCreate As INDArray = Nd4j.createFromNpyFile((New ClassPathResource("nd4j-tests/test.npy")).File)
			assertEquals(2, arrCreate.size(0))
			assertEquals(2, arrCreate.size(1))
			assertEquals(1.0, arrCreate.getDouble(0, 0), 1e-1)
			assertEquals(2.0, arrCreate.getDouble(0, 1), 1e-1)
			assertEquals(3.0, arrCreate.getDouble(1, 0), 1e-1)
			assertEquals(4.0, arrCreate.getDouble(1, 1), 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateNpz(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCreateNpz(ByVal backend As Nd4jBackend)
			Dim map As IDictionary(Of String, INDArray) = Nd4j.createFromNpzFile((New ClassPathResource("nd4j-tests/test.npz")).File)
			assertEquals(True, map.ContainsKey("x"))
			assertEquals(True, map.ContainsKey("y"))
			Dim arrX As INDArray = map("x")
			Dim arrY As INDArray = map("y")
			assertEquals(1.0, arrX.getDouble(0), 1e-1)
			assertEquals(2.0, arrX.getDouble(1), 1e-1)
			assertEquals(3.0, arrX.getDouble(2), 1e-1)
			assertEquals(4.0, arrX.getDouble(3), 1e-1)
			assertEquals(5.0, arrY.getDouble(0), 1e-1)
			assertEquals(6.0, arrY.getDouble(1), 1e-1)
			assertEquals(7.0, arrY.getDouble(2), 1e-1)
			assertEquals(8.0, arrY.getDouble(3), 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCreateNpy3(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCreateNpy3(ByVal backend As Nd4jBackend)
			Dim arrCreate As INDArray = Nd4j.createFromNpyFile((New ClassPathResource("nd4j-tests/rank3.npy")).File)
			assertEquals(8, arrCreate.length())
			assertEquals(3, arrCreate.rank())

			Dim pointer As Pointer = NativeOpsHolder.Instance.getDeviceNativeOps().pointerForAddress(arrCreate.data().address())
			assertEquals(arrCreate.data().address(), pointer.address())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testEndlessAllocation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testEndlessAllocation(ByVal backend As Nd4jBackend)
			Nd4j.Environment.MaxSpecialMemory = 1
			Do
				Dim arr As val = Nd4j.createUninitialized(DataType.FLOAT, 100000000)
				arr.assign(1.0f)
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("This test is designed to run in isolation. With parallel gc it makes no real sense since allocated amount changes at any time") @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAllocationLimits(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testAllocationLimits(ByVal backend As Nd4jBackend)
			Nd4j.create(1)

			Dim origDeviceLimit As val = Nd4j.Environment.getDeviceLimit(0)
			Dim origDeviceCount As val = Nd4j.Environment.getDeviceCouner(0)

			Dim limit As val = origDeviceCount + 10000

			Nd4j.Environment.setDeviceLimit(0, limit)

			Dim array As val = Nd4j.createUninitialized(DataType.DOUBLE, 1024)
			assertNotNull(array)

			Try
				Nd4j.createUninitialized(DataType.DOUBLE, 1024)
				assertTrue(False)
			Catch e As Exception
				'
			End Try

			' we want to be sure there's nothing left after exception
			assertEquals(0, NativeOpsHolder.Instance.getDeviceNativeOps().lastErrorCode())

			Nd4j.Environment.setDeviceLimit(0, origDeviceLimit)

		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace