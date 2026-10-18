' DeepLearning4VB: the managed backend's shape information provider (phase 2, see docs/PHASE2_DESIGN.md).
' Counterpart of nd4j-native's org.nd4j.linalg.cpu.nativecpu.DirectShapeInfoProvider.

Imports System
Imports System.Collections.Concurrent
Imports System.Threading
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports AllocationsTracker = DeepLearning4VB.org.nd4vb.linalg.api.memory.AllocationsTracker
Imports AllocationKind = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.AllocationKind
Imports BaseShapeInfoProvider = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.BaseShapeInfoProvider
Imports ArrayOptionsHelper = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayOptionsHelper
Imports ArrayType = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayType
Imports LongShapeDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.shape.LongShapeDescriptor
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

Namespace org.nd4vb.linalg.managed

    ''' <summary>
    ''' A thread-safe cache of constant shape information buffers, keyed by shape, strides, order, element-wise stride and
    ''' extras (data type and array type flags), like nd4j-native's <c>DirectShapeInfoProvider</c>.
    ''' <para>
    ''' A shape information array is built with <see cref="ShapeT.createShapeInfoArray(Long(), Long(), Long, Char, Long)"/>
    ''' (<c>[rank, shape..., stride..., extras, elementWiseStride, order]</c>, the layout the managed kernels read) and
    ''' wrapped in a LONG buffer from <c>Nd4vb.DataBufferFactory.createLong</c> that is marked constant. The returned pair
    ''' holds the buffer and the array; callers must not change either. The first <see cref="MAX_ENTRIES"/> distinct
    ''' descriptors are cached; later ones are built on every request, as in Java.
    ''' </para>
    ''' <para>
    ''' Differences from <c>DirectShapeInfoProvider</c>: the buffer is built in managed code instead of by libnd4j (a
    ''' negative element-wise stride is stored as 0, as Java did before calling native code); the overload taking a data
    ''' type and an empty flag goes through the cache too (Java built those buffers through <c>Shape</c> and the
    ''' executioner without caching), marking the array empty when the flag is set or a dimension is 0;
    ''' <see cref="purgeCache"/> really empties the cache (Java's cleared an unused map) and releases the tracked bytes.
    ''' </para>
    ''' </summary>
    Public Class ManagedShapeInfoProvider
        Inherits BaseShapeInfoProvider

        ''' <summary>The number of distinct descriptors that are cached (Java <c>MAX_ENTRIES</c>).</summary>
        Public Const MAX_ENTRIES As Integer = 1000

        Private longCache As New ConcurrentDictionary(Of LongShapeDescriptor, Pair(Of DataBuffer, Long()))()
        Private counter As Integer = 0
        Private ReadOnly cacheLock As New Object()
        Private ReadOnly bufferFactory As Func(Of Long(), DataBuffer)

        ''' <summary>A provider whose buffers come from <c>Nd4vb.DataBufferFactory</c>.</summary>
        Public Sub New()
            Me.New(AddressOf createConstantBuffer)
        End Sub

        ''' <summary>A provider whose buffers come from <paramref name="bufferFactory"/> (for tests).</summary>
        ''' <param name="bufferFactory"> wraps a shape information array in a buffer </param>
        Friend Sub New(ByVal bufferFactory As Func(Of Long(), DataBuffer))
            If bufferFactory Is Nothing Then
                Throw New ArgumentNullException(NameOf(bufferFactory))
            End If
            Me.bufferFactory = bufferFactory
        End Sub

        ''' <summary>The number of cached descriptors.</summary>
        Public ReadOnly Property CachedEntries As Integer
            Get
                Return longCache.Count
            End Get
        End Property

        ''' <summary>
        ''' The shape information for the given shape, strides, element-wise stride, order and data type (Java
        ''' <c>DirectShapeInfoProvider.createShapeInformation(long[], long[], long, char, DataType)</c>).
        ''' </summary>
        Public Overridable Overloads Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dataType As DataType) As Pair(Of DataBuffer, Long())
            Dim extras As Long = 0
            extras = ArrayOptionsHelper.setOptionBit(extras, dataType)
            Return createShapeInformation(shape, stride, elementWiseStride, order, extras)
        End Function

        ''' <summary>
        ''' The shape information for the given shape, strides, element-wise stride, order and data type; the array is
        ''' marked empty when <paramref name="empty"/> is set or any dimension is 0.
        ''' </summary>
        Public Overrides Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal dataType As DataType, ByVal empty As Boolean) As Pair(Of DataBuffer, Long())
            If shape Is Nothing Then
                Throw New ArgumentNullException(NameOf(shape))
            End If
            Dim extras As Long = ArrayOptionsHelper.setOptionBit(0L, dataType)
            If empty OrElse ShapeT.containsZeros(shape) Then
                extras = ArrayOptionsHelper.setOptionBit(extras, ArrayType.EMPTY)
            End If
            Return createShapeInformation(shape, stride, elementWiseStride, order, extras)
        End Function

        ''' <summary>
        ''' The shape information for the given shape, strides, element-wise stride, order and extras, from the cache when
        ''' an equal descriptor was requested before.
        ''' </summary>
        Public Overrides Function createShapeInformation(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As Pair(Of DataBuffer, Long())
            If shape Is Nothing Then
                Throw New ArgumentNullException(NameOf(shape))
            End If
            If stride Is Nothing Then
                Throw New ArgumentNullException(NameOf(stride))
            End If
            ' We enforce offset to 0 in shapeBuffer, since we need it for cache efficiency + we don't actually use offset value @ native side
            ' We also enforce elementWiseStride = 0
            If elementWiseStride < 0 Then
                elementWiseStride = 0
            End If

            Dim descriptor As New LongShapeDescriptor(shape, stride, 0, elementWiseStride, order, extras)
            Dim cached As Pair(Of DataBuffer, Long()) = Nothing
            If longCache.TryGetValue(descriptor, cached) Then
                Return cached
            End If

            If Volatile.Read(counter) < MAX_ENTRIES Then
                SyncLock cacheLock
                    If longCache.TryGetValue(descriptor, cached) Then
                        Return cached
                    End If
                    If counter < MAX_ENTRIES Then
                        Dim buffer As Pair(Of DataBuffer, Long()) = build(shape, stride, elementWiseStride, order, extras)
                        longCache(descriptor) = buffer
                        Volatile.Write(counter, counter + 1)

                        Dim size As Long = CLng(buffer.Second.Length) * 8 * 2
                        Interlocked.Add(bytes, size)
                        AllocationsTracker.Instance.markAllocated(AllocationKind.CONSTANT, 0, size)
                        Return buffer
                    End If
                End SyncLock
            End If

            Return build(shape, stride, elementWiseStride, order, extras)
        End Function

        ''' <summary>
        ''' Empties the cache and releases the bytes it held.
        ''' </summary>
        Public Overrides Sub purgeCache()
            SyncLock cacheLock
                longCache = New ConcurrentDictionary(Of LongShapeDescriptor, Pair(Of DataBuffer, Long()))()
                Volatile.Write(counter, 0)
                Dim held As Long = Interlocked.Exchange(bytes, 0L)
                If held > 0 Then
                    AllocationsTracker.Instance.markReleased(AllocationKind.CONSTANT, 0, held)
                End If
            End SyncLock
        End Sub

        Private Function build(ByVal shape() As Long, ByVal stride() As Long, ByVal elementWiseStride As Long, ByVal order As Char, ByVal extras As Long) As Pair(Of DataBuffer, Long())
            Dim info() As Long = ShapeT.createShapeInfoArray(shape, stride, elementWiseStride, order, extras)
            Dim buffer As DataBuffer = bufferFactory(CType(info.Clone(), Long()))
            Return Pair.create(buffer, info)
        End Function

        ''' <summary>A constant LONG buffer holding <paramref name="info"/>, from <c>Nd4vb.DataBufferFactory</c>.</summary>
        Private Shared Function createConstantBuffer(ByVal info() As Long) As DataBuffer
            Dim buffer As DataBuffer = Nd4vbT.DataBufferFactory.createLong(info)
            buffer.Constant = True
            Return buffer
        End Function

    End Class

End Namespace
