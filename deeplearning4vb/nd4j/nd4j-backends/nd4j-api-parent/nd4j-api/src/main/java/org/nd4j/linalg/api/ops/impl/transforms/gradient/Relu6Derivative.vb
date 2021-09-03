Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
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


	Public Class Relu6Derivative
		Inherits DynamicCustomOp

		Private Const DEFAULT_CUTOFF As Double = 0.0

		Private cutoff As Double = DEFAULT_CUTOFF

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v1 As SDVariable, ByVal i_v2 As SDVariable, ByVal cutoff As Double)
			MyBase.New("relu6_bp", sameDiff, New SDVariable(){i_v1, i_v2})
			Me.cutoff = cutoff
			Me.extraArgs = New Object(){cutoff}
		End Sub

		Public Sub New()
			Me.extraArgs = New Object(){cutoff}
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Relu6Derivative(@NonNull INDArray input, @NonNull INDArray gradient, org.nd4j.linalg.api.ndarray.INDArray output)
		Public Sub New(ByVal input As INDArray, ByVal gradient As INDArray, ByVal output As INDArray)
			Me.New(input, gradient, output, DEFAULT_CUTOFF)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Relu6Derivative(@NonNull INDArray input, @NonNull INDArray gradient, org.nd4j.linalg.api.ndarray.INDArray output, double cutoff)
		Public Sub New(ByVal input As INDArray, ByVal gradient As INDArray, ByVal output As INDArray, ByVal cutoff As Double)
			MyBase.New(New INDArray(){input, gradient}, wrapOrNull(output))
			Me.cutoff = cutoff
			Me.extraArgs = New Object(){cutoff}
		End Sub

		Public Overrides Function opNum() As Integer
			Return 0
		End Function

		Public Overrides Function opName() As String
			Return "relu6_bp"
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx op opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow op opName found for " & opName())
		End Function


		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso inputDataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(inputDataTypes(0))
		End Function
	End Class

End Namespace