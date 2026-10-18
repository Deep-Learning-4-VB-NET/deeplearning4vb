Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports DataOutputStream = DeepLearning4VB.JavaCompat.DataOutputStream
Imports ManagedMemoryManager = DeepLearning4VB.org.nd4vb.linalg.managed.memory.ManagedMemoryManager
Imports ManagedWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.managed.memory.ManagedWorkspaceManager
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration

Namespace org.nd4vb.linalg.managed.buffer

    ''' <summary>
    ''' Tests for <see cref="ManagedDataBuffer"/> (and the shared logic of <see cref="BaseDataBuffer"/>): element access with
    ''' Java's casts for every type, views and offsets, dup, the assign/setData/asX/getXAt families, the Java stream layout,
    ''' close/Dispose, the constant and workspace flags, and UTF8 buffers. Expected values follow ND4J 1.0.0-M2's
    ''' <c>BaseDataBuffer</c> and Java's primitive casts.
    ''' </summary>
    <TestClass>
    Public Class ManagedDataBufferTests

        Private Shared ReadOnly NumericTypes As DataType() = {DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.LONG, DataType.INT, DataType.SHORT, DataType.UBYTE, DataType.BYTE, DataType.BOOL, DataType.BFLOAT16, DataType.UINT16, DataType.UINT32, DataType.UINT64}

        Private Shared Function filled(ByVal type As DataType, ParamArray ByVal values() As Double) As ManagedDataBuffer
            Dim b As New ManagedDataBuffer(type, values.Length)
            For i As Integer = 0 To values.Length - 1
                b.put(i, values(i))
            Next
            Return b
        End Function

