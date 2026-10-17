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
Imports System.Collections.Generic
Imports SequenceComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceComparator
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.sequence.merge

    ''' <summary>
    ''' Merge multiple sequences into one single sequence. Merging is done by appending all the time steps of all the
    ''' sequences and then sorting them with the comparator. The sort is stable, like Java's <c>Collections.sort</c>:
    ''' time steps that compare equal keep their order. Equality (Lombok <c>@Data</c>) compares the comparator.
    ''' </summary>
    <Serializable>
    Public Class SequenceMerge

        Private ReadOnly comparator As SequenceComparatorT

        Public Sub New(ByVal comparator As SequenceComparatorT)
            Me.comparator = comparator
        End Sub

        ''' <summary>Java <c>getComparator()</c> (Lombok).</summary>
        Public Function getComparator() As SequenceComparatorT
            Return comparator
        End Function

        ''' <summary>Appends all time steps of the sequences and sorts them (stable) with the comparator.</summary>
        Public Overridable Function mergeSequences(ByVal multipleSequences As IList(Of IList(Of IList(Of WritableT)))) As IList(Of IList(Of WritableT))

            'Approach here: append all time steps, then sort

            Dim [out] As New List(Of IList(Of WritableT))()
            For Each sequence As IList(Of IList(Of WritableT)) In multipleSequences
                [out].AddRange(sequence)
            Next sequence

            stableSort([out], comparator)

            Return [out]
        End Function

        ''' <summary>Stable in-place sort (List.Sort is not stable; ties are broken by the original position).</summary>
        Private Shared Sub stableSort(Of T)(ByVal list As List(Of T), ByVal comparer As IComparer(Of T))
            Dim indexed As New List(Of KeyValuePair(Of Integer, T))(list.Count)
            For i As Integer = 0 To list.Count - 1
                indexed.Add(New KeyValuePair(Of Integer, T)(i, list(i)))
            Next i
            indexed.Sort(Function(a As KeyValuePair(Of Integer, T), b As KeyValuePair(Of Integer, T)) As Integer
                             Dim c As Integer = comparer.Compare(a.Value, b.Value)
                             If c <> 0 Then Return c
                             Return a.Key.CompareTo(b.Key)
                         End Function)
            For i As Integer = 0 To list.Count - 1
                list(i) = indexed(i).Value
            Next i
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            If obj Is Nothing OrElse obj.GetType() IsNot Me.GetType() Then Return False
            Return Object.Equals(comparator, DirectCast(obj, SequenceMerge).comparator)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 59 + Objects.hashCode(comparator)
        End Function

        Public Overrides Function ToString() As String
            Return "SequenceMerge(comparator=" & Objects.toString(comparator) & ")"
        End Function

    End Class

End Namespace
