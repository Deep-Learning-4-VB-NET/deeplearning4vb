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

	Public MustInherit Class BaseSentenceIterator
		Implements SentenceIterator

		Public MustOverride Sub reset() Implements SentenceIterator.reset
		Public MustOverride Function hasNext() As Boolean Implements SentenceIterator.hasNext
		Public MustOverride Function nextSentence() As String Implements SentenceIterator.nextSentence

'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preProcessor_Conflict As SentencePreProcessor



		Public Sub New(ByVal preProcessor As SentencePreProcessor)
			MyBase.New()
			Me.preProcessor_Conflict = preProcessor
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Public Overridable Property PreProcessor As SentencePreProcessor Implements SentenceIterator.getPreProcessor
			Get
				Return preProcessor_Conflict
			End Get
			Set(ByVal preProcessor As SentencePreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property


		Public Overridable Sub finish() Implements SentenceIterator.finish
			'No-op
		End Sub



	End Class

End Namespace