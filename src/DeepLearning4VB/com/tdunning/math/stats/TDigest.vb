Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.Json.Serialization

' DeepLearning4VB shim for the t-digest API that DataVec's column analysis uses
' (com.tdunning.math.stats.TDigest). The t-digest algorithm is by Ted Dunning (Apache-2.0,
' https://github.com/tdunning/t-digest); this is an independent managed implementation of the
' merging variant, not a translation of that library.

Namespace com.tdunning.math.stats

    ''' <summary>
    ''' A streaming estimator of quantiles and cumulative distribution (Dunning's merging t-digest).
    ''' Values are buffered and merged into at most about <c>compression</c> weighted centroids, so the
    ''' extreme quantiles are the most accurate. Serializes with System.Text.Json as the compression,
    ''' min, max and the centroid means/weights; it is also <see cref="SerializableAttribute"/>.
    ''' </summary>
    <Serializable>
    Public Class TDigest

        Private compression_ As Double
        Private means As List(Of Double)
        Private weights As List(Of Double)
        Private bufferMeans As List(Of Double)
        Private bufferWeights As List(Of Double)
        Private totalWeight As Double
        Private min_ As Double = Double.PositiveInfinity
        Private max_ As Double = Double.PositiveInfinity * -1
        Private bufferLimit As Integer

        ''' <summary>A digest with compression 100.</summary>
        Public Sub New()
            Me.New(100)
        End Sub

        ''' <summary>A digest with the given compression (more = more centroids = more accurate; 100 is typical).</summary>
        Public Sub New(ByVal compression As Double)
            If Double.IsNaN(compression) OrElse compression < 1 Then Throw New ArgumentException("compression must be >= 1, got " & compression)
            compression_ = compression
            means = New List(Of Double)()
            weights = New List(Of Double)()
            bufferMeans = New List(Of Double)()
            bufferWeights = New List(Of Double)()
            bufferLimit = Global.System.Math.Max(50, CInt(Global.System.Math.Ceiling(5 * compression)))
        End Sub

        ''' <summary>Factory matching the Java API: <c>TDigest.createDigest(100)</c>.</summary>
        Public Shared Function createDigest(ByVal compression As Double) As TDigest
            Return New TDigest(compression)
        End Function

        Public Shared Function createMergingDigest(ByVal compression As Double) As TDigest
            Return New TDigest(compression)
        End Function

        ''' <summary>Adds a sample with weight 1.</summary>
        Public Overridable Sub add(ByVal x As Double)
            add(x, 1.0)
        End Sub

        Public Overridable Sub add(ByVal x As Double, ByVal w As Integer)
            add(x, CDbl(w))
        End Sub

        ''' <summary>Adds a weighted sample.</summary>
        Public Overridable Sub add(ByVal x As Double, ByVal w As Double)
            If Double.IsNaN(x) Then Throw New ArgumentException("Cannot add NaN to t-digest")
            If Double.IsNaN(w) OrElse w <= 0 Then Throw New ArgumentException("Weight must be positive, got " & w)
            bufferMeans.Add(x)
            bufferWeights.Add(w)
            totalWeight += w
            If x < min_ Then min_ = x
            If x > max_ Then max_ = x
            If bufferMeans.Count >= bufferLimit Then MergeBuffer()
        End Sub

        ''' <summary>Merges another digest into this one.</summary>
        Public Overridable Sub add(ByVal other As TDigest)
            If other Is Nothing Then Throw New ArgumentNullException("other")
            other.MergeBuffer()
            For i As Integer = 0 To other.means.Count - 1
                add(other.means(i), other.weights(i))
            Next
            ' The centroid means do not reach the true extremes; keep the other digest's min and max
            ' so quantile(0)/quantile(1) and cdf stay correct after a merge.
            If other.min_ < min_ Then min_ = other.min_
            If other.max_ > max_ Then max_ = other.max_
        End Sub

        Public Overridable Sub add(ByVal others As IList(Of TDigest))
            If others Is Nothing Then Throw New ArgumentNullException("others")
            For Each o As TDigest In others
                add(o)
            Next
        End Sub

        ''' <summary>Merges the buffered samples into the centroids.</summary>
        Public Overridable Sub compress()
            MergeBuffer()
        End Sub

        ''' <summary>Total weight of all samples added (the sample count for unit weights).</summary>
        Public Overridable Function size() As Long
            Return CLng(totalWeight)
        End Function

        Public Overridable Function compression() As Double
            Return compression_
        End Function

        Public Overridable Function getMin() As Double
            Return min_
        End Function

        Public Overridable Function getMax() As Double
            Return max_
        End Function

        ''' <summary>Number of centroids after merging the buffer.</summary>
        Public Overridable Function centroidCount() As Integer
            MergeBuffer()
            Return means.Count
        End Function

        ''' <summary>The centroids (mean and weight), sorted by mean.</summary>
        Public Overridable Function centroids() As IList(Of Centroid)
            MergeBuffer()
            Dim ret As New List(Of Centroid)(means.Count)
            For i As Integer = 0 To means.Count - 1
                ret.Add(New Centroid(means(i), weights(i)))
            Next
            Return ret
        End Function

        ''' <summary>Estimated fraction of the samples that are &lt;= x.</summary>
        Public Overridable Function cdf(ByVal x As Double) As Double
            MergeBuffer()
            Dim n As Integer = means.Count
            If n = 0 Then Return Double.NaN
            If n = 1 Then
                If x < min_ Then Return 0
                If x > max_ Then Return 1
                If max_ - min_ < Double.Epsilon Then Return 0.5
                Return (x - min_) / (max_ - min_)
            End If
            If x < min_ Then Return 0
            If x > max_ Then Return 1
            If x < means(0) Then
                If means(0) - min_ > 0 Then Return (x - min_) / (means(0) - min_) * weights(0) / totalWeight / 2
                Return 0
            End If
            If x > means(n - 1) Then
                If max_ - means(n - 1) > 0 Then Return 1 - (max_ - x) / (max_ - means(n - 1)) * weights(n - 1) / totalWeight / 2
                Return 1
            End If
            Dim weightSoFar As Double = 0
            For i As Integer = 0 To n - 2
                If means(i) = x Then
                    Dim dw As Double = 0
                    Dim j As Integer = i
                    While j < n AndAlso means(j) = x
                        dw += weights(j)
                        j += 1
                    End While
                    Return (weightSoFar + dw / 2) / totalWeight
                End If
                If means(i) <= x AndAlso x < means(i + 1) Then
                    If means(i + 1) - means(i) > 0 Then
                        Dim leftExcluded As Double = If(weights(i) = 1, 0.5, 0)
                        Dim rightExcluded As Double = If(weights(i + 1) = 1, 0.5, 0)
                        Dim dw As Double = (weights(i) + weights(i + 1)) / 2
                        Dim base As Double = weightSoFar + weights(i) / 2 + leftExcluded
                        Dim dwNoSingleton As Double = dw - leftExcluded - rightExcluded
                        Return (base + dwNoSingleton * (x - means(i)) / (means(i + 1) - means(i))) / totalWeight
                    Else
                        Dim dw As Double = (weights(i) + weights(i + 1)) / 2
                        Return (weightSoFar + dw) / totalWeight
                    End If
                End If
                weightSoFar += weights(i)
            Next
            Return 1 - 0.5 * weights(n - 1) / totalWeight
        End Function

        ''' <summary>Estimated value at the quantile q (0..1): <c>quantile(0.5)</c> is the median.</summary>
        Public Overridable Function quantile(ByVal q As Double) As Double
            If Double.IsNaN(q) OrElse q < 0 OrElse q > 1 Then Throw New ArgumentException("q should be in [0,1], got " & q)
            MergeBuffer()
            Dim n As Integer = means.Count
            If n = 0 Then Return Double.NaN
            If n = 1 Then Return means(0)
            Dim index As Double = q * totalWeight
            If index < 1 Then Return min_
            If weights(0) > 1 AndAlso index < weights(0) / 2 Then
                Return min_ + (index - 1) / (weights(0) / 2 - 1) * (means(0) - min_)
            End If
            If index > totalWeight - 1 Then Return max_
            If weights(n - 1) > 1 AndAlso totalWeight - index <= weights(n - 1) / 2 Then
                Return max_ - (totalWeight - index - 1) / (weights(n - 1) / 2 - 1) * (max_ - means(n - 1))
            End If
            Dim weightSoFar As Double = weights(0) / 2
            For i As Integer = 0 To n - 2
                Dim dw As Double = (weights(i) + weights(i + 1)) / 2
                If weightSoFar + dw > index Then
                    Dim leftUnit As Double = 0
                    If weights(i) = 1 Then
                        If index - weightSoFar < 0.5 Then Return means(i)
                        leftUnit = 0.5
                    End If
                    Dim rightUnit As Double = 0
                    If weights(i + 1) = 1 Then
                        If weightSoFar + dw - index <= 0.5 Then Return means(i + 1)
                        rightUnit = 0.5
                    End If
                    Dim z1 As Double = index - weightSoFar - leftUnit
                    Dim z2 As Double = weightSoFar + dw - index - rightUnit
                    Return WeightedAverage(means(i), z2, means(i + 1), z1)
                End If
                weightSoFar += dw
            Next
            Dim zz1 As Double = index - totalWeight - weights(n - 1) / 2
            Dim zz2 As Double = weights(n - 1) / 2 - zz1
            Return WeightedAverage(means(n - 1), zz1, max_, zz2)
        End Function

        Private Shared Function WeightedAverage(ByVal x1 As Double, ByVal w1 As Double, ByVal x2 As Double, ByVal w2 As Double) As Double
            If x1 <= x2 Then
                Dim x As Double = (x1 * w1 + x2 * w2) / (w1 + w2)
                Return Global.System.Math.Max(x1, Global.System.Math.Min(x, x2))
            End If
            Return WeightedAverage(x2, w2, x1, w1)
        End Function

        Private Function IntegratedLocation(ByVal q As Double) As Double
            Return compression_ * (Global.System.Math.Asin(2 * q - 1) + Global.System.Math.PI / 2) / (2 * Global.System.Math.PI)
        End Function

        Private Sub MergeBuffer()
            If bufferMeans.Count = 0 Then Return
            Dim n As Integer = means.Count + bufferMeans.Count
            Dim m(n - 1) As Double
            Dim w(n - 1) As Double
            means.CopyTo(m, 0)
            weights.CopyTo(w, 0)
            bufferMeans.CopyTo(m, means.Count)
            bufferWeights.CopyTo(w, means.Count)
            bufferMeans.Clear()
            bufferWeights.Clear()
            Array.Sort(m, w)

            Dim total As Double = 0
            For i As Integer = 0 To n - 1
                total += w(i)
            Next
            Dim newMeans As New List(Of Double)()
            Dim newWeights As New List(Of Double)()
            Dim wSoFar As Double = 0
            Dim curMean As Double = m(0)
            Dim curWeight As Double = w(0)
            Dim k1 As Double = IntegratedLocation(0)
            For i As Integer = 1 To n - 1
                Dim q2 As Double = (wSoFar + curWeight + w(i)) / total
                If IntegratedLocation(q2) - k1 <= 1 Then
                    curMean += (m(i) - curMean) * w(i) / (curWeight + w(i))
                    curWeight += w(i)
                Else
                    newMeans.Add(curMean)
                    newWeights.Add(curWeight)
                    wSoFar += curWeight
                    k1 = IntegratedLocation(wSoFar / total)
                    curMean = m(i)
                    curWeight = w(i)
                End If
            Next
            newMeans.Add(curMean)
            newWeights.Add(curWeight)
            means = newMeans
            weights = newWeights
            totalWeight = total
        End Sub

        ''' <summary>The compression (JSON property <c>compression</c>).</summary>
        <JsonPropertyName("compression")>
        Public Property CompressionFactor As Double
            Get
                Return compression_
            End Get
            Set(ByVal value As Double)
                If Double.IsNaN(value) OrElse value < 1 Then Throw New ArgumentException("compression must be >= 1, got " & value)
                compression_ = value
                bufferLimit = Global.System.Math.Max(50, CInt(Global.System.Math.Ceiling(5 * value)))
            End Set
        End Property

        ''' <summary>Smallest sample added (+Infinity while the digest is empty).</summary>
        <JsonIgnore>
        Public Property Min As Double
            Get
                Return min_
            End Get
            Set(ByVal value As Double)
                min_ = value
            End Set
        End Property

        ''' <summary>Largest sample added (-Infinity while the digest is empty).</summary>
        <JsonIgnore>
        Public Property Max As Double
            Get
                Return max_
            End Get
            Set(ByVal value As Double)
                max_ = value
            End Set
        End Property

        Private Function IsEmpty() As Boolean
            Return means.Count = 0 AndAlso bufferMeans.Count = 0
        End Function

        ''' <summary>JSON property <c>min</c>: the smallest sample, omitted while the digest is empty (JSON has no infinities).</summary>
        <JsonPropertyName("min"), JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property MinValue As Double?
            Get
                If IsEmpty() Then Return Nothing
                Return min_
            End Get
            Set(ByVal value As Double?)
                min_ = If(value.HasValue, value.Value, Double.PositiveInfinity)
            End Set
        End Property

        ''' <summary>JSON property <c>max</c>: the largest sample, omitted while the digest is empty.</summary>
        <JsonPropertyName("max"), JsonIgnore(Condition:=JsonIgnoreCondition.WhenWritingNull)>
        Public Property MaxValue As Double?
            Get
                If IsEmpty() Then Return Nothing
                Return max_
            End Get
            Set(ByVal value As Double?)
                max_ = If(value.HasValue, value.Value, Double.NegativeInfinity)
            End Set
        End Property

        ''' <summary>Centroid means after merging the buffer (JSON property <c>means</c>).</summary>
        <JsonPropertyName("means")>
        Public Property CentroidMeans As Double()
            Get
                MergeBuffer()
                Return means.ToArray()
            End Get
            Set(ByVal value As Double())
                bufferMeans.Clear()
                bufferWeights.Clear()
                means = New List(Of Double)(If(value, New Double() {}))
                RecomputeTotal()
            End Set
        End Property

        ''' <summary>Centroid weights after merging the buffer (JSON property <c>weights</c>).</summary>
        <JsonPropertyName("weights")>
        Public Property CentroidWeights As Double()
            Get
                MergeBuffer()
                Return weights.ToArray()
            End Get
            Set(ByVal value As Double())
                bufferMeans.Clear()
                bufferWeights.Clear()
                weights = New List(Of Double)(If(value, New Double() {}))
                RecomputeTotal()
            End Set
        End Property

        Private Sub RecomputeTotal()
            totalWeight = 0
            If means.Count <> weights.Count Then Return
            For Each w As Double In weights
                totalWeight += w
            Next
        End Sub

        Public Overrides Function ToString() As String
            MergeBuffer()
            Dim sb As New StringBuilder()
            sb.Append("TDigest(compression=").Append(compression_).Append(", size=").Append(size()).Append(", centroids=").Append(means.Count)
            If means.Count > 0 Then sb.Append(", min=").Append(min_).Append(", max=").Append(max_)
            sb.Append(")")
            Return sb.ToString()
        End Function

        ''' <summary>A merged cluster of samples: its mean and total weight.</summary>
        <Serializable>
        Public NotInheritable Class Centroid
            Private ReadOnly mean_ As Double
            Private ReadOnly count_ As Double

            Public Sub New(ByVal mean As Double, ByVal count As Double)
                mean_ = mean
                count_ = count
            End Sub

            Public Function mean() As Double
                Return mean_
            End Function

            Public Function count() As Double
                Return count_
            End Function

            Public Overrides Function ToString() As String
                Return "Centroid(" & mean_ & ", " & count_ & ")"
            End Function
        End Class

    End Class

End Namespace
