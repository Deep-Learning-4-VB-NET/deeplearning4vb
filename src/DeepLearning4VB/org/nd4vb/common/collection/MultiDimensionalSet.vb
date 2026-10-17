Imports System
Imports System.Collections
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
    ''' A set of pairs (K, V) with convenience overloads that take the two components separately.
    ''' </summary>
    <Serializable>
    Public Class MultiDimensionalSet(Of K, V)
        Implements ISet(Of Pair(Of K, V))

        Private ReadOnly backedSet As ISet(Of Pair(Of K, V))

        Public Sub New(ByVal backedSet As ISet(Of Pair(Of K, V)))
            If backedSet Is Nothing Then Throw New ArgumentNullException(NameOf(backedSet))
            Me.backedSet = backedSet
        End Sub

        Public Sub New()
            Me.New(New HashSet(Of Pair(Of K, V))())
        End Sub

        Public Shared Function hashSet() As MultiDimensionalSet(Of K, V)
            Return New MultiDimensionalSet(Of K, V)(New HashSet(Of Pair(Of K, V))())
        End Function

        ''' <summary>A sorted set (K and V must be comparable).</summary>
        Public Shared Function treeSet() As MultiDimensionalSet(Of K, V)
            Return New MultiDimensionalSet(Of K, V)(New SortedSet(Of Pair(Of K, V))(New PairComparer(Of K, V)()))
        End Function

        ''' <summary>A thread-safe sorted set (K and V must be comparable).</summary>
        Public Shared Function concurrentSkipListSet() As MultiDimensionalSet(Of K, V)
            Return New MultiDimensionalSet(Of K, V)(New SynchronizedSet(Of Pair(Of K, V))(New SortedSet(Of Pair(Of K, V))(New PairComparer(Of K, V)())))
        End Function

        ' ---- Java-style API ----

        Public Overridable Function size() As Integer
            Return backedSet.Count
        End Function

        Public Overridable ReadOnly Property Empty As Boolean
            Get
                Return backedSet.Count = 0
            End Get
        End Property

        Public Function isEmpty() As Boolean
            Return Empty
        End Function

        Public Overridable Function contains(ByVal k As K, ByVal v As V) As Boolean
            Return backedSet.Contains(New Pair(Of K, V)(k, v))
        End Function

        Public Overridable Function add(ByVal k As K, ByVal v As V) As Boolean
            Return backedSet.Add(New Pair(Of K, V)(k, v))
        End Function

        Public Overridable Function remove(ByVal k As K, ByVal v As V) As Boolean
            Return backedSet.Remove(New Pair(Of K, V)(k, v))
        End Function

        Public Overridable Function addAll(ByVal c As IEnumerable(Of Pair(Of K, V))) As Boolean
            Dim changed As Boolean = False
            For Each p In c
                changed = backedSet.Add(p) OrElse changed
            Next
            Return changed
        End Function

        Public Overridable Function containsAll(ByVal c As IEnumerable(Of Pair(Of K, V))) As Boolean
            For Each p In c
                If Not backedSet.Contains(p) Then Return False
            Next
            Return True
        End Function

        Public Overridable Function removeAll(ByVal c As IEnumerable(Of Pair(Of K, V))) As Boolean
            Dim changed As Boolean = False
            For Each p In New List(Of Pair(Of K, V))(c)
                changed = backedSet.Remove(p) OrElse changed
            Next
            Return changed
        End Function

        Public Overridable Function retainAll(ByVal c As IEnumerable(Of Pair(Of K, V))) As Boolean
            Dim before As Integer = backedSet.Count
            backedSet.IntersectWith(c)
            Return backedSet.Count <> before
        End Function

        Public Overridable Function toArray() As Pair(Of K, V)()
            Dim ret(backedSet.Count - 1) As Pair(Of K, V)
            backedSet.CopyTo(ret, 0)
            Return ret
        End Function

        ' ---- ISet(Of Pair(Of K, V)) ----

        Public ReadOnly Property Count As Integer Implements ICollection(Of Pair(Of K, V)).Count
            Get
                Return backedSet.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of Pair(Of K, V)).IsReadOnly
            Get
                Return backedSet.IsReadOnly
            End Get
        End Property

        Public Function Add(ByVal item As Pair(Of K, V)) As Boolean Implements ISet(Of Pair(Of K, V)).Add
            Return backedSet.Add(item)
        End Function

        Private Sub AddCollection(ByVal item As Pair(Of K, V)) Implements ICollection(Of Pair(Of K, V)).Add
            backedSet.Add(item)
        End Sub

        Public Sub Clear() Implements ICollection(Of Pair(Of K, V)).Clear
            backedSet.Clear()
        End Sub

        Public Function Contains(ByVal item As Pair(Of K, V)) As Boolean Implements ICollection(Of Pair(Of K, V)).Contains
            Return backedSet.Contains(item)
        End Function

        Public Sub CopyTo(ByVal array() As Pair(Of K, V), ByVal arrayIndex As Integer) Implements ICollection(Of Pair(Of K, V)).CopyTo
            backedSet.CopyTo(array, arrayIndex)
        End Sub

        Public Function Remove(ByVal item As Pair(Of K, V)) As Boolean Implements ICollection(Of Pair(Of K, V)).Remove
            Return backedSet.Remove(item)
        End Function

        Public Function GetEnumerator() As IEnumerator(Of Pair(Of K, V)) Implements IEnumerable(Of Pair(Of K, V)).GetEnumerator
            Return backedSet.GetEnumerator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return backedSet.GetEnumerator()
        End Function

        Public Sub UnionWith(ByVal other As IEnumerable(Of Pair(Of K, V))) Implements ISet(Of Pair(Of K, V)).UnionWith
            backedSet.UnionWith(other)
        End Sub

        Public Sub IntersectWith(ByVal other As IEnumerable(Of Pair(Of K, V))) Implements ISet(Of Pair(Of K, V)).IntersectWith
            backedSet.IntersectWith(other)
        End Sub

        Public Sub ExceptWith(ByVal other As IEnumerable(Of Pair(Of K, V))) Implements ISet(Of Pair(Of K, V)).ExceptWith
            backedSet.ExceptWith(other)
        End Sub

        Public Sub SymmetricExceptWith(ByVal other As IEnumerable(Of Pair(Of K, V))) Implements ISet(Of Pair(Of K, V)).SymmetricExceptWith
            backedSet.SymmetricExceptWith(other)
        End Sub

        Public Function IsSubsetOf(ByVal other As IEnumerable(Of Pair(Of K, V))) As Boolean Implements ISet(Of Pair(Of K, V)).IsSubsetOf
            Return backedSet.IsSubsetOf(other)
        End Function

        Public Function IsSupersetOf(ByVal other As IEnumerable(Of Pair(Of K, V))) As Boolean Implements ISet(Of Pair(Of K, V)).IsSupersetOf
            Return backedSet.IsSupersetOf(other)
        End Function

        Public Function IsProperSubsetOf(ByVal other As IEnumerable(Of Pair(Of K, V))) As Boolean Implements ISet(Of Pair(Of K, V)).IsProperSubsetOf
            Return backedSet.IsProperSubsetOf(other)
        End Function

        Public Function IsProperSupersetOf(ByVal other As IEnumerable(Of Pair(Of K, V))) As Boolean Implements ISet(Of Pair(Of K, V)).IsProperSupersetOf
            Return backedSet.IsProperSupersetOf(other)
        End Function

        Public Function Overlaps(ByVal other As IEnumerable(Of Pair(Of K, V))) As Boolean Implements ISet(Of Pair(Of K, V)).Overlaps
            Return backedSet.Overlaps(other)
        End Function

        Public Function SetEquals(ByVal other As IEnumerable(Of Pair(Of K, V))) As Boolean Implements ISet(Of Pair(Of K, V)).SetEquals
            Return backedSet.SetEquals(other)
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If Me Is o Then Return True
            Dim other = TryCast(o, MultiDimensionalSet(Of K, V))
            If other Is Nothing Then Return False
            Return backedSet.SetEquals(other.backedSet)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 0
            For Each p In backedSet
                h = h Xor Objects.hashCode(p)
            Next
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "MultiDimensionalSet" & Arrays.toString(toArray())
        End Function
    End Class

End Namespace
