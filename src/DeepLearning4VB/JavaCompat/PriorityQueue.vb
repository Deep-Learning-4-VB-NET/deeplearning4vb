Imports System
Imports System.Collections
Imports System.Collections.Generic

Namespace JavaCompat

    ''' <summary>
    ''' A binary-heap priority queue with the <c>java.util.PriorityQueue</c> API used by the
    ''' ported code (<c>add</c>, <c>poll</c>, <c>peek</c>, <c>isEmpty</c>, <c>size</c>).
    ''' The head of the queue is the least element according to the comparer.
    ''' .NET Framework has no built-in priority queue.
    ''' </summary>
    Public Class PriorityQueue(Of T)
        Implements IEnumerable(Of T)

        Private ReadOnly heap As List(Of T)
        Private ReadOnly comparer As IComparer(Of T)

        Public Sub New()
            Me.New(11, Nothing)
        End Sub

        Public Sub New(ByVal initialCapacity As Integer)
            Me.New(initialCapacity, Nothing)
        End Sub

        Public Sub New(ByVal comparer As IComparer(Of T))
            Me.New(11, comparer)
        End Sub

        Public Sub New(ByVal initialCapacity As Integer, ByVal comparer As IComparer(Of T))
            If initialCapacity < 1 Then
                Throw New ArgumentException("Initial capacity must be at least 1", NameOf(initialCapacity))
            End If
            heap = New List(Of T)(initialCapacity)
            Me.comparer = If(comparer, Global.System.Collections.Generic.Comparer(Of T).Default)
        End Sub

        Public Sub New(ByVal items As IEnumerable(Of T))
            Me.New(11, Nothing)
            For Each item In items
                add(item)
            Next
        End Sub

        Public ReadOnly Property Count As Integer
            Get
                Return heap.Count
            End Get
        End Property

        Public Function size() As Integer
            Return heap.Count
        End Function

        Public Function isEmpty() As Boolean
            Return heap.Count = 0
        End Function

        ''' <summary>Inserts the element. Always returns true (as in Java).</summary>
        Public Function add(ByVal item As T) As Boolean
            If item Is Nothing Then
                Throw New ArgumentNullException(NameOf(item))
            End If
            heap.Add(item)
            SiftUp(heap.Count - 1)
            Return True
        End Function

        Public Function offer(ByVal item As T) As Boolean
            Return add(item)
        End Function

        ''' <summary>Returns the head of the queue without removing it, or the default value when empty.</summary>
        Public Function peek() As T
            If heap.Count = 0 Then
                Return Nothing
            End If
            Return heap(0)
        End Function

        ''' <summary>Removes and returns the head of the queue, or the default value when empty.</summary>
        Public Function poll() As T
            If heap.Count = 0 Then
                Return Nothing
            End If
            Dim result As T = heap(0)
            Dim last As Integer = heap.Count - 1
            heap(0) = heap(last)
            heap.RemoveAt(last)
            If heap.Count > 0 Then
                SiftDown(0)
            End If
            Return result
        End Function

        ''' <summary>Removes and returns the head of the queue; throws when empty.</summary>
        Public Function remove() As T
            If heap.Count = 0 Then
                Throw New InvalidOperationException("Queue is empty")
            End If
            Return poll()
        End Function

        Public Sub clear()
            heap.Clear()
        End Sub

        Public Function contains(ByVal item As T) As Boolean
            Return heap.Contains(item)
        End Function

        ''' <summary>Returns the elements in heap (not sorted) order, as Java's toArray does.</summary>
        Public Function toArray() As T()
            Return heap.ToArray()
        End Function

        Private Sub SiftUp(ByVal index As Integer)
            Dim item As T = heap(index)
            While index > 0
                Dim parent As Integer = (index - 1) \ 2
                If comparer.Compare(item, heap(parent)) >= 0 Then
                    Exit While
                End If
                heap(index) = heap(parent)
                index = parent
            End While
            heap(index) = item
        End Sub

        Private Sub SiftDown(ByVal index As Integer)
            Dim item As T = heap(index)
            Dim count As Integer = heap.Count
            Dim half As Integer = count \ 2
            While index < half
                Dim child As Integer = 2 * index + 1
                Dim right As Integer = child + 1
                If right < count AndAlso comparer.Compare(heap(right), heap(child)) < 0 Then
                    child = right
                End If
                If comparer.Compare(item, heap(child)) <= 0 Then
                    Exit While
                End If
                heap(index) = heap(child)
                index = child
            End While
            heap(index) = item
        End Sub

        Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            Return heap.GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return heap.GetEnumerator()
        End Function
    End Class

End Namespace
