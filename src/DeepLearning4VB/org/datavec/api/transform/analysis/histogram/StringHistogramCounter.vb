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
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.histogram

    ''' <summary>
    ''' Histogram of the string lengths of a column over <c>nBins</c> bins. As in Java, the bin edges are
    ''' <c>i * (maxLength - minLength) / nBins</c> - they start at 0, not at <c>minLength</c> - except the last edge,
    ''' which is <c>maxLength</c>; a length outside every bin <c>bins(i) &lt;= length &lt; bins(i + 1)</c> is counted
    ''' in the last bin.
    ''' </summary>
    <Serializable>
    Public Class StringHistogramCounter
        Implements HistogramCounter

        Private ReadOnly minLength As Integer
        Private ReadOnly maxLength As Integer
        Private ReadOnly nBins As Integer
        Private ReadOnly binsValue() As Double
        Private ReadOnly binCounts() As Long

        Public Sub New(ByVal minLength As Integer, ByVal maxLength As Integer, ByVal nBins As Integer)
            Me.minLength = minLength
            Me.maxLength = maxLength
            Me.nBins = nBins

            binsValue = New Double(nBins) {} '+1 because bins are defined by a range of values: bins[i] to bins[i+1]
            Dim [step] As Double = (CDbl(maxLength - minLength)) / nBins
            For i As Integer = 0 To binsValue.Length - 1
                If i = binsValue.Length - 1 Then
                    binsValue(i) = maxLength
                Else
                    binsValue(i) = i * [step]
                End If
            Next i

            binCounts = New Long(nBins - 1) {}
        End Sub

        Public Overridable Function add(ByVal w As WritableT) As HistogramCounter Implements HistogramCounter.add
            Dim d As Double = w.ToString().Length

            'Not super efficient, but linear search on 20-50 items should be good enough
            Dim idx As Integer = -1
            For i As Integer = 0 To nBins - 1
                If d >= binsValue(i) AndAlso d < binsValue(i + 1) Then
                    idx = i
                    Exit For
                End If
            Next i
            If idx = -1 Then
                idx = nBins - 1
            End If

            binCounts(idx) += 1

            Return Me
        End Function

        ''' <summary>Adds the other histogram's counts to this one (same lengths and number of bins required) and returns this.</summary>
        Public Overridable Function merge(ByVal other As HistogramCounter) As StringHistogramCounter
            If other Is Nothing Then
                Return Me
            End If
            If Not (TypeOf other Is StringHistogramCounter) Then
                Throw New ArgumentException("Cannot merge " & other.ToString())
            End If

            Dim o As StringHistogramCounter = DirectCast(other, StringHistogramCounter)

            If minLength <> o.minLength OrElse maxLength <> o.maxLength Then
                Throw New InvalidOperationException("Min/max values differ: (" & minLength & "," & maxLength & ") " & " vs. (" & o.minLength & "," & o.maxLength & ")")
            End If
            If nBins <> o.nBins Then
                Throw New InvalidOperationException("Different number of bins: " & nBins & " vs " & o.nBins)
            End If

            For i As Integer = 0 To nBins - 1
                binCounts(i) += o.binCounts(i)
            Next i

            Return Me
        End Function

        Private Function mergeCounter(ByVal other As HistogramCounter) As HistogramCounter Implements HistogramCounter.merge
            Return merge(other)
        End Function

        Public Overridable ReadOnly Property Bins As Double() Implements HistogramCounter.Bins
            Get
                Return binsValue
            End Get
        End Property

        Public Overridable ReadOnly Property Counts As Long() Implements HistogramCounter.Counts
            Get
                Return binCounts
            End Get
        End Property
    End Class

End Namespace
