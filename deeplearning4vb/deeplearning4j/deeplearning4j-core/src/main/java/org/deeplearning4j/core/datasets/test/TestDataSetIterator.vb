Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports DataSet = org.nd4j.linalg.dataset.DataSet
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

Namespace org.deeplearning4j.core.datasets.test


	<Serializable>
	Public Class TestDataSetIterator
		Implements DataSetIterator

		''' 
		Private Const serialVersionUID As Long = -3042802726018263331L
		Private wrapped As DataSetIterator
'JAVA TO VB CONVERTER NOTE: The field numDataSets was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private numDataSets_Conflict As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor


		Public Sub New(ByVal wrapped As DataSetIterator)
			MyBase.New()
			Me.wrapped = wrapped
		End Sub

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return wrapped.hasNext()
		End Function

		Public Overrides Function [next]() As DataSet
			numDataSets_Conflict += 1
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim next_Conflict As DataSet = wrapped.next()
			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(next_Conflict)
			End If
			Return next_Conflict
		End Function

		Public Overrides Sub remove()
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
			wrapped.remove()
		End Sub

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return wrapped.inputColumns()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return wrapped.totalOutcomes()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return wrapped.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return wrapped.asyncSupported()
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			wrapped.reset()
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return wrapped.batch()
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


		Public Overridable ReadOnly Property NumDataSets As Integer
			Get
				SyncLock Me
					Return numDataSets_Conflict
				End SyncLock
			End Get
		End Property

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Return wrapped.next(num)
		End Function



	End Class

End Namespace