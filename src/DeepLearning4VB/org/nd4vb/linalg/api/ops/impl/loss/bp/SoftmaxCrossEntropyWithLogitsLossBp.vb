Imports System
Imports System.Collections.Generic
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
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

    ''' <summary>The custom op <c>softmax_cross_entropy_loss_with_logits_grad</c> (Java <c>org.nd4j.linalg.api.ops.impl.loss.bp.SoftmaxCrossEntropyWithLogitsLossBp</c>).</summary>
    Public Class SoftmaxCrossEntropyWithLogitsLossBp
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend classesDim As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal logits As SDVariable, ByVal labels As SDVariable, ByVal classesDim As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){logits, labels}, False)
			Me.classesDim = classesDim
			addIArgument(classesDim)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "softmax_cross_entropy_loss_with_logits_grad"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New System.NotSupportedException("Differentiation of " & Me.GetType().FullName & " not supported")
		End Function
#End If

#If SAMEDIFF Then
		Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
			Return New List(Of DataType) From {arg(0).dataType(), arg(1).dataType()}
		End Function
#Else
        Public Overrides Function calculateOutputDataTypes(ByVal dataTypes As IList(Of DataType)) As IList(Of DataType)
            Throw New NotSupportedException("calculateOutputDataTypes needs SameDiff (phase 2 milestone M5)")
        End Function
#End If

    End Class

End Namespace
