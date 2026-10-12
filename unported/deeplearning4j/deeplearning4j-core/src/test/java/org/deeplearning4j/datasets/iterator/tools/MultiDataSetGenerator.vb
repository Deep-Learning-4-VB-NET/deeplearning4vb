Imports System
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
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
	Public Class MultiDataSetGenerator
		Implements MultiDataSetIterator

		Protected Friend ReadOnly shapeFeatures() As Integer
		Protected Friend ReadOnly shapeLabels() As Integer
		Protected Friend ReadOnly totalBatches As Long
		Protected Friend counter As New AtomicLong(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MultiDataSetGenerator(long numBatches, @NonNull int[] shapeFeatures, int[] shapeLabels)
		Public Sub New(ByVal numBatches As Long, ByVal shapeFeatures() As Integer, ByVal shapeLabels() As Integer)
			Me.shapeFeatures = shapeFeatures
			Me.shapeLabels = shapeLabels
			Me.totalBatches = numBatches
		End Sub

		Public Overridable Sub shift()
			counter.incrementAndGet()
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
    
			End Set
			Get
				Return Nothing
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			counter.set(0)
		End Sub

		Public Overrides Function hasNext() As Boolean
			Return counter.get() < totalBatches
		End Function

		Public Overrides Function [next]() As MultiDataSet
			Return New org.nd4j.linalg.dataset.MultiDataSet(New INDArray(){Nd4j.create(shapeFeatures).assign(counter.get())}, New INDArray(){Nd4j.create(shapeLabels).assign(counter.getAndIncrement())})
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub
	End Class

End Namespace