Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports BaseActivationFunction = org.nd4j.linalg.activations.BaseActivationFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports EluBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.EluBp
Imports ELU = org.nd4j.linalg.api.ops.impl.transforms.strict.ELU
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Getter public class ActivationELU extends org.nd4j.linalg.activations.BaseActivationFunction
	<Serializable>
	Public Class ActivationELU
		Inherits BaseActivationFunction

		Public Const DEFAULT_ALPHA As Double = 1.0

		Private alpha As Double

		Public Sub New()
			Me.New(DEFAULT_ALPHA)
		End Sub

		Public Sub New(ByVal alpha As Double)
			Me.alpha = alpha
		End Sub

	'    
	'             = alpha * (exp(x) - 1.0); x < 0
	'       f(x)
	'             = x ; x >= 0
	'     
		Public Overrides Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray
			Return Nd4j.exec(New ELU([in], [in], alpha))(0)
		End Function

	'    
	'             = alpha * exp(x) ; x < 0
	'       f'(x)
	'             = 1 ; x >= 0
	'     
		Public Overrides Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray)
			assertShape([in], epsilon)
			Nd4j.Executioner.execAndReturn(New EluBp([in], epsilon, [in]))
			Return New Pair(Of INDArray, INDArray)([in], Nothing)
		End Function

		Public Overrides Function ToString() As String
			Return "elu(alpha=" & alpha & ")"
		End Function
	End Class

End Namespace