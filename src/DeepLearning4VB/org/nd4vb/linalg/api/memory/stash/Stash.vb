Imports System
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

Namespace org.nd4vb.linalg.api.memory.stash

    ''' <summary>
    ''' A keyed store of arrays (Java <c>Stash</c>, an unfinished API in ND4J; the basic implementations are no-ops).
    ''' </summary>
    Public Interface Stash(Of T)

        Function checkIfExists(ByVal key As T) As Boolean

        Sub put(ByVal key As T, ByVal [object] As INDArray)

        Function [get](ByVal key As T) As INDArray

        Sub purge()

    End Interface

End Namespace
