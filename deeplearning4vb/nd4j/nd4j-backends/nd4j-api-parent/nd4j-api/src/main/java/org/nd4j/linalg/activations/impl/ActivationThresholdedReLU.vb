Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports BaseActivationFunction = org.nd4j.linalg.activations.BaseActivationFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives

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

Namespace org.nd4j.linalg.activations.impl

	''' <summary>
	''' Thresholded RELU
	''' 
	''' f(x) = x for x > theta, f(x) = 0 otherwise. theta defaults to 1.0
	''' 
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Getter public class ActivationThresholdedReLU extends org.nd4j.linalg.activations.BaseActivationFunction
	<Serializable>
	Public Class ActivationThresholdedReLU
		Inherits BaseActivationFunction

		Public Const DEFAULT_THETA As Double = 1.0
		Private theta As Double

		Public Sub New()
			Me.New(DEFAULT_THETA)
		End Sub

		Public Sub New(ByVal theta As Double)
			Me.theta = theta
		End Sub

		Public Overrides Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray
			Dim threshRelu As DynamicCustomOp = DynamicCustomOp.builder("thresholdedrelu").addOutputs([in]).addInputs([in]).addFloatingPointArguments(theta).build()
			Nd4j.Executioner.execAndReturn(threshRelu)
			Return [in]
		End Function

		Public Overrides Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray)
			assertShape([in], epsilon)
			Dim threshReluBp As DynamicCustomOp = DynamicCustomOp.builder("thresholdedrelu_bp").addInputs([in], epsilon).addOutputs([in]).addFloatingPointArguments(theta).build()
			Nd4j.Executioner.execAndReturn(threshReluBp)
			Return New Pair(Of INDArray, INDArray)([in], Nothing)
		End Function

		Public Overrides Function ToString() As String
			Return "thresholdedrelu(theta=" & theta & ")"
		End Function
	End Class

End Namespace