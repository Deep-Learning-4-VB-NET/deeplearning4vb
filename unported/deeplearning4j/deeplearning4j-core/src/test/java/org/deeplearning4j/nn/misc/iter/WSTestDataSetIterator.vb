Imports System
Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.misc.iter


	<Serializable>
	Public Class WSTestDataSetIterator
		Implements DataSetIterator

		Friend cursor As Integer = 0
		Friend batchSize As Integer = 32

		Friend ReadOnly vectors As INDArray = Nd4j.rand(30, 300)

		Public Overridable Function [next](ByVal i As Integer) As DataSet Implements DataSetIterator.next
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.LinkedList<org.nd4j.linalg.dataset.DataSet> parts = new java.util.LinkedList<>();
			Dim parts As New LinkedList(Of DataSet)()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While parts.Count < i AndAlso hasNext()
				parts.AddLast(nextOne())
			Loop
			cursor += 1
			Return DataSet.merge(parts)
		End Function


		Public Overridable Function nextOne() As DataSet
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray features = org.nd4j.linalg.factory.Nd4j.create(1, 1, 10);
			Dim features As INDArray = Nd4j.create(1, 1, 10)
			For i As Integer = 0 To 9
				features.putScalar(1, 1, i, i)
			Next i

			Return New DataSet(features, vectors.getRow(7, True), Nd4j.ones(1, 10), Nothing)
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return 1
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return 300
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset

		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return 0
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal dataSetPreProcessor As DataSetPreProcessor)
    
			End Set
			Get
				Return Nothing
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			Return cursor < 10
		End Function

		Public Overrides Function [next]() As DataSet
			Return [next](batchSize)
		End Function

		Public Overrides Sub remove()

		End Sub
	End Class
End Namespace