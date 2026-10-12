Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseTransformBoolOp = org.nd4j.linalg.api.ops.BaseTransformBoolOp

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.bool


	Public Class BooleanNot
		Inherits BaseTransformBoolOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal i_v As SDVariable)
			MyBase.New(sameDiff, i_v, False)
		End Sub

		Public Sub New()
			'
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BooleanNot(@NonNull INDArray x)
		Public Sub New(ByVal x As INDArray)
			Me.New(x, x)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BooleanNot(@NonNull INDArray x, org.nd4j.linalg.api.ndarray.INDArray z)
		Public Sub New(ByVal x As INDArray, ByVal z As INDArray)
			MyBase.New(x, Nothing, z)
			Preconditions.checkArgument(x.dataType() = DataType.BOOL, "X operand must be BOOL")
			Preconditions.checkArgument(z.dataType() = DataType.BOOL, "Z operand must be BOOL")
		End Sub

		Public Overrides Function opNum() As Integer
			Return 7
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("Onnx name not found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("Tensorflow name not found for " & opName())
		End Function

		Public Overrides Function opName() As String
			Return "bool_not"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return Collections.singletonList(sameDiff.zerosLike(arg()))
		End Function
	End Class

End Namespace