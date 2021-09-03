Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.deeplearning4j.datasets.iterator.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BenchmarkDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.DataSetIterator
	<Serializable>
	Public Class BenchmarkDataSetIterator
		Implements DataSetIterator

		Private baseFeatures As INDArray
		Private baseLabels As INDArray
		Private limit As Long
		Private counter As New AtomicLong(0)

		''' <param name="featuresShape">   Shape of the features data to randomly generate </param>
		''' <param name="numLabels">       Number of label classes (for classification) </param>
		''' <param name="totalIterations"> Total number of iterations per epoch </param>
		Public Sub New(ByVal featuresShape() As Integer, ByVal numLabels As Integer, ByVal totalIterations As Integer)
			Me.New(featuresShape, numLabels, totalIterations, -1, -1)
		End Sub

		''' <summary>
		''' Creates 2d (shape [minibatch, numLabels]) or 4d labels ([minibatch, numLabels, gridWidth, gridHeight]),
		''' depending on value of gridWidth and gridHeight.
		''' </summary>
		''' <param name="featuresShape">   Shape of the features data to randomly generate </param>
		''' <param name="numLabels">       Number of label classes (for classification) </param>
		''' <param name="totalIterations"> Total number of iterations </param>
		''' <param name="gridWidth">       If > 0, use to create 4d labels </param>
		''' <param name="gridHeight">      If > 0, use to create 4d labels </param>
		Public Sub New(ByVal featuresShape() As Integer, ByVal numLabels As Integer, ByVal totalIterations As Integer, ByVal gridWidth As Integer, ByVal gridHeight As Integer)
			Me.baseFeatures = Nd4j.rand(featuresShape)
			Me.baseLabels = If(gridWidth > 0 AndAlso gridHeight > 0, Nd4j.create(featuresShape(0), numLabels, gridWidth, gridHeight), Nd4j.create(featuresShape(0), numLabels))
			If Me.baseLabels.rank() = 2 Then
				Me.baseLabels.getColumn(1).assign(1.0)
			Else
				Me.baseLabels.get(NDArrayIndex.all(), NDArrayIndex.point(1), NDArrayIndex.all(), NDArrayIndex.all())
			End If

			Nd4j.Executioner.commit()
			Me.limit = totalIterations
		End Sub

		''' <param name="example">         DataSet to return on each call of next() </param>
		''' <param name="totalIterations"> Total number of iterations </param>
		Public Sub New(ByVal example As DataSet, ByVal totalIterations As Integer)
			Me.baseFeatures = example.Features.dup()
			Me.baseLabels = example.Labels.dup()

			Nd4j.Executioner.commit()
			Me.limit = totalIterations
		End Sub

		Public Overridable Function [next](ByVal i As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.NotSupportedException()
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
			Me.counter.set(0)
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

		''' <summary>
		''' Returns {@code true} if the iteration has more elements.
		''' (In other words, returns {@code true} if <seealso cref="next"/> would
		''' return an element rather than throwing an exception.)
		''' </summary>
		''' <returns> {@code true} if the iteration has more elements </returns>
		Public Overrides Function hasNext() As Boolean
			Return counter.get() < limit
		End Function

		''' <summary>
		''' Returns the next element in the iteration.
		''' </summary>
		''' <returns> the next element in the iteration </returns>
		Public Overrides Function [next]() As DataSet
			counter.incrementAndGet()

			Dim ds As New DataSet(baseFeatures, baseLabels)

			Return ds
		End Function

		''' <summary>
		''' Removes from the underlying collection the last element returned
		''' by this iterator (optional operation).  This method can be called
		''' only once per call to <seealso cref="next"/>.  The behavior of an iterator
		''' is unspecified if the underlying collection is modified while the
		''' iteration is in progress in any way other than by calling this
		''' method.
		''' </summary>
		''' <exception cref="UnsupportedOperationException"> if the {@code remove}
		'''                                       operation is not supported by this iterator </exception>
		''' <exception cref="IllegalStateException">         if the {@code next} method has not
		'''                                       yet been called, or the {@code remove} method has already
		'''                                       been called after the last call to the {@code next}
		'''                                       method
		''' @implSpec The default implementation throws an instance of
		''' <seealso cref="System.NotSupportedException"/> and performs no other action. </exception>
		Public Overrides Sub remove()

		End Sub
	End Class

End Namespace