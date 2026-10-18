Imports System
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SysMath = System.Math
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>Variance and standard deviation from Welford moments, sequential and merged, with libnd4j's n ≤ 1 rule.</summary>
    <TestClass>
    Public Class SummaryStatsMathTest

        Private Shared ReadOnly Sample As Double() = {2.0, 4.0, 4.0, 4.0, 5.0, 5.0, 7.0, 9.0}

        Private Shared Function Moments(ByVal values As Double()) As WelfordMoments
            Dim m As New WelfordMoments()
            For Each v As Double In values
                m.Add(v)
            Next
            Return m
        End Function

        <TestMethod>
        Public Sub Variance_HandComputed()
            ' mean 5, sum of squared deviations 32
            Dim m As WelfordMoments = Moments(Sample)
            assertEquals(8.0, m.Count, 0.0)
            assertEquals(5.0, m.Mean, 0.000000000000001)
            assertEquals(32.0, m.M2, 0.00000000000001)
            Dim var As SummaryStatsDefinition = ManagedOpMath.SummaryStats("var")
            Dim std As SummaryStatsDefinition = ManagedOpMath.SummaryStats("std")
            assertEquals(4.0, var.Finish(m, False), 0.00000000000001)
            assertEquals(32.0 / 7.0, var.Finish(m, True), 0.00000000000001)
            assertEquals(2.0, std.Finish(m, False), 0.00000000000001)
            assertEquals(SysMath.Sqrt(32.0 / 7.0), std.Finish(m, True), 0.00000000000001)
        End Sub

        <TestMethod>
        Public Sub MergedHalves_EqualTheSequentialMoments()
            Dim rnd As New Random(17)
            Dim values As Double() = Enumerable.Range(0, 101).Select(Function(i) rnd.NextDouble() * 10.0 - 3.0).ToArray()
            Dim whole As WelfordMoments = Moments(values)
            For Each cut As Integer In New Integer() {0, 1, 37, 100, 101}
                Dim merged As WelfordMoments = WelfordMoments.Merge(Moments(values.Take(cut).ToArray()), Moments(values.Skip(cut).ToArray()))
                assertEquals(whole.Count, merged.Count, 0.0)
                assertEquals(whole.Mean, merged.Mean, 0.000000000001)
                assertEquals(whole.M2, merged.M2, 0.000000001)
            Next
            ' and both agree with the two-pass definition
            Dim mean As Double = values.Average()
            Dim twoPass As Double = values.Sum(Function(v) (v - mean) * (v - mean)) / (values.Length - 1)
            assertEquals(twoPass, ManagedOpMath.SummaryStats("var").Finish(whole, True), 0.000000001)
        End Sub

        <TestMethod>
        Public Sub OneOrNoValues_GiveZero_NaNPropagates()
            For Each name As String In New String() {"var", "std"}
                Dim definition As SummaryStatsDefinition = ManagedOpMath.SummaryStats(name)
                assertEquals(0.0, definition.Finish(Moments(New Double() {}), True), 0.0)
                assertEquals(0.0, definition.Finish(Moments(New Double() {3.5}), True), 0.0)
                assertEquals(0.0, definition.Finish(Moments(New Double() {3.5}), False), 0.0)
                assertTrue(Double.IsNaN(definition.Finish(Moments(New Double() {1.0, Double.NaN, 2.0}), True)), name)
            Next
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.SummaryStats, "var"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.SummaryStats, "std"))
            assertEquals(New String() {"std", "var"}, ManagedOpMath.Names(ManagedOpKind.SummaryStats))
        End Sub

    End Class

End Namespace
