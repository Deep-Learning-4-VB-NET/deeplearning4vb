Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffUtils = org.nd4j.autodiff.util.SameDiffUtils
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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


	Public Class JaccardDistance
		Inherits BaseReduce3Op

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			extraArgs = New Object(){0.0f, 0.0f}
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New()

		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, Nothing, False, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, allDistances, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, z, False, allDistances, dimensions)
			Me.isComplex = allDistances
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			Me.New(x, y, z, False, Nothing)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal allDistances As Boolean)
			Me.New(x, y)
			Me.isComplex = allDistances
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, allDistances, dimensions)
			extraArgs = New Object(){0.0f, 0.0f}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sd,x,y,keepDims,isComplex,dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(x,y,Nothing,keepDims,isComplex,dimensions)
		End Sub

		Public Overrides Function opType() As Type
			Return Type.REDUCE3
		End Function

		Public Overrides ReadOnly Property OpType As Type
			Get
				Return opType()
			End Get
		End Property

		Public Overrides Function opNum() As Integer
			Return 6
		End Function

		Public Overrides Function opName() As String
			Return "jaccarddistance"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'Jaccard distance: https://en.wikipedia.org/wiki/Jaccard_index#Generalized_Jaccard_similarity_and_distance
			'J(x,y) = 1 - sum_i min(x_i, y_i) / sum_i max(x_i, y_i)

			Dim min As SDVariable = sameDiff.math_Conflict.min(larg(), rarg())
			Dim max As SDVariable = sameDiff.math_Conflict.max(larg(), rarg())
			Dim sumMax As SDVariable = max.sum(True, dimensions)
			Dim sumMin As SDVariable = min.sum(True, dimensions)

			Dim d As DataType = arg().dataType()
			Dim xIsMin As SDVariable = sameDiff.eq(min, larg()).castTo(d)
			Dim xIsMax As SDVariable = sameDiff.eq(max, larg()).castTo(d)
			Dim yIsMin As SDVariable = sameDiff.eq(min, rarg()).castTo(d)
			Dim yIsMax As SDVariable = sameDiff.eq(max, rarg()).castTo(d)

			Dim sqSumMax As SDVariable = sameDiff.math_Conflict.square(sumMax)
			Dim dldx As SDVariable = xIsMax.mul(sumMin).sub(xIsMin.mul(sumMax)).div(sqSumMax)
			Dim dldy As SDVariable = yIsMax.mul(sumMin).sub(yIsMin.mul(sumMax)).div(sqSumMax)

			Dim bcGradOut As SDVariable
			If keepDims_Conflict OrElse dimensions Is Nothing OrElse dimensions.Length = 0 OrElse (dimensions.Length = 1 AndAlso dimensions(0) = Integer.MaxValue) Then
				'KeepDims or full array reduction - already broadcastable
				bcGradOut = f1(0)
			Else
				bcGradOut = SameDiffUtils.reductionBroadcastableWithOrigShape(arg(), sameDiff.constant(Nd4j.createFromArray(dimensions)), f1(0))
			End If
			Return New List(Of SDVariable) From {dldx.mul(bcGradOut), dldy.mul(bcGradOut)}
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())

		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function resultType() As DataType
			Return Nd4j.defaultFloatingPointType()
		End Function
	End Class

End Namespace