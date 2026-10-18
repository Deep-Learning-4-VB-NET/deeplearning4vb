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
Imports System.Text.Json.Serialization
Imports ColumnTypeEnum = DeepLearning4VB.org.datavec.api.transform.ColumnType

Namespace org.datavec.api.transform.analysis.columns

    ''' <summary>Analysis of a string column: statistics of the string lengths and an optional length histogram.</summary>
    ' Java: @Data @AllArgsConstructor @NoArgsConstructor
    <Serializable>
    Public Class StringAnalysis
        Implements ColumnAnalysis

        Private minLength As Integer
        Private maxLength As Integer
        Private meanLength As Double
        Private sampleStdevLength As Double
        Private sampleVarianceLength As Double
        ''' <summary>Java field <c>countTotal</c> (renamed: VB names are case-insensitive and <see cref="CountTotal"/> exists).</summary>
        <JsonPropertyName("countTotal")>
        Private countTotalValue As Long
        Private histogramBuckets() As Double
        Private histogramBucketCounts() As Long

        Public Sub New()
        End Sub

        Public Sub New(ByVal minLength As Integer, ByVal maxLength As Integer, ByVal meanLength As Double, ByVal sampleStdevLength As Double, ByVal sampleVarianceLength As Double, ByVal countTotal As Long, ByVal histogramBuckets() As Double, ByVal histogramBucketCounts() As Long)
            Me.minLength = minLength
            Me.maxLength = maxLength
            Me.meanLength = meanLength
            Me.sampleStdevLength = sampleStdevLength
            Me.sampleVarianceLength = sampleVarianceLength
            Me.countTotalValue = countTotal
            Me.histogramBuckets = histogramBuckets
            Me.histogramBucketCounts = histogramBucketCounts
        End Sub

        Private Sub New(ByVal builder As Builder)
            Me.New(builder.minLengthValue, builder.maxLengthValue, builder.meanLengthValue, builder.sampleStdevLengthValue, builder.sampleVarianceLengthValue,
                   builder.countTotalValue, builder.histogramBucketsValue, builder.histogramBucketCountsValue)
        End Sub

        Public Overrides Function ToString() As String
            Return "StringAnalysis(minLen=" & minLength & ",maxLen=" & maxLength & ",meanLen=" & Arrays.JavaDoubleToString(meanLength) &
                   ",sampleStDevLen=" & Arrays.JavaDoubleToString(sampleStdevLength) & ",sampleVarianceLen=" & Arrays.JavaDoubleToString(sampleVarianceLength) &
                   ",count=" & countTotalValue & ")"
        End Function

        Public Overridable ReadOnly Property ColumnType As ColumnTypeEnum Implements ColumnAnalysis.ColumnType
            Get
                Return ColumnTypeEnum.String
            End Get
        End Property

        Public Overridable ReadOnly Property CountTotal As Long Implements ColumnAnalysis.CountTotal
            Get
                Return countTotalValue
            End Get
        End Property

        ' ---- Lombok @Data accessors ----

        Public Function getMinLength() As Integer
            Return minLength
        End Function

        Public Sub setMinLength(ByVal minLength As Integer)
            Me.minLength = minLength
        End Sub

        Public Function getMaxLength() As Integer
            Return maxLength
        End Function

        Public Sub setMaxLength(ByVal maxLength As Integer)
            Me.maxLength = maxLength
        End Sub

        Public Function getMeanLength() As Double
            Return meanLength
        End Function

        Public Sub setMeanLength(ByVal meanLength As Double)
            Me.meanLength = meanLength
        End Sub

        Public Function getSampleStdevLength() As Double
            Return sampleStdevLength
        End Function

        Public Sub setSampleStdevLength(ByVal sampleStdevLength As Double)
            Me.sampleStdevLength = sampleStdevLength
        End Sub

        Public Function getSampleVarianceLength() As Double
            Return sampleVarianceLength
        End Function

        Public Sub setSampleVarianceLength(ByVal sampleVarianceLength As Double)
            Me.sampleVarianceLength = sampleVarianceLength
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

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As StringAnalysis = TryCast(obj, StringAnalysis)
            If other Is Nothing Then Return False
            Return minLength = other.minLength AndAlso maxLength = other.maxLength AndAlso meanLength.Equals(other.meanLength) AndAlso
                   sampleStdevLength.Equals(other.sampleStdevLength) AndAlso sampleVarianceLength.Equals(other.sampleVarianceLength) AndAlso
                   countTotalValue = other.countTotalValue AndAlso Arrays.equals(histogramBuckets, other.histogramBuckets) AndAlso
                   Arrays.equals(histogramBucketCounts, other.histogramBucketCounts)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(minLength, maxLength, meanLength, sampleStdevLength, sampleVarianceLength, countTotalValue,
                                Arrays.hashCode(histogramBuckets), Arrays.hashCode(histogramBucketCounts))
        End Function

        Public Class Builder
            Friend minLengthValue As Integer
            Friend maxLengthValue As Integer
            Friend meanLengthValue As Double
            Friend sampleStdevLengthValue As Double
            Friend sampleVarianceLengthValue As Double
            Friend countTotalValue As Long
            Friend histogramBucketsValue() As Double
            Friend histogramBucketCountsValue() As Long

            Public Overridable Function minLength(ByVal value As Integer) As Builder
                Me.minLengthValue = value
                Return Me
            End Function

            Public Overridable Function maxLength(ByVal value As Integer) As Builder
                Me.maxLengthValue = value
                Return Me
            End Function

            Public Overridable Function meanLength(ByVal value As Double) As Builder
                Me.meanLengthValue = value
                Return Me
            End Function

            Public Overridable Function sampleStdevLength(ByVal value As Double) As Builder
                Me.sampleStdevLengthValue = value
                Return Me
            End Function

            Public Overridable Function sampleVarianceLength(ByVal value As Double) As Builder
                Me.sampleVarianceLengthValue = value
                Return Me
            End Function

            Public Overridable Function countTotal(ByVal value As Long) As Builder
                Me.countTotalValue = value
                Return Me
            End Function

            Public Overridable Function histogramBuckets(ByVal value() As Double) As Builder
                Me.histogramBucketsValue = value
                Return Me
            End Function

            Public Overridable Function histogramBucketCounts(ByVal value() As Long) As Builder
                Me.histogramBucketCountsValue = value
                Return Me
            End Function

            Public Overridable Function build() As StringAnalysis
                Return New StringAnalysis(Me)
            End Function
        End Class

    End Class

End Namespace
