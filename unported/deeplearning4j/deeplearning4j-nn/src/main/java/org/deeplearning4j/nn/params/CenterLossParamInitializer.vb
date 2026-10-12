Imports System.Collections.Generic
Imports val = lombok.val
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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



	Public Class CenterLossParamInitializer
		Inherits DefaultParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New CenterLossParamInitializer()

		Public Shared ReadOnly Property Instance As CenterLossParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Shadows Const WEIGHT_KEY As String = DefaultParamInitializer.WEIGHT_KEY
		Public Shadows Const BIAS_KEY As String = DefaultParamInitializer.BIAS_KEY
		Public Const CENTER_KEY As String = "cL"

		Public Overrides Function numParams(ByVal conf As NeuralNetConfiguration) As Long
			Dim layerConf As org.deeplearning4j.nn.conf.layers.FeedForwardLayer = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.FeedForwardLayer)
			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut() ' also equal to numClasses
			Return nIn * nOut + nOut + nIn * nOut 'weights + bias + embeddings
		End Function

		Public Overrides Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())

			Dim layerConf As org.deeplearning4j.nn.conf.layers.CenterLossOutputLayer = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.CenterLossOutputLayer)

			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut() ' also equal to numClasses

			Dim wEndOffset As val = nIn * nOut
			Dim bEndOffset As val = wEndOffset + nOut
			Dim cEndOffset As val = bEndOffset + nIn * nOut

			Dim weightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, wEndOffset))
			Dim biasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(wEndOffset, bEndOffset))
			Dim centerLossView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(bEndOffset, cEndOffset)).reshape("c"c, nOut, nIn)

			params(WEIGHT_KEY) = createWeightMatrix(conf, weightView, initializeParams)
			params(BIAS_KEY) = createBias(conf, biasView, initializeParams)
			params(CENTER_KEY) = createCenterLossMatrix(conf, centerLossView, initializeParams)
			conf.addVariable(WEIGHT_KEY)
			conf.addVariable(BIAS_KEY)
			conf.addVariable(CENTER_KEY)

			Return params
		End Function

		Public Overrides Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim layerConf As org.deeplearning4j.nn.conf.layers.CenterLossOutputLayer = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.CenterLossOutputLayer)

			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut() ' also equal to numClasses

			Dim wEndOffset As val = nIn * nOut
			Dim bEndOffset As val = wEndOffset + nOut
			Dim cEndOffset As val = bEndOffset + nIn * nOut ' note: numClasses == nOut

			Dim weightGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, wEndOffset)).reshape("f"c, nIn, nOut)
			Dim biasView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(wEndOffset, bEndOffset)) 'Already a row vector
			Dim centerLossView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(bEndOffset, cEndOffset)).reshape("c"c, nOut, nIn)

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			[out](WEIGHT_KEY) = weightGradientView
			[out](BIAS_KEY) = biasView
			[out](CENTER_KEY) = centerLossView

			Return [out]
		End Function


		Protected Friend Overridable Function createCenterLossMatrix(ByVal conf As NeuralNetConfiguration, ByVal centerLossView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			Dim layerConf As org.deeplearning4j.nn.conf.layers.CenterLossOutputLayer = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.CenterLossOutputLayer)

			If initializeParameters Then
				centerLossView.assign(0.0)
			End If
			Return centerLossView
		End Function
	End Class

End Namespace