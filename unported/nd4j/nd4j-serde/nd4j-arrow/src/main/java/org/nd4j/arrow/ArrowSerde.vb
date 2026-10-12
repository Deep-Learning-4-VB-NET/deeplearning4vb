Imports System
Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports org.apache.arrow.flatbuf
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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

	Public Class ArrowSerde


		''' <summary>
		''' Convert a <seealso cref="Tensor"/>
		''' to an <seealso cref="INDArray"/> </summary>
		''' <param name="tensor"> the input tensor </param>
		''' <returns> the equivalent <seealso cref="INDArray"/> </returns>
		Public Shared Function fromTensor(ByVal tensor As Tensor) As INDArray
			Dim b As SByte = tensor.typeType()
			Dim shape(tensor.shapeLength() - 1) As Integer
			Dim stride(tensor.stridesLength() - 1) As Integer
			For i As Integer = 0 To shape.Length - 1
				shape(i) = CInt(tensor.shape(i).size())
				stride(i) = CInt(Math.Truncate(tensor.strides(i)))
			Next i

			Dim length As Integer = ArrayUtil.prod(shape)
			Dim buffer As Buffer = tensor.data()
			If buffer Is Nothing Then
				Throw New ND4JIllegalStateException("Buffer was not serialized properly.")
			End If
			'deduce element size
			Dim elementSize As Integer = CInt(buffer.length()) \ length
			'nd4j strides aren't  based on element size
			For i As Integer = 0 To stride.Length - 1
				stride(i) \= elementSize
			Next i

			Dim type As DataType = typeFromTensorType(b,elementSize)
			Dim dataBuffer As DataBuffer = DataBufferStruct.createFromByteBuffer(tensor.getByteBuffer(),CInt(Math.Truncate(tensor.data().offset())),type,length)
			Dim arr As INDArray = Nd4j.create(dataBuffer,shape)
			arr.setShapeAndStride(shape,stride)
			Return arr
		End Function

		''' <summary>
		''' Convert an <seealso cref="INDArray"/>
		''' to an arrow <seealso cref="Tensor"/> </summary>
		''' <param name="arr"> the array to convert </param>
		''' <returns> the equivalent <seealso cref="Tensor"/> </returns>
		Public Shared Function toTensor(ByVal arr As INDArray) As Tensor
			Dim bufferBuilder As New FlatBufferBuilder(1024)
			Dim strides() As Long = getArrowStrides(arr)
			Dim shapeOffset As Integer = createDims(bufferBuilder,arr)
			Dim stridesOffset As Integer = Tensor.createStridesVector(bufferBuilder,strides)

			Tensor.startTensor(bufferBuilder)

			addTypeTypeRelativeToNDArray(bufferBuilder,arr)
			Tensor.addShape(bufferBuilder,shapeOffset)
			Tensor.addStrides(bufferBuilder,stridesOffset)

			Tensor.addData(bufferBuilder,addDataForArr(bufferBuilder,arr))
			Dim endTensor As Integer = Tensor.endTensor(bufferBuilder)
			Tensor.finishTensorBuffer(bufferBuilder,endTensor)
			Return Tensor.getRootAsTensor(bufferBuilder.dataBuffer())
		End Function


		''' <summary>
		''' Create a <seealso cref="Buffer"/>
		''' representing the location metadata of the actual data
		''' contents for the ndarrays' <seealso cref="DataBuffer"/> </summary>
		''' <param name="bufferBuilder"> the buffer builder in use </param>
		''' <param name="arr"> the array to add the underlying data for </param>
		''' <returns> the offset added </returns>
		Public Shared Function addDataForArr(ByVal bufferBuilder As FlatBufferBuilder, ByVal arr As INDArray) As Integer
			Dim toAdd As DataBuffer = If(arr.View, arr.dup().data(), arr.data())
			Dim offset As Integer = DataBufferStruct.createDataBufferStruct(bufferBuilder,toAdd)
			Dim ret As Integer = Buffer.createBuffer(bufferBuilder,offset,toAdd.length() * toAdd.ElementSize)
			Return ret

		End Function

		''' <summary>
		''' Convert the given <seealso cref="INDArray"/>
		''' data  type to the proper data type for the tensor. </summary>
		''' <param name="bufferBuilder"> the buffer builder in use </param>
		''' <param name="arr"> the array to conver tthe data type for </param>
		Public Shared Sub addTypeTypeRelativeToNDArray(ByVal bufferBuilder As FlatBufferBuilder, ByVal arr As INDArray)
			Select Case arr.data().dataType()
				Case [LONG], INT
					Tensor.addTypeType(bufferBuilder,Type.Int)
				Case FLOAT
					Tensor.addTypeType(bufferBuilder,Type.FloatingPoint)
				Case [DOUBLE]
					Tensor.addTypeType(bufferBuilder,Type.Decimal)
			End Select
		End Sub

		''' <summary>
		''' Create the dimensions for the flatbuffer builder </summary>
		''' <param name="bufferBuilder"> the buffer builder to use </param>
		''' <param name="arr"> the input array
		''' @return </param>
		Public Shared Function createDims(ByVal bufferBuilder As FlatBufferBuilder, ByVal arr As INDArray) As Integer
			Dim tensorDimOffsets(arr.rank() - 1) As Integer
			Dim nameOffset(arr.rank() - 1) As Integer
			For i As Integer = 0 To tensorDimOffsets.Length - 1
				nameOffset(i) = bufferBuilder.createString("")
				tensorDimOffsets(i) = TensorDim.createTensorDim(bufferBuilder,arr.size(i),nameOffset(i))
			Next i

			Return Tensor.createShapeVector(bufferBuilder,tensorDimOffsets)
		End Function


		''' <summary>
		''' Get the strides of this <seealso cref="INDArray"/>
		''' multiplieed by  the element size.
		''' This is the <seealso cref="Tensor"/> and numpy format </summary>
		''' <param name="arr"> the array to convert
		''' @return </param>
		Public Shared Function getArrowStrides(ByVal arr As INDArray) As Long()
			Dim ret(arr.rank() - 1) As Long
			Dim i As Integer = 0
			Do While i < arr.rank()
				ret(i) = arr.stride(i) * arr.data().ElementSize
				i += 1
			Loop

			Return ret
		End Function



		''' <summary>
		''' Create thee databuffer type frm the given type,
		''' relative to the bytes in arrow in class:
		''' <seealso cref="Type"/> </summary>
		''' <param name="type"> the type to create the nd4j <seealso cref="DataType"/> from </param>
		''' <param name="elementSize"> the element size </param>
		''' <returns> the data buffer type </returns>
		Public Shared Function typeFromTensorType(ByVal type As SByte, ByVal elementSize As Integer) As DataType
			If type = Type.FloatingPoint Then
				Return DataType.FLOAT
			ElseIf type = Type.Decimal Then
				Return DataType.DOUBLE
			ElseIf type = Type.Int Then
				If elementSize = 4 Then
					Return DataType.INT
				ElseIf elementSize = 8 Then
					Return DataType.LONG
				End If
			Else
				Throw New System.ArgumentException("Only valid types are Type.Decimal and Type.Int")
			End If

			Throw New System.ArgumentException("Unable to determine data type")
		End Function
	End Class

End Namespace