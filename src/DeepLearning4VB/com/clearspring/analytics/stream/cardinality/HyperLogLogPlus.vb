Imports System
Imports System.Text
Imports DeepLearning4VB.JavaCompat

' DeepLearning4VB shim for the part of stream-lib's HyperLogLog++ API that DataVec uses
' (com.clearspring.analytics.stream.cardinality.HyperLogLogPlus: offer, offerHashed, cardinality,
' addAll, merge). The algorithm is from Heule, Nunkesser and Hall, "HyperLogLog in Practice" (2013);
' stream-lib is Apache-2.0 (https://github.com/addthis/stream-lib). This is an independent managed
' implementation of the dense ("normal") register representation only, not a translation of that
' library. Objects are hashed like stream-lib does (MurmurHash64A of the UTF-8 bytes of the string
' form, or of the byte array), so register collisions match the Java original for ints and strings.

Namespace com.clearspring.analytics.stream.cardinality

    ''' <summary>
    ''' An approximate distinct-value counter with <c>2^p</c> registers. The relative accuracy is about
    ''' <c>1.04 / sqrt(2^p)</c>. Differences from stream-lib: the sparse representation (<c>sp</c>) is not
    ''' implemented (the value is accepted and ignored), and the empirical bias-correction tables of
    ''' HyperLogLog++ are not included, so estimates between the linear-counting threshold and
    ''' <c>5 * 2^p</c> carry the raw HyperLogLog bias of a few percent. Small cardinalities use linear
    ''' counting exactly as stream-lib does.
    ''' </summary>
    <Serializable>
    Public Class HyperLogLogPlus

        Private Shared ReadOnly THRESHOLD_DATA() As Double = {10, 20, 40, 80, 220, 400, 900, 1800, 3100, 6500, 11500, 20000, 50000, 120000, 350000}

        Private ReadOnly pField As Integer
        Private ReadOnly spField As Integer
        Private ReadOnly m As Integer
        Private ReadOnly registers() As Byte
        Private ReadOnly alphaMM As Double

        ''' <summary>A dense estimator with <c>2^p</c> registers.</summary>
        Public Sub New(ByVal p As Integer)
            Me.New(p, 0)
        End Sub

        ''' <summary>
        ''' A dense estimator with <c>2^p</c> registers. <paramref name="sp"/> (the sparse-representation precision of
        ''' stream-lib) is validated like the original but otherwise ignored.
        ''' </summary>
        Public Sub New(ByVal p As Integer, ByVal sp As Integer)
            If p < 4 OrElse p > 18 Then
                Throw New ArgumentException("p must be between 4 and 18 (inclusive)")
            End If
            If sp > 32 Then
                Throw New ArgumentException("sp values greater than 32 not supported")
            End If
            If sp <> 0 AndAlso p > sp Then
                Throw New ArgumentException("p must be less than or equal to sp")
            End If
            pField = p
            spField = sp
            m = 1 << p
            registers = New Byte(m - 1) {}
            alphaMM = getAlphaMM(p, m)
        End Sub

        Private Shared Function getAlphaMM(ByVal p As Integer, ByVal m As Integer) As Double
            Select Case p
                Case 4
                    Return 0.673 * m * m
                Case 5
                    Return 0.697 * m * m
                Case 6
                    Return 0.709 * m * m
                Case Else
                    Return (0.7213 / (1 + 1.079 / m)) * m * m
            End Select
        End Function

        ''' <summary>The precision <c>p</c> (number of index bits).</summary>
        Public ReadOnly Property Precision As Integer
            Get
                Return pField
            End Get
        End Property

        ''' <summary>Offers a value; returns true if a register changed.</summary>
        Public Function offer(ByVal o As Object) As Boolean
            Return offerHashed(MurmurHash.hash64(o))
        End Function

        ''' <summary>Offers an already hashed (64 bit) value; returns true if a register changed.</summary>
        Public Function offerHashed(ByVal hashedLong As Long) As Boolean
            ' the first p bits select the register, the run of leading zeros of the rest is the value
            Dim idx As Integer = CInt(urshift(hashedLong, 64 - pField))
            Dim r As Integer = numberOfLeadingZeros((hashedLong << pField) Or (1L << (pField - 1))) + 1
            If registers(idx) < r Then
                registers(idx) = CByte(r)
                Return True
            End If
            Return False
        End Function

        ''' <summary>Offers an already hashed (32 bit) value, widened like stream-lib does.</summary>
        Public Function offerHashed(ByVal hashedInt As Integer) As Boolean
            Return offerHashed(CLng(hashedInt) And &HFFFFFFFFL)
        End Function

        ''' <summary>The estimated number of distinct values offered.</summary>
        Public Function cardinality() As Long
            Dim registerSum As Double = 0
            Dim zeros As Double = 0
            For j As Integer = 0 To m - 1
                Dim val As Integer = registers(j)
                registerSum += 1.0 / Global.System.Math.Pow(2.0, val)
                If val = 0 Then
                    zeros += 1
                End If
            Next j
            Dim estimate As Double = alphaMM * (1 / registerSum)
            Dim estimatePrime As Double = estimate ' no bias-correction tables, see the class comment
            Dim h As Double
            If zeros > 0 Then
                h = linearCounting(m, zeros)
            Else
                h = estimatePrime
            End If
            If h <= getThreshold(pField) Then
                Return javaRound(h)
            Else
                Return javaRound(estimatePrime)
            End If
        End Function

        ''' <summary>The number of registers.</summary>
        Public Function sizeof() As Integer
            Return m
        End Function

        ''' <summary>Adds the registers of another estimator of the same precision into this one.</summary>
        Public Sub addAll(ByVal other As HyperLogLogPlus)
            If other Is Nothing Then
                Throw New ArgumentNullException("other")
            End If
            If other.pField <> pField Then
                Throw New CardinalityMergeException("Cannot merge estimators of different sizes")
            End If
            For j As Integer = 0 To m - 1
                If other.registers(j) > registers(j) Then
                    registers(j) = other.registers(j)
                End If
            Next j
        End Sub

        ''' <summary>A new estimator holding the union of this one and the given ones (none of them is modified).</summary>
        Public Function merge(ByVal ParamArray estimators() As HyperLogLogPlus) As HyperLogLogPlus
            Dim result As New HyperLogLogPlus(pField, spField)
            result.addAll(Me)
            If estimators IsNot Nothing Then
                For Each e As HyperLogLogPlus In estimators
                    result.addAll(e)
                Next e
            End If
            Return result
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other = TryCast(obj, HyperLogLogPlus)
            If other Is Nothing Then Return False
            If other.pField <> pField Then Return False
            For j As Integer = 0 To m - 1
                If registers(j) <> other.registers(j) Then Return False
            Next j
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = pField
            For j As Integer = 0 To m - 1
                h = 31 * h + registers(j)
            Next j
            Return h
        End Function

        Private Shared Function linearCounting(ByVal m As Integer, ByVal v As Double) As Double
            Return m * Global.System.Math.Log(m / v)
        End Function

        Private Shared Function getThreshold(ByVal p As Integer) As Double
            Return THRESHOLD_DATA(p - 4)
        End Function

        ''' <summary>Java <c>Math.round(double)</c>: floor(x + 0.5).</summary>
        Private Shared Function javaRound(ByVal d As Double) As Long
            Return CLng(Global.System.Math.Floor(d + 0.5))
        End Function

        ''' <summary>Java <c>Long.numberOfLeadingZeros</c>.</summary>
        Friend Shared Function numberOfLeadingZeros(ByVal v As Long) As Integer
            If v = 0 Then Return 64
            Dim n As Integer = 0
            Dim mask As Long = Long.MinValue ' only the top bit set
            Do While (v And mask) = 0
                n += 1
                mask = urshift(mask, 1)
            Loop
            Return n
        End Function

        ''' <summary>Java <c>&gt;&gt;&gt;</c> (logical shift right) for a signed long.</summary>
        Friend Shared Function urshift(ByVal v As Long, ByVal n As Integer) As Long
            If n <= 0 Then Return v
            If n >= 64 Then Return 0
            Return (v >> n) And (Not (-1L << (64 - n)))
        End Function

    End Class

    ''' <summary>
    ''' stream-lib's <c>MurmurHash.hash64</c> (MurmurHash64A, seed 0xe17a1465) over the UTF-8 bytes of a string,
    ''' a byte array, or the Java string form of any other object.
    ''' </summary>
    Friend Module MurmurHash

        Private Const DEFAULT_SEED As Integer = &HE17A1465

        Public Function hash64(ByVal o As Object) As Long
            If o Is Nothing Then Return 0L
            Dim bytes() As Byte
            If TypeOf o Is String Then
                bytes = Encoding.UTF8.GetBytes(DirectCast(o, String))
            ElseIf TypeOf o Is Byte() Then
                bytes = DirectCast(o, Byte())
            Else
                bytes = Encoding.UTF8.GetBytes(javaToString(o))
            End If
            Return hash64(bytes, bytes.Length)
        End Function

        ''' <summary>Java <c>Object.toString()</c> for the boxed values whose .NET string form differs.</summary>
        Private Function javaToString(ByVal o As Object) As String
            If TypeOf o Is Double Then Return Arrays.JavaDoubleToString(CDbl(o))
            If TypeOf o Is Single Then Return Arrays.JavaFloatToString(CSng(o))
            If TypeOf o Is Boolean Then Return If(CBool(o), "true", "false")
            Return o.ToString()
        End Function

        Public Function hash64(ByVal data() As Byte, ByVal length As Integer) As Long
            Return hash64(data, length, DEFAULT_SEED)
        End Function

        Public Function hash64(ByVal data() As Byte, ByVal length As Integer, ByVal seed As Integer) As Long
            Const m As Long = &HC6A4A7935BD1E995
            Const r As Integer = 47

            Dim h As Long = (CLng(seed) And &HFFFFFFFFL) Xor (CLng(length) * m)

            Dim length8 As Integer = length \ 8
            For i As Integer = 0 To length8 - 1
                Dim i8 As Integer = i * 8
                Dim k As Long = (CLng(data(i8)) And &HFFL) + ((CLng(data(i8 + 1)) And &HFFL) << 8) + ((CLng(data(i8 + 2)) And &HFFL) << 16) + ((CLng(data(i8 + 3)) And &HFFL) << 24) + ((CLng(data(i8 + 4)) And &HFFL) << 32) + ((CLng(data(i8 + 5)) And &HFFL) << 40) + ((CLng(data(i8 + 6)) And &HFFL) << 48) + ((CLng(data(i8 + 7)) And &HFFL) << 56)
                k *= m
                k = k Xor HyperLogLogPlus.urshift(k, r)
                k *= m
                h = h Xor k
                h *= m
            Next i

            Dim tail As Integer = length Mod 8
            Dim base As Integer = length And (Not 7)
            If tail >= 7 Then h = h Xor ((CLng(data(base + 6)) And &HFFL) << 48)
            If tail >= 6 Then h = h Xor ((CLng(data(base + 5)) And &HFFL) << 40)
            If tail >= 5 Then h = h Xor ((CLng(data(base + 4)) And &HFFL) << 32)
            If tail >= 4 Then h = h Xor ((CLng(data(base + 3)) And &HFFL) << 24)
            If tail >= 3 Then h = h Xor ((CLng(data(base + 2)) And &HFFL) << 16)
            If tail >= 2 Then h = h Xor ((CLng(data(base + 1)) And &HFFL) << 8)
            If tail >= 1 Then
                h = h Xor (CLng(data(base)) And &HFFL)
                h *= m
            End If

            h = h Xor HyperLogLogPlus.urshift(h, r)
            h *= m
            h = h Xor HyperLogLogPlus.urshift(h, r)
            Return h
        End Function

    End Module

End Namespace
