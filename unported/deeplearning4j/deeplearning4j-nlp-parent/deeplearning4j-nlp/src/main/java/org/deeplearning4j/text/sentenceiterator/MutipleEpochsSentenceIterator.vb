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


	Public Class MutipleEpochsSentenceIterator
		Implements SentenceIterator

		Private iterator As SentenceIterator
		Private numEpochs As Integer
		Private counter As New AtomicInteger(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MutipleEpochsSentenceIterator(@NonNull SentenceIterator iterator, int numEpochs)
		Public Sub New(ByVal iterator As SentenceIterator, ByVal numEpochs As Integer)
			Me.numEpochs = numEpochs
			Me.iterator = iterator

			Me.iterator.reset()
		End Sub

		Public Overridable Function nextSentence() As String Implements SentenceIterator.nextSentence
			Return iterator.nextSentence()
		End Function

		Public Overridable Function hasNext() As Boolean Implements SentenceIterator.hasNext
			If Not iterator.hasNext() Then
				If counter.get() < numEpochs - 1 Then
					counter.incrementAndGet()
					iterator.reset()
					Return True
				Else
					Return False
				End If
			End If
			Return True
		End Function

		Public Overridable Sub reset() Implements SentenceIterator.reset
			Me.counter.set(0)
			Me.iterator.reset()
		End Sub

		Public Overridable Sub finish() Implements SentenceIterator.finish
			' no-op
		End Sub

		Public Overridable Property PreProcessor As SentencePreProcessor Implements SentenceIterator.getPreProcessor
			Get
				Return Me.iterator.PreProcessor
			End Get
			Set(ByVal preProcessor As SentencePreProcessor)
				Me.iterator.PreProcessor = preProcessor
			End Set
		End Property

	End Class

End Namespace