#Region "Element access"

        <TestMethod>
        Public Sub testFloatingPointCasts()
            For Each t As DataType In {DataType.FLOAT, DataType.DOUBLE}
                Dim b As New ManagedDataBuffer(t, 5)
                b.put(0, 1.5)
                b.put(1, -2.7)
                b.put(2, 3.0E+10)
                b.put(3, Double.NaN)
                b.put(4, -3.0E+10)
                assertEquals(1.5, b.getDouble(0), 0.0)
                assertEquals(1, b.getInt(0))
                assertEquals(1L, b.getLong(0))
                assertEquals(-2, b.getInt(1))
                assertEquals(-2L, b.getLong(1))
                ' Java (int) saturates, (long) keeps 3e10 (exact in FLOAT too: 30000001024)
                assertEquals(Integer.MaxValue, b.getInt(2))
                assertEquals(If(t = DataType.FLOAT, 30000001024L, 30000000000L), b.getLong(2))
                assertEquals(0, b.getInt(3))
                assertEquals(0L, b.getLong(3))
                assertTrue(Double.IsNaN(b.getDouble(3)))
                assertEquals(Integer.MinValue, b.getInt(4))
            Next
            Dim f As New ManagedDataBuffer(DataType.FLOAT, 1)
            f.put(0, 0.1)
            assertEquals(CDbl(0.1F), f.getDouble(0), 0.0)
            assertEquals(0.1F, f.getFloat(0), 0.0F)
        End Sub

        <TestMethod>
        Public Sub testSignedIntegerCasts()
            Dim i As New ManagedDataBuffer(DataType.INT, 4)
            i.put(0, 3.9)
            i.put(1, CLng(Integer.MaxValue) + 1L)
            i.put(2, -1.5F)
            i.put(3, 1.0E+20)
            assertEquals(3, i.getInt(0))
            assertEquals(Integer.MinValue, i.getInt(1))
            assertEquals(-1, i.getInt(2))
            assertEquals(Integer.MaxValue, i.getInt(3))

            Dim l As New ManagedDataBuffer(DataType.LONG, 3)
            l.put(0, 2.5E+18)
            l.put(1, Double.MaxValue)
            l.put(2, Long.MinValue)
            assertEquals(2500000000000000000L, l.getLong(0))
            assertEquals(Long.MaxValue, l.getLong(1))
            assertEquals(Long.MinValue, l.getLong(2))
            ' (int) of a long wraps
            assertEquals(0, l.getInt(2))

            Dim s As New ManagedDataBuffer(DataType.SHORT, 3)
            s.put(0, 40000)
            s.put(1, 70000.0)
            s.put(2, -32769L)
            assertEquals(-25536, s.getInt(0))
            assertEquals(4464, s.getInt(1))
            assertEquals(32767, s.getInt(2))

            Dim b As New ManagedDataBuffer(DataType.BYTE, 3)
            b.put(0, 200)
            b.put(1, 300.7)
            b.put(2, -129L)
            assertEquals(-56, b.getInt(0))
            assertEquals(44, b.getInt(1))
            assertEquals(127, b.getInt(2))
        End Sub

        <TestMethod>
        Public Sub testUnsignedIntegerCasts()
            Dim ub As New ManagedDataBuffer(DataType.UBYTE, 3)
            ub.put(0, -1)
            ub.put(1, 256)
            ub.put(2, 255.9)
            assertEquals(255, ub.getInt(0))
            assertEquals(0, ub.getInt(1))
            assertEquals(255, ub.getInt(2))
            assertEquals(255.0, ub.getDouble(0), 0.0)

            Dim us As New ManagedDataBuffer(DataType.UINT16, 2)
            us.put(0, -1)
            us.put(1, 65536L)
            assertEquals(65535, us.getInt(0))
            assertEquals(0, us.getInt(1))

            Dim ui As New ManagedDataBuffer(DataType.UINT32, 2)
            ui.put(0, -1)
            ui.put(1, 4294967296.0 + 5.0)
            assertEquals(4294967295L, ui.getLong(0))
            assertEquals(-1, ui.getInt(0))
            assertEquals(4294967295.0, ui.getDouble(0), 0.0)
            assertEquals(5L, ui.getLong(1))

            Dim ul As New ManagedDataBuffer(DataType.UINT64, 2)
            ul.put(0, -1L)
            ul.put(1, 1.0E+19)
            ' getLong returns the bits (Java); getDouble reads unsigned (deliberate difference)
            assertEquals(-1L, ul.getLong(0))
            assertEquals(18446744073709551615.0, ul.getDouble(0), 0.0)
            assertEquals(1.0E+19, ul.getDouble(1), 0.0)
        End Sub

        <TestMethod>
        Public Sub testBoolAndHalfCasts()
            Dim b As New ManagedDataBuffer(DataType.BOOL, 5)
            b.put(0, 2)
            b.put(1, -1.0)  ' put(double): element > 0 (Java)
            b.put(2, -1.0F) ' put(float): element != 0 (Java)
            b.put(3, True)
            b.put(4, Double.NaN)
            assertEquals(1, b.getInt(0))
            assertEquals(0, b.getInt(1))
            assertEquals(1, b.getInt(2))
            assertEquals(1L, b.getLong(3))
            assertEquals(0.0, b.getDouble(4), 0.0)

            Dim h As New ManagedDataBuffer(DataType.HALF, 3)
            h.put(0, 1.5F)
            h.put(1, 0.333F)
            h.put(2, 7)
            assertEquals(1.5F, h.getFloat(0), 0.0F)
            assertEquals(NumericCasts.HalfToSingle(NumericCasts.SingleToHalf(0.333F)), h.getFloat(1), 0.0F)
            assertEquals(7, h.getInt(2))

            Dim bf As New ManagedDataBuffer(DataType.BFLOAT16, 1)
            Dim third As Single = 1.0F / 3.0F
            bf.put(0, third)
            assertEquals(NumericCasts.BitsToSingle(NumericCasts.SingleToBits(third) And &HFFFF0000), bf.getFloat(0), 0.0F)
        End Sub

        <TestMethod>
        Public Sub testEveryTypeStoresSmallValuesExactly()
            For Each t As DataType In NumericTypes
                Dim b As New ManagedDataBuffer(t, 3)
                assertEquals(t, b.dataType())
                assertEquals(3L, b.length())
                assertEquals(Math.Max(t.width(), 1), b.ElementSize)
                b.put(0, 1)
                b.put(1, 0L)
                b.put(2, 1.0F)
                assertEquals(1.0, b.getDouble(0), 0.0, t.name())
                assertEquals(0.0, b.getDouble(1), 0.0, t.name())
                assertEquals(1L, b.getLong(2), t.name())
            Next
        End Sub

        <TestMethod>
        Public Sub testGetNumberTypes()
            assertTrue(TypeOf filled(DataType.DOUBLE, 1).getNumber(0) Is Double)
            assertTrue(TypeOf filled(DataType.INT, 1).getNumber(0) Is Integer)
            assertTrue(TypeOf filled(DataType.LONG, 1).getNumber(0) Is Long)
            assertTrue(TypeOf filled(DataType.FLOAT, 1).getNumber(0) Is Single)
            assertTrue(TypeOf filled(DataType.SHORT, 1).getNumber(0) Is Single)
            assertTrue(TypeOf filled(DataType.UINT64, 1).getNumber(0) Is Single)
        End Sub

        <TestMethod>
        Public Sub testDataTypeStorageMapping()
            For Each t As DataType In NumericTypes
                assertEquals(t.name(), ManagedDataBuffer.toStorageType(t).ToString())
                assertEquals(t, ManagedDataBuffer.toDataType(ManagedDataBuffer.toStorageType(t)))
            Next
            assertEquals(StorageType.UTF8, ManagedDataBuffer.toStorageType(DataType.UTF8))
            assertThrows(Of ArgumentException)(Sub() ManagedDataBuffer.toStorageType(DataType.COMPRESSED))
            assertThrows(Of ArgumentException)(Sub() ManagedDataBuffer.toStorageType(DataType.UNKNOWN))
        End Sub

