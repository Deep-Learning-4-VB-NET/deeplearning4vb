Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
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

Namespace org.nd4vb.linalg.api.ops.impl.loss

    ''' <summary>
    ''' L2 loss op wrapper
    ''' </summary>
    Public Class L2Loss
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal var As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){var})
		End Sub
#End If

        Public Sub New(ByVal var As INDArray)
            MyBase.New(New INDArray(){var}, Nothing)
        End Sub

        Public Overrides Function opName() As String
            Return "l2_loss"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "L2Loss"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 1, "Expected 1 input type for %s, got %s", Me.GetType(), inputDataTypes)
            Preconditions.checkState(inputDataTypes(0).isFPType(), "Input datatype must be floating point for %s, got %s", Me.GetType(), inputDataTypes)
            Return inputDataTypes
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'L2 loss: L = 1/2 * sum(x_i^2)
			'dL/dxi = xi
			Return Collections.singletonList(sameDiff.identity(arg()))
		End Function
#End If

    End Class

End Namespace
