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

Namespace org.deeplearning4j.models.embeddings.learning.impl.elements

	Public Class BatchItem(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
'JAVA TO VB CONVERTER NOTE: The field word was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private word_Conflict As T
'JAVA TO VB CONVERTER NOTE: The field lastWord was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private lastWord_Conflict As T

'JAVA TO VB CONVERTER NOTE: The field windowWords was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private windowWords_Conflict() As Integer ' CBOW only
'JAVA TO VB CONVERTER NOTE: The field wordStatuses was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private wordStatuses_Conflict() As Boolean

'JAVA TO VB CONVERTER NOTE: The field randomValue was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private randomValue_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field alpha was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private alpha_Conflict As Double
		Private windowWordsLength As Integer

'JAVA TO VB CONVERTER NOTE: The field numLabel was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numLabel_Conflict As Integer

		Public Sub New(ByVal word As T, ByVal lastWord As T, ByVal randomValue As Long, ByVal alpha As Double)
			Me.word_Conflict = word
			Me.lastWord_Conflict = lastWord
			Me.randomValue_Conflict = randomValue
			Me.alpha_Conflict = alpha
		End Sub

		Public Sub New(ByVal word As T, ByVal windowWords() As Integer, ByVal wordStatuses() As Boolean, ByVal randomValue As Long, ByVal alpha As Double, ByVal numLabel As Integer)
			Me.word_Conflict = word
			Me.lastWord_Conflict = lastWord_Conflict
			Me.randomValue_Conflict = randomValue
			Me.alpha_Conflict = alpha
			Me.numLabel_Conflict = numLabel
			Me.windowWords_Conflict = CType(windowWords.Clone(), Integer())
			Me.wordStatuses_Conflict = CType(wordStatuses.Clone(), Boolean())
		End Sub

		Public Sub New(ByVal word As T, ByVal windowWords() As Integer, ByVal wordStatuses() As Boolean, ByVal randomValue As Long, ByVal alpha As Double)
			Me.word_Conflict = word
			Me.lastWord_Conflict = lastWord_Conflict
			Me.randomValue_Conflict = randomValue
			Me.alpha_Conflict = alpha
			Me.windowWords_Conflict = CType(windowWords.Clone(), Integer())
			Me.wordStatuses_Conflict = CType(wordStatuses.Clone(), Boolean())
		End Sub

		Public Overridable Property Word As T
			Get
				Return word_Conflict
			End Get
			Set(ByVal word As T)
				Me.word_Conflict = word
			End Set
		End Property


		Public Overridable Property LastWord As T
			Get
				Return lastWord_Conflict
			End Get
			Set(ByVal lastWord As T)
				Me.lastWord_Conflict = lastWord
			End Set
		End Property


		Public Overridable ReadOnly Property RandomValue As Long
			Get
				Return randomValue_Conflict
			End Get
		End Property

		Public Overridable Property Alpha As Double
			Get
				Return alpha_Conflict
			End Get
			Set(ByVal alpha As Double)
				Me.alpha_Conflict = alpha
			End Set
		End Property


		Public Overridable ReadOnly Property WindowWords As Integer()
			Get
				Return windowWords_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property WordStatuses As Boolean()
			Get
				Return wordStatuses_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property NumLabel As Integer
			Get
				Return numLabel_Conflict
			End Get
		End Property
	End Class

End Namespace