' DeepLearning4VB: the folds of ND4J's reduction ops (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original: the semantics are those of libnd4j's simdOps reduce ops (ops.h); the Java classes are in
' unported/nd4j/.../linalg/api/ops/impl/reduce/{floating,same,longer,bool} (and reduce/custom/LogSumExp).

Imports System
Imports SysMath = System.Math
Imports NF = DeepLearning4VB.org.nd4vb.linalg.managed.ops.NumericFunctions

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Registers the single-input reductions as <see cref="ReduceDefinition"/>s:
    ''' acc = StartingValue; acc = Update(acc, Map(x)) per element; partial accumulators combine with Merge;
    ''' result = PostProcess(acc, n). Over an empty input the result is PostProcess(StartingValue, 0): 0 for sums and
    ''' counts, NaN for means (0/0), -Infinity / +Infinity for <c>reduce_max</c> / <c>reduce_min</c>, 1 for
    ''' <c>reduce_prod</c> and <c>all</c>, 0 (false) for <c>any</c>, <c>hasnan</c>, <c>hasinf</c>.
    ''' <list type="bullet">
    ''' <item><c>reduce_max</c>, <c>reduce_min</c>, <c>amax</c>, <c>amin</c>, <c>reduce_normmax</c> propagate NaN. libnd4j folds
    ''' with <c>nd4j_max</c> (<c>a &gt; b ? a : b</c>), whose result for a NaN depends on where it sits and on the
    ''' thread split, so it cannot be reproduced; NaN in, NaN out is the deterministic choice.</item>
    ''' <item><c>entropy</c> (-Σ x ln x), <c>logentropy</c> (ln of that) and <c>shannonentropy</c> (-Σ x log₂ x) give NaN
    ''' for a zero element (0 · -Infinity), as libnd4j does.</item>
    ''' <item><c>any</c>/<c>all</c> test x ≠ 0 per element (NaN counts as true). libnd4j adds/multiplies the raw values and
    ''' compares the result with 0, which agrees for BOOL inputs (the normal use) but not for numeric inputs with mixed
    ''' signs; this follows the documented meaning instead.</item>
    ''' <item><c>match_condition</c> counts the elements matching the condition in extraArgs {compare, eps, mode}.</item>
    ''' <item><c>reduce_logsumexp</c> (a custom op) folds with a stable log-add-exp, so it needs no separate max pass.</item>
    ''' </list>
    ''' </summary>
    Friend NotInheritable Class ReduceMath

        Private Sub New()
        End Sub

        Private Shared ReadOnly SumFn As PairwiseFunction = Function(acc, v, a) acc + v
        Private Shared ReadOnly ProductFn As PairwiseFunction = Function(acc, v, a) acc * v
        Private Shared ReadOnly MaxFn As PairwiseFunction = Function(acc, v, a) NF.NanMax(acc, v)
        Private Shared ReadOnly MinFn As PairwiseFunction = Function(acc, v, a) NF.NanMin(acc, v)
        Private Shared ReadOnly OrFn As PairwiseFunction = Function(acc, v, a) NF.FromBoolean(acc <> 0.0 OrElse v <> 0.0)
        Private Shared ReadOnly AndFn As PairwiseFunction = Function(acc, v, a) NF.FromBoolean(acc <> 0.0 AndAlso v <> 0.0)
        Private Shared ReadOnly IdentityMap As TransformFunction = Function(x, a) x
        Private Shared ReadOnly AbsMap As TransformFunction = Function(x, a) SysMath.Abs(x)
        Private Shared ReadOnly SquareMap As TransformFunction = Function(x, a) x * x
        Private Shared ReadOnly NoPost As Func(Of Double, Long, Double(), Double) = Function(acc, n, a) acc
        Private Shared ReadOnly MeanPost As Func(Of Double, Long, Double(), Double) = Function(acc, n, a) acc / n

        ''' <summary>A definition whose Merge equals its Update (true for every legacy reduction).</summary>
        Friend Shared Function Define(ByVal name As String, ByVal start As Double, ByVal map As TransformFunction,
                                      ByVal update As PairwiseFunction, ByVal post As Func(Of Double, Long, Double(), Double)) As ReduceDefinition
            Return New ReduceDefinition With {
                .Name = name,
                .StartingValue = Function(a) start,
                .Map = map,
                .MapPair = Nothing,
                .Update = update,
                .Merge = update,
                .PostProcess = post
            }
        End Function

        Friend Shared Sub Register(ByVal t As OpTable)
            ' ---- reduce same: the result has the input's type
            Dim s As ManagedOpKind = ManagedOpKind.ReduceSame
            t.AddReduce(s, Define("reduce_sum", 0.0, IdentityMap, SumFn, NoPost))
            t.AddReduce(s, Define("reduce_prod", 1.0, IdentityMap, ProductFn, NoPost))
            t.AddReduce(s, Define("reduce_max", Double.NegativeInfinity, IdentityMap, MaxFn, NoPost))
            t.AddReduce(s, Define("reduce_min", Double.PositiveInfinity, IdentityMap, MinFn, NoPost))
            t.AddReduce(s, Define("asum", 0.0, AbsMap, SumFn, NoPost))
            t.AddReduce(s, Define("amax", 0.0, AbsMap, MaxFn, NoPost))
            t.AddReduce(s, Define("amin", Double.PositiveInfinity, AbsMap, MinFn, NoPost))

            ' ---- reduce float: floating-point result
            Dim f As ManagedOpKind = ManagedOpKind.ReduceFloat
            t.AddReduce(f, Define("reduce_mean", 0.0, IdentityMap, SumFn, MeanPost))
            t.AddReduce(f, Define("amean", 0.0, AbsMap, SumFn, MeanPost))
            t.AddReduce(f, Define("reduce_norm1", 0.0, AbsMap, SumFn, NoPost))
            t.AddReduce(f, Define("reduce_norm2", 0.0, SquareMap, SumFn, Function(acc, n, a) SysMath.Sqrt(acc)))
            t.AddReduce(f, Define("reduce_sqnorm", 0.0, SquareMap, SumFn, NoPost))
            t.AddReduce(f, Define("reduce_normmax", 0.0, AbsMap, MaxFn, NoPost))
            ' Entropy: -sum(x * ln x)
            t.AddReduce(f, Define("entropy", 0.0, Function(x, a) x * SysMath.Log(x), SumFn, Function(acc, n, a) -acc))
            ' LogEntropy: ln(-sum(x * ln x))
            t.AddReduce(f, Define("logentropy", 0.0, Function(x, a) x * SysMath.Log(x), SumFn, Function(acc, n, a) SysMath.Log(-acc)))
            ' ShannonEntropy: -sum(x * log2 x)
            t.AddReduce(f, Define("shannonentropy", 0.0, Function(x, a) x * NF.Log2(x), SumFn, Function(acc, n, a) -acc))

            ' ---- reduce long: counts
            Dim l As ManagedOpKind = ManagedOpKind.ReduceLong
            t.AddReduce(l, Define("countNonZero", 0.0, Function(x, a) NF.FromBoolean(x <> 0.0), SumFn, NoPost))
            t.AddReduce(l, Define("countZero", 0.0, Function(x, a) NF.FromBoolean(x = 0.0), SumFn, NoPost))
            t.AddReduce(l, Define("match_condition", 0.0, Function(x, a) NF.FromBoolean(ConditionMath.MatchesArgs(x, a, "match_condition")), SumFn, NoPost))

            ' ---- reduce bool
            Dim b As ManagedOpKind = ManagedOpKind.ReduceBool
            t.AddReduce(b, Define("any", 0.0, Function(x, a) NF.FromBoolean(NF.IsTrue(x)), OrFn, NoPost))
            t.AddReduce(b, Define("all", 1.0, Function(x, a) NF.FromBoolean(NF.IsTrue(x)), AndFn, NoPost))
            t.AddReduce(b, Define("hasnan", 0.0, Function(x, a) NF.FromBoolean(Double.IsNaN(x)), OrFn, NoPost))
            t.AddReduce(b, Define("hasinf", 0.0, Function(x, a) NF.FromBoolean(Double.IsInfinity(x)), OrFn, NoPost))

            ' ---- custom reductions with a plain fold
            t.AddCustomReduce(ManagedOpKind.ReduceFloat, Define("reduce_logsumexp", Double.NegativeInfinity, IdentityMap,
                                                                Function(acc, v, a) NF.LogAddExp(acc, v), NoPost))
        End Sub

    End Class

End Namespace
