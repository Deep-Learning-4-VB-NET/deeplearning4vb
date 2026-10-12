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

Namespace org.nd4j.aeron.util



	''' <summary>
	''' Minor <seealso cref="ByteBuffer"/> utils
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class BufferUtil
		''' <summary>
		''' Merge all byte buffers together </summary>
		''' <param name="buffers"> the bytebuffers to merge </param>
		''' <param name="overAllCapacity"> the capacity of the
		'''                        merged bytebuffer </param>
		''' <returns> the merged byte buffer
		'''  </returns>
		Public Shared Function concat(ByVal buffers() As ByteBuffer, ByVal overAllCapacity As Integer) As ByteBuffer
			Dim all As ByteBuffer = ByteBuffer.allocateDirect(overAllCapacity)
			For i As Integer = 0 To buffers.Length - 1
				Dim curr As ByteBuffer = buffers(i).slice()
				all.put(curr)
			Next i
			Dim buffer As Buffer = CType(all, Buffer)
			buffer.rewind()
			Return all
		End Function

		''' <summary>
		''' Merge all bytebuffers together </summary>
		''' <param name="buffers"> the bytebuffers to merge </param>
		''' <returns> the merged bytebuffer </returns>
		Public Shared Function concat(ByVal buffers() As ByteBuffer) As ByteBuffer
			Dim overAllCapacity As Integer = 0
			For i As Integer = 0 To buffers.Length - 1
				overAllCapacity += buffers(i).limit() - buffers(i).position()
			Next i
			'padding
			overAllCapacity += buffers(0).limit() - buffers(0).position()
			Dim all As ByteBuffer = ByteBuffer.allocateDirect(overAllCapacity)
			For i As Integer = 0 To buffers.Length - 1
				Dim curr As ByteBuffer = buffers(i)
				all.put(curr)
			Next i

			Dim buffer As Buffer = CType(all, Buffer)
			buffer.flip()
			Return all
		End Function

	End Class

End Namespace