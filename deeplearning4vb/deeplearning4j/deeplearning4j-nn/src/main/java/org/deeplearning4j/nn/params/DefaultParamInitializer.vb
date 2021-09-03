Imports System.Collections.Generic
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports org.deeplearning4j.nn.conf.layers
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
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


	Public Class DefaultParamInitializer
		Implements ParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New DefaultParamInitializer()

		Public Shared ReadOnly Property Instance As DefaultParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Const WEIGHT_KEY As String = "W"
		Public Const BIAS_KEY As String = "b"
		Public Const GAIN_KEY As String = "g"

		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal l As Layer) As Long
			Dim layerConf As FeedForwardLayer = DirectCast(l, FeedForwardLayer)
			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut()
			Return (nIn * nOut + (If(hasBias(l), nOut, 0)) + (If(hasLayerNorm(l), nOut, 0))) 'weights + bias + gain
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ArrayList<String> keys = new ArrayList<>(3);
			Dim keys As New List(Of String)(3)
			keys.AddRange(weightKeys(layer))
			keys.AddRange(biasKeys(layer))
			Return keys
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String)
			If hasLayerNorm(layer) Then
				Return New List(Of String) From {WEIGHT_KEY, GAIN_KEY}
			End If
			Return Collections.singletonList(WEIGHT_KEY)
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String)
			If hasBias(layer) Then
				Return Collections.singletonList(BIAS_KEY)
			Else
				Return java.util.Collections.emptyList()
			End If
		End Function


		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean
			Return WEIGHT_KEY.Equals(key) OrElse (hasLayerNorm(layer) AndAlso GAIN_KEY.Equals(key))
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean
			Return BIAS_KEY.Equals(key)
		End Function

		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			If Not (TypeOf conf.getLayer() Is FeedForwardLayer) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException("unsupported layer type: " & conf.getLayer().GetType().FullName)
			End If

			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())

			Dim length As val = numParams(conf)
			If paramsView.length() <> length Then
				Throw New System.InvalidOperationException("Expected params view of length " & length & ", got length " & paramsView.length())
			End If

			Dim layerConf As FeedForwardLayer = CType(conf.getLayer(), FeedForwardLayer)
			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut()

			Dim nWeightParams As val = nIn * nOut
			Dim weightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nWeightParams))

			params(WEIGHT_KEY) = createWeightMatrix(conf, weightView, initializeParams)
			conf.addVariable(WEIGHT_KEY)

			Dim offset As Long = nWeightParams
			If hasBias(layerConf) Then
				Dim biasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(offset, offset + nOut))
				params(BIAS_KEY) = createBias(conf, biasView, initializeParams)
				conf.addVariable(BIAS_KEY)
				offset += nOut
			End If

			If hasLayerNorm(layerConf) Then
				Dim gainView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(offset, offset + nOut))
				params(GAIN_KEY) = createGain(conf, gainView, initializeParams)
				conf.addVariable(GAIN_KEY)
			End If

			Return params
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim layerConf As FeedForwardLayer = CType(conf.getLayer(), FeedForwardLayer)
			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut()
			Dim nWeightParams As val = nIn * nOut

			Dim weightGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nWeightParams)).reshape("f"c, nIn, nOut)

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			[out](WEIGHT_KEY) = weightGradientView

			Dim offset As Long = nWeightParams
			If hasBias(layerConf) Then
				Dim biasView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(offset, offset + nOut)) 'Already a row vector
				[out](BIAS_KEY) = biasView
				offset += nOut
			End If

			If hasLayerNorm(layerConf) Then
				Dim gainView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(offset, offset + nOut)) 'Already a row vector
				[out](GAIN_KEY) = gainView
			End If

			Return [out]
		End Function


		Protected Friend Overridable Function createBias(ByVal conf As NeuralNetConfiguration, ByVal biasParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			Dim layerConf As FeedForwardLayer = CType(conf.getLayer(), FeedForwardLayer)
			Return createBias(layerConf.getNOut(), layerConf.getBiasInit(), biasParamView, initializeParameters)
		End Function

		Protected Friend Overridable Function createBias(ByVal nOut As Long, ByVal biasInit As Double, ByVal biasParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			If initializeParameters Then
				biasParamView.assign(biasInit)
			End If
			Return biasParamView
		End Function

		Protected Friend Overridable Function createGain(ByVal conf As NeuralNetConfiguration, ByVal gainParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			Dim layerConf As FeedForwardLayer = CType(conf.getLayer(), FeedForwardLayer)
			Return createGain(layerConf.getNOut(), layerConf.getGainInit(), gainParamView, initializeParameters)
		End Function

		Protected Friend Overridable Function createGain(ByVal nOut As Long, ByVal gainInit As Double, ByVal gainParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			If initializeParameters Then
				gainParamView.assign(gainInit)
			End If
			Return gainParamView
		End Function


		Protected Friend Overridable Function createWeightMatrix(ByVal conf As NeuralNetConfiguration, ByVal weightParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			Dim layerConf As FeedForwardLayer = CType(conf.getLayer(), FeedForwardLayer)

			If initializeParameters Then
				Return createWeightMatrix(layerConf.getNIn(), layerConf.getNOut(), layerConf.getWeightInitFn(), weightParamView, True)
			Else
				Return createWeightMatrix(layerConf.getNIn(), layerConf.getNOut(), Nothing, weightParamView, False)
			End If
		End Function

		Protected Friend Overridable Function createWeightMatrix(ByVal nIn As Long, ByVal nOut As Long, ByVal weightInit As IWeightInit, ByVal weightParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			Dim shape As val = New Long() {nIn, nOut}

			If initializeParameters Then
				Dim ret As INDArray = weightInit.init(nIn, nOut, shape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, weightParamView)
				Return ret
			Else
				Return WeightInitUtil.reshapeWeights(shape, weightParamView)
			End If
		End Function

		Protected Friend Overridable Function hasBias(ByVal layer As Layer) As Boolean
			If TypeOf layer Is BaseOutputLayer Then
				Return DirectCast(layer, BaseOutputLayer).hasBias()
			ElseIf TypeOf layer Is DenseLayer Then
				Return DirectCast(layer, DenseLayer).hasBias()
			ElseIf TypeOf layer Is EmbeddingLayer Then
				Return DirectCast(layer, EmbeddingLayer).hasBias()
			ElseIf TypeOf layer Is EmbeddingSequenceLayer Then
				Return DirectCast(layer, EmbeddingSequenceLayer).hasBias()
			End If
			Return True
		End Function

		Protected Friend Overridable Function hasLayerNorm(ByVal layer As Layer) As Boolean
			If TypeOf layer Is DenseLayer Then
				Return DirectCast(layer, DenseLayer).hasLayerNorm()
			End If
			Return False
		End Function
	End Class

End Namespace