Imports System.Collections.Generic
Imports LossReduce = org.nd4j.autodiff.loss.LossReduce
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LogPoissonLossBp = org.nd4j.linalg.api.ops.impl.loss.bp.LogPoissonLossBp

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


	Public Class LogPoissonLoss
		Inherits BaseLoss

		Private full As Boolean

		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable)
			Me.New(sameDiff, lossReduce, predictions, weights, labels, False)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal labels As SDVariable, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal lossReduce As LossReduce, ByVal full As Boolean)
			Me.New(sameDiff, lossReduce, predictions, weights, labels, full)
		End Sub

		Public Sub New(ByVal sameDiff As SameDiff, ByVal lossReduce As LossReduce, ByVal predictions As SDVariable, ByVal weights As SDVariable, ByVal labels As SDVariable, ByVal full As Boolean)
			MyBase.New(sameDiff, lossReduce, predictions, weights, labels)
			Me.full = full
			addArgs()
		End Sub

		Public Sub New(ByVal labels As INDArray, ByVal predictions As INDArray, ByVal weights As INDArray, ByVal lossReduce As LossReduce, ByVal full As Boolean)
			MyBase.New(lossReduce, predictions, weights, labels)
			Me.full = full
			addArgs()
		End Sub

		Public Sub New()
		End Sub

		Protected Friend Overrides Sub addArgs()
			MyBase.addArgs()
			If full Then
				iArguments.Add(CLng(1))
			End If
		End Sub

		Public Overrides Function opName() As String
			Return "log_poisson_loss"
		End Function

		Public Overrides Function doDiff(ByVal grad As IList(Of SDVariable)) As IList(Of SDVariable)
			'No external gradient
			'Args are: predictions, weights, label
			Return (New LogPoissonLossBp(sameDiff, lossReduce, arg(0), arg(1), arg(2), full)).outputs()
		End Function

	End Class

End Namespace