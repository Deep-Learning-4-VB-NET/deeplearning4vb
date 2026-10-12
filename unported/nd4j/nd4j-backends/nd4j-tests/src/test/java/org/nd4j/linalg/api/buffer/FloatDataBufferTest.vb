Imports System
Imports System.IO
Imports val = lombok.val
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports FloatIndexer = org.bytedeco.javacpp.indexer.FloatIndexer
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
	''' Float data buffer tests
	''' 
	''' This tests the float buffer data opType
	''' Put all buffer related tests here
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class FloatDataBufferTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class FloatDataBufferTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir Path tempDir;
		Friend tempDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()
			Console.WriteLine("DATATYPE HERE: " & Nd4j.dataType())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPointerCreation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPointerCreation(ByVal backend As Nd4jBackend)
			Dim floatPointer As New FloatPointer(1, 2, 3, 4)
			Dim indexer As Indexer = FloatIndexer.create(floatPointer)
			Dim buffer As DataBuffer = Nd4j.createBuffer(floatPointer, DataType.FLOAT, 4, indexer)
			Dim other As DataBuffer = Nd4j.createBuffer(New Single() {1, 2, 3, 4})
			assertArrayEquals(other.asFloat(), buffer.asFloat(), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetSet(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetSet(ByVal backend As Nd4jBackend)
			Dim d1() As Single = {1, 2, 3, 4}
			Dim d As DataBuffer = Nd4j.createBuffer(d1)
			Dim d2() As Single = d.asFloat()
			assertArrayEquals(d1, d2, 1e-1f,getFailureMessage(backend))

		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSerialization(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testSerialization(ByVal backend As Nd4jBackend)
			Dim dir As File = tempDir.resolve("new-dir-1").toFile()
			dir.mkdirs()
			Dim buf As DataBuffer = Nd4j.createBuffer(5)
			Dim fileName As String = "buf.ser"
			Dim file As New File(dir, fileName)
			file.deleteOnExit()
			SerializationUtils.saveObject(buf, file)
			Dim buf2 As DataBuffer = SerializationUtils.readObject(file)
			'        assertEquals(buf, buf2);
			assertArrayEquals(buf.asFloat(), buf2.asFloat(), 0.0001f)

			Nd4j.alloc = DataBuffer.AllocationMode.DIRECT
			buf = Nd4j.createBuffer(5)
			file.deleteOnExit()
			SerializationUtils.saveObject(buf, file)
			buf2 = SerializationUtils.readObject(file)
			'assertEquals(buf, buf2);
			assertArrayEquals(buf.asFloat(), buf2.asFloat(), 0.0001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDup(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDup(ByVal backend As Nd4jBackend)
			Dim d1() As Single = {1, 2, 3, 4}
			Dim d As DataBuffer = Nd4j.createBuffer(d1)
			Dim d2 As DataBuffer = d.dup()
			assertArrayEquals(d.asFloat(), d2.asFloat(), 0.001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToNio(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToNio(ByVal backend As Nd4jBackend)
			Dim buff As DataBuffer = Nd4j.createTypedBuffer(New Double() {1, 2, 3, 4}, DataType.FLOAT)
			assertEquals(4, buff.length())
			If buff.allocationMode() = DataBuffer.AllocationMode.HEAP Then
				Return
			End If

			Dim nio As ByteBuffer = buff.asNio()
			assertEquals(16, nio.capacity())

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPut(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPut(ByVal backend As Nd4jBackend)
			Dim d1() As Single = {1, 2, 3, 4}
			Dim d As DataBuffer = Nd4j.createBuffer(d1)
			d.put(0, 0.0)
			Dim result() As Single = {0, 2, 3, 4}
			d1 = d.asFloat()
			assertArrayEquals(d1, result, 1e-1f,getFailureMessage(backend))
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetRange(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetRange(ByVal backend As Nd4jBackend)
			Dim buffer As DataBuffer = Nd4j.linspace(1, 5, 5).data()
			Dim get() As Single = buffer.getFloatsAt(0, 3)
			Dim data() As Single = {1, 2, 3}
			assertArrayEquals(get, data, 1e-1f,getFailureMessage(backend))


			Dim get2() As Single = buffer.asFloat()
			Dim allData() As Single = buffer.getFloatsAt(0, CInt(buffer.length()))
			assertArrayEquals(get2, allData, 1e-1f,getFailureMessage(backend))


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testGetOffsetRange(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGetOffsetRange(ByVal backend As Nd4jBackend)
			Dim buffer As DataBuffer = Nd4j.linspace(1, 5, 5).data()
			Dim get() As Single = buffer.getFloatsAt(1, 3)
			Dim data() As Single = {2, 3, 4}
			assertArrayEquals(get, data, 1e-1f,getFailureMessage(backend))


			Dim allButLast() As Single = {2, 3, 4, 5}

			Dim allData() As Single = buffer.getFloatsAt(1, CInt(buffer.length()))
			assertArrayEquals(allButLast, allData, 1e-1f,getFailureMessage(backend))


		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAsBytes(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAsBytes(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.create(DataType.FLOAT,5)
			Dim d() As SByte = arr.data().asBytes()
			assertEquals(4 * 5, d.Length,getFailureMessage(backend))
			Dim rand As INDArray = Nd4j.rand(3, 3)
			rand.data().asBytes()

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAssign(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAssign(ByVal backend As Nd4jBackend)
			Dim assertion As DataBuffer = Nd4j.createBuffer(New Double() {1, 2, 3})
			Dim one As DataBuffer = Nd4j.createBuffer(New Double() {1})
			Dim twoThree As DataBuffer = Nd4j.createBuffer(New Double() {2, 3})
			Dim blank As DataBuffer = Nd4j.createBuffer(New Double() {0, 0, 0})
			blank.assign(one, twoThree)
			assertArrayEquals(assertion.asFloat(), blank.asFloat(), 0.0001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReadWrite(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testReadWrite(ByVal backend As Nd4jBackend)
			Dim assertion As DataBuffer = Nd4j.createBuffer(New Double() {1, 2, 3})
			Dim bos As New MemoryStream()
			Dim dos As New DataOutputStream(bos)
			assertion.write(dos)

			Dim clone As DataBuffer = assertion.dup()
			Dim stream As val = New DataInputStream(New MemoryStream(bos.toByteArray()))
			Dim header As val = BaseDataBuffer.readHeader(stream)
			assertion.read(stream, header.getLeft(), header.getMiddle(), header.getRight())
			assertArrayEquals(assertion.asFloat(), clone.asFloat(), 0.0001f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOffset(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testOffset(ByVal backend As Nd4jBackend)
			Dim create As DataBuffer = Nd4j.createBuffer(New Single() {1, 2, 3, 4}, 2)
			assertEquals(2, create.length())
			assertEquals(0, create.offset())
			assertEquals(3, create.getDouble(0), 1e-1)
			assertEquals(4, create.getDouble(1), 1e-1)

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReallocation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReallocation(ByVal backend As Nd4jBackend)
			Dim buffer As DataBuffer = Nd4j.createBuffer(New Single() {1, 2, 3, 4})
			assertEquals(4, buffer.capacity())
			Dim old() As Single = buffer.asFloat()
			buffer.reallocate(6)
			Dim newBuf() As Single = buffer.asFloat()
			assertEquals(6, buffer.capacity())
			'note: old and new buf are not equal because java automatically populates the arrays with zeros
			'the new buffer is actually 1,2,3,4,0,0 because of this
			assertArrayEquals(New Single(){1, 2, 3, 4, 0, 0}, newBuf, 1e-4F)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReallocationWorkspace(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReallocationWorkspace(ByVal backend As Nd4jBackend)
			Dim initialConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.NONE).build()
			Using workspace As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(initialConfig, "SOME_ID")
				Dim buffer As DataBuffer = Nd4j.createBuffer(New Single() {1, 2, 3, 4})
				assertTrue(buffer.Attached)
				Dim old() As Single = buffer.asFloat()
				assertEquals(4, buffer.capacity())
				buffer.reallocate(6)
				assertEquals(6, buffer.capacity())
				Dim newBuf() As Single = buffer.asFloat()
				'note: java creates new zeros by default for empty array spots
				assertArrayEquals(New Single(){1, 2, 3, 4, 0, 0}, newBuf, 1e-4F)
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAddressPointer(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAddressPointer(ByVal backend As Nd4jBackend)
			If Nd4j.Executioner.type() <> OpExecutioner.ExecutionerType.NATIVE_CPU Then
				Return
			End If

			Dim buffer As DataBuffer = Nd4j.createBuffer(New Single() {1, 2, 3, 4})
			Dim wrappedBuffer As DataBuffer = Nd4j.createBuffer(buffer, 1, 2)

			Dim pointer As FloatPointer = CType(wrappedBuffer.addressPointer(), FloatPointer)
			assertEquals(buffer.getFloat(1), pointer.get(0), 1e-1)
			assertEquals(buffer.getFloat(2), pointer.get(1), 1e-1)

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