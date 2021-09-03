Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports BaseWrapperLayer = org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
Imports TimeDistributedLayer = org.deeplearning4j.nn.layers.recurrent.TimeDistributedLayer
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

Namespace org.deeplearning4j.nn.conf.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @EqualsAndHashCode(callSuper = true) public class TimeDistributed extends org.deeplearning4j.nn.conf.layers.wrapper.BaseWrapperLayer
	<Serializable>
	Public Class TimeDistributed
		Inherits BaseWrapperLayer

		Private rnnDataFormat As RNNFormat = RNNFormat.NCW

		''' <param name="underlying"> Underlying (internal) layer - should be a feed forward type such as DenseLayer </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TimeDistributed(@JsonProperty("underlying") @NonNull Layer underlying, @JsonProperty("rnnDataFormat") org.deeplearning4j.nn.conf.RNNFormat rnnDataFormat)
		Public Sub New(ByVal underlying As Layer, ByVal rnnDataFormat As RNNFormat)
			MyBase.New(underlying)
			Me.rnnDataFormat = rnnDataFormat
		End Sub

		Public Sub New(ByVal underlying As Layer)
			MyBase.New(underlying)
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As org.deeplearning4j.nn.api.Layer
			Dim conf2 As NeuralNetConfiguration = conf.clone()
			conf2.setLayer(CType(conf2.getLayer(), TimeDistributed).getUnderlying())
			Return New TimeDistributedLayer(underlying.instantiate(conf2, trainingListeners, layerIndex, layerParamsView, initializeParams, networkDataType), rnnDataFormat)
		End Function

		Public Overrides Function getOutputType(ByVal layerIndex As Integer, ByVal inputType As InputType) As InputType
			If inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Only RNN input type is supported as input to TimeDistributed layer (layer #" & layerIndex & ")")
			End If

			Dim rnn As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Dim ff As InputType = InputType.feedForward(rnn.getSize())
			Dim ffOut As InputType = underlying.getOutputType(layerIndex, ff)
			Return InputType.recurrent(ffOut.arrayElementsPerExample(), rnn.getTimeSeriesLength(), rnnDataFormat)
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Only RNN input type is supported as input to TimeDistributed layer")
			End If

			Dim rnn As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Dim ff As InputType = InputType.feedForward(rnn.getSize())
			Me.rnnDataFormat = rnn.getFormat()
			underlying.setNIn(ff, override)
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			'No preprocessor - the wrapper layer operates as the preprocessor
			Return Nothing
		End Function
	End Class

End Namespace