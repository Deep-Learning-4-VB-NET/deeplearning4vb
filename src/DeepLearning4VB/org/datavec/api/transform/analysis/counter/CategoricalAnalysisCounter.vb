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

Namespace org.datavec.api.transform.analysis.counter

    ''' <summary>Analysis counter of a categorical column: the number of occurrences of each value (as text).</summary>
    ' Java: @AllArgsConstructor @Data
    <Serializable>
    Public Class CategoricalAnalysisCounter
        Implements AnalysisCounter(Of CategoricalAnalysisCounter)

        Private counts As IDictionary(Of String, Long) = New Dictionary(Of String, Long)()
        Private countTotal As Long = 0

        Public Sub New()
        End Sub

        Public Sub New(ByVal counts As IDictionary(Of String, Long), ByVal countTotal As Long)
            Me.counts = counts
            Me.countTotal = countTotal
        End Sub

        Public Overridable Function add(ByVal writable As WritableT) As CategoricalAnalysisCounter Implements AnalysisCounter(Of CategoricalAnalysisCounter).add
            Dim value As String = writable.ToString()

            Dim newCount As Long = 0
            If counts.ContainsKey(value) Then
                newCount = counts(value)
            End If
            newCount += 1
            counts(value) = newCount

            countTotal += 1

            Return Me
        End Function

        ''' <summary>Adds the other counter's counts to this one and returns this counter (as in Java).</summary>
        Public Overridable Function merge(ByVal other As CategoricalAnalysisCounter) As CategoricalAnalysisCounter Implements AnalysisCounter(Of CategoricalAnalysisCounter).merge
            Dim combinedKeySet As ISet(Of String) = New HashSet(Of String)(counts.Keys)
            combinedKeySet.UnionWith(other.counts.Keys)

            For Each s As String In combinedKeySet
                Dim count As Long = 0
                If counts.ContainsKey(s) Then
                    count += counts(s)
                End If
                If other.counts.ContainsKey(s) Then
                    count += other.counts(s)
                End If
                counts(s) = count
            Next s

            countTotal += other.countTotal

            Return Me
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As AnalysisCounter Implements AnalysisCounter.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As AnalysisCounter) As AnalysisCounter Implements AnalysisCounter.merge
            Return merge(DirectCast(other, CategoricalAnalysisCounter))
        End Function

        ' ---- Lombok @Data accessors ----

        Public Function getCounts() As IDictionary(Of String, Long)
            Return counts
        End Function

        Public Sub setCounts(ByVal counts As IDictionary(Of String, Long))
            Me.counts = counts
        End Sub

        Public Function getCountTotal() As Long
            Return countTotal
        End Function

        Public Sub setCountTotal(ByVal countTotal As Long)
            Me.countTotal = countTotal
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim other As CategoricalAnalysisCounter = TryCast(obj, CategoricalAnalysisCounter)
            If other Is Nothing Then Return False
            If countTotal <> other.countTotal Then Return False
            If counts Is Nothing OrElse other.counts Is Nothing Then Return counts Is other.counts
            If counts.Count <> other.counts.Count Then Return False
            For Each kv As KeyValuePair(Of String, Long) In counts
                Dim v As Long
                If Not other.counts.TryGetValue(kv.Key, v) OrElse v <> kv.Value Then Return False
            Next
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 0
            If counts IsNot Nothing Then
                For Each kv As KeyValuePair(Of String, Long) In counts
                    h += If(kv.Key Is Nothing, 0, kv.Key.GetHashCode()) Xor kv.Value.GetHashCode()
                Next
            End If
            Return Objects.hash(h, countTotal)
        End Function

        Public Overrides Function ToString() As String
            Dim sb As New System.Text.StringBuilder()
            sb.Append("CategoricalAnalysisCounter(counts={")
            Dim first As Boolean = True
            If counts IsNot Nothing Then
                For Each kv As KeyValuePair(Of String, Long) In counts
                    If Not first Then sb.Append(", ")
                    first = False
                    sb.Append(kv.Key).Append("="c).Append(kv.Value)
                Next
            End If
            sb.Append("}, countTotal=").Append(countTotal).Append(")")
            Return sb.ToString()
        End Function
    End Class

End Namespace
