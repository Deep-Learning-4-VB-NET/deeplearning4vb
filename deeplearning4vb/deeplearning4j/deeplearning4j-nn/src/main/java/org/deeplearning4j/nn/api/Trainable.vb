Imports System.Collections.Generic
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

Namespace org.deeplearning4j.nn.api


	Public Interface Trainable

		''' <returns> Training configuration </returns>
		ReadOnly Property Config As TrainingConfig

		''' <returns> Number of parameters </returns>
		Function numParams() As Long

		''' <returns> 1d parameter vector </returns>
		Function params() As INDArray

		''' <param name="backpropOnly"> If true: return only parameters that are not exclusively used for layerwise pretraining </param>
		''' <returns> Parameter table </returns>
		Function paramTable(ByVal backpropOnly As Boolean) As IDictionary(Of String, INDArray)

		''' <summary>
		''' DL4J layers typically produce the sum of the gradients during the backward pass for each layer, and if required
		''' (if minibatch=true) then divide by the minibatch size.<br>
		''' However, there are some exceptions, such as the batch norm mean/variance estimate parameters: these "gradients"
		''' are actually not gradients, but are updates to be applied directly to the parameter vector. Put another way,
		''' most gradients should be divided by the minibatch to get the average; some "gradients" are actually final updates
		''' already, and should not be divided by the minibatch size.
		''' </summary>
		''' <param name="paramName"> Name of the parameter </param>
		''' <returns> True if gradients should be divided by minibatch (most params); false otherwise (edge cases like batch norm mean/variance estimates) </returns>
		Function updaterDivideByMinibatch(ByVal paramName As String) As Boolean

		''' <returns> 1D gradients view array </returns>
		ReadOnly Property GradientsViewArray As INDArray

	End Interface

End Namespace