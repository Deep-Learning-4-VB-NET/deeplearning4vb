Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
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

Namespace org.nd4j.linalg.api.ops.impl.loss


	''' <summary>
	''' L2 loss op wrapper
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class L2Loss extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class L2Loss
		Inherits DynamicCustomOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal var As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){var})
		End Sub

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

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'L2 loss: L = 1/2 * sum(x_i^2)
			'dL/dxi = xi
			Return Collections.singletonList(sameDiff.identity(arg()))
		End Function
	End Class

End Namespace