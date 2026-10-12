Imports System.Collections.Generic
Imports Getter = lombok.Getter
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.gradient

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class PReluBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class PReluBp
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected int[] sharedAxes;
		Protected Friend sharedAxes() As Integer

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal alpha As SDVariable, ByVal gradient As SDVariable, ParamArray ByVal sharedAxes() As Integer)
			MyBase.New(sd, New SDVariable(){input, alpha, gradient})
			Me.sharedAxes = sharedAxes
			addIArgument(sharedAxes)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public PReluBp(@NonNull INDArray input, @NonNull INDArray alpha, @NonNull INDArray gradient, org.nd4j.linalg.api.ndarray.INDArray dLdI, org.nd4j.linalg.api.ndarray.INDArray dLdA, int... sharedAxes)
		Public Sub New(ByVal input As INDArray, ByVal alpha As INDArray, ByVal gradient As INDArray, ByVal dLdI As INDArray, ByVal dLdA As INDArray, ParamArray ByVal sharedAxes() As Integer)
			MyBase.New(New INDArray(){input, alpha, gradient}, wrapFilterNull(dLdI, dLdA))
			Me.sharedAxes = sharedAxes
			addIArgument(sharedAxes)
		End Sub

		Public Overrides Function opName() As String
			Return "prelu_bp"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 3, "Expected exactly 3 input datatypes, got %s", dataTypes)
			Preconditions.checkArgument(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType() AndAlso dataTypes(2).isFPType(), "Input datatypes must be floating point, got %s", dataTypes)

			Return New List(Of DataType) From {dataTypes(0), dataTypes(1)}
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function
	End Class

End Namespace