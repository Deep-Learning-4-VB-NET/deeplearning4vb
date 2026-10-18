' DeepLearning4VB: the managed backend's memory manager (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.CpuMemoryManager.

Imports System
Imports System.Collections.Generic
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports AllocationKind = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.AllocationKind
Imports AllocationsTracker = DeepLearning4VB.org.nd4vb.linalg.api.memory.AllocationsTracker
Imports BasicMemoryManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.BasicMemoryManager
Imports DeallocatorService = DeepLearning4VB.org.nd4vb.linalg.api.memory.deallocation.DeallocatorService
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray

Namespace org.nd4vb.linalg.managed.memory

    ''' <summary>
    ''' The memory manager of the managed backend (Java <c>CpuMemoryManager</c>): memory comes from the garbage-collected
    ''' heap, so it manages the current workspace of each thread (inherited), zeroes arrays and copies buffers.
    ''' </summary>
    ''' <remarks>
    ''' <para>Like the CPU backend it never runs periodic garbage collection (<see cref="PeriodicGcActive"/> is False).</para>
    ''' <para>Differences from Java: <c>allocate</c>/<c>release</c> of native memory are not ported;
    ''' <see cref="allocatedMemory"/> reports the managed heap (<c>GC.GetTotalMemory</c>, Java: JavaCPP's native bytes) plus
    ''' the allocation tracker's general and workspace bytes; <see cref="invokeGc"/> also purges the deallocator service
    ''' given to the constructor.</para>
    ''' </remarks>
    Public Class ManagedMemoryManager
        Inherits BasicMemoryManager

        Private ReadOnly deallocatorService As DeallocatorService

        ''' <summary>A memory manager.</summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub

        ''' <summary>A memory manager that purges <paramref name="deallocatorService"/> after each <see cref="invokeGc"/> (may be Nothing).</summary>
        Public Sub New(ByVal deallocatorService As DeallocatorService)
            Me.deallocatorService = deallocatorService
        End Sub

        ''' <summary>Not implemented in Java either: throws <see cref="NotSupportedException"/>.</summary>
        Public Overrides Sub collect(ParamArray ByVal arrays() As INDArray)
            MyBase.collect(arrays)
        End Sub

        ''' <summary>The managed backend doesn't use periodic GC: always False.</summary>
        Public Overrides ReadOnly Property PeriodicGcActive As Boolean
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Sets the array to zero: a view through <c>assign(0.0)</c>, otherwise every element of its buffer (empty strings
        ''' for UTF8), as Java does.
        ''' </summary>
        Public Overrides Sub memset(ByVal array As INDArray)
            If array Is Nothing Then
                Throw New ArgumentNullException(NameOf(array))
            End If
            If array.View Then
                array.assign(0.0)
                Return
            End If

            Dim data As DataBuffer = array.data()
            Dim managed As ManagedDataBuffer = TryCast(data, ManagedDataBuffer)
            If managed Is Nothing Then
                For i As Long = 0 To data.length() - 1
                    data.put(i, 0)
                Next
                Return
            End If
            Dim s As ManagedStorage = managed.Storage
            If s.StorageType = StorageType.UTF8 Then
                For i As Long = 0 To managed.length() - 1
                    s.SetString(managed.StorageOffset + i, "")
                Next
            Else
                s.Fill(0L, managed.StorageOffset, managed.length())
            End If
        End Sub

        ''' <summary>
        ''' Copies the elements of <paramref name="srcBuffer"/> to <paramref name="dstBuffer"/> (same type required); managed
        ''' buffers copy their storage directly.
        ''' </summary>
        Public Overrides Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer)
            Dim dst As ManagedDataBuffer = TryCast(dstBuffer, ManagedDataBuffer)
            Dim src As ManagedDataBuffer = TryCast(srcBuffer, ManagedDataBuffer)
            If dst IsNot Nothing AndAlso src IsNot Nothing AndAlso dst.dataType() = src.dataType() Then
                src.Storage.CopyTo(src.StorageOffset, dst.Storage, dst.StorageOffset, src.length())
                Return
            End If
            MyBase.memcpy(dstBuffer, srcBuffer)
        End Sub

        ''' <summary>Runs a garbage collection, waits for finalizers and purges the deallocator service.</summary>
        Public Overrides Sub invokeGc()
            MyBase.invokeGc()
            If deallocatorService IsNot Nothing Then
                GC.WaitForPendingFinalizers()
                deallocatorService.purge()
            End If
        End Sub

        ''' <summary>No bandwidth statistics: Nothing, as in Java.</summary>
        Public Overrides ReadOnly Property BandwidthUse As IDictionary(Of Integer, Long)
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>The managed heap size plus the tracked general and workspace bytes on the device.</summary>
        Public Overrides Function allocatedMemory(ByVal deviceId As Integer?) As Long
            Dim device As Integer = deviceId.GetValueOrDefault()
            Return GC.GetTotalMemory(False) + AllocationsTracker.Instance.bytesOnDevice(AllocationKind.GENERAL, device) + AllocationsTracker.Instance.bytesOnDevice(AllocationKind.WORKSPACE, device)
        End Function

    End Class

End Namespace
