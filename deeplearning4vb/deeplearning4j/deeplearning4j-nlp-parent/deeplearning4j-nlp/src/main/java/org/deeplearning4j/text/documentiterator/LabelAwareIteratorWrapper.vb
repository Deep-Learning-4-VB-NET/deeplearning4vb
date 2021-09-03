Imports System.Collections.Generic

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


	Public Class LabelAwareIteratorWrapper
		Implements LabelAwareIterator

	  Private ReadOnly [delegate] As LabelAwareIterator
	  Private ReadOnly sink As LabelsSource

	  Public Sub New(ByVal [delegate] As LabelAwareIterator, ByVal sink As LabelsSource)
		Me.delegate = [delegate]
		Me.sink = sink
	  End Sub

	  Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
		Return [delegate].hasNext()
	  End Function

	  Public Overridable Function hasNextDocument() As Boolean Implements LabelAwareIterator.hasNextDocument
		Return [delegate].hasNextDocument()
	  End Function

	  Public Overridable ReadOnly Property LabelsSource As LabelsSource Implements LabelAwareIterator.getLabelsSource
		  Get
			Return sink
		  End Get
	  End Property

	  Public Overrides Function [next]() As LabelledDocument
		Return nextDocument()
	  End Function

	  Public Overrides Sub remove()

	  End Sub

	  Public Overridable Function nextDocument() As LabelledDocument Implements LabelAwareIterator.nextDocument
		Dim doc As LabelledDocument = [delegate].nextDocument()
		Dim labels As IList(Of String) = doc.getLabels()
		If labels IsNot Nothing Then
		  For Each label As String In labels
			sink.storeLabel(label)
		  Next label
		End If
		Return doc
	  End Function

	  Public Overridable Sub reset() Implements LabelAwareIterator.reset
		[delegate].reset()
		sink.reset()
	  End Sub

	  Public Overridable Sub shutdown() Implements LabelAwareIterator.shutdown
	  End Sub
	End Class

End Namespace