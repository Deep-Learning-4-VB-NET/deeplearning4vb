' DeepLearning4VB: variance and standard deviation kernels (ND4J's summary-stats ops).

Imports System
Imports System.Threading.Tasks
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Variance and standard deviation with Welford's online algorithm (numerically stable in one pass);
    ''' parallel partial states are combined with Chan et al.'s pairwise formula in chunk order.
    ''' </summary>
    ''' <remarks>
    ''' The variance is M2 / n, or M2 / (n - 1) with bias correction, and 0 when n &lt;= 1, as libnd4j's
    ''' <c>SummaryStatsData</c> computes it. Results along dimensions follow the TAD numbering of
    ''' <see cref="TadPack"/> (TAD i goes to the element of z with row-major logical index i).
    ''' </remarks>
    Public NotInheritable Class SummaryStatsKernels

        Private Sub New()
        End Sub

        ''' <summary>Variance of all elements of x.</summary>
        Public Shared Function Variance(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(), ByVal biasCorrected As Boolean) As Double
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            Dim n As Long = ShapeInfoHelper.Length(xShapeInfo)
            If n = 0 Then
                Return 0.0
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {xShapeInfo}, New Long() {xOffset}, True, 0)
            Return Finish(Accumulate(x, plan, plan.Offsets, n, True), biasCorrected)
        End Function

        ''' <summary>Standard deviation of all elements of x (the square root of <see cref="Variance"/>).</summary>
        Public Shared Function StandardDeviation(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(), ByVal biasCorrected As Boolean) As Double
            Return SysMath.Sqrt(Variance(x, xOffset, xShapeInfo, biasCorrected))
        End Function

        ''' <summary>Variance (or standard deviation) of every tensor of x along <paramref name="dimensions"/> into the matching element of z.</summary>
        Public Shared Sub VarianceAlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                  ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                  ByVal dimensions As Integer(), ByVal biasCorrected As Boolean, ByVal standardDeviation As Boolean)
            ShapeInfoHelper.Validate(xShapeInfo, NameOf(xShapeInfo))
            VarianceAlongDimensions(x, xOffset, xShapeInfo, z, zOffset, zShapeInfo, TadPack.Create(xShapeInfo, dimensions), biasCorrected, standardDeviation)
        End Sub

        ''' <summary>Variance (or standard deviation) of every TAD of <paramref name="tad"/> (computed for x's shape info) into the matching element of z.</summary>
        Public Shared Sub VarianceAlongDimensions(ByVal x As ManagedStorage, ByVal xOffset As Long, ByVal xShapeInfo As Long(),
                                                  ByVal z As ManagedStorage, ByVal zOffset As Long, ByVal zShapeInfo As Long(),
                                                  ByVal tad As TadPack, ByVal biasCorrected As Boolean, ByVal standardDeviation As Boolean)
            KernelUtil.CheckNumericOperand(x, xOffset, xShapeInfo, "x")
            KernelUtil.CheckNumericOperand(z, zOffset, zShapeInfo, "z")
            If tad Is Nothing Then
                Throw New ArgumentNullException(NameOf(tad))
            End If
            Dim numTads As Long = tad.NumTads
            ReduceKernels.CheckResultLength(zShapeInfo, numTads)
            If numTads = 0 Then
                Return
            End If
            Dim zOffsets As Long() = ShapeInfoHelper.Offsets(zShapeInfo)
            Dim tadLength As Long = tad.TadLength
            If tadLength = 0 Then
                For t As Integer = 0 To CInt(numTads) - 1
                    z.SetDouble(zOffset + zOffsets(t), 0.0)
                Next
                Return
            End If
            Dim plan As StridedPlan = StridedPlan.CreateAligned(New Long()() {tad.TadShapeInfo}, New Long() {0L}, True, 0)
            Dim tadOffsets As Long() = tad.TadOffsets
            ReduceKernels.ForEachTad(numTads, tadLength,
                Sub(t As Integer, parallelInside As Boolean)
                    Dim v As Double = Finish(Accumulate(x, plan, New Long() {xOffset + tadOffsets(t)}, tadLength, parallelInside), biasCorrected)
                    z.SetDouble(zOffset + zOffsets(t), If(standardDeviation, SysMath.Sqrt(v), v))
                End Sub)
        End Sub

        ''' <summary>Welford state: count, running mean and sum of squared deviations.</summary>
        Private Structure Moments
            Public N As Double
            Public Mean As Double
            Public M2 As Double
        End Structure

        Private Shared Function Finish(ByVal m As Moments, ByVal biasCorrected As Boolean) As Double
            If m.N <= 1.0 Then
                Return 0.0
            End If
            Return If(biasCorrected, m.M2 / (m.N - 1.0), m.M2 / m.N)
        End Function

        Private Shared Function Combine(ByVal a As Moments, ByVal b As Moments) As Moments
            If a.N = 0 Then
                Return b
            End If
            If b.N = 0 Then
                Return a
            End If
            Dim r As Moments
            r.N = a.N + b.N
            Dim delta As Double = b.Mean - a.Mean
            r.Mean = a.Mean + delta * (b.N / r.N)
            r.M2 = a.M2 + b.M2 + delta * delta * (a.N * b.N / r.N)
            Return r
        End Function

        Private Shared Function Accumulate(ByVal x As ManagedStorage, ByVal plan As StridedPlan, ByVal baseOffsets As Long(), ByVal count As Long, ByVal allowParallel As Boolean) As Moments
            Dim chunks As Integer = If(allowParallel, ReductionChunks.Count(count), 1)
            If chunks <= 1 Then
                Return Segment(x, plan, baseOffsets, 0, count)
            End If
            Dim partials As Moments() = New Moments(chunks - 1) {}
            Parallel.For(0, chunks, KernelOptions.CreateParallelOptions(),
                Sub(c As Integer)
                    Dim s As Long = ReductionChunks.Start(count, c, chunks)
                    Dim e As Long = ReductionChunks.Start(count, c + 1, chunks)
                    partials(c) = Segment(x, plan, baseOffsets, s, e - s)
                End Sub)
            Dim result As Moments = partials(0)
            For c As Integer = 1 To chunks - 1
                result = Combine(result, partials(c))
            Next
            Return result
        End Function

        Private Shared Function Segment(ByVal x As ManagedStorage, ByVal plan As StridedPlan, ByVal baseOffsets As Long(), ByVal start As Long, ByVal count As Long) As Moments
            Dim n As Double = 0
            Dim mean As Double = 0
            Dim m2 As Double = 0
            Dim xf As Single() = x.Floats
            Dim xd As Double() = x.Doubles
            Dim inc As Integer = CInt(plan.InnerStrides(0))
            plan.Run(start, count, baseOffsets,
                Sub(b As Long(), cnt As Integer)
                    Dim ln As Double = n, lmean As Double = mean, lm2 As Double = m2
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
                        ln += 1.0
                        Dim delta As Double = v - lmean
                        lmean += delta / ln
                        lm2 += delta * (v - lmean)
                        xi += inc
                    Next
                    n = ln
                    mean = lmean
                    m2 = lm2
                End Sub)
            Dim result As Moments
            result.N = n
            result.Mean = mean
            result.M2 = m2
            Return result
        End Function

    End Class

End Namespace
