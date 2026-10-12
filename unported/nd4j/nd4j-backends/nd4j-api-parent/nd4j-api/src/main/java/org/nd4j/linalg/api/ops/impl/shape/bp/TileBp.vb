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

Namespace org.nd4j.linalg.api.ops.impl.shape.bp


	Public Class TileBp
		Inherits DynamicCustomOp

		Private repeat() As Integer

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal grad As SDVariable, ByVal repeat() As Integer)
			MyBase.New(Nothing,sameDiff, New SDVariable(){[in], grad}, False)
			Me.repeat = repeat
			addArguments()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal repeat As SDVariable, ByVal grad As SDVariable)
			MyBase.New(Nothing,sameDiff, New SDVariable(){[in], repeat, grad}, False)
			Me.repeat = Nothing
		End Sub

		Public Sub New()
		End Sub

		Private Sub addArguments()
			addIArgument(repeat)
		End Sub


		Public Overrides Function opName() As String
			Return "tile_bp"
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Backprop of gradient op not supported")
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes.Count = 2 OrElse (repeat Is Nothing AndAlso dataTypes.Count = 3), "Expected list with exactly 2 datatypes for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as (original) input type
			Return Collections.singletonList(arg().dataType())
		End Function
	End Class

End Namespace