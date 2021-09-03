Imports System.Collections.Generic
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports LSTM = org.deeplearning4j.nn.conf.layers.LSTM
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
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


	Public Class LSTMParamInitializer
		Implements ParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New LSTMParamInitializer()

		Public Shared ReadOnly Property Instance As LSTMParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		''' <summary>
		''' Weights for previous time step -> current time step connections </summary>
		Public Const RECURRENT_WEIGHT_KEY As String = "RW"
		Public Const BIAS_KEY As String = DefaultParamInitializer.BIAS_KEY
		Public Const INPUT_WEIGHT_KEY As String = DefaultParamInitializer.WEIGHT_KEY

		Private Shared ReadOnly LAYER_PARAM_KEYS As IList(Of String) = Collections.unmodifiableList(java.util.Arrays.asList(INPUT_WEIGHT_KEY, RECURRENT_WEIGHT_KEY, BIAS_KEY))
		Private Shared ReadOnly WEIGHT_KEYS As IList(Of String) = Collections.unmodifiableList(java.util.Arrays.asList(INPUT_WEIGHT_KEY, RECURRENT_WEIGHT_KEY))
		Private Shared ReadOnly BIAS_KEYS As IList(Of String) = Collections.unmodifiableList(Collections.singletonList(BIAS_KEY))

		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal l As Layer) As Long Implements ParamInitializer.numParams
			Dim layerConf As LSTM = DirectCast(l, LSTM)

			Dim nL As val = layerConf.getNOut() 'i.e., n neurons in this layer
			Dim nLast As val = layerConf.getNIn() 'i.e., n neurons in previous layer

			Dim nParams As val = nLast * (4 * nL) + nL * (4 * nL) + 4 * nL 'bias

			Return nParams
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String)
			Return LAYER_PARAM_KEYS
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String)
			Return WEIGHT_KEYS
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String)
			Return BIAS_KEYS
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return RECURRENT_WEIGHT_KEY.Equals(key) OrElse INPUT_WEIGHT_KEY.Equals(key)
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return BIAS_KEY.Equals(key)
		End Function

		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())
			Dim layerConf As LSTM = CType(conf.getLayer(), LSTM)
			Dim forgetGateInit As Double = layerConf.getForgetGateBiasInit()

			Dim nL As val = layerConf.getNOut() 'i.e., n neurons in this layer
			Dim nLast As val = layerConf.getNIn() 'i.e., n neurons in previous layer

			conf.addVariable(INPUT_WEIGHT_KEY)
			conf.addVariable(RECURRENT_WEIGHT_KEY)
			conf.addVariable(BIAS_KEY)

			Dim length As val = numParams(conf)
			If paramsView.length() <> length Then
				Throw New System.InvalidOperationException("Expected params view of length " & length & ", got length " & paramsView.length())
			End If

			Dim nParamsIn As val = nLast * (4 * nL)
			Dim nParamsRecurrent As val = nL * (4 * nL)
			Dim nBias As val = 4 * nL
			Dim inputWeightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nParamsIn))
			Dim recurrentWeightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nParamsIn, nParamsIn + nParamsRecurrent))
			Dim biasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nParamsIn + nParamsRecurrent, nParamsIn + nParamsRecurrent + nBias))

			If initializeParams Then
				Dim fanIn As val = nL
				Dim fanOut As val = nLast + nL
				Dim inputWShape As val = New Long() {nLast, 4 * nL}
				Dim recurrentWShape As val = New Long() {nL, 4 * nL}

				Dim rwInit As IWeightInit
				If layerConf.getWeightInitFnRecurrent() IsNot Nothing Then
					rwInit = layerConf.getWeightInitFnRecurrent()
				Else
					rwInit = layerConf.getWeightInitFn()
				End If

				params(INPUT_WEIGHT_KEY) = layerConf.getWeightInitFn().init(fanIn, fanOut, inputWShape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, inputWeightView)
				params(RECURRENT_WEIGHT_KEY) = rwInit.init(fanIn, fanOut, recurrentWShape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, recurrentWeightView)
				biasView.put(New INDArrayIndex() {NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nL, 2 * nL)}, Nd4j.valueArrayOf(New Long(){1, nL}, forgetGateInit)) 'Order: input, forget, output, input modulation, i.e., IFOG}
	'            The above line initializes the forget gate biases to specified value.
	'             * See Sutskever PhD thesis, pg19:
	'             * "it is important for [the forget gate activations] to be approximately 1 at the early stages of learning,
	'             *  which is accomplished by initializing [the forget gate biases] to a large value (such as 5). If it is
	'             *  not done, it will be harder to learn long range dependencies because the smaller values of the forget
	'             *  gates will create a vanishing gradients problem."
	'             *  http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf
	'             
				params(BIAS_KEY) = biasView
			Else
				params(INPUT_WEIGHT_KEY) = WeightInitUtil.reshapeWeights(New Long() {nLast, 4 * nL}, inputWeightView)
				params(RECURRENT_WEIGHT_KEY) = WeightInitUtil.reshapeWeights(New Long() {nL, 4 * nL}, recurrentWeightView)
				params(BIAS_KEY) = biasView
			End If

			Return params
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim layerConf As LSTM = CType(conf.getLayer(), LSTM)

			Dim nL As val = layerConf.getNOut() 'i.e., n neurons in this layer
			Dim nLast As val = layerConf.getNIn() 'i.e., n neurons in previous layer

			Dim length As val = numParams(conf)
			If gradientView.length() <> length Then
				Throw New System.InvalidOperationException("Expected gradient view of length " & length & ", got length " & gradientView.length())
			End If

			Dim nParamsIn As val = nLast * (4 * nL)
			Dim nParamsRecurrent As val = nL * (4 * nL)
			Dim nBias As val = 4 * nL
			Dim inputWeightGradView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nParamsIn)).reshape("f"c, nLast, 4 * nL)
			Dim recurrentWeightGradView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nParamsIn, nParamsIn + nParamsRecurrent)).reshape("f"c, nL, 4 * nL)
			Dim biasGradView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nParamsIn + nParamsRecurrent, nParamsIn + nParamsRecurrent + nBias)) 'already a row vector

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			[out](INPUT_WEIGHT_KEY) = inputWeightGradView
			[out](RECURRENT_WEIGHT_KEY) = recurrentWeightGradView
			[out](BIAS_KEY) = biasGradView

			Return [out]
		End Function
	End Class

End Namespace