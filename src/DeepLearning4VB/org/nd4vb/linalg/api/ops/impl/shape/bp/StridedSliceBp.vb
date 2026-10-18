Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
#End If

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

Namespace org.nd4vb.linalg.api.ops.impl.shape.bp

    ''' <summary>The custom op <c>strided_slice_bp</c> (Java <c>org.nd4j.linalg.api.ops.impl.shape.bp.StridedSliceBp</c>).</summary>
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

#If SAMEDIFF Then
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
#End If

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
                Throw New ND4VBIllegalStateException("Num input arguments must be 2 or 4.")
            End If

            If numIArguments() < 5 Then
                Throw New ND4VBIllegalStateException("Number of integer arguments must >= 5")
            End If
        End Sub

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal i_v As IList(Of SDVariable)) As IList(Of SDVariable)
			Throw New System.NotSupportedException("Differentation not supported for backprop function: " & Me.GetType().Name)
		End Function
#End If

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(dataTypes.Count = 2 OrElse dataTypes.Count = 5, "Expected list with exactly 2 or 5 datatypes for %s, got %s", Me.GetType(), dataTypes)
			'Output type is same as (original) input type
			Return Collections.singletonList(arg().dataType())
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
