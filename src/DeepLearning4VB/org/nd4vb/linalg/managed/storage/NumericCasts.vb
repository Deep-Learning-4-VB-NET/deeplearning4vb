' DeepLearning4VB: Java primitive-cast semantics and half/bfloat16 bit conversions for the managed storages.

Imports System
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports ArrayUtil = DeepLearning4VB.org.nd4vb.common.util.ArrayUtil
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.storage

    ''' <summary>
    ''' The value conversions the storages use, following Java's primitive casts: a floating-point value
    ''' converts to an integer by truncation toward zero, saturating at the target's range like Java's
    ''' <c>(int)</c>/<c>(long)</c> casts (NaN becomes 0); <c>(short)</c>/<c>(byte)</c> casts go through
    ''' <c>(int)</c> and then wrap; integer narrowing wraps (two's complement); booleans are 0 and 1.
    ''' HALF and BFLOAT16 values are 16-bit patterns converted with <c>ArrayUtil.toHalf</c> and
    ''' <c>ArrayUtil.toBFloat16</c> (both truncate the mantissa) and their exact inverses written here.
    ''' </summary>
    ''' <remarks>
    ''' Deliberate differences from the Java/ND4J originals:
    ''' <list type="bullet">
    ''' <item><description><c>ArrayUtil.toHalf</c> mis-encodes magnitudes between 5.96E-8 and 2^-14 (the
    ''' half-precision subnormal range: its exponent arithmetic underflows into the exponent field, so
    ''' 1E-5 becomes about 32768). <see cref="SingleToHalf"/> encodes that range itself, truncating the
    ''' mantissa like <c>toHalf</c> does for normal values; every other value goes through <c>toHalf</c>.</description></item>
    ''' <item><description>Unsigned 64-bit values read back unsigned (ND4J's Java <c>getDouble</c> reads UINT64
    ''' through a signed <c>LongIndexer</c>), and <see cref="DoubleToULong"/> keeps values from 2^63 to
    ''' 2^64 instead of saturating them at <c>Long.MAX_VALUE</c>.</description></item>
    ''' </list>
    ''' </remarks>
    Public NotInheritable Class NumericCasts

        Private Sub New()
        End Sub

        ' 2^63 and 2^31 as doubles (exact).
        Private Const TwoPow63 As Double = 9.2233720368547758E+18
        Private Const TwoPow64 As Double = 1.8446744073709552E+19
        Private Const TwoPow31 As Double = 2147483648.0
        ' Smallest normal half (2^-14) and the smallest magnitude ArrayUtil.toHalf encodes itself.
        Private Const HalfMinNormal As Single = 6.10351563E-05F
        Private Const ToHalfTinyThreshold As Single = 5.96046E-08F

        <StructLayout(LayoutKind.Explicit)>
        Private Structure FloatBits
            <FieldOffset(0)> Public F As Single
            <FieldOffset(0)> Public I As Integer
        End Structure

        ''' <summary>Java's <c>(long) value</c>: truncates toward zero, saturates, NaN gives 0.</summary>
        Public Shared Function DoubleToLong(ByVal value As Double) As Long
            If Double.IsNaN(value) Then
                Return 0L
            End If
            If value >= TwoPow63 Then
                Return Long.MaxValue
            End If
            If value <= -TwoPow63 Then
                Return Long.MinValue
            End If
            Return CLng(SysMath.Truncate(value))
        End Function

        ''' <summary>Java's <c>(int) value</c>: truncates toward zero, saturates, NaN gives 0.</summary>
        Public Shared Function DoubleToInt(ByVal value As Double) As Integer
            If Double.IsNaN(value) Then
                Return 0
            End If
            If value >= TwoPow31 Then
                Return Integer.MaxValue
            End If
            If value <= -TwoPow31 Then
                Return Integer.MinValue
            End If
            Return CInt(SysMath.Truncate(value))
        End Function

        ''' <summary>Java's <c>(short) value</c>: <c>(int)</c> first, then the low 16 bits.</summary>
        Public Shared Function DoubleToShort(ByVal value As Double) As Short
            Return IntToShort(DoubleToInt(value))
        End Function

        ''' <summary>Java's <c>(byte) value</c>: <c>(int)</c> first, then the low 8 bits.</summary>
        Public Shared Function DoubleToSByte(ByVal value As Double) As SByte
            Return IntToSByte(DoubleToInt(value))
        End Function

        ''' <summary>UINT8 store as ND4J does it: <c>(int)</c> first, then the low 8 bits (so -1 becomes 255).</summary>
        Public Shared Function DoubleToByte(ByVal value As Double) As Byte
            Return CByte(DoubleToInt(value) And &HFF)
        End Function

        ''' <summary>UINT16 store as ND4J does it: <c>(int)</c> first, then the low 16 bits.</summary>
        Public Shared Function DoubleToUShort(ByVal value As Double) As UShort
            Return CUShort(DoubleToInt(value) And &HFFFF)
        End Function

        ''' <summary>UINT32 store as ND4J does it: <c>(long)</c> first, then the low 32 bits.</summary>
        Public Shared Function DoubleToUInteger(ByVal value As Double) As UInteger
            Return CUInt(DoubleToLong(value) And &HFFFFFFFFL)
        End Function

        ''' <summary>
        ''' UINT64 store: values in [2^63, 2^64) convert exactly (2^64 and above saturate at
        ''' <c>ULong.MaxValue</c>); everything else is Java's <c>(long)</c> cast reinterpreted as unsigned,
        ''' so -1 becomes 2^64 - 1.
        ''' </summary>
        Public Shared Function DoubleToULong(ByVal value As Double) As ULong
            If value >= TwoPow63 Then
                If value >= TwoPow64 Then
                    Return ULong.MaxValue
                End If
                ' value - 2^63 is exact here (Sterbenz), so no rounding and no unsigned conversion helper
                Return CULng(CLng(value - TwoPow63)) Or &H8000000000000000UL
            End If
            Return LongToULong(DoubleToLong(value))
        End Function

        ''' <summary>Java's <c>(int) value</c> for a long: the low 32 bits.</summary>
        Public Shared Function LongToInt(ByVal value As Long) As Integer
            Return CInt(If((value And &H80000000L) <> 0, (value And &HFFFFFFFFL) - &H100000000L, value And &HFFFFFFFFL))
        End Function

        ''' <summary>Java's <c>(short) value</c> for a long: the low 16 bits.</summary>
        Public Shared Function LongToShort(ByVal value As Long) As Short
            Return CShort(If((value And &H8000L) <> 0, (value And &HFFFFL) - &H10000L, value And &HFFFFL))
        End Function

        ''' <summary>Java's <c>(byte) value</c> for a long: the low 8 bits.</summary>
        Public Shared Function LongToSByte(ByVal value As Long) As SByte
            Return CSByte(If((value And &H80L) <> 0, (value And &HFFL) - &H100L, value And &HFFL))
        End Function

        ''' <summary>The low 16 bits of an int as a signed short (Java's <c>(short)</c>).</summary>
        Public Shared Function IntToShort(ByVal value As Integer) As Short
            Return CShort(If((value And &H8000) <> 0, (value And &HFFFF) - &H10000, value And &HFFFF))
        End Function

        ''' <summary>The low 8 bits of an int as a signed byte (Java's <c>(byte)</c>).</summary>
        Public Shared Function IntToSByte(ByVal value As Integer) As SByte
            Return CSByte(If((value And &H80) <> 0, (value And &HFF) - &H100, value And &HFF))
        End Function

        ''' <summary>The bits of a long reinterpreted as unsigned.</summary>
        Public Shared Function LongToULong(ByVal value As Long) As ULong
            If value >= 0 Then
                Return CULng(value)
            End If
            Return CULng(value And Long.MaxValue) Or &H8000000000000000UL
        End Function

        ''' <summary>The bits of an unsigned long reinterpreted as signed (Java's view of a UINT64 element).</summary>
        Public Shared Function ULongToLong(ByVal value As ULong) As Long
            If value <= CULng(Long.MaxValue) Then
                Return CLng(value)
            End If
            Return CLng(value And CULng(Long.MaxValue)) Or Long.MinValue
        End Function

        ''' <summary>The numeric value of an unsigned long as a double.</summary>
        Public Shared Function ULongToDouble(ByVal value As ULong) As Double
            If value <= CULng(Long.MaxValue) Then
                Return CDbl(CLng(value))
            End If
            ' halve with a sticky low bit so the single rounding to 53 bits stays correct on every runtime
            Dim halved As Long = CLng(value >> 1) Or CLng(value And 1UL)
            Return CDbl(halved) * 2.0
        End Function

        ''' <summary>The numeric value of an unsigned long as a single (rounded once).</summary>
        Public Shared Function ULongToSingle(ByVal value As ULong) As Single
            If value <= CULng(Long.MaxValue) Then
                Return CSng(CLng(value))
            End If
            ' keep a sticky bit so that halving does not change the rounding
            Dim halved As Long = CLng(value >> 1) Or CLng(value And 1UL)
            Return CSng(halved) * 2.0F
        End Function

        ''' <summary>Converts a single to a half-precision bit pattern (see the class remarks).</summary>
        Public Shared Function SingleToHalf(ByVal value As Single) As UShort
            Dim magnitude As Single = SysMath.Abs(value)
            If magnitude >= ToHalfTinyThreshold AndAlso magnitude < HalfMinNormal Then
                ' subnormal half: mantissa = |value| / 2^-24, truncated (exact multiplication by 2^24)
                Dim mantissa As Integer = CInt(SysMath.Truncate(magnitude * 16777216.0F))
                Dim sign As Integer = If(value < 0, &H8000, 0)
                Return CUShort(sign Or mantissa)
            End If
            Return CUShort(CInt(ArrayUtil.toHalf(value)) And &HFFFF)
        End Function

        ''' <summary>Converts a half-precision bit pattern to a single (exact, including subnormals, infinities and NaN).</summary>
        Public Shared Function HalfToSingle(ByVal bits As UShort) As Single
            Dim h As Integer = bits
            Dim sign As Integer = (h And &H8000) << 16
            Dim exponent As Integer = (h >> 10) And &H1F
            Dim mantissa As Integer = h And &H3FF
            Dim fb As New FloatBits()
            If exponent = 0 Then
                If mantissa = 0 Then
                    fb.I = sign
                    Return fb.F
                End If
                Dim v As Single = CSng(mantissa) / 16777216.0F
                Return If(sign <> 0, -v, v)
            End If
            If exponent = 31 Then
                fb.I = sign Or &H7F800000 Or (mantissa << 13)
            Else
                fb.I = sign Or ((exponent + 112) << 23) Or (mantissa << 13)
            End If
            Return fb.F
        End Function

        ''' <summary>Converts a single to a bfloat16 bit pattern with <c>ArrayUtil.toBFloat16</c> (truncation).</summary>
        Public Shared Function SingleToBFloat16(ByVal value As Single) As UShort
            Return CUShort(CInt(ArrayUtil.toBFloat16(value)) And &HFFFF)
        End Function

        ''' <summary>Converts a bfloat16 bit pattern to a single (exact).</summary>
        Public Shared Function BFloat16ToSingle(ByVal bits As UShort) As Single
            Dim fb As New FloatBits()
            fb.I = CInt(bits) << 16
            Return fb.F
        End Function

        ''' <summary>The IEEE bit pattern of a single (Java's <c>Float.floatToRawIntBits</c>).</summary>
        Public Shared Function SingleToBits(ByVal value As Single) As Integer
            Dim fb As New FloatBits()
            fb.F = value
            Return fb.I
        End Function

        ''' <summary>The single with the given IEEE bit pattern (Java's <c>Float.intBitsToFloat</c>).</summary>
        Public Shared Function BitsToSingle(ByVal bits As Integer) As Single
            Dim fb As New FloatBits()
            fb.I = bits
            Return fb.F
        End Function

        ''' <summary>Formats a double the way the storages print floating-point values (round-trip, invariant culture).</summary>
        Friend Shared Function FormatDouble(ByVal value As Double) As String
            Return value.ToString("R", CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Parses a number written in invariant culture ("NaN", "Infinity" and "true"/"false" included).</summary>
        Friend Shared Function ParseDouble(ByVal text As String) As Double
            If text Is Nothing Then
                Throw New ArgumentNullException(NameOf(text))
            End If
            Dim t As String = text.Trim()
            If String.Equals(t, "true", StringComparison.OrdinalIgnoreCase) Then
                Return 1.0
            End If
            If String.Equals(t, "false", StringComparison.OrdinalIgnoreCase) Then
                Return 0.0
            End If
            Return Double.Parse(t, NumberStyles.Float, CultureInfo.InvariantCulture)
        End Function

    End Class

End Namespace
