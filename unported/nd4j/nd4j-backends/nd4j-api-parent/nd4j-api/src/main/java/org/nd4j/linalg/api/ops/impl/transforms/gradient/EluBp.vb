Imports System.Collections.Generic
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

	Public Class EluBp
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal gradient As SDVariable, ByVal alpha As Double)
			MyBase.New(sd, New SDVariable(){input, gradient})
			addTArgument(alpha)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EluBp(@NonNull INDArray input, @NonNull INDArray gradient, org.nd4j.linalg.api.ndarray.INDArray output)
		Public Sub New(ByVal input As INDArray, ByVal gradient As INDArray, ByVal output As INDArray)
			Me.New(input, gradient, output, 1.0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public EluBp(@NonNull INDArray input, @NonNull INDArray gradient, org.nd4j.linalg.api.ndarray.INDArray output, double alpha)
		Public Sub New(ByVal input As INDArray, ByVal gradient As INDArray, ByVal output As INDArray, ByVal alpha As Double)
			MyBase.New(New INDArray(){input, gradient}, wrapOrNull(output))
			addTArgument(alpha)
		End Sub

		Public Overrides Function opName() As String
			Return "elu_bp"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkArgument(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes, got %s", dataTypes)
			Preconditions.checkArgument(dataTypes(0).isFPType() AndAlso dataTypes(1).isFPType(), "Input datatypes must be floating point, got %s", dataTypes)

			Return Collections.singletonList(dataTypes(0))
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function
	End Class

End Namespace