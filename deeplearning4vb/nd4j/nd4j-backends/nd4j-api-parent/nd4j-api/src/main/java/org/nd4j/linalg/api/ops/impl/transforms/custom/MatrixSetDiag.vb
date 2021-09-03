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


	Public Class MatrixSetDiag
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal diag As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable(){[in], diag}, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal diag As SDVariable)
			Me.New(sameDiff, [in], diag, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MatrixSetDiag(@NonNull INDArray in, @NonNull INDArray diag)
		Public Sub New(ByVal [in] As INDArray, ByVal diag As INDArray)
			MyBase.New(New INDArray(){[in], diag}, Nothing)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"MatrixSetDiag", "BatchMatrixSetDiag"}
		End Function

		Public Overrides Function opName() As String
			Return "matrix_set_diag"
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim grad As SDVariable = i_v(0)
			Dim in1Grad As SDVariable = sameDiff.math_Conflict.setDiag(grad, sameDiff.zerosLike(arg(1)))
			Dim in2Grad As SDVariable = sameDiff.math_Conflict.diagPart(grad)
			Return New List(Of SDVariable) From {in1Grad, in2Grad}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes(0) = dataTypes(1), "Input datatypes must be same type, got %s", dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace