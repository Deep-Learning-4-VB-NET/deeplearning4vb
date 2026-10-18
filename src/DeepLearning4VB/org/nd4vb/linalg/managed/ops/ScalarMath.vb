' DeepLearning4VB: the math of ND4J's scalar ops (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original: the semantics are those of libnd4j's simdOps (ops.h); the Java classes are in
' unported/nd4j/.../linalg/api/ops/impl/scalar (and transforms/gradient/LeakyReLUDerivative).

Imports System
Imports SysMath = System.Math
Imports NF = DeepLearning4VB.org.nd4vb.linalg.managed.ops.NumericFunctions

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Registers the scalar ops: z = f(x, s, extraArgs) with s the op's scalar. Where the Java op also copies its
    ''' parameter into <c>extraArgs</c> (LeakyReLU's alpha, Pow's exponent, LogX's base, Step's cutoff, ReplaceNans' value),
    ''' the scalar is what counts, as in libnd4j. Arithmetic follows IEEE; <c>max_scalar</c>/<c>scalar_min</c> use
    ''' libnd4j's <c>nd4j_max</c>/<c>nd4j_min</c> (<c>x &gt; s ? x : s</c>), so a NaN in x becomes s for <c>max_scalar</c>.
    ''' </summary>
    Friend NotInheritable Class ScalarMath

        Private Sub New()
        End Sub

        Friend Shared Sub Register(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.Scalar
            t.AddScalar(k, "add_scalar", PairwiseMath.AddFn)
            t.AddScalar(k, "sub_scalar", PairwiseMath.SubtractFn)
            t.AddScalar(k, "mul_scalar", PairwiseMath.MultiplyFn)
            t.AddScalar(k, "div_scalar", PairwiseMath.DivideFn)
            t.AddScalar(k, "rdiv_scalar", PairwiseMath.ReverseDivideFn)
            t.AddScalar(k, "rsub_scalar", PairwiseMath.ReverseSubtractFn)
            t.AddScalar(k, "max_scalar", PairwiseMath.MaxFn)
            t.AddScalar(k, "scalar_min", PairwiseMath.MinFn)
            t.AddScalar(k, "fmod_scalar", PairwiseMath.FModFn)
            t.AddScalar(k, "remainder_scalar", PairwiseMath.RemainderFn)
            ' ScalarSet: every element becomes the scalar
            t.AddScalar(k, "set_scalar", PairwiseMath.CopyYFn)
            ' ScalarSetValue (SetValOrLess): values below the scalar become the scalar (s < x ? x : s; NaN becomes s)
            t.AddScalar(k, "setvalorless_scalar", Function(x, s, a) If(s < x, x, s))
            ' Pow: x^s (C99 pow)
            t.AddScalar(k, "pow", PairwiseMath.PowFn)
            ' PowDerivative: s * x^(s - 1)
            t.AddScalar(k, "_powderivative", Function(x, s, a) s * NF.Pow(x, s - 1.0))
            ' LogX: log(x) / log(s)
            t.AddScalar(k, "log_x", Function(x, s, a) SysMath.Log(x) / SysMath.Log(s))
            ' RectifiedLinear (relu) with cutoff s: x < s ? s : x (libnd4j's RELU; s = 0 gives max(x, 0))
            t.AddScalar(k, "relu", Function(x, s, a) If(x < s, s, x))
            ' Relu6 with cutoff s: min(relu(x, s), 6)
            t.AddScalar(k, "relu6", Function(x, s, a)
                                        Dim r As Double = If(x < s, s, x)
                                        Return If(r < 6.0, r, 6.0)
                                    End Function)
            ' LeakyReLU with alpha s: x < 0 ? s * x : x
            t.AddScalar(k, "leakyrelu", Function(x, s, a) If(x < 0.0, s * x, x))
            ' LeakyReLUDerivative with alpha s: x >= 0 ? 1 : s
            t.AddScalar(k, "leakyreluderivative", Function(x, s, a) If(x >= 0.0, 1.0, s))
            ' Step with cutoff s: x > s ? 1 : 0
            t.AddScalar(k, "step", Function(x, s, a) NF.FromBoolean(x > s))
            ' ReplaceNans: NaN becomes s
            t.AddScalar(k, "replace_nans", Function(x, s, a) If(Double.IsNaN(x), s, x))

            Dim b As ManagedOpKind = ManagedOpKind.ScalarBool
            t.AddScalar(b, "equals_scalar", PairwiseMath.EqualFn)
            t.AddScalar(b, "notequals_scalar", PairwiseMath.NotEqualFn)
            t.AddScalar(b, "greaterthan_scalar", PairwiseMath.GreaterFn)
            t.AddScalar(b, "greaterthanorequal_scalar", PairwiseMath.GreaterOrEqualFn)
            t.AddScalar(b, "lessthan_scalar", PairwiseMath.LessFn)
            t.AddScalar(b, "lessthanorequal_scalar", PairwiseMath.LessOrEqualFn)
            ' ScalarEps: |x - s| <= MIN_V (1e-12)
            t.AddScalar(b, "eps_scalar", PairwiseMath.EpsilonFn)
            ' ScalarAnd/Or/Xor: bool(x) op bool(s) (the Java ops carry no extra arguments; a comparable in
            ' extraArgs(0) replaces 0 as the "false" value, as for the pairwise and/or/xor)
            t.AddScalar(b, "and_scalar", PairwiseMath.AndFn)
            t.AddScalar(b, "or_scalar", PairwiseMath.OrFn)
            t.AddScalar(b, "xor_scalar", PairwiseMath.XorFn)
            ' ScalarNot: x == s ? 1 : 0, the scalar playing the role of the "false" value (Transforms.not passes 0,
            ' which gives !bool(x))
            t.AddScalar(b, "not_scalar", Function(x, s, a) NF.FromBoolean(x = s))
        End Sub

    End Class

End Namespace
