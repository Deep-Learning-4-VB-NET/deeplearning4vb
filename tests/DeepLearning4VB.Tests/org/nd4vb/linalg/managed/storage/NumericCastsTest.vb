Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.util
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>Java primitive-cast semantics and the half/bfloat16 conversions of the managed storages.</summary>
    <TestClass>
    Public Class NumericCastsTest

        <TestMethod>
        Public Sub DoubleToIntTruncatesSaturatesAndMapsNaNToZero()
            Assert.AreEqual(2, NumericCasts.DoubleToInt(2.9))
            Assert.AreEqual(-2, NumericCasts.DoubleToInt(-2.9))
            Assert.AreEqual(0, NumericCasts.DoubleToInt(-0.5))
            Assert.AreEqual(0, NumericCasts.DoubleToInt(Double.NaN))
            Assert.AreEqual(Integer.MaxValue, NumericCasts.DoubleToInt(1.0E+20))
            Assert.AreEqual(Integer.MinValue, NumericCasts.DoubleToInt(-1.0E+20))
            Assert.AreEqual(Integer.MaxValue, NumericCasts.DoubleToInt(Double.PositiveInfinity))
            Assert.AreEqual(Integer.MinValue, NumericCasts.DoubleToInt(Double.NegativeInfinity))
            Assert.AreEqual(Integer.MaxValue, NumericCasts.DoubleToInt(2147483647.9))
            Assert.AreEqual(Integer.MinValue, NumericCasts.DoubleToInt(-2147483648.9))
        End Sub

        <TestMethod>
        Public Sub DoubleToLongTruncatesSaturatesAndMapsNaNToZero()
            Assert.AreEqual(123456789012L, NumericCasts.DoubleToLong(123456789012.75))
            Assert.AreEqual(-7L, NumericCasts.DoubleToLong(-7.99))
            Assert.AreEqual(0L, NumericCasts.DoubleToLong(Double.NaN))
            Assert.AreEqual(Long.MaxValue, NumericCasts.DoubleToLong(1.0E+30))
            Assert.AreEqual(Long.MinValue, NumericCasts.DoubleToLong(-1.0E+30))
            Assert.AreEqual(Long.MaxValue, NumericCasts.DoubleToLong(9.2233720368547758E+18))
            Assert.AreEqual(Long.MinValue, NumericCasts.DoubleToLong(Double.NegativeInfinity))
        End Sub

        <TestMethod>
        Public Sub NarrowingCastsGoThroughIntAndWrap()
            ' Java: (short) 70000.7 == (short) 70000 == 4464; (byte) 300.2 == 44; (byte) -129.5 == 127
            Assert.AreEqual(CShort(4464), NumericCasts.DoubleToShort(70000.7))
            Assert.AreEqual(CSByte(44), NumericCasts.DoubleToSByte(300.2))
            Assert.AreEqual(CSByte(127), NumericCasts.DoubleToSByte(-129.5))
            ' (short) of a huge double saturates to Integer.MaxValue first: 0x7FFFFFFF -> 0xFFFF -> -1
            Assert.AreEqual(CShort(-1), NumericCasts.DoubleToShort(1.0E+20))
            Assert.AreEqual(CShort(0), NumericCasts.DoubleToShort(Double.NaN))
            Assert.AreEqual(-2147483648, NumericCasts.LongToInt(&H80000000L))
            Assert.AreEqual(1, NumericCasts.LongToInt(&H100000001L))
            Assert.AreEqual(CShort(-32768), NumericCasts.LongToShort(32768))
            Assert.AreEqual(CSByte(-1), NumericCasts.LongToSByte(255))
            Assert.AreEqual(CShort(-1), NumericCasts.IntToShort(65535))
            Assert.AreEqual(CSByte(-128), NumericCasts.IntToSByte(128))
        End Sub

        <TestMethod>
        Public Sub NegativeValuesWrapIntoUnsignedTypes()
            Assert.AreEqual(CByte(255), NumericCasts.DoubleToByte(-1.0))
            Assert.AreEqual(CByte(44), NumericCasts.DoubleToByte(300.0))
            Assert.AreEqual(CUShort(65535), NumericCasts.DoubleToUShort(-1.0))
            Assert.AreEqual(4294967295UI, NumericCasts.DoubleToUInteger(-1.0))
            Assert.AreEqual(ULong.MaxValue, NumericCasts.DoubleToULong(-1.0))
            Assert.AreEqual(0UL, NumericCasts.DoubleToULong(Double.NaN))
            ' UINT64 keeps the range from 2^63 to 2^64 and saturates above
            Assert.AreEqual(&H8000000000000000UL, NumericCasts.DoubleToULong(9.2233720368547758E+18))
            Assert.AreEqual(&HC000000000000000UL, NumericCasts.DoubleToULong(1.3835058055282164E+19))
            Assert.AreEqual(ULong.MaxValue, NumericCasts.DoubleToULong(1.0E+30))
        End Sub

        <TestMethod>
        Public Sub UnsignedLongConversions()
            Assert.AreEqual(-1L, NumericCasts.ULongToLong(ULong.MaxValue))
            Assert.AreEqual(ULong.MaxValue, NumericCasts.LongToULong(-1L))
            Assert.AreEqual(Long.MinValue, NumericCasts.ULongToLong(&H8000000000000000UL))
            Assert.AreEqual(1.8446744073709552E+19, NumericCasts.ULongToDouble(ULong.MaxValue), 0.0)
            Assert.AreEqual(9.2233720368547758E+18, NumericCasts.ULongToDouble(&H8000000000000000UL), 0.0)
            Assert.AreEqual(12345.0, NumericCasts.ULongToDouble(12345UL), 0.0)
            ' 2^63 + 1025 is just above the midpoint between 2^63 and 2^63 + 2048 and must round up
            Dim justAboveTie As ULong = &H8000000000000401UL
            Assert.AreEqual(9.2233720368547779E+18, NumericCasts.ULongToDouble(justAboveTie), 0.0)
            Assert.AreEqual(1.8446744E+19F, NumericCasts.ULongToSingle(ULong.MaxValue), 0.0F)
        End Sub

        <TestMethod>
        Public Sub HalfRoundTripsExactValues()
            Dim exact As Single() = {0.0F, 1.0F, -1.0F, 0.5F, 2.0F, 1024.0F, 65504.0F, -65504.0F, 0.333251953F, 6.10351563E-05F, 1.5F, -3.75F}
            For Each v As Single In exact
                Dim bits As UShort = NumericCasts.SingleToHalf(v)
                Assert.AreEqual(v, NumericCasts.HalfToSingle(bits), 0.0F, "value " & v)
                ' the encoder agrees with ArrayUtil.toHalf for normal values
                Assert.AreEqual(CInt(ArrayUtil.toHalf(v)) And &HFFFF, CInt(bits), "bits of " & v)
            Next
            Assert.AreEqual(CUShort(&H3C00), NumericCasts.SingleToHalf(1.0F))
            Assert.AreEqual(CUShort(&HC000), NumericCasts.SingleToHalf(-2.0F))
            Assert.AreEqual(CUShort(&H7BFF), NumericCasts.SingleToHalf(65504.0F))
        End Sub

        <TestMethod>
        Public Sub HalfSpecialValues()
            Assert.IsTrue(Single.IsPositiveInfinity(NumericCasts.HalfToSingle(NumericCasts.SingleToHalf(Single.PositiveInfinity))))
            Assert.IsTrue(Single.IsNegativeInfinity(NumericCasts.HalfToSingle(NumericCasts.SingleToHalf(Single.NegativeInfinity))))
            Assert.IsTrue(Single.IsNaN(NumericCasts.HalfToSingle(NumericCasts.SingleToHalf(Single.NaN))))
            ' ArrayUtil.toHalf saturates finite overflow at the largest half instead of infinity
            Assert.AreEqual(65504.0F, NumericCasts.HalfToSingle(NumericCasts.SingleToHalf(1.0E+6F)), 0.0F)
            ' the mantissa is truncated, not rounded: 1 + 2^-11 is between 1 and the next half
            Assert.AreEqual(1.0F, NumericCasts.HalfToSingle(NumericCasts.SingleToHalf(1.00048828F)), 0.0F)
            Assert.AreEqual(-1.0F, NumericCasts.HalfToSingle(NumericCasts.SingleToHalf(-1.00048828F)), 0.0F)
        End Sub

        <TestMethod>
        Public Sub HalfSubnormalsAreEncodedCorrectly()
            ' smallest subnormal half is 2^-24; 1E-5 lies in the subnormal range (ArrayUtil.toHalf garbles it)
            Dim tiny As Single = CSng(Math.Pow(2, -24))
            Assert.AreEqual(CUShort(1), NumericCasts.SingleToHalf(tiny))
            Assert.AreEqual(tiny, NumericCasts.HalfToSingle(1US), 0.0F)
            Dim v As Single = 1.0E-5F
            Dim back As Single = NumericCasts.HalfToSingle(NumericCasts.SingleToHalf(v))
            Assert.IsTrue(back <= v AndAlso v - back < tiny, "1E-5 round trip gave " & back)
            Dim neg As Single = NumericCasts.HalfToSingle(NumericCasts.SingleToHalf(-3.0E-5F))
            Assert.IsTrue(neg >= -3.0E-5F AndAlso neg - (-3.0E-5F) < tiny, "-3E-5 round trip gave " & neg)
            ' every subnormal pattern decodes to mantissa * 2^-24 and encodes back to itself
            For m As Integer = 1 To &H3FF
                Dim f As Single = NumericCasts.HalfToSingle(CUShort(m))
                Assert.AreEqual(CSng(m) * tiny, f, 0.0F)
                Assert.AreEqual(CUShort(m), NumericCasts.SingleToHalf(f), "subnormal " & m)
                Assert.AreEqual(CUShort(m Or &H8000), NumericCasts.SingleToHalf(-f), "negative subnormal " & m)
            Next
        End Sub

        <TestMethod>
        Public Sub EveryFiniteHalfPatternRoundTrips()
            For bits As Integer = 0 To &HFFFF
                Dim exponent As Integer = (bits >> 10) And &H1F
                If exponent = &H1F Then
                    Continue For
                End If
                If bits = &H8000 Then
                    Continue For ' -0 encodes as +0 through ArrayUtil.toHalf
                End If
                Dim f As Single = NumericCasts.HalfToSingle(CUShort(bits))
                Assert.AreEqual(CUShort(bits), NumericCasts.SingleToHalf(f), "pattern " & bits.ToString("X4"))
            Next
        End Sub

        <TestMethod>
        Public Sub BFloat16RoundTrips()
            Dim exact As Single() = {0.0F, 1.0F, -2.0F, 0.5F, 3.0F, 1.0E+30F, -1.0E-30F, 256.0F}
            For Each v As Single In exact
                Dim bits As UShort = NumericCasts.SingleToBFloat16(v)
                Assert.AreEqual(v, NumericCasts.BFloat16ToSingle(bits), Math.Abs(v) * 0.01F, "value " & v)
                Assert.AreEqual(CInt(ArrayUtil.toBFloat16(v)) And &HFFFF, CInt(bits))
            Next
            Assert.AreEqual(CUShort(&H3F80), NumericCasts.SingleToBFloat16(1.0F))
            Assert.AreEqual(1.0F, NumericCasts.BFloat16ToSingle(&H3F80US), 0.0F)
            ' truncation: 1.00390625 (1 + 2^-8) is below the next bfloat16 above 1 (1 + 2^-7)
            Assert.AreEqual(1.0F, NumericCasts.BFloat16ToSingle(NumericCasts.SingleToBFloat16(1.00390625F)), 0.0F)
            Assert.IsTrue(Single.IsNaN(NumericCasts.BFloat16ToSingle(NumericCasts.SingleToBFloat16(Single.NaN))))
            Assert.IsTrue(Single.IsPositiveInfinity(NumericCasts.BFloat16ToSingle(NumericCasts.SingleToBFloat16(Single.PositiveInfinity))))
            For bits As Integer = 0 To &HFFFF
                If ((bits >> 7) And &HFF) = &HFF Then
                    Continue For
                End If
                Dim f As Single = NumericCasts.BFloat16ToSingle(CUShort(bits))
                Assert.AreEqual(CUShort(bits), NumericCasts.SingleToBFloat16(f))
            Next
        End Sub

        <TestMethod>
        Public Sub SingleBitsHelpers()
            Assert.AreEqual(&H3F800000, NumericCasts.SingleToBits(1.0F))
            Assert.AreEqual(-2.0F, NumericCasts.BitsToSingle(&HC0000000), 0.0F)
        End Sub

    End Class

End Namespace
