Imports System.Collections.Generic
Imports SequenceMerge = org.datavec.api.transform.sequence.merge.SequenceMerge
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.function
Imports org.nd4j.common.primitives

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

Namespace org.datavec.local.transforms.misc


	Public Class SequenceMergeFunction(Of T)
		Implements [Function](Of Pair(Of T, IEnumerable(Of IList(Of IList(Of Writable)))), IList(Of IList(Of Writable)))

		Private sequenceMerge As SequenceMerge

		Public Sub New(ByVal sequenceMerge As SequenceMerge)
			Me.sequenceMerge = sequenceMerge
		End Sub

		Public Overridable Function apply(ByVal t2 As Pair(Of T, IEnumerable(Of IList(Of IList(Of Writable))))) As IList(Of IList(Of Writable))
			Dim sequences As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			For Each l As IList(Of IList(Of Writable)) In t2.Second
				sequences.Add(l)
			Next l

			Return sequenceMerge.mergeSequences(sequences)
		End Function
	End Class

End Namespace