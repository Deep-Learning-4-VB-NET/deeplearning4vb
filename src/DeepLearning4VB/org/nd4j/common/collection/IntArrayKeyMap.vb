Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports DeepLearning4VB.JavaCompat
Imports Preconditions = DeepLearning4VB.org.nd4j.common.base.Preconditions

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

Namespace org.nd4j.common.collection

    ''' <summary>
    ''' A wrapper that gives an integer array value-based equality and hashing so it can be
    ''' used as a dictionary key. As in the Java original, the backing array is de-duplicated
    ''' (order preserved) when wrapped. In Java this was the nested class
    ''' <c>IntArrayKeyMap.IntArray</c>; VB cannot refer to a nested type of a generic class
    ''' without type arguments, so it is a top-level class here.
    ''' </summary>
    <Serializable>
    Public Class IntArray
        Implements IComparable(Of IntArray)

        Private ReadOnly backing() As Integer

        Public Sub New(ByVal backingArray() As Integer)
            Preconditions.checkNotNull(backingArray, "Backing array must not be null!")
            backing = backingArray.Distinct().ToArray()
        End Sub

        Public ReadOnly Property BackingArray As Integer()
            Get
                Return backing
            End Get
        End Property

        Public Function getBackingArray() As Integer()
            Return backing
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then
                Return True
            End If
            Dim other = TryCast(o, IntArray)
            If other Is Nothing Then
                Return False
            End If
            Return other.backing.SequenceEqual(backing)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Arrays.hashCode(backing)
        End Function

        ''' <summary>
        ''' Ordering used by the Java original: empty arrays and equal arrays compare as 1,
        ''' otherwise arrays are ordered by their maximum element.
        ''' </summary>
        Public Overridable Function CompareTo(ByVal intArray As IntArray) As Integer Implements IComparable(Of IntArray).CompareTo
            If backing.Length = 0 OrElse intArray.backing.Length = 0 Then
                Return 1
            ElseIf backing.SequenceEqual(intArray.backing) Then
                Return 1
            End If
            Return Ints.compare(Ints.max(backing), Ints.max(intArray.backing))
        End Function

        Public Overrides Function ToString() As String
            Return Arrays.toString(backing)
        End Function
    End Class

    ''' <summary>
    ''' A dictionary keyed by integer arrays compared by content (the .NET default for array keys is
    ''' reference equality). Keys are kept in insertion order.
    ''' </summary>
    <Serializable>
    Public Class IntArrayKeyMap(Of V)
        Implements IDictionary(Of Integer(), V)

        Private ReadOnly map As New LinkedHashMap(Of IntArray, V)()

        Private Shared Function Wrap(ByVal key() As Integer) As IntArray
            Return New IntArray(key)
        End Function

        Public Function size() As Integer
            Return map.Count
        End Function

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return map.Count = 0
            End Get
        End Property

        Public Function containsValue(ByVal value As V) As Boolean
            Return map.containsValue(value)
        End Function

        ''' <summary>Returns the value for the key, or the default value if absent.</summary>
        Public Function [get](ByVal key() As Integer) As V
            Return map.[get](Wrap(key))
        End Function

        ''' <summary>Associates the value with the key and returns the previous value (default if none).</summary>
        Public Function put(ByVal key() As Integer, ByVal value As V) As V
            Return map.put(Wrap(key), value)
        End Function

        ''' <summary>Removes the key and returns its value (default if it was absent).</summary>
        Public Function removeAndGet(ByVal key() As Integer) As V
            Return map.removeAndGet(Wrap(key))
        End Function

        Public Sub putAll(ByVal other As IDictionary(Of Integer(), V))
            For Each entry As KeyValuePair(Of Integer(), V) In other
                map.put(Wrap(entry.Key), entry.Value)
            Next entry
        End Sub

        ''' <summary>A snapshot of the keys (as arrays) in insertion order.</summary>
        Public Function keySet() As ISet(Of Integer())
            Dim ret As New IntArrayKeySet()
            For Each k As IntArray In map.Keys
                ret.Add(k.getBackingArray())
            Next k
            Return ret
        End Function

        ''' <summary>A snapshot of the entries in insertion order.</summary>
        Public Function entrySet() As IList(Of KeyValuePair(Of Integer(), V))
            Dim ret As New List(Of KeyValuePair(Of Integer(), V))(map.Count)
            For Each entry As KeyValuePair(Of IntArray, V) In map
                ret.Add(New KeyValuePair(Of Integer(), V)(entry.Key.getBackingArray(), entry.Value))
            Next entry
            Return ret
        End Function

        ' ---- IDictionary(Of Integer(), V) ----

        Default Public Property Item(ByVal key() As Integer) As V Implements IDictionary(Of Integer(), V).Item
            Get
                Return map(Wrap(key))
            End Get
            Set(ByVal value As V)
                map(Wrap(key)) = value
            End Set
        End Property

        Public ReadOnly Property Keys As ICollection(Of Integer()) Implements IDictionary(Of Integer(), V).Keys
            Get
                Return keySet()
            End Get
        End Property

        Public ReadOnly Property Values As ICollection(Of V) Implements IDictionary(Of Integer(), V).Values
            Get
                Return map.Values
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of Integer(), V)).Count
            Get
                Return map.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of Integer(), V)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function ContainsKey(ByVal key() As Integer) As Boolean Implements IDictionary(Of Integer(), V).ContainsKey
            Return map.ContainsKey(Wrap(key))
        End Function

        Public Sub Add(ByVal key() As Integer, ByVal value As V) Implements IDictionary(Of Integer(), V).Add
            map.Add(Wrap(key), value)
        End Sub

        Public Function Remove(ByVal key() As Integer) As Boolean Implements IDictionary(Of Integer(), V).Remove
            Return map.Remove(Wrap(key))
        End Function

        Public Function TryGetValue(ByVal key() As Integer, ByRef value As V) As Boolean Implements IDictionary(Of Integer(), V).TryGetValue
            Return map.TryGetValue(Wrap(key), value)
        End Function

        Private Sub AddPair(ByVal item As KeyValuePair(Of Integer(), V)) Implements ICollection(Of KeyValuePair(Of Integer(), V)).Add
            Add(item.Key, item.Value)
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of Integer(), V)).Clear
            map.Clear()
        End Sub

        Private Function ContainsPair(ByVal item As KeyValuePair(Of Integer(), V)) As Boolean Implements ICollection(Of KeyValuePair(Of Integer(), V)).Contains
            Dim v As V = Nothing
            Return TryGetValue(item.Key, v) AndAlso EqualityComparer(Of V).Default.Equals(v, item.Value)
        End Function

        Private Sub CopyTo(ByVal array() As KeyValuePair(Of Integer(), V), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of Integer(), V)).CopyTo
            For Each entry In entrySet()
                array(arrayIndex) = entry
                arrayIndex += 1
            Next entry
        End Sub

        Private Function RemovePair(ByVal item As KeyValuePair(Of Integer(), V)) As Boolean Implements ICollection(Of KeyValuePair(Of Integer(), V)).Remove
            If ContainsPair(item) Then
                Return Remove(item.Key)
            End If
            Return False
        End Function

        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of Integer(), V)) Implements IEnumerable(Of KeyValuePair(Of Integer(), V)).GetEnumerator
            Return entrySet().GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            Dim other = TryCast(o, IntArrayKeyMap(Of V))
            If other Is Nothing Then
                Return False
            End If
            Return map.Equals(other.map)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return map.GetHashCode()
        End Function

        Public Overrides Function ToString() As String
            Return map.ToString()
        End Function
    End Class

End Namespace
