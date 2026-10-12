Imports System
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer

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
	''' A Jcuda buffer
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Interface JCudaBuffer
		Inherits DataBuffer

		''' <summary>
		''' Get the underlying host bytebuffer
		''' @return
		''' </summary>
		<Obsolete>
		ReadOnly Property HostBuffer As Buffer

		''' <summary>
		''' THe pointer for the buffer
		''' </summary>
		''' <returns> the pointer for this buffer </returns>
		<Obsolete>
		ReadOnly Property HostPointer As Pointer

		''' <summary>
		''' Get the host pointer with the given offset
		''' note that this will automatically
		''' multiply the specified offset
		''' by the element size </summary>
		''' <param name="offset"> the offset (NOT MULTIPLIED BY ELEMENT SIZE) to index in to the pointer at </param>
		''' <returns> the pointer at the given byte offset </returns>
		<Obsolete>
		Function getHostPointer(ByVal offset As Long) As Pointer
	End Interface

End Namespace