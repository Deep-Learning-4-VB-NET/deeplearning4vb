Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports BaseDataFetcher = org.nd4j.linalg.dataset.api.iterator.fetcher.BaseDataFetcher

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

Namespace org.deeplearning4j.datasets.iterator


	<Serializable>
	Public Class BaseDatasetIterator
		Implements DataSetIterator

'JAVA TO VB CONVERTER NOTE: The field batch was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend batch_Conflict, numExamples As Integer
		Protected Friend fetcher As BaseDataFetcher
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preProcessor_Conflict As DataSetPreProcessor


		Public Sub New(ByVal batch As Integer, ByVal numExamples As Integer, ByVal fetcher As BaseDataFetcher)
			If batch <= 0 Then
				Throw New System.ArgumentException("Invalid minibatch size: must be > 0 (got: " & batch & ")")
			End If
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
			fetcher.fetch(batch_Conflict)
			Dim result As DataSet = fetcher.next()
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(result)
			End If
			Return result
		End Function

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			fetcher.fetch(num)
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim next_Conflict As DataSet = fetcher.next()
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(next_Conflict)
			End If
			Return next_Conflict
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

		Public Overridable WriteOnly Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property


	End Class

End Namespace