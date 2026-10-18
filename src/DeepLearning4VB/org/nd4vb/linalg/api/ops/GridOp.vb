Imports System
Imports GridDescriptorT = DeepLearning4VB.org.nd4vb.linalg.api.ops.grid.GridDescriptor

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' An op that executes a sequence of ops as one grid (Java <c>GridOp</c>, used by the CUDA grid executioner).
    ''' </summary>
    Public Interface GridOp
        Inherits Op

        ''' <summary>The ops of this grid.</summary>
        ReadOnly Property GridDescriptor As GridDescriptorT

    End Interface

End Namespace
