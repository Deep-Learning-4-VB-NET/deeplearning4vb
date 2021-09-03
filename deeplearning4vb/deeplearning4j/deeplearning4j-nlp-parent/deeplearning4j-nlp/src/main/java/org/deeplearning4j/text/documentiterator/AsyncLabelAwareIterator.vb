Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.core.parallelism

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


	Public Class AsyncLabelAwareIterator
		Implements LabelAwareIterator, IEnumerator(Of LabelledDocument)

		Protected Friend backedIterator As LabelAwareIterator
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.core.parallelism.AsyncIterator<LabelledDocument> asyncIterator;
		Protected Friend asyncIterator As AsyncIterator(Of LabelledDocument)
		Protected Friend bufferSize As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AsyncLabelAwareIterator(@NonNull LabelAwareIterator iterator, int bufferSize)
		Public Sub New(ByVal iterator As LabelAwareIterator, ByVal bufferSize As Integer)
			Me.backedIterator = iterator
			Me.bufferSize = bufferSize
			Me.asyncIterator = New AsyncIterator(Of LabelledDocument)(backedIterator, bufferSize)
		End Sub

		Public Overrides Sub remove()
			' no-op
		End Sub

		Public Overridable Function hasNextDocument() As Boolean Implements LabelAwareIterator.hasNextDocument
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return asyncIterator.hasNext()
		End Function

		Public Overridable Function nextDocument() As LabelledDocument Implements LabelAwareIterator.nextDocument
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return asyncIterator.next()
		End Function

		Public Overridable Sub reset() Implements LabelAwareIterator.reset
			asyncIterator.shutdown()
			backedIterator.reset()
			asyncIterator = New AsyncIterator(Of LabelledDocument)(backedIterator, bufferSize)
		End Sub

		Public Overridable Sub shutdown() Implements LabelAwareIterator.shutdown
			asyncIterator.shutdown()
			backedIterator.shutdown()
		End Sub

		Public Overridable ReadOnly Property LabelsSource As LabelsSource Implements LabelAwareIterator.getLabelsSource
			Get
				Return backedIterator.LabelsSource
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			Return hasNextDocument()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public LabelledDocument next() throws java.util.NoSuchElementException
		Public Overrides Function [next]() As LabelledDocument
			Return nextDocument()
		End Function
	End Class

End Namespace