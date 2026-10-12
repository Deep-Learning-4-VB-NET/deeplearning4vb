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
Namespace org.nd4j.linalg.api.ops.impl.layers.recurrent


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class GRU extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class GRU
		Inherits DynamicCustomOp


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public GRU(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable hI, @NonNull SDVariable Wx, @NonNull SDVariable Wh, @NonNull SDVariable biases)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal hI As SDVariable, ByVal Wx As SDVariable, ByVal Wh As SDVariable, ByVal biases As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){x, hI, Wx, Wh, biases})

		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public GRU(@NonNull INDArray x, @NonNull INDArray hI, @NonNull INDArray Wx, @NonNull INDArray Wh, @NonNull INDArray biases)
		Public Sub New(ByVal x As INDArray, ByVal hI As INDArray, ByVal Wx As INDArray, ByVal Wh As INDArray, ByVal biases As INDArray)
			MyBase.New(New INDArray(){x, hI, Wx, Wh, biases}, Nothing)

		End Sub

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 5, "Expected 5 inputs to GRU: initial cell output, input-to-hidden weights, hidden-to-hidden weights and biases got %s", inputDataTypes)
			Dim dt As DataType = inputDataTypes(1)
			For i As Integer = 0 To inputDataTypes.Count - 1
				Preconditions.checkState(inputDataTypes(i).isFPType(), "All input types must be a floating point type, got %s", dt)
			Next i
			Preconditions.checkState(dt.isFPType(), "Input type 1 must be a floating point type, got %s", dt)
			Return Collections.singletonList(dt)
		End Function

		Public Overrides Function doDiff(ByVal grads As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {(New GRUBp(sameDiff, arg(0), arg(1), arg(2), arg(3), arg(4), grads(0))).outputVariables()}
		End Function


		Public Overrides Function opName() As String
			Return "gru"
		End Function
	End Class

End Namespace