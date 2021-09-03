Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports IntPointer = org.bytedeco.javacpp.IntPointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports AbstractCompressor = org.nd4j.compression.impl.AbstractCompressor
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeEx = org.nd4j.linalg.api.buffer.DataTypeEx
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports CompressionDescriptor = org.nd4j.linalg.compression.CompressionDescriptor
Imports CompressionType = org.nd4j.linalg.compression.CompressionType
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions

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

Namespace org.nd4j.linalg.cpu.nativecpu.compression

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CpuThreshold extends org.nd4j.compression.impl.AbstractCompressor
	Public Class CpuThreshold
		Inherits AbstractCompressor

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter protected float threshold = 1e-3f;
		Protected Friend threshold As Single = 1e-3f

		''' <summary>
		''' This method returns compression descriptor. It should be unique for any compressor implementation
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property Descriptor As String
			Get
				Return "THRESHOLD"
			End Get
		End Property

		''' <summary>
		''' This method allows you to configure threshold for delta extraction. Pass it as float/double value
		''' 
		''' Default value: 1e-3 </summary>
		''' <param name="vars"> </param>
		Public Overrides Sub configure(ParamArray ByVal vars() As Object)
			If TypeOf vars(0) Is Number Then
				Dim t As Number = DirectCast(vars(0), Number)
				threshold = FastMath.abs(t.floatValue())
				log.info("Setting threshold to [{}]", threshold)
			Else
				Throw New ND4JIllegalStateException("Threshold value should be Number")
			End If
		End Sub

		Public Overrides Function compress(ByVal array As INDArray) As INDArray
			'logger.info("Threshold [{}] compression", threshold);

			Nd4j.Executioner.commit()
			Nd4j.AffinityManager.ensureLocation(array, AffinityManager.Location.HOST)

			Dim buffer As DataBuffer = compress(array.data())
			If buffer Is Nothing Then
				Return Nothing
			End If

			Dim dup As INDArray = Nd4j.createArrayFromShapeBuffer(buffer, array.shapeInfoDataBuffer())
			dup.markAsCompressed(True)

			Return dup
		End Function

		Public Overrides ReadOnly Property CompressionType As CompressionType
			Get
				Return CompressionType.LOSSLESS
			End Get
		End Property

		Public Overrides Function decompress(ByVal buffer As DataBuffer, ByVal dataType As DataType) As DataBuffer


			Dim result As DataBuffer = Nd4j.NDArrayFactory.convertDataEx(DataTypeEx.THRESHOLD, buffer, GlobalTypeEx)

			Return result
		End Function

		Public Overrides Function compress(ByVal buffer As DataBuffer) As DataBuffer
			Dim temp As INDArray = Nd4j.createArrayFromShapeBuffer(buffer, Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){1, buffer.length()}, buffer.dataType()).First)
			Dim condition As New MatchCondition(temp, Conditions.absGreaterThanOrEqual(threshold))
			Dim cntAbs As Integer = Nd4j.Executioner.exec(condition).getInt(0)


			'log.info("density ratio: {}", String.format("%.2f", cntAbs * 100.0f / buffer.length()));

			If cntAbs < 2 Then
				Return Nothing
			End If

			Dim originalLength As Long = buffer.length() * Nd4j.sizeOfDataType(buffer.dataType())
			Dim compressedLength As Integer = cntAbs + 4
			' first 3 elements contain header
			Dim pointer As New IntPointer(compressedLength)
			pointer.put(0, cntAbs)
			pointer.put(1, CInt(buffer.length()))
			pointer.put(2, Float.floatToIntBits(threshold))
			pointer.put(3, 0)

			Dim descriptor As New CompressionDescriptor()
			descriptor.setCompressedLength(compressedLength * 4) ' sizeOf(INT)
			descriptor.setOriginalLength(originalLength)
			descriptor.setOriginalElementSize(Nd4j.sizeOfDataType(buffer.dataType()))
			descriptor.setNumberOfElements(buffer.length())

			descriptor.setCompressionAlgorithm(Me.Descriptor)
			descriptor.setCompressionType(CompressionType)



			Dim cbuff As New CompressedDataBuffer(pointer, descriptor)

			Nd4j.NDArrayFactory.convertDataEx(getBufferTypeEx(buffer), buffer.addressPointer(), DataTypeEx.THRESHOLD, pointer, buffer.length())

			Nd4j.AffinityManager.tagLocation(buffer, AffinityManager.Location.HOST)

			Return cbuff
		End Function

		Protected Friend Overrides Function compressPointer(ByVal srcType As DataTypeEx, ByVal srcPointer As Pointer, ByVal length As Integer, ByVal elementSize As Integer) As CompressedDataBuffer
			Throw New System.NotSupportedException()
		End Function
	End Class

End Namespace