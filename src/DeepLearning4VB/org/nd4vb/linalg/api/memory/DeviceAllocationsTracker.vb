Imports System
Imports System.Threading
Imports AllocationKind = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.AllocationKind

Namespace org.nd4vb.linalg.api.memory

    ''' <summary>
    ''' Thread-safe counters of the bytes allocated on one device, per <see cref="AllocationKind"/> (Java keeps an
    ''' <c>AtomicLong</c> per kind; here an array updated with <see cref="Interlocked"/>).
    ''' </summary>
    Public Class DeviceAllocationsTracker

        Private ReadOnly bytesMap As Long()

        Public Sub New()
            bytesMap = New Long([Enum].GetValues(GetType(AllocationKind)).Length - 1) {}
        End Sub

        Public Overridable Sub updateState(ByVal kind As AllocationKind, ByVal bytes As Long)
            Interlocked.Add(bytesMap(CInt(kind)), bytes)
        End Sub

        Public Overridable Function getState(ByVal kind As AllocationKind) As Long
            Return Interlocked.Read(bytesMap(CInt(kind)))
        End Function

    End Class

End Namespace
