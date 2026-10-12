Imports System.Collections.Generic
Imports val = lombok.val
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports IWeightInit = org.deeplearning4j.nn.weights.IWeightInit
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


	Public Class ElementWiseParamInitializer
		Inherits DefaultParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New ElementWiseParamInitializer()

		Public Shared ReadOnly Property Instance As ElementWiseParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Overrides Function numParams(ByVal layer As Layer) As Long
			Dim layerConf As FeedForwardLayer = DirectCast(layer, FeedForwardLayer)
			Dim nIn As val = layerConf.getNIn()
			Return nIn*2 'weights + bias
		End Function

		''' <summary>
		''' Initialize the parameters
		''' </summary>
		''' <param name="conf">             the configuration </param>
		''' <param name="paramsView">       a view of the full network (backprop) parameters </param>
		''' <param name="initializeParams"> if true: initialize the parameters according to the configuration. If false: don't modify the
		'''                         values in the paramsView array (but do select out the appropriate subset, reshape etc as required) </param>
		''' <returns> Map of parameters keyed by type (view of the 'paramsView' array) </returns>
		Public Overrides Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray)
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

			Dim nWeightParams As val = nIn
			Dim weightView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nWeightParams))
			Dim biasView As INDArray = paramsView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nWeightParams, nWeightParams + nIn))


			params(WEIGHT_KEY) = createWeightMatrix(conf, weightView, initializeParams)
			params(BIAS_KEY) = createBias(conf, biasView, initializeParams)
			conf.addVariable(WEIGHT_KEY)
			conf.addVariable(BIAS_KEY)

			Return params
		End Function

		''' <summary>
		''' Return a map of gradients (in their standard non-flattened representation), taken from the flattened (row vector) gradientView array.
		''' The idea is that operates in exactly the same way as the paramsView does in
		''' thus the position in the view (and, the array orders) must match those of the parameters
		''' </summary>
		''' <param name="conf">         Configuration </param>
		''' <param name="gradientView"> The flattened gradients array, as a view of the larger array </param>
		''' <returns> A map containing an array by parameter type, that is a view of the full network gradients array </returns>
		Public Overrides Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray)
			Dim layerConf As FeedForwardLayer = CType(conf.getLayer(), FeedForwardLayer)
			Dim nIn As val = layerConf.getNIn()
			Dim nOut As val = layerConf.getNOut()
			Dim nWeightParams As val = nIn

			Dim weightGradientView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(0, nWeightParams))
			Dim biasView As INDArray = gradientView.get(NDArrayIndex.interval(0,0,True), NDArrayIndex.interval(nWeightParams, nWeightParams + nOut)) 'Already a row vector

			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			[out](WEIGHT_KEY) = weightGradientView
			[out](BIAS_KEY) = biasView

			Return [out]
		End Function

		Protected Friend Overrides Function createWeightMatrix(ByVal nIn As Long, ByVal nOut As Long, ByVal weightInit As IWeightInit, ByVal weightParamView As INDArray, ByVal initializeParameters As Boolean) As INDArray
			Dim shape As val = New Long() {1, nIn}

			If initializeParameters Then
				Dim ret As INDArray = weightInit.init(nIn, nOut, shape, IWeightInit.DEFAULT_WEIGHT_INIT_ORDER, weightParamView)
				Return ret
			Else
				Return weightParamView
			End If
		End Function


	End Class


End Namespace