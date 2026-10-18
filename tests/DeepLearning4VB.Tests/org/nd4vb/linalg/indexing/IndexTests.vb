Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.indexing
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape

Namespace org.nd4vb.linalg.indexing

    ''' <summary>
    ''' Index arithmetic without arrays: point, interval, all, new-axis and specified indexes, the <c>NDArrayIndex</c>
    ''' factories and resolve logic over shapes and shape information, and the <c>Indices</c> helpers. The expected values
    ''' follow the Java semantics (including the length of an interval with a stride).
    ''' </summary>
    <TestClass>
    Public Class IndexTests

        <TestMethod>
        Public Sub testPointIndex()
            Dim p As INDArrayIndex = NDArrayIndex.point(3)
            assertTrue(TypeOf p Is PointIndex)
            assertEquals(3L, p.offset())
            assertEquals(3L, p.end())
            assertEquals(1L, p.length())
            assertEquals(1L, p.stride())
            assertFalse(p.Interval)
            p.reverse()
            p.init(0, 5)
            assertEquals(3L, p.offset())
            assertEquals("Point(3)", p.ToString())
            assertEquals(New PointIndex(3), p)
            assertEquals(New PointIndex(3).GetHashCode(), p.GetHashCode())
            assertNotEquals(New PointIndex(4), p)
            assertEquals(62, New PointIndex(3).GetHashCode())
        End Sub

        <TestMethod>
        Public Sub testIntervalLengths()
            Dim a As INDArrayIndex = NDArrayIndex.interval(1, 4)
            assertTrue(TypeOf a Is IntervalIndex)
            assertTrue(a.Interval)
            assertEquals(1L, a.offset())
            assertEquals(4L, a.end())
            assertEquals(3L, a.length())
            assertEquals(1L, a.stride())
            assertFalse(DirectCast(a, IntervalIndex).isInclusive())
            assertEquals("Interval(b=1,e=4,s=1)", a.ToString())

            ' stride 2 over [0, 7): 0, 2, 4, 6
            Dim s As INDArrayIndex = NDArrayIndex.interval(0, 2, 7)
            assertEquals(4L, s.length())
            assertEquals(2L, s.stride())

            ' inclusive end
            Dim inc As INDArrayIndex = NDArrayIndex.interval(1L, 3L, True)
            assertEquals(3L, inc.length())
            assertTrue(DirectCast(inc, IntervalIndex).isInclusive())
            assertEquals("Interval(b=1,e=3,s=1,inclusive)", inc.ToString())

            ' Java quirks of interval(begin, stride, end): an empty range grows by one, and a range of one with a stride
            ' greater than 1 multiplies the end by the stride
            assertEquals(1L, NDArrayIndex.interval(3L, 1L, 3L).length())
            Dim q As INDArrayIndex = NDArrayIndex.interval(1L, 2L, 2L)
            assertEquals(4L, q.end())
            assertEquals(2L, q.length())

            ' negative bounds counted from max
            Dim n As INDArrayIndex = NDArrayIndex.interval(-3L, 1L, -1L, 10L)
            assertEquals(7L, n.offset())
            assertEquals(9L, n.end())
            assertEquals(2L, n.length())

            assertThrows(Of ArgumentException)(Sub() NDArrayIndex.interval(2L, 1L))
            assertThrows(Of ArgumentException)(Sub() NDArrayIndex.interval(2, 1, 1, False))
        End Sub

        <TestMethod>
        Public Sub testIntervalInit()
            Dim i As New IntervalIndex(False, 1)
            i.init(-2, -1, 5)
            assertEquals(3L, i.offset())
            assertEquals(4L, i.end())
            assertEquals(1L, i.length())

            Dim j As New IntervalIndex(True, 2)
            j.init(0, 6)
            assertEquals(4L, j.length())
            assertThrows(Of ArgumentException)(Sub() j.init(-1, 3))

            j.reverse()
            assertEquals(6L, j.offset())
            assertEquals(0L, j.end())

            Dim a As INDArrayIndex = NDArrayIndex.interval(1, 4)
            Dim b As INDArrayIndex = NDArrayIndex.interval(1L, 4L)
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertNotEquals(a, NDArrayIndex.interval(1L, 4L, True))
            assertNotEquals(a, NDArrayIndex.interval(1L, 2L, 4L))
        End Sub

        <TestMethod>
        Public Sub testAllNewAxisAndSpecified()
            Dim all As INDArrayIndex = NDArrayIndex.all()
            assertTrue(TypeOf all Is NDArrayIndexAll)
            assertTrue(TypeOf all Is IntervalIndex)
            assertTrue(all.Interval)
            assertEquals("all()", all.ToString())

            Dim axis As INDArrayIndex = NDArrayIndex.newAxis()
            assertTrue(TypeOf axis Is NewAxis)
            assertSame(axis, NDArrayIndex.newAxis())
            assertEquals(0L, axis.length())
            assertEquals(0L, axis.offset())
            assertEquals(1L, axis.stride())
            assertFalse(axis.Interval)
            assertEquals("newAxis()", axis.ToString())

            Dim spec As INDArrayIndex = NDArrayIndex.indices(1, 3, 5)
            assertTrue(TypeOf spec Is SpecifiedIndex)
            assertEquals(3L, spec.length())
            assertEquals(1L, spec.offset())
            assertEquals(5L, spec.end())
            assertEquals(1L, spec.stride())
            assertFalse(spec.Interval)
            assertEquals("SpecifiedIndex(indexes=[1, 3, 5])", spec.ToString())
            assertEquals(New SpecifiedIndex(1, 3, 5), spec)
            assertEquals(New SpecifiedIndex(New Integer() {1, 3, 5}), spec)
            assertEquals(New SpecifiedIndex(1, 3, 5).GetHashCode(), spec.GetHashCode())
            assertArrayEquals(New Long() {1, 3, 5}, DirectCast(spec, SpecifiedIndex).getIndexes())
        End Sub

        <TestMethod>
        Public Sub testFactoriesAndCounts()
            Dim pts As INDArrayIndex() = NDArrayIndex.indexesFor(2, 3)
            assertEquals(2, pts.Length)
            assertEquals(New PointIndex(2), pts(0))
            assertEquals(New PointIndex(3), pts(1))

            Dim copies As INDArrayIndex() = NDArrayIndex.nTimes(NDArrayIndex.point(1), 3)
            assertEquals(3, copies.Length)
            assertSame(copies(0), copies(2))

            Dim mixed As INDArrayIndex() = {NDArrayIndex.point(0), NDArrayIndex.newAxis(), NDArrayIndex.all(), NDArrayIndex.point(1), NDArrayIndex.newAxis()}
            assertEquals(2, NDArrayIndex.numPoints(mixed))
            assertEquals(2, NDArrayIndex.numNewAxis(mixed))
            Dim info As New IndexInfo(mixed)
            assertEquals(2, info.NumPoints)
            assertEquals(2, info.NumNewAxes)

            assertEquals(5L, NDArrayIndex.offset(New Long() {3, 1}, New Long() {1, 2}))
            assertEquals(5L, NDArrayIndex.offset(New Integer() {3, 1}, New Long() {1, 2}))

            Dim cover As INDArrayIndex() = NDArrayIndex.createCoveringShape(New Long() {2, 3})
            assertEquals(2L, cover(0).length())
            assertEquals(3L, cover(1).length())
            Dim coverInt As INDArrayIndex() = NDArrayIndex.createCoveringShape(New Integer() {4})
            assertEquals(NDArrayIndex.interval(0, 4), coverInt(0))

            Dim ranges As INDArrayIndex() = NDArrayIndex.rangeOfLength(New INDArrayIndex() {NDArrayIndex.interval(2, 5), NDArrayIndex.indices(4, 8)})
            assertEquals(NDArrayIndex.interval(0L, 3L), ranges(0))
            assertEquals(NDArrayIndex.interval(0L, 2L), ranges(1))
        End Sub

        <TestMethod>
        Public Sub testResolveOverShapes()
            ' a matrix with one index: the index applies to the rows, all() to the columns
            Dim r As INDArrayIndex() = NDArrayIndex.resolve(New Long() {2, 3}, NDArrayIndex.point(1))
            assertEquals(2, r.Length)
            assertEquals(New PointIndex(1), r(0))
            assertTrue(TypeOf r(1) Is NDArrayIndexAll)

            ' a row vector with one index: point(0) for the row, the index for the columns
            Dim v As INDArrayIndex() = NDArrayIndex.resolve(New Long() {1, 5}, NDArrayIndex.interval(1, 3))
            assertEquals(2, v.Length)
            assertEquals(New PointIndex(0), v(0))
            assertEquals(NDArrayIndex.interval(1, 3), v(1))

            ' missing indexes are filled with all()
            Dim f As INDArrayIndex() = NDArrayIndex.resolve(New Long() {2, 3, 4}, NDArrayIndex.interval(0, 1))
            assertEquals(3, f.Length)
            assertEquals(NDArrayIndex.interval(0, 1), f(0))
            assertTrue(TypeOf f(1) Is NDArrayIndexAll)
            assertTrue(TypeOf f(2) Is NDArrayIndexAll)

            ' new axes add dimensions
            Dim a As INDArrayIndex() = NDArrayIndex.resolve(New Long() {2, 3, 4}, NDArrayIndex.newAxis(), NDArrayIndex.point(0))
            assertEquals(4, a.Length)
            assertTrue(TypeOf a(0) Is NewAxis)
            assertTrue(TypeOf a(3) Is NDArrayIndexAll)

            ' as many indexes as dimensions are validated and kept
            Dim full As INDArrayIndex() = NDArrayIndex.resolve(New Integer() {2, 3}, NDArrayIndex.point(1), NDArrayIndex.interval(0, 3))
            assertEquals(New PointIndex(1), full(0))
            assertEquals(NDArrayIndex.interval(0, 3), full(1))

            ' out of range
            assertThrows(Of ArgumentException)(Sub() NDArrayIndex.resolve(New Long() {2, 3}, NDArrayIndex.point(5), NDArrayIndex.all()))
            assertThrows(Of ArgumentException)(Sub() NDArrayIndex.resolve(New Long() {2, 3}, NDArrayIndex.interval(0, 5), NDArrayIndex.all()))
        End Sub

        <TestMethod>
        Public Sub testResolveOverShapeInfo()
            Dim info As Long() = ShapeT.createShapeInfoArray(New Long() {2, 3}, New Long() {3, 1}, 1, "c"c, 8192L)

            ' with a specified index, all() and intervals become specified indexes
            Dim s As INDArrayIndex() = NDArrayIndex.resolveLong(info, NDArrayIndex.indices(0, 1), NDArrayIndex.all())
            assertEquals(New SpecifiedIndex(0, 1), s(0))
            assertEquals(New SpecifiedIndex(0, 1, 2), s(1))
            Dim s2 As INDArrayIndex() = NDArrayIndex.resolveLong(info, NDArrayIndex.interval(0, 2, 4), NDArrayIndex.indices(2))
            assertEquals(New SpecifiedIndex(0, 2), s2(0))
            ' Java quirk of ArrayUtil.range(begin, end, stride): (end - begin) \ stride positions, so [0, 3) by 2 gives {0}
            Dim s3 As INDArrayIndex() = NDArrayIndex.resolveLong(info, NDArrayIndex.interval(0, 2, 3), NDArrayIndex.indices(2))
            assertEquals(New SpecifiedIndex(0L), s3(0))

            ' without, as resolve(shape, ...)
            Dim r As INDArrayIndex() = NDArrayIndex.resolveLong(info, NDArrayIndex.point(1))
            assertEquals(New PointIndex(1), r(0))
            assertTrue(TypeOf r(1) Is NDArrayIndexAll)

            ' rank 1 with one index is returned as it is
            Dim info1 As Long() = ShapeT.createShapeInfoArray(New Long() {5}, New Long() {1}, 1, "c"c, 8192L)
            Dim one As INDArrayIndex() = {NDArrayIndex.interval(1, 2)}
            assertSame(one, NDArrayIndex.resolveLong(info1, one))
        End Sub

        <TestMethod>
        Public Sub testResolveWithAllIndex()
            Dim allIndex As INDArrayIndex() = {NDArrayIndex.all(), NDArrayIndex.all(), NDArrayIndex.all()}
            Dim r As INDArrayIndex() = NDArrayIndex.resolve(allIndex, NDArrayIndex.point(1), NDArrayIndex.newAxis())
            assertEquals(4, r.Length)
            assertEquals(New PointIndex(1), r(0))
            assertTrue(TypeOf r(1) Is NewAxis)
            assertTrue(TypeOf r(2) Is NDArrayIndexAll)
            assertTrue(TypeOf r(3) Is NDArrayIndexAll)
        End Sub

        <TestMethod>
        Public Sub testIndicesOffsetsAndShapes()
            assertArrayEquals(New Long() {1, 1}, Indices.offsets(New Long() {2, 3}, NDArrayIndex.point(1), NDArrayIndex.interval(1, 3)))
            assertArrayEquals(New Long() {2, 0}, Indices.offsets(New Long() {4}, NDArrayIndex.point(2)))
            assertArrayEquals(New Long() {1, 0, 0}, Indices.offsets(New Long() {2, 3, 4}, NDArrayIndex.point(1)))

            assertArrayEquals(New Long() {2, 4}, Indices.shape(New Long() {2, 3, 4}, NDArrayIndex.point(0), NDArrayIndex.interval(0, 2), NDArrayIndex.all()))
            ' one point on a matrix keeps a row vector
            assertArrayEquals(New Long() {1, 3}, Indices.shape(New Long() {2, 3}, NDArrayIndex.point(1)))
            ' new axes before all() are prepended, later ones inserted
            assertArrayEquals(New Long() {1, 3, 4}, Indices.shape(New Long() {3, 4}, NDArrayIndex.newAxis(), NDArrayIndex.all(), NDArrayIndex.all()))
            assertArrayEquals(New Long() {3, 1, 4}, Indices.shape(New Long() {3, 4}, NDArrayIndex.all(), NDArrayIndex.newAxis(), NDArrayIndex.all()))
            assertArrayEquals(New Long() {2, 3}, Indices.shape(New Long() {5, 3}, NDArrayIndex.indices(0, 4)))
            assertArrayEquals(New Integer() {2, 4}, Indices.shape(New Integer() {2, 3, 4}, NDArrayIndex.point(0), NDArrayIndex.interval(0, 2), NDArrayIndex.all()))

            ' the lengths of the indexes, dropping zero lengths (new axes)
            assertArrayEquals(New Integer() {2, 1}, Indices.shape(NDArrayIndex.interval(0, 2), NDArrayIndex.point(1), NDArrayIndex.newAxis()))
        End Sub

        <TestMethod>
        Public Sub testIndicesFillInAndAdjust()
            Dim f As INDArrayIndex() = Indices.fillIn(New Integer() {2, 3, 4}, NDArrayIndex.point(0))
            assertEquals(3, f.Length)
            assertEquals(New PointIndex(0), f(0))
            assertEquals(NDArrayIndex.interval(0, 3), f(1))
            assertEquals(NDArrayIndex.interval(0, 4), f(2))

            Dim same As INDArrayIndex() = {NDArrayIndex.point(0), NDArrayIndex.point(1)}
            assertSame(same, Indices.fillIn(New Integer() {2, 3}, same))

            Dim pruned As INDArrayIndex() = Indices.adjustIndices(New Integer() {2, 3}, NDArrayIndex.point(0), NDArrayIndex.point(1), NDArrayIndex.point(2))
            assertEquals(2, pruned.Length)
            Dim vector As INDArrayIndex() = {NDArrayIndex.point(1)}
            assertSame(vector, Indices.adjustIndices(New Integer() {5}, vector))
            assertEquals(2, Indices.adjustIndices(New Integer() {2, 3}, NDArrayIndex.point(0)).Length)

            assertTrue(Indices.isContiguous(New Integer() {1, 2, 3}, 1))
            assertFalse(Indices.isContiguous(New Integer() {1, 3}, 1))
            assertTrue(Indices.isContiguous(New Integer() {}, 1))
        End Sub

        <TestMethod>
        Public Sub testSpecifiedIndexIterate()
            Dim idx As INDArrayIndex() = {NDArrayIndex.indices(0, 1), NDArrayIndex.point(2)}
            Dim combos As List(Of IList(Of IList(Of Long))) = SpecifiedIndex.iterate(idx).ToList()
            assertEquals(2, combos.Count)
            assertArrayEquals(New Long() {0}, combos(0)(0).ToArray())
            assertArrayEquals(New Long() {2}, combos(0)(1).ToArray())
            assertArrayEquals(New Long() {1}, combos(1)(0).ToArray())
            ' as in Java, the point index was replaced in the array
            assertTrue(TypeOf idx(1) Is SpecifiedIndex)

            Dim grid As List(Of IList(Of IList(Of Long))) = SpecifiedIndex.iterate(NDArrayIndex.indices(0, 1), NDArrayIndex.indices(5, 6, 7)).ToList()
            assertEquals(6, grid.Count)
            assertEquals("0,5;0,6;0,7;1,5;1,6;1,7", String.Join(";", grid.Select(Function(c) c(0)(0) & "," & c(1)(0))))

            Dim sparse As List(Of IList(Of IList(Of Long))) = SpecifiedIndex.iterateOverSparse(NDArrayIndex.indices(5, 7)).ToList()
            assertEquals(2, sparse.Count)
            assertArrayEquals(New Long() {5, 0}, sparse(0)(0).ToArray())
            assertArrayEquals(New Long() {7, 1}, sparse(1)(0).ToArray())

            assertThrows(Of InvalidCastException)(Sub() SpecifiedIndex.iterate(NDArrayIndex.interval(0, 2)).ToList())
        End Sub

        Private Shared Sub assertSame(ByVal expected As Object, ByVal actual As Object)
            Assert.AreSame(expected, actual)
        End Sub

    End Class

End Namespace
