Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports LabeledSentenceProvider = org.deeplearning4j.iterator.LabeledSentenceProvider
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


	Public Class CollectionLabeledSentenceProvider
		Implements LabeledSentenceProvider

		Private ReadOnly sentences As IList(Of String)
		Private ReadOnly labels As IList(Of String)
		Private ReadOnly rng As Random
		Private ReadOnly order() As Integer
'JAVA TO VB CONVERTER NOTE: The field allLabels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly allLabels_Conflict As IList(Of String)

		Private cursor As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CollectionLabeledSentenceProvider(@NonNull List<String> sentences, @NonNull List<String> labelsForSentences)
		Public Sub New(ByVal sentences As IList(Of String), ByVal labelsForSentences As IList(Of String))
			Me.New(sentences, labelsForSentences, New Random())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CollectionLabeledSentenceProvider(@NonNull List<String> sentences, @NonNull List<String> labelsForSentences, Random rng)
		Public Sub New(ByVal sentences As IList(Of String), ByVal labelsForSentences As IList(Of String), ByVal rng As Random)
			If sentences.Count <> labelsForSentences.Count Then
				Throw New System.ArgumentException("Sentences and labels must be same size (sentences size: " & sentences.Count & ", labels size: " & labelsForSentences.Count & ")")
			End If

			Me.sentences = sentences
			Me.labels = labelsForSentences
			Me.rng = rng
			If rng Is Nothing Then
				order = Nothing
			Else
				order = New Integer(sentences.Count - 1){}
				For i As Integer = 0 To sentences.Count - 1
					order(i) = i
				Next i

				MathUtils.shuffleArray(order, rng)
			End If

			'Collect set of unique labels for all sentences
			Dim uniqueLabels As ISet(Of String) = New HashSet(Of String)(labelsForSentences)
			allLabels_Conflict = New List(Of String)(uniqueLabels)
			allLabels_Conflict.Sort()
		End Sub

		Public Overridable Function hasNext() As Boolean Implements LabeledSentenceProvider.hasNext
			Return cursor < sentences.Count
		End Function

		Public Overridable Function nextSentence() As Pair(Of String, String) Implements LabeledSentenceProvider.nextSentence
			Preconditions.checkState(hasNext(), "No next element available")
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
			Return New Pair(Of String, String)(sentences(idx), labels(idx))
		End Function

		Public Overridable Sub reset() Implements LabeledSentenceProvider.reset
			cursor = 0
			If rng IsNot Nothing Then
				MathUtils.shuffleArray(order, rng)
			End If
		End Sub

		Public Overridable Function totalNumSentences() As Integer Implements LabeledSentenceProvider.totalNumSentences
			Return sentences.Count
		End Function

		Public Overridable Function allLabels() As IList(Of String)
			Return allLabels_Conflict
		End Function

		Public Overridable Function numLabelClasses() As Integer Implements LabeledSentenceProvider.numLabelClasses
			Return allLabels_Conflict.Count
		End Function
	End Class

End Namespace