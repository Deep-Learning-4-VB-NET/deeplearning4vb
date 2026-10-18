Imports System
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports LongUtils = DeepLearning4VB.org.nd4vb.linalg.util.LongUtils

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

Namespace org.nd4vb.linalg.indexing

    ''' <summary>
    ''' An index that selects an explicit list of positions along a dimension. <c>INDArray.get</c> with a specified
    ''' index always returns a copy. Lombok's <c>@Data</c> members are written out (<see cref="getIndexes"/>,
    ''' <see cref="setIndexes"/>, <c>Equals</c>, <c>GetHashCode</c>, <c>ToString</c>).
    ''' <para>
    ''' Deliberate differences: Java's generators came from the neoitertools library (<c>Generator</c>,
    ''' <c>Itertools.product</c>) and their single-index generators were unimplemented (<c>next()</c> threw). Here
    ''' <see cref="iterate"/> and <see cref="iterateOverSparse"/> return <see cref="IEnumerable(Of T)"/> sequences of the
    ''' cross product (last index fastest, like <c>Itertools.product</c>); each element holds one list per index, which
    ''' is <c>{position}</c> for <see cref="generator"/> and <c>{position, i}</c> (the position and its place in the index
    ''' list, as in Java's old <c>nextSparse()</c>) for <see cref="sparseGenerator"/>. The nested generator classes
    ''' <c>SpecifiedIndexesGenerator</c>, <c>SparseSpecifiedIndexesGenerator</c>, <c>SingleGenerator</c> and
    ''' <c>SparseSingleGenerator</c> are replaced by these iterators.
    ''' </para>
    ''' </summary>
    ''' @author Adam Gibson
    Public Class SpecifiedIndex
        Implements INDArrayIndex

        Private indexes() As Long

        ''' <param name="indexes"> the positions to select </param>
        Public Sub New(ParamArray ByVal indexes() As Integer)
            Me.indexes = LongUtils.toLongs(indexes)
        End Sub

        ''' <param name="indexes"> the positions to select </param>
        Public Sub New(ParamArray ByVal indexes() As Long)
            Me.indexes = indexes
        End Sub

        ''' <summary>The selected positions (Lombok getter).</summary>
        Public Overridable Function getIndexes() As Long()
            Return indexes
        End Function

        ''' <summary>Replaces the selected positions (Lombok setter).</summary>
        Public Overridable Sub setIndexes(ByVal indexes() As Long)
            Me.indexes = indexes
        End Sub

        ''' <summary>The last position.</summary>
        Public Overridable Function [end]() As Long Implements INDArrayIndex.end
            Return indexes(indexes.Length - 1)
        End Function

        ''' <summary>The first position.</summary>
        Public Overridable Function offset() As Long Implements INDArrayIndex.offset
            Return indexes(0)
        End Function

        ''' <summary>The number of positions.</summary>
        Public Overridable Function length() As Long Implements INDArrayIndex.length
            Return indexes.Length
        End Function

        ''' <summary>1.</summary>
        Public Overridable Function stride() As Long Implements INDArrayIndex.stride
            Return 1
        End Function

        ''' <summary>A no-op.</summary>
        Public Overridable Sub reverse() Implements INDArrayIndex.reverse
        End Sub

        ''' <summary>False.</summary>
        Public Overridable ReadOnly Property Interval As Boolean Implements INDArrayIndex.Interval
            Get
                Return False
            End Get
        End Property

        ''' <summary>A no-op.</summary>
        Public Overridable Sub init(ByVal arr As INDArray, ByVal begin As Long, ByVal dimension As Integer) Implements INDArrayIndex.init
        End Sub

        ''' <summary>A no-op.</summary>
        Public Overridable Sub init(ByVal arr As INDArray, ByVal dimension As Integer) Implements INDArrayIndex.init
        End Sub

        ''' <summary>A no-op.</summary>
        Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long, ByVal max As Long) Implements INDArrayIndex.init
        End Sub

        ''' <summary>A no-op.</summary>
        Public Overridable Sub init(ByVal begin As Long, ByVal [end] As Long) Implements INDArrayIndex.init
        End Sub

        ''' <summary>
        ''' Iterates over the cross product of the positions of the given indexes. As in Java, point indexes in the array
        ''' are replaced by equivalent specified indexes; any other kind of index throws <see cref="InvalidCastException"/>.
        ''' </summary>
        ''' <param name="indexes"> the specified (or point) indexes </param>
        ''' <returns> one element per combination, holding <c>{position}</c> for each index </returns>
        Public Shared Function iterate(ParamArray ByVal indexes() As INDArrayIndex) As IEnumerable(Of IList(Of IList(Of Long)))
            For i As Integer = 0 To indexes.Length - 1
                'Replace point indices with specified indices
                If TypeOf indexes(i) Is PointIndex Then
                    indexes(i) = New SpecifiedIndex(indexes(i).offset())
                End If
            Next i
            Dim generators As New List(Of IList(Of IList(Of Long)))()
            For Each index As INDArrayIndex In indexes
                generators.Add(DirectCast(index, SpecifiedIndex).generator().ToList())
            Next index
            Return product(generators)
        End Function

        ''' <summary>
        ''' Iterates over the cross product of the positions of the given specified indexes, with each position paired
        ''' with its place in its index list.
        ''' </summary>
        ''' <param name="indexes"> the specified indexes (any other kind throws <see cref="InvalidCastException"/>) </param>
        ''' <returns> one element per combination, holding <c>{position, i}</c> for each index </returns>
        Public Shared Function iterateOverSparse(ParamArray ByVal indexes() As INDArrayIndex) As IEnumerable(Of IList(Of IList(Of Long)))
            Dim generators As New List(Of IList(Of IList(Of Long)))()
            For Each index As INDArrayIndex In indexes
                generators.Add(DirectCast(index, SpecifiedIndex).sparseGenerator().ToList())
            Next index
            Return product(generators)
        End Function

        ''' <summary>The positions, each as a one-element list.</summary>
        Public Overridable Function generator() As IEnumerable(Of IList(Of Long))
            Return indexes.Select(Function(p) CType(New List(Of Long) From {p}, IList(Of Long)))
        End Function

        ''' <summary>The positions, each as the list <c>{position, i}</c>.</summary>
        Public Overridable Function sparseGenerator() As IEnumerable(Of IList(Of Long))
            Return indexes.Select(Function(p, i) CType(New List(Of Long) From {p, CLng(i)}, IList(Of Long)))
        End Function

        ''' <summary>The cross product of the lists, last list fastest.</summary>
        Private Shared Iterator Function product(ByVal lists As IList(Of IList(Of IList(Of Long)))) As IEnumerable(Of IList(Of IList(Of Long)))
            If lists.Count = 0 Then
                Return
            End If
            For Each l As IList(Of IList(Of Long)) In lists
                If l.Count = 0 Then
                    Return
                End If
            Next l
            Dim positions(lists.Count - 1) As Integer
            Do
                Dim combination As New List(Of IList(Of Long))(lists.Count)
                For d As Integer = 0 To lists.Count - 1
                    combination.Add(lists(d)(positions(d)))
                Next d
                Yield combination
                Dim k As Integer = lists.Count - 1
                Do While k >= 0
                    positions(k) += 1
                    If positions(k) < lists(k).Count Then
                        Exit Do
                    End If
                    positions(k) = 0
                    k -= 1
                Loop
                If k < 0 Then
                    Return
                End If
            Loop
        End Function

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            If o Is Me Then
                Return True
            End If
            Dim other As SpecifiedIndex = TryCast(o, SpecifiedIndex)
            Return other IsNot Nothing AndAlso Arrays.equals(indexes, other.indexes)
        End Function

        ''' <summary>Lombok's hash code: <c>59 + Arrays.hashCode(indexes)</c>.</summary>
        Public Overrides Function GetHashCode() As Integer
            Return 59 + Arrays.hashCode(indexes)
        End Function

        Public Overrides Function ToString() As String
            Return "SpecifiedIndex(indexes=" & Arrays.toString(indexes) & ")"
        End Function

    End Class

End Namespace
