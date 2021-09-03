Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports FeedForwardLayer = org.deeplearning4j.nn.conf.layers.FeedForwardLayer
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
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

Namespace org.deeplearning4j.nn.layers.custom.testclasses


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class CustomLayer extends org.deeplearning4j.nn.conf.layers.FeedForwardLayer
	<Serializable>
	Public Class CustomLayer
		Inherits FeedForwardLayer

		Private ReadOnly someCustomParameter As Double

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CustomLayer(@JsonProperty("someCustomParameter") double someCustomParameter)
		Public Sub New(ByVal someCustomParameter As Double)
			Me.someCustomParameter = someCustomParameter
			Me.nIn = 10
			Me.nOut = 10
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim ret As New CustomLayerImpl(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return DefaultParamInitializer.Instance
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			Return InputType.feedForward(10)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			'No op
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return Nothing
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Throw New System.NotSupportedException()
		End Function
	End Class

End Namespace