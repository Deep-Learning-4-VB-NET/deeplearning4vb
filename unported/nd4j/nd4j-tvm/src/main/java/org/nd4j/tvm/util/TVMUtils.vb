Imports System
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports org.bytedeco.tvm
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.bytedeco.tvm.global.tvm_runtime
Imports org.nd4j.linalg.api.buffer.DataType

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
Namespace org.nd4j.tvm.util

	Public Class TVMUtils

		''' <summary>
		''' Return a <seealso cref="DataType"/>
		''' for the tvm data type </summary>
		''' <param name="dataType"> the equivalent nd4j data type
		''' @return </param>
		Public Shared Function dataTypeForTvmType(ByVal dataType As DLDataType) As DataType
			If dataType.code() = kDLInt AndAlso dataType.bits() = 8 Then
				Return INT8
			ElseIf dataType.code() = kDLInt AndAlso dataType.bits() = 16 Then
				Return INT16
			ElseIf dataType.code() = kDLInt AndAlso dataType.bits() = 32 Then
				Return INT32
			ElseIf dataType.code() = kDLInt AndAlso dataType.bits() = 64 Then
				Return INT64
			ElseIf dataType.code() = kDLUInt AndAlso dataType.bits() = 8 Then
				Return UINT8
			ElseIf dataType.code() = kDLUInt AndAlso dataType.bits() = 16 Then
				Return UINT16
			ElseIf dataType.code() = kDLUInt AndAlso dataType.bits() = 32 Then
				Return UINT32
			ElseIf dataType.code() = kDLUInt AndAlso dataType.bits() = 64 Then
				Return UINT64
			ElseIf dataType.code() = kDLFloat AndAlso dataType.bits() = 16 Then
				Return FLOAT16
			ElseIf dataType.code() = kDLFloat AndAlso dataType.bits() = 32 Then
				Return FLOAT
			ElseIf dataType.code() = kDLFloat AndAlso dataType.bits() = 64 Then
				Return [DOUBLE]
			ElseIf dataType.code() = kDLBfloat AndAlso dataType.bits() = 16 Then
				Return BFLOAT16
			Else
				Throw New System.ArgumentException("Illegal data type code " & dataType.code() & " with bits " & dataType.bits())
			End If
		End Function

		''' <summary>
		''' Convert the tvm type for the given data type </summary>
		''' <param name="dataType">
		''' @return </param>
		Public Shared Function tvmTypeForDataType(ByVal dataType As DataType) As DLDataType
			If dataType = INT8 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLInt))).bits(CSByte(8)).lanes(CShort(1))
			ElseIf dataType = INT16 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLInt))).bits(CSByte(16)).lanes(CShort(1))
			ElseIf dataType = INT32 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLInt))).bits(CSByte(32)).lanes(CShort(1))
			ElseIf dataType = INT64 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLInt))).bits(CSByte(64)).lanes(CShort(1))
			ElseIf dataType = UINT8 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLUInt))).bits(CSByte(8)).lanes(CShort(1))
			ElseIf dataType = UINT16 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLUInt))).bits(CSByte(16)).lanes(CShort(1))
			ElseIf dataType = UINT32 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLUInt))).bits(CSByte(32)).lanes(CShort(1))
			ElseIf dataType = UINT64 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLUInt))).bits(CSByte(64)).lanes(CShort(1))
			ElseIf dataType = FLOAT16 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLFloat))).bits(CSByte(16)).lanes(CShort(1))
			ElseIf dataType = FLOAT Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLFloat))).bits(CSByte(32)).lanes(CShort(1))
			ElseIf dataType = [DOUBLE] Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLFloat))).bits(CSByte(64)).lanes(CShort(1))
			ElseIf dataType = BFLOAT16 Then
				Return (New DLDataType()).code(CSByte(Math.Truncate(kDLBfloat))).bits(CSByte(16)).lanes(CShort(1))
			Else
				Throw New System.ArgumentException("Illegal data type " & dataType)
			End If
		End Function

		''' <summary>
		''' Convert an tvm <seealso cref="DLTensor"/>
		'''  in to an <seealso cref="INDArray"/> </summary>
		''' <param name="value"> the tensor to convert
		''' @return </param>
		Public Shared Function getArray(ByVal value As DLTensor) As INDArray
			Dim dataType As DataType = dataTypeForTvmType(value.dtype())
			Dim shape As LongPointer = value.shape()
			Dim stride As LongPointer = value.strides()
			Dim shapeConvert() As Long
			If shape IsNot Nothing Then
				shapeConvert = New Long(value.ndim() - 1){}
				shape.get(shapeConvert)
			Else
				shapeConvert = New Long(){1}
			End If
			Dim strideConvert() As Long
			If stride IsNot Nothing Then
				strideConvert = New Long(value.ndim() - 1){}
				stride.get(strideConvert)
			Else
				strideConvert = Nd4j.getStrides(shapeConvert)
			End If
			Dim size As Long = 1
			For i As Integer = 0 To shapeConvert.Length - 1
				size *= shapeConvert(i)
			Next i
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			size *= value.dtype().bits() / 8

			Dim getBuffer As DataBuffer = getDataBuffer(value,size)
			Preconditions.checkState(dataType.Equals(getBuffer.dataType()),"Data type must be equivalent as specified by the tvm metadata.")
			Return Nd4j.create(getBuffer,shapeConvert,strideConvert,0)
		End Function

		''' <summary>
		''' Get an tvm tensor from an ndarray. </summary>
		''' <param name="ndArray"> the ndarray to get the value from </param>
		''' <param name="ctx"> the <seealso cref="DLContext"/> to use.
		''' @return </param>
		Public Shared Function getTensor(ByVal ndArray As INDArray, ByVal ctx As DLContext) As DLTensor
			Dim ret As New DLTensor()
			ret.data(ndArray.data().pointer())
			ret.ctx(ctx)
			ret.ndim(ndArray.rank())
			ret.dtype(tvmTypeForDataType(ndArray.dataType()))
			ret.shape(New LongPointer(ndArray.shape()))
			ret.strides(New LongPointer(ndArray.stride()))
			ret.byte_offset(ndArray.offset())
			Return ret
		End Function

		''' <summary>
		''' Get the data buffer from the given value </summary>
		''' <param name="tens"> the values to get </param>
		''' <returns> the equivalent data buffer </returns>
		Public Shared Function getDataBuffer(ByVal tens As DLTensor, ByVal size As Long) As DataBuffer
			Dim buffer As DataBuffer = Nothing
			Dim type As DataType = dataTypeForTvmType(tens.dtype())
			Select Case type.innerEnumValue
				Case DataType.InnerEnum.BYTE
					Dim pInt8 As BytePointer = (New BytePointer(tens.data())).capacity(size)
					Dim int8Indexer As Indexer = ByteIndexer.create(pInt8)
					buffer = Nd4j.createBuffer(pInt8, type, size, int8Indexer)
				Case DataType.InnerEnum.SHORT
					Dim pInt16 As ShortPointer = (New ShortPointer(tens.data())).capacity(size)
					Dim int16Indexer As Indexer = ShortIndexer.create(pInt16)
					buffer = Nd4j.createBuffer(pInt16, type, size, int16Indexer)
				Case DataType.InnerEnum.INT
					Dim pInt32 As IntPointer = (New IntPointer(tens.data())).capacity(size)
					Dim int32Indexer As Indexer = IntIndexer.create(pInt32)
					buffer = Nd4j.createBuffer(pInt32, type, size, int32Indexer)
				Case DataType.InnerEnum.LONG
					Dim pInt64 As LongPointer = (New LongPointer(tens.data())).capacity(size)
					Dim int64Indexer As Indexer = LongIndexer.create(pInt64)
					buffer = Nd4j.createBuffer(pInt64, type, size, int64Indexer)
				Case DataType.InnerEnum.UBYTE
					Dim pUint8 As BytePointer = (New BytePointer(tens.data())).capacity(size)
					Dim uint8Indexer As Indexer = UByteIndexer.create(pUint8)
					buffer = Nd4j.createBuffer(pUint8, type, size, uint8Indexer)
				Case DataType.InnerEnum.UINT16
					Dim pUint16 As ShortPointer = (New ShortPointer(tens.data())).capacity(size)
					Dim uint16Indexer As Indexer = UShortIndexer.create(pUint16)
					buffer = Nd4j.createBuffer(pUint16, type, size, uint16Indexer)
				Case DataType.InnerEnum.UINT32
					Dim pUint32 As IntPointer = (New IntPointer(tens.data())).capacity(size)
					Dim uint32Indexer As Indexer = UIntIndexer.create(pUint32)
					buffer = Nd4j.createBuffer(pUint32, type, size, uint32Indexer)
				Case DataType.InnerEnum.UINT64
					Dim pUint64 As LongPointer = (New LongPointer(tens.data())).capacity(size)
					Dim uint64Indexer As Indexer = LongIndexer.create(pUint64)
					buffer = Nd4j.createBuffer(pUint64, type, size, uint64Indexer)
				Case DataType.InnerEnum.HALF
					Dim pFloat16 As ShortPointer = (New ShortPointer(tens.data())).capacity(size)
					Dim float16Indexer As Indexer = HalfIndexer.create(pFloat16)
					buffer = Nd4j.createBuffer(pFloat16, type, size, float16Indexer)
				Case DataType.InnerEnum.FLOAT
					Dim pFloat As FloatPointer = (New FloatPointer(tens.data())).capacity(size)
					Dim floatIndexer As FloatIndexer = FloatIndexer.create(pFloat)
					buffer = Nd4j.createBuffer(pFloat, type, size, floatIndexer)
				Case DataType.InnerEnum.DOUBLE
					Dim pDouble As DoublePointer = (New DoublePointer(tens.data())).capacity(size)
					Dim doubleIndexer As Indexer = DoubleIndexer.create(pDouble)
					buffer = Nd4j.createBuffer(pDouble, type, size, doubleIndexer)
				Case DataType.InnerEnum.BFLOAT16
					Dim pBfloat16 As ShortPointer = (New ShortPointer(tens.data())).capacity(size)
					Dim bfloat16Indexer As Indexer = Bfloat16Indexer.create(pBfloat16)
					buffer = Nd4j.createBuffer(pBfloat16, type, size, bfloat16Indexer)
				Case Else
					Throw New Exception("Unsupported data type encountered")
			End Select
			Return buffer
		End Function

	End Class

End Namespace