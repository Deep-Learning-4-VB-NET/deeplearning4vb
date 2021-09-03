Imports System.Threading
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
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports DeviceLocalNDArray = org.nd4j.linalg.util.DeviceLocalNDArray
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

Namespace org.nd4j.linalg.memory


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.WORKSPACES) @NativeTag public class DeviceLocalNDArrayTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DeviceLocalNDArrayTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDeviceLocalStringArray(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDeviceLocalStringArray(ByVal backend As Nd4jBackend)
			Dim arr As val = Nd4j.create(Arrays.asList("first", "second"), 2)
			assertEquals(DataType.UTF8, arr.dataType())
			assertArrayEquals(New Long(){2}, arr.shape())

			Dim dl As val = New DeviceLocalNDArray(arr)

			Dim e As Integer = 0
			Do While e < Nd4j.AffinityManager.NumberOfDevices
				Dim arr2 As val = dl.get(e)
				assertEquals(arr, arr2)
				e += 1
			Loop
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDtypes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDtypes(ByVal backend As Nd4jBackend)
			For Each globalDType As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
				Nd4j.setDefaultDataTypes(globalDType, globalDType)
				For Each arrayDtype As DataType In New DataType(){DataType.DOUBLE, DataType.FLOAT, DataType.HALF}
					Dim arr As INDArray = Nd4j.linspace(arrayDtype, 1, 10, 1)
					Dim dl As New DeviceLocalNDArray(arr)
					Dim get As INDArray = dl.get()
					assertEquals(arr, get)
				Next arrayDtype
			Next globalDType
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDeviceLocalUpdate_1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeviceLocalUpdate_1(ByVal backend As Nd4jBackend)
			Dim numDevices As val = Nd4j.AffinityManager.NumberOfDevices
			If numDevices < 2 Then
				Return
			End If

			Dim array As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f, 4.0f)

			Dim deviceLocal As val = New DeviceLocalNDArray(array)
			For e As Integer = 0 To numDevices - 1
				Dim t As val = New Thread(Sub()
				deviceLocal.get().add(1.0f)
				Nd4j.Executioner.commit()
				End Sub)

				t.start()
				t.join()
			Next e

			Dim counter As val = New AtomicInteger(0)

			Dim update As val = Nd4j.createFromArray(5.0f, 5.0f, 5.0f, 5.0f)
			deviceLocal.update(update)

			For e As Integer = 0 To numDevices - 1
				Dim t As val = New Thread(Sub()
				assertEquals(5.0f, deviceLocal.get().meanNumber().floatValue(), 1e-5f)
				counter.incrementAndGet()
				End Sub)

				t.start()
				t.join()
			Next e

			assertEquals(numDevices, counter.get())
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDelayedDeviceLocalUpdate_1(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDelayedDeviceLocalUpdate_1(ByVal backend As Nd4jBackend)
			Dim numDevices As val = Nd4j.AffinityManager.NumberOfDevices
			If numDevices < 2 Then
				Return
			End If

			Dim array As val = Nd4j.createFromArray(5.0f, 5.0f, 5.0f, 5.0f)

			Dim deviceLocal As val = New DeviceLocalNDArray(array, True)
			Dim counter As val = New AtomicInteger(0)

			For e As Integer = 0 To numDevices - 1
				Dim t As val = New Thread(Sub()
				assertEquals(5.0f, deviceLocal.get().meanNumber().floatValue(), 1e-5f)
				counter.incrementAndGet()
				End Sub)

				t.start()
				t.join()
			Next e

			assertEquals(numDevices, counter.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDelayedDeviceLocalUpdate_2(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDelayedDeviceLocalUpdate_2(ByVal backend As Nd4jBackend)
			Dim numDevices As val = Nd4j.AffinityManager.NumberOfDevices
			If numDevices < 2 Then
				Return
			End If

			Dim array As val = Nd4j.createFromArray(5.0f, 5.0f, 5.0f, 5.0f)

			Dim deviceLocal As val = New DeviceLocalNDArray(array, True)
			Dim counter As val = New AtomicInteger(0)

			deviceLocal.update(Nd4j.createFromArray(4.0f, 4.0f, 4.0f, 4.0f))

			For e As Integer = 0 To numDevices - 1
				Dim t As val = New Thread(Sub()
				assertEquals(4.0f, deviceLocal.get().meanNumber().floatValue(), 1e-5f)
				counter.incrementAndGet()
				End Sub)

				t.start()
				t.join()
			Next e

			assertEquals(numDevices, counter.get())
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace