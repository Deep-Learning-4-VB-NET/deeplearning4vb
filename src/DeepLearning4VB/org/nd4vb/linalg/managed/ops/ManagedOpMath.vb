' DeepLearning4VB: the op math registry of the managed ND4VB engine (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original. It replaces the per-op math of libnd4j's legacy_ops.h / ops.h; the op inventory is
' docs/phase2/op-inventory.md.

Imports System
Imports System.Collections.Generic

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' The math of ND4J's legacy ops, keyed by the Java <c>opName()</c>, for the managed op executioner and kernels.
    ''' </summary>
    ''' <remarks>
    ''' <para>Lookups and the op kinds they serve:</para>
    ''' <list type="bullet">
    ''' <item><see cref="Transform"/>: transform same, float, strict, bool and any (z = f(x, extraArgs)).</item>
    ''' <item><see cref="Pairwise"/>: pairwise, pairwise bool, broadcast and broadcast bool (z = f(x, y, extraArgs), y the
    ''' second operand or the broadcast value). The transform-op classes that run as pairwise ops when given a y array
    ''' (<c>copy</c>, <c>set</c>, <c>old_assign</c>, <c>cas</c>, <c>not</c>) are in both tables.</item>
    ''' <item><see cref="Scalar"/>: scalar and scalar bool (z = f(x, scalar, extraArgs)).</item>
    ''' <item><see cref="Reduce"/>: reduce float, same, long and bool, and reduce3 (whose definitions set
    ''' <see cref="ReduceDefinition.MapPair"/>). Reduce3 ops with accumulator slots (<see cref="AccumulatorSlotCount"/>)
    ''' need a private state array per reduction: see <see cref="CreateReduceState"/>.</item>
    ''' <item><see cref="IndexReduce"/>: index reductions. <see cref="IndexReduceDefinition.Prefer"/> is called as
    ''' <c>Prefer(candidateValue, candidateIndex, bestValue, bestIndex, extraArgs)</c>; it treats a negative best index as
    ''' "nothing chosen yet" (map a final -1 to <see cref="IndexReduceDefinition.EmptyIndex"/>) and decides by index
    ''' where values tie (argmax family: the smaller index; <c>first_index</c>/<c>last_index</c>: the smaller/larger one),
    ''' so partial results of a parallel scan can be merged in any order by offering one partial as the candidate to
    ''' the other. Candidates with a negative index (an empty partial) must not be offered.</item>
    ''' <item><see cref="SummaryStats"/>: variance and standard deviation from Welford moments (the kernel accumulates
    ''' them, see <see cref="WelfordMoments"/>).</item>
    ''' </list>
    ''' <para>Extra arguments arrive as doubles in the order the Java op class fills <c>extraArgs</c> (for custom ops,
    ''' their <c>tArguments</c>); missing optional arguments take the Java defaults, missing required ones throw
    ''' <see cref="ArgumentException"/>. Booleans are returned as 1.0/0.0; all math is <c>Double</c> and the executioner
    ''' rounds to the target type.</para>
    ''' <para>Besides the legacy ops, the element-wise math of some custom ops (<c>DynamicCustomOp</c>) is registered in
    ''' the same tables under their custom names (for example <c>add</c>, <c>subtract</c>, <c>equals</c>, <c>elu</c>,
    ''' <c>argmax</c>, <c>reduce_logsumexp</c>): <see cref="Names"/> lists them under <see cref="ManagedOpKind.Custom"/>
    ''' and <see cref="TryGetCustomFamily"/> tells which lookup holds each one.</para>
    ''' <para>Lookups by an unknown name throw <see cref="NotSupportedException"/>; the <c>TryGet</c> forms return False.
    ''' The registry is immutable and thread-safe; the delegates are pure functions, except that reduce3 delegates write
    ''' to the state array they are given.</para>
    ''' </remarks>
    Public NotInheritable Class ManagedOpMath

        Private Sub New()
        End Sub

        Private Shared ReadOnly Tables As OpTable = OpTable.Build()

        ' ---------------------------------------------------------------- transforms

        ''' <summary>The function of a transform op (same, float, strict, bool or any).</summary>
        ''' <exception cref="NotSupportedException">No transform with that name is registered.</exception>
        Public Shared Function Transform(ByVal opName As String) As TransformFunction
            Dim fn As TransformFunction = Nothing
            If TryGetTransform(opName, fn) Then Return fn
            Throw Unknown("transform", opName)
        End Function

        ''' <summary>Looks up a transform op; False when the name is unknown.</summary>
        Public Shared Function TryGetTransform(ByVal opName As String, ByRef fn As TransformFunction) As Boolean
            fn = Nothing
            If opName Is Nothing Then Return False
            Return Tables.TransformTable.TryGetValue(opName, fn)
        End Function

        ' ---------------------------------------------------------------- pairwise and broadcast

        ''' <summary>The function of a pairwise, pairwise bool, broadcast or broadcast bool op: z = f(x, y).</summary>
        ''' <exception cref="NotSupportedException">No pairwise op with that name is registered.</exception>
        Public Shared Function Pairwise(ByVal opName As String) As PairwiseFunction
            Dim fn As PairwiseFunction = Nothing
            If TryGetPairwise(opName, fn) Then Return fn
            Throw Unknown("pairwise", opName)
        End Function

        ''' <summary>Looks up a pairwise or broadcast op; False when the name is unknown.</summary>
        Public Shared Function TryGetPairwise(ByVal opName As String, ByRef fn As PairwiseFunction) As Boolean
            fn = Nothing
            If opName Is Nothing Then Return False
            Return Tables.PairwiseTable.TryGetValue(opName, fn)
        End Function

        ' ---------------------------------------------------------------- scalar

        ''' <summary>The function of a scalar or scalar bool op: z = f(x, scalar).</summary>
        ''' <exception cref="NotSupportedException">No scalar op with that name is registered.</exception>
        Public Shared Function Scalar(ByVal opName As String) As PairwiseFunction
            Dim fn As PairwiseFunction = Nothing
            If TryGetScalar(opName, fn) Then Return fn
            Throw Unknown("scalar", opName)
        End Function

        ''' <summary>Looks up a scalar op; False when the name is unknown.</summary>
        Public Shared Function TryGetScalar(ByVal opName As String, ByRef fn As PairwiseFunction) As Boolean
            fn = Nothing
            If opName Is Nothing Then Return False
            Return Tables.ScalarTable.TryGetValue(opName, fn)
        End Function

        ' ---------------------------------------------------------------- reductions

        ''' <summary>The definition of a reduction (float, same, long, bool) or reduce3 op.</summary>
        ''' <exception cref="NotSupportedException">No reduction with that name is registered.</exception>
        Public Shared Function Reduce(ByVal opName As String) As ReduceDefinition
            Dim definition As ReduceDefinition = Nothing
            If TryGetReduce(opName, definition) Then Return definition
            Throw Unknown("reduce", opName)
        End Function

        ''' <summary>Looks up a reduction or reduce3 op; False when the name is unknown.</summary>
        Public Shared Function TryGetReduce(ByVal opName As String, ByRef definition As ReduceDefinition) As Boolean
            definition = Nothing
            If opName Is Nothing Then Return False
            Return Tables.ReduceTable.TryGetValue(opName, definition)
        End Function

        ''' <summary>
        ''' libnd4j's <c>extraParamsLen</c>: how many leading slots of the reduction state a reduce3 op uses as running
        ''' sums (2 for <c>cosinesimilarity</c>, <c>cosinedistance</c> and <c>jaccarddistance</c>, 0 for everything else).
        ''' </summary>
        Public Shared Function AccumulatorSlotCount(ByVal opName As String) As Integer
            Dim n As Integer
            If opName IsNot Nothing AndAlso Tables.AccumulatorSlots.TryGetValue(opName, n) Then Return n
            Return 0
        End Function

        ''' <summary>
        ''' A fresh state array for one reduction (one TAD, or one parallel partial of it): a copy of the op's extra
        ''' arguments, at least <see cref="AccumulatorSlotCount"/> long, with the accumulator slots zeroed. Pass it as
        ''' <c>extraArgs</c> to every delegate of that reduction, including <c>PostProcess</c>. Reductions without
        ''' accumulator slots may use the op's extra arguments directly; using a state array is harmless for them.
        ''' </summary>
        Public Shared Function CreateReduceState(ByVal opName As String, ByVal extraArgs As Double()) As Double()
            Dim slots As Integer = AccumulatorSlotCount(opName)
            Dim source As Double() = If(extraArgs, Array.Empty(Of Double)())
            Dim state(Global.System.Math.Max(source.Length, slots) - 1) As Double
            Array.Copy(source, state, source.Length)
            For i As Integer = 0 To slots - 1
                state(i) = 0.0
            Next
            Return state
        End Function

        ''' <summary>
        ''' Folds the accumulator slots of a partial reduction's state into the total's (libnd4j's
        ''' <c>aggregateExtraParams</c>, a sum for every reduce3 op). Call it together with
        ''' <see cref="ReduceDefinition.Merge"/> when combining partial results.
        ''' </summary>
        Public Shared Sub MergeReduceState(ByVal opName As String, ByVal total As Double(), ByVal partialState As Double())
            Dim slots As Integer = AccumulatorSlotCount(opName)
            For i As Integer = 0 To slots - 1
                total(i) += partialState(i)
            Next
        End Sub

        ' ---------------------------------------------------------------- index reductions

        ''' <summary>The definition of an index reduction (argmax, argmin, first/last index matching a condition...).</summary>
        ''' <exception cref="NotSupportedException">No index reduction with that name is registered.</exception>
        Public Shared Function IndexReduce(ByVal opName As String) As IndexReduceDefinition
            Dim definition As IndexReduceDefinition = Nothing
            If TryGetIndexReduce(opName, definition) Then Return definition
            Throw Unknown("index reduce", opName)
        End Function

        ''' <summary>Looks up an index reduction; False when the name is unknown.</summary>
        Public Shared Function TryGetIndexReduce(ByVal opName As String, ByRef definition As IndexReduceDefinition) As Boolean
            definition = Nothing
            If opName Is Nothing Then Return False
            Return Tables.IndexReduceTable.TryGetValue(opName, definition)
        End Function

        ' ---------------------------------------------------------------- summary statistics

        ''' <summary>The definition of a summary-statistics op (<c>var</c>, <c>std</c>).</summary>
        ''' <exception cref="NotSupportedException">No summary-statistics op with that name is registered.</exception>
        Public Shared Function SummaryStats(ByVal opName As String) As SummaryStatsDefinition
            Dim definition As SummaryStatsDefinition = Nothing
            If TryGetSummaryStats(opName, definition) Then Return definition
            Throw Unknown("summary stats", opName)
        End Function

        ''' <summary>Looks up a summary-statistics op; False when the name is unknown.</summary>
        Public Shared Function TryGetSummaryStats(ByVal opName As String, ByRef definition As SummaryStatsDefinition) As Boolean
            definition = Nothing
            If opName Is Nothing Then Return False
            Return Tables.SummaryStatsTable.TryGetValue(opName, definition)
        End Function

        ' ---------------------------------------------------------------- inventory

        ''' <summary>True when an op of that kind and name is registered (custom ops: when their element-wise math is).</summary>
        Public Shared Function Supports(ByVal kind As ManagedOpKind, ByVal opName As String) As Boolean
            If opName Is Nothing Then Return False
            Dim names As SortedSet(Of String) = Nothing
            Return Tables.KindNames.TryGetValue(kind, names) AndAlso names.Contains(opName)
        End Function

        ''' <summary>The names registered under a kind, in ordinal order (empty for <see cref="ManagedOpKind.Random"/>).</summary>
        Public Shared Function Names(ByVal kind As ManagedOpKind) As String()
            Dim set1 As SortedSet(Of String) = Nothing
            If Not Tables.KindNames.TryGetValue(kind, set1) Then Return Array.Empty(Of String)()
            Dim result(set1.Count - 1) As String
            set1.CopyTo(result)
            Return result
        End Function

        ''' <summary>
        ''' For a custom op whose math is registered here, the family of the lookup that holds it: a transform kind
        ''' (<see cref="Transform"/>), <see cref="ManagedOpKind.Pairwise"/> or <see cref="ManagedOpKind.PairwiseBool"/>
        ''' (<see cref="Pairwise"/>, applied with broadcasting to the op's two inputs in order), a reduce kind
        ''' (<see cref="Reduce"/>) or <see cref="ManagedOpKind.IndexReduce"/> (<see cref="IndexReduce"/>).
        ''' </summary>
        Public Shared Function TryGetCustomFamily(ByVal opName As String, ByRef family As ManagedOpKind) As Boolean
            family = ManagedOpKind.Custom
            If opName Is Nothing Then Return False
            Return Tables.CustomFamilies.TryGetValue(opName, family)
        End Function

        Private Shared Function Unknown(ByVal what As String, ByVal opName As String) As NotSupportedException
            Return New NotSupportedException("The managed op math registry has no " & what & " op named '" &
                                             If(opName, "(null)") & "'")
        End Function

    End Class

End Namespace
