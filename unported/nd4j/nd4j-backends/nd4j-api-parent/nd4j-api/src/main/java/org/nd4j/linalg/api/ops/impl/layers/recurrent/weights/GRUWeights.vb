Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GRUCell = org.nd4j.linalg.api.ops.impl.layers.recurrent.GRUCell

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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data @Builder public class GRUWeights extends RNNWeights
	Public Class GRUWeights
		Inherits RNNWeights

		''' <summary>
		''' Reset and Update gate weights, with a shape of [inSize + numUnits, 2*numUnits].
		''' 
		''' The reset weights are the [:, 0:numUnits] subset and the update weights are the [:, numUnits:2*numUnits] subset.
		''' </summary>
		Private ruWeight As SDVariable
		Private iRuWeights As INDArray

		''' <summary>
		''' Cell gate weights, with a shape of [inSize + numUnits, numUnits]
		''' </summary>
		Private cWeight As SDVariable
		Private iCWeight As INDArray

		''' <summary>
		''' Reset and Update gate bias, with a shape of [2*numUnits].  May be null.
		''' 
		''' The reset bias is the [0:numUnits] subset and the update bias is the [numUnits:2*numUnits] subset.
		''' </summary>
		Private ruBias As SDVariable
		Private iRUBias As INDArray

		''' <summary>
		''' Cell gate bias, with a shape of [numUnits].  May be null.
		''' </summary>
		Private cBias As SDVariable
		Private iCBias As INDArray

		Public Overrides Function args() As SDVariable()
			Return filterNonNull(ruWeight, cWeight, ruBias, cBias)
		End Function

		Public Overrides Function arrayArgs() As INDArray()
			Return filterNonNull(iRuWeights, iCWeight, iRUBias, iCBias)
		End Function
	End Class

End Namespace