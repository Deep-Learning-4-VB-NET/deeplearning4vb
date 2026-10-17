Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.nd4vb.common.primitives

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

Namespace org.nd4vb.common.collection

    ''' <summary>
    ''' Orders pairs by their first element, then by their second (both must be comparable).
    ''' </summary>
    Friend Class PairComparer(Of K, T)
        Implements IComparer(Of Pair(Of K, T))

        Public Function Compare(ByVal x As Pair(Of K, T), ByVal y As Pair(Of K, T)) As Integer Implements IComparer(Of Pair(Of K, T)).Compare
            Dim c As Integer = Comparer(Of K).Default.Compare(x.First, y.First)
            If c <> 0 Then Return c
            Return Comparer(Of T).Default.Compare(x.Second, y.Second)
        End Function
    End Class

    ''' <summary>
    ''' A dictionary keyed by a pair of values (K, T), with convenience overloads that take the two
    ''' key components separately.
    ''' </summary>
    <Serializable>
    Public Class MultiDimensionalMap(Of K, T, V)
        Implements IDictionary(Of Pair(Of K, T), V)

        Private ReadOnly backedMap As IDictionary(Of Pair(Of K, T), V)

        ''' <summary>Thread-safe sorted map implementation.</summary>
        Public Shared Function newThreadSafeTreeBackedMap() As MultiDimensionalMap(Of K, T, V)
            Return New MultiDimensionalMap(Of K, T, V)(New SynchronizedDictionary(Of Pair(Of K, T), V)(New SortedDictionary(Of Pair(Of K, T), V)(New PairComparer(Of K, T)())))
        End Function

        ''' <summary>Thread-safe hash map implementation.</summary>
        Public Shared Function newThreadSafeHashBackedMap() As MultiDimensionalMap(Of K, T, V)
            Return New MultiDimensionalMap(Of K, T, V)(New ConcurrentDictionary(Of Pair(Of K, T), V)())
        End Function

        ''' <summary>Hash map implementation.</summary>
        Public Shared Function newHashBackedMap() As MultiDimensionalMap(Of K, T, V)
            Return New MultiDimensionalMap(Of K, T, V)(New Dictionary(Of Pair(Of K, T), V)())
        End Function

        ''' <summary>Sorted map implementation (K and T must be comparable).</summary>
        Public Shared Function newTreeBackedMap() As MultiDimensionalMap(Of K, T, V)
            Return New MultiDimensionalMap(Of K, T, V)(New SortedDictionary(Of Pair(Of K, T), V)(New PairComparer(Of K, T)()))
        End Function

        Public Sub New(ByVal backedMap As IDictionary(Of Pair(Of K, T), V))
            If backedMap Is Nothing Then Throw New ArgumentNullException(NameOf(backedMap))
            Me.backedMap = backedMap
        End Sub

        Public Sub New()
            Me.New(New Dictionary(Of Pair(Of K, T), V)())
        End Sub

        ' ---- Java-style API ----

        Public Overridable Function size() As Integer
            Return backedMap.Count
        End Function

        Public Overridable ReadOnly Property Empty As Boolean
            Get
                Return backedMap.Count = 0
            End Get
        End Property

        Public Function isEmpty() As Boolean
            Return Empty
        End Function

        Public Overridable Function containsValue(ByVal value As V) As Boolean
            Dim cmp = EqualityComparer(Of V).Default
            For Each v In backedMap.Values
                If cmp.Equals(v, value) Then Return True
            Next
            Return False
        End Function

        ''' <summary>Returns the value for the key, or the default value if absent.</summary>
        Public Overridable Function [get](ByVal key As Pair(Of K, T)) As V
            Dim v As V = Nothing
            backedMap.TryGetValue(key, v)
            Return v
        End Function

        ''' <summary>Returns the value for the key (k, t), or the default value if absent.</summary>
        Public Overridable Function [get](ByVal k As K, ByVal t As T) As V
            Return [get](New Pair(Of K, T)(k, t))
        End Function

        ''' <summary>Associates the value with the key and returns the previous value (default if none).</summary>
        Public Overridable Function put(ByVal key As Pair(Of K, T), ByVal value As V) As V
            Dim previous As V = Nothing
            backedMap.TryGetValue(key, previous)
            backedMap(key) = value
            Return previous
        End Function

        Public Overridable Sub put(ByVal k As K, ByVal t As T, ByVal v As V)
            put(New Pair(Of K, T)(k, t), v)
        End Sub

        ''' <summary>Removes the key and returns its value (default if it was absent).</summary>
        Public Overridable Function removeAndGet(ByVal key As Pair(Of K, T)) As V
            Dim previous As V = Nothing
            If backedMap.TryGetValue(key, previous) Then
                backedMap.Remove(key)
                Return previous
            End If
            Return Nothing
        End Function

        Public Overridable Function remove(ByVal k As K, ByVal t As T) As Boolean
            Return backedMap.Remove(New Pair(Of K, T)(k, t))
        End Function

        Public Overridable Sub putAll(ByVal m As IDictionary(Of Pair(Of K, T), V))
            For Each kv In m
                backedMap(kv.Key) = kv.Value
            Next
        End Sub

        Public Overridable Function keySet() As ISet(Of Pair(Of K, T))
            Return New LinkedHashSet(Of Pair(Of K, T))(backedMap.Keys)
        End Function

        Public Overridable Function entrySet() As ISet(Of Entry)
            Dim ret As New LinkedHashSet(Of Entry)()
            For Each kv In backedMap
                ret.Add(New Entry(kv.Key.First, kv.Key.Second, kv.Value))
            Next
            Return ret
        End Function

        Public Overridable Function contains(ByVal k As K, ByVal t As T) As Boolean
            Return backedMap.ContainsKey(New Pair(Of K, T)(k, t))
        End Function

        ' ---- IDictionary(Of Pair(Of K, T), V) ----

        Default Public Property Item(ByVal key As Pair(Of K, T)) As V Implements IDictionary(Of Pair(Of K, T), V).Item
            Get
                Return backedMap(key)
            End Get
            Set(ByVal value As V)
                backedMap(key) = value
            End Set
        End Property

        Public ReadOnly Property Keys As ICollection(Of Pair(Of K, T)) Implements IDictionary(Of Pair(Of K, T), V).Keys
            Get
                Return backedMap.Keys
            End Get
        End Property

        Public ReadOnly Property Values As ICollection(Of V) Implements IDictionary(Of Pair(Of K, T), V).Values
            Get
                Return backedMap.Values
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of Pair(Of K, T), V)).Count
            Get
                Return backedMap.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of Pair(Of K, T), V)).IsReadOnly
            Get
                Return backedMap.IsReadOnly
            End Get
        End Property

        Public Function ContainsKey(ByVal key As Pair(Of K, T)) As Boolean Implements IDictionary(Of Pair(Of K, T), V).ContainsKey
            Return backedMap.ContainsKey(key)
        End Function

        Public Sub Add(ByVal key As Pair(Of K, T), ByVal value As V) Implements IDictionary(Of Pair(Of K, T), V).Add
            backedMap.Add(key, value)
        End Sub

        Public Function Remove(ByVal key As Pair(Of K, T)) As Boolean Implements IDictionary(Of Pair(Of K, T), V).Remove
            Return backedMap.Remove(key)
        End Function

        Public Function TryGetValue(ByVal key As Pair(Of K, T), ByRef value As V) As Boolean Implements IDictionary(Of Pair(Of K, T), V).TryGetValue
            Return backedMap.TryGetValue(key, value)
        End Function

        Private Sub AddPair(ByVal item As KeyValuePair(Of Pair(Of K, T), V)) Implements ICollection(Of KeyValuePair(Of Pair(Of K, T), V)).Add
            backedMap.Add(item)
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of Pair(Of K, T), V)).Clear
            backedMap.Clear()
        End Sub

        Private Function ContainsPair(ByVal item As KeyValuePair(Of Pair(Of K, T), V)) As Boolean Implements ICollection(Of KeyValuePair(Of Pair(Of K, T), V)).Contains
            Return backedMap.Contains(item)
        End Function

        Private Sub CopyTo(ByVal array() As KeyValuePair(Of Pair(Of K, T), V), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of Pair(Of K, T), V)).CopyTo
            backedMap.CopyTo(array, arrayIndex)
        End Sub

        Private Function RemovePair(ByVal item As KeyValuePair(Of Pair(Of K, T), V)) As Boolean Implements ICollection(Of KeyValuePair(Of Pair(Of K, T), V)).Remove
            Return backedMap.Remove(item)
        End Function

        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of Pair(Of K, T), V)) Implements IEnumerable(Of KeyValuePair(Of Pair(Of K, T), V)).GetEnumerator
            Return backedMap.GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return backedMap.GetEnumerator()
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then Return True
            Dim other = TryCast(o, MultiDimensionalMap(Of K, T, V))
            If other Is Nothing OrElse other.Count <> Count Then Return False
            For Each kv In backedMap
                Dim v As V = Nothing
                If Not other.backedMap.TryGetValue(kv.Key, v) OrElse Not EqualityComparer(Of V).Default.Equals(v, kv.Value) Then
                    Return False
                End If
            Next
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 0
            For Each kv In backedMap
                h = h Xor (Objects.hashCode(kv.Key) Xor Objects.hashCode(kv.Value))
            Next
            Return h
        End Function

        Public Overrides Function ToString() As String
            Dim parts As New List(Of String)(backedMap.Count)
            For Each kv In backedMap
                parts.Add(kv.Key.ToString() & "=" & Arrays.FormatElement(kv.Value))
            Next
            Return "MultiDimensionalMap{" & String.Join(", ", parts) & "}"
        End Function

        ''' <summary>An entry of a <see cref="MultiDimensionalMap(Of K, T, V)"/>: the two key components and the value.</summary>
        <Serializable>
        Public Class Entry
            Private firstKeyValue As K
            Private secondKeyValue As T
            Private entryValue As V

            Public Sub New(ByVal firstKey As K, ByVal secondKey As T, ByVal value As V)
                firstKeyValue = firstKey
                secondKeyValue = secondKey
                entryValue = value
            End Sub

            Public Overridable Property FirstKey As K
                Get
                    Return firstKeyValue
                End Get
                Set(ByVal value As K)
                    firstKeyValue = value
                End Set
            End Property

            Public Overridable Property SecondKey As T
                Get
                    Return secondKeyValue
                End Get
                Set(ByVal value As T)
                    secondKeyValue = value
                End Set
            End Property

            Public Overridable ReadOnly Property Value As V
                Get
                    Return entryValue
                End Get
            End Property

            Public Overridable ReadOnly Property Key As Pair(Of K, T)
                Get
                    Return New Pair(Of K, T)(firstKeyValue, secondKeyValue)
                End Get
            End Property

            Public Function getFirstKey() As K
                Return firstKeyValue
            End Function

            Public Function getSecondKey() As T
                Return secondKeyValue
            End Function

            Public Function getValue() As V
                Return entryValue
            End Function

            Public Function getKey() As Pair(Of K, T)
                Return Key
            End Function

            ''' <summary>Replaces the value and returns the old value.</summary>
            Public Overridable Function setValue(ByVal value As V) As V
                Dim old As V = entryValue
                entryValue = value
                Return old
            End Function

            Public Overrides Function Equals(ByVal o As Object) As Boolean
                Dim other = TryCast(o, Entry)
                If other Is Nothing Then Return False
                Return Object.Equals(firstKeyValue, other.firstKeyValue) AndAlso Object.Equals(secondKeyValue, other.secondKeyValue) AndAlso Object.Equals(entryValue, other.entryValue)
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return Objects.hash(firstKeyValue, secondKeyValue, entryValue)
            End Function

            Public Overrides Function ToString() As String
                Return "Entry{" & Arrays.FormatElement(firstKeyValue) & ", " & Arrays.FormatElement(secondKeyValue) & " -> " & Arrays.FormatElement(entryValue) & "}"
            End Function
        End Class

    End Class

End Namespace
