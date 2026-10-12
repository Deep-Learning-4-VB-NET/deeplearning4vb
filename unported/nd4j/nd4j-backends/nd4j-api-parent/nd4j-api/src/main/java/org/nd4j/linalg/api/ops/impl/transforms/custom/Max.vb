Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp

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


	Public Class Max
		Inherits BaseDynamicTransformOp

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Max(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable first, @NonNull SDVariable second)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal first As SDVariable, ByVal second As SDVariable)
			Me.New(sameDiff, New SDVariable(){first, second}, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, args, inPlace)
		End Sub

		Public Sub New(ByVal first As INDArray, ByVal second As INDArray, ByVal [out] As INDArray)
			MyBase.New(New INDArray(){first, second},If([out] Is Nothing, Nothing, New INDArray()){[out]})
		End Sub

		Public Sub New(ByVal first As INDArray, ByVal second As INDArray)
			Me.New(first, second, Nothing)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(inputs, outputs)
		End Sub

	  Public Overrides Function opName() As String
			Return "maximum"
	  End Function

		Public Overrides Function onnxName() As String
		   Return "Max"
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Maximum"
		End Function

		Public Overrides Function doDiff(ByVal f1 As IList(Of SDVariable)) As IList(Of SDVariable)
			Return New List(Of SDVariable) From {(New MaximumBp(sameDiff, arg(0), arg(1), f1(0))).outputVariables()}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 2, "Expected exactly 2 input datatypes for %s, got %s", Me.GetType(), dataTypes)
			Preconditions.checkState(dataTypes(0) = dataTypes(1), "Input datatypes must be the same, got %s", dataTypes)
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace