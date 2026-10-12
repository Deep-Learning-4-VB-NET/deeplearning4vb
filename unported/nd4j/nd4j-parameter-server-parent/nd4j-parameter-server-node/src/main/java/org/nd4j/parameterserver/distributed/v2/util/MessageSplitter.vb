Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports ND4JFileUtils = org.nd4j.common.util.ND4JFileUtils
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports org.nd4j.parameterserver.distributed.v2.chunks
Imports org.nd4j.parameterserver.distributed.v2.chunks.impl
Imports VoidChunk = org.nd4j.parameterserver.distributed.v2.chunks.VoidChunk
Imports org.nd4j.parameterserver.distributed.v2.chunks.impl
Imports VoidMessage = org.nd4j.parameterserver.distributed.v2.messages.VoidMessage
Imports org.nd4j.common.primitives

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

Namespace org.nd4j.parameterserver.distributed.v2.util


	Public Class MessageSplitter
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New MessageSplitter()

		Protected Friend trackers As IDictionary(Of String, ChunksTracker) = New ConcurrentDictionary(Of String, ChunksTracker)()

		' simple counter for memory used by all in-memory trackers
		Protected Friend ReadOnly memoryUse As New AtomicLong(0)

		Public Sub New()
			'
		End Sub

		''' <summary>
		''' This method returns shared instance of MessageSplitter
		''' 
		''' @return
		''' </summary>
		Public Shared ReadOnly Property Instance As MessageSplitter
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		''' <summary>
		''' This method splits VoidMessage into chunks, and returns them as Collection </summary>
		''' <param name="message">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public java.util.Collection<org.nd4j.parameterserver.distributed.v2.chunks.VoidChunk> split(@NonNull VoidMessage message, int maxBytes) throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Function split(ByVal message As VoidMessage, ByVal maxBytes As Integer) As ICollection(Of VoidChunk)
			If maxBytes <= 0 Then
				Throw New ND4JIllegalStateException("MaxBytes must be > 0")
			End If

			Dim tempFile As val = ND4JFileUtils.createTempFile("messageSplitter","temp")
			Dim result As val = New List(Of VoidChunk)()

			Using fos As lombok.val = New FileStream(tempFile, FileMode.Create, FileAccess.Write), bos As lombok.val = New BufferedOutputStream(fos)
				' serializing original message to disc
				SerializationUtils.serialize(message, fos)

				Dim length As val = tempFile.length()
				Dim numChunks As Integer = CInt(length / maxBytes + (If(length Mod maxBytes > 0, 1, 0)))
				Using fis As lombok.val = New FileStream(tempFile, FileMode.Open, FileAccess.Read), bis As lombok.val = New BufferedInputStream(fis)
					' now we'll be reading serialized message into
					Dim bytes As val = New SByte(maxBytes - 1){}
					Dim cnt As Integer = 0
					Dim id As Integer = 0

					Do While cnt < length
						Dim c As val = bis.read(bytes)

						Dim tmp As val = Arrays.CopyOf(bytes, c)

						' FIXME: we don't really want UUID used here, it's just a placeholder for now
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: lombok.val msg = org.nd4j.parameterserver.distributed.v2.chunks.VoidChunk.builder().messageId(java.util.UUID.randomUUID().toString()).originalId(message.getMessageId()).chunkId(id++).numberOfChunks(numChunks).splitSize(maxBytes).payload(tmp).totalSize(length).build();
						Dim msg As val = VoidChunk.builder().messageId(System.Guid.randomUUID().ToString()).originalId(message.getMessageId()).chunkId(id).numberOfChunks(numChunks).splitSize(maxBytes).payload(tmp).totalSize(length).build()
							id += 1

						result.add(msg)
						cnt += c
					Loop
				End Using
			End Using

			tempFile.delete()
			Return result
		End Function


		''' <summary>
		''' This method checks, if specified message Id is being tracked </summary>
		''' <param name="messageId"> </param>
		''' <returns> true if tracked, and false otherwise </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: boolean isTrackedMessage(@NonNull String messageId)
		Friend Overridable Function isTrackedMessage(ByVal messageId As String) As Boolean
			Return trackers.ContainsKey(messageId)
		End Function

		''' <summary>
		''' This method checks, if specified message is being tracked </summary>
		''' <param name="chunk"> </param>
		''' <returns> true if tracked, and false otherwise </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: boolean isTrackedMessage(@NonNull VoidChunk chunk)
		Friend Overridable Function isTrackedMessage(ByVal chunk As VoidChunk) As Boolean
			Return isTrackedMessage(chunk.getOriginalId())
		End Function


		''' <summary>
		''' This method tries to merge using files tracker
		''' </summary>
		''' <param name="chunk"> </param>
		''' @param <T>
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T extends org.nd4j.parameterserver.distributed.v2.messages.VoidMessage> org.nd4j.common.primitives.@Optional<T> merge(@NonNull VoidChunk chunk)
		Public Overridable Function merge(Of T As VoidMessage)(ByVal chunk As VoidChunk) As [Optional](Of T)
			Return merge(chunk, -1L)
		End Function

		''' <summary>
		''' This method removes specified messageId from tracking
		''' </summary>
		''' <param name="messageId"> </param>
		Public Overridable Sub release(ByVal messageId As String)
			'
		End Sub

		''' 
		''' <param name="chunk"> </param>
		''' <param name="memoryLimit"> </param>
		''' @param <T>
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public <T extends org.nd4j.parameterserver.distributed.v2.messages.VoidMessage> org.nd4j.common.primitives.@Optional<T> merge(@NonNull VoidChunk chunk, long memoryLimit)
		Public Overridable Function merge(Of T As VoidMessage)(ByVal chunk As VoidChunk, ByVal memoryLimit As Long) As [Optional](Of T)
			Dim originalId As val= chunk.getOriginalId()
			Dim checker As val = New AtomicBoolean(False)
			Dim tracker As ChunksTracker = Nothing

			If memoryUse.get() + chunk.getTotalSize() < memoryLimit Then
				tracker = New InmemoryChunksTracker(chunk)
				tracker = trackers.putIfAbsent(originalId, tracker)
				If tracker Is Nothing Then
					memoryUse.addAndGet(chunk.getTotalSize())
				End If
			Else
				tracker = New FileChunksTracker(chunk)
				tracker = trackers.putIfAbsent(originalId, tracker)
			End If

			If tracker Is Nothing Then
				tracker = trackers(chunk.getOriginalId())
			End If

			If tracker.append(chunk) Then
				Try
					Return [Optional].of(DirectCast(tracker.getMessage(), T))
				Finally
					' we should decrease  memory amouint
					If TypeOf tracker Is InmemoryChunksTracker Then
						memoryUse.addAndGet(-chunk.getTotalSize())
					End If

					tracker.release()

					trackers.Remove(chunk.getOriginalId())
				End Try
			Else
				Return [Optional].empty()
			End If
		End Function

		Public Overridable Sub reset()
			memoryUse.set(0)
			trackers.Clear()
		End Sub
	End Class

End Namespace