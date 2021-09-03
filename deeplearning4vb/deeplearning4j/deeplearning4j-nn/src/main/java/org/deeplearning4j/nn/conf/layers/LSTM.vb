Imports System
Imports System.Collections.Generic
Imports lombok
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ParamInitializer = org.deeplearning4j.nn.api.ParamInitializer
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports LSTMHelpers = org.deeplearning4j.nn.layers.recurrent.LSTMHelpers
Imports LSTMParamInitializer = org.deeplearning4j.nn.params.LSTMParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public class LSTM extends AbstractLSTM
	<Serializable>
	Public Class LSTM
		Inherits AbstractLSTM

		Private Shadows forgetGateBiasInit As Double
		Private Shadows gateActivationFn As IActivation = New ActivationSigmoid()

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.forgetGateBiasInit = builder.forgetGateBiasInit_Conflict
			Me.gateActivationFn = builder.gateActivationFn
			initializeConstraints(builder)
		End Sub

		Protected Friend Overrides Sub initializeConstraints(Of T1)(ByVal builder As org.deeplearning4j.nn.conf.layers.Layer.Builder(Of T1))
			MyBase.initializeConstraints(builder)
			If DirectCast(builder, Builder).recurrentConstraints IsNot Nothing Then
				If constraints Is Nothing Then
					constraints = New List(Of LayerConstraint)()
				End If
				For Each c As LayerConstraint In (DirectCast(builder, Builder)).recurrentConstraints
					Dim c2 As LayerConstraint = c.clone()
					c2.Params = Collections.singleton(LSTMParamInitializer.RECURRENT_WEIGHT_KEY)
					constraints.Add(c2)
				Next c
			End If
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			LayerValidation.assertNInNOutSet("LSTM", LayerName, layerIndex, getNIn(), getNOut())
			Dim ret As New org.deeplearning4j.nn.layers.recurrent.LSTM(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return LSTMParamInitializer.Instance
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			'TODO - CuDNN etc
			Return LSTMHelpers.getMemoryReport(Me, inputType)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public static class Builder extends AbstractLSTM.Builder<Builder>
		Public Class Builder
			Inherits AbstractLSTM.Builder(Of Builder)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public LSTM build()
			Public Overridable Function build() As LSTM
				Return New LSTM(Me)
			End Function
		End Class

	End Class

End Namespace