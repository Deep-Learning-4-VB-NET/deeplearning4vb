Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseReduceLongOp = org.nd4j.linalg.api.ops.BaseReduceLongOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Condition = org.nd4j.linalg.indexing.conditions.Condition

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

Namespace org.nd4j.linalg.api.ops.impl.reduce.longer


	Public Class MatchCondition
		Inherits BaseReduceLongOp

		Private compare As Double
		Private eps As Double
		Private mode As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal condition As Condition)
			Me.New(sameDiff, [in], condition, False, Nothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, [in], dimensions, keepDims)
			Me.compare = condition.getValue()
			Me.mode = condition.condtionNum()
			Me.eps = Nd4j.EPS_THRESHOLD
			Me.extraArgs = New Object() {compare, eps, CDbl(mode)}
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer)
			Me.New(x, Nd4j.EPS_THRESHOLD, condition, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal condition As Condition, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			Me.New(x, Nd4j.EPS_THRESHOLD, condition, dimensions)
			Me.keepDims_Conflict = keepDims
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal eps As Double, ByVal condition As Condition, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x)
			Me.compare = condition.getValue()
			Me.mode = condition.condtionNum()
			Me.eps = eps

			Me.extraArgs = New Object() {compare, eps, CDbl(mode)}

			defineDimensions(dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal compare As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal compare As Double)
			MyBase.New(sameDiff, i_v, keepDims)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean, ByVal compare As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal compare As Double)
			MyBase.New(sameDiff, i_v, i_v2)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal compare As Double)
			MyBase.New(sameDiff, input, dimensions, keepDims)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal compare As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal compare As Double)
			MyBase.New(sameDiff, i_v)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal compare As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, input, dimensions)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal compare As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, dimensions)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal compare As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, dimensions)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal compare As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, z, dimensions)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal compare As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal compare As Double)
			MyBase.New(sameDiff)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal compare As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal compare As Double)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer, ByVal compare As Double)
			MyBase.New(x, y, z, keepDims, dimensions)
			Me.compare = compare
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, keepDims)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, i_v2)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(sameDiff, input, dimensions, keepDims)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(sameDiff, i_v)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal compare As Double, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, input, dimensions)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal compare As Double, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, dimensions)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, dimensions)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal compare As Double, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, z, dimensions)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal compare As Double, ByVal eps As Double, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(sameDiff)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal compare As Double, ByVal eps As Double)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer, ByVal compare As Double, ByVal eps As Double)
			MyBase.New(x, y, z, keepDims, dimensions)
			Me.compare = compare
			Me.eps = eps
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(sameDiff, i_v, keepDims)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(sameDiff, i_v, i_v2)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(sameDiff, input, dimensions, keepDims)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(sameDiff, i_v)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, input, dimensions)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, dimensions)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, dimensions)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, z, dimensions)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(sameDiff)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer, ByVal compare As Double, ByVal eps As Double, ByVal mode As Integer)
			MyBase.New(x, y, z, keepDims, dimensions)
			Me.compare = compare
			Me.eps = eps
			Me.mode = mode
		End Sub

		Public Overrides Function opNum() As Integer
			Return 2
		End Function

		Public Overrides Function opName() As String
			Return "match_condition"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
	End Class

End Namespace