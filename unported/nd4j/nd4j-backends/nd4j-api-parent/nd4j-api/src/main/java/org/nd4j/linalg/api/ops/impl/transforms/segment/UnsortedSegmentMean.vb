Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports UnsortedSegmentMeanBp = org.nd4j.linalg.api.ops.impl.transforms.segment.bp.UnsortedSegmentMeanBp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.segment


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class UnsortedSegmentMean extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class UnsortedSegmentMean
		Inherits DynamicCustomOp

		Private numSegments As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal data As SDVariable, ByVal segmentIds As SDVariable, ByVal numSegments As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable() {data, segmentIds}, False)
			Me.numSegments = numSegments
			addIArgument(numSegments)
		End Sub

		Public Sub New(ByVal data As INDArray, ByVal segmentIds As INDArray, ByVal numSegments As Integer)
			MyBase.New(New INDArray(){data, segmentIds}, Nothing)
			Me.numSegments = numSegments
			addIArgument(numSegments)
		End Sub

		Public Overrides Function opName() As String
			Return "unsorted_segment_mean"
		End Function


		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New UnsortedSegmentMeanBp(sameDiff, arg(0), arg(1), gradients(0), numSegments)).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			If dArguments.Count > 0 Then
				Return Collections.singletonList(dArguments(0))
			End If
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso (inputDataTypes.Count = 2 OrElse inputDataTypes.Count = 3), "Expected exactly 2 input data types for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function

	End Class

End Namespace