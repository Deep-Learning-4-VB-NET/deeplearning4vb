Imports System
Imports System.Collections
Imports System.Collections.Generic

Namespace JavaCompat

    ''' <summary>
    ''' A dictionary that enumerates its entries in insertion order, equivalent to
    ''' <c>java.util.LinkedHashMap</c>. Removal is O(1). Null keys are not supported.
    ''' Java-style accessors (<c>put</c>, <c>get</c>, <c>removeAndGet</c>) are provided
    ''' alongside the <see cref="IDictionary(Of TKey, TValue)"/> members.
    ''' </summary>
    <Serializable>
    Public Class LinkedHashMap(Of K, V)
        Implements IDictionary(Of K, V)

        Private ReadOnly nodes As Dictionary(Of K, LinkedListNode(Of KeyValuePair(Of K, V)))
        Private ReadOnly order As New LinkedList(Of KeyValuePair(Of K, V))()

        Public Sub New()
            nodes = New Dictionary(Of K, LinkedListNode(Of KeyValuePair(Of K, V)))()
        End Sub

        Public Sub New(ByVal initialCapacity As Integer)
            nodes = New Dictionary(Of K, LinkedListNode(Of KeyValuePair(Of K, V)))(Math.Max(0, initialCapacity))
        End Sub

        Public Sub New(ByVal comparer As IEqualityComparer(Of K))
            nodes = New Dictionary(Of K, LinkedListNode(Of KeyValuePair(Of K, V)))(comparer)
        End Sub

        Public Sub New(ByVal other As IDictionary(Of K, V))
            Me.New()
            If other IsNot Nothing Then
                For Each kv In other
                    Me(kv.Key) = kv.Value
                Next
            End If
        End Sub

        Default Public Property Item(ByVal key As K) As V Implements IDictionary(Of K, V).Item
            Get
                Dim node As LinkedListNode(Of KeyValuePair(Of K, V)) = Nothing
                If nodes.TryGetValue(key, node) Then
                    Return node.Value.Value
                End If
                Throw New KeyNotFoundException("Key not found: " & Arrays.FormatElement(key))
            End Get
            Set(ByVal value As V)
                put(key, value)
            End Set
        End Property

        ''' <summary>Associates the value with the key and returns the previous value, or the default if none.</summary>
        Public Function put(ByVal key As K, ByVal value As V) As V
            Dim node As LinkedListNode(Of KeyValuePair(Of K, V)) = Nothing
            If nodes.TryGetValue(key, node) Then
                Dim previous As V = node.Value.Value
                node.Value = New KeyValuePair(Of K, V)(key, value)
                Return previous
            End If
            node = order.AddLast(New KeyValuePair(Of K, V)(key, value))
            nodes(key) = node
            Return Nothing
        End Function

        ''' <summary>Returns the value for the key, or the default value if it is absent.</summary>
        Public Function [get](ByVal key As K) As V
            Dim node As LinkedListNode(Of KeyValuePair(Of K, V)) = Nothing
            If nodes.TryGetValue(key, node) Then
                Return node.Value.Value
            End If
            Return Nothing
        End Function

        ''' <summary>Removes the key and returns its value, or the default value if it was absent.</summary>
        Public Function removeAndGet(ByVal key As K) As V
            Dim node As LinkedListNode(Of KeyValuePair(Of K, V)) = Nothing
            If nodes.TryGetValue(key, node) Then
                nodes.Remove(key)
                order.Remove(node)
                Return node.Value.Value
            End If
            Return Nothing
        End Function

        Public Sub putAll(ByVal other As IDictionary(Of K, V))
            If other Is Nothing Then Throw New ArgumentNullException(NameOf(other))
            For Each kv In other
                put(kv.Key, kv.Value)
            Next
        End Sub

        Public Function size() As Integer
            Return nodes.Count
        End Function

        Public Function isEmpty() As Boolean
            Return nodes.Count = 0
        End Function

        ''' <summary>A snapshot of the keys in insertion order.</summary>
        Public Function keySet() As ISet(Of K)
            Dim result As New LinkedHashSet(Of K)()
            For Each kv In order
                result.Add(kv.Key)
            Next
            Return result
        End Function

        ''' <summary>A snapshot of the entries in insertion order.</summary>
        Public Function entrySet() As ISet(Of KeyValuePair(Of K, V))
            Dim result As New LinkedHashSet(Of KeyValuePair(Of K, V))()
            For Each kv In order
                result.Add(kv)
            Next
            Return result
        End Function

        Public ReadOnly Property Keys As ICollection(Of K) Implements IDictionary(Of K, V).Keys
            Get
                Dim result As New List(Of K)(order.Count)
                For Each kv In order
                    result.Add(kv.Key)
                Next
                Return result
            End Get
        End Property

        ''' <summary>A snapshot of the values in insertion order.</summary>
        Public ReadOnly Property Values As ICollection(Of V) Implements IDictionary(Of K, V).Values
            Get
                Dim result As New List(Of V)(order.Count)
                For Each kv In order
                    result.Add(kv.Value)
                Next
                Return result
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of K, V)).Count
            Get
                Return nodes.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function ContainsKey(ByVal key As K) As Boolean Implements IDictionary(Of K, V).ContainsKey
            Return nodes.ContainsKey(key)
        End Function

        Public Function containsValue(ByVal value As V) As Boolean
            Dim cmp = EqualityComparer(Of V).Default
            For Each kv In order
                If cmp.Equals(kv.Value, value) Then Return True
            Next
            Return False
        End Function

        Public Sub Add(ByVal key As K, ByVal value As V) Implements IDictionary(Of K, V).Add
            If nodes.ContainsKey(key) Then
                Throw New ArgumentException("An item with the same key has already been added.")
            End If
            put(key, value)
        End Sub

        Public Function Remove(ByVal key As K) As Boolean Implements IDictionary(Of K, V).Remove
            Dim node As LinkedListNode(Of KeyValuePair(Of K, V)) = Nothing
            If nodes.TryGetValue(key, node) Then
                nodes.Remove(key)
                order.Remove(node)
                Return True
            End If
            Return False
        End Function

        Public Function TryGetValue(ByVal key As K, ByRef value As V) As Boolean Implements IDictionary(Of K, V).TryGetValue
            Dim node As LinkedListNode(Of KeyValuePair(Of K, V)) = Nothing
            If nodes.TryGetValue(key, node) Then
                value = node.Value.Value
                Return True
            End If
            value = Nothing
            Return False
        End Function

        Private Sub AddPair(ByVal item As KeyValuePair(Of K, V)) Implements ICollection(Of KeyValuePair(Of K, V)).Add
            Add(item.Key, item.Value)
        End Sub

        Public Sub Clear() Implements ICollection(Of KeyValuePair(Of K, V)).Clear
            nodes.Clear()
            order.Clear()
        End Sub

        Private Function ContainsPair(ByVal item As KeyValuePair(Of K, V)) As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).Contains
            Dim node As LinkedListNode(Of KeyValuePair(Of K, V)) = Nothing
            Return nodes.TryGetValue(item.Key, node) AndAlso EqualityComparer(Of V).Default.Equals(node.Value.Value, item.Value)
        End Function

        Private Sub CopyTo(ByVal array() As KeyValuePair(Of K, V), ByVal arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of K, V)).CopyTo
            order.CopyTo(array, arrayIndex)
        End Sub

        Private Function RemovePair(ByVal item As KeyValuePair(Of K, V)) As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).Remove
            If ContainsPair(item) Then
                Return Remove(item.Key)
            End If
            Return False
        End Function

        Public Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of K, V)) Implements IEnumerable(Of KeyValuePair(Of K, V)).GetEnumerator
            Return order.GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return order.GetEnumerator()
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim other = TryCast(obj, IDictionary(Of K, V))
            If other Is Nothing OrElse other.Count <> Count Then Return False
            For Each kv In order
                Dim v As V = Nothing
                If Not other.TryGetValue(kv.Key, v) OrElse Not EqualityComparer(Of V).Default.Equals(v, kv.Value) Then
                    Return False
                End If
            Next
            Return True
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 0
            For Each kv In order
                h = h Xor (Objects.hashCode(kv.Key) Xor Objects.hashCode(kv.Value))
            Next
            Return h
        End Function

        Public Overrides Function ToString() As String
            Dim parts As New List(Of String)(order.Count)
            For Each kv In order
                parts.Add(Arrays.FormatElement(kv.Key) & "=" & Arrays.FormatElement(kv.Value))
            Next
            Return "{" & String.Join(", ", parts) & "}"
        End Function
    End Class

    ''' <summary>
    ''' A set that enumerates its elements in insertion order, equivalent to <c>java.util.LinkedHashSet</c>.
    ''' </summary>
    <Serializable>
    Public Class LinkedHashSet(Of T)
        Implements ISet(Of T)

        Private ReadOnly map As LinkedHashMap(Of T, Boolean)

        Public Sub New()
            map = New LinkedHashMap(Of T, Boolean)()
        End Sub

        Public Sub New(ByVal items As IEnumerable(Of T))
            Me.New()
            If items IsNot Nothing Then
                For Each item In items
                    Add(item)
                Next
            End If
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of T).Count
            Get
                Return map.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of T).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function size() As Integer
            Return map.Count
        End Function

        Public Function isEmpty() As Boolean
            Return map.Count = 0
        End Function

        Public Function Add(ByVal item As T) As Boolean Implements ISet(Of T).Add
            If map.ContainsKey(item) Then Return False
            map.put(item, True)
            Return True
        End Function

        Private Sub AddCollection(ByVal item As T) Implements ICollection(Of T).Add
            Add(item)
        End Sub

        Public Sub Clear() Implements ICollection(Of T).Clear
            map.Clear()
        End Sub

        Public Function Contains(ByVal item As T) As Boolean Implements ICollection(Of T).Contains
            Return map.ContainsKey(item)
        End Function

        Public Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements ICollection(Of T).CopyTo
            For Each item In Me
                array(arrayIndex) = item
                arrayIndex += 1
            Next
        End Sub

        Public Function Remove(ByVal item As T) As Boolean Implements ICollection(Of T).Remove
            Return map.Remove(item)
        End Function

        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Return map.Keys.GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Sub UnionWith(ByVal other As IEnumerable(Of T)) Implements ISet(Of T).UnionWith
            For Each item In other
                Add(item)
            Next
        End Sub

        Public Sub IntersectWith(ByVal other As IEnumerable(Of T)) Implements ISet(Of T).IntersectWith
            Dim keep As New HashSet(Of T)(other)
            For Each item In New List(Of T)(Me)
                If Not keep.Contains(item) Then Remove(item)
            Next
        End Sub

        Public Sub ExceptWith(ByVal other As IEnumerable(Of T)) Implements ISet(Of T).ExceptWith
            For Each item In other
                Remove(item)
            Next
        End Sub

        Public Sub SymmetricExceptWith(ByVal other As IEnumerable(Of T)) Implements ISet(Of T).SymmetricExceptWith
            For Each item In New List(Of T)(other)
                If Not Remove(item) Then Add(item)
            Next
        End Sub

        Private Function AsHashSet() As HashSet(Of T)
            Return New HashSet(Of T)(Me)
        End Function

        Public Function IsSubsetOf(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).IsSubsetOf
            Return AsHashSet().IsSubsetOf(other)
        End Function

        Public Function IsSupersetOf(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).IsSupersetOf
            Return AsHashSet().IsSupersetOf(other)
        End Function

        Public Function IsProperSubsetOf(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).IsProperSubsetOf
            Return AsHashSet().IsProperSubsetOf(other)
        End Function

        Public Function IsProperSupersetOf(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).IsProperSupersetOf
            Return AsHashSet().IsProperSupersetOf(other)
        End Function

        Public Function Overlaps(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).Overlaps
            Return AsHashSet().Overlaps(other)
        End Function

        Public Function SetEquals(ByVal other As IEnumerable(Of T)) As Boolean Implements ISet(Of T).SetEquals
            Return AsHashSet().SetEquals(other)
        End Function

        Public Overrides Function ToString() As String
            Return Arrays.toString(New List(Of T)(Me).ToArray())
        End Function
    End Class

End Namespace
