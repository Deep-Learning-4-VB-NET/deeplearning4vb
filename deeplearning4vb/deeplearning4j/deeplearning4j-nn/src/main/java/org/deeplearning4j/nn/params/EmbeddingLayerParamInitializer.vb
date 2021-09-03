Imports val = lombok.val
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
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
Namespace org.deeplearning4j.nn.params

	Public Class EmbeddingLayerParamInitializer
		Inherits DefaultParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New EmbeddingLayerParamInitializer()

		Public Shared ReadOnly Property Instance As EmbeddingLayerParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property



		Protected Friend Overrides Function createWeightMatrix(ByVal nIn As Long, ByVal nOut As Long, ByVal weightInit As IWeightInit, ByVal weightParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			Dim shape As val = New Long() {nIn, nOut}

			If initializeParameters Then
				Dim ret As INDArray = weightInit.init(1, nOut, shape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, weightParamView)
				Return ret
			Else
				Return WeightInitUtil.reshapeWeights(shape, weightParamView)
			End If
		End Function

	End Class

End Namespace