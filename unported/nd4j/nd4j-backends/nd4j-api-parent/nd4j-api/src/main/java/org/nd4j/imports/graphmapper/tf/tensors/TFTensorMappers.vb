Imports System
Imports Bfloat16ArrayIndexer = org.bytedeco.javacpp.indexer.Bfloat16ArrayIndexer
Imports HalfIndexer = org.bytedeco.javacpp.indexer.HalfIndexer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports TensorProto = org.tensorflow.framework.TensorProto

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

Namespace org.nd4j.imports.graphmapper.tf.tensors


	Public Class TFTensorMappers

		Private Sub New()
		End Sub


'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: public static TFTensorMapper<?,?> newMapper(org.tensorflow.framework.TensorProto tp)
		Public Shared Function newMapper(ByVal tp As TensorProto) As TFTensorMapper(Of Object, Object)

			Select Case tp.getDtype()
				Case DT_HALF
					Return New Float16TensorMapper(tp)
				Case DT_FLOAT
					Return New Float32TensorMapper(tp)
				Case DT_DOUBLE
					Return New Float64TensorMapper(tp)
				Case DT_BFLOAT16
					Return New BFloat16TensorMapper(tp)

				Case DT_INT8
					Return New Int8TensorMapper(tp)
				Case DT_INT16
					Return New Int16TensorMapper(tp)
				Case DT_INT32
					Return New Int32TensorMapper(tp)
				Case DT_INT64
					Return New Int64TensorMapper(tp)


				Case DT_STRING
					Return New StringTensorMapper(tp)

				Case DT_BOOL
					Return New BoolTensorMapper(tp)

				Case DT_UINT8
					Return New UInt8TensorMapper(tp)
				Case DT_UINT16
					Return New UInt16TensorMapper(tp)
				Case DT_UINT32
					Return New UInt32TensorMapper(tp)
				Case DT_UINT64
					Return New UInt64TensorMapper(tp)

				Case DT_QINT8, DT_QUINT8, DT_QINT32, DT_QINT16, DT_QUINT16
					Throw New System.InvalidOperationException("Unable to map quantized type: " & tp.getDtype())
				Case DT_COMPLEX64, DT_COMPLEX128
					Throw New System.InvalidOperationException("Unable to map complex type: " & tp.getDtype())
				Case DT_FLOAT_REF, DT_DOUBLE_REF, DT_INT32_REF, DT_UINT8_REF, DT_INT16_REF, DT_INT8_REF, DT_STRING_REF, DT_COMPLEX64_REF, DT_INT64_REF, DT_BOOL_REF, DT_QINT8_REF, DT_QUINT8_REF, DT_QINT32_REF, DT_BFLOAT16_REF, DT_QINT16_REF, DT_QUINT16_REF, DT_UINT16_REF, DT_COMPLEX128_REF, DT_HALF_REF, DT_RESOURCE_REF, DT_VARIANT_REF, DT_UINT32_REF, DT_UINT64_REF
					Throw New System.InvalidOperationException("Unable to map reference type: " & tp.getDtype())
				Case Else
					Throw New System.InvalidOperationException("Unable to map type: " & tp.getDtype())
			End Select
		End Function


		Public MustInherit Class BaseTensorMapper(Of T, U As Buffer)
			Implements TFTensorMapper(Of T, U)

			Public MustOverride Function arrayFor(ByVal shape() As Long, ByVal jArr As J) As INDArray
			Public MustOverride Sub getValue(ByVal jArr As J, ByVal buffer As B, ByVal i As Integer)
			Public MustOverride Sub getValue(ByVal jArr As J, ByVal i As Integer)
			Public MustOverride Function getBuffer(ByVal bb As java.nio.ByteBuffer) As java.nio.Buffer
			Public MustOverride Function newArray(ByVal length As Integer) As J
			Public MustOverride Function valueCount() As Integer Implements TFTensorMapper(Of T, U).valueCount

			Protected Friend tfTensor As TensorProto

			Public Sub New(ByVal tensorProto As TensorProto)
				Me.tfTensor = tensorProto
			End Sub

			Public Overridable Function dataType() As DataType Implements TFTensorMapper(Of T, U).dataType
				Return ArrayOptionsHelper.convertToDataType(tfTensor.getDtype())
			End Function

			Public Overridable Function shape() As Long() Implements TFTensorMapper(Of T, U).shape
				Dim dims As Integer = tfTensor.getTensorShape().getDimCount()
				Dim arrayShape(dims - 1) As Long
				For e As Integer = 0 To dims - 1
					arrayShape(e) = tfTensor.getTensorShape().getDim(e).getSize()
				Next e
				Return arrayShape
			End Function

			Public Overridable ReadOnly Property Empty As Boolean Implements TFTensorMapper(Of T, U).isEmpty
				Get
					Return valueSource() = ValueSource.EMPTY
				End Get
			End Property

			Public Overridable Function valueSource() As ValueSource Implements TFTensorMapper(Of T, U).valueSource
				If valueCount() > 0 Then
					Return ValueSource.VALUE_COUNT
				End If
				If tfTensor.getTensorContent() IsNot Nothing AndAlso tfTensor.getTensorContent().size() > 0 Then
					Return ValueSource.BINARY
				End If

				Return ValueSource.EMPTY
			End Function

			Public Overridable Function toNDArray() As INDArray Implements TFTensorMapper(Of T, U).toNDArray
				Dim dt As DataType = dataType()
				Dim vs As ValueSource = valueSource()
				Dim shape() As Long = Me.shape()

				Dim [out] As INDArray
				Select Case vs
					Case org.nd4j.imports.graphmapper.tf.tensors.TFTensorMapper.ValueSource.EMPTY
						[out] = Nd4j.create(dt, shape)
					Case org.nd4j.imports.graphmapper.tf.tensors.TFTensorMapper.ValueSource.VALUE_COUNT
						Dim n As Integer = valueCount()
						Dim array As T = newArray(n)
						For i As Integer = 0 To n - 1
							getValue(array, i)
						Next i
						[out] = arrayFor(shape, array)
					Case org.nd4j.imports.graphmapper.tf.tensors.TFTensorMapper.ValueSource.BINARY
						Dim buffer As U = getBuffer(tfTensor.getTensorContent().asReadOnlyByteBuffer().order(ByteOrder.nativeOrder()))
						Dim m As Integer = buffer.capacity()
						Dim array2 As T = newArray(m)
						For i As Integer = 0 To m - 1
							getValue(array2, buffer, i)
						Next i
						[out] = arrayFor(shape, array2)
					Case Else
						Throw New Exception("Error converting TF tensor to INDArray")
				End Select

				Return [out]
			End Function
		End Class

		Public Class Float16TensorMapper
			Inherits BaseTensorMapper(Of Single(), Buffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				Return tfTensor.getHalfValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Single()
				Return New Single(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As Buffer
				Throw New System.NotSupportedException("Not yet implemnted: FP16 reading from buffer")
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Single, ByVal i As Integer)
				Dim asIntBytes As Integer = tfTensor.getHalfVal(i)
				jArr(i) = HalfIndexer.toFloat(asIntBytes)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Single, ByVal buffer As Buffer, ByVal i As Integer)
				Throw New System.NotSupportedException("Not yet implemented: FP16 reading from buffer")
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Single) As INDArray
				'Edge case: sometimes tf has single float value for entire array (getFloatValCount() == 1)
				If jArr.Length = 1 AndAlso ArrayUtil.prod(shape) > 1 Then
					Return Nd4j.createUninitialized(DataType.HALF, shape).assign(jArr(0))
				End If
				Return Nd4j.create(jArr, shape, "c"c).castTo(DataType.HALF)
			End Function
		End Class

		Public Class Float32TensorMapper
			Inherits BaseTensorMapper(Of Single(), FloatBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				Return tfTensor.getFloatValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Single()
				Return New Single(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As FloatBuffer
				Return bb.asFloatBuffer()
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Single, ByVal i As Integer)
				jArr(i) = tfTensor.getFloatVal(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Single, ByVal buffer As FloatBuffer, ByVal i As Integer)
				jArr(i) = buffer.get(i)
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Single) As INDArray
				'Edge case: sometimes tf has single float value for entire array (getFloatValCount() == 1)
				If jArr.Length = 1 AndAlso ArrayUtil.prod(shape) > 1 Then
					Return Nd4j.valueArrayOf(shape, jArr(0))
				End If
				Return Nd4j.create(jArr, shape, "c"c)
			End Function
		End Class

		Public Class Float64TensorMapper
			Inherits BaseTensorMapper(Of Double(), DoubleBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				Return tfTensor.getDoubleValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Double()
				Return New Double(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As DoubleBuffer
				Return bb.asDoubleBuffer()
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Double, ByVal i As Integer)
				jArr(i) = tfTensor.getDoubleVal(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Double, ByVal buffer As DoubleBuffer, ByVal i As Integer)
				jArr(i) = buffer.get(i)
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Double) As INDArray
				'Edge case: sometimes tf has double float value for entire array (getDoubleValCount() == 1)
				If jArr.Length = 1 AndAlso ArrayUtil.prod(shape) > 1 Then
					Return Nd4j.valueArrayOf(shape, jArr(0))
				End If
				Return Nd4j.create(jArr, shape, "c"c)
			End Function
		End Class

		Public Class BFloat16TensorMapper
			Inherits BaseTensorMapper(Of Single(), ShortBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				Return tfTensor.getHalfValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Single()
				Return New Single(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As ShortBuffer
				Return bb.asShortBuffer()
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Single, ByVal i As Integer)
				Dim asIntBytes As Integer = tfTensor.getHalfVal(i)
				jArr(i) = Bfloat16ArrayIndexer.toFloat(asIntBytes)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Single, ByVal buffer As ShortBuffer, ByVal i As Integer)
				Throw New System.NotSupportedException("Not yet implemnted: BFP16 reading from buffer")
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Single) As INDArray
				'Edge case: sometimes tf has single float value for entire array (getFloatValCount() == 1)
				If jArr.Length = 1 AndAlso ArrayUtil.prod(shape) > 1 Then
					Return Nd4j.createUninitialized(DataType.HALF, shape).assign(jArr(0))
				End If
				Return Nd4j.create(jArr, shape, "c"c).castTo(DataType.BFLOAT16)
			End Function
		End Class

		'Note TF stortes bytes as integer (other than when in a biffer)
		Public Class Int8TensorMapper
			Inherits BaseTensorMapper(Of Integer(), ByteBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				'int8 as integer
				Return tfTensor.getIntValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Integer()
				Return New Integer(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As ByteBuffer
				Return bb
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal i As Integer)
				jArr(i) = tfTensor.getIntVal(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal buffer As ByteBuffer, ByVal i As Integer)
				jArr(i) = buffer.get(i)
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Integer) As INDArray
				Dim dt As DataType = dataType()
				Return Nd4j.create(Nd4j.createTypedBuffer(jArr, dt), shape,Nd4j.getStrides(shape, "c"c), 0, "c"c, dt)
			End Function
		End Class

		Public Class Int16TensorMapper
			Inherits BaseTensorMapper(Of Integer(), ShortBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				'Shorts as integer
				Return tfTensor.getIntValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Integer()
				Return New Integer(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As ShortBuffer
				Return bb.asShortBuffer()
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal i As Integer)
				jArr(i) = tfTensor.getIntVal(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal buffer As ShortBuffer, ByVal i As Integer)
				jArr(i) = buffer.get(i)
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Integer) As INDArray
				Dim dt As DataType = dataType()
				Return Nd4j.create(Nd4j.createTypedBuffer(jArr, dt), shape,Nd4j.getStrides(shape, "c"c), 0, "c"c, dt)
			End Function
		End Class


		Public Class Int32TensorMapper
			Inherits BaseTensorMapper(Of Integer(), IntBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				Return tfTensor.getIntValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Integer()
				Return New Integer(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As IntBuffer
				Return bb.asIntBuffer()
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal i As Integer)
				jArr(i) = tfTensor.getIntVal(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal buffer As IntBuffer, ByVal i As Integer)
				jArr(i) = buffer.get(i)
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Integer) As INDArray
				Dim dt As DataType = dataType()
				Return Nd4j.create(Nd4j.createTypedBuffer(jArr, dt), shape,Nd4j.getStrides(shape, "c"c), 0, "c"c, dt)
			End Function
		End Class

		Public Class Int64TensorMapper
			Inherits BaseTensorMapper(Of Long(), LongBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				Return tfTensor.getInt64ValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Long()
				Return New Long(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As LongBuffer
				Return bb.asLongBuffer()
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Long, ByVal i As Integer)
				jArr(i) = tfTensor.getInt64Val(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Long, ByVal buffer As LongBuffer, ByVal i As Integer)
				jArr(i) = buffer.get(i)
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Long) As INDArray
				Dim dt As DataType = dataType()
				Return Nd4j.create(Nd4j.createTypedBuffer(jArr, dt), shape,Nd4j.getStrides(shape, "c"c), 0, "c"c, dt)
			End Function
		End Class

		'Note TF stortes bytes as integer (other than when in a buffer)
		Public Class UInt8TensorMapper
			Inherits BaseTensorMapper(Of Integer(), ByteBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				'int8 as integer
				Return tfTensor.getIntValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Integer()
				Return New Integer(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As ByteBuffer
				Return bb
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal i As Integer)
				jArr(i) = tfTensor.getIntVal(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal buffer As ByteBuffer, ByVal i As Integer)
				Dim b As SByte = buffer.get(i) 'Signed, but bytes are really for unsigned...
				jArr(i) = b And &Hff
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Integer) As INDArray
				Dim dt As DataType = dataType()
				Return Nd4j.create(Nd4j.createTypedBuffer(jArr, dt), shape,Nd4j.getStrides(shape, "c"c), 0, "c"c, dt)
			End Function
		End Class

		Public Class UInt16TensorMapper
			Inherits BaseTensorMapper(Of Integer(), ShortBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				'int8 as integer
				Return tfTensor.getIntValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Integer()
				Return New Integer(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As ShortBuffer
				Return bb.asShortBuffer()
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal i As Integer)
				jArr(i) = tfTensor.getIntVal(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Integer, ByVal buffer As ShortBuffer, ByVal i As Integer)
				Dim b As Short = buffer.get(i) 'Signed, but bytes are really for unsigned...
				jArr(i) = b And &Hffff
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Integer) As INDArray
				Dim dt As DataType = dataType()
				Return Nd4j.create(Nd4j.createTypedBuffer(jArr, dt), shape,Nd4j.getStrides(shape, "c"c), 0, "c"c, dt)
			End Function
		End Class

		Public Class UInt32TensorMapper
			Inherits BaseTensorMapper(Of Long(), IntBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				'int8 as integer
				Return tfTensor.getInt64ValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Long()
				Return New Long(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As IntBuffer
				Return bb.asIntBuffer()
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Long, ByVal i As Integer)
				jArr(i) = tfTensor.getInt64Val(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Long, ByVal buffer As IntBuffer, ByVal i As Integer)
				Dim b As Integer = buffer.get(i) 'Signed, but bytes are really for unsigned...
				jArr(i) = b And &HffffffffL
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Long) As INDArray
				Dim dt As DataType = dataType()
				Return Nd4j.create(Nd4j.createTypedBuffer(jArr, dt), shape,Nd4j.getStrides(shape, "c"c), 0, "c"c, dt)
			End Function
		End Class

		Public Class UInt64TensorMapper
			Inherits BaseTensorMapper(Of Long(), LongBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				'int8 as integer
				Return tfTensor.getInt64ValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Long()
				Return New Long(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As LongBuffer
				Return bb.asLongBuffer()
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Long, ByVal i As Integer)
				'TODO out of range for largest values!
				jArr(i) = tfTensor.getInt64Val(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Long, ByVal buffer As LongBuffer, ByVal i As Integer)
				'TODO out of range for largest values!
				jArr(i) = buffer.get(i)
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Long) As INDArray
				Dim dt As DataType = dataType()
				Return Nd4j.create(Nd4j.createTypedBuffer(jArr, dt), shape,Nd4j.getStrides(shape, "c"c), 0, "c"c, dt)
			End Function
		End Class


		Public Class StringTensorMapper
			Inherits BaseTensorMapper(Of String(), ByteBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				Return tfTensor.getStringValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As String()
				Return New String(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As ByteBuffer
				Throw New System.NotSupportedException("Not supported for String types")
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As String, ByVal i As Integer)
				jArr(i) = tfTensor.getStringVal(i).toStringUtf8()
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As String, ByVal buffer As ByteBuffer, ByVal i As Integer)
				Throw New System.NotSupportedException("Not supported for String types")
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As String) As INDArray
				Return Nd4j.create(jArr).reshape(shape)
			End Function
		End Class

		Public Class BoolTensorMapper
			Inherits BaseTensorMapper(Of Boolean(), ByteBuffer)

			Public Sub New(ByVal tensorProto As TensorProto)
				MyBase.New(tensorProto)
			End Sub

			Public Overrides Function valueCount() As Integer
				Return tfTensor.getBoolValCount()
			End Function

			Public Overrides Function newArray(ByVal length As Integer) As Boolean()
				Return New Boolean(length - 1){}
			End Function

			Public Overrides Function getBuffer(ByVal bb As ByteBuffer) As ByteBuffer
				Throw New System.NotSupportedException("Not supported for String types")
			End Function

			Public Overridable Overloads Sub getValue(ByVal jArr() As Boolean, ByVal i As Integer)
				jArr(i) = tfTensor.getBoolVal(i)
			End Sub

			Public Overridable Overloads Sub getValue(ByVal jArr() As Boolean, ByVal buffer As ByteBuffer, ByVal i As Integer)
				Throw New System.NotSupportedException("Not supported for boolean types")
			End Sub

			Public Overridable Overloads Function arrayFor(ByVal shape() As Long, ByVal jArr() As Boolean) As INDArray
				Return Nd4j.create(jArr).reshape(shape)
			End Function
		End Class
	End Class

End Namespace