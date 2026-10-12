Imports System.Collections.Generic
Imports val = lombok.val
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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


	''' <summary>
	''' Pretrain weight initializer.
	''' Has the visible bias as well as hidden and weight matrix.
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class PretrainParamInitializer
		Inherits DefaultParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New PretrainParamInitializer()

		Public Shared ReadOnly Property Instance As PretrainParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Shared ReadOnly VISIBLE_BIAS_KEY As String = "v" & DefaultParamInitializer.BIAS_KEY

		Public Overrides Function numParams(ByVal conf As NeuralNetConfiguration) As Long
			Dim layerConf As org.deeplearning4j.nn.conf.layers.BasePretrainNetwork = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.BasePretrainNetwork)
			Return MyBase.numParams(conf) + layerConf.getNIn()
		End Function

		Public Overrides Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim params As IDictionary(Of String, INDArray) = MyBase.init(conf, paramsView, initializeParams)

			Dim layerConf As org.deeplearning4j.nn.conf.layers.BasePretrainNetwork = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.BasePretrainNetwork)
			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut()
			Dim nWeightParams As val = nIn * nOut

			Dim visibleBiasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nWeightParams + nOut, nWeightParams + nOut + nIn))
			params(VISIBLE_BIAS_KEY) = createVisibleBias(conf, visibleBiasView, initializeParams)
			conf.addVariable(VISIBLE_BIAS_KEY)

			Return params
		End Function

		Protected Friend Overridable Function createVisibleBias(ByVal conf As NeuralNetConfiguration, ByVal visibleBiasView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			Dim layerConf As org.deeplearning4j.nn.conf.layers.BasePretrainNetwork = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.BasePretrainNetwork)
			If initializeParameters Then
				Dim ret As INDArray = Nd4j.valueArrayOf(New Long(){1, layerConf.getNIn()}, layerConf.getVisibleBiasInit())
				visibleBiasView.assign(ret)
			End If
			Return visibleBiasView
		End Function


		Public Overrides Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim [out] As IDictionary(Of String, INDArray) = MyBase.getGradientsFromFlattened(conf, gradientView)
			Dim layerConf As org.deeplearning4j.nn.conf.layers.FeedForwardLayer = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.FeedForwardLayer)

			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut()
			Dim nWeightParams As val = nIn * nOut

			Dim vBiasView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nWeightParams + nOut, nWeightParams + nOut + nIn))

			[out](VISIBLE_BIAS_KEY) = vBiasView

			Return [out]
		End Function
	End Class

End Namespace