Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports CtcLossBp = org.nd4j.linalg.api.ops.impl.loss.bp.CtcLossBp

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

Namespace org.nd4j.linalg.api.ops.impl.loss


	Public Class CtcLoss
		Inherits DynamicCustomOp


		Public Sub New(ByVal targetLabels As INDArray, ByVal logitInputs As INDArray, ByVal targetLabelLengths As INDArray, ByVal logitInputLengths As INDArray)
			MyBase.New(New INDArray(){targetLabels, logitInputs, targetLabelLengths, logitInputLengths},Nothing)
		End Sub


		Public Sub New(ByVal sameDiff As SameDiff, ByVal targetLabels As SDVariable, ByVal logitInputs As SDVariable, ByVal targetLabelLengths As SDVariable, ByVal logitInputLengths As SDVariable)
			MyBase.New(sameDiff,New SDVariable(){targetLabels, logitInputs, targetLabelLengths, logitInputLengths})
		End Sub


		Public Sub New()
		End Sub

		Public Overrides Function opName() As String
			Return "ctc_loss"
		End Function

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'No external gradient
			'Args are: predictions, weights, label
			Return (New CtcLossBp(sameDiff, arg(0), arg(1), arg(2),arg(3))).outputs()
		End Function
	End Class

End Namespace