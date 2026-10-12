Imports System.Collections.Generic
Imports val = lombok.val
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports BaseLayer = org.deeplearning4j.nn.conf.layers.BaseLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports PReLULayer = org.deeplearning4j.nn.conf.layers.PReLULayer
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


	Public Class PReLUParamInitializer
		Implements ParamInitializer

		Public Const WEIGHT_KEY As String = "W"
		Private weightShape() As Long
		Private sharedAxes() As Long

		Public Sub New(ByVal shape() As Long, ByVal sharedAxes() As Long)
			Me.weightShape = shape
			Me.sharedAxes = sharedAxes
			' Set shared axes to 1, broadcasting will take place on c++ level.
			If sharedAxes IsNot Nothing Then
				For Each axis As Long In sharedAxes
					weightShape(CInt(axis) - 1) = 1
				Next axis
			End If
		End Sub


		Public Shared Function getInstance(ByVal shape() As Long, ByVal sharedAxes() As Long) As PReLUParamInitializer
			Return New PReLUParamInitializer(shape, sharedAxes)
		End Function


		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal l As Layer) As Long Implements ParamInitializer.numParams
			Return numParams(weightShape)
		End Function

		Private Function numParams(ByVal shape() As Long) As Long
			Dim flattened As Long = 1
			For Each value As Long In shape
				flattened *= value
			Next value
			Return flattened
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.paramKeys
				Return weightKeys(layer)
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.weightKeys
			Return Collections.singletonList(WEIGHT_KEY)
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.biasKeys
			Return Collections.emptyList()
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return WEIGHT_KEY.Equals(key)
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return False
		End Function

		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray) Implements ParamInitializer.init
			If Not (TypeOf conf.getLayer() Is BaseLayer) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Throw New System.ArgumentException("unsupported layer type: " & conf.getLayer().GetType().FullName)
			End If

			Dim params As IDictionary(Of String, INDArray) = Collections.synchronizedMap(New LinkedHashMap(Of String, INDArray)())

			Dim length As val = numParams(conf)
			If paramsView.length() <> length Then
				Throw New System.InvalidOperationException("Expected params view of length " & length & ", got length " & paramsView.length())
			End If

			Dim weightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, length))

			params(WEIGHT_KEY) = createWeightMatrix(conf, weightView, initializeParams)
			conf.addVariable(WEIGHT_KEY)

			Return params
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray) Implements ParamInitializer.getGradientsFromFlattened

			Dim length As val = numParams(conf)
			Dim weightGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, length)).reshape("f"c, weightShape)
			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			[out](WEIGHT_KEY) = weightGradientView

			Return [out]
		End Function


		Protected Friend Overridable Function createWeightMatrix(ByVal conf As NeuralNetConfiguration, ByVal weightParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray

			Dim layerConf As PReLULayer = CType(conf.getLayer(), PReLULayer)
			If initializeParameters Then
				Return layerConf.getWeightInitFn().init(layerConf.getNIn(), layerConf.getNOut(), weightShape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, weightParamView)
			Else
				Return WeightInitUtil.reshapeWeights(weightShape, weightParamView)
			End If
		End Function

	End Class

End Namespace