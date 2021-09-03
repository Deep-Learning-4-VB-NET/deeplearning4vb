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

Namespace org.deeplearning4j.datasets.iterator.tools


	<Serializable>
	Public Class SimpleVariableGenerator
		Implements DataSetIterator

		Private seed As Long
		Private numBatches As Integer
		Private batchSize As Integer
		Private numFeatures As Integer
		Private numLabels As Integer

		Private counter As New AtomicInteger(0)

		Public Sub New(ByVal seed As Long, ByVal numBatches As Integer, ByVal batchSize As Integer, ByVal numFeatures As Integer, ByVal numLabels As Integer)
			Me.seed = seed
			Me.numBatches = numBatches
			Me.batchSize = batchSize
			Me.numFeatures = numFeatures
			Me.numLabels = numLabels
		End Sub

		Public Overrides Function [next]() As DataSet
			Dim features As INDArray = Nd4j.create(batchSize, numFeatures).assign(counter.get())
			Dim labels As INDArray = Nd4j.create(batchSize, numFeatures).assign(counter.getAndIncrement() + 0.5)
			Nd4j.Executioner.commit()
			Return New DataSet(features, labels)
		End Function

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return numFeatures
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return numLabels
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			counter.set(0)
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return batchSize
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
    
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
			Return counter.get() < numBatches
		End Function

		Public Overrides Sub remove()

		End Sub
	End Class

End Namespace