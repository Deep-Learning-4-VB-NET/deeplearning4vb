Imports System
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports CompressionDescriptor = org.nd4j.linalg.compression.CompressionDescriptor
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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

Namespace org.nd4j.serde.binary


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BinarySerde
	Public Class BinarySerde

		''' <summary>
		''' Create an ndarray
		''' from the unsafe buffer </summary>
		''' <param name="buffer"> the buffer to create the array from </param>
		''' <returns> the ndarray derived from this buffer </returns>
		Public Shared Function toArray(ByVal buffer As ByteBuffer, ByVal offset As Integer) As INDArray
			Return toArrayAndByteBuffer(buffer, offset).Left
		End Function

		''' <summary>
		''' Create an ndarray
		''' from the unsafe buffer </summary>
		''' <param name="buffer"> the buffer to create the array from </param>
		''' <returns> the ndarray derived from this buffer </returns>
		Public Shared Function toArray(ByVal buffer As ByteBuffer) As INDArray
			Return toArray(buffer, 0)
		End Function

		''' <summary>
		''' Create an ndarray and existing bytebuffer </summary>
		''' <param name="buffer"> the buffer to create the arrays from </param>
		''' <param name="offset"> position in buffer to create the arrays from. </param>
		''' <returns> the created INDArray and Bytebuffer pair. </returns>
		Protected Friend Shared Function toArrayAndByteBuffer(ByVal buffer As ByteBuffer, ByVal offset As Integer) As Pair(Of INDArray, ByteBuffer)
			Dim byteBuffer As ByteBuffer = If(buffer.hasArray(), ByteBuffer.allocateDirect(buffer.array().length).put(buffer.array()).order(ByteOrder.nativeOrder()), buffer.order(ByteOrder.nativeOrder()))
			'bump the byte buffer to the proper position
			byteBuffer.position(offset)
			Dim rank As Integer = byteBuffer.getInt()
			If rank < 0 Then
				Throw New System.InvalidOperationException("Found negative integer. Corrupt serialization?")
			End If
			'get the shape buffer length to create the shape information buffer
			Dim shapeBufferLength As Integer = Shape.shapeInfoLength(rank)
			'create the ndarray shape information
			Dim shapeBuff As DataBuffer = Nd4j.createBufferDetached(New Integer(shapeBufferLength - 1){})

			'compute the databuffer opType from the index
			Dim type As DataType = DataType.values()(byteBuffer.getInt())
			For i As Integer = 0 To shapeBufferLength - 1
				shapeBuff.put(i, byteBuffer.getLong())
			Next i

			'after the rank,data opType, shape buffer (of length shape buffer length) * sizeof(int)
			If type <> DataType.COMPRESSED Then
				Dim slice As ByteBuffer = byteBuffer.slice()
				'wrap the data buffer for the last bit
				If Shape.length(shapeBuff) > Integer.MaxValue Then
					Throw New ND4JArraySizeException()
				End If
				Dim buff As DataBuffer = Nd4j.createBuffer(slice, type, CInt(Shape.length(shapeBuff)))
				'advance past the data
				Dim position As Integer = byteBuffer.position() + (buff.ElementSize * CInt(buff.length()))
				byteBuffer.position(position)
				'create the final array
				'TODO: see how to avoid dup here
				Dim arr As INDArray = Nd4j.createArrayFromShapeBuffer(buff.dup(), shapeBuff.dup())
				Return Pair.of(arr, byteBuffer)
			Else
				Dim compressionDescriptor As CompressionDescriptor = CompressionDescriptor.fromByteBuffer(byteBuffer)
				Dim slice As ByteBuffer = byteBuffer.slice()
				'ensure that we only deal with the slice of the buffer that is actually the data
				Dim byteBufferPointer As New BytePointer(slice)
				'create a compressed array based on the rest of the data left in the buffer
				Dim compressedDataBuffer As New CompressedDataBuffer(byteBufferPointer, compressionDescriptor)
				'TODO: see how to avoid dup()
				Dim arr As INDArray = Nd4j.createArrayFromShapeBuffer(compressedDataBuffer.dup(), shapeBuff.dup())
				'advance past the data
				Dim compressLength As Integer = CInt(Math.Truncate(compressionDescriptor.getCompressedLength()))
				byteBuffer.position(byteBuffer.position() + compressLength)
				Return Pair.of(arr, byteBuffer)
			End If

		End Function


		''' <summary>
		''' Convert an ndarray to an unsafe buffer
		''' for use by aeron </summary>
		''' <param name="arr"> the array to convert </param>
		''' <returns> the unsafebuffer representation of this array </returns>
		Public Shared Function toByteBuffer(ByVal arr As INDArray) As ByteBuffer
			'subset and get rid of 1 off non 1 element wise stride cases
			If arr.View Then
				arr = arr.dup()
			End If
			If Not arr.Compressed Then
				Dim b3 As ByteBuffer = ByteBuffer.allocateDirect(byteBufferSizeFor(arr)).order(ByteOrder.nativeOrder())
				doByteBufferPutUnCompressed(arr, b3, True)
				Return b3
			'compressed array
			Else
				Dim b3 As ByteBuffer = ByteBuffer.allocateDirect(byteBufferSizeFor(arr)).order(ByteOrder.nativeOrder())
				doByteBufferPutCompressed(arr, b3, True)
				Return b3
			End If

		End Function

		''' <summary>
		''' Returns the byte buffer size for the given
		''' ndarray. This is an auxillary method
		''' for determining the size of the buffer
		''' size to allocate for sending an ndarray via
		''' the aeron media driver.
		''' 
		''' The math break down for uncompressed is:
		''' 2 ints for rank of the array and an ordinal representing the data opType of the data buffer
		''' The rest is in order:
		''' shape information
		''' data buffer
		''' 
		''' The math break down for compressed is:
		''' 2 ints for rank and an ordinal representing the data opType for the data buffer
		''' 
		''' The rest is in order:
		''' shape information
		''' codec information
		''' data buffer
		''' </summary>
		''' <param name="arr"> the array to compute the size for </param>
		''' <returns> the size of the byte buffer that was allocated </returns>
		Public Shared Function byteBufferSizeFor(ByVal arr As INDArray) As Integer
			If Not arr.Compressed Then
				Dim buffer As ByteBuffer = arr.data().pointer().asByteBuffer().order(ByteOrder.nativeOrder())
				Dim shapeBuffer As ByteBuffer = arr.shapeInfoDataBuffer().pointer().asByteBuffer().order(ByteOrder.nativeOrder())
				'2 four byte ints at the beginning
				Dim twoInts As Integer = 8
				Return twoInts + buffer.limit() + shapeBuffer.limit()
			Else
				Dim compressedDataBuffer As CompressedDataBuffer = DirectCast(arr.data(), CompressedDataBuffer)
				Dim descriptor As CompressionDescriptor = compressedDataBuffer.getCompressionDescriptor()
				Dim codecByteBuffer As ByteBuffer = descriptor.toByteBuffer()
				Dim buffer As ByteBuffer = arr.data().pointer().asByteBuffer().order(ByteOrder.nativeOrder())
				Dim shapeBuffer As ByteBuffer = arr.shapeInfoDataBuffer().pointer().asByteBuffer().order(ByteOrder.nativeOrder())
				Dim twoInts As Integer = 2 * 4
				Return twoInts + buffer.limit() + shapeBuffer.limit() + codecByteBuffer.limit()
			End If
		End Function



		''' <summary>
		''' Setup the given byte buffer
		''' for serialization (note that this is for uncompressed INDArrays)
		''' 4 bytes int for rank
		''' 4 bytes for data opType
		''' shape buffer
		''' data buffer
		''' </summary>
		''' <param name="arr"> the array to setup </param>
		''' <param name="allocated"> the byte buffer to setup </param>
		''' <param name="rewind"> whether to rewind the byte buffer or nt </param>
		Public Shared Sub doByteBufferPutUnCompressed(ByVal arr As INDArray, ByVal allocated As ByteBuffer, ByVal rewind As Boolean)
			' ensure we send data to host memory
			Nd4j.Executioner.commit()
			Nd4j.AffinityManager.ensureLocation(arr, AffinityManager.Location.HOST)

			Dim buffer As ByteBuffer = arr.data().pointer().asByteBuffer().order(ByteOrder.nativeOrder())
			Dim shapeBuffer As ByteBuffer = arr.shapeInfoDataBuffer().pointer().asByteBuffer().order(ByteOrder.nativeOrder())
			'2 four byte ints at the beginning
			allocated.putInt(arr.rank())
			'put data opType next so its self describing
			allocated.putInt(arr.data().dataType().ordinal())
			allocated.put(shapeBuffer)
			allocated.put(buffer)
			If rewind Then
				CType(allocated, Buffer).rewind()
			End If
		End Sub

		''' <summary>
		''' Setup the given byte buffer
		''' for serialization (note that this is for compressed INDArrays)
		''' 4 bytes for rank
		''' 4 bytes for data opType
		''' shape information
		''' codec information
		''' data opType
		''' </summary>
		''' <param name="arr"> the array to setup </param>
		''' <param name="allocated"> the byte buffer to setup </param>
		''' <param name="rewind"> whether to rewind the byte buffer or not </param>
		Public Shared Sub doByteBufferPutCompressed(ByVal arr As INDArray, ByVal allocated As ByteBuffer, ByVal rewind As Boolean)
			Dim compressedDataBuffer As CompressedDataBuffer = DirectCast(arr.data(), CompressedDataBuffer)
			Dim descriptor As CompressionDescriptor = compressedDataBuffer.getCompressionDescriptor()
			Dim codecByteBuffer As ByteBuffer = descriptor.toByteBuffer()
			Dim buffer As ByteBuffer = arr.data().pointer().asByteBuffer().order(ByteOrder.nativeOrder())
			Dim shapeBuffer As ByteBuffer = arr.shapeInfoDataBuffer().pointer().asByteBuffer().order(ByteOrder.nativeOrder())
			allocated.putInt(arr.rank())
			'put data opType next so its self describing
			allocated.putInt(arr.data().dataType().ordinal())
			'put shape next
			allocated.put(shapeBuffer)
			'put codec information next
			allocated.put(codecByteBuffer)
			'finally put the data
			allocated.put(buffer)
			If rewind Then
				CType(allocated, Buffer).rewind()
			End If
		End Sub


		''' <summary>
		''' Write an array to an output stream. </summary>
		''' <param name="arr"> the array to write </param>
		''' <param name="outputStream"> the output stream to write to </param>
		Public Shared Sub writeArrayToOutputStream(ByVal arr As INDArray, ByVal outputStream As Stream)
			Dim buffer As ByteBuffer = BinarySerde.toByteBuffer(arr)
			Try
					Using channel As WritableByteChannel = Channels.newChannel(outputStream)
					channel.write(buffer)
					End Using
			Catch e As IOException
				log.error("",e)
			End Try
		End Sub


		''' <summary>
		''' Write an ndarray to disk in
		''' binary format </summary>
		''' <param name="arr"> the array to write </param>
		''' <param name="toWrite"> the file tow rite to </param>
		''' <exception cref="IOException"> on an I/O exception. </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeArrayToDisk(org.nd4j.linalg.api.ndarray.INDArray arr, File toWrite) throws IOException
		Public Shared Sub writeArrayToDisk(ByVal arr As INDArray, ByVal toWrite As File)
			Using os As New FileStream(toWrite, FileMode.Create, FileAccess.Write)
				Dim channel As FileChannel = os.getChannel()
				Dim buffer As ByteBuffer = BinarySerde.toByteBuffer(arr)
				channel.write(buffer)
			End Using
		End Sub


		''' <summary>
		''' Read an ndarray from disk </summary>
		''' <param name="readFrom"> file to read </param>
		''' <returns> the created INDArray. </returns>
		''' <exception cref="IOException"> on an I/O exception. </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.linalg.api.ndarray.INDArray readFromDisk(File readFrom) throws IOException
		Public Shared Function readFromDisk(ByVal readFrom As File) As INDArray
			Using os As New FileStream(readFrom, FileMode.Open, FileAccess.Read)
				Dim channel As FileChannel = os.getChannel()
				Dim buffer As ByteBuffer = ByteBuffer.allocateDirect(CInt(readFrom.length()))
				channel.read(buffer)
				Return toArray(buffer)
			End Using
		End Function

		''' <summary>
		''' This method returns shape databuffer from saved earlier file
		''' </summary>
		''' <param name="readFrom"> file to read </param>
		''' <returns> the created databuffer, </returns>
		''' <exception cref="IOException"> on an I/O exception. </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.linalg.api.buffer.DataBuffer readShapeFromDisk(File readFrom) throws IOException
		Public Shared Function readShapeFromDisk(ByVal readFrom As File) As DataBuffer
			Using os As New FileStream(readFrom, FileMode.Open, FileAccess.Read)
				Dim channel As FileChannel = os.getChannel()
				' we read shapeinfo up to max_rank value, which is 32
				Dim len As Integer = CInt(Math.Min((32 * 2 + 3) * 8, readFrom.length()))
				Dim buffer As ByteBuffer = ByteBuffer.allocateDirect(len)
				channel.read(buffer)

				Dim byteBuffer As ByteBuffer = buffer.order(ByteOrder.nativeOrder())

				CType(buffer, Buffer).position(0)
				Dim rank As Integer = byteBuffer.getInt()

				Dim result As val = New Long(Shape.shapeInfoLength(rank) - 1){}

				' filling DataBuffer with shape info
				result(0) = rank

				' skipping two next values (dtype and rank again)
				' please , that this time rank has dtype of LONG, so takes 8 bytes.
				CType(byteBuffer, Buffer).position(16)

				' filling shape information
				Dim e As Integer = 1
				Do While e < Shape.shapeInfoLength(rank)
					result(e) = byteBuffer.getLong()
					e += 1
				Loop

				' creating nd4j databuffer now
				Return Nd4j.DataBufferFactory.createLong(result)
			End Using
		End Function
	End Class

End Namespace