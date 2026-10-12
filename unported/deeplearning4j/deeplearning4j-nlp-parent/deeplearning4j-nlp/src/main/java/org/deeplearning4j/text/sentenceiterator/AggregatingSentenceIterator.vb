Imports System
Imports System.Collections.Generic
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


	Public Class AggregatingSentenceIterator
		Implements SentenceIterator

		Private backendIterators As IList(Of SentenceIterator)
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As SentencePreProcessor
		Private position As New AtomicInteger(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private AggregatingSentenceIterator(@NonNull List<SentenceIterator> list)
		Private Sub New(ByVal list As IList(Of SentenceIterator))
			Me.backendIterators = list
		End Sub

		Public Overridable Function nextSentence() As String Implements SentenceIterator.nextSentence
			If Not backendIterators(position.get()).hasNext() AndAlso position.get() < backendIterators.Count Then
				position.incrementAndGet()
			End If

			Return If(preProcessor_Conflict Is Nothing, backendIterators(position.get()).nextSentence(), preProcessor_Conflict.preProcess(backendIterators(position.get()).nextSentence()))
		End Function

		Public Overridable Function hasNext() As Boolean Implements SentenceIterator.hasNext
			For Each iterator As SentenceIterator In backendIterators
				If iterator.hasNext() Then
					Return True
				End If
			Next iterator
			Return False
		End Function

		Public Overridable Sub reset() Implements SentenceIterator.reset
			For Each iterator As SentenceIterator In backendIterators
				iterator.reset()
			Next iterator
			Me.position.set(0)
		End Sub

		Public Overridable Sub finish() Implements SentenceIterator.finish
			For Each iterator As SentenceIterator In backendIterators
				iterator.finish()
			Next iterator
		End Sub

		Public Overridable Property PreProcessor As SentencePreProcessor Implements SentenceIterator.getPreProcessor
			Get
				Return Me.preProcessor_Conflict
			End Get
			Set(ByVal preProcessor As SentencePreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property


		Public Class Builder
			Friend backendIterators As IList(Of SentenceIterator) = New List(Of SentenceIterator)()
			Friend preProcessor As SentencePreProcessor

			Public Sub New()

			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addSentenceIterator(@NonNull SentenceIterator iterator)
			Public Overridable Function addSentenceIterator(ByVal iterator As SentenceIterator) As Builder
				Me.backendIterators.Add(iterator)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addSentenceIterators(@NonNull Collection<SentenceIterator> iterator)
			Public Overridable Function addSentenceIterators(ByVal iterator As ICollection(Of SentenceIterator)) As Builder
				CType(Me.backendIterators, List(Of SentenceIterator)).AddRange(iterator)
				Return Me
			End Function

			''' @deprecated Use <seealso cref="sentencePreProcessor(SentencePreProcessor)"/> 
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated("Use <seealso cref=""sentencePreProcessor(SentencePreProcessor)""/>") public Builder addSentencePreProcessor(@NonNull SentencePreProcessor preProcessor)
			<Obsolete("Use <seealso cref=""sentencePreProcessor(SentencePreProcessor)""/>")>
			Public Overridable Function addSentencePreProcessor(ByVal preProcessor As SentencePreProcessor) As Builder
				Return sentencePreProcessor(preProcessor)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder sentencePreProcessor(@NonNull SentencePreProcessor preProcessor)
			Public Overridable Function sentencePreProcessor(ByVal preProcessor As SentencePreProcessor) As Builder
				Me.preProcessor = preProcessor
				Return Me
			End Function

			Public Overridable Function build() As AggregatingSentenceIterator
				Dim sentenceIterator As New AggregatingSentenceIterator(Me.backendIterators)
				If Me.preProcessor IsNot Nothing Then
					sentenceIterator.PreProcessor = Me.preProcessor
				End If
				Return sentenceIterator
			End Function
		End Class
	End Class

End Namespace