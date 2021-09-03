Imports System
Imports System.IO
Imports ByteArrayOutputStream = org.apache.commons.io.output.ByteArrayOutputStream
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports BaseDataBuffer = org.nd4j.linalg.api.buffer.BaseDataBuffer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeEx = org.nd4j.linalg.api.buffer.DataTypeEx
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports CompressionDescriptor = org.nd4j.linalg.compression.CompressionDescriptor
Imports CompressionType = org.nd4j.linalg.compression.CompressionType
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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


	Public Class Gzip
		Inherits AbstractCompressor

		''' <summary>
		''' This method returns compression descriptor. It should be unique for any compressor implementation
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property Descriptor As String
			Get
				Return "GZIP"
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

		Public Overrides Function decompress(ByVal buffer As DataBuffer, ByVal dataType As DataType) As DataBuffer
			Try

				Dim compressed As CompressedDataBuffer = DirectCast(buffer, CompressedDataBuffer)
				Dim descriptor As CompressionDescriptor = compressed.getCompressionDescriptor()

				Dim pointer As BytePointer = CType(compressed.addressPointer(), BytePointer)
				Dim bis As New MemoryStream(pointer.getStringBytes())
				Dim gzip As New GZIPInputStream(bis)
				Dim dis As New DataInputStream(gzip)

				Dim bufferRestored As DataBuffer = Nd4j.createBuffer(dataType, descriptor.getNumberOfElements(), False)
				BaseDataBuffer.readHeader(dis)
				bufferRestored.read(dis, DataBuffer.AllocationMode.MIXED_DATA_TYPES, descriptor.getNumberOfElements(), dataType)

				Return bufferRestored
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Public Overrides Function compress(ByVal buffer As DataBuffer) As DataBuffer
			Try
				Dim stream As New ByteArrayOutputStream()
				Dim gzip As New GZIPOutputStream(stream)
				Dim dos As New DataOutputStream(gzip)

				buffer.write(dos)
				dos.flush()
				dos.close()

				Dim bytes() As SByte = stream.toByteArray()
				'            logger.info("Bytes: {}", Arrays.toString(bytes));
				Dim pointer As New BytePointer(bytes)
				Dim descriptor As New CompressionDescriptor(buffer, Me)
				descriptor.setCompressedLength(bytes.Length)

				Dim result As New CompressedDataBuffer(pointer, descriptor)

				Return result
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Protected Friend Overrides Function compressPointer(ByVal srcType As DataTypeEx, ByVal srcPointer As Pointer, ByVal length As Integer, ByVal elementSize As Integer) As CompressedDataBuffer
			Throw New System.NotSupportedException("Not implemented yet")
		End Function
	End Class

End Namespace