Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text.Json
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB
Imports DeepLearning4VB.com.tdunning.math.stats
Imports DeepLearning4VB.org.nd4vb.common.util

Namespace com.tdunning.math.stats

    ''' <summary>Tests for the merging t-digest shim (quantile and cdf estimation, merging, serialization).</summary>
    <TestClass>
    Public Class TDigestTest

        Private Const N As Integer = 10000

        ''' <summary>0..N-1 in a fixed-seed shuffled order.</summary>
        Private Shared Function ShuffledRange(ByVal seed As Integer) As Integer()
            Dim values(N - 1) As Integer
            For i As Integer = 0 To N - 1
                values(i) = i
            Next
            Dim rnd As New Random(seed)
            For i As Integer = N - 1 To 1 Step -1
                Dim j As Integer = rnd.Next(i + 1)
                Dim tmp As Integer = values(i)
                values(i) = values(j)
                values(j) = tmp
            Next
            Return values
        End Function

        Private Shared Function UniformDigest() As TDigest
            Dim d = TDigest.createDigest(100)
            For Each v As Integer In ShuffledRange(42)
                d.add(v)
            Next
            Return d
        End Function

        <TestMethod>
        Public Sub Uniform_SizeMinMaxAndExtremeQuantiles()
            Dim d = UniformDigest()
            assertEquals(10000L, d.size())
            assertEquals(100.0, d.compression(), 0.0)
            assertEquals(0.0, d.getMin(), 0.0)
            assertEquals(9999.0, d.getMax(), 0.0)
            assertEquals(0.0, d.quantile(0), 0.0)
            assertEquals(9999.0, d.quantile(1), 0.0)
            assertEquals(0.0, d.Min, 0.0)
            assertEquals(9999.0, d.Max, 0.0)
        End Sub

        <TestMethod>
        Public Sub Uniform_QuantilesAreAccurate()
            Dim d = UniformDigest()
            assertEquals(4999.5, d.quantile(0.5), 50.0, "median")
            assertEquals(999.5, d.quantile(0.1), 200.0, "q=0.1")
            assertEquals(8999.5, d.quantile(0.9), 200.0, "q=0.9")
            assertEquals(9899.5, d.quantile(0.99), 100.0, "q=0.99")
            assertEquals(99.5, d.quantile(0.01), 100.0, "q=0.01")
            assertEquals(2499.5, d.quantile(0.25), 100.0, "q=0.25")
            assertEquals(7499.5, d.quantile(0.75), 100.0, "q=0.75")
            ' quantile is non-decreasing in q and stays inside [min, max]
            Dim prev As Double = Double.NegativeInfinity
            For i As Integer = 0 To 100
                Dim q As Double = i / 100.0
                Dim v As Double = d.quantile(q)
                assertTrue(v >= prev, "quantile not monotonic at q=" & q)
                assertTrue(v >= 0.0 AndAlso v <= 9999.0, "quantile out of range at q=" & q)
                prev = v
            Next
        End Sub

        <TestMethod>
        Public Sub Uniform_CdfIsAccurateAndMonotonic()
            Dim d = UniformDigest()
            assertEquals(0.5, d.cdf(5000), 0.02, "cdf(5000)")
            assertEquals(0.1, d.cdf(1000), 0.02, "cdf(1000)")
            assertEquals(0.9, d.cdf(9000), 0.02, "cdf(9000)")
            assertEquals(0.0, d.cdf(-1), 0.0)
            assertTrue(d.cdf(0) <= 0.001, "cdf(min) = " & d.cdf(0))
            assertEquals(1.0, d.cdf(10000), 0.0)
            assertTrue(d.cdf(9999) >= 0.999, "cdf(max) = " & d.cdf(9999))
            Dim prev As Double = -1.0
            For x As Double = -100 To 10100 Step 25
                Dim c As Double = d.cdf(x)
                assertTrue(c >= prev, "cdf not monotonic at x=" & x & ": " & c & " < " & prev)
                assertTrue(c >= 0.0 AndAlso c <= 1.0, "cdf out of range at x=" & x)
                prev = c
            Next
            ' cdf and quantile are approximately inverse
            For Each q As Double In New Double() {0.05, 0.3, 0.5, 0.7, 0.95}
                assertEquals(q, d.cdf(d.quantile(q)), 0.02, "cdf(quantile(" & q & "))")
            Next
        End Sub

        <TestMethod>
        Public Sub Uniform_CentroidCountIsCompressed()
            Dim d = UniformDigest()
            Dim count As Integer = d.centroidCount()
            assertTrue(count > 1, "expected several centroids, got " & count)
            assertTrue(count < 1000, "expected far fewer centroids than samples, got " & count)
            Dim cs = d.centroids()
            assertEquals(count, cs.Count)
            Dim total As Double = 0
            Dim prevMean As Double = Double.NegativeInfinity
            For Each c As TDigest.Centroid In cs
                assertTrue(c.mean() >= prevMean, "centroids not sorted")
                assertTrue(c.count() >= 1.0)
                assertTrue(c.mean() >= 0.0 AndAlso c.mean() <= 9999.0)
                total += c.count()
                prevMean = c.mean()
                assertTrue(c.ToString().StartsWith("Centroid("))
            Next
            assertEquals(10000.0, total, 1.0E-6)
            assertEquals(count, d.CentroidMeans.Length)
            assertEquals(count, d.CentroidWeights.Length)
            d.compress()
            assertEquals(count, d.centroidCount())
        End Sub

        <TestMethod>
        Public Sub SameDataDifferentOrder_GivesSimilarEstimates()
            Dim a = UniformDigest()
            Dim b = TDigest.createDigest(100)
            For Each v As Integer In ShuffledRange(7)
                b.add(v)
            Next
            Dim c = TDigest.createDigest(100)
            For i As Integer = 0 To N - 1
                c.add(i)
            Next
            For Each q As Double In New Double() {0.01, 0.1, 0.5, 0.9, 0.99}
                assertEquals(a.quantile(q), b.quantile(q), 150.0, "q=" & q)
                assertEquals(a.quantile(q), c.quantile(q), 150.0, "sorted, q=" & q)
            Next
        End Sub

        <TestMethod>
        Public Sub SingleValue_Digest()
            Dim d = TDigest.createDigest(100)
            d.add(42.0)
            assertEquals(1L, d.size())
            assertEquals(42.0, d.getMin(), 0.0)
            assertEquals(42.0, d.getMax(), 0.0)
            assertEquals(42.0, d.quantile(0), 0.0)
            assertEquals(42.0, d.quantile(0.5), 0.0)
            assertEquals(42.0, d.quantile(1), 0.0)
            assertEquals(0.0, d.cdf(41.0), 0.0)
            assertEquals(0.5, d.cdf(42.0), 0.0)
            assertEquals(1.0, d.cdf(43.0), 0.0)
            assertEquals(1, d.centroidCount())
            assertEquals(42.0, d.centroids()(0).mean(), 0.0)
            assertEquals(1.0, d.centroids()(0).count(), 0.0)
        End Sub

        <TestMethod>
        Public Sub TwoValues_Digest()
            Dim d = TDigest.createDigest(100)
            d.add(10.0)
            d.add(20.0)
            assertEquals(2L, d.size())
            assertEquals(10.0, d.quantile(0), 0.0)
            assertEquals(20.0, d.quantile(1), 0.0)
            assertTrue(d.quantile(0.5) >= 10.0 AndAlso d.quantile(0.5) <= 20.0)
            assertEquals(0.0, d.cdf(9.0), 0.0)
            assertEquals(1.0, d.cdf(21.0), 0.0)
            assertTrue(d.cdf(15.0) > 0.0 AndAlso d.cdf(15.0) < 1.0)
            assertTrue(d.cdf(10.0) <= d.cdf(15.0) AndAlso d.cdf(15.0) <= d.cdf(20.0))
        End Sub

        <TestMethod>
        Public Sub Empty_Digest()
            Dim d = TDigest.createDigest(100)
            assertEquals(0L, d.size())
            assertEquals(0, d.centroidCount())
            assertEquals(0, d.centroids().Count)
            assertTrue(Double.IsNaN(d.quantile(0.5)))
            assertTrue(Double.IsNaN(d.cdf(1.0)))
            assertTrue(Double.IsPositiveInfinity(d.getMin()))
            assertTrue(Double.IsNegativeInfinity(d.getMax()))
            assertEquals("TDigest(compression=100, size=0, centroids=0)", d.ToString())
        End Sub

        <TestMethod>
        Public Sub Empty_Digest_JsonRoundTrip_OmitsInfiniteMinMax()
            Dim d = TDigest.createDigest(50)
            Dim json As String = JsonSerializer.Serialize(d)
            assertFalse(json.Contains("""min"""), json)
            assertFalse(json.Contains("""max"""), json)
            assertTrue(json.Contains("""compression"":50"), json)
            Dim back = JsonSerializer.Deserialize(Of TDigest)(json)
            assertEquals(0L, back.size())
            assertTrue(Double.IsNaN(back.quantile(0.5)))
            assertTrue(Double.IsPositiveInfinity(back.getMin()))
            assertTrue(Double.IsNegativeInfinity(back.getMax()))
            assertEquals(50.0, back.compression(), 0.0)
            back.add(3.0)
            assertEquals(3.0, back.quantile(0.5), 0.0)
            assertEquals(3.0, back.getMin(), 0.0)
        End Sub

        <TestMethod>
        Public Sub MergingTwoDigests_EqualsAddingAllValues()
            Dim all = UniformDigest()
            Dim low = TDigest.createDigest(100)
            Dim high = TDigest.createDigest(100)
            For Each v As Integer In ShuffledRange(42)
                If v < 5000 Then low.add(v) Else high.add(v)
            Next
            assertEquals(5000L, low.size())
            assertEquals(5000L, high.size())

            Dim merged = TDigest.createDigest(100)
            merged.add(low)
            merged.add(high)
            assertEquals(10000L, merged.size())
            assertEquals(0.0, merged.getMin(), 0.0)
            assertEquals(9999.0, merged.getMax(), 0.0)
            assertEquals(0.0, merged.quantile(0), 0.0)
            assertEquals(9999.0, merged.quantile(1), 0.0)
            For Each q As Double In New Double() {0.01, 0.1, 0.25, 0.5, 0.75, 0.9, 0.99}
                assertEquals(all.quantile(q), merged.quantile(q), 150.0, "q=" & q)
                assertEquals(q * 9999, merged.quantile(q), 200.0, "q=" & q)
            Next
            For Each x As Double In New Double() {500, 2500, 5000, 7500, 9500}
                assertEquals(all.cdf(x), merged.cdf(x), 0.02, "x=" & x)
            Next
            assertTrue(merged.centroidCount() < 1000)
            ' the sources are not modified
            assertEquals(5000L, low.size())
            assertEquals(5000L, high.size())

            ' add(IList) and merging an existing digest into itself-sized data
            Dim viaList = TDigest.createDigest(100)
            viaList.add(New List(Of TDigest) From {low, high})
            assertEquals(10000L, viaList.size())
            assertEquals(merged.quantile(0.5), viaList.quantile(0.5), 1.0E-9)
            Dim grown = TDigest.createDigest(100)
            grown.add(low)
            grown.add(high)
            assertEquals(merged.quantile(0.5), grown.quantile(0.5), 1.0E-9)
            assertThrows(Of ArgumentNullException)(Sub() merged.add(DirectCast(Nothing, TDigest)))
            assertThrows(Of ArgumentNullException)(Sub() merged.add(DirectCast(Nothing, IList(Of TDigest))))
        End Sub

        <TestMethod>
        Public Sub WeightedAdd()
            Dim d = TDigest.createDigest(100)
            d.add(1.0, 3)
            assertEquals(3L, d.size())
            d.add(10.0, 1)
            assertEquals(4L, d.size())
            d.add(5.0, 2.5)
            assertEquals(6L, d.size())
            assertEquals(1.0, d.getMin(), 0.0)
            assertEquals(10.0, d.getMax(), 0.0)
            assertEquals(1.0, d.quantile(0), 0.0)
            assertEquals(10.0, d.quantile(1), 0.0)
            assertEquals(0.0, d.cdf(0.0), 0.0)
            assertEquals(1.0, d.cdf(11.0), 0.0)
            Dim total As Double = 0
            For Each c As TDigest.Centroid In d.centroids()
                total += c.count()
            Next
            assertEquals(6.5, total, 1.0E-9)

            ' Weighted samples behave like repeated samples once there is enough data.
            Dim weighted = TDigest.createDigest(100)
            Dim repeated = TDigest.createDigest(100)
            For Each v As Integer In ShuffledRange(3)
                If v < 1000 Then
                    weighted.add(v, 3)
                    repeated.add(v)
                    repeated.add(v)
                    repeated.add(v)
                End If
            Next
            assertEquals(3000L, weighted.size())
            assertEquals(3000L, repeated.size())
            For Each q As Double In New Double() {0.1, 0.5, 0.9}
                assertEquals(q * 999, weighted.quantile(q), 30.0, "weighted q=" & q)
                assertEquals(repeated.quantile(q), weighted.quantile(q), 30.0, "q=" & q)
            Next
            assertEquals(0.5, weighted.cdf(500), 0.02)

            assertThrows(Of ArgumentException)(Sub() d.add(1.0, 0))
            assertThrows(Of ArgumentException)(Sub() d.add(1.0, -1.0))
            assertThrows(Of ArgumentException)(Sub() d.add(1.0, Double.NaN))
        End Sub

        <TestMethod>
        Public Sub NaN_And_BadQuantile_Throw()
            Dim d = UniformDigest()
            assertThrows(Of ArgumentException)(Sub() d.add(Double.NaN))
            assertThrows(Of ArgumentException)(Sub() d.add(Double.NaN, 2))
            assertThrows(Of ArgumentException)(Sub() d.quantile(1.5))
            assertThrows(Of ArgumentException)(Sub() d.quantile(-0.1))
            assertThrows(Of ArgumentException)(Sub() d.quantile(Double.NaN))
            assertThrows(Of ArgumentException)(Sub() TDigest.createDigest(0.5))
            assertThrows(Of ArgumentException)(Sub() TDigest.createDigest(Double.NaN))
            assertThrows(Of ArgumentException)(Sub()
                                                   Dim rejected As New TDigest(0)
                                               End Sub)
            assertEquals(10000L, d.size(), "a rejected sample must not change the digest")
        End Sub

        <TestMethod>
        Public Sub Factories_And_Infinity()
            assertEquals(100.0, New TDigest().compression(), 0.0)
            assertEquals(50.0, TDigest.createMergingDigest(50).compression(), 0.0)
            assertEquals(200.0, TDigest.createDigest(200).compression(), 0.0)
            Dim d = TDigest.createDigest(100)
            d.add(Double.NegativeInfinity)
            d.add(0.0)
            d.add(Double.PositiveInfinity)
            assertEquals(3L, d.size())
            assertTrue(Double.IsNegativeInfinity(d.getMin()))
            assertTrue(Double.IsPositiveInfinity(d.getMax()))
            ' a larger compression keeps more centroids
            Dim coarse = TDigest.createDigest(20)
            Dim fine = TDigest.createDigest(500)
            For Each v As Integer In ShuffledRange(1)
                coarse.add(v)
                fine.add(v)
            Next
            assertTrue(coarse.centroidCount() < fine.centroidCount(), coarse.centroidCount() & " vs " & fine.centroidCount())
            assertEquals(4999.5, fine.quantile(0.5), 20.0)
        End Sub

        <TestMethod>
        Public Sub Json_RoundTrip_KeepsSizeMinMaxAndMedian()
            Dim d = UniformDigest()
            Dim median As Double = d.quantile(0.5)
            Dim json As String = JsonSerializer.Serialize(d)
            assertTrue(json.Contains("""compression"":100"), json)
            assertTrue(json.Contains("""min"":0"), json)
            assertTrue(json.Contains("""max"":9999"), json)
            assertTrue(json.Contains("""means"":["), json)
            assertTrue(json.Contains("""weights"":["), json)

            Dim back = JsonSerializer.Deserialize(Of TDigest)(json)
            assertNotNull(back)
            assertEquals(10000L, back.size())
            assertEquals(0.0, back.getMin(), 0.0)
            assertEquals(9999.0, back.getMax(), 0.0)
            assertEquals(100.0, back.compression(), 0.0)
            assertEquals(median, back.quantile(0.5), 1.0E-9)
            assertEquals(d.centroidCount(), back.centroidCount())
            assertEquals(d.quantile(0.9), back.quantile(0.9), 1.0E-9)
            assertEquals(d.cdf(5000), back.cdf(5000), 1.0E-12)
            assertEquals(0.0, back.quantile(0), 0.0)
            assertEquals(9999.0, back.quantile(1), 0.0)
            assertEquals(json, JsonSerializer.Serialize(back))

            ' the deserialized digest keeps working
            back.add(20000.0)
            assertEquals(10001L, back.size())
            assertEquals(20000.0, back.getMax(), 0.0)
            assertEquals(20000.0, back.quantile(1), 0.0)

            ' a small digest with a weighted sample
            Dim small = TDigest.createDigest(50)
            small.add(1.0, 3)
            small.add(2.0)
            Dim small2 = JsonSerializer.Deserialize(Of TDigest)(JsonSerializer.Serialize(small))
            assertEquals(4L, small2.size())
            assertEquals(50.0, small2.compression(), 0.0)
            assertEquals(small.quantile(0.5), small2.quantile(0.5), 1.0E-9)
        End Sub

        <TestMethod>
        Public Sub BinarySerialization_RoundTrip()
            Dim d = UniformDigest()
            Dim median As Double = d.quantile(0.5)
            Dim back = SerializationUtils.fromByteArray(Of TDigest)(SerializationUtils.toByteArray(d))
            assertEquals(10000L, back.size())
            assertEquals(0.0, back.getMin(), 0.0)
            assertEquals(9999.0, back.getMax(), 0.0)
            assertEquals(median, back.quantile(0.5), 1.0E-9)
            assertEquals(d.centroidCount(), back.centroidCount())
            assertEquals(d.ToString(), back.ToString())

            ' Unmerged buffered samples survive a SerializationUtils round trip too.
            Dim buffered = TDigest.createDigest(100)
            For i As Integer = 1 To 10
                buffered.add(i)
            Next
            Dim bf As New ObjectFormatter()
            Dim bytes As Byte()
            Using ms As New MemoryStream()
                bf.Serialize(ms, buffered)
                bytes = ms.ToArray()
            End Using
            Dim buffered2 As TDigest
            Using ms As New MemoryStream(bytes)
                buffered2 = DirectCast(bf.Deserialize(ms), TDigest)
            End Using
            assertEquals(10L, buffered2.size())
            assertEquals(1.0, buffered2.getMin(), 0.0)
            assertEquals(10.0, buffered2.getMax(), 0.0)
            assertEquals(buffered.quantile(0.5), buffered2.quantile(0.5), 1.0E-9)
            assertEquals(10, buffered2.centroidCount())
        End Sub

        <TestMethod>
        Public Sub ToString_DescribesTheDigest()
            Dim d = UniformDigest()
            Dim s As String = d.ToString()
            assertFalse(String.IsNullOrEmpty(s))
            assertTrue(s.StartsWith("TDigest("), s)
            assertTrue(s.Contains("compression=100"), s)
            assertTrue(s.Contains("size=10000"), s)
            assertTrue(s.Contains("centroids=" & d.centroidCount()), s)
            assertTrue(s.Contains("min=0"), s)
            assertTrue(s.Contains("max=9999"), s)
        End Sub

    End Class

End Namespace
