Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DoublePointer = org.bytedeco.javacpp.DoublePointer
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeEx = org.nd4j.linalg.api.buffer.DataTypeEx
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports NDArrayCompressor = org.nd4j.linalg.compression.NDArrayCompressor
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

Namespace org.nd4j.compression.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class AbstractCompressor implements org.nd4j.linalg.compression.NDArrayCompressor
	Public MustInherit Class AbstractCompressor
		Implements NDArrayCompressor

		Public MustOverride ReadOnly Property CompressionType As CompressionType Implements NDArrayCompressor.getCompressionType
		Public MustOverride ReadOnly Property Descriptor As String Implements NDArrayCompressor.getDescriptor
		Public Overridable Function compress(ByVal array As INDArray) As INDArray Implements NDArrayCompressor.compress
			Dim dup As INDArray = array.dup(array.ordering())

			Nd4j.Executioner.commit()

			dup.Data = compress(dup.data())
			dup.markAsCompressed(True)

			Return dup
		End Function

		''' <summary>
		''' This method has no effect in this compressor
		''' </summary>
		''' <param name="vars"> </param>
		Public Overridable Sub configure(ParamArray ByVal vars() As Object) Implements NDArrayCompressor.configure
			' no-op
		End Sub

		''' <summary>
		''' Inplace compression of INDArray
		''' </summary>
		''' <param name="array"> </param>
		Public Overridable Sub compressi(ByVal array As INDArray) Implements NDArrayCompressor.compressi
			' TODO: lift this restriction
			If array.View Then
				Throw New System.NotSupportedException("Impossible to apply inplace compression on View")
			End If

			array.Data = compress(array.data())
			array.markAsCompressed(True)
		End Sub

		Public Overridable Sub decompressi(ByVal array As INDArray) Implements NDArrayCompressor.decompressi
			If Not array.Compressed Then
				Return
			End If

			array.markAsCompressed(False)
			array.Data = decompress(array.data(), DirectCast(array.data(), CompressedDataBuffer).getCompressionDescriptor().getOriginalDataType())
		End Sub

		Public Overridable Function decompress(ByVal array As INDArray) As INDArray Implements NDArrayCompressor.decompress
			If Not array.Compressed Then
				Return array
			End If

			Dim descriptor As val = DirectCast(array.data(), CompressedDataBuffer).getCompressionDescriptor()
			Dim buffer As val = decompress(array.data(), descriptor.getOriginalDataType())
			Dim shapeInfo As val = array.shapeInfoDataBuffer()
			Dim rest As INDArray = Nd4j.createArrayFromShapeBuffer(buffer, shapeInfo)

			Return rest
		End Function

		Public MustOverride Function decompress(ByVal buffer As DataBuffer, ByVal dataType As DataType) As DataBuffer Implements NDArrayCompressor.decompress

		Public MustOverride Function compress(ByVal buffer As DataBuffer) As DataBuffer Implements NDArrayCompressor.compress

		Protected Friend Shared Function convertType(ByVal type As DataType) As DataTypeEx
			If type = DataType.HALF Then
				Return DataTypeEx.FLOAT16
			ElseIf type = DataType.FLOAT Then
				Return DataTypeEx.FLOAT
			ElseIf type = DataType.DOUBLE Then
				Return DataTypeEx.DOUBLE
			Else
				Throw New System.InvalidOperationException("Unknown dataType: [" & type & "]")
			End If
		End Function

		Protected Friend Overridable ReadOnly Property GlobalTypeEx As DataTypeEx
			Get
				Dim type As DataType = Nd4j.dataType()
    
				Return convertType(type)
			End Get
		End Property

		Public Shared Function getBufferTypeEx(ByVal buffer As DataBuffer) As DataTypeEx
			Dim type As DataType = buffer.dataType()

			Return convertType(type)
		End Function

		''' <summary>
		''' This method creates compressed INDArray from Java float array, skipping usual INDArray instantiation routines
		''' Please note: This method compresses input data as vector
		''' </summary>
		''' <param name="data">
		''' @return </param>
		Public Overridable Function compress(ByVal data() As Single) As INDArray Implements NDArrayCompressor.compress
			Return compress(data, New Integer() {1, data.Length}, Nd4j.order())
		End Function

		''' <summary>
		''' This method creates compressed INDArray from Java double array, skipping usual INDArray instantiation routines
		''' Please note: This method compresses input data as vector
		''' </summary>
		''' <param name="data">
		''' @return </param>
		Public Overridable Function compress(ByVal data() As Double) As INDArray Implements NDArrayCompressor.compress
			Return compress(data, New Integer() {1, data.Length}, Nd4j.order())
		End Function

		''' <summary>
		''' This method creates compressed INDArray from Java float array, skipping usual INDArray instantiation routines
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="order">
		''' @return </param>
		Public Overridable Function compress(ByVal data() As Single, ByVal shape() As Integer, ByVal order As Char) As INDArray Implements NDArrayCompressor.compress
			Dim pointer As New FloatPointer(data)

			Dim shapeInfo As DataBuffer = Nd4j.ShapeInfoProvider.createShapeInformation(ArrayUtil.toLongArray(shape), order, DataType.FLOAT).First
			Dim buffer As DataBuffer = compressPointer(DataTypeEx.FLOAT, pointer, data.Length, 4)

			Return Nd4j.createArrayFromShapeBuffer(buffer, shapeInfo)
		End Function

		''' <summary>
		''' This method creates compressed INDArray from Java double array, skipping usual INDArray instantiation routines
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="shape"> </param>
		''' <param name="order">
		''' @return </param>
		Public Overridable Function compress(ByVal data() As Double, ByVal shape() As Integer, ByVal order As Char) As INDArray Implements NDArrayCompressor.compress
			Dim pointer As New DoublePointer(data)

			Dim shapeInfo As DataBuffer = Nd4j.ShapeInfoProvider.createShapeInformation(ArrayUtil.toLongArray(shape), order, DataType.DOUBLE).First
			Dim buffer As DataBuffer = compressPointer(DataTypeEx.DOUBLE, pointer, data.Length, 8)

			Return Nd4j.createArrayFromShapeBuffer(buffer, shapeInfo)
		End Function

		Protected Friend MustOverride Function compressPointer(ByVal srcType As DataTypeEx, ByVal srcPointer As Pointer, ByVal length As Integer, ByVal elementSize As Integer) As CompressedDataBuffer
	End Class

End Namespace