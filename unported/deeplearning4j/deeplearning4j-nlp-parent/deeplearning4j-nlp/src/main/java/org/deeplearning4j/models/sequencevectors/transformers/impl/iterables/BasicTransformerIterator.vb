Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SentenceTransformer = org.deeplearning4j.models.sequencevectors.transformers.impl.SentenceTransformer
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports LabelledDocument = org.deeplearning4j.text.documentiterator.LabelledDocument

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

Namespace org.deeplearning4j.models.sequencevectors.transformers.impl.iterables


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BasicTransformerIterator implements java.util.Iterator<org.deeplearning4j.models.sequencevectors.sequence.Sequence<org.deeplearning4j.models.word2vec.VocabWord>>
	Public Class BasicTransformerIterator
		Implements IEnumerator(Of Sequence(Of VocabWord))

		Protected Friend ReadOnly iterator As LabelAwareIterator
		Protected Friend allowMultithreading As Boolean
		Protected Friend ReadOnly sentenceTransformer As SentenceTransformer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BasicTransformerIterator(@NonNull LabelAwareIterator iterator, @NonNull SentenceTransformer transformer)
		Public Sub New(ByVal iterator As LabelAwareIterator, ByVal transformer As SentenceTransformer)
			Me.iterator = iterator
			Me.allowMultithreading = False
			Me.sentenceTransformer = transformer

			Me.iterator.reset()
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return Me.iterator.hasNextDocument()
		End Function

		Public Overrides Function [next]() As Sequence(Of VocabWord)
			Dim document As LabelledDocument = iterator.nextDocument()
			If document Is Nothing OrElse document.getContent() Is Nothing Then
				Return New Sequence(Of VocabWord)()
			End If
			Dim sequence As Sequence(Of VocabWord) = sentenceTransformer.transformToSequence(document.getContent())

			If document.getLabels() IsNot Nothing Then
				For Each label As String In document.getLabels()
					If label IsNot Nothing AndAlso label.Length > 0 Then
						sequence.addSequenceLabel(New VocabWord(1.0, label))
					End If
				Next label
			End If
	'        
	'        if (document.getLabel() != null && !document.getLabel().isEmpty()) {
	'            sequence.setSequenceLabel(new VocabWord(1.0, document.getLabel()));
	'        }

			Return sequence
		End Function


		Public Overridable Sub reset()
			Me.iterator.reset()
		End Sub

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace