Imports System
Imports System.IO
Imports System.Text
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.text.documentiterator

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

Namespace org.deeplearning4j.text.documentiterator.interoperability


	Public Class DocumentIteratorConverter
		Implements LabelAwareIterator

		Protected Friend backendIterator As DocumentIterator
		Protected Friend generator As LabelsSource

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DocumentIteratorConverter(@NonNull LabelAwareDocumentIterator iterator)
		Public Sub New(ByVal iterator As LabelAwareDocumentIterator)
			Me.backendIterator = iterator
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DocumentIteratorConverter(@NonNull DocumentIterator iterator, @NonNull LabelsSource generator)
		Public Sub New(ByVal iterator As DocumentIterator, ByVal generator As LabelsSource)
			Me.backendIterator = iterator
			Me.generator = generator
		End Sub

		Public Overridable Function hasNextDocument() As Boolean Implements LabelAwareIterator.hasNextDocument
			Return backendIterator.hasNext()
		End Function

		Public Overridable Function nextDocument() As LabelledDocument Implements LabelAwareIterator.nextDocument
			Try
				Dim document As New LabelledDocument()

				document.setContent(readStream(backendIterator.nextDocument()))

				If TypeOf backendIterator Is LabelAwareDocumentIterator Then
					Dim currentLabel As String = DirectCast(backendIterator, LabelAwareDocumentIterator).currentLabel()
					document.addLabel(currentLabel)
					generator.storeLabel(currentLabel)
				Else
					document.addLabel(generator.nextLabel())
				End If

				Return document
			Catch e As Exception
				' we just publish caught exception, no magic or automation here
				Throw New Exception(e)
			End Try
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

		Public Overridable ReadOnly Property LabelsSource As LabelsSource Implements LabelAwareIterator.getLabelsSource
			Get
				Return generator
			End Get
		End Property

		Public Overridable Sub shutdown() Implements LabelAwareIterator.shutdown

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected String readStream(java.io.InputStream stream) throws java.io.IOException
		Protected Friend Overridable Function readStream(ByVal stream As Stream) As String
			Dim builder As New StringBuilder()

			Dim reader As New StreamReader(stream)
			Dim line As String = ""
			line = reader.ReadLine()
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: while ((line = reader.readLine()) != null)
			Do While line IsNot Nothing
				builder.Append(line).Append(" ")
					line = reader.ReadLine()
			Loop
			Return builder.ToString()
		End Function
	End Class

End Namespace