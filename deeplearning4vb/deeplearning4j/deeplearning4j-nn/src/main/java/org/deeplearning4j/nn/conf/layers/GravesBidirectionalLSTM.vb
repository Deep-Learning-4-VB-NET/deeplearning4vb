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
Imports GravesBidirectionalLSTMParamInitializer = org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Activation = org.nd4j.linalg.activations.Activation
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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) @Deprecated public class GravesBidirectionalLSTM extends BaseRecurrentLayer
	<Obsolete, Serializable>
	Public Class GravesBidirectionalLSTM
		Inherits BaseRecurrentLayer

		Private forgetGateBiasInit As Double
		Private gateActivationFn As IActivation = New ActivationSigmoid()
		Protected Friend helperAllowFallback As Boolean = True

		Private Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.forgetGateBiasInit = builder.forgetGateBiasInit_Conflict
			Me.gateActivationFn = builder.gateActivationFn
			Me.helperAllowFallback = builder.helperAllowFallback_Conflict

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
					Dim s As ISet(Of String) = New HashSet(Of String)()
					s.Add(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS)
					s.Add(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS)
					c2.Params = s
					constraints.Add(c2)
				Next c
			End If
		End Sub

		Public Overrides Function instantiate(ByVal conf As NeuralNetConfiguration, ByVal trainingListeners As ICollection(Of TrainingListener), ByVal layerIndex As Integer, ByVal layerParamsView As INDArray, ByVal initializeParams As Boolean, ByVal networkDataType As DataType) As Layer
			Dim ret As New org.deeplearning4j.nn.layers.recurrent.GravesBidirectionalLSTM(conf, networkDataType)
			ret.setListeners(trainingListeners)
			ret.Index = layerIndex
			ret.ParamsViewArray = layerParamsView
			Dim paramTable As IDictionary(Of String, INDArray) = initializer().init(conf, layerParamsView, initializeParams)
			ret.ParamTable = paramTable
			ret.Conf = conf
			Return ret
		End Function

		Public Overrides Function initializer() As ParamInitializer
			Return GravesBidirectionalLSTMParamInitializer.Instance
		End Function

		Public Overrides Function getMemoryReport(ByVal inputType As InputType) As LayerMemoryReport
			Return LSTMHelpers.getMemoryReport(Me, inputType)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Getter @Setter public static class Builder extends BaseRecurrentLayer.Builder<Builder>
		Public Class Builder
			Inherits BaseRecurrentLayer.Builder(Of Builder)

			''' <summary>
			''' Set forget gate bias initalizations. Values in range 1-5 can potentially help with learning or longer-term
			''' dependencies.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field forgetGateBiasInit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend forgetGateBiasInit_Conflict As Double = 1.0

			''' <summary>
			''' Activation function for the LSTM gates. Note: This should be bounded to range 0-1: sigmoid or hard sigmoid,
			''' for example
			''' 
			''' </summary>
			Friend gateActivationFn As IActivation = New ActivationSigmoid()

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If false, the built-in
			''' (non-CuDNN) implementation for GravesBidirectionalLSTM will be used
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field helperAllowFallback was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend helperAllowFallback_Conflict As Boolean = True

			''' <summary>
			''' Set forget gate bias initalizations. Values in range 1-5 can potentially help with learning or longer-term
			''' dependencies.
			''' </summary>
			Public Overridable Function forgetGateBiasInit(ByVal biasInit As Double) As Builder
				Me.setForgetGateBiasInit(biasInit)
				Return Me
			End Function

			''' <summary>
			''' Activation function for the LSTM gates. Note: This should be bounded to range 0-1: sigmoid or hard sigmoid,
			''' for example
			''' </summary>
			''' <param name="gateActivationFn"> Activation function for the LSTM gates </param>
			Public Overridable Function gateActivationFunction(ByVal gateActivationFn As String) As Builder
				Return gateActivationFunction(Activation.fromString(gateActivationFn))
			End Function

			''' <summary>
			''' Activation function for the LSTM gates. Note: This should be bounded to range 0-1: sigmoid or hard sigmoid,
			''' for example
			''' </summary>
			''' <param name="gateActivationFn"> Activation function for the LSTM gates </param>
			Public Overridable Function gateActivationFunction(ByVal gateActivationFn As Activation) As Builder
				Return gateActivationFunction(gateActivationFn.getActivationFunction())
			End Function

			''' <summary>
			''' Activation function for the LSTM gates. Note: This should be bounded to range 0-1: sigmoid or hard sigmoid,
			''' for example
			''' </summary>
			''' <param name="gateActivationFn"> Activation function for the LSTM gates </param>
			Public Overridable Function gateActivationFunction(ByVal gateActivationFn As IActivation) As Builder
				Me.setGateActivationFn(gateActivationFn)
				Return Me
			End Function

			''' <summary>
			''' When using a helper (CuDNN or MKLDNN in some cases) and an error is encountered, should fallback to the non-helper implementation be allowed?
			''' If set to false, an exception in the helper will be propagated back to the user. If false, the built-in
			''' (non-helper) implementation for GravesBidirectionalLSTM will be used
			''' </summary>
			''' <param name="allowFallback"> Whether fallback to non-helper implementation should be used </param>
			Public Overridable Function helperAllowFallback(ByVal allowFallback As Boolean) As Builder
				Me.setHelperAllowFallback(allowFallback)
				Return CType(Me, Builder)
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuppressWarnings("unchecked") public GravesBidirectionalLSTM build()
			Public Overridable Function build() As GravesBidirectionalLSTM
				Return New GravesBidirectionalLSTM(Me)
			End Function
		End Class

	End Class

End Namespace