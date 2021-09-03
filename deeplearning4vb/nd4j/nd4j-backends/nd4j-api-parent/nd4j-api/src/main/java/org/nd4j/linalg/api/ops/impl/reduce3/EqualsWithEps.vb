Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
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


	Public Class EqualsWithEps
		Inherits BaseReduce3Op

		Private eps As Double

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, dimensions)
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, dimensions)
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.eps = eps
		End Sub

		Public Sub New(ByVal eps As Double)
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.eps = eps
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, allDistances, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, False, dimensions)
			Me.extraArgs = New Object() {0.0, 0.0, eps}
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			Me.New(x, y, Nothing, eps, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal allDistances As Boolean, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, allDistances, dimensions)
			Me.eps = eps
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal eps As Double)
			MyBase.New(x, y, z)
			Me.eps = eps
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, dimensions)
			Me.eps = eps
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal allDistances As Boolean, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, allDistances, dimensions)
			Me.eps = eps
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray)
			Me.New(x, y, z, Nd4j.EPS_THRESHOLD, Nothing)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal allDistances As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, allDistances, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 4
		End Function

		Public Overrides Function opName() As String
			Return "equals_with_eps"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {outputVariables()(0)}
		End Function
	End Class

End Namespace