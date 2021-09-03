Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseReduceFloatOp = org.nd4j.linalg.api.ops.BaseReduceFloatOp
Imports NormMaxBp = org.nd4j.linalg.api.ops.impl.reduce.bp.NormMaxBp
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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

Namespace org.nd4j.linalg.api.ops.impl.reduce.floating


	''' <summary>
	''' The max absolute value
	''' 
	''' @author Adam Gibson
	''' </summary>
	Public Class NormMax
		Inherits BaseReduceFloatOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, input, dimensions, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, input, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, input, dimensions, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, input, dimensions)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal output As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(input, output, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, Nothing, z, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal indArray As INDArray, ByVal keepDims As Boolean)
			MyBase.New([in],indArray,keepDims)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New([in],keepDims,dimensions)
		End Sub

		Public Overrides Function noOp() As INDArray
			Return Transforms.abs(x())
		End Function


		Public Overrides Function opNum() As Integer
			Return 4
		End Function

		Public Overrides Function opName() As String
			Return "reduce_normmax"
		End Function


		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'maxnorm(in) = max_i |x_i|
			'd maxnorm(in)/dx = 0 if x_i is not the max, or d|x|/dx otherwise
			Return (New NormMaxBp(sameDiff, arg(), grad(0), keepDims_Conflict, dimensions)).outputs()
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function
	End Class

End Namespace