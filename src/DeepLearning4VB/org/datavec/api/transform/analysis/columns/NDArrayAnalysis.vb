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
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Text.Json.Serialization
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.analysis.columns

    ''' <summary>
    ''' Analysis of an NDArray column: counts, array lengths, counts by rank and the value range. This class does not
    ''' need the tensor API; the counter that fills it in (<c>NDArrayAnalysisCounter</c>) is not ported yet.
    ''' Lombok's builder method <c>Builder()</c> is <see cref="newBuilder"/> (VB names are case-insensitive, so it
    ''' cannot share the name of the nested <see cref="Builder"/> class).
    ''' </summary>
    ' Java: @AllArgsConstructor @NoArgsConstructor @Builder(builderClassName = "Builder", builderMethodName = "Builder") @Data
    <Serializable>
    Public Class NDArrayAnalysis
        Implements ColumnAnalysis

        ''' <summary>Java field <c>countTotal</c> (renamed: VB names are case-insensitive and <see cref="CountTotal"/> exists).</summary>
        <JsonPropertyName("countTotal")>
        Private countTotalValue As Long
        Private countNull As Long
        Private minLength As Long
        Private maxLength As Long
        Private totalNDArrayValues As Long
        Private countsByRank As IDictionary(Of Integer, Long)
        Private minValue As Double
        Private maxValue As Double
        Protected Friend histogramBuckets() As Double
        Protected Friend histogramBucketCounts() As Long

        Public Sub New()
        End Sub

        Public Sub New(ByVal countTotal As Long, ByVal countNull As Long, ByVal minLength As Long, ByVal maxLength As Long, ByVal totalNDArrayValues As Long, ByVal countsByRank As IDictionary(Of Integer, Long), ByVal minValue As Double, ByVal maxValue As Double, ByVal histogramBuckets() As Double, ByVal histogramBucketCounts() As Long)
            Me.countTotalValue = countTotal
            Me.countNull = countNull
            Me.minLength = minLength
            Me.maxLength = maxLength
            Me.totalNDArrayValues = totalNDArrayValues
            Me.countsByRank = countsByRank
            Me.minValue = minValue
            Me.maxValue = maxValue
            Me.histogramBuckets = histogramBuckets
            Me.histogramBucketCounts = histogramBucketCounts
        End Sub

        ''' <summary>Creates a builder (Lombok's static <c>Builder()</c> method).</summary>
        Public Shared Function newBuilder() As Builder
            Return New Builder()
        End Function

        Public Overridable ReadOnly Property ColumnType As ColumnTypeEnum Implements ColumnAnalysis.ColumnType
            Get
                Return ColumnTypeEnum.NDArray
            End Get
        End Property

        Public Overridable ReadOnly Property CountTotal As Long Implements ColumnAnalysis.CountTotal
            Get
                Return countTotalValue
            End Get
        End Property

        Public Function getCountTotal() As Long Implements ColumnAnalysis.getCountTotal
            Return countTotalValue
        End Function

        Public Function getColumnType() As ColumnTypeEnum Implements ColumnAnalysis.getColumnType
            Return ColumnType
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder("{")
            If countsByRank IsNot Nothing Then
                Dim first As Boolean = True
                For Each rank As Integer In countsByRank.Keys.OrderBy(Function(k) k)
                    If Not first Then sb.Append(", ")
                    first = False
                    sb.Append(rank).Append("="c).Append(countsByRank(rank))
                Next
            End If
            sb.Append("}"c)

            Return "NDArrayAnalysis(countTotal=" & countTotalValue & ",countNull=" & countNull & ",minLength=" & minLength & ",maxLength=" & maxLength &
                   ",totalValuesAllNDArrays=" & totalNDArrayValues & ",minValue=" & Arrays.JavaDoubleToString(minValue) & ",maxValue=" & Arrays.JavaDoubleToString(maxValue) &
                   ",countsByNDArrayRank=" & sb.ToString() & ")"
        End Function

        ' ---- Lombok @Data accessors ----

        Public Sub setCountTotal(ByVal countTotal As Long)
            Me.countTotalValue = countTotal
        End Sub

        Public Function getCountNull() As Long
            Return countNull
        End Function

        Public Sub setCountNull(ByVal countNull As Long)
            Me.countNull = countNull
        End Sub

        Public Function getMinLength() As Long
            Return minLength
        End Function

        Public Sub setMinLength(ByVal minLength As Long)
            Me.minLength = minLength
        End Sub

        Public Function getMaxLength() As Long
            Return maxLength
        End Function

        Public Sub setMaxLength(ByVal maxLength As Long)
            Me.maxLength = maxLength
        End Sub

        Public Function getTotalNDArrayValues() As Long
            Return totalNDArrayValues
        End Function

        Public Sub setTotalNDArrayValues(ByVal totalNDArrayValues As Long)
            Me.totalNDArrayValues = totalNDArrayValues
        End Sub

        Public Function getCountsByRank() As IDictionary(Of Integer, Long)
            Return countsByRank
        End Function

        Public Sub setCountsByRank(ByVal countsByRank As IDictionary(Of Integer, Long))
            Me.countsByRank = countsByRank
        End Sub

        Public Function getMinValue() As Double
            Return minValue
        End Function

        Public Sub setMinValue(ByVal minValue As Double)
            Me.minValue = minValue
        End Sub

        Public Function getMaxValue() As Double
            Return maxValue
        End Function

        Public Sub setMaxValue(ByVal maxValue As Double)
            Me.maxValue = maxValue
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

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As NDArrayAnalysis = TryCast(obj, NDArrayAnalysis)
            If other Is Nothing Then Return False
            If Not (countTotalValue = other.countTotalValue AndAlso countNull = other.countNull AndAlso minLength = other.minLength AndAlso
                    maxLength = other.maxLength AndAlso totalNDArrayValues = other.totalNDArrayValues AndAlso minValue.Equals(other.minValue) AndAlso
                    maxValue.Equals(other.maxValue) AndAlso Arrays.equals(histogramBuckets, other.histogramBuckets) AndAlso
                    Arrays.equals(histogramBucketCounts, other.histogramBucketCounts)) Then
                Return False
            End If
            If countsByRank Is Nothing OrElse other.countsByRank Is Nothing Then Return countsByRank Is other.countsByRank
            If countsByRank.Count <> other.countsByRank.Count Then Return False
            For Each kv As KeyValuePair(Of Integer, Long) In countsByRank
                Dim v As Long
                If Not other.countsByRank.TryGetValue(kv.Key, v) OrElse v <> kv.Value Then Return False
            Next
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim mapHash As Integer = 0
            If countsByRank IsNot Nothing Then
                For Each kv As KeyValuePair(Of Integer, Long) In countsByRank
                    mapHash += kv.Key.GetHashCode() Xor kv.Value.GetHashCode()
                Next
            End If
            Return Objects.hash(countTotalValue, countNull, minLength, maxLength, totalNDArrayValues, mapHash, minValue, maxValue,
                                Arrays.hashCode(histogramBuckets), Arrays.hashCode(histogramBucketCounts))
        End Function

        ''' <summary>Lombok's builder of <see cref="NDArrayAnalysis"/>.</summary>
        Public Class Builder
            Private countTotalValue As Long
            Private countNullValue As Long
            Private minLengthValue As Long
            Private maxLengthValue As Long
            Private totalNDArrayValuesValue As Long
            Private countsByRankValue As IDictionary(Of Integer, Long)
            Private minValueValue As Double
            Private maxValueValue As Double
            Private histogramBucketsValue() As Double
            Private histogramBucketCountsValue() As Long

            Public Function countTotal(ByVal value As Long) As Builder
                countTotalValue = value
                Return Me
            End Function

            Public Function countNull(ByVal value As Long) As Builder
                countNullValue = value
                Return Me
            End Function

            Public Function minLength(ByVal value As Long) As Builder
                minLengthValue = value
                Return Me
            End Function

            Public Function maxLength(ByVal value As Long) As Builder
                maxLengthValue = value
                Return Me
            End Function

            Public Function totalNDArrayValues(ByVal value As Long) As Builder
                totalNDArrayValuesValue = value
                Return Me
            End Function

            Public Function countsByRank(ByVal value As IDictionary(Of Integer, Long)) As Builder
                countsByRankValue = value
                Return Me
            End Function

            Public Function minValue(ByVal value As Double) As Builder
                minValueValue = value
                Return Me
            End Function

            Public Function maxValue(ByVal value As Double) As Builder
                maxValueValue = value
                Return Me
            End Function

            Public Function histogramBuckets(ByVal value() As Double) As Builder
                histogramBucketsValue = value
                Return Me
            End Function

            Public Function histogramBucketCounts(ByVal value() As Long) As Builder
                histogramBucketCountsValue = value
                Return Me
            End Function

            Public Function build() As NDArrayAnalysis
                Return New NDArrayAnalysis(countTotalValue, countNullValue, minLengthValue, maxLengthValue, totalNDArrayValuesValue, countsByRankValue,
                                           minValueValue, maxValueValue, histogramBucketsValue, histogramBucketCountsValue)
            End Function

            Public Overrides Function ToString() As String
                Return "NDArrayAnalysis.Builder(countTotal=" & countTotalValue & ", countNull=" & countNullValue & ", minLength=" & minLengthValue &
                       ", maxLength=" & maxLengthValue & ", totalNDArrayValues=" & totalNDArrayValuesValue & ", minValue=" & Arrays.JavaDoubleToString(minValueValue) &
                       ", maxValue=" & Arrays.JavaDoubleToString(maxValueValue) & ")"
            End Function
        End Class

    End Class

End Namespace
