Imports System
Imports System.Collections.Generic

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

Namespace org.nd4vb.linalg.profiler.data.primitives

    ''' <summary>
    ''' The recorded times of one profiler key (Java <c>TimeSet</c>). Sets sort in descending order of their sum.
    ''' Like Java, <see cref="getMedian"/> returns the middle element in insertion order (the times are not sorted).
    ''' </summary>
    Public Class TimeSet
        Implements IComparable(Of TimeSet)

        Private ReadOnly times As New List(Of ComparableAtomicLong)()
        Private sumValue As Long = 0

        Public Overridable Sub addTime(ByVal time As Long)
            times.Add(New ComparableAtomicLong(time))
            sumValue = 0
        End Sub

        Public Overridable Function getSum() As Long
            If sumValue = 0 Then
                For Each time As ComparableAtomicLong In times
                    sumValue += time.get()
                Next time
            End If

            Return sumValue
        End Function

        Public Overridable Function getAverage() As Long
            If times.Count = 0 Then
                Return 0L
            End If

            Dim tSum As Long = getSum()
            Return tSum \ times.Count
        End Function

        Public Overridable Function getMedian() As Long
            If times.Count = 0 Then
                Return 0L
            End If

            Return times(times.Count \ 2).longValue()
        End Function

        Public Overridable Function getMinimum() As Long
            Dim min As Long = Long.MaxValue
            For Each time As ComparableAtomicLong In times
                If time.get() < min Then
                    min = time.get()
                End If
            Next time

            Return min
        End Function

        Public Overridable Function getMaximum() As Long
            Dim max As Long = Long.MinValue
            For Each time As ComparableAtomicLong In times
                If time.get() > max Then
                    max = time.get()
                End If
            Next time

            Return max
        End Function

        Public Overridable Function size() As Integer
            Return times.Count
        End Function

        Public Overridable Function CompareTo(ByVal o As TimeSet) As Integer Implements IComparable(Of TimeSet).CompareTo
            Return o.getSum().CompareTo(Me.getSum())
        End Function

    End Class

End Namespace
