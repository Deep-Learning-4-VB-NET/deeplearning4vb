' DeepLearning4VB: tuning knobs of the managed kernels (phase 2, see docs/PHASE2_DESIGN.md).

Imports System
Imports System.Threading.Tasks

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Process-wide settings of the managed kernels: when they go parallel and whether the .NET 10 build
    ''' uses SIMD. The defaults suit a desktop CPU; changing them affects only speed, except that
    ''' <see cref="ReductionChunkSize"/> also decides how floating-point partial sums are grouped.
    ''' </summary>
    Public NotInheritable Class KernelOptions

        Private Sub New()
        End Sub

        ''' <summary>Element count from which element-wise kernels, reductions and level-1/2 BLAS run in parallel (default 32768).</summary>
        Public Shared Property ParallelThreshold As Long = 32768L

        ''' <summary>
        ''' Elements per partial result in parallel reductions (default 16384). Partials are merged in
        ''' chunk order, so a reduction gives the same result on every machine and every run.
        ''' </summary>
        Public Shared Property ReductionChunkSize As Long = 16384L

        ''' <summary>Multiply-add count (m * n * k) from which gemm runs in parallel (default 2^18).</summary>
        Public Shared Property GemmParallelThreshold As Long = 262144L

        ''' <summary>Upper bound on the threads a kernel uses (default: the processor count; 1 disables parallelism).</summary>
        Public Shared Property MaxDegreeOfParallelism As Integer = Environment.ProcessorCount

        ''' <summary>Lets the .NET 10 build use <c>System.Numerics.Vector(Of T)</c> in BLAS (default True; ignored on .NET Framework).</summary>
        Public Shared Property UseSimd As Boolean = True

        ''' <summary>True when the SIMD BLAS paths are compiled in (.NET 10 build), hardware-accelerated and enabled by <see cref="UseSimd"/>.</summary>
        Public Shared ReadOnly Property SimdEnabled As Boolean
            Get
#If NET Then
                Return UseSimd AndAlso System.Numerics.Vector.IsHardwareAccelerated
#Else
                Return False
#End If
            End Get
        End Property

        ''' <summary>Parallel options honouring <see cref="MaxDegreeOfParallelism"/>.</summary>
        Friend Shared Function CreateParallelOptions() As ParallelOptions
            Dim po As New ParallelOptions()
            po.MaxDegreeOfParallelism = Math.Max(1, MaxDegreeOfParallelism)
            Return po
        End Function

        ''' <summary>True when work of <paramref name="size"/> elements should run in parallel.</summary>
        Friend Shared Function ShouldParallelize(ByVal size As Long) As Boolean
            Return MaxDegreeOfParallelism > 1 AndAlso size >= ParallelThreshold
        End Function

        ''' <summary>Number of chunks for parallel element-wise work over <paramref name="size"/> elements (chunks of at least a quarter of the threshold).</summary>
        Friend Shared Function ChunkCount(ByVal size As Long) As Integer
            Dim maxChunks As Long = Math.Max(1, MaxDegreeOfParallelism) * 4L
            Dim bySize As Long = Math.Max(1L, size \ Math.Max(1L, ParallelThreshold \ 4L))
            Return CInt(Math.Max(1L, Math.Min(maxChunks, bySize)))
        End Function

    End Class

End Namespace
