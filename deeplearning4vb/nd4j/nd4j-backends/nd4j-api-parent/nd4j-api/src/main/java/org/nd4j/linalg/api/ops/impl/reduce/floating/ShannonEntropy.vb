﻿Imports System
Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseReduceFloatOp = org.nd4j.linalg.api.ops.BaseReduceFloatOp
Imports SumBp = org.nd4j.linalg.api.ops.impl.reduce.bp.SumBp

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


	Public Class ShannonEntropy
		Inherits BaseReduceFloatOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, dimensions)
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

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, input, dimensions, keepDims)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, Nothing, z, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal dimensions() As Integer, ByVal [in] As INDArray, ByVal indArray As INDArray, ByVal keepDims As Boolean)
			MyBase.New(x, dimensions)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New([in],keepDims,dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, dimensions)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal dimensions As INDArray, ByVal keepDims As Boolean)
			MyBase.New([in],keepDims,dimensions.toIntVector())
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New([in],keepDims,dimensions)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 10
		End Function

		Public Overrides Function opName() As String
			Return "shannonentropy"
		End Function


		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			'dL/dx = dL/dOut * dOut/dIn
			'out = -sum(x*log2(x))
			' let z = x * log2(x)
			'Then we can do sumBp(z, -dL/dOut)
			'Note d/dx(x*log2(x)) = (log(x)+1)/log(2)

			Dim log2x As SDVariable = sameDiff.math_Conflict.log(arg(),2)
			Dim logx As SDVariable = sameDiff.math_Conflict.log(arg())
			Dim xLog2X As SDVariable = arg().mul(log2x)
			Dim sumBp As SDVariable = (New SumBp(sameDiff, xLog2X, f1(0).neg(), False, dimensions)).outputVariable()
			Return Collections.singletonList(sumBp.mul(logx.add(1.0)).div(Math.Log(2.0)))
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "entropy_shannon"
		End Function
	End Class

End Namespace