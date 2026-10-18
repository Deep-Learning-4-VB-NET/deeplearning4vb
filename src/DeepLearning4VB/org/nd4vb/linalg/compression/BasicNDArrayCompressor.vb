Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Text
Imports DeepLearning4VB.org.slf4j
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ND4VBClassLoading = DeepLearning4VB.org.nd4vb.common.config.ND4VBClassLoading

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

Namespace org.nd4vb.linalg.compression

    ''' <summary>
    ''' The registry of compression codecs (Java <c>BasicNDArrayCompressor</c>): finds every <see cref="NDArrayCompressor"/>
    ''' through <see cref="ND4VBClassLoading.loadService(Of S)"/> (Java: <c>ServiceLoader</c>), keys them by their upper-case
    ''' descriptor, and compresses or decompresses buffers and arrays with the default or a named codec.
    ''' </summary>
    ''' <remarks>
    ''' <para>The codecs of ND4J (FLOAT16, INT8, ..., THRESHOLD, BITMAP) are ops that arrive in a later wave, so until then
    ''' only the codecs an application registers (<c>ND4VBClassLoading.registerService(Of NDArrayCompressor)</c>) or defines
    ''' are found. As in Java, creating the registry without any codec throws; <see cref="Instance"/> retries on the next call
    ''' (Java failed once, in the static initializer).</para>
    ''' <para>Difference from Java: <c>compress(INDArray)</c> does not call <c>Nd4j.getExecutioner().commit()</c> (the
    ''' managed executioner runs synchronously); <see cref="getCompressor"/> returns Nothing for an unknown name.</para>
    ''' </remarks>
    Public Class BasicNDArrayCompressor

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(BasicNDArrayCompressor))

        Private Shared instanceField As BasicNDArrayCompressor
        Private Shared ReadOnly instanceGate As New Object()

        Protected Friend codecs As IDictionary(Of String, NDArrayCompressor)
        Protected Friend defaultCompressionField As String = "FLOAT16"

        ''' <summary>Loads the codecs (Java: private; Friend so that tests can build a fresh registry).</summary>
        Friend Sub New()
            loadCompressors()
        End Sub

        ''' <summary>Finds every codec and keys it by its upper-case descriptor; throws when there is none.</summary>
        Protected Friend Overridable Sub loadCompressors()
            ' We scan the configured assemblies for NDArrayCompressor implementations and add them one by one to codecs map
            Dim found As New ConcurrentDictionary(Of String, NDArrayCompressor)()
            For Each compressor As NDArrayCompressor In ND4VBClassLoading.loadService(Of NDArrayCompressor)()
                found(compressor.Descriptor.ToUpperInvariant()) = compressor
            Next compressor
            codecs = found

            If codecs.Count = 0 Then
                'No compressors found
                Dim msg As String = "Error loading ND4VB Compressors: No compressors were found. The codecs are ops that arrive in a later phase-2 wave;" & " register one with ND4VBClassLoading.registerService(Of NDArrayCompressor)"
                log.error(msg)
                Throw New InvalidOperationException(msg)
            End If
        End Sub

        ''' <summary>The descriptors (upper case) of the available codecs.</summary>
        Public Overridable ReadOnly Property AvailableCompressors As ISet(Of String)
            Get
                Return New HashSet(Of String)(codecs.Keys)
            End Get
        End Property

        ''' <summary>Prints the available codecs to the console.</summary>
        Public Overridable Sub printAvailableCompressors()
            Dim builder As New StringBuilder()
            builder.Append("Available compressors: ")
            For Each comp As String In codecs.Keys
                builder.Append("[").Append(comp).Append("] ")
            Next comp

            Console.WriteLine(builder.ToString())
        End Sub

        ''' <summary>The process-wide registry, created on first use.</summary>
        ''' <exception cref="InvalidOperationException">No codec is available.</exception>
        Public Shared ReadOnly Property Instance As BasicNDArrayCompressor
            Get
                SyncLock instanceGate
                    If instanceField Is Nothing Then
                        instanceField = New BasicNDArrayCompressor()
                    End If
                    Return instanceField
                End SyncLock
            End Get
        End Property

        ''' <summary>Sets the default codec (upper-cased; not checked, as in Java).</summary>
        Public Overridable Function setDefaultCompression(ByVal algorithm As String) As BasicNDArrayCompressor
            If algorithm Is Nothing Then
                Throw New ArgumentNullException(NameOf(algorithm))
            End If
            algorithm = algorithm.ToUpperInvariant()
            SyncLock Me
                defaultCompressionField = algorithm
            End SyncLock
            Return Me
        End Function

        ''' <summary>The default codec (FLOAT16 unless changed).</summary>
        Public Overridable ReadOnly Property DefaultCompression As String
            Get
                SyncLock Me
                    Return defaultCompressionField
                End SyncLock
            End Get
        End Property

        Private Function codecFor(ByVal algorithm As String) As NDArrayCompressor
            Dim codec As NDArrayCompressor = Nothing
            If algorithm Is Nothing OrElse Not codecs.TryGetValue(algorithm, codec) Then
                Throw New InvalidOperationException("Non-existent compression algorithm requested: [" & algorithm & "]")
            End If
            Return codec
        End Function

        ''' <summary>Compresses a buffer with the default codec.</summary>
        Public Overridable Function compress(ByVal buffer As DataBuffer) As DataBuffer
            Return compress(buffer, DefaultCompression)
        End Function

        ''' <summary>Compresses a buffer with the named codec.</summary>
        Public Overridable Function compress(ByVal buffer As DataBuffer, ByVal algorithm As String) As DataBuffer
            Return codecFor(algorithm.ToUpperInvariant()).compress(buffer)
        End Function

        ''' <summary>Compresses an array with the default codec.</summary>
        Public Overridable Function compress(ByVal array As INDArray) As INDArray
            Return compress(array, DefaultCompression)
        End Function

        ''' <summary>Compresses an array in place with the default codec.</summary>
        Public Overridable Sub compressi(ByVal array As INDArray)
            compressi(array, DefaultCompression)
        End Sub

        ''' <summary>Compresses an array with the named codec.</summary>
        Public Overridable Function compress(ByVal array As INDArray, ByVal algorithm As String) As INDArray
            Return codecFor(algorithm.ToUpperInvariant()).compress(array)
        End Function

        ''' <summary>Compresses an array in place with the named codec.</summary>
        Public Overridable Sub compressi(ByVal array As INDArray, ByVal algorithm As String)
            codecFor(algorithm.ToUpperInvariant()).compressi(array)
        End Sub

        ''' <summary>Decompresses a COMPRESSED buffer to <paramref name="targetType"/> with the codec named in its descriptor.</summary>
        Public Overridable Function decompress(ByVal buffer As DataBuffer, ByVal targetType As DataType) As DataBuffer
            If buffer.dataType() <> DataType.COMPRESSED Then
                Throw New InvalidOperationException("You can't decompress DataBuffer with dataType of: " & buffer.dataType().ToString())
            End If

            Dim comp As CompressedDataBuffer = DirectCast(buffer, CompressedDataBuffer)
            Dim descriptor As CompressionDescriptor = comp.getCompressionDescriptor()

            Return codecFor(descriptor.getCompressionAlgorithm()).decompress(buffer, targetType)
        End Function

        ''' <summary>The codec with the given descriptor, or Nothing.</summary>
        Public Overridable Function getCompressor(ByVal name As String) As NDArrayCompressor
            Dim codec As NDArrayCompressor = Nothing
            If name IsNot Nothing AndAlso codecs.TryGetValue(name, codec) Then
                Return codec
            End If
            Return Nothing
        End Function

        ''' <summary>Decompresses an array whose buffer is COMPRESSED (other arrays are returned as they are).</summary>
        Public Overridable Function decompress(ByVal array As INDArray) As INDArray
            If array.data().dataType() <> DataType.COMPRESSED Then
                Return array
            End If

            Dim comp As CompressedDataBuffer = DirectCast(array.data(), CompressedDataBuffer)
            Dim descriptor As CompressionDescriptor = comp.getCompressionDescriptor()

            Return codecFor(descriptor.getCompressionAlgorithm()).decompress(array)
        End Function

        ''' <summary>Decompresses an array in place (nothing to do when its buffer is not COMPRESSED).</summary>
        Public Overridable Sub decompressi(ByVal array As INDArray)
            If array.data().dataType() <> DataType.COMPRESSED Then
                Return
            End If

            Dim comp As CompressedDataBuffer = DirectCast(array.data(), CompressedDataBuffer)
            Dim descriptor As CompressionDescriptor = comp.getCompressionDescriptor()

            codecFor(descriptor.getCompressionAlgorithm()).decompressi(array)
        End Sub

        ''' <summary>Decompresses every compressed array in place.</summary>
        Public Overridable Sub autoDecompress(ParamArray ByVal arrays() As INDArray)
            For Each array As INDArray In arrays
                autoDecompress(array)
            Next array
        End Sub

        ''' <summary>Decompresses the array in place if it is compressed.</summary>
        Public Overridable Sub autoDecompress(ByVal array As INDArray)
            If array.Compressed Then
                decompressi(array)
            End If
        End Sub

        ''' <summary>Compresses float data with the default codec.</summary>
        Public Overridable Function compress(ByVal array() As Single) As INDArray
            Return codecFor(DefaultCompression).compress(array)
        End Function

        ''' <summary>Compresses double data with the default codec.</summary>
        Public Overridable Function compress(ByVal array() As Double) As INDArray
            Return codecFor(DefaultCompression).compress(array)
        End Function

    End Class

End Namespace
