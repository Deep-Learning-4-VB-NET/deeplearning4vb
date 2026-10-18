Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports ArrayOptionsHelper = DeepLearning4VB.org.nd4vb.linalg.api.shape.options.ArrayOptionsHelper
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports ShapeInfoHelper = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.ShapeInfoHelper

Namespace org.nd4vb.linalg.managed

    ''' <summary>
    ''' The managed shape information provider: the layout of the arrays it builds, its cache (keyed by shape, strides,
    ''' order, element-wise stride and extras), the entry limit, purging and thread safety. The buffers come from a test
    ''' factory that records the arrays it was given (the default factory, <c>Nd4vb.DataBufferFactory</c>, arrives in
    ''' phase 2 wave 3).
    ''' </summary>
    <TestClass>
    Public Class ManagedShapeInfoProviderTests

        Private created As ConcurrentBag(Of Long())

        Private Function newProvider() As ManagedShapeInfoProvider
            created = New ConcurrentBag(Of Long())()
            Return New ManagedShapeInfoProvider(Function(info)
                                                    created.Add(info)
                                                    Return Nothing
                                                End Function)
        End Function

        <TestMethod>
        Public Sub testLayout()
            Dim p As ManagedShapeInfoProvider = newProvider()
            Dim r As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {5, 3}, New Long() {3, 1}, 1, "c"c, DataType.DOUBLE)
            ' upstream LongShapeTests.testLongShape_1
            assertArrayEquals(New Long() {2, 5, 3, 3, 1, 16384, 1, 99}, r.Second)
            assertEquals(1, created.Count)
            assertArrayEquals(r.Second, created.Single())
            ' the buffer factory gets its own copy
            assertFalse(Object.ReferenceEquals(r.Second, created.Single()))

            ' what the kernels read
            assertArrayEquals(ShapeInfoHelper.Create(New Long() {5, 3}, "c"c, ArrayOptionsHelper.DTYPE_DOUBLE_BIT), r.Second)

            ' a negative element-wise stride is stored as 0
            Dim neg As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {4}, New Long() {2}, -1, "c"c, DataType.FLOAT)
            assertEquals(0L, ShapeT.elementWiseStride(neg.Second))

            ' scalars
            Dim scalar As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {}, New Long() {}, 1, "c"c, DataType.INT)
            assertArrayEquals(New Long() {0, ArrayOptionsHelper.DTYPE_INT_BIT, 1, 99}, scalar.Second)
        End Sub

        <TestMethod>
        Public Sub testEmptyFlagAndExtras()
            Dim p As ManagedShapeInfoProvider = newProvider()
            Dim e As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {2, 2}, New Long() {2, 1}, 1, "c"c, DataType.FLOAT, True)
            assertTrue(ShapeT.isEmpty(e.Second))
            assertEquals(DataType.FLOAT, ArrayOptionsHelper.dataType(e.Second))

            Dim z As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {0, 3}, New Long() {3, 1}, 1, "f"c, DataType.LONG, False)
            assertTrue(ShapeT.isEmpty(z.Second))
            assertEquals("f"c, ShapeT.order(z.Second))

            Dim notEmpty As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {2, 2}, New Long() {2, 1}, 1, "c"c, DataType.FLOAT, False)
            assertFalse(ShapeT.isEmpty(notEmpty.Second))
            ' the same descriptor through the extras overload hits the cache
            Assert.AreSame(notEmpty, p.createShapeInformation(New Long() {2, 2}, New Long() {2, 1}, 1, "c"c, ArrayOptionsHelper.DTYPE_FLOAT_BIT))
            Assert.AreSame(notEmpty, p.createShapeInformation(New Long() {2, 2}, New Long() {2, 1}, 1, "c"c, DataType.FLOAT))
            assertEquals(3, p.CachedEntries)
        End Sub

        <TestMethod>
        Public Sub testCacheKeys()
            Dim p As ManagedShapeInfoProvider = newProvider()
            Dim a As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {2, 3}, New Long() {3, 1}, 1, "c"c, DataType.FLOAT)
            Assert.AreSame(a, p.createShapeInformation(New Long() {2, 3}, New Long() {3, 1}, 1, "c"c, DataType.FLOAT))
            assertEquals(1, created.Count)

            ' every part of the descriptor is part of the key
            Assert.AreNotSame(a, p.createShapeInformation(New Long() {2, 3}, New Long() {3, 1}, 1, "c"c, DataType.DOUBLE))
            Assert.AreNotSame(a, p.createShapeInformation(New Long() {3, 2}, New Long() {2, 1}, 1, "c"c, DataType.FLOAT))
            Assert.AreNotSame(a, p.createShapeInformation(New Long() {2, 3}, New Long() {1, 2}, 1, "f"c, DataType.FLOAT))
            Assert.AreNotSame(a, p.createShapeInformation(New Long() {2, 3}, New Long() {3, 1}, 1, "f"c, DataType.FLOAT))
            Assert.AreNotSame(a, p.createShapeInformation(New Long() {2, 3}, New Long() {3, 1}, 0, "c"c, DataType.FLOAT))
            assertEquals(6, p.CachedEntries)
            assertEquals(6, created.Count)
            ' 8 longs per rank-2 shape information, counted twice as in Java
            assertEquals(6L * 8 * 8 * 2, p.CachedBytes)

            ' changing the caller's arrays afterwards does not change the key
            Dim shape As Long() = {4, 4}
            Dim stride As Long() = {4, 1}
            Dim b As Pair(Of DataBuffer, Long()) = p.createShapeInformation(shape, stride, 1, "c"c, DataType.FLOAT)
            shape(0) = 5
            Assert.AreSame(b, p.createShapeInformation(New Long() {4, 4}, New Long() {4, 1}, 1, "c"c, DataType.FLOAT))
        End Sub

        <TestMethod>
        Public Sub testEntryLimitAndPurge()
            Dim p As ManagedShapeInfoProvider = newProvider()
            For i As Integer = 1 To ManagedShapeInfoProvider.MAX_ENTRIES + 10
                p.createShapeInformation(New Long() {i}, New Long() {1}, 1, "c"c, DataType.FLOAT)
            Next i
            assertEquals(ManagedShapeInfoProvider.MAX_ENTRIES, p.CachedEntries)
            assertEquals(CLng(ManagedShapeInfoProvider.MAX_ENTRIES) * 6 * 8 * 2, p.CachedBytes)

            ' beyond the limit, each request builds a new buffer
            Dim before As Integer = created.Count
            Dim x As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {5000}, New Long() {1}, 1, "c"c, DataType.FLOAT)
            Dim y As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {5000}, New Long() {1}, 1, "c"c, DataType.FLOAT)
            Assert.AreNotSame(x, y)
            assertArrayEquals(x.Second, y.Second)
            assertEquals(before + 2, created.Count)
            ' cached entries are still served from the cache
            Dim c1 As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {1}, New Long() {1}, 1, "c"c, DataType.FLOAT)
            Assert.AreSame(c1, p.createShapeInformation(New Long() {1}, New Long() {1}, 1, "c"c, DataType.FLOAT))

            p.purgeCache()
            assertEquals(0, p.CachedEntries)
            assertEquals(0L, p.CachedBytes)
            Dim again As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {5000}, New Long() {1}, 1, "c"c, DataType.FLOAT)
            Assert.AreSame(again, p.createShapeInformation(New Long() {5000}, New Long() {1}, 1, "c"c, DataType.FLOAT))
            assertEquals(1, p.CachedEntries)
        End Sub

        <TestMethod>
        Public Sub testConcurrentRequestsShareOneBuffer()
            Dim p As ManagedShapeInfoProvider = newProvider()
            Dim results As New ConcurrentDictionary(Of Integer, ConcurrentBag(Of Pair(Of DataBuffer, Long())))()
            Parallel.For(0, 2000, Sub(i)
                                      Dim k As Integer = i Mod 10
                                      Dim r As Pair(Of DataBuffer, Long()) = p.createShapeInformation(New Long() {k + 1, 2}, New Long() {2, 1}, 1, "c"c, DataType.FLOAT)
                                      results.GetOrAdd(k, Function(key) New ConcurrentBag(Of Pair(Of DataBuffer, Long()))()).Add(r)
                                  End Sub)
            assertEquals(10, created.Count)
            assertEquals(10, p.CachedEntries)
            For Each kv As KeyValuePair(Of Integer, ConcurrentBag(Of Pair(Of DataBuffer, Long()))) In results
                Dim first As Pair(Of DataBuffer, Long()) = Nothing
                kv.Value.TryPeek(first)
                For Each r As Pair(Of DataBuffer, Long()) In kv.Value
                    Assert.AreSame(first, r)
                Next r
                assertEquals(CLng(kv.Key + 1), ShapeT.size(first.Second, 0))
            Next kv
        End Sub

        <TestMethod>
        Public Sub testArgumentChecks()
            Dim p As ManagedShapeInfoProvider = newProvider()
            assertThrows(Of ArgumentNullException)(Sub() p.createShapeInformation(Nothing, New Long() {}, 1, "c"c, 0L))
            assertThrows(Of ArgumentNullException)(Sub() p.createShapeInformation(New Long() {}, Nothing, 1, "c"c, 0L))
            assertThrows(Of ArgumentNullException)(Sub() p.createShapeInformation(Nothing, New Long() {}, 1, "c"c, DataType.FLOAT, False))
            assertThrows(Of ArgumentNullException)(Sub()
                                                         Dim unused As New ManagedShapeInfoProvider(Nothing)
                                                     End Sub)
        End Sub

    End Class

End Namespace
