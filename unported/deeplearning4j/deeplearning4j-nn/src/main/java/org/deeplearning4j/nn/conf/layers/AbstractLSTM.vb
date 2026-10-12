Imports System
Imports lombok
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid

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
'ORIGINAL LINE: @Data @NoArgsConstructor @ToString(callSuper = true) @EqualsAndHashCode(callSuper = true) public abstract class AbstractLSTM extends BaseRecurrentLayer
	<Serializable>
	Public MustInherit Class AbstractLSTM
		Inherits BaseRecurrentLayer

		Protected Friend forgetGateBiasInit As Double
		Protected Friend gateActivationFn As IActivation = New ActivationSigmoid()
		Protected Friend helperAllowFallback As Boolean = True

		Protected Friend Sub New(ByVal builder As Builder)
			MyBase.New(builder)
			Me.forgetGateBiasInit = builder.forgetGateBiasInit
			Me.gateActivationFn = builder.gateActivationFn
			Me.helperAllowFallback = builder.helperAllowFallback
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Getter @Setter public static abstract class Builder<T extends Builder<T>> extends BaseRecurrentLayer.Builder<T>
		Public MustInherit Class Builder(Of T As Builder(Of T))
			Inherits BaseRecurrentLayer.Builder(Of T)

			''' <summary>
			''' Set forget gate bias initalizations. Values in range 1-5 can potentially help with learning or longer-term
			''' dependencies.
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field forgetGateBiasInit was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend forgetGateBiasInit_Conflict As Double = 1.0

			''' <summary>
			''' Activation function for the LSTM gates. Note: This should be bounded to range 0-1: sigmoid or hard sigmoid,
			''' for example
			''' </summary>
			Protected Friend gateActivationFn As IActivation = New ActivationSigmoid()

			''' <summary>
			''' When using CuDNN and an error is encountered, should fallback to the non-CuDNN implementatation be allowed?
			''' If set to false, an exception in CuDNN will be propagated back to the user. If false, the built-in
			''' (non-CuDNN) implementation for LSTM/GravesLSTM will be used
			''' 
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The field helperAllowFallback was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Protected Friend helperAllowFallback_Conflict As Boolean = True

			''' <summary>
			''' Set forget gate bias initalizations. Values in range 1-5 can potentially help with learning or longer-term
			''' dependencies.
			''' </summary>
			Public Overridable Function forgetGateBiasInit(ByVal biasInit As Double) As T
				Me.setForgetGateBiasInit(biasInit)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' Activation function for the LSTM gates. Note: This should be bounded to range 0-1: sigmoid or hard sigmoid,
			''' for example
			''' </summary>
			''' <param name="gateActivationFn"> Activation function for the LSTM gates </param>
			Public Overridable Function gateActivationFunction(ByVal gateActivationFn As String) As T
				Return CType(gateActivationFunction(Activation.fromString(gateActivationFn)), T)
			End Function

			''' <summary>
			''' Activation function for the LSTM gates. Note: This should be bounded to range 0-1: sigmoid or hard sigmoid,
			''' for example
			''' </summary>
			''' <param name="gateActivationFn"> Activation function for the LSTM gates </param>
			Public Overridable Function gateActivationFunction(ByVal gateActivationFn As Activation) As T
				Return CType(gateActivationFunction(gateActivationFn.getActivationFunction()), T)
			End Function

			''' <summary>
			''' Activation function for the LSTM gates. Note: This should be bounded to range 0-1: sigmoid or hard sigmoid,
			''' for example
			''' </summary>
			''' <param name="gateActivationFn"> Activation function for the LSTM gates </param>
			Public Overridable Function gateActivationFunction(ByVal gateActivationFn As IActivation) As T
				Me.setGateActivationFn(gateActivationFn)
				Return CType(Me, T)
			End Function

			''' <summary>
			''' When using a helper (CuDNN or MKLDNN in some cases) and an error is encountered, should fallback to the non-helper implementation be allowed?
			''' If set to false, an exception in the helper will be propagated back to the user. If false, the built-in
			''' (non-helper) implementation for LSTM/GravesLSTM will be used
			''' </summary>
			''' <param name="allowFallback"> Whether fallback to non-helper implementation should be used </param>
			Public Overridable Function helperAllowFallback(ByVal allowFallback As Boolean) As T
				Me.setHelperAllowFallback(allowFallback)
				Return CType(Me, T)
			End Function

		End Class

	End Class

End Namespace