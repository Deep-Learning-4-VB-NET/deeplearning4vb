Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.util
Imports DeepLearning4VB.org.nd4vb.linalg.compression
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DataTypeEx = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataTypeEx

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.compression

    ''' <summary>
    ''' Tests for the compression descriptors: <c>CompressionAlgorithm</c> helpers, the 44-byte form of
    ''' <c>CompressionDescriptor</c>, its Lombok members, and <c>CompressionUtils</c>. (The compressors themselves need
    ''' arrays and arrive with the backend.)
    ''' </summary>
    <TestClass>
    Public Class CompressionTests

        <TestMethod>
        Public Sub testAlgorithmFromStringKeepsJavasMapping()
            assertEquals(CompressionAlgorithm.GZIP, CompressionAlgorithmExtensions.CompressionAlgorithmFromString("gzip"))
            assertEquals(CompressionAlgorithm.INT16, CompressionAlgorithmExtensions.CompressionAlgorithmFromString("Int16"))
            assertEquals(CompressionAlgorithm.UNIT8, CompressionAlgorithmExtensions.CompressionAlgorithmFromString("unit8"))
            ' Java quirk kept: FP16 maps to FLOAT8 and FP32 to FLOAT16
            assertEquals(CompressionAlgorithm.FLOAT8, CompressionAlgorithmExtensions.CompressionAlgorithmFromString("FP16"))
            assertEquals(CompressionAlgorithm.FLOAT16, CompressionAlgorithmExtensions.CompressionAlgorithmFromString("fp32"))
            Dim ex = assertThrows(Of ArgumentException)(Sub() CompressionAlgorithmExtensions.CompressionAlgorithmFromString("lz4"))
            assertEquals("Wrong algorithm lz4", ex.Message)
        End Sub

        <TestMethod>
        Public Sub testAlgorithmValuesAndValueOf()
            Dim values As CompressionAlgorithm() = CompressionAlgorithmExtensions.CompressionAlgorithmValues()
            assertEquals(8, values.Length)
            For i As Integer = 0 To values.Length - 1
                assertEquals(i, values(i).ordinal())
                assertEquals(values(i), CompressionAlgorithmExtensions.CompressionAlgorithmValueOf(values(i).ToString()))
            Next i
            ' values() returns a copy
            values(0) = CompressionAlgorithm.CUSTOM
            assertEquals(CompressionAlgorithm.FLOAT8, CompressionAlgorithmExtensions.CompressionAlgorithmValues()(0))
            ' valueOf is case-sensitive and rejects numbers, like Java
            assertThrows(Of ArgumentException)(Sub() CompressionAlgorithmExtensions.CompressionAlgorithmValueOf("gzip"))
            assertThrows(Of ArgumentException)(Sub() CompressionAlgorithmExtensions.CompressionAlgorithmValueOf("2"))
            assertThrows(Of ArgumentException)(Sub() CompressionAlgorithmExtensions.CompressionAlgorithmValueOf(Nothing))
        End Sub

        Private Shared Function sample() As CompressionDescriptor
            Dim d As New CompressionDescriptor()
            d.setCompressionType(CompressionType.LOSSY)
            d.setCompressionAlgorithm("INT16")
            d.setOriginalLength(400)
            d.setCompressedLength(200)
            d.setNumberOfElements(100)
            d.setOriginalElementSize(4)
            d.setOriginalDataType(DataType.FLOAT)
            Return d
        End Function

        <TestMethod>
        Public Sub testDescriptorBytesLikeJava()
            Dim bytes As Byte() = sample().toByteBuffer()
            ' 3 ints + 4 longs, little-endian (Java: ByteOrder.nativeOrder() on x86/ARM)
            assertEquals(44, bytes.Length)
            assertEquals(1, BitConverter.ToInt32(bytes, 0)) ' LOSSY
            assertEquals(4, BitConverter.ToInt32(bytes, 4)) ' INT16
            assertEquals(400L, BitConverter.ToInt64(bytes, 8))
            assertEquals(200L, BitConverter.ToInt64(bytes, 16))
            assertEquals(100L, BitConverter.ToInt64(bytes, 24))
            assertEquals(4L, BitConverter.ToInt64(bytes, 32))
            assertEquals(1, BitConverter.ToInt32(bytes, 40)) ' FLOAT
            ' Java's constant says 40
            assertEquals(40, CompressionDescriptor.COMPRESSION_BYTE_BUFFER_LENGTH)
        End Sub

        <TestMethod>
        Public Sub testDescriptorRoundTrip()
            Dim d As CompressionDescriptor = sample()
            Dim back As CompressionDescriptor = CompressionDescriptor.fromByteBuffer(d.toByteBuffer())
            assertEquals(d, back)
            assertEquals(d.GetHashCode(), back.GetHashCode())

            ' at an offset
            Dim padded(49) As Byte
            Array.Copy(d.toByteBuffer(), 0, padded, 6, 44)
            assertEquals(d, CompressionDescriptor.fromByteBuffer(padded, 6))

            ' an unknown algorithm name cannot be written, an out-of-range ordinal cannot be read
            Dim custom As CompressionDescriptor = sample()
            custom.setCompressionAlgorithm("MyCodec")
            assertThrows(Of ArgumentException)(Sub() custom.toByteBuffer())
            Dim bad As Byte() = d.toByteBuffer()
            bad(4) = 99
            assertThrows(Of IndexOutOfRangeException)(Sub() CompressionDescriptor.fromByteBuffer(bad))
        End Sub

        <TestMethod>
        Public Sub testDescriptorCloneEqualsAndSerialization()
            Dim d As CompressionDescriptor = sample()
            Dim c As CompressionDescriptor = d.clone()
            Assert.AreNotSame(d, c)
            assertEquals(d, c)
            Dim viaInterface As Object = DirectCast(d, ICloneable).Clone()
            assertEquals(d, viaInterface)
            c.setCompressedLength(1)
            assertNotEquals(d, c)
            assertEquals(200L, d.getCompressedLength())

            assertEquals("CompressionDescriptor(compressionType=LOSSY, compressionAlgorithm=INT16, originalLength=400, compressedLength=200, numberOfElements=100, originalElementSize=4, originalDataType=FLOAT)", d.ToString())

            Dim back As CompressionDescriptor = SerializationUtils.deserialize(Of CompressionDescriptor)(SerializationUtils.toByteArray(d))
            assertEquals(d, back)
        End Sub

        <TestMethod>
        Public Sub testCompressionUtils()
            assertTrue(CompressionUtils.goingToDecompress(DataTypeEx.FLOAT16, DataTypeEx.FLOAT))
            assertTrue(CompressionUtils.goingToDecompress(DataTypeEx.INT8, DataTypeEx.DOUBLE))
            assertFalse(CompressionUtils.goingToDecompress(DataTypeEx.FLOAT, DataTypeEx.INT8))
            assertTrue(CompressionUtils.goingToCompress(DataTypeEx.FLOAT, DataTypeEx.INT8))
            assertFalse(CompressionUtils.goingToCompress(DataTypeEx.FLOAT, DataTypeEx.DOUBLE))
            assertFalse(CompressionUtils.goingToCompress(DataTypeEx.INT8, DataTypeEx.FLOAT))
        End Sub

        <TestMethod>
        Public Sub testThresholdEncodings()
            assertEquals(0, ThresholdCompression.FLEXIBLE_ENCODING)
            assertEquals(1, ThresholdCompression.BITMAP_ENCODING)
        End Sub

    End Class

End Namespace
