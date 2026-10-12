Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports PReluBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.PReluBp

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

Namespace org.nd4j.linalg.api.ops.impl.scalar

	''' <summary>
	''' Parameterized ReLU op
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class PRelu extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class PRelu
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected int[] sharedAxes;
		Protected Friend sharedAxes() As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public PRelu(@NonNull SameDiff sameDiff, @NonNull SDVariable x, @NonNull SDVariable alpha, @NonNull int... sharedAxes)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal alpha As SDVariable, ParamArray ByVal sharedAxes() As Integer)
			MyBase.New(sameDiff, New SDVariable(){x, alpha})
			Me.sharedAxes = sharedAxes
			addIArgument(sharedAxes)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public PRelu(@NonNull INDArray x, @NonNull INDArray alpha, @NonNull int... sharedAxes)
		Public Sub New(ByVal x As INDArray, ByVal alpha As INDArray, ParamArray ByVal sharedAxes() As Integer)
			Me.New(x, Nothing, alpha, sharedAxes)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public PRelu(@NonNull INDArray x, org.nd4j.linalg.api.ndarray.INDArray z, @NonNull INDArray alpha, @NonNull int... sharedAxes)
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray, ByVal alpha As INDArray, ParamArray ByVal sharedAxes() As Integer)
			MyBase.New(New INDArray(){x, alpha}, New INDArray(){z})
			Me.sharedAxes = sharedAxes
			addIArgument(sharedAxes)
		End Sub

		Public Overrides Function opName() As String
			Return "prelu"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes, got %s", dataTypes)
			Preconditions.checkArgument(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType(), "Input datatypes must be floating point, got %s", dataTypes)

			Return Collections.singletonList(dataTypes(0))
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New PReluBp(sameDiff, arg(0), arg(1), i_v(0), sharedAxes)).outputs()
		End Function
	End Class

End Namespace