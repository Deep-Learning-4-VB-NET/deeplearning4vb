Imports System
Imports NonNull = lombok.NonNull
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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
	Public Class AsyncShieldMultiDataSetIterator
		Implements MultiDataSetIterator

		Private backingIterator As MultiDataSetIterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AsyncShieldMultiDataSetIterator(@NonNull MultiDataSetIterator iterator)
		Public Sub New(ByVal iterator As MultiDataSetIterator)
			Me.backingIterator = iterator
		End Sub

		''' <summary>
		''' Fetch the next 'num' examples. Similar to the next method, but returns a specified number of examples
		''' </summary>
		''' <param name="num"> Number of examples to fetch </param>
		Public Overridable Function [next](ByVal num As Integer) As MultiDataSet Implements MultiDataSetIterator.next
			Return backingIterator.next(num)
		End Function

		''' <summary>
		''' Set the preprocessor to be applied to each MultiDataSet, before each MultiDataSet is returned.
		''' </summary>
		''' <param name="preProcessor"> MultiDataSetPreProcessor. May be null. </param>
		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal preProcessor As MultiDataSetPreProcessor)
				backingIterator.PreProcessor = preProcessor
			End Set
			Get
				Return backingIterator.PreProcessor
			End Get
		End Property


		''' <summary>
		''' Is resetting supported by this DataSetIterator? Many DataSetIterators do support resetting,
		''' but some don't
		''' </summary>
		''' <returns> true if reset method is supported; false otherwise </returns>
		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return backingIterator.resetSupported()
		End Function

		''' <summary>
		''' /**
		''' Does this DataSetIterator support asynchronous prefetching of multiple DataSet objects?
		''' 
		''' PLEASE NOTE: This iterator ALWAYS returns FALSE
		''' </summary>
		''' <returns> true if asynchronous prefetching from this iterator is OK; false if asynchronous prefetching should not
		''' be used with this iterator </returns>
		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return False
		End Function

		''' <summary>
		''' Resets the iterator back to the beginning
		''' </summary>
		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			backingIterator.reset()
		End Sub

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
		Public Overrides Function [next]() As MultiDataSet
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