﻿Imports System
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

Namespace org.deeplearning4j.datasets.iterator



	''' <summary>
	''' Wraps a data set iterator setting the first (feature matrix) as the labels.
	''' 
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class ReconstructionDataSetIterator
		Implements DataSetIterator

		Private iter As DataSetIterator
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor

		Public Sub New(ByVal iter As DataSetIterator)
			Me.iter = iter
		End Sub

		''' <summary>
		''' Like the standard next method but allows a
		''' customizable number of examples returned
		''' </summary>
		''' <param name="num"> the number of examples </param>
		''' <returns> the next data applyTransformToDestination </returns>
		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Dim ret As DataSet = iter.next(num)
			ret.Labels = ret.Features
			Return ret
		End Function

		''' <summary>
		''' Input columns for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return iter.inputColumns()
		End Function

		''' <summary>
		''' The number of labels for the dataset
		''' 
		''' @return
		''' </summary>
		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return iter.totalOutcomes()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return iter.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return iter.asyncSupported()
		End Function

		''' <summary>
		''' Resets the iterator back to the beginning
		''' </summary>
		Public Overridable Sub reset() Implements DataSetIterator.reset
			iter.reset()
		End Sub

		''' <summary>
		''' Batch size
		''' 
		''' @return
		''' </summary>
		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return iter.batch()
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


		''' <summary>
		''' Returns {@code true} if the iteration has more elements.
		''' (In other words, returns {@code true} if <seealso cref="next"/> would
		''' return an element rather than throwing an exception.)
		''' </summary>
		''' <returns> {@code true} if the iteration has more elements </returns>
		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iter.hasNext()
		End Function

		''' <summary>
		''' Returns the next element in the iteration.
		''' </summary>
		''' <returns> the next element in the iteration </returns>
		Public Overrides Function [next]() As DataSet
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim next_Conflict As DataSet = iter.next()
			next_Conflict.Labels = next_Conflict.Features
			Return next_Conflict
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
		'''                                       method </exception>
		Public Overrides Sub remove()
'JAVA TO VB CONVERTER TODO TASK: .NET enumerators are read-only:
			iter.remove()
		End Sub
	End Class

End Namespace