Imports System
#If SAMEDIFF Then
Imports LossReduce = DeepLearning4VB.org.nd4vb.autodiff.loss.LossReduce
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

    ''' <summary>Java <c>org.nd4j.linalg.api.ops.impl.loss.bp.CosineDistanceLossBp</c>.</summary>
    Public Class CosineDistanceLossBp
        Inherits BaseLossBp

        Private dimension As Integer

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable, ByVal dimension As Integer)
			MyBase.New(sameDiff, lossReduce, predictions, weights, labels)
			Me.dimension = dimension
			Me.addIArgument(dimension)
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "cosine_distance_loss_grad"
        End Function

    End Class

End Namespace
