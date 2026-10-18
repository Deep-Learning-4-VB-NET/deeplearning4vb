' DeepLearning4VB: reduction kernels (reduce and reduce3) driven by ReduceDefinition.

Imports System
Imports System.Threading.Tasks
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Reductions driven by a <see cref="ReduceDefinition"/>: <c>acc = StartingValue(extra)</c>, then
    ''' <c>acc = Update(acc, Map(x, extra), extra)</c> for every element (<c>MapPair(x, y, extra)</c> for
    ''' the two-input reduce3 form), partial accumulators combined with <c>Merge</c>, and the result
    ''' <c>PostProcess(acc, n, extra)</c> where n is the number of elements reduced.
    ''' </summary>
    ''' <remarks>
    ''' <para>Element order is not specified (the kernels walk memory in its natural order), which only
    ''' affects floating-point rounding. Above <see cref="KernelOptions.ParallelThreshold"/> elements a
    ''' single reduction is cut into chunks of <see cref="KernelOptions.ReductionChunkSize"/> elements whose
    ''' partial accumulators are merged with <c>Merge</c> in chunk order, so results are deterministic;
    ''' when <c>Merge</c> is Nothing the reduction stays sequential. Many small reductions along
    ''' dimensions run in parallel over the tensors instead.</para>
    ''' <para>Delegate defaults: <c>StartingValue</c> Nothing means 0, <c>Map</c> Nothing means identity,
    ''' <c>PostProcess</c> Nothing returns the accumulator; <c>Update</c> is required, and <c>MapPair</c>
    ''' for reduce3. The delegates must be thread-safe and must not modify <c>extraArgs</c> (they all
    ''' receive the same array, never Nothing).</para>
    ''' <para>Reductions along dimensions write TAD <c>i</c> (see <see cref="TadPack"/>) to the element of z
    ''' with row-major logical index <c>i</c>; z needs exactly as many elements as there are TADs and may
    ''' have any shape (with or without kept unit dimensions) and any numeric type.</para>
    ''' </remarks>
    Public NotInheritable Class ReduceKernels

        Private Sub New()
        End Sub

#Region "Single input"

        ''' <summary>Reduces all elements of x to one value.</summary>
        Public Shared Function ReduceToScalar(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                              ByVal definition As ReduceDefinition, ByVal extraArgs As Double()) As Double
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            Dim fns As New ReduceFns(definition, False)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim n As Long = ShapeInfoHelper.Length(xShapeInfo)
            If n = 0 Then
                Return fns.Post(fns.Start(extra), 0, extra)
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {xShapeInfo}, New Long() {xOffset}, True, 0)
            Dim acc As Double = ReduceRange(x, Nothing, plan, plan.Offsets, n, fns, extra, True)
            Return fns.Post(acc, n, extra)
        End Function

        ''' <summary>Reduces every tensor of x along <paramref name="dimensions"/> into the matching element of z.</summary>
        Public Shared Sub ReduceAlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                ByVal dimensions As Integer(), ByVal definition As ReduceDefinition, ByVal extraArgs As Double())
            ShapeInfoHelper.Validate(xShapeInfo, NameOf(xShapeInfo))
            ReduceAlongDimensions(x, xOffset, xShapeInfo, z, zOffset, zShapeInfo, TadPack.Create(xShapeInfo, dimensions), definition, extraArgs)
        End Sub

        ''' <summary>Reduces every TAD of <paramref name="tad"/> (computed for x's shape info) into the matching element of z.</summary>
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
                Dim empty As Double = fns.Post(fns.Start(extra), 0, extra)
                For t As Integer = 0 To CInt(numTads) - 1
                    z.SetDouble(zOffset + zOffsets(t), empty)
                Next
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {tad.TadShapeInfo}, New Long() {0L}, True, 0)
            Dim tadOffsets As Long() = tad.TadOffsets
            ForEachTad(numTads, tadLength,
                Sub(t As Integer, parallelInside As Boolean)
                    Dim acc As Double = ReduceRange(x, Nothing, plan, New Long() {xOffset + tadOffsets(t)}, tadLength, fns, extra, parallelInside)
                    z.SetDouble(zOffset + zOffsets(t), fns.Post(acc, tadLength, extra))
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
                Return fns.Post(fns.Start(extra), 0, extra)
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
            Dim acc As Double = ReduceRange(xs, ys, plan, plan.Offsets, n, fns, extra, True)
            Return fns.Post(acc, n, extra)
        End Function

        ''' <summary>
        ''' Reduce3 along dimensions: pairs TAD <c>i</c> of x with TAD <c>i</c> of y (same TAD shape and
        ''' count) and writes the result to element <c>i</c> of z. When y has exactly as many elements as one
        ''' TAD of x, every TAD of x is paired with the whole of y instead.
        ''' </summary>
        Public Shared Sub Reduce3AlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                 ByVal y As ManagedStorage, ByVal yOffset As Long, ByVal yShapeInfo As Long(),
                                                 ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                 ByVal dimensions As Integer(), ByVal definition As ReduceDefinition, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(y, yOffset, yShapeInfo, "y")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            Dim fns As New ReduceFns(definition, True)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim tadX As TadPack = TadPack.Create(xShapeInfo, dimensions)
            Dim numTads As Long = tadX.NumTads
            Dim tadLength As Long = tadX.TadLength
            CheckResultLength(zShapeInfo, numTads)
            Dim tadShape As Long() = ShapeInfoHelper.GetShape(tadX.TadShapeInfo)
            Dim yWhole As Boolean = ShapeInfoHelper.Length(yShapeInfo) = tadLength
            Dim ys As ManagedStorage = y
            Dim yo As Long = yOffset
            Dim yStrides As Long()
            Dim yTadOffsets As Long()
            If yWhole Then
                yStrides = YStridesForTad(y, yOffset, yShapeInfo, tadShape, ys, yo)
                yTadOffsets = Nothing
            Else
                Dim tadY As TadPack = TadPack.Create(yShapeInfo, dimensions)
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
                Dim empty As Double = fns.Post(fns.Start(extra), 0, extra)
                For t As Integer = 0 To CInt(numTads) - 1
                    z.SetDouble(zOffset + zOffsets(t), empty)
                Next
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.Create(tadShape, New Long()() {ShapeInfoHelper.GetStrides(tadX.TadShapeInfo), yStrides}, New Long() {0L, 0L}, True, 0)
            Dim xTadOffsets As Long() = tadX.TadOffsets
            ForEachTad(numTads, tadLength,
                Sub(t As Integer, parallelInside As Boolean)
                    Dim yBase As Long = If(yTadOffsets Is Nothing, yo, yo + yTadOffsets(t))
                    Dim acc As Double = ReduceRange(x, ys, plan, New Long() {xOffset + xTadOffsets(t), yBase}, tadLength, fns, extra, parallelInside)
                    z.SetDouble(zOffset + zOffsets(t), fns.Post(acc, tadLength, extra))
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
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(y, yOffset, yShapeInfo, "y")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            Dim fns As New ReduceFns(definition, True)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim tadX As TadPack = TadPack.Create(xShapeInfo, dimensions)
            Dim tadY As TadPack = TadPack.Create(yShapeInfo, dimensions)
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
            Dim plan As StridedPlan = StridedPlan.Create(tadShape, New Long()() {ShapeInfoHelper.GetStrides(tadX.TadShapeInfo), yStrides}, New Long() {0L, 0L}, True, 0)
            Dim xTadOffsets As Long() = tadX.TadOffsets
            Dim yTadOffsets As Long() = tadY.TadOffsets
            ForEachTad(nx * ny, tadLength,
                Sub(p As Integer, parallelInside As Boolean)
                    Dim i As Long = p \ ny
                    Dim j As Long = p - i * ny
                    Dim acc As Double
                    If tadLength = 0 Then
                        acc = fns.Start(extra)
                    Else
                        acc = ReduceRange(x, y, plan, New Long() {xOffset + xTadOffsets(CInt(i)), yOffset + yTadOffsets(CInt(j))}, tadLength, fns, extra, parallelInside)
                    End If
                    z.SetDouble(zOffset + zOffsets(p), fns.Post(acc, tadLength, extra))
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

        ''' <summary>
        ''' Calls <paramref name="body"/>(t, parallelInside) for every TAD t: in parallel over TADs when
        ''' there are many, otherwise sequentially with parallelInside = True so that each large TAD is
        ''' reduced in parallel chunks.
        ''' </summary>
        Friend Shared Sub ForEachTad(ByVal numTads As Long, ByVal tadLength As Long, ByVal body As Action(Of Integer, Boolean))
            Dim total As Long = numTads * Math.Max(1L, tadLength)
            Dim workers As Integer = Math.Max(1, KernelOptions.MaxDegreeOfParallelism)
            If KernelOptions.ShouldParallelize(total) AndAlso numTads > 1 AndAlso (numTads >= 2L * workers OrElse tadLength < KernelOptions.ParallelThreshold) Then
                Dim chunks As Integer = CInt(Math.Min(numTads, CLng(workers) * 4L))
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
        ''' Reduces the first <paramref name="count"/> elements of the plan from the given base offsets
        ''' (y Nothing: single input; otherwise pairs), in deterministic parallel chunks when allowed and large.
        ''' </summary>
        Friend Shared Function ReduceRange(ByVal x As ManagedStorage, ByVal y As ManagedStorage, ByVal plan As StridedPlan, ByVal baseOffsets As Long(),
                                           ByVal count As Long, ByVal fns As ReduceFns, ByVal extra As Double(), ByVal allowParallel As Boolean) As Double
            Dim chunks As Integer = If(allowParallel AndAlso fns.Merge IsNot Nothing, ReductionChunks.Count(count), 1)
            If chunks <= 1 Then
                Return ReduceSegment(x, y, plan, baseOffsets, 0, count, fns, extra)
            End If
            Dim partials As Double() = New Double(chunks - 1) {}
            Parallel.For(0, chunks, KernelOptions.CreateParallelOptions(),
                Sub(c As Integer)
                    Dim s As Long = ReductionChunks.Start(count, c, chunks)
                    Dim e As Long = ReductionChunks.Start(count, c + 1, chunks)
                    partials(c) = ReduceSegment(x, y, plan, baseOffsets, s, e - s, fns, extra)
                End Sub)
            Dim acc As Double = partials(0)
            For c As Integer = 1 To chunks - 1
                acc = fns.Merge(acc, partials(c), extra)
            Next
            Return acc
        End Function

        ''' <summary>Sequential reduction of elements [start, start + count) of the plan; the accumulator starts at StartingValue.</summary>
        Friend Shared Function ReduceSegment(ByVal x As ManagedStorage, ByVal y As ManagedStorage, ByVal plan As StridedPlan, ByVal baseOffsets As Long(),
                                             ByVal start As Long, ByVal count As Long, ByVal fns As ReduceFns, ByVal extra As Double()) As Double
            Dim acc As Double = fns.Start(extra)
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
                        Dim xi As Integer = CInt(b(0))
                        If xf IsNot Nothing Then
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, map(xf(xi), extra), extra)
                                xi += inc
                            Next
                        ElseIf xd IsNot Nothing Then
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, map(xd(xi), extra), extra)
                                xi += inc
                            Next
                        Else
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, map(x.GetDouble(xi), extra), extra)
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
                        Dim xi As Integer = CInt(b(0))
                        Dim yi As Integer = CInt(b(1))
                        If xf IsNot Nothing AndAlso yf IsNot Nothing Then
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, mapPair(xf(xi), yf(yi), extra), extra)
                                xi += xInc
                                yi += yInc
                            Next
                        ElseIf xd IsNot Nothing AndAlso yd IsNot Nothing Then
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, mapPair(xd(xi), yd(yi), extra), extra)
                                xi += xInc
                                yi += yInc
                            Next
                        Else
                            For j As Integer = 0 To cnt - 1
                                a = upd(a, mapPair(x.GetDouble(xi), y.GetDouble(yi), extra), extra)
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

    ''' <summary>The delegates of a <see cref="ReduceDefinition"/> with their documented defaults filled in.</summary>
    Friend NotInheritable Class ReduceFns

        Friend ReadOnly Start As Func(Of Double(), Double)
        Friend ReadOnly Map As TransformFunction
        Friend ReadOnly MapPair As PairwiseFunction
        Friend ReadOnly Update As PairwiseFunction
        Friend ReadOnly Merge As PairwiseFunction
        Friend ReadOnly Post As Func(Of Double, Long, Double(), Double)

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
            Start = If(definition.StartingValue, New Func(Of Double(), Double)(Function(e As Double()) 0.0))
            Map = If(definition.Map, New TransformFunction(Function(v As Double, e As Double()) v))
            MapPair = definition.MapPair
            Update = definition.Update
            Merge = definition.Merge
            Post = If(definition.PostProcess, New Func(Of Double, Long, Double(), Double)(Function(a As Double, n As Long, e As Double()) a))
        End Sub

    End Class

End Namespace
