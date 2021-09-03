Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore

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

Namespace org.deeplearning4j.models.sequencevectors.iterators

	Public Class FilteredSequenceIterator(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements SequenceIterator(Of T)

		Private ReadOnly underlyingIterator As SequenceIterator(Of T)
		Private ReadOnly vocabCache As VocabCache(Of T)

		''' <summary>
		''' Creates Filtered SequenceIterator on top of another SequenceIterator and appropriate VocabCache instance
		''' </summary>
		''' <param name="iterator"> </param>
		''' <param name="vocabCache"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FilteredSequenceIterator(@NonNull SequenceIterator<T> iterator, @NonNull VocabCache<T> vocabCache)
		Public Sub New(ByVal iterator As SequenceIterator(Of T), ByVal vocabCache As VocabCache(Of T))
			Me.vocabCache = vocabCache
			Me.underlyingIterator = iterator
		End Sub

		''' <summary>
		''' Checks, if there's any more sequences left in underlying iterator
		''' @return
		''' </summary>
		Public Overridable Function hasMoreSequences() As Boolean Implements SequenceIterator(Of T).hasMoreSequences
			Return underlyingIterator.hasMoreSequences()
		End Function

		''' <summary>
		''' Returns filtered sequence, that contains sequence elements from vocabulary only.
		''' Please note: it can return empty sequence, if no elements were found in vocabulary
		''' @return
		''' </summary>
		Public Overridable Function nextSequence() As Sequence(Of T) Implements SequenceIterator(Of T).nextSequence
			Dim originalSequence As Sequence(Of T) = underlyingIterator.nextSequence()
			Dim newSequence As New Sequence(Of T)()

			If originalSequence IsNot Nothing Then
				For Each element As T In originalSequence.getElements()
					If element IsNot Nothing AndAlso vocabCache.hasToken(element.getLabel()) Then
						newSequence.addElement(vocabCache.wordFor(element.getLabel()))
					End If
				Next element
			End If

			newSequence.setSequenceId(originalSequence.getSequenceId())

			Return newSequence
		End Function

		''' <summary>
		''' Resets iterator down to first sequence
		''' </summary>
		Public Overridable Sub reset() Implements SequenceIterator(Of T).reset
			underlyingIterator.reset()
		End Sub
	End Class

End Namespace