Imports System
Imports GridPointers = DeepLearning4VB.org.nd4vb.linalg.api.ops.grid.GridPointers
Imports OpDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.ops.grid.OpDescriptor

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' A grid op of two ops executed back to back (Java <c>MetaOp</c>, used by the CUDA grid executioner).
    ''' The setters <c>setFirstPointers</c>/<c>setSecondPointers</c> take the <see cref="GridPointers"/>, which in
    ''' ND4VB carry no native pointers.
    ''' </summary>
    Public Interface MetaOp
        Inherits GridOp

        ReadOnly Property FirstOp As Op

        ReadOnly Property SecondOp As Op

        ReadOnly Property FirstOpDescriptor As OpDescriptor

        ReadOnly Property SecondOpDescriptor As OpDescriptor

        WriteOnly Property FirstPointers As GridPointers

        WriteOnly Property SecondPointers As GridPointers

    End Interface

End Namespace
