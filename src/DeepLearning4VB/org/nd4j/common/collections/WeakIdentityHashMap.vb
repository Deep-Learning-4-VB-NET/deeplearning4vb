Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

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

Namespace org.nd4j.common.collections

    ''' <summary>
    ''' A dictionary whose keys are compared by reference identity and held weakly: once a key is
    ''' garbage collected its entry disappears. Java used a ReferenceQueue to learn about collected
    ''' keys; this port sweeps dead entries on every operation instead.
    ''' </summary>
    ''' <typeparam name="K">Key type (must be a reference type).</typeparam>
    Public Class WeakIdentityHashMap(Of K As Class, V)
        Implements IDictionary(Of K, V)

        Protected Friend Class KeyRef
            Friend ReadOnly Reference As WeakReference
            Friend ReadOnly Hash As Integer

            Public Sub New(ByVal referent As K)
                If referent Is Nothing Then Throw New ArgumentNullException(NameOf(referent))
                Reference = New WeakReference(referent)
                Hash = RuntimeHelpers.GetHashCode(referent)
            End Sub

            Public Function [get]() As K
                Return TryCast(Reference.Target, K)
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return Hash
            End Function

            Public Overrides Function Equals(ByVal o As Object) As Boolean
                If Me Is o Then Return True
                Dim other = TryCast(o, KeyRef)
                If other Is Nothing Then Return False
                Dim a As K = Me.get()
                Dim b As K = other.get()
                Return a IsNot Nothing AndAlso a Is b
            End Function
        End Class

        Protected Friend ReadOnly map As New Dictionary(Of KeyRef, V)()

        Public Sub New()
        End Sub

        ''' <summary>Removes the entries whose keys have been garbage collected.</summary>
        Protected Friend Overridable Sub clearReferences()
            Dim dead As List(Of KeyRef) = Nothing
            For Each k As KeyRef In map.Keys
                If k.get() Is Nothing Then
                    If dead Is Nothing Then dead = New List(Of KeyRef)()
                    dead.Add(k)
                End If
            Next k
            If dead IsNot Nothing Then
                For Each k In dead
                    map.Remove(k)
                Next k
            End If
        End Sub

        ' ---- Java-style API ----

        Public Overridable Function size() As Integer
            clearReferences()
            Return map.Count
        End Function

        Public Overridable Function isEmpty() As Boolean
            Return size() = 0
        End Function

        Public Overridable Function [get](ByVal key As K) As V
            clearReferences()
            Dim v As V = Nothing
            If key IsNot Nothing Then map.TryGetValue(New KeyRef(key), v)
            Return v
        End Function

        Public Overridable Function put(ByVal key As K, ByVal value As V) As V
            clearReferences()
            Dim previous As V = Nothing
            Dim kr As New KeyRef(key)
            map.TryGetValue(kr, previous)
            map(kr) = value
            Return previous
        End Function

        Public Overridable Function removeAndGet(ByVal key As K) As V
            clearReferences()
            Dim previous As V = Nothing
            Dim kr As New KeyRef(key)
            If map.TryGetValue(kr, previous) Then map.Remove(kr)
            Return previous
        End Function

        Public Overridable Sub putAll(ByVal m As IDictionary(Of K, V))
            For Each e As KeyValuePair(Of K, V) In m
                put(e.Key, e.Value)
            Next e
        End Sub

        Public Overridable Function containsValue(ByVal value As V) As Boolean
            clearReferences()
            Return map.ContainsValue(value)
        End Function

        ''' <summary>A snapshot of the live keys.</summary>
        Public Overridable Function keySet() As ISet(Of K)
            clearReferences()
            Dim ret As New HashSet(Of K)()
            For Each k As KeyRef In map.Keys
                Dim key As K = k.get()
                If key IsNot Nothing Then ret.Add(key)
            Next k
            Return ret
        End Function

        ''' <summary>A snapshot of the live entries.</summary>
        Public Overridable Function entrySet() As ISet(Of KeyValuePair(Of K, V))
            Dim ret As New HashSet(Of KeyValuePair(Of K, V))()
            For Each e In Snapshot()
                ret.Add(e)
            Next e
            Return ret
        End Function

        Private Function Snapshot() As List(Of KeyValuePair(Of K, V))
            clearReferences()
            Dim ret As New List(Of KeyValuePair(Of K, V))(map.Count)
            For Each e As KeyValuePair(Of KeyRef, V) In map
                Dim k As K = e.Key.get()
                If k IsNot Nothing Then ret.Add(New KeyValuePair(Of K, V)(k, e.Value))
            Next e
            Return ret
        End Function

        ' ---- IDictionary(Of K, V) ----

        Default Public Property Item(ByVal key As K) As V Implements IDictionary(Of K, V).Item
            Get
                clearReferences()
                Return map(New KeyRef(key))
            End Get
            Set(ByVal value As V)
                put(key, value)
            End Set
        End Property

        Public ReadOnly Property Keys As ICollection(Of K) Implements IDictionary(Of K, V).Keys
            Get
                Return New List(Of K)(keySet())
            End Get
        End Property

        Public ReadOnly Property Values As ICollection(Of V) Implements IDictionary(Of K, V).Values
            Get
                Dim ret As New List(Of V)()
                For Each e In Snapshot()
                    ret.Add(e.Value)
                Next e
                Return ret
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of K, V)).Count
            Get
                Return size()
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function ContainsKey(ByVal key As K) As Boolean Implements IDictionary(Of K, V).ContainsKey
            clearReferences()
            Return key IsNot Nothing AndAlso map.ContainsKey(New KeyRef(key))
        End Function

        Public Sub Add(ByVal key As K, ByVal value As V) Implements IDictionary(Of K, V).Add
            If ContainsKey(key) Then Throw New ArgumentException("An item with the same key has already been added.")
            put(key, value)
        End Sub

        Public Function Remove(ByVal key As K) As Boolean Implements IDictionary(Of K, V).Remove
            clearReferences()
            Return key IsNot Nothing AndAlso map.Remove(New KeyRef(key))
        End Function

        Public Function TryGetValue(ByVal key As K, ByRef value As V) As Boolean Implements IDictionary(Of K, V).TryGetValue
            clearReferences()
            If key Is Nothing Then
                value = Nothing
                Return False
            End If
            Return map.TryGetValue(New KeyRef(key), value)
        End Function

        Private Sub AddPair(ByVal item As KeyValuePair(Of K, V)) Implements ICollection(Of KeyValuePair(Of K, V)).Add
            Add(item.Key, item.Value)
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of K, V)).Clear
            map.Clear()
        End Sub

        Private Function ContainsPair(ByVal item As KeyValuePair(Of K, V)) As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).Contains
            Dim v As V = Nothing
            Return TryGetValue(item.Key, v) AndAlso EqualityComparer(Of V).Default.Equals(v, item.Value)
        End Function

        Private Sub CopyTo(ByVal array() As KeyValuePair(Of K, V), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of K, V)).CopyTo
            For Each e In Snapshot()
                array(arrayIndex) = e
                arrayIndex += 1
            Next e
        End Sub

        Private Function RemovePair(ByVal item As KeyValuePair(Of K, V)) As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).Remove
            If ContainsPair(item) Then Return Remove(item.Key)
            Return False
        End Function

        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of K, V)) Implements IEnumerable(Of KeyValuePair(Of K, V)).GetEnumerator
            Return Snapshot().GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class

End Namespace
