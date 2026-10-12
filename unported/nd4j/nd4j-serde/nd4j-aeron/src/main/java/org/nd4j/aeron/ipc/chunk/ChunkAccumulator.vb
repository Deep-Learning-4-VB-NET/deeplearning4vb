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

	Public Interface ChunkAccumulator

		''' <summary>
		''' Returns the number of chunks
		''' accumulated for a given id so far </summary>
		''' <param name="id"> the id to get the
		'''           number of chunks for </param>
		''' <returns> the number of chunks accumulated
		''' for a given id so far </returns>
		Function numChunksSoFar(ByVal id As String) As Integer

		''' <summary>
		''' Returns true if all chunks are present </summary>
		''' <param name="id"> the id to check for </param>
		''' <returns> true if all the chunks are present,false otherwise </returns>
		Function allPresent(ByVal id As String) As Boolean

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
		Function reassemble(ByVal id As String) As NDArrayMessage

		''' <summary>
		''' Accumulate chunks </summary>
		''' <param name="chunk"> the chunk to accumulate </param>
		Sub accumulateChunk(ByVal chunk As NDArrayMessageChunk)
	End Interface

End Namespace