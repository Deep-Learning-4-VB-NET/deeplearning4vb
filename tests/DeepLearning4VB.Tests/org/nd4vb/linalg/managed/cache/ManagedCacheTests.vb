Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.cache
Imports AllocationKind = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.AllocationKind
Imports AllocationsTracker = DeepLearning4VB.org.nd4vb.linalg.api.memory.AllocationsTracker
Imports FakeArray = DeepLearning4VB.Tests.org.nd4vb.linalg.cache.FakeArray
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer
Imports TadPackT = DeepLearning4VB.org.nd4vb.linalg.api.shape.TadPack

Namespace org.nd4vb.linalg.managed.cache

    ''' <summary>Tests for <see cref="ManagedConstantHandler"/> and <see cref="ManagedTADManager"/>.</summary>
    <TestClass>
    Public Class ManagedCacheTests

#Region "Constant handler"

        <TestMethod>
        Public Sub testConstantBuffersAreCachedAndConstant()
            Dim handler As New ManagedConstantHandler()
            Dim trackerBefore As Long = AllocationsTracker.Instance.bytesOnDevice(AllocationKind.CONSTANT, 0)
            Dim a As DataBuffer = handler.getConstantBuffer(New Integer() {1, 2, 3}, DataType.INT)
            Dim b As DataBuffer = handler.getConstantBuffer(New Integer() {1, 2, 3}, DataType.INT)
            Assert.AreSame(a, b)
            assertTrue(a.Constant)
            assertFalse(a.Attached)
            assertArrayEquals(New Integer() {1, 2, 3}, a.asInt())
            assertEquals(12L, handler.CachedBytes)
            assertEquals(trackerBefore + 12, AllocationsTracker.Instance.bytesOnDevice(AllocationKind.CONSTANT, 0))

            ' the descriptor includes the data type and the values
            Dim asLong As DataBuffer = handler.getConstantBuffer(New Integer() {1, 2, 3}, DataType.LONG)
            Assert.AreNotSame(a, asLong)
            assertEquals(DataType.LONG, asLong.dataType())
            Assert.AreNotSame(a, handler.getConstantBuffer(New Integer() {1, 2, 4}, DataType.INT))
            assertEquals(12L + 24L + 12L, handler.CachedBytes)
            assertEquals(3, handler.CachedEntries)

            ' every overload
            assertEquals(DataType.LONG, handler.getConstantBuffer(New Long() {4, 5}, DataType.LONG).dataType())
            assertEquals(DataType.INT, handler.getConstantBuffer(New Long() {4, 5}, DataType.INT).dataType())
            assertArrayEquals(New Double() {0.5}, handler.getConstantBuffer(New Double() {0.5}, DataType.DOUBLE).asDouble(), 0.0)
            assertArrayEquals(New Single() {1.5F}, handler.getConstantBuffer(New Single() {1.5F}, DataType.FLOAT).asFloat(), 0.0F)
            assertArrayEquals(New Integer() {1, 0}, handler.getConstantBuffer(New Boolean() {True, False}, DataType.BOOL).asInt())
            Dim cachedBool As DataBuffer = handler.getConstantBuffer(New Boolean() {True, False}, DataType.BOOL)
            Assert.AreSame(cachedBool, handler.getConstantBuffer(New Boolean() {True, False}, DataType.BOOL))

            ' constants cannot be closed
            assertThrows(Of InvalidOperationException)(Sub() a.close())
            assertEquals(0L, handler.moveToConstantSpace(a))
            Assert.AreSame(a, handler.relocateConstantSpace(a))

            handler.purgeConstants()
            assertEquals(0L, handler.CachedBytes)
            assertEquals(0, handler.CachedEntries)
            assertEquals(trackerBefore, AllocationsTracker.Instance.bytesOnDevice(AllocationKind.CONSTANT, 0))
            Assert.AreNotSame(a, handler.getConstantBuffer(New Integer() {1, 2, 3}, DataType.INT))
            handler.purgeConstants()
        End Sub

        <TestMethod>
        Public Sub testConstantCacheIsThreadSafeAndBounded()
            Dim handler As New ManagedConstantHandler()
            Dim results(63) As DataBuffer
            Parallel.For(0, results.Length, Sub(i) results(i) = handler.getConstantBuffer(New Long() {7, 8, 9}, DataType.LONG))
            For Each r As DataBuffer In results
                Assert.AreSame(results(0), r)
            Next
            For i As Integer = 0 To ManagedConstantHandler.MAX_ENTRIES + 10
                handler.getConstantBuffer(New Integer() {i, -i}, DataType.INT)
            Next
            assertEquals(ManagedConstantHandler.MAX_ENTRIES, handler.CachedEntries)
            ' past the limit a new (still constant) buffer is created each time
            Dim x As DataBuffer = handler.getConstantBuffer(New Integer() {-1, -2, -3}, DataType.INT)
            assertTrue(x.Constant)
            Assert.AreNotSame(x, handler.getConstantBuffer(New Integer() {-1, -2, -3}, DataType.INT))
            handler.purgeConstants()
        End Sub

#End Region

#Region "TAD manager"

        Private Shared Function cOrder(ParamArray ByVal shape() As Long) As Long()
            Dim rank As Integer = shape.Length
            Dim info(rank * 2 + 3) As Long
            info(0) = rank
            Dim stride As Long = 1
            For i As Integer = rank - 1 To 0 Step -1
                info(1 + i) = shape(i)
                info(1 + rank + i) = stride
                stride *= shape(i)
            Next
            info(rank * 2 + 1) = 0
            info(rank * 2 + 2) = 1
            info(rank * 2 + 3) = AscW("c"c)
            Return info
        End Function

        <TestMethod>
        Public Sub testTadsOfAMatrix()
            Dim tm As New ManagedTADManager()
            ' a 2x3 'c' matrix: TADs along dimension 1 are its rows, along 0 its columns
            Dim rows As TadPackT = tm.tadShapeInfoAndOffsets(cOrder(2, 3), 1)
            assertArrayEquals(New Long() {0, 3}, rows.getTadOffsets().asLong())
            Dim rowInfo As Long() = rows.getTadShapeInfo().asLong()
            assertEquals(1L, rowInfo(0))
            assertEquals(3L, rowInfo(1))
            assertEquals(1L, rowInfo(2))
            assertTrue(rows.getTadShapeInfo().Constant)
            assertTrue(rows.getTadOffsets().Constant)

            Dim cols As TadPackT = tm.tadShapeInfoAndOffsets(cOrder(2, 3), 0)
            assertArrayEquals(New Long() {0, 1, 2}, cols.getTadOffsets().asLong())
            Dim colInfo As Long() = cols.getTadShapeInfo().asLong()
            assertEquals(2L, colInfo(1))
            assertEquals(3L, colInfo(2))

            ' negative dimensions count from the end; Nothing and Integer.MaxValue mean the whole array
            assertArrayEquals(New Long() {0, 3}, tm.tadShapeInfoAndOffsets(cOrder(2, 3), -1).getTadOffsets().asLong())
            assertArrayEquals(New Long() {0}, tm.tadShapeInfoAndOffsets(cOrder(2, 3), DirectCast(Nothing, Integer())).getTadOffsets().asLong())
            assertArrayEquals(New Long() {0}, tm.tadShapeInfoAndOffsets(cOrder(2, 3), Integer.MaxValue).getTadOffsets().asLong())
        End Sub

        <TestMethod>
        Public Sub testTadsOfA3dArray()
            Dim tm As New ManagedTADManager()
            ' shape 2x3x4, strides 12,4,1: TADs along {0,2} are 2x4 matrices at offsets 0, 4, 8
            Dim pack As TadPackT = tm.tadShapeInfoAndOffsets(cOrder(2, 3, 4), 2, 0)
            assertArrayEquals(New Long() {0, 4, 8}, pack.getTadOffsets().asLong())
            Dim info As Long() = pack.getTadShapeInfo().asLong()
            assertEquals(2L, info(0))
            assertEquals(2L, info(1))
            assertEquals(4L, info(2))
            assertEquals(12L, info(3))
            assertEquals(1L, info(4))
            ' TADs along dimension 1: 8 vectors of length 3 with stride 4
            Dim alongOne As TadPackT = tm.tadShapeInfoAndOffsets(cOrder(2, 3, 4), 1)
            assertArrayEquals(New Long() {0, 1, 2, 3, 12, 13, 14, 15}, alongOne.getTadOffsets().asLong())
            Dim v As Long() = alongOne.getTadShapeInfo().asLong()
            assertEquals(3L, v(1))
            assertEquals(4L, v(2))
        End Sub

        <TestMethod>
        Public Sub testTadsAreCachedByDescriptor()
            Dim tm As New ManagedTADManager()
            Dim array As New FakeArray(New ManagedDataBuffer(DataType.FLOAT, 6), cOrder(2, 3))
            Dim dims As Integer() = {1, 0}
            Dim first As TadPackT = tm.tadShapeInfoAndOffsets(array, dims)
            ' the caller's array is not sorted in place
            assertArrayEquals(New Integer() {1, 0}, dims)
            Assert.AreSame(first, tm.tadShapeInfoAndOffsets(array, 0, 1))
            assertEquals(1, tm.CachedEntries)
            assertEquals((first.getTadShapeInfo().length() + first.getTadOffsets().length()) * 8, tm.CachedBytes)

            Dim pair As Pair(Of DataBuffer, DataBuffer) = tm.getTADOnlyShapeInfo(array, 1)
            assertArrayEquals(New Long() {0, 3}, pair.Second.asLong())
            Dim samePair As Pair(Of DataBuffer, DataBuffer) = tm.getTADOnlyShapeInfo(array, 1)
            Assert.AreSame(pair.First, samePair.First)
            assertEquals(2, tm.CachedEntries)

            ' another array with the same shape information shares the cache entry
            Dim other As New FakeArray(New ManagedDataBuffer(DataType.FLOAT, 6), cOrder(2, 3))
            Assert.AreSame(pair.Second, tm.getTADOnlyShapeInfo(other, 1).Second)

            tm.purgeBuffers()
            assertEquals(0, tm.CachedEntries)
            assertEquals(0L, tm.CachedBytes)
            Assert.AreNotSame(pair.First, tm.getTADOnlyShapeInfo(array, 1).First)
            assertThrows(Of ArgumentNullException)(Sub() tm.tadShapeInfoAndOffsets(DirectCast(Nothing, Global.DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray), 1))
        End Sub

#End Region

    End Class

End Namespace
