Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports SliceBp = org.nd4j.linalg.api.ops.impl.shape.bp.SliceBp

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

Namespace org.nd4j.linalg.api.ops.impl.shape


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class Slice extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class Slice
		Inherits DynamicCustomOp

		Private begin() As Integer
		Private size() As Integer

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Slice(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable input, @NonNull int[] begin, @NonNull int[] size)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal begin() As Integer, ByVal size() As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){input})
			Me.begin = begin
			Me.size = size
			addIArgument(begin)
			addIArgument(size)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Slice(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable input, @NonNull SDVariable begin, @NonNull SDVariable end)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal input As SDVariable, ByVal begin As SDVariable, ByVal [end] As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){input, begin, [end]})
		End Sub

		Public Sub New(ByVal input As INDArray, ByVal begin() As Integer, ParamArray ByVal size() As Integer)
			MyBase.New(New INDArray() {input}, Nothing)
			Me.begin = begin
			Me.size = size
			addIArgument(begin)
			addIArgument(size)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Slice(@NonNull INDArray input, @NonNull INDArray begin, @NonNull INDArray end)
		Public Sub New(ByVal input As INDArray, ByVal begin As INDArray, ByVal [end] As INDArray)
			MyBase.New(New INDArray(){input, begin, [end]}, Nothing)
		End Sub

		Public Overrides Function opName() As String
			Return "slice"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No onnx opName found for " & opName())
		End Function

		Public Overrides Function tensorflowName() As String
			Return "Slice"
		End Function

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			If args().Length = 1 Then
				Return (New SliceBp(sameDiff, arg(), grad(0), begin, size)).outputs()
			Else
				'Dynamic begin/size
				Return (New SliceBp(sameDiff, arg(0), grad(0), arg(1), arg(2))).outputs()
			End If
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes IsNot Nothing And (dataTypes.Count = 1 OrElse dataTypes.Count = 3), "Expected list with 1 or 3 datatypes for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as input type. 3 inputs for import case
			Return Collections.singletonList(dataTypes(0))
		End Function
	End Class

End Namespace