Imports System
Imports System.Collections.Generic
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports SameDiffOutputLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffOutputLayer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Activation = org.nd4j.linalg.activations.Activation
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
	Public Class SameDiffMSEOutputLayer
		Inherits SameDiffOutputLayer

		Private nIn As Integer
		Private nOut As Integer
		Private activation As Activation
		Private weightInit As WeightInit

		Public Sub New(ByVal nIn As Integer, ByVal nOut As Integer, ByVal activation As Activation, ByVal weightInit As WeightInit)
			Me.nIn = nIn
			Me.nOut = nOut
			Me.activation = activation
			Me.weightInit = weightInit
		End Sub

		Public Overrides Function defineLayer(ByVal sameDiff As SameDiff, ByVal layerInput As SDVariable, ByVal labels As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable)) As SDVariable
			Dim z As SDVariable = sameDiff.mmul(layerInput, paramTable("W")).add(paramTable("b"))
			Dim [out] As SDVariable = activation.asSameDiff("out", sameDiff, z)
			'MSE: 1/nOut * (input-labels)^2
			Dim diff As SDVariable = [out].sub(labels)
			Return diff.mul(diff).mean(1).sum()
		End Function

		Public Overrides Function activationsVertexName() As String
			Return "out"
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			params.addWeightParam("W", nIn, nOut)
			params.addBiasParam("b", 1, nOut)
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			WeightInitUtil.initWeights(nIn, nOut, New Long(){nIn, nOut}, weightInit, Nothing, "f"c, params("W"))
			params("b").assign(0.0)
		End Sub

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return InputType.feedForward(nOut)
		End Function

		Public Overrides Function paramReshapeOrder(ByVal param As String) As Char
			'To match DL4J for easy comparison
			Return "f"c
		End Function

		Public Overrides Sub applyGlobalConfigToLayer(ByVal globalConfig As NeuralNetConfiguration.Builder)

		End Sub

	End Class

End Namespace