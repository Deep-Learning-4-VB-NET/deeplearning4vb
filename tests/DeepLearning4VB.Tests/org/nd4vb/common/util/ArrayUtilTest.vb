Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.util

Namespace org.nd4vb.common.util

    ''' <summary>The two invertPermutation cases are ported from the DL4J JUnit test; the rest exercise the converted half of ArrayUtil.</summary>
    <TestClass>
    Public Class ArrayUtilTest

        <TestMethod>
        Public Sub testInvertPermutationInt()
            assertArrayEquals(New Integer() {2, 4, 3, 0, 1}, ArrayUtil.invertPermutation(3, 4, 0, 2, 1))
        End Sub

        <TestMethod>
        Public Sub testInvertPermutationLong()
            assertArrayEquals(New Long() {2, 4, 3, 0, 1}, ArrayUtil.invertPermutation(3L, 4L, 0L, 2L, 1L))
        End Sub

        <TestMethod>
        Public Sub ProdSumRange()
            assertEquals(24, ArrayUtil.prod(2, 3, 4))
            assertEquals(24L, ArrayUtil.prodLong(2L, 3L, 4L))
            assertEquals(6, ArrayUtil.sum(New Integer() {1, 2, 3}))
            assertArrayEquals(New Integer() {0, 1, 2, 3, 4}, ArrayUtil.range(0, 5))
            ' as in the Java original, a descending range excludes 'from' and includes 'to'
            assertArrayEquals(New Integer() {4, 3, 2, 1, 0}, ArrayUtil.range(5, 0))
            assertArrayEquals(New Integer() {0, 2, 4}, ArrayUtil.range(0, 6, 2))
            assertArrayEquals(New Long() {0, 1, 2}, ArrayUtil.range(0L, 3L))
            assertEquals(0, ArrayUtil.range(3, 3).Length)
            assertArrayEquals(New Integer() {1, 2, 3}, ArrayUtil.combine(New Integer() {1, 2}, New Integer() {3}))
            assertArrayEquals(New Integer() {7, 7, 7}, ArrayUtil.nTimes(3, 7))
        End Sub

        <TestMethod>
        Public Sub FlattenAndReshape()
            Dim m = New Integer()() {New Integer() {1, 2, 3}, New Integer() {4, 5, 6}}
            assertArrayEquals(New Integer() {1, 2, 3, 4, 5, 6}, ArrayUtil.flatten(m))
            assertArrayEquals(New Integer() {1, 4, 2, 5, 3, 6}, ArrayUtil.flattenF(m))
            Dim back = ArrayUtil.reshapeInt(New Integer() {1, 2, 3, 4, 5, 6}, 2, 3)
            assertEquals(2, back.Length)
            assertArrayEquals(New Integer() {4, 5, 6}, back(1))
            Dim d3 = New Double()()() {New Double()() {New Double() {1, 2}, New Double() {3, 4}}, New Double()() {New Double() {5, 6}, New Double() {7, 8}}}
            assertArrayEquals(New Double() {1, 2, 3, 4, 5, 6, 7, 8}, ArrayUtil.flatten(d3), 0.0)
            assertArrayEquals(New Integer() {2, 2, 2}, ArrayUtil.arrayShape(d3))
            assertArrayEquals(New Integer() {2, 3}, ArrayUtil.arrayShape(m))
        End Sub

        <TestMethod>
        Public Sub SortingAndIndexHelpers()
            assertArrayEquals(New Integer() {1, 2, 0}, ArrayUtil.argsort(New Integer() {3, 1, 2}, True))
            assertArrayEquals(New Integer() {0, 2, 1}, ArrayUtil.argsort(New Integer() {3, 1, 2}, False))
            assertEquals(1, ArrayUtil.argMax(New Integer() {3, 9, 2}))
            assertEquals(2, ArrayUtil.argMin(New Integer() {3, 9, 2}))
            assertEquals(9, ArrayUtil.max(New Integer() {3, 9, 2}))
            assertEquals(2, ArrayUtil.min(New Integer() {3, 9, 2}))
            assertArrayEquals(New Integer() {1, 3}, ArrayUtil.removeIndex(New Integer() {1, 2, 3}, 1))
            assertArrayEquals(New Integer() {3, 2, 1}, ArrayUtil.reverseCopy(New Integer() {1, 2, 3}))
            assertArrayEquals(New Integer() {1, 3}, ArrayUtil.keep(New Integer() {1, 2, 3}, 0, 2))
            assertTrue(ArrayUtil.allUnique(New Integer() {1, 2, 3}))
            assertFalse(ArrayUtil.allUnique(New Integer() {1, 2, 1}))
            assertTrue(ArrayUtil.contains(New Integer() {1, 2, 3}, 2))
            assertFalse(ArrayUtil.contains(New Integer() {1, 2, 3}, 5))
            ' isZero is true when ANY element is zero (Java semantics)
            assertTrue(ArrayUtil.isZero(New Integer() {0, 0}))
            assertTrue(ArrayUtil.isZero(New Integer() {0, 1}))
            assertFalse(ArrayUtil.isZero(New Integer() {1, 2}))
            assertTrue(ArrayUtil.containsAnyNegative(New Integer() {0, -1}))
            assertTrue(ArrayUtil.isInverse(New Integer() {1, 2, 3}, New Integer() {3, 2, 1}))
            ' randomPermutation(n) permutes 1..n, as in the Java original
            Dim perm = ArrayUtil.randomPermutation(20)
            assertArrayEquals(ArrayUtil.range(1, 21), perm.OrderBy(Function(x) x).ToArray())
        End Sub

        <TestMethod>
        Public Sub StridesAndOffsets()
            assertArrayEquals(New Integer() {3, 1}, ArrayUtil.calcStrides(New Integer() {2, 3}))
            assertArrayEquals(New Integer() {1, 2}, ArrayUtil.calcStridesFortran(New Integer() {2, 3}))
            assertArrayEquals(New Long() {12L, 4L, 1L}, ArrayUtil.calcStrides(New Long() {2L, 3L, 4L}))
            assertTrue(ArrayUtil.isEmptyShape(New Integer() {2, 0}))
            assertFalse(ArrayUtil.isEmptyShape(New Integer() {2, 1}))
            assertFalse(ArrayUtil.isEmptyShape(New Long() {}))
        End Sub

        <TestMethod>
        Public Sub BigEndianByteConversionsRoundTrip()
            Dim ints = New Integer() {1, -2, Integer.MaxValue}
            Dim bytes = ArrayUtil.toByteArray(ints)
            assertEquals(12, bytes.Length)
            assertEquals(0, bytes(0))
            assertEquals(1, bytes(3))
            assertArrayEquals(ints, ArrayUtil.toIntArray(bytes))

            Dim doubles = New Double() {1.5, -2.25}
            assertArrayEquals(doubles, ArrayUtil.toDoubleArray(ArrayUtil.toByteArray(doubles)), 0.0)

            Dim floats = New Single() {1.5F, -2.25F}
            assertArrayEquals(floats, ArrayUtil.toFloatArray(ArrayUtil.toByteArray(floats)), 0.0F)

            Using ms As New MemoryStream()
                ArrayUtil.write(New Single() {1.0F, 2.0F}, New BinaryWriter(ms))
                ms.Position = 0
                assertArrayEquals(New Single() {1.0F, 2.0F}, ArrayUtil.readFloat(2, New BinaryReader(ms)), 0.0F)
            End Using
        End Sub

        <TestMethod>
        Public Sub HalfPrecisionConversions()
            ' IEEE-754 binary16 encodings
            assertEquals(CShort(&H3C00), ArrayUtil.fromFloat(1.0F))
            assertEquals(CShort(&H3C00), ArrayUtil.toHalf(1.0F))
            assertEquals(CShort(&HB800S), ArrayUtil.toHalf(-0.5F))
            assertEquals(CShort(&H7BFF), ArrayUtil.toHalf(65504.0F))
            assertEquals(CShort(0), ArrayUtil.toHalf(0.0))
            assertArrayEquals(New Short() {CShort(&H3C00), CShort(&H4000)}, ArrayUtil.toHalfs(New Single() {1.0F, 2.0F}))
            ' bfloat16 keeps the top 16 bits of the IEEE-754 single
            assertEquals(CShort(&H3F80), ArrayUtil.toBFloat16(1.0F))
            assertEquals(CShort(&HBF80S), ArrayUtil.toBFloat16(-1.0))
        End Sub

        <TestMethod>
        Public Sub ListAndMapHelpers()
            Dim map As New Dictionary(Of String, Integer) From {{"b", 2}, {"a", 1}, {"c", 3}}
            Dim sorted = ArrayUtil.sortMapByValue(map)
            assertEquals(New String() {"a", "b", "c"}, sorted.Keys.ToArray())
            Dim list As IList(Of Integer()) = New List(Of Integer()) From {New Integer() {1, 2}, New Integer() {3}}
            assertTrue(ArrayUtil.listOfIntsContains(list, New Integer() {3}))
            assertFalse(ArrayUtil.listOfIntsContains(list, New Integer() {4}))
            assertArrayEquals(New String() {"a", "b"}, ArrayUtil.filterNull("a", Nothing, "b"))
            Dim element = ArrayUtil.getRandomElement(New List(Of String) From {"only"})
            assertEquals("only", element)
            assertArrayEquals(New Integer() {1, 2, 3}, ArrayUtil.toInts(New Long() {1L, 2L, 3L}))
            assertArrayEquals(New Long() {1L, 2L}, ArrayUtil.toLongArray(New Integer() {1, 2}))
            assertArrayEquals(New Double() {1.0, 2.0}, ArrayUtil.toDoubles(New Integer() {1, 2}), 0.0)
        End Sub

    End Class

End Namespace
