Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports org.nd4j.linalg.api.ops.impl.scalar
Imports LeakyReLUBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUBp
Imports org.nd4j.common.primitives
Imports BaseActivationFunction = org.nd4j.linalg.activations.BaseActivationFunction
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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
	''' f(x) = max(0, x)
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = false) @Getter public class ActivationReLU extends org.nd4j.linalg.activations.BaseActivationFunction
	<Serializable>
	Public Class ActivationReLU
		Inherits BaseActivationFunction

		Private max As Double?
		Private threshold As Double?
		Private negativeSlope As Double?

		Public Sub New()
			Me.New(Nothing, Nothing, Nothing)
		End Sub

		Public Sub New(ByVal maxValue As Double?, ByVal threshold As Double?, ByVal negativeSlope As Double?)
			Me.max = maxValue
			Me.threshold = threshold
			Me.negativeSlope = negativeSlope
		End Sub

		Public Overrides Function getActivation(ByVal [in] As INDArray, ByVal training As Boolean) As INDArray
			If negativeSlope IsNot Nothing OrElse threshold IsNot Nothing Then
				Dim t As Double = If(threshold Is Nothing, 0.0, threshold)
				Dim ns As Double = If(negativeSlope Is Nothing, 0.0, negativeSlope)
				If t = 0.0 Then
					Nd4j.Executioner.execAndReturn(New LeakyReLU([in], ns))
				Else
					'Non-zero threshold, and non-zero slope
					'TODO optimize this... but, extremely rare case in practice?
					Dim oneGte As INDArray = [in].gte(t).castTo([in].dataType())
					Dim oneLt As INDArray = [in].lt(t).castTo([in].dataType())
					Dim lower As INDArray = oneLt.muli(ns).muli([in].sub(threshold))
					Dim upper As INDArray = oneGte.muli([in])
					[in].assign(lower.addi(upper))
				End If
			Else
				Nd4j.Executioner.exec(New RectifiedLinear([in], [in]))
			End If
			If max IsNot Nothing Then
				Nd4j.exec(New ScalarMin([in], Nothing, [in], max))
			End If
			Return [in]
		End Function

		Public Overrides Function backprop(ByVal [in] As INDArray, ByVal epsilon As INDArray) As Pair(Of INDArray, INDArray)
			assertShape([in], epsilon)

			Dim dLdz As INDArray
			Dim maxMask As INDArray = (If(max Is Nothing OrElse max = 0.0, Nothing, [in].lt(max)))
			If negativeSlope IsNot Nothing OrElse threshold IsNot Nothing Then
				Dim t As Double = If(threshold Is Nothing, 0.0, threshold)
				Dim ns As Double = If(negativeSlope Is Nothing, 0.0, negativeSlope)
				If t = 0.0 Then
					dLdz = Nd4j.Executioner.exec(New LeakyReLUBp([in], epsilon, [in].ulike(), ns))(0)
				Else
					'Non-zero threshold, and non-zero slope
					'TODO optimize this... but, extremely rare case in practice?
					Dim oneGte As INDArray = [in].gte(t).castTo([in].dataType())
					Dim oneLt As INDArray = [in].lt(t).castTo([in].dataType())
					Dim lower As INDArray = oneLt.muli(ns)
					Dim upper As INDArray = oneGte
					dLdz = [in].assign(lower.addi(upper)).muli(epsilon)
				End If
			Else
				dLdz = Nd4j.Executioner.exec(New RectifiedLinearDerivative([in], epsilon, [in].ulike(),If(threshold Is Nothing, 0.0, threshold)))(0)
			End If

			If maxMask IsNot Nothing Then
				dLdz.muli(maxMask)
			End If
			Return New Pair(Of INDArray, INDArray)(dLdz, Nothing)
		End Function

		Public Overrides Function ToString() As String
			Return "relu"
		End Function

	End Class

End Namespace