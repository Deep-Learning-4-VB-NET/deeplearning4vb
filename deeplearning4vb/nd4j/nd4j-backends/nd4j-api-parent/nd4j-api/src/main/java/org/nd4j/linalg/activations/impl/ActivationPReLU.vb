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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Getter public class ActivationPReLU extends org.nd4j.linalg.activations.BaseActivationFunction
	<Serializable>
	Public Class ActivationPReLU
		Inherits BaseActivationFunction

		Private alpha As INDArray
		Private sharedAxes() As Long = Nothing

		Public Sub New(ByVal alpha As INDArray, ByVal sharedAxes() As Long)
			Me.alpha = alpha
			Me.sharedAxes = sharedAxes
		End Sub

		Public Overrides Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray
			Dim prelu As DynamicCustomOp.DynamicCustomOpsBuilder = DynamicCustomOp.builder("prelu").addOutputs([in]).addInputs([in], alpha)
			If sharedAxes IsNot Nothing Then
				For Each axis As Long In sharedAxes
					prelu.addIntegerArguments(axis)
				Next axis
			End If
			Nd4j.Executioner.execAndReturn(prelu.build())
			Return [in]
		End Function

		Public Overrides Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray)
			assertShape([in], epsilon)
			Dim dLdalpha As INDArray = alpha.ulike()
			Dim outTemp As INDArray = [in].ulike()
			Dim preluBp As DynamicCustomOp.DynamicCustomOpsBuilder = DynamicCustomOp.builder("prelu_bp").addInputs([in], alpha, epsilon).addOutputs(outTemp, dLdalpha)

			If sharedAxes IsNot Nothing Then
				For Each axis As Long In sharedAxes
					preluBp.addIntegerArguments(axis)
				Next axis
			End If
			Nd4j.exec(preluBp.build())
			[in].assign(outTemp)
			Return New Pair(Of INDArray, INDArray)([in], dLdalpha)
		End Function

		Public Overrides Function ToString() As String
			Return "prelu"
		End Function
	End Class
End Namespace