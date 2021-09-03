Imports Pointer = org.bytedeco.javacpp.Pointer
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext

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

Namespace org.nd4j.linalg.jcublas.buffer


	''' <summary>
	''' Address retriever
	''' for a data buffer (both on host and device)
	''' </summary>
	Public Class AddressRetriever
		Private Shared ReadOnly allocator As AtomicAllocator = AtomicAllocator.Instance

		''' <summary>
		''' Retrieves the device pointer
		''' for the given data buffer </summary>
		''' <param name="buffer"> the buffer to get the device
		'''               address for </param>
		''' <returns> the device address for the given
		''' data buffer </returns>
		Public Shared Function retrieveDeviceAddress(ByVal buffer As DataBuffer, ByVal context As CudaContext) As Long
			Return allocator.getPointer(buffer, context).address()
		End Function


		''' <summary>
		''' Returns the host address </summary>
		''' <param name="buffer">
		''' @return </param>
		Public Shared Function retrieveHostAddress(ByVal buffer As DataBuffer) As Long
			Return allocator.getHostPointer(buffer).address()
		End Function

		''' <summary>
		''' Retrieves the device pointer
		''' for the given data buffer </summary>
		''' <param name="buffer"> the buffer to get the device
		'''               address for </param>
		''' <returns> the device pointer for the given
		''' data buffer </returns>
		Public Shared Function retrieveDevicePointer(ByVal buffer As DataBuffer, ByVal context As CudaContext) As Pointer
			Return allocator.getPointer(buffer, context)
		End Function


		''' <summary>
		''' Returns the host pointer </summary>
		''' <param name="buffer">
		''' @return </param>
		Public Shared Function retrieveHostPointer(ByVal buffer As DataBuffer) As Pointer
			Return allocator.getHostPointer(buffer)
		End Function
	End Class

End Namespace