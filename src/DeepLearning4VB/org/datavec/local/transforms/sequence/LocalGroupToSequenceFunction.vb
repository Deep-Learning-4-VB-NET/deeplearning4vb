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

Imports System.Collections.Generic
Imports System.Linq
Imports DeepLearning4VB.org.nd4vb.common.function
Imports SequenceComparatorT = DeepLearning4VB.org.datavec.api.transform.sequence.SequenceComparator
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.sequence

    ''' <summary>
    ''' Turns the group of records collected for one key into a sequence, ordered with the sequence comparator. The sort
    ''' is stable, like Java's <c>Collections.sort</c>, so records that compare equal keep their input order. Without a
    ''' comparator the input order is kept (Java's <c>Collections.sort(list, null)</c> would have needed comparable records).
    ''' </summary>
    Public Class LocalGroupToSequenceFunction
        Implements [Function](Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT)))

        Private ReadOnly comparator As SequenceComparatorT

        Public Sub New(ByVal comparator As SequenceComparatorT)
            Me.comparator = comparator
        End Sub

        Public Overridable Function apply(ByVal lists As IList(Of IList(Of WritableT))) As IList(Of IList(Of WritableT)) Implements [Function](Of IList(Of IList(Of WritableT)), IList(Of IList(Of WritableT))).apply
            If comparator Is Nothing Then
                Return New List(Of IList(Of WritableT))(lists)
            End If
            Return lists.OrderBy(Function(record) record, comparator).ToList()
        End Function

    End Class

End Namespace
