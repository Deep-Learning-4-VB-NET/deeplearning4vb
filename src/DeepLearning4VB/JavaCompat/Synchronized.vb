Imports System
Imports System.Collections
Imports System.Collections.Generic

Namespace JavaCompat

    ''' <summary>
    ''' A dictionary wrapper that serialises every operation on a lock, like
    ''' <c>Collections.synchronizedMap</c>. Enumeration works on a snapshot.
    ''' </summary>
    <Serializable>
    Public Class SynchronizedDictionary(Of K, V)
        Implements IDictionary(Of K, V)

        Private ReadOnly inner As IDictionary(Of K, V)
        Private ReadOnly gate As New Object()

        Public Sub New(ByVal inner As IDictionary(Of K, V))
            If inner Is Nothing Then Throw New ArgumentNullException(NameOf(inner))
            Me.inner = inner
        End Sub

        Default Public Property Item(ByVal key As K) As V Implements IDictionary(Of K, V).Item
            Get
                SyncLock gate
                    Return inner(key)
                End SyncLock
            End Get
            Set(ByVal value As V)
                SyncLock gate
                    inner(key) = value
                End SyncLock
            End Set
        End Property

        Public ReadOnly Property Keys As ICollection(Of K) Implements IDictionary(Of K, V).Keys
            Get
                SyncLock gate
                    Return New List(Of K)(inner.Keys)
                End SyncLock
            End Get
        End Property

        Public ReadOnly Property Values As ICollection(Of V) Implements IDictionary(Of K, V).Values
            Get
                SyncLock gate
                    Return New List(Of V)(inner.Values)
                End SyncLock
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of K, V)).Count
            Get
                SyncLock gate
                    Return inner.Count
                End SyncLock
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).IsReadOnly
            Get
                Return inner.IsReadOnly
            End Get
        End Property

        Public Function ContainsKey(ByVal key As K) As Boolean Implements IDictionary(Of K, V).ContainsKey
            SyncLock gate
                Return inner.ContainsKey(key)
            End SyncLock
        End Function

        Public Sub Add(ByVal key As K, ByVal value As V) Implements IDictionary(Of K, V).Add
            SyncLock gate
                inner.Add(key, value)
            End SyncLock
        End Sub

        Public Function Remove(ByVal key As K) As Boolean Implements IDictionary(Of K, V).Remove
            SyncLock gate
                Return inner.Remove(key)
            End SyncLock
        End Function

        Public Function TryGetValue(ByVal key As K, ByRef value As V) As Boolean Implements IDictionary(Of K, V).TryGetValue
            SyncLock gate
                Return inner.TryGetValue(key, value)
            End SyncLock
        End Function

        Private Sub AddPair(ByVal item As KeyValuePair(Of K, V)) Implements ICollection(Of KeyValuePair(Of K, V)).Add
            SyncLock gate
                inner.Add(item)
            End SyncLock
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of K, V)).Clear
            SyncLock gate
                inner.Clear()
            End SyncLock
        End Sub

        Private Function ContainsPair(ByVal item As KeyValuePair(Of K, V)) As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).Contains
            SyncLock gate
                Return inner.Contains(item)
            End SyncLock
        End Function

        Private Sub CopyTo(ByVal array() As KeyValuePair(Of K, V), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of K, V)).CopyTo
            SyncLock gate
                inner.CopyTo(array, arrayIndex)
            End SyncLock
        End Sub

        Private Function RemovePair(ByVal item As KeyValuePair(Of K, V)) As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).Remove
            SyncLock gate
                Return inner.Remove(item)
            End SyncLock
        End Function

        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of K, V)) Implements IEnumerable(Of KeyValuePair(Of K, V)).GetEnumerator
            SyncLock gate
                Return New List(Of KeyValuePair(Of K, V))(inner).GetEnumerator()
            End SyncLock
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function
    End Class

    ''' <summary>
    ''' A set wrapper that serialises every operation on a lock, like
    ''' <c>Collections.synchronizedSet</c>. Enumeration works on a snapshot.
    ''' </summary>
    <Serializable>
    Public Class SynchronizedSet(Of T)
        Implements ISet(Of T)

        Private ReadOnly inner As ISet(Of T)
        Private ReadOnly gate As New Object()

        Public Sub New(ByVal inner As ISet(Of T))
            If inner Is Nothing Then Throw New ArgumentNullException(NameOf(inner))
            Me.inner = inner
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of T).Count
            Get
                SyncLock gate
                    Return inner.Count
                End SyncLock
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of T).IsReadOnly
            Get
                Return inner.IsReadOnly
            End Get
        End Property

        Public Function Add(ByVal item As T) As Boolean Implements ISet(Of T).Add
            SyncLock gate
                Return inner.Add(item)
            End SyncLock
        End Function

        Private Sub AddCollection(ByVal item As T) Implements ICollection(Of T).Add
            Add(item)
        End Sub

        Public Sub Clear() Implements ICollection(Of T).Clear
            SyncLock gate
                inner.Clear()
            End SyncLock
        End Sub

        Public Function Contains(ByVal item As T) As Boolean Implements ICollection(Of T).Contains
            SyncLock gate
                Return inner.Contains(item)
            End SyncLock
        End Function

        Public Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements ICollection(Of T).CopyTo
            SyncLock gate
                inner.CopyTo(array, arrayIndex)
            End SyncLock
        End Sub

        Public Function Remove(ByVal item As T) As Boolean Implements ICollection(Of T).Remove
            SyncLock gate
                Return inner.Remove(item)
            End SyncLock
        End Function

        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            SyncLock gate
                Return New List(Of T)(inner).GetEnumerator()
            End SyncLock
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Sub UnionWith(ByVal other As IEnumerable(Of T)) Implements ISet(Of T).UnionWith
            SyncLock gate
                inner.UnionWith(other)
            End SyncLock
        End Sub

        Public Sub IntersectWith(ByVal other As IEnumerable(Of T)) Implements ISet(Of T).IntersectWith
            SyncLock gate
                inner.IntersectWith(other)
            End SyncLock
        End Sub

        Public Sub ExceptWith(ByVal other As IEnumerable(Of T)) Implements ISet(Of T).ExceptWith
            SyncLock gate
                inner.ExceptWith(other)
            End SyncLock
        End Sub

        Public Sub SymmetricExceptWith(ByVal other As IEnumerable(Of T)) Implements ISet(Of T).SymmetricExceptWith
            SyncLock gate
                inner.SymmetricExceptWith(other)
            End SyncLock
        End Sub

        Public Function IsSubsetOf(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).IsSubsetOf
            SyncLock gate
                Return inner.IsSubsetOf(other)
            End SyncLock
        End Function

        Public Function IsSupersetOf(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).IsSupersetOf
            SyncLock gate
                Return inner.IsSupersetOf(other)
            End SyncLock
        End Function

        Public Function IsProperSubsetOf(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).IsProperSubsetOf
            SyncLock gate
                Return inner.IsProperSubsetOf(other)
            End SyncLock
        End Function

        Public Function IsProperSupersetOf(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).IsProperSupersetOf
            SyncLock gate
                Return inner.IsProperSupersetOf(other)
            End SyncLock
        End Function

        Public Function Overlaps(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).Overlaps
            SyncLock gate
                Return inner.Overlaps(other)
            End SyncLock
        End Function

        Public Function SetEquals(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).SetEquals
            SyncLock gate
                Return inner.SetEquals(other)
            End SyncLock
        End Function
    End Class

End Namespace
