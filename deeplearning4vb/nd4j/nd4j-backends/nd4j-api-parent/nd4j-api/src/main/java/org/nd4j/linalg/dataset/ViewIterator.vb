Imports System
Imports System.Collections.Generic
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

Namespace org.nd4j.linalg.dataset


	''' <summary>
	''' Iterate over a dataset
	''' with views
	''' 
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class ViewIterator
		Implements DataSetIterator

		Private batchSize As Integer = -1
		Private cursor As Integer = 0
		Private data As DataSet
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor

		Public Sub New(ByVal data As DataSet, ByVal batchSize As Integer)
			Me.batchSize = batchSize
			Me.data = data
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As DataSet
			Throw New System.NotSupportedException("Only allowed to retrieve dataset based on batch size")
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return data.numInputs()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return data.numOutcomes()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			'Already all in memory
			Return False
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			cursor = 0
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return batchSize
		End Function

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

		Public Overrides Function hasNext() As Boolean
			Return cursor < data.numExamples()
		End Function

		Public Overrides Sub remove()
		End Sub
		Public Overrides Function [next]() As DataSet
			Dim last As Integer = Math.Min(data.numExamples(), cursor + batch())
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim next_Conflict As DataSet = DirectCast(data.getRange(cursor, last), DataSet)
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(next_Conflict)
			End If
			cursor += batch()
			Return next_Conflict
		End Function
	End Class

End Namespace