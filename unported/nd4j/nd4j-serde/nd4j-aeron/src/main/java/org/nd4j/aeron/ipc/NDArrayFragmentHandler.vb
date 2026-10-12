Imports FragmentHandler = io.aeron.logbuffer.FragmentHandler
Imports Header = io.aeron.logbuffer.Header
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DirectBuffer = org.agrona.DirectBuffer
Imports ChunkAccumulator = org.nd4j.aeron.ipc.chunk.ChunkAccumulator
Imports InMemoryChunkAccumulator = org.nd4j.aeron.ipc.chunk.InMemoryChunkAccumulator
Imports NDArrayMessageChunk = org.nd4j.aeron.ipc.chunk.NDArrayMessageChunk

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



	''' <summary>
	''' NDArray fragment handler
	''' for listening to an aeron queue
	''' 
	''' @author Adam Gibson
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class NDArrayFragmentHandler implements io.aeron.logbuffer.FragmentHandler
	Public Class NDArrayFragmentHandler
		Implements FragmentHandler

		Private ndArrayCallback As NDArrayCallback
		Private chunkAccumulator As ChunkAccumulator = New InMemoryChunkAccumulator()

		Public Sub New(ByVal ndArrayCallback As NDArrayCallback)
			Me.ndArrayCallback = ndArrayCallback
		End Sub

		''' <summary>
		''' Callback for handling
		''' fragments of data being read from a log.
		''' </summary>
		''' <param name="buffer"> containing the data. </param>
		''' <param name="offset"> at which the data begins. </param>
		''' <param name="length"> of the data in bytes. </param>
		''' <param name="header"> representing the meta data for the data. </param>
		Public Overrides Sub onFragment(ByVal buffer As DirectBuffer, ByVal offset As Integer, ByVal length As Integer, ByVal header As Header)
			Dim byteBuffer As ByteBuffer = buffer.byteBuffer()
			Dim byteArrayInput As Boolean = False
			If byteBuffer Is Nothing Then
				byteArrayInput = True
				Dim destination(length - 1) As SByte
				Dim wrap As ByteBuffer = ByteBuffer.wrap(buffer.byteArray())
				wrap.get(destination, offset, length)
				byteBuffer = ByteBuffer.wrap(destination).order(ByteOrder.nativeOrder())
			End If


			'only applicable for direct buffers where we don't wrap the array
			If Not byteArrayInput Then
				byteBuffer.position(offset)
				byteBuffer.order(ByteOrder.nativeOrder())
			End If

			Dim messageTypeIndex As Integer = byteBuffer.getInt()
			If messageTypeIndex >= System.Enum.GetValues(GetType(NDArrayMessage.MessageType)).length Then
				Throw New System.InvalidOperationException("Illegal index on message opType. Likely corrupt message. Please check the serialization of the bytebuffer. Input was bytebuffer: " & byteArrayInput)
			End If
			Dim messageType As NDArrayMessage.MessageType = System.Enum.GetValues(GetType(NDArrayMessage.MessageType))(messageTypeIndex)

			If messageType = NDArrayMessage.MessageType.CHUNKED Then
				Dim chunk As NDArrayMessageChunk = NDArrayMessageChunk.fromBuffer(byteBuffer, messageType)
				If chunk.getNumChunks() < 1 Then
					Throw New System.InvalidOperationException("Found invalid number of chunks " & chunk.getNumChunks() & " on chunk index " & chunk.getChunkIndex())
				End If
				chunkAccumulator.accumulateChunk(chunk)
				log.info("Number of chunks " & chunk.getNumChunks() & " and number of chunks " & chunk.getNumChunks() & " for id " & chunk.getId() & " is " & chunkAccumulator.numChunksSoFar(chunk.getId()))

				If chunkAccumulator.allPresent(chunk.getId()) Then
					Dim message As NDArrayMessage = chunkAccumulator.reassemble(chunk.getId())
					ndArrayCallback.onNDArrayMessage(message)
				End If
			Else
				Dim message As NDArrayMessage = NDArrayMessage.fromBuffer(buffer, offset)
				ndArrayCallback.onNDArrayMessage(message)
			End If


		End Sub
	End Class

End Namespace