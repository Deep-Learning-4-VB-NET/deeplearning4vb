Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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

Namespace org.nd4j.linalg.api.ops.impl.shape



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class Cross extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Cross
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			MyBase.New(Nothing, sameDiff, args, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal a As SDVariable, ByVal b As SDVariable)
			Me.New(sameDiff, New SDVariable(){a, b})
		End Sub

		Public Sub New(ByVal a As INDArray, ByVal b As INDArray)
			Me.New(a,b,Nothing)
		End Sub

		Public Sub New(ByVal a As INDArray, ByVal b As INDArray, ByVal [out] As INDArray)
			MyBase.New(Nothing, New INDArray(){a, b}, wrapOrNull([out]), Nothing, DirectCast(Nothing, Integer()))
		End Sub

		Public Overrides Function opName() As String
			Return "cross"
		End Function


		Public Overrides Function tensorflowName() As String
			Return "Cross"
		End Function

		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			''' <summary>
			''' dL / dx = dL / dCross * dCross / dx
			''' dCross(a,b) / da = Cross(1, b)
			''' dCross(a,b) / db = Cross(a, 1)
			''' 
			''' return (grad * Cross(1, b), grad * Cross(a, 1)
			''' </summary>
			Dim grad As SDVariable = gradients(0)
			Dim a As SDVariable = larg()
			Dim b As SDVariable = rarg()
			Dim ones As SDVariable = sameDiff.onesLike(a)

			Dim gradLeft As SDVariable = grad.mul(sameDiff.math().cross(b, ones))
			Dim gradRight As SDVariable = grad.mul(sameDiff.math().cross(ones, a))

			Return New List(Of SDVariable) From {gradLeft, gradRight}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes.Count = 2, "Expected list with exactly 2 datatype for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as input type
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace