Imports System
Imports System.Collections.Generic
Imports LossReduce = DeepLearning4VB.org.nd4vb.autodiff.loss.LossReduce
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports SigmoidCrossEntropyLossBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.loss.bp.SigmoidCrossEntropyLossBp
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.loss.SigmoidCrossEntropyLoss</c>.</summary>
    Public Class SigmoidCrossEntropyLoss
        Inherits BaseLoss

        Public Sub New()
        End Sub

        Public Const DEFAULT_LABEL_SMOOTHING As Double = 0.0
        Private labelSmoothing As Double = 0.0

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal labels As SDVariable, ByVal logits As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal labelSmoothing As Double)
			Me.New(sameDiff, lossReduce, logits, weights, labels, labelSmoothing)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal logits As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable, ByVal labelSmoothing As Double)
			MyBase.New(sameDiff, lossReduce, logits, weights, labels)
			Me.labelSmoothing = labelSmoothing
			addArgs()
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal reductionMode As LossReduce, ByVal logits As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable)
			Me.New(sameDiff, reductionMode, logits, weights, labels, 0.0)
		End Sub
#End If

        Public Sub New(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal weights As INDArray, ByVal lossReduce As LossReduce, ByVal labelSmoothing As Double)
            MyBase.New(lossReduce, predictions, weights, labels)
            Me.labelSmoothing = labelSmoothing
            addArgs()
        End Sub

        Protected Friend Overrides Sub addArgs()
            MyBase.addArgs()
            addTArgument(labelSmoothing)
        End Sub

        Public Overrides Function opName() As String
            Return "sigm_cross_entropy_loss"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'No external gradient
			'Args are: predictions, weights, label
			Return (New SigmoidCrossEntropyLossBp(sameDiff, lossReduce, arg(0), arg(1), arg(2), labelSmoothing)).outputs()
		End Function
#End If

    End Class

End Namespace
