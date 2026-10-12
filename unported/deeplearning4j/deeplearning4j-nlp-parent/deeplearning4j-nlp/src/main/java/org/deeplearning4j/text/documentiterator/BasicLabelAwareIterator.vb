Imports System
Imports NonNull = lombok.NonNull
Imports DocumentIteratorConverter = org.deeplearning4j.text.documentiterator.interoperability.DocumentIteratorConverter
Imports SentenceIterator = org.deeplearning4j.text.sentenceiterator.SentenceIterator
Imports SentenceIteratorConverter = org.deeplearning4j.text.sentenceiterator.interoperability.SentenceIteratorConverter
Imports LabelAwareSentenceIterator = org.deeplearning4j.text.sentenceiterator.labelaware.LabelAwareSentenceIterator

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

Namespace org.deeplearning4j.text.documentiterator


	Public Class BasicLabelAwareIterator
		Implements LabelAwareIterator

		' this counter is used for dumb labels generation
		Protected Friend documentPosition As New AtomicLong(0)

		Protected Friend generator As LabelsSource

		<NonSerialized>
		Protected Friend backendIterator As LabelAwareIterator

		Private Sub New()

		End Sub

		''' <summary>
		''' This method checks, if there's more LabelledDocuments
		''' @return
		''' </summary>
		Public Overridable Function hasNextDocument() As Boolean Implements LabelAwareIterator.hasNextDocument
			Return backendIterator.hasNextDocument()
		End Function

		''' <summary>
		''' This method returns next LabelledDocument
		''' @return
		''' </summary>
		Public Overridable Function nextDocument() As LabelledDocument Implements LabelAwareIterator.nextDocument
			Return backendIterator.nextDocument()
		End Function

		''' <summary>
		''' This methods resets LabelAwareIterator
		''' </summary>
		Public Overridable Sub reset() Implements LabelAwareIterator.reset
			backendIterator.reset()
		End Sub

		''' <summary>
		''' This method returns LabelsSource instance, containing all labels derived from this iterator
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property LabelsSource As LabelsSource Implements LabelAwareIterator.getLabelsSource
			Get
				Return generator
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			Return hasNextDocument()
		End Function

		Public Overrides Function [next]() As LabelledDocument
			Return nextDocument()
		End Function

		Public Overridable Sub shutdown() Implements LabelAwareIterator.shutdown
			' no-op
		End Sub

		Public Overrides Sub remove()
			' no-op
		End Sub

		Public Class Builder
'JAVA TO VB CONVERTER NOTE: The field labelTemplate was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend labelTemplate_Conflict As String = "DOC_"

			Friend labelAwareIterator As LabelAwareIterator
			Friend generator As New LabelsSource(labelTemplate_Conflict)

			''' <summary>
			''' This method should stay protected, since it's only viable for testing purposes
			''' </summary>
			Protected Friend Sub New()

			End Sub

			''' <summary>
			''' We assume that each sentence in this iterator is separate document/paragraph
			''' </summary>
			''' <param name="iterator"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull SentenceIterator iterator)
			Public Sub New(ByVal iterator As SentenceIterator)
				Me.labelAwareIterator = New SentenceIteratorConverter(iterator, generator)
			End Sub

			''' <summary>
			''' We assume that each inputStream in this iterator is separate document/paragraph </summary>
			''' <param name="iterator"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull DocumentIterator iterator)
			Public Sub New(ByVal iterator As DocumentIterator)
				Me.labelAwareIterator = New DocumentIteratorConverter(iterator, generator)
			End Sub

			''' <summary>
			''' We assume that each sentence in this iterator is separate document/paragraph.
			''' Labels will be converted into LabelledDocument format
			''' </summary>
			''' <param name="iterator"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull LabelAwareSentenceIterator iterator)
			Public Sub New(ByVal iterator As LabelAwareSentenceIterator)
				Me.labelAwareIterator = New SentenceIteratorConverter(iterator, generator)
			End Sub

			''' <summary>
			''' We assume that each inputStream in this iterator is separate document/paragraph
			''' Labels will be converted into LabelledDocument format
			''' </summary>
			''' <param name="iterator"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull LabelAwareDocumentIterator iterator)
			Public Sub New(ByVal iterator As LabelAwareDocumentIterator)
				Me.labelAwareIterator = New DocumentIteratorConverter(iterator, generator)
			End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull LabelAwareIterator iterator)
			Public Sub New(ByVal iterator As LabelAwareIterator)
				Me.labelAwareIterator = iterator
				Me.generator = iterator.LabelsSource
			End Sub

			''' <summary>
			''' Label template will be used for sentence labels generation. I.e. if provided template is "DOCUMENT_", all documents/paragraphs will have their labels starting from "DOCUMENT_0" to "DOCUMENT_X", where X is the total number of documents - 1
			''' </summary>
			''' <param name="template">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setLabelTemplate(@NonNull String template)
			Public Overridable Function setLabelTemplate(ByVal template As String) As Builder
				Me.labelTemplate_Conflict = template
				Me.generator.setTemplate(template)
				Return Me
			End Function

			''' <summary>
			''' TODO: To be implemented
			''' </summary>
			''' <param name="source">
			''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder setLabelsSource(@NonNull LabelsSource source)
			Public Overridable Function setLabelsSource(ByVal source As LabelsSource) As Builder
				Me.generator = source
				Return Me
			End Function

			Public Overridable Function build() As BasicLabelAwareIterator
				Dim iterator As New BasicLabelAwareIterator()
				iterator.generator = Me.generator
				iterator.backendIterator = Me.labelAwareIterator

				Return iterator
			End Function
		End Class
	End Class

End Namespace