Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports StatCounter = DeepLearning4VB.org.datavec.api.transform.analysis.counter.StatCounter

Namespace org.datavec.api.transform.analysis.counter

    ''' <summary>Tests of the running statistics <c>StatCounter</c> (no JUnit test exists upstream).</summary>
    <TestClass>
    Public Class StatCounterTest

        Private Shared Function counterOf(ByVal ParamArray values() As Double) As StatCounter
            Dim c As New StatCounter()
            For Each v As Double In values
                c.add(v)
            Next v
            Return c
        End Function

        ''' <summary>Two-pass reference: (count, mean, sum of squared deviations).</summary>
        Private Shared Sub reference(ByVal values() As Double, ByRef mean As Double, ByRef m2 As Double)
            Dim sum As Double = 0
            For Each v As Double In values
                sum += v
            Next v
            mean = sum / values.Length
            m2 = 0
            For Each v As Double In values
                m2 += (v - mean) * (v - mean)
            Next v
        End Sub

        <TestMethod>
        Public Sub emptyCounter()
            Dim c As New StatCounter()
            assertEquals(0L, c.Count)
            assertEquals(0.0, c.Mean, 0.0)
            assertEquals(0.0, c.Sum, 0.0)
            ' Java initialises min to Double.MAX_VALUE and max to -Double.MAX_VALUE
            assertEquals(Double.MaxValue, c.Min, 0.0)
            assertEquals(-Double.MaxValue, c.Max, 0.0)
            assertTrue(Double.IsNaN(c.getVariance(True)))
            assertTrue(Double.IsNaN(c.getStddev(True)))
        End Sub

        <TestMethod>
        Public Sub singleValueVariance()
            Dim c As StatCounter = counterOf(3.5)
            ' a one-element sample has no variance, a one-element population has variance 0
            assertTrue(Double.IsNaN(c.getVariance(False)))
            assertEquals(0.0, c.getVariance(True), 0.0)
            assertEquals(3.5, c.Min, 0.0)
            assertEquals(3.5, c.Max, 0.0)
        End Sub

        <TestMethod>
        Public Sub meanVarianceMinMax()
            Dim c As StatCounter = counterOf(2, 4, 4, 4, 5, 5, 7, 9)
            assertEquals(8L, c.Count)
            assertEquals(5.0, c.Mean, 1.0E-12)
            assertEquals(40.0, c.Sum, 1.0E-9)
            assertEquals(2.0, c.Min, 0.0)
            assertEquals(9.0, c.Max, 0.0)
            assertEquals(4.0, c.getVariance(True), 1.0E-12)
            assertEquals(2.0, c.getStddev(True), 1.0E-12)
            assertEquals(32.0 / 7.0, c.getVariance(False), 1.0E-12)
            assertEquals(Math.Sqrt(32.0 / 7.0), c.getStddev(False), 1.0E-12)
        End Sub

        <TestMethod>
        Public Sub mergeEqualSizes()
            Dim a As StatCounter = counterOf(2, 4, 4, 4)
            Dim b As StatCounter = counterOf(5, 5, 7, 9)
            Dim merged As StatCounter = a.merge(b)
            assertTrue(merged Is a)
            assertEquals(8L, merged.Count)
            assertEquals(5.0, merged.Mean, 1.0E-12)
            assertEquals(4.0, merged.getVariance(True), 1.0E-12)
            assertEquals(2.0, merged.Min, 0.0)
            assertEquals(9.0, merged.Max, 0.0)
            ' the other counter is not modified
            assertEquals(4L, b.Count)
            assertEquals(6.5, b.Mean, 1.0E-12)
        End Sub

        <TestMethod>
        Public Sub mergeUnbalancedSizesUsesBothUpdateBranches()
            Dim big As New List(Of Double)()
            For i As Integer = 1 To 25
                big.Add(i)
            Next i
            Dim small() As Double = {100, 200}
            Dim all As New List(Of Double)(big)
            all.AddRange(small)
            Dim mean As Double, m2 As Double
            reference(all.ToArray(), mean, m2)

            ' this.count * 10 > other.count: the "small other" branch
            Dim a As StatCounter = counterOf(big.ToArray()).merge(counterOf(small))
            assertEquals(27L, a.Count)
            assertEquals(mean, a.Mean, 1.0E-9)
            assertEquals(m2 / 26, a.getVariance(False), 1.0E-9)

            ' other.count * 10 > this.count: the "small this" branch
            Dim b As StatCounter = counterOf(small).merge(counterOf(big.ToArray()))
            assertEquals(27L, b.Count)
            assertEquals(mean, b.Mean, 1.0E-9)
            assertEquals(m2 / 27, b.getVariance(True), 1.0E-9)
            assertEquals(1.0, b.Min, 0.0)
            assertEquals(200.0, b.Max, 0.0)
        End Sub

        <TestMethod>
        Public Sub mergeIntoEmptyCounterCopiesTheStatistics()
            ' Java assigned the variance numerator to the mean here (runningMean = o.runningM2); the port copies it
            ' to the variance numerator, so the merged counter equals the other one.
            Dim other As StatCounter = counterOf(2, 4, 4, 4, 5, 5, 7, 9)
            Dim empty As New StatCounter()
            Dim merged As StatCounter = empty.merge(other)
            assertTrue(merged Is empty)
            assertEquals(8L, merged.Count)
            assertEquals(5.0, merged.Mean, 1.0E-12)
            assertEquals(4.0, merged.getVariance(True), 1.0E-12)
            assertEquals(2.0, merged.Min, 0.0)
            assertEquals(9.0, merged.Max, 0.0)
        End Sub

        <TestMethod>
        Public Sub mergeWithEmptyNullAndSelf()
            Dim c As StatCounter = counterOf(1, 3)
            assertTrue(c.merge(Nothing) Is c)
            assertTrue(c.merge(New StatCounter()) Is c)
            assertEquals(2L, c.Count)
            assertEquals(2.0, c.Mean, 0.0)

            ' merging with itself merges a copy: {1, 3, 1, 3}
            c.merge(c)
            assertEquals(4L, c.Count)
            assertEquals(2.0, c.Mean, 1.0E-12)
            assertEquals(1.0, c.getVariance(True), 1.0E-12)
        End Sub

        <TestMethod>
        Public Sub cloneIsIndependent()
            Dim c As StatCounter = counterOf(1, 2, 3)
            Dim copy As StatCounter = c.clone()
            assertEquals(c.Count, copy.Count)
            assertEquals(c.Mean, copy.Mean, 0.0)
            assertEquals(c.getVariance(False), copy.getVariance(False), 0.0)
            copy.add(100)
            assertEquals(3L, c.Count)
            assertEquals(3.0, c.Max, 0.0)
            assertEquals(4L, copy.Count)
        End Sub

        <TestMethod>
        Public Sub toStringUsesJavaNumberFormat()
            Dim c As StatCounter = counterOf(1, 2, 3)
            assertEquals("StatCounter(count=3,mean=2.0,stdev=1.0,min=1.0,max=3.0)", c.ToString())
        End Sub

    End Class

End Namespace
