Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
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

Namespace org.deeplearning4j.datasets.iterator



	<Serializable>
	Public Class AsyncShieldDataSetIterator
		Implements DataSetIterator

		Private backingIterator As DataSetIterator

		''' <param name="iterator"> Iterator to wrop, to disable asynchronous prefetching for </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AsyncShieldDataSetIterator(@NonNull DataSetIterator iterator)
		Public Sub New(ByVal iterator As DataSetIterator)
			Me.backingIterator = iterator
		End Sub

		''' <summary>
		''' Like the standard next method but allows a
		''' customizable number of examples returned
		''' </summary>
		''' <param name="num"> the number of examples </param>
		''' <returns> the next data applyTransformToDestination </returns>
		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Return backingIterator.next(num)
		End Function

		''' <summary>
		''' Input columns for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return backingIterator.inputColumns()
		End Function

		''' <summary>
		''' The number of labels for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return backingIterator.totalOutcomes()
		End Function

		''' <summary>
		''' Is resetting supported by this DataSetIterator? Many DataSetIterators do support resetting,
		''' but some don't
		''' </summary>
		''' <returns> true if reset method is supported; false otherwise </returns>
		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return backingIterator.resetSupported()
		End Function

		''' <summary>
		''' Does this DataSetIterator support asynchronous prefetching of multiple DataSet objects?
		''' 
		''' PLEASE NOTE: This iterator ALWAYS returns FALSE
		''' </summary>
		''' <returns> true if asynchronous prefetching from this iterator is OK; false if asynchronous prefetching should not
		''' be used with this iterator </returns>
		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return False
		End Function

		''' <summary>
		''' Resets the iterator back to the beginning
		''' </summary>
		Public Overridable Sub reset() Implements DataSetIterator.reset
			backingIterator.reset()
		End Sub

		''' <summary>
		''' Batch size
		''' 
		''' @return
		''' </summary>
		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return backingIterator.batch()
		End Function

		''' <summary>
		''' Set a pre processor
		''' </summary>
		''' <param name="preProcessor"> a pre processor to set </param>
		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				backingIterator.PreProcessor = preProcessor
			End Set
			Get
				Return backingIterator.PreProcessor
			End Get
		End Property


		''' <summary>
		''' Get dataset iterator record reader labels
		''' </summary>
		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return backingIterator.getLabels()
			End Get
		End Property

		''' <summary>
		''' Returns {@code true} if the iteration has more elements.
		''' (In other words, returns {@code true} if <seealso cref="next"/> would
		''' return an element rather than throwing an exception.)
		''' </summary>
		''' <returns> {@code true} if the iteration has more elements </returns>
		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return backingIterator.hasNext()
		End Function

		''' <summary>
		''' Returns the next element in the iteration.
		''' </summary>
		''' <returns> the next element in the iteration </returns>
		Public Overrides Function [next]() As DataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return backingIterator.next()
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
			' no-op
		End Sub
	End Class

End Namespace