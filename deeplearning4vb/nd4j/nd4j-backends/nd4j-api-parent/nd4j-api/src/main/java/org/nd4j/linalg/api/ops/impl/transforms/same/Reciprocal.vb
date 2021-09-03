﻿Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseTransformSameOp = org.nd4j.linalg.api.ops.BaseTransformSameOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.same


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class Reciprocal extends org.nd4j.linalg.api.ops.BaseTransformSameOp
	Public Class Reciprocal
		Inherits BaseTransformSameOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable)
			MyBase.New(sameDiff, [in], False)
		End Sub

		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			MyBase.New(x, z)
		End Sub

		Public Sub New(ByVal x As INDArray)
			MyBase.New(x)
		End Sub

		Public Overrides Function opNum() As Integer
			Return 11
		End Function

		Public Overrides Function opName() As String
			Return "Reciprocal"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No  onnx opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Reciprocal"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"Reciprocal", "Inv"}
		End Function

		Public Overrides Function doDiff(ByVal i_v1 As IList(Of SDVariable)) As IList(Of SDVariable)
			' -1/(x^2)
			Dim g As SDVariable = sameDiff.math_Conflict.pow(arg(), 2).rdiv(-1).mul(i_v1(0))
			Return Collections.singletonList(g)
		End Function
	End Class

End Namespace