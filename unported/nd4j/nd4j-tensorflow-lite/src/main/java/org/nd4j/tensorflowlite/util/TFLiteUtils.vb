Imports System
Imports org.bytedeco.javacpp
Imports org.bytedeco.javacpp.indexer
Imports org.bytedeco.tensorflowlite
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports org.bytedeco.tensorflowlite.global.tensorflowlite
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
Namespace org.nd4j.tensorflowlite.util

	Public Class TFLiteUtils

		''' <summary>
		''' Return a <seealso cref="DataType"/>
		''' for the tflite data type </summary>
		''' <param name="dataType"> the equivalent nd4j data type
		''' @return </param>
		Public Shared Function dataTypeForTfliteType(ByVal dataType As Integer) As DataType
			Select Case dataType
				Case kTfLiteFloat32
					Return FLOAT
				Case kTfLiteInt32
					Return INT32
				Case kTfLiteUInt8
					Return UINT8
				Case kTfLiteInt64
					Return INT64
				Case kTfLiteString
					Return UTF8
				Case kTfLiteBool
					Return BOOL
				Case kTfLiteInt16
					Return INT16
				Case kTfLiteInt8
					Return INT8
				Case kTfLiteFloat16
					Return FLOAT16
				Case kTfLiteFloat64
					Return [DOUBLE]
				Case kTfLiteUInt64
					Return UINT64
				Case kTfLiteUInt32
					Return UINT32
				Case Else
					Throw New System.ArgumentException("Illegal data type " & dataType)
			End Select
		End Function

		''' <summary>
		''' Convert the tflite type for the given data type </summary>
		''' <param name="dataType">
		''' @return </param>
		Public Shared Function tfliteTypeForDataType(ByVal dataType As DataType) As Integer
			Select Case dataType.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return kTfLiteFloat64
				Case DataType.InnerEnum.FLOAT
					Return kTfLiteFloat32
				Case DataType.InnerEnum.HALF
					Return kTfLiteFloat16
				Case DataType.InnerEnum.LONG
					Return kTfLiteInt64
				Case DataType.InnerEnum.INT
					Return kTfLiteInt32
				Case DataType.InnerEnum.SHORT
					Return kTfLiteInt16
				Case DataType.InnerEnum.UBYTE
					Return kTfLiteUInt8
				Case DataType.InnerEnum.BYTE
					Return kTfLiteInt8
				Case DataType.InnerEnum.BOOL
					Return kTfLiteBool
				Case DataType.InnerEnum.UTF8
					Return kTfLiteString
				Case DataType.InnerEnum.UINT32
					Return kTfLiteUInt32
				Case DataType.InnerEnum.UINT64
					Return kTfLiteUInt64
				Case Else
					Throw New System.ArgumentException("Illegal data type " & dataType)
			End Select
		End Function

		''' <summary>
		''' Convert a <seealso cref="TfLiteTensor"/>
		'''  in to an <seealso cref="INDArray"/> </summary>
		''' <param name="value"> the value to convert
		''' @return </param>
		Public Shared Function getArray(ByVal value As TfLiteTensor) As INDArray
			Dim dataType As DataType = dataTypeForTfliteType(value.type())
			Dim shape As TfLiteIntArray = value.dims()
			Dim shapeConvert() As Long
			If shape IsNot Nothing Then
				shapeConvert = New Long(shape.size() - 1){}
				For i As Integer = 0 To shapeConvert.Length - 1
					shapeConvert(i) = shape.data(i)
				Next i
			Else
				shapeConvert = New Long(){1}
			End If

			Dim getBuffer As DataBuffer = getDataBuffer(value)
			Preconditions.checkState(dataType.Equals(getBuffer.dataType()),"Data type must be equivalent as specified by the tflite metadata.")
			Return Nd4j.create(getBuffer,shapeConvert,Nd4j.getStrides(shapeConvert),0)
		End Function

		''' <summary>
		''' Get the data buffer from the given tensor </summary>
		''' <param name="tens"> the values to get </param>
		''' <returns> the equivalent data buffer </returns>
		Public Shared Function getDataBuffer(ByVal tens As TfLiteTensor) As DataBuffer
			Dim buffer As DataBuffer = Nothing
			Dim type As Integer = tens.type()
			Dim size As Long = tens.bytes()
			Dim data As Pointer = tens.data().data()
			Select Case type
				Case kTfLiteFloat32
					Dim pFloat As FloatPointer = (New FloatPointer(data)).capacity(size \ 4)
					Dim floatIndexer As FloatIndexer = FloatIndexer.create(pFloat)
					buffer = Nd4j.createBuffer(pFloat, FLOAT, size \ 4, floatIndexer)
				Case kTfLiteInt32
					Dim pInt32 As IntPointer = (New IntPointer(data)).capacity(size \ 4)
					Dim int32Indexer As Indexer = IntIndexer.create(pInt32)
					buffer = Nd4j.createBuffer(pInt32, DataType.INT32, size \ 4, int32Indexer)
				Case kTfLiteUInt8
					Dim pUint8 As BytePointer = (New BytePointer(data)).capacity(size)
					Dim uint8Indexer As Indexer = ByteIndexer.create(pUint8)
					buffer = Nd4j.createBuffer(pUint8, DataType.UINT8, size, uint8Indexer)
				Case kTfLiteInt64
					Dim pInt64 As LongPointer = (New LongPointer(data)).capacity(size \ 8)
					Dim int64Indexer As Indexer = LongIndexer.create(pInt64)
					buffer = Nd4j.createBuffer(pInt64, DataType.INT64, size \ 8, int64Indexer)
				Case kTfLiteString
					Dim pString As BytePointer = (New BytePointer(data)).capacity(size)
					Dim stringIndexer As Indexer = ByteIndexer.create(pString)
					buffer = Nd4j.createBuffer(pString, DataType.INT8, size, stringIndexer)
				Case kTfLiteBool
					Dim pBool As BoolPointer = (New BoolPointer(data)).capacity(size)
					Dim boolIndexer As Indexer = BooleanIndexer.create(New BooleanPointer(pBool)) 'Converting from JavaCPP Bool to Boolean here - C++ bool type size is not defined, could cause problems on some platforms
					buffer = Nd4j.createBuffer(pBool, BOOL, size, boolIndexer)
				Case kTfLiteInt16
					Dim pInt16 As ShortPointer = (New ShortPointer(data)).capacity(size \ 2)
					Dim int16Indexer As Indexer = ShortIndexer.create(pInt16)
					buffer = Nd4j.createBuffer(pInt16, INT16, size \ 2, int16Indexer)
				Case kTfLiteInt8
					Dim pInt8 As BytePointer = (New BytePointer(data)).capacity(size)
					Dim int8Indexer As Indexer = ByteIndexer.create(pInt8)
					buffer = Nd4j.createBuffer(pInt8, DataType.UINT8, size, int8Indexer)
				Case kTfLiteFloat16
					Dim pFloat16 As ShortPointer = (New ShortPointer(data)).capacity(size \ 2)
					Dim float16Indexer As Indexer = ShortIndexer.create(pFloat16)
					buffer = Nd4j.createBuffer(pFloat16, DataType.FLOAT16, size \ 2, float16Indexer)
				Case kTfLiteFloat64
					Dim pDouble As DoublePointer = (New DoublePointer(data)).capacity(size \ 8)
					Dim doubleIndexer As Indexer = DoubleIndexer.create(pDouble)
					buffer = Nd4j.createBuffer(pDouble, [DOUBLE], size \ 8, doubleIndexer)
				Case kTfLiteUInt64
					Dim pUint64 As LongPointer = (New LongPointer(data)).capacity(size \ 8)
					Dim uint64Indexer As Indexer = LongIndexer.create(pUint64)
					buffer = Nd4j.createBuffer(pUint64, UINT64, size \ 8, uint64Indexer)
				Case kTfLiteUInt32
					Dim pUint32 As IntPointer = (New IntPointer(data)).capacity(size \ 4)
					Dim uint32Indexer As Indexer = IntIndexer.create(pUint32)
					buffer = Nd4j.createBuffer(pUint32, UINT32, size \ 4, uint32Indexer)
				Case Else
					Throw New Exception("Unsupported data type encountered")
			End Select
			Return buffer
		End Function

	End Class

End Namespace