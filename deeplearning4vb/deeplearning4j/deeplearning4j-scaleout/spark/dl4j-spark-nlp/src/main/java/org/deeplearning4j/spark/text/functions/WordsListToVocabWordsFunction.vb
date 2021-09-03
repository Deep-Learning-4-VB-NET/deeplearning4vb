Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
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
	''' @author jeffreytang
	''' </summary>
	Public Class WordsListToVocabWordsFunction
		Implements [Function](Of Pair(Of IList(Of String), AtomicLong), IList(Of VocabWord))

		Friend vocabCacheBroadcast As Broadcast(Of VocabCache(Of VocabWord))

		Public Sub New(ByVal vocabCacheBroadcast As Broadcast(Of VocabCache(Of VocabWord)))
			Me.vocabCacheBroadcast = vocabCacheBroadcast
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.deeplearning4j.models.word2vec.VocabWord> call(org.nd4j.common.primitives.Pair<java.util.List<String>, java.util.concurrent.atomic.AtomicLong> pair) throws Exception
		Public Overrides Function [call](ByVal pair As Pair(Of IList(Of String), AtomicLong)) As IList(Of VocabWord)
			Dim wordsList As IList(Of String) = pair.First
			Dim vocabWordsList As IList(Of VocabWord) = New List(Of VocabWord)()
			Dim vocabCache As VocabCache(Of VocabWord) = vocabCacheBroadcast.getValue()
			For Each s As String In wordsList
				If vocabCache.containsWord(s) Then
					Dim word As VocabWord = vocabCache.wordFor(s)

					vocabWordsList.Add(word)
				ElseIf vocabCache.containsWord("UNK") Then
					Dim word As VocabWord = vocabCache.wordFor("UNK")

					vocabWordsList.Add(word)
				End If
			Next s
			Return vocabWordsList
		End Function
	End Class


End Namespace