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
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.analysis.histogram

    ''' <summary>
    ''' Histogram of a categorical column: one bin per state name (in the order of the state names). The bin edges are
    ''' <c>0, 1, ..., stateNames.Count</c>; values that are not state names are counted but not reported.
    ''' </summary>
    <Serializable>
    Public Class CategoricalHistogramCounter
        Implements HistogramCounter

        Private countsByValue As New Dictionary(Of String, Integer)()

        Private stateNames As IList(Of String)

        Public Sub New(ByVal stateNames As IList(Of String))
            Me.stateNames = stateNames
        End Sub

        Public Overridable Function add(ByVal w As WritableT) As HistogramCounter Implements HistogramCounter.add
            Dim value As String = w.ToString()
            If countsByValue.ContainsKey(value) Then
                countsByValue(value) = countsByValue(value) + 1
            Else
                countsByValue(value) = 1
            End If
            Return Me
        End Function

        Public Overridable Function merge(ByVal other As HistogramCounter) As HistogramCounter Implements HistogramCounter.merge
            If Not (TypeOf other Is CategoricalHistogramCounter) Then
                Throw New ArgumentException("Input must be CategoricalHistogramCounter; got " & If(other Is Nothing, "null", other.ToString()))
            End If

            Dim o As CategoricalHistogramCounter = DirectCast(other, CategoricalHistogramCounter)

            For Each entry As KeyValuePair(Of String, Integer) In o.countsByValue
                Dim key As String = entry.Key
                If countsByValue.ContainsKey(key) Then
                    countsByValue(key) = countsByValue(key) + entry.Value
                Else
                    countsByValue(key) = entry.Value
                End If
            Next entry

            Return Me
        End Function

        Public Overridable ReadOnly Property Bins As Double() Implements HistogramCounter.Bins
            Get
                Dim binEdges(stateNames.Count) As Double
                For i As Integer = 0 To binEdges.Length - 1
                    binEdges(i) = i
                Next i
                Return binEdges
            End Get
        End Property

        Public Overridable ReadOnly Property Counts As Long() Implements HistogramCounter.Counts
            Get
                Dim ret(stateNames.Count - 1) As Long
                Dim i As Integer = 0
                For Each s As String In stateNames
                    Dim c As Integer
                    ret(i) = If(countsByValue.TryGetValue(s, c), c, 0)
                    i += 1
                Next s
                Return ret
            End Get
        End Property
    End Class

End Namespace
