Imports System
Imports Data = lombok.Data
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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

Namespace org.nd4j.linalg.compression


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class CompressionDescriptor implements Cloneable, java.io.Serializable
	<Serializable>
	Public Class CompressionDescriptor
		Implements ICloneable

		Private compressionType As CompressionType
		Private compressionAlgorithm As String
		Private originalLength As Long
		Private compressedLength As Long
		Private numberOfElements As Long
		Private originalElementSize As Long
		Private originalDataType As DataType

		'40 bytes for the compression descriptor bytebuffer
		Public Const COMPRESSION_BYTE_BUFFER_LENGTH As Integer = 40

		Public Sub New()

		End Sub

		''' <summary>
		''' Create a  compression descriptor from the given
		''' data buffer elements </summary>
		''' <param name="buffer"> the databuffer to create
		'''               the compression descriptor from </param>
		Public Sub New(ByVal buffer As DataBuffer)
			Me.originalLength = buffer.length() * buffer.ElementSize
			Me.numberOfElements = buffer.length()
			Me.originalElementSize = buffer.ElementSize
			Me.originalDataType = buffer.dataType()
		End Sub

		''' <summary>
		''' Initialize a compression descriptor
		''' based on the given algorithm and data buffer </summary>
		''' <param name="buffer"> the data buffer to base the sizes off of </param>
		''' <param name="algorithm"> the algorithm used
		'''                  in the descriptor </param>
		Public Sub New(ByVal buffer As DataBuffer, ByVal algorithm As String)
			Me.New(buffer)
			Me.compressionAlgorithm = algorithm
		End Sub

		''' <summary>
		''' Initialize a compression descriptor
		''' based on the given data buffer (for the sizes)
		''' and the compressor to get the opType </summary>
		''' <param name="buffer"> </param>
		''' <param name="compressor"> </param>
		Public Sub New(ByVal buffer As DataBuffer, ByVal compressor As NDArrayCompressor)
			Me.New(buffer, compressor.Descriptor)
			Me.compressionType = compressor.CompressionType
		End Sub


		''' <summary>
		''' Instantiate a compression descriptor from
		''' the given bytebuffer </summary>
		''' <param name="byteBuffer"> the bytebuffer to instantiate
		'''                   the descriptor from </param>
		''' <returns> the instantiated descriptor based on the given
		''' bytebuffer </returns>
		Public Shared Function fromByteBuffer(ByVal byteBuffer As ByteBuffer) As CompressionDescriptor
'JAVA TO VB CONVERTER NOTE: The variable compressionDescriptor was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
			Dim compressionDescriptor_Conflict As New CompressionDescriptor()
			'compression opType
			Dim compressionTypeOrdinal As Integer = byteBuffer.getInt()
			Dim compressionType As CompressionType = System.Enum.GetValues(GetType(CompressionType))(compressionTypeOrdinal)
			compressionDescriptor_Conflict.setCompressionType(compressionType)

			'compression algo
			Dim compressionAlgoOrdinal As Integer = byteBuffer.getInt()
			Dim compressionAlgorithm As CompressionAlgorithm = CompressionAlgorithm.values()(compressionAlgoOrdinal)
			compressionDescriptor_Conflict.setCompressionAlgorithm(compressionAlgorithm.ToString())
			'from here everything is longs
			compressionDescriptor_Conflict.setOriginalLength(byteBuffer.getLong())
			compressionDescriptor_Conflict.setCompressedLength(byteBuffer.getLong())
			compressionDescriptor_Conflict.setNumberOfElements(byteBuffer.getLong())
			compressionDescriptor_Conflict.setOriginalElementSize(byteBuffer.getLong())
			compressionDescriptor_Conflict.setOriginalDataType(DataType.values()(byteBuffer.getInt()))
			Return compressionDescriptor_Conflict
		End Function

		''' <summary>
		''' Return a direct allocated
		''' bytebuffer from the compression codec.
		''' The size of the bytebuffer is calculated to be:
		''' 40: 8 + 32
		''' two ints representing their enum values
		''' for the compression algorithm and opType
		''' 
		''' and 4 longs for the compressed and
		''' original sizes </summary>
		''' <returns> the bytebuffer described above </returns>
		Public Overridable Function toByteBuffer() As ByteBuffer
			'3 ints  at 4 bytes a piece, this includes the compression algorithm
			'that we convert to enum
			Dim enumSize As Integer = 3 * 4
			'4 longs at 8 bytes a piece
			Dim sizesLength As Integer = 4 * 8
			Dim directAlloc As ByteBuffer = ByteBuffer.allocateDirect(enumSize + sizesLength).order(ByteOrder.nativeOrder())
			directAlloc.putInt(compressionType.ordinal())
			directAlloc.putInt(CompressionAlgorithm.valueOf(compressionAlgorithm).ordinal())
			directAlloc.putLong(originalLength)
			directAlloc.putLong(compressedLength)
			directAlloc.putLong(numberOfElements)
			directAlloc.putLong(originalElementSize)
			directAlloc.putInt(originalDataType.ordinal())
			CType(directAlloc, Buffer).rewind()
			Return directAlloc
		End Function

		Public Overrides Function clone() As CompressionDescriptor
			Dim descriptor As New CompressionDescriptor()
			descriptor.compressionType = Me.compressionType
			descriptor.compressionAlgorithm = Me.compressionAlgorithm
			descriptor.originalLength = Me.originalLength
			descriptor.compressedLength = Me.compressedLength
			descriptor.numberOfElements = Me.numberOfElements
			descriptor.originalElementSize = Me.originalElementSize
			descriptor.originalDataType = Me.originalDataType

			Return descriptor
		End Function
	End Class

End Namespace