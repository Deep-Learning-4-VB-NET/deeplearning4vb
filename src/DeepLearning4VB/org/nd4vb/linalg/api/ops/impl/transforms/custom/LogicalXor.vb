Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
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

    ''' <summary>The custom op <c>boolean_xor</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.custom.LogicalXor</c>).</summary>
    Public Class LogicalXor
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sd As SameDiff, ByVal in1 As SDVariable, ByVal in2 As SDVariable)
			MyBase.New(Nothing, sd, New SDVariable(){in1, in2})
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "boolean_xor"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {sameDiff.zerosLike(larg()), sameDiff.zerosLike(rarg())}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes(0) = DataType.BOOL, "Datatype for input 0 must be BOOL: got %s", dataTypes(0))
            Preconditions.checkState(dataTypes(1) = DataType.BOOL, "Datatype for input 1 must be BOOL: got %s", dataTypes(1))
            Return New List(Of DataType) From {DataType.BOOL}
        End Function

    End Class

End Namespace
