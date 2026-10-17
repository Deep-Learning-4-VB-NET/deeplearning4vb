Imports System
Imports System.Collections
Imports System.Collections.Concurrent
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

Namespace org.nd4vb.common.primitives

    ''' <summary>
    ''' A two-level counter: a map from a first element to a <see cref="Counter(Of T)"/> of second elements.
    ''' </summary>
    <Serializable>
    Public Class CounterMap(Of F, S)
        Implements IEnumerable(Of Pair(Of F, S))

        Private Const serialVersionUID As Long = 119L

        Protected Friend maps As New ConcurrentDictionary(Of F, Counter(Of S))()

        Public Sub New()
        End Sub

        ''' <summary>True if this CounterMap has no values stored.</summary>
        Public Overridable ReadOnly Property Empty As Boolean
            Get
                Return maps.Count = 0
            End Get
        End Property

        Public Function isEmpty() As Boolean
            Return Empty
        End Function

        ''' <summary>True if this CounterMap has no values stored for the given first element.</summary>
        Public Overridable Function isEmpty(ByVal element As F) As Boolean
            If Empty Then
                Return True
            End If
            Dim m As Counter(Of S) = maps.GetValueOrNull(element)
            If m Is Nothing Then
                Return True
            End If
            Return m.Empty
        End Function

        ''' <summary>Increments the values of this counter by the counts of another counter.</summary>
        Public Overridable Sub incrementAll(ByVal other As CounterMap(Of F, S))
            For Each entry As KeyValuePair(Of F, Counter(Of S)) In other.maps
                Dim key As F = entry.Key
                Dim innerCounter As Counter(Of S) = entry.Value
                For Each innerEntry As KeyValuePair(Of S, AtomicDouble) In innerCounter.entrySet()
                    incrementCount(key, innerEntry.Key, innerEntry.Value.[get]())
                Next innerEntry
            Next entry
        End Sub

        ''' <summary>Increments the count for the given first/second pair.</summary>
        Public Overridable Sub incrementCount(ByVal first As F, ByVal second As S, ByVal inc As Double)
            Dim counter As Counter(Of S) = maps.GetOrAdd(first, Function(k) New Counter(Of S)())
            counter.incrementCount(second, inc)
        End Sub

        ''' <summary>Returns the count for the given first/second pair (0 if absent).</summary>
        Public Overridable Function getCount(ByVal first As F, ByVal second As S) As Double
            Dim counter As Counter(Of S) = maps.GetValueOrNull(first)
            If counter Is Nothing Then
                Return 0.0
            End If
            Return counter.getCount(second)
        End Function

        ''' <summary>Sets the count for the given first/second pair.</summary>
        ''' <returns>The previous count.</returns>
        Public Overridable Function setCount(ByVal first As F, ByVal second As S, ByVal value As Double) As Double
            Dim counter As Counter(Of S) = maps.GetOrAdd(first, Function(k) New Counter(Of S)())
            Return counter.setCount(second, value)
        End Function

        ''' <summary>Returns the first/second pair with the highest count (Nothing if empty).</summary>
        Public Overridable Function argMax() As Pair(Of F, S)
            Dim maxCount As Double = Double.NegativeInfinity
            Dim maxKey As Pair(Of F, S) = Nothing
            For Each entry As KeyValuePair(Of F, Counter(Of S)) In maps
                Dim counter As Counter(Of S) = entry.Value
                If counter.Empty Then
                    Continue For
                End If
                Dim localMax As S = counter.argMax()
                Dim localCount As Double = counter.getCount(localMax)
                If localCount > maxCount OrElse maxKey Is Nothing Then
                    maxKey = New Pair(Of F, S)(entry.Key, localMax)
                    maxCount = localCount
                End If
            Next entry
            Return maxKey
        End Function

        ''' <summary>Purges all counters.</summary>
        Public Overridable Sub clear()
            maps.Clear()
        End Sub

        ''' <summary>Purges the counter for the given first element.</summary>
        Public Overridable Sub clear(ByVal element As F)
            Dim s As Counter(Of S) = maps.GetValueOrNull(element)
            If s IsNot Nothing Then
                s.clear()
            End If
        End Sub

        ''' <summary>Returns a snapshot of all first elements.</summary>
        Public Overridable Function keySet() As ISet(Of F)
            Return New HashSet(Of F)(maps.Keys)
        End Function

        ''' <summary>Returns the counter for the given first element, or Nothing.</summary>
        Public Overridable Function getCounter(ByVal first As F) As Counter(Of S)
            Return maps.GetValueOrNull(first)
        End Function

        ''' <summary>Returns an enumerator over all first/second pairs stored in this counter.</summary>
        Public Overridable Function getIterator() As IEnumerator(Of Pair(Of F, S))
            Return GetEnumerator()
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of Pair(Of F, S)) Implements IEnumerable(Of Pair(Of F, S)).GetEnumerator
            For Each first As F In keySet()
                Dim counter As Counter(Of S) = getCounter(first)
                If counter Is Nothing Then
                    Continue For
                End If
                For Each second As S In counter.keySet()
                    Yield Pair.makePair(first, second)
                Next second
            Next first
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        ''' <summary>Returns the number of first elements.</summary>
        Public Overridable Function size() As Integer
            Return maps.Count
        End Function

        ''' <summary>Returns the total number of first/second pairs.</summary>
        Public Overridable Function totalSize() As Integer
            Dim total As Integer = 0
            For Each first As F In keySet()
                Dim c = getCounter(first)
                If c IsNot Nothing Then
                    total += c.size()
                End If
            Next first
            Return total
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            Dim other = TryCast(o, CounterMap(Of F, S))
            If other Is Nothing Then
                Return False
            End If
            If maps.Count <> other.maps.Count Then
                Return False
            End If
            For Each entry As KeyValuePair(Of F, Counter(Of S)) In maps
                Dim otherCounter = other.maps.GetValueOrNull(entry.Key)
                If otherCounter Is Nothing OrElse Not otherCounter.Equals(entry.Value) Then
                    Return False
                End If
            Next entry
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 0
            For Each entry As KeyValuePair(Of F, Counter(Of S)) In maps
                h = h Xor (Objects.hashCode(entry.Key) Xor entry.Value.GetHashCode())
            Next entry
            Return h
        End Function
    End Class

End Namespace
