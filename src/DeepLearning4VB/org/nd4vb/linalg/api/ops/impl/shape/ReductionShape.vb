Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports Op = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op
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

Namespace org.nd4vb.linalg.api.ops.impl.shape

    ''' <summary>The custom op <c>evaluate_reduction_shape</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.ReductionShape</c>).</summary>
    Public Class ReductionShape
        Inherits DynamicCustomOp

        Private keepDimsField As Boolean

        Public Sub New()
        End Sub

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReductionShape(@NonNull SameDiff sameDiff, @NonNull SDVariable shape, @NonNull SDVariable axis, boolean keepDims)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal axis As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, New SDVariable(){shape, axis})
			Me.keepDims = keepDims
			addBArgument(keepDims)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "evaluate_reduction_shape"
        End Function

        Public Overrides Function opType() As Op.Type
            Return Op.Type.CUSTOM
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {sameDiff.zerosLike(arg(0)), sameDiff.zerosLike(arg(1))}
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(dataTypes.Count = 2, "Expected list with exactly 2 datatypes for %s, got %s", Me.GetType(), dataTypes)
            Preconditions.checkState(dataTypes(0).isIntType(), "Input 0 (shape) must be integer datatype, is %s", dataTypes(0))
            Preconditions.checkState(dataTypes(0).isIntType(), "Input 1 (axis) must be an integer datatype, is %s", dataTypes(1))
            Return New List(Of DataType) From {dataTypes(0)}
        End Function

    End Class

End Namespace
