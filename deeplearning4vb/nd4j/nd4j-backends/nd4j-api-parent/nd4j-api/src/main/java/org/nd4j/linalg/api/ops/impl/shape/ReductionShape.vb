Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op

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

Namespace org.nd4j.linalg.api.ops.impl.shape


	Public Class ReductionShape
		Inherits DynamicCustomOp

		Private keepDims As Boolean

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ReductionShape(@NonNull SameDiff sameDiff, @NonNull SDVariable shape, @NonNull SDVariable axis, boolean keepDims)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal shape As SDVariable, ByVal axis As SDVariable, ByVal keepDims As Boolean)
			MyBase.New(sameDiff, New SDVariable(){shape, axis})
			Me.keepDims = keepDims
			addBArgument(keepDims)
		End Sub


		Public Overrides Function opName() As String
			Return "evaluate_reduction_shape"
		End Function

		Public Overrides Function opType() As Op.Type
			Return Op.Type.CUSTOM
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {sameDiff.zerosLike(arg(0)), sameDiff.zerosLike(arg(1))}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes.Count = 2, "Expected list with exactly 2 datatypes for %s, got %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes(0).isIntType(), "Input 0 (shape) must be integer datatype, is %s", dataTypes(0))
			Preconditions.checkState(dataTypes(0).isIntType(), "Input 1 (axis) must be an integer datatype, is %s", dataTypes(1))
			Return Collections.singletonList(dataTypes(0))
		End Function

	End Class

End Namespace