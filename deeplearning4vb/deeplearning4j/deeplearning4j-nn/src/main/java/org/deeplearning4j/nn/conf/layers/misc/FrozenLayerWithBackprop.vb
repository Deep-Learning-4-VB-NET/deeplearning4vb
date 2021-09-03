Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports BaseWrapperLayer = org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
Imports FrozenLayerWithBackpropParamInitializer = org.deeplearning4j.nn.params.FrozenLayerWithBackpropParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.deeplearning4j.nn.conf.layers.misc


	''' <summary>
	''' Frozen layer freezes parameters of the layer it wraps, but allows the backpropagation to continue.
	''' 
	''' @author Ugljesa Jovanovic (ugljesa.jovanovic@ionspin.com) on 06/05/2018. </summary>
	''' <seealso cref= FrozenLayer </seealso>

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class FrozenLayerWithBackprop extends org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
	<Serializable>
	Public Class FrozenLayerWithBackprop
		Inherits BaseWrapperLayer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FrozenLayerWithBackprop(@JsonProperty("layer") org.deeplearning4j.nn.conf.layers.Layer layer)
		Public Sub New(ByVal layer As Layer)
			MyBase.New(layer)
		End Sub

		Public Overridable Function getInnerConf(ByVal conf As NeuralNetConfiguration) As NeuralNetConfiguration
			Dim nnc As NeuralNetConfiguration = conf.clone()
			nnc.setLayer(underlying)
			Return nnc
		End Function

		Public Overrides Function clone() As Layer
			Dim l As FrozenLayerWithBackprop = DirectCast(MyBase.clone(), FrozenLayerWithBackprop)
			l.underlying = underlying.clone()
			Return l
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer

			'Need to be able to instantiate a layer, from a config - for JSON -> net type situations
			Dim underlying As org.deeplearning4j.nn.api.Layer = getUnderlying().instantiate(getInnerConf(conf), trainingListeners, layerIndex, layerParamsView, initializeParams, networkDataType)

			Dim nncUnderlying As NeuralNetConfiguration = underlying.conf()

			If nncUnderlying.variables() IsNot Nothing Then
				Dim vars As IList(Of String) = nncUnderlying.variables(True)
				nncUnderlying.clearVariables()
				conf.clearVariables()
				For Each s As String In vars
					conf.variables(False).Add(s)
					nncUnderlying.variables(False).Add(s)
				Next s
			End If

			Return New org.deeplearning4j.nn.layers.FrozenLayerWithBackprop(underlying)
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return FrozenLayerWithBackpropParamInitializer.Instance
		End Function

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			'No regularization for frozen layers
			Return Nothing
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return False
		End Function

		Public Overrides Function getUpdaterByParam(ByVal paramName As String) As IUpdater
			Return Nothing
		End Function

		Public Overrides WriteOnly Property LayerName As String
			Set(ByVal layerName As String)
				MyBase.LayerName = layerName
				underlying.setLayerName(layerName)
			End Set
		End Property

		Public Overrides WriteOnly Property Constraints As IList(Of LayerConstraint)
			Set(ByVal constraints As IList(Of LayerConstraint))
				Me.constraints = constraints
				Me.underlying.setConstraints(constraints)
			End Set
		End Property
	End Class

End Namespace