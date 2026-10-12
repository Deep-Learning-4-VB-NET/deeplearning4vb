Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
	Public Class WorkspacesShieldDataSetIterator
		Implements DataSetIterator

		Protected Friend iterator As DataSetIterator

		''' <param name="iterator"> The underlying iterator to detach values from </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public WorkspacesShieldDataSetIterator(@NonNull DataSetIterator iterator)
		Public Sub New(ByVal iterator As DataSetIterator)
			Me.iterator = iterator
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return iterator.inputColumns()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return iterator.totalOutcomes()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return iterator.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return iterator.asyncSupported()
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			iterator.reset()
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return iterator.batch()
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				iterator.PreProcessor = preProcessor
			End Set
			Get
				Return iterator.PreProcessor
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return iterator.getLabels()
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iterator.hasNext()
		End Function

		Public Overrides Function [next]() As DataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iterator.next()

			If ds.Features.Attached Then
				If Nd4j.MemoryManager.CurrentWorkspace Is Nothing Then
					ds.detach()
				Else
					ds.migrate()
				End If
			End If

			Return ds
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub
	End Class

End Namespace