' DeepLearning4VB: the math of ND4J's transform ops (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original: the semantics are those of libnd4j's simdOps (ops.h); the Java classes are in
' unported/nd4j/.../linalg/api/ops/impl/transforms/{same,floating,strict,bool,any,comparison,gradient,pairwise}.

Imports System
Imports SysMath = System.Math
Imports NF = DeepLearning4VB.org.nd4vb.linalg.managed.ops.NumericFunctions

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Registers the transform ops: z = f(x, extraArgs). Kinds follow the Java base class (BaseTransformSameOp,
    ''' FloatOp, StrictOp, BoolOp, AnyOp). Notes on edge cases (all from libnd4j unless said otherwise):
    ''' <list type="bullet">
    ''' <item><c>sign</c> of NaN is 0 (<c>(x &gt; 0) - (x &lt; 0)</c>); <c>round</c> rounds halves away from zero (C <c>round</c>),
    ''' <c>rint</c> to even (C <c>rint</c>); <c>Reciprocal</c>, <c>rsqrt</c> and <c>log</c> of 0 give ±Infinity; <c>log</c>,
    ''' <c>sqrt</c>, <c>log1p</c> (below -1), <c>acosh</c> (below 1), <c>asin</c>/<c>acos</c>/<c>atanh</c> (outside [-1, 1]) give NaN.</item>
    ''' <item><c>softplus</c>, <c>logsigmoid</c>, <c>mish</c> and the sigmoid-based ops are evaluated without overflow; libnd4j's
    ''' literal formulas overflow to ±Infinity or NaN for |x| beyond roughly 700 in double (88 in float). Not libnd4j-exact
    ''' there, by design.</item>
    ''' <item>The float literals of libnd4j (1.702f in <c>gelu</c>, 1.41645f and 1.7159f in <c>rational_tanh</c>) are kept as
    ''' float values, so DOUBLE results match libnd4j's double instantiation.</item>
    ''' <item>The derivative ops (<c>_sigmoidderivative</c>, <c>_geluderivative</c>, <c>_mishderivative</c>, ...) return the
    ''' exact derivative of the matching forward op; at the kinks of piecewise ops they follow libnd4j's choice
    ''' (<c>hardtanhderivative</c> is 1 on the closed interval [-1, 1], <c>hard_sigmoidderivative</c> 0.2 on [-2.5, 2.5],
    ''' <c>rectified_tanh_derivative</c> 0 at x = 0).</item>
    ''' </list>
    ''' </summary>
    Friend NotInheritable Class TransformMath

        Private Sub New()
        End Sub

        Private Const SqrtTwoOverPi As Double = 0.79788456080286536
        Private Const GeluCubic As Double = 0.044715
        Private Const TwoThirds As Double = 2.0 / 3.0

        ''' <summary>The identity, shared by <c>copy</c>, <c>set</c>, <c>old_assign</c> and the custom <c>identity</c>.</summary>
        Friend Shared ReadOnly Identity As TransformFunction = Function(x, a) x

        Friend Shared Sub Register(ByVal t As OpTable)
            RegisterSame(t)
            RegisterFloat(t)
            RegisterStrict(t)
            RegisterBool(t)
            RegisterAny(t)
            RegisterCustom(t)
        End Sub

        ' ---------------------------------------------------------------- transform same

        Private Shared Sub RegisterSame(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.TransformSame
            t.AddTransform(k, "abs", Function(x, a) SysMath.Abs(x))
            t.AddTransform(k, "ceil", Function(x, a) SysMath.Ceiling(x))
            t.AddTransform(k, "cube", Function(x, a) x * x * x)
            t.AddTransform(k, "floor", Function(x, a) SysMath.Floor(x))
            t.AddTransform(k, "neg", Function(x, a) -x)
            t.AddTransform(k, "oneminus", Function(x, a) 1.0 - x)
            t.AddTransform(k, "Reciprocal", Function(x, a) 1.0 / x)
            t.AddTransform(k, "round", Function(x, a) NF.RoundHalfAwayFromZero(x))
            t.AddTransform(k, "sign", Function(x, a) NF.Sign(x))
            t.AddTransform(k, "square", Function(x, a) x * x)
            t.AddTransform(k, "timesoneminus", Function(x, a) x * (1.0 - x))
            ' CompareAndSet without y: z = condition(x) ? set : x; extraArgs {compare, set, eps, mode}
            t.AddTransform(k, "cas", Function(x, a) If(ConditionMath.MatchesCompareAndSetArgs(x, a, "cas"), a(1), x))
            ' CopyOp without y copies x
            t.AddTransform(k, "copy", Identity)
        End Sub

        ' ---------------------------------------------------------------- transform float

        Private Shared Sub RegisterFloat(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.TransformFloat
            t.AddTransform(k, "sqrt", Function(x, a) SysMath.Sqrt(x))
            t.AddTransform(k, "rsqrt", Function(x, a) 1.0 / SysMath.Sqrt(x))
        End Sub

        ' ---------------------------------------------------------------- transform strict

        Private Shared Sub RegisterStrict(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.TransformStrict

            ' trigonometric and hyperbolic
            t.AddTransform(k, "sin", Function(x, a) SysMath.Sin(x))
            t.AddTransform(k, "cos", Function(x, a) SysMath.Cos(x))
            t.AddTransform(k, "tan", Function(x, a) SysMath.Tan(x))
            t.AddTransform(k, "asin", Function(x, a) SysMath.Asin(x))
            t.AddTransform(k, "acos", Function(x, a) SysMath.Acos(x))
            t.AddTransform(k, "atan", Function(x, a) SysMath.Atan(x))
            t.AddTransform(k, "sinh", Function(x, a) SysMath.Sinh(x))
            t.AddTransform(k, "cosh", Function(x, a) SysMath.Cosh(x))
            t.AddTransform(k, "tanh", Function(x, a) SysMath.Tanh(x))
            t.AddTransform(k, "asinh", Function(x, a) NF.Asinh(x))
            t.AddTransform(k, "acosh", Function(x, a) NF.Acosh(x))
            t.AddTransform(k, "atanh", Function(x, a) NF.Atanh(x))
            ' TanDerivative: 1 / cos(x)^2
            t.AddTransform(k, "tanderivative", Function(x, a)
                                                   Dim c As Double = SysMath.Cos(x)
                                                   Return 1.0 / (c * c)
                                               End Function)
            ' TanhDerivative: 1 - tanh(x)^2
            t.AddTransform(k, "_tanhderivative", Function(x, a) TanhDerivative(x))

            ' exponential and logarithmic
            t.AddTransform(k, "exp", Function(x, a) SysMath.Exp(x))
            t.AddTransform(k, "expm1", Function(x, a) NF.Expm1(x))
            t.AddTransform(k, "log", Function(x, a) SysMath.Log(x))
            t.AddTransform(k, "log1p", Function(x, a) NF.Log1p(x))
            t.AddTransform(k, "erf", Function(x, a) NF.Erf(x))
            t.AddTransform(k, "erfc", Function(x, a) NF.Erfc(x))
            t.AddTransform(k, "rint", Function(x, a) NF.RoundHalfToEven(x))

            ' logistic family
            t.AddTransform(k, "sigmoid", Function(x, a) NF.Sigmoid(x))
            t.AddTransform(k, "_sigmoidderivative", Function(x, a)
                                                        Dim s As Double = NF.Sigmoid(x)
                                                        Return s * (1.0 - s)
                                                    End Function)
            t.AddTransform(k, "logsigmoid", Function(x, a) -NF.Softplus(-x))
            t.AddTransform(k, "softplus", Function(x, a) NF.Softplus(x))
            t.AddTransform(k, "softsign", Function(x, a) x / (1.0 + SysMath.Abs(x)))
            t.AddTransform(k, "softsignderivative", Function(x, a)
                                                        Dim d As Double = 1.0 + SysMath.Abs(x)
                                                        Return 1.0 / (d * d)
                                                    End Function)
            ' Swish: x * sigmoid(x); derivative sigmoid(x) + x * sigmoid(x) * (1 - sigmoid(x))
            t.AddTransform(k, "swish", Function(x, a) x * NF.Sigmoid(x))
            t.AddTransform(k, "_swishderivative", Function(x, a)
                                                      Dim s As Double = NF.Sigmoid(x)
                                                      Return s + x * s * (1.0 - s)
                                                  End Function)
            ' GELU (sigmoid approximation): x * sigmoid(1.702 x)
            t.AddTransform(k, "gelu", Function(x, a) x * NF.Sigmoid(NF.GeluFactor * x))
            t.AddTransform(k, "_geluderivative", Function(x, a)
                                                     Dim s As Double = NF.Sigmoid(NF.GeluFactor * x)
                                                     Return s + NF.GeluFactor * x * s * (1.0 - s)
                                                 End Function)
            ' precise GELU (tanh approximation): 0.5 x (1 + tanh(sqrt(2/pi) (x + 0.044715 x^3)))
            t.AddTransform(k, "precise_gelu", Function(x, a) 0.5 * x * (1.0 + SysMath.Tanh(SqrtTwoOverPi * (x + GeluCubic * x * x * x))))
            t.AddTransform(k, "_precise_geluderivative", Function(x, a)
                                                             Dim th As Double = SysMath.Tanh(SqrtTwoOverPi * (x + GeluCubic * x * x * x))
                                                             Return 0.5 * (1.0 + th) + 0.5 * x * (1.0 - th * th) * SqrtTwoOverPi * (1.0 + 3.0 * GeluCubic * x * x)
                                                         End Function)
            ' Mish: x * tanh(softplus(x)); derivative tanh(sp) + x * sech(sp)^2 * sigmoid(x)
            t.AddTransform(k, "mish", Function(x, a) x * SysMath.Tanh(NF.Softplus(x)))
            t.AddTransform(k, "_mishderivative", Function(x, a)
                                                     Dim th As Double = SysMath.Tanh(NF.Softplus(x))
                                                     Return th + x * (1.0 - th * th) * NF.Sigmoid(x)
                                                 End Function)

            ' piecewise activations
            t.AddTransform(k, "hardtanh", Function(x, a)
                                              If x < -1.0 Then Return -1.0
                                              If x > 1.0 Then Return 1.0
                                              Return x
                                          End Function)
            t.AddTransform(k, "hardtanhderivative", Function(x, a) If(x >= -1.0 AndAlso x <= 1.0, 1.0, 0.0))
            ' HardSigmoid: min(1, max(0, 0.2 x + 0.5)) with nd4j_min/nd4j_max (NaN stays NaN)
            t.AddTransform(k, "hard_sigmoid", Function(x, a) NF.NdMin(1.0, NF.NdMax(0.0, 0.2 * x + 0.5)))
            t.AddTransform(k, "hard_sigmoidderivative", Function(x, a) If(x < -2.5 OrElse x > 2.5, 0.0, 0.2))
            ' SELU
            t.AddTransform(k, "selu", Function(x, a) If(x > 0.0, NF.SeluLambda * x, NF.SeluLambda * NF.SeluAlpha * NF.Expm1(x)))
            t.AddTransform(k, "seluderivative", Function(x, a) If(x > 0.0, NF.SeluLambda, NF.SeluAlpha * NF.SeluLambda * SysMath.Exp(x)))
            ' RectifiedTanh: max(0, tanh(x)); derivative x > 0 ? 1 - tanh^2 : 0
            t.AddTransform(k, "rectified_tanh", Function(x, a) NF.NdMax(0.0, SysMath.Tanh(x)))
            t.AddTransform(k, "rectified_tanh_derivative", Function(x, a) If(x > 0.0, TanhDerivative(x), 0.0))
            ' RationalTanh: 1.7159 * sgn(d) * (1 - 1/(1 + |d| + d^2 + 1.41645 d^4)) with d = 2x/3
            t.AddTransform(k, "rational_tanh", Function(x, a) RationalTanh(x))
            t.AddTransform(k, "rational_tanh_derivative", Function(x, a) RationalTanhDerivative(x))
            ' CubeDerivative: 3 x^2
            t.AddTransform(k, "cubederivative", Function(x, a) 3.0 * x * x)

            ' Stabilize(k): clamps x*k to [MIN_CUTFOFF, -MIN_CUTFOFF] and divides back by k; extraArgs {k} (default 1)
            t.AddTransform(k, "stabilize", Function(x, a)
                                               Dim kk As Double = OpArgs.Get(a, 0, 1.0)
                                               Dim xk As Double = x * kk
                                               If xk > -NF.MinCutoff Then Return -NF.MinCutoff / kk
                                               If xk < NF.MinCutoff Then Return NF.MinCutoff / kk
                                               Return x
                                           End Function)
            ' SetRange(min, max): libnd4j keeps values inside [min, max]; outside, floor(v * (max - min)) + min with
            ' v = sigmoid(x) for the range [0, 1] and v = x otherwise. extraArgs {min, max} (Java default 0, 1)
            t.AddTransform(k, "setrange", Function(x, a)
                                              Dim lo As Double = OpArgs.Get(a, 0, 0.0)
                                              Dim hi As Double = OpArgs.Get(a, 1, 1.0)
                                              If x >= lo AndAlso x <= hi Then Return x
                                              If lo = 0.0 AndAlso hi = 1.0 Then
                                                  Return SysMath.Floor(NF.Sigmoid(x) * (hi - lo)) + lo
                                              End If
                                              Return SysMath.Floor(x * (hi - lo)) + lo
                                          End Function)
        End Sub

        Friend Shared Function TanhDerivative(ByVal x As Double) As Double
            Dim th As Double = SysMath.Tanh(x)
            Return 1.0 - th * th
        End Function

        Friend Shared Function RationalTanh(ByVal x As Double) As Double
            Dim d As Double = TwoThirds * x
            Dim d2 As Double = d * d
            Dim denominator As Double = 1.0 + SysMath.Abs(d) + d2 + NF.RationalTanhQuartic * d2 * d2
            Return NF.RationalTanhScale * NF.Sign(d) * (1.0 - 1.0 / denominator)
        End Function

        Friend Shared Function RationalTanhDerivative(ByVal x As Double) As Double
            Dim d As Double = TwoThirds * x
            Dim d2 As Double = d * d
            Dim denominator As Double = 1.0 + SysMath.Abs(d) + d2 + NF.RationalTanhQuartic * d2 * d2
            Dim numerator As Double = 1.0 + NF.Sign(d) * (2.0 * d + 4.0 * NF.RationalTanhQuartic * d2 * d)
            Return NF.RationalTanhScale * TwoThirds * numerator / (denominator * denominator)
        End Function

        ' ---------------------------------------------------------------- transform bool

        Private Shared Sub RegisterBool(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.TransformBool
            t.AddTransform(k, "isnan", Function(x, a) NF.FromBoolean(Double.IsNaN(x)))
            t.AddTransform(k, "isinf", Function(x, a) NF.FromBoolean(Double.IsInfinity(x)))
            t.AddTransform(k, "isfinite", Function(x, a) NF.FromBoolean(Not (Double.IsNaN(x) OrElse Double.IsInfinity(x))))
            ' BooleanNot: !static_cast<bool>(x) (NaN is true, so its negation is 0)
            t.AddTransform(k, "bool_not", Function(x, a) NF.FromBoolean(Not NF.IsTrue(x)))
            ' MatchConditionTransform: extraArgs {compare, eps, mode}
            t.AddTransform(k, "match_condition_transform", Function(x, a) NF.FromBoolean(ConditionMath.MatchesArgs(x, a, "match_condition_transform")))
            ' Not without y: x == comparable (extraArgs {comparable}, default 0, which is !bool(x))
            t.AddTransform(k, "not", Function(x, a) NF.FromBoolean(x = OpArgs.Get(a, 0, 0.0)))
        End Sub

        ' ---------------------------------------------------------------- transform any

        Private Shared Sub RegisterAny(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.TransformAny
            ' Assign (old_assign) and Set without y: copy x (with type conversion by the executioner)
            t.AddTransform(k, "old_assign", Identity)
            t.AddTransform(k, "set", Identity)
        End Sub

        ' ---------------------------------------------------------------- custom ops with element-wise math

        Private Shared Sub RegisterCustom(ByVal t As OpTable)
            ' elu: x >= 0 ? x : alpha * (e^x - 1); tArgs {alpha}, default 1
            t.AddCustomTransform(ManagedOpKind.TransformStrict, "elu", Function(x, a)
                                                                         If x >= 0.0 Then Return x
                                                                         Return OpArgs.Get(a, 0, 1.0) * NF.Expm1(x)
                                                                     End Function)
            ' thresholdedrelu: x > cutoff ? x : 0; tArgs {cutoff}, default 0
            t.AddCustomTransform(ManagedOpKind.TransformSame, "thresholdedrelu", Function(x, a) If(x > OpArgs.Get(a, 0, 0.0), x, 0.0))
            ' ClipByValue: clamp to [min, max]; tArgs {min, max} required
            t.AddCustomTransform(ManagedOpKind.TransformSame, "ClipByValue", Function(x, a)
                                                                                 Dim lo As Double = OpArgs.Require(a, 0, "ClipByValue", "clipValueMin")
                                                                                 Dim hi As Double = OpArgs.Require(a, 1, "ClipByValue", "clipValueMax")
                                                                                 If x > hi Then Return hi
                                                                                 If x < lo Then Return lo
                                                                                 Return x
                                                                             End Function)
            ' boolean_not (LogicalNot): !bool(x)
            t.AddCustomTransform(ManagedOpKind.TransformBool, "boolean_not", Function(x, a) NF.FromBoolean(Not NF.IsTrue(x)))
            ' identity
            t.AddCustomTransform(ManagedOpKind.TransformSame, "identity", Identity)
        End Sub

    End Class

End Namespace
