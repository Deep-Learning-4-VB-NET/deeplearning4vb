Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.nd4j.linalg.compression

	Public Interface NDArrayCompressor

		''' <summary>
		''' This method returns compression descriptor.
		''' It should be
		''' unique for any compressor implementation
		''' @return
		''' </summary>
		ReadOnly Property Descriptor As String

		''' <summary>
		''' This method allows to pass compressor-dependent configuration options to this compressor
		''' 
		''' PLEASE NOTE: Each compressor has own options, please check corresponding implementations javadoc </summary>
		''' <param name="vars"> </param>
		Sub configure(ParamArray ByVal vars() As Object)

		''' <summary>
		''' This method returns compression opType provided
		''' by specific NDArrayCompressor implementation
		''' @return
		''' </summary>
		ReadOnly Property CompressionType As CompressionType

		''' <summary>
		''' This method returns compressed copy of referenced array
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Function compress(ByVal array As INDArray) As INDArray

		''' <summary>
		''' Inplace compression of INDArray
		''' </summary>
		''' <param name="array"> </param>
		Sub compressi(ByVal array As INDArray)

		''' 
		''' <param name="buffer">
		''' @return </param>
		Function compress(ByVal buffer As DataBuffer) As DataBuffer

		''' <summary>
		''' This method returns
		''' decompressed copy of referenced array
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Function decompress(ByVal array As INDArray) As INDArray

		''' <summary>
		''' Inplace decompression of INDArray
		''' </summary>
		''' <param name="array"> </param>
		Sub decompressi(ByVal array As INDArray)

		''' <summary>
		''' Return a compressed databuffer </summary>
		''' <param name="buffer"> the buffer to decompress </param>
		''' <returns> the decompressed data buffer </returns>
		Function decompress(ByVal buffer As DataBuffer, ByVal targetType As DataType) As DataBuffer

		''' <summary>
		''' This method creates compressed INDArray from Java float array, skipping usual INDArray instantiation routines
		''' Please note: This method compresses input data as vector
		''' </summary>
		''' <param name="data">
		''' @return </param>
		Function compress(ByVal data() As Single) As INDArray

		''' <summary>
		''' This method creates compressed INDArray from Java double array, skipping usual INDArray instantiation routines
		''' Please note: This method compresses input data as vector
		''' </summary>
		''' <param name="data">
		''' @return </param>
		Function compress(ByVal data() As Double) As INDArray

		''' <summary>
		''' This method creates compressed INDArray from Java float array, skipping usual INDArray instantiation routines
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="shape">
		''' @return </param>
		Function compress(ByVal data() As Single, ByVal shape() As Integer, ByVal order As Char) As INDArray

		''' <summary>
		''' This method creates compressed INDArray from Java double array, skipping usual INDArray instantiation routines
		''' </summary>
		''' <param name="data"> </param>
		''' <param name="shape">
		''' @return </param>
		Function compress(ByVal data() As Double, ByVal shape() As Integer, ByVal order As Char) As INDArray
	End Interface

End Namespace