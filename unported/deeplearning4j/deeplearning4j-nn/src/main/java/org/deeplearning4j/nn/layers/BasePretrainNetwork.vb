Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports PretrainParamInitializer = org.deeplearning4j.nn.params.PretrainParamInitializer
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.nn.layers




	<Serializable>
	Public MustInherit Class BasePretrainNetwork(Of LayerConfT As org.deeplearning4j.nn.conf.layers.BasePretrainNetwork)
		Inherits BaseLayer(Of LayerConfT)


		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub


		''' <summary>
		''' Corrupts the given input by doing a binomial sampling
		''' given the corruption level </summary>
		''' <param name="x"> the input to corrupt </param>
		''' <param name="corruptionLevel"> the corruption value </param>
		''' <returns> the binomial sampled corrupted input </returns>
		Public Overridable Function getCorruptedInput(ByVal x As INDArray, ByVal corruptionLevel As Double) As INDArray
			Dim corrupted As INDArray = Nd4j.Distributions.createBinomial(1, 1 - corruptionLevel).sample(x.ulike())
			corrupted.muli(x.castTo(corrupted.dataType()))
			Return corrupted
		End Function


		Protected Friend Overridable Function createGradient(ByVal wGradient As INDArray, ByVal vBiasGradient As INDArray, ByVal hBiasGradient As INDArray) As Gradient
			Dim ret As Gradient = New DefaultGradient(gradientsFlattened)
			' The order of the following statements matter! The gradient is being flattened and applied to
			' flattened params in this order.
			' The arrays neeed to be views, with the current Updater implementation

			'TODO: optimize this, to do it would the assigns
			Dim wg As INDArray = gradientViews(PretrainParamInitializer.WEIGHT_KEY)
			wg.assign(wGradient)

			Dim hbg As INDArray = gradientViews(PretrainParamInitializer.BIAS_KEY)
			hbg.assign(hBiasGradient)

			Dim vbg As INDArray = gradientViews(PretrainParamInitializer.VISIBLE_BIAS_KEY)
			vbg.assign(vBiasGradient)

			ret.gradientForVariable()(PretrainParamInitializer.WEIGHT_KEY) = wg
			ret.gradientForVariable()(PretrainParamInitializer.BIAS_KEY) = hbg
			ret.gradientForVariable()(PretrainParamInitializer.VISIBLE_BIAS_KEY) = vbg

			Return ret
		End Function

		Public Overrides Function numParams(ByVal backwards As Boolean) As Long
			Return MyBase.numParams(backwards)
		End Function

		''' <summary>
		''' Sample the hidden distribution given the visible </summary>
		''' <param name="v"> the visible to sample from </param>
		''' <returns> the hidden mean and sample </returns>
		Public MustOverride Function sampleHiddenGivenVisible(ByVal v As INDArray) As Pair(Of INDArray, INDArray)

		''' <summary>
		''' Sample the visible distribution given the hidden </summary>
		''' <param name="h"> the hidden to sample from </param>
		''' <returns> the mean and sample </returns>
		Public MustOverride Function sampleVisibleGivenHidden(ByVal h As INDArray) As Pair(Of INDArray, INDArray)

		Protected Friend Overrides WriteOnly Property ScoreWithZ As INDArray
			Set(ByVal z As INDArray)
				If input_Conflict Is Nothing OrElse z Is Nothing Then
					Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
				End If
				Dim lossFunction As ILossFunction = layerConf().getLossFunction().getILossFunction()
    
				'double score = lossFunction.computeScore(input, z, layerConf().getActivationFunction(), maskArray, false);
				Dim score As Double = lossFunction.computeScore(input_Conflict, z, layerConf().getActivationFn(), maskArray_Conflict, False)
				score /= InputMiniBatchSize
				score += calcRegularizationScore(False)
    
				Me.score_Conflict = score
			End Set
		End Property

		Public Overrides Function paramTable(ByVal backpropParamsOnly As Boolean) As IDictionary(Of String, INDArray)
			If Not backpropParamsOnly Then
				Return params_Conflict
			End If
			Dim map As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			map(PretrainParamInitializer.WEIGHT_KEY) = params(PretrainParamInitializer.WEIGHT_KEY)
			map(PretrainParamInitializer.BIAS_KEY) = params(PretrainParamInitializer.BIAS_KEY)
			Return map
		End Function

		Public Overrides Function params() As INDArray
			Return paramsFlattened
		End Function

		''' <summary>
		'''The number of parameters for the model, for backprop (i.e., excluding visible bias) </summary>
		''' <returns> the number of parameters for the model (ex. visible bias) </returns>
		Public Overrides Function numParams() As Long
			Dim ret As Integer = 0
			For Each entry As KeyValuePair(Of String, INDArray) In params_Conflict.SetOfKeyValuePairs()
				ret += entry.Value.length()
			Next entry
			Return ret
		End Function

		Public Overrides WriteOnly Property Params As INDArray
			Set(ByVal params As INDArray)
				If params Is paramsFlattened Then
					Return 'No op
				End If
    
				'SetParams has two different uses: during pretrain vs. backprop.
				'pretrain = 3 sets of params (inc. visible bias); backprop = 2
    
				Dim parameterList As IList(Of String) = conf_Conflict.variables()
				Dim paramLength As Long = 0
				For Each s As String In parameterList
					Dim len As val = getParam(s).length()
					paramLength += len
				Next s
    
				If params.length() <> paramLength Then
					Throw New System.ArgumentException("Unable to set parameters: must be of length " & paramLength & ", got params of length " & params.length() & " " & layerId())
				End If
    
				' Set for backprop and only W & hb
				paramsFlattened.assign(params)
    
			End Set
		End Property

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim result As Pair(Of Gradient, INDArray) = MyBase.backpropGradient(epsilon, workspaceMgr)
			DirectCast(result.First, DefaultGradient).setFlattenedGradient(gradientsFlattened)

			'During backprop, visible bias gradients are set to 0 - this is necessary due to the gradient view mechanics
			' that DL4J uses
			Dim vBiasGradient As INDArray = gradientViews(PretrainParamInitializer.VISIBLE_BIAS_KEY)
			result.First.gradientForVariable()(PretrainParamInitializer.VISIBLE_BIAS_KEY) = vBiasGradient
			vBiasGradient.assign(0)

			weightNoiseParams.Clear()

			Return result
		End Function


		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Dim scoreSum As Double = MyBase.calcRegularizationScore(True)
			If backpropParamsOnly Then
				Return scoreSum
			End If
			If layerConf().getRegularizationBias() IsNot Nothing AndAlso Not layerConf().getRegularizationBias().isEmpty() Then
				For Each r As Regularization In layerConf().getRegularizationBias()
					Dim p As INDArray = getParam(PretrainParamInitializer.VISIBLE_BIAS_KEY)
					scoreSum += r.score(p, IterationCount, EpochCount)
				Next r
			End If
			Return scoreSum
		End Function
	End Class

End Namespace