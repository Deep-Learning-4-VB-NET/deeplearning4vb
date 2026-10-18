Imports System
Imports System.Collections.Generic
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.DynamicCustomOp
#If SAMEDIFF Then
Imports SDVariable = DeepLearning4VB.org.nd4vb.autodiff.samediff.SDVariable
Imports SameDiff = DeepLearning4VB.org.nd4vb.autodiff.samediff.SameDiff
Imports CtcLossBp = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.loss.bp.CtcLossBp
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

    ''' <summary>The custom op <c>ctc_loss</c> (Java <c>org.nd4j.linalg.api.ops.impl.loss.CtcLoss</c>).</summary>
    Public Class CtcLoss
        Inherits DynamicCustomOp

        Public Sub New(ByVal targetLabels As INDArray, ByVal logitInputs As INDArray, ByVal targetLabelLengths As INDArray, ByVal logitInputLengths As INDArray)
            MyBase.New(New INDArray(){targetLabels, logitInputs, targetLabelLengths, logitInputLengths},Nothing)
        End Sub

#If SAMEDIFF Then
		Public Sub New(ByVal sameDiff As SameDiff, ByVal targetLabels As SDVariable, ByVal logitInputs As SDVariable, ByVal targetLabelLengths As SDVariable, ByVal logitInputLengths As SDVariable)
			MyBase.New(sameDiff,New SDVariable(){targetLabels, logitInputs, targetLabelLengths, logitInputLengths})
		End Sub
#End If

        Public Sub New()
        End Sub

        Public Overrides Function opName() As String
            Return "ctc_loss"
        End Function

#If SAMEDIFF Then
		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'No external gradient
			'Args are: predictions, weights, label
			Return (New CtcLossBp(sameDiff, arg(0), arg(1), arg(2),arg(3))).outputs()
		End Function
#End If

    End Class

End Namespace
