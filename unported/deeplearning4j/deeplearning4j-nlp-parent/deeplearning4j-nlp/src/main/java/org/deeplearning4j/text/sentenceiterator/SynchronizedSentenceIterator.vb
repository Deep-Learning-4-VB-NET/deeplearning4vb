Imports NonNull = lombok.NonNull

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

Namespace org.deeplearning4j.text.sentenceiterator

	Public Class SynchronizedSentenceIterator
		Implements SentenceIterator

		Private underlyingIterator As SentenceIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SynchronizedSentenceIterator(@NonNull SentenceIterator iterator)
		Public Sub New(ByVal iterator As SentenceIterator)
			Me.underlyingIterator = iterator
		End Sub

		Public Overridable Function nextSentence() As String Implements SentenceIterator.nextSentence
			SyncLock Me
				Return Me.underlyingIterator.nextSentence()
			End SyncLock
		End Function

		Public Overridable Function hasNext() As Boolean Implements SentenceIterator.hasNext
			SyncLock Me
				Return underlyingIterator.hasNext()
			End SyncLock
		End Function

		Public Overridable Sub reset() Implements SentenceIterator.reset
			SyncLock Me
				Me.underlyingIterator.reset()
			End SyncLock
		End Sub

		Public Overridable Sub finish() Implements SentenceIterator.finish
			SyncLock Me
				Me.underlyingIterator.finish()
			End SyncLock
		End Sub

		Public Overridable Property PreProcessor As SentencePreProcessor Implements SentenceIterator.getPreProcessor
			Get
				SyncLock Me
					Return Me.underlyingIterator.PreProcessor
				End SyncLock
			End Get
			Set(ByVal preProcessor As SentencePreProcessor)
				SyncLock Me
					Me.underlyingIterator.PreProcessor = preProcessor
				End SyncLock
			End Set
		End Property

	End Class

End Namespace