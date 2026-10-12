Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports GradientNormalization = org.deeplearning4j.nn.conf.GradientNormalization
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports WrapperLayerParamInitializer = org.deeplearning4j.nn.params.WrapperLayerParamInitializer
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization

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

Namespace org.deeplearning4j.nn.conf.layers.wrapper


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public abstract class BaseWrapperLayer extends org.deeplearning4j.nn.conf.layers.Layer
	<Serializable>
	Public MustInherit Class BaseWrapperLayer
		Inherits Layer

		Protected Friend underlying As Layer

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
		End Sub

		Protected Friend Sub New()
		End Sub

		Public Sub New(ByVal underlying As Layer)
			Me.underlying = underlying
		End Sub

		Public Overrides Function initializer() As ParamInitializer
			Return WrapperLayerParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return underlying.getOutputType(layerIndex, inputType)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			underlying.setNIn(inputType, override)
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return underlying.getPreProcessorForInputType(inputType)
		End Function

		Public Overrides Function getRegularizationByParam(ByVal paramName As String) As IList(Of Regularization)
			Return underlying.getRegularizationByParam(paramName)
		End Function

		Public Overrides ReadOnly Property GradientNormalization As GradientNormalization
			Get
				Return underlying.GradientNormalization
			End Get
		End Property

		Public Overrides ReadOnly Property GradientNormalizationThreshold As Double
			Get
				Return underlying.GradientNormalizationThreshold
			End Get
		End Property

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return underlying.isPretrainParam(paramName)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return underlying.getMemoryReport(inputType)
		End Function

		Public Overrides WriteOnly Property LayerName As String
			Set(ByVal layerName As String)
				MyBase.setLayerName(layerName)
				If underlying IsNot Nothing Then
					'May be null at some points during JSON deserialization
					underlying.setLayerName(layerName)
				End If
			End Set
		End Property
	End Class

End Namespace