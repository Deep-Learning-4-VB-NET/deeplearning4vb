Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.analysis.columns
Imports ColumnAnalysisT = DeepLearning4VB.org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType
Imports TDigest = DeepLearning4VB.com.tdunning.math.stats.TDigest

Namespace org.datavec.api.transform.analysis.columns

    ''' <summary>
    ''' Tests of the column analysis classes: builders, Lombok accessors and equality, the column type and the Java text
    ''' of <c>ToString</c> (Java number formatting, quantiles from the t-digest).
    ''' </summary>
    <TestClass>
    Public Class ColumnAnalysisTest

        Private Shared Function digestOf(ByVal fromValue As Integer, ByVal toValue As Integer) As TDigest
            Dim d As TDigest = TDigest.createDigest(100)
            For i As Integer = fromValue To toValue
                d.add(CDbl(i))
            Next i
            Return d
        End Function

        Private Shared Function sampleIntegerAnalysis(ByVal digest As TDigest) As IntegerAnalysis
            Return (New IntegerAnalysis.Builder()).min(1).max(9).mean(5.0).sampleStdev(2.5).sampleVariance(6.25).countZero(0).countNegative(0).countPositive(9).countMinValue(1).countMaxValue(1).countTotal(9).digest(digest).build()
        End Function

        ' ---------------- numerical analyses ----------------

        <TestMethod>
        Public Sub integerAnalysisBuilderAndGetters()
            Dim a As IntegerAnalysis = sampleIntegerAnalysis(Nothing)
            assertEquals(1, a.getMin())
            assertEquals(9, a.getMax())
            assertEquals(1.0, a.MinDouble, 0.0)
            assertEquals(9.0, a.MaxDouble, 0.0)
            assertEquals(5.0, a.getMean(), 0.0)
            assertEquals(2.5, a.getSampleStdev(), 0.0)
            assertEquals(6.25, a.getSampleVariance(), 0.0)
            assertEquals(9L, a.getCountPositive())
            assertEquals(1L, a.getCountMinValue())
            assertEquals(1L, a.getCountMaxValue())
            assertEquals(9L, a.CountTotal)
            assertEquals(9L, a.getCountTotal())
            assertEquals(ColumnTypeEnum.Integer, a.ColumnType)
            assertEquals(ColumnTypeEnum.Integer, a.getColumnType())
            assertNull(a.getDigest())
            assertNull(a.getHistogramBuckets())
        End Sub

        <TestMethod>
        Public Sub integerAnalysisToStringWithoutDigest()
            Dim expected As String = "IntegerAnalysis(min=1,max=9,mean=5.0,sampleStDev=2.5,sampleVariance=6.25,countZero=0,countNegative=0," &
                                     "countPositive=9,countMinValue=1,countMaxValue=1,count=9)"
            assertEquals(expected, sampleIntegerAnalysis(Nothing).ToString())
        End Sub

        <TestMethod>
        Public Sub numericalToStringListsTheDigestQuantiles()
            Dim s As String = sampleIntegerAnalysis(digestOf(1, 1000)).ToString()
            Dim m As Match = Regex.Match(s, ", quantiles=\[0\.001 -> ([^,]+),0\.01 -> ([^,]+),0\.1 -> ([^,]+),0\.5 -> ([^,]+),0\.9 -> ([^,]+),0\.99 -> ([^,]+),0\.999 -> ([^\]]+)\]\)$")
            assertTrue(m.Success, s)
            assertTrue(s.StartsWith("IntegerAnalysis(min=1,max=9,mean=5.0,", StringComparison.Ordinal))
            Dim expectedQuantiles() As Double = {1.5, 10.5, 100.5, 500.5, 900.5, 990.5, 999.5}
            For i As Integer = 0 To expectedQuantiles.Length - 1
                Dim q As Double = Double.Parse(m.Groups(i + 1).Value, CultureInfo.InvariantCulture)
                assertEquals(expectedQuantiles(i), q, 5.0, "quantile " & i)
            Next i
        End Sub

        <TestMethod>
        Public Sub numericalEqualityIgnoresTheDigest()
            Dim a As IntegerAnalysis = sampleIntegerAnalysis(digestOf(1, 10))
            Dim b As IntegerAnalysis = sampleIntegerAnalysis(digestOf(100, 200))
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())

            b.setMax(10)
            assertNotEquals(a, b)
            b.setMax(9)
            b.setMean(5.5)
            assertNotEquals(a, b)
            b.setMean(5.0)

            ' histograms are compared by content
            a.setHistogramBuckets(New Double() {0, 5, 10})
            a.setHistogramBucketCounts(New Long() {4, 5})
            assertNotEquals(a, b)
            b.setHistogramBuckets(New Double() {0, 5, 10})
            b.setHistogramBucketCounts(New Long() {4, 5})
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
        End Sub

        <TestMethod>
        Public Sub numericalAnalysesOfDifferentClassesAreNotEqual()
            Dim i As IntegerAnalysis = (New IntegerAnalysis.Builder()).min(1).max(2).countTotal(3).build()
            Dim l As LongAnalysis = (New LongAnalysis.Builder()).min(1).max(2).countTotal(3).build()
            Dim t As TimeAnalysis = (New TimeAnalysis.Builder()).min(1).max(2).countTotal(3).build()
            assertNotEquals(i, l)
            assertNotEquals(l, t)
            assertEquals(l, (New LongAnalysis.Builder()).min(1).max(2).countTotal(3).build())
            assertEquals(New IntegerAnalysis(), New IntegerAnalysis())
        End Sub

        <TestMethod>
        Public Sub longAnalysisGettersAndText()
            Dim a As LongAnalysis = (New LongAnalysis.Builder()).min(-5).max(10000000000L).mean(2.5).sampleStdev(1.0).sampleVariance(1.0).countZero(1).countNegative(1).countPositive(2).countMinValue(1).countMaxValue(2).countTotal(3).build()
            assertEquals(-5L, a.getMin())
            assertEquals(10000000000L, a.getMax())
            assertEquals(1.0E+10, a.MaxDouble, 0.0)
            assertEquals(ColumnTypeEnum.Long, a.ColumnType)
            assertEquals("LongAnalysis(min=-5,max=10000000000,mean=2.5,sampleStDev=1.0,sampleVariance=1.0,countZero=1,countNegative=1," &
                         "countPositive=2,countMinValue=1,countMaxValue=2,count=3)", a.ToString())
        End Sub

        <TestMethod>
        Public Sub doubleAnalysisGettersTextAndEquality()
            Dim a As DoubleAnalysis = (New DoubleAnalysis.Builder()).min(-1.5).max(2.0).mean(0.25).sampleStdev(1.0E-4).sampleVariance(1.0E-8).countZero(0).countNegative(1).countPositive(1).countMinValue(1).countMaxValue(1).countNaN(3).countTotal(2).build()
            assertEquals(-1.5, a.getMin(), 0.0)
            assertEquals(2.0, a.getMax(), 0.0)
            assertEquals(3L, a.getCountNaN())
            assertEquals(ColumnTypeEnum.Double, a.ColumnType)
            ' Java Double.toString formatting (1.0E-4, 1.0E-8); countNaN is not printed
            assertEquals("DoubleAnalysis(min=-1.5,max=2.0,mean=0.25,sampleStDev=1.0E-4,sampleVariance=1.0E-8,countZero=0,countNegative=1," &
                         "countPositive=1,countMinValue=1,countMaxValue=1,count=2)", a.ToString())

            Dim b As DoubleAnalysis = (New DoubleAnalysis.Builder()).min(-1.5).max(2.0).mean(0.25).sampleStdev(1.0E-4).sampleVariance(1.0E-8).countZero(0).countNegative(1).countPositive(1).countMinValue(1).countMaxValue(1).countNaN(3).countTotal(2).build()
            assertEquals(a, b)
            b.setCountNaN(4)
            assertNotEquals(a, b)
        End Sub

        <TestMethod>
        Public Sub timeAnalysisPrintsUtcTimesAndReportsLongType()
            Dim a As TimeAnalysis = (New TimeAnalysis.Builder()).min(0).max(1500000000000L).mean(7.5E+11).sampleStdev(0).sampleVariance(0).countTotal(2).countPositive(2).countZero(1).countMinValue(1).countMaxValue(1).build()
            assertEquals(0L, a.getMin())
            assertEquals(1500000000000L, a.getMax())
            ' As in Java, the time analysis reports the Long column type
            assertEquals(ColumnTypeEnum.Long, a.ColumnType)
            assertEquals("TimeAnalysis(min=0 (1970-01-01 00:00:00.000 UTC),max=1500000000000 (2017-07-14 02:40:00.000 UTC)," &
                         "mean=7.5E11,sampleStDev=0.0,sampleVariance=0.0,countZero=1,countNegative=0,countPositive=2,countMinValue=1," &
                         "countMaxValue=1,count=2)", a.ToString())
        End Sub

        ' ---------------- string / bytes / categorical / NDArray ----------------

        <TestMethod>
        Public Sub stringAnalysisBuilderAndAllArgsConstructor()
            Dim a As StringAnalysis = (New StringAnalysis.Builder()).minLength(0).maxLength(5).meanLength(2.5).sampleStdevLength(2.0).sampleVarianceLength(4.0).countTotal(6).build()
            assertEquals(0, a.getMinLength())
            assertEquals(5, a.getMaxLength())
            assertEquals(2.5, a.getMeanLength(), 0.0)
            assertEquals(6L, a.CountTotal)
            assertEquals(6L, a.getCountTotal())
            assertEquals(ColumnTypeEnum.String, a.ColumnType)
            assertEquals("StringAnalysis(minLen=0,maxLen=5,meanLen=2.5,sampleStDevLen=2.0,sampleVarianceLen=4.0,count=6)", a.ToString())

            Dim allArgs As New StringAnalysis(0, 5, 2.5, 2.0, 4.0, 6, Nothing, Nothing)
            assertEquals(a, allArgs)
            assertEquals(a.GetHashCode(), allArgs.GetHashCode())
            allArgs.setHistogramBucketCounts(New Long() {1, 5})
            assertNotEquals(a, allArgs)
        End Sub

        <TestMethod>
        Public Sub bytesAnalysisBuilderAndGetters()
            Dim a As BytesAnalysis = (New BytesAnalysis.Builder()).countTotal(7).countNull(1).countZeroLength(2).minNumBytes(3).maxNumBytes(40).build()
            assertEquals(7L, a.CountTotal)
            assertEquals(7L, a.getCountTotal())
            assertEquals(1L, a.getCountNull())
            assertEquals(2L, a.getCountZeroLength())
            assertEquals(3, a.getMinNumBytes())
            assertEquals(40, a.getMaxNumBytes())
            assertEquals(ColumnTypeEnum.Bytes, a.ColumnType)
            assertEquals("BytesAnalysis()", a.ToString())
            assertEquals(New BytesAnalysis(7, 1, 2, 3, 40), a)
        End Sub

        <TestMethod>
        Public Sub categoricalAnalysisSortsByDescendingCount()
            Dim counts As New Dictionary(Of String, Long) From {{"a", 1L}, {"b", 5L}, {"c", 3L}}
            Dim a As New CategoricalAnalysis(counts)
            assertEquals(9L, a.CountTotal)
            assertEquals(ColumnTypeEnum.Categorical, a.ColumnType)
            assertEquals("CategoricalAnalysis(CategoryCounts={b=5, c=3, a=1})", a.ToString())
            assertTrue(a.getMapOfCounts() Is counts)

            ' a stable sort: ties keep the map order
            Dim ties As New CategoricalAnalysis(New Dictionary(Of String, Long) From {{"x", 2L}, {"y", 2L}, {"z", 1L}})
            assertEquals("CategoricalAnalysis(CategoryCounts={x=2, y=2, z=1})", ties.ToString())

            assertEquals(a, New CategoricalAnalysis(New Dictionary(Of String, Long) From {{"c", 3L}, {"b", 5L}, {"a", 1L}}))
            assertNotEquals(a, ties)
        End Sub

        <TestMethod>
        Public Sub ndArrayAnalysisPrintsRanksInOrder()
            Dim byRank As New Dictionary(Of Integer, Long) From {{3, 2L}, {1, 5L}}
            Dim a As NDArrayAnalysis = NDArrayAnalysis.newBuilder().countTotal(7).countNull(0).minLength(4).maxLength(12).totalNDArrayValues(50).countsByRank(byRank).minValue(-1.5).maxValue(3.0).build()
            assertEquals(7L, a.CountTotal)
            assertEquals(ColumnTypeEnum.NDArray, a.ColumnType)
            assertEquals(-1.5, a.getMinValue(), 0.0)
            assertEquals(3.0, a.getMaxValue(), 0.0)
            ' the counts by rank are printed sorted by rank
            assertEquals("NDArrayAnalysis(countTotal=7,countNull=0,minLength=4,maxLength=12,totalValuesAllNDArrays=50,minValue=-1.5,maxValue=3.0," &
                         "countsByNDArrayRank={1=5, 3=2})", a.ToString())
            assertEquals(a, New NDArrayAnalysis(7, 0, 4, 12, 50, New Dictionary(Of Integer, Long) From {{1, 5L}, {3, 2L}}, -1.5, 3.0, Nothing, Nothing))
            assertTrue((New NDArrayAnalysis()).ToString().EndsWith(",countsByNDArrayRank={})", StringComparison.Ordinal))
        End Sub

        <TestMethod>
        Public Sub allAnalysesImplementTheColumnAnalysisInterface()
            Dim all As New List(Of ColumnAnalysisT) From {
                sampleIntegerAnalysis(Nothing),
                (New LongAnalysis.Builder()).countTotal(2).build(),
                (New DoubleAnalysis.Builder()).countTotal(3).build(),
                (New TimeAnalysis.Builder()).countTotal(4).build(),
                (New StringAnalysis.Builder()).countTotal(5).build(),
                (New BytesAnalysis.Builder()).countTotal(6).build(),
                New CategoricalAnalysis(New Dictionary(Of String, Long) From {{"a", 7L}}),
                NDArrayAnalysis.newBuilder().countTotal(8).build()}
            Dim expectedTypes() As ColumnTypeEnum = {ColumnTypeEnum.Integer, ColumnTypeEnum.Long, ColumnTypeEnum.Double, ColumnTypeEnum.Long,
                                                    ColumnTypeEnum.String, ColumnTypeEnum.Bytes, ColumnTypeEnum.Categorical, ColumnTypeEnum.NDArray}
            Dim expectedCounts() As Long = {9, 2, 3, 4, 5, 6, 7, 8}
            For i As Integer = 0 To all.Count - 1
                assertEquals(expectedTypes(i), all(i).ColumnType)
                assertEquals(expectedTypes(i), all(i).getColumnType())
                assertEquals(expectedCounts(i), all(i).CountTotal)
                assertEquals(expectedCounts(i), all(i).getCountTotal())
                assertTrue(isSerializableType(CObj(all(i)).GetType()))
            Next i
        End Sub

    End Class

End Namespace
