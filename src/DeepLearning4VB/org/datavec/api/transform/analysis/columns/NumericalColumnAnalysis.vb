'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Imports System
Imports System.Text
Imports System.Text.Json.Serialization
Imports TDigest = DeepLearning4VB.com.tdunning.math.stats.TDigest
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.analysis.columns

    ''' <summary>
    ''' Base of the analyses of numerical columns (integer, long, double, time): mean, sample standard deviation and
    ''' variance, counts of zero/negative/positive/minimum/maximum values, an optional histogram and an optional
    ''' t-digest for the quantiles. Equality ignores the digest (Lombok <c>@EqualsAndHashCode(exclude = "digest")</c>).
    ''' <para>The builders derive from <see cref="Builder(Of T)"/>; its non-generic base <see cref="BuilderBase"/>
    ''' stands in for Java's raw <c>NumericalColumnAnalysis.Builder</c> type taken by the constructor.</para>
    ''' </summary>
    ' Java: @Data @EqualsAndHashCode(exclude = {"digest"})
    ' The Jackson @JsonSerialize/@JsonDeserialize(using = TDigestSerializer/TDigestDeserializer) of the digest are
    ' dropped: the TDigest stand-in serializes itself with System.Text.Json.
    <Serializable>
    Public MustInherit Class NumericalColumnAnalysis
        Implements ColumnAnalysis

        Protected Friend mean As Double
        Protected Friend sampleStdev As Double
        Protected Friend sampleVariance As Double
        Protected Friend countZero As Long
        Protected Friend countNegative As Long
        Protected Friend countPositive As Long
        Protected Friend countMinValue As Long
        Protected Friend countMaxValue As Long
        ''' <summary>Java field <c>countTotal</c> (renamed: VB names are case-insensitive and <see cref="CountTotal"/> exists).</summary>
        <JsonPropertyName("countTotal")>
        Protected Friend countTotalValue As Long
        Protected Friend histogramBuckets() As Double
        Protected Friend histogramBucketCounts() As Long
        Protected Friend digest As TDigest

        Protected Friend Sub New(ByVal builder As BuilderBase)
            Me.mean = builder.meanValue
            Me.sampleStdev = builder.sampleStdevValue
            Me.sampleVariance = builder.sampleVarianceValue
            Me.countZero = builder.countZeroValue
            Me.countNegative = builder.countNegativeValue
            Me.countPositive = builder.countPositiveValue
            Me.countMinValue = builder.countMinValueValue
            Me.countMaxValue = builder.countMaxValueValue
            Me.countTotalValue = builder.countTotalValue
            Me.histogramBuckets = builder.histogramBucketsValue
            Me.histogramBucketCounts = builder.histogramBucketCountsValue
            Me.digest = builder.digestValue
        End Sub

        Protected Friend Sub New()
            'No arg for Jackson
        End Sub

        Public MustOverride ReadOnly Property ColumnType As ColumnTypeEnum Implements ColumnAnalysis.ColumnType

        Public Overridable ReadOnly Property CountTotal As Long Implements ColumnAnalysis.CountTotal
            Get
                Return countTotalValue
            End Get
        End Property

        Public MustOverride ReadOnly Property MinDouble As Double

        Public MustOverride ReadOnly Property MaxDouble As Double

        Public Overrides Function ToString() As String
            Dim q As String = ""
            If digest IsNot Nothing Then
                Dim quantiles As New StringBuilder(", quantiles=[")
                Dim printReports() As Double = {0.001, 0.01, 0.1, 0.5, 0.9, 0.99, 0.999}
                For i As Integer = 0 To printReports.Length - 1
                    quantiles.Append(Arrays.JavaDoubleToString(printReports(i))).Append(" -> ").Append(Arrays.JavaDoubleToString(digest.quantile(printReports(i))))
                    If i < printReports.Length - 1 Then
                        quantiles.Append(",")
                    End If
                Next i
                quantiles.Append("]")
                q = quantiles.ToString()
            End If
            Return "mean=" & Arrays.JavaDoubleToString(mean) & ",sampleStDev=" & Arrays.JavaDoubleToString(sampleStdev) & ",sampleVariance=" & Arrays.JavaDoubleToString(sampleVariance) &
                   ",countZero=" & countZero & ",countNegative=" & countNegative & ",countPositive=" & countPositive & ",countMinValue=" & countMinValue &
                   ",countMaxValue=" & countMaxValue & ",count=" & countTotalValue & q
        End Function

        ' ---- Lombok @Data accessors ----

        Public Function getMean() As Double
            Return mean
        End Function

        Public Sub setMean(ByVal mean As Double)
            Me.mean = mean
        End Sub

        Public Function getSampleStdev() As Double
            Return sampleStdev
        End Function

        Public Sub setSampleStdev(ByVal sampleStdev As Double)
            Me.sampleStdev = sampleStdev
        End Sub

        Public Function getSampleVariance() As Double
            Return sampleVariance
        End Function

        Public Sub setSampleVariance(ByVal sampleVariance As Double)
            Me.sampleVariance = sampleVariance
        End Sub

        Public Function getCountZero() As Long
            Return countZero
        End Function

        Public Sub setCountZero(ByVal countZero As Long)
            Me.countZero = countZero
        End Sub

        Public Function getCountNegative() As Long
            Return countNegative
        End Function

        Public Sub setCountNegative(ByVal countNegative As Long)
            Me.countNegative = countNegative
        End Sub

        Public Function getCountPositive() As Long
            Return countPositive
        End Function

        Public Sub setCountPositive(ByVal countPositive As Long)
            Me.countPositive = countPositive
        End Sub

        Public Function getCountMinValue() As Long
            Return countMinValue
        End Function

        Public Sub setCountMinValue(ByVal countMinValue As Long)
            Me.countMinValue = countMinValue
        End Sub

        Public Function getCountMaxValue() As Long
            Return countMaxValue
        End Function

        Public Sub setCountMaxValue(ByVal countMaxValue As Long)
            Me.countMaxValue = countMaxValue
        End Sub

        Public Function getCountTotal() As Long Implements ColumnAnalysis.getCountTotal
            Return countTotalValue
        End Function

        Public Function getColumnType() As ColumnTypeEnum Implements ColumnAnalysis.getColumnType
            Return ColumnType
        End Function

        Public Sub setCountTotal(ByVal countTotal As Long)
            Me.countTotalValue = countTotal
        End Sub

        Public Function getHistogramBuckets() As Double()
            Return histogramBuckets
        End Function

        Public Sub setHistogramBuckets(ByVal histogramBuckets() As Double)
            Me.histogramBuckets = histogramBuckets
        End Sub

        Public Function getHistogramBucketCounts() As Long()
            Return histogramBucketCounts
        End Function

        Public Sub setHistogramBucketCounts(ByVal histogramBucketCounts() As Long)
            Me.histogramBucketCounts = histogramBucketCounts
        End Sub

        Public Function getDigest() As TDigest
            Return digest
        End Function

        Public Sub setDigest(ByVal digest As TDigest)
            Me.digest = digest
        End Sub

        ''' <summary>Equal when the other object is of the same class and all fields except the digest are equal.</summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot CObj(Me).GetType() Then Return False
            Dim other As NumericalColumnAnalysis = DirectCast(obj, NumericalColumnAnalysis)
            Return mean.Equals(other.mean) AndAlso sampleStdev.Equals(other.sampleStdev) AndAlso sampleVariance.Equals(other.sampleVariance) AndAlso
                   countZero = other.countZero AndAlso countNegative = other.countNegative AndAlso countPositive = other.countPositive AndAlso
                   countMinValue = other.countMinValue AndAlso countMaxValue = other.countMaxValue AndAlso countTotalValue = other.countTotalValue AndAlso
                   Arrays.equals(histogramBuckets, other.histogramBuckets) AndAlso Arrays.equals(histogramBucketCounts, other.histogramBucketCounts)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(mean, sampleStdev, sampleVariance, countZero, countNegative, countPositive, countMinValue, countMaxValue, countTotalValue,
                                Arrays.hashCode(histogramBuckets), Arrays.hashCode(histogramBucketCounts))
        End Function

        ''' <summary>Non-generic base of <see cref="Builder(Of T)"/> holding the values (Java's raw <c>Builder</c> type).</summary>
        <Serializable>
        Public MustInherit Class BuilderBase
            Friend meanValue As Double
            Friend sampleStdevValue As Double
            Friend sampleVarianceValue As Double
            Friend countZeroValue As Long
            Friend countNegativeValue As Long
            Friend countPositiveValue As Long
            Friend countMinValueValue As Long
            Friend countMaxValueValue As Long
            Friend countTotalValue As Long
            Friend histogramBucketsValue() As Double
            Friend histogramBucketCountsValue() As Long
            Friend digestValue As TDigest
        End Class

        ''' <summary>Fluent builder of the common numerical fields; <typeparamref name="T"/> is the concrete builder.</summary>
        <Serializable>
        Public MustInherit Class Builder(Of T As Builder(Of T))
            Inherits BuilderBase

            Private Function self() As T
                Return DirectCast(CObj(Me), T)
            End Function

            Public Overridable Function mean(ByVal value As Double) As T
                Me.meanValue = value
                Return self()
            End Function

            Public Overridable Function sampleStdev(ByVal value As Double) As T
                Me.sampleStdevValue = value
                Return self()
            End Function

            Public Overridable Function sampleVariance(ByVal value As Double) As T
                Me.sampleVarianceValue = value
                Return self()
            End Function

            Public Overridable Function countZero(ByVal value As Long) As T
                Me.countZeroValue = value
                Return self()
            End Function

            Public Overridable Function countNegative(ByVal value As Long) As T
                Me.countNegativeValue = value
                Return self()
            End Function

            Public Overridable Function countPositive(ByVal value As Long) As T
                Me.countPositiveValue = value
                Return self()
            End Function

            Public Overridable Function countMinValue(ByVal value As Long) As T
                Me.countMinValueValue = value
                Return self()
            End Function

            Public Overridable Function countMaxValue(ByVal value As Long) As T
                Me.countMaxValueValue = value
                Return self()
            End Function

            Public Overridable Function countTotal(ByVal value As Long) As T
                Me.countTotalValue = value
                Return self()
            End Function

            Public Overridable Function histogramBuckets(ByVal value() As Double) As T
                Me.histogramBucketsValue = value
                Return self()
            End Function

            Public Overridable Function histogramBucketCounts(ByVal value() As Long) As T
                Me.histogramBucketCountsValue = value
                Return self()
            End Function

            Public Overridable Function digest(ByVal value As TDigest) As T
                Me.digestValue = value
                Return self()
            End Function

        End Class

    End Class

End Namespace
