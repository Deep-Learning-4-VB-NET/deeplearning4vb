' DeepLearning4VB: the managed backend's TAD cache (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.CpuTADManager.

Imports System
Imports System.Collections.Concurrent
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.buffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports KernelTadPack = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.TadPack
Imports TADManager = DeepLearning4VB.org.nd4vb.linalg.cache.TADManager
Imports TadDescriptor = DeepLearning4VB.org.nd4vb.linalg.cache.TadDescriptor
Imports TadPackT = DeepLearning4VB.org.nd4vb.linalg.api.shape.TadPack

Namespace org.nd4vb.linalg.managed.cache

    ''' <summary>
    ''' The TAD (tensor along dimension) manager of the managed backend (Java <c>CpuTADManager</c>, which asked libnd4j's
    ''' cached <c>tadShapeInfoAndOffsets</c>). It computes the TADs with <see cref="KernelTadPack.Create"/> and caches them
    ''' by <see cref="TadDescriptor"/> (the dimensions and the array's shape information); the result is a
    ''' <see cref="TadPackT"/> holding two constant LONG buffers: the shape information shared by every TAD and the
    ''' element offset of each TAD.
    ''' </summary>
    ''' <remarks>
    ''' <para>As in Java, the dimensions are sorted and Nothing means the whole array (Java passed
    ''' <c>{Integer.MAX_VALUE}</c>, which this manager also accepts). Negative dimensions count from the end. TAD offsets
    ''' are relative to the array's first element (add <c>array.offset()</c> for its position in the buffer). At most
    ''' <see cref="MAX_ENTRIES"/> packs are cached.</para>
    ''' <para>Differences from Java: <see cref="getTADOnlyShapeInfo"/> does not sort the caller's dimension array in place;
    ''' the cache lives here (Java's lived in libnd4j's <c>ConstantTadHelper</c>). Native-only members not ported:
    ''' <c>init(NativeOps, ConstantHandler)</c>.</para>
    ''' </remarks>
    Public Class ManagedTADManager
        Implements TADManager

        ''' <summary>The maximum number of cached packs.</summary>
        Public Const MAX_ENTRIES As Integer = 100

        Private cache As New ConcurrentDictionary(Of TadDescriptor, TadPackT)()
        Private counter As Integer = 0
        Private bytes As Long = 0
        Private ReadOnly gate As New Object()

        ''' <summary>Forgets every cached pack.</summary>
        Public Overridable Sub purgeBuffers() Implements TADManager.purgeBuffers
            SyncLock gate
                cache = New ConcurrentDictionary(Of TadDescriptor, TadPackT)()
                counter = 0
                bytes = 0
            End SyncLock
        End Sub

        ''' <summary>The TAD shape information and offsets of <paramref name="array"/> along <paramref name="dimension"/>, as a pair of buffers.</summary>
        Public Overridable Function getTADOnlyShapeInfo(ByVal array As INDArray, ParamArray ByVal dimension() As Integer) As Pair(Of DataBuffer, DataBuffer) Implements TADManager.getTADOnlyShapeInfo
            Dim pack As TadPackT = tadShapeInfoAndOffsets(array, dimension)
            Return New Pair(Of DataBuffer, DataBuffer)(pack.getTadShapeInfo(), pack.getTadOffsets())
        End Function

        ''' <summary>The TADs of <paramref name="array"/> along <paramref name="dimension"/> (cached).</summary>
        Public Overridable Function tadShapeInfoAndOffsets(ByVal array As INDArray, ParamArray ByVal dimension() As Integer) As TadPackT
            If array Is Nothing Then
                Throw New ArgumentNullException(NameOf(array))
            End If
            Dim dims As Integer() = normalize(dimension)
            Dim descriptor As New TadDescriptor(array, dims)
            Dim current As ConcurrentDictionary(Of TadDescriptor, TadPackT) = cache
            Dim pack As TadPackT = Nothing
            If current.TryGetValue(descriptor, pack) Then
                Return pack
            End If

            pack = tadShapeInfoAndOffsets(array.shapeInfoDataBuffer().asLong(), dims)

            SyncLock gate
                Dim existing As TadPackT = Nothing
                If current.TryGetValue(descriptor, existing) Then
                    Return existing
                End If
                If counter < MAX_ENTRIES AndAlso current Is cache Then
                    current(descriptor) = pack
                    counter += 1
                    bytes += (pack.getTadShapeInfo().length() + pack.getTadOffsets().length()) * 8
                End If
            End SyncLock
            Return pack
        End Function

        ''' <summary>
        ''' The TADs of the array described by <paramref name="shapeInfo"/> along <paramref name="dimension"/> (not cached):
        ''' two new constant LONG buffers.
        ''' </summary>
        Public Overridable Function tadShapeInfoAndOffsets(ByVal shapeInfo As Long(), ParamArray ByVal dimension() As Integer) As TadPackT
            If shapeInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(shapeInfo))
            End If
            Dim kernelPack As KernelTadPack = KernelTadPack.Create(shapeInfo, normalize(dimension))
            Dim shapeBuffer As New ManagedDataBuffer(DataType.LONG, kernelPack.TadShapeInfo)
            shapeBuffer.Constant = True
            Dim offsetsBuffer As New ManagedDataBuffer(DataType.LONG, kernelPack.TadOffsets)
            offsetsBuffer.Constant = True
            Return New TadPackT(shapeBuffer, offsetsBuffer)
        End Function

        ''' <summary>A sorted copy of the dimensions; Nothing or <c>{Integer.MAX_VALUE}</c> mean the whole array (no dimensions).</summary>
        Private Shared Function normalize(ByVal dimension() As Integer) As Integer()
            If dimension Is Nothing OrElse (dimension.Length >= 1 AndAlso dimension(0) = Integer.MaxValue) Then
                Return System.Array.Empty(Of Integer)()
            End If
            Dim dims As Integer() = DirectCast(dimension.Clone(), Integer())
            System.Array.Sort(dims)
            Return dims
        End Function

        ''' <summary>The bytes held by the cached packs.</summary>
        Public Overridable ReadOnly Property CachedBytes As Long Implements TADManager.CachedBytes
            Get
                SyncLock gate
                    Return bytes
                End SyncLock
            End Get
        End Property

        ''' <summary>The number of cached packs.</summary>
        Public ReadOnly Property CachedEntries As Integer
            Get
                SyncLock gate
                    Return counter
                End SyncLock
            End Get
        End Property

    End Class

End Namespace