#End Region

#Region "Views, offsets and dup"

        <TestMethod>
        Public Sub testViewsShareStorage()
            Dim root As ManagedDataBuffer = filled(DataType.FLOAT, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9)
            Dim view As New ManagedDataBuffer(root, 4, 3)
            assertEquals(4L, view.length())
            assertEquals(3L, view.offset())
            assertEquals(3L, view.originalOffset())
            assertEquals(10L, view.underlyingLength())
            assertEquals(10L, view.capacity())
            Assert.AreSame(root, view.underlyingDataBuffer())
            Assert.AreSame(root, view.originalDataBuffer())
            Assert.AreSame(root, root.underlyingDataBuffer())
            assertNull(root.originalDataBuffer())
            assertEquals(3.0F, view.getFloat(0), 0.0F)
            view.put(1, 42.0F)
            assertEquals(42.0F, root.getFloat(4), 0.0F)
            root.put(6, -1.0F)
            assertEquals(-1.0F, view.getFloat(3), 0.0F)
            assertTrue(view.sameUnderlyingData(root))
            assertTrue(root.sameUnderlyingData(view))
            assertFalse(root.sameUnderlyingData(root.dup()))
            assertEquals(3L, view.StorageOffset)
            Assert.AreSame(root.Storage, view.Storage)

            ' a view of a view: the offset is absolute within the original buffer (ND4J M2)
            Dim inner As New ManagedDataBuffer(view, 2, 5)
            assertEquals(5.0F, inner.getFloat(0), 0.0F)
            Assert.AreSame(root, inner.originalDataBuffer())
            Assert.AreSame(view, inner.underlyingDataBuffer())

            ' Java turns a zero-length view into a length-1 view
            assertEquals(1L, New ManagedDataBuffer(root, 0, 2).length())
            assertThrows(Of ArgumentException)(Sub() Call New ManagedDataBuffer(root, -1, 0).length())
            assertTrue(root.referenced.get())
        End Sub

        <TestMethod>
        Public Sub testRecordedOffsetKeepsJavaSemantics()
            Dim b As New ManagedDataBuffer(DataType.FLOAT, New Single() {1, 2, 3, 4, 5}, 2L)
            assertEquals(3L, b.length())
            assertEquals(2L, b.offset())
            assertEquals(2L, b.originalOffset())
            assertEquals(5L, b.underlyingLength())
            ' as in Java, element 0 is data(0): the offset is only recorded
            assertEquals(1.0F, b.getFloat(0), 0.0F)
            Dim viaFactory As DataBuffer = New ManagedDataBufferFactory().createDouble(1, 4)
            assertEquals(3L, viaFactory.length())
            assertEquals(1L, viaFactory.offset())
        End Sub

        <TestMethod>
        Public Sub testDupIsExactAndIndependent()
            Dim big As New ManagedDataBuffer(DataType.LONG, 2)
            big.put(0, 9007199254740993L) ' 2^53 + 1: lost when copied through double
            big.put(1, -5L)
            Dim copy As DataBuffer = big.dup()
            assertEquals(9007199254740993L, copy.getLong(0))
            assertEquals(DataType.LONG, copy.dataType())
            copy.put(1, 7L)
            assertEquals(-5L, big.getLong(1))

            Dim root As ManagedDataBuffer = filled(DataType.DOUBLE, 1, 2, 3, 4, 5)
            root.Constant = True
            Dim viewCopy As DataBuffer = New ManagedDataBuffer(root, 2, 3).dup()
            assertEquals(2L, viewCopy.length())
            assertEquals(4.0, viewCopy.getDouble(0), 0.0)
            assertFalse(viewCopy.Constant)
            assertFalse(viewCopy.Attached)

            Dim strings As New ManagedDataBuffer(New List(Of String) From {"a", "b"})
            assertEquals("b", DirectCast(strings.dup(), ManagedDataBuffer).getString(1))
        End Sub

        <TestMethod>
        Public Sub testReallocate()
            Dim b As ManagedDataBuffer = filled(DataType.INT, 1, 2, 3)
            Dim v As New ManagedDataBuffer(b, 2, 1)
            Assert.AreSame(b, b.reallocate(5))
            assertEquals(5L, b.length())
            assertEquals(5L, b.underlyingLength())
            assertEquals(5L, b.capacity())
            assertArrayEquals(New Integer() {1, 2, 3, 0, 0}, b.asInt())
            ' the view sees the new storage
            assertEquals(3, v.getInt(1))
            b.reallocate(2)
            assertArrayEquals(New Integer() {1, 2}, b.asInt())
        End Sub

