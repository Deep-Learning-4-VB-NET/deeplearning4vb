Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports SegmentSumBp = org.nd4j.linalg.api.ops.impl.transforms.segment.bp.SegmentSumBp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom.segment


	Public Class SegmentSum
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal data As SDVariable, ByVal segmentIds As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable() {data, segmentIds}, False)
		End Sub

		Public Sub New()
		End Sub

		Public Sub New(ByVal data As INDArray, ByVal segmentIds As INDArray)
			MyBase.New(New INDArray(){data, segmentIds}, Nothing)
		End Sub

		Public Overrides Function opName() As String
			Return "segment_sum"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "SegmentSum"
		End Function

		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New SegmentSumBp(sameDiff, arg(0), arg(1), gradients(0))).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Preconditions.checkState(inputDataTypes(1).isIntType(), "Datatype for input 1 (Segment IDs) must be an integer type, got %s", inputDataTypes(1))
			Return Collections.singletonList(inputDataTypes(0))
		End Function

	End Class

End Namespace