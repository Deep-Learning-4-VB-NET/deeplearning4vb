Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports SequenceComparator = org.datavec.api.transform.sequence.SequenceComparator
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.api.transform.sequence.merge


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class SequenceMerge implements java.io.Serializable
	<Serializable>
	Public Class SequenceMerge

		Private ReadOnly comparator As SequenceComparator

		Public Sub New(ByVal comparator As SequenceComparator)
			Me.comparator = comparator
		End Sub

		Public Overridable Function mergeSequences(ByVal multipleSequences As IList(Of IList(Of IList(Of Writable)))) As IList(Of IList(Of Writable))

			'Approach here: append all time steps, then sort

			Dim [out] As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			For Each sequence As IList(Of IList(Of Writable)) In multipleSequences
				CType([out], List(Of IList(Of Writable))).AddRange(sequence)
			Next sequence

			[out].Sort(comparator)

			Return [out]
		End Function
	End Class

End Namespace