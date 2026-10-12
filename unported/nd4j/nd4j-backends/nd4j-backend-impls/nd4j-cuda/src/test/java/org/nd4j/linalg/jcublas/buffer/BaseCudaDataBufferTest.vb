Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Test = org.junit.jupiter.api.Test
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports PrintVariable = org.nd4j.linalg.api.ops.util.PrintVariable
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.linalg.jcublas.buffer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BaseCudaDataBufferTest extends org.nd4j.common.tests.BaseND4JTest
	Public Class BaseCudaDataBufferTest
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setUp()
		Public Overridable Sub setUp()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicAllocation_1()
		Public Overridable Sub testBasicAllocation_1()
			Dim array As val = Nd4j.create(DataType.FLOAT, 5)

			' basic validation
			assertNotNull(array)
			assertNotNull(array.data())
			assertNotNull(CType(array.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			' shape part
			assertArrayEquals(New Long(){1, 5, 1, 8192, 1, 99}, array.shapeInfoJava())
			assertArrayEquals(New Long(){1, 5, 1, 8192, 1, 99}, array.shapeInfoDataBuffer().asLong())

			' arrat as full of zeros at this point
			assertArrayEquals(New Single() {0.0f, 0.0f, 0.0f, 0.0f, 0.0f}, array.data().asFloat(), 1e-5f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicAllocation_2()
		Public Overridable Sub testBasicAllocation_2()
			Dim array As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f, 4.0f, 5.0f)

			' basic validation
			assertNotNull(array)
			assertNotNull(array.data())
			assertNotNull(CType(array.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			' shape part
			assertArrayEquals(New Long(){1, 5, 1, 8192, 1, 99}, array.shapeInfoJava())
			assertArrayEquals(New Long(){1, 5, 1, 8192, 1, 99}, array.shapeInfoDataBuffer().asLong())

			' arrat as full of values at this point
			assertArrayEquals(New Single() {1.0f, 2.0f, 3.0f, 4.0f, 5.0f}, array.data().asFloat(), 1e-5f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicView_1()
		Public Overridable Sub testBasicView_1()
			Dim array As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f).reshape(3, 2)

			' basic validation
			assertNotNull(array)
			assertNotNull(array.data())
			assertNotNull(CType(array.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			' checking TAD equality
			Dim row As val = array.getRow(1)
			assertArrayEquals(New Single(){3.0f, 4.0f}, row.data().dup().asFloat(), 1e-5f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScalar_1()
		Public Overridable Sub testScalar_1()
			Dim scalar As val = Nd4j.scalar(119.0f)

			' basic validation
			assertNotNull(scalar)
			assertNotNull(scalar.data())
			assertNotNull(CType(scalar.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			' shape part
			assertArrayEquals(New Long(){0, 8192, 1, 99}, scalar.shapeInfoJava())
			assertArrayEquals(New Long(){0, 8192, 1, 99}, scalar.shapeInfoDataBuffer().asLong())

			' pointers part
			Dim devPtr As val = AtomicAllocator.Instance.getPointer(scalar.data())
			Dim hostPtr As val = AtomicAllocator.Instance.getHostPointer(scalar.data())

			' dev pointer supposed to exist, and host pointer is not
			assertNotNull(devPtr)
			assertNull(hostPtr)

			assertEquals(119.0f, scalar.getFloat(0), 1e-5f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerDe_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerDe_1()
			Dim array As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f)
			Dim baos As val = New MemoryStream()

			Nd4j.write(baos, array)
			Dim restored As INDArray = Nd4j.read(New MemoryStream(baos.toByteArray()))

			' basic validation
			assertNotNull(restored)
			assertNotNull(restored.data())
			assertNotNull(DirectCast(restored.data(), BaseCudaDataBuffer).OpaqueDataBuffer)

			' shape part
			assertArrayEquals(New Long(){1, 6, 1, 8192, 1, 99}, restored.shapeInfoJava())
			assertArrayEquals(New Long(){1, 6, 1, 8192, 1, 99}, restored.shapeInfoDataBuffer().asLong())

			' data equality
			assertArrayEquals(array.data().asFloat(), restored.data().asFloat(), 1e-5f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicOpInvocation_1()
		Public Overridable Sub testBasicOpInvocation_1()
			Dim array1 As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f)
			Dim array2 As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f)

			' shape pointers must be equal here
			Dim devPtr1 As val = AtomicAllocator.Instance.getPointer(array1.shapeInfoDataBuffer())
			Dim devPtr2 As val = AtomicAllocator.Instance.getPointer(array2.shapeInfoDataBuffer())

			Dim hostPtr1 As val = AtomicAllocator.Instance.getHostPointer(array1.shapeInfoDataBuffer())
			Dim hostPtr2 As val = AtomicAllocator.Instance.getHostPointer(array2.shapeInfoDataBuffer())

			' pointers must be equal on host and device, since we have shape cache
			assertEquals(devPtr1.address(), devPtr2.address())
			assertEquals(hostPtr1.address(), hostPtr2.address())

			assertEquals(array1, array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicOpInvocation_2()
		Public Overridable Sub testBasicOpInvocation_2()
			Dim array1 As val = Nd4j.createFromArray(1.0f, 200.0f, 3.0f, 4.0f, 5.0f, 6.0f)
			Dim array2 As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f)

			assertNotEquals(array1, array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicOpInvocation_3()
		Public Overridable Sub testBasicOpInvocation_3()
			Dim array As val = Nd4j.create(DataType.FLOAT, 6)
			Dim exp As val = Nd4j.createFromArray(1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f)

			array.addi(1.0f)

			assertEquals(exp, array)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCustomOpInvocation_1()
		Public Overridable Sub testCustomOpInvocation_1()
			Dim array As val = Nd4j.createFromArray(1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f)

			Nd4j.exec(New PrintVariable(array, True))
			Nd4j.exec(New PrintVariable(array))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testMultiDeviceMigration_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiDeviceMigration_1()
			If Nd4j.AffinityManager.NumberOfDevices < 2 Then
				Return
			End If

			' creating all arrays within main thread context
			Dim list As val = New List(Of INDArray)()
			Dim e As Integer = 0
			Do While e < Nd4j.AffinityManager.NumberOfDevices
				list.add(Nd4j.create(DataType.FLOAT, 3, 5))
				e += 1
			Loop

			Dim cnt As val = New AtomicInteger(0)

			' now we're creating threads
			e = 0
			Do While e < Nd4j.AffinityManager.NumberOfDevices
				Dim f As val = e
				Dim t As val = New Thread(Sub()
				' issuing one operation, just to see how migration works
				list.get(f).addi(1.0f)

				' synchronizing immediately
				Nd4j.Executioner.commit()
				cnt.incrementAndGet()
				End Sub)

				t.start()
				t.join()
				e += 1
			Loop

			' there shoul dbe no exceptions during execution
			assertEquals(Nd4j.AffinityManager.NumberOfDevices, cnt.get())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClose_1()
		Public Overridable Sub testClose_1()
			Dim x As val = Nd4j.createFromArray(1, 2, 3)

			x.close()

			assertTrue(x.wasClosed())
			assertTrue(x.data().wasClosed())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testClose_2()
		Public Overridable Sub testClose_2()
			Dim x As val = Nd4j.create(DataType.FLOAT, 5, 6)
			Dim row As val = x.getRow(1)
			x.close()

			assertTrue(x.wasClosed())
			assertTrue(x.data().wasClosed())

			assertTrue(row.wasClosed())
			assertTrue(row.data().wasClosed())
		End Sub
	End Class
End Namespace