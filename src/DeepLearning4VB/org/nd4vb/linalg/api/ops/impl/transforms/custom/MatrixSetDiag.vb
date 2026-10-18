Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.custom

    ''' <summary>The custom op <c>matrix_set_diag</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.MatrixSetDiag</c>).</summary>
    Public Class MatrixSetDiag
        Inherits DynamicCustomOp

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal diag As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(Nothing, sameDiff, New SDVariable(){[in], diag}, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal diag As SDVariable)
			Me.New(sameDiff, [in], diag, False)
		End Sub
#End If

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

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim grad As SDVariable = i_v(0)
			Dim in1Grad As SDVariable = sameDiff.math_Conflict.setDiag(grad, sameDiff.zerosLike(arg(1)))
			Dim in2Grad As SDVariable = sameDiff.math_Conflict.diagPart(grad)
			Return New List(Of SDVariable) From {in1Grad, in2Grad}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes(0) = dataTypes(1), "Input datatypes must be same type, got %s", dataTypes)
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
