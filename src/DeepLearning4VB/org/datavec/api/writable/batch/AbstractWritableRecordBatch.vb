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

Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports Writable = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.writable.batch

    ''' <summary>
    ''' A read-only <c>List&lt;List&lt;Writable&gt;&gt;</c> view of a batch of records. Subclasses provide
    ''' <see cref="size"/> and <see cref="[get]"/>; every mutating operation throws
    ''' <see cref="NotSupportedException"/>, and <see cref="Contains"/>/<c>containsAll</c> return false,
    ''' like the Java original. <see cref="CopyTo"/> is implemented (Java's <c>toArray</c> threw) so that
    ''' .NET collection constructors work.
    ''' </summary>
    Public MustInherit Class AbstractWritableRecordBatch
        Implements IList(Of IList(Of Writable))

        ''' <summary>Java <c>size()</c>: the number of records.</summary>
        Public MustOverride Function size() As Integer

        ''' <summary>Java <c>get(int)</c>: the record at the index.</summary>
        Public MustOverride Function [get](ByVal index As Integer) As IList(Of Writable)

        Public Overridable Function isEmpty() As Boolean
            Return size() = 0
        End Function

        Public ReadOnly Property Count As Integer Implements ICollection(Of IList(Of Writable)).Count
            Get
                Return size()
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of IList(Of Writable)).IsReadOnly
            Get
                Return True
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As IList(Of Writable) Implements IList(Of IList(Of Writable)).Item
            Get
                Return [get](index)
            End Get
            Set(ByVal value As IList(Of Writable))
                Throw New NotSupportedException()
            End Set
        End Property

        Public Overridable Function Contains(ByVal item As IList(Of Writable)) As Boolean Implements ICollection(Of IList(Of Writable)).Contains
            Return False
        End Function

        Public Overridable Function containsAll(Of T1)(ByVal collection As ICollection(Of T1)) As Boolean
            Return False
        End Function

        Public Overridable Function GetEnumerator() As IEnumerator(Of IList(Of Writable)) Implements IEnumerable(Of IList(Of Writable)).GetEnumerator
            Return listIterator()
        End Function

        Private Function GetEnumeratorNonGeneric() As IEnumerator Implements IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Overridable Function listIterator() As RecordBatchListIterator
            Return New RecordBatchListIterator(Me)
        End Function

        Public Overridable Function listIterator(ByVal i As Integer) As RecordBatchListIterator
            Throw New NotSupportedException()
        End Function

        Public Overridable Function toArray() As Object()
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub CopyTo(ByVal array() As IList(Of Writable), ByVal arrayIndex As Integer) Implements ICollection(Of IList(Of Writable)).CopyTo
            For i As Integer = 0 To size() - 1
                array(arrayIndex + i) = [get](i)
            Next i
        End Sub

        Public Overridable Sub Add(ByVal item As IList(Of Writable)) Implements ICollection(Of IList(Of Writable)).Add
            Throw New NotSupportedException()
        End Sub

        Public Overridable Function Remove(ByVal item As IList(Of Writable)) As Boolean Implements ICollection(Of IList(Of Writable)).Remove
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub Clear() Implements ICollection(Of IList(Of Writable)).Clear
            ' no-op, like the Java original
        End Sub

        Public Overridable Function [set](ByVal i As Integer, ByVal writable As IList(Of Writable)) As IList(Of Writable)
            Throw New NotSupportedException()
        End Function

        Public Overridable Sub Insert(ByVal index As Integer, ByVal item As IList(Of Writable)) Implements IList(Of IList(Of Writable)).Insert
            Throw New NotSupportedException()
        End Sub

        Public Overridable Sub RemoveAt(ByVal index As Integer) Implements IList(Of IList(Of Writable)).RemoveAt
            Throw New NotSupportedException()
        End Sub

        Public Overridable Function IndexOf(ByVal item As IList(Of Writable)) As Integer Implements IList(Of IList(Of Writable)).IndexOf
            Throw New NotSupportedException()
        End Function

        Public Overridable Function lastIndexOf(ByVal o As Object) As Integer
            Throw New NotSupportedException()
        End Function

        Public Overridable Function subList(ByVal i As Integer, ByVal i1 As Integer) As IList(Of IList(Of Writable))
            Throw New NotSupportedException()
        End Function

        ''' <summary>
        ''' An enumerator over the records that also offers the Java <c>ListIterator</c> methods
        ''' (<c>hasNext</c>/<c>next</c>/<c>hasPrevious</c>/<c>previous</c>, ...).
        ''' </summary>
        Public Class RecordBatchListIterator
            Implements IEnumerator(Of IList(Of Writable))

            Friend index As Integer
            Friend underlying As AbstractWritableRecordBatch
            Private currentRecord As IList(Of Writable)

            Public Sub New(ByVal underlying As AbstractWritableRecordBatch)
                Me.underlying = underlying
            End Sub

            Public Overridable Function hasNext() As Boolean
                Return index < underlying.size()
            End Function

            Public Overridable Function [next]() As IList(Of Writable)
                Dim tempVar As IList(Of Writable) = underlying.get(index)
                index += 1
                Return tempVar
            End Function

            Public Overridable Function hasPrevious() As Boolean
                Return index > 0
            End Function

            Public Overridable Function previous() As IList(Of Writable)
                Return underlying.get(index - 1)
            End Function

            Public Overridable Function nextIndex() As Integer
                Return index + 1
            End Function

            Public Overridable Function previousIndex() As Integer
                Return index - 1
            End Function

            Public Overridable Sub remove()
                Throw New NotSupportedException()
            End Sub

            Public Overridable Sub [set](ByVal writables As IList(Of Writable))
                Throw New NotSupportedException()
            End Sub

            Public Overridable Sub add(ByVal writables As IList(Of Writable))
                Throw New NotSupportedException()
            End Sub

            Public ReadOnly Property Current As IList(Of Writable) Implements IEnumerator(Of IList(Of Writable)).Current
                Get
                    Return currentRecord
                End Get
            End Property

            Private ReadOnly Property CurrentNonGeneric As Object Implements IEnumerator.Current
                Get
                    Return currentRecord
                End Get
            End Property

            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                If Not hasNext() Then
                    currentRecord = Nothing
                    Return False
                End If
                currentRecord = [next]()
                Return True
            End Function

            Public Sub Reset() Implements IEnumerator.Reset
                index = 0
                currentRecord = Nothing
            End Sub

            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub
        End Class
    End Class

End Namespace
