Imports System
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports AeronNDArraySubscriber = org.nd4j.aeron.ipc.AeronNDArraySubscriber
Imports NDArrayMessage = org.nd4j.aeron.ipc.NDArrayMessage

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
'ORIGINAL LINE: @Data @Builder public class NDArrayMessageChunk implements java.io.Serializable
	<Serializable>
	Public Class NDArrayMessageChunk
		'id of the chunk (meant for tracking for reassembling)
		Private id As String
		'the chunk size (message size over the network)
		Private chunkSize As Integer
		'the message opType, this should be chunked
		Private messageType As NDArrayMessage.MessageType
		'the number of chunks for reassembling the message
		Private numChunks As Integer
		'the index of this particular chunk
		Private chunkIndex As Integer
		'the actual chunk data
		Private data As ByteBuffer


		''' <summary>
		''' Returns the overall size for an <seealso cref="NDArrayMessageChunk"/>.
		''' The size of a message chunk is:
		''' idLengthSize(4) + messageTypeSize(4) + indexSize(4) + chunkSizeSize(4) +  numChunksSize(4) + chunk.getData().limit() + chunk.getId().getBytes().length
		''' Many of these are flat out integers and are mainly variables for accounting purposes and ease of readbility </summary>
		''' <param name="chunk"> the size of a message chunk </param>
		''' <returns> the size of an <seealso cref="ByteBuffer"/> for the given <seealso cref="NDArrayMessageChunk"/> </returns>
		Public Shared Function sizeForMessage(ByVal chunk As NDArrayMessageChunk) As Integer
			Dim messageTypeSize As Integer = 4
			Dim indexSize As Integer = 4
			Dim numChunksSize As Integer = 4
			Dim chunkSizeSize As Integer = 4
			Dim idLengthSize As Integer = 4
			Return idLengthSize + messageTypeSize + indexSize + chunkSizeSize + numChunksSize + chunk.getData().limit() + chunk.getId().getBytes().length

		End Function

		''' <summary>
		''' Convert an ndarray message chunk to a buffer. </summary>
		''' <param name="chunk"> the chunk to convert </param>
		''' <returns> an <seealso cref="ByteBuffer"/> based on the
		''' passed in message chunk. </returns>
		Public Shared Function toBuffer(ByVal chunk As NDArrayMessageChunk) As ByteBuffer
			Dim ret As ByteBuffer = ByteBuffer.allocateDirect(sizeForMessage(chunk)).order(ByteOrder.nativeOrder())
			'the messages opType enum as an int
			ret.putInt(chunk.getMessageType().ordinal())
			'the number of chunks this chunk is apart of
			ret.putInt(chunk.getNumChunks())
			'the chunk size
			ret.putInt(chunk.getChunkSize())
			'the length of the id (for self describing purposes)
			ret.putInt(chunk.getId().getBytes().length)
			' the actual id as a string
			ret.put(chunk.getId().getBytes())
			'the chunk index
			ret.putInt(chunk.getChunkIndex())
			'the actual data
			ret.put(chunk.getData())
			Return ret
		End Function

		''' <summary>
		''' Returns a chunk given the passed in <seealso cref="ByteBuffer"/>
		''' NOTE THAT THIS WILL MODIFY THE PASSED IN BYTEBUFFER's POSITION.
		''' </summary>
		''' <param name="byteBuffer"> the byte buffer to extract the chunk from </param>
		''' <returns> the ndarray message chunk based on the passed in <seealso cref="ByteBuffer"/> </returns>
		Public Shared Function fromBuffer(ByVal byteBuffer As ByteBuffer, ByVal type As NDArrayMessage.MessageType) As NDArrayMessageChunk
			Dim numChunks As Integer = byteBuffer.getInt()
			Dim chunkSize As Integer = byteBuffer.getInt()
			Dim idLength As Integer = byteBuffer.getInt()
			Dim id(idLength - 1) As SByte
			byteBuffer.get(id)
			Dim idString As String = StringHelper.NewString(id)
			Dim index As Integer = byteBuffer.getInt()
			Dim firstData As ByteBuffer = byteBuffer.slice()
			Dim chunk As NDArrayMessageChunk = NDArrayMessageChunk.builder().chunkSize(chunkSize).numChunks(numChunks).data(firstData).messageType(type).id(idString).chunkIndex(index).build()
			Return chunk

		End Function

	End Class

End Namespace