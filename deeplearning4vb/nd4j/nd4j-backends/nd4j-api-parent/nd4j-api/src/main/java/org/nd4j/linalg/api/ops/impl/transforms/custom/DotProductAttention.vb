Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class DotProductAttention extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class DotProductAttention
		Inherits DynamicCustomOp

		Private withWeights As Boolean
		Private scaled As Boolean


		Public Sub New(ByVal sameDiff As SameDiff, ByVal queries As SDVariable, ByVal keys As SDVariable, ByVal values As SDVariable, ByVal mask As SDVariable, ByVal scaled As Boolean, ByVal withWeights As Boolean)
			MyBase.New(Nothing, sameDiff,If(mask Is Nothing, New SDVariable() {queries, keys, values}, New SDVariable()){queries, keys, values, mask}, False)
			Me.scaled = scaled
			Me.withWeights = withWeights
			addIArgument(If(scaled, 1, 0))
			addIArgument(If(withWeights, 1, 0))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DotProductAttention(@NonNull INDArray queries, @NonNull INDArray keys, @NonNull INDArray values, org.nd4j.linalg.api.ndarray.INDArray mask, boolean scaled)
		Public Sub New(ByVal queries As INDArray, ByVal keys As INDArray, ByVal values As INDArray, ByVal mask As INDArray, ByVal scaled As Boolean)
			Me.New(queries, keys, values, mask, scaled, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DotProductAttention(@NonNull INDArray queries, @NonNull INDArray keys, @NonNull INDArray values, org.nd4j.linalg.api.ndarray.INDArray mask, boolean scaled, boolean withWeights)
		Public Sub New(ByVal queries As INDArray, ByVal keys As INDArray, ByVal values As INDArray, ByVal mask As INDArray, ByVal scaled As Boolean, ByVal withWeights As Boolean)
			MyBase.New(wrapFilterNull(queries, keys, values, mask), Nothing)
			Me.scaled = scaled
			Me.withWeights = withWeights
			addIArgument(If(scaled, 1, 0))
			addIArgument(If(withWeights, 1, 0))
		End Sub

		Public Overrides Function opName() As String
			Return "dot_product_attention"
		End Function

		Public Overrides Function doDiff(ByVal gradient As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim mask As SDVariable = If(args().Length = 4, arg(3), Nothing)
			Return New List(Of SDVariable) From {(New DotProductAttentionBp(sameDiff, arg(0), arg(1), arg(2), gradient(0), mask, scaled)).outputVariables()}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso (dataTypes.Count = 3 OrElse dataTypes.Count = 4), "Expected exactly 3 or 4 input datatypes, got %s", dataTypes)
			Dim first As DataType = dataTypes(0)
			For i As Integer = 0 To dataTypes.Count - 1
				Preconditions.checkState(dataTypes(i).isFPType(), "Input %s datatype must be a floating point type, got datypes %s", dataTypes)
				If i > 0 Then
					Preconditions.checkState(first = dataTypes(i), "All datatypes must be same type, got input datatypes %s", dataTypes)
				End If
			Next i
			If withWeights Then
				Return New List(Of DataType) From {first, first}
			Else
				Return Collections.singletonList(first)
			End If
		End Function

		Public Overrides ReadOnly Property NumOutputs As Integer
			Get
				If withWeights Then
					Return 2
				Else
					Return 1
				End If
			End Get
		End Property
	End Class

End Namespace