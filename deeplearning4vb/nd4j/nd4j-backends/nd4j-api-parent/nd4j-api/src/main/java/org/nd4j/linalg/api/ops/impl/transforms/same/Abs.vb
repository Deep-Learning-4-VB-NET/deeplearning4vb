﻿Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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
'ORIGINAL LINE: @NoArgsConstructor public class Abs extends org.nd4j.linalg.api.ops.BaseTransformSameOp
	Public Class Abs
		Inherits BaseTransformSameOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			Me.New(sameDiff, i_v, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, i_v, inPlace)
		End Sub


		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			MyBase.New(x, z)
		End Sub

		Public Sub New(ByVal x As INDArray)
			MyBase.New(x)
		End Sub


		Public Overrides Function opNum() As Integer
			Return 0
		End Function

		Public Overrides Function opName() As String
			Return "abs"
		End Function

		Public Overrides Function onnxName() As String
			Return "Abs"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Abs"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim ret As SDVariable = sameDiff.math_Conflict.sign(arg()).mul(i_v(0))
			Return New List(Of SDVariable) From {ret}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim n As Integer = args().Length
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = n, "Expected %s input data types for %s, got %s", n, Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace