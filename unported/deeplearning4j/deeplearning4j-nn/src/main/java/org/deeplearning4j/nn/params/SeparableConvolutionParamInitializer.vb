Imports System.Collections.Generic
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports SeparableConvolution2D = org.deeplearning4j.nn.conf.layers.SeparableConvolution2D
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



	Public Class SeparableConvolutionParamInitializer
		Implements ParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New SeparableConvolutionParamInitializer()

		Public Shared ReadOnly Property Instance As SeparableConvolutionParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Const DEPTH_WISE_WEIGHT_KEY As String = DefaultParamInitializer.WEIGHT_KEY
		Public Const POINT_WISE_WEIGHT_KEY As String = "pW"
		Public Const BIAS_KEY As String = DefaultParamInitializer.BIAS_KEY

		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal l As Layer) As Long Implements ParamInitializer.numParams
			Dim layerConf As SeparableConvolution2D = DirectCast(l, SeparableConvolution2D)

			Dim depthWiseParams As val = numDepthWiseParams(layerConf)
			Dim pointWiseParams As val = numPointWiseParams(layerConf)
			Dim biasParams As val = numBiasParams(layerConf)

			Return depthWiseParams + pointWiseParams + biasParams
		End Function

		Private Function numBiasParams(ByVal layerConf As SeparableConvolution2D) As Long
			Dim nOut As val = layerConf.getNOut()
			Return (If(layerConf.hasBias(), nOut, 0))
		End Function

		''' <summary>
		''' For each input feature we separately compute depthMultiplier many
		''' output maps for the given kernel size
		''' </summary>
		''' <param name="layerConf"> layer configuration of the separable conv2d layer </param>
		''' <returns> number of parameters of the channels-wise convolution operation </returns>
		Private Function numDepthWiseParams(ByVal layerConf As SeparableConvolution2D) As Long
			Dim kernel() As Integer = layerConf.getKernelSize()
			Dim nIn As val = layerConf.getNIn()
			Dim depthMultiplier As val = layerConf.getDepthMultiplier()

			Return nIn * depthMultiplier * kernel(0) * kernel(1)
		End Function

		''' <summary>
		''' For the point-wise convolution part we have (nIn * depthMultiplier) many
		''' input maps and nOut output maps. Kernel size is (1, 1) for this operation.
		''' </summary>
		''' <param name="layerConf"> layer configuration of the separable conv2d layer </param>
		''' <returns> number of parameters of the point-wise convolution operation </returns>
		Private Function numPointWiseParams(ByVal layerConf As SeparableConvolution2D) As Long
			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut()
			Dim depthMultiplier As val = layerConf.getDepthMultiplier()

			Return (nIn * depthMultiplier) * nOut
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String)
			Dim layerConf As SeparableConvolution2D = DirectCast(layer, SeparableConvolution2D)
			If layerConf.hasBias() Then
				Return New List(Of String) From {DEPTH_WISE_WEIGHT_KEY, POINT_WISE_WEIGHT_KEY, BIAS_KEY}
			Else
				Return weightKeys(layer)
			End If
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String)
			Return New List(Of String) From {DEPTH_WISE_WEIGHT_KEY, POINT_WISE_WEIGHT_KEY}
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String)
			Dim layerConf As SeparableConvolution2D = DirectCast(layer, SeparableConvolution2D)
			If layerConf.hasBias() Then
				Return Collections.singletonList(BIAS_KEY)
			Else
				Return java.util.Collections.emptyList()
			End If
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return DEPTH_WISE_WEIGHT_KEY.Equals(key) OrElse POINT_WISE_WEIGHT_KEY.Equals(key)
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return BIAS_KEY.Equals(key)
		End Function


		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim layer As SeparableConvolution2D = CType(conf.getLayer(), SeparableConvolution2D)
			If layer.getKernelSize().length <> 2 Then
				Throw New System.ArgumentException("Filter size must be == 2")
			End If

			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())
			Dim layerConf As SeparableConvolution2D = CType(conf.getLayer(), SeparableConvolution2D)

			Dim depthWiseParams As val = numDepthWiseParams(layerConf)
			Dim biasParams As val = numBiasParams(layerConf)

			Dim depthWiseWeightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(biasParams, biasParams + depthWiseParams))
			Dim pointWiseWeightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(biasParams + depthWiseParams, numParams(conf)))

			params(DEPTH_WISE_WEIGHT_KEY) = createDepthWiseWeightMatrix(conf, depthWiseWeightView, initializeParams)
			conf.addVariable(DEPTH_WISE_WEIGHT_KEY)
			params(POINT_WISE_WEIGHT_KEY) = createPointWiseWeightMatrix(conf, pointWiseWeightView, initializeParams)
			conf.addVariable(POINT_WISE_WEIGHT_KEY)

			If layer.hasBias() Then
				Dim biasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, biasParams))
				params(BIAS_KEY) = createBias(conf, biasView, initializeParams)
				conf.addVariable(BIAS_KEY)
			End If

			Return params
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)

			Dim layerConf As SeparableConvolution2D = CType(conf.getLayer(), SeparableConvolution2D)

			Dim kernel() As Integer = layerConf.getKernelSize()
			Dim nIn As val = layerConf.getNIn()
			Dim depthMultiplier As val = layerConf.getDepthMultiplier()
			Dim nOut As val = layerConf.getNOut()

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()

			Dim depthWiseParams As val = numDepthWiseParams(layerConf)
			Dim biasParams As val = numBiasParams(layerConf)

			Dim depthWiseWeightGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(biasParams, biasParams + depthWiseParams)).reshape("c"c, depthMultiplier, nIn, kernel(0), kernel(1))
			Dim pointWiseWeightGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(biasParams + depthWiseParams, numParams(conf))).reshape("c"c, nOut, nIn * depthMultiplier, 1, 1)
			[out](DEPTH_WISE_WEIGHT_KEY) = depthWiseWeightGradientView
			[out](POINT_WISE_WEIGHT_KEY) = pointWiseWeightGradientView

			If layerConf.hasBias() Then
				Dim biasGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nOut))
				[out](BIAS_KEY) = biasGradientView
			End If
			Return [out]
		End Function

		Protected Friend Overridable Function createBias(ByVal conf As NeuralNetConfiguration, ByVal biasView As INDArray, ByVal initializeParams As Boolean) As INDArray
			Dim layerConf As SeparableConvolution2D = CType(conf.getLayer(), SeparableConvolution2D)
			If initializeParams Then
				biasView.assign(layerConf.getBiasInit())
			End If
			Return biasView
		End Function


		Protected Friend Overridable Function createDepthWiseWeightMatrix(ByVal conf As NeuralNetConfiguration, ByVal weightView As INDArray, ByVal initializeParams As Boolean) As INDArray
	'        
	'         Create a 4d weight matrix of: (channels multiplier, num input channels, kernel height, kernel width)
	'         Inputs to the convolution layer are: (batch size, num input feature maps, image height, image width)
	'         
			Dim layerConf As SeparableConvolution2D = CType(conf.getLayer(), SeparableConvolution2D)
			Dim depthMultiplier As Integer = layerConf.getDepthMultiplier()

			If initializeParams Then
				Dim kernel() As Integer = layerConf.getKernelSize()
				Dim stride() As Integer = layerConf.getStride()

				Dim inputDepth As val = layerConf.getNIn()

				Dim fanIn As Double = inputDepth * kernel(0) * kernel(1)
				Dim fanOut As Double = depthMultiplier * kernel(0) * kernel(1) / (CDbl(stride(0)) * stride(1))

				Dim weightsShape As val = New Long() {depthMultiplier, inputDepth, kernel(0), kernel(1)}

				Return layerConf.getWeightInitFn().init(fanIn, fanOut, weightsShape, "c"c, weightView)
			Else
				Dim kernel() As Integer = layerConf.getKernelSize()
				Return WeightInitUtil.reshapeWeights(New Long() {depthMultiplier, layerConf.getNIn(), kernel(0), kernel(1)}, weightView, "c"c)
			End If
		End Function

		Protected Friend Overridable Function createPointWiseWeightMatrix(ByVal conf As NeuralNetConfiguration, ByVal weightView As INDArray, ByVal initializeParams As Boolean) As INDArray
	'        
	'         Create a 4d weight matrix of: (num output channels, channels multiplier * num input channels,
	'         kernel height, kernel width)
	'         
			Dim layerConf As SeparableConvolution2D = CType(conf.getLayer(), SeparableConvolution2D)
			Dim depthMultiplier As Integer = layerConf.getDepthMultiplier()

			If initializeParams Then

				Dim inputDepth As val = layerConf.getNIn()
				Dim outputDepth As val = layerConf.getNOut()

				Dim fanIn As Double = inputDepth * depthMultiplier
				Dim fanOut As Double = fanIn

				Dim weightsShape As val = New Long() {outputDepth, depthMultiplier * inputDepth, 1, 1}

				Return layerConf.getWeightInitFn().init(fanIn, fanOut, weightsShape, "c"c, weightView)
			Else
				Return WeightInitUtil.reshapeWeights(New Long() {layerConf.getNOut(), depthMultiplier * layerConf.getNIn(), 1, 1}, weightView, "c"c)
			End If
		End Function
	End Class

End Namespace