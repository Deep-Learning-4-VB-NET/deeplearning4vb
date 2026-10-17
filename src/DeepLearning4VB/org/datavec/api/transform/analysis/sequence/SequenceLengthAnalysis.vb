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

Namespace org.datavec.api.transform.analysis.sequence

    ''' <summary>
    ''' Statistics of the lengths of the sequences in a sequence data set: number of sequences, shortest and longest
    ''' length, counts of empty and one-step sequences, mean length and an optional length histogram. Lombok's
    ''' <c>@Builder</c> is <see cref="builder"/> returning a <see cref="SequenceLengthAnalysisBuilder"/>.
    ''' </summary>
    ' Java: @AllArgsConstructor @Data @Builder
    <Serializable>
    Public Class SequenceLengthAnalysis

        Private totalNumSequences As Long
        Private minSeqLength As Integer
        Private maxSeqLength As Integer
        Private countZeroLength As Long
        Private countOneLength As Long
        Private meanLength As Double
        Private histogramBuckets() As Double
        Private histogramBucketCounts() As Long

        Protected Friend Sub New()
            'No-arg for JSON
        End Sub

        Public Sub New(ByVal totalNumSequences As Long, ByVal minSeqLength As Integer, ByVal maxSeqLength As Integer, ByVal countZeroLength As Long, ByVal countOneLength As Long, ByVal meanLength As Double, ByVal histogramBuckets() As Double, ByVal histogramBucketCounts() As Long)
            Me.totalNumSequences = totalNumSequences
            Me.minSeqLength = minSeqLength
            Me.maxSeqLength = maxSeqLength
            Me.countZeroLength = countZeroLength
            Me.countOneLength = countOneLength
            Me.meanLength = meanLength
            Me.histogramBuckets = histogramBuckets
            Me.histogramBucketCounts = histogramBucketCounts
        End Sub

        ''' <summary>Creates a builder (Lombok <c>@Builder</c>).</summary>
        Public Shared Function builder() As SequenceLengthAnalysisBuilder
            Return New SequenceLengthAnalysisBuilder()
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.Append("SequenceLengthAnalysis(").Append("totalNumSequences=").Append(totalNumSequences).Append(",minSeqLength=").Append(minSeqLength).Append(",maxSeqLength=").Append(maxSeqLength).Append(",countZeroLength=").Append(countZeroLength).Append(",countOneLength=").Append(countOneLength).Append(",meanLength=").Append(Arrays.JavaDoubleToString(meanLength)).Append(")")
            Return sb.ToString()
        End Function

        ' ---- Lombok @Data accessors ----

        Public Function getTotalNumSequences() As Long
            Return totalNumSequences
        End Function

        Public Sub setTotalNumSequences(ByVal totalNumSequences As Long)
            Me.totalNumSequences = totalNumSequences
        End Sub

        Public Function getMinSeqLength() As Integer
            Return minSeqLength
        End Function

        Public Sub setMinSeqLength(ByVal minSeqLength As Integer)
            Me.minSeqLength = minSeqLength
        End Sub

        Public Function getMaxSeqLength() As Integer
            Return maxSeqLength
        End Function

        Public Sub setMaxSeqLength(ByVal maxSeqLength As Integer)
            Me.maxSeqLength = maxSeqLength
        End Sub

        Public Function getCountZeroLength() As Long
            Return countZeroLength
        End Function

        Public Sub setCountZeroLength(ByVal countZeroLength As Long)
            Me.countZeroLength = countZeroLength
        End Sub

        Public Function getCountOneLength() As Long
            Return countOneLength
        End Function

        Public Sub setCountOneLength(ByVal countOneLength As Long)
            Me.countOneLength = countOneLength
        End Sub

        Public Function getMeanLength() As Double
            Return meanLength
        End Function

        Public Sub setMeanLength(ByVal meanLength As Double)
            Me.meanLength = meanLength
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
            Dim other As SequenceLengthAnalysis = TryCast(obj, SequenceLengthAnalysis)
            If other Is Nothing Then Return False
            Return totalNumSequences = other.totalNumSequences AndAlso minSeqLength = other.minSeqLength AndAlso maxSeqLength = other.maxSeqLength AndAlso
                   countZeroLength = other.countZeroLength AndAlso countOneLength = other.countOneLength AndAlso meanLength.Equals(other.meanLength) AndAlso
                   Arrays.equals(histogramBuckets, other.histogramBuckets) AndAlso Arrays.equals(histogramBucketCounts, other.histogramBucketCounts)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(totalNumSequences, minSeqLength, maxSeqLength, countZeroLength, countOneLength, meanLength,
                                Arrays.hashCode(histogramBuckets), Arrays.hashCode(histogramBucketCounts))
        End Function

        ''' <summary>Lombok's builder of <see cref="SequenceLengthAnalysis"/>.</summary>
        Public Class SequenceLengthAnalysisBuilder
            Private totalNumSequencesValue As Long
            Private minSeqLengthValue As Integer
            Private maxSeqLengthValue As Integer
            Private countZeroLengthValue As Long
            Private countOneLengthValue As Long
            Private meanLengthValue As Double
            Private histogramBucketsValue() As Double
            Private histogramBucketCountsValue() As Long

            Friend Sub New()
            End Sub

            Public Function totalNumSequences(ByVal value As Long) As SequenceLengthAnalysisBuilder
                totalNumSequencesValue = value
                Return Me
            End Function

            Public Function minSeqLength(ByVal value As Integer) As SequenceLengthAnalysisBuilder
                minSeqLengthValue = value
                Return Me
            End Function

            Public Function maxSeqLength(ByVal value As Integer) As SequenceLengthAnalysisBuilder
                maxSeqLengthValue = value
                Return Me
            End Function

            Public Function countZeroLength(ByVal value As Long) As SequenceLengthAnalysisBuilder
                countZeroLengthValue = value
                Return Me
            End Function

            Public Function countOneLength(ByVal value As Long) As SequenceLengthAnalysisBuilder
                countOneLengthValue = value
                Return Me
            End Function

            Public Function meanLength(ByVal value As Double) As SequenceLengthAnalysisBuilder
                meanLengthValue = value
                Return Me
            End Function

            Public Function histogramBuckets(ByVal value() As Double) As SequenceLengthAnalysisBuilder
                histogramBucketsValue = value
                Return Me
            End Function

            Public Function histogramBucketCounts(ByVal value() As Long) As SequenceLengthAnalysisBuilder
                histogramBucketCountsValue = value
                Return Me
            End Function

            Public Function build() As SequenceLengthAnalysis
                Return New SequenceLengthAnalysis(totalNumSequencesValue, minSeqLengthValue, maxSeqLengthValue, countZeroLengthValue, countOneLengthValue,
                                                  meanLengthValue, histogramBucketsValue, histogramBucketCountsValue)
            End Function

            Public Overrides Function ToString() As String
                Return "SequenceLengthAnalysis.SequenceLengthAnalysisBuilder(totalNumSequences=" & totalNumSequencesValue & ", minSeqLength=" & minSeqLengthValue &
                       ", maxSeqLength=" & maxSeqLengthValue & ", countZeroLength=" & countZeroLengthValue & ", countOneLength=" & countOneLengthValue &
                       ", meanLength=" & Arrays.JavaDoubleToString(meanLengthValue) & ", histogramBuckets=" & Arrays.toString(histogramBucketsValue) &
                       ", histogramBucketCounts=" & Arrays.toString(histogramBucketCountsValue) & ")"
            End Function
        End Class

    End Class

End Namespace
