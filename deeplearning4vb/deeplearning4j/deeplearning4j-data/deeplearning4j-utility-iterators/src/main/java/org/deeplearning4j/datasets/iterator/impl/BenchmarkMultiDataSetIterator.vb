Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.MultiDataSet
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

Namespace org.deeplearning4j.datasets.iterator.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class BenchmarkMultiDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
	<Serializable>
	Public Class BenchmarkMultiDataSetIterator
		Implements MultiDataSetIterator

		Private baseFeatures() As INDArray
		Private baseLabels() As INDArray
		Private limit As Long
		Private counter As New AtomicLong(0)

		Public Sub New(ByVal featuresShape()() As Integer, ByVal numLabels() As Integer, ByVal totalIterations As Integer)
			If featuresShape.Length <> numLabels.Length Then
				Throw New System.ArgumentException("Number of input features must match length of input labels.")
			End If

			Me.baseFeatures = New INDArray(featuresShape.Length - 1){}
			For i As Integer = 0 To featuresShape.Length - 1
				baseFeatures(i) = Nd4j.rand(featuresShape(i))
			Next i
			Me.baseLabels = New INDArray(featuresShape.Length - 1){}
			For i As Integer = 0 To featuresShape.Length - 1
				baseLabels(i) = Nd4j.create(featuresShape(i)(0), numLabels(i))
				baseLabels(i).getColumn(1).assign(1.0)
			Next i

			Nd4j.Executioner.commit()
			Me.limit = totalIterations
		End Sub

		Public Sub New(ByVal example As MultiDataSet, ByVal totalIterations As Integer)
			Me.baseFeatures = New INDArray(example.Features.Length - 1){}
			Dim i As Integer = 0
			Do While i < example.Features.Length
				baseFeatures(i) = example.Features(i).dup()
				i += 1
			Loop
			Me.baseLabels = New INDArray(example.Labels.Length - 1){}
			i = 0
			Do While i < example.Labels.Length
				baseFeatures(i) = example.Labels(i).dup()
				i += 1
			Loop

			Nd4j.Executioner.commit()
			Me.limit = totalIterations
		End Sub

		Public Overridable Function [next](ByVal i As Integer) As MultiDataSet
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			Me.counter.set(0)
		End Sub

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal dataSetPreProcessor As MultiDataSetPreProcessor)
    
			End Set
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
		Public Overrides Function [next]() As MultiDataSet
			counter.incrementAndGet()

			Dim features(baseFeatures.Length - 1) As INDArray
			Array.Copy(baseFeatures, 0, features, 0, baseFeatures.Length)
			Dim labels(baseLabels.Length - 1) As INDArray
			Array.Copy(baseLabels, 0, labels, 0, baseLabels.Length)

			Dim ds As New MultiDataSet(features, labels)

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