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
Imports DeepLearning4VB.org.nd4vb.common.function
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports SequenceMergeT = DeepLearning4VB.org.datavec.api.transform.sequence.merge.SequenceMerge
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.local.transforms.misc

    ''' <summary>
    ''' Merges the sequences grouped under one key, given as a (key, sequences) pair, into one sequence with a
    ''' <see cref="SequenceMergeT"/>.
    ''' </summary>
    ''' <typeparam name="T">the key type</typeparam>
    Public Class SequenceMergeFunction(Of T)
        Implements [Function](Of Pair(Of T, IEnumerable(Of IList(Of IList(Of WritableT)))), IList(Of IList(Of WritableT)))

        Private sequenceMerge As SequenceMergeT

        Public Sub New(ByVal sequenceMerge As SequenceMergeT)
            Me.sequenceMerge = sequenceMerge
        End Sub

        Public Overridable Function apply(ByVal t2 As Pair(Of T, IEnumerable(Of IList(Of IList(Of WritableT))))) As IList(Of IList(Of WritableT)) Implements [Function](Of Pair(Of T, IEnumerable(Of IList(Of IList(Of WritableT)))), IList(Of IList(Of WritableT))).apply
            Dim sequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            For Each l As IList(Of IList(Of WritableT)) In t2.Second
                sequences.Add(l)
            Next l
            Return sequenceMerge.mergeSequences(sequences)
        End Function

    End Class

End Namespace
