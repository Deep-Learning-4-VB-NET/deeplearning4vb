Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports ND4JUnknownDataTypeException = org.nd4j.linalg.exception.ND4JUnknownDataTypeException

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

Namespace org.nd4j.linalg.api.shape.options

	Public Class ArrayOptionsHelper
		Public Const ATYPE_SPARSE_BIT As Long = 2
		Public Const ATYPE_COMPRESSED_BIT As Long = 4
		Public Const ATYPE_EMPTY_BIT As Long = 8

		Public Const DTYPE_COMPRESSED_BIT As Long = 4
		Public Const DTYPE_BFLOAT16_BIT As Long = 2048
		Public Const DTYPE_HALF_BIT As Long = 4096
		Public Const DTYPE_FLOAT_BIT As Long = 8192
		Public Const DTYPE_DOUBLE_BIT As Long = 16384
		Public Const DTYPE_INT_BIT As Long = 131072
		Public Const DTYPE_LONG_BIT As Long = 262144
		Public Const DTYPE_BOOL_BIT As Long = 524288
		Public Const DTYPE_BYTE_BIT As Long = 32768 'Also used for UBYTE in conjunction with sign bit
		Public Const DTYPE_SHORT_BIT As Long = 65536
		Public Const DTYPE_UTF8_BIT As Long = 1048576
		Public Const DTYPE_UNSIGNED_BIT As Long = 8388608

		Public Shared ReadOnly HAS_PADDED_BUFFER As Long = (1<<25)

		Public Shared Function hasBitSet(ByVal shapeInfo() As Long, ByVal bit As Long) As Boolean
			Dim opt As val = Shape.options(shapeInfo)

			Return hasBitSet(opt, bit)
		End Function

		Public Shared Function setOptionBit(ByVal extras As Long, ByVal bit As Long) As Long
			Return extras Or bit
		End Function

		Public Shared Sub setOptionBit(ByVal storage() As Long, ByVal type As ArrayType)
			Dim length As Integer = Shape.shapeInfoLength(storage)
			storage(length - 3) = setOptionBit(storage(length - 3), type)
		End Sub

		Public Shared Function hasBitSet(ByVal storage As Long, ByVal bit As Long) As Boolean
			Return ((storage And bit) = bit)
		End Function

		Public Shared Function arrayType(ByVal shapeInfo() As Long) As ArrayType
			Dim opt As val = Shape.options(shapeInfo)

			If hasBitSet(opt, ATYPE_SPARSE_BIT) Then
				Return ArrayType.SPARSE
			ElseIf hasBitSet(opt, ATYPE_COMPRESSED_BIT) Then
				Return ArrayType.COMPRESSED
			ElseIf hasBitSet(opt, ATYPE_EMPTY_BIT) Then
				Return ArrayType.EMPTY
			Else
				Return ArrayType.DENSE
			End If
		End Function

		Public Shared Function dataType(ByVal opt As Long) As DataType
			If hasBitSet(opt, DTYPE_COMPRESSED_BIT) Then
				Return DataType.COMPRESSED
			ElseIf hasBitSet(opt, DTYPE_HALF_BIT) Then
				Return DataType.HALF
			ElseIf hasBitSet(opt, DTYPE_BFLOAT16_BIT) Then
				Return DataType.BFLOAT16
			ElseIf hasBitSet(opt, DTYPE_FLOAT_BIT) Then
				Return DataType.FLOAT
			ElseIf hasBitSet(opt, DTYPE_DOUBLE_BIT) Then
				Return DataType.DOUBLE
			ElseIf hasBitSet(opt, DTYPE_INT_BIT) Then
				Return If(hasBitSet(opt, DTYPE_UNSIGNED_BIT), DataType.UINT32, DataType.INT)
			ElseIf hasBitSet(opt, DTYPE_LONG_BIT) Then
				Return If(hasBitSet(opt, DTYPE_UNSIGNED_BIT), DataType.UINT64, DataType.LONG)
			ElseIf hasBitSet(opt, DTYPE_BOOL_BIT) Then
				Return DataType.BOOL
			ElseIf hasBitSet(opt, DTYPE_BYTE_BIT) Then
				Return If(hasBitSet(opt, DTYPE_UNSIGNED_BIT), DataType.UBYTE, DataType.BYTE) 'Byte bit set for both UBYTE and BYTE
			ElseIf hasBitSet(opt, DTYPE_SHORT_BIT) Then
				Return If(hasBitSet(opt, DTYPE_UNSIGNED_BIT), DataType.UINT16, DataType.SHORT)
			ElseIf hasBitSet(opt, DTYPE_UTF8_BIT) Then
				Return DataType.UTF8
			Else
				Throw New ND4JUnknownDataTypeException("Unknown extras set: [" & opt & "]")
			End If
		End Function

		Public Shared Function dataType(ByVal shapeInfo() As Long) As DataType
			Dim opt As val = Shape.options(shapeInfo)
			Return dataType(opt)
		End Function

		Public Shared Function setOptionBit(ByVal storage As Long, ByVal type As DataType) As Long
			Dim bit As Long = 0
			Select Case type.innerEnumValue
				Case DataType.InnerEnum.HALF
					bit = DTYPE_HALF_BIT
				Case DataType.InnerEnum.BFLOAT16
					bit = DTYPE_BFLOAT16_BIT
				Case DataType.InnerEnum.FLOAT
					bit = DTYPE_FLOAT_BIT
				Case DataType.InnerEnum.DOUBLE
					bit = DTYPE_DOUBLE_BIT
				Case DataType.InnerEnum.UINT32
					storage = storage Or DTYPE_UNSIGNED_BIT
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case DataType.InnerEnum.INT
					bit = DTYPE_INT_BIT
				Case DataType.InnerEnum.UINT64
					storage = storage Or DTYPE_UNSIGNED_BIT
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case DataType.InnerEnum.LONG
					bit = DTYPE_LONG_BIT
				Case DataType.InnerEnum.BOOL
					bit = DTYPE_BOOL_BIT
				Case DataType.InnerEnum.UBYTE
					storage = storage Or DTYPE_UNSIGNED_BIT ' unsigned bit
					'Intentional fallthrough
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case DataType.InnerEnum.BYTE
					bit = DTYPE_BYTE_BIT
				Case DataType.InnerEnum.UINT16
					storage = storage Or DTYPE_UNSIGNED_BIT
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
				Case DataType.InnerEnum.SHORT
					bit = DTYPE_SHORT_BIT
				Case DataType.InnerEnum.UTF8
					bit = DTYPE_UTF8_BIT
				Case DataType.InnerEnum.COMPRESSED
					bit = DTYPE_COMPRESSED_BIT
				Case Else
					Throw New System.NotSupportedException()
			End Select

			storage = storage Or bit
			Return storage
		End Function

		Public Shared Function setOptionBit(ByVal storage As Long, ByVal type As ArrayType) As Long
			Dim bit As Long = 0
			Select Case type
				Case org.nd4j.linalg.api.shape.options.ArrayType.SPARSE
					bit = ATYPE_SPARSE_BIT
				Case org.nd4j.linalg.api.shape.options.ArrayType.COMPRESSED
					bit = ATYPE_COMPRESSED_BIT
				Case org.nd4j.linalg.api.shape.options.ArrayType.EMPTY
					bit = ATYPE_EMPTY_BIT
				Case Else
					Return storage
			End Select

			storage = storage Or bit
			Return storage
		End Function

		Public Shared Function convertToDataType(ByVal dataType As org.tensorflow.framework.DataType) As DataType
			Select Case dataType
				Case DT_UINT16
					Return DataType.UINT16
				Case DT_UINT32
					Return DataType.UINT32
				Case DT_UINT64
					Return DataType.UINT64
				Case DT_BOOL
					Return DataType.BOOL
				Case DT_BFLOAT16
					Return DataType.BFLOAT16
				Case DT_FLOAT
					Return DataType.FLOAT
				Case DT_INT32
					Return DataType.INT
				Case DT_INT64
					Return DataType.LONG
				Case DT_INT8
					Return DataType.BYTE
				Case DT_INT16
					Return DataType.SHORT
				Case DT_DOUBLE
					Return DataType.DOUBLE
				Case DT_UINT8
					Return DataType.UBYTE
				Case DT_HALF
					Return DataType.HALF
				Case DT_STRING
					Return DataType.UTF8
				Case Else
					Throw New System.NotSupportedException("Unknown TF data type: [" & dataType.name() & "]")
			End Select
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static org.nd4j.linalg.api.buffer.DataType dataType(@NonNull String dataType)
'JAVA TO VB CONVERTER NOTE: The parameter dataType was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
		Public Shared Function dataType(ByVal dataType_Conflict As String) As DataType
			Select Case dataType_Conflict
				Case "uint64"
					Return DataType.UINT64
				Case "uint32"
					Return DataType.UINT32
				Case "uint16"
					Return DataType.UINT16
				Case "int64"
					Return DataType.LONG
				Case "int32"
					Return DataType.INT
				Case "int16"
					Return DataType.SHORT
				Case "int8"
					Return DataType.BYTE
				Case "bool"
					Return DataType.BOOL
				Case "resource", "float32" 'special case, nodes like Enter
					Return DataType.FLOAT
				Case "float64", "double"
					Return DataType.DOUBLE
				Case "string"
					Return DataType.UTF8
				Case "uint8", "ubyte"
					Return DataType.UBYTE
				Case "bfloat16"
					Return DataType.BFLOAT16
				Case "float16"
					Return DataType.HALF
				Case Else
					Throw New ND4JIllegalStateException("Unknown data type used: [" & dataType_Conflict & "]")
			End Select
		End Function

	End Class

End Namespace