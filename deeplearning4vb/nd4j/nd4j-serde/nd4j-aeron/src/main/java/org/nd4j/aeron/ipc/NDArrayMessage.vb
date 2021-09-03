Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports DirectBuffer = org.agrona.DirectBuffer
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports org.nd4j.common.primitives
Imports NDArrayMessageChunk = org.nd4j.aeron.ipc.chunk.NDArrayMessageChunk
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.aeron.ipc


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Builder @AllArgsConstructor @NoArgsConstructor public class NDArrayMessage implements java.io.Serializable
	<Serializable>
	Public Class NDArrayMessage
		Private arr As INDArray
		Private sent As Long
		Private index As Long
		Private dimensions() As Integer
		Private chunk() As SByte
		Private numChunks As Integer = 0
		'default dimensions: a 1 length array of -1 means use the whole array for an update.
		Private Shared WHOLE_ARRAY_UPDATE() As Integer = {-1}
		'represents the constant for indicating using the whole array for an update (-1)
		Private Shared WHOLE_ARRAY_INDEX As Integer = -1

		Public Enum MessageValidity
			VALID
			NULL_VALUE
			INCONSISTENT_DIMENSIONS
		End Enum

		Public Enum MessageType
			CHUNKED
			WHOLE
		End Enum

		''' <summary>
		''' Determine the number of chunks </summary>
		''' <param name="message"> </param>
		''' <param name="chunkSize">
		''' @return </param>
		Public Shared Function numChunksForMessage(ByVal message As NDArrayMessage, ByVal chunkSize As Integer) As Integer
			Dim sizeOfMessage As Integer = NDArrayMessage.byteBufferSizeForMessage(message)
			Dim numMessages As Integer = sizeOfMessage \ chunkSize
			'increase by 1 for padding
			If numMessages * chunkSize < sizeOfMessage Then
				numMessages += 1
			End If
			Return numMessages
		End Function

		''' <summary>
		''' Create an array of messages to send
		''' based on a specified chunk size </summary>
		''' <param name="arrayMessage"> </param>
		''' <param name="chunkSize">
		''' @return </param>
		Public Shared Function chunkedMessages(ByVal arrayMessage As NDArrayMessage, ByVal chunkSize As Integer) As NDArrayMessage()
			Dim sizeOfMessage As Integer = NDArrayMessage.byteBufferSizeForMessage(arrayMessage) - 4
			Dim numMessages As Integer = sizeOfMessage \ chunkSize
			Dim direct As ByteBuffer = NDArrayMessage.toBuffer(arrayMessage).byteBuffer()
			Dim ret(numMessages - 1) As NDArrayMessage
			Dim i As Integer = 0
			Do While i < numMessages
				Dim chunk(chunkSize - 1) As SByte
				direct.get(chunk, i * chunkSize, chunkSize)
				ret(i) = NDArrayMessage.builder().chunk(chunk).numChunks(numMessages).build()
				i += 1
			Loop
			Return ret
		End Function

		''' <summary>
		''' Prepare a whole array update
		''' which includes the default dimensions
		''' for indicating updating
		''' the whole array (a 1 length int array with -1 as its only element)
		''' -1 representing the dimension </summary>
		''' <param name="arr">
		''' @return </param>
		Public Shared Function wholeArrayUpdate(ByVal arr As INDArray) As NDArrayMessage
			Return NDArrayMessage.builder().arr(arr).dimensions(WHOLE_ARRAY_UPDATE).index(WHOLE_ARRAY_INDEX).sent(CurrentTimeUtc).build()
		End Function

		''' <summary>
		''' Factory method for creating an array
		''' to send now (uses now in utc for the timestamp).
		''' Note that this method will throw an
		''' <seealso cref="System.ArgumentException"/> if an invalid message is passed in.
		''' An invalid message is as follows:
		''' An index of -1 and dimensions that are of greater length than 1 with an element that isn't -1
		''' </summary>
		''' <param name="arr"> the array to send </param>
		''' <param name="dimensions"> the dimensions to use </param>
		''' <param name="index"> the index to use </param>
		''' <returns> the created </returns>
		Public Shared Function [of](ByVal arr As INDArray, ByVal dimensions() As Integer, ByVal index As Long) As NDArrayMessage
			'allow null dimensions as long as index is -1
			If dimensions Is Nothing Then
				dimensions = WHOLE_ARRAY_UPDATE
			End If

			'validate index being built
			If index > 0 Then
				If dimensions.Length > 1 OrElse dimensions.Length = 1 AndAlso dimensions(0) <> -1 Then
					Throw New System.ArgumentException("Inconsistent message. Your index is > 0 indicating you want to send a whole ndarray message but your dimensions indicate you are trying to send a partial update. Please ensure you use a 1 length int array with negative 1 as an element or use NDArrayMesage.wholeArrayUpdate(ndarray) for creation instead")
				End If
			End If

			Return NDArrayMessage.builder().index(index).dimensions(dimensions).sent(CurrentTimeUtc).arr(arr).build()
		End Function


		''' <summary>
		''' Returns if a message is valid or not based on a few simple conditions:
		''' no null values
		''' both index and the dimensions array must be -1 and of length 1 with an element of -1 in it
		''' otherwise it is a valid message. </summary>
		''' <param name="message"> the message to validate </param>
		''' <returns> 1 of: NULL_VALUE,INCONSISTENT_DIMENSIONS,VALID see <seealso cref="MessageValidity"/> </returns>
		Public Shared Function validMessage(ByVal message As NDArrayMessage) As MessageValidity
			If message.getDimensions() Is Nothing OrElse message.getArr() Is Nothing Then
				Return MessageValidity.NULL_VALUE
			End If

			If message.getIndex() <> -1 AndAlso message.getDimensions().length = 1 AndAlso message.getDimensions()(0) <> -1 Then
				Return MessageValidity.INCONSISTENT_DIMENSIONS
			End If
			Return MessageValidity.VALID
		End Function


		''' <summary>
		''' Get the current time in utc in milliseconds </summary>
		''' <returns> the current time in utc in
		''' milliseconds </returns>
		Public Shared ReadOnly Property CurrentTimeUtc As Long
			Get
				Dim instant As Instant = Instant.now()
				Dim dateTime As ZonedDateTime = instant.atZone(ZoneOffset.UTC)
				Return dateTime.toInstant().toEpochMilli()
			End Get
		End Property

		''' <summary>
		''' Returns the size needed in bytes
		''' for a bytebuffer for a given ndarray message.
		''' The formula is:
		''' <seealso cref="AeronNDArraySerde.byteBufferSizeFor(INDArray)"/>
		''' + size of dimension length (4)
		''' + time stamp size (8)
		''' + index size (8)
		''' + 4 * message.getDimensions.length </summary>
		''' <param name="message"> the message to get the length for </param>
		''' <returns> the size of the byte buffer for a message </returns>
		Public Shared Function byteBufferSizeForMessage(ByVal message As NDArrayMessage) As Integer
			Dim enumSize As Integer = 4
			Dim nInts As Integer = 4 * message.getDimensions().length
			Dim sizeofDimensionLength As Integer = 4
			Dim timeStampSize As Integer = 8
			Dim indexSize As Integer = 8
			Return enumSize + nInts + sizeofDimensionLength + timeStampSize + indexSize + AeronNDArraySerde.byteBufferSizeFor(message.getArr())
		End Function


		''' 
		''' <summary>
		''' Create an ndarray message from an array of buffers.
		''' This array of buffers would be assembled by an
		''' <seealso cref="io.aeron.logbuffer.FragmentHandler"/>
		''' capable of merging these messages together.
		''' Typically what happens is an <seealso cref="AeronNDArraySubscriber"/>
		''' will track chunks being sent.
		''' 
		''' Anytime a subscriber received an <seealso cref="MessageType.CHUNKED"/>
		''' as a opType it will store the buffer temporarily.
		''' </summary>
		''' <param name="chunks">
		''' @return </param>
		Public Shared Function fromChunks(ByVal chunks() As NDArrayMessageChunk) As NDArrayMessage
			Dim overAllCapacity As Integer = chunks(0).getChunkSize() * chunks.Length

			Dim all As ByteBuffer = ByteBuffer.allocateDirect(overAllCapacity).order(ByteOrder.nativeOrder())
			For i As Integer = 0 To chunks.Length - 1
				Dim curr As ByteBuffer = chunks(i).getData()
				If curr.capacity() > chunks(0).getChunkSize() Then
					CType(curr, Buffer).position(0).limit(chunks(0).getChunkSize())
					curr = curr.slice()
				End If
				all.put(curr)
			Next i

			'create an ndarray message from the given buffer
			Dim unsafeBuffer As New UnsafeBuffer(all)
			'rewind the buffer
			all.rewind()
			Return NDArrayMessage.fromBuffer(unsafeBuffer, 0)
		End Function



		''' <summary>
		''' Returns an array of
		''' message chunks meant to be sent
		''' in parallel.
		''' Each message chunk has the layout:
		''' messageType
		''' number of chunks
		''' chunkSize
		''' length of uuid
		''' uuid
		''' buffer index
		''' actual raw data </summary>
		''' <param name="message"> the message to turn into chunks </param>
		''' <param name="chunkSize"> the chunk size </param>
		''' <returns> an array of buffers </returns>
		Public Shared Function chunks(ByVal message As NDArrayMessage, ByVal chunkSize As Integer) As NDArrayMessageChunk()
			Dim numChunks As Integer = numChunksForMessage(message, chunkSize)
			Dim ret(numChunks - 1) As NDArrayMessageChunk
			Dim wholeBuffer As DirectBuffer = NDArrayMessage.toBuffer(message)
			Dim messageId As String = System.Guid.randomUUID().ToString()
			For i As Integer = 0 To ret.Length - 1
				'data: only grab a chunk of the data
				Dim view As ByteBuffer = CType(wholeBuffer.byteBuffer().asReadOnlyBuffer().position(i * chunkSize), ByteBuffer)
				view.limit(Math.Min(i * chunkSize + chunkSize, wholeBuffer.capacity()))
				view.order(ByteOrder.nativeOrder())
				view = view.slice()
				Dim chunk As NDArrayMessageChunk = NDArrayMessageChunk.builder().id(messageId).chunkSize(chunkSize).numChunks(numChunks).messageType(MessageType.CHUNKED).chunkIndex(i).data(view).build()
				'insert in to the array itself
				ret(i) = chunk
			Next i

			Return ret
		End Function

		''' <summary>
		''' Convert a message to a direct buffer.
		''' See <seealso cref="NDArrayMessage.fromBuffer(DirectBuffer, Integer)"/>
		''' for a description of the format for the buffer </summary>
		''' <param name="message"> the message to convert </param>
		''' <returns> a direct byte buffer representing this message. </returns>
		Public Shared Function toBuffer(ByVal message As NDArrayMessage) As DirectBuffer
			Dim byteBuffer As ByteBuffer = ByteBuffer.allocateDirect(byteBufferSizeForMessage(message)).order(ByteOrder.nativeOrder())
			'declare message opType
			byteBuffer.putInt(MessageType.WHOLE.ordinal())
			'perform the ndarray put on the
			If message.getArr().isCompressed() Then
				AeronNDArraySerde.doByteBufferPutCompressed(message.getArr(), byteBuffer, False)
			Else
				AeronNDArraySerde.doByteBufferPutUnCompressed(message.getArr(), byteBuffer, False)
			End If

			Dim sent As Long = message.getSent()
			Dim index As Long = message.getIndex()

			byteBuffer.putLong(sent)
			byteBuffer.putLong(index)
			byteBuffer.putInt(message.getDimensions().length)
			Dim i As Integer = 0
			Do While i < message.getDimensions().length
				byteBuffer.putInt(message.getDimensions()(i))
				i += 1
			Loop

			'rewind the buffer before putting it in to the unsafe buffer
			'note that we set rewind to false in the do byte buffer put methods
			CType(byteBuffer, Buffer).rewind()

			Return New UnsafeBuffer(byteBuffer)
		End Function

		''' <summary>
		''' Convert a direct buffer to an ndarray
		''' message.
		''' The format of the byte buffer is:
		''' ndarray
		''' time
		''' index
		''' dimension length
		''' dimensions
		''' 
		''' We use <seealso cref="AeronNDArraySerde.toArrayAndByteBuffer(DirectBuffer, Integer)"/>
		''' to read in the ndarray and just use normal <seealso cref="ByteBuffer.getInt()"/> and
		''' <seealso cref="ByteBuffer.getLong()"/> to get the things like dimensions and index
		''' and time stamp.
		''' 
		''' 
		''' </summary>
		''' <param name="buffer"> the buffer to convert </param>
		''' <param name="offset">  the offset to start at with the buffer - note that this
		'''                method call assumes that the message opType is specified at the beginning of the buffer.
		'''                This means whatever offset you pass in will be increased by 4 (the size of an int) </param>
		''' <returns> the ndarray message based on this direct buffer. </returns>
		Public Shared Function fromBuffer(ByVal buffer As DirectBuffer, ByVal offset As Integer) As NDArrayMessage
			'skip the message opType
			Dim pair As Pair(Of INDArray, ByteBuffer) = AeronNDArraySerde.toArrayAndByteBuffer(buffer, offset + 4)
			Dim arr As INDArray = pair.getKey()
			Nd4j.Compressor.decompressi(arr)
			'use the rest of the buffer, of note here the offset is already set, we should only need to use
			Dim rest As ByteBuffer = pair.Right
			Dim time As Long = rest.getLong()
			Dim index As Long = rest.getLong()
			'get the array next for dimensions
			Dim dimensionLength As Integer = rest.getInt()
			If dimensionLength <= 0 Then
				Throw New System.ArgumentException("Invalid dimension length " & dimensionLength)
			End If
			Dim dimensions(dimensionLength - 1) As Integer
			For i As Integer = 0 To dimensionLength - 1
				dimensions(i) = rest.getInt()
			Next i
			Return NDArrayMessage.builder().sent(time).arr(arr).index(index).dimensions(dimensions).build()
		End Function

	End Class

End Namespace