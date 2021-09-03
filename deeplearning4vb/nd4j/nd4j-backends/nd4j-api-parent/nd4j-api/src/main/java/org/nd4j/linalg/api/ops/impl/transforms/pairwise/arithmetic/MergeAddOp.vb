Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BaseDynamicTransformOp = org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
Imports MergeAddBp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp.MergeAddBp

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class MergeAddOp extends org.nd4j.linalg.api.ops.impl.transforms.BaseDynamicTransformOp
	Public Class MergeAddOp
		Inherits BaseDynamicTransformOp

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable, ByVal inPlace As Boolean)
			MyBase.New(sameDiff, args, inPlace)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal args() As SDVariable)
			Me.New(sameDiff, args, False)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public MergeAddOp(@NonNull INDArray... inputs)
		Public Sub New(ParamArray ByVal inputs() As INDArray)
			Me.New(inputs, Nothing)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal outputs() As INDArray)
			MyBase.New(inputs, outputs)
		End Sub

		Public Overrides Function opName() As String
			Return "mergeadd"
		End Function

		Public Overrides Function onnxName() As String
			Return "mergeadd"
		End Function

		Public Overrides Function tensorflowNames() As String()
			Return New String(){"add_n", "AccumulateNV2"}
		End Function

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			 Return New List(Of SDVariable) From {(New MergeAddBp(sameDiff, args(), i_v(0))).outputVariables()}

		End Function


		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Dim first As DataType = dataTypes(0)
			For i As Integer = 1 To dataTypes.Count - 1
				Preconditions.checkState(first = dataTypes(i), "Expected all input datatypes to be the same: first input is %s, input %s is %s", first, i, dataTypes(i))
			Next i
			Return Collections.singletonList(first)
		End Function

	End Class

End Namespace