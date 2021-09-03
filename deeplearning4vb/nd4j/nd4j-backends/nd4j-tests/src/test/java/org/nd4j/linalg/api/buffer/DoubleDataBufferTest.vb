Imports System
Imports System.IO
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports DoubleIndexer = org.bytedeco.javacpp.indexer.DoubleIndexer
Imports Indexer = org.bytedeco.javacpp.indexer.Indexer
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
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

Namespace org.nd4j.linalg.api.buffer


	''' <summary>
	''' Double data buffer tests
	''' 
	''' This tests the double buffer data opType
	''' Put all buffer related tests here
	''' 
	''' @author Adam Gibson
	''' </summary>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class DoubleDataBufferTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DoubleDataBufferTest
		Inherits BaseNd4jTestWithBackends



		Friend initialType As DataType = Nd4j.dataType()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path testDir;
		Friend testDir As Path


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			DataTypeUtil.setDTypeForContext(DataType.DOUBLE)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			DataTypeUtil.setDTypeForContext(initialType)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPointerCreation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPointerCreation(ByVal backend As Nd4jBackend)
			Dim floatPointer As New DoublePointer(1, 2, 3, 4)
			Dim indexer As Indexer = DoubleIndexer.create(floatPointer)
			Dim buffer As DataBuffer = Nd4j.createBuffer(floatPointer, DataType.DOUBLE, 4, indexer)
			Dim other As DataBuffer = Nd4j.createBuffer(New Double() {1, 2, 3, 4})
			assertArrayEquals(other.asDouble(), buffer.asDouble(), 0.001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetSet(ByVal backend As Nd4jBackend)
			Dim d1() As Double = {1, 2, 3, 4}
			Dim d As DataBuffer = Nd4j.createBuffer(d1)
			Dim d2() As Double = d.asDouble()
			assertArrayEquals(d1, d2, 1e-1f)

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerialization2(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerialization2(ByVal backend As Nd4jBackend)
			Dim arr() As INDArray = {Nd4j.ones(1, 10)}

			For Each a As INDArray In arr
				Dim baos As New MemoryStream()
				Using oos As New ObjectOutputStream(baos)
					oos.writeObject(a)
					oos.flush()
				End Using



				Dim bytes() As SByte = baos.toByteArray()

				Dim bais As New MemoryStream(bytes)
				Dim ois As New ObjectInputStream(bais)

				Dim aDeserialized As INDArray = DirectCast(ois.readObject(), INDArray)

				Console.WriteLine(aDeserialized)
				assertEquals(Nd4j.ones(1, 10).castTo(aDeserialized.dataType()), aDeserialized)
			Next a
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerialization(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerialization(ByVal backend As Nd4jBackend)
			Dim dir As File = testDir.toFile()
			Dim buf As DataBuffer = Nd4j.createBuffer(5)
			Dim fileName As String = "buf.ser"
			Dim file As New File(dir, fileName)
			file.deleteOnExit()
			SerializationUtils.saveObject(buf, file)
			Dim buf2 As DataBuffer = SerializationUtils.readObject(file)
			'assertEquals(buf, buf2);
			assertArrayEquals(buf.asDouble(), buf2.asDouble(), 0.001)

			Nd4j.alloc = DataBuffer.AllocationMode.DIRECT
			buf = Nd4j.createBuffer(5)
			file.deleteOnExit()
			SerializationUtils.saveObject(buf, file)
			buf2 = SerializationUtils.readObject(file)
			'        assertEquals(buf, buf2);
			assertArrayEquals(buf.asDouble(), buf2.asDouble(), 0.001)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDup(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDup(ByVal backend As Nd4jBackend)
			Dim d1() As Double = {1, 2, 3, 4}
			Dim d As DataBuffer = Nd4j.createBuffer(d1)
			Dim d2 As DataBuffer = d.dup()
			assertArrayEquals(d.asDouble(), d2.asDouble(), 0.0001f)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPut(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPut(ByVal backend As Nd4jBackend)
			Dim d1() As Double = {1, 2, 3, 4}
			Dim d As DataBuffer = Nd4j.createBuffer(d1)
			d.put(0, 0.0)
			Dim result() As Double = {0, 2, 3, 4}
			d1 = d.asDouble()
			assertArrayEquals(d1, result, 1e-1f)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRange(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRange(ByVal backend As Nd4jBackend)
			Dim buffer As DataBuffer = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).data()
			Dim get() As Double = buffer.getDoublesAt(0, 3)
			Dim data() As Double = {1, 2, 3}
			assertArrayEquals(get, data, 1e-1f)


			Dim get2() As Double = buffer.asDouble()
			Dim allData() As Double = buffer.getDoublesAt(0, CInt(buffer.length()))
			assertArrayEquals(get2, allData, 1e-1f)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetOffsetRange(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetOffsetRange(ByVal backend As Nd4jBackend)
			Dim buffer As DataBuffer = Nd4j.linspace(1, 5, 5, DataType.DOUBLE).data()
			Dim get() As Double = buffer.getDoublesAt(1, 3)
			Dim data() As Double = {2, 3, 4}
			assertArrayEquals(get, data, 1e-1f)


			Dim allButLast() As Double = {2, 3, 4, 5}

			Dim allData() As Double = buffer.getDoublesAt(1, CInt(buffer.length()))
			assertArrayEquals(allButLast, allData, 1e-1f)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssign(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssign(ByVal backend As Nd4jBackend)
			Dim assertion As DataBuffer = Nd4j.createBuffer(New Double() {1, 2, 3})
			Dim one As DataBuffer = Nd4j.createBuffer(New Double() {1})
			Dim twoThree As DataBuffer = Nd4j.createBuffer(New Double() {2, 3})
			Dim blank As DataBuffer = Nd4j.createBuffer(New Double() {0, 0, 0})
			blank.assign(one, twoThree)
			assertArrayEquals(assertion.asDouble(), blank.asDouble(), 0.0001)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOffset(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOffset(ByVal backend As Nd4jBackend)
			Dim create As DataBuffer = Nd4j.createBuffer(New Double() {1, 2, 3, 4}, 2)
			assertEquals(2, create.length())
			assertEquals(0, create.offset())
			assertEquals(3, create.getDouble(0), 1e-1)
			assertEquals(4, create.getDouble(1), 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReallocation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReallocation(ByVal backend As Nd4jBackend)
			Dim buffer As DataBuffer = Nd4j.createBuffer(New Double() {1, 2, 3, 4})
			assertEquals(4, buffer.capacity())
			Dim old() As Double = buffer.asDouble()
			buffer.reallocate(6)
			assertEquals(6, buffer.capacity())
			assertArrayEquals(old, Arrays.CopyOf(buffer.asDouble(), 4), 1e-1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReallocationWorkspace(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReallocationWorkspace(ByVal backend As Nd4jBackend)
			Dim initialConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.NONE).build()
			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.getAndActivateWorkspace(initialConfig, "SOME_ID")

			Dim buffer As DataBuffer = Nd4j.createBuffer(New Double() {1, 2, 3, 4})
			Dim old() As Double = buffer.asDouble()
			assertTrue(buffer.Attached)
			assertEquals(4, buffer.capacity())
			buffer.reallocate(6)
			assertEquals(6, buffer.capacity())
			assertArrayEquals(old, Arrays.CopyOf(buffer.asDouble(), 4), 1e-1)
			workspace.close()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddressPointer()
		Public Overridable Sub testAddressPointer()
			If Nd4j.Executioner.type() <> OpExecutioner.ExecutionerType.NATIVE_CPU Then
				Return
			End If
			Dim buffer As DataBuffer = Nd4j.createBuffer(New Double() {1, 2, 3, 4})
			Dim wrappedBuffer As DataBuffer = Nd4j.createBuffer(buffer, 1, 2)

			Dim pointer As DoublePointer = CType(wrappedBuffer.addressPointer(), DoublePointer)
			assertEquals(buffer.getDouble(1), pointer.get(0), 1e-1)
			assertEquals(buffer.getDouble(2), pointer.get(1), 1e-1)

			Try
				pointer.asBuffer().get(3) ' Try to access element outside pointer capacity.
				fail("Accessing this address should not be allowed!")
			Catch e As System.IndexOutOfRangeException
				' do nothing
			End Try
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace