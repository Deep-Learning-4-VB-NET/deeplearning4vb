Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils
Imports org.nd4j.parameterserver.distributed.v2.chunks
Imports VoidChunk = org.nd4j.parameterserver.distributed.v2.chunks.VoidChunk
Imports VoidMessage = org.nd4j.parameterserver.distributed.v2.messages.VoidMessage

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

Namespace org.nd4j.parameterserver.distributed.v2.chunks.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class InmemoryChunksTracker<T extends org.nd4j.parameterserver.distributed.v2.messages.VoidMessage> implements org.nd4j.parameterserver.distributed.v2.chunks.ChunksTracker<T>
	Public Class InmemoryChunksTracker(Of T As org.nd4j.parameterserver.distributed.v2.messages.VoidMessage)
		Implements ChunksTracker(Of T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final String originId;
		Private ReadOnly originId As String

		Private ReadOnly numChunks As Integer

		Private map As IDictionary(Of Integer, AtomicBoolean) = New ConcurrentDictionary(Of Integer, AtomicBoolean)()

		Private ReadOnly buffer() As SByte

'JAVA TO VB CONVERTER NOTE: The field size was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly size_Conflict As Long


		Public Sub New(ByVal chunk As VoidChunk)
			originId = chunk.getOriginalId()
			numChunks = chunk.getNumberOfChunks()

			If chunk.getTotalSize() > Integer.MaxValue Then
				Throw New ND4JIllegalStateException("Total message size > Integer.MAX_VALUE")
			End If

			size_Conflict = chunk.getTotalSize()

			Try
				buffer = New SByte(CInt(size_Conflict) - 1){}

				' we'll pre-initialize states map
				For e As Integer = 0 To numChunks - 1
					map(e) = New AtomicBoolean(False)
				Next e
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub

		Public Overridable Function size() As Long Implements ChunksTracker(Of T).size
			Return size_Conflict
		End Function

		Public Overridable ReadOnly Property Complete As Boolean Implements ChunksTracker(Of T).isComplete
			Get
				For Each b As val In map.Values
					If Not b.get() Then
						Return False
					End If
				Next b
    
				Return True
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public synchronized boolean append(@NonNull VoidChunk chunk)
		Public Overridable Function append(ByVal chunk As VoidChunk) As Boolean Implements ChunksTracker(Of T).append
			SyncLock Me
				Dim b As val = map(chunk.getChunkId())
        
				If b.get() Then
					Return Complete
				End If
        
				Dim offset As val = chunk.getChunkId() * chunk.getSplitSize()
        
				Dim cnt As Integer = 0
				Dim e As Integer = offset
				Do While e < offset + chunk.getPayload().length
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: buffer[e] = chunk.getPayload()[cnt++];
					buffer(e) = chunk.getPayload()(cnt)
						cnt += 1
					e += 1
				Loop
        
				' tagging this chunk as received
				b.set(True)
        
				Return Complete
			End SyncLock
		End Function

		Public Overridable ReadOnly Property Message As T Implements ChunksTracker(Of T).getMessage
			Get
				If Not Complete Then
					Throw New ND4JIllegalStateException("Message isn't ready for concatenation")
				End If
    
				Try
						Using bais As val = New MemoryStream(buffer)
						Return SerializationUtils.deserialize(bais)
						End Using
				Catch e As Exception
					log.error("Exception: {}",e)
					Throw New Exception(e)
				End Try
			End Get
		End Property

		Public Overridable Sub release() Implements ChunksTracker(Of T).release
			'
		End Sub
	End Class

End Namespace