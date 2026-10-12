Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
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
	Public Class DataSetGenerator
		Implements DataSetIterator

		Protected Friend ReadOnly shapeFeatures() As Integer
		Protected Friend ReadOnly shapeLabels() As Integer
		Protected Friend ReadOnly totalBatches As Long
		Protected Friend counter As New AtomicLong(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DataSetGenerator(long numBatches, @NonNull int[] shapeFeatures, int[] shapeLabels)
		Public Sub New(ByVal numBatches As Long, ByVal shapeFeatures() As Integer, ByVal shapeLabels() As Integer)
			Me.shapeFeatures = shapeFeatures
			Me.shapeLabels = shapeLabels
			Me.totalBatches = numBatches
		End Sub

		Public Overridable Function [next](ByVal i As Integer) As DataSet Implements DataSetIterator.next
			Return Nothing
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return 0
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return 0
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
			Return counter.get() < totalBatches
		End Function

		Public Overrides Function [next]() As DataSet
			Return New DataSet(Nd4j.create(shapeFeatures).assign(counter.get()), Nd4j.create(shapeLabels).assign(counter.getAndIncrement()))
		End Function

		Public Overrides Sub remove()

		End Sub

		Public Overridable Sub shift()
			counter.incrementAndGet()
		End Sub
	End Class

End Namespace