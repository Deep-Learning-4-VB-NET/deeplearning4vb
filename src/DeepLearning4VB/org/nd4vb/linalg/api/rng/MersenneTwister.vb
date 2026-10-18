' DeepLearning4VB: stand-in for Apache Commons Math 3's RandomGenerator, BitsStreamGenerator and MersenneTwister
' (org.apache.commons.math3.random), used by DefaultRandom (phase 2, see docs/PHASE2_DESIGN.md).

Imports System
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.api.rng

    ''' <summary>
    ''' A source of random numbers (Apache Commons Math <c>RandomGenerator</c>, which Java's <c>DefaultRandom</c> wraps).
    ''' </summary>
    Public Interface RandomGenerator
        ''' <summary>Reseeds with an int seed.</summary>
        Sub setSeed(ByVal seed As Integer)
        ''' <summary>Reseeds with an int array seed.</summary>
        Sub setSeed(ByVal seed() As Integer)
        ''' <summary>Reseeds with a long seed.</summary>
        Sub setSeed(ByVal seed As Long)
        ''' <summary>Fills the array with random bytes.</summary>
        Sub nextBytes(ByVal bytes() As Byte)
        ''' <summary>A uniformly distributed int.</summary>
        Function nextInt() As Integer
        ''' <summary>A uniformly distributed int in [0, n).</summary>
        Function nextInt(ByVal n As Integer) As Integer
        ''' <summary>A uniformly distributed long.</summary>
        Function nextLong() As Long
        ''' <summary>A uniformly distributed boolean.</summary>
        Function nextBoolean() As Boolean
        ''' <summary>A uniformly distributed float in [0, 1).</summary>
        Function nextFloat() As Single
        ''' <summary>A uniformly distributed double in [0, 1).</summary>
        Function nextDouble() As Double
        ''' <summary>A standard normal value.</summary>
        Function nextGaussian() As Double
    End Interface

    ''' <summary>
    ''' The MT19937 Mersenne Twister of Apache Commons Math 3 (<c>MersenneTwister</c> with the methods of its base class
    ''' <c>BitsStreamGenerator</c>): the same seeding (<c>init_genrand</c> for int seeds, <c>init_by_array</c> for array and
    ''' long seeds) and the same derivation of ints, longs, booleans, floats, doubles and bytes, so a seed gives the same
    ''' sequence as Java's <c>new MersenneTwister(seed)</c>.
    ''' </summary>
    ''' <remarks>
    ''' <see cref="nextGaussian"/> uses the Box-Muller transform of Commons Math with <see cref="System.Math"/> (Commons Math:
    ''' <c>FastMath</c>), so its last bits may differ from Java. Instances are not thread-safe.
    ''' </remarks>
    Public Class MersenneTwister
        Implements RandomGenerator

        Private Const N As Integer = 624
        Private Const M As Integer = 397
        Private Shared ReadOnly MAG01 As UInteger() = {0UI, &H9908B0DFUI}

        Private ReadOnly mt(N - 1) As UInteger
        Private mti As Integer
        Private nextGaussianValue As Double = Double.NaN

        ''' <summary>A generator seeded from the clock (Commons Math adds the identity hash code).</summary>
        Public Sub New()
            setSeed(JavaSystem.currentTimeMillis() + Runtime.CompilerServices.RuntimeHelpers.GetHashCode(Me))
        End Sub

        ''' <summary>A generator seeded with an int (MT19937 <c>init_genrand</c>).</summary>
        Public Sub New(ByVal seed As Integer)
            setSeed(seed)
        End Sub

        ''' <summary>A generator seeded with an int array (MT19937 <c>init_by_array</c>).</summary>
        Public Sub New(ByVal seed() As Integer)
            setSeed(seed)
        End Sub

        ''' <summary>A generator seeded with a long (<c>init_by_array</c> of its high and low words).</summary>
        Public Sub New(ByVal seed As Long)
            setSeed(seed)
        End Sub

        ''' <summary>Reseeds with an int (MT19937 <c>init_genrand</c>).</summary>
        Public Overridable Sub setSeed(ByVal seed As Integer) Implements RandomGenerator.setSeed
            ' we use a long masked by 0xffffffffL as a poor man unsigned int
            Dim longMT As Long = seed
            mt(0) = CUInt(longMT And &HFFFFFFFFL)
            For i As Integer = 1 To N - 1
                ' See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier.
                longMT = (1812433253L * (longMT Xor (longMT >> 30)) + i) And &HFFFFFFFFL
                mt(i) = CUInt(longMT)
            Next
            mti = N
            clear()
        End Sub

        ''' <summary>Reseeds with an int array (MT19937 <c>init_by_array</c>); Nothing seeds from the clock.</summary>
        Public Overridable Sub setSeed(ByVal seed() As Integer) Implements RandomGenerator.setSeed
            If seed Is Nothing Then
                setSeed(JavaSystem.currentTimeMillis() + Runtime.CompilerServices.RuntimeHelpers.GetHashCode(Me))
                Return
            End If

            setSeed(19650218)
            Dim i As Integer = 1
            Dim j As Integer = 0

            For k As Integer = SysMath.Max(N, seed.Length) To 1 Step -1
                Dim l0 As Long = mt(i)
                Dim l1 As Long = mt(i - 1)
                Dim l As Long = (l0 Xor ((l1 Xor (l1 >> 30)) * 1664525L)) + seed(j) + j ' non linear
                mt(i) = CUInt(l And &HFFFFFFFFL)
                i += 1
                j += 1
                If i >= N Then
                    mt(0) = mt(N - 1)
                    i = 1
                End If
                If j >= seed.Length Then
                    j = 0
                End If
            Next

            For k As Integer = N - 1 To 1 Step -1
                Dim l0 As Long = mt(i)
                Dim l1 As Long = mt(i - 1)
                Dim l As Long = (l0 Xor ((l1 Xor (l1 >> 30)) * 1566083941L)) - i ' non linear
                mt(i) = CUInt(l And &HFFFFFFFFL)
                i += 1
                If i >= N Then
                    mt(0) = mt(N - 1)
                    i = 1
                End If
            Next

            mt(0) = &H80000000UI ' MSB is 1; assuring non-zero initial array
            clear()
        End Sub

        ''' <summary>Reseeds with a long: <c>init_by_array</c> of its high and low words.</summary>
        Public Overridable Sub setSeed(ByVal seed As Long) Implements RandomGenerator.setSeed
            setSeed(New Integer() {CInt((seed >> 32) And &HFFFFFFFFL), CInt(seed And &HFFFFFFFFL)})
        End Sub

        ''' <summary>Clears the cached second Gaussian value.</summary>
        Public Overridable Sub clear()
            nextGaussianValue = Double.NaN
        End Sub

        ''' <summary>The next <paramref name="bits"/> (1 to 32) random bits, as Java's <c>next(int)</c>.</summary>
        Protected Overridable Function [next](ByVal bits As Integer) As Integer
            Dim y As UInteger

            If mti >= N Then ' generate N words at one time
                Dim mtNext As UInteger = mt(0)
                For k As Integer = 0 To N - M - 1
                    Dim mtCurr As UInteger = mtNext
                    mtNext = mt(k + 1)
                    y = (mtCurr And &H80000000UI) Or (mtNext And &H7FFFFFFFUI)
                    mt(k) = mt(k + M) Xor (y >> 1) Xor MAG01(CInt(y And 1UI))
                Next
                For k As Integer = N - M To N - 2
                    Dim mtCurr As UInteger = mtNext
                    mtNext = mt(k + 1)
                    y = (mtCurr And &H80000000UI) Or (mtNext And &H7FFFFFFFUI)
                    mt(k) = mt(k + (M - N)) Xor (y >> 1) Xor MAG01(CInt(y And 1UI))
                Next
                y = (mtNext And &H80000000UI) Or (mt(0) And &H7FFFFFFFUI)
                mt(N - 1) = mt(M - 1) Xor (y >> 1) Xor MAG01(CInt(y And 1UI))

                mti = 0
            End If

            y = mt(mti)
            mti += 1

            ' tempering
            y = y Xor (y >> 11)
            y = y Xor ((y << 7) And &H9D2C5680UI)
            y = y Xor ((y << 15) And &HEFC60000UI)
            y = y Xor (y >> 18)

            Dim result As UInteger = y >> (32 - bits)
            Return If(result > Integer.MaxValue, CInt(CLng(result) - 4294967296L), CInt(result))
        End Function

        ''' <summary>Java <c>nextBoolean()</c>: one random bit.</summary>
        Public Overridable Function nextBoolean() As Boolean Implements RandomGenerator.nextBoolean
            Return [next](1) <> 0
        End Function

        ''' <summary>Java <c>nextDouble()</c>: 52 random bits scaled to [0, 1).</summary>
        Public Overridable Function nextDouble() As Double Implements RandomGenerator.nextDouble
            Dim high As Long = CLng([next](26)) << 26
            Dim low As Integer = [next](26)
            Return (high Or low) * 2.2204460492503131E-16
        End Function

        ''' <summary>Java <c>nextFloat()</c>: 23 random bits scaled to [0, 1).</summary>
        Public Overridable Function nextFloat() As Single Implements RandomGenerator.nextFloat
            Return [next](23) * 1.1920929E-07F
        End Function

        ''' <summary>Java <c>nextGaussian()</c>: Box-Muller pairs, the second value cached.</summary>
        Public Overridable Function nextGaussian() As Double Implements RandomGenerator.nextGaussian
            Dim random As Double
            If Double.IsNaN(nextGaussianValue) Then
                ' generate a new pair of gaussian numbers
                Dim x As Double = nextDouble()
                Dim y As Double = nextDouble()
                Dim alpha As Double = 2 * SysMath.PI * x
                Dim r As Double = SysMath.Sqrt(-2 * SysMath.Log(y))
                random = r * SysMath.Cos(alpha)
                nextGaussianValue = r * SysMath.Sin(alpha)
            Else
                ' use the second element of the pair already generated
                random = nextGaussianValue
                nextGaussianValue = Double.NaN
            End If
            Return random
        End Function

        ''' <summary>Java <c>nextInt()</c>: 32 random bits.</summary>
        Public Overridable Function nextInt() As Integer Implements RandomGenerator.nextInt
            Return [next](32)
        End Function

        ''' <summary>Java <c>nextInt(n)</c>: uniform in [0, n) by rejection.</summary>
        ''' <exception cref="ArgumentException"><paramref name="n"/> is not positive.</exception>
        Public Overridable Function nextInt(ByVal n As Integer) As Integer Implements RandomGenerator.nextInt
            If n > 0 Then
                If (n And -n) = n Then
                    Return CInt((n * CLng([next](31))) >> 31)
                End If
                Dim bits As Integer
                Dim val As Integer
                Do
                    bits = [next](31)
                    val = bits Mod n
                Loop While bits - val + (n - 1) < 0
                Return val
            End If
            Throw New ArgumentException("Bound must be strictly positive, was " & n)
        End Function

        ''' <summary>Java <c>nextLong()</c>: two 32-bit draws.</summary>
        Public Overridable Function nextLong() As Long Implements RandomGenerator.nextLong
            Dim high As Long = CLng([next](32)) << 32
            Dim low As Long = CLng([next](32)) And &HFFFFFFFFL
            Return high Or low
        End Function

        ''' <summary>Java <c>nextBytes(byte[])</c>: four bytes per 32-bit draw, low byte first.</summary>
        Public Overridable Sub nextBytes(ByVal bytes() As Byte) Implements RandomGenerator.nextBytes
            Dim index As Integer = 0
            Dim indexLoopLimit As Integer = bytes.Length And &H7FFFFFFC
            Do While index < indexLoopLimit
                Dim random As Integer = [next](32)
                bytes(index) = CByte(random And &HFF)
                bytes(index + 1) = CByte((random >> 8) And &HFF)
                bytes(index + 2) = CByte((random >> 16) And &HFF)
                bytes(index + 3) = CByte((random >> 24) And &HFF)
                index += 4
            Loop
            If index < bytes.Length Then
                Dim random As Integer = [next](32)
                Do
                    bytes(index) = CByte(random And &HFF)
                    index += 1
                    If index < bytes.Length Then
                        random >>= 8
                    Else
                        Exit Do
                    End If
                Loop
            End If
        End Sub

    End Class

End Namespace
