Imports System
Imports System.Collections.Generic
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetFetcher = org.nd4j.linalg.dataset.api.iterator.fetcher.DataSetFetcher

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

Namespace org.nd4j.linalg.dataset.api.iterator


	<Serializable>
	Public Class BaseDatasetIterator
		Implements DataSetIterator


		Private Const serialVersionUID As Long = -116636792426198949L
'JAVA TO VB CONVERTER NOTE: The field batch was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend batch_Conflict, numExamples As Integer
		Protected Friend fetcher As DataSetFetcher
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preProcessor_Conflict As DataSetPreProcessor


		Public Sub New(ByVal batch As Integer, ByVal numExamples As Integer, ByVal fetcher As DataSetFetcher)
			Me.batch_Conflict = batch
			If numExamples < 0 Then
				numExamples = fetcher.totalExamples()
			End If

			Me.numExamples = numExamples
			Me.fetcher = fetcher
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return fetcher.hasMore() AndAlso fetcher.cursor() < numExamples
		End Function

		Public Overrides Function [next]() As DataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not hasNext() Then
				Throw New NoSuchElementException("No next element - hasNext() == false")
			End If
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim next_Conflict As Integer = Math.Min(batch_Conflict, numExamples - fetcher.cursor())
			fetcher.fetch(next_Conflict)
			Dim ds As DataSet = fetcher.next()
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(ds)
			End If
			Return ds
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return fetcher.inputColumns()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return fetcher.totalOutcomes()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			fetcher.reset()
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return batch_Conflict
		End Function

		''' <summary>
		''' Set a pre processor
		''' </summary>
		''' <param name="preProcessor"> a pre processor to set </param>
		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			fetcher.fetch(num)
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim next_Conflict As DataSet = fetcher.next()
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(next_Conflict)
			End If
			Return next_Conflict
		End Function


	End Class

End Namespace