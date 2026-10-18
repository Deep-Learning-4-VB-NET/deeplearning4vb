' DeepLearning4VB: the managed random number generator of the ND4VB engine.

Imports System

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' A seedable 64-bit pseudo-random generator: xoshiro256** (Blackman and Vigna, 2018) with its
    ''' 256-bit state expanded from the 64-bit seed by SplitMix64 (Steele, Lea and Flood, 2014).
    ''' Sequences are deterministic per seed and identical on every platform and framework, but they are
    ''' not the sequences of libnd4j's generator or of <c>java.util.Random</c>.
    ''' </summary>
    ''' <remarks>
    ''' <para>Doubles use the top 53 bits of an output ([0, 1)), floats the top 24 bits; <see cref="nextInt(Integer)"/>
    ''' is unbiased (Lemire's multiply-and-reject); <see cref="nextGaussian"/> uses the Marsaglia polar
    ''' method and caches the second value of each pair, like <c>java.util.Random</c>.</para>
    ''' <para>Instances are not thread-safe; use one generator per thread or synchronize.</para>
    ''' </remarks>
    Public NotInheritable Class ManagedRandomGenerator

        Private _seed As Long
        Private _s0 As ULong
        Private _s1 As ULong
        Private _s2 As ULong
        Private _s3 As ULong
        Private _position As Long
        Private _haveNextGaussian As Boolean
        Private _nextGaussian As Double

        ''' <summary>A generator seeded from the clock and a process-wide counter.</summary>
        Public Sub New()
            Me.New(DefaultSeed())
        End Sub

        ''' <summary>A generator with the given seed.</summary>
        Public Sub New(ByVal seed As Long)
            setSeed(seed)
        End Sub

        Private Shared _seedCounter As Long = 0

        Private Shared Function DefaultSeed() As Long
            Dim n As Long = Global.System.Threading.Interlocked.Increment(_seedCounter)
            Return DateTime.UtcNow.Ticks Xor (n * &H5851F42D4C957F2DL) Xor Environment.TickCount
        End Function

        ''' <summary>Restarts the sequence for <paramref name="seed"/>.</summary>
        Public Sub setSeed(ByVal seed As Long)
            _seed = seed
            Dim x As ULong = ToULong(seed)
            _s0 = SplitMix64(x)
            _s1 = SplitMix64(x)
            _s2 = SplitMix64(x)
            _s3 = SplitMix64(x)
            _position = 0
            _haveNextGaussian = False
        End Sub

        ''' <summary>Restarts the sequence for an int seed (sign-extended to 64 bits).</summary>
        Public Sub setSeed(ByVal seed As Integer)
            setSeed(CLng(seed))
        End Sub

        ''' <summary>The seed last set.</summary>
        Public Function getSeed() As Long
            Return _seed
        End Function

        ''' <summary>Number of 64-bit outputs drawn since the seed was set.</summary>
        Public Function getPosition() As Long
            Return _position
        End Function

        ''' <summary>The next 64 random bits (xoshiro256** step).</summary>
        Public Function nextLong() As Long
            Return ToLong(NextBits())
        End Function

        ''' <summary>A uniformly distributed int (the upper 32 bits of a 64-bit output).</summary>
        Public Function nextInt() As Integer
            Dim u As Long = ToLong(NextBits() >> 32)
            Return CInt(If(u >= &H80000000L, u - &H100000000L, u))
        End Function

        ''' <summary>A uniformly distributed int in [0, bound).</summary>
        ''' <exception cref="ArgumentException"><paramref name="bound"/> is not positive.</exception>
        Public Function nextInt(ByVal bound As Integer) As Integer
            If bound <= 0 Then
                Throw New ArgumentException("bound must be positive, was " & bound, NameOf(bound))
            End If
            Dim ub As ULong = CULng(bound)
            Dim m As ULong = (NextBits() >> 32) * ub
            Dim low As ULong = m And &HFFFFFFFFUL
            If low < ub Then
                ' reject the values that would bias the result: (2^32 - bound) mod bound
                Dim threshold As ULong = ((&H100000000UL - ub) Mod ub)
                Do While low < threshold
                    m = (NextBits() >> 32) * ub
                    low = m And &HFFFFFFFFUL
                Loop
            End If
            Return CInt(m >> 32)
        End Function

        ''' <summary>A uniformly distributed int in [origin, bound).</summary>
        ''' <exception cref="ArgumentException"><paramref name="origin"/> is not below <paramref name="bound"/>.</exception>
        Public Function nextInt(ByVal origin As Integer, ByVal bound As Integer) As Integer
            If origin >= bound Then
                Throw New ArgumentException("origin must be below bound", NameOf(origin))
            End If
            Dim range As Long = CLng(bound) - origin
            If range <= Integer.MaxValue Then
                Return origin + nextInt(CInt(range))
            End If
            ' range above 2^31: rejection on 32-bit draws
            Do
                Dim r As Long = CLng(ToLong(NextBits() >> 32))
                If r < range Then
                    Return CInt(origin + r)
                End If
            Loop
        End Function

        ''' <summary>A random boolean (the top bit of an output).</summary>
        Public Function nextBoolean() As Boolean
            Return (NextBits() >> 63) <> 0UL
        End Function

        ''' <summary>A uniformly distributed double in [0, 1) with 53 random bits.</summary>
        Public Function nextDouble() As Double
            Return CDbl(ToLong(NextBits() >> 11)) * (1.0 / 9007199254740992.0)
        End Function

        ''' <summary>A uniformly distributed float in [0, 1) with 24 random bits.</summary>
        Public Function nextFloat() As Single
            Return CSng(ToLong(NextBits() >> 40)) * (1.0F / 16777216.0F)
        End Function

        ''' <summary>A standard normal value (mean 0, standard deviation 1), by the Marsaglia polar method.</summary>
        Public Function nextGaussian() As Double
            If _haveNextGaussian Then
                _haveNextGaussian = False
                Return _nextGaussian
            End If
            Dim v1 As Double
            Dim v2 As Double
            Dim s As Double
            Do
                v1 = 2.0 * nextDouble() - 1.0
                v2 = 2.0 * nextDouble() - 1.0
                s = v1 * v1 + v2 * v2
            Loop While s >= 1.0 OrElse s = 0.0
            Dim multiplier As Double = Math.Sqrt(-2.0 * Math.Log(s) / s)
            _nextGaussian = v2 * multiplier
            _haveNextGaussian = True
            Return v1 * multiplier
        End Function

        ''' <summary>Fills <paramref name="bytes"/> with random bytes (eight per 64-bit output, low byte first).</summary>
        Public Sub nextBytes(ByVal bytes As Byte())
            If bytes Is Nothing Then
                Throw New ArgumentNullException(NameOf(bytes))
            End If
            Dim i As Integer = 0
            Do While i < bytes.Length
                Dim r As ULong = NextBits()
                For k As Integer = 0 To 7
                    If i >= bytes.Length Then
                        Exit For
                    End If
                    bytes(i) = CByte(r And &HFFUL)
                    r >>= 8
                    i += 1
                Next
            Loop
        End Sub

        ''' <summary>
        ''' Advances the state by 2^128 outputs (xoshiro256**'s jump function), giving a stream that does
        ''' not overlap this one for practical purposes; useful to derive per-thread generators.
        ''' </summary>
        Public Sub jump()
            Dim jumpTable As ULong() = {&H180EC6D33CFD0ABAUL, &HD5A61266F0C9392CUL, &HA9582618E03FC9AAUL, &H39ABDC4529B1661CUL}
            Dim t0 As ULong = 0, t1 As ULong = 0, t2 As ULong = 0, t3 As ULong = 0
            For Each word As ULong In jumpTable
                For b As Integer = 0 To 63
                    If ((word >> b) And 1UL) <> 0UL Then
                        t0 = t0 Xor _s0
                        t1 = t1 Xor _s1
                        t2 = t2 Xor _s2
                        t3 = t3 Xor _s3
                    End If
                    StepState()
                Next
            Next
            _s0 = t0
            _s1 = t1
            _s2 = t2
            _s3 = t3
            _haveNextGaussian = False
        End Sub

        ''' <summary>The next raw 64-bit output.</summary>
        Private Function NextBits() As ULong
            Dim result As ULong = RotateLeft(_s1 * 5UL, 7) * 9UL
            StepState()
            _position += 1
            Return result
        End Function

        Private Sub StepState()
            Dim t As ULong = _s1 << 17
            _s2 = _s2 Xor _s0
            _s3 = _s3 Xor _s1
            _s1 = _s1 Xor _s2
            _s0 = _s0 Xor _s3
            _s2 = _s2 Xor t
            _s3 = RotateLeft(_s3, 45)
        End Sub

        Private Shared Function RotateLeft(ByVal x As ULong, ByVal k As Integer) As ULong
            Return (x << k) Or (x >> (64 - k))
        End Function

        ''' <summary>One SplitMix64 step: advances <paramref name="state"/> and returns the mixed output.</summary>
        Private Shared Function SplitMix64(ByRef state As ULong) As ULong
            state += &H9E3779B97F4A7C15UL
            Dim z As ULong = state
            z = (z Xor (z >> 30)) * &HBF58476D1CE4E5B9UL
            z = (z Xor (z >> 27)) * &H94D049BB133111EBUL
            Return z Xor (z >> 31)
        End Function

        Private Shared Function ToULong(ByVal value As Long) As ULong
            If value >= 0 Then
                Return CULng(value)
            End If
            Return CULng(value And Long.MaxValue) Or &H8000000000000000UL
        End Function

        Private Shared Function ToLong(ByVal value As ULong) As Long
            If value <= CULng(Long.MaxValue) Then
                Return CLng(value)
            End If
            Return CLng(value And CULng(Long.MaxValue)) Or Long.MinValue
        End Function

    End Class

End Namespace
