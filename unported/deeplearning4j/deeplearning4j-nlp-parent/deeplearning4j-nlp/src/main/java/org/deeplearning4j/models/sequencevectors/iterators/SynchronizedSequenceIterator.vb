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

	Public Class SynchronizedSequenceIterator(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements SequenceIterator(Of T)

		Protected Friend underlyingIterator As SequenceIterator(Of T)

		''' <summary>
		''' Creates SynchronizedSequenceIterator on top of any SequenceIterator </summary>
		''' <param name="iterator"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SynchronizedSequenceIterator(@NonNull SequenceIterator<T> iterator)
		Public Sub New(ByVal iterator As SequenceIterator(Of T))
			Me.underlyingIterator = iterator
		End Sub

		''' <summary>
		''' Checks, if there's any more sequences left in data source
		''' @return
		''' </summary>
		Public Overridable Function hasMoreSequences() As Boolean Implements SequenceIterator(Of T).hasMoreSequences
			SyncLock Me
				Return underlyingIterator.hasMoreSequences()
			End SyncLock
		End Function

		''' <summary>
		''' Returns next sequence from data source
		''' 
		''' @return
		''' </summary>
		Public Overridable Function nextSequence() As Sequence(Of T) Implements SequenceIterator(Of T).nextSequence
			SyncLock Me
				Return underlyingIterator.nextSequence()
			End SyncLock
		End Function

		''' <summary>
		''' This method resets underlying iterator
		''' </summary>
		Public Overridable Sub reset() Implements SequenceIterator(Of T).reset
			SyncLock Me
				underlyingIterator.reset()
			End SyncLock
		End Sub
	End Class

End Namespace