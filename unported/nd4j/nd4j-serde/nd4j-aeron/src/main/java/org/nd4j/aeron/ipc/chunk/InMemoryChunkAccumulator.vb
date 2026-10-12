Imports System.Collections.Generic
Imports Maps = org.nd4j.shade.guava.collect.Maps
Imports Slf4j = lombok.extern.slf4j.Slf4j
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
'ORIGINAL LINE: @Slf4j public class InMemoryChunkAccumulator implements ChunkAccumulator
	Public Class InMemoryChunkAccumulator
		Implements ChunkAccumulator

		Private chunks As IDictionary(Of String, IList(Of NDArrayMessageChunk)) = Maps.newConcurrentMap()

		''' <summary>
		''' Returns the number of chunks
		''' accumulated for a given id so far
		''' </summary>
		''' <param name="id"> the id to get the
		'''           number of chunks for </param>
		''' <returns> the number of chunks accumulated
		''' for a given id so far </returns>
		Public Overridable Function numChunksSoFar(ByVal id As String) As Integer Implements ChunkAccumulator.numChunksSoFar
			If Not chunks.ContainsKey(id) Then
				Return 0
			End If
			Return chunks(id).Count
		End Function

		''' <summary>
		''' Returns true if all chunks are present
		''' </summary>
		''' <param name="id"> the id to check for </param>
		''' <returns> true if all the chunks are present,false otherwise </returns>
		Public Overridable Function allPresent(ByVal id As String) As Boolean Implements ChunkAccumulator.allPresent
			If Not chunks.ContainsKey(id) Then
				Return False
			End If
			Dim chunkList As IList(Of NDArrayMessageChunk) = chunks(id)
			Return chunkList.Count = chunkList(0).getNumChunks()
		End Function

		''' <summary>
		''' Reassemble an ndarray message
		''' from a set of chunks
		''' 
		''' Note that once reassemble is called,
		''' the associated chunk lists will automatically
		''' be removed from storage.
		''' 
		''' </summary>
		''' <param name="id"> the id to reassemble </param>
		''' <returns> the reassembled message </returns>
		Public Overridable Function reassemble(ByVal id As String) As NDArrayMessage Implements ChunkAccumulator.reassemble
			Dim chunkList As IList(Of NDArrayMessageChunk) = chunks(id)
			If chunkList.Count <> chunkList(0).getNumChunks() Then
				Throw New System.InvalidOperationException("Unable to reassemble message chunk " & id & " missing " & (chunkList(0).getNumChunks() - chunkList.Count) & "chunks")
			End If
			'ensure the chunks are in contiguous ordering according to their chunk index
			Dim inOrder(chunkList.Count - 1) As NDArrayMessageChunk
			For Each chunk As NDArrayMessageChunk In chunkList
				inOrder(chunk.getChunkIndex()) = chunk
			Next chunk

			'reassemble the in order chunks
			Dim message As NDArrayMessage = NDArrayMessage.fromChunks(inOrder)
			chunkList.Clear()
			chunks.Remove(id)
			Return message

		End Function

		''' <summary>
		''' Accumulate chunks in a map
		''' until all chunks have been accumulated.
		''' You can check all chunks are present with
		''' <seealso cref="ChunkAccumulator.allPresent(String)"/>
		''' where the parameter is the id
		''' After all chunks have been accumulated
		''' you can call <seealso cref="ChunkAccumulator.reassemble(String)"/>
		''' where the id is the id of the chunk. </summary>
		''' <param name="chunk"> the chunk </param>
		Public Overridable Sub accumulateChunk(ByVal chunk As NDArrayMessageChunk) Implements ChunkAccumulator.accumulateChunk
			Dim id As String = chunk.getId()
			If Not chunks.ContainsKey(id) Then
				Dim list As IList(Of NDArrayMessageChunk) = New List(Of NDArrayMessageChunk)()
				list.Add(chunk)
				chunks(id) = list
			Else
				Dim chunkList As IList(Of NDArrayMessageChunk) = chunks(id)
				chunkList.Add(chunk)
			End If

			log.debug("Accumulating chunk for id " & chunk.getId())


		End Sub

	End Class

End Namespace