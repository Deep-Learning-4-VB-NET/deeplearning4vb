Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.analysis.histogram
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports HistogramCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.histogram.HistogramCounter
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.transform.analysis.histogram

    ''' <summary>
    ''' Tests of the histogram counters: bin edges, bin assignment (half-open bins, everything else in the last bin, as in
    ''' Java) and merging.
    ''' </summary>
    <TestClass>
    Public Class HistogramCountersTest

        Private Shared Function doubleHistogram(ByVal minValue As Double, ByVal maxValue As Double, ByVal nBins As Integer, ByVal ParamArray values() As Double) As DoubleHistogramCounter
            Dim h As New DoubleHistogramCounter(minValue, maxValue, nBins)
            For Each v As Double In values
                h.add(New DoubleWritable(v))
            Next v
            Return h
        End Function

        ' ---------------- DoubleHistogramCounter ----------------

        <TestMethod>
        Public Sub doubleHistogramBinEdges()
            Dim h As New DoubleHistogramCounter(0, 10, 5)
            assertArrayEquals(New Double() {0, 2, 4, 6, 8, 10}, h.Bins, 0.0)
            assertArrayEquals(New Long() {0, 0, 0, 0, 0}, h.Counts)

            Dim thirds As New DoubleHistogramCounter(-1, 1, 3)
            assertEquals(4, thirds.Bins.Length)
            assertEquals(-1.0, thirds.Bins(0), 0.0)
            assertEquals(-1.0 / 3.0, thirds.Bins(1), 1.0E-12)
            assertEquals(1.0 / 3.0, thirds.Bins(2), 1.0E-12)
            assertEquals(1.0, thirds.Bins(3), 0.0) ' the last edge is exactly the maximum
        End Sub

        <TestMethod>
        Public Sub doubleHistogramAssignsValuesToHalfOpenBins()
            Dim h As DoubleHistogramCounter = doubleHistogram(0, 10, 5, 0.0, 1.99, 2.0, 3.0, 5.5, 7.999, 8.0, 9.99)
            assertArrayEquals(New Long() {2, 2, 1, 1, 2}, h.Counts)
        End Sub

        <TestMethod>
        Public Sub doubleHistogramPutsMaximumOutOfRangeAndNaNIntoTheLastBin()
            ' bins are [bins(i), bins(i+1)); a value matching no bin (the maximum itself, values below the minimum or above
            ' the maximum, NaN) is counted in the last bin, as in Java
            Dim h As DoubleHistogramCounter = doubleHistogram(0, 10, 5, 10.0, -1.0, 11.0, Double.NaN, 0.5)
            assertArrayEquals(New Long() {1, 0, 0, 0, 4}, h.Counts)
        End Sub

        <TestMethod>
        Public Sub doubleHistogramMergeAddsCounts()
            Dim h1 As DoubleHistogramCounter = doubleHistogram(0, 4, 2, 1.0, 3.0)
            Dim h2 As DoubleHistogramCounter = doubleHistogram(0, 4, 2, 0.5, 1.5, 2.5)
            Dim merged As DoubleHistogramCounter = h1.merge(h2)
            assertTrue(merged Is h1)
            assertArrayEquals(New Long() {3, 2}, merged.Counts)
            assertArrayEquals(New Long() {2, 1}, h2.Counts)
            assertTrue(h1.merge(Nothing) Is h1)

            ' through the interface
            Dim asInterface As HistogramCounterT = h1
            assertTrue(asInterface.merge(doubleHistogram(0, 4, 2, 3.5)) Is h1)
            assertArrayEquals(New Long() {3, 3}, h1.Counts)
        End Sub

        <TestMethod>
        Public Sub doubleHistogramMergeRejectsIncompatibleHistograms()
            Dim h As New DoubleHistogramCounter(0, 10, 5)
            assertThrows(Of InvalidOperationException)(Sub() Call h.merge(New DoubleHistogramCounter(0, 11, 5)))
            assertThrows(Of InvalidOperationException)(Sub() Call h.merge(New DoubleHistogramCounter(0, 10, 4)))
            assertThrows(Of ArgumentException)(Sub() Call h.merge(New StringHistogramCounter(0, 10, 5)))
        End Sub

        ' ---------------- StringHistogramCounter ----------------

        <TestMethod>
        Public Sub stringHistogramBinEdgesStartAtZero()
            ' As in Java: the edges are i * (maxLength - minLength) / nBins (starting at 0, not at minLength), the last
            ' edge is maxLength
            Dim h As New StringHistogramCounter(2, 10, 4)
            assertArrayEquals(New Double() {0, 2, 4, 6, 10}, h.Bins, 0.0)
        End Sub

        <TestMethod>
        Public Sub stringHistogramCountsLengths()
            Dim h As New StringHistogramCounter(2, 10, 4)
            For Each s As String In New String() {"", "a", "ab", "abcde", "abcdefg", "abcdefghij", "abcdefghijkl"}
                h.add(New TextT(s))
            Next s
            ' lengths 0,1 -> bin 0; 2 -> bin 1; 5 -> bin 2; 7 -> bin 3; 10 (the maximum) and 12 -> last bin
            assertArrayEquals(New Long() {2, 1, 1, 3}, h.Counts)
        End Sub

        <TestMethod>
        Public Sub stringHistogramMerge()
            Dim h1 As New StringHistogramCounter(0, 4, 2)
            h1.add(New TextT("a"))
            Dim h2 As New StringHistogramCounter(0, 4, 2)
            h2.add(New TextT("abc")).add(New IntWritable(12345))
            Dim merged As StringHistogramCounter = h1.merge(h2)
            assertTrue(merged Is h1)
            assertArrayEquals(New Long() {1, 2}, merged.Counts)
            assertThrows(Of InvalidOperationException)(Sub() Call h1.merge(New StringHistogramCounter(1, 4, 2)))
            assertThrows(Of InvalidOperationException)(Sub() Call h1.merge(New StringHistogramCounter(0, 4, 3)))
            assertThrows(Of ArgumentException)(Sub() Call h1.merge(New DoubleHistogramCounter(0, 4, 2)))
        End Sub

        ' ---------------- CategoricalHistogramCounter ----------------

        <TestMethod>
        Public Sub categoricalHistogramCountsStateNamesInOrder()
            Dim h As New CategoricalHistogramCounter(New List(Of String) From {"x", "y", "z"})
            For Each s As String In New String() {"x", "y", "x", "w"}
                h.add(New TextT(s))
            Next s
            assertArrayEquals(New Double() {0, 1, 2, 3}, h.Bins, 0.0)
            ' "w" is not a state name: counted, but not reported
            assertArrayEquals(New Long() {2, 1, 0}, h.Counts)
        End Sub

        <TestMethod>
        Public Sub categoricalHistogramMerge()
            Dim states As New List(Of String) From {"x", "y", "z"}
            Dim h1 As New CategoricalHistogramCounter(states)
            h1.add(New TextT("x")).add(New TextT("y"))
            Dim h2 As New CategoricalHistogramCounter(states)
            h2.add(New TextT("z")).add(New TextT("x"))
            Dim merged As HistogramCounterT = h1.merge(h2)
            assertTrue(merged Is h1)
            assertArrayEquals(New Long() {2, 1, 1}, h1.Counts)
            assertThrows(Of ArgumentException)(Sub() Call h1.merge(New DoubleHistogramCounter(0, 1, 1)))
            assertThrows(Of ArgumentException)(Sub() Call h1.merge(Nothing))
        End Sub

    End Class

End Namespace
