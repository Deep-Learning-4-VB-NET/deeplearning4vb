Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException

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

Namespace org.nd4j.linalg.api.ops.impl.shape.bp


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class StridedSliceBp extends org.nd4j.linalg.api.ops.DynamicCustomOp
	Public Class StridedSliceBp
		Inherits DynamicCustomOp

		Private begin() As Long
		Private [end]() As Long
		Private strides() As Long
		Private beginMask As Integer
		Private endMask As Integer
		Private ellipsisMask As Integer
		Private newAxisMask As Integer
		Private shrinkAxisMask As Integer

		Public Sub New()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StridedSliceBp(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable in, @NonNull SDVariable grad, @NonNull long[] begin, @NonNull long[] end, @NonNull long[] strides, int beginMask, int endMask, int ellipsisMask, int newAxisMask, int shrinkAxisMask)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal grad As SDVariable, ByVal begin() As Long, ByVal [end]() As Long, ByVal strides() As Long, ByVal beginMask As Integer, ByVal endMask As Integer, ByVal ellipsisMask As Integer, ByVal newAxisMask As Integer, ByVal shrinkAxisMask As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){[in], grad})
			Me.begin = begin
			Me.end = [end]
			Me.strides = strides
			Me.beginMask = beginMask
			Me.endMask = endMask
			Me.ellipsisMask = ellipsisMask
			Me.newAxisMask = newAxisMask
			Me.shrinkAxisMask = shrinkAxisMask

			'https://github.com/deeplearning4j/libnd4j/blob/master/include/ops/declarable/generic/parity_ops/strided_slice.cpp#L279
			addArguments()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public StridedSliceBp(org.nd4j.autodiff.samediff.SameDiff sameDiff, @NonNull SDVariable in, @NonNull SDVariable grad, @NonNull SDVariable begin, @NonNull SDVariable end, @NonNull SDVariable strides, int beginMask, int endMask, int ellipsisMask, int newAxisMask, int shrinkAxisMask)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal [in] As SDVariable, ByVal grad As SDVariable, ByVal begin As SDVariable, ByVal [end] As SDVariable, ByVal strides As SDVariable, ByVal beginMask As Integer, ByVal endMask As Integer, ByVal ellipsisMask As Integer, ByVal newAxisMask As Integer, ByVal shrinkAxisMask As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){[in], grad, begin, [end], strides})
			Me.beginMask = beginMask
			Me.endMask = endMask
			Me.ellipsisMask = ellipsisMask
			Me.newAxisMask = newAxisMask
			Me.shrinkAxisMask = shrinkAxisMask
			addArguments()
		End Sub

		Private Sub addArguments()
			addIArgument(beginMask)
			addIArgument(ellipsisMask)
			addIArgument(endMask)
			addIArgument(newAxisMask)
			addIArgument(shrinkAxisMask)
			If begin IsNot Nothing Then 'May be null for SDVariable inputs of these args
				addIArgument(begin)
				addIArgument([end])
				addIArgument(strides)
			End If
		End Sub


		Public Overrides Function opName() As String
			Return "strided_slice_bp"
		End Function


		Public Overrides Sub assertValidForExecution()
			If numInputArguments() <> 2 AndAlso numInputArguments() <> 4 Then
				Throw New ND4JIllegalStateException("Num input arguments must be 2 or 4.")
			End If

			If numIArguments() < 5 Then
				Throw New ND4JIllegalStateException("Number of integer arguments must >= 5")
			End If
		End Sub

		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Differentation not supported for backprop function: " & Me.GetType().Name)
		End Function

		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes.Count = 2 OrElse dataTypes.Count = 5, "Expected list with exactly 2 or 5 datatypes for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as (original) input type
			Return Collections.singletonList(arg().dataType())
		End Function
	End Class

End Namespace