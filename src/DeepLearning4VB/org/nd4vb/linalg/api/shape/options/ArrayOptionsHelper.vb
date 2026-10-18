Imports System
Imports ArrayTypeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayType
Imports DataTypeT = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports ND4VBUnknownDataTypeException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBUnknownDataTypeException
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape

Namespace org.nd4vb.linalg.api.shape.options

    ''' <summary>
    ''' Reads and writes the "extras" (options) field of the shape information: the array type bits and the data type
    ''' bits. The shape information layout is <c>[rank, shape..., stride..., extras, elementWiseStride, order]</c>.
    ''' <para>
    ''' The TensorFlow overload <c>convertToDataType(org.tensorflow.framework.DataType)</c> is fenced with
    ''' <c>#If MODEL_IMPORT</c>.
    ''' </para>
    ''' </summary>
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
        ''' <summary>Also used for UBYTE in conjunction with sign bit</summary>
        Public Const DTYPE_BYTE_BIT As Long = 32768
        Public Const DTYPE_SHORT_BIT As Long = 65536
        Public Const DTYPE_UTF8_BIT As Long = 1048576
        Public Const DTYPE_UNSIGNED_BIT As Long = 8388608

        Public Const HAS_PADDED_BUFFER As Long = 1L << 25

        Public Shared Function hasBitSet(ByVal shapeInfo() As Long, ByVal bit As Long) As Boolean
            Dim opt As Long = ShapeT.options(shapeInfo)
            Return hasBitSet(opt, bit)
        End Function

        Public Shared Function setOptionBit(ByVal extras As Long, ByVal bit As Long) As Long
            Return extras Or bit
        End Function

        Public Shared Sub setOptionBit(ByVal storage() As Long, ByVal type As ArrayTypeT)
            Dim length As Integer = ShapeT.shapeInfoLength(storage)
            storage(length - 3) = setOptionBit(storage(length - 3), type)
        End Sub

        Public Shared Function hasBitSet(ByVal storage As Long, ByVal bit As Long) As Boolean
            Return (storage And bit) = bit
        End Function

        Public Shared Function arrayType(ByVal shapeInfo() As Long) As ArrayTypeT
            Dim opt As Long = ShapeT.options(shapeInfo)
            If hasBitSet(opt, ATYPE_SPARSE_BIT) Then
                Return ArrayTypeT.SPARSE
            ElseIf hasBitSet(opt, ATYPE_COMPRESSED_BIT) Then
                Return ArrayTypeT.COMPRESSED
            ElseIf hasBitSet(opt, ATYPE_EMPTY_BIT) Then
                Return ArrayTypeT.EMPTY
            Else
                Return ArrayTypeT.DENSE
            End If
        End Function

        Public Shared Function dataType(ByVal opt As Long) As DataTypeT
            If hasBitSet(opt, DTYPE_COMPRESSED_BIT) Then
                Return DataTypeT.COMPRESSED
            ElseIf hasBitSet(opt, DTYPE_HALF_BIT) Then
                Return DataTypeT.HALF
            ElseIf hasBitSet(opt, DTYPE_BFLOAT16_BIT) Then
                Return DataTypeT.BFLOAT16
            ElseIf hasBitSet(opt, DTYPE_FLOAT_BIT) Then
                Return DataTypeT.FLOAT
            ElseIf hasBitSet(opt, DTYPE_DOUBLE_BIT) Then
                Return DataTypeT.DOUBLE
            ElseIf hasBitSet(opt, DTYPE_INT_BIT) Then
                Return If(hasBitSet(opt, DTYPE_UNSIGNED_BIT), DataTypeT.UINT32, DataTypeT.INT)
            ElseIf hasBitSet(opt, DTYPE_LONG_BIT) Then
                Return If(hasBitSet(opt, DTYPE_UNSIGNED_BIT), DataTypeT.UINT64, DataTypeT.LONG)
            ElseIf hasBitSet(opt, DTYPE_BOOL_BIT) Then
                Return DataTypeT.BOOL
            ElseIf hasBitSet(opt, DTYPE_BYTE_BIT) Then
                ' Byte bit set for both UBYTE and BYTE
                Return If(hasBitSet(opt, DTYPE_UNSIGNED_BIT), DataTypeT.UBYTE, DataTypeT.BYTE)
            ElseIf hasBitSet(opt, DTYPE_SHORT_BIT) Then
                Return If(hasBitSet(opt, DTYPE_UNSIGNED_BIT), DataTypeT.UINT16, DataTypeT.SHORT)
            ElseIf hasBitSet(opt, DTYPE_UTF8_BIT) Then
                Return DataTypeT.UTF8
            Else
                Throw New ND4VBUnknownDataTypeException("Unknown extras set: [" & opt & "]")
            End If
        End Function

        Public Shared Function dataType(ByVal shapeInfo() As Long) As DataTypeT
            Dim opt As Long = ShapeT.options(shapeInfo)
            Return dataType(opt)
        End Function

        Public Shared Function setOptionBit(ByVal storage As Long, ByVal type As DataTypeT) As Long
            Dim bit As Long = 0
            ' Java falls through from the unsigned cases into their signed counterparts
            Select Case type
                Case DataTypeT.HALF
                    bit = DTYPE_HALF_BIT
                Case DataTypeT.BFLOAT16
                    bit = DTYPE_BFLOAT16_BIT
                Case DataTypeT.FLOAT
                    bit = DTYPE_FLOAT_BIT
                Case DataTypeT.DOUBLE
                    bit = DTYPE_DOUBLE_BIT
                Case DataTypeT.UINT32
                    storage = storage Or DTYPE_UNSIGNED_BIT
                    bit = DTYPE_INT_BIT
                Case DataTypeT.INT
                    bit = DTYPE_INT_BIT
                Case DataTypeT.UINT64
                    storage = storage Or DTYPE_UNSIGNED_BIT
                    bit = DTYPE_LONG_BIT
                Case DataTypeT.LONG
                    bit = DTYPE_LONG_BIT
                Case DataTypeT.BOOL
                    bit = DTYPE_BOOL_BIT
                Case DataTypeT.UBYTE
                    storage = storage Or DTYPE_UNSIGNED_BIT ' unsigned bit
                    bit = DTYPE_BYTE_BIT
                Case DataTypeT.BYTE
                    bit = DTYPE_BYTE_BIT
                Case DataTypeT.UINT16
                    storage = storage Or DTYPE_UNSIGNED_BIT
                    bit = DTYPE_SHORT_BIT
                Case DataTypeT.SHORT
                    bit = DTYPE_SHORT_BIT
                Case DataTypeT.UTF8
                    bit = DTYPE_UTF8_BIT
                Case DataTypeT.COMPRESSED
                    bit = DTYPE_COMPRESSED_BIT
                Case Else
                    Throw New NotSupportedException()
            End Select

            storage = storage Or bit
            Return storage
        End Function

        Public Shared Function setOptionBit(ByVal storage As Long, ByVal type As ArrayTypeT) As Long
            Dim bit As Long = 0
            Select Case type
                Case ArrayTypeT.SPARSE
                    bit = ATYPE_SPARSE_BIT
                Case ArrayTypeT.COMPRESSED
                    bit = ATYPE_COMPRESSED_BIT
                Case ArrayTypeT.EMPTY
                    bit = ATYPE_EMPTY_BIT
                Case Else
                    Return storage
            End Select

            storage = storage Or bit
            Return storage
        End Function

#If MODEL_IMPORT Then
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
#End If

        ''' <summary>The data type for a TensorFlow/numpy style type name (Java <c>dataType(String)</c>).</summary>
        Public Shared Function dataType(ByVal name As String) As DataTypeT
            Select Case name
                Case "uint64"
                    Return DataTypeT.UINT64
                Case "uint32"
                    Return DataTypeT.UINT32
                Case "uint16"
                    Return DataTypeT.UINT16
                Case "int64"
                    Return DataTypeT.LONG
                Case "int32"
                    Return DataTypeT.INT
                Case "int16"
                    Return DataTypeT.SHORT
                Case "int8"
                    Return DataTypeT.BYTE
                Case "bool"
                    Return DataTypeT.BOOL
                Case "resource", "float32" 'special case, nodes like Enter
                    Return DataTypeT.FLOAT
                Case "float64", "double"
                    Return DataTypeT.DOUBLE
                Case "string"
                    Return DataTypeT.UTF8
                Case "uint8", "ubyte"
                    Return DataTypeT.UBYTE
                Case "bfloat16"
                    Return DataTypeT.BFLOAT16
                Case "float16"
                    Return DataTypeT.HALF
                Case Else
                    Throw New ND4VBIllegalStateException("Unknown data type used: [" & name & "]")
            End Select
        End Function

    End Class

End Namespace
