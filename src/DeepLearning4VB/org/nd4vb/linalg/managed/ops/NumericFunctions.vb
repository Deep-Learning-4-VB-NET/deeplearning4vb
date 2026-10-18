' DeepLearning4VB: scalar helper functions of the managed op math registry (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original. Where a function mirrors libnd4j (templatemath.h, ops.h), the XML comment says so.

Imports System
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Scalar functions shared by the op math registry and available to the kernels: the special
    ''' functions .NET Framework lacks (<see cref="Erf"/>, <see cref="Erfc"/>, <see cref="Log1p"/>,
    ''' <see cref="Expm1"/>, the inverse hyperbolic functions), C-style rounding, and the comparison and
    ''' NaN conventions of libnd4j's <c>templatemath.h</c>. Everything is <c>Double</c>; the same code runs on
    ''' .NET Framework 4.8.1 and .NET 10, so both give identical results.
    ''' </summary>
    Public NotInheritable Class NumericFunctions

        Private Sub New()
        End Sub

        ''' <summary>libnd4j's <c>SELU_ALPHA</c>.</summary>
        Public Const SeluAlpha As Double = 1.6732632423543772
        ''' <summary>libnd4j's <c>SELU_LAMBDA</c>.</summary>
        Public Const SeluLambda As Double = 1.0507009873554805
        ''' <summary>libnd4j's <c>MIN_V</c> (1e-12): the tolerance of the <c>Epsilon</c> comparison ops.</summary>
        Public Const MinV As Double = 0.000000000001
        ''' <summary>libnd4j's <c>MIN_CUTFOFF</c> (the float literal -3.79297773665f), used by <c>stabilize</c>.</summary>
        Public Const MinCutoff As Double = -3.79297773665F
        ''' <summary>ND4J's default comparison tolerance, <c>Nd4j.EPS_THRESHOLD</c> (1e-5).</summary>
        Public Const EpsThreshold As Double = 0.00001
        ''' <summary>The GELU sigmoid-approximation factor; libnd4j writes it as the float literal 1.702f.</summary>
        Public Const GeluFactor As Double = 1.702F
        ''' <summary>The quartic coefficient of libnd4j's rational tanh approximation (float literal 1.41645f).</summary>
        Public Const RationalTanhQuartic As Double = 1.41645F
        ''' <summary>The output scale of libnd4j's rational tanh (float literal 1.7159f).</summary>
        Public Const RationalTanhScale As Double = 1.7159F

        Private Const Ln2 As Double = 0.69314718055994531
        Private Const InvSqrtPi As Double = 0.56418958354775628
        Private Const TwoOverSqrtPi As Double = 1.1283791670955126
        Private Const TwoPow52 As Double = 4503599627370496.0

        ' ---------------------------------------------------------------- booleans and comparisons

        ''' <summary>C's <c>static_cast&lt;bool&gt;(x)</c>: every value but zero is true, NaN included.</summary>
        Public Shared Function IsTrue(ByVal x As Double) As Boolean
            Return x <> 0.0
        End Function

        ''' <summary>1.0 for True, 0.0 for False: how the registry encodes boolean results.</summary>
        Public Shared Function FromBoolean(ByVal value As Boolean) As Double
            Return If(value, 1.0, 0.0)
        End Function

        ''' <summary>libnd4j's <c>nd4j_max</c>: <c>a &gt; b ? a : b</c>. Not symmetric for NaN: (NaN, 1) gives 1, (1, NaN) gives NaN.</summary>
        Public Shared Function NdMax(ByVal a As Double, ByVal b As Double) As Double
            Return If(a > b, a, b)
        End Function

        ''' <summary>libnd4j's <c>nd4j_min</c>: <c>a &lt; b ? a : b</c> (same NaN asymmetry as <see cref="NdMax"/>).</summary>
        Public Shared Function NdMin(ByVal a As Double, ByVal b As Double) As Double
            Return If(a < b, a, b)
        End Function

        ''' <summary>Maximum that propagates NaN from either side (used by the reductions, see <see cref="ReduceMath"/>).</summary>
        Public Shared Function NanMax(ByVal a As Double, ByVal b As Double) As Double
            If Double.IsNaN(a) OrElse Double.IsNaN(b) Then Return Double.NaN
            Return If(b > a, b, a)
        End Function

        ''' <summary>Minimum that propagates NaN from either side.</summary>
        Public Shared Function NanMin(ByVal a As Double, ByVal b As Double) As Double
            If Double.IsNaN(a) OrElse Double.IsNaN(b) Then Return Double.NaN
            Return If(b < a, b, a)
        End Function

        ''' <summary>
        ''' libnd4j's <c>nd4j_eq(d1, d2, eps)</c>: two infinities are equal when they have the same sign;
        ''' otherwise equal when <c>|d1 - d2| &lt;= eps</c> or <c>|d1 - d2| &lt;= max(|d1|, |d2|) * eps</c>. NaN equals nothing.
        ''' </summary>
        Public Shared Function EqualsWithEps(ByVal d1 As Double, ByVal d2 As Double, ByVal eps As Double) As Boolean
            If Double.IsInfinity(d1) AndAlso Double.IsInfinity(d2) Then
                Return (d1 > 0.0) = (d2 > 0.0)
            End If
            Dim diff As Double = SysMath.Abs(d1 - d2)
            If diff <= eps Then Return True
            Return diff <= SysMath.Max(SysMath.Abs(d1), SysMath.Abs(d2)) * eps
        End Function

        ''' <summary>libnd4j's <c>nd4j_re</c> (relative error): 0 when both are zero, else <c>|a - b| / (|a| + |b|)</c>.</summary>
        Public Shared Function RelativeError(ByVal a As Double, ByVal b As Double) As Double
            If a = 0.0 AndAlso b = 0.0 Then Return 0.0
            Return SysMath.Abs(a - b) / (SysMath.Abs(a) + SysMath.Abs(b))
        End Function

        ' ---------------------------------------------------------------- rounding and signs

        ''' <summary>libnd4j's <c>Sign</c>: <c>(x &gt; 0) - (x &lt; 0)</c>, so NaN and both zeros give 0 (.NET's Math.Sign throws on NaN).</summary>
        Public Shared Function Sign(ByVal x As Double) As Double
            If x > 0.0 Then Return 1.0
            If x < 0.0 Then Return -1.0
            Return 0.0
        End Function

        ''' <summary>C's <c>round</c> (libnd4j's <c>round</c> op): halves go away from zero; the sign of zero is kept.</summary>
        Public Shared Function RoundHalfAwayFromZero(ByVal x As Double) As Double
            If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then Return x
            Dim a As Double = SysMath.Abs(x)
            If a >= TwoPow52 Then Return x
            Dim r As Double = SysMath.Floor(a)
            If a - r >= 0.5 Then r += 1.0
            Return If(x < 0.0, -r, r)
        End Function

        ''' <summary>C's <c>rint</c> in the default rounding mode (libnd4j's <c>rint</c> op): halves go to the even neighbour.</summary>
        Public Shared Function RoundHalfToEven(ByVal x As Double) As Double
            If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then Return x
            Dim a As Double = SysMath.Abs(x)
            If a >= TwoPow52 Then Return x
            Dim r As Double = SysMath.Floor(a)
            Dim d As Double = a - r
            If d > 0.5 OrElse (d = 0.5 AndAlso (r Mod 2.0) <> 0.0) Then r += 1.0
            Return If(x < 0.0, -r, r)
        End Function

        ' ---------------------------------------------------------------- powers and remainders

        ''' <summary>
        ''' C99 <c>pow</c> (what libnd4j's <c>nd4j_pow</c> calls). Wraps <see cref="System.Math.Pow"/> with the C99 special
        ''' cases that .NET Framework gets wrong: <c>pow(x, ±0) = 1</c> and <c>pow(1, y) = 1</c> even for NaN,
        ''' <c>pow(-1, ±Infinity) = 1</c>.
        ''' </summary>
        Public Shared Function Pow(ByVal x As Double, ByVal y As Double) As Double
            If y = 0.0 Then Return 1.0
            If x = 1.0 Then Return 1.0
            If Double.IsNaN(x) OrElse Double.IsNaN(y) Then Return Double.NaN
            If Double.IsInfinity(y) AndAlso x = -1.0 Then Return 1.0
            Return SysMath.Pow(x, y)
        End Function

        ''' <summary>C's <c>fmod</c> (libnd4j's <c>FMod</c>): truncated division remainder with the sign of <paramref name="x"/>; NaN for y = 0 or infinite x.</summary>
        Public Shared Function FMod(ByVal x As Double, ByVal y As Double) As Double
            Return x Mod y
        End Function

        ''' <summary>
        ''' libnd4j's <c>FloorMod</c> (Python's <c>%</c>): <c>m = fmod(x, y)</c>; when x and y have different signs the result is
        ''' <c>fmod(m + y, y)</c>, so it takes the sign of <paramref name="y"/>.
        ''' </summary>
        Public Shared Function FloorMod(ByVal x As Double, ByVal y As Double) As Double
            Dim m As Double = x Mod y
            If (x < 0.0) = (y < 0.0) Then Return m
            Return (m + y) Mod y
        End Function

        ''' <summary>C99 <c>remainder</c> (libnd4j's <c>Remainder</c>): <c>x - n*y</c> with n = x/y rounded half to even.</summary>
        Public Shared Function IeeeRemainder(ByVal x As Double, ByVal y As Double) As Double
            Return SysMath.IEEERemainder(x, y)
        End Function

        ' ---------------------------------------------------------------- logistic family

        ''' <summary>The logistic function 1/(1 + e^-x), evaluated without overflow for either sign.</summary>
        Public Shared Function Sigmoid(ByVal x As Double) As Double
            If x >= 0.0 Then Return 1.0 / (1.0 + SysMath.Exp(-x))
            Dim e As Double = SysMath.Exp(x)
            Return e / (1.0 + e)
        End Function

        ''' <summary>
        ''' log(1 + e^x), evaluated stably (x + log1p(e^-x) for positive x). libnd4j's <c>nd4j_softplus</c> computes
        ''' <c>log(1 + exp(x))</c> literally and overflows to Infinity for large x; this returns x there.
        ''' </summary>
        Public Shared Function Softplus(ByVal x As Double) As Double
            If Double.IsNaN(x) Then Return x
            If x > 0.0 Then Return x + Log1p(SysMath.Exp(-x))
            Return Log1p(SysMath.Exp(x))
        End Function

        ''' <summary>log(e^a + e^b) without overflow; -Infinity is the identity, NaN propagates.</summary>
        Public Shared Function LogAddExp(ByVal a As Double, ByVal b As Double) As Double
            If Double.IsNaN(a) OrElse Double.IsNaN(b) Then Return Double.NaN
            If Double.IsNegativeInfinity(a) Then Return b
            If Double.IsNegativeInfinity(b) Then Return a
            If Double.IsPositiveInfinity(a) OrElse Double.IsPositiveInfinity(b) Then Return Double.PositiveInfinity
            Dim m As Double = If(a > b, a, b)
            Return m + Log1p(SysMath.Exp(-SysMath.Abs(a - b)))
        End Function

        ' ---------------------------------------------------------------- log1p, expm1

        ''' <summary>log(1 + x), accurate for small x (.NET Framework has no log1p); -Infinity at -1, NaN below.</summary>
        Public Shared Function Log1p(ByVal x As Double) As Double
            If Double.IsNaN(x) OrElse Double.IsPositiveInfinity(x) Then Return x
            Dim u As Double = 1.0 + x
            If u = 1.0 Then Return x
            If u <= 0.0 Then Return If(u = 0.0, Double.NegativeInfinity, Double.NaN)
            Return SysMath.Log(u) * x / (u - 1.0)
        End Function

        ''' <summary>e^x - 1, accurate for small x (Kahan's method; .NET Framework has no expm1).</summary>
        Public Shared Function Expm1(ByVal x As Double) As Double
            If Double.IsNaN(x) OrElse Double.IsPositiveInfinity(x) Then Return x
            If x < -40.0 Then Return -1.0
            If SysMath.Abs(x) >= 0.5 Then Return SysMath.Exp(x) - 1.0
            Dim u As Double = SysMath.Exp(x)
            If u = 1.0 Then Return x
            Return (u - 1.0) * x / SysMath.Log(u)
        End Function

        ' ---------------------------------------------------------------- inverse hyperbolic functions

        ''' <summary>Inverse hyperbolic sine (not in .NET Framework's Math).</summary>
        Public Shared Function Asinh(ByVal x As Double) As Double
            If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then Return x
            Dim a As Double = SysMath.Abs(x)
            Dim r As Double
            If a > 100000000.0 Then
                r = SysMath.Log(a) + Ln2
            ElseIf a < 0.5 Then
                r = Log1p(a + a * a / (1.0 + SysMath.Sqrt(1.0 + a * a)))
            Else
                r = SysMath.Log(a + SysMath.Sqrt(a * a + 1.0))
            End If
            Return If(x < 0.0, -r, r)
        End Function

        ''' <summary>Inverse hyperbolic cosine; NaN below 1.</summary>
        Public Shared Function Acosh(ByVal x As Double) As Double
            If Double.IsNaN(x) Then Return x
            If x < 1.0 Then Return Double.NaN
            If Double.IsPositiveInfinity(x) Then Return x
            If x > 100000000.0 Then Return SysMath.Log(x) + Ln2
            Dim t As Double = x - 1.0
            If t < 0.5 Then Return Log1p(t + SysMath.Sqrt(2.0 * t + t * t))
            Return SysMath.Log(x + SysMath.Sqrt(x * x - 1.0))
        End Function

        ''' <summary>Inverse hyperbolic tangent; ±Infinity at ±1, NaN outside [-1, 1].</summary>
        Public Shared Function Atanh(ByVal x As Double) As Double
            If Double.IsNaN(x) Then Return x
            Dim a As Double = SysMath.Abs(x)
            If a > 1.0 Then Return Double.NaN
            If a = 1.0 Then Return If(x > 0.0, Double.PositiveInfinity, Double.NegativeInfinity)
            Dim r As Double
            If a < 0.5 Then
                r = 0.5 * Log1p(2.0 * a + 2.0 * a * a / (1.0 - a))
            Else
                r = 0.5 * Log1p(2.0 * a / (1.0 - a))
            End If
            Return If(x < 0.0, -r, r)
        End Function

        ' ---------------------------------------------------------------- error function

        ''' <summary>
        ''' The error function, accurate to a few units in the last place (.NET Framework has none).
        ''' |x| &lt; 0.5: the series 2/√π e^(-x²) Σ 2ⁿx^(2n+1)/(1·3·…·(2n+1)) (all terms positive);
        ''' otherwise 1 - <see cref="Erfc"/>(|x|) with the sign of x.
        ''' </summary>
        Public Shared Function Erf(ByVal x As Double) As Double
            If Double.IsNaN(x) Then Return x
            Dim a As Double = SysMath.Abs(x)
            If a < 0.5 Then Return ErfSeries(x)
            Dim r As Double = If(a >= 6.0, 1.0, 1.0 - ErfcPositive(a))
            Return If(x < 0.0, -r, r)
        End Function

        ''' <summary>
        ''' The complementary error function 1 - erf(x), accurate in relative terms far into the tail (it underflows
        ''' to 0 only beyond x ≈ 27.3). x &lt; 0.5 uses 1 - <see cref="Erf"/>; 0.5 ≤ x &lt; 4.0625 a Taylor expansion
        ''' (Hermite polynomials) around a table of erfc values at steps of 1/8; larger x the continued fraction
        ''' e^(-x²)/√π · 1/(x + (1/2)/(x + 1/(x + (3/2)/(x + …)))) evaluated backwards.
        ''' </summary>
        Public Shared Function Erfc(ByVal x As Double) As Double
            If Double.IsNaN(x) Then Return x
            If x < 0.0 Then
                If x <= -6.0 Then Return 2.0
                If x > -0.5 Then Return 1.0 - ErfSeries(x)
                Return 2.0 - ErfcPositive(-x)
            End If
            If x < 0.5 Then Return 1.0 - ErfSeries(x)
            Return ErfcPositive(x)
        End Function

        ''' <summary>erf for |x| &lt; 0.5 (and accurate further out) by the positive series.</summary>
        Private Shared Function ErfSeries(ByVal x As Double) As Double
            Dim x2 As Double = x * x
            Dim term As Double = x
            Dim s As Double = x
            For n As Integer = 1 To 500
                term *= 2.0 * x2 / (2 * n + 1)
                s += term
                If SysMath.Abs(term) <= SysMath.Abs(s) * 1.0E-17 Then Exit For
            Next
            Return TwoOverSqrtPi * ExpMinusSquare(SysMath.Abs(x)) * s
        End Function

        ''' <summary>erfc(x) for x ≥ 0.5.</summary>
        Private Shared Function ErfcPositive(ByVal x As Double) As Double
            If x >= 27.3 Then Return 0.0
            If x < ErfcTable.Upper Then Return ErfcTable.Evaluate(x)
            Return ErfcContinuedFraction(x, 60)
        End Function

        ''' <summary>The continued fraction for erfc, evaluated from the tail backwards (numerically stable).</summary>
        Friend Shared Function ErfcContinuedFraction(ByVal x As Double, ByVal terms As Integer) As Double
            Dim t As Double = x
            For j As Integer = terms To 2 Step -1
                t = x + ((j - 1) / 2.0) / t
            Next
            Return ExpMinusSquare(x) * InvSqrtPi / t
        End Function

        ''' <summary>e^(-x²) without the rounding error of x²: x is split into a 12-bit head s and the rest, e^(-s²)·e^(-(x-s)(x+s)).</summary>
        Friend Shared Function ExpMinusSquare(ByVal x As Double) As Double
            Dim s As Double = SysMath.Floor(x * 4096.0) / 4096.0
            Dim t As Double = (x - s) * (x + s)
            Return SysMath.Exp(-s * s) * SysMath.Exp(-t)
        End Function

        ''' <summary>erfc on [0.5, 4.0625): Taylor series around the nearest of 29 points 0.5, 0.625, …, 4.0.</summary>
        Private NotInheritable Class ErfcTable
            Friend Const Lower As Double = 0.5
            Friend Const StepSize As Double = 0.125
            Friend Const Count As Integer = 29
            Friend Const Upper As Double = Lower + (Count - 1) * StepSize + StepSize / 2.0

            Private Shared ReadOnly Values As Double() = BuildValues()
            Private Shared ReadOnly Gaussians As Double() = BuildGaussians()

            Private Shared Function BuildValues() As Double()
                Dim v(Count - 1) As Double
                For k As Integer = 0 To Count - 1
                    Dim c As Double = Lower + k * StepSize
                    ' enough continued-fraction terms for full double precision at c (converged at c = 0.5 with 3260)
                    v(k) = ErfcContinuedFraction(c, CInt(800.0 / (c * c)) + 60)
                Next
                Return v
            End Function

            Private Shared Function BuildGaussians() As Double()
                Dim g(Count - 1) As Double
                For k As Integer = 0 To Count - 1
                    g(k) = ExpMinusSquare(Lower + k * StepSize)
                Next
                Return g
            End Function

            ''' <summary>
            ''' erfc(c + h) = erfc(c) - (2/√π) e^(-c²) Σ_{n≥1} (-1)^(n-1) H_(n-1)(c) hⁿ/n!, with the physicists' Hermite
            ''' polynomials H_0 = 1, H_1 = 2c, H_(k+1) = 2c H_k - 2k H_(k-1); |h| ≤ 1/16.
            ''' </summary>
            Friend Shared Function Evaluate(ByVal x As Double) As Double
                Dim k As Integer = CInt(SysMath.Floor((x - Lower) / StepSize + 0.5))
                If k < 0 Then k = 0
                If k > Count - 1 Then k = Count - 1
                Dim c As Double = Lower + k * StepSize
                Dim h As Double = x - c
                Dim hPrev As Double = 0.0
                Dim hCur As Double = 1.0
                Dim p As Double = h
                Dim s As Double = 0.0
                Dim sign As Double = 1.0
                For n As Integer = 1 To 60
                    Dim t As Double = sign * hCur * p
                    s += t
                    If n > 2 AndAlso SysMath.Abs(t) <= 1.0E-17 * SysMath.Abs(s) Then Exit For
                    Dim hNext As Double = 2.0 * c * hCur - 2.0 * (n - 1) * hPrev
                    hPrev = hCur
                    hCur = hNext
                    p *= h / (n + 1)
                    sign = -sign
                Next
                Return Values(k) - TwoOverSqrtPi * Gaussians(k) * s
            End Function
        End Class

        ' ---------------------------------------------------------------- misc

        ''' <summary>
        ''' C's <c>atan2(y, x)</c>: the angle of the point (x, y). Same as <see cref="System.Math.Atan2"/> except that two
        ''' infinite arguments give ±π/4 or ±3π/4 as in C (.NET Framework returns NaN there).
        ''' </summary>
        Public Shared Function Atan2(ByVal y As Double, ByVal x As Double) As Double
            If Double.IsInfinity(y) AndAlso Double.IsInfinity(x) Then
                Dim r As Double = If(x > 0.0, SysMath.PI / 4.0, 3.0 * SysMath.PI / 4.0)
                Return If(y > 0.0, r, -r)
            End If
            Return SysMath.Atan2(y, x)
        End Function

        ''' <summary>Natural logarithm divided by ln 2.</summary>
        Public Shared Function Log2(ByVal x As Double) As Double
            Return SysMath.Log(x) / Ln2
        End Function

    End Class

End Namespace
