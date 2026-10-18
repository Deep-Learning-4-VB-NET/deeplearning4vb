' DeepLearning4VB: index reductions (argmax, argmin, first/last index...) driven by IndexReduceDefinition.

Imports System
Imports System.Threading.Tasks
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Index reductions driven by an <see cref="IndexReduceDefinition"/>: the kernel walks the elements in
    ''' row-major logical order, starting from the candidate (<c>StartingValue(extra)</c>, index -1), and
    ''' replaces the best candidate whenever <c>Prefer(value, index, bestValue, bestIndex, extra)</c> is
    ''' True. The result is the best index, or <c>EmptyIndex</c> when no element was preferred (index
    ''' still -1). With a strict comparison in <c>Prefer</c>, ties keep the first index, as libnd4j does.
    ''' </summary>
    ''' <remarks>
    ''' Indexes are row-major logical indexes: over the whole array for <see cref="IndexReduceToScalar"/>,
    ''' within each TAD for <see cref="IndexReduceAlongDimensions(ManagedStorage, Long, Long(), ManagedStorage, Long, Long(), Integer(), IndexReduceDefinition, Double())"/>.
    ''' Large inputs are split into chunks whose winners are merged in chunk order with the same
    ''' <c>Prefer</c>, which gives the sequential result. <c>Prefer</c> and <c>StartingValue</c> must be
    ''' thread-safe and must not modify <c>extraArgs</c>.
    ''' </remarks>
    Public NotInheritable Class IndexReduceKernels

        Private Sub New()
        End Sub

        ''' <summary>The index-reduction result over all elements of x (a row-major logical index, or EmptyIndex).</summary>
        Public Shared Function IndexReduceToScalar(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                   ByVal definition As IndexReduceDefinition, ByVal extraArgs As Double()) As Long
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            CheckDefinition(definition)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim n As Long = ShapeInfoHelper.Length(xShapeInfo)
            If n = 0 Then
                Return definition.EmptyIndex
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {xShapeInfo}, New Long() {xOffset}, False, 0)
            Return Finish(definition, ReduceRange(x, plan, plan.Offsets, n, definition, extra, True))
        End Function

        ''' <summary>Index reduction of every tensor of x along <paramref name="dimensions"/>; z (any numeric type, one element per TAD) receives indexes within each TAD.</summary>
        Public Shared Sub IndexReduceAlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                     ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                     ByVal dimensions As Integer(), ByVal definition As IndexReduceDefinition, ByVal extraArgs As Double())
            ShapeInfoHelper.Validate(xShapeInfo, NameOf(xShapeInfo))
            IndexReduceAlongDimensions(x, xOffset, xShapeInfo, z, zOffset, zShapeInfo, TadPack.Create(xShapeInfo, dimensions), definition, extraArgs)
        End Sub

        ''' <summary>Index reduction of every TAD of <paramref name="tad"/> (computed for x's shape info) into the matching element of z.</summary>
        Public Shared Sub IndexReduceAlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                     ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                     ByVal tad As TadPack, ByVal definition As IndexReduceDefinition, ByVal extraArgs As Double())
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            If tad Is Nothing Then
                Throw New ArgumentNullException(NameOf(tad))
            End If
            CheckDefinition(definition)
            Dim extra As Double() = KernelUtil.Args(extraArgs)
            Dim numTads As Long = tad.NumTads
            ReduceKernels.CheckResultLength(zShapeInfo, numTads)
            If numTads = 0 Then
                Return
            End If
            Dim zOffsets As Long() = ShapeInfoHelper.Offsets(zShapeInfo)
            Dim tadLength As Long = tad.TadLength
            If tadLength = 0 Then
                For t As Integer = 0 To CInt(numTads) - 1
                    z.SetLong(zOffset + zOffsets(t), definition.EmptyIndex)
                Next
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {tad.TadShapeInfo}, New Long() {0L}, False, 0)
            Dim tadOffsets As Long() = tad.TadOffsets
            ReduceKernels.ForEachTad(numTads, tadLength,
                Sub(t As Integer, parallelInside As Boolean)
                    Dim best As Candidate = ReduceRange(x, plan, New Long() {xOffset + tadOffsets(t)}, tadLength, definition, extra, parallelInside)
                    z.SetLong(zOffset + zOffsets(t), Finish(definition, best))
                End Sub)
        End Sub

        Private Structure Candidate
            Public Value As Double
            Public Index As Long
        End Structure

        Private Shared Sub CheckDefinition(ByVal definition As IndexReduceDefinition)
            If definition Is Nothing Then
                Throw New ArgumentNullException(NameOf(definition))
            End If
            If definition.Prefer Is Nothing Then
                Throw New ArgumentException("IndexReduceDefinition '" & definition.Name & "' has no Prefer function", NameOf(definition))
            End If
        End Sub

        Private Shared Function Finish(ByVal definition As IndexReduceDefinition, ByVal best As Candidate) As Long
            Return If(best.Index < 0, definition.EmptyIndex, best.Index)
        End Function

        Private Shared Function ReduceRange(ByVal x As ManagedStorage, ByVal plan As StridedPlan, ByVal baseOffsets As Long(), ByVal count As Long,
                                            ByVal definition As IndexReduceDefinition, ByVal extra As Double(), ByVal allowParallel As Boolean) As Candidate
            Dim chunks As Integer = If(allowParallel, ReductionChunks.Count(count), 1)
            If chunks <= 1 Then
                Return Segment(x, plan, baseOffsets, 0, count, definition, extra)
            End If
            Dim partials As Candidate() = New Candidate(chunks - 1) {}
            Parallel.For(0, chunks, KernelOptions.CreateParallelOptions(),
                Sub(c As Integer)
                    Dim s As Long = ReductionChunks.Start(count, c, chunks)
                    Dim e As Long = ReductionChunks.Start(count, c + 1, chunks)
                    partials(c) = Segment(x, plan, baseOffsets, s, e - s, definition, extra)
                End Sub)
            Dim prefer As Func(Of Double, Long, Double, Long, Double(), Boolean) = definition.Prefer
            Dim best As Candidate = partials(0)
            For c As Integer = 1 To chunks - 1
                Dim p As Candidate = partials(c)
                If p.Index < 0 Then
                    Continue For
                End If
                If best.Index < 0 OrElse prefer(p.Value, p.Index, best.Value, best.Index, extra) Then
                    best = p
                End If
            Next
            Return best
        End Function

        Private Shared Function Segment(ByVal x As ManagedStorage, ByVal plan As StridedPlan, ByVal baseOffsets As Long(), ByVal start As Long, ByVal count As Long,
                                        ByVal definition As IndexReduceDefinition, ByVal extra As Double()) As Candidate
            Dim prefer As Func(Of Double, Long, Double, Long, Double(), Boolean) = definition.Prefer
            Dim bestValue As Double = If(definition.StartingValue Is Nothing, 0.0, definition.StartingValue(extra))
            Dim bestIndex As Long = -1
            Dim index As Long = start
            Dim xf As Single() = x.Floats
            Dim xd As Double() = x.Doubles
            Dim inc As Integer = CInt(plan.InnerStrides(0))
            plan.Run(start, count, baseOffsets,
                Sub(b As Long(), cnt As Integer)
                    Dim bv As Double = bestValue
                    Dim bi As Long = bestIndex
                    Dim idx As Long = index
                    Dim xi As Integer = CInt(b(0))
                    For j As Integer = 0 To cnt - 1
                        Dim v As Double
                        If xf IsNot Nothing Then
                            v = xf(xi)
                        ElseIf xd IsNot Nothing Then
                            v = xd(xi)
                        Else
                            v = x.GetDouble(xi)
                        End If
                        If prefer(v, idx, bv, bi, extra) Then
                            bv = v
                            bi = idx
                        End If
                        idx += 1
                        xi += inc
                    Next
                    bestValue = bv
                    bestIndex = bi
                    index = idx
                End Sub)
            Dim result As Candidate
            result.Value = bestValue
            result.Index = bestIndex
            Return result
        End Function

    End Class

End Namespace
