Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Setter = lombok.Setter
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports BaseRecurrentLayer = org.deeplearning4j.nn.conf.layers.BaseRecurrentLayer
Imports LayerValidation = org.deeplearning4j.nn.conf.layers.LayerValidation
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports SimpleRnnParamInitializer = org.deeplearning4j.nn.params.SimpleRnnParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.deeplearning4j.nn.conf.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class SimpleRnn extends org.deeplearning4j.nn.conf.layers.BaseRecurrentLayer
	<Serializable>
	Public Class SimpleRnn
		Inherits BaseRecurrentLayer

'JAVA TO VB CONVERTER NOTE: The field hasLayerNorm was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private hasLayerNorm_Conflict As Boolean = False

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.hasLayerNorm_Conflict = builder.hasLayerNorm_Conflict
		End Sub

		Private Sub New()

		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("SimpleRnn", LayerName, layerIndex, getNIn(), getNOut())

			Dim ret As New org.deeplearning4j.nn.layers.recurrent.SimpleRnn(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return SimpleRnnParamInitializer.Instance
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return Nothing
		End Function

		Public Overridable Function hasLayerNorm() As Boolean
			Return hasLayerNorm_Conflict
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor @Getter @Setter public static class Builder extends org.deeplearning4j.nn.conf.layers.BaseRecurrentLayer.Builder<Builder>
		Public Class Builder
			Inherits BaseRecurrentLayer.Builder(Of Builder)

			Public Overrides Function build() As SimpleRnn
				Return New SimpleRnn(Me)
			End Function

			''' <summary>
			''' If true (default = false): enable layer normalization on this layer
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field hasLayerNorm was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend hasLayerNorm_Conflict As Boolean = False
'JAVA TO VB CONVERTER NOTE: The parameter hasLayerNorm was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function hasLayerNorm(ByVal hasLayerNorm_Conflict As Boolean) As Builder
				Me.hasLayerNorm_Conflict = hasLayerNorm_Conflict
				Return Me
			End Function
		End Class
	End Class

End Namespace