Imports System
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DT = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DataBufferFactory = DeepLearning4VB.org.nd4vb.linalg.api.buffer.factory.DataBufferFactory
Imports DataInputStream = DeepLearning4VB.JavaCompat.DataInputStream
Imports DataOutputStream = DeepLearning4VB.JavaCompat.DataOutputStream
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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
    ''' A data buffer of type COMPRESSED: the compressed bytes produced by a codec and the <see cref="CompressionDescriptor"/>
    ''' that describes the original data (Java <c>CompressedDataBuffer</c>). It has no element access; a codec
    ''' (<see cref="NDArrayCompressor.decompress(DataBuffer, DataType)"/>) turns it back into a regular buffer.
    ''' </summary>
    ''' <remarks>
    ''' <para>The stream layout of <see cref="write(DataOutputStream)"/> is Java's: the usual header with the compressed
    ''' length and the type COMPRESSED, then <c>writeUTF(algorithm)</c>, the compressed length, the original length, the
    ''' number of elements, the original data type's ordinal (an int) and the compressed bytes;
    ''' <see cref="readUnknown(DataInputStream, DataBuffer.AllocationMode, Long, DataType)"/> reads it back.</para>
    ''' <para>The codecs themselves (threshold, bitmap, ...) are ops that arrive in a later wave; until then
    ''' <see cref="BasicNDArrayCompressor"/> finds only the codecs an application registers.</para>
    ''' <para>Differences from Java: the compressed bytes are a <c>Byte()</c> (<see cref="getCompressedBytes"/>; Java held a
    ''' JavaCPP pointer, see the native-only <c>addressPointer()</c>); a buffer read with <c>readUnknown</c> gets the original
    ''' element size of its original data type (Java left it 0).</para>
    ''' </remarks>
    Public Class CompressedDataBuffer
        Inherits BaseDataBuffer

        ''' <summary>The descriptor of the compressed data.</summary>
        Protected Friend compressionDescriptor As CompressionDescriptor

        Private bytes As Byte()

        ''' <summary>A compressed buffer over <paramref name="data"/> (not copied) described by <paramref name="descriptor"/>.</summary>
        Public Sub New(ByVal data As Byte(), ByVal descriptor As CompressionDescriptor)
            If descriptor Is Nothing Then
                Throw New ArgumentNullException(NameOf(descriptor))
            End If
            Me.compressionDescriptor = descriptor
            Me.bytes = data
            Me.lengthField = descriptor.getNumberOfElements()
            Me.underlyingLengthField = Me.lengthField
            Me.elementSizeField = CInt(descriptor.getOriginalElementSize())
            initTypeAndSize()
        End Sub

        ''' <summary>The type is COMPRESSED and the allocation mode MIXED_DATA_TYPES.</summary>
        Protected Friend Overrides Sub initTypeAndSize()
            type = DT.COMPRESSED
            allocationModeField = DataBuffer.AllocationMode.MIXED_DATA_TYPES
        End Sub

        ''' <summary>The descriptor of the compressed data.</summary>
        Public Overridable Function getCompressionDescriptor() As CompressionDescriptor
            Return compressionDescriptor
        End Function

        ''' <summary>Replaces the descriptor.</summary>
        Public Overridable Sub setCompressionDescriptor(ByVal descriptor As CompressionDescriptor)
            Me.compressionDescriptor = descriptor
        End Sub

        ''' <summary>The compressed bytes (shared, not copied).</summary>
        Public Overridable Function getCompressedBytes() As Byte()
            If released Then
                Throw New InvalidOperationException("You can't use DataBuffer once it was released")
            End If
            Return bytes
        End Function

        ''' <summary>Writes the buffer in Java's layout (see the class remarks).</summary>
        Public Overrides Sub write(ByVal [out] As DataOutputStream)
            ' here we should mimic to usual DataBuffer array
            [out].writeUTF(allocationModeField.ToString())
            [out].writeLong(compressionDescriptor.getCompressedLength())
            [out].writeUTF(DT.COMPRESSED.name())
            ' at this moment we don't care about mimics anymore
            [out].writeUTF(compressionDescriptor.getCompressionAlgorithm())
            [out].writeLong(compressionDescriptor.getCompressedLength())
            [out].writeLong(compressionDescriptor.getOriginalLength())
            [out].writeLong(compressionDescriptor.getNumberOfElements())
            [out].writeInt(compressionDescriptor.getOriginalDataType().ordinal())
            Dim data As Byte() = getCompressedBytes()
            If data IsNot Nothing Then
                [out].write(data)
            End If
        End Sub

        ''' <summary>
        ''' Reads a buffer whose header (allocation mode, length, type) was already read: a regular buffer created with
        ''' <c>Nd4vb.DataBufferFactory</c> unless the type is COMPRESSED.
        ''' </summary>
        Public Shared Function readUnknown(ByVal s As DataInputStream, ByVal allocMode As DataBuffer.AllocationMode, ByVal length As Long, ByVal type As DataType) As DataBuffer
            If type <> DT.COMPRESSED Then
                Return readUnknown(s, allocMode, length, type, Nd4vbT.DataBufferFactory)
            End If
            Return readUnknown(s, allocMode, length, type, Nothing)
        End Function

        ''' <summary>Same as <see cref="readUnknown(DataInputStream, DataBuffer.AllocationMode, Long, DataType)"/> with the given buffer factory.</summary>
        Public Shared Function readUnknown(ByVal s As DataInputStream, ByVal allocMode As DataBuffer.AllocationMode, ByVal length As Long, ByVal type As DataType, ByVal factory As DataBufferFactory) As DataBuffer
            ' if buffer is uncompressed, it'll be valid buffer, so we'll just return it
            If type <> DT.COMPRESSED Then
                If factory Is Nothing Then
                    Throw New ArgumentNullException(NameOf(factory))
                End If
                Dim buffer As DataBuffer = factory.create(type, length, False)
                buffer.read(s, allocMode, length, type)
                Return buffer
            Else
                ' if buffer is compressed one, we'll restore it here
                Dim compressionAlgorithm As String = s.readUTF()
                Dim compressedLength As Long = s.readLong()
                Dim originalLength As Long = s.readLong()
                Dim numberOfElements As Long = s.readLong()
                Dim originalType As DataType = DataTypeExtensions.DataTypeValues()(s.readInt())

                Dim temp(CInt(compressedLength) - 1) As Byte
                s.readFully(temp)

                Dim descriptor As New CompressionDescriptor()
                descriptor.setCompressedLength(compressedLength)
                descriptor.setCompressionAlgorithm(compressionAlgorithm)
                descriptor.setOriginalLength(originalLength)
                descriptor.setNumberOfElements(numberOfElements)
                descriptor.setOriginalDataType(originalType)
                descriptor.setOriginalElementSize(Math.Max(originalType.width(), 0))
                Return New CompressedDataBuffer(temp, descriptor)
            End If
        End Function

        ''' <summary>A copy with its own bytes and a cloned descriptor.</summary>
        Public Overrides Function dup() As DataBuffer
            Dim data As Byte() = getCompressedBytes()
            Dim copy As Byte() = If(data Is Nothing, Nothing, DirectCast(data.Clone(), Byte()))
            Return New CompressedDataBuffer(copy, compressionDescriptor.clone())
        End Function

        ''' <summary>The number of original elements.</summary>
        Public Overrides Function length() As Long
            Return compressionDescriptor.getNumberOfElements()
        End Function

        ''' <summary>Not supported for compressed buffers.</summary>
        Protected Friend Overrides Function create(ByVal length As Long) As DataBuffer
            Throw New NotSupportedException("This operation isn't supported for CompressedDataBuffer")
        End Function

        ''' <summary>Not supported for compressed buffers.</summary>
        Public Overrides Function create(ByVal data() As Double) As DataBuffer
            Throw New NotSupportedException("This operation isn't supported for CompressedDataBuffer")
        End Function

        ''' <summary>Not supported for compressed buffers.</summary>
        Public Overrides Function create(ByVal data() As Single) As DataBuffer
            Throw New NotSupportedException("This operation isn't supported for CompressedDataBuffer")
        End Function

        ''' <summary>Not supported for compressed buffers.</summary>
        Public Overrides Function create(ByVal data() As Integer) As DataBuffer
            Throw New NotSupportedException("This method isn't supported by CompressedDataBuffer")
        End Function

        ''' <summary>Not supported for compressed buffers.</summary>
        Public Overrides Sub pointerIndexerByCurrentType(ByVal currentType As DataType)
            Throw New NotSupportedException("This method isn't supported by CompressedDataBuffer")
        End Sub

        ''' <summary>Not supported for compressed buffers.</summary>
        Public Overrides Function reallocate(ByVal length As Long) As DataBuffer
            Throw New NotSupportedException("This method isn't supported by CompressedDataBuffer")
        End Function

        ''' <summary>The number of compressed bytes.</summary>
        Public Overrides Function capacity() As Long
            Dim data As Byte() = getCompressedBytes()
            Return If(data Is Nothing, 0, data.LongLength)
        End Function

        ''' <summary>True when both are compressed buffers over the same bytes.</summary>
        Public Overrides Function sameUnderlyingData(ByVal buffer As DataBuffer) As Boolean
            Dim other As CompressedDataBuffer = TryCast(buffer, CompressedDataBuffer)
            Return other IsNot Nothing AndAlso other.bytes Is bytes
        End Function

        ''' <summary>No element access: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Function getDouble(ByVal i As Long) As Double
            Throw unsupportedGet("double")
        End Function

        ''' <summary>No element access: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Function getLong(ByVal i As Long) As Long
            Throw unsupportedGet("long")
        End Function

        ''' <summary>No element access: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Function getFloat(ByVal i As Long) As Single
            Throw unsupportedGet("float")
        End Function

        ''' <summary>No element access: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Function getInt(ByVal ix As Long) As Integer
            Throw unsupportedGet("integer")
        End Function

        Private Shared Function unsupportedGet(ByVal what As String) As System.Exception
            Return New NotSupportedException("Cannot get " & what & " value from buffer of type COMPRESSED")
        End Function

        ''' <summary>No element access: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Single)
            Throw New NotSupportedException("Unsupported data type: COMPRESSED")
        End Sub

        ''' <summary>No element access: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Double)
            Throw New NotSupportedException("Unsupported data type: COMPRESSED")
        End Sub

        ''' <summary>No element access: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Integer)
            Throw New NotSupportedException("Unsupported data type: COMPRESSED")
        End Sub

        ''' <summary>No element access: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Long)
            Throw New NotSupportedException("Unsupported data type: COMPRESSED")
        End Sub

        ''' <summary>No element access: always throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Sub put(ByVal i As Long, ByVal element As Boolean)
            Throw New NotSupportedException("Unsupported data type: COMPRESSED")
        End Sub

        ''' <summary>Drops the compressed bytes.</summary>
        Protected Friend Overrides Sub release()
            bytes = Nothing
            MyBase.release()
        End Sub

    End Class

End Namespace
