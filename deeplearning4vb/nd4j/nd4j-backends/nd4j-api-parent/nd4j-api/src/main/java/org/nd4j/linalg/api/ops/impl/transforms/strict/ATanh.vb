﻿Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseTransformStrictOp = org.nd4j.linalg.api.ops.BaseTransformStrictOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.strict


	Public Class ATanh
		Inherits BaseTransformStrictOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			Me.New(sameDiff, i_v, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, inPlace)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			MyBase.New(x, z)
		End Sub

		Public Sub New(ByVal x As INDArray)
			MyBase.New(x)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 52
		End Function

		Public Overrides Function opName() As String
			Return "atanh"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Atanh"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'd(atanh(x))/dx = 1 / (1-x^2)

			Dim oneMinusX2 As SDVariable = sameDiff.math().square(arg()).rsub(1.0)
			Dim ret As SDVariable = oneMinusX2.rdiv(1.0).mul(i_v(0))
			Return New List(Of SDVariable) From {ret}
		End Function

	End Class

End Namespace