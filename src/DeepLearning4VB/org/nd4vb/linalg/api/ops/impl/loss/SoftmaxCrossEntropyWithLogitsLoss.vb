Imports System
Imports System.Collections.Generic
Imports Preconditions = DeepLearning4VB.org.nd4vb.common.base.Preconditions
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports SoftmaxCrossEntropyWithLogitsLossBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.loss.bp.SoftmaxCrossEntropyWithLogitsLossBp
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

Namespace org.nd4vb.linalg.api.ops.impl.loss

    ''' <summary>The custom op <c>softmax_cross_entropy_loss_with_logits</c> (Java <c>org.nd4j.linalg.api.ops.impl.loss.SoftmaxCrossEntropyWithLogitsLoss</c>).</summary>
    Public Class SoftmaxCrossEntropyWithLogitsLoss
        Inherits DynamicCustomOp

        Public Sub New()
        End Sub

        Protected Friend classesDim As Integer

#If SAMEDIFF Then
	'    public SoftmaxCrossEntropyWithLogitsLoss(SameDiff sameDiff, SDVariable logits, SDVariable weights, SDVariable labels, int classesDim) {
	'        super(null, sameDiff, new SDVariable[]{logits, weights, labels}, false);
	'        this.classesDim = classesDim;
	'        addIArgument(classesDim);
	'    }

		Public Sub New(ByVal sameDiff As SameDiff, ByVal logits As SDVariable, ByVal labels As SDVariable, ByVal classesDim As Integer)
			MyBase.New(Nothing, sameDiff, New SDVariable(){logits, labels}, False)
			Me.classesDim = classesDim
			addIArgument(classesDim)
		End Sub
#End If

        Public Overrides Function opName() As String
            Return "softmax_cross_entropy_loss_with_logits"
        End Function

        Public Overrides Function tensorflowName() As String
            Return "SoftmaxCrossEntropyWithLogits"
        End Function

        Public Overrides Function calculateOutputDataTypes(ByVal inputDataTypes As IList(Of DataType)) As IList(Of DataType)
            Preconditions.checkState(inputDataTypes IsNot Nothing AndAlso (inputDataTypes.Count = 2 OrElse inputDataTypes.Count = 3), "Expected 2 or 3 input datatypes for %s, got %s", Me.GetType(), inputDataTypes)

            Return JavaCollections.singletonList(inputDataTypes(0)) 'Same as predictions
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'No external gradient
			'Args: logits, weigths, label
			Return (New SoftmaxCrossEntropyWithLogitsLossBp(sameDiff, arg(0), arg(1), classesDim)).outputs()
		End Function
#End If

    End Class

End Namespace
