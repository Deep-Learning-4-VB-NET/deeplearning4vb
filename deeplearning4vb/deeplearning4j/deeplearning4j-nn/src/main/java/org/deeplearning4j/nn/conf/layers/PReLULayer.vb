Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports PReLUParamInitializer = org.deeplearning4j.nn.params.PReLUParamInitializer
Imports WeightInitConstant = org.deeplearning4j.nn.weights.WeightInitConstant
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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class PReLULayer extends BaseLayer
	<Serializable>
	Public Class PReLULayer
		Inherits BaseLayer

		Private inputShape() As Long = Nothing
		Private sharedAxes() As Long = Nothing

		Private nIn As Integer
		Private nOut As Integer

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.inputShape = builder.inputShape_Conflict
			Me.sharedAxes = builder.sharedAxes_Conflict
			initializeConstraints(builder)
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.feedforward.PReLU(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType Is Nothing Then
				Throw New System.InvalidOperationException("Invalid input type: null for layer name """ & LayerName & """")
			End If
			Return inputType
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			' not needed
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			' None needed
			Return Nothing
		End Function

		Public Overrides Function isPretrainParam(ByVal paramName As String) As Boolean
			Return False
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return PReLUParamInitializer.getInstance(inputShape, sharedAxes)
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Dim outputType As InputType = getOutputType(-1, inputType)

			Dim numParams As val = initializer().numParams(Me)
			Dim updaterStateSize As val = CInt(Math.Truncate(getIUpdater().stateSize(numParams)))

			Return (New LayerMemoryReport.Builder(layerName, GetType(PReLULayer), inputType, outputType)).standardMemory(numParams, updaterStateSize).workingMemory(0, 0, 0, 0).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, MemoryReport.CACHE_MODE_ALL_ZEROS).build()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter public static class Builder extends FeedForwardLayer.Builder<Builder>
		Public Class Builder
			Inherits FeedForwardLayer.Builder(Of Builder)

			Public Sub New()
				'Default to 0s, and don't inherit global default
				Me.weightInitFn = New WeightInitConstant(0)
			End Sub

			''' <summary>
			''' Explicitly set input shape of incoming activations so that parameters can be initialized properly. This
			''' explicitly excludes the mini-batch dimension.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field inputShape was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend inputShape_Conflict() As Long = Nothing

			''' <summary>
			''' Set the broadcasting axes of PReLU's alpha parameter.
			''' 
			''' For instance, given input data of shape [mb, channels, height, width], setting axes to [2,3] will set alpha
			''' to shape [channels, 1, 1] and broadcast alpha across height and width dimensions of each channel.
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field sharedAxes was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend sharedAxes_Conflict() As Long = Nothing

			''' <summary>
			''' Explicitly set input shape of incoming activations so that parameters can be initialized properly. This
			''' explicitly excludes the mini-batch dimension.
			''' </summary>
			''' <param name="shape"> shape of input data </param>
			Public Overridable Function inputShape(ParamArray ByVal shape() As Long) As Builder
				Me.setInputShape(shape)
				Return Me
			End Function

			''' <summary>
			''' Set the broadcasting axes of PReLU's alpha parameter.
			''' 
			''' For instance, given input data of shape [mb, channels, height, width], setting axes to [2,3] will set alpha
			''' to shape [channels, 1, 1] and broadcast alpha across height and width dimensions of each channel.
			''' </summary>
			''' <param name="axes"> shared/broadcasting axes </param>
			''' <returns> Builder </returns>
			Public Overridable Function sharedAxes(ParamArray ByVal axes() As Long) As Builder
				Me.setSharedAxes(axes)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public PReLULayer build()
			Public Overrides Function build() As PReLULayer
				Return New PReLULayer(Me)
			End Function
		End Class

	End Class

End Namespace