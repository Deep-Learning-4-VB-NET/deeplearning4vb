Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.primitives

Namespace org.nd4vb.common.primitives

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class CounterTest

        <TestMethod>
        Public Sub testCounterIncrementAll1()
            Dim counterA As New Counter(Of String)()

            counterA.incrementCount("A", 1)
            counterA.incrementCount("A", 1)
            counterA.incrementCount("A", 1)

            Dim counterB As New Counter(Of String)()
            counterB.incrementCount("B", 2)
            counterB.incrementCount("B", 2)

            assertEquals(3.0, counterA.getCount("A"), 0.00001)
            assertEquals(4.0, counterB.getCount("B"), 0.00001)

            counterA.incrementAll(counterB)

            assertEquals(3.0, counterA.getCount("A"), 0.00001)
            assertEquals(4.0, counterA.getCount("B"), 0.00001)

            counterA.setCount("B", 234)

            assertEquals(234.0, counterA.getCount("B"), 0.00001)
        End Sub

        <TestMethod>
        Public Sub testCounterTopN1()
            Dim counterA As New Counter(Of String)()

            counterA.incrementCount("A", 1)
            counterA.incrementCount("B", 2)
            counterA.incrementCount("C", 3)
            counterA.incrementCount("D", 4)
            counterA.incrementCount("E", 5)

            counterA.keepTopNElements(4)

            assertEquals(4, counterA.size())

            ' we expect element A to be gone
            assertEquals(0.0, counterA.getCount("A"), 0.00001)
            assertEquals(2.0, counterA.getCount("B"), 0.00001)
            assertEquals(3.0, counterA.getCount("C"), 0.00001)
            assertEquals(4.0, counterA.getCount("D"), 0.00001)
            assertEquals(5.0, counterA.getCount("E"), 0.00001)
        End Sub

        <TestMethod>
        Public Sub testKeysSorted1()
            Dim counterA As New Counter(Of String)()

            counterA.incrementCount("A", 1)
            counterA.incrementCount("B", 2)
            counterA.incrementCount("C", 3)
            counterA.incrementCount("D", 4)
            counterA.incrementCount("E", 5)

            assertEquals("E", counterA.argMax())

            Dim list As IList(Of String) = counterA.keySetSorted()

            assertEquals(5, list.Count)

            assertEquals("E", list(0))
            assertEquals("D", list(1))
            assertEquals("C", list(2))
            assertEquals("B", list(3))
            assertEquals("A", list(4))
        End Sub

        <TestMethod>
        Public Sub testCounterTotal()
            Dim counter As New Counter(Of String)()

            counter.incrementCount("A", 1)
            counter.incrementCount("B", 1)
            counter.incrementCount("C", 1)

            assertEquals(3.0, counter.totalCount(), 0.00001)

            counter.setCount("B", 234)

            assertEquals(236.0, counter.totalCount(), 0.00001)

            counter.setCount("D", 1)

            assertEquals(237.0, counter.totalCount(), 0.00001)

            counter.removeKey("B")

            assertEquals(3.0, counter.totalCount(), 0.00001)
        End Sub

        <TestMethod>
        Public Sub testProbabilityNormalizeAndThreshold()
            Dim counter As New Counter(Of String)()
            assertThrows(Of System.InvalidOperationException)(Sub() counter.getProbability("A"))

            counter.incrementCount("A", 1)
            counter.incrementCount("B", 3)
            assertEquals(0.25, counter.getProbability("A"), 0.00001)
            assertEquals(0.75, counter.getProbability("B"), 0.00001)

            counter.normalize()
            assertEquals(1.0, counter.totalCount(), 0.00001)
            assertEquals(0.25, counter.getCount("A"), 0.00001)

            counter.dropElementsBelowThreshold(0.5)
            assertEquals(1, counter.size())
            assertFalse(counter.containsElement("A"))
            assertTrue(counter.containsElement("B"))

            counter.clear()
            assertTrue(counter.Empty)
            assertEquals(0.0, counter.totalCount(), 0.0)
        End Sub

        <TestMethod>
        Public Sub testEqualityAndReversedQueue()
            Dim a As New Counter(Of Integer)()
            Dim b As New Counter(Of Integer)()
            a.incrementCount(1, 2.0)
            a.incrementCount(2, 1.0)
            b.incrementCount(2, 1.0)
            b.incrementCount(1, 2.0)
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())

            Dim reversed = a.asReversedPriorityQueue()
            assertEquals(2, reversed.poll().First)
            assertEquals(1, reversed.poll().First)
            assertTrue(reversed.isEmpty())
        End Sub

    End Class

End Namespace
