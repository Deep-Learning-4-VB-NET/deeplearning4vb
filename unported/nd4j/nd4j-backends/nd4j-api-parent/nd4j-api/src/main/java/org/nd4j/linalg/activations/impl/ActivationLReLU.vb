Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports BaseActivationFunction = org.nd4j.linalg.activations.BaseActivationFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LeakyReLU = org.nd4j.linalg.api.ops.impl.scalar.LeakyReLU
Imports LeakyReLUBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUBp
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
	''' Leaky RELU
	''' f(x) = max(0, x) + alpha * min(0, x)
	''' alpha defaults to 0.01
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Getter public class ActivationLReLU extends org.nd4j.linalg.activations.BaseActivationFunction
	<Serializable>
	Public Class ActivationLReLU
		Inherits BaseActivationFunction

		Public Const DEFAULT_ALPHA As Double = 0.01

		Private alpha As Double

		Public Sub New()
			Me.New(DEFAULT_ALPHA)
		End Sub

		Public Sub New(ByVal alpha As Double)
			Me.alpha = alpha
		End Sub

		Public Overrides Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray
			Nd4j.Executioner.execAndReturn(New LeakyReLU([in], alpha))
			Return [in]
		End Function

		Public Overrides Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray)
			assertShape([in], epsilon)

			Nd4j.Executioner.execAndReturn(New LeakyReLUBp([in], epsilon, [in], alpha))

			Return New Pair(Of INDArray, INDArray)([in], Nothing)
		End Function

		Public Overrides Function ToString() As String
			Return "leakyrelu(a=" & alpha & ")"
		End Function
	End Class

End Namespace