Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports BaseActivationFunction = org.nd4j.linalg.activations.BaseActivationFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports GELU = org.nd4j.linalg.api.ops.impl.transforms.strict.GELU
Imports GELUDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.GELUDerivative
Imports PreciseGELU = org.nd4j.linalg.api.ops.impl.transforms.strict.PreciseGELU
Imports PreciseGELUDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.PreciseGELUDerivative
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Getter public class ActivationGELU extends org.nd4j.linalg.activations.BaseActivationFunction
	<Serializable>
	Public Class ActivationGELU
		Inherits BaseActivationFunction

		Private precise As Boolean

		Public Sub New(ByVal precise As Boolean)
			Me.precise = precise
		End Sub

		Public Sub New()
			Me.New(False)
		End Sub

		Public Overrides Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray
			If precise Then
				Nd4j.Executioner.execAndReturn(New PreciseGELU([in], [in]))
			Else
				Nd4j.Executioner.execAndReturn(New GELU([in], [in]))
			End If
			Return [in]
		End Function

		Public Overrides Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray)
			assertShape([in], epsilon)
			Dim dLdz As INDArray
			If precise Then
				dLdz = Nd4j.Executioner.exec(New PreciseGELUDerivative([in], [in]))
			Else
				dLdz = Nd4j.Executioner.exec(New GELUDerivative([in], [in]))
			End If

			dLdz.muli(epsilon)
			Return New Pair(Of INDArray, INDArray)(dLdz, Nothing)
		End Function

		Public Overrides Function ToString() As String
			Return "gelu(precise=" & precise & ")"
		End Function

	End Class

End Namespace