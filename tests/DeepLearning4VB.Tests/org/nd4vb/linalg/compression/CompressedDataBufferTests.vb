Imports System
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.compression
Imports DataInputStream = DeepLearning4VB.JavaCompat.DataInputStream
Imports FakeArray = DeepLearning4VB.Tests.org.nd4vb.linalg.cache.FakeArray
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer
Imports ManagedDataBufferFactory = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBufferFactory

Namespace org.nd4vb.linalg.compression

    ''' <summary>
    ''' A lossless test codec ("CUSTOM"): the elements as big-endian doubles. Public with a parameterless constructor, so
    ''' <see cref="BasicNDArrayCompressor"/> finds it through <c>ND4VBClassLoading</c> (the real codecs are ops that arrive
    ''' in a later wave).
    ''' </summary>
    Public Class TestCustomCodec
        Implements NDArrayCompressor

        Public ReadOnly Property Descriptor As String Implements NDArrayCompressor.Descriptor
            Get
                Return "custom"
            End Get
        End Property

        Public Sub configure(ParamArray ByVal vars() As Object) Implements NDArrayCompressor.configure
        End Sub

        Public ReadOnly Property CompressionType As CompressionType Implements NDArrayCompressor.CompressionType
            Get
                Return CompressionType.LOSSLESS
            End Get
        End Property

        Public Function compress(ByVal buffer As DataBuffer) As DataBuffer Implements NDArrayCompressor.compress
            Dim bytes(CInt(buffer.length()) * 8 - 1) As Byte
            For i As Integer = 0 To CInt(buffer.length()) - 1
                Dim b As Byte() = BitConverter.GetBytes(buffer.getDouble(i)).Reverse().ToArray()
                Array.Copy(b, 0, bytes, i * 8, 8)
            Next
            Dim descriptor As New CompressionDescriptor(buffer, Me)
            descriptor.setCompressedLength(bytes.Length)
            Return New CompressedDataBuffer(bytes, descriptor)
        End Function

        Public Function decompress(ByVal buffer As DataBuffer, ByVal targetType As DataType) As DataBuffer Implements NDArrayCompressor.decompress
            Dim bytes As Byte() = DirectCast(buffer, CompressedDataBuffer).getCompressedBytes()
            Dim values(bytes.Length \ 8 - 1) As Double
            For i As Integer = 0 To values.Length - 1
                values(i) = BitConverter.ToDouble(bytes.Skip(i * 8).Take(8).Reverse().ToArray(), 0)
            Next
            Return New ManagedDataBuffer(targetType, values)
        End Function

        Public Function compress(ByVal array As INDArray) As INDArray Implements NDArrayCompressor.compress
            Throw New NotSupportedException()
        End Function

        Public Sub compressi(ByVal array As INDArray) Implements NDArrayCompressor.compressi
            Throw New NotSupportedException()
        End Sub

        Public Function decompress(ByVal array As INDArray) As INDArray Implements NDArrayCompressor.decompress
            Return array
        End Function

        Public Sub decompressi(ByVal array As INDArray) Implements NDArrayCompressor.decompressi
        End Sub

        Public Function compress(ByVal data() As Single) As INDArray Implements NDArrayCompressor.compress
            Throw New NotSupportedException()
        End Function

        Public Function compress(ByVal data() As Double) As INDArray Implements NDArrayCompressor.compress
            Throw New NotSupportedException()
        End Function

        Public Function compress(ByVal data() As Single, ByVal shape() As Integer, ByVal order As Char) As INDArray Implements NDArrayCompressor.compress
            Throw New NotSupportedException()
        End Function

        Public Function compress(ByVal data() As Double, ByVal shape() As Integer, ByVal order As Char) As INDArray Implements NDArrayCompressor.compress
            Throw New NotSupportedException()
        End Function
    End Class

    ''' <summary>Tests for <see cref="CompressedDataBuffer"/> and <see cref="BasicNDArrayCompressor"/>.</summary>
    <TestClass>
    Public Class CompressedDataBufferTests

        <TestMethod>
        Public Sub testCompressedBufferBasics()
            Dim source As New ManagedDataBuffer(DataType.FLOAT, New Single() {1.5F, -2.0F, 3.0F})
            Dim compressed As CompressedDataBuffer = DirectCast(New TestCustomCodec().compress(source), CompressedDataBuffer)
            assertEquals(DataType.COMPRESSED, compressed.dataType())
            assertEquals(3L, compressed.length())
            assertEquals(4, compressed.ElementSize)
            assertEquals(24L, compressed.capacity())
            assertEquals("custom", compressed.getCompressionDescriptor().getCompressionAlgorithm())
            assertEquals(DataType.FLOAT, compressed.getCompressionDescriptor().getOriginalDataType())
            assertThrows(Of NotSupportedException)(Sub() compressed.getDouble(0))
            assertThrows(Of NotSupportedException)(Sub() compressed.put(0, 1.0))
            assertThrows(Of NotSupportedException)(Sub() compressed.reallocate(5))
            assertThrows(Of NotSupportedException)(Sub() compressed.pointerIndexerByCurrentType(DataType.FLOAT))

            Dim copy As CompressedDataBuffer = DirectCast(compressed.dup(), CompressedDataBuffer)
            assertArrayEquals(compressed.getCompressedBytes(), copy.getCompressedBytes())
            Assert.AreNotSame(compressed.getCompressedBytes(), copy.getCompressedBytes())
            assertFalse(copy.sameUnderlyingData(compressed))
            assertTrue(compressed.sameUnderlyingData(compressed))
            Assert.AreNotSame(compressed.getCompressionDescriptor(), copy.getCompressionDescriptor())

            compressed.close()
            assertTrue(compressed.wasClosed())
            assertThrows(Of InvalidOperationException)(Sub() compressed.getCompressedBytes())
        End Sub

        <TestMethod>
        Public Sub testStreamLayoutRoundTrip()
            Dim source As New ManagedDataBuffer(DataType.DOUBLE, New Double() {0.25, 8})
            Dim compressed As DataBuffer = New TestCustomCodec().compress(source)
            Using ms As New MemoryStream()
                compressed.write(ms)
                ms.Position = 0
                Dim dis As New DataInputStream(ms, True)
                Dim header = BaseDataBuffer.readHeader(dis)
                assertEquals(DataBuffer.AllocationMode.MIXED_DATA_TYPES, header.First)
                assertEquals(16L, header.Second)
                assertEquals(DataType.COMPRESSED, header.Third)
                Dim read As CompressedDataBuffer = DirectCast(CompressedDataBuffer.readUnknown(dis, header.First, header.Second, header.Third), CompressedDataBuffer)
                assertEquals(ms.Length, ms.Position)
                assertEquals("custom", read.getCompressionDescriptor().getCompressionAlgorithm())
                assertEquals(2L, read.length())
                assertEquals(DataType.DOUBLE, read.getCompressionDescriptor().getOriginalDataType())
                assertEquals(8, read.ElementSize)
                assertArrayEquals(New Double() {0.25, 8}, New TestCustomCodec().decompress(read, DataType.DOUBLE).asDouble(), 0.0)
            End Using
            ' an uncompressed buffer is read with the given factory
            Using ms As New MemoryStream()
                Dim plain As New ManagedDataBuffer(DataType.INT, New Integer() {4, 5})
                plain.write(ms)
                ms.Position = 0
                Dim dis As New DataInputStream(ms, True)
                Dim header = BaseDataBuffer.readHeader(dis)
                Dim read As DataBuffer = CompressedDataBuffer.readUnknown(dis, header.First, header.Second, header.Third, New ManagedDataBufferFactory())
                assertArrayEquals(New Integer() {4, 5}, read.asInt())
            End Using
        End Sub

        <TestMethod>
        Public Sub testCompressorRegistry()
            Dim compressor As New BasicNDArrayCompressor()
            assertTrue(compressor.AvailableCompressors.Contains("CUSTOM"))
            assertEquals("FLOAT16", compressor.DefaultCompression)
            Assert.AreSame(compressor, compressor.setDefaultCompression("custom"))
            assertEquals("CUSTOM", compressor.DefaultCompression)
            assertNotNull(compressor.getCompressor("CUSTOM"))
            assertNull(compressor.getCompressor("NOPE"))
            compressor.printAvailableCompressors()

            Dim source As New ManagedDataBuffer(DataType.FLOAT, New Single() {1, 2, 3})
            Dim compressed As DataBuffer = compressor.compress(source)
            assertEquals(DataType.COMPRESSED, compressed.dataType())
            ' the descriptor names the codec in its own case; decompress looks it up as written
            DirectCast(compressed, CompressedDataBuffer).getCompressionDescriptor().setCompressionAlgorithm("CUSTOM")
            assertArrayEquals(New Single() {1, 2, 3}, compressor.decompress(compressed, DataType.FLOAT).asFloat(), 0.0F)
            assertThrows(Of InvalidOperationException)(Sub() compressor.decompress(source, DataType.FLOAT))
            assertThrows(Of InvalidOperationException)(Sub() compressor.compress(source, "missing"))

            ' arrays: uncompressed arrays are left alone
            Dim plainArray As New FakeArray(source)
            Assert.AreSame(plainArray, compressor.decompress(plainArray))
            compressor.decompressi(plainArray)
            compressor.autoDecompress(plainArray, plainArray)
            Dim compressedArray As New FakeArray(compressed)
            Assert.AreSame(compressedArray, compressor.decompress(compressedArray))
            compressor.autoDecompress(compressedArray)

            Assert.AreSame(BasicNDArrayCompressor.Instance, BasicNDArrayCompressor.Instance)
        End Sub

    End Class

End Namespace
