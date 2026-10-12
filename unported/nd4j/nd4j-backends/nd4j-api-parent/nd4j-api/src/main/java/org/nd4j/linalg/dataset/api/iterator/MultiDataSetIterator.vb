Imports System.Collections.Generic
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor

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


	Public Interface MultiDataSetIterator
		Inherits IEnumerator(Of MultiDataSet)

		''' <summary>
		''' Fetch the next 'num' examples. Similar to the next method, but returns a specified number of examples
		''' </summary>
		''' <param name="num"> Number of examples to fetch </param>
		Function [next](ByVal num As Integer) As MultiDataSet

		''' <summary>
		''' Set the preprocessor to be applied to each MultiDataSet, before each MultiDataSet is returned. </summary>
		''' <param name="preProcessor"> MultiDataSetPreProcessor. May be null. </param>
		Property PreProcessor As MultiDataSetPreProcessor



		''' <summary>
		''' Is resetting supported by this DataSetIterator? Many DataSetIterators do support resetting,
		''' but some don't
		''' </summary>
		''' <returns> true if reset method is supported; false otherwise </returns>
		Function resetSupported() As Boolean

		''' <summary>
		''' Does this MultiDataSetIterator support asynchronous prefetching of multiple MultiDataSet objects?
		''' Most MultiDataSetIterators do, but in some cases it may not make sense to wrap this iterator in an
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

	End Interface

End Namespace