Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Setter = lombok.Setter
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports BaseWrapperLayer = org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

Namespace org.deeplearning4j.nn.conf.layers.util


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class MaskZeroLayer extends org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
	<Serializable>
	Public Class MaskZeroLayer
		Inherits BaseWrapperLayer

		Private maskingValue As Double = 0.0

		Private Const serialVersionUID As Long = 9074525846200921839L

		Public Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.underlying = builder.underlying_Conflict
			Me.maskingValue = builder.maskValue_Conflict
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MaskZeroLayer(@JsonProperty("underlying") org.deeplearning4j.nn.conf.layers.Layer underlying, @JsonProperty("maskingValue") double maskingValue)
		Public Sub New(ByVal underlying As Layer, ByVal maskingValue As Double)
			Me.underlying = underlying
			Me.maskingValue = maskingValue
		End Sub


		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer

			Dim conf2 As NeuralNetConfiguration = conf.clone()
			conf2.setLayer(CType(conf2.getLayer(), BaseWrapperLayer).getUnderlying())

			Dim underlyingLayer As org.deeplearning4j.nn.api.Layer = underlying.instantiate(conf2, trainingListeners, layerIndex, layerParamsView, initializeParams, networkDataType)
			Return New org.deeplearning4j.nn.layers.recurrent.MaskZeroLayer(underlyingLayer, maskingValue)
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return underlying.getOutputType(layerIndex, inputType)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			underlying.setNIn(inputType, override)
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return underlying.getPreProcessorForInputType(inputType) 'No op
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return False
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return underlying.getMemoryReport(inputType)
		End Function

		Public Overrides Function ToString() As String
			Return "MaskZeroLayer(" & underlying.ToString() & ")"
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.Layer.Builder<Builder>
		Public Class Builder
			Inherits Layer.Builder(Of Builder)

'JAVA TO VB CONVERTER NOTE: The field underlying was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend underlying_Conflict As Layer
'JAVA TO VB CONVERTER NOTE: The field maskValue was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend maskValue_Conflict As Double

			Public Overridable Function setUnderlying(ByVal underlying As Layer) As Builder
				Me.underlying_Conflict = underlying
				Return Me
			End Function

			Public Overridable Function setMaskValue(ByVal maskValue As Double) As Builder
				Me.maskValue_Conflict = maskValue
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter underlying was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function underlying(ByVal underlying_Conflict As Layer) As Builder
				Me.Underlying = underlying_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER NOTE: The parameter maskValue was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function maskValue(ByVal maskValue_Conflict As Double) As Builder
				Me.MaskValue = maskValue_Conflict
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public MaskZeroLayer build()
			Public Overrides Function build() As MaskZeroLayer
				Return New MaskZeroLayer(Me)
			End Function
		End Class

	End Class

End Namespace