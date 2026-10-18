Imports System
Imports System.Collections.Generic
Imports LossReduce = DeepLearning4VB.org.nd4vb.autodiff.loss.LossReduce
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports AbsoluteDifferenceLossBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.loss.bp.AbsoluteDifferenceLossBp
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.loss.AbsoluteDifferenceLoss</c>.</summary>
    Public Class AbsoluteDifferenceLoss
        Inherits BaseLoss

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable)
			MyBase.New(sameDiff, lossReduce, predictions, weights, labels)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal label As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce)
			Me.New(sameDiff, lossReduce, predictions, weights, label)
		End Sub
#End If

        Public Sub New(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal weights As INDArray, ByVal lossReduce As LossReduce)
            MyBase.New(lossReduce, predictions, weights, labels)
        End Sub

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "absolute_difference_loss"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'No external gradient
			'Args are: predictions, weights, label
			Return (New AbsoluteDifferenceLossBp(sameDiff, lossReduce, arg(0), arg(1), arg(2))).outputs()
		End Function
#End If

    End Class

End Namespace
