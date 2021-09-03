Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray

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


	Public Class CosineDistance
		Inherits BaseReduce3Op

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			Me.New(x, y, z, Nothing)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimension() As Integer)
			MyBase.New(x, y, z, dimension)
			extraArgs = New Object(){0.0f, 0.0f}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimension() As Integer)
			Me.New(x, y, Nothing, dimension)
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

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sd,x,y,keepDims,isComplex,dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal keepDims As Boolean, ByVal isComplex As Boolean, ByVal dimensions() As Integer)
			MyBase.New(x,y,Nothing,keepDims,isComplex,dimensions)
		End Sub



		Public Overrides Function opNum() As Integer
			Return 5
		End Function

		Public Overrides Function opName() As String
			Return "cosinedistance"
		End Function



		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'Cosine distance = 1 - cosine similarity
			'Therefore: just need to negate gradients from cosine similarity...

			Dim diff As IList(Of SDVariable) = CosineSimilarity.doDiff(sameDiff, larg(), rarg(), i_v1(0), keepDims_Conflict, dimensions)
			Return New List(Of SDVariable) From {sameDiff.math_Conflict.neg(Me.diff(0)), sameDiff.math_Conflict.neg(Me.diff(1))}
		End Function
	End Class

End Namespace