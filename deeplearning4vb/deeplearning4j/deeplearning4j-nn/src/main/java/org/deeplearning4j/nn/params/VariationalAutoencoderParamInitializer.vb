Imports System
Imports System.Collections.Generic
Imports val = lombok.val
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports VariationalAutoencoder = org.deeplearning4j.nn.conf.layers.variational.VariationalAutoencoder
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
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


	Public Class VariationalAutoencoderParamInitializer
		Inherits DefaultParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New VariationalAutoencoderParamInitializer()

		Public Shared ReadOnly Property Instance As VariationalAutoencoderParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Const WEIGHT_KEY_SUFFIX As String = "W"
		Public Const BIAS_KEY_SUFFIX As String = "b"
		Public Const PZX_PREFIX As String = "pZX"
		Public Shared ReadOnly PZX_MEAN_PREFIX As String = PZX_PREFIX & "Mean"
		Public Shared ReadOnly PZX_LOGSTD2_PREFIX As String = PZX_PREFIX & "LogStd2"

		Public Const ENCODER_PREFIX As String = "e"
		Public Const DECODER_PREFIX As String = "d"

		''' <summary>
		''' Key for weight parameters connecting the last encoder layer and the mean values for p(z|data) </summary>
		Public Shared ReadOnly PZX_MEAN_W As String = "pZXMean" & WEIGHT_KEY_SUFFIX
		''' <summary>
		''' Key for bias parameters for the mean values for p(z|data) </summary>
		Public Shared ReadOnly PZX_MEAN_B As String = "pZXMean" & BIAS_KEY_SUFFIX
		''' <summary>
		''' Key for weight parameters connecting the last encoder layer and the log(sigma^2) values for p(z|data) </summary>
		Public Shared ReadOnly PZX_LOGSTD2_W As String = PZX_LOGSTD2_PREFIX & WEIGHT_KEY_SUFFIX
		''' <summary>
		''' Key for bias parameters for log(sigma^2) in p(z|data) </summary>
		Public Shared ReadOnly PZX_LOGSTD2_B As String = PZX_LOGSTD2_PREFIX & BIAS_KEY_SUFFIX

		Public Const PXZ_PREFIX As String = "pXZ"
		''' <summary>
		''' Key for weight parameters connecting the last decoder layer and p(data|z) (according to whatever
		'''  <seealso cref="org.deeplearning4j.nn.conf.layers.variational.ReconstructionDistribution"/> is set for the VAE) 
		''' </summary>
		Public Shared ReadOnly PXZ_W As String = PXZ_PREFIX & WEIGHT_KEY_SUFFIX
		''' <summary>
		''' Key for bias parameters connecting the last decoder layer and p(data|z) (according to whatever
		'''  <seealso cref="org.deeplearning4j.nn.conf.layers.variational.ReconstructionDistribution"/> is set for the VAE) 
		''' </summary>
		Public Shared ReadOnly PXZ_B As String = PXZ_PREFIX & BIAS_KEY_SUFFIX



		Public Overrides Function numParams(ByVal conf As NeuralNetConfiguration) As Long
			Dim layer As VariationalAutoencoder = CType(conf.getLayer(), VariationalAutoencoder)

			Dim nIn As val = layer.getNIn()
			Dim nOut As val = layer.getNOut()
			Dim encoderLayerSizes() As Integer = layer.getEncoderLayerSizes()
			Dim decoderLayerSizes() As Integer = layer.getDecoderLayerSizes()

			Dim paramCount As Integer = 0
			For i As Integer = 0 To encoderLayerSizes.Length - 1
				Dim encoderLayerIn As Long
				If i = 0 Then
					encoderLayerIn = nIn
				Else
					encoderLayerIn = encoderLayerSizes(i - 1)
				End If
				paramCount += (encoderLayerIn + 1) * encoderLayerSizes(i) 'weights + bias
			Next i

			'Between the last encoder layer and the parameters for p(z|x):
			Dim lastEncLayerSize As Integer = encoderLayerSizes(encoderLayerSizes.Length - 1)
			paramCount += (lastEncLayerSize + 1) * 2 * nOut 'Mean and variance parameters used in unsupervised training

			'Decoder:
			For i As Integer = 0 To decoderLayerSizes.Length - 1
				Dim decoderLayerNIn As Long
				If i = 0 Then
					decoderLayerNIn = nOut
				Else
					decoderLayerNIn = decoderLayerSizes(i - 1)
				End If
				paramCount += (decoderLayerNIn + 1) * decoderLayerSizes(i)
			Next i

			'Between last decoder layer and parameters for p(x|z):
			If nIn > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim nDistributionParams As val = layer.getOutputDistribution().distributionInputSize(CInt(nIn))
			Dim lastDecLayerSize As val = decoderLayerSizes(decoderLayerSizes.Length - 1)
			paramCount += (lastDecLayerSize + 1) * nDistributionParams

			Return paramCount
		End Function

		Public Overrides Function paramKeys(ByVal l As Layer) As IList(Of String)
			Dim layer As VariationalAutoencoder = DirectCast(l, VariationalAutoencoder)
			Dim encoderLayerSizes() As Integer = layer.getEncoderLayerSizes()
			Dim decoderLayerSizes() As Integer = layer.getDecoderLayerSizes()

			Dim p As IList(Of String) = New List(Of String)()

			Dim soFar As Integer = 0
			For i As Integer = 0 To encoderLayerSizes.Length - 1
				Dim sW As String = "e" & i & WEIGHT_KEY_SUFFIX
				Dim sB As String = "e" & i & BIAS_KEY_SUFFIX
				p.Add(sW)
				p.Add(sB)
			Next i

			'Last encoder layer -> p(z|x)
			p.Add(PZX_MEAN_W)
			p.Add(PZX_MEAN_B)

			'Pretrain params
			p.Add(PZX_LOGSTD2_W)
			p.Add(PZX_LOGSTD2_B)

			For i As Integer = 0 To decoderLayerSizes.Length - 1
				Dim sW As String = "d" & i & WEIGHT_KEY_SUFFIX
				Dim sB As String = "d" & i & BIAS_KEY_SUFFIX
				p.Add(sW)
				p.Add(sB)
			Next i

			'Finally, p(x|z):
			p.Add(PXZ_W)
			p.Add(PXZ_B)

			Return p
		End Function

		Public Overrides Function weightKeys(ByVal layer As Layer) As IList(Of String)
			Dim [out] As IList(Of String) = New List(Of String)()
			For Each s As String In paramKeys(layer)
				If isWeightParam(layer, s) Then
					[out].Add(s)
				End If
			Next s
			Return [out]
		End Function

		Public Overrides Function biasKeys(ByVal layer As Layer) As IList(Of String)
			Dim [out] As IList(Of String) = New List(Of String)()
			For Each s As String In paramKeys(layer)
				If isBiasParam(layer, s) Then
					[out].Add(s)
				End If
			Next s
			Return [out]
		End Function

		Public Overrides Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean
			Return key.EndsWith(WEIGHT_KEY_SUFFIX, StringComparison.Ordinal)
		End Function

		Public Overrides Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean
			Return key.EndsWith(BIAS_KEY_SUFFIX, StringComparison.Ordinal)
		End Function

		Public Overrides Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			If paramsView.length() <> numParams(conf) Then
				Throw New System.ArgumentException("Incorrect paramsView length: Expected length " & numParams(conf) & ", got length " & paramsView.length())
			End If

			Dim ret As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			Dim layer As VariationalAutoencoder = CType(conf.getLayer(), VariationalAutoencoder)

			Dim nIn As val = layer.getNIn()
			Dim nOut As val = layer.getNOut()
			Dim encoderLayerSizes() As Integer = layer.getEncoderLayerSizes()
			Dim decoderLayerSizes() As Integer = layer.getDecoderLayerSizes()

			Dim weightInit As IWeightInit = layer.getWeightInitFn()

			Dim soFar As Integer = 0
			For i As Integer = 0 To encoderLayerSizes.Length - 1
				Dim encoderLayerNIn As Long
				If i = 0 Then
					encoderLayerNIn = nIn
				Else
					encoderLayerNIn = encoderLayerSizes(i - 1)
				End If
				Dim weightParamCount As val = encoderLayerNIn * encoderLayerSizes(i)
				Dim weightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + weightParamCount))
				soFar += weightParamCount
				Dim biasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + encoderLayerSizes(i)))
				soFar += encoderLayerSizes(i)

				Dim layerWeights As INDArray = createWeightMatrix(encoderLayerNIn, encoderLayerSizes(i), weightInit, weightView, initializeParams)
				Dim layerBiases As INDArray = createBias(encoderLayerSizes(i), 0.0, biasView, initializeParams) 'TODO don't hardcode 0

				Dim sW As String = "e" & i & WEIGHT_KEY_SUFFIX
				Dim sB As String = "e" & i & BIAS_KEY_SUFFIX
				ret(sW) = layerWeights
				ret(sB) = layerBiases

				conf.addVariable(sW)
				conf.addVariable(sB)
			Next i

			'Last encoder layer -> p(z|x)
			Dim nWeightsPzx As val = encoderLayerSizes(encoderLayerSizes.Length - 1) * nOut
			Dim pzxWeightsMean As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nWeightsPzx))
			soFar += nWeightsPzx
			Dim pzxBiasMean As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nOut))
			soFar += nOut

			Dim pzxWeightsMeanReshaped As INDArray = createWeightMatrix(encoderLayerSizes(encoderLayerSizes.Length - 1), nOut, weightInit, pzxWeightsMean, initializeParams)
			Dim pzxBiasMeanReshaped As INDArray = createBias(nOut, 0.0, pzxBiasMean, initializeParams) 'TODO don't hardcode 0

			ret(PZX_MEAN_W) = pzxWeightsMeanReshaped
			ret(PZX_MEAN_B) = pzxBiasMeanReshaped
			conf.addVariable(PZX_MEAN_W)
			conf.addVariable(PZX_MEAN_B)


			'Pretrain params
			Dim pzxWeightsLogStdev2 As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nWeightsPzx))
			soFar += nWeightsPzx
			Dim pzxBiasLogStdev2 As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nOut))
			soFar += nOut

			Dim pzxWeightsLogStdev2Reshaped As INDArray = createWeightMatrix(encoderLayerSizes(encoderLayerSizes.Length - 1), nOut, weightInit, pzxWeightsLogStdev2, initializeParams)
			Dim pzxBiasLogStdev2Reshaped As INDArray = createBias(nOut, 0.0, pzxBiasLogStdev2, initializeParams) 'TODO don't hardcode 0

			ret(PZX_LOGSTD2_W) = pzxWeightsLogStdev2Reshaped
			ret(PZX_LOGSTD2_B) = pzxBiasLogStdev2Reshaped
			conf.addVariable(PZX_LOGSTD2_W)
			conf.addVariable(PZX_LOGSTD2_B)

			For i As Integer = 0 To decoderLayerSizes.Length - 1
				Dim decoderLayerNIn As Long
				If i = 0 Then
					decoderLayerNIn = nOut
				Else
					decoderLayerNIn = decoderLayerSizes(i - 1)
				End If
				Dim weightParamCount As val = decoderLayerNIn * decoderLayerSizes(i)
				Dim weightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + weightParamCount))
				soFar += weightParamCount
				Dim biasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + decoderLayerSizes(i)))
				soFar += decoderLayerSizes(i)

				Dim layerWeights As INDArray = createWeightMatrix(decoderLayerNIn, decoderLayerSizes(i), weightInit, weightView, initializeParams)
				Dim layerBiases As INDArray = createBias(decoderLayerSizes(i), 0.0, biasView, initializeParams) 'TODO don't hardcode 0

				Dim sW As String = "d" & i & WEIGHT_KEY_SUFFIX
				Dim sB As String = "d" & i & BIAS_KEY_SUFFIX
				ret(sW) = layerWeights
				ret(sB) = layerBiases
				conf.addVariable(sW)
				conf.addVariable(sB)
			Next i

			'Finally, p(x|z):
			If nIn > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim nDistributionParams As Integer = layer.getOutputDistribution().distributionInputSize(CInt(nIn))
			Dim pxzWeightCount As Integer = decoderLayerSizes(decoderLayerSizes.Length - 1) * nDistributionParams
			Dim pxzWeightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + pxzWeightCount))
			soFar += pxzWeightCount
			Dim pxzBiasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nDistributionParams))

			Dim pxzWeightsReshaped As INDArray = createWeightMatrix(decoderLayerSizes(decoderLayerSizes.Length - 1), nDistributionParams, weightInit, pxzWeightView, initializeParams)
			Dim pxzBiasReshaped As INDArray = createBias(nDistributionParams, 0.0, pxzBiasView, initializeParams) 'TODO don't hardcode 0

			ret(PXZ_W) = pxzWeightsReshaped
			ret(PXZ_B) = pxzBiasReshaped
			conf.addVariable(PXZ_W)
			conf.addVariable(PXZ_B)

			Return ret
		End Function

		Public Overrides Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim ret As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			Dim layer As VariationalAutoencoder = CType(conf.getLayer(), VariationalAutoencoder)

			Dim nIn As val = layer.getNIn()
			Dim nOut As val = layer.getNOut()
			Dim encoderLayerSizes() As Integer = layer.getEncoderLayerSizes()
			Dim decoderLayerSizes() As Integer = layer.getDecoderLayerSizes()

			Dim soFar As Integer = 0
			For i As Integer = 0 To encoderLayerSizes.Length - 1
				Dim encoderLayerNIn As Long
				If i = 0 Then
					encoderLayerNIn = nIn
				Else
					encoderLayerNIn = encoderLayerSizes(i - 1)
				End If
				Dim weightParamCount As val = encoderLayerNIn * encoderLayerSizes(i)
				Dim weightGradView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + weightParamCount))
				soFar += weightParamCount
				Dim biasGradView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + encoderLayerSizes(i)))
				soFar += encoderLayerSizes(i)

				Dim layerWeights As INDArray = weightGradView.reshape("f"c, encoderLayerNIn, encoderLayerSizes(i))
				Dim layerBiases As INDArray = biasGradView 'Aready correct shape (row vector)

				ret("e" & i & WEIGHT_KEY_SUFFIX) = layerWeights
				ret("e" & i & BIAS_KEY_SUFFIX) = layerBiases
			Next i

			'Last encoder layer -> p(z|x)
			Dim nWeightsPzx As val = encoderLayerSizes(encoderLayerSizes.Length - 1) * nOut
			Dim pzxWeightsMean As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nWeightsPzx))
			soFar += nWeightsPzx
			Dim pzxBiasMean As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nOut))
			soFar += nOut

			Dim pzxWeightGradMeanReshaped As INDArray = pzxWeightsMean.reshape("f"c, encoderLayerSizes(encoderLayerSizes.Length - 1), nOut)

			ret(PZX_MEAN_W) = pzxWeightGradMeanReshaped
			ret(PZX_MEAN_B) = pzxBiasMean

			'//////////////////////////////////////////////////////

			Dim pzxWeightsLogStdev2 As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nWeightsPzx))
			soFar += nWeightsPzx
			Dim pzxBiasLogStdev2 As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nOut))
			soFar += nOut

			Dim pzxWeightsLogStdev2Reshaped As INDArray = createWeightMatrix(encoderLayerSizes(encoderLayerSizes.Length - 1), nOut, Nothing, pzxWeightsLogStdev2, False) 'TODO

			ret(PZX_LOGSTD2_W) = pzxWeightsLogStdev2Reshaped
			ret(PZX_LOGSTD2_B) = pzxBiasLogStdev2

			For i As Integer = 0 To decoderLayerSizes.Length - 1
				Dim decoderLayerNIn As Long
				If i = 0 Then
					decoderLayerNIn = nOut
				Else
					decoderLayerNIn = decoderLayerSizes(i - 1)
				End If
				Dim weightParamCount As Long = decoderLayerNIn * decoderLayerSizes(i)
				Dim weightView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + weightParamCount))
				soFar += weightParamCount
				Dim biasView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + decoderLayerSizes(i)))
				soFar += decoderLayerSizes(i)

				Dim layerWeights As INDArray = createWeightMatrix(decoderLayerNIn, decoderLayerSizes(i), Nothing, weightView, False)
				Dim layerBiases As INDArray = createBias(decoderLayerSizes(i), 0.0, biasView, False) 'TODO don't hardcode 0

				Dim sW As String = "d" & i & WEIGHT_KEY_SUFFIX
				Dim sB As String = "d" & i & BIAS_KEY_SUFFIX
				ret(sW) = layerWeights
				ret(sB) = layerBiases
			Next i

			'Finally, p(x|z):
			If nIn > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim nDistributionParams As Integer = layer.getOutputDistribution().distributionInputSize(CInt(nIn))
			Dim pxzWeightCount As Integer = decoderLayerSizes(decoderLayerSizes.Length - 1) * nDistributionParams
			Dim pxzWeightView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + pxzWeightCount))
			soFar += pxzWeightCount
			Dim pxzBiasView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(soFar, soFar + nDistributionParams))

			Dim pxzWeightsReshaped As INDArray = createWeightMatrix(decoderLayerSizes(decoderLayerSizes.Length - 1), nDistributionParams, Nothing, pxzWeightView, False)
			Dim pxzBiasReshaped As INDArray = createBias(nDistributionParams, 0.0, pxzBiasView, False)

			ret(PXZ_W) = pxzWeightsReshaped
			ret(PXZ_B) = pxzBiasReshaped

			Return ret
		End Function
	End Class

End Namespace