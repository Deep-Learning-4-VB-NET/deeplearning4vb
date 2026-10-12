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

Namespace org.nd4j.parameterserver.distributed.v2.chunks

	Public Interface ChunksTracker(Of T As org.nd4j.parameterserver.distributed.v2.messages.VoidMessage)

		''' <summary>
		''' This message returns ID of the original message we're tracking here
		''' @return
		''' </summary>
		ReadOnly Property OriginId As String

		''' <summary>
		''' This method checks if all chunks were received </summary>
		''' <returns> true if all chunks were received, false otherwise </returns>
		ReadOnly Property Complete As Boolean

		''' <summary>
		''' This message appends chunk to this tracker </summary>
		''' <param name="chunk"> Chunk to be added </param>
		''' <returns> true if that was last chunk, false otherwise </returns>
		Function append(ByVal chunk As VoidChunk) As Boolean

		''' <summary>
		''' This method returns original message
		''' @return
		''' </summary>
		ReadOnly Property Message As T

		''' <summary>
		''' This method releases all resources used (if used) for this message
		''' </summary>
		Sub release()

		''' <summary>
		''' This method returns amount of bytes reserved for final message
		''' 
		''' @return
		''' </summary>
		Function size() As Long
	End Interface

End Namespace