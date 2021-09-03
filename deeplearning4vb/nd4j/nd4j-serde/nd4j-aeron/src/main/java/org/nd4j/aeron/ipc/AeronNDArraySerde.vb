Imports DirectBuffer = org.agrona.DirectBuffer
Imports UnsafeBuffer = org.agrona.concurrent.UnsafeBuffer
Imports org.nd4j.common.primitives
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BinarySerde = org.nd4j.serde.binary.BinarySerde

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


	Public Class AeronNDArraySerde
		Inherits BinarySerde


		''' <summary>
		''' Get the direct byte buffer from the given direct buffer </summary>
		''' <param name="directBuffer">
		''' @return </param>
		Public Shared Function getDirectByteBuffer(ByVal directBuffer As DirectBuffer) As ByteBuffer
			Return If(directBuffer.byteBuffer() Is Nothing, ByteBuffer.allocateDirect(directBuffer.capacity()).put(directBuffer.byteArray()), directBuffer.byteBuffer())
		End Function

		''' <summary>
		''' Convert an ndarray to an unsafe buffer
		''' for use by aeron </summary>
		''' <param name="arr"> the array to convert </param>
		''' <returns> the unsafebuffer representation of this array </returns>
		Public Shared Function toBuffer(ByVal arr As INDArray) As UnsafeBuffer
			Return New UnsafeBuffer(toByteBuffer(arr))

		End Function



		''' <summary>
		''' Create an ndarray
		''' from the unsafe buffer.
		''' Note that if you are interacting with a buffer that specifies
		''' an <seealso cref="org.nd4j.aeron.ipc.NDArrayMessage.MessageType"/>
		''' then you must pass in an offset + 4.
		''' Adding 4 to the offset will cause the inter </summary>
		''' <param name="buffer"> the buffer to create the array from </param>
		''' <returns> the ndarray derived from this buffer </returns>
		Public Shared Function toArrayAndByteBuffer(ByVal buffer As DirectBuffer, ByVal offset As Integer) As Pair(Of INDArray, ByteBuffer)
			Return toArrayAndByteBuffer(getDirectByteBuffer(buffer), offset)
		End Function


		''' <summary>
		''' Create an ndarray
		''' from the unsafe buffer </summary>
		''' <param name="buffer"> the buffer to create the array from </param>
		''' <returns> the ndarray derived from this buffer </returns>
		Public Shared Function toArray(ByVal buffer As DirectBuffer, ByVal offset As Integer) As INDArray
			Return toArrayAndByteBuffer(buffer, offset).Left
		End Function

		''' <summary>
		''' Create an ndarray
		''' from the unsafe buffer </summary>
		''' <param name="buffer"> the buffer to create the array from </param>
		''' <returns> the ndarray derived from this buffer </returns>
		Public Shared Function toArray(ByVal buffer As DirectBuffer) As INDArray
			Return toArray(buffer, 0)
		End Function



	End Class

End Namespace