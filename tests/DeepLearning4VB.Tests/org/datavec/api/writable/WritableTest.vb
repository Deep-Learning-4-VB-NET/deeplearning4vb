Imports System
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.writable
Imports DVW = DeepLearning4VB.org.datavec.api.writable

Namespace org.datavec.api.writable

    ''' <summary>
    ''' Ported from the DataVec JUnit test of the same name. The tests that need the ND4J tensor API
    ''' (<c>NDArrayWritable</c>, <c>NDArrayRecordBatch</c>, <c>BytesWritable.asNd4jBuffer</c>) are
    ''' left out; the rest keep their shape, plus a few checks of the port's own behaviour.
    ''' </summary>
    <TestClass>
    Public Class WritableTest

        <TestMethod>
        Public Sub testWritableEqualityReflexive()
            assertEquals(New IntWritable(1), New IntWritable(1))
            assertEquals(New LongWritable(1), New LongWritable(1))
            assertEquals(New DoubleWritable(1), New DoubleWritable(1))
            assertEquals(New FloatWritable(1), New FloatWritable(1))
            assertEquals(New Text("Hello"), New Text("Hello"))
            assertEquals(New BytesWritable(Encoding.UTF8.GetBytes("Hello")), New BytesWritable(Encoding.UTF8.GetBytes("Hello")))
            assertEquals(New NullWritable(), New NullWritable())
            assertEquals(New BooleanWritable(True), New BooleanWritable(True))
            Dim b As SByte = 0
            assertEquals(New ByteWritable(b), New ByteWritable(b))
        End Sub

        <TestMethod>
        Public Sub testBytesWritableIndexing()
            ' two big-endian doubles, as Java's ByteBuffer.putDouble writes them
            Dim doubleWrite(15) As Byte
            Array.Copy(BigEndian(BitConverter.DoubleToInt64Bits(1.0)), 0, doubleWrite, 0, 8)
            Array.Copy(BigEndian(BitConverter.DoubleToInt64Bits(2.0)), 0, doubleWrite, 8, 8)
            Dim byteWritable As New BytesWritable(doubleWrite)
            assertEquals(2, byteWritable.getDouble(1), 1e-1)
            assertEquals(1, byteWritable.getDouble(0), 1e-1)
            assertEquals(16L, byteWritable.length())
            assertEquals(WritableType.Bytes, byteWritable.getType())
            assertThrows(Of NotSupportedException)(Sub() byteWritable.toDouble())
        End Sub

        <TestMethod>
        Public Sub testByteWritable()
            Dim b As SByte = -2 ' (byte) 0xfe
            assertEquals(New IntWritable(-2), New ByteWritable(b))
            assertEquals(New LongWritable(-2), New ByteWritable(b))
            assertEquals(New ByteWritable(b), New IntWritable(-2))
            assertEquals(New ByteWritable(b), New LongWritable(-2))
            ' those would cast to the same Int
            Dim minus126 As SByte = -126 ' (byte) 0x82
            assertNotEquals(New ByteWritable(minus126), New IntWritable(130))
        End Sub

        <TestMethod>
        Public Sub testIntLongWritable()
            assertEquals(New IntWritable(1), New LongWritable(1L))
            assertEquals(New LongWritable(2L), New IntWritable(2))
            Dim l As Long = 1L << 34
            ' those would cast to the same Int
            assertNotEquals(New LongWritable(l), New IntWritable(4))
        End Sub

        <TestMethod>
        Public Sub testDoubleFloatWritable()
            assertEquals(New DoubleWritable(1.0), New FloatWritable(1.0F))
            assertEquals(New FloatWritable(2.0F), New DoubleWritable(2.0))
            ' we defer to Java equality for Floats
            assertNotEquals(New DoubleWritable(1.1), New FloatWritable(1.1F))
            ' same idea as above
            assertNotEquals(New DoubleWritable(1.1), New FloatWritable(CSng(1.1)))
            assertNotEquals(New DoubleWritable(CDbl(Single.MaxValue) + 1), New FloatWritable(Single.PositiveInfinity))
        End Sub

        <TestMethod>
        Public Sub testFuzzies()
            assertTrue((New DoubleWritable(1.1)).fuzzyEquals(New FloatWritable(1.1F), 1e-6))
            assertTrue((New FloatWritable(1.1F)).fuzzyEquals(New DoubleWritable(1.1), 1e-6))
            Dim b As SByte = -2
            assertTrue((New ByteWritable(b)).fuzzyEquals(New DoubleWritable(-2.0), 1e-6))
            assertFalse((New IntWritable(1)).fuzzyEquals(New FloatWritable(1.1F), 1e-2))
            assertTrue((New IntWritable(1)).fuzzyEquals(New FloatWritable(1.05F), 1e-1))
            assertTrue((New LongWritable(1)).fuzzyEquals(New DoubleWritable(1.05F), 1e-1))
            assertFalse((New LongWritable(1)).fuzzyEquals(New Text("1"), 1e-1))
        End Sub

        ' ---- checks of the port's own behaviour ----

        <TestMethod>
        Public Sub testJavaStyleToString()
            assertEquals("1.0", New DoubleWritable(1).ToString())
            assertEquals("2.5", New FloatWritable(2.5F).ToString())
            assertEquals("true", New BooleanWritable(True).ToString())
            assertEquals("false", New BooleanWritable(False).ToString())
            assertEquals("-7", New IntWritable(-7).ToString())
            assertEquals("NullWritable", NullWritable.INSTANCE.ToString())
            assertEquals("-128", New ByteWritable(-128).ToString())
        End Sub

        <TestMethod>
        Public Sub testScalarConversions()
            Dim d As New DoubleWritable(3.75)
            assertEquals(3, d.toInt())
            assertEquals(3L, d.toLong())
            assertEquals(3.75F, d.toFloat(), 0.0F)
            Dim neg As New DoubleWritable(-3.75)
            assertEquals(-3, neg.toInt())
            assertEquals(0, New DoubleWritable(Double.NaN).toInt())
            assertEquals(Integer.MaxValue, New DoubleWritable(1e300).toInt())
            assertEquals(1, New BooleanWritable(True).toInt())
            assertEquals(0.0, New BooleanWritable(False).toDouble(), 0.0)
            assertEquals(-1, New LongWritable(&HFFFFFFFFFFFFFFFFL).toInt())
            assertThrows(Of NotSupportedException)(Sub() NullWritable.INSTANCE.toInt())
        End Sub

        <TestMethod>
        Public Sub testTextConversions()
            assertEquals(1.5, New Text("1.5").toDouble(), 0.0)
            assertEquals(1.5F, New Text(" 1.5 ").toFloat(), 0.0F)
            assertEquals(42, New Text("42").toInt())
            assertEquals(-42L, New Text("-42").toLong())
            assertEquals(16, New Text("0x10").toInt())
            assertEquals(255L, New Text("0xFF").toLong())
            assertEquals(16.0, New Text("0x10").toDouble(), 0.0)
            assertTrue(Double.IsNaN(New Text("NaN").toDouble()))
            assertThrows(Of NumberFormatException)(Sub() Call New Text("abc").toInt())
            assertThrows(Of NumberFormatException)(Sub() Call New Text("1.5").toInt())
            assertThrows(Of NumberFormatException)(Sub() Call New Text("abc").toDouble())
        End Sub

        <TestMethod>
        Public Sub testCompareTo()
            assertTrue(New IntWritable(1).CompareTo(New IntWritable(2)) < 0)
            assertTrue(New LongWritable(5).CompareTo(New LongWritable(2)) > 0)
            assertEquals(0, New DoubleWritable(2.5).CompareTo(New DoubleWritable(2.5)))
            assertTrue(New FloatWritable(-1).CompareTo(New FloatWritable(1)) < 0)
            assertTrue(New ByteWritable(-1).CompareTo(New ByteWritable(1)) < 0)
            assertTrue(New BooleanWritable(False).CompareTo(New BooleanWritable(True)) < 0)
            assertTrue(New Text("a").CompareTo(New Text("b")) < 0)
            assertTrue(New Text("ab").CompareTo(New Text("a")) > 0)
            assertEquals(0, NullWritable.INSTANCE.CompareTo(New NullWritable()))
            assertThrows(Of ArgumentException)(Sub() NullWritable.INSTANCE.CompareTo(New IntWritable(1)))
        End Sub

        <TestMethod>
        Public Sub testNullWritable()
            assertEquals(NullWritable.INSTANCE, New NullWritable())
            assertEquals(NullWritable.INSTANCE.GetHashCode(), New NullWritable().GetHashCode())
            assertNotEquals(NullWritable.INSTANCE, New IntWritable(0))
            assertEquals(WritableType.Null, NullWritable.INSTANCE.getType())
        End Sub

        <TestMethod>
        Public Sub testHashCodesMatchJava()
            assertEquals(1, New IntWritable(1).GetHashCode())
            assertEquals(0, New BooleanWritable(True).GetHashCode())
            assertEquals(1, New BooleanWritable(False).GetHashCode())
            assertEquals(-2, New ByteWritable(-2).GetHashCode())
            assertEquals(1072693248, New DoubleWritable(1.0).GetHashCode()) ' Double.hashCode(1.0)
            assertEquals(1065353216, New FloatWritable(1.0F).GetHashCode()) ' Float.floatToIntBits(1.0f)
            assertEquals(-2147483648, New LongWritable(Long.MinValue).GetHashCode()) ' Long.hashCode(Long.MIN_VALUE)
            assertEquals(126145, New Text("abc").GetHashCode()) ' hashBytes over the signed bytes
            assertEquals(-1017, New Text(ChrW(&HE9)).GetHashCode()) ' 'é' = C3 A9, signed -61 -87
            ' Arrays.hashCode(byte[]) with signed bytes: [1, -1]
            assertEquals(31 * (31 * 1 + 1) - 1, New BytesWritable(New Byte() {1, 255}).GetHashCode())
        End Sub

        <TestMethod>
        Public Sub testWritableType()
            assertEquals(CShort(4), WritableType.Int.typeIdx())
            assertEquals(CShort(11), WritableType.Bytes.typeIdx())
            assertEquals(7, WritableType.Text.ordinal())
            assertTrue(WritableType.Int.isCoreWritable())
            assertFalse(WritableType.Image.isCoreWritable())
            assertFalse(WritableType.Arrow.isCoreWritable())
            assertEquals(GetType(IntWritable), WritableType.Int.getWritableClass())
            assertEquals(GetType(Text), WritableType.Text.getWritableClass())
            assertEquals(GetType(ByteWritable), WritableType.Bytes.getWritableClass()) ' Java quirk, preserved
            assertNull(WritableType.Image.getWritableClass())
            assertNull(WritableType.NDArray.getWritableClass()) ' not ported yet
            assertEquals(WritableType.Long, WritableTypeExtensions.valueOf("Long"))
            assertEquals(12, WritableTypeExtensions.values().Length)
            assertThrows(Of ArgumentException)(Sub() WritableTypeExtensions.valueOf("Nope"))
            For Each w As DVW.Writable In New DVW.Writable() {New BooleanWritable(True), New ByteWritable(1), New DoubleWritable(1), New FloatWritable(1), New IntWritable(1), New LongWritable(1), NullWritable.INSTANCE, New Text("x")}
                assertEquals(DirectCast(w, Object).GetType(), w.getType().getWritableClass())
            Next w
        End Sub

        <TestMethod>
        Public Sub testUnsafeWritableInjector()
            assertEquals(New IntWritable(3), UnsafeWritableInjector.inject(3))
            assertEquals(New LongWritable(3), UnsafeWritableInjector.inject(3L))
            assertEquals(New FloatWritable(1.5F), UnsafeWritableInjector.inject(1.5F))
            assertEquals(New DoubleWritable(2.5), UnsafeWritableInjector.inject(2.5))
            assertEquals(New Text("abc"), UnsafeWritableInjector.inject("abc"))
            assertEquals(New ByteWritable(-2), UnsafeWritableInjector.inject(CSByte(-2)))
            assertEquals(New ByteWritable(-2), UnsafeWritableInjector.inject(CByte(254)))
            assertEquals(NullWritable.INSTANCE, UnsafeWritableInjector.inject(Of Object)(Nothing))
            Dim w As DVW.Writable = New Text("same")
            assertTrue(w Is UnsafeWritableInjector.inject(w))
            assertThrows(Of ArgumentException)(Sub() UnsafeWritableInjector.inject(New Object()))
        End Sub

        <TestMethod>
        Public Sub testWritableFactoryRegistry()
            Dim f As WritableFactory = WritableFactory.getInstance()
            assertTrue(f Is WritableFactory.Instance)
            assertEquals(GetType(IntWritable), DirectCast(f.newWritable(WritableType.Int.typeIdx()), Object).GetType())
            assertEquals(GetType(Text), DirectCast(f.newWritable(WritableType.Text.typeIdx()), Object).GetType())
            assertThrows(Of NotSupportedException)(Sub() f.registerWritableType(WritableType.Int.typeIdx(), GetType(IntWritable)))
            assertThrows(Of InvalidOperationException)(Sub() f.newWritable(12345))
            assertThrows(Of InvalidOperationException)(Sub() f.registerWritableType(12346, GetType(NoDefaultCtorWritable)))
            assertThrows(Of ArgumentNullException)(Sub() f.registerWritableType(12347, Nothing))
        End Sub

        ''' <summary>A writable without a no-arg constructor, which the factory must reject.</summary>
        Private Class NoDefaultCtorWritable
            Inherits IntWritable

            Public Sub New(ByVal v As Integer)
                MyBase.New(v)
            End Sub
        End Class

        Private Shared Function BigEndian(ByVal v As Long) As Byte()
            Dim b() As Byte = BitConverter.GetBytes(v)
            If BitConverter.IsLittleEndian Then Array.Reverse(b)
            Return b
        End Function

    End Class

End Namespace
