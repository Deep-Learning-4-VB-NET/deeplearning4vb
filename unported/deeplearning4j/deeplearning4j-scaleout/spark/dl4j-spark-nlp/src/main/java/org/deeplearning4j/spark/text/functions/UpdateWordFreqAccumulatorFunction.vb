Imports System.Collections.Generic
Imports Accumulator = org.apache.spark.Accumulator
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports org.nd4j.common.primitives
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

Namespace org.deeplearning4j.spark.text.functions


	''' <summary>
	''' @author Jeffrey Tang
	''' </summary>
	Public Class UpdateWordFreqAccumulatorFunction
		Implements [Function](Of IList(Of String), Pair(Of IList(Of String), AtomicLong))

		Private stopWords As Broadcast(Of IList(Of String))
		Private wordFreqAcc As Accumulator(Of Counter(Of String))

		Public Sub New(ByVal stopWords As Broadcast(Of IList(Of String)), ByVal wordFreqAcc As Accumulator(Of Counter(Of String)))
			Me.wordFreqAcc = wordFreqAcc
			Me.stopWords = stopWords
		End Sub

		' Function to add to word freq counter and total count of words
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.common.primitives.Pair<java.util.List<String>, java.util.concurrent.atomic.AtomicLong> call(java.util.List<String> lstOfWords) throws Exception
		Public Overrides Function [call](ByVal lstOfWords As IList(Of String)) As Pair(Of IList(Of String), AtomicLong)
			Dim stops As IList(Of String) = stopWords.getValue()
			Dim counter As New Counter(Of String)()

			For Each w As String In lstOfWords
				If w.Length = 0 Then
					Continue For
				End If

				If stops.Count > 0 Then
					If stops.Contains(w) Then
						counter.incrementCount("STOP", 1.0f)
					Else
						counter.incrementCount(w, 1.0f)
					End If
				Else
					counter.incrementCount(w, 1.0f)
				End If
			Next w
			wordFreqAcc.add(counter)
			Dim lstOfWordsSize As New AtomicLong(lstOfWords.Count)
			Return New Pair(Of IList(Of String), AtomicLong)(lstOfWords, lstOfWordsSize)
		End Function
	End Class


End Namespace