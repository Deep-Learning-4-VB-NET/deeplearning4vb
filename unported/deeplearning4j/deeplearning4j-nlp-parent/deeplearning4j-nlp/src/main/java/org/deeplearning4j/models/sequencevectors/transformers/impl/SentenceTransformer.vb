Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports org.deeplearning4j.models.sequencevectors.transformers
Imports BasicTransformerIterator = org.deeplearning4j.models.sequencevectors.transformers.impl.iterables.BasicTransformerIterator
Imports ParallelTransformerIterator = org.deeplearning4j.models.sequencevectors.transformers.impl.iterables.ParallelTransformerIterator
Imports VocabWord = org.deeplearning4j.models.word2vec.VocabWord
Imports org.deeplearning4j.models.word2vec.wordstore
Imports BasicLabelAwareIterator = org.deeplearning4j.text.documentiterator.BasicLabelAwareIterator
Imports DocumentIterator = org.deeplearning4j.text.documentiterator.DocumentIterator
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports Tokenizer = org.deeplearning4j.text.tokenization.tokenizer.Tokenizer
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.models.sequencevectors.transformers.impl


	Public Class SentenceTransformer
		Implements SequenceTransformer(Of VocabWord, String), IEnumerable(Of Sequence(Of VocabWord))

	'    
	'            So, we must accept any SentenceIterator implementations, and build vocab out of it, and use it for further transforms between text and Sequences
	'     
		Protected Friend tokenizerFactory As TokenizerFactory
'JAVA TO VB CONVERTER NOTE: The field iterator was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend iterator_Conflict As LabelAwareIterator
		Protected Friend [readOnly] As Boolean = False
		Protected Friend sentenceCounter As New AtomicInteger(0)
		Protected Friend allowMultithreading As Boolean = False
		Protected Friend currentIterator As BasicTransformerIterator

		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(SentenceTransformer))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private SentenceTransformer(@NonNull LabelAwareIterator iterator)
		Private Sub New(ByVal iterator As LabelAwareIterator)
			Me.iterator_Conflict = iterator
		End Sub

		Public Overridable Function transformToSequence(ByVal [object] As String) As Sequence(Of VocabWord)
			Dim sequence As New Sequence(Of VocabWord)()

			Dim tokenizer As Tokenizer = tokenizerFactory.create([object])
			Dim list As IList(Of String) = tokenizer.getTokens()

			For Each token As String In list
				If token Is Nothing OrElse token.Length = 0 OrElse token.Trim().Length = 0 Then
					Continue For
				End If

				Dim word As New VocabWord(1.0, token)
				sequence.addElement(word)
			Next token

			sequence.setSequenceId(sentenceCounter.getAndIncrement())
			Return sequence
		End Function

		Public Overridable Function GetEnumerator() As IEnumerator(Of Sequence(Of VocabWord)) Implements IEnumerator(Of Sequence(Of VocabWord)).GetEnumerator
			If currentIterator Is Nothing Then
				'if (!allowMultithreading)
					currentIterator = New BasicTransformerIterator(iterator_Conflict, Me)
				'else
				'    currentIterator = new ParallelTransformerIterator(iterator, this, true);
			Else
				reset()
			End If

			Return currentIterator
		End Function

		Public Overridable Sub reset() Implements SequenceTransformer(Of VocabWord, String).reset
			If currentIterator IsNot Nothing Then
				currentIterator.reset()
			End If
		End Sub


		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field tokenizerFactory was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend tokenizerFactory_Conflict As TokenizerFactory
'JAVA TO VB CONVERTER NOTE: The field iterator was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend iterator_Conflict As LabelAwareIterator
			Protected Friend vocabCache As VocabCache(Of VocabWord)
'JAVA TO VB CONVERTER NOTE: The field readOnly was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend readOnly_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The field allowMultithreading was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend allowMultithreading_Conflict As Boolean = False

			Public Sub New()

			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder tokenizerFactory(@NonNull TokenizerFactory tokenizerFactory)
'JAVA TO VB CONVERTER NOTE: The parameter tokenizerFactory was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function tokenizerFactory(ByVal tokenizerFactory_Conflict As TokenizerFactory) As Builder
				Me.tokenizerFactory_Conflict = tokenizerFactory_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder iterator(@NonNull LabelAwareIterator iterator)
'JAVA TO VB CONVERTER NOTE: The parameter iterator was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function iterator(ByVal iterator_Conflict As LabelAwareIterator) As Builder
				Me.iterator_Conflict = iterator_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder iterator(@NonNull SentenceIterator iterator)
'JAVA TO VB CONVERTER NOTE: The parameter iterator was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function iterator(ByVal iterator_Conflict As SentenceIterator) As Builder
				Me.iterator_Conflict = (New BasicLabelAwareIterator.Builder(iterator_Conflict)).build()
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder iterator(@NonNull DocumentIterator iterator)
'JAVA TO VB CONVERTER NOTE: The parameter iterator was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function iterator(ByVal iterator_Conflict As DocumentIterator) As Builder
				Me.iterator_Conflict = (New BasicLabelAwareIterator.Builder(iterator_Conflict)).build()
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter readOnly was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function [readOnly](ByVal readOnly_Conflict As Boolean) As Builder
				Me.readOnly_Conflict = True
				Return Me
			End Function

			''' <summary>
			''' This method enables/disables parallel processing over sentences
			''' </summary>
			''' <param name="reallyAllow">
			''' @return </param>
			Public Overridable Function allowMultithreading(ByVal reallyAllow As Boolean) As Builder
				Me.allowMultithreading_Conflict = reallyAllow
				Return Me
			End Function

			Public Overridable Function build() As SentenceTransformer
				Dim transformer As New SentenceTransformer(Me.iterator_Conflict)
				transformer.tokenizerFactory = Me.tokenizerFactory_Conflict
				transformer.readOnly = Me.readOnly_Conflict
				transformer.allowMultithreading = Me.allowMultithreading_Conflict

				Return transformer
			End Function
		End Class
	End Class

End Namespace