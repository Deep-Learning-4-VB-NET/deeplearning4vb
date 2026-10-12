Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports RectifiedLinear = org.nd4j.linalg.api.ops.impl.scalar.RectifiedLinear
Imports ThresholdReluBp = org.nd4j.linalg.api.ops.impl.transforms.gradient.ThresholdReluBp

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

	Public Class ThresholdRelu
		Inherits DynamicCustomOp

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private double cutoff = 0.0;
		Private cutoff As Double = 0.0

		Public Sub New()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal inPlace As Boolean, ByVal cutoff As Double)
			MyBase.New(sd, New SDVariable(){input}, inPlace)
			Me.cutoff = cutoff
			addTArgument(cutoff)
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal cutoff As Double)
			MyBase.New(sd, New SDVariable(){input})
			Me.cutoff = cutoff
			addTArgument(cutoff)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ThresholdRelu(@NonNull INDArray input, org.nd4j.linalg.api.ndarray.INDArray output, double cutoff)
		Public Sub New(ByVal input As INDArray, ByVal output As INDArray, ByVal cutoff As Double)
			MyBase.New(New INDArray(){input}, wrapOrNull(output))
			Me.cutoff = cutoff
			addTArgument(cutoff)
		End Sub

		Public Overrides Function opName() As String
			Return "thresholdedrelu"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 1, "Expected exactly 1 input datatypes, got %s", dataTypes)
			Preconditions.checkArgument(dataTypes(0).isFPType(), "Input datatype must be floating point, got %s", dataTypes)

			Return Collections.singletonList(dataTypes(0))
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New ThresholdReluBp(sameDiff, arg(), f1(0), cutoff)).outputs()
		End Function
	End Class

End Namespace