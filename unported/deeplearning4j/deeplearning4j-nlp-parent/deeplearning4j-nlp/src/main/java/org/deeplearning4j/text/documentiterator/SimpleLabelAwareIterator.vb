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

Namespace org.deeplearning4j.text.documentiterator


	Public Class SimpleLabelAwareIterator
		Implements LabelAwareIterator

		<NonSerialized>
		Protected Friend underlyingIterable As IEnumerable(Of LabelledDocument)
		<NonSerialized>
		Protected Friend currentIterator As IEnumerator(Of LabelledDocument)
		Protected Friend labels As New LabelsSource()

		''' <summary>
		''' Builds LabelAwareIterator instance using Iterable object </summary>
		''' <param name="iterable"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SimpleLabelAwareIterator(@NonNull Iterable<LabelledDocument> iterable)
		Public Sub New(ByVal iterable As IEnumerable(Of LabelledDocument))
			Me.underlyingIterable = iterable
			Me.currentIterator = underlyingIterable.GetEnumerator()
		End Sub

		''' <summary>
		''' Builds LabelAwareIterator instance using Iterator object
		''' PLEASE NOTE: If instance is built using Iterator object, reset() method becomes unavailable
		''' </summary>
		''' <param name="iterator"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SimpleLabelAwareIterator(@NonNull Iterator<LabelledDocument> iterator)
		Public Sub New(ByVal iterator As IEnumerator(Of LabelledDocument))
			Me.currentIterator = iterator
		End Sub

		''' <summary>
		''' This method checks, if there's more LabelledDocuments in underlying iterator
		''' @return
		''' </summary>
		Public Overridable Function hasNextDocument() As Boolean Implements LabelAwareIterator.hasNextDocument
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return currentIterator.hasNext()
		End Function

		''' <summary>
		''' This method returns next LabelledDocument from underlying iterator
		''' @return
		''' </summary>
		Public Overridable Function nextDocument() As LabelledDocument Implements LabelAwareIterator.nextDocument
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim document As LabelledDocument = currentIterator.next()
			For Each label As String In document.getLabels()
				labels.storeLabel(label)
			Next label

			Return document
		End Function

		Public Overrides Function hasNext() As Boolean
			Return hasNextDocument()
		End Function

		Public Overrides Function [next]() As LabelledDocument
			Return nextDocument()
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub

		Public Overridable Sub shutdown() Implements LabelAwareIterator.shutdown
			' no-op
		End Sub

		''' <summary>
		''' This methods resets LabelAwareIterator by creating new Iterator from Iterable internally
		''' </summary>
		Public Overridable Sub reset() Implements LabelAwareIterator.reset
			If underlyingIterable IsNot Nothing Then
				Me.currentIterator = Me.underlyingIterable.GetEnumerator()
			Else
				Throw New System.NotSupportedException("You can't use reset() method for Iterator<> based instance, please provide Iterable<> instead, or avoid reset()")
			End If
		End Sub

		''' <summary>
		''' This method returns LabelsSource instance, containing all labels derived from this iterator
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property LabelsSource As LabelsSource Implements LabelAwareIterator.getLabelsSource
			Get
				Return labels
			End Get
		End Property
	End Class

End Namespace