#End Region

#Region "Bulk operations"

        <TestMethod>
        Public Sub testAssignVariants()
            Dim b As New ManagedDataBuffer(DataType.DOUBLE, 6)
            b.assign(CObj(2.5))
            assertArrayEquals(New Double() {2.5, 2.5, 2.5, 2.5, 2.5, 2.5}, b.asDouble(), 0.0)
            b.assign(CObj(7), 4)
            assertArrayEquals(New Double() {2.5, 2.5, 2.5, 2.5, 7, 7}, b.asDouble(), 0.0)

            b.assign(New Long() {0, 2}, New Single() {9.0F, 8.0F}, True)
            assertEquals(9.0, b.getDouble(0), 0.0)
            assertEquals(8.0, b.getDouble(2), 0.0)
            ' the double version steps by inc (the float version ignores it, as in Java)
            b.assign(New Long() {1, 3, 5}, New Double() {-1, -3, -5}, True, 2)
            assertEquals(-1.0, b.getDouble(1), 0.0)
            assertEquals(2.5, b.getDouble(3), 0.0)
            assertEquals(-5.0, b.getDouble(5), 0.0)
            b.assign(New Long() {1, 3}, New Single() {4.0F, 6.0F}, True, 5)
            assertEquals(6.0, b.getDouble(3), 0.0)
            assertThrows(Of ArgumentException)(Sub() b.assign(New Long() {1}, New Double() {1, 2}, True))
            assertThrows(Of ArgumentException)(Sub() Call New ManagedDataBuffer(DataType.DOUBLE, 1).assign(New Long() {0, 1}, New Double() {1, 2}, True))

            Dim other As ManagedDataBuffer = filled(DataType.FLOAT, 1, 2, 3, 4, 5, 6)
            b.assign(DirectCast(other, DataBuffer))
            assertArrayEquals(New Double() {1, 2, 3, 4, 5, 6}, b.asDouble(), 0.0)
            assertThrows(Of ArgumentException)(Sub() b.assign(DirectCast(filled(DataType.FLOAT, 1), DataBuffer)))

            ' concatenation
            Dim target As New ManagedDataBuffer(DataType.INT, 5)
            target.assign(filled(DataType.INT, 1, 2), filled(DataType.INT, 3, 4, 5))
            assertArrayEquals(New Integer() {1, 2, 3, 4, 5}, target.asInt())
            ' offsets and strides: every second element from offset 1, then all
            target.assign(New Long() {1, 0}, New Long() {2, 1}, 5, filled(DataType.INT, 0, 10, 0, 20), filled(DataType.INT, 30, 40, 50))
            assertArrayEquals(New Integer() {10, 20, 30, 40, 50}, target.asInt())
            Dim ex = assertThrows(Of ArgumentException)(Sub() target.assign(New Long() {0}, New Long() {1}, 5, filled(DataType.INT, 1, 2)))
            assertEquals("Strides and offsets didn't match up to length 5", ex.Message)
            assertThrows(Of ArgumentException)(Sub() target.assign(New Long() {0, 0}, New Long() {1}, filled(DataType.INT, 1)))
        End Sub

        <TestMethod>
        Public Sub testSetDataOverloads()
            Dim b As New ManagedDataBuffer(DataType.INT, 3)
            b.setData(New Integer() {1, 2, 3})
            assertArrayEquals(New Integer() {1, 2, 3}, b.asInt())
            b.setData(New Long() {4, 5})
            assertArrayEquals(New Integer() {4, 5, 3}, b.asInt())
            b.setData(New Single() {6.9F})
            b.setData(New Double() {7.9, 8.9})
            assertArrayEquals(New Integer() {7, 8, 3}, b.asInt())
            b.setData(New Short() {-1})
            assertEquals(-1, b.getInt(0))
            ' Java bytes are signed: 200 is -56
            b.setData(New Byte() {200, 100})
            assertArrayEquals(New Integer() {-56, 100, 3}, b.asInt())
            b.setData(New Boolean() {True, False, True})
            assertArrayEquals(New Integer() {1, 0, 1}, b.asInt())
        End Sub

        <TestMethod>
        Public Sub testAsArrays()
            Dim d As ManagedDataBuffer = filled(DataType.DOUBLE, 1.5, -2.5, 1.0E+20, Double.NaN)
            assertArrayEquals(New Integer() {1, -2, Integer.MaxValue, 0}, d.asInt())
            assertArrayEquals(New Long() {1, -2, Long.MaxValue, 0}, d.asLong())
            Dim floats As Single() = d.asFloat()
            assertEquals(1.5F, floats(0), 0.0F)
            assertEquals(-2.5F, floats(1), 0.0F)
            assertEquals(1.0E+20F, floats(2), 0.0F)
            assertTrue(Single.IsNaN(floats(3)))
            Dim u As New ManagedDataBuffer(DataType.UINT32, 2)
            u.put(0, -1)
            assertArrayEquals(New Long() {4294967295L, 0}, u.asLong())
            assertArrayEquals(New Integer() {-1, 0}, u.asInt())
            Dim view As New ManagedDataBuffer(filled(DataType.SHORT, 1, 2, 3, 4), 2, 1)
            assertArrayEquals(New Double() {2, 3}, view.asDouble(), 0.0)
            Dim bools As ManagedDataBuffer = filled(DataType.BOOL, 1, 0)
            assertArrayEquals(New Single() {1.0F, 0.0F}, bools.asFloat(), 0.0F)
        End Sub

        <TestMethod>
        Public Sub testGetXAt()
            Dim b As ManagedDataBuffer = filled(DataType.DOUBLE, 0, 1, 2, 3, 4, 5, 6, 7)
            assertArrayEquals(New Double() {2, 3, 4}, b.getDoublesAt(2, 3), 0.0)
            assertArrayEquals(New Double() {1, 3, 5}, b.getDoublesAt(1, 2, 3), 0.0)
            assertArrayEquals(New Single() {4, 5}, b.getFloatsAt(4, 2), 0.0F)
            assertArrayEquals(New Integer() {0, 4}, b.getIntsAt(0, 4, 2))
            assertArrayEquals(New Long() {7}, b.getLongsAt(7, 1))
            ' Java quirk: when offset + length passes the end, offset is subtracted from length
            assertArrayEquals(New Double() {6, 7}, b.getDoublesAt(6, 8), 0.0)
        End Sub

        <TestMethod>
        Public Sub testCopyAtStride()
            Dim src As ManagedDataBuffer = filled(DataType.DOUBLE, 0, 1, 2, 3, 4, 5)
            Dim dst As New ManagedDataBuffer(DataType.FLOAT, 6)
            dst.copyAtStride(src, 3, 2, 1, 0, 3)
            assertArrayEquals(New Single() {3, 0, 4, 0, 5, 0}, dst.asFloat(), 0.0F)
        End Sub

