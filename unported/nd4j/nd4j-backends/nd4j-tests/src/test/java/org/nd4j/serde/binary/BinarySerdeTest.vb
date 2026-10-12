Imports System
Imports System.IO
Imports StopWatch = org.apache.commons.lang3.time.StopWatch
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports OpValidationSuite = org.nd4j.OpValidationSuite
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
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

Namespace org.nd4j.serde.binary


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.NDARRAY_SERDE) public class BinarySerdeTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class BinarySerdeTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToAndFrom(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToAndFrom(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.scalar(1.0)
			Dim buffer As ByteBuffer = BinarySerde.toByteBuffer(arr)
			Dim back As INDArray = BinarySerde.toArray(buffer)
			assertEquals(arr, back)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToAndFromHeapBuffer(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToAndFromHeapBuffer(ByVal backend As Nd4jBackend)
			Dim arr As INDArray = Nd4j.scalar(1.0)
			Dim buffer As ByteBuffer = BinarySerde.toByteBuffer(arr)
			Dim heapBuffer As ByteBuffer = ByteBuffer.allocate(buffer.remaining())
			heapBuffer.put(buffer)
			Dim back As INDArray = BinarySerde.toArray(heapBuffer)
			assertEquals(arr, back)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToAndFromCompressed(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToAndFromCompressed(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing() 'Failing 2019/01/24
			Dim arr As INDArray = Nd4j.scalar(1.0)
			Dim compress As INDArray = Nd4j.Compressor.compress(arr, "GZIP")
			assertTrue(compress.Compressed)
			Dim buffer As ByteBuffer = BinarySerde.toByteBuffer(compress)
			Dim back As INDArray = BinarySerde.toArray(buffer)
			Dim decompressed As INDArray = Nd4j.Compressor.decompress(compress)
			assertEquals(arr, decompressed)
			assertEquals(arr, back)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testToAndFromCompressedLarge(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testToAndFromCompressedLarge(ByVal backend As Nd4jBackend)
			OpValidationSuite.ignoreFailing() 'Failing 2019/01/24
			Dim arr As INDArray = Nd4j.zeros(CInt(Math.Truncate(1e))7)
			Dim compress As INDArray = Nd4j.Compressor.compress(arr, "GZIP")
			assertTrue(compress.Compressed)
			Dim buffer As ByteBuffer = BinarySerde.toByteBuffer(compress)
			Dim back As INDArray = BinarySerde.toArray(buffer)
			Dim decompressed As INDArray = Nd4j.Compressor.decompress(compress)
			assertEquals(arr, decompressed)
			assertEquals(arr, back)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReadWriteFile(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testReadWriteFile(ByVal backend As Nd4jBackend)
			Dim tmpFile As New File(System.getProperty("java.io.tmpdir"), "ndarraytmp-" & System.Guid.randomUUID().ToString() & " .bin")
			tmpFile.deleteOnExit()
			Dim rand As INDArray = Nd4j.randn(5, 5)
			BinarySerde.writeArrayToDisk(rand, tmpFile)
			Dim fromDisk As INDArray = BinarySerde.readFromDisk(tmpFile)
			assertEquals(rand, fromDisk)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testReadShapeFile(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testReadShapeFile(ByVal backend As Nd4jBackend)
			Dim tmpFile As New File(System.getProperty("java.io.tmpdir"), "ndarraytmp-" & System.Guid.randomUUID().ToString() & " .bin")
			tmpFile.deleteOnExit()
			Dim rand As INDArray = Nd4j.randn(5, 5)
			BinarySerde.writeArrayToDisk(rand, tmpFile)
			Dim buffer As DataBuffer = BinarySerde.readShapeFromDisk(tmpFile)

			assertArrayEquals(rand.shapeInfoDataBuffer().asLong(), buffer.asLong())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void timeOldVsNew(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub timeOldVsNew(ByVal backend As Nd4jBackend)
			Dim numTrials As Integer = 1000
			Dim oldTotal As Long = 0
			Dim newTotal As Long = 0
			Dim arr As INDArray = Nd4j.create(100000)
			Nd4j.Compressor.compressi(arr, "GZIP")
			For i As Integer = 0 To numTrials - 1
				Dim oldStopWatch As New StopWatch()
				Dim bos As New BufferedOutputStream(New MemoryStream(CInt(arr.length())))
				Dim dos As New DataOutputStream(bos)
				oldStopWatch.start()
				Nd4j.write(arr, dos)
				oldStopWatch.stop()
				' System.out.println("Old " + oldStopWatch.getNanoTime());
				oldTotal += oldStopWatch.getNanoTime()
				Dim newStopWatch As New StopWatch()
				newStopWatch.start()
				BinarySerde.toByteBuffer(arr)
				newStopWatch.stop()
				'  System.out.println("New " + newStopWatch.getNanoTime());
				newTotal += newStopWatch.getNanoTime()

			Next i

			oldTotal \= numTrials
			newTotal \= numTrials
			Console.WriteLine("Old avg " & oldTotal & " New avg " & newTotal)

		End Sub

	End Class

End Namespace