Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.nd4j.linalg.dataset.api.iterator.fetcher


	Public Interface DataSetFetcher

		''' <summary>
		''' Whether the dataset has more to load
		''' </summary>
		''' <returns> whether the data applyTransformToDestination has more to load </returns>
		Function hasMore() As Boolean

		''' <summary>
		''' Returns the next data applyTransformToDestination
		''' </summary>
		''' <returns> the next dataset </returns>
		Function [next]() As DataSet

		''' <summary>
		''' Fetches the next dataset. You need to call this
		''' to getFromOrigin a new dataset, otherwise <seealso cref="next()"/>
		''' just returns the last data applyTransformToDestination fetch
		''' </summary>
		''' <param name="numExamples"> the number of examples to fetch </param>
		Sub fetch(ByVal numExamples As Integer)

		''' <summary>
		''' The number of labels for a dataset
		''' </summary>
		''' <returns> the number of labels for a dataset </returns>
		Function totalOutcomes() As Integer

		''' <summary>
		''' The length of a feature vector for an individual example
		''' </summary>
		''' <returns> the length of a feature vector for an individual example </returns>
		Function inputColumns() As Integer

		''' <summary>
		''' The total number of examples
		''' </summary>
		''' <returns> the total number of examples </returns>
		Function totalExamples() As Integer

		''' <summary>
		''' Returns the fetcher back to the beginning of the dataset
		''' </summary>
		Sub reset()

		''' <summary>
		''' Direct access to a number represenative of iterating through a dataset
		''' </summary>
		''' <returns> a cursor similar to an index </returns>
		Function cursor() As Integer
	End Interface

End Namespace