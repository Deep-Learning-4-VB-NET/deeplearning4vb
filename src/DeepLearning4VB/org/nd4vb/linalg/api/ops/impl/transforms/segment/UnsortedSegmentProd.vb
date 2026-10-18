Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports UnsortedSegmentProdBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.transforms.segment.bp.UnsortedSegmentProdBp
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

Namespace org.nd4vb.linalg.api.ops.impl.transforms.segment

    ''' <summary>The custom op <c>unsorted_segment_prod</c> (Java <c>org.nd4j.linalg.api.ops.impl.transforms.segment.UnsortedSegmentProd</c>).</summary>
    Public Class UnsortedSegmentProd
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Private numSegments As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal data As SDVariable, ByVal segmentIds As SDVariable, ByVal numSegments As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable() {data, segmentIds}, False)
			Me.numSegments = numSegments
			addIArgument(numSegments)
		End Sub
#End If

        Public Sub New(ByVal data As INDArray, ByVal segmentIds As INDArray, ByVal numSegments As Integer)
            MyBase.New(New INDArray(){data, segmentIds}, Nothing)
            Me.numSegments = numSegments
            addIArgument(numSegments)
        End Sub

        Public Overrides Function opName() As String
            Return "unsorted_segment_prod"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "UnsortedSegmentProd"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal gradients As IList(Of SDVariable)) As IList(Of SDVariable)
			Return (New UnsortedSegmentProdBp(sameDiff, arg(0), arg(1), gradients(0), numSegments)).outputs()
		End Function
#End If

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            If dArguments.Count > 0 Then
                Return New List(Of DataType) From {dArguments(0)}
            End If
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso (inputDataTypes.Count = 2 OrElse inputDataTypes.Count = 3), "Expected exactly at least 2 input data types for %s, got %s", Me.GetType(), inputDataTypes)
            Return New List(Of DataType) From {inputDataTypes(0)}
        End Function

    End Class

End Namespace
