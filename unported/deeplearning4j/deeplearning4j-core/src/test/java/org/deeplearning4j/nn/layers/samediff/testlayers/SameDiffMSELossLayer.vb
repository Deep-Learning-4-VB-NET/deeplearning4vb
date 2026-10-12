Imports System
Imports System.Collections.Generic
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffOutputLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffOutputLayer
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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

Namespace org.deeplearning4j.nn.layers.samediff.testlayers


	<Serializable>
	Public Class SameDiffMSELossLayer
		Inherits SameDiffOutputLayer

		Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal labels As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable)) As SDVariable
			'MSE: 1/nOut * (input-labels)^2
			Dim diff As SDVariable = layerInput.sub(labels)
			Return diff.mul(diff).mean(1).sum(0)
		End Function

		Public Overrides Function activationsVertexName() As String
			Return "input"
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			'No op for loss layer (no params)
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			'No op for loss layer (no params)
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return Nothing
		End Function
	End Class

End Namespace