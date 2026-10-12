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


	Public Class SoftmaxBp
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal sd As SameDiff, ByVal input As SDVariable, ByVal grad As SDVariable, ByVal dimension As Integer?)
			MyBase.New(Nothing, sd, New SDVariable(){input, grad})
			If dimension IsNot Nothing Then
				addIArgument(dimension)
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SoftmaxBp(@NonNull INDArray input, @NonNull INDArray grad, System.Nullable<Integer> dimension)
		Public Sub New(ByVal input As INDArray, ByVal grad As INDArray, ByVal dimension As Integer?)
			Me.New(input, grad, Nothing, dimension)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public SoftmaxBp(@NonNull INDArray input, @NonNull INDArray grad, org.nd4j.linalg.api.ndarray.INDArray output, System.Nullable<Integer> dimension)
		Public Sub New(ByVal input As INDArray, ByVal grad As INDArray, ByVal output As INDArray, ByVal dimension As Integer?)
			MyBase.New(New INDArray(){input, grad}, wrapOrNull(output))
			If dimension IsNot Nothing Then
				addIArgument(dimension)
			End If
		End Sub

		Public Overrides Function opName() As String
			Return "softmax_bp"
		End Function

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Differentiating op softmax_bp not supported")
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 inputs datatype for %s, got %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes(0).isFPType(), "Input 0 must be a floating point type, got %s", dataTypes(0))
			Preconditions.checkState(dataTypes(1).isFPType(), "Input 1 must be a floating point type, got %s", dataTypes(1))
			Preconditions.checkState(dataTypes(0) = dataTypes(1), "Both input must be same type: got %s", dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace