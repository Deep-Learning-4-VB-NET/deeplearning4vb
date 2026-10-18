' DeepLearning4VB: the BLAS settings object of the managed ND4VB backend (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.blas.CpuBlas and org.nd4j.nativeblas.Nd4jBlas.

Imports System
Imports System.Globalization
Imports BlasT = DeepLearning4VB.org.nd4vb.linalg.api.blas.Blas
Imports ND4VBEnvironmentVars = DeepLearning4VB.org.nd4vb.common.config.ND4VBEnvironmentVars
Imports KernelOptions = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.KernelOptions

Namespace org.nd4vb.linalg.managed.blas

    ''' <summary>
    ''' The <see cref="BlasT"/> object of the managed backend (Java <c>CpuBlas</c>, what <c>NDArrayFactory.blas()</c> returns):
    ''' the BLAS thread count is the managed kernels' <see cref="KernelOptions.MaxDegreeOfParallelism"/>, and the vendor is
    ''' <see cref="BlasT.Vendor.UNKNOWN"/> (id 0): the routines are the managed kernels, not a native library.
    ''' </summary>
    ''' <remarks>
    ''' As Java's <c>Nd4jBlas</c> constructor did, a new instance sets the thread count from the environment variable
    ''' <c>OMP_NUM_THREADS</c> (a positive integer) unless <c>ND4VB_SKIP_BLAS_THREADS</c> is set; without
    ''' <c>OMP_NUM_THREADS</c> the default (the processor count) stays.
    ''' </remarks>
    Public Class ManagedBlasSettings
        Implements BlasT

        ''' <summary>A settings object; applies <c>OMP_NUM_THREADS</c> as described above.</summary>
        Public Sub New()
            Dim skipper As String = Global.System.Environment.GetEnvironmentVariable(ND4VBEnvironmentVars.ND4VB_SKIP_BLAS_THREADS)
            If String.IsNullOrEmpty(skipper) Then
                Dim threads As String = Global.System.Environment.GetEnvironmentVariable(ND4VBEnvironmentVars.OMP_NUM_THREADS)
                Dim n As Integer
                If Not String.IsNullOrEmpty(threads) AndAlso Integer.TryParse(threads, NumberStyles.Integer, CultureInfo.InvariantCulture, n) AndAlso n > 0 Then
                    MaxThreads = n
                End If
            End If
        End Sub

        ''' <summary>The number of threads the managed BLAS kernels use (<see cref="KernelOptions.MaxDegreeOfParallelism"/>, at least 1).</summary>
        Public Overridable Property MaxThreads As Integer Implements BlasT.MaxThreads
            Get
                Return KernelOptions.MaxDegreeOfParallelism
            End Get
            Set(ByVal value As Integer)
                KernelOptions.MaxDegreeOfParallelism = Math.Max(1, value)
            End Set
        End Property

        ''' <summary>0: no native BLAS library.</summary>
        Public Overridable ReadOnly Property BlasVendorId As Integer Implements BlasT.BlasVendorId
            Get
                Return 0
            End Get
        End Property

        ''' <summary><see cref="BlasT.Vendor.UNKNOWN"/>, as Java mapped vendor id 0.</summary>
        Public Overridable ReadOnly Property BlasVendor As BlasT.Vendor Implements BlasT.BlasVendor
            Get
                Return BlasT.Vendor.UNKNOWN
            End Get
        End Property

    End Class

End Namespace
