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

Namespace org.datavec.api.transform.analysis.counter

    ''' <summary>Analysis counter of a bytes column: only the number of values.</summary>
    ' Java: @AllArgsConstructor @Data
    <Serializable>
    Public Class BytesAnalysisCounter
        Implements AnalysisCounter(Of BytesAnalysisCounter)

        Private countTotal As Long = 0

        Public Sub New()
        End Sub

        Public Sub New(ByVal countTotal As Long)
            Me.countTotal = countTotal
        End Sub

        Public Overridable Function add(ByVal writable As WritableT) As BytesAnalysisCounter Implements AnalysisCounter(Of BytesAnalysisCounter).add
            countTotal += 1

            Return Me
        End Function

        ''' <summary>Returns a new counter with the sum of both counts.</summary>
        Public Overridable Function merge(ByVal other As BytesAnalysisCounter) As BytesAnalysisCounter Implements AnalysisCounter(Of BytesAnalysisCounter).merge
            Return New BytesAnalysisCounter(countTotal + other.countTotal)
        End Function

        Private Function addUntyped(ByVal writable As WritableT) As AnalysisCounter Implements AnalysisCounter.add
            Return add(writable)
        End Function

        Private Function mergeUntyped(ByVal other As AnalysisCounter) As AnalysisCounter Implements AnalysisCounter.merge
            Return merge(DirectCast(other, BytesAnalysisCounter))
        End Function

        ' ---- Lombok @Data accessors ----

        Public Function getCountTotal() As Long
            Return countTotal
        End Function

        Public Sub setCountTotal(ByVal countTotal As Long)
            Me.countTotal = countTotal
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other As BytesAnalysisCounter = TryCast(obj, BytesAnalysisCounter)
            Return other IsNot Nothing AndAlso countTotal = other.countTotal
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Objects.hash(countTotal)
        End Function

        Public Overrides Function ToString() As String
            Return "BytesAnalysisCounter(countTotal=" & countTotal & ")"
        End Function
    End Class

End Namespace
