Imports System.IO
Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class IntDataBufferTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class IntDataBufferTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicSerde1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBasicSerde1()


			Dim dataBuffer As DataBuffer = Nd4j.createBuffer(New Integer() {1, 2, 3, 4, 5})
			Dim shapeBuffer As DataBuffer = Nd4j.ShapeInfoProvider.createShapeInformation(New Long() {1, 5}, DataType.INT).First
			Dim intArray As INDArray = Nd4j.createArrayFromShapeBuffer(dataBuffer, shapeBuffer)

			Dim tempFile As File = File.createTempFile("test", "test")
			tempFile.deleteOnExit()

			Nd4j.saveBinary(intArray, tempFile)

			Dim stream As Stream = New FileStream(tempFile, FileMode.Open, FileAccess.Read)
			Dim bis As New BufferedInputStream(stream)
			Dim dis As New DataInputStream(bis)

			Dim loaded As INDArray = Nd4j.read(dis)

			assertEquals(DataType.INT, loaded.data().dataType())
			assertEquals(DataType.LONG, loaded.shapeInfoDataBuffer().dataType())

			assertEquals(intArray.data().length(), loaded.data().length())

			assertArrayEquals(intArray.data().asInt(), loaded.data().asInt())
		End Sub

	'
	'    @Test(expected = ND4JIllegalStateException.class)
	'    public void testOpDiscarded() throws Exception {
	'        DataBuffer dataBuffer = Nd4j.createBuffer(new int[] {1, 2, 3, 4, 5});
	'        DataBuffer shapeBuffer = Nd4j.getShapeInfoProvider().createShapeInformation(new long[] {1, 5}, DataType.INT).getFirst();
	'        INDArray intArray = Nd4j.createArrayFromShapeBuffer(dataBuffer, shapeBuffer);
	'
	'        intArray.add(10f);
	'    }
	'    

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReallocation(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReallocation(ByVal backend As Nd4jBackend)
			Dim buffer As DataBuffer = Nd4j.createBuffer(New Integer() {1, 2, 3, 4})
			assertEquals(4, buffer.capacity())
			buffer.reallocate(6)
			Dim old As val = buffer.asInt()
			assertEquals(6, buffer.capacity())
			Dim newContent As val = buffer.asInt()
			assertEquals(6, newContent.length)
			assertArrayEquals(old, Arrays.CopyOf(newContent, old.length))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReallocationWorkspace(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testReallocationWorkspace(ByVal backend As Nd4jBackend)
			Dim initialConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024L * 1024L).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.NONE).build()
			Dim workspace As MemoryWorkspace = Nd4j.WorkspaceManager.getAndActivateWorkspace(initialConfig, "SOME_ID")

			Dim buffer As DataBuffer = Nd4j.createBuffer(New Integer() {1, 2, 3, 4})
			Dim old As val = buffer.asInt()
			assertTrue(buffer.Attached)
			assertEquals(4, buffer.capacity())
			buffer.reallocate(6)
			assertEquals(6, buffer.capacity())
			Dim newContent As val = buffer.asInt()
			assertEquals(6, newContent.length)
			assertArrayEquals(old, Arrays.CopyOf(newContent, old.length))
			workspace.close()
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace