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
Namespace org.nd4j.linalg.api.ops.custom


	Public Class RandomCrop
		Inherits DynamicCustomOp

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomCrop(@NonNull INDArray input, @NonNull INDArray shape)
		Public Sub New(ByVal input As INDArray, ByVal shape As INDArray)
			Preconditions.checkArgument(shape.isVector(),"RandomCrop:Shape tensor should be a vector")
			Preconditions.checkArgument(input.rank() = shape.length(), "RandomCrop:The length of the shape vector is not match input rank")
			addInputArgument(input, shape)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public RandomCrop(@NonNull SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable shape)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal shape As SDVariable)
				MyBase.New("", sameDiff, New SDVariable(){input, shape})
		End Sub

		Public Overrides Function opName() As String
			Return "random_crop"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "RandomCrop"
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes IsNot Nothing, "Expected 4 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)
			Return Collections.singletonList(DataType.FLOAT) 'TF import: always returns float32...
		End Function
	End Class

End Namespace