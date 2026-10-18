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
Imports DeepLearning4VB.org.nd4vb.common.function
Imports HistogramCounterT = DeepLearning4VB.org.datavec.api.transform.analysis.histogram.HistogramCounter

Namespace org.datavec.local.transforms.analysis.histogram

    ''' <summary>
    ''' Merges two lists of per-column histogram counters column by column. A <c>Nothing</c> list stands for "no data
    ''' yet" and returns the other list; a <c>Nothing</c> counter (bytes columns) yields the other counter.
    ''' </summary>
    Public Class HistogramCombineFunction
        Implements BiFunction(Of IList(Of HistogramCounterT), IList(Of HistogramCounterT), IList(Of HistogramCounterT))

        Public Overridable Function apply(ByVal l1 As IList(Of HistogramCounterT), ByVal l2 As IList(Of HistogramCounterT)) As IList(Of HistogramCounterT) Implements BiFunction(Of IList(Of HistogramCounterT), IList(Of HistogramCounterT), IList(Of HistogramCounterT)).apply
            If l1 Is Nothing Then
                Return l2
            End If
            If l2 Is Nothing Then
                Return l1
            End If

            Dim size As Integer = l1.Count
            If size <> l2.Count Then
                Throw New InvalidOperationException("List lengths differ")
            End If

            Dim [out] As IList(Of HistogramCounterT) = New List(Of HistogramCounterT)()
            For i As Integer = 0 To size - 1
                Dim c1 As HistogramCounterT = l1(i)
                Dim c2 As HistogramCounterT = l2(i)

                'Normally shouldn't get null values here - but maybe for Bytes column, etc.
                If c1 Is Nothing Then
                    [out].Add(c2)
                ElseIf c2 Is Nothing Then
                    [out].Add(c1)
                Else
                    [out].Add(c1.merge(c2))
                End If
            Next i
            Return [out]
        End Function

    End Class

End Namespace
