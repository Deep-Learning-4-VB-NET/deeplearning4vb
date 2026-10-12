Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports SameDiffLayer = org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
Imports SDLayerParams = org.deeplearning4j.nn.conf.layers.samediff.SDLayerParams
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class MinimalSameDiffDense extends org.deeplearning4j.nn.conf.layers.samediff.SameDiffLayer
	<Serializable>
	Public Class MinimalSameDiffDense
		Inherits SameDiffLayer

		Private nIn As Integer
		Private nOut As Integer
		Private activation As Activation

		Public Sub New(ByVal nIn As Integer, ByVal nOut As Integer, ByVal activation As Activation, ByVal weightInit As WeightInit)
			Me.nIn = nIn
			Me.nOut = nOut
			Me.activation = activation
			Me.weightInit = weightInit
		End Sub

		Protected Friend Sub New()
			'For JSON serialization
		End Sub

		Public Overrides Function defineLayer(ByVal sd As SameDiff, ByVal layerInput As SDVariable, ByVal paramTable As IDictionary(Of String, SDVariable), ByVal mask As SDVariable) As SDVariable
			Dim weights As SDVariable = paramTable(DefaultParamInitializer.WEIGHT_KEY)
			Dim bias As SDVariable = paramTable(DefaultParamInitializer.BIAS_KEY)

			Dim mmul As SDVariable = sd.mmul("mmul", layerInput, weights)
			Dim z As SDVariable = mmul.add("z", bias)
			Return activation.asSameDiff("out", sd, z)
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return InputType.feedForward(nOut)
		End Function

		Public Overrides Sub defineParameters(ByVal params As SDLayerParams)
			params.addWeightParam(DefaultParamInitializer.WEIGHT_KEY, nIn, nOut)
			params.addBiasParam(DefaultParamInitializer.BIAS_KEY, 1, nOut)
		End Sub

		Public Overrides Sub initializeParameters(ByVal params As IDictionary(Of String, INDArray))
			Dim b As String = DefaultParamInitializer.BIAS_KEY
			If paramWeightInit IsNot Nothing AndAlso paramWeightInit.ContainsKey(b) Then
				paramWeightInit(b).init(nIn, nOut, params(b).shape(), "c"c, params(b))
			Else
				params(DefaultParamInitializer.BIAS_KEY).assign(0)
			End If

			Dim w As String = DefaultParamInitializer.WEIGHT_KEY
			If paramWeightInit IsNot Nothing AndAlso paramWeightInit.ContainsKey(w) Then
				paramWeightInit(w).init(nIn, nOut, params(w).shape(), "c"c, params(w))
			Else
				initWeights(nIn, nOut, weightInit, params(DefaultParamInitializer.WEIGHT_KEY))
			End If
		End Sub

		'OPTIONAL methods:
	'    public void setNIn(InputType inputType, boolean override)
	'    public InputPreProcessor getPreProcessorForInputType(InputType inputType)
	'    public void applyGlobalConfigToLayer(NeuralNetConfiguration.Builder globalConfig)
	End Class

End Namespace