Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LSTMBlockCell = org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMBlockCell
Imports LSTMLayer = org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayer

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

Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent.weights

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @Builder public class LSTMWeights extends RNNWeights
	Public Class LSTMWeights
		Inherits RNNWeights

		''' <summary>
		''' Input to hidden weights and hidden to hidden weights, with a shape of [inSize + numUnits, 4*numUnits].
		''' 
		''' Input to hidden and hidden to hidden are concatenated in dimension 0,
		''' so the input to hidden weights are [:inSize, :] and the hidden to hidden weights are [inSize:, :].
		''' </summary>
		Private weights As SDVariable
		Private iWeights As INDArray

		''' <summary>
		''' Cell peephole (t-1) connections to input modulation gate, with a shape of [numUnits].
		''' </summary>
		Private inputPeepholeWeights As SDVariable
		Private iInputPeepholeWeights As INDArray

		''' <summary>
		''' Cell peephole (t-1) connections to forget gate, with a shape of [numUnits].
		''' </summary>
		Private forgetPeepholeWeights As SDVariable
		Private iForgetPeepholeWeights As INDArray

		''' <summary>
		''' Cell peephole (t) connections to output gate, with a shape of [numUnits].
		''' </summary>
		Private outputPeepholeWeights As SDVariable
		Private iOutputPeepholeWeights As INDArray

		''' <summary>
		''' Input to hidden and hidden to hidden biases, with shape [1, 4*numUnits].
		''' </summary>
		Private bias As SDVariable
		Private iBias As INDArray

		Public Overrides Function args() As SDVariable()
			Return filterNonNull(weights, inputPeepholeWeights, forgetPeepholeWeights, outputPeepholeWeights, bias)
		End Function

		Public Overrides Function arrayArgs() As INDArray()
			Return filterNonNull(iWeights, iInputPeepholeWeights, iForgetPeepholeWeights, iOutputPeepholeWeights, iBias)
		End Function
	End Class

End Namespace