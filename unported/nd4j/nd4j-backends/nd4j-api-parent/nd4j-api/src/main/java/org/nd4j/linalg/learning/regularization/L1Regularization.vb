Imports System
Imports Data = lombok.Data
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Axpy = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.Axpy
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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
'ORIGINAL LINE: @Data public class L1Regularization implements Regularization
	<Serializable>
	Public Class L1Regularization
		Implements Regularization

		Protected Friend ReadOnly l1 As ISchedule

		''' <param name="l1">   l1 regularization coefficient </param>
		Public Sub New(ByVal l1 As Double)
			Me.New(New FixedSchedule(l1))
		End Sub

		''' <param name="l1"> L1 regularization coefficient (schedule) </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public L1Regularization(@JsonProperty("l1") @NonNull ISchedule l1)
		Public Sub New(ByVal l1 As ISchedule)
			Me.l1 = l1
		End Sub

		Public Overridable Function applyStep() As ApplyStep Implements Regularization.applyStep
			Return ApplyStep.BEFORE_UPDATER
		End Function

		Public Overridable Sub apply(ByVal param As INDArray, ByVal gradView As INDArray, ByVal lr As Double, ByVal iteration As Integer, ByVal epoch As Integer) Implements Regularization.apply
			'L = loss + l1 * sum_i abs(x[i])
			'dL/dx[i] = dloss/dx[i] + l1 * sign(x[i])
			'where sign(x[i]) is -1 or 1
			Dim coeff As Double = l1.valueAt(iteration, epoch)
			Dim sign As INDArray = Transforms.sign(param, True)
			Nd4j.exec(New Axpy(sign, gradView, gradView, coeff)) 'Gradient = l1 * sign(param) + gradient
		End Sub

		Public Overridable Function score(ByVal param As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As Double Implements Regularization.score
			Return l1.valueAt(iteration, epoch) * param.norm1Number().doubleValue()
		End Function

		Public Overridable Function clone() As Regularization Implements Regularization.clone
			Return New L1Regularization(l1.clone())
		End Function
	End Class

End Namespace