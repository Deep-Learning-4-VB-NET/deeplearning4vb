Imports System.Collections.Generic
Imports val = lombok.val
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
Imports WeightInitUtil = org.deeplearning4j.nn.weights.WeightInitUtil
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
import static org.nd4j.linalg.indexing.NDArrayIndex.interval
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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

Namespace org.deeplearning4j.nn.layers.ocnn


	Public Class OCNNParamInitializer
		Inherits DefaultParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New OCNNParamInitializer()


		Public Const NU_KEY As String = "nu"
		Public Const K_KEY As String = "k"

		Public Const V_KEY As String = "v"
		Public Const W_KEY As String = "w"

		Public Const R_KEY As String = "r"


		Private Shared ReadOnly WEIGHT_KEYS As IList(Of String) = New List(Of String) From {W_KEY,V_KEY,R_KEY}
		Private Shared ReadOnly PARAM_KEYS As IList(Of String) = New List(Of String) From {W_KEY,V_KEY,R_KEY}

		Public Shared ReadOnly Property Instance As OCNNParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Overrides Function numParams(ByVal conf As NeuralNetConfiguration) As Long
			Return numParams(conf.getLayer())
		End Function


		Public Overrides Function numParams(ByVal layer As Layer) As Long
			Dim ocnnOutputLayer As org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer = DirectCast(layer, org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer)
			Dim nIn As val = ocnnOutputLayer.getNIn()
			Dim hiddenLayer As val = ocnnOutputLayer.getHiddenSize()

			Dim firstLayerWeightLength As val = hiddenLayer
			Dim secondLayerLength As val = nIn * hiddenLayer
			Dim rLength As val = 1
			Return firstLayerWeightLength + secondLayerLength + rLength
		End Function

		Public Overrides Function paramKeys(ByVal layer As Layer) As IList(Of String)
			Return PARAM_KEYS
		End Function

		Public Overrides Function weightKeys(ByVal layer As Layer) As IList(Of String)
			Return WEIGHT_KEYS
		End Function

		Public Overrides Function biasKeys(ByVal layer As Layer) As IList(Of String)
			Return java.util.Collections.emptyList()
		End Function

		Public Overrides Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean
			Return WEIGHT_KEYS.Contains(key)
		End Function

		Public Overrides Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean
			Return False
		End Function

		Public Overrides Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
			Dim ocnnOutputLayer As org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer = CType(conf.getLayer(), org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer)
			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())
			Dim nIn As val = ocnnOutputLayer.getNIn()
			Dim hiddenLayer As Integer = ocnnOutputLayer.getHiddenSize()
			Preconditions.checkState(hiddenLayer > 0, "OCNNOutputLayer hidden layer state: must be non-zero.")

			Dim firstLayerWeightLength As val = hiddenLayer
			Dim secondLayerLength As val = nIn * hiddenLayer
			Dim rLength As Integer = 1
			Dim weightView As INDArray = paramsView.get(point(0),interval(0, firstLayerWeightLength)).reshape(1,hiddenLayer)
			Dim weightsTwoView As INDArray = paramsView.get(point(0), NDArrayIndex.interval(firstLayerWeightLength, firstLayerWeightLength + secondLayerLength)).reshape("f"c,nIn,hiddenLayer)
			Dim rView As INDArray = paramsView.get(point(0),point(paramsView.length() - rLength))


			Dim paramViewPut As INDArray = createWeightMatrix(conf, weightView, initializeParams)
			params(W_KEY) = paramViewPut
			conf.addVariable(W_KEY)
			Dim paramIvewPutTwo As INDArray = createWeightMatrix(conf,weightsTwoView,initializeParams)
			params(V_KEY) = paramIvewPutTwo
			conf.addVariable(V_KEY)
			Dim rViewPut As INDArray = createWeightMatrix(conf,rView,initializeParams)
			params(R_KEY) = rViewPut
			conf.addVariable(R_KEY)

			Return params
		End Function

		Public Overrides Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim ocnnOutputLayer As org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer = CType(conf.getLayer(), org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer)
			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())
			Dim nIn As val = ocnnOutputLayer.getNIn()
			Dim hiddenLayer As val = ocnnOutputLayer.getHiddenSize()

			Dim firstLayerWeightLength As val = hiddenLayer
			Dim secondLayerLength As val = nIn * hiddenLayer

			Dim weightView As INDArray = gradientView.get(point(0),interval(0, firstLayerWeightLength)).reshape("f"c,1,hiddenLayer)
			Dim vView As INDArray = gradientView.get(point(0), NDArrayIndex.interval(firstLayerWeightLength,firstLayerWeightLength + secondLayerLength)).reshape("f"c,nIn,hiddenLayer)
			params(W_KEY) = weightView
			params(V_KEY) = vView
			params(R_KEY) = gradientView.get(point(0),point(gradientView.length() - 1))
			Return params

		End Function


		Protected Friend Overrides Function createWeightMatrix(ByVal configuration As NeuralNetConfiguration, ByVal weightParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray

			Dim ocnnOutputLayer As org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer = CType(configuration.getLayer(), org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer)
			Dim weightInit As IWeightInit = ocnnOutputLayer.getWeightInitFn()
			If initializeParameters Then
				Dim ret As INDArray = weightInit.init(weightParamView.size(0), weightParamView.size(1), weightParamView.shape(), IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, weightParamView)
				Return ret
			Else
				Return WeightInitUtil.reshapeWeights(weightParamView.shape(), weightParamView)
			End If
		End Function
	End Class

End Namespace