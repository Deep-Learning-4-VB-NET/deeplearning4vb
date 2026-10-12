Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports LabeledSentenceProvider = org.deeplearning4j.iterator.LabeledSentenceProvider
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports LabelledDocument = org.deeplearning4j.text.documentiterator.LabelledDocument
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

Namespace org.deeplearning4j.iterator.provider


	Public Class LabelAwareConverter
		Implements LabeledSentenceProvider

		Private backingIterator As LabelAwareIterator
		Private labels As IList(Of String)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LabelAwareConverter(@NonNull LabelAwareIterator iterator, @NonNull List<String> labels)
		Public Sub New(ByVal iterator As LabelAwareIterator, ByVal labels As IList(Of String))
			Me.backingIterator = iterator
			Me.labels = labels
		End Sub

		Public Overridable Function hasNext() As Boolean Implements LabeledSentenceProvider.hasNext
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return backingIterator.hasNext()
		End Function

		Public Overridable Function nextSentence() As Pair(Of String, String) Implements LabeledSentenceProvider.nextSentence
			Dim document As LabelledDocument = backingIterator.nextDocument()

			' TODO: probably worth to allow more then one label? i.e. pass same document twice, sequentially
			Return Pair.makePair(document.getContent(), document.getLabels().get(0))
		End Function

		Public Overridable Sub reset() Implements LabeledSentenceProvider.reset
			backingIterator.reset()
		End Sub

		Public Overridable Function totalNumSentences() As Integer Implements LabeledSentenceProvider.totalNumSentences
			Return -1
		End Function

		Public Overridable Function allLabels() As IList(Of String) Implements LabeledSentenceProvider.allLabels
			Return labels
		End Function

		Public Overridable Function numLabelClasses() As Integer Implements LabeledSentenceProvider.numLabelClasses
			Return labels.Count
		End Function
	End Class

End Namespace