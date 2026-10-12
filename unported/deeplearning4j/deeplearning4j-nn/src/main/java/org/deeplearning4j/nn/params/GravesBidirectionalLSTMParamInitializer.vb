Imports System.Collections.Generic
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
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


	Public Class GravesBidirectionalLSTMParamInitializer
		Implements ParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New GravesBidirectionalLSTMParamInitializer()

		Public Shared ReadOnly Property Instance As GravesBidirectionalLSTMParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		''' <summary>
		''' Weights for previous time step -> current time step connections
		''' </summary>
		Public Const RECURRENT_WEIGHT_KEY_FORWARDS As String = "RWF"
		Public Shared ReadOnly BIAS_KEY_FORWARDS As String = DefaultParamInitializer.BIAS_KEY & "F"
		Public Shared ReadOnly INPUT_WEIGHT_KEY_FORWARDS As String = DefaultParamInitializer.WEIGHT_KEY & "F"

		Public Const RECURRENT_WEIGHT_KEY_BACKWARDS As String = "RWB"
		Public Shared ReadOnly BIAS_KEY_BACKWARDS As String = DefaultParamInitializer.BIAS_KEY & "B"
		Public Shared ReadOnly INPUT_WEIGHT_KEY_BACKWARDS As String = DefaultParamInitializer.WEIGHT_KEY & "B"

		Private Shared ReadOnly WEIGHT_KEYS As IList(Of String) = Collections.unmodifiableList(java.util.Arrays.asList(INPUT_WEIGHT_KEY_FORWARDS, INPUT_WEIGHT_KEY_BACKWARDS, RECURRENT_WEIGHT_KEY_FORWARDS, RECURRENT_WEIGHT_KEY_BACKWARDS))
		Private Shared ReadOnly BIAS_KEYS As IList(Of String) = Collections.unmodifiableList(java.util.Arrays.asList(BIAS_KEY_FORWARDS, BIAS_KEY_BACKWARDS))
		Private Shared ReadOnly ALL_PARAM_KEYS As IList(Of String) = Collections.unmodifiableList(java.util.Arrays.asList(INPUT_WEIGHT_KEY_FORWARDS, INPUT_WEIGHT_KEY_BACKWARDS, RECURRENT_WEIGHT_KEY_FORWARDS, RECURRENT_WEIGHT_KEY_BACKWARDS, BIAS_KEY_FORWARDS, BIAS_KEY_BACKWARDS))

		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal l As Layer) As Long Implements ParamInitializer.numParams
			Dim layerConf As org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM = DirectCast(l, org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM)

			Dim nL As val = layerConf.getNOut() 'i.e., n neurons in this layer
			Dim nLast As val = layerConf.getNIn() 'i.e., n neurons in previous layer

			Dim nParamsForward As val = nLast * (4 * nL) + nL * (4 * nL + 3) + 4 * nL 'bias

			Return 2 * nParamsForward
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String)
			Return ALL_PARAM_KEYS
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String)
			Return WEIGHT_KEYS
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String)
			Return BIAS_KEYS
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return RECURRENT_WEIGHT_KEY_FORWARDS.Equals(key) OrElse INPUT_WEIGHT_KEY_FORWARDS.Equals(key) OrElse RECURRENT_WEIGHT_KEY_BACKWARDS.Equals(key) OrElse INPUT_WEIGHT_KEY_BACKWARDS.Equals(key)
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return BIAS_KEY_FORWARDS.Equals(key) OrElse BIAS_KEY_BACKWARDS.Equals(key)
		End Function

		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())

			Dim layerConf As org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM)
			Dim forgetGateInit As Double = layerConf.getForgetGateBiasInit()

			Dim nL As val = layerConf.getNOut() 'i.e., n neurons in this layer
			Dim nLast As val = layerConf.getNIn() 'i.e., n neurons in previous layer

			conf.addVariable(INPUT_WEIGHT_KEY_FORWARDS)
			conf.addVariable(RECURRENT_WEIGHT_KEY_FORWARDS)
			conf.addVariable(BIAS_KEY_FORWARDS)
			conf.addVariable(INPUT_WEIGHT_KEY_BACKWARDS)
			conf.addVariable(RECURRENT_WEIGHT_KEY_BACKWARDS)
			conf.addVariable(BIAS_KEY_BACKWARDS)

			Dim nParamsInput As val = nLast * (4 * nL)
			Dim nParamsRecurrent As val = nL * (4 * nL + 3)
			Dim nBias As val = 4 * nL

			Dim rwFOffset As val = nParamsInput
			Dim bFOffset As val = rwFOffset + nParamsRecurrent
			Dim iwROffset As val = bFOffset + nBias
			Dim rwROffset As val = iwROffset + nParamsInput
			Dim bROffset As val = rwROffset + nParamsRecurrent

			Dim iwF As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, rwFOffset))
			Dim rwF As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(rwFOffset, bFOffset))
			Dim bF As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(bFOffset, iwROffset))
			Dim iwR As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(iwROffset, rwROffset))
			Dim rwR As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(rwROffset, bROffset))
			Dim bR As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(bROffset, bROffset + nBias))

			If initializeParams Then
				bF.put(New INDArrayIndex(){NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nL, 2 * nL)}, Nd4j.ones(1, nL).muli(forgetGateInit)) 'Order: input, forget, output, input modulation, i.e., IFOG
				bR.put(New INDArrayIndex(){NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nL, 2 * nL)}, Nd4j.ones(1, nL).muli(forgetGateInit))
			End If
	'        The above line initializes the forget gate biases to specified value.
	'         * See Sutskever PhD thesis, pg19:
	'         * "it is important for [the forget gate activations] to be approximately 1 at the early stages of learning,
	'         *  which is accomplished by initializing [the forget gate biases] to a large value (such as 5). If it is
	'         *  not done, it will be harder to learn long range dependencies because the smaller values of the forget
	'         *  gates will create a vanishing gradients problem."
	'         *  http://www.cs.utoronto.ca/~ilya/pubs/ilya_sutskever_phd_thesis.pdf
	'         

			If initializeParams Then
				'As per standard LSTM
				Dim fanIn As val = nL
				Dim fanOut As val = nLast + nL
				Dim inputWShape As val = New Long(){nLast, 4 * nL}
				Dim recurrentWShape As val = New Long(){nL, 4 * nL + 3}

				params(INPUT_WEIGHT_KEY_FORWARDS) = layerConf.getWeightInitFn().init(fanIn, fanOut, inputWShape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, iwF)
				params(RECURRENT_WEIGHT_KEY_FORWARDS) = layerConf.getWeightInitFn().init(fanIn, fanOut, recurrentWShape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, rwF)
				params(BIAS_KEY_FORWARDS) = bF
				params(INPUT_WEIGHT_KEY_BACKWARDS) = layerConf.getWeightInitFn().init(fanIn, fanOut, inputWShape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, iwR)
				params(RECURRENT_WEIGHT_KEY_BACKWARDS) = layerConf.getWeightInitFn().init(fanIn, fanOut, recurrentWShape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, rwR)
				params(BIAS_KEY_BACKWARDS) = bR
			Else
				params(INPUT_WEIGHT_KEY_FORWARDS) = WeightInitUtil.reshapeWeights(New Long(){nLast, 4 * nL}, iwF)
				params(RECURRENT_WEIGHT_KEY_FORWARDS) = WeightInitUtil.reshapeWeights(New Long(){nL, 4 * nL + 3}, rwF)
				params(BIAS_KEY_FORWARDS) = bF
				params(INPUT_WEIGHT_KEY_BACKWARDS) = WeightInitUtil.reshapeWeights(New Long(){nLast, 4 * nL}, iwR)
				params(RECURRENT_WEIGHT_KEY_BACKWARDS) = WeightInitUtil.reshapeWeights(New Long(){nL, 4 * nL + 3}, rwR)
				params(BIAS_KEY_BACKWARDS) = bR
			End If

			Return params
		End Function


		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim layerConf As org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM)

			Dim nL As val = layerConf.getNOut() 'i.e., n neurons in this layer
			Dim nLast As val = layerConf.getNIn() 'i.e., n neurons in previous layer

			Dim nParamsInput As val = nLast * (4 * nL)
			Dim nParamsRecurrent As val = nL * (4 * nL + 3)
			Dim nBias As val = 4 * nL

			Dim rwFOffset As val = nParamsInput
			Dim bFOffset As val = rwFOffset + nParamsRecurrent
			Dim iwROffset As val = bFOffset + nBias
			Dim rwROffset As val = iwROffset + nParamsInput
			Dim bROffset As val = rwROffset + nParamsRecurrent

			Dim iwFG As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, rwFOffset)).reshape("f"c, nLast, 4 * nL)
			Dim rwFG As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(rwFOffset, bFOffset)).reshape("f"c, nL, 4 * nL + 3)
			Dim bFG As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(bFOffset, iwROffset))
			Dim iwRG As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(iwROffset, rwROffset)).reshape("f"c, nLast, 4 * nL)
			Dim rwRG As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(rwROffset, bROffset)).reshape("f"c, nL, 4 * nL + 3)
			Dim bRG As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(bROffset, bROffset + nBias))

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			[out](INPUT_WEIGHT_KEY_FORWARDS) = iwFG
			[out](RECURRENT_WEIGHT_KEY_FORWARDS) = rwFG
			[out](BIAS_KEY_FORWARDS) = bFG
			[out](INPUT_WEIGHT_KEY_BACKWARDS) = iwRG
			[out](RECURRENT_WEIGHT_KEY_BACKWARDS) = rwRG
			[out](BIAS_KEY_BACKWARDS) = bRG

			Return [out]
		End Function
	End Class

End Namespace