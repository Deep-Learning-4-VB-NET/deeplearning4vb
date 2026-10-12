Imports System
Imports System.Collections.Generic
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports FrozenLayerParamInitializer = org.deeplearning4j.nn.params.FrozenLayerParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports IUpdater = org.nd4j.linalg.learning.config.IUpdater
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) public class FrozenLayer extends org.deeplearning4j.nn.conf.layers.Layer
	<Serializable>
	Public Class FrozenLayer
		Inherits Layer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.deeplearning4j.nn.conf.layers.Layer layer;
		Protected Friend layer As Layer

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.layer = builder.layer_Conflict
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FrozenLayer(@JsonProperty("layer") org.deeplearning4j.nn.conf.layers.Layer layer)
		Public Sub New(ByVal layer As Layer)
			Me.layer = layer
		End Sub

		Public Overridable Function getInnerConf(ByVal conf As NeuralNetConfiguration) As NeuralNetConfiguration
			Dim nnc As NeuralNetConfiguration = conf.clone()
			nnc.setLayer(layer)
			Return nnc
		End Function

		Public Overrides Function clone() As Layer
			Dim l As FrozenLayer = DirectCast(MyBase.clone(), FrozenLayer)
			l.layer = layer.clone()
			Return l
		End Function

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer

			'Need to be able to instantiate a layer, from a config - for JSON -> net type situations
			Dim underlying As org.deeplearning4j.nn.api.Layer = layer.instantiate(getInnerConf(conf), trainingListeners, layerIndex, layerParamsView, initializeParams, networkDataType)

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

			Return New org.deeplearning4j.nn.layers.FrozenLayer(underlying)
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return FrozenLayerParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return layer.getOutputType(layerIndex, inputType)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			layer.setNIn(inputType, override)
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return layer.getPreProcessorForInputType(inputType)
		End Function

		Public Overrides Function getRegularizationByParam(ByVal param As String) As IList(Of Regularization)
			Return Nothing
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return False
		End Function

		Public Overrides Function getUpdaterByParam(ByVal paramName As String) As IUpdater
			Return Nothing
		End Function

		Public Overrides ReadOnly Property GradientNormalization As GradientNormalization
			Get
				Return layer.GradientNormalization
			End Get
		End Property

		Public Overrides ReadOnly Property GradientNormalizationThreshold As Double
			Get
				Return layer.GradientNormalizationThreshold
			End Get
		End Property

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return layer.getMemoryReport(inputType)
		End Function

		Public Overrides WriteOnly Property LayerName As String
			Set(ByVal layerName As String)
				MyBase.setLayerName(layerName)
				layer.setLayerName(layerName)
			End Set
		End Property

		Public Overrides WriteOnly Property Constraints As IList(Of LayerConstraint)
			Set(ByVal constraints As IList(Of LayerConstraint))
				Me.constraints = constraints
				Me.layer.setConstraints(constraints)
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field layer was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend layer_Conflict As Layer

'JAVA TO VB CONVERTER NOTE: The parameter layer was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function layer(ByVal layer_Conflict As Layer) As Builder
				Me.setLayer(layer_Conflict)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public FrozenLayer build()
			Public Overrides Function build() As FrozenLayer
				Return New FrozenLayer(Me)
			End Function
		End Class
	End Class

End Namespace