Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports SequenceMerge = org.datavec.api.transform.sequence.merge.SequenceMerge
Imports Writable = org.datavec.api.writable.Writable
Imports Tuple2 = scala.Tuple2

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

Namespace org.datavec.spark.transform.misc


	Public Class SequenceMergeFunction(Of T)
		Implements [Function](Of Tuple2(Of T, IEnumerable(Of IList(Of IList(Of Writable)))), IList(Of IList(Of Writable)))

		Private sequenceMerge As SequenceMerge

		Public Sub New(ByVal sequenceMerge As SequenceMerge)
			Me.sequenceMerge = sequenceMerge
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<java.util.List<org.datavec.api.writable.Writable>> call(scala.Tuple2<T, Iterable<java.util.List<java.util.List<org.datavec.api.writable.Writable>>>> t2) throws Exception
		Public Overrides Function [call](ByVal t2 As Tuple2(Of T, IEnumerable(Of IList(Of IList(Of Writable))))) As IList(Of IList(Of Writable))
			Dim sequences As IList(Of IList(Of IList(Of Writable))) = New List(Of IList(Of IList(Of Writable)))()
			For Each l As IList(Of IList(Of Writable)) In t2._2()
				sequences.Add(l)
			Next l

			Return sequenceMerge.mergeSequences(sequences)
		End Function
	End Class

End Namespace