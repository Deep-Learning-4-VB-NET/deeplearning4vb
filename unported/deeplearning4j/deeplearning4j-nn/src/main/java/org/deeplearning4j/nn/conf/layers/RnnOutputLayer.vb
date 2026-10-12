Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports ToString = lombok.ToString
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class RnnOutputLayer extends BaseOutputLayer
	<Serializable>
	Public Class RnnOutputLayer
		Inherits BaseOutputLayer

		Private rnnDataFormat As RNNFormat

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			initializeConstraints(builder)
			Me.rnnDataFormat = builder.rnnDataFormat
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("RnnOutputLayer", LayerName, layerIndex, getNIn(), getNOut())

			Dim ret As New org.deeplearning4j.nn.layers.recurrent.RnnOutputLayer(conf, networkDataType)
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
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input type for RnnOutputLayer (layer index = " & layerIndex & ", layer name=""" & LayerName & """): Expected RNN input, got " & inputType)
			End If
			Dim itr As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)

			Return InputType.recurrent(nOut, itr.getTimeSeriesLength(), itr.getFormat())
		End Function

		Public Overrides Sub setNIn(ByVal inputType As InputType, ByVal override As Boolean)
			If inputType Is Nothing OrElse inputType.getType() <> InputType.Type.RNN Then
				Throw New System.InvalidOperationException("Invalid input type for RnnOutputLayer (layer name=""" & LayerName & """): Expected RNN input, got " & inputType)
			End If

			Dim r As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			If rnnDataFormat = Nothing OrElse override Then
				Me.rnnDataFormat = r.getFormat()
			End If

			If nIn <= 0 OrElse override Then
				Me.nIn = r.getSize()
			End If
		End Sub

		Public Overrides Function getPreProcessorForInputType(ByVal inputType As InputType) As InputPreProcessor
			Return InputTypeUtil.getPreprocessorForInputTypeRnnLayers(inputType, rnnDataFormat, LayerName)
		End Function


		Public Class Builder
			Inherits BaseOutputLayer.Builder(Of Builder)

			Friend rnnDataFormat As RNNFormat
			Public Sub New()
				'Set default activation function to softmax (to match default loss function MCXENT)
				Me.setActivationFn(New ActivationSoftmax())
			End Sub

			''' <param name="lossFunction"> Loss function for the output layer </param>
			Public Sub New(ByVal lossFunction As LossFunction)
				Me.lossFunction(lossFunction)
				'Set default activation function to softmax (for consistent behaviour with no-arg constructor)
				Me.setActivationFn(New ActivationSoftmax())
			End Sub

			''' <param name="lossFunction"> Loss function for the output layer </param>
			Public Sub New(ByVal lossFunction As ILossFunction)
				Me.setLossFn(lossFunction)
				'Set default activation function to softmax (for consistent behaviour with no-arg constructor)
				Me.setActivationFn(New ActivationSoftmax())
			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override @SuppressWarnings("unchecked") public RnnOutputLayer build()
			Public Overrides Function build() As RnnOutputLayer
				Return New RnnOutputLayer(Me)
			End Function

			''' <param name="rnnDataFormat"> Data format expected by the layer. NCW = [miniBatchSize, size, timeSeriesLength],
			''' NWC = [miniBatchSize, timeSeriesLength, size]. Defaults to NCW. </param>
			Public Overridable Function dataFormat(ByVal rnnDataFormat As RNNFormat) As Builder
				Me.rnnDataFormat = rnnDataFormat
				Return Me
			End Function
		End Class
	End Class

End Namespace