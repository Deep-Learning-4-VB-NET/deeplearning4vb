﻿Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseReduceLongOp = org.nd4j.linalg.api.ops.BaseReduceLongOp

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


	Public Class CountNonZero
		Inherits BaseReduceLongOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, i_v, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal dimensions As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, i_v, dimensions, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, input, dimensions, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, i_v, i_v2, dimensions, keepDims)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			MyBase.New(sameDiff, i_v)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ParamArray ByVal dimensions() As Integer)
			MyBase.New(sameDiff, input, dimensions)
		End Sub


		Public Sub New(ByVal x As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, z, dimensions)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ParamArray ByVal dimensions() As Integer)
			MyBase.New(x, y, z, dimensions)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff)
			MyBase.New(sameDiff)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal i_v2 As SDVariable, ByVal dimensions As SDVariable)
			MyBase.New(sameDiff, i_v, i_v2, dimensions)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(x, y, z, keepDims, dimensions)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
		   MyBase.New([in],keepDims,dimensions)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal [in] As SDVariable, ByVal keepDims As Boolean, ByVal dimensions() As Integer)
			MyBase.New(sd,[in],dimensions,keepDims)
		End Sub

		Public Sub New(ByVal [in] As INDArray, ByVal dimensions() As Integer, ByVal keepDims As Boolean)
			MyBase.New([in],keepDims,dimensions)
		End Sub


		Public Overrides Function opNum() As Integer
			Return 0
		End Function

		Public Overrides Function opName() As String
			Return "countNonZero"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function

	End Class

End Namespace