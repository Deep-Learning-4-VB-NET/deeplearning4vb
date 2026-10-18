Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

Namespace org.nd4vb.linalg.api.memory.stash

    ''' <summary>
    ''' The placeholder base class of ND4J's stashes: the methods are stubs (the Java comments describe the intent).
    ''' </summary>
    Public MustInherit Class BasicStash(Of T)
        Implements Stash(Of T)

        Protected Friend stash As IDictionary(Of T, INDArray) = New ConcurrentDictionary(Of T, INDArray)()

        Protected Friend Sub New()
        End Sub

        Public Overridable Function checkIfExists(ByVal key As T) As Boolean Implements Stash(Of T).checkIfExists
            ' Just checkin'
            Return False
        End Function

        Public Overridable Sub put(ByVal key As T, ByVal [object] As INDArray) Implements Stash(Of T).put
            ' Basically we want to get DataBuffer here, and store it here together with shape
            ' Special case here is GPU: we want to synchronize HOST memory, and store only HOST memory.
        End Sub

        Public Overridable Function [get](ByVal key As T) As INDArray Implements Stash(Of T).get
            ' We want to restore INDArray here, In case of GPU backend - we want to ensure data is replicated to device.
            Return Nothing
        End Function

        Public Overridable Sub purge() Implements Stash(Of T).purge
            ' We want to purge all stored stuff here.
        End Sub

    End Class

End Namespace
