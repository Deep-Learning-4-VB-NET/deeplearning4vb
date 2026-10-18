Imports System
Imports System.IO
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
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
    ''' Describes a compressed buffer: compression type and algorithm, lengths, element count and size, and the original
    ''' data type (Java <c>CompressionDescriptor</c>). Lombok's <c>@Data</c> members are written out.
    ''' <para>
    ''' Java's <c>ByteBuffer</c> becomes a <c>Byte()</c>: <see cref="toByteBuffer"/> returns the 44 bytes Java wrote in
    ''' native order (always little-endian on the platforms .NET runs on: two Int32 ordinals, four Int64 sizes and the
    ''' Int32 data-type ordinal), and <see cref="fromByteBuffer(Byte(), Integer)"/> reads them from an array and offset.
    ''' Like Java, <see cref="COMPRESSION_BYTE_BUFFER_LENGTH"/> says 40.
    ''' </para>
    ''' </summary>
    <Serializable>
    Public Class CompressionDescriptor
        Implements ICloneable

        Private compressionType As CompressionType
        Private compressionAlgorithm As String
        Private originalLength As Long
        Private compressedLength As Long
        Private numberOfElements As Long
        Private originalElementSize As Long
        Private originalDataType As DataType

        '40 bytes for the compression descriptor bytebuffer
        Public Const COMPRESSION_BYTE_BUFFER_LENGTH As Integer = 40

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Create a  compression descriptor from the given
        ''' data buffer elements </summary>
        ''' <param name="buffer"> the databuffer to create
        '''               the compression descriptor from </param>
        Public Sub New(ByVal buffer As DataBuffer)
            Me.originalLength = buffer.length() * buffer.ElementSize
            Me.numberOfElements = buffer.length()
            Me.originalElementSize = buffer.ElementSize
            Me.originalDataType = buffer.dataType()
        End Sub

        ''' <summary>
        ''' Initialize a compression descriptor
        ''' based on the given algorithm and data buffer </summary>
        ''' <param name="buffer"> the data buffer to base the sizes off of </param>
        ''' <param name="algorithm"> the algorithm used
        '''                  in the descriptor </param>
        Public Sub New(ByVal buffer As DataBuffer, ByVal algorithm As String)
            Me.New(buffer)
            Me.compressionAlgorithm = algorithm
        End Sub

        ''' <summary>
        ''' Initialize a compression descriptor
        ''' based on the given data buffer (for the sizes)
        ''' and the compressor to get the opType </summary>
        ''' <param name="buffer"> </param>
        ''' <param name="compressor"> </param>
        Public Sub New(ByVal buffer As DataBuffer, ByVal compressor As NDArrayCompressor)
            Me.New(buffer, compressor.Descriptor)
            Me.compressionType = compressor.CompressionType
        End Sub

        ''' <summary>
        ''' Instantiate a compression descriptor from the bytes written by <see cref="toByteBuffer"/>.
        ''' </summary>
        ''' <param name="byteBuffer"> the bytes to read </param>
        ''' <param name="offset"> the position of the first byte </param>
        ''' <returns> the instantiated descriptor </returns>
        Public Shared Function fromByteBuffer(ByVal byteBuffer() As Byte, ByVal offset As Integer) As CompressionDescriptor
            If byteBuffer Is Nothing Then Throw New ArgumentNullException(NameOf(byteBuffer))
            Using reader As New BinaryReader(New MemoryStream(byteBuffer, offset, byteBuffer.Length - offset, False))
                Dim descriptor As New CompressionDescriptor()
                'compression opType
                Dim compressionTypeOrdinal As Integer = reader.ReadInt32()
                If compressionTypeOrdinal < 0 OrElse compressionTypeOrdinal > 1 Then
                    Throw New IndexOutOfRangeException("Index " & compressionTypeOrdinal & " out of bounds for length 2")
                End If
                descriptor.setCompressionType(CType(compressionTypeOrdinal, CompressionType))

                'compression algo
                Dim compressionAlgoOrdinal As Integer = reader.ReadInt32()
                Dim algorithm As CompressionAlgorithm = CompressionAlgorithmValues()(compressionAlgoOrdinal)
                descriptor.setCompressionAlgorithm(algorithm.ToString())
                'from here everything is longs
                descriptor.setOriginalLength(reader.ReadInt64())
                descriptor.setCompressedLength(reader.ReadInt64())
                descriptor.setNumberOfElements(reader.ReadInt64())
                descriptor.setOriginalElementSize(reader.ReadInt64())
                descriptor.setOriginalDataType(Global.DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataTypeExtensions.DataTypeValues()(reader.ReadInt32()))
                Return descriptor
            End Using
        End Function

        ''' <summary>Reads a descriptor from the start of <paramref name="byteBuffer"/>.</summary>
        Public Shared Function fromByteBuffer(ByVal byteBuffer() As Byte) As CompressionDescriptor
            Return fromByteBuffer(byteBuffer, 0)
        End Function

        ''' <summary>
        ''' Return the descriptor as bytes. The size is calculated to be:
        ''' 44: 12 + 32
        ''' three ints representing the enum values
        ''' for the compression opType, the compression algorithm and the original data type
        ''' and 4 longs for the compressed and
        ''' original sizes </summary>
        ''' <returns> the bytes described above </returns>
        Public Overridable Function toByteBuffer() As Byte()
            '3 ints  at 4 bytes a piece, this includes the compression algorithm
            'that we convert to enum
            Dim enumSize As Integer = 3 * 4
            '4 longs at 8 bytes a piece
            Dim sizesLength As Integer = 4 * 8
            Using stream As New MemoryStream(enumSize + sizesLength)
                Using writer As New BinaryWriter(stream)
                    writer.Write(CInt(compressionType))
                    writer.Write(CompressionAlgorithmValueOf(compressionAlgorithm).ordinal())
                    writer.Write(originalLength)
                    writer.Write(compressedLength)
                    writer.Write(numberOfElements)
                    writer.Write(originalElementSize)
                    writer.Write(CInt(originalDataType))
                End Using
                Return stream.ToArray()
            End Using
        End Function

        Public Overridable Function getCompressionType() As CompressionType
            Return compressionType
        End Function

        Public Overridable Sub setCompressionType(ByVal compressionType As CompressionType)
            Me.compressionType = compressionType
        End Sub

        Public Overridable Function getCompressionAlgorithm() As String
            Return compressionAlgorithm
        End Function

        Public Overridable Sub setCompressionAlgorithm(ByVal compressionAlgorithm As String)
            Me.compressionAlgorithm = compressionAlgorithm
        End Sub

        Public Overridable Function getOriginalLength() As Long
            Return originalLength
        End Function

        Public Overridable Sub setOriginalLength(ByVal originalLength As Long)
            Me.originalLength = originalLength
        End Sub

        Public Overridable Function getCompressedLength() As Long
            Return compressedLength
        End Function

        Public Overridable Sub setCompressedLength(ByVal compressedLength As Long)
            Me.compressedLength = compressedLength
        End Sub

        Public Overridable Function getNumberOfElements() As Long
            Return numberOfElements
        End Function

        Public Overridable Sub setNumberOfElements(ByVal numberOfElements As Long)
            Me.numberOfElements = numberOfElements
        End Sub

        Public Overridable Function getOriginalElementSize() As Long
            Return originalElementSize
        End Function

        Public Overridable Sub setOriginalElementSize(ByVal originalElementSize As Long)
            Me.originalElementSize = originalElementSize
        End Sub

        Public Overridable Function getOriginalDataType() As DataType
            Return originalDataType
        End Function

        Public Overridable Sub setOriginalDataType(ByVal originalDataType As DataType)
            Me.originalDataType = originalDataType
        End Sub

        Public Overridable Function clone() As CompressionDescriptor
            Dim descriptor As New CompressionDescriptor()
            descriptor.compressionType = Me.compressionType
            descriptor.compressionAlgorithm = Me.compressionAlgorithm
            descriptor.originalLength = Me.originalLength
            descriptor.compressedLength = Me.compressedLength
            descriptor.numberOfElements = Me.numberOfElements
            descriptor.originalElementSize = Me.originalElementSize
            descriptor.originalDataType = Me.originalDataType

            Return descriptor
        End Function

        Private Function ICloneable_Clone() As Object Implements ICloneable.Clone
            Return clone()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As CompressionDescriptor = TryCast(obj, CompressionDescriptor)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return compressionType.Equals(o.compressionType) AndAlso Object.Equals(compressionAlgorithm, o.compressionAlgorithm) AndAlso originalLength.Equals(o.originalLength) AndAlso compressedLength.Equals(o.compressedLength) AndAlso numberOfElements.Equals(o.numberOfElements) AndAlso originalElementSize.Equals(o.originalElementSize) AndAlso originalDataType.Equals(o.originalDataType)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + compressionType.GetHashCode()
            h = h * 59 + If(compressionAlgorithm Is Nothing, 43, compressionAlgorithm.GetHashCode())
            h = h * 59 + originalLength.GetHashCode()
            h = h * 59 + compressedLength.GetHashCode()
            h = h * 59 + numberOfElements.GetHashCode()
            h = h * 59 + originalElementSize.GetHashCode()
            h = h * 59 + originalDataType.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "CompressionDescriptor(" & "compressionType=" & compressionType.ToString() & ", compressionAlgorithm=" & Objects.toString(compressionAlgorithm) & ", originalLength=" & originalLength.ToString() & ", compressedLength=" & compressedLength.ToString() & ", numberOfElements=" & numberOfElements.ToString() & ", originalElementSize=" & originalElementSize.ToString() & ", originalDataType=" & originalDataType.ToString() & ")"
        End Function

    End Class

End Namespace
