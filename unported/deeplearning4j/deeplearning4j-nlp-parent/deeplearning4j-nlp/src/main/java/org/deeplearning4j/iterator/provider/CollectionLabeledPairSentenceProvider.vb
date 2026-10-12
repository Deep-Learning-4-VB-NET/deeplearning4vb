Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports LabeledPairSentenceProvider = org.deeplearning4j.iterator.LabeledPairSentenceProvider
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.primitives
Imports MathUtils = org.nd4j.common.util.MathUtils

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

Namespace org.deeplearning4j.iterator.provider


	Public Class CollectionLabeledPairSentenceProvider
		Implements LabeledPairSentenceProvider

		Private ReadOnly sentenceL As IList(Of String)
		Private ReadOnly sentenceR As IList(Of String)
		Private ReadOnly labels As IList(Of String)
		Private ReadOnly rng As Random
		Private ReadOnly order() As Integer
'JAVA TO VB CONVERTER NOTE: The field allLabels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly allLabels_Conflict As IList(Of String)

		Private cursor As Integer = 0

		''' <summary>
		''' Lists containing sentences to iterate over with a third for labels
		''' Sentences in the same position in the first two lists are considered a pair </summary>
		''' <param name="sentenceL"> </param>
		''' <param name="sentenceR"> </param>
		''' <param name="labelsForSentences"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CollectionLabeledPairSentenceProvider(@NonNull List<String> sentenceL, @NonNull List<String> sentenceR, @NonNull List<String> labelsForSentences)
		Public Sub New(ByVal sentenceL As IList(Of String), ByVal sentenceR As IList(Of String), ByVal labelsForSentences As IList(Of String))
			Me.New(sentenceL, sentenceR, labelsForSentences, New Random())
		End Sub

		''' <summary>
		''' Lists containing sentences to iterate over with a third for labels
		''' Sentences in the same position in the first two lists are considered a pair </summary>
		''' <param name="sentenceL"> </param>
		''' <param name="sentenceR"> </param>
		''' <param name="labelsForSentences"> </param>
		''' <param name="rng"> If null, list order is not shuffled </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CollectionLabeledPairSentenceProvider(@NonNull List<String> sentenceL, List<String> sentenceR, @NonNull List<String> labelsForSentences, Random rng)
		Public Sub New(ByVal sentenceL As IList(Of String), ByVal sentenceR As IList(Of String), ByVal labelsForSentences As IList(Of String), ByVal rng As Random)
			If sentenceR.Count <> sentenceL.Count Then
				Throw New System.ArgumentException("Sentence lists must be same size (first list size: " & sentenceL.Count & ", second list size: " & sentenceR.Count & ")")
			End If
			If sentenceR.Count <> labelsForSentences.Count Then
				Throw New System.ArgumentException("Sentence pairs and labels must be same size (sentence pair size: " & sentenceR.Count & ", labels size: " & labelsForSentences.Count & ")")
			End If

			Me.sentenceL = sentenceL
			Me.sentenceR = sentenceR
			Me.labels = labelsForSentences
			Me.rng = rng
			If rng Is Nothing Then
				order = Nothing
			Else
				order = New Integer(sentenceR.Count - 1){}
				For i As Integer = 0 To sentenceR.Count - 1
					order(i) = i
				Next i

				MathUtils.shuffleArray(order, rng)
			End If

			'Collect set of unique labels for all sentences
			Dim uniqueLabels As ISet(Of String) = New HashSet(Of String)(labelsForSentences)
			allLabels_Conflict = New List(Of String)(uniqueLabels)
			allLabels_Conflict.Sort()
		End Sub

		Public Overridable Function hasNext() As Boolean Implements LabeledPairSentenceProvider.hasNext
			Return cursor < sentenceR.Count
		End Function

		Public Overridable Function nextSentencePair() As Triple(Of String, String, String) Implements LabeledPairSentenceProvider.nextSentencePair
			Preconditions.checkState(hasNext(),"No next element available")
			Dim idx As Integer
			If rng Is Nothing Then
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idx = cursor++;
				idx = cursor
					cursor += 1
			Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: idx = order[cursor++];
				idx = order(cursor)
					cursor += 1
			End If
			Return New Triple(Of String, String, String)(sentenceL(idx), sentenceR(idx), labels(idx))
		End Function

		Public Overridable Sub reset() Implements LabeledPairSentenceProvider.reset
			cursor = 0
			If rng IsNot Nothing Then
				MathUtils.shuffleArray(order, rng)
			End If
		End Sub

		Public Overridable Function totalNumSentences() As Integer Implements LabeledPairSentenceProvider.totalNumSentences
			Return sentenceR.Count
		End Function

		Public Overridable Function allLabels() As IList(Of String)
			Return allLabels_Conflict
		End Function

		Public Overridable Function numLabelClasses() As Integer Implements LabeledPairSentenceProvider.numLabelClasses
			Return allLabels_Conflict.Count
		End Function
	End Class


End Namespace