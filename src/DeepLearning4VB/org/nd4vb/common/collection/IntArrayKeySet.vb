Imports System.Collections
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

Namespace org.nd4vb.common.collection

    ''' <summary>
    ''' An insertion-ordered set of integer arrays that compares arrays by content
    ''' rather than by reference (the .NET default for arrays).
    ''' Ported from the Java <c>IntArrayKeySet</c>; the Java set interface has been
    ''' replaced by <see cref="ISet(Of T)"/>.
    ''' </summary>
    Public Class IntArrayKeySet
        Implements ISet(Of Integer())

        Private ReadOnly lookup As New HashSet(Of IntArray)()
        Private ReadOnly order As New List(Of IntArray)()

        Private Shared Function Wrap(ByVal ints() As Integer) As IntArray
            Return New IntArray(ints)
        End Function

        Public ReadOnly Property Count As Integer Implements ICollection(Of Integer()).Count
            Get
                Return lookup.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of Integer()).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return lookup.Count = 0
            End Get
        End Property

        Public Function Add(ByVal ints() As Integer) As Boolean Implements ISet(Of Integer()).Add
            Dim w = Wrap(ints)
            If lookup.Add(w) Then
                order.Add(w)
                Return True
            End If
            Return False
        End Function

        Private Sub AddCollection(ByVal ints() As Integer) Implements ICollection(Of Integer()).Add
            Add(ints)
        End Sub

        Public Function Contains(ByVal ints() As Integer) As Boolean Implements ICollection(Of Integer()).Contains
            Return lookup.Contains(Wrap(ints))
        End Function

        Public Function Remove(ByVal ints() As Integer) As Boolean Implements ICollection(Of Integer()).Remove
            Dim w = Wrap(ints)
            If lookup.Remove(w) Then
                order.Remove(w)
                Return True
            End If
            Return False
        End Function

        Public Sub Clear() Implements ICollection(Of Integer()).Clear
            lookup.Clear()
            order.Clear()
        End Sub

        Public Sub CopyTo(ByVal array As Integer()(), ByVal arrayIndex As Integer) Implements ICollection(Of Integer()).CopyTo
            For Each w In order
                array(arrayIndex) = w.getBackingArray()
                arrayIndex += 1
            Next
        End Sub

        ''' <summary>Returns the contents of the set, in insertion order.</summary>
        Public Function ToArray() As Integer()()
            Dim ret As Integer()() = New Integer(order.Count - 1)() {}
            For i As Integer = 0 To order.Count - 1
                ret(i) = order(i).getBackingArray()
            Next
            Return ret
        End Function

        Public Function GetEnumerator() As IEnumerator(Of Integer()) Implements IEnumerable(Of Integer()).GetEnumerator
            Return order.Select(Function(w) w.getBackingArray()).GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Sub UnionWith(ByVal other As IEnumerable(Of Integer())) Implements ISet(Of Integer()).UnionWith
            For Each arr In other
                Add(arr)
            Next
        End Sub

        Public Sub IntersectWith(ByVal other As IEnumerable(Of Integer())) Implements ISet(Of Integer()).IntersectWith
            Dim keep As New HashSet(Of IntArray)(other.Select(AddressOf Wrap))
            For Each w In order.ToArray()
                If Not keep.Contains(w) Then
                    lookup.Remove(w)
                    order.Remove(w)
                End If
            Next
        End Sub

        Public Sub ExceptWith(ByVal other As IEnumerable(Of Integer())) Implements ISet(Of Integer()).ExceptWith
            For Each arr In other
                Remove(arr)
            Next
        End Sub

        Public Sub SymmetricExceptWith(ByVal other As IEnumerable(Of Integer())) Implements ISet(Of Integer()).SymmetricExceptWith
            For Each arr In other.ToArray()
                If Not Remove(arr) Then
                    Add(arr)
                End If
            Next
        End Sub

        Public Function IsSubsetOf(ByVal other As IEnumerable(Of Integer())) As Boolean Implements ISet(Of Integer()).IsSubsetOf
            Return lookup.IsSubsetOf(other.Select(AddressOf Wrap))
        End Function

        Public Function IsSupersetOf(ByVal other As IEnumerable(Of Integer())) As Boolean Implements ISet(Of Integer()).IsSupersetOf
            Return lookup.IsSupersetOf(other.Select(AddressOf Wrap))
        End Function

        Public Function IsProperSubsetOf(ByVal other As IEnumerable(Of Integer())) As Boolean Implements ISet(Of Integer()).IsProperSubsetOf
            Return lookup.IsProperSubsetOf(other.Select(AddressOf Wrap))
        End Function

        Public Function IsProperSupersetOf(ByVal other As IEnumerable(Of Integer())) As Boolean Implements ISet(Of Integer()).IsProperSupersetOf
            Return lookup.IsProperSupersetOf(other.Select(AddressOf Wrap))
        End Function

        Public Function Overlaps(ByVal other As IEnumerable(Of Integer())) As Boolean Implements ISet(Of Integer()).Overlaps
            Return lookup.Overlaps(other.Select(AddressOf Wrap))
        End Function

        Public Function SetEquals(ByVal other As IEnumerable(Of Integer())) As Boolean Implements ISet(Of Integer()).SetEquals
            Return lookup.SetEquals(other.Select(AddressOf Wrap))
        End Function

    End Class

End Namespace
