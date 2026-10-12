Imports System.Collections.Generic
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DepthwiseConvolution2D = org.deeplearning4j.nn.conf.layers.DepthwiseConvolution2D
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



	Public Class DepthwiseConvolutionParamInitializer
		Implements ParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New DepthwiseConvolutionParamInitializer()

		Public Shared ReadOnly Property Instance As DepthwiseConvolutionParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Const WEIGHT_KEY As String = DefaultParamInitializer.WEIGHT_KEY
		Public Const BIAS_KEY As String = DefaultParamInitializer.BIAS_KEY

		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal l As Layer) As Long Implements ParamInitializer.numParams
			Dim layerConf As DepthwiseConvolution2D = DirectCast(l, DepthwiseConvolution2D)

			Dim depthWiseParams As val = numDepthWiseParams(layerConf)
			Dim biasParams As val = numBiasParams(layerConf)

			Return depthWiseParams + biasParams
		End Function

		Private Function numBiasParams(ByVal layerConf As DepthwiseConvolution2D) As Long
			Dim nOut As val = layerConf.getNOut()
			Return (If(layerConf.hasBias(), nOut, 0))
		End Function

		''' <summary>
		''' For each input feature we separately compute depthMultiplier many
		''' output maps for the given kernel size
		''' </summary>
		''' <param name="layerConf"> layer configuration of the separable conv2d layer </param>
		''' <returns> number of parameters of the channels-wise convolution operation </returns>
		Private Function numDepthWiseParams(ByVal layerConf As DepthwiseConvolution2D) As Long
			Dim kernel() As Integer = layerConf.getKernelSize()
			Dim nIn As val = layerConf.getNIn()
			Dim depthMultiplier As val = layerConf.getDepthMultiplier()

			Return nIn * depthMultiplier * kernel(0) * kernel(1)
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String)
			Dim layerConf As DepthwiseConvolution2D = DirectCast(layer, DepthwiseConvolution2D)
			If layerConf.hasBias() Then
				Return New List(Of String) From {WEIGHT_KEY, BIAS_KEY}
			Else
				Return weightKeys(layer)
			End If
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String)
			Return New List(Of String) From {WEIGHT_KEY}
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String)
			Dim layerConf As DepthwiseConvolution2D = DirectCast(layer, DepthwiseConvolution2D)
			If layerConf.hasBias() Then
				Return Collections.singletonList(BIAS_KEY)
			Else
				Return java.util.Collections.emptyList()
			End If
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return WEIGHT_KEY.Equals(key)
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return BIAS_KEY.Equals(key)
		End Function


		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim layer As DepthwiseConvolution2D = CType(conf.getLayer(), DepthwiseConvolution2D)
			If layer.getKernelSize().length <> 2 Then
				Throw New System.ArgumentException("Filter size must be == 2")
			End If

			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())
			Dim layerConf As DepthwiseConvolution2D = CType(conf.getLayer(), DepthwiseConvolution2D)

			Dim depthWiseParams As val = numDepthWiseParams(layerConf)
			Dim biasParams As val = numBiasParams(layerConf)

			Dim depthWiseWeightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(biasParams, biasParams + depthWiseParams))

			params(WEIGHT_KEY) = createDepthWiseWeightMatrix(conf, depthWiseWeightView, initializeParams)
			conf.addVariable(WEIGHT_KEY)

			If layer.hasBias() Then
				Dim biasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, biasParams))
				params(BIAS_KEY) = createBias(conf, biasView, initializeParams)
				conf.addVariable(BIAS_KEY)
			End If

			Return params
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)

			Dim layerConf As DepthwiseConvolution2D = CType(conf.getLayer(), DepthwiseConvolution2D)

			Dim kernel() As Integer = layerConf.getKernelSize()
			Dim nIn As val = layerConf.getNIn()
			Dim depthMultiplier As val = layerConf.getDepthMultiplier()
			Dim nOut As val = layerConf.getNOut()

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()

			Dim depthWiseParams As val = numDepthWiseParams(layerConf)
			Dim biasParams As val = numBiasParams(layerConf)

			Dim depthWiseWeightGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(biasParams, biasParams + depthWiseParams)).reshape("c"c, kernel(0), kernel(1), nIn, depthMultiplier)
			[out](WEIGHT_KEY) = depthWiseWeightGradientView

			If layerConf.hasBias() Then
				Dim biasGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nOut))
				[out](BIAS_KEY) = biasGradientView
			End If
			Return [out]
		End Function

		Protected Friend Overridable Function createBias(ByVal conf As NeuralNetConfiguration, ByVal biasView As INDArray, ByVal initializeParams As Boolean) As INDArray
			Dim layerConf As DepthwiseConvolution2D = CType(conf.getLayer(), DepthwiseConvolution2D)
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
			Dim layerConf As DepthwiseConvolution2D = CType(conf.getLayer(), DepthwiseConvolution2D)
			Dim depthMultiplier As Integer = layerConf.getDepthMultiplier()

			If initializeParams Then
				Dim kernel() As Integer = layerConf.getKernelSize()
				Dim stride() As Integer = layerConf.getStride()

				Dim inputDepth As val = layerConf.getNIn()

				Dim fanIn As Double = inputDepth * kernel(0) * kernel(1)
				Dim fanOut As Double = depthMultiplier * kernel(0) * kernel(1) / (CDbl(stride(0)) * stride(1))

				Dim weightsShape As val = New Long() {kernel(0), kernel(1), inputDepth, depthMultiplier}

				Return layerConf.getWeightInitFn().init(fanIn, fanOut, weightsShape, "c"c, weightView)
			Else
				Dim kernel() As Integer = layerConf.getKernelSize()
				Return WeightInitUtil.reshapeWeights(New Long() {kernel(0), kernel(1), layerConf.getNIn(), depthMultiplier}, weightView, "c"c)
			End If
		End Function
	End Class

End Namespace