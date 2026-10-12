Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Linq
Imports DeepLearning4VB.JavaCompat

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

Namespace org.nd4j.common.primitives

    ''' <summary>
    ''' A thread-safe map from elements to double-valued counts.
    ''' </summary>
    ''' <typeparam name="T">Element type.</typeparam>
    <Serializable>
    Public Class Counter(Of T)
        Private Const serialVersionUID As Long = 119L

        Protected Friend map As New ConcurrentDictionary(Of T, AtomicDouble)()
        Protected Friend totalCountValue As New AtomicDouble(0)
        Protected Friend dirty As New AtomicBoolean(False)

        Public Sub New()
        End Sub

        ''' <summary>Returns the count for the element, or 0 if the element is not present.</summary>
        Public Overridable Function getCount(ByVal element As T) As Double
            Dim t As AtomicDouble = map.GetValueOrNull(element)
            If t Is Nothing Then
                Return 0.0
            End If
            Return t.[get]()
        End Function

        Public Overridable Sub incrementCount(ByVal element As T, ByVal inc As Double)
            Dim t As AtomicDouble = map.GetOrAdd(element, Function(k) New AtomicDouble(0.0))
            t.addAndGet(inc)
            totalCountValue.addAndGet(inc)
        End Sub

        ''' <summary>Increments the count of every element in the collection by <paramref name="inc"/>.</summary>
        Public Overridable Sub incrementAll(ByVal elements As IEnumerable(Of T), ByVal inc As Double)
            For Each element As T In elements
                incrementCount(element, inc)
            Next element
        End Sub

        ''' <summary>Increments the counts of this counter by the counts of another counter.</summary>
        Public Overridable Sub incrementAll(Of T2 As T)(ByVal other As Counter(Of T2))
            For Each element As T2 In other.keySet()
                Dim cnt As Double = other.getCount(element)
                incrementCount(element, cnt)
            Next element
        End Sub

        ''' <summary>Returns the probability of the element: its count divided by the total count.</summary>
        Public Overridable Function getProbability(ByVal element As T) As Double
            If totalCount() <= 0.0 Then
                Throw New InvalidOperationException("Can't calculate probability with empty counter")
            End If
            Return getCount(element) / totalCount()
        End Function

        ''' <summary>Sets a new count for the element.</summary>
        ''' <returns>The previous count (0 if the element was not present).</returns>
        Public Overridable Function setCount(ByVal element As T, ByVal count As Double) As Double
            Dim t As AtomicDouble = map.GetValueOrNull(element)
            If t IsNot Nothing Then
                Dim previous As Double = t.getAndSet(count)
                dirty.[set](True)
                Return previous
            Else
                map(element) = New AtomicDouble(count)
                totalCountValue.addAndGet(count)
                Return 0
            End If
        End Function

        ''' <summary>Returns a snapshot of the elements tracked by this counter.</summary>
        Public Overridable Function keySet() As ISet(Of T)
            Return New HashSet(Of T)(map.Keys)
        End Function

        ''' <summary>True if the counter has no elements.</summary>
        Public Overridable ReadOnly Property Empty As Boolean
            Get
                Return map.Count = 0
            End Get
        End Property

        Public Function isEmpty() As Boolean
            Return Empty
        End Function

        ''' <summary>Returns a snapshot of the element/count entries.</summary>
        Public Overridable Function entrySet() As ISet(Of KeyValuePair(Of T, AtomicDouble))
            Return New HashSet(Of KeyValuePair(Of T, AtomicDouble))(map)
        End Function

        ''' <summary>Returns the elements sorted by descending count.</summary>
        Public Overridable Function keySetSorted() As IList(Of T)
            Dim result As IList(Of T) = New List(Of T)()
            Dim pq As PriorityQueue(Of Pair(Of T, Double)) = asPriorityQueue()
            Do While Not pq.isEmpty()
                result.Add(pq.poll().First)
            Loop
            Return result
        End Function

        ''' <summary>Divides every count by the total so that the counts sum to 1.</summary>
        Public Overridable Sub normalize()
            Dim total As Double = totalCount()
            For Each key As T In keySet()
                setCount(key, getCount(key) / total)
            Next key
            rebuildTotals()
        End Sub

        Protected Friend Overridable Sub rebuildTotals()
            Dim total As Double = 0.0
            For Each key As T In keySet()
                total += getCount(key)
            Next key
            totalCountValue.[set](total)
            dirty.[set](False)
        End Sub

        ''' <summary>Returns the sum of all counts.</summary>
        Public Overridable Function totalCount() As Double
            If dirty.[get]() Then
                rebuildTotals()
            End If
            Return totalCountValue.[get]()
        End Function

        ''' <summary>Removes the element from the counter.</summary>
        ''' <returns>The removed count, or 0 if the element was not present.</returns>
        Public Overridable Function removeKey(ByVal element As T) As Double
            Dim v As AtomicDouble = Nothing
            map.TryRemove(element, v)
            dirty.[set](True)
            If v IsNot Nothing Then
                Return v.[get]()
            End If
            Return 0.0
        End Function

        ''' <summary>Returns the element with the highest count (default value if the counter is empty).</summary>
        Public Overridable Function argMax() As T
            Dim maxCount As Double = Double.NegativeInfinity
            Dim maxKey As T = Nothing
            Dim found As Boolean = False
            For Each entry As KeyValuePair(Of T, AtomicDouble) In map
                Dim c As Double = entry.Value.[get]()
                If c > maxCount OrElse Not found Then
                    maxKey = entry.Key
                    maxCount = c
                    found = True
                End If
            Next entry
            Return maxKey
        End Function

        ''' <summary>Removes all elements whose count is below the threshold.</summary>
        Public Overridable Sub dropElementsBelowThreshold(ByVal threshold As Double)
            For Each element As T In keySet()
                Dim t As AtomicDouble = map.GetValueOrNull(element)
                If t IsNot Nothing AndAlso t.[get]() < threshold Then
                    Dim removed As AtomicDouble = Nothing
                    map.TryRemove(element, removed)
                    dirty.[set](True)
                End If
            Next element
        End Sub

        Public Overridable Function containsElement(ByVal element As T) As Boolean
            Return map.ContainsKey(element)
        End Function

        ''' <summary>Resets the counter to the empty state.</summary>
        Public Overridable Sub clear()
            map.Clear()
            totalCountValue.[set](0.0)
            dirty.[set](False)
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            Dim other = TryCast(o, Counter(Of T))
            If other Is Nothing Then
                Return False
            End If
            If map.Count <> other.map.Count Then
                Return False
            End If
            For Each entry As KeyValuePair(Of T, AtomicDouble) In map
                Dim otherValue As AtomicDouble = other.map.GetValueOrNull(entry.Key)
                If otherValue Is Nothing OrElse otherValue.[get]() <> entry.Value.[get]() Then
                    Return False
                End If
            Next entry
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 0
            For Each entry As KeyValuePair(Of T, AtomicDouble) In map
                h = h Xor (Objects.hashCode(entry.Key) Xor entry.Value.GetHashCode())
            Next entry
            Return h
        End Function

        ''' <summary>Returns the number of tracked elements.</summary>
        Public Overridable Function size() As Integer
            Return map.Count
        End Function

        ''' <summary>Removes all elements except the N with the highest counts.</summary>
        Public Overridable Sub keepTopNElements(ByVal N As Integer)
            Dim queue As PriorityQueue(Of Pair(Of T, Double)) = asPriorityQueue()
            clear()
            For e As Integer = 0 To N - 1
                Dim pair As Pair(Of T, Double) = queue.poll()
                If pair IsNot Nothing Then
                    incrementCount(pair.First, pair.Second)
                End If
            Next e
        End Sub

        ''' <summary>Returns the element/count pairs as a queue ordered by descending count.</summary>
        Public Overridable Function asPriorityQueue() As PriorityQueue(Of Pair(Of T, Double))
            Dim pq As New PriorityQueue(Of Pair(Of T, Double))(Math.Max(1, map.Count), New PairComparator(Me))
            For Each entry As KeyValuePair(Of T, AtomicDouble) In map
                pq.add(Pair.create(entry.Key, entry.Value.[get]()))
            Next entry
            Return pq
        End Function

        ''' <summary>Returns the element/count pairs as a queue ordered by ascending count.</summary>
        Public Overridable Function asReversedPriorityQueue() As PriorityQueue(Of Pair(Of T, Double))
            Dim pq As New PriorityQueue(Of Pair(Of T, Double))(Math.Max(1, map.Count), New ReversedPairComparator(Me))
            For Each entry As KeyValuePair(Of T, AtomicDouble) In map
                pq.add(Pair.create(entry.Key, entry.Value.[get]()))
            Next entry
            Return pq
        End Function

        Public Overrides Function ToString() As String
            Return "Counter{" & String.Join(", ", map.Select(Function(e) Arrays.FormatElement(e.Key) & "=" & Arrays.FormatElement(e.Value.[get]()))) & "}"
        End Function

        Public Class PairComparator
            Implements IComparer(Of Pair(Of T, Double))

            Private ReadOnly outerInstance As Counter(Of T)

            Public Sub New(ByVal outerInstance As Counter(Of T))
                Me.outerInstance = outerInstance
            End Sub

            Public Overridable Function Compare(ByVal o1 As Pair(Of T, Double), ByVal o2 As Pair(Of T, Double)) As Integer Implements IComparer(Of Pair(Of T, Double)).Compare
                Return o2.Second.CompareTo(o1.Second)
            End Function
        End Class

        Public Class ReversedPairComparator
            Implements IComparer(Of Pair(Of T, Double))

            Private ReadOnly outerInstance As Counter(Of T)

            Public Sub New(ByVal outerInstance As Counter(Of T))
                Me.outerInstance = outerInstance
            End Sub

            Public Overridable Function Compare(ByVal o1 As Pair(Of T, Double), ByVal o2 As Pair(Of T, Double)) As Integer Implements IComparer(Of Pair(Of T, Double)).Compare
                Return o1.Second.CompareTo(o2.Second)
            End Function
        End Class
    End Class

End Namespace
