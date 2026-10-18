' DeepLearning4VB: the definitions of ND4J's summary-statistics ops (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original: the semantics are those of libnd4j's SummaryStatsVariance and
' SummaryStatsStandardDeviation (summarystats.h); the Java classes are in
' unported/nd4j/.../linalg/api/ops/impl/summarystats (Variance "var", StandardDeviation "std").

Imports System
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' The running moments of Welford's algorithm: count, mean and M2 (the sum of squared deviations from the mean).
    ''' A kernel adds every element with <see cref="Add"/> (or merges partial moments with <see cref="Merge"/>, Chan et
    ''' al.'s parallel formula) and finishes with <see cref="SummaryStatsDefinition.Finish"/>.
    ''' </summary>
    Public Structure WelfordMoments

        ''' <summary>The number of values (a Double, as in libnd4j's SummaryStatsData).</summary>
        Public Count As Double
        ''' <summary>The running mean.</summary>
        Public Mean As Double
        ''' <summary>The sum of squared deviations from the running mean.</summary>
        Public M2 As Double

        ''' <summary>Adds one value.</summary>
        Public Sub Add(ByVal x As Double)
            Count += 1.0
            Dim delta As Double = x - Mean
            Mean += delta / Count
            M2 += delta * (x - Mean)
        End Sub

        ''' <summary>The moments of the union of two disjoint sets of values.</summary>
        Public Shared Function Merge(ByVal first As WelfordMoments, ByVal second As WelfordMoments) As WelfordMoments
            If first.Count = 0.0 Then Return second
            If second.Count = 0.0 Then Return first
            Dim n As Double = first.Count + second.Count
            Dim delta As Double = second.Mean - first.Mean
            Dim result As New WelfordMoments()
            result.Count = n
            result.Mean = first.Mean + delta * second.Count / n
            result.M2 = first.M2 + second.M2 + delta * delta * first.Count * second.Count / n
            Return result
        End Function

    End Structure

    ''' <summary>A summary-statistics op: how its value follows from the Welford moments.</summary>
    Public NotInheritable Class SummaryStatsDefinition

        ''' <summary>The op name (Java <c>opName()</c>).</summary>
        Public Property Name As String

        ''' <summary>
        ''' The result from the moments and the op's bias-corrected flag (<c>Variance.isBiasCorrected()</c>, true by default
        ''' in Java; the executioner passes it separately, it is not an extra argument).
        ''' </summary>
        Public Property Finish As Func(Of WelfordMoments, Boolean, Double)

    End Class

    ''' <summary>
    ''' Registers <c>var</c> and <c>std</c>. As in libnd4j, the variance is M2/(n - 1) when bias-corrected and M2/n
    ''' otherwise, and 0 (not NaN) when n ≤ 1, including empty input; NaN inputs propagate. The standard deviation is the
    ''' square root of that variance. The kernels accumulate the moments with a dedicated Welford kernel.
    ''' </summary>
    Friend NotInheritable Class SummaryStatsMath

        Private Sub New()
        End Sub

        Friend Shared Function Variance(ByVal moments As WelfordMoments, ByVal biasCorrected As Boolean) As Double
            If moments.Count <= 1.0 Then Return 0.0
            If biasCorrected Then
                Dim corrected As Double = moments.M2 / (moments.Count - 1.0)
                If corrected >= 0.0 OrElse Double.IsNaN(corrected) Then Return corrected
            End If
            Return moments.M2 / moments.Count
        End Function

        Friend Shared Sub Register(ByVal t As OpTable)
            t.AddSummaryStats(New SummaryStatsDefinition With {
                .Name = "var",
                .Finish = Function(m, bias) Variance(m, bias)
            })
            t.AddSummaryStats(New SummaryStatsDefinition With {
                .Name = "std",
                .Finish = Function(m, bias) SysMath.Sqrt(Variance(m, bias))
            })
        End Sub

    End Class

End Namespace
