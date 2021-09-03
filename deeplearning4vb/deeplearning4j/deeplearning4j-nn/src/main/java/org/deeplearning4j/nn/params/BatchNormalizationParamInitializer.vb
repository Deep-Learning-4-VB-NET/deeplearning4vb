Imports System.Collections.Generic
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BatchNormalization = org.deeplearning4j.nn.conf.layers.BatchNormalization
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
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


	Public Class BatchNormalizationParamInitializer
		Implements ParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New BatchNormalizationParamInitializer()

		Public Shared ReadOnly Property Instance As BatchNormalizationParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Const GAMMA As String = "gamma"
		Public Const BETA As String = "beta"
		Public Const GLOBAL_MEAN As String = "mean"
		Public Const GLOBAL_VAR As String = "var"
		Public Const GLOBAL_LOG_STD As String = "log10stdev"

		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal l As Layer) As Long Implements ParamInitializer.numParams
			Dim layer As BatchNormalization = DirectCast(l, BatchNormalization)
			'Parameters in batch norm:
			'gamma, beta, global mean estimate, global variance estimate
			' latter 2 are treated as parameters, which greatly simplifies spark training and model serialization

			If layer.isLockGammaBeta() Then
				'Special case: gamma and beta are fixed values for all outputs -> no parameters for gamma and  beta in this case
				Return 2 * layer.getNOut()
			Else
				'Standard case: gamma and beta are learned per output; additional 2*nOut for global mean/variance estimate
				Return 4 * layer.getNOut()
			End If
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String)
			If DirectCast(layer, BatchNormalization).isUseLogStd() Then
				Return New List(Of String) From {GAMMA, BETA, GLOBAL_MEAN, GLOBAL_LOG_STD}
			Else
				Return New List(Of String) From {GAMMA, BETA, GLOBAL_MEAN, GLOBAL_VAR}
			End If
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String)
			Return java.util.Collections.emptyList()
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String)
			Return java.util.Collections.emptyList()
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return False
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return False
		End Function

		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())
			' TODO setup for RNN
			Dim layer As BatchNormalization = CType(conf.getLayer(), BatchNormalization)
			Dim nOut As val = layer.getNOut()

			Dim meanOffset As Long = 0
			If Not layer.isLockGammaBeta() Then 'No gamma/beta parameters when gamma/beta are locked
				Dim gammaView As INDArray = paramView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nOut))
				Dim betaView As INDArray = paramView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nOut, 2 * nOut))

				params(GAMMA) = createGamma(conf, gammaView, initializeParams)
				conf.addVariable(GAMMA)
				params(BETA) = createBeta(conf, betaView, initializeParams)
				conf.addVariable(BETA)

				meanOffset = 2 * nOut
			End If

			Dim globalMeanView As INDArray = paramView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(meanOffset, meanOffset + nOut))
			Dim globalVarView As INDArray = paramView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(meanOffset + nOut, meanOffset + 2 * nOut))

			If initializeParams Then
				globalMeanView.assign(0)
				If layer.isUseLogStd() Then
					'Global log stdev: assign 0.0 as initial value (s=sqrt(v), and log10(s) = log10(sqrt(v)) -> log10(1) = 0
					globalVarView.assign(0)
				Else
					'Global variance view: assign 1.0 as initial value
					globalVarView.assign(1)
				End If
			End If

			params(GLOBAL_MEAN) = globalMeanView
			conf.addVariable(GLOBAL_MEAN)
			If layer.isUseLogStd() Then
				params(GLOBAL_LOG_STD) = globalVarView
				conf.addVariable(GLOBAL_LOG_STD)
			Else
				params(GLOBAL_VAR) = globalVarView
				conf.addVariable(GLOBAL_VAR)
			End If

			Return params
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim layer As BatchNormalization = CType(conf.getLayer(), BatchNormalization)
			Dim nOut As val = layer.getNOut()

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			Dim meanOffset As Long = 0
			If Not layer.isLockGammaBeta() Then
				Dim gammaView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nOut))
				Dim betaView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nOut, 2 * nOut))
				[out](GAMMA) = gammaView
				[out](BETA) = betaView
				meanOffset = 2 * nOut
			End If

			[out](GLOBAL_MEAN) = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(meanOffset, meanOffset + nOut))
			If layer.isUseLogStd() Then
				[out](GLOBAL_LOG_STD) = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(meanOffset + nOut, meanOffset + 2 * nOut))
			Else
				[out](GLOBAL_VAR) = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(meanOffset + nOut, meanOffset + 2 * nOut))
			End If

			Return [out]
		End Function

		Private Function createBeta(ByVal conf As NeuralNetConfiguration, ByVal betaView As INDArray, ByVal initializeParams As Boolean) As INDArray
			Dim layer As BatchNormalization = CType(conf.getLayer(), BatchNormalization)
			If initializeParams Then
				betaView.assign(layer.getBeta())
			End If
			Return betaView
		End Function

		Private Function createGamma(ByVal conf As NeuralNetConfiguration, ByVal gammaView As INDArray, ByVal initializeParams As Boolean) As INDArray
			Dim layer As BatchNormalization = CType(conf.getLayer(), BatchNormalization)
			If initializeParams Then
				gammaView.assign(layer.getGamma())
			End If
			Return gammaView
		End Function
	End Class

End Namespace