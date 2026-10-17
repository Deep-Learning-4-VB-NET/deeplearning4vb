Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SequenceLengthAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis

Namespace org.datavec.api.transform.analysis.sequence

    ''' <summary>Tests of <c>SequenceLengthAnalysis</c> (Lombok builder, accessors, equality and the Java text).</summary>
    <TestClass>
    Public Class SequenceLengthAnalysisTest

        Private Shared Function sample() As SequenceLengthAnalysisT
            Return SequenceLengthAnalysisT.builder().totalNumSequences(10).minSeqLength(0).maxSeqLength(7).countZeroLength(2).countOneLength(3).meanLength(2.5).histogramBuckets(New Double() {0, 3.5, 7}).histogramBucketCounts(New Long() {8, 2}).build()
        End Function

        <TestMethod>
        Public Sub builderAndGetters()
            Dim a As SequenceLengthAnalysisT = sample()
            assertEquals(10L, a.getTotalNumSequences())
            assertEquals(0, a.getMinSeqLength())
            assertEquals(7, a.getMaxSeqLength())
            assertEquals(2L, a.getCountZeroLength())
            assertEquals(3L, a.getCountOneLength())
            assertEquals(2.5, a.getMeanLength(), 0.0)
            assertArrayEquals(New Double() {0, 3.5, 7}, a.getHistogramBuckets(), 0.0)
            assertArrayEquals(New Long() {8, 2}, a.getHistogramBucketCounts())
        End Sub

        <TestMethod>
        Public Sub toStringUsesJavaFormat()
            ' the histogram is not printed
            assertEquals("SequenceLengthAnalysis(totalNumSequences=10,minSeqLength=0,maxSeqLength=7,countZeroLength=2,countOneLength=3,meanLength=2.5)", sample().ToString())
            Dim whole As SequenceLengthAnalysisT = SequenceLengthAnalysisT.builder().totalNumSequences(1).minSeqLength(4).maxSeqLength(4).meanLength(4).build()
            assertEquals("SequenceLengthAnalysis(totalNumSequences=1,minSeqLength=4,maxSeqLength=4,countZeroLength=0,countOneLength=0,meanLength=4.0)", whole.ToString())
        End Sub

        <TestMethod>
        Public Sub equalityComparesHistogramsByContent()
            Dim a As SequenceLengthAnalysisT = sample()
            Dim b As New SequenceLengthAnalysisT(10, 0, 7, 2, 3, 2.5, New Double() {0, 3.5, 7}, New Long() {8, 2})
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            b.setHistogramBucketCounts(New Long() {7, 3})
            assertNotEquals(a, b)
            b.setHistogramBucketCounts(New Long() {8, 2})
            b.setMeanLength(2.6)
            assertNotEquals(a, b)
        End Sub

        <TestMethod>
        Public Sub settersAndBuilderText()
            Dim a As SequenceLengthAnalysisT = sample()
            a.setTotalNumSequences(11)
            a.setMinSeqLength(1)
            a.setMaxSeqLength(9)
            a.setCountZeroLength(0)
            a.setCountOneLength(4)
            assertEquals("SequenceLengthAnalysis(totalNumSequences=11,minSeqLength=1,maxSeqLength=9,countZeroLength=0,countOneLength=4,meanLength=2.5)", a.ToString())
            assertEquals("SequenceLengthAnalysis.SequenceLengthAnalysisBuilder(totalNumSequences=3, minSeqLength=0, maxSeqLength=0, countZeroLength=0, " &
                         "countOneLength=0, meanLength=0.0, histogramBuckets=null, histogramBucketCounts=null)",
                         SequenceLengthAnalysisT.builder().totalNumSequences(3).ToString())
            assertTrue(isSerializableType(GetType(SequenceLengthAnalysisT)))
        End Sub

    End Class

End Namespace
