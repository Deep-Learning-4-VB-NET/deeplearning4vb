' DeepLearning4VB: the managed backend's affinity manager (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.CpuAffinityManager.

Imports System
Imports BasicAffinityManager = DeepLearning4VB.org.nd4vb.linalg.api.concurrency.BasicAffinityManager
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

Namespace org.nd4vb.linalg.managed

    ''' <summary>
    ''' The affinity manager of the managed backend (Java <c>CpuAffinityManager</c>): one device, number 0, shared by every
    ''' thread; arrays and buffers live everywhere, so touching and replicating need no work.
    ''' </summary>
    Public Class ManagedAffinityManager
        Inherits BasicAffinityManager

        ''' <summary>Nothing to do: there is only host memory.</summary>
        Public Overrides Sub touch(ByVal array As INDArray)
            ' no-op
        End Sub

        ''' <summary>Nothing to do: there is only host memory.</summary>
        Public Overrides Sub touch(ByVal buffer As DataBuffer)
            ' no-op
        End Sub

    End Class

End Namespace
