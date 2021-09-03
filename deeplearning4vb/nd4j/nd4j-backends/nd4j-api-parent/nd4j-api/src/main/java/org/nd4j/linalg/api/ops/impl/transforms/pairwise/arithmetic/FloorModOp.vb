Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
Imports FloorModBpOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp.FloorModBpOp
Imports Shape = org.nd4j.linalg.api.shape.Shape

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

Namespace org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic


	Public Class FloorModOp
		Inherits BaseDynamicTransformOp

		Public Sub New()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal x As SDVariable, ByVal y As SDVariable)
			MyBase.New(sameDiff, New SDVariable(){x, y}, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FloorModOp(@NonNull INDArray x, @NonNull INDArray y)
		Public Sub New(ByVal x As INDArray, ByVal y As INDArray)
			Me.New(New INDArray(){x, y}, Nothing)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(inputs, outputs)
		End Sub

		Public Overrides Function opName() As String
			Return "floormod"
		End Function

		Public Overrides Function onnxName() As String
			Return "FloorMod"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "FloorMod"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New FloorModBpOp(sameDiff, larg(), rarg(), f1(0))).outputs()
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got input %s", Me.GetType(), dataTypes)

			Dim z As DataType = Shape.pickPairwiseDataType(dataTypes(0), dataTypes(1))
			Return Collections.singletonList(z)
		End Function
	End Class

End Namespace