' DeepLearning4VB: reduction kernels (reduce and reduce3) driven by ReduceDefinition.

Imports System
Imports System.Threading.Tasks
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Reductions driven by a <see cref="ReduceDefinition"/>: <c>acc = StartingValue(args)</c>, then
    ''' <c>acc = Update(acc, Map(x, args), args)</c> for every element (<c>MapPair(x, y, args)</c> for
    ''' the two-input reduce3 form), partial accumulators combined with <c>Merge</c>, and the result
    ''' <c>PostProcess(acc, n, args)</c> where n is the number of elements reduced.
    ''' </summary>
    ''' <remarks>
    ''' <para><b>Arguments and per-reduction state.</b> A "reduction" is one result value: the whole array
    ''' for the <c>...ToScalar</c> methods, one tensor along dimensions (TAD) or one TAD pair otherwise.
    ''' When the definition has no <see cref="ReduceDefinition.CreateState"/>, <c>args</c> is the op's
    ''' extra-argument array (never Nothing), shared by every reduction and every thread, and the
    ''' delegates must not modify it. When <c>CreateState</c> is set, every reduction gets its own
    ''' <c>state = CreateState(extraArgs)</c>, and that array is the <c>args</c> of its
    ''' <c>StartingValue</c>, <c>Map</c>/<c>MapPair</c>, <c>Update</c>, <c>Merge</c> and <c>PostProcess</c>
    ''' calls, so the delegates may write into it (libnd4j's extra-params accumulators: cosine similarity,
    ''' cosine distance, Jaccard distance). A reduction that is split into parallel partials creates one
    ''' state per partial; partials are folded in chunk order with
    ''' <c>acc = Merge(acc, partialAcc, totalState)</c> followed by
    ''' <c>MergeState(totalState, partialState)</c> (the first partial's state becomes the total), and
    ''' <c>PostProcess</c> receives the merged state. A stateful definition without
    ''' <see cref="ReduceDefinition.MergeState"/> is never split.</para>
    ''' <para><b>Order and parallelism.</b> Element order is not specified (the kernels walk memory in its
    ''' natural order), which only affects floating-point rounding. Above
    ''' <see cref="KernelOptions.ParallelThreshold"/> elements a single reduction is cut into chunks of
    ''' <see cref="KernelOptions.ReductionChunkSize"/> elements whose partial accumulators are merged in
    ''' chunk order, so results are deterministic; when <c>Merge</c> is Nothing the reduction stays
    ''' sequential. Many small reductions along dimensions run in parallel over the tensors instead, so
    ''' the delegates must be thread-safe (stateful ones only ever see their own state array).</para>
    ''' <para><b>Delegate defaults.</b> <c>StartingValue</c> Nothing means 0, <c>Map</c> Nothing means
    ''' identity, <c>PostProcess</c> Nothing returns the accumulator; <c>Update</c> is required, and
    ''' <c>MapPair</c> for reduce3. An empty input (or empty TADs) gives
    ''' <c>PostProcess(StartingValue(args), 0, args)</c>.</para>
    ''' <para><b>Results.</b> Reductions along dimensions write TAD <c>i</c> (see <see cref="TadPack"/>) to
    ''' the element of z with row-major logical index <c>i</c>; z needs exactly as many elements as there
    ''' are TADs and may have any shape (with or without kept unit dimensions) and any numeric type (the
    ''' value is stored with z's conversion, see <see cref="ManagedStorage.SetDouble"/>; when the inputs and
    ''' z are all integer storages the result wraps around to z's width instead, as integer arithmetic
    ''' does in libnd4j, see <see cref="ElementWiseKernels"/>).</para>
    ''' </remarks>
    Public NotInheritable Class ReduceKernels

        Private Sub New()
        End Sub

#Region "Single input"

        ''' <summary>Reduces all elements of x to one value.</summary>
        ''' <param name="x">The input storage (any numeric type).</param>
        ''' <param name="xOffset">Element offset of x's first element in its storage.</param>
        ''' <param name="xShapeInfo">x's ND4J shape info.</param>
        ''' <param name="definition">The fold.</param>
        ''' <param name="extraArgs">The op's extra arguments (Nothing means none).</param>
        ''' <returns>The post-processed result.</returns>
        Public Shared Function ReduceToScalar(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                              ByVal definition As ReduceDefinition, ByVal extraArgs As Double()) As Double
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            Dim fns As New ReduceFns(definition, False)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim n As Long = ShapeInfoHelper.Length(xShapeInfo)
            If n = 0 Then
                Return fns.EmptyResult(extra)
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {xShapeInfo}, New Long() {xOffset}, True, 0)
            Return fns.Finish(ReduceRange(x, Nothing, plan, plan.Offsets, n, fns, extra, True), n)
        End Function

        ''' <summary>Reduces every tensor of x along <paramref name="dimensions"/> into the matching element of z.</summary>
        ''' <param name="dimensions">The reduced dimensions (negative values count from the end); empty or Nothing reduces everything into one element.</param>
        Public Shared Sub ReduceAlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                ByVal dimensions As Integer(), ByVal definition As ReduceDefinition, ByVal extraArgs As Double())
            ShapeInfoHelper.Validate(xShapeInfo, NameOf(xShapeInfo))
            ReduceAlongDimensions(x, xOffset, xShapeInfo, z, zOffset, zShapeInfo, TadPack.Create(xShapeInfo, dimensions), definition, extraArgs)
        End Sub

        ''' <summary>Reduces every TAD of <paramref name="tad"/> (computed for x's shape info, for example by a TAD cache) into the matching element of z.</summary>
        Public Shared Sub ReduceAlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                ByVal tad As TadPack, ByVal definition As ReduceDefinition, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            If tad Is Nothing Then
                Throw New ArgumentNullException(NameOf(tad))
            End If
            Dim fns As New ReduceFns(definition, False)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim numTads As Long = tad.NumTads
            CheckResultLength(zShapeInfo, numTads)
            If numTads = 0 Then
                Return
            End If
            Dim zOffsets As Long() = ShapeInfoHelper.Offsets(zShapeInfo)
            Dim tadLength As Long = tad.TadLength
            If tadLength = 0 Then
                WriteEmptyResults(z, zOffset, zOffsets, numTads, fns, extra)
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {tad.TadShapeInfo}, New Long() {0L}, True, 0)
            Dim tadOffsets As Long() = tad.TadOffsets
            Dim wrap As Boolean = KernelUtil.IsIntegerOp(z, x)
            ForEachTad(numTads, tadLength,
                Sub(t As Integer, parallelInside As Boolean)
                    Dim r As PartialReduction = ReduceRange(x, Nothing, plan, New Long() {xOffset + tadOffsets(t)}, tadLength, fns, extra, parallelInside)
                    KernelUtil.StoreResult(z, zOffset + zOffsets(t), fns.Finish(r, tadLength), wrap)
                End Sub)
        End Sub

#End Region

#Region "Reduce3 (two inputs)"

        ''' <summary>Reduces the element pairs of x and y (same number of elements, matched by row-major logical index) to one value with <c>MapPair</c>.</summary>
        Public Shared Function Reduce3ToScalar(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                               ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(),
                                               ByVal definition As ReduceDefinition, ByVal extraArgs As Double()) As Double
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(y, yOffset, yShapeInfo, "y")
            KernelUtil.CheckSameLength(xShapeInfo, "x", yShapeInfo, "y")
            Dim fns As New ReduceFns(definition, True)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim n As Long = ShapeInfoHelper.Length(xShapeInfo)
            If n = 0 Then
                Return fns.EmptyResult(extra)
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {xShapeInfo, yShapeInfo}, New Long() {xOffset, yOffset}, True, 0)
            Dim xs As ManagedStorage = x
            Dim ys As ManagedStorage = y
            If plan Is Nothing Then
                Dim xo As Long, xi As Long() = Nothing, yo As Long, yi As Long() = Nothing
                KernelUtil.Densify(x, xOffset, xShapeInfo, xs, xo, xi)
                KernelUtil.Densify(y, yOffset, yShapeInfo, ys, yo, yi)
                plan = StridedPlan.CreateAligned(New Long()() {xi, yi}, New Long() {xo, yo}, True, 0)
            End If
            Return fns.Finish(ReduceRange(xs, ys, plan, plan.Offsets, n, fns, extra, True), n)
        End Function

        ''' <summary>
        ''' Reduce3 along dimensions: pairs TAD <c>i</c> of x with TAD <c>i</c> of y (same TAD shape and
        ''' count) and writes the result to element <c>i</c> of z. When y has exactly as many elements as one
        ''' TAD of x, every TAD of x is paired with the whole of y instead (y is then read in the TAD's
        ''' row-major logical order).
        ''' </summary>
        Public Shared Sub Reduce3AlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                 ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(),
                                                 ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                 ByVal dimensions As Integer(), ByVal definition As ReduceDefinition, ByVal extraArgs As Double())
            ShapeInfoHelper.Validate(xShapeInfo, NameOf(xShapeInfo))
            ShapeInfoHelper.Validate(yShapeInfo, NameOf(yShapeInfo))
            Dim tadX As TadPack = TadPack.Create(xShapeInfo, dimensions)
            Dim tadY As TadPack = Nothing
            If ShapeInfoHelper.Length(yShapeInfo) <> tadX.TadLength Then
                tadY = TadPack.Create(yShapeInfo, dimensions)
            End If
            Reduce3AlongDimensions(x, xOffset, xShapeInfo, y, yOffset, yShapeInfo, z, zOffset, zShapeInfo, tadX, tadY, definition, extraArgs)
        End Sub

        ''' <summary>
        ''' Reduce3 along dimensions with precomputed TADs: TAD <c>i</c> of <paramref name="tadX"/> pairs with
        ''' TAD <c>i</c> of <paramref name="tadY"/> (same count and TAD shape), or with the whole of y when
        ''' <paramref name="tadY"/> is Nothing (y must then have as many elements as one TAD of x).
        ''' </summary>
        Public Shared Sub Reduce3AlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                 ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(),
                                                 ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                 ByVal tadX As TadPack, ByVal tadY As TadPack, ByVal definition As ReduceDefinition, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(y, yOffset, yShapeInfo, "y")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            If tadX Is Nothing Then
                Throw New ArgumentNullException(NameOf(tadX))
            End If
            Dim fns As New ReduceFns(definition, True)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim numTads As Long = tadX.NumTads
            Dim tadLength As Long = tadX.TadLength
            CheckResultLength(zShapeInfo, numTads)
            Dim tadShape As Long() = ShapeInfoHelper.GetShape(tadX.TadShapeInfo)
            Dim ys As ManagedStorage = y
            Dim yo As Long = yOffset
            Dim yStrides As Long()
            Dim yTadOffsets As Long()
            If tadY Is Nothing Then
                If ShapeInfoHelper.Length(yShapeInfo) <> tadLength Then
                    Throw New ArgumentException("y has " & ShapeInfoHelper.Length(yShapeInfo) & " elements; pairing every tensor of x with the whole of y needs " & tadLength, NameOf(yShapeInfo))
                End If
                yStrides = YStridesForTad(y, yOffset, yShapeInfo, tadShape, ys, yo)
                yTadOffsets = Nothing
            Else
                If tadY.NumTads <> numTads OrElse tadY.TadLength <> tadLength Then
                    Throw New ArgumentException("x and y must have the same tensors along dimensions [" & String.Join(", ", tadX.Dimensions) & "], or y the size of one tensor")
                End If
                yStrides = BroadcastKernels.StridesInShape(tadY.TadShapeInfo, tadShape)
                If yStrides Is Nothing Then
                    Throw New ArgumentException("The tensors of y along the dimensions do not have the shape of those of x")
                End If
                yTadOffsets = tadY.TadOffsets
            End If
            If numTads = 0 Then
                Return
            End If
            Dim zOffsets As Long() = ShapeInfoHelper.Offsets(zShapeInfo)
            If tadLength = 0 Then
                WriteEmptyResults(z, zOffset, zOffsets, numTads, fns, extra)
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.Create(tadShape, New Long()() {ShapeInfoHelper.GetStrides(tadX.TadShapeInfo), yStrides}, New Long() {0L, 0L}, True, 0)
            Dim xTadOffsets As Long() = tadX.TadOffsets
            Dim wrap As Boolean = KernelUtil.IsIntegerOp(z, x, y)
            ForEachTad(numTads, tadLength,
                Sub(t As Integer, parallelInside As Boolean)
                    Dim yBase As Long = If(yTadOffsets Is Nothing, yo, yo + yTadOffsets(t))
                    Dim r As PartialReduction = ReduceRange(x, ys, plan, New Long() {xOffset + xTadOffsets(t), yBase}, tadLength, fns, extra, parallelInside)
                    KernelUtil.StoreResult(z, zOffset + zOffsets(t), fns.Finish(r, tadLength), wrap)
                End Sub)
        End Sub

        ''' <summary>
        ''' Reduce3 over all TAD pairs (libnd4j's <c>execReduce3All</c>): for every TAD <c>i</c> of x and
        ''' every TAD <c>j</c> of y along the same <paramref name="dimensions"/> (same TAD shape), writes the
        ''' result to element <c>i * numTadsY + j</c> (row-major logical index) of z, normally of shape
        ''' [numTadsX, numTadsY].
        ''' </summary>
        Public Shared Sub Reduce3All(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                     ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(),
                                     ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                     ByVal dimensions As Integer(), ByVal definition As ReduceDefinition, ByVal extraArgs As Double())
            ShapeInfoHelper.Validate(xShapeInfo, NameOf(xShapeInfo))
            ShapeInfoHelper.Validate(yShapeInfo, NameOf(yShapeInfo))
            Reduce3All(x, xOffset, xShapeInfo, y, yOffset, yShapeInfo, z, zOffset, zShapeInfo,
                       TadPack.Create(xShapeInfo, dimensions), TadPack.Create(yShapeInfo, dimensions), definition, extraArgs)
        End Sub

        ''' <summary>Reduce3 over all pairs of TADs of <paramref name="tadX"/> and <paramref name="tadY"/> (precomputed for x's and y's shape infos, same TAD shape).</summary>
        Public Shared Sub Reduce3All(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                     ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(),
                                     ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                     ByVal tadX As TadPack, ByVal tadY As TadPack, ByVal definition As ReduceDefinition, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(y, yOffset, yShapeInfo, "y")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            If tadX Is Nothing Then
                Throw New ArgumentNullException(NameOf(tadX))
            End If
            If tadY Is Nothing Then
                Throw New ArgumentNullException(NameOf(tadY))
            End If
            Dim fns As New ReduceFns(definition, True)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim tadLength As Long = tadX.TadLength
            If tadY.TadLength <> tadLength Then
                Throw New ArgumentException("The tensors of x and y along the dimensions must have the same length")
            End If
            Dim tadShape As Long() = ShapeInfoHelper.GetShape(tadX.TadShapeInfo)
            Dim yStrides As Long() = BroadcastKernels.StridesInShape(tadY.TadShapeInfo, tadShape)
            If yStrides Is Nothing Then
                Throw New ArgumentException("The tensors of y along the dimensions do not have the shape of those of x")
            End If
            Dim nx As Long = tadX.NumTads
            Dim ny As Long = tadY.NumTads
            CheckResultLength(zShapeInfo, nx * ny)
            If nx * ny = 0 Then
                Return
            End If
            Dim zOffsets As Long() = ShapeInfoHelper.Offsets(zShapeInfo)
            If tadLength = 0 Then
                WriteEmptyResults(z, zOffset, zOffsets, nx * ny, fns, extra)
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.Create(tadShape, New Long()() {ShapeInfoHelper.GetStrides(tadX.TadShapeInfo), yStrides}, New Long() {0L, 0L}, True, 0)
            Dim xTadOffsets As Long() = tadX.TadOffsets
            Dim yTadOffsets As Long() = tadY.TadOffsets
            Dim wrap As Boolean = KernelUtil.IsIntegerOp(z, x, y)
            ForEachTad(nx * ny, tadLength,
                Sub(p As Integer, parallelInside As Boolean)
                    Dim i As Long = p \ ny
                    Dim j As Long = p - i * ny
                    Dim r As PartialReduction = ReduceRange(x, y, plan, New Long() {xOffset + xTadOffsets(CInt(i)), yOffset + yTadOffsets(CInt(j))}, tadLength, fns, extra, parallelInside)
                    KernelUtil.StoreResult(z, zOffset + zOffsets(p), fns.Finish(r, tadLength), wrap)
                End Sub)
        End Sub

#End Region

#Region "Shared machinery"

        ''' <summary>Throws unless z has <paramref name="expected"/> elements.</summary>
        Friend Shared Sub CheckResultLength(ByVal zShapeInfo As Long(), ByVal expected As Long)
            Dim n As Long = ShapeInfoHelper.Length(zShapeInfo)
            If n <> expected Then
                Throw New ArgumentException("z has " & n & " elements but the reduction produces " & expected, NameOf(zShapeInfo))
            End If
        End Sub

        ''' <summary>Writes the result of an empty reduction to every element of z (each with its own state when the definition is stateful).</summary>
        Private Shared Sub WriteEmptyResults(ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zOffsets As Long(), ByVal count As Long, ByVal fns As ReduceFns, ByVal extra As Double())
            For t As Integer = 0 To CInt(count) - 1
                z.SetDouble(zOffset + zOffsets(t), fns.EmptyResult(extra))
            Next
        End Sub

        ''' <summary>
        ''' Calls <paramref name="body"/>(t, parallelInside) for every TAD t: in parallel over TADs when
        ''' there are many, otherwise sequentially with parallelInside = True so that each large TAD is
        ''' reduced in parallel chunks.
        ''' </summary>
        Friend Shared Sub ForEachTad(ByVal numTads As Long, ByVal tadLength As Long, ByVal body As Action(Of Integer, Boolean))
            Dim total As Long = numTads * SysMath.Max(1L, tadLength)
            Dim workers As Integer = SysMath.Max(1, KernelOptions.MaxDegreeOfParallelism)
            If KernelOptions.ShouldParallelize(total) AndAlso numTads > 1 AndAlso (numTads >= 2L * workers OrElse tadLength < KernelOptions.ParallelThreshold) Then
                Dim chunks As Integer = CInt(SysMath.Min(numTads, CLng(workers) * 4L))
                Parallel.For(0, chunks, KernelOptions.CreateParallelOptions(),
                    Sub(c As Integer)
                        Dim s As Long = numTads * c \ chunks
                        Dim e As Long = numTads * (c + 1) \ chunks
                        For t As Long = s To e - 1
                            body(CInt(t), False)
                        Next
                    End Sub)
                Return
            End If
            For t As Long = 0 To numTads - 1
                body(CInt(t), True)
            Next
        End Sub

        ''' <summary>y's strides over the TAD shape, densifying y when its shape cannot be mapped (ys/yo receive the operand to read).</summary>
        Friend Shared Function YStridesForTad(ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(), ByVal tadShape As Long(),
                                              ByRef ys As ManagedStorage, ByRef yo As Long) As Long()
            Dim strides As Long() = BroadcastKernels.StridesInShape(yShapeInfo, tadShape)
            If strides IsNot Nothing Then
                ys = y
                yo = yOffset
                Return strides
            End If
            Dim yi As Long() = Nothing
            KernelUtil.Densify(y, yOffset, yShapeInfo, ys, yo, yi)
            Return ShapeInfoHelper.CalculateStrides(tadShape, "c"c)
        End Function

        ''' <summary>
        ''' Runs one reduction over the first <paramref name="count"/> elements of the plan from the given
        ''' base offsets (y Nothing: single input; otherwise pairs): sequentially, or in deterministic
        ''' parallel chunks (each with its own state) when allowed, large and mergeable.
        ''' </summary>
        Friend Shared Function ReduceRange(ByVal x As ManagedStorage, ByVal y As ManagedStorage, ByVal plan As StridedPlan, ByVal baseOffsets As Long(),
                                           ByVal count As Long, ByVal fns As ReduceFns, ByVal extra As Double(), ByVal allowParallel As Boolean) As PartialReduction
            Dim chunks As Integer = If(allowParallel AndAlso fns.CanSplit, ReductionChunks.Count(count), 1)
            If chunks <= 1 Then
                Dim state As Double() = fns.NewState(extra)
                Return New PartialReduction(ReduceSegment(x, y, plan, baseOffsets, 0, count, fns, state), state)
            End If
            Dim partials As PartialReduction() = New PartialReduction(chunks - 1) {}
            Parallel.For(0, chunks, KernelOptions.CreateParallelOptions(),
                Sub(c As Integer)
                    Dim s As Long = ReductionChunks.Start(count, c, chunks)
                    Dim e As Long = ReductionChunks.Start(count, c + 1, chunks)
                    Dim st As Double() = fns.NewState(extra)
                    partials(c) = New PartialReduction(ReduceSegment(x, y, plan, baseOffsets, s, e - s, fns, st), st)
                End Sub)
            Dim acc As Double = partials(0).Accumulator
            Dim total As Double() = partials(0).State
            For c As Integer = 1 To chunks - 1
                acc = fns.Merge(acc, partials(c).Accumulator, total)
                If fns.MergeState IsNot Nothing Then
                    fns.MergeState(total, partials(c).State)
                End If
            Next
            Return New PartialReduction(acc, total)
        End Function

        ''' <summary>Sequential reduction of elements [start, start + count) of the plan; the accumulator starts at StartingValue(args).</summary>
        Friend Shared Function ReduceSegment(ByVal x As ManagedStorage, ByVal y As ManagedStorage, ByVal plan As StridedPlan, ByVal baseOffsets As Long(),
                                             ByVal start As Long, ByVal count As Long, ByVal fns As ReduceFns, ByVal args As Double()) As Double
            Dim acc As Double = fns.Start(args)
            If count <= 0 Then
                Return acc
            End If
            If y Is Nothing Then
                Dim map As TransformFunction = fns.Map
                Dim upd As PairwiseFunction = fns.Update
                Dim xf As Single() = x.Floats
                Dim xd As Double() = x.Doubles
                Dim inc As Integer = CInt(plan.InnerStrides(0))
                plan.Run(start, count, baseOffsets,
                    Sub(b As Long(), cnt As Integer)
                        Dim a As Double = acc
                        Dim ea As Double() = args
                        Dim xi As Integer = CInt(b(0))
                        If xf IsNot Nothing Then
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, map(xf(xi), ea), ea)
                                xi += inc
                            Next
                        ElseIf xd IsNot Nothing Then
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, map(xd(xi), ea), ea)
                                xi += inc
                            Next
                        Else
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, map(x.GetDouble(xi), ea), ea)
                                xi += inc
                            Next
                        End If
                        acc = a
                    End Sub)
            Else
                Dim mapPair As PairwiseFunction = fns.MapPair
                Dim upd As PairwiseFunction = fns.Update
                Dim xf As Single() = x.Floats
                Dim yf As Single() = y.Floats
                Dim xd As Double() = x.Doubles
                Dim yd As Double() = y.Doubles
                Dim xInc As Integer = CInt(plan.InnerStrides(0))
                Dim yInc As Integer = CInt(plan.InnerStrides(1))
                plan.Run(start, count, baseOffsets,
                    Sub(b As Long(), cnt As Integer)
                        Dim a As Double = acc
                        Dim ea As Double() = args
                        Dim xi As Integer = CInt(b(0))
                        Dim yi As Integer = CInt(b(1))
                        If xf IsNot Nothing AndAlso yf IsNot Nothing Then
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, mapPair(xf(xi), yf(yi), ea), ea)
                                xi += xInc
                                yi += yInc
                            Next
                        ElseIf xd IsNot Nothing AndAlso yd IsNot Nothing Then
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, mapPair(xd(xi), yd(yi), ea), ea)
                                xi += xInc
                                yi += yInc
                            Next
                        Else
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, mapPair(x.GetDouble(xi), y.GetDouble(yi), ea), ea)
                                xi += xInc
                                yi += yInc
                            Next
                        End If
                        acc = a
                    End Sub)
            End If
            Return acc
        End Function

