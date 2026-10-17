Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.transform.analysis.counter
Imports AnalysisCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.AnalysisCounter
Imports DoubleWritable = DeepLearning4VB.org.datavec.api.writable.DoubleWritable
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports LongWritable = DeepLearning4VB.org.datavec.api.writable.LongWritable
Imports SerializationUtils = DeepLearning4VB.org.nd4vb.common.util.SerializationUtils
Imports TDigest = DeepLearning4VB.com.tdunning.math.stats.TDigest
Imports TextT = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.transform.analysis.counter

    ''' <summary>
    ''' Tests of the analysis counters (Double, Integer, Long, String, Categorical, Bytes) against the Java semantics:
    ''' zero counts as positive, minimum/maximum counts are tracked while adding and combined while merging, the
    ''' standard deviation and variance are the sample ones, and merge updates the left counter in place.
    ''' </summary>
    <TestClass>
    Public Class AnalysisCountersTest

        Private Shared Function doubles(ByVal ParamArray values() As Double) As DoubleAnalysisCounter
            Dim c As New DoubleAnalysisCounter()
            For Each v As Double In values
                c.add(New DoubleWritable(v))
            Next v
            Return c
        End Function

        Private Shared Function ints(ByVal ParamArray values() As Integer) As IntegerAnalysisCounter
            Dim c As New IntegerAnalysisCounter()
            For Each v As Integer In values
                c.add(New IntWritable(v))
            Next v
            Return c
        End Function

        Private Shared Function strings(ByVal ParamArray values() As String) As StringAnalysisCounter
            Dim c As New StringAnalysisCounter()
            For Each v As String In values
                c.add(New TextT(v))
            Next v
            Return c
        End Function

        ' ---------------- DoubleAnalysisCounter ----------------

        <TestMethod>
        Public Sub doubleCounterCountsAndStatistics()
            Dim c As DoubleAnalysisCounter = doubles(2.0, -1.0, 0.0, 5.0, 5.0, -1.0, 3.0)
            assertEquals(7L, c.CountTotal)
            assertEquals(1L, c.getCountZero())
            ' Java counts value >= 0 as positive, so zero is positive too
            assertEquals(5L, c.getCountPositive())
            assertEquals(2L, c.getCountNegative())
            assertEquals(-1.0, c.MinValueSeen, 0.0)
            assertEquals(5.0, c.MaxValueSeen, 0.0)
            assertEquals(2L, c.getCountMinValue())
            assertEquals(2L, c.getCountMaxValue())
            assertEquals(13.0, c.Sum, 1.0E-12)
            assertEquals(13.0 / 7.0, c.Mean, 1.0E-12)
            ' sample variance (divide by n - 1): sum of squared deviations 286/7 over 6
            assertEquals(286.0 / 42.0, c.SampleVariance, 1.0E-12)
            assertEquals(Math.Sqrt(286.0 / 42.0), c.SampleStdev, 1.0E-12)
            assertEquals(0L, c.getCountNaN())
            assertEquals(7L, c.getDigest().size())
            assertEquals(7L, c.getCounter().Count)
        End Sub

        <TestMethod>
        Public Sub doubleCounterEmptyStateAndDigest()
            Dim c As New DoubleAnalysisCounter()
            assertEquals(0L, c.CountTotal)
            assertEquals(Double.MaxValue, c.MinValueSeen, 0.0)
            assertEquals(-Double.MaxValue, c.MaxValueSeen, 0.0)
            assertEquals(100.0, c.getDigest().compression(), 0.0)

            For i As Integer = 1 To 1000
                c.add(New DoubleWritable(i))
            Next i
            assertEquals(500.5, c.getDigest().quantile(0.5), 5.0)
            assertEquals(900.0, c.getDigest().quantile(0.9), 10.0)
            assertEquals(1.0, c.getDigest().quantile(0.0), 1.0E-9)
            assertEquals(1000.0, c.getDigest().quantile(1.0), 1.0E-9)
        End Sub

        <TestMethod>
        Public Sub doubleCounterNaNIsNotCountedAndRejectedByTheDigest()
            ' Java tests value == Double.NaN (never true), then t-digest's add throws IllegalArgumentException for NaN
            Dim c As DoubleAnalysisCounter = doubles(1.0)
            assertThrows(Of ArgumentException)(Sub() Call c.add(New DoubleWritable(Double.NaN)))
            assertEquals(0L, c.getCountNaN())
        End Sub

        <TestMethod>
        Public Sub doubleCounterMergeKeepsSmallerMinAndLargerMax()
            Dim c1 As DoubleAnalysisCounter = doubles(1.0, 2.0, 2.0)
            Dim c2 As DoubleAnalysisCounter = doubles(2.0, 5.0, 5.0)
            assertEquals(1L, c1.getCountMinValue())
            assertEquals(2L, c1.getCountMaxValue())
            assertEquals(1L, c2.getCountMinValue())
            assertEquals(2L, c2.getCountMaxValue())

            Dim merged As DoubleAnalysisCounter = c1.merge(c2)
            assertEquals(6L, merged.CountTotal)
            assertEquals(1.0, merged.MinValueSeen, 0.0)
            assertEquals(5.0, merged.MaxValueSeen, 0.0)
            assertEquals(1L, merged.getCountMinValue()) ' this min is smaller: keep this count
            assertEquals(2L, merged.getCountMaxValue()) ' other max is larger: take the other count
            assertEquals(6L, merged.getCountPositive())
            assertEquals(0L, merged.getCountNegative())
            assertEquals(17.0 / 6.0, merged.Mean, 1.0E-12)
            assertEquals(89.0 / 30.0, merged.SampleVariance, 1.0E-12)
            ' as in Java, the left counter's statistics and digest are updated in place and shared
            assertTrue(merged.getCounter() Is c1.getCounter())
            assertTrue(merged.getDigest() Is c1.getDigest())
            assertEquals(6L, c1.getDigest().size())
        End Sub

        <TestMethod>
        Public Sub doubleCounterMergeAddsCountsOfEqualExtremes()
            Dim c1 As DoubleAnalysisCounter = doubles(1.0, 3.0)
            Dim c2 As DoubleAnalysisCounter = doubles(1.0, 1.0, 2.0)
            Dim merged As DoubleAnalysisCounter = c1.merge(c2)
            assertEquals(3L, merged.getCountMinValue()) ' equal minimum: counts added
            assertEquals(1L, merged.getCountMaxValue()) ' this max is larger: keep this count
            assertEquals(3.0, merged.MaxValueSeen, 0.0)
        End Sub

        <TestMethod>
        Public Sub doubleCounterMergeIntoEmptyCounter()
            Dim c As DoubleAnalysisCounter = doubles(2.0, 4.0, 4.0, 4.0, 5.0, 5.0, 7.0, 9.0)
            Dim merged As DoubleAnalysisCounter = (New DoubleAnalysisCounter()).merge(c)
            assertEquals(8L, merged.CountTotal)
            assertEquals(2.0, merged.MinValueSeen, 0.0)
            assertEquals(9.0, merged.MaxValueSeen, 0.0)
            assertEquals(1L, merged.getCountMinValue())
            assertEquals(1L, merged.getCountMaxValue())
            ' the mean and variance are those of the other counter (Java corrupted them, see StatCounter)
            assertEquals(5.0, merged.Mean, 1.0E-12)
            assertEquals(32.0 / 7.0, merged.SampleVariance, 1.0E-12)
        End Sub

        <TestMethod>
        Public Sub doubleCounterAllArgsConstructorAndLombokMembers()
            Dim stats As New StatCounter()
            stats.add(4.0)
            Dim digest As TDigest = TDigest.createDigest(100)
            Dim a As New DoubleAnalysisCounter(stats, 1, 2, 3, 4, 5, 6, digest)
            Dim b As New DoubleAnalysisCounter(stats, 1, 2, 3, 4, 5, 6, digest)
            assertEquals(1L, a.getCountZero())
            assertEquals(2L, a.getCountMinValue())
            assertEquals(3L, a.getCountMaxValue())
            assertEquals(4L, a.getCountPositive())
            assertEquals(5L, a.getCountNegative())
            assertEquals(6L, a.getCountNaN())
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            b.setCountNaN(7)
            assertNotEquals(a, b)
            ' StatCounter has no equals in Java, so different counters are never equal
            assertNotEquals(New DoubleAnalysisCounter(), New DoubleAnalysisCounter())
            assertTrue(a.ToString().StartsWith("DoubleAnalysisCounter(counter=StatCounter(count=1,mean=4.0,", StringComparison.Ordinal))
            assertTrue(a.ToString().Contains(", countZero=1, countMinValue=2, countMaxValue=3, countPositive=4, countNegative=5, countNaN=6, digest="))
        End Sub

        ' ---------------- IntegerAnalysisCounter ----------------

        <TestMethod>
        Public Sub integerCounterCountsAndStatistics()
            Dim c As IntegerAnalysisCounter = ints(3, -2, 0, 7, 7, -2, -2)
            assertEquals(7L, c.CountTotal)
            assertEquals(1L, c.getCountZero())
            assertEquals(4L, c.getCountPositive())
            assertEquals(3L, c.getCountNegative())
            assertEquals(-2, c.MinValueSeen)
            assertEquals(7, c.MaxValueSeen)
            assertEquals(3L, c.getCountMinValue())
            assertEquals(2L, c.getCountMaxValue())
            assertEquals(11L, c.Sum)
            assertEquals(11.0 / 7.0, c.Mean, 1.0E-12)
            Dim mean As Double = 11.0 / 7.0
            Dim m2 As Double = 0
            For Each v As Integer In New Integer() {3, -2, 0, 7, 7, -2, -2}
                m2 += (v - mean) * (v - mean)
            Next v
            assertEquals(m2 / 6, c.SampleVariance, 1.0E-12)
            assertEquals(Math.Sqrt(m2 / 6), c.SampleStdev, 1.0E-12)
            assertEquals(7L, c.getDigest().size())
        End Sub

        <TestMethod>
        Public Sub integerCounterEmptyStateUsesJavaSaturatingCasts()
            ' Java: (int) Double.MAX_VALUE = Integer.MAX_VALUE and (int) -Double.MAX_VALUE = Integer.MIN_VALUE
            Dim c As New IntegerAnalysisCounter()
            assertEquals(Integer.MaxValue, c.MinValueSeen)
            assertEquals(Integer.MinValue, c.MaxValueSeen)
            assertEquals(0L, c.Sum)

            ' so the first value always starts the min and max counts at 1, even the extreme values
            Dim e As IntegerAnalysisCounter = ints(Integer.MaxValue)
            assertEquals(1L, e.getCountMinValue())
            assertEquals(1L, e.getCountMaxValue())
            Dim f As IntegerAnalysisCounter = ints(Integer.MinValue)
            assertEquals(1L, f.getCountMinValue())
            assertEquals(1L, f.getCountMaxValue())
        End Sub

        <TestMethod>
        Public Sub integerCounterMerge()
            Dim c1 As IntegerAnalysisCounter = ints(5, 9, 9)
            Dim c2 As IntegerAnalysisCounter = ints(-4, 0, 9)
            Dim merged As IntegerAnalysisCounter = c1.merge(c2)
            assertEquals(6L, merged.CountTotal)
            assertEquals(-4, merged.MinValueSeen)
            assertEquals(9, merged.MaxValueSeen)
            assertEquals(1L, merged.getCountMinValue()) ' other min is smaller: take the other count
            assertEquals(3L, merged.getCountMaxValue()) ' equal maximum: 2 + 1
            assertEquals(1L, merged.getCountZero())
            assertEquals(5L, merged.getCountPositive())
            assertEquals(1L, merged.getCountNegative())
            assertEquals(28L, merged.Sum)
            assertEquals(6L, merged.getDigest().size())
        End Sub

        ' ---------------- LongAnalysisCounter ----------------

        <TestMethod>
        Public Sub longCounterCountsAndStatistics()
            Dim c As New LongAnalysisCounter()
            For Each v As Long In New Long() {10000000000L, -5L, 0L, 10000000000L}
                c.add(New LongWritable(v))
            Next v
            assertEquals(4L, c.CountTotal)
            assertEquals(1L, c.getCountZero())
            assertEquals(3L, c.getCountPositive())
            assertEquals(1L, c.getCountNegative())
            assertEquals(-5L, c.MinValueSeen)
            assertEquals(10000000000L, c.MaxValueSeen)
            assertEquals(1L, c.getCountMinValue())
            assertEquals(2L, c.getCountMaxValue())
            assertEquals(19999999995L, c.Sum)
            assertEquals(19999999995.0 / 4, c.Mean, 1.0E-3)
        End Sub

        <TestMethod>
        Public Sub longCounterEmptyStateAndMerge()
            Dim empty As New LongAnalysisCounter()
            assertEquals(Long.MaxValue, empty.MinValueSeen)
            assertEquals(Long.MinValue, empty.MaxValueSeen)

            Dim c1 As New LongAnalysisCounter()
            c1.add(New LongWritable(3))
            c1.add(New LongWritable(8))
            Dim c2 As New LongAnalysisCounter()
            c2.add(New LongWritable(3))
            c2.add(New LongWritable(1))
            Dim merged As LongAnalysisCounter = c1.merge(c2)
            assertEquals(4L, merged.CountTotal)
            assertEquals(1L, merged.MinValueSeen)
            assertEquals(8L, merged.MaxValueSeen)
            assertEquals(1L, merged.getCountMinValue())
            assertEquals(1L, merged.getCountMaxValue())
            assertEquals(15L, merged.Sum)
            assertEquals(3.75, merged.Mean, 1.0E-12)
        End Sub

        ' ---------------- StringAnalysisCounter ----------------

        <TestMethod>
        Public Sub stringCounterLengthStatistics()
            Dim c As StringAnalysisCounter = strings("abc", "", "hello", "hi", "", "world")
            assertEquals(6L, c.CountTotal)
            assertEquals(2L, c.getCountZeroLength())
            assertEquals(0, c.MinLengthSeen)
            assertEquals(5, c.MaxLengthSeen)
            assertEquals(2L, c.getCountMinLength())
            assertEquals(2L, c.getCountMaxLength())
            assertEquals(15L, c.SumLength)
            assertEquals(2.5, c.Mean, 1.0E-12)
            assertEquals(5.1, c.SampleVariance, 1.0E-12)
            assertEquals(Math.Sqrt(5.1), c.SampleStdev, 1.0E-12)
        End Sub

        <TestMethod>
        Public Sub stringCounterCountsUtf16CodeUnitsLikeJava()
            ' Java String.length() counts UTF-16 code units: a character outside the BMP counts twice
            Dim c As StringAnalysisCounter = strings("h" & ChrW(&HE9) & "llo", Char.ConvertFromUtf32(&H1F600))
            assertEquals(2, c.MinLengthSeen)
            assertEquals(5, c.MaxLengthSeen)
        End Sub

        <TestMethod>
        Public Sub stringCounterEmptyStateAndMerge()
            Dim empty As New StringAnalysisCounter()
            assertEquals(Integer.MaxValue, empty.MinLengthSeen)
            assertEquals(Integer.MinValue, empty.MaxLengthSeen)

            Dim c1 As StringAnalysisCounter = strings("a", "bb")
            Dim c2 As StringAnalysisCounter = strings("ccc", "d")
            Dim merged As StringAnalysisCounter = c1.merge(c2)
            assertEquals(4L, merged.CountTotal)
            assertEquals(1, merged.MinLengthSeen)
            assertEquals(3, merged.MaxLengthSeen)
            assertEquals(2L, merged.getCountMinLength()) ' equal minimum length: 1 + 1
            assertEquals(1L, merged.getCountMaxLength()) ' other is longer: take the other count
            assertEquals(0L, merged.getCountZeroLength())
            assertEquals(1.75, merged.Mean, 1.0E-12)
            assertTrue(merged.getCounter() Is c1.getCounter())
        End Sub

        ' ---------------- CategoricalAnalysisCounter ----------------

        <TestMethod>
        Public Sub categoricalCounterCountsValues()
            Dim c As New CategoricalAnalysisCounter()
            For Each s As String In New String() {"a", "b", "a", "c", "a"}
                c.add(New TextT(s))
            Next s
            c.add(New IntWritable(1)) ' counted by its text
            assertEquals(6L, c.getCountTotal())
            Dim expected As New Dictionary(Of String, Long) From {{"a", 3L}, {"b", 1L}, {"c", 1L}, {"1", 1L}}
            assertEquals(expected, c.getCounts())
        End Sub

        <TestMethod>
        Public Sub categoricalCounterMergeIsInPlace()
            Dim c1 As New CategoricalAnalysisCounter()
            c1.add(New TextT("a")).add(New TextT("b"))
            Dim c2 As New CategoricalAnalysisCounter()
            c2.add(New TextT("b")).add(New TextT("d")).add(New TextT("d"))
            Dim merged As CategoricalAnalysisCounter = c1.merge(c2)
            assertTrue(merged Is c1)
            assertEquals(5L, merged.getCountTotal())
            Dim expected As New Dictionary(Of String, Long) From {{"a", 1L}, {"b", 2L}, {"d", 2L}}
            assertEquals(expected, merged.getCounts())
            ' the other counter is unchanged
            assertEquals(3L, c2.getCountTotal())
            assertEquals(2, c2.getCounts().Count)
        End Sub

        ' ---------------- BytesAnalysisCounter ----------------

        <TestMethod>
        Public Sub bytesCounterCountsAndMergesIntoANewCounter()
            Dim c1 As New BytesAnalysisCounter()
            c1.add(New TextT("x")).add(New TextT("y")).add(New TextT("z"))
            Dim c2 As New BytesAnalysisCounter(4)
            Dim merged As BytesAnalysisCounter = c1.merge(c2)
            assertEquals(7L, merged.getCountTotal())
            assertTrue(merged IsNot c1)
            assertEquals(3L, c1.getCountTotal())
            assertEquals(New BytesAnalysisCounter(7), merged)
            assertEquals("BytesAnalysisCounter(countTotal=7)", merged.ToString())
        End Sub

        ' ---------------- untyped (raw) AnalysisCounter view ----------------

        <TestMethod>
        Public Sub untypedCounterInterfaceAddsAndMerges()
            Dim counters As New List(Of AnalysisCounterT) From {New IntegerAnalysisCounter(), New StringAnalysisCounter(), New BytesAnalysisCounter()}
            For i As Integer = 0 To counters.Count - 1
                counters(i) = counters(i).add(New IntWritable(42))
            Next i
            Dim others As New List(Of AnalysisCounterT) From {ints(1), strings("abcd"), New BytesAnalysisCounter(2)}
            For i As Integer = 0 To counters.Count - 1
                counters(i) = counters(i).merge(others(i))
            Next i

            Dim ic As IntegerAnalysisCounter = DirectCast(counters(0), IntegerAnalysisCounter)
            assertEquals(2L, ic.CountTotal)
            assertEquals(1, ic.MinValueSeen)
            assertEquals(42, ic.MaxValueSeen)
            Dim sc As StringAnalysisCounter = DirectCast(counters(1), StringAnalysisCounter)
            assertEquals(2, sc.MinLengthSeen) ' "42"
            assertEquals(4, sc.MaxLengthSeen)
            assertEquals(3L, DirectCast(counters(2), BytesAnalysisCounter).getCountTotal())

            ' Java's raw merge casts the argument: a counter of another class is rejected
            assertThrows(Of InvalidCastException)(Sub() Call counters(0).merge(New BytesAnalysisCounter()))
        End Sub

        <TestMethod>
        Public Sub countersSurviveBinarySerializationAndKeepCounting()
            Dim c As DoubleAnalysisCounter = doubles(1.0, 2.0, 3.0)
            Dim copy As DoubleAnalysisCounter = SerializationUtils.fromByteArray(Of DoubleAnalysisCounter)(SerializationUtils.toByteArray(c))
            copy.add(New DoubleWritable(4.0))
            assertEquals(4L, copy.CountTotal)
            assertEquals(2.5, copy.Mean, 1.0E-12)
            assertEquals(4.0, copy.MaxValueSeen, 0.0)
            assertEquals(4L, copy.getDigest().size())
            assertEquals(3L, c.CountTotal)

            Dim cat As New CategoricalAnalysisCounter()
            cat.add(New TextT("a")).add(New TextT("a"))
            Dim catCopy As CategoricalAnalysisCounter = SerializationUtils.fromByteArray(Of CategoricalAnalysisCounter)(SerializationUtils.toByteArray(cat))
            assertEquals(cat, catCopy)
        End Sub

    End Class

End Namespace
