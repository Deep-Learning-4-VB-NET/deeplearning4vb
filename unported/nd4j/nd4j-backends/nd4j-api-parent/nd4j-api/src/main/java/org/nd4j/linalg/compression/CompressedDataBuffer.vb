Imports System
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports val = lombok.val
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Indexer = org.bytedeco.javacpp.indexer.Indexer
Imports BaseDataBuffer = org.nd4j.linalg.api.buffer.BaseDataBuffer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports PerformanceTracker = org.nd4j.linalg.api.ops.performance.PerformanceTracker
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MemcpyDirection = org.nd4j.linalg.api.memory.MemcpyDirection
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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


	<Serializable>
	Public Class CompressedDataBuffer
		Inherits BaseDataBuffer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected CompressionDescriptor compressionDescriptor;
		Protected Friend compressionDescriptor As CompressionDescriptor
		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(CompressedDataBuffer))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CompressedDataBuffer(org.bytedeco.javacpp.Pointer pointer, @NonNull CompressionDescriptor descriptor)
		Public Sub New(ByVal pointer As Pointer, ByVal descriptor As CompressionDescriptor)
			Me.compressionDescriptor = descriptor
			Me.pointer_Conflict = pointer
			Me.length_Conflict = descriptor.getNumberOfElements()
			Me.elementSize_Conflict = CSByte(Math.Truncate(descriptor.getOriginalElementSize()))

			initTypeAndSize()
		End Sub

		''' <summary>
		''' Initialize the opType of this buffer
		''' </summary>
		Protected Friend Overrides Sub initTypeAndSize()
			type = DataType.COMPRESSED
			allocationMode_Conflict = AllocationMode.MIXED_DATA_TYPES
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(java.io.DataOutputStream out) throws java.io.IOException
		Public Overrides Sub write(ByVal [out] As DataOutputStream)
			'        logger.info("Writing out CompressedDataBuffer");
			' here we should mimic to usual DataBuffer array
			[out].writeUTF(allocationMode_Conflict.ToString())
			[out].writeLong(compressionDescriptor.getCompressedLength())
			[out].writeUTF(DataType.COMPRESSED.ToString())
			' at this moment we don't care about mimics anymore
			'ByteIndexer indexer = ByteIndexer.create((BytePointer) pointer);
			[out].writeUTF(compressionDescriptor.getCompressionAlgorithm())
			[out].writeLong(compressionDescriptor.getCompressedLength())
			[out].writeLong(compressionDescriptor.getOriginalLength())
			[out].writeLong(compressionDescriptor.getNumberOfElements())
			[out].writeInt(compressionDescriptor.getOriginalDataType().ordinal())
			'        out.write(((BytePointer) pointer).getStringBytes());
			Dim x As Integer = 0
			Do While x < pointer_Conflict.capacity() * pointer_Conflict.sizeof()
				Dim b As SByte = pointer_Conflict.asByteBuffer().get(x)
				[out].writeByte(b)
				x += 1
			Loop
		End Sub

		Protected Friend Overrides WriteOnly Property Indexer As Indexer
			Set(ByVal indexer As Indexer)
				' no-op
			End Set
		End Property

		Public Overrides Function addressPointer() As Pointer
			Return pointer_Conflict
		End Function

		''' <summary>
		''' Drop-in replacement wrapper for BaseDataBuffer.read() method, aware of CompressedDataBuffer </summary>
		''' <param name="s">
		''' @return </param>
		Public Shared Function readUnknown(ByVal s As DataInputStream, ByVal allocMode As AllocationMode, ByVal length As Long, ByVal type As DataType) As DataBuffer
			' if buffer is uncompressed, it'll be valid buffer, so we'll just return it
			If type <> DataType.COMPRESSED Then
				Dim buffer As DataBuffer = Nd4j.createBuffer(type, length, False)
				buffer.read(s, allocMode, length, type)
				Return buffer
			Else
				Try
					' if buffer is compressed one, we''ll restore it here
					Dim compressionAlgorithm As String = s.readUTF()
					Dim compressedLength As Long = s.readLong()
					Dim originalLength As Long = s.readLong()
					Dim numberOfElements As Long = s.readLong()
					Dim originalType As DataType = DataType.values()(s.readInt())

					Dim temp(CInt(compressedLength) - 1) As SByte
					For i As Integer = 0 To compressedLength - 1
						temp(i) = s.readByte()
					Next i

					Dim pointer As Pointer = New BytePointer(temp)
					Dim descriptor As New CompressionDescriptor()
					descriptor.setCompressedLength(compressedLength)
					descriptor.setCompressionAlgorithm(compressionAlgorithm)
					descriptor.setOriginalLength(originalLength)
					descriptor.setNumberOfElements(numberOfElements)
					descriptor.setOriginalDataType(originalType)
					Return New CompressedDataBuffer(pointer, descriptor)
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End If
		End Function

		Public Overrides Function dup() As DataBuffer
			Dim nPtr As Pointer = New BytePointer(compressionDescriptor.getCompressedLength())

			Dim perfD As val = PerformanceTracker.Instance.helperStartTransaction()

			Pointer.memcpy(nPtr, pointer_Conflict, compressionDescriptor.getCompressedLength())

			PerformanceTracker.Instance.helperRegisterTransaction(0, perfD, compressionDescriptor.getCompressedLength(), MemcpyDirection.HOST_TO_HOST)

			Dim nDesc As CompressionDescriptor = compressionDescriptor.clone()

			Dim nBuf As New CompressedDataBuffer(nPtr, nDesc)
			Return nBuf
		End Function

		Public Overrides Function length() As Long
			Return compressionDescriptor.getNumberOfElements()
		End Function

		''' <summary>
		''' Create with length
		''' </summary>
		''' <param name="length"> a databuffer of the same opType as
		'''               this with the given length </param>
		''' <returns> a data buffer with the same length and datatype as this one </returns>
		Protected Friend Overrides Function create(ByVal length As Long) As DataBuffer
			Throw New System.NotSupportedException("This operation isn't supported for CompressedDataBuffer")
		End Function

		''' <summary>
		''' Create the data buffer
		''' with respect to the given byte buffer
		''' </summary>
		''' <param name="data"> the buffer to create </param>
		''' <returns> the data buffer based on the given buffer </returns>
		Public Overrides Function create(ByVal data() As Double) As DataBuffer
			Throw New System.NotSupportedException("This operation isn't supported for CompressedDataBuffer")
		End Function

		''' <summary>
		''' Create the data buffer
		''' with respect to the given byte buffer
		''' </summary>
		''' <param name="data"> the buffer to create </param>
		''' <returns> the data buffer based on the given buffer </returns>
		Public Overrides Function create(ByVal data() As Single) As DataBuffer
			Throw New System.NotSupportedException("This operation isn't supported for CompressedDataBuffer")
		End Function

		''' <summary>
		''' Create the data buffer
		''' with respect to the given byte buffer
		''' </summary>
		''' <param name="data"> the buffer to create </param>
		''' <returns> the data buffer based on the given buffer </returns>
		Public Overrides Function create(ByVal data() As Integer) As DataBuffer
			Throw New System.NotSupportedException("This method isn't supported by CompressedDataBuffer")
		End Function

		Public Overrides Sub pointerIndexerByCurrentType(ByVal currentType As DataType)
			Throw New System.NotSupportedException("This method isn't supported by CompressedDataBuffer")
		End Sub

		Public Overrides Function reallocate(ByVal length As Long) As DataBuffer
			Throw New System.NotSupportedException("This method isn't supported by CompressedDataBuffer")
		End Function

		Public Overrides Sub syncToPrimary()
			'No-op
		End Sub

		Public Overrides Sub syncToSpecial()
			'No-op
		End Sub

		Protected Friend Overrides Function getDoubleUnsynced(ByVal index As Long) As Double
			Return MyBase.getDouble(index)
		End Function

		Protected Friend Overrides Function getFloatUnsynced(ByVal index As Long) As Single
			Return MyBase.getFloat(index)
		End Function

		Protected Friend Overrides Function getLongUnsynced(ByVal index As Long) As Long
			Return MyBase.getLong(index)
		End Function

		Protected Friend Overrides Function getIntUnsynced(ByVal index As Long) As Integer
			Return MyBase.getInt(index)
		End Function
	End Class

End Namespace