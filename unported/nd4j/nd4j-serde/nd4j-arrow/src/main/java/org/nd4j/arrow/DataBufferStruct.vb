Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports Struct = com.google.flatbuffers.Struct
Imports Getter = lombok.Getter
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeUtil = org.nd4j.linalg.api.buffer.util.DataTypeUtil
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

Namespace org.nd4j.arrow


	Public Class DataBufferStruct
		Inherits Struct

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.api.buffer.DataBuffer dataBuffer;
		Private dataBuffer As DataBuffer

		Public Sub New(ByVal dataBuffer As DataBuffer)
			Me.dataBuffer = dataBuffer
		End Sub

		Public Sub New(ByVal byteBuffer As ByteBuffer, ByVal offset As Integer)
			__init(offset,byteBuffer)
		End Sub

		Public Overridable Sub __init(ByVal _i As Integer, ByVal _bb As ByteBuffer)
			bb_pos = _i
			bb = _bb
		End Sub
		Public Overridable Function __assign(ByVal _i As Integer, ByVal _bb As ByteBuffer) As DataBufferStruct
			__init(_i, _bb)
			Return Me
		End Function

		''' <summary>
		''' Create a <seealso cref="DataBuffer"/> from a
		''' byte buffer. This is meant to be used with flatbuffers </summary>
		''' <param name="bb"> the flat buffers buffer </param>
		''' <param name="bb_pos"> the position to start from </param>
		''' <param name="type"> the type of buffer to create </param>
		''' <param name="length"> the length of the buffer to create </param>
		''' <returns> the created databuffer </returns>
		Public Shared Function createFromByteBuffer(ByVal bb As ByteBuffer, ByVal bb_pos As Integer, ByVal type As DataType, ByVal length As Integer) As DataBuffer
			bb.order(ByteOrder.LITTLE_ENDIAN)
			Dim elementSize As Integer = DataTypeUtil.lengthForDtype(type)
			Dim ret As DataBuffer = Nd4j.createBuffer(ByteBuffer.allocateDirect(length * elementSize),type,length,0)

			Select Case type.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Dim i As Integer = 0
					Do While i < ret.length()
						Dim doubleGet As Double = bb.getDouble(bb.capacity() - bb_pos + (i * elementSize))
						ret.put(i,doubleGet)
						i += 1
					Loop
				Case DataType.InnerEnum.FLOAT
					Dim i As Integer = 0
					Do While i < ret.length()
						Dim floatGet As Single = bb.getFloat(bb.capacity() - bb_pos + (i * elementSize))
						ret.put(i,floatGet)
						i += 1
					Loop
				Case DataType.InnerEnum.INT
					Dim i As Integer = 0
					Do While i < ret.length()
						Dim intGet As Integer = bb.getInt(bb.capacity() - bb_pos + (i * elementSize))
						ret.put(i,intGet)
						i += 1
					Loop
				Case DataType.InnerEnum.LONG
					Dim i As Integer = 0
					Do While i < ret.length()
						Dim longGet As Long = bb.getLong(bb.capacity() - bb_pos + (i * elementSize))
						ret.put(i,longGet)
						i += 1
					Loop
			End Select

			Return ret
		End Function


		''' <summary>
		''' Create a data buffer struct within
		''' the passed in <seealso cref="FlatBufferBuilder"/> </summary>
		''' <param name="bufferBuilder"> the existing flatbuffer
		'''                      to use to serialize the <seealso cref="DataBuffer"/> </param>
		''' <param name="create"> the databuffer to serialize </param>
		''' <returns> an int representing the offset of the buffer </returns>
		Public Shared Function createDataBufferStruct(ByVal bufferBuilder As FlatBufferBuilder, ByVal create As DataBuffer) As Integer
			bufferBuilder.prep(create.ElementSize, CInt(create.length()) * create.ElementSize)
			For i As Integer = CInt(create.length() - 1) To 0 Step -1
				Select Case create.dataType()
					Case [DOUBLE]
						Dim putDouble As Double = create.getDouble(i)
						bufferBuilder.putDouble(putDouble)
					Case FLOAT
						Dim putFloat As Single = create.getFloat(i)
						bufferBuilder.putFloat(putFloat)
					Case INT
						Dim putInt As Integer = create.getInt(i)
						bufferBuilder.putInt(putInt)
					Case [LONG]
						Dim putLong As Long = create.getLong(i)
						bufferBuilder.putLong(putLong)
				End Select
			Next i

			Return bufferBuilder.offset()

		End Function
	End Class

End Namespace