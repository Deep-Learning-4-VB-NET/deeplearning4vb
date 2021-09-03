Imports DirectBuffer = org.agrona.DirectBuffer
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage
Imports BufferUtil = org.nd4j.aeron.util.BufferUtil
Imports BaseND4JTest = org.nd4j.common.tests.BaseND4JTest
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.nd4j.aeron.ipc.chunk


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NotThreadSafe @Disabled("Tests are too flaky") @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class NDArrayMessageChunkTests extends org.nd4j.common.tests.BaseND4JTest
	Public Class NDArrayMessageChunkTests
		Inherits BaseND4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testChunkSerialization()
		Public Overridable Sub testChunkSerialization()
			Dim message As NDArrayMessage = NDArrayMessage.wholeArrayUpdate(Nd4j.ones(1000))
			Dim chunkSize As Integer = 128
			Dim numChunks As Integer = NDArrayMessage.numChunksForMessage(message, chunkSize)
			Dim chunks() As NDArrayMessageChunk = NDArrayMessage.chunks(message, chunkSize)
			assertEquals(numChunks, chunks.Length)
			For i As Integer = 1 To numChunks - 1
				assertEquals(chunks(0).getMessageType(), chunks(i).getMessageType())
				assertEquals(chunks(0).getId(), chunks(i).getId())
				assertEquals(chunks(0).getChunkSize(), chunks(i).getChunkSize())
				assertEquals(chunks(0).getNumChunks(), chunks(i).getNumChunks())
			Next i

			Dim concat(chunks.Length - 1) As ByteBuffer
			For i As Integer = 0 To concat.Length - 1
				concat(i) = chunks(i).getData()
			Next i


			Dim buffer As DirectBuffer = NDArrayMessage.toBuffer(message)
			'test equality of direct byte buffer contents vs chunked
			Dim byteBuffer As ByteBuffer = buffer.byteBuffer()
			Dim concatAll As ByteBuffer = BufferUtil.concat(concat, buffer.capacity())
			Dim concatAllBuffer As Buffer = CType(concatAll, Buffer)
			Dim byteBuffer1 As Buffer = CType(byteBuffer, Buffer)
			Dim arrays(byteBuffer.capacity() - 1) As SByte
			byteBuffer1.rewind()
			byteBuffer.get(arrays)
			Dim arrays2(concatAll.capacity() - 1) As SByte
			concatAllBuffer.rewind()
			concatAll.get(arrays2)
			assertArrayEquals(arrays, arrays2)
			Dim message1 As NDArrayMessage = NDArrayMessage.fromChunks(chunks)
			assertEquals(message, message1)

		End Sub

	End Class

End Namespace