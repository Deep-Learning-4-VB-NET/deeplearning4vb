Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LSTMLayer = org.nd4j.linalg.api.ops.impl.layers.recurrent.LSTMLayer
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil

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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @Builder public class LSTMLayerWeights extends RNNWeights
	Public Class LSTMLayerWeights
		Inherits RNNWeights

		''' <summary>
		''' Input to hidden weights with a shape of [inSize, 4*numUnits].
		''' 
		''' Input to hidden and hidden to hidden are concatenated in dimension 0,
		''' so the input to hidden weights are [:inSize, :] and the hidden to hidden weights are [inSize:, :].
		''' </summary>
		Private weights As SDVariable
		Private iWeights As INDArray

		''' <summary>
		''' hidden to hidden weights (aka "recurrent weights", with a shape of [numUnits, 4*numUnits].
		''' 
		''' </summary>
		Private rWeights As SDVariable
		Private irWeights As INDArray

		''' <summary>
		''' Peephole weights, with a shape of [3*numUnits].
		''' </summary>
		Private peepholeWeights As SDVariable
		Private iPeepholeWeights As INDArray

		''' <summary>
		''' Input to hidden and hidden to hidden biases, with shape [4*numUnits].
		''' </summary>
		Private bias As SDVariable
		Private iBias As INDArray

		Public Overrides Function args() As SDVariable()
			Return filterNonNull(weights, rWeights, peepholeWeights, bias)
		End Function

		Public Overrides Function arrayArgs() As INDArray()
			Return filterNonNull(iWeights, irWeights, iPeepholeWeights, iBias)
		End Function

		Public Overrides Function argsWithInputs(ParamArray ByVal inputs() As SDVariable) As SDVariable()
			Preconditions.checkArgument(inputs.Length = 4, "Expected 4 inputs, got %s", inputs.Length) 'Order: x, seqLen, yLast, cLast
			'lstmLayer c++ op expects: x, Wx, Wr, Wp, b, seqLen, yLast, cLast
			Return ArrayUtil.filterNull(inputs(0), weights, rWeights, bias, inputs(1), inputs(2), inputs(3), peepholeWeights)
		End Function

		Public Overrides Function argsWithInputs(ParamArray ByVal inputs() As INDArray) As INDArray()
			Preconditions.checkArgument(inputs.Length = 4, "Expected 4 inputs, got %s", inputs.Length) 'Order: x, seqLen, yLast, cLast
			'lstmLayer c++ op expects: x, Wx, Wr, Wp, b, seqLen, yLast, cLast
			Return ArrayUtil.filterNull(inputs(0), iWeights, irWeights, iBias, inputs(1), inputs(2), inputs(3), iPeepholeWeights)
		End Function


		Public Overridable Function hasBias() As Boolean
			Return (bias IsNot Nothing OrElse iBias IsNot Nothing)
		End Function

		Public Overridable Function hasPH() As Boolean
			Return (peepholeWeights IsNot Nothing OrElse iPeepholeWeights IsNot Nothing)
		End Function

	End Class

End Namespace