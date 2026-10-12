Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports LegacyIActivationDeserializerHelper = org.nd4j.serde.json.LegacyIActivationDeserializerHelper
Imports JsonAutoDetect = org.nd4j.shade.jackson.annotation.JsonAutoDetect
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.nd4j.linalg.activations


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class", defaultImpl = LegacyIActivationDeserializerHelper.class) @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY, getterVisibility = JsonAutoDetect.Visibility.NONE, setterVisibility = JsonAutoDetect.Visibility.NONE) public interface IActivation extends java.io.Serializable
	Public Interface IActivation

		''' <summary>
		''' Carry out activation function on the input array (usually known as 'preOut' or 'z')
		''' Implementations must overwrite "in", transform in place and return "in"
		''' Can support separate behaviour during test </summary>
		''' <param name="in"> input array. </param>
		''' <param name="training"> true when training. </param>
		''' <returns> transformed activation </returns>
		Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray

		''' <summary>
		''' Backpropagate the errors through the activation function, given input z and epsilon dL/da.<br>
		''' Returns 2 INDArrays:<br>
		''' (a) The gradient dL/dz, calculated from dL/da, and<br>
		''' (b) The parameter gradients dL/dW, where w is the weights in the activation function. For activation functions
		'''     with no gradients, this will be null.
		''' </summary>
		''' <param name="in">      Input, before applying the activation function (z, or 'preOut') </param>
		''' <param name="epsilon"> Gradient to be backpropagated: dL/da, where L is the loss function </param>
		''' <returns>        dL/dz and dL/dW, for weights w (null if activation function has no weights) </returns>
		Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray)


		Function numParams(ByVal inputSize As Integer) As Integer

	End Interface

End Namespace