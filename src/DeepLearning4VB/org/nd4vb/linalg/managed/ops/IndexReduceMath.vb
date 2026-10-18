' DeepLearning4VB: the comparisons of ND4J's index reductions (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original: the semantics are those of libnd4j's simdOps IndexMax, IndexMin, IndexAbsoluteMax,
' IndexAbsoluteMin, FirstIndex and LastIndex (ops.h); the Java classes are in
' unported/nd4j/.../linalg/api/ops/impl/indexaccum (FirstIndex, LastIndex, and the custom ArgMax/ArgMin/ArgAmax/ArgAmin).

Imports System
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Registers the index reductions. <see cref="IndexReduceDefinition.Prefer"/> is called as
    ''' <c>Prefer(candidateValue, candidateIndex, bestValue, bestIndex, extraArgs)</c>.
    ''' <list type="bullet">
    ''' <item><c>argmax</c>/<c>imax</c>: the largest value; <c>argmin</c>/<c>imin</c>: the smallest; <c>argamax</c>/<c>iamax</c> and
    ''' <c>argamin</c>/<c>iamin</c>: the largest / smallest absolute value. Ties keep the smaller index. NaN never wins (a
    ''' comparison with NaN is false), so an all-NaN input, like an all -Infinity input to argmax, gives index 0
    ''' (<see cref="IndexReduceDefinition.EmptyIndex"/>), as libnd4j does.</item>
    ''' <item><c>first_index</c>/<c>last_index</c>: the smallest / largest index whose value matches the condition in
    ''' extraArgs {compare, eps, mode}; -1 when none matches.</item>
    ''' </list>
    ''' A best index below 0 means "nothing chosen yet", so a kernel may start from (StartingValue, -1) and map a final -1
    ''' to EmptyIndex, or start from (StartingValue, EmptyIndex); both give the same results.
    ''' </summary>
    Friend NotInheritable Class IndexReduceMath

        Private Sub New()
        End Sub

        Private Shared Function Define(ByVal name As String, ByVal start As Double, ByVal prefer As Func(Of Double, Long, Double, Long, Double(), Boolean),
                                       ByVal emptyIndex As Long) As IndexReduceDefinition
            Return New IndexReduceDefinition With {
                .Name = name,
                .StartingValue = Function(a) start,
                .Prefer = prefer,
                .EmptyIndex = emptyIndex
            }
        End Function

        ''' <summary>Strictly better wins; an equal value wins only from a smaller index (merging partials of a parallel scan).</summary>
        Private Shared Function Better(ByVal candidateBeats As Boolean, ByVal tie As Boolean, ByVal candidateIndex As Long, ByVal bestIndex As Long) As Boolean
            If candidateBeats Then Return True
            Return tie AndAlso bestIndex >= 0 AndAlso candidateIndex < bestIndex
        End Function

        Friend Shared ReadOnly MaxPrefer As Func(Of Double, Long, Double, Long, Double(), Boolean) =
            Function(c, ci, b, bi, a) Better(c > b, c = b, ci, bi)
        Friend Shared ReadOnly MinPrefer As Func(Of Double, Long, Double, Long, Double(), Boolean) =
            Function(c, ci, b, bi, a) Better(c < b, c = b, ci, bi)
        Friend Shared ReadOnly AbsMaxPrefer As Func(Of Double, Long, Double, Long, Double(), Boolean) =
            Function(c, ci, b, bi, a) Better(SysMath.Abs(c) > SysMath.Abs(b), SysMath.Abs(c) = SysMath.Abs(b), ci, bi)
        Friend Shared ReadOnly AbsMinPrefer As Func(Of Double, Long, Double, Long, Double(), Boolean) =
            Function(c, ci, b, bi, a) Better(SysMath.Abs(c) < SysMath.Abs(b), SysMath.Abs(c) = SysMath.Abs(b), ci, bi)

        Friend Shared Sub Register(ByVal t As OpTable)
            ' legacy libnd4j names (the Java IMax/IMin/IAMax/IAMin classes were replaced by the custom ArgMax family)
            Dim imax As IndexReduceDefinition = Define("imax", Double.NegativeInfinity, MaxPrefer, 0)
            Dim imin As IndexReduceDefinition = Define("imin", Double.PositiveInfinity, MinPrefer, 0)
            Dim iamax As IndexReduceDefinition = Define("iamax", 0.0, AbsMaxPrefer, 0)
            Dim iamin As IndexReduceDefinition = Define("iamin", Double.PositiveInfinity, AbsMinPrefer, 0)
            t.AddIndexReduce(imax)
            t.AddIndexReduce(imin)
            t.AddIndexReduce(iamax)
            t.AddIndexReduce(iamin)

            ' FirstIndex / LastIndex: extraArgs {compare, eps, mode}
            t.AddIndexReduce(Define("first_index", Double.NegativeInfinity,
                                    Function(c, ci, b, bi, a) ConditionMath.MatchesArgs(c, a, "first_index") AndAlso (bi < 0 OrElse ci < bi), -1))
            t.AddIndexReduce(Define("last_index", Double.NegativeInfinity,
                                    Function(c, ci, b, bi, a) ConditionMath.MatchesArgs(c, a, "last_index") AndAlso (bi < 0 OrElse ci > bi), -1))

            ' the custom ops of the Java API (indexaccum/custom), same comparisons
            t.AddCustomIndexReduce(Define("argmax", Double.NegativeInfinity, MaxPrefer, 0))
            t.AddCustomIndexReduce(Define("argmin", Double.PositiveInfinity, MinPrefer, 0))
            t.AddCustomIndexReduce(Define("argamax", 0.0, AbsMaxPrefer, 0))
            t.AddCustomIndexReduce(Define("argamin", Double.PositiveInfinity, AbsMinPrefer, 0))
        End Sub

    End Class

End Namespace
