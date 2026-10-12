Imports LayerHelper = org.deeplearning4j.nn.layers.LayerHelper
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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

Namespace org.deeplearning4j.nn.conf.dropout

	Public Interface DropoutHelper
		Inherits LayerHelper

		''' <returns> Check if this dropout helper is supported in the current environment </returns>
		Function checkSupported() As Boolean

		''' <summary>
		''' Apply the dropout during forward pass </summary>
		''' <param name="inputActivations">       Input activations (pre dropout) </param>
		''' <param name="resultArray">            Output activations (post dropout). May be same as (or different to) input array </param>
		''' <param name="dropoutInputRetainProb"> Probability of retaining an activation </param>
		Sub applyDropout(ByVal inputActivations As INDArray, ByVal resultArray As INDArray, ByVal dropoutInputRetainProb As Double)

		''' <summary>
		''' Perform backpropagation. Note that the same dropout mask should be used for backprop as was used during the last
		''' call to <seealso cref="applyDropout(INDArray, INDArray, Double)"/> </summary>
		''' <param name="gradAtOutput"> Gradient at output (from perspective of forward pass) </param>
		''' <param name="gradAtInput">  Result array - gradient at input. May be same as (or different to) gradient at input </param>
		Sub backprop(ByVal gradAtOutput As INDArray, ByVal gradAtInput As INDArray)


	End Interface


End Namespace