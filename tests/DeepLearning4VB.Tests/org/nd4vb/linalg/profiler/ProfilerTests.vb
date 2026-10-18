Imports System
Imports System.Diagnostics
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.profiler
Imports DeepLearning4VB.org.nd4vb.linalg.profiler.data
Imports DeepLearning4VB.org.nd4vb.linalg.profiler.data.primitives
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.profiler

    ''' <summary>
    ''' Tests for the op profiler's configuration and data structures (counters, timing sets, stack trees) and the parts
    ''' of <c>OpProfiler</c> that need no arrays. Op calls are profiled by the executioner of a later wave.
    ''' </summary>
    <TestClass>
    Public Class ProfilerTests

        <TestMethod>
        Public Sub testProfilerConfigDefaultsAndBuilder()
            Dim d As ProfilerConfig = ProfilerConfig.builder().build()
            assertFalse(d.isNotOptimalArguments())
            assertFalse(d.isNotOptimalTAD())
            assertFalse(d.isNativeStatistics())
            assertFalse(d.isCheckForNAN())
            assertFalse(d.isCheckForINF())
            assertFalse(d.isStackTrace())
            assertFalse(d.isCheckElapsedTime())
            assertTrue(d.isCheckWorkspaces())
            assertFalse(d.isCheckLocality())

            Dim c As ProfilerConfig = ProfilerConfig.builder().checkForNAN(True).checkForINF(True).checkWorkspaces(False).stackTrace(True).build()
            assertTrue(c.isCheckForNAN())
            assertTrue(c.isCheckForINF())
            assertFalse(c.isCheckWorkspaces())
            assertTrue(c.isStackTrace())
            assertFalse(c.isNotOptimalArguments())

            assertNotEquals(d, c)
            Dim d2 As ProfilerConfig = ProfilerConfig.builder().build()
            assertEquals(d, d2)
            assertEquals(d.GetHashCode(), d2.GetHashCode())
            d2.setCheckLocality(True)
            assertTrue(d2.isCheckLocality())
            assertNotEquals(d, d2)
            assertEquals("ProfilerConfig(notOptimalArguments=false, notOptimalTAD=false, nativeStatistics=false, checkForNAN=false, checkForINF=false, stackTrace=false, checkElapsedTime=false, checkWorkspaces=true, checkLocality=false)", d.ToString())
        End Sub

        <TestMethod>
        Public Sub testComparableAtomicLong()
            Dim a As New ComparableAtomicLong(5)
            assertEquals(6L, a.incrementAndGet())
            assertEquals(10L, a.addAndGet(4))
            a.set(3)
            assertEquals(3L, a.get())
            assertEquals(3L, a.longValue())
            assertEquals("3", a.ToString())
            ' descending order: the larger value sorts first
            Dim items As ComparableAtomicLong() = {New ComparableAtomicLong(1), New ComparableAtomicLong(7), New ComparableAtomicLong(4)}
            Array.Sort(items)
            assertEquals(7L, items(0).get())
            assertEquals(1L, items(2).get())
        End Sub

        <TestMethod>
        Public Sub testTimeSet()
            Dim t As New TimeSet()
            assertEquals(0L, t.getAverage())
            assertEquals(0L, t.getMedian())
            t.addTime(30)
            t.addTime(10)
            t.addTime(20)
            assertEquals(3, t.size())
            assertEquals(60L, t.getSum())
            assertEquals(20L, t.getAverage())
            ' Java's median is the middle element in insertion order
            assertEquals(10L, t.getMedian())
            assertEquals(10L, t.getMinimum())
            assertEquals(30L, t.getMaximum())
            t.addTime(40)
            assertEquals(100L, t.getSum())

            Dim small As New TimeSet()
            small.addTime(1)
            assertTrue(t.CompareTo(small) < 0, "larger sums sort first")
        End Sub

        <TestMethod>
        Public Sub testStringCounter()
            Dim c As New StringCounter()
            assertEquals(1L, c.incrementCount("a"))
            assertEquals(2L, c.incrementCount("a"))
            c.incrementCount("b")
            c.totalsIncrement()
            assertEquals(2L, c.getCount("a"))
            assertEquals(1L, c.getCount("b"))
            assertEquals(0L, c.getCount("missing"))
            ' 4 events in total; integer percentages printed like Java floats, most frequent first
            assertEquals("a  >>> [2] perc: [50.0]" & vbLf & "b  >>> [1] perc: [25.0]" & vbLf, c.asString())
            c.reset()
            assertEquals(0L, c.getCount("a"))
            assertEquals("", c.asString())
        End Sub

        <TestMethod>
        Public Sub testStringAggregator()
            Dim a As New StringAggregator()
            a.putTime("x", 2000000)
            a.putTime("x", 4000000)
            a.putTime("y", 6000000)
            Dim s As String = a.asString()
            ' without long calls the number of calls is printed too
            assertTrue(s.Contains("x  >>>  2 calls; Min: 2000000 ns; Max: 4000000 ns; Average: 3000000 ns; Median: 4000000 ns; "), s)
            Dim p As String = a.asPercentageString()
            assertTrue(p.StartsWith("Total time spent: 12 ms." & vbLf), p)
            assertTrue(p.Contains("y  >>>  perc: 50.0 Time spent: 6 ms"), p)
            assertTrue(p.Contains("x  >>>  perc: 50.0 Time spent: 6 ms"), p)
            a.reset()
            assertTrue(a.asPercentageString().StartsWith("Total time spent: 0 ms."))
        End Sub

        <TestMethod>
        Public Sub testStackTree()
            Dim agg As New StackAggregator()
            agg.incrementCount()
            agg.incrementCount(5)
            assertEquals(2L, agg.TotalEventsNumber)
            ' the two calls come from different lines (or at least the same method), so one or two branches
            assertTrue(agg.UniqueBranchesNumber >= 1 AndAlso agg.UniqueBranchesNumber <= 2)
            assertNotNull(agg.LastDescriptor)
            assertTrue(agg.LastDescriptor.size() > 0)
            agg.reset()
            assertEquals(0L, agg.TotalEventsNumber)
            assertEquals(0, agg.UniqueBranchesNumber)

            Dim frames As StackFrame() = New StackTrace(False).GetFrames()
            Dim a As New StackDescriptor(frames)
            Dim b As New StackDescriptor(frames)
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            ' outermost frame first: this test method is the last element
            assertTrue(a.getElementName(a.size() - 1).Contains("ProfilerTests.testStackTree"), a.getElementName(a.size() - 1))
            assertTrue(a.ToString().StartsWith("Stack trace: " & vbLf))

            Dim tree As New StackTree()
            tree.consumeStackTrace(a)
            tree.consumeStackTrace(b, 3)
            assertEquals(2L, tree.TotalEventsNumber)
            assertEquals(1, tree.UniqueBranchesNumber)
            Assert.AreSame(b, tree.getLastDescriptor())
            assertThrows(Of ArgumentNullException)(Sub() tree.consumeStackTrace(Nothing))

            Dim node As New StackNode(a.getElementName(0))
            node.consume(a, 0, 4)
            assertEquals(1, node.getNodes().Count)
            assertTrue(node.CompareTo(New StackNode("other")) < 0)
        End Sub

        <TestMethod>
        Public Sub testOpProfilerWithoutArrays()
            Dim profiler As OpProfiler = OpProfiler.Instance
            Assert.AreSame(profiler, OpProfiler.getInstance())
            Dim previous As ProfilerConfig = profiler.Config
            Try
                profiler.reset()
                assertEquals(0L, profiler.InvocationsCount)
                profiler.processScalarCall()
                profiler.processScalarCall()
                assertEquals(2L, profiler.InvocationsCount)
                assertEquals(2L, profiler.ScalarAggregator.TotalEventsNumber)

                ' no operands: no penalty
                Dim causes As OpProfiler.PenaltyCause() = profiler.processOperands(DirectCast(Nothing, INDArray), DirectCast(Nothing, INDArray))
                assertArrayEquals(New OpProfiler.PenaltyCause() {OpProfiler.PenaltyCause.NONE}, causes)
                assertArrayEquals(New OpProfiler.PenaltyCause() {OpProfiler.PenaltyCause.NONE}, profiler.processOperands(DirectCast(Nothing, INDArray())))
                assertArrayEquals(New OpProfiler.PenaltyCause() {OpProfiler.PenaltyCause.NONE}, profiler.processTADOperands(DirectCast(Nothing, DataBuffer)))

                profiler.Config = ProfilerConfig.builder().notOptimalArguments(True).build()
                assertTrue(profiler.Config.isNotOptimalArguments())

                profiler.reset()
                assertEquals(0L, profiler.InvocationsCount)
                assertEquals(0L, profiler.ScalarAggregator.TotalEventsNumber)
                assertNotNull(profiler.Statistics)
            Finally
                profiler.Config = previous
                profiler.reset()
            End Try
        End Sub

    End Class

End Namespace
