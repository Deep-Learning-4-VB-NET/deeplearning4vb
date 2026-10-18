Imports System
Imports System.Collections.Generic
Imports LossReduce = DeepLearning4VB.org.nd4vb.autodiff.loss.LossReduce
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
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

Namespace org.nd4vb.linalg.api.ops.impl.loss.bp

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.loss.bp.BaseLossBp</c>.</summary>
    Public MustInherit Class BaseLossBp
        Inherits DynamicCustomOp

        Protected Friend lossReduce As LossReduce

#If SAMEDIFF Then
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseLossBp(@NonNull SameDiff sameDiff, @NonNull LossReduce lossReduce, @NonNull SDVariable predictions, @NonNull SDVariable weights, @NonNull SDVariable labels)
		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable)
			MyBase.New(Nothing, sameDiff, New SDVariable(){predictions, weights, labels})
			Me.lossReduce = lossReduce
			addArgs()
		End Sub
#End If

        Protected Friend Sub New()
        End Sub

        Protected Friend Overridable Sub addArgs()
            iArguments.Clear()
            tArguments.Clear()
            addIArgument(CInt(lossReduce)) 'Ops: 0 - 
        End Sub

        Public Overrides MustOverride Function opName() As String

        Public Overrides ReadOnly Property NumOutputs As Integer
            Get
                Return 3
            End Get
        End Property

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
			Preconditions.checkState(inputDataTypes(0).isFPType(), "Input 0 (predictions) must be a floating point type; inputs datatypes are %s for %s", inputDataTypes, Me.GetType())
			Dim dt0 As DataType = inputDataTypes(0)
			Dim dt1 As DataType = arg(1).dataType()
			Dim dt2 As DataType = arg(2).dataType()
			If Not dt1.isFPType() Then
				dt1 = dt0
			End If
			If Not dt2.isFPType() Then
				dt2 = dt0
			End If
			Return New List(Of DataType) From {dt0, dt1, dt2}
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentiation of " & Me.GetType().FullName & " not supported")
		End Function
#End If

    End Class

End Namespace
