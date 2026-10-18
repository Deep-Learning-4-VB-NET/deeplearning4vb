' DeepLearning4VB: shared contracts between the managed op math registry (org.nd4vb.linalg.managed.ops)
' and the managed kernels (org.nd4vb.linalg.managed.kernels). See docs/PHASE2_DESIGN.md.

Imports System

Namespace org.nd4vb.linalg.managed

    ''' <summary>Element-wise transform: z = f(x). <paramref name="extraArgs"/> are the op's extra arguments as doubles (may be empty, never Nothing).</summary>
    Public Delegate Function TransformFunction(ByVal x As Double, ByVal extraArgs As Double()) As Double

    ''' <summary>Element-wise binary function: z = f(x, y) for pairwise and broadcast ops, z = f(x, scalar) for scalar ops.</summary>
    Public Delegate Function PairwiseFunction(ByVal x As Double, ByVal y As Double, ByVal extraArgs As Double()) As Double

    ''' <summary>
    ''' How a reduction folds values, mirroring libnd4j's reduce op definitions:
    ''' acc = StartingValue; for each x: acc = Update(acc, Map(x)); partial results combine with Merge;
    ''' the result is PostProcess(acc, n). For a reduce3 op (two inputs) Map receives both values.
    ''' </summary>
    Public NotInheritable Class ReduceDefinition
        ''' <summary>The op name (Java <c>opName()</c>).</summary>
        Public Property Name As String
        ''' <summary>Initial accumulator value (for example 0 for sum, -Infinity for max).</summary>
        Public Property StartingValue As Func(Of Double(), Double)
        ''' <summary>Maps one element before it is accumulated (identity for sum, Abs for norm1, x*x for norm2...).</summary>
        Public Property Map As TransformFunction
        ''' <summary>Maps a pair of elements (reduce3 ops such as cosine similarity or euclidean distance); Nothing for single-input reductions.</summary>
        Public Property MapPair As PairwiseFunction
        ''' <summary>Folds a mapped value into the accumulator.</summary>
        Public Property Update As PairwiseFunction
        ''' <summary>Combines two partial accumulators (parallel reduction).</summary>
        Public Property Merge As PairwiseFunction
        ''' <summary>Final value from the accumulator and the number of elements reduced (mean divides by n, norm2 takes the square root).</summary>
        Public Property PostProcess As Func(Of Double, Long, Double(), Double)
        ''' <summary>
        ''' Optional per-reduction state (libnd4j's extra-params accumulators, used by cosine similarity, cosine distance
        ''' and Jaccard distance). When set, the kernel calls it once for every reduction it runs (every TAD, and every
        ''' parallel partial of a TAD) and passes the returned array as <c>extraArgs</c> to <c>Map</c>/<c>MapPair</c>,
        ''' <c>Update</c> and <c>PostProcess</c> of that reduction, so delegates may write into it. Nothing means the op
        ''' is stateless and every delegate receives the op's shared, read-only extra arguments.
        ''' </summary>
        Public Property CreateState As Func(Of Double(), Double())
        ''' <summary>
        ''' Optional, set together with <see cref="CreateState"/>: folds a partial reduction's state (second argument) into
        ''' the total's (first argument). The kernel calls it whenever <see cref="Merge"/> combines two partials, and passes
        ''' the merged state to <see cref="PostProcess"/>.
        ''' </summary>
        Public Property MergeState As Action(Of Double(), Double())
    End Class

    ''' <summary>
    ''' An index reduction (argmax, argmin, first index matching a condition...): the kernel visits
    ''' (value, index) pairs and keeps the pair the comparison prefers. Ties keep the first index, as libnd4j does.
    ''' </summary>
    Public NotInheritable Class IndexReduceDefinition
        Public Property Name As String
        ''' <summary>Value of the initial "best" candidate (for example -Infinity for argmax).</summary>
        Public Property StartingValue As Func(Of Double(), Double)
        ''' <summary>True when <c>candidate</c> (with its index) should replace <c>best</c>.</summary>
        Public Property Prefer As Func(Of Double, Long, Double, Long, Double(), Boolean)
        ''' <summary>Index returned when no element qualifies (-1 for the condition-based ops, 0 otherwise).</summary>
        Public Property EmptyIndex As Long
    End Class

    ''' <summary>The op families the managed executioner dispatches on (the legacy op types of libnd4j).</summary>
    Public Enum ManagedOpKind
        TransformSame
        TransformFloat
        TransformStrict
        TransformBool
        TransformAny
        Scalar
        ScalarBool
        Pairwise
        PairwiseBool
        Broadcast
        BroadcastBool
        ReduceFloat
        ReduceSame
        ReduceLong
        ReduceBool
        Reduce3
        IndexReduce
        SummaryStats
        Random
        Custom
    End Enum

End Namespace
