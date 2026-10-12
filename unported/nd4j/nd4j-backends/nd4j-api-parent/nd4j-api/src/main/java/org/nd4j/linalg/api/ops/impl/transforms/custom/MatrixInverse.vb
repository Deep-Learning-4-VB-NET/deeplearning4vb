Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom


	Public Class MatrixInverse
		Inherits DynamicCustomOp

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatrixInverse(@NonNull INDArray input)
		Public Sub New(ByVal input As INDArray)
			MyBase.New(New INDArray(){input}, Nothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable(){[in]}, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable)
			Me.New(sameDiff, [in], False)
		End Sub

		Public Overrides Function opName() As String
			Return "matrix_inverse"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"MatrixInverse", "BatchMatrixInverse"}
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			'Derivative of matrix determinant
			'From: Matrix Cookbook - Petersen & Pedersen
			'if z = inverse(X)
			'dz/dx = - z * dX/dx * z
			'note that dX/dx is just identity matrix
			'TODO non-matrix case
			Dim dOutdIn As SDVariable = outputVariable().mmul(outputVariable()).neg()
			Return Collections.singletonList(i_v(0).mul(dOutdIn))
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatype for %s, got %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes(0).isFPType(), "Input datatype must be a floating point type, got %s", dataTypes(0))
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace