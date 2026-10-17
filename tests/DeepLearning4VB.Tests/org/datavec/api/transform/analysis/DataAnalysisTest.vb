Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.analysis.columns
Imports DeepLearning4VB.org.datavec.api.transform.analysis.counter
Imports AnalysisCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.AnalysisCounter
Imports BooleanWritable = DeepLearning4VB.org.datavec.api.writable.BooleanWritable
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports DataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.DataAnalysis
Imports DataVecAnalysisUtils = DeepLearning4VB.org.datavec.api.transform.analysis.DataVecAnalysisUtils
Imports DoubleHistogramCounter = DeepLearning4VB.org.datavec.api.transform.analysis.histogram.DoubleHistogramCounter
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports HistogramCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.histogram.HistogramCounter
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports SchemaT = DeepLearning4VB.org.datavec.api.transform.schema.Schema
Imports SequenceDataAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.SequenceDataAnalysis
Imports SequenceLengthAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis
Imports SerializationUtils = DeepLearning4VB.org.nd4j.common.util.SerializationUtils
Imports StringHistogramCounter = DeepLearning4VB.org.datavec.api.transform.analysis.histogram.StringHistogramCounter
Imports TDigest = DeepLearning4VB.com.tdunning.math.stats.TDigest
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.transform.analysis

    ''' <summary>
    ''' Tests of <c>DataAnalysis</c>, <c>SequenceDataAnalysis</c> and <c>DataVecAnalysisUtils</c> (counters to column
    ''' analyses, histograms attached to the analyses).
    ''' </summary>
    <TestClass>
    Public Class DataAnalysisTest

        Private Shared Function threeColumnSchema() As SchemaT
            Return (New SchemaT.Builder()).addColumnInteger("int").addColumnString("str").addColumnCategorical("cat", "a", "b").build()
        End Function

        Private Shared Function threeAnalyses() As IList(Of ColumnAnalysisT)
            Return New List(Of ColumnAnalysisT) From {
                (New IntegerAnalysis.Builder()).min(1).max(3).mean(2.0).sampleStdev(1.0).sampleVariance(1.0).countPositive(3).countMinValue(1).countMaxValue(1).countTotal(3).build(),
                (New StringAnalysis.Builder()).minLength(1).maxLength(2).meanLength(1.5).countTotal(3).build(),
                New CategoricalAnalysis(New Dictionary(Of String, Long) From {{"a", 1L}, {"b", 2L}})}
        End Function

        Private Shared Function sequenceLengths() As SequenceLengthAnalysisT
            Return SequenceLengthAnalysisT.builder().totalNumSequences(4).minSeqLength(0).maxSeqLength(5).countZeroLength(1).countOneLength(1).meanLength(2.5).build()
        End Function

        <TestMethod>
        Public Sub getColumnAnalysisByNameAndIndex()
            Dim analyses As IList(Of ColumnAnalysisT) = threeAnalyses()
            Dim da As New DataAnalysisT(threeColumnSchema(), analyses)
            assertTrue(da.getColumnAnalysis("int") Is analyses(0))
            assertTrue(da.getColumnAnalysis("str") Is analyses(1))
            assertTrue(da.getColumnAnalysis("cat") Is analyses(2))
            assertTrue(da.getColumnAnalysis() Is analyses)
            assertEquals(ColumnTypeEnum.Categorical, da.getColumnAnalysis()(2).ColumnType)
            assertEquals(3, da.getSchema().numColumns())
            assertThrows(Of KeyNotFoundException)(Sub() Call da.getColumnAnalysis("missing"))
        End Sub

        <TestMethod>
        Public Sub dataAnalysisToStringIsAPaddedTable()
            Dim analyses As IList(Of ColumnAnalysisT) = threeAnalyses()
            Dim da As New DataAnalysisT(threeColumnSchema(), analyses)
            ' Java: "%-6s" for the index, "%-(maxNameLength + 8)s" for the quoted name, "%-15s" for the type
            Dim expected As String =
                "idx   " & "name".PadRight(11) & "type".PadRight(15) & "analysis" & vbLf &
                "0     " & """int""".PadRight(11) & "Integer".PadRight(15) & analyses(0).ToString() & vbLf &
                "1     " & """str""".PadRight(11) & "String".PadRight(15) & analyses(1).ToString() & vbLf &
                "2     " & """cat""".PadRight(11) & "Categorical".PadRight(15) & analyses(2).ToString() & vbLf
            assertEquals(expected, da.ToString())
        End Sub

        <TestMethod>
        Public Sub dataAnalysisEqualityAndSetters()
            Dim a As New DataAnalysisT(threeColumnSchema(), threeAnalyses())
            Dim b As New DataAnalysisT(threeColumnSchema(), threeAnalyses())
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())

            Dim changed As IList(Of ColumnAnalysisT) = threeAnalyses()
            changed(2) = New CategoricalAnalysis(New Dictionary(Of String, Long) From {{"a", 1L}, {"b", 3L}})
            b.setColumnAnalysis(changed)
            assertNotEquals(a, b)

            b.setColumnAnalysis(threeAnalyses())
            b.setSchema((New SchemaT.Builder()).addColumnInteger("int").addColumnString("str").addColumnCategorical("cat", "a", "b", "c").build())
            assertNotEquals(a, b)
        End Sub

        <TestMethod>
        Public Sub sequenceDataAnalysisTextAndEquality()
            Dim lengths As SequenceLengthAnalysisT = sequenceLengths()
            Dim sda As New SequenceDataAnalysisT(threeColumnSchema(), threeAnalyses(), lengths)
            assertTrue(sda.getSequenceLengthAnalysis() Is lengths)
            assertTrue(sda.getColumnAnalysis("str") IsNot Nothing)
            Dim plain As New DataAnalysisT(threeColumnSchema(), threeAnalyses())
            assertEquals(lengths.ToString() & vbLf & plain.ToString(), sda.ToString())

            assertEquals(sda, New SequenceDataAnalysisT(threeColumnSchema(), threeAnalyses(), sequenceLengths()))
            assertNotEquals(sda, New SequenceDataAnalysisT(threeColumnSchema(), threeAnalyses(),
                                                            SequenceLengthAnalysisT.builder().totalNumSequences(5).build()))
            ' a sequence analysis is not equal to a plain one with the same columns
            assertNotEquals(plain, sda)
            assertNotEquals(sda, plain)
        End Sub

        <TestMethod>
        Public Sub convertCountersBuildsTheAnalysisOfEveryColumnType()
            Dim types As New List(Of ColumnTypeEnum) From {ColumnTypeEnum.String, ColumnTypeEnum.Integer, ColumnTypeEnum.Long, ColumnTypeEnum.Double,
                                                           ColumnTypeEnum.Float, ColumnTypeEnum.Categorical, ColumnTypeEnum.Time, ColumnTypeEnum.Bytes,
                                                           ColumnTypeEnum.Boolean}
            Dim sc As New StringAnalysisCounter()
            sc.add(New TextT("ab")).add(New TextT("abcd"))
            Dim ic As New IntegerAnalysisCounter()
            ic.add(New IntWritable(-3)).add(New IntWritable(7))
            Dim lc As New LongAnalysisCounter()
            lc.add(New LongWritable(10)).add(New LongWritable(20)).add(New LongWritable(30))
            Dim dc As New DoubleAnalysisCounter()
            dc.add(New DoubleWritable(0.5)).add(New DoubleWritable(-0.5))
            Dim fc As New DoubleAnalysisCounter()
            fc.add(New DoubleWritable(1.25))
            Dim cc As New CategoricalAnalysisCounter()
            cc.add(New TextT("x")).add(New TextT("x")).add(New TextT("y"))
            Dim tc As New LongAnalysisCounter()
            tc.add(New LongWritable(1000)).add(New LongWritable(5000))
            Dim bc As New BytesAnalysisCounter(4)
            Dim boolc As New IntegerAnalysisCounter()
            boolc.add(New BooleanWritable(True)).add(New BooleanWritable(False)).add(New BooleanWritable(True))
            Dim counters As New List(Of AnalysisCounterT) From {sc, ic, lc, dc, fc, cc, tc, bc, boolc}

            Dim minsMaxes(types.Count - 1)() As Double
            For i As Integer = 0 To types.Count - 1
                minsMaxes(i) = New Double(1) {}
            Next i

            Dim list As IList(Of ColumnAnalysisT) = DataVecAnalysisUtils.convertCounters(counters, minsMaxes, types)
            assertEquals(9, list.Count)

            Dim sa As StringAnalysis = DirectCast(list(0), StringAnalysis)
            assertEquals(2, sa.getMinLength())
            assertEquals(4, sa.getMaxLength())
            assertEquals(3.0, sa.getMeanLength(), 1.0E-12)
            assertEquals(2.0, sa.getSampleVarianceLength(), 1.0E-12)
            assertEquals(2L, sa.getCountTotal())
            assertArrayEquals(New Double() {2, 4}, minsMaxes(0), 0.0)

            Dim ia As IntegerAnalysis = DirectCast(list(1), IntegerAnalysis)
            assertEquals(-3, ia.getMin())
            assertEquals(7, ia.getMax())
            assertEquals(2.0, ia.getMean(), 1.0E-12)
            assertEquals(50.0, ia.getSampleVariance(), 1.0E-12)
            assertEquals(1L, ia.getCountNegative())
            assertEquals(1L, ia.getCountPositive())
            assertEquals(2L, ia.getCountTotal())
            assertTrue(ia.getDigest() Is ic.getDigest())
            assertArrayEquals(New Double() {-3, 7}, minsMaxes(1), 0.0)

            Dim la As LongAnalysis = DirectCast(list(2), LongAnalysis)
            assertEquals(10L, la.getMin())
            assertEquals(30L, la.getMax())
            assertEquals(20.0, la.getMean(), 1.0E-12)
            assertEquals(10.0, la.getSampleStdev(), 1.0E-12)
            assertArrayEquals(New Double() {10, 30}, minsMaxes(2), 0.0)

            Dim da As DoubleAnalysis = DirectCast(list(3), DoubleAnalysis)
            assertEquals(-0.5, da.getMin(), 0.0)
            assertEquals(0.5, da.getMax(), 0.0)
            assertEquals(0L, da.getCountNaN())
            assertEquals(2L, da.getCountTotal())
            Dim fa As DoubleAnalysis = DirectCast(list(4), DoubleAnalysis) ' Float columns use the double analysis
            assertEquals(1.25, fa.getMin(), 0.0)
            assertArrayEquals(New Double() {1.25, 1.25}, minsMaxes(4), 0.0)

            Dim ca As CategoricalAnalysis = DirectCast(list(5), CategoricalAnalysis)
            assertEquals(New Dictionary(Of String, Long) From {{"x", 2L}, {"y", 1L}}, ca.getMapOfCounts())
            assertArrayEquals(New Double() {0, 0}, minsMaxes(5), 0.0) ' not set for categorical columns

            Dim ta As TimeAnalysis = DirectCast(list(6), TimeAnalysis)
            assertEquals(1000L, ta.getMin())
            assertEquals(5000L, ta.getMax())
            assertArrayEquals(New Double() {1000, 5000}, minsMaxes(6), 0.0)

            assertEquals(4L, DirectCast(list(7), BytesAnalysis).CountTotal)

            Dim boolA As IntegerAnalysis = DirectCast(list(8), IntegerAnalysis) ' Boolean columns use the integer analysis
            assertEquals(0, boolA.getMin())
            assertEquals(1, boolA.getMax())
            assertEquals(2.0 / 3.0, boolA.getMean(), 1.0E-12)
            assertEquals(1L, boolA.getCountZero())
        End Sub

        <TestMethod>
        Public Sub convertCountersRejectsNDArrayColumns()
            Dim minsMaxes()() As Double = {New Double(1) {}}
            assertThrows(Of NotSupportedException)(
                Sub() Call DataVecAnalysisUtils.convertCounters(New List(Of AnalysisCounterT) From {New BytesAnalysisCounter()}, minsMaxes, New List(Of ColumnTypeEnum) From {ColumnTypeEnum.NDArray}))
        End Sub

        <TestMethod>
        Public Sub mergeCountersAttachesTheHistograms()
            Dim analyses As IList(Of ColumnAnalysisT) = threeAnalyses()
            Dim ih As New DoubleHistogramCounter(1, 3, 2)
            ih.add(New IntWritable(1)).add(New IntWritable(2)).add(New IntWritable(3))
            Dim sh As New StringHistogramCounter(1, 2, 1)
            sh.add(New TextT("a")).add(New TextT("bb"))
            Dim histograms As New List(Of HistogramCounterT) From {ih, sh, ih}

            DataVecAnalysisUtils.mergeCounters(analyses, histograms)
            Dim ia As IntegerAnalysis = DirectCast(analyses(0), IntegerAnalysis)
            assertArrayEquals(New Double() {1, 2, 3}, ia.getHistogramBuckets(), 0.0)
            assertArrayEquals(New Long() {1, 2}, ia.getHistogramBucketCounts())
            Dim sa As StringAnalysis = DirectCast(analyses(1), StringAnalysis)
            assertArrayEquals(New Double() {0, 2}, sa.getHistogramBuckets(), 0.0)
            assertArrayEquals(New Long() {2}, sa.getHistogramBucketCounts())

            ' no histograms: nothing to do
            Dim untouched As IList(Of ColumnAnalysisT) = threeAnalyses()
            DataVecAnalysisUtils.mergeCounters(untouched, Nothing)
            assertNull(DirectCast(untouched(0), IntegerAnalysis).getHistogramBuckets())
        End Sub

        <TestMethod>
        Public Sub sequenceDataAnalysisSurvivesBinarySerialization()
            ' Java's analysis classes are Serializable (Spark ships them between executors); here they are <Serializable>
            Dim digest As TDigest = TDigest.createDigest(100)
            For i As Integer = 1 To 100
                digest.add(CDbl(i))
            Next i
            Dim analyses As IList(Of ColumnAnalysisT) = threeAnalyses()
            DirectCast(analyses(0), IntegerAnalysis).setDigest(digest)
            DirectCast(analyses(0), IntegerAnalysis).setHistogramBuckets(New Double() {1, 2, 3})
            Dim original As New SequenceDataAnalysisT(threeColumnSchema(), analyses, sequenceLengths())

            Dim copy As SequenceDataAnalysisT = SerializationUtils.fromByteArray(Of SequenceDataAnalysisT)(SerializationUtils.toByteArray(original))
            assertTrue(copy IsNot original)
            assertEquals(original, copy)
            assertEquals(original.ToString(), copy.ToString())
            Dim copiedDigest As TDigest = DirectCast(copy.getColumnAnalysis("int"), IntegerAnalysis).getDigest()
            assertEquals(100L, copiedDigest.size())
            assertEquals(digest.quantile(0.5), copiedDigest.quantile(0.5), 1.0E-9)
        End Sub

    End Class

End Namespace
