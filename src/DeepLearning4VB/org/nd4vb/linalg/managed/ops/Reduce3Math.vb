' DeepLearning4VB: the folds of ND4J's reduce3 (two-input) ops (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original: the semantics are those of libnd4j's simdOps reduce3 ops (ops.h); the Java classes are in
' unported/nd4j/.../linalg/api/ops/impl/reduce3.

Imports System
Imports SysMath = System.Math
Imports NF = DeepLearning4VB.org.nd4vb.linalg.managed.ops.NumericFunctions

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Registers the reduce3 ops (distances and similarities of two arrays). Their definitions set
    ''' <see cref="ReduceDefinition.MapPair"/> instead of <see cref="ReduceDefinition.Map"/>.
    ''' <para>
    ''' As in libnd4j, <c>cosinesimilarity</c>, <c>cosinedistance</c> and <c>jaccarddistance</c> keep two running sums in the
    ''' first two slots of their extra arguments (libnd4j's <c>extraParamsLen = 2</c>): <c>MapPair</c> adds to them and
    ''' <c>PostProcess</c> reads them. A kernel must give each reduction its own state array
    ''' (<see cref="ManagedOpMath.CreateReduceState"/>) and fold the slots of parallel partials together
    ''' (<see cref="ManagedOpMath.MergeReduceState"/>). The Java ops pass <c>{0, 0}</c> for these slots.
    ''' </para>
    ''' <list type="bullet">
    ''' <item><c>manhattan</c> Σ|x - y|; <c>euclidean</c> √Σ(x - y)²; <c>dot</c> Σxy.</item>
    ''' <item><c>cosinesimilarity</c> Σxy / (√Σx² · √Σy²); <c>cosinedistance</c> 1 minus that. NaN for a zero vector (0/0).</item>
    ''' <item><c>jaccarddistance</c> 1 - Σmin(x, y) / Σmax(x, y) (NaN when Σmax is 0).</item>
    ''' <item><c>hammingdistance</c> the fraction of positions where x ≠ y (count / n).</item>
    ''' <item><c>equals_with_eps</c> the number of positions where x and y differ by more than eps
    ''' (<see cref="NumericFunctions.EqualsWithEps"/>); eps is |extraArgs(2)| (the Java op passes {0, 0, eps}),
    ''' |extraArgs(0)| when only one argument is given, else Nd4j.EPS_THRESHOLD.</item>
    ''' </list>
    ''' </summary>
    Friend NotInheritable Class Reduce3Math

        Private Sub New()
        End Sub

        Private Shared ReadOnly SumFn As PairwiseFunction = Function(acc, v, a) acc + v
        Private Shared ReadOnly NoPost As Func(Of Double, Long, Double(), Double) = Function(acc, n, a) acc

        Private Shared Function Define(ByVal name As String, ByVal mapPair As PairwiseFunction,
                                       ByVal post As Func(Of Double, Long, Double(), Double)) As ReduceDefinition
            Return New ReduceDefinition With {
                .Name = name,
                .StartingValue = Function(a) 0.0,
                .Map = Nothing,
                .MapPair = mapPair,
                .Update = SumFn,
                .Merge = SumFn,
                .PostProcess = post,
                .CreateState = Function(args) ManagedOpMath.CreateReduceState(name, args),
                .MergeState = Sub(total, partialState) ManagedOpMath.MergeReduceState(name, total, partialState)
            }
        End Function

        Private Shared Sub RequireSlots(ByVal state As Double(), ByVal opName As String)
            If state Is Nothing OrElse state.Length < 2 Then
                Throw New ArgumentException("Reduce3 op '" & opName & "' keeps two running sums in its state; create it with ManagedOpMath.CreateReduceState", NameOf(state))
            End If
        End Sub

        Friend Shared Function EpsilonOf(ByVal state As Double()) As Double
            If state IsNot Nothing Then
                If state.Length >= 3 Then Return SysMath.Abs(state(2))
                If state.Length = 1 Then Return SysMath.Abs(state(0))
            End If
            Return NF.EpsThreshold
        End Function

        Friend Shared Sub Register(ByVal t As OpTable)
            Dim k As ManagedOpKind = ManagedOpKind.Reduce3

            t.AddReduce(k, Define("manhattan", Function(x, y, a) SysMath.Abs(x - y), NoPost))
            t.AddReduce(k, Define("euclidean", Function(x, y, a)
                                                   Dim d As Double = x - y
                                                   Return d * d
                                               End Function, Function(acc, n, a) SysMath.Sqrt(acc)))
            t.AddReduce(k, Define("dot", Function(x, y, a) x * y, NoPost))

            t.AddReduce(k, Define("cosinesimilarity", Function(x, y, st)
                                                          RequireSlots(st, "cosinesimilarity")
                                                          st(0) += x * x
                                                          st(1) += y * y
                                                          Return x * y
                                                      End Function,
                                  Function(acc, n, st)
                                      RequireSlots(st, "cosinesimilarity")
                                      Return acc / (SysMath.Sqrt(st(0)) * SysMath.Sqrt(st(1)))
                                  End Function))
            t.AddReduce(k, Define("cosinedistance", Function(x, y, st)
                                                        RequireSlots(st, "cosinedistance")
                                                        st(0) += x * x
                                                        st(1) += y * y
                                                        Return x * y
                                                    End Function,
                                  Function(acc, n, st)
                                      RequireSlots(st, "cosinedistance")
                                      Return 1.0 - acc / (SysMath.Sqrt(st(0)) * SysMath.Sqrt(st(1)))
                                  End Function))
            t.AddReduce(k, Define("jaccarddistance", Function(x, y, st)
                                                         RequireSlots(st, "jaccarddistance")
                                                         st(0) += NF.NdMin(x, y)
                                                         st(1) += NF.NdMax(x, y)
                                                         Return 0.0
                                                     End Function,
                                  Function(acc, n, st)
                                      RequireSlots(st, "jaccarddistance")
                                      Return 1.0 - st(0) / st(1)
                                  End Function))
            t.AccumulatorSlots("cosinesimilarity") = 2
            t.AccumulatorSlots("cosinedistance") = 2
            t.AccumulatorSlots("jaccarddistance") = 2

            t.AddReduce(k, Define("hammingdistance", Function(x, y, a) NF.FromBoolean(x <> y), Function(acc, n, a) acc / n))
            t.AddReduce(k, Define("equals_with_eps", Function(x, y, a) NF.FromBoolean(Not NF.EqualsWithEps(x, y, EpsilonOf(a))), NoPost))
        End Sub

    End Class

End Namespace
