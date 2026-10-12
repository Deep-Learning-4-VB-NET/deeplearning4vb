Imports System
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Axpy = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.Axpy
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports FixedSchedule = org.nd4j.linalg.schedule.FixedSchedule
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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

Namespace org.nd4j.linalg.learning.regularization

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class WeightDecay implements Regularization
	<Serializable>
	Public Class WeightDecay
		Implements Regularization

		Protected Friend ReadOnly coeff As ISchedule
		Protected Friend ReadOnly applyLR As Boolean

		''' <param name="coeff">   Weight decay regularization coefficient </param>
		''' <param name="applyLR"> If true, multiply the regularization coefficient by the current learning rate. If false, do not multiply by LR. </param>
		Public Sub New(ByVal coeff As Double, ByVal applyLR As Boolean)
			Me.New(New FixedSchedule(coeff), applyLR)
		End Sub

		''' <param name="coeff">   Weight decay regularization coefficient (schedule) </param>
		''' <param name="applyLR"> If true, multiply the regularization coefficient by the current learning rate. If false, do not multiply by LR. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public WeightDecay(@JsonProperty("coeff") @NonNull ISchedule coeff, @JsonProperty("applyLR") boolean applyLR)
		Public Sub New(ByVal coeff As ISchedule, ByVal applyLR As Boolean)
			Me.coeff = coeff
			Me.applyLR = applyLR
		End Sub

		Public Overridable Function applyStep() As ApplyStep Implements Regularization.applyStep
			Return ApplyStep.POST_UPDATER
		End Function

		Public Overridable Sub apply(ByVal param As INDArray, ByVal gradView As INDArray, ByVal lr As Double, ByVal iteration As Integer, ByVal epoch As Integer) Implements Regularization.apply
			'L = loss + coeff * 0.5 * sum_i x[i]^2
			'dL/dx[i] = coeff * x[i]
			'update(x[i]) = coeff * x[i] * ( applyLR ? lr : )
			Dim scale As Double = coeff.valueAt(iteration, epoch)
			If applyLR Then
				scale *= lr
			End If
			Nd4j.exec(New Axpy(param, gradView, gradView, scale)) 'update = scale * param + update
		End Sub

		Public Overridable Function score(ByVal param As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements Regularization.score
			'Score: L = 0.5 * sum_i x[i]^2
			Dim norm2 As Double = param.norm2Number().doubleValue() 'Norm2 is sqrt(sum_i x[i]^2)
			Return coeff.valueAt(iteration, epoch) * 0.5 * norm2 * norm2
		End Function

		Public Overridable Function clone() As Regularization Implements Regularization.clone
			Return New WeightDecay(coeff.clone(), applyLR)
		End Function
	End Class

End Namespace