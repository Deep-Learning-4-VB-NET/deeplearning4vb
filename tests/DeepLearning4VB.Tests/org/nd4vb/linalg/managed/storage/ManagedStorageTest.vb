Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>The typed storages: creation, element access with Java cast semantics, bulk copies, fills and cloning.</summary>
    <TestClass>
    Public Class ManagedStorageTest

        Private Shared ReadOnly AllTypes As StorageType() = CType([Enum].GetValues(GetType(StorageType)), StorageType())

        <TestMethod>
        Public Sub CreateGivesZeroFilledStorageOfEveryType()
            For Each t As StorageType In AllTypes
                Dim s As ManagedStorage = ManagedStorage.Create(t, 5)
                Assert.AreEqual(t, s.StorageType, t.ToString())
                Assert.AreEqual(5L, s.Length)
                Assert.AreEqual(StorageTypes.ElementType(t), s.RawArray.GetType().GetElementType(), t.ToString())
                If t = StorageType.UTF8 Then
                    Assert.IsNull(s.GetString(0))
                Else
                    For i As Integer = 0 To 4
                        Assert.AreEqual(0.0, s.GetDouble(i), 0.0, t.ToString())
                        Assert.AreEqual(0L, s.GetLong(i), t.ToString())
                        Assert.IsFalse(s.GetBoolean(i), t.ToString())
                    Next
                End If
            Next
        End Sub

        <TestMethod>
        Public Sub TypedFastPathsMatchTheType()
            For Each t As StorageType In AllTypes
                Dim s As ManagedStorage = ManagedStorage.Create(t, 2)
                Assert.AreEqual(t = StorageType.FLOAT, s.Floats IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.DOUBLE, s.Doubles IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.HALF OrElse t = StorageType.BFLOAT16, s.HalfBits IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.BYTE, s.SBytes IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.UBYTE, s.UBytes IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.SHORT, s.Shorts IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.UINT16, s.UShorts IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.INT, s.Ints IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.UINT32, s.UInts IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.LONG, s.Longs IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.UINT64, s.ULongs IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.BOOL, s.Booleans IsNot Nothing, t.ToString())
                Assert.AreEqual(t = StorageType.UTF8, s.Strings IsNot Nothing, t.ToString())
                Assert.AreSame(s.RawArray, CType(s.GetType().GetProperty("Data").GetValue(s), Array), t.ToString())
            Next
        End Sub

        <TestMethod>
        Public Sub SetDoubleFollowsJavaCasts()
            CheckDouble(StorageType.DOUBLE, 2.75, 2.75, 2)
            CheckDouble(StorageType.FLOAT, 2.75, 2.75, 2)
            CheckDouble(StorageType.FLOAT, 1.0E+40, Double.PositiveInfinity, Long.MaxValue)
            CheckDouble(StorageType.HALF, 2.75, 2.75, 2)
            CheckDouble(StorageType.HALF, -0.125, -0.125, 0)
            CheckDouble(StorageType.BFLOAT16, 2.75, 2.75, 2)
            CheckDouble(StorageType.LONG, -3.9, -3.0, -3)
            CheckDouble(StorageType.LONG, Double.NaN, 0.0, 0)
            CheckDouble(StorageType.LONG, 1.0E+300, 9.2233720368547758E+18, Long.MaxValue)
            CheckDouble(StorageType.INT, 3.0E+10, Integer.MaxValue, Integer.MaxValue)
            CheckDouble(StorageType.INT, -3.0E+10, Integer.MinValue, Integer.MinValue)
            CheckDouble(StorageType.INT, Double.NaN, 0.0, 0)
            CheckDouble(StorageType.SHORT, 70000.0, 4464.0, 4464)
            CheckDouble(StorageType.SHORT, -1.5, -1.0, -1)
            CheckDouble(StorageType.BYTE, 300.0, 44.0, 44)
            CheckDouble(StorageType.BYTE, -129.5, 127.0, 127)
            CheckDouble(StorageType.UBYTE, -1.0, 255.0, 255)
            CheckDouble(StorageType.UBYTE, 256.9, 0.0, 0)
            CheckDouble(StorageType.UINT16, -1.0, 65535.0, 65535)
            CheckDouble(StorageType.UINT32, -1.0, 4294967295.0, 4294967295L)
            CheckDouble(StorageType.UINT32, 4294967296.0, 0.0, 0)
            CheckDouble(StorageType.UINT64, -1.0, 1.8446744073709552E+19, -1)
            CheckDouble(StorageType.UINT64, 1.0E+19, 1.0E+19, NumericCasts.ULongToLong(10000000000000000000UL))
            CheckDouble(StorageType.BOOL, 0.5, 1.0, 1)
            CheckDouble(StorageType.BOOL, -2.0, 1.0, 1)
            CheckDouble(StorageType.BOOL, Double.NaN, 1.0, 1)
            CheckDouble(StorageType.BOOL, 0.0, 0.0, 0)
        End Sub

        Private Shared Sub CheckDouble(ByVal t As StorageType, ByVal stored As Double, ByVal expectedDouble As Double, ByVal expectedLong As Long)
            Dim s As ManagedStorage = ManagedStorage.Create(t, 3)
            s.SetDouble(1, stored)
            Assert.AreEqual(expectedDouble, s.GetDouble(1), 0.0, t.ToString() & " <- " & stored)
            Assert.AreEqual(expectedLong, s.GetLong(1), t.ToString() & " <- " & stored & " (long)")
            Assert.AreEqual(0.0, s.GetDouble(0), 0.0)
            Assert.AreEqual(0.0, s.GetDouble(2), 0.0)
        End Sub

        <TestMethod>
        Public Sub SetLongWrapsAndKeepsSixtyFourBitValues()
            CheckLong(StorageType.INT, &H100000005L, 5, 5.0)
            CheckLong(StorageType.INT, Long.MaxValue, -1, -1.0)
            CheckLong(StorageType.SHORT, 65537, 1, 1.0)
            CheckLong(StorageType.BYTE, -129, 127, 127.0)
            CheckLong(StorageType.UBYTE, -1, 255, 255.0)
            CheckLong(StorageType.UINT16, 65536 + 7, 7, 7.0)
            CheckLong(StorageType.UINT32, -1, 4294967295L, 4294967295.0)
            CheckLong(StorageType.UINT64, -1, -1, 1.8446744073709552E+19)
            CheckLong(StorageType.LONG, Long.MaxValue, Long.MaxValue, 9.2233720368547758E+18)
            CheckLong(StorageType.LONG, Long.MinValue, Long.MinValue, -9.2233720368547758E+18)
            CheckLong(StorageType.FLOAT, 16777217, 16777216, 16777216.0)
            CheckLong(StorageType.DOUBLE, 9007199254740993L, 9007199254740992L, 9007199254740992.0)
            CheckLong(StorageType.HALF, 3, 3, 3.0)
            CheckLong(StorageType.BOOL, 42, 1, 1.0)
            CheckLong(StorageType.BOOL, 0, 0, 0.0)
        End Sub

        Private Shared Sub CheckLong(ByVal t As StorageType, ByVal stored As Long, ByVal expectedLong As Long, ByVal expectedDouble As Double)
            Dim s As ManagedStorage = ManagedStorage.Create(t, 2)
            s.SetLong(1, stored)
            Assert.AreEqual(expectedLong, s.GetLong(1), t.ToString() & " <- " & stored)
            Assert.AreEqual(expectedDouble, s.GetDouble(1), 0.0, t.ToString() & " <- " & stored & " (double)")
        End Sub

        <TestMethod>
        Public Sub IntFloatAndBooleanAccessors()
            Dim l As ManagedStorage = ManagedStorage.Create(StorageType.LONG, 2)
            l.SetLong(0, &H1FFFFFFFFL)
            Assert.AreEqual(-1, l.GetInt(0))
            l.SetLong(1, 9007199254740993L)
            ' long to float rounds once (via double it would round twice)
            Assert.AreEqual(CSng(9007199254740993L), l.GetFloat(1))
            Dim f As ManagedStorage = ManagedStorage.Create(StorageType.FLOAT, 2)
            f.SetFloat(0, 3.5F)
            Assert.AreEqual(3, f.GetInt(0))
            Assert.AreEqual(3.5F, f.GetFloat(0))
            f.SetInt(1, -7)
            Assert.AreEqual(-7.0, f.GetDouble(1), 0.0)
            f.SetBoolean(1, True)
            Assert.AreEqual(1.0F, f.GetFloat(1))
            Assert.IsTrue(f.GetBoolean(1))
            Dim d As ManagedStorage = ManagedStorage.Create(StorageType.DOUBLE, 1)
            d.SetDouble(0, Double.NaN)
            Assert.IsTrue(d.GetBoolean(0))
            Assert.AreEqual(0, d.GetInt(0))
            Dim b As ManagedStorage = ManagedStorage.Create(StorageType.BOOL, 2)
            b.SetBoolean(1, True)
            Assert.IsTrue(b.GetBoolean(1))
            Assert.AreEqual(1, b.GetInt(1))
            Assert.AreEqual(1.0F, b.GetFloat(1))
            b.SetInt(0, 5)
            Assert.IsTrue(b.Booleans(0))
            Dim h As ManagedStorage = ManagedStorage.Create(StorageType.HALF, 1)
            h.SetFloat(0, 0.1F)
            Assert.AreEqual(0.1F, h.GetFloat(0), 0.0001F)
            Assert.AreEqual(NumericCasts.SingleToHalf(0.1F), h.HalfBits(0))
            Dim u As ManagedStorage = ManagedStorage.Create(StorageType.UINT32, 1)
            u.SetLong(0, 4294967295L)
            Assert.AreEqual(-1, u.GetInt(0))
        End Sub

        <TestMethod>
        Public Sub StringAccess()
            Dim d As ManagedStorage = ManagedStorage.Create(StorageType.DOUBLE, 2)
            d.SetString(0, "1.5E3")
            Assert.AreEqual(1500.0, d.GetDouble(0), 0.0)
            Assert.AreEqual("1500", d.GetString(0))
            d.SetDouble(1, 0.1)
            Assert.AreEqual("0.1", d.GetString(1))
            Dim l As ManagedStorage = ManagedStorage.Create(StorageType.LONG, 1)
            l.SetString(0, "9007199254740993")
            Assert.AreEqual(9007199254740993L, l.GetLong(0))
            Assert.AreEqual("9007199254740993", l.GetString(0))
            Dim u As ManagedStorage = ManagedStorage.Create(StorageType.UINT64, 1)
            u.SetString(0, "18446744073709551615")
            Assert.AreEqual(ULong.MaxValue, u.ULongs(0))
            Assert.AreEqual("18446744073709551615", u.GetString(0))
            Dim b As ManagedStorage = ManagedStorage.Create(StorageType.BOOL, 2)
            b.SetString(0, "true")
            Assert.AreEqual("true", b.GetString(0))
            Assert.AreEqual("false", b.GetString(1))
            Dim s As ManagedStorage = ManagedStorage.FromArray(New String() {"a", "b"})
            Assert.AreEqual("b", s.GetString(1))
            s.SetString(0, "z")
            Assert.AreEqual("z", s.Strings(0))
            Assert.ThrowsException(Of NotSupportedException)(Function() s.GetDouble(0))
            Assert.ThrowsException(Of NotSupportedException)(Sub() s.SetDouble(0, 1.0))
            Assert.ThrowsException(Of NotSupportedException)(Function() s.GetLong(0))
            Assert.ThrowsException(Of NotSupportedException)(Sub() s.SetLong(0, 1))
            Assert.ThrowsException(Of NotSupportedException)(Sub() s.Fill(1.0))
        End Sub

        <TestMethod>
        Public Sub FromArrayWrapsOrCopies()
            Dim data As Single() = {1.0F, 2.0F, 3.0F}
            Dim shared1 As FloatStorage = ManagedStorage.FromArray(data)
            Assert.AreSame(data, shared1.Data)
            shared1.SetDouble(0, 9.0)
            Assert.AreEqual(9.0F, data(0))
            Dim copied As FloatStorage = ManagedStorage.FromArray(data, True)
            Assert.AreNotSame(data, copied.Data)
            copied.SetDouble(1, 7.0)
            Assert.AreEqual(2.0F, data(1))

            Assert.AreEqual(StorageType.UINT16, ManagedStorage.FromArray(New UShort() {1US}).StorageType)
            Dim half As ManagedStorage = ManagedStorage.FromArray(New UShort() {&H3C00US}, StorageType.HALF)
            Assert.AreEqual(StorageType.HALF, half.StorageType)
            Assert.AreEqual(1.0, half.GetDouble(0), 0.0)
            Dim bf As ManagedStorage = ManagedStorage.FromArray(New UShort() {&H3F80US}, StorageType.BFLOAT16)
            Assert.AreEqual(1.0, bf.GetDouble(0), 0.0)
            Assert.ThrowsException(Of ArgumentException)(Function() ManagedStorage.FromArray(New UShort() {1US}, StorageType.INT))

            Assert.AreEqual(StorageType.DOUBLE, ManagedStorage.FromArray(New Double() {1.0}).StorageType)
            Assert.AreEqual(StorageType.BYTE, ManagedStorage.FromArray(New SByte() {1}).StorageType)
            Assert.AreEqual(StorageType.UBYTE, ManagedStorage.FromArray(New Byte() {1}).StorageType)
            Assert.AreEqual(StorageType.SHORT, ManagedStorage.FromArray(New Short() {1}).StorageType)
            Assert.AreEqual(StorageType.INT, ManagedStorage.FromArray(New Integer() {1}).StorageType)
            Assert.AreEqual(StorageType.UINT32, ManagedStorage.FromArray(New UInteger() {1UI}).StorageType)
            Assert.AreEqual(StorageType.LONG, ManagedStorage.FromArray(New Long() {1L}).StorageType)
            Assert.AreEqual(StorageType.UINT64, ManagedStorage.FromArray(New ULong() {1UL}).StorageType)
            Assert.AreEqual(StorageType.BOOL, ManagedStorage.FromArray(New Boolean() {True}).StorageType)
            Assert.AreEqual(StorageType.UTF8, ManagedStorage.FromArray(New String() {"x"}).StorageType)

            Dim ints As Integer() = {4, 5}
            Dim wrapped As ManagedStorage = ManagedStorage.Wrap(ints)
            Assert.AreEqual(StorageType.INT, wrapped.StorageType)
            Assert.AreSame(ints, wrapped.RawArray)
            Assert.AreEqual(StorageType.UINT16, ManagedStorage.Wrap(New UShort(2) {}).StorageType)
            Assert.ThrowsException(Of ArgumentException)(Function() ManagedStorage.Wrap(New Decimal(1) {}))
            Assert.ThrowsException(Of ArgumentException)(Function() ManagedStorage.Wrap(New Integer(1, 1) {}))
        End Sub

        <TestMethod>
        Public Sub CreateValidatesLength()
            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Function() ManagedStorage.Create(StorageType.FLOAT, -1))
            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Function() ManagedStorage.Create(StorageType.FLOAT, CLng(Integer.MaxValue) + 1))
            Assert.AreEqual(0L, ManagedStorage.Create(StorageType.DOUBLE, 0).Length)
        End Sub

        <TestMethod>
        Public Sub CopyToSameTypeBehavesLikeMemmove()
            Dim s As ManagedStorage = ManagedStorage.FromArray(New Integer() {1, 2, 3, 4, 5, 6})
            s.CopyTo(0, s, 2, 4)
            CollectionAssert.AreEqual(New Integer() {1, 2, 1, 2, 3, 4}, s.Ints)
            Dim t As ManagedStorage = ManagedStorage.FromArray(New Integer() {1, 2, 3, 4, 5, 6})
            t.CopyTo(2, t, 0, 4)
            CollectionAssert.AreEqual(New Integer() {3, 4, 5, 6, 5, 6}, t.Ints)
            Dim h As ManagedStorage = ManagedStorage.FromArray(New UShort() {&H3C00US, &H4000US}, StorageType.HALF)
            Dim h2 As ManagedStorage = ManagedStorage.Create(StorageType.HALF, 2)
            h.CopyTo(h2)
            CollectionAssert.AreEqual(New UShort() {&H3C00US, &H4000US}, h2.HalfBits)
        End Sub

        <TestMethod>
        Public Sub CopyToConvertsBetweenTypes()
            Dim f As ManagedStorage = ManagedStorage.FromArray(New Single() {1.9F, -2.9F, 300.5F})
            Dim i As ManagedStorage = ManagedStorage.Create(StorageType.INT, 3)
            f.CopyTo(0, i, 0, 3)
            CollectionAssert.AreEqual(New Integer() {1, -2, 300}, i.Ints)
            Dim sb As ManagedStorage = ManagedStorage.Create(StorageType.BYTE, 3)
            f.CopyTo(0, sb, 0, 3)
            CollectionAssert.AreEqual(New SByte() {1, -2, 44}, sb.SBytes)

            Dim l As ManagedStorage = ManagedStorage.FromArray(New Long() {Long.MaxValue, -1L})
            Dim u As ManagedStorage = ManagedStorage.Create(StorageType.UINT64, 2)
            l.CopyTo(0, u, 0, 2)
            CollectionAssert.AreEqual(New ULong() {CULng(Long.MaxValue), ULong.MaxValue}, u.ULongs)
            Dim back As ManagedStorage = ManagedStorage.Create(StorageType.LONG, 2)
            u.CopyTo(0, back, 0, 2)
            CollectionAssert.AreEqual(New Long() {Long.MaxValue, -1L}, back.Longs)

            Dim bools As ManagedStorage = ManagedStorage.Create(StorageType.BOOL, 3)
            ManagedStorage.FromArray(New Integer() {0, 5, -1}).CopyTo(0, bools, 0, 3)
            CollectionAssert.AreEqual(New Boolean() {False, True, True}, bools.Booleans)
            Dim fromBool As ManagedStorage = ManagedStorage.Create(StorageType.FLOAT, 3)
            bools.CopyTo(0, fromBool, 0, 3)
            CollectionAssert.AreEqual(New Single() {0.0F, 1.0F, 1.0F}, fromBool.Floats)

            Dim d As ManagedStorage = ManagedStorage.FromArray(New Double() {0.5, 1.0E-5, 70000.0})
            Dim h As ManagedStorage = ManagedStorage.Create(StorageType.HALF, 3)
            d.CopyTo(0, h, 0, 3)
            Assert.AreEqual(0.5, h.GetDouble(0), 0.0)
            Assert.AreEqual(1.0E-5, h.GetDouble(1), 1.0E-7)
            Assert.AreEqual(65504.0, h.GetDouble(2), 0.0)
            Dim f2 As ManagedStorage = ManagedStorage.Create(StorageType.FLOAT, 3)
            h.CopyTo(0, f2, 0, 3)
            Assert.AreEqual(0.5F, f2.Floats(0))

            Dim df As ManagedStorage = ManagedStorage.Create(StorageType.FLOAT, 2)
            ManagedStorage.FromArray(New Double() {0.1, 1.0E+300}).CopyTo(0, df, 0, 2)
            Assert.AreEqual(0.1F, df.Floats(0))
            Assert.IsTrue(Single.IsPositiveInfinity(df.Floats(1)))
            Dim fd As ManagedStorage = ManagedStorage.Create(StorageType.DOUBLE, 1)
            ManagedStorage.FromArray(New Single() {0.1F}).CopyTo(0, fd, 0, 1)
            Assert.AreEqual(CDbl(0.1F), fd.Doubles(0), 0.0)

            Dim text As ManagedStorage = ManagedStorage.Create(StorageType.UTF8, 2)
            ManagedStorage.FromArray(New Double() {1.5, -2.0}).CopyTo(0, text, 0, 2)
            CollectionAssert.AreEqual(New String() {"1.5", "-2"}, text.Strings)
            Dim parsed As ManagedStorage = ManagedStorage.Create(StorageType.INT, 2)
            text.CopyTo(0, parsed, 0, 2)
            CollectionAssert.AreEqual(New Integer() {1, -2}, parsed.Ints)

            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Sub() f.CopyTo(2, i, 0, 2))
        End Sub

        <TestMethod>
        Public Sub CopyFromAndToArrays()
            Dim f As ManagedStorage = ManagedStorage.Create(StorageType.FLOAT, 4)
            f.CopyFrom(New Integer() {7, 8, 9}, 1, 1, 2)
            CollectionAssert.AreEqual(New Single() {0.0F, 8.0F, 9.0F, 0.0F}, f.Floats)
            f.CopyFrom(New Single() {1.0F, 2.0F, 3.0F, 4.0F})
            CollectionAssert.AreEqual(New Single() {1.0F, 2.0F, 3.0F, 4.0F}, f.Floats)
            Dim ints As Integer() = New Integer(3) {}
            ManagedStorage.FromArray(New Double() {1.9, -1.9, 1.0E+20, Double.NaN}).CopyToArray(0, ints, 0, 4)
            CollectionAssert.AreEqual(New Integer() {1, -1, Integer.MaxValue, 0}, ints)

            Dim h As ManagedStorage = ManagedStorage.Create(StorageType.HALF, 2)
            h.CopyFrom(New UShort() {&H3C00US, &HC000US})
            Assert.AreEqual(1.0, h.GetDouble(0), 0.0)
            Assert.AreEqual(-2.0, h.GetDouble(1), 0.0)
            Dim bits As UShort() = New UShort(1) {}
            h.CopyToArray(0, bits, 0, 2)
            CollectionAssert.AreEqual(New UShort() {&H3C00US, &HC000US}, bits)
            Dim singles As Single() = New Single(1) {}
            h.CopyToArray(0, singles, 0, 2)
            CollectionAssert.AreEqual(New Single() {1.0F, -2.0F}, singles)
            Dim l As ManagedStorage = ManagedStorage.Create(StorageType.LONG, 2)
            l.CopyFrom(New Double() {-2.5, 3.99})
            CollectionAssert.AreEqual(New Long() {-2L, 3L}, l.Longs)
            Dim bytes As Byte() = New Byte(1) {}
            ManagedStorage.FromArray(New Integer() {-1, 256}).CopyToArray(0, bytes, 0, 2)
            CollectionAssert.AreEqual(New Byte() {255, 0}, bytes)
        End Sub

        <TestMethod>
        Public Sub FillConvertsOnce()
            Dim i As ManagedStorage = ManagedStorage.Create(StorageType.INT, 5)
            i.Fill(2.9)
            CollectionAssert.AreEqual(New Integer() {2, 2, 2, 2, 2}, i.Ints)
            i.Fill(7L, 1, 2)
            CollectionAssert.AreEqual(New Integer() {2, 7, 7, 2, 2}, i.Ints)
            Dim u As ManagedStorage = ManagedStorage.Create(StorageType.UINT64, 2)
            u.Fill(-1L)
            CollectionAssert.AreEqual(New ULong() {ULong.MaxValue, ULong.MaxValue}, u.ULongs)
            Dim b As ManagedStorage = ManagedStorage.Create(StorageType.BOOL, 3)
            b.Fill(0.25, 1, 2)
            CollectionAssert.AreEqual(New Boolean() {False, True, True}, b.Booleans)
            Dim h As ManagedStorage = ManagedStorage.Create(StorageType.HALF, 2)
            h.Fill(1.5)
            Assert.AreEqual(1.5, h.GetDouble(1), 0.0)
            Dim bf As ManagedStorage = ManagedStorage.Create(StorageType.BFLOAT16, 2)
            bf.Fill(-4.0)
            Assert.AreEqual(-4.0, bf.GetDouble(0), 0.0)
            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Sub() i.Fill(1.0, 4, 2))
        End Sub

        <TestMethod>
        Public Sub CloneAndConvert()
            For Each t As StorageType In AllTypes
                Dim s As ManagedStorage = ManagedStorage.Create(t, 4)
                For k As Integer = 0 To 3
                    If t = StorageType.UTF8 Then
                        s.SetString(k, "v" & k)
                    Else
                        s.SetLong(k, k)
                    End If
                Next
                Dim c As ManagedStorage = s.Clone()
                Assert.AreEqual(t, c.StorageType)
                Assert.AreNotSame(s.RawArray, c.RawArray)
                CollectionAssert.AreEqual(s.RawArray, c.RawArray, t.ToString())
                Dim part As ManagedStorage = s.Clone(1, 2)
                Assert.AreEqual(2L, part.Length)
                Assert.AreEqual(s.GetString(1), part.GetString(0), t.ToString())
                Assert.AreEqual(s.GetString(2), part.GetString(1), t.ToString())
            Next
            Dim f As ManagedStorage = ManagedStorage.FromArray(New Single() {1.5F, -2.5F})
            Dim asLong As ManagedStorage = f.ConvertTo(StorageType.LONG)
            CollectionAssert.AreEqual(New Long() {1L, -2L}, asLong.Longs)
            Dim same As ManagedStorage = f.ConvertTo(StorageType.FLOAT)
            Assert.AreNotSame(f, same)
            CollectionAssert.AreEqual(f.Floats, same.Floats)
            Dim fromArray As ManagedStorage = ManagedStorage.Create(StorageType.SHORT, New Double() {1.5, 70000.0})
            CollectionAssert.AreEqual(New Short() {1, 4464}, fromArray.Shorts)
            Assert.AreEqual("SHORT[2]", fromArray.ToString())
        End Sub

        ''' <summary>Edge values for conversions: fractions, integer boundaries of every width, half/float limits, NaN and infinities.</summary>
        Friend Shared ReadOnly EdgeValues As Double() = {
            0.0, 1.0, -1.0, 2.5, -2.5, 127.9, 128.0, -128.5, -129.0, 255.0, 256.0, 32767.5, -32769.0, 65535.0, 65536.0,
            2147483647.0, 2147483648.0, -2147483649.0, 4294967295.0, 4294967296.0, 1.0E+10, 9007199254740993.0, -9.2233720368547758E+18,
            1.0E+19, 1.0E+20, 1.0E-5, -3.0E-8, 0.1, 65504.0, 65520.0, 70000.0, 3.4E+38, 1.0E+300, -1.0E+300,
            Double.NaN, Double.PositiveInfinity, Double.NegativeInfinity}

        ''' <summary>The documented element rule of a converting copy (see ManagedStorage.CopyTo).</summary>
        Friend Shared Sub ConvertByRule(ByVal source As ManagedStorage, ByVal sourceIndex As Long, ByVal destination As ManagedStorage, ByVal destinationIndex As Long)
            Select Case destination.StorageType
                Case StorageType.FLOAT, StorageType.HALF, StorageType.BFLOAT16
                    destination.SetFloat(destinationIndex, source.GetFloat(sourceIndex))
                Case StorageType.DOUBLE
                    destination.SetDouble(destinationIndex, source.GetDouble(sourceIndex))
                Case Else
                    If StorageTypes.IsFloatingPoint(source.StorageType) Then
                        destination.SetDouble(destinationIndex, source.GetDouble(sourceIndex))
                    Else
                        destination.SetLong(destinationIndex, source.GetLong(sourceIndex))
                    End If
            End Select
        End Sub

        ''' <summary>Asserts that two storages of the same type hold identical elements (raw values, so NaN and bit patterns compare exactly).</summary>
        Friend Shared Sub AssertSameElements(ByVal expected As ManagedStorage, ByVal actual As ManagedStorage, ByVal label As String)
            Assert.AreEqual(expected.StorageType, actual.StorageType, label)
            CollectionAssert.AreEqual(expected.RawArray, actual.RawArray, label)
        End Sub

        <TestMethod>
        Public Sub BulkConversionsFollowTheElementRuleForEveryTypePair()
            Dim numeric As New System.Collections.Generic.List(Of StorageType)()
            For Each t As StorageType In AllTypes
                If t <> StorageType.UTF8 Then
                    numeric.Add(t)
                End If
            Next
            Dim n As Integer = EdgeValues.Length
            For Each st As StorageType In numeric
                Dim src As ManagedStorage = ManagedStorage.Create(st, n + 2)
                For i As Integer = 0 To n - 1
                    src.SetDouble(i + 1, EdgeValues(i))
                Next
                For Each dt As StorageType In numeric
                    Dim label As String = st.ToString() & " -> " & dt.ToString()
                    Dim expected As ManagedStorage = ManagedStorage.Create(dt, n + 1)
                    For i As Integer = 0 To n - 1
                        ConvertByRule(src, i + 1, expected, i)
                    Next
                    Dim viaCopy As ManagedStorage = ManagedStorage.Create(dt, n + 1)
                    src.CopyTo(1, viaCopy, 0, n)
                    AssertSameElements(expected, viaCopy, label & " (CopyTo)")
                    Dim viaConvert As ManagedStorage = src.Clone(1, n).ConvertTo(dt)
                    For i As Integer = 0 To n - 1
                        Assert.AreEqual(expected.GetString(i), viaConvert.GetString(i), label & " (ConvertTo) element " & i)
                    Next
                    ' to and from .NET arrays of the destination's element type (HALF/BFLOAT16 arrays are raw bits)
                    If dt <> StorageType.HALF AndAlso dt <> StorageType.BFLOAT16 Then
                        Dim arr As Array = Array.CreateInstance(StorageTypes.ElementType(dt), n)
                        src.CopyToArray(1, arr, 0, n)
                        Dim wrapped As ManagedStorage = ManagedStorage.Wrap(arr)
                        Dim halfSource As Boolean = st = StorageType.HALF OrElse st = StorageType.BFLOAT16
                        For i As Integer = 0 To n - 1
                            If halfSource AndAlso dt = StorageType.UINT16 Then
                                ' documented: HALF/BFLOAT16 into a UShort() array copies the raw bit patterns
                                Assert.AreEqual(src.HalfBits(i + 1), wrapped.UShorts(i), label & " (CopyToArray raw bits) element " & i)
                            Else
                                Assert.AreEqual(expected.GetString(i), wrapped.GetString(i), label & " (CopyToArray) element " & i)
                            End If
                        Next
                        Dim back As ManagedStorage = ManagedStorage.Create(dt, n + 1)
                        back.CopyFrom(src.RawArray, 1, 0, n)
                        If st = StorageType.HALF OrElse st = StorageType.BFLOAT16 Then
                            ' a UShort() source is read as UINT16 values, not as half bit patterns
                            Continue For
                        End If
                        AssertSameElements(expected, back, label & " (CopyFrom)")
                    End If
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub StorageTypeFacts()
            Assert.IsTrue(StorageTypes.IsFloatingPoint(StorageType.HALF))
            Assert.IsFalse(StorageTypes.IsFloatingPoint(StorageType.LONG))
            Assert.IsTrue(StorageTypes.IsInteger(StorageType.UINT64))
            Assert.IsFalse(StorageTypes.IsInteger(StorageType.BOOL))
            Assert.IsTrue(StorageTypes.IsUnsigned(StorageType.UBYTE))
            Assert.IsFalse(StorageTypes.IsUnsigned(StorageType.BYTE))
            Assert.IsFalse(StorageTypes.IsNumeric(StorageType.UTF8))
            Assert.AreEqual(2, StorageTypes.ElementSize(StorageType.BFLOAT16))
            Assert.AreEqual(8, StorageTypes.ElementSize(StorageType.UINT64))
            Assert.AreEqual(1, StorageTypes.ElementSize(StorageType.BOOL))
            Assert.AreEqual(GetType(UShort), StorageTypes.ElementType(StorageType.HALF))
        End Sub

    End Class

End Namespace
