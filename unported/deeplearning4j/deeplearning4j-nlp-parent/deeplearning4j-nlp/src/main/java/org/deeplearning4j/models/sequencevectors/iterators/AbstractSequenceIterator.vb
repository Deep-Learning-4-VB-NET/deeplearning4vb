Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.models.sequencevectors.interfaces
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement

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


	Public Class AbstractSequenceIterator(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements SequenceIterator(Of T)

		Private underlyingIterable As IEnumerable(Of Sequence(Of T))
		Private currentIterator As IEnumerator(Of Sequence(Of T))

		' used to tag each sequence with own Id
		Protected Friend tagger As New AtomicInteger(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected AbstractSequenceIterator(@NonNull Iterable<org.deeplearning4j.models.sequencevectors.sequence.Sequence<T>> iterable)
		Protected Friend Sub New(ByVal iterable As IEnumerable(Of Sequence(Of T)))
			Me.underlyingIterable = iterable
			Me.currentIterator = iterable.GetEnumerator()
		End Sub

		''' <summary>
		''' Checks, if there's more sequences available
		''' @return
		''' </summary>
		Public Overridable Function hasMoreSequences() As Boolean Implements SequenceIterator(Of T).hasMoreSequences
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return currentIterator.hasNext()
		End Function

		''' <summary>
		''' Returns next sequence out of iterator
		''' @return
		''' </summary>
		Public Overridable Function nextSequence() As Sequence(Of T) Implements SequenceIterator(Of T).nextSequence
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim sequence As Sequence(Of T) = currentIterator.next()
			sequence.setSequenceId(tagger.getAndIncrement())
			Return sequence
		End Function

		''' <summary>
		''' Resets iterator to first position
		''' </summary>
		Public Overridable Sub reset() Implements SequenceIterator(Of T).reset
			tagger.set(0)
			Me.currentIterator = underlyingIterable.GetEnumerator()
		End Sub

		Public Class Builder(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
			Friend underlyingIterable As IEnumerable(Of Sequence(Of T))

			''' <summary>
			''' Builds AbstractSequenceIterator on top of Iterable object </summary>
			''' <param name="iterable"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull Iterable<org.deeplearning4j.models.sequencevectors.sequence.Sequence<T>> iterable)
			Public Sub New(ByVal iterable As IEnumerable(Of Sequence(Of T)))
				Me.underlyingIterable = iterable
			End Sub

			''' <summary>
			''' Builds SequenceIterator
			''' @return
			''' </summary>
			Public Overridable Function build() As AbstractSequenceIterator(Of T)
				Dim iterator As New AbstractSequenceIterator(Of T)(underlyingIterable)

				Return iterator
			End Function
		End Class
	End Class

End Namespace