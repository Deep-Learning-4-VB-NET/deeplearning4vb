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
    ''' Histogram of a numerical column over <c>nBins</c> equal-width bins from <c>minValue</c> to <c>maxValue</c>.
    ''' A value <c>v</c> falls into bin <c>i</c> when <c>bins(i) &lt;= v &lt; bins(i + 1)</c>; as in Java, every other
    ''' value (the maximum itself, values out of range on either side and NaN) is counted in the last bin.
    ''' </summary>
    <Serializable>
    Public Class DoubleHistogramCounter
        Implements HistogramCounter

        Private ReadOnly minValue As Double
        Private ReadOnly maxValue As Double
        Private ReadOnly nBins As Integer
        Private ReadOnly binsValue() As Double
        Private ReadOnly binCounts() As Long

        Public Sub New(ByVal minValue As Double, ByVal maxValue As Double, ByVal nBins As Integer)
            Me.minValue = minValue
            Me.maxValue = maxValue
            Me.nBins = nBins

            binsValue = New Double(nBins) {} '+1 because bins are defined by a range of values: bins[i] to bins[i+1]
            Dim [step] As Double = (maxValue - minValue) / nBins
            For i As Integer = 0 To binsValue.Length - 1
                If i = binsValue.Length - 1 Then
                    binsValue(i) = maxValue
                Else
                    binsValue(i) = minValue + i * [step]
                End If
            Next i

            binCounts = New Long(nBins - 1) {}
        End Sub

        Public Overridable Function add(ByVal w As WritableT) As HistogramCounter Implements HistogramCounter.add
            Dim d As Double = w.toDouble()

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

        ''' <summary>Adds the other histogram's counts to this one (same range and number of bins required) and returns this.</summary>
        Public Overridable Function merge(ByVal other As HistogramCounter) As DoubleHistogramCounter
            If other Is Nothing Then
                Return Me
            End If
            If Not (TypeOf other Is DoubleHistogramCounter) Then
                Throw New ArgumentException("Cannot merge " & other.ToString())
            End If

            Dim o As DoubleHistogramCounter = DirectCast(other, DoubleHistogramCounter)

            If minValue <> o.minValue OrElse maxValue <> o.maxValue Then
                Throw New InvalidOperationException("Min/max values differ: (" & Arrays.JavaDoubleToString(minValue) & "," & Arrays.JavaDoubleToString(maxValue) & ") " & " vs. (" & Arrays.JavaDoubleToString(o.minValue) & "," & Arrays.JavaDoubleToString(o.maxValue) & ")")
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