#End Region

#Region "Streams"

        Private Shared Function roundTrip(ByVal buffer As DataBuffer) As ManagedDataBuffer
            Using ms As New MemoryStream()
                buffer.write(ms)
                ms.Position = 0
                Dim header = BaseDataBuffer.readHeader(ms)
                Dim result As New ManagedDataBuffer(header.Third, 0, True)
                result.read(ms, header.First, header.Second, header.Third)
                assertEquals(ms.Length, ms.Position)
                Return result
            End Using
        End Function

        <TestMethod>
        Public Sub testWriteReadRoundTripEveryType()
            For Each t As DataType In NumericTypes
                Dim b As New ManagedDataBuffer(t, 5)
                b.put(0, 1)
                b.put(1, 0)
                b.put(2, 100)
                b.put(3, -3L)
                b.put(4, 2.5F)
                Dim r As ManagedDataBuffer = roundTrip(b)
                assertEquals(t, r.dataType())
                assertEquals(5L, r.length())
                For i As Integer = 0 To 4
                    assertEquals(b.getLong(i), r.getLong(i), t.name() & " element " & i)
                    assertEquals(b.getDouble(i), r.getDouble(i), 0.0, t.name() & " element " & i)
                Next
            Next
            ' a view writes only its elements
            Dim view As New ManagedDataBuffer(filled(DataType.INT, 1, 2, 3, 4), 2, 2)
            assertArrayEquals(New Integer() {3, 4}, roundTrip(view).asInt())
        End Sub

        <TestMethod>
        Public Sub testUtf8RoundTripAndLayout()
            Dim b As New ManagedDataBuffer(New List(Of String) From {"ab", "héllo", ""})
            assertEquals(DataType.UTF8, b.dataType())
            assertEquals(3L, b.length())
            assertEquals(3L, b.getNumWords())
            assertEquals("héllo", b.getString(1))
            assertThrows(Of ArgumentException)(Sub() b.getString(3))
            assertThrows(Of NotSupportedException)(Sub() b.getDouble(0))
            assertThrows(Of NotSupportedException)(Sub() b.put(0, 1.0))
            assertThrows(Of InvalidOperationException)(Sub() b.put(0, 1.0F))
            assertThrows(Of NotSupportedException)(Sub() b.ToString())

            ' Java layout: 4 little-endian long offsets (0, 2, 8, 8), then the UTF-8 bytes
            Dim layout As Byte() = b.asBytes()
            assertEquals(4 * 8 + 2 + 6, layout.Length)
            assertEquals(0L, BitConverter.ToInt64(layout, 0))
            assertEquals(2L, BitConverter.ToInt64(layout, 8))
            assertEquals(8L, BitConverter.ToInt64(layout, 16))
            assertEquals(8L, BitConverter.ToInt64(layout, 24))
            assertEquals("abhéllo", Encoding.UTF8.GetString(layout, 32, 8))

            Dim fromLayout As DataBuffer = New ManagedDataBufferFactory().createUtf8Buffer(layout, 3)
            assertEquals("ab", DirectCast(fromLayout, ManagedDataBuffer).getString(0))
            assertEquals("", DirectCast(fromLayout, ManagedDataBuffer).getString(2))

            Dim r As ManagedDataBuffer = roundTrip(b)
            assertEquals(3L, r.length())
            assertEquals("héllo", r.getString(1))
            assertEquals("", r.getString(2))
        End Sub

        ''' <summary>The bytes Java's DataOutputStream writes for a header (writeUTF of ASCII text, writeLong, writeUTF).</summary>
        Private Shared Function javaHeader(ByVal mode As String, ByVal length As Long, ByVal type As String, Optional ByVal intLength As Boolean = False) As List(Of Byte)
            Dim bytes As New List(Of Byte)()
            bytes.Add(0) : bytes.Add(CByte(mode.Length))
            bytes.AddRange(Encoding.ASCII.GetBytes(mode))
            Dim size As Integer = If(intLength, 4, 8)
            For i As Integer = size - 1 To 0 Step -1
                bytes.Add(CByte((length >> (8 * i)) And &HFF))
            Next
            bytes.Add(0) : bytes.Add(CByte(type.Length))
            bytes.AddRange(Encoding.ASCII.GetBytes(type))
            Return bytes
        End Function

        <TestMethod>
        Public Sub testJavaStreamLayoutBytes()
            Using ms As New MemoryStream()
                Dim f As New ManagedDataBuffer(DataType.FLOAT, New Single() {1.0F, -2.5F})
                f.write(ms)
                Dim expected As List(Of Byte) = javaHeader("MIXED_DATA_TYPES", 2, "FLOAT")
                expected.AddRange(New Byte() {&H3F, &H80, 0, 0, &HC0, &H20, 0, 0})
                assertArrayEquals(expected.ToArray(), ms.ToArray())
            End Using
            Using ms As New MemoryStream()
                Dim h As New ManagedDataBuffer(DataType.HALF, New Single() {1.0F, -2.0F})
                Dim b As New ManagedDataBuffer(DataType.BOOL, New Boolean() {True, False})
                Dim i As New ManagedDataBuffer(DataType.INT, New Integer() {1, -1})
                h.write(ms)
                b.write(ms)
                i.write(ms)
                Dim expected As List(Of Byte) = javaHeader("MIXED_DATA_TYPES", 2, "HALF")
                expected.AddRange(New Byte() {&H3C, 0, &HC0, 0})
                expected.AddRange(javaHeader("MIXED_DATA_TYPES", 2, "BOOL"))
                expected.AddRange(New Byte() {1, 0})
                expected.AddRange(javaHeader("MIXED_DATA_TYPES", 2, "INT"))
                expected.AddRange(New Byte() {0, 0, 0, 1, &HFF, &HFF, &HFF, &HFF})
                assertArrayEquals(expected.ToArray(), ms.ToArray())
            End Using
        End Sub

        <TestMethod>
        Public Sub testReadsJavaWrittenStreams()
            ' a legacy JAVACPP header has an int length
            Dim legacy As List(Of Byte) = javaHeader("JAVACPP", 2, "DOUBLE", True)
            legacy.AddRange(BitConverter.GetBytes(&H3FF8000000000000L).Reverse()) ' 1.5
            legacy.AddRange(BitConverter.GetBytes(&HC004000000000000L).Reverse()) ' -2.5
            Using ms As New MemoryStream(legacy.ToArray())
                Dim header = BaseDataBuffer.readHeader(ms)
                assertEquals(DataBuffer.AllocationMode.JAVACPP, header.First)
                assertEquals(2L, header.Second)
                assertEquals(DataType.DOUBLE, header.Third)
                Dim b As New ManagedDataBuffer(DataType.DOUBLE, 0, True)
                b.read(ms, header.First, header.Second, header.Third)
                assertArrayEquals(New Double() {1.5, -2.5}, b.asDouble(), 0.0)
                assertEquals(DataBuffer.AllocationMode.MIXED_DATA_TYPES, b.getAllocationMode())
            End Using
            ' UINT16 elements are written as shorts: 0xFFFF reads back as 65535
            Dim u As List(Of Byte) = javaHeader("MIXED_DATA_TYPES", 2, "UINT16")
            u.AddRange(New Byte() {&HFF, &HFF, 0, 7})
            Using ms As New MemoryStream(u.ToArray())
                Dim header = BaseDataBuffer.readHeader(ms)
                Dim b As New ManagedDataBuffer(DataType.FLOAT, 0, True)
                b.read(ms, header.First, header.Second, header.Third)
                assertEquals(DataType.UINT16, b.dataType())
                assertArrayEquals(New Integer() {65535, 7}, b.asInt())
            End Using
            assertThrows(Of ArgumentException)(Sub() BaseDataBuffer.readHeader(New MemoryStream(javaHeader("NOPE", 1, "INT").ToArray())))
        End Sub

        <TestMethod>
        Public Sub testAsBytesIsBigEndian()
            assertArrayEquals(New Byte() {0, 0, 0, 1, &HFF, &HFF, &HFF, &HFE}, New ManagedDataBuffer(DataType.INT, New Integer() {1, -2}).asBytes())
            assertArrayEquals(New Byte() {&H3C, 0}, New ManagedDataBuffer(DataType.HALF, New Single() {1.0F}).asBytes())
            assertArrayEquals(New Byte() {&H3F, &H80}, New ManagedDataBuffer(DataType.BFLOAT16, New Single() {1.0F}).asBytes())
            assertArrayEquals(New Byte() {&HFF, 1}, New ManagedDataBuffer(DataType.UBYTE, New Integer() {255, 1}).asBytes())
            assertArrayEquals(New Byte() {1, 0}, New ManagedDataBuffer(DataType.BOOL, New Boolean() {True, False}).asBytes())
            assertArrayEquals(New Byte() {&HFF, &HFF, &HFF, &HFF}, New ManagedDataBuffer(DataType.UINT32, New Long() {4294967295L}).asBytes())
            assertEquals(16, New ManagedDataBuffer(DataType.UINT64, New Long() {1, 2}).asBytes().Length)
        End Sub

#End Region

#Region "Lifecycle and flags"

        <TestMethod>
        Public Sub testCloseAndDispose()
            Dim root As ManagedDataBuffer = filled(DataType.DOUBLE, 1, 2, 3)
            Dim view As New ManagedDataBuffer(root, 2, 1)
            assertTrue(root.closeable())
            assertFalse(view.closeable())
            assertThrows(Of InvalidOperationException)(Sub() view.close())
            root.close()
            assertTrue(root.wasClosed())
            assertTrue(view.wasClosed())
            assertFalse(root.closeable())
            assertThrows(Of InvalidOperationException)(Sub() root.getDouble(0))
            assertThrows(Of InvalidOperationException)(Sub() view.getDouble(0))
            assertThrows(Of InvalidOperationException)(Sub() root.put(0, 1.0))
            assertThrows(Of InvalidOperationException)(Sub() root.close())
            ' Dispose after close does nothing
            root.Dispose()
            assertThrows(Of InvalidOperationException)(Sub() Call New ManagedDataBuffer(root, 1, 0).length())

            Dim disposed As ManagedDataBuffer
            Using b As New ManagedDataBuffer(DataType.INT, 2)
                disposed = b
            End Using
            assertTrue(disposed.wasClosed())

            Dim constant As ManagedDataBuffer = filled(DataType.INT, 1)
            constant.Constant = True
            assertTrue(constant.Constant)
            assertFalse(constant.closeable())
            assertThrows(Of InvalidOperationException)(Sub() constant.Dispose())
        End Sub

        <TestMethod>
        Public Sub testConstantBuffersAreNotReferencedByViews()
            Dim constant As ManagedDataBuffer = filled(DataType.INT, 1, 2)
            constant.Constant = True
            Dim view As New ManagedDataBuffer(constant, 1, 1)
            assertFalse(constant.referenced.get())
            assertEquals(2, view.getInt(0))
        End Sub

        <TestMethod>
        Public Sub testWorkspaceFlags()
            Dim mm As New ManagedMemoryManager()
            Dim wm As New ManagedWorkspaceManager(mm, Nothing)
            Dim config As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1024 * 1024).policyLearning(Global.DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.LearningPolicy.NONE).build()
            Dim attached As ManagedDataBuffer
            Dim view As ManagedDataBuffer
            Dim detached As New ManagedDataBuffer(DataType.FLOAT, 4)
            assertFalse(detached.Attached)
            assertTrue(detached.InScope)
            assertNull(detached.ParentWorkspace)
            assertEquals(0L, detached.GenerationId)
            Dim ws As MemoryWorkspace = wm.getAndActivateWorkspace(config, "BUFFER_WS")
            Using ws
                attached = New ManagedDataBuffer(DataType.FLOAT, 10, True, ws)
                assertTrue(attached.Attached)
                assertTrue(attached.InScope)
                Assert.AreSame(ws, attached.ParentWorkspace)
                assertEquals(ws.GenerationId, attached.GenerationId)
                ' 40 bytes, aligned to 32
                assertEquals(64L, ws.CurrentOffset)
                assertFalse(attached.closeable())
                ' a view is not attached itself but reports the parent workspace and generation (Java)
                view = New ManagedDataBuffer(attached, 2, 1)
                assertFalse(view.Attached)
                Assert.AreSame(ws, view.ParentWorkspace)
                assertEquals(attached.GenerationId, view.GenerationId)
                ' a DUMMY workspace does not attach
                assertFalse(New ManagedDataBuffer(DataType.INT, 2, True, New Global.DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.DummyWorkspace(mm)).Attached)
                assertThrows(Of ArgumentException)(Sub() Call New ManagedDataBuffer(DataType.INT, 0, True, ws).length())
            End Using
            assertFalse(attached.InScope)
            Dim before As Long = attached.GenerationId
            ws.notifyScopeEntered()
            ws.close()
            assertEquals(before + 1, ws.GenerationId)
            assertEquals(before, attached.GenerationId)
        End Sub

        <TestMethod>
        Public Sub testEqualsHashCodeAndToString()
            Dim a As ManagedDataBuffer = filled(DataType.DOUBLE, 1, 2, 3)
            Dim b As ManagedDataBuffer = filled(DataType.FLOAT, 1, 2, 3)
            assertTrue(a.Equals(b))
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertFalse(a.Equals(filled(DataType.DOUBLE, 1, 2, 4)))
            assertFalse(a.Equals(filled(DataType.DOUBLE, 1, 2)))
            ' Java quirk: anything that is not a data buffer is "equal"
            assertTrue(a.Equals("not a buffer"))

            assertEquals("[1,2,3]", filled(DataType.INT, 1, 2, 3).ToString())
            assertEquals("[1.0,2.5,-0.125]", filled(DataType.FLOAT, 1, 2.5, -0.125).ToString())
            assertEquals("[ true, false]", filled(DataType.BOOL, 1, 0).ToString())
            Dim big As New ManagedDataBuffer(DataType.INT, 1002)
            assertTrue(big.ToString().EndsWith(",0,<2 more elements>]"))
        End Sub

        <TestMethod>
        Public Sub testDeprecatedMembers()
            Dim b As DataBuffer = filled(DataType.INT, 1)
            assertThrows(Of NotSupportedException)(Sub() b.persist())
            assertThrows(Of NotSupportedException)(Sub() b.isPersist())
            assertThrows(Of NotSupportedException)(Sub() b.unPersist())
            assertThrows(Of NotSupportedException)(Sub() b.references())
            b.addReferencing("x")
            b.removeReferencing("x")
            assertFalse(b.dirty())
            assertNull(b.array())
            b.flush()
            b.destroy()
            assertEquals(DataBuffer.AllocationMode.MIXED_DATA_TYPES, b.getAllocationMode())
        End Sub

        <TestMethod>
        Public Sub testCreateFromArraysAndStorage()
            Dim fromLongs As New ManagedDataBuffer(DataType.FLOAT, New Long() {1, 2, 3})
            assertArrayEquals(New Single() {1, 2, 3}, fromLongs.asFloat(), 0.0F)
            Dim s As ManagedStorage = ManagedStorage.FromArray(New Double() {4, 5})
            Dim wrapped As New ManagedDataBuffer(s)
            assertEquals(DataType.DOUBLE, wrapped.dataType())
            s.SetDouble(0, 9.0)
            assertEquals(9.0, wrapped.getDouble(0), 0.0)
            ' arrays are copied
            Dim data As Integer() = {1, 2}
            Dim copied As New ManagedDataBuffer(DataType.INT, data)
            data(0) = 7
            assertEquals(1, copied.getInt(0))
            assertThrows(Of NotSupportedException)(Sub() Call New ManagedDataBuffer(New List(Of String) From {"x"}).create(New Double() {1}))
            assertEquals(DataType.INT, copied.create(New Double() {1.5}).dataType())
        End Sub

#End Region

    End Class

End Namespace
