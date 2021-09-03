Imports IntPointer = org.bytedeco.javacpp.IntPointer
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataTypeEx = org.nd4j.linalg.api.buffer.DataTypeEx
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CompressedDataBuffer = org.nd4j.linalg.compression.CompressedDataBuffer
Imports CompressionDescriptor = org.nd4j.linalg.compression.CompressionDescriptor
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

	Public Class CpuFlexibleThreshold
		Inherits CpuThreshold

		Public Sub New()
			MyBase.New()
			Me.threshold = 0.1f
		End Sub

		''' <summary>
		''' This method returns compression descriptor. It should be unique for any compressor implementation
		''' 
		''' @return
		''' </summary>
		Public Overrides ReadOnly Property Descriptor As String
			Get
				Return "FTHRESHOLD"
			End Get
		End Property

		''' <summary>
		''' This method allows you to configure desired sparsity/density ratio for updates. Pass it as float/double value
		''' 
		''' Default value: 0.1 </summary>
		''' <param name="vars"> </param>
		Public Overrides Sub configure(ParamArray ByVal vars() As Object)
			MyBase.configure(vars)
		End Sub


		Public Overrides Function compress(ByVal buffer As DataBuffer) As DataBuffer
			Dim temp As INDArray = Nd4j.createArrayFromShapeBuffer(buffer, Nd4j.ShapeInfoProvider.createShapeInformation(New Long(){1, buffer.length()}, DataType.INT).First)
			Dim max As Double = temp.amaxNumber().doubleValue()

			Dim cntAbs As Integer = temp.scan(Conditions.absGreaterThanOrEqual(max - (max * threshold))).intValue()

			Dim originalLength As Long = buffer.length() * Nd4j.sizeOfDataType(buffer.dataType())
			Dim compressedLength As Integer = cntAbs + 4
			' first 3 elements contain header
			Dim pointer As New IntPointer(compressedLength)
			pointer.put(0, cntAbs)
			pointer.put(1, CInt(buffer.length()))
			pointer.put(2, Float.floatToIntBits(threshold)) ' please note, this value will be ovewritten anyway
			pointer.put(3, 0)

			Dim descriptor As New CompressionDescriptor()
			descriptor.setCompressedLength(compressedLength * 4) ' sizeOf(INT)
			descriptor.setOriginalLength(originalLength)
			descriptor.setOriginalElementSize(Nd4j.sizeOfDataType(buffer.dataType()))
			descriptor.setNumberOfElements(buffer.length())

			descriptor.setCompressionAlgorithm(Me.Descriptor)
			descriptor.setCompressionType(CompressionType)

			Dim cbuff As New CompressedDataBuffer(pointer, descriptor)

			Nd4j.NDArrayFactory.convertDataEx(getBufferTypeEx(buffer), buffer.addressPointer(), DataTypeEx.FTHRESHOLD, pointer, buffer.length())

			Nd4j.AffinityManager.tagLocation(buffer, AffinityManager.Location.HOST)

			Return cbuff
		End Function
	End Class

End Namespace