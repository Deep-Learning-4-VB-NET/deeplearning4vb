Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
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

Namespace org.nd4j.linalg.api.ops.impl.reduce3


	Public Class CosineSimilarity
		Inherits BaseReduce3Op

		Public Const OP_NAME As String = "cosinesimilarity"

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New()
			extraArgs = New Object(){0.0f, 0.0f}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
			extraArgs = New Object(){0.0f, 0.0f}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, Nothing, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			Me.New(x, y, z, Nothing)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal allDistances As Boolean, ParamArray ByVal dimension() As Integer)
			Me.New(x, y, z, dimension)
			Me.isComplex = allDistances
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal allDistances As Boolean, ParamArray ByVal dimension() As Integer)
			Me.New(x, y, Nothing, allDistances, dimension)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, allDistances, dimensions)
			extraArgs = New Object(){0.0f, 0.0f}
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sd, x, y, keepDims, isComplex, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(x,y,Nothing,keepDims,isComplex,dimensions)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 2
		End Function

		Public Overrides Function opName() As String
			Return OP_NAME
		End Function

		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'Let cosine(x,y) = a / b
			'a = sum_i (x_i * y_i)
			'b = sqrt(sum_i x_i^2) * sqrt(sum_i y_i^2) = l2(x) * l2(y)
			'Then:
			' dc(x,y)/dx_i = 1/b * (y - x * a / (l2(x))^2)

			Return doDiff(sameDiff, larg(), rarg(), i_v1(0), keepDims_Conflict, dimensions)
		End Function

		Public Shared Overloads Function doDiff(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal gradOut As SDVariable, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer) As IList(Of SDVariable)
			Dim a As SDVariable = sameDiff.sum(x.mul(y),True, dimensions)
			Dim l2x As SDVariable = sameDiff.norm2(x, True, dimensions)
			Dim l2y As SDVariable = sameDiff.norm2(y, True, dimensions)
			Dim b As SDVariable = l2x.mul(l2y)

			Dim l2xSq As SDVariable = sameDiff.math().square(l2x)
			Dim l2ySq As SDVariable = sameDiff.math().square(l2y)
			Dim broadcastableGrad As SDVariable
			If keepDims OrElse dimensions Is Nothing OrElse dimensions.Length = 0 OrElse (dimensions.Length = 1 AndAlso dimensions(0) = Integer.MaxValue) Then
				'keepDims or full array reduction
				broadcastableGrad = gradOut
			Else
				broadcastableGrad = SameDiffUtils.reductionBroadcastableWithOrigShape(x, sameDiff.constant(Nd4j.createFromArray(dimensions)), gradOut)
			End If

			Dim dcdx As SDVariable = y.sub(x.mul(a).div(l2xSq)).div(b)
			Dim dcdy As SDVariable = x.sub(y.mul(a).div(l2ySq)).div(b)

			Return New List(Of SDVariable) From {dcdx.mul(broadcastableGrad), dcdy.mul(broadcastableGrad)}
		End Function
	End Class

End Namespace