#End Region

    End Class

    ''' <summary>The accumulator of one reduction (or one parallel partial of it) and the argument array its delegates received.</summary>
    Friend Structure PartialReduction
        ''' <summary>The folded value (before PostProcess).</summary>
        Friend ReadOnly Accumulator As Double
        ''' <summary>The reduction's state (the shared extra arguments for stateless definitions).</summary>
        Friend ReadOnly State As Double()

        Friend Sub New(ByVal accumulator As Double, ByVal state As Double())
            Me.Accumulator = accumulator
            Me.State = state
        End Sub
    End Structure

    ''' <summary>The delegates of a <see cref="ReduceDefinition"/> with their documented defaults filled in.</summary>
    Friend NotInheritable Class ReduceFns

        Friend ReadOnly Name As String
        Friend ReadOnly Start As Func(Of Double(), Double)
        Friend ReadOnly Map As TransformFunction
        Friend ReadOnly MapPair As PairwiseFunction
        Friend ReadOnly Update As PairwiseFunction
        Friend ReadOnly Merge As PairwiseFunction
        Friend ReadOnly Post As Func(Of Double, Long, Double(), Double)
        Friend ReadOnly CreateState As Func(Of Double(), Double())
        Friend ReadOnly MergeState As Action(Of Double(), Double())

        Friend Sub New(ByVal definition As ReduceDefinition, ByVal pair As Boolean)
            If definition Is Nothing Then
                Throw New ArgumentNullException(NameOf(definition))
            End If
            If definition.Update Is Nothing Then
                Throw New ArgumentException("ReduceDefinition '" & definition.Name & "' has no Update function", NameOf(definition))
            End If
            If pair AndAlso definition.MapPair Is Nothing Then
                Throw New ArgumentException("ReduceDefinition '" & definition.Name & "' has no MapPair function, which reduce3 needs", NameOf(definition))
            End If
            Name = definition.Name
            Start = If(definition.StartingValue, New Func(Of Double(), Double)(Function(e As Double()) 0.0))
            Map = If(definition.Map, New TransformFunction(Function(v As Double, e As Double()) v))
            MapPair = definition.MapPair
            Update = definition.Update
            Merge = definition.Merge
            Post = If(definition.PostProcess, New Func(Of Double, Long, Double(), Double)(Function(a As Double, n As Long, e As Double()) a))
            CreateState = definition.CreateState
            MergeState = definition.MergeState
        End Sub

        ''' <summary>True when a reduction may be split into parallel partials: Merge is set, and MergeState too for a stateful definition.</summary>
        Friend ReadOnly Property CanSplit As Boolean
            Get
                Return Merge IsNot Nothing AndAlso (CreateState Is Nothing OrElse MergeState IsNot Nothing)
            End Get
        End Property

        ''' <summary>The argument array of one reduction: a fresh <c>CreateState(extra)</c> for a stateful definition, otherwise the shared <paramref name="extra"/>.</summary>
        Friend Function NewState(ByVal extra As Double()) As Double()
            If CreateState Is Nothing Then
                Return extra
            End If
            Dim state As Double() = CreateState(extra)
            If state Is Nothing Then
                Throw New InvalidOperationException("CreateState of ReduceDefinition '" & Name & "' returned Nothing")
            End If
            Return state
        End Function

        ''' <summary>PostProcess of a finished reduction over <paramref name="n"/> elements, with its state.</summary>
        Friend Function Finish(ByVal r As PartialReduction, ByVal n As Long) As Double
            Return Post(r.Accumulator, n, r.State)
        End Function

        ''' <summary>The result of an empty reduction: PostProcess(StartingValue(state), 0, state) with a fresh state.</summary>
        Friend Function EmptyResult(ByVal extra As Double()) As Double
            Dim state As Double() = NewState(extra)
            Return Post(Start(state), 0, state)
        End Function

    End Class

End Namespace
