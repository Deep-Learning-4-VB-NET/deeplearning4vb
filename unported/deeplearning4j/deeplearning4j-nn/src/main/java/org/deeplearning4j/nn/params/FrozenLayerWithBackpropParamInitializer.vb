Imports System.Collections.Generic
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports FrozenLayer = org.deeplearning4j.nn.conf.layers.misc.FrozenLayer
Imports FrozenLayerWithBackprop = org.deeplearning4j.nn.conf.layers.misc.FrozenLayerWithBackprop
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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


	Public Class FrozenLayerWithBackpropParamInitializer
		Implements ParamInitializer

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New FrozenLayerWithBackpropParamInitializer()

		Public Shared ReadOnly Property Instance As FrozenLayerWithBackpropParamInitializer
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Overridable Function numParams(ByVal conf As NeuralNetConfiguration) As Long Implements ParamInitializer.numParams
			Return numParams(conf.getLayer())
		End Function

		Public Overridable Function numParams(ByVal layer As Layer) As Long Implements ParamInitializer.numParams
			Dim fl As FrozenLayerWithBackprop = DirectCast(layer, FrozenLayerWithBackprop)
			Dim initializer As ParamInitializer = fl.getUnderlying().initializer()
			Return initializer.numParams(fl.getUnderlying())
		End Function

		Public Overridable Function paramKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.paramKeys
			Return Collections.emptyList()
		End Function

		Public Overridable Function weightKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.weightKeys
			Return Collections.emptyList()
		End Function

		Public Overridable Function biasKeys(ByVal layer As Layer) As IList(Of String) Implements ParamInitializer.biasKeys
			Return Collections.emptyList()
		End Function

		Public Overridable Function isWeightParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isWeightParam
			Return False
		End Function

		Public Overridable Function isBiasParam(ByVal layer As Layer, ByVal key As String) As Boolean Implements ParamInitializer.isBiasParam
			Return False
		End Function

		Public Overridable Function init(ByVal conf As NeuralNetConfiguration, ByVal paramsView As INDArray, ByVal initializeParams As Boolean) As IDictionary(Of String, INDArray) Implements ParamInitializer.init
			Dim fl As FrozenLayerWithBackprop = CType(conf.getLayer(), FrozenLayerWithBackprop)
			Dim innerLayer As Layer = fl.getUnderlying()
			Dim initializer As ParamInitializer = innerLayer.initializer()
			conf.setLayer(innerLayer)
			Dim m As IDictionary(Of String, INDArray) = initializer.init(conf, paramsView, initializeParams)
			conf.setLayer(fl)

			Return m
		End Function

		Public Overridable Function getGradientsFromFlattened(ByVal conf As NeuralNetConfiguration, ByVal gradientView As INDArray) As IDictionary(Of String, INDArray) Implements ParamInitializer.getGradientsFromFlattened
			Dim fl As FrozenLayerWithBackprop = CType(conf.getLayer(), FrozenLayerWithBackprop)
			Dim innerLayer As Layer = fl.getUnderlying()
			Dim initializer As ParamInitializer = innerLayer.initializer()
			conf.setLayer(innerLayer)
			Dim m As IDictionary(Of String, INDArray) = initializer.getGradientsFromFlattened(conf, gradientView)
			conf.setLayer(fl)
			Return m
		End Function
	End Class

End Namespace