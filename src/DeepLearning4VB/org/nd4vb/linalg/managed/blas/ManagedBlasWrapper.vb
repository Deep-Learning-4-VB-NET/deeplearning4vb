' DeepLearning4VB: the BLAS wrapper of the managed ND4VB backend (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.BlasWrapper.

Imports System
Imports BaseBlasWrapper = DeepLearning4VB.org.nd4vb.linalg.factory.BaseBlasWrapper

Namespace org.nd4vb.linalg.managed.blas

    ''' <summary>
    ''' The BLAS wrapper of the managed backend (Java's CPU <c>BlasWrapper</c>, the class the backend configuration names
    ''' under <c>blas.ops</c>): everything is inherited from <see cref="BaseBlasWrapper"/>, which delegates to the levels
    ''' and LAPACK of the array factory (<see cref="ManagedLevel1"/>, <see cref="ManagedLevel2"/>,
    ''' <see cref="ManagedLevel3"/> and <see cref="ManagedLapack"/> for the managed backend).
    ''' </summary>
    Public Class ManagedBlasWrapper
        Inherits BaseBlasWrapper

    End Class

End Namespace
