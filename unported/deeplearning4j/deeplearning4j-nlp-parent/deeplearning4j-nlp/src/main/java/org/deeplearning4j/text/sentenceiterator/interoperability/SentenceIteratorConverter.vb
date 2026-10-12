Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports LabelAwareIterator = org.deeplearning4j.text.documentiterator.LabelAwareIterator
Imports LabelledDocument = org.deeplearning4j.text.documentiterator.LabelledDocument
Imports LabelsSource = org.deeplearning4j.text.documentiterator.LabelsSource
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports LabelAwareSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator
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

Namespace org.deeplearning4j.text.sentenceiterator.interoperability


	Public Class SentenceIteratorConverter
		Implements LabelAwareIterator

		Private backendIterator As SentenceIterator
		Private generator As LabelsSource
		Protected Friend Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(SentenceIteratorConverter))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SentenceIteratorConverter(@NonNull SentenceIterator iterator)
		Public Sub New(ByVal iterator As SentenceIterator)
			Me.backendIterator = iterator
			Me.generator = New LabelsSource()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SentenceIteratorConverter(@NonNull SentenceIterator iterator, @NonNull LabelsSource generator)
		Public Sub New(ByVal iterator As SentenceIterator, ByVal generator As LabelsSource)
			Me.backendIterator = iterator
			Me.generator = generator
		End Sub

		Public Overridable Function hasNextDocument() As Boolean Implements LabelAwareIterator.hasNextDocument
			Return backendIterator.hasNext()
		End Function

		Public Overridable Function nextDocument() As LabelledDocument
			Dim document As New LabelledDocument()

			document.setContent(backendIterator.nextSentence())
			If TypeOf backendIterator Is LabelAwareSentenceIterator Then
				Dim labels As IList(Of String) = DirectCast(backendIterator, LabelAwareSentenceIterator).currentLabels()
				If labels IsNot Nothing Then
					For Each label As String In labels
						document.addLabel(label)
						generator.storeLabel(label)
					Next label
				Else
					Dim label As String = DirectCast(backendIterator, LabelAwareSentenceIterator).currentLabel()
					If label IsNot Nothing Then
						document.addLabel(label)
						generator.storeLabel(label)
					End If
				End If
			ElseIf generator IsNot Nothing Then
				document.addLabel(generator.nextLabel())
			End If

			Return document
		End Function

		Public Overridable Sub reset() Implements LabelAwareIterator.reset
			generator.reset()
			backendIterator.reset()
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return hasNextDocument()
		End Function

		Public Overrides Function [next]() As LabelledDocument
			Return nextDocument()
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub

		Public Overridable ReadOnly Property LabelsSource As LabelsSource
			Get
				Return generator
			End Get
		End Property

		Public Overridable Sub shutdown() Implements LabelAwareIterator.shutdown
			' no-op
		End Sub
	End Class

End Namespace