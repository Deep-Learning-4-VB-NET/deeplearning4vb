Imports System.Collections.Generic
Imports val = lombok.val
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Deconvolution3D = org.deeplearning4j.nn.conf.layers.Deconvolution3D
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
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



	Public Class Deconvolution3DParamInitializer
		Inherits ConvolutionParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New Deconvolution3DParamInitializer()

		Public Shared ReadOnly Property Instance As Deconvolution3DParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Shadows Const WEIGHT_KEY As String = DefaultParamInitializer.WEIGHT_KEY
		Public Shadows Const BIAS_KEY As String = DefaultParamInitializer.BIAS_KEY

		Public Overrides Function numParams(ByVal conf As NeuralNetConfiguration) As Long
			Return numParams(conf.getLayer())
		End Function

		Public Overrides Function numParams(ByVal l As Layer) As Long
			Dim layerConf As Deconvolution3D = DirectCast(l, Deconvolution3D)

			Dim kernel() As Integer = layerConf.getKernelSize()
			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut()
			Return nIn * nOut * kernel(0) * kernel(1) * kernel(2) + (If(layerConf.hasBias(), nOut, 0))
		End Function


		Public Overrides Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim layer As Deconvolution3D = CType(conf.getLayer(), Deconvolution3D)
			If layer.getKernelSize().length <> 3 Then
				Throw New System.ArgumentException("Filter size must be == 3")
			End If

			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())

			Dim layerConf As Deconvolution3D = CType(conf.getLayer(), Deconvolution3D)
			Dim nOut As val = layerConf.getNOut()

			If layer.hasBias() Then
				Dim biasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nOut))
				Dim weightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nOut, numParams(conf)))
				params(BIAS_KEY) = createBias(conf, biasView, initializeParams)
				params(WEIGHT_KEY) = createWeightMatrix(conf, weightView, initializeParams)
				conf.addVariable(WEIGHT_KEY)
				conf.addVariable(BIAS_KEY)
			Else
				Dim weightView As INDArray = paramsView
				params(WEIGHT_KEY) = createWeightMatrix(conf, weightView, initializeParams)
				conf.addVariable(WEIGHT_KEY)
			End If

			Return params
		End Function

		Public Overrides Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)

			Dim layerConf As Deconvolution3D = CType(conf.getLayer(), Deconvolution3D)

			Dim kernel() As Integer = layerConf.getKernelSize()
			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut()

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			If layerConf.hasBias() Then
				Dim biasGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nOut))
				Dim weightGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nOut, numParams(conf))).reshape("c"c, kernel(0), kernel(1), kernel(2), nOut, nIn)
				[out](BIAS_KEY) = biasGradientView
				[out](WEIGHT_KEY) = weightGradientView
			Else
				Dim weightGradientView As INDArray = gradientView.reshape("c"c, kernel(0), kernel(1), kernel(2), nOut, nIn)
				[out](WEIGHT_KEY) = weightGradientView
			End If
			Return [out]
		End Function


		Protected Friend Overrides Function createWeightMatrix(ByVal conf As NeuralNetConfiguration, ByVal weightView As INDArray, ByVal initializeParams As Boolean) As INDArray
	'        
	'         Create a 5d weight matrix of:
	'           (number of kernels, num input channels, kernel depth, kernel height, kernel width)
	'         Note c order is used specifically for the CNN weights, as opposed to f order elsewhere
	'         Inputs to the convolution layer are:
	'         (batch size, num input feature maps, image depth, image height, image width)
	'         
			Dim layerConf As Deconvolution3D = CType(conf.getLayer(), Deconvolution3D)

			If initializeParams Then
				Dim kernel() As Integer = layerConf.getKernelSize()
				Dim stride() As Integer = layerConf.getStride()

				Dim inputDepth As val = layerConf.getNIn()
				Dim outputDepth As val = layerConf.getNOut()

				Dim fanIn As Double = inputDepth * kernel(0) * kernel(1) * kernel(2)
				Dim fanOut As Double = outputDepth * kernel(0) * kernel(1) * kernel(2) / (CDbl(stride(0)) * stride(1) * stride(2))

				'libnd4j: [kD, kH, kW, oC, iC]
				Dim weightsShape As val = New Long(){kernel(0), kernel(1), kernel(2), outputDepth, inputDepth}

				Return layerConf.getWeightInitFn().init(fanIn, fanOut, weightsShape, "c"c, weightView)
			Else
				Dim kernel() As Integer = layerConf.getKernelSize()
				Return WeightInitUtil.reshapeWeights(New Long(){kernel(0), kernel(1), kernel(2), layerConf.getNOut(), layerConf.getNIn()}, weightView, "c"c)
			End If
		End Function
	End Class

End Namespace