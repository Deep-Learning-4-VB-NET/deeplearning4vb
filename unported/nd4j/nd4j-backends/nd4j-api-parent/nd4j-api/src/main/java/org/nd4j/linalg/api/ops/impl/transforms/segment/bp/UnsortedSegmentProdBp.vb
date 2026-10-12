Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.segment.bp


	Public Class UnsortedSegmentProdBp
		Inherits DynamicCustomOp

		Private numSegments As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal data As SDVariable, ByVal segmentIds As SDVariable, ByVal gradient As SDVariable, ByVal numSegments As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable() {data, segmentIds, gradient}, False)
			Me.numSegments = numSegments
			addIArgument(numSegments)
		End Sub

		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "unsorted_segment_prod_bp"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 3, "Expected exactly 3 input data types for %s, got %s", Me.GetType(), inputDataTypes)
			Return New List(Of DataType) From {inputDataTypes(0), inputDataTypes(1)}
		End Function

	End Class

End Namespace