Imports System
Imports System.Collections.Concurrent
Imports AllocationKind = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.AllocationKind

Namespace org.nd4vb.linalg.api.memory

    ''' <summary>
    ''' Keeps per-device counters of the bytes allocated for each <see cref="AllocationKind"/>. A process-wide
    ''' singleton (<see cref="Instance"/>).
    ''' </summary>
    Public Class AllocationsTracker

        Private Shared ReadOnly INSTANCE_Conflict As New AllocationsTracker()

        Private ReadOnly devices As New ConcurrentDictionary(Of Integer, DeviceAllocationsTracker)()

        Protected Friend Sub New()
        End Sub

        Public Shared ReadOnly Property Instance As AllocationsTracker
            Get
                Return INSTANCE_Conflict
            End Get
        End Property

        ''' <summary>Java <c>getInstance()</c>.</summary>
        Public Shared Function getInstance() As AllocationsTracker
            Return INSTANCE_Conflict
        End Function

        Protected Friend Overridable Function trackerForDevice(ByVal deviceId As Integer) As DeviceAllocationsTracker
            Return devices.GetOrAdd(deviceId, Function(id) New DeviceAllocationsTracker())
        End Function

        Public Overridable Sub markAllocated(ByVal kind As AllocationKind, ByVal deviceId As Integer, ByVal bytes As Long)
            Dim tracker As DeviceAllocationsTracker = trackerForDevice(deviceId)
            tracker.updateState(kind, bytes)
        End Sub

        Public Overridable Sub markReleased(ByVal kind As AllocationKind, ByVal deviceId As Integer, ByVal bytes As Long)
            Dim tracker As DeviceAllocationsTracker = trackerForDevice(deviceId)
            tracker.updateState(kind, -bytes)
        End Sub

        Public Overridable Function bytesOnDevice(ByVal deviceId As Integer) As Long
            Return bytesOnDevice(AllocationKind.GENERAL, deviceId)
        End Function

        Public Overridable Function bytesOnDevice(ByVal kind As AllocationKind, ByVal deviceId As Integer) As Long
            Dim tracker As DeviceAllocationsTracker = trackerForDevice(deviceId)
            Return tracker.getState(kind)
        End Function

    End Class

End Namespace
