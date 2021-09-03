Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NormalizerStats = org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats

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

Namespace org.nd4j.linalg.dataset.api.preprocessor


	Public Interface NormalizerStrategy(Of S As org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats)
		''' <summary>
		''' Normalize a data array
		''' </summary>
		''' <param name="array"> the data to normalize </param>
		''' <param name="stats"> statistics of the data population </param>
		Sub preProcess(ByVal array As INDArray, ByVal maskArray As INDArray, ByVal stats As S)

		''' <summary>
		''' Denormalize a data array
		''' </summary>
		''' <param name="array"> the data to denormalize </param>
		''' <param name="stats"> statistics of the data population </param>
		Sub revert(ByVal array As INDArray, ByVal maskArray As INDArray, ByVal stats As S)

		''' <summary>
		''' Create a new <seealso cref="NormalizerStats.Builder"/> instance that can be used to fit new data and of the opType that
		''' belongs to the current NormalizerStrategy implementation
		''' </summary>
		''' <returns> the new builder </returns>
		Function newStatsBuilder() As S.Builder
	End Interface

End Namespace