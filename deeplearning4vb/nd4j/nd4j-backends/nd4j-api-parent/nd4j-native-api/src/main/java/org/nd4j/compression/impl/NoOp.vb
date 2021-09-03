Imports val = lombok.val
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeEx = org.nd4j.linalg.api.buffer.DataTypeEx
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports CompressionDescriptor = org.nd4j.linalg.compression.CompressionDescriptor
Imports CompressionType = org.nd4j.linalg.compression.CompressionType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection

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

Namespace org.nd4j.compression.impl

	Public Class NoOp
		Inherits AbstractCompressor

		''' <summary>
		''' This method returns compression descriptor. It should be unique for any compressor implementation
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property Descriptor As String
			Get
				Return "NOOP"
			End Get
		End Property

		''' <summary>
		''' This method returns compression opType provided by specific NDArrayCompressor implementation
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property CompressionType As CompressionType
			Get
				Return CompressionType.LOSSLESS
			End Get
		End Property

		Public Overrides Function decompress(ByVal buffer As DataBuffer, ByVal type As DataType) As DataBuffer

			Dim comp As CompressedDataBuffer = DirectCast(buffer, CompressedDataBuffer)

			Dim result As DataBuffer = Nd4j.createBuffer(comp.length(), False)
			Nd4j.MemoryManager.memcpy(result, buffer)

			Return result
		End Function

		Public Overrides Function compress(ByVal buffer As DataBuffer) As DataBuffer

			Dim descriptor As New CompressionDescriptor(buffer, Me)

			Dim ptr As New BytePointer(buffer.length() * buffer.ElementSize)
			Dim result As New CompressedDataBuffer(ptr, descriptor)

			Nd4j.MemoryManager.memcpy(result, buffer)

			Return result
		End Function

		Protected Friend Overrides Function compressPointer(ByVal srcType As DataTypeEx, ByVal srcPointer As Pointer, ByVal length As Integer, ByVal elementSize As Integer) As CompressedDataBuffer

			Dim descriptor As New CompressionDescriptor()
			descriptor.setCompressionType(CompressionType)
			descriptor.setOriginalLength(length * elementSize)
			descriptor.setCompressionAlgorithm(Me.Descriptor)
			descriptor.setOriginalElementSize(elementSize)
			descriptor.setCompressedLength(length * elementSize)
			descriptor.setNumberOfElements(length)

			Dim ptr As New BytePointer(length * elementSize)

			Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

			' this Pointer.memcpy is used intentionally. This method operates on host memory ALWAYS
			Pointer.memcpy(ptr, srcPointer, length * elementSize)

			PerformanceTracker.Instance.helperRegisterTransaction(0, perfD, length * elementSize, MemcpyDirection.HOST_TO_HOST)

			Dim buffer As New CompressedDataBuffer(ptr, descriptor)

			Return buffer
		End Function
	End Class

End Namespace