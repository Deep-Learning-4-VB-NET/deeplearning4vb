Imports System.Collections.Generic
Imports NoArgsConstructor = lombok.NoArgsConstructor
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

Namespace org.nd4j.linalg.api.ops.impl.transforms.custom



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class XwPlusB extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class XwPlusB
		Inherits DynamicCustomOp


		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal weights As SDVariable, ByVal bias As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable() {input, weights, bias}, False)
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal weights As INDArray, ByVal bias As INDArray)
			MyBase.New(New INDArray() {input, weights, bias}, Nothing)
		End Sub

		Public Sub New(ByVal inputs() As INDArray, ByVal output As INDArray)
			MyBase.New(inputs, wrapOrNull(output))
		End Sub

		Public Overrides Function opName() As String
			Return "xw_plus_b"
		End Function


		Public Overrides Function tensorflowName() As String
			Throw New NoOpNameFoundException("No tensorflow name found for shape " & opName())
		End Function

		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx name found for shape " & opName())
		End Function

		Public Overrides Function doDiff(ByVal gradient As IList(Of SDVariable)) As IList(Of SDVariable)
			Dim [in] As SDVariable = arg(0)
			Dim w As SDVariable = arg(1)
			Dim dLdOut As SDVariable = gradient(0)

			Dim dLdb As SDVariable = dLdOut.sum(0)
			Dim dLdIn As SDVariable = sameDiff.mmul(dLdOut, w, False, True, False)
			Dim dLdW As SDVariable = sameDiff.mmul([in], dLdOut, True, False, False)

			Return New List(Of SDVariable) From {dLdIn, dLdW, dLdb}
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing AndAlso dataTypes.Count = 3, "Expected exactly 3 input datatypes, got %s", dataTypes)
			Dim first As DataType = dataTypes(0)
			For i As Integer = 0 To 2
				Preconditions.checkState(dataTypes(i).isFPType(), "Input %s datatype must be a floating point type, got datypes %s", dataTypes)
				If i > 0 Then
					Preconditions.checkState(first = dataTypes(i), "All datatypes must be same type, got input datatypes %s", dataTypes)
				End If
			Next i
			Return Collections.singletonList(first)
		End Function

	End Class

End Namespace