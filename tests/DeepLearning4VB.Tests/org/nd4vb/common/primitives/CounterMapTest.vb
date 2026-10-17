Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.primitives

Namespace org.nd4vb.common.primitives

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class CounterMapTest

        ''' <summary>
        ''' The Java test relied on the (unspecified) iteration order of a concurrent hash map;
        ''' here the pairs are sorted before being compared.
        ''' </summary>
        <TestMethod>
        Public Sub testIterator()
            Dim counterMap As New CounterMap(Of Integer, Integer)()

            counterMap.incrementCount(0, 0, 1)
            counterMap.incrementCount(0, 1, 1)
            counterMap.incrementCount(0, 2, 1)
            counterMap.incrementCount(1, 0, 1)
            counterMap.incrementCount(1, 1, 1)
            counterMap.incrementCount(1, 2, 1)

            Dim pairs As New List(Of Pair(Of Integer, Integer))()
            Dim iterator As IEnumerator(Of Pair(Of Integer, Integer)) = counterMap.getIterator()
            Do While iterator.MoveNext()
                pairs.Add(iterator.Current)
            Loop
            assertEquals(6, pairs.Count)

            pairs = pairs.OrderBy(Function(p) p.First).ThenBy(Function(p) p.Second).ToList()
            Dim expected = {{0, 0}, {0, 1}, {0, 2}, {1, 0}, {1, 1}, {1, 2}}
            For i As Integer = 0 To 5
                assertEquals(expected(i, 0), pairs(i).First)
                assertEquals(expected(i, 1), pairs(i).Second)
            Next i

            ' For Each works too, since CounterMap is enumerable
            assertEquals(6, counterMap.Count())
        End Sub

        <TestMethod>
        Public Sub testIncrementAll()
            Dim counterMapA As New CounterMap(Of Integer, Integer)()

            counterMapA.incrementCount(0, 0, 1)
            counterMapA.incrementCount(0, 1, 1)
            counterMapA.incrementCount(0, 2, 1)
            counterMapA.incrementCount(1, 0, 1)
            counterMapA.incrementCount(1, 1, 1)
            counterMapA.incrementCount(1, 2, 1)

            Dim counterMapB As New CounterMap(Of Integer, Integer)()

            counterMapB.incrementCount(1, 1, 1)
            counterMapB.incrementCount(2, 1, 1)

            counterMapA.incrementAll(counterMapB)

            assertEquals(2.0, counterMapA.getCount(1, 1), 0.00001)
            assertEquals(1.0, counterMapA.getCount(2, 1), 0.00001)
            assertEquals(1.0, counterMapA.getCount(0, 0), 0.00001)

            assertEquals(7, counterMapA.totalSize())

            counterMapA.setCount(2, 1, 17)

            assertEquals(17.0, counterMapA.getCount(2, 1), 0.00001)
        End Sub

        <TestMethod>
        Public Sub testArgMaxAndEmpty()
            Dim counterMap As New CounterMap(Of String, String)()
            assertTrue(counterMap.Empty)
            assertNull(counterMap.argMax())
            assertEquals(0.0, counterMap.getCount("a", "b"), 0.0)

            counterMap.incrementCount("a", "x", 1)
            counterMap.incrementCount("a", "y", 5)
            counterMap.incrementCount("b", "z", 3)
            assertFalse(counterMap.Empty)
            assertFalse(counterMap.isEmpty("a"))
            assertTrue(counterMap.isEmpty("missing"))

            Dim best = counterMap.argMax()
            assertEquals("a", best.First)
            assertEquals("y", best.Second)

            counterMap.clear("a")
            assertTrue(counterMap.isEmpty("a"))
            assertEquals(1, counterMap.totalSize())
        End Sub

    End Class

End Namespace
