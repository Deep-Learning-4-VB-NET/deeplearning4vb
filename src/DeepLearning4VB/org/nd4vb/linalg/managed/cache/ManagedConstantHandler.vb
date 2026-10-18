' DeepLearning4VB: the managed backend's constant buffer cache (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.cache.ConstantBuffersCache.

Imports System
Imports System.Collections.Concurrent
Imports System.Threading
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.buffer
Imports AllocationKind = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.AllocationKind
Imports AllocationsTracker = DeepLearning4VB.org.nd4vb.linalg.api.memory.AllocationsTracker
Imports ArrayDescriptor = DeepLearning4VB.org.nd4vb.linalg.cache.ArrayDescriptor
Imports BasicConstantHandler = DeepLearning4VB.org.nd4vb.linalg.cache.BasicConstantHandler

Namespace org.nd4vb.linalg.managed.cache

    ''' <summary>
    ''' The constant buffer cache of the managed backend (Java <c>ConstantBuffersCache</c>): a thread-safe dictionary from
    ''' <see cref="ArrayDescriptor"/> (the values and the data type) to a detached buffer flagged
    ''' <see cref="DataBuffer.Constant"/>. At most <see cref="MAX_ENTRIES"/> buffers are cached; later requests get a new
    ''' (constant) buffer each time. Cached bytes are recorded as CONSTANT allocations in the <see cref="AllocationsTracker"/>.
    ''' </summary>
    ''' <remarks>
    ''' Differences from Java: the buffers are flagged constant (Java's CPU cache did not flag them; CUDA's did);
    ''' <c>getConstantBuffer(Long(), dataType)</c> creates a buffer of <paramref name="dataType"/> like the other overloads
    ''' (Java always created a LONG buffer for long values); <see cref="purgeConstants"/> also resets the entry count, the cached
    ''' bytes and the tracker (Java kept counting, so after 1,000 entries nothing was ever cached again); a buffer is added
    ''' atomically, so concurrent requests for the same values return the same buffer.
    ''' </remarks>
    Public Class ManagedConstantHandler
        Inherits BasicConstantHandler

        ''' <summary>The maximum number of cached buffers.</summary>
        Public Const MAX_ENTRIES As Integer = 1000

        Protected Friend buffersCache As New ConcurrentDictionary(Of ArrayDescriptor, DataBuffer)()
        Private counter As Integer = 0
        Private bytes As Long = 0
        Private ReadOnly gate As New Object()

        ''' <summary>Forgets every cached buffer.</summary>
        Public Overrides Sub purgeConstants()
            SyncLock gate
                buffersCache = New ConcurrentDictionary(Of ArrayDescriptor, DataBuffer)()
                AllocationsTracker.Instance.markReleased(AllocationKind.CONSTANT, 0, bytes)
                counter = 0
                bytes = 0
            End SyncLock
        End Sub

        Private Function lookup(ByVal descriptor As ArrayDescriptor, ByVal values As Array, ByVal dataType As DataType) As DataBuffer
            Dim cache As ConcurrentDictionary(Of ArrayDescriptor, DataBuffer) = buffersCache
            Dim existing As DataBuffer = Nothing
            If cache.TryGetValue(descriptor, existing) Then
                Return existing
            End If

            Dim buffer As New ManagedDataBuffer(dataType, values)
            buffer.Constant = True

            SyncLock gate
                If cache.TryGetValue(descriptor, existing) Then
                    Return existing
                End If
                If counter < MAX_ENTRIES AndAlso cache Is buffersCache Then
                    cache(descriptor) = buffer
                    counter += 1
                    Dim size As Long = values.LongLength * buffer.ElementSize
                    bytes += size
                    AllocationsTracker.Instance.markAllocated(AllocationKind.CONSTANT, 0, size)
                End If
            End SyncLock
            Return buffer
        End Function

        ''' <summary>A constant buffer of <paramref name="dataType"/> holding the values of <paramref name="array"/>.</summary>
        Public Overrides Function getConstantBuffer(ByVal array() As Integer, ByVal dataType As DataType) As DataBuffer
            Return lookup(New ArrayDescriptor(array, dataType), array, dataType)
        End Function

        ''' <summary>A constant buffer of <paramref name="dataType"/> holding the values of <paramref name="array"/>.</summary>
        Public Overrides Function getConstantBuffer(ByVal array() As Boolean, ByVal dataType As DataType) As DataBuffer
            Return lookup(New ArrayDescriptor(array, dataType), array, dataType)
        End Function

        ''' <summary>A constant buffer of <paramref name="dataType"/> holding the values of <paramref name="array"/>.</summary>
        Public Overrides Function getConstantBuffer(ByVal array() As Double, ByVal dataType As DataType) As DataBuffer
            Return lookup(New ArrayDescriptor(array, dataType), array, dataType)
        End Function

        ''' <summary>A constant buffer of <paramref name="dataType"/> holding the values of <paramref name="array"/>.</summary>
        Public Overrides Function getConstantBuffer(ByVal array() As Single, ByVal dataType As DataType) As DataBuffer
            Return lookup(New ArrayDescriptor(array, dataType), array, dataType)
        End Function

        ''' <summary>A constant buffer of <paramref name="dataType"/> holding the values of <paramref name="array"/>.</summary>
        Public Overrides Function getConstantBuffer(ByVal array() As Long, ByVal dataType As DataType) As DataBuffer
            Return lookup(New ArrayDescriptor(array, dataType), array, dataType)
        End Function

        ''' <summary>The bytes held by the cached buffers.</summary>
        Public Overrides ReadOnly Property CachedBytes As Long
            Get
                SyncLock gate
                    Return bytes
                End SyncLock
            End Get
        End Property

        ''' <summary>The number of cached buffers.</summary>
        Public ReadOnly Property CachedEntries As Integer
            Get
                SyncLock gate
                    Return counter
                End SyncLock
            End Get
        End Property

    End Class

End Namespace
