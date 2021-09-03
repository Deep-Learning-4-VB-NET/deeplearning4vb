﻿Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseScalarOp = org.nd4j.linalg.api.ops.BaseScalarOp

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

Namespace org.nd4j.linalg.api.ops.impl.scalar


	Public Class ScalarReverseSubtraction
		Inherits BaseScalarOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal num As Number)
			MyBase.New(x, num)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal num As Number)
			MyBase.New(x, z, num)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal y As INDArray, ByVal z As INDArray, ByVal num As Number)
			MyBase.New(x, y, z, num)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number)
			MyBase.New(sameDiff, i_v, scalar)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal scalar As Number, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, scalar, inPlace)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 5
		End Function

		Public Overrides Function opName() As String
			Return "rsub_scalar"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function



		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim g As SDVariable = sameDiff.math_Conflict.neg(i_v1(0))
			Return Collections.singletonList(g)
		End Function

	End Class

End Namespace