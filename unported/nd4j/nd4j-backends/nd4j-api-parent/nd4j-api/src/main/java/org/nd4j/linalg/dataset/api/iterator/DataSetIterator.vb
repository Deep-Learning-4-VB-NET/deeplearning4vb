Imports System.Collections.Generic
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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

Namespace org.nd4j.linalg.dataset.api.iterator



	Public Interface DataSetIterator
		Inherits IEnumerator(Of DataSet)

		''' <summary>
		''' Like the standard next method but allows a
		''' customizable number of examples returned
		''' </summary>
		''' <param name="num"> the number of examples </param>
		''' <returns> the next data applyTransformToDestination </returns>
		Function [next](ByVal num As Integer) As DataSet

		''' <summary>
		''' Input columns for the dataset
		''' 
		''' @return
		''' </summary>
		Function inputColumns() As Integer

		''' <summary>
		''' The number of labels for the dataset
		''' 
		''' @return
		''' </summary>
		Function totalOutcomes() As Integer


		''' <summary>
		''' Is resetting supported by this DataSetIterator? Many DataSetIterators do support resetting,
		''' but some don't
		''' </summary>
		''' <returns> true if reset method is supported; false otherwise </returns>
		Function resetSupported() As Boolean

		''' <summary>
		''' Does this DataSetIterator support asynchronous prefetching of multiple DataSet objects?
		''' Most DataSetIterators do, but in some cases it may not make sense to wrap this iterator in an
		''' iterator that does asynchronous prefetching. For example, it would not make sense to use asynchronous
		''' prefetching for the following types of iterators:
		''' (a) Iterators that store their full contents in memory already
		''' (b) Iterators that re-use features/labels arrays (as future next() calls will overwrite past contents)
		''' (c) Iterators that already implement some level of asynchronous prefetching
		''' (d) Iterators that may return different data depending on when the next() method is called
		''' </summary>
		''' <returns> true if asynchronous prefetching from this iterator is OK; false if asynchronous prefetching should not
		''' be used with this iterator </returns>
		Function asyncSupported() As Boolean

		''' <summary>
		''' Resets the iterator back to the beginning
		''' </summary>
		Sub reset()

		''' <summary>
		''' Batch size
		''' 
		''' @return
		''' </summary>
		Function batch() As Integer

		''' <summary>
		''' Set a pre processor
		''' </summary>
		''' <param name="preProcessor"> a pre processor to set </param>
		Property PreProcessor As DataSetPreProcessor


		''' <summary>
		''' Get dataset iterator class labels, if any.
		''' Note that implementations are not required to implement this, and can simply return null
		''' </summary>
		ReadOnly Property Labels As IList(Of String)

	End Interface

End Namespace