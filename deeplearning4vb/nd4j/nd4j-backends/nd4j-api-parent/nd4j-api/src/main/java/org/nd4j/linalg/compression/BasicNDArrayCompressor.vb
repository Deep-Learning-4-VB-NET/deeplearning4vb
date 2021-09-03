Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Text
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BasicNDArrayCompressor
	Public Class BasicNDArrayCompressor
'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New BasicNDArrayCompressor()

		Protected Friend codecs As IDictionary(Of String, NDArrayCompressor)

'JAVA TO VB CONVERTER NOTE: The field defaultCompression was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend defaultCompression_Conflict As String = "FLOAT16"

		Private Sub New()
			loadCompressors()
		End Sub

		Protected Friend Overridable Sub loadCompressors()
	'        
	'            We scan classpath for NDArrayCompressor implementations and add them one by one to codecs map
	'         
			codecs = New ConcurrentDictionary(Of String, NDArrayCompressor)()

			Dim loader As ServiceLoader(Of NDArrayCompressor) = ND4JClassLoading.loadService(GetType(NDArrayCompressor))
			For Each compressor As NDArrayCompressor In loader
				codecs(compressor.Descriptor.ToUpper()) = compressor
			Next compressor

			If codecs.Count = 0 Then
				'No compressors found - bad uber-jar?
				Dim msg As String = "Error loading ND4J Compressors via service loader: No compressors were found. This usually occurs" & " when running ND4J UI from an uber-jar, which was built incorrectly (without services resource" & " files being included)"
				log.error(msg)
				Throw New Exception(msg)
			End If
		End Sub

		''' <summary>
		''' Get the set of available codecs for
		''' compression
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property AvailableCompressors As ISet(Of String)
			Get
				Return codecs.Keys
			End Get
		End Property

		''' <summary>
		''' Prints available compressors to standard out
		''' </summary>
		Public Overridable Sub printAvailableCompressors()
			Dim builder As New StringBuilder()
			builder.Append("Available compressors: ")
			For Each comp As String In codecs.Keys
				builder.Append("[").Append(comp).Append("] ")
			Next comp

			Console.WriteLine(builder.ToString())
		End Sub

		''' <summary>
		''' Get the ndarray compressor
		''' singleton
		''' @return
		''' </summary>
		Public Shared ReadOnly Property Instance As BasicNDArrayCompressor
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		''' <summary>
		''' Set the default compression algorithm </summary>
		''' <param name="algorithm"> the algorithm to set
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BasicNDArrayCompressor setDefaultCompression(@NonNull String algorithm)
		Public Overridable Function setDefaultCompression(ByVal algorithm As String) As BasicNDArrayCompressor
			algorithm = algorithm.ToUpper()
			'       if (!codecs.containsKey(algorithm))
			'            throw new RuntimeException("Non-existent compression algorithm requested: [" + algorithm + "]");

			SyncLock Me
				defaultCompression_Conflict = algorithm
			End SyncLock

			Return Me
		End Function

		''' <summary>
		''' Get the default compression algorithm as a string.
		''' This is an all caps algorithm with a representation in the
		''' CompressionAlgorithm enum
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DefaultCompression As String
			Get
				SyncLock Me
					Return defaultCompression_Conflict
				End SyncLock
			End Get
		End Property

		''' <summary>
		''' Compress the given data buffer
		''' given the default compression algorithm </summary>
		''' <param name="buffer"> the data buffer to compress </param>
		''' <returns> the compressed version of the data buffer </returns>
		Public Overridable Function compress(ByVal buffer As DataBuffer) As DataBuffer
			Return compress(buffer, DefaultCompression)
		End Function

		''' <summary>
		''' Compress the data buffer
		''' given a specified algorithm </summary>
		''' <param name="buffer"> the buffer to compress </param>
		''' <param name="algorithm"> the algorithm to compress
		''' use </param>
		''' <returns> the compressed data buffer </returns>
		Public Overridable Function compress(ByVal buffer As DataBuffer, ByVal algorithm As String) As DataBuffer
			algorithm = algorithm.ToUpper()
			If Not codecs.ContainsKey(algorithm) Then
				Throw New Exception("Non-existent compression algorithm requested: [" & algorithm & "]")
			End If

			Return codecs(algorithm).compress(buffer)
		End Function

		Public Overridable Function compress(ByVal array As INDArray) As INDArray
			Nd4j.Executioner.commit()

			Return compress(array, DefaultCompression)
		End Function

		''' <summary>
		''' In place compression of the passed in ndarray
		''' with the default compression algorithm </summary>
		''' <param name="array"> </param>
		Public Overridable Sub compressi(ByVal array As INDArray)

			compressi(array, DefaultCompression)
		End Sub


		''' <summary>
		''' Returns a compressed version of the
		''' given ndarray </summary>
		''' <param name="array"> the array to compress </param>
		''' <param name="algorithm"> the algorithm to compress with </param>
		''' <returns> a compressed copy of this ndarray </returns>
		Public Overridable Function compress(ByVal array As INDArray, ByVal algorithm As String) As INDArray
			algorithm = algorithm.ToUpper()
			If Not codecs.ContainsKey(algorithm) Then
				Throw New Exception("Non-existent compression algorithm requested: [" & algorithm & "]")
			End If

			Return codecs(algorithm).compress(array)
		End Function

		''' <summary>
		''' In place Compress the given ndarray
		''' with the given algorithm </summary>
		''' <param name="array"> the array to compress </param>
		''' <param name="algorithm"> </param>
		Public Overridable Sub compressi(ByVal array As INDArray, ByVal algorithm As String)
			algorithm = algorithm.ToUpper()
			If Not codecs.ContainsKey(algorithm) Then
				Throw New Exception("Non-existent compression algorithm requested: [" & algorithm & "]")
			End If

			codecs(algorithm).compressi(array)
		End Sub

		''' <summary>
		''' Decompress the given databuffer </summary>
		''' <param name="buffer"> the databuffer to compress </param>
		''' <returns> the decompressed databuffer </returns>
		Public Overridable Function decompress(ByVal buffer As DataBuffer, ByVal targetType As DataType) As DataBuffer
			If buffer.dataType() <> DataType.COMPRESSED Then
				Throw New System.InvalidOperationException("You can't decompress DataBuffer with dataType of: " & buffer.dataType())
			End If

			Dim comp As CompressedDataBuffer = DirectCast(buffer, CompressedDataBuffer)
			Dim descriptor As CompressionDescriptor = comp.getCompressionDescriptor()

			If Not codecs.ContainsKey(descriptor.getCompressionAlgorithm()) Then
				Throw New Exception("Non-existent compression algorithm requested: [" & descriptor.getCompressionAlgorithm() & "]")
			End If

			Return codecs(descriptor.getCompressionAlgorithm()).decompress(buffer, targetType)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NDArrayCompressor getCompressor(@NonNull String name)
		Public Overridable Function getCompressor(ByVal name As String) As NDArrayCompressor
			Return codecs(name)
		End Function

		''' 
		''' <param name="array">
		''' @return </param>
		Public Overridable Function decompress(ByVal array As INDArray) As INDArray
			If array.data().dataType() <> DataType.COMPRESSED Then
				Return array
			End If

			Dim comp As CompressedDataBuffer = DirectCast(array.data(), CompressedDataBuffer)
			Dim descriptor As CompressionDescriptor = comp.getCompressionDescriptor()

			If Not codecs.ContainsKey(descriptor.getCompressionAlgorithm()) Then
				Throw New Exception("Non-existent compression algorithm requested: [" & descriptor.getCompressionAlgorithm() & "]")
			End If

			Return codecs(descriptor.getCompressionAlgorithm()).decompress(array)
		End Function

		''' <summary>
		''' in place decompression of the given
		''' ndarray. If the ndarray isn't compressed
		''' this will do nothing </summary>
		''' <param name="array"> the array to decompressed
		'''              if it is comprssed </param>
		Public Overridable Sub decompressi(ByVal array As INDArray)
			If array.data().dataType() <> DataType.COMPRESSED Then
				Return
			End If

			Dim comp As val = DirectCast(array.data(), CompressedDataBuffer)
			Dim descriptor As val = comp.getCompressionDescriptor()


			If Not codecs.ContainsKey(descriptor.getCompressionAlgorithm()) Then
				Throw New Exception("Non-existent compression algorithm requested: [" & descriptor.getCompressionAlgorithm() & "]")
			End If

			codecs(descriptor.getCompressionAlgorithm()).decompressi(array)
		End Sub

		''' <summary>
		''' Decompress several ndarrays </summary>
		''' <param name="arrays"> </param>
		Public Overridable Sub autoDecompress(ParamArray ByVal arrays() As INDArray)
			For Each array As INDArray In arrays
				autoDecompress(array)
			Next array
		End Sub

		''' 
		''' <param name="array"> </param>
		Public Overridable Sub autoDecompress(ByVal array As INDArray)
			If array.Compressed Then
				decompressi(array)
			End If
		End Sub

		''' <summary>
		''' This method returns compressed INDArray instance which contains JVM array passed in
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Public Overridable Function compress(ByVal array() As Single) As INDArray
			Return codecs(defaultCompression_Conflict).compress(array)
		End Function

		''' <summary>
		''' This method returns compressed INDArray instance which contains JVM array passed in
		''' </summary>
		''' <param name="array">
		''' @return </param>
		Public Overridable Function compress(ByVal array() As Double) As INDArray
			Return codecs(defaultCompression_Conflict).compress(array)
		End Function
	End Class

End Namespace