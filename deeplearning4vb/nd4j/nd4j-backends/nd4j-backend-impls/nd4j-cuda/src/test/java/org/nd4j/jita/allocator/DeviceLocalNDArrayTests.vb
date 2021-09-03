Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DeviceLocalNDArray = org.nd4j.linalg.util.DeviceLocalNDArray
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals

' ******************************************************************************
' *
' *
' * This program and the accompanying materials are made available under the
' * terms of the Apache License, Version 2.0 which is available at
' * https://www.apache.org/licenses/LICENSE-2.0.
' *
' *  See the NOTICE file distributed with this work for additional
' *  information regarding copyright ownership.
' * Unless required by applicable law or agreed to in writing, software
' * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' * License for the specific language governing permissions and limitations
' * under the License.
' *
' * SPDX-License-Identifier: Apache-2.0
' *****************************************************************************

Namespace org.nd4j.jita.allocator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DeviceLocalNDArrayTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class DeviceLocalNDArrayTests
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDeviceLocalArray_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeviceLocalArray_1()
			Dim arr As val = Nd4j.create(DataType.DOUBLE, 10, 10)

			Dim dl As val = New DeviceLocalNDArray(arr)

			Dim e As Integer = 0
			Do While e < Nd4j.AffinityManager.NumberOfDevices
				Dim f As val = e
				Dim t As val = New Thread(Sub()
				Nd4j.AffinityManager.unsafeSetDevice(f)
				dl.get().addi(1.0)
				Nd4j.Executioner.commit()
				End Sub)

				t.start()
				t.join()
				e += 1
			Loop
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDeviceLocalArray_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testDeviceLocalArray_2()
			Dim shape As val = New Long(){10, 10}
			Dim arr As val = Nd4j.create(DataType.DOUBLE, shape)

			Dim dl As val = New DeviceLocalNDArray(arr)

			Dim e As Integer = 0
			Do While e < Nd4j.AffinityManager.NumberOfDevices
				Dim f As val = e
				Dim t As val = New Thread(Sub()
				Nd4j.AffinityManager.unsafeSetDevice(f)
				For i As Integer = 0 To 9
					Dim tmp As val = Nd4j.create(DataType.DOUBLE, shape)
					tmp.addi(1.0)
					Nd4j.Executioner.commit()
				Next i
				End Sub)

				t.start()
				t.join()

				System.GC.Collect()
				e += 1
			Loop
			System.GC.Collect()

			e = 0
			Do While e < Nd4j.AffinityManager.NumberOfDevices
				Dim f As val = e
				Dim t As val = New Thread(Sub()
				Nd4j.AffinityManager.unsafeSetDevice(f)
				dl.get().addi(1.0)
				Nd4j.Executioner.commit()
				End Sub)

				t.start()
				t.join()
				e += 1
			Loop
		End Sub
	End Class

End Namespace