' DeepLearning4VB: the math of ND4J's pairwise and broadcast ops (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original: the semantics are those of libnd4j's simdOps (ops.h); the Java classes are in
' unported/nd4j/.../linalg/api/ops/impl/{transforms/pairwise,transforms/same,transforms/comparison,broadcast}.

Imports System
Imports SysMath = System.Math
Imports NF = DeepLearning4VB.org.nd4vb.linalg.managed.ops.NumericFunctions

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Registers the pairwise and broadcast ops: z = f(x, y, extraArgs) where y is the second operand (pairwise) or the
    ''' broadcast operand. Also registers the element-wise math of the broadcastable custom ops (<c>add</c>,
    ''' <c>subtract</c>, <c>equals</c>, ..., the activation backprop ops <c>*_bp</c>), which libnd4j implements with the same
    ''' simdOps; for those, x and y are the op's first and second inputs.
    ''' <list type="bullet">
    ''' <item>Division follows IEEE (x/0 is ±Infinity, 0/0 NaN). <c>max</c>/<c>min</c> use libnd4j's <c>nd4j_max</c>/<c>nd4j_min</c>
    ''' (<c>x &gt; y ? x : y</c>), so a NaN in y is dropped and a NaN in x kept.</item>
    ''' <item>The comparisons return 1/0 and are false for NaN except "not equal".</item>
    ''' <item><c>mod</c> and <c>truncatediv</c> cast both operands to integers first, as libnd4j's <c>Mod</c>/<c>TruncateDiv</c>
    ''' do; a zero divisor (undefined behaviour in libnd4j) gives NaN or ±Infinity here.</item>
    ''' </list>
    ''' </summary>
    Friend NotInheritable Class PairwiseMath

        Private Sub New()
        End Sub

        ' shared element functions (one delegate per math, reused by the pairwise, broadcast and custom names)
        Friend Shared ReadOnly AddFn As PairwiseFunction = Function(x, y, a) x + y
        Friend Shared ReadOnly SubtractFn As PairwiseFunction = Function(x, y, a) x - y
        Friend Shared ReadOnly MultiplyFn As PairwiseFunction = Function(x, y, a) x * y
        Friend Shared ReadOnly DivideFn As PairwiseFunction = Function(x, y, a) x / y
        Friend Shared ReadOnly ReverseDivideFn As PairwiseFunction = Function(x, y, a) y / x
        Friend Shared ReadOnly ReverseSubtractFn As PairwiseFunction = Function(x, y, a) y - x
        Friend Shared ReadOnly CopyYFn As PairwiseFunction = Function(x, y, a) y
        Friend Shared ReadOnly MaxFn As PairwiseFunction = Function(x, y, a) NF.NdMax(x, y)
        Friend Shared ReadOnly MinFn As PairwiseFunction = Function(x, y, a) NF.NdMin(x, y)
        ' AMaxPairwise / AMinPairwise return the operand (with its sign) of larger / smaller magnitude
        Friend Shared ReadOnly AMaxFn As PairwiseFunction = Function(x, y, a) If(SysMath.Abs(x) > SysMath.Abs(y), x, y)
        Friend Shared ReadOnly AMinFn As PairwiseFunction = Function(x, y, a) If(SysMath.Abs(x) < SysMath.Abs(y), x, y)
        Friend Shared ReadOnly PowFn As PairwiseFunction = Function(x, y, a) NF.Pow(x, y)
        Friend Shared ReadOnly FModFn As PairwiseFunction = Function(x, y, a) NF.FMod(x, y)
        Friend Shared ReadOnly RemainderFn As PairwiseFunction = Function(x, y, a) NF.IeeeRemainder(x, y)
        Friend Shared ReadOnly EqualFn As PairwiseFunction = Function(x, y, a) NF.FromBoolean(x = y)
        Friend Shared ReadOnly NotEqualFn As PairwiseFunction = Function(x, y, a) NF.FromBoolean(x <> y)
        Friend Shared ReadOnly GreaterFn As PairwiseFunction = Function(x, y, a) NF.FromBoolean(x > y)
        Friend Shared ReadOnly GreaterOrEqualFn As PairwiseFunction = Function(x, y, a) NF.FromBoolean(x >= y)
        Friend Shared ReadOnly LessFn As PairwiseFunction = Function(x, y, a) NF.FromBoolean(x < y)
        Friend Shared ReadOnly LessOrEqualFn As PairwiseFunction = Function(x, y, a) NF.FromBoolean(x <= y)
        ' libnd4j Epsilon: |x - y| <= MIN_V (1e-12)
        Friend Shared ReadOnly EpsilonFn As PairwiseFunction = Function(x, y, a) NF.FromBoolean(SysMath.Abs(x - y) <= NF.MinV)
        ' And/Or/Xor/Not with the "comparable" of the Java ops (extraArgs {comparable}, default 0): a value counts as
        ' false when it equals the comparable. With 0 this is C's bool conversion (NaN is true).
        Friend Shared ReadOnly AndFn As PairwiseFunction = Function(x, y, a)
                                                                Dim c As Double = OpArgs.Get(a, 0, 0.0)
                                                                Return NF.FromBoolean(x <> c AndAlso y <> c)
                                                            End Function
        Friend Shared ReadOnly OrFn As PairwiseFunction = Function(x, y, a)
                                                               Dim c As Double = OpArgs.Get(a, 0, 0.0)
                                                               Return NF.FromBoolean(x <> c OrElse y <> c)
                                                           End Function
        Friend Shared ReadOnly XorFn As PairwiseFunction = Function(x, y, a)
                                                                Dim c As Double = OpArgs.Get(a, 0, 0.0)
                                                                Return NF.FromBoolean((x <> c) <> (y <> c))
                                                            End Function

        Friend Shared Sub Register(ByVal t As OpTable)
            RegisterLegacyPairwise(t)
            RegisterPairwiseBool(t)
            RegisterBroadcast(t)
            RegisterCustom(t)
            RegisterCustomBackprop(t)
        End Sub

        ' ---------------------------------------------------------------- legacy pairwise (transform classes given a y)

        Private Shared Sub RegisterLegacyPairwise(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.Pairwise
            ' CopyOp, Set, Assign (old_assign) with y: z = y
            t.AddPairwise(k, "copy", CopyYFn)
            t.AddPairwise(k, "set", CopyYFn)
            t.AddPairwise(k, "old_assign", CopyYFn)
            t.AddPairwise(k, "fmod", FModFn)
            t.AddPairwise(k, "remainder", RemainderFn)
            t.AddPairwise(k, "pow_pairwise", PowFn)
            ' Axpy(p): z = p * x + y; extraArgs {p, n} (p defaults to 1, libnd4j's two-argument form)
            t.AddPairwise(k, "axpy", Function(x, y, a) OpArgs.Get(a, 0, 1.0) * x + y)
            t.AddPairwise(k, "max_pairwise", MaxFn)
            t.AddPairwise(k, "min_pairwise", MinFn)
            t.AddPairwise(k, "amax_pairwise", AMaxFn)
            t.AddPairwise(k, "amin_pairwise", AMinFn)
            ' RelativeError: nd4j_re(x, y) = |x - y| / (|x| + |y|), 0 when both are 0
            t.AddPairwise(k, "RelativeError", Function(x, y, a) NF.RelativeError(x, y))
            ' BinaryRelativeError(threshold): nd4j_re(x, y) > threshold ? 1 : 0; extraArgs {threshold}, default 0
            t.AddPairwise(k, "BinaryRelativeError", Function(x, y, a) NF.FromBoolean(NF.RelativeError(x, y) > OpArgs.Get(a, 0, 0.0)))
            ' BinaryMinimalRelativeError(relative, absolute): 1 when |x - y| >= absolute and nd4j_re(x, y) > relative,
            ' and 1 when either is NaN; extraArgs {thresholdRelative, thresholdAbsolute}, defaults 0
            t.AddPairwise(k, "BinaryMinimalRelativeError", Function(x, y, a)
                                                               If Double.IsNaN(x) OrElse Double.IsNaN(y) Then Return 1.0
                                                               If x = y Then Return 0.0
                                                               If SysMath.Abs(x - y) < OpArgs.Get(a, 1, 0.0) Then Return 0.0
                                                               Return NF.FromBoolean(NF.RelativeError(x, y) > OpArgs.Get(a, 0, 0.0))
                                                           End Function)
            ' CompareAndSet with y: z = condition(y) ? y : x; extraArgs {compare, set, eps, mode}
            t.AddPairwise(k, "cas", Function(x, y, a) If(ConditionMath.MatchesCompareAndSetArgs(y, a, "cas"), y, x))
            ' CompareAndReplace: z = condition(x) ? y : x; extraArgs {compare, set, eps, mode}
            t.AddPairwise(k, "car", Function(x, y, a) If(ConditionMath.MatchesCompareAndSetArgs(x, a, "car"), y, x))
        End Sub

        ' ---------------------------------------------------------------- pairwise bool

        Private Shared Sub RegisterPairwiseBool(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.PairwiseBool
            t.AddPairwise(k, "and", AndFn)
            t.AddPairwise(k, "or", OrFn)
            t.AddPairwise(k, "xor", XorFn)
            ' Not with y: x == comparable (y is ignored, as in libnd4j)
            t.AddPairwise(k, "not", Function(x, y, a) NF.FromBoolean(x = OpArgs.Get(a, 0, 0.0)))
            t.AddPairwise(k, "eps", EpsilonFn)
        End Sub

        ' ---------------------------------------------------------------- broadcast

        Private Shared Sub RegisterBroadcast(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.Broadcast
            t.AddPairwise(k, "broadcastadd", AddFn)
            t.AddPairwise(k, "broadcastsub", SubtractFn)
            t.AddPairwise(k, "broadcastmul", MultiplyFn)
            t.AddPairwise(k, "broadcastdiv", DivideFn)
            t.AddPairwise(k, "broadcastrdiv", ReverseDivideFn)
            t.AddPairwise(k, "broadcastrsub", ReverseSubtractFn)
            t.AddPairwise(k, "broadcastcopy", CopyYFn)
            t.AddPairwise(k, "broadcast_max", MaxFn)
            t.AddPairwise(k, "broadcast_min", MinFn)
            t.AddPairwise(k, "broadcast_amax", AMaxFn)
            t.AddPairwise(k, "broadcast_amin", AMinFn)

            Dim b As ManagedOpKind = ManagedOpKind.BroadcastBool
            t.AddPairwise(b, "broadcast_equalto", EqualFn)
            t.AddPairwise(b, "broadcast_notequal", NotEqualFn)
            t.AddPairwise(b, "broadcast_greaterthan", GreaterFn)
            t.AddPairwise(b, "broadcast_greaterthanorequal", GreaterOrEqualFn)
            t.AddPairwise(b, "broadcast_lessthan", LessFn)
            t.AddPairwise(b, "broadcast_lessthanorequal", LessOrEqualFn)
        End Sub

        ' ---------------------------------------------------------------- broadcastable custom ops

        Private Shared Sub RegisterCustom(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.Pairwise
            t.AddCustomPairwise(k, "add", AddFn)
            t.AddCustomPairwise(k, "subtract", SubtractFn)
            t.AddCustomPairwise(k, "multiply", MultiplyFn)
            t.AddCustomPairwise(k, "divide", DivideFn)
            t.AddCustomPairwise(k, "realdiv", DivideFn)
            t.AddCustomPairwise(k, "reversedivide", ReverseDivideFn)
            t.AddCustomPairwise(k, "reversesubtract", ReverseSubtractFn)
            t.AddCustomPairwise(k, "maximum", MaxFn)
            t.AddCustomPairwise(k, "minimum", MinFn)
            t.AddCustomPairwise(k, "Pow", PowFn)
            ' assign: z = in1 broadcast to in0's shape
            t.AddCustomPairwise(k, "assign", CopyYFn)
            ' FloorDiv: floor(x / y)
            t.AddCustomPairwise(k, "floordiv", Function(x, y, a) SysMath.Floor(x / y))
            ' FloorMod: Python-style modulo, the sign of y
            t.AddCustomPairwise(k, "floormod", Function(x, y, a) NF.FloorMod(x, y))
            ' Mod: static_cast<int>(x) % static_cast<int>(y), i.e. fmod of the truncated operands
            t.AddCustomPairwise(k, "mod", Function(x, y, a) SysMath.Truncate(x) Mod SysMath.Truncate(y))
            ' TruncateDiv: static_cast<int>(x) / static_cast<int>(y) (C integer division truncates)
            t.AddCustomPairwise(k, "truncatediv", Function(x, y, a) SysMath.Truncate(SysMath.Truncate(x) / SysMath.Truncate(y)))
            ' SquaredSubtract: (x - y)^2
            t.AddCustomPairwise(k, "squaredsubtract", Function(x, y, a)
                                                          Dim d As Double = x - y
                                                          Return d * d
                                                      End Function)
            ' tf_atan2(y, x): inputs in TensorFlow's order, z = atan2(in0, in1). libnd4j applies simdOps::Atan2
            ' (op(d1, d2) = atan2(d2, d1)) as in1.applyTrueBroadcast(Atan2, in0), which is the same thing.
            t.AddCustomPairwise(k, "tf_atan2", Function(y0, x1, a) NF.Atan2(y0, x1))
            ' prelu: x < 0 ? alpha * x : x, alpha the second input (broadcast)
            t.AddCustomPairwise(k, "prelu", Function(x, alpha, a) If(x < 0.0, alpha * x, x))

            Dim b As ManagedOpKind = ManagedOpKind.PairwiseBool
            t.AddCustomPairwise(b, "equals", EqualFn)
            t.AddCustomPairwise(b, "not_equals", NotEqualFn)
            t.AddCustomPairwise(b, "greater", GreaterFn)
            t.AddCustomPairwise(b, "greater_equal", GreaterOrEqualFn)
            t.AddCustomPairwise(b, "less", LessFn)
            t.AddCustomPairwise(b, "less_equal", LessOrEqualFn)
            ' boolean_and/or/xor: C bool conversion of both inputs
            t.AddCustomPairwise(b, "boolean_and", Function(x, y, a) NF.FromBoolean(NF.IsTrue(x) AndAlso NF.IsTrue(y)))
            t.AddCustomPairwise(b, "boolean_or", Function(x, y, a) NF.FromBoolean(NF.IsTrue(x) OrElse NF.IsTrue(y)))
            t.AddCustomPairwise(b, "boolean_xor", Function(x, y, a) NF.FromBoolean(NF.IsTrue(x) <> NF.IsTrue(y)))
        End Sub

        ' ---------------------------------------------------------------- activation backprop custom ops: z = f(x, dLdz)

        Private Shared Sub RegisterCustomBackprop(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.Pairwise
            t.AddCustomPairwise(k, "sigmoid_bp", Function(x, g, a)
                                                     Dim s As Double = NF.Sigmoid(x)
                                                     Return g * s * (1.0 - s)
                                                 End Function)
            t.AddCustomPairwise(k, "tanh_bp", Function(x, g, a) g * TransformMath.TanhDerivative(x))
            t.AddCustomPairwise(k, "softplus_bp", Function(x, g, a) g * NF.Sigmoid(x))
            t.AddCustomPairwise(k, "softsign_bp", Function(x, g, a)
                                                      Dim d As Double = 1.0 + SysMath.Abs(x)
                                                      Return g / (d * d)
                                                  End Function)
            t.AddCustomPairwise(k, "cube_bp", Function(x, g, a) g * 3.0 * x * x)
            t.AddCustomPairwise(k, "selu_bp", Function(x, g, a) g * If(x > 0.0, NF.SeluLambda, NF.SeluAlpha * NF.SeluLambda * SysMath.Exp(x)))
            t.AddCustomPairwise(k, "rationaltanh_bp", Function(x, g, a) g * TransformMath.RationalTanhDerivative(x))
            t.AddCustomPairwise(k, "rectifiedtanh_bp", Function(x, g, a) If(x > 0.0, g * TransformMath.TanhDerivative(x), 0.0))
            t.AddCustomPairwise(k, "hardsigmoid_bp", Function(x, g, a) If(x < -2.5 OrElse x > 2.5, 0.0, 0.2 * g))
            t.AddCustomPairwise(k, "hardtanh_bp", Function(x, g, a) If(x >= -1.0 AndAlso x <= 1.0, g, 0.0))
            ' elu_bp: x >= 0 ? g : g * alpha * e^x; tArgs {alpha}, default 1
            t.AddCustomPairwise(k, "elu_bp", Function(x, g, a) If(x >= 0.0, g, g * OpArgs.Get(a, 0, 1.0) * SysMath.Exp(x)))
            ' lrelu_bp: x >= 0 ? g : alpha * g; tArgs {alpha}, default 0.01 (LeakyReLU.DEFAULT_ALPHA)
            t.AddCustomPairwise(k, "lrelu_bp", Function(x, g, a) If(x >= 0.0, g, OpArgs.Get(a, 0, 0.01) * g))
            ' relu_bp: x > cutoff ? g : 0; tArgs {cutoff}, default 0
            t.AddCustomPairwise(k, "relu_bp", Function(x, g, a) If(x > OpArgs.Get(a, 0, 0.0), g, 0.0))
            ' relu6_bp: cutoff < x < 6 ? g : 0; tArgs {cutoff}, default 0
            t.AddCustomPairwise(k, "relu6_bp", Function(x, g, a) If(x > OpArgs.Get(a, 0, 0.0) AndAlso x < 6.0, g, 0.0))
            ' thresholdedrelu_bp: x > cutoff ? g : 0; tArgs {cutoff}, default 0
            t.AddCustomPairwise(k, "thresholdedrelu_bp", Function(x, g, a) If(x > OpArgs.Get(a, 0, 0.0), g, 0.0))
        End Sub

    End Class

End Namespace
