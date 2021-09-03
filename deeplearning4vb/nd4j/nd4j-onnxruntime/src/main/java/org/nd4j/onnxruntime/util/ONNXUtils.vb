Imports System
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports MemoryInfo = org.bytedeco.onnxruntime.MemoryInfo
Imports Value = org.bytedeco.onnxruntime.Value
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports org.bytedeco.onnxruntime.global.onnxruntime
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
Namespace org.nd4j.onnxruntime.util

	Public Class ONNXUtils

		''' 
		''' <param name="expected"> </param>
		''' <param name="array"> </param>
		Public Shared Sub validateType(ByVal expected As DataType, ByVal array As INDArray)
			If Not array.dataType().Equals(expected) Then
				Throw New Exception("INDArray data type (" & array.dataType() & ") does not match required ONNX data type (" & expected & ")")
			End If
		End Sub

		''' <summary>
		''' Return a <seealso cref="DataType"/>
		''' for the onnx data type </summary>
		''' <param name="dataType"> the equivalent nd4j data type
		''' @return </param>
		Public Shared Function dataTypeForOnnxType(ByVal dataType As Integer) As DataType
			If dataType = dataType Then
				Return FLOAT
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_INT8 Then
				Return INT8
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_DOUBLE Then
				Return [DOUBLE]
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_BOOL Then
				Return BOOL
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT8 Then
				Return UINT8
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT16 Then
				Return UINT16
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_INT16 Then
				Return INT16
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_INT32 Then
				Return INT32
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_INT64 Then
				Return INT64
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT16 Then
				Return FLOAT16
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT32 Then
				Return UINT32
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT64 Then
				Return UINT64
			ElseIf dataType = ONNX_TENSOR_ELEMENT_DATA_TYPE_BFLOAT16 Then
				Return BFLOAT16
			Else
				Throw New System.ArgumentException("Illegal data type " & dataType)
			End If
		End Function

		''' <summary>
		''' Convert the onnx type for the given data type </summary>
		''' <param name="dataType">
		''' @return </param>
		Public Shared Function onnxTypeForDataType(ByVal dataType As DataType) As Integer
			If dataType = FLOAT Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT
			ElseIf dataType = INT8 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_INT8
			ElseIf dataType = [DOUBLE] Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_DOUBLE
			ElseIf dataType = BOOL Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_BOOL
			ElseIf dataType = UINT8 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT8
			ElseIf dataType = UINT16 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT16
			ElseIf dataType = INT16 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_INT16
			ElseIf dataType = INT32 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_INT32
			ElseIf dataType = INT64 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_INT64
			ElseIf dataType = FLOAT16 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT16
			ElseIf dataType = UINT32 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT32
			ElseIf dataType = UINT64 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT64
			ElseIf dataType = BFLOAT16 Then
				Return ONNX_TENSOR_ELEMENT_DATA_TYPE_BFLOAT16
			Else
				Throw New System.ArgumentException("Illegal data type " & dataType)
			End If
		End Function


		''' <summary>
		''' Convert an onnx <seealso cref="Value"/>
		'''  in to an <seealso cref="INDArray"/> </summary>
		''' <param name="value"> the value to convert
		''' @return </param>
		Public Shared Function getArray(ByVal value As Value) As INDArray
			Dim dataType As DataType = dataTypeForOnnxType(value.GetTypeInfo().GetONNXType())
			Dim shape As LongPointer = value.GetTensorTypeAndShapeInfo().GetShape()
			Dim shapeConvert() As Long
			If shape IsNot Nothing Then
				shapeConvert = New Long(CInt(Math.Truncate(value.GetTensorTypeAndShapeInfo().GetDimensionsCount())) - 1){}
				shape.get(shapeConvert)
			Else
				shapeConvert = New Long(){1}
			End If

			Dim getBuffer As DataBuffer = getDataBuffer(value)
			Preconditions.checkState(dataType.Equals(getBuffer.dataType()),"Data type must be equivalent as specified by the onnx metadata.")
			Return Nd4j.create(getBuffer,shapeConvert,Nd4j.getStrides(shapeConvert),0)
		End Function


		''' <summary>
		''' Get the onnx log level relative to the given slf4j logger.
		''' Trace or debug will return ORT_LOGGING_LEVEL_VERBOSE
		''' Info will return: ORT_LOGGING_LEVEL_INFO
		''' Warn returns ORT_LOGGING_LEVEL_WARNING
		''' Error returns error ORT_LOGGING_LEVEL_ERROR
		''' 
		''' The default is info </summary>
		''' <param name="logger"> the slf4j logger to get the onnx log level for
		''' @return </param>
		Public Shared Function getOnnxLogLevelFromLogger(ByVal logger As Logger) As Integer
			If logger.isTraceEnabled() OrElse logger.isDebugEnabled() Then
				Return ORT_LOGGING_LEVEL_VERBOSE
			ElseIf logger.isInfoEnabled() Then
				Return ORT_LOGGING_LEVEL_INFO
			ElseIf logger.isWarnEnabled() Then
				Return ORT_LOGGING_LEVEL_WARNING
			ElseIf logger.isErrorEnabled() Then
				Return ORT_LOGGING_LEVEL_ERROR
			End If

			Return ORT_LOGGING_LEVEL_INFO

		End Function

		''' <summary>
		''' Get an onnx tensor from an ndarray. </summary>
		''' <param name="ndArray"> the ndarray to get the value from </param>
		''' <param name="memoryInfo"> the <seealso cref="MemoryInfo"/> to use.
		'''                   Can be created with:
		'''                   MemoryInfo memoryInfo = MemoryInfo.CreateCpu(OrtArenaAllocator, OrtMemTypeDefault);
		''' @return </param>
		Public Shared Function getTensor(ByVal ndArray As INDArray, ByVal memoryInfo As MemoryInfo) As Value
			Dim inputTensorValuesPtr As Pointer = ndArray.data().pointer()
			Dim inputTensorValues As Pointer = inputTensorValuesPtr
			Dim sizeInBytes As Long = ndArray.length() * ndArray.data().ElementSize

			'        public static native Value CreateTensor(@Const OrtMemoryInfo var0, Pointer var1, @Cast({"size_t"}) long var2, @Cast({"const int64_t*"}) LongPointer var4, @Cast({"size_t"}) long var5, @Cast({"ONNXTensorElementDataType"}) int var7);
			''' <summary>
			'''   static Value CreateTensor(const OrtMemoryInfo* info, void* p_data, size_t p_data_byte_count, const int64_t* shape, size_t shape_len,
			'''                             ONNXTensorElementDataType type)
			''' </summary>
			Dim dims As New LongPointer(ndArray.shape())
			Dim ret As Value = Value.CreateTensor(memoryInfo.asOrtMemoryInfo(), inputTensorValues, sizeInBytes, dims, ndArray.rank(), onnxTypeForDataType(ndArray.dataType()))
			Return ret
		End Function

		''' <summary>
		''' Get the data buffer from the given value </summary>
		''' <param name="tens"> the values to get </param>
		''' <returns> the equivalent data buffer </returns>
		Public Shared Function getDataBuffer(ByVal tens As Value) As DataBuffer
			Using scope As New PointerScope()
				Dim buffer As DataBuffer = Nothing
				Dim type As Integer = tens.GetTensorTypeAndShapeInfo().GetElementType()
				Dim size As Long = tens.GetTensorTypeAndShapeInfo().GetElementCount()
				Select Case type
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT
						Dim pFloat As FloatPointer = tens.GetTensorMutableDataFloat().capacity(size)
						Dim floatIndexer As FloatIndexer = FloatIndexer.create(pFloat)
						buffer = Nd4j.createBuffer(pFloat, FLOAT, size, floatIndexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT8
						Dim pUint8 As BytePointer = tens.GetTensorMutableDataUByte().capacity(size)
						Dim uint8Indexer As Indexer = ByteIndexer.create(pUint8)
						buffer = Nd4j.createBuffer(pUint8, DataType.UINT8, size, uint8Indexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_INT8
						Dim pInt8 As BytePointer = tens.GetTensorMutableDataByte().capacity(size)
						Dim int8Indexer As Indexer = ByteIndexer.create(pInt8)
						buffer = Nd4j.createBuffer(pInt8, DataType.UINT8, size, int8Indexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT16
						Dim pUint16 As ShortPointer = tens.GetTensorMutableDataUShort().capacity(size)
						Dim uint16Indexer As Indexer = ShortIndexer.create(pUint16)
						buffer = Nd4j.createBuffer(pUint16, UINT16, size, uint16Indexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_INT16
						Dim pInt16 As ShortPointer = tens.GetTensorMutableDataShort().capacity(size)
						Dim int16Indexer As Indexer = ShortIndexer.create(pInt16)
						buffer = Nd4j.createBuffer(pInt16, INT16, size, int16Indexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_INT32
						Dim pInt32 As IntPointer = tens.GetTensorMutableDataInt().capacity(size)
						Dim int32Indexer As Indexer = IntIndexer.create(pInt32)
						buffer = Nd4j.createBuffer(pInt32, DataType.INT32, size, int32Indexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_INT64
						Dim pInt64 As LongPointer = tens.GetTensorMutableDataLong().capacity(size)
						Dim int64Indexer As Indexer = LongIndexer.create(pInt64)
						buffer = Nd4j.createBuffer(pInt64, DataType.INT64, size, int64Indexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_STRING
						Dim pString As BytePointer = tens.GetTensorMutableDataByte().capacity(size)
						Dim stringIndexer As Indexer = ByteIndexer.create(pString)
						buffer = Nd4j.createBuffer(pString, DataType.INT8, size, stringIndexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_BOOL
						Dim pBool As BoolPointer = tens.GetTensorMutableDataBool().capacity(size)
						Dim boolIndexer As Indexer = BooleanIndexer.create(New BooleanPointer(pBool)) 'Converting from JavaCPP Bool to Boolean here - C++ bool type size is not defined, could cause problems on some platforms
						buffer = Nd4j.createBuffer(pBool, BOOL, size, boolIndexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_FLOAT16
						Dim pFloat16 As ShortPointer = tens.GetTensorMutableDataShort().capacity(size)
						Dim float16Indexer As Indexer = ShortIndexer.create(pFloat16)
						buffer = Nd4j.createBuffer(pFloat16, DataType.FLOAT16, size, float16Indexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_DOUBLE
						Dim pDouble As DoublePointer = tens.GetTensorMutableDataDouble().capacity(size)
						Dim doubleIndexer As Indexer = DoubleIndexer.create(pDouble)
						buffer = Nd4j.createBuffer(pDouble, [DOUBLE], size, doubleIndexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT32
						Dim pUint32 As IntPointer = tens.GetTensorMutableDataUInt().capacity(size)
						Dim uint32Indexer As Indexer = IntIndexer.create(pUint32)
						buffer = Nd4j.createBuffer(pUint32, UINT32, size, uint32Indexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_UINT64
						Dim pUint64 As LongPointer = tens.GetTensorMutableDataULong().capacity(size)
						Dim uint64Indexer As Indexer = LongIndexer.create(pUint64)
						buffer = Nd4j.createBuffer(pUint64, UINT64, size, uint64Indexer)
					Case ONNX_TENSOR_ELEMENT_DATA_TYPE_BFLOAT16
						Dim pBfloat16 As ShortPointer = tens.GetTensorMutableDataShort().capacity(size)
						Dim bfloat16Indexer As Indexer = ShortIndexer.create(pBfloat16)
						buffer = Nd4j.createBuffer(pBfloat16, BFLOAT16, size, bfloat16Indexer)
					Case Else
						Throw New Exception("Unsupported data type encountered")
				End Select
				Return buffer
			End Using
		End Function

	End Class

End Namespace