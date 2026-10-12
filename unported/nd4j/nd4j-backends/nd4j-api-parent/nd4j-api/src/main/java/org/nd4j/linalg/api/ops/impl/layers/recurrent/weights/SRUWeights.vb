Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports SRU = org.nd4j.linalg.api.ops.impl.layers.recurrent.SRU
Imports SRUCell = org.nd4j.linalg.api.ops.impl.layers.recurrent.SRUCell

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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @Builder public class SRUWeights extends RNNWeights
	Public Class SRUWeights
		Inherits RNNWeights

		''' <summary>
		''' Weights, with shape [inSize, 3*inSize].
		''' </summary>
		Private weights As SDVariable

		Private iWeights As INDArray

		''' <summary>
		''' Biases, with shape [2*inSize].
		''' </summary>
		Private bias As SDVariable

		Private iBias As INDArray

		Public Overrides Function args() As SDVariable()
			Return New SDVariable(){weights, bias}
		End Function

		Public Overrides Function arrayArgs() As INDArray()
			Return New INDArray(){iWeights, iBias}
		End Function
	End Class

End Namespace