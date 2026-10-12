Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MulOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp
Imports GaussianDistribution = org.nd4j.linalg.api.ops.random.impl.GaussianDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ISchedule = org.nd4j.linalg.schedule.ISchedule
Imports JsonIgnoreProperties = org.nd4j.shade.jackson.annotation.JsonIgnoreProperties
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

Namespace org.deeplearning4j.nn.conf.dropout

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @JsonIgnoreProperties({"noise"}) @EqualsAndHashCode(exclude = {"noise"}) public class GaussianDropout implements IDropout
	<Serializable>
	Public Class GaussianDropout
		Implements IDropout

		Private ReadOnly rate As Double
		Private ReadOnly rateSchedule As ISchedule
		<NonSerialized>
		Private noise As INDArray

		''' <param name="rate"> Rate parameter, see <seealso cref="GaussianDropout"/> </param>
		Public Sub New(ByVal rate As Double)
			Me.New(rate, Nothing)
		End Sub

		''' <param name="rateSchedule"> Schedule for rate parameter, see <seealso cref="GaussianDropout"/> </param>
		Public Sub New(ByVal rateSchedule As ISchedule)
			Me.New(Double.NaN, rateSchedule)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected GaussianDropout(@JsonProperty("rate") double rate, @JsonProperty("rateSchedule") org.nd4j.linalg.schedule.ISchedule rateSchedule)
		Protected Friend Sub New(ByVal rate As Double, ByVal rateSchedule As ISchedule)
			Me.rate = rate
			Me.rateSchedule = rateSchedule
		End Sub

		Public Overridable Function applyDropout(ByVal inputActivations As INDArray, ByVal output As INDArray, ByVal iteration As Integer, ByVal epoch As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IDropout.applyDropout
			Dim r As Double
			If rateSchedule IsNot Nothing Then
				r = rateSchedule.valueAt(iteration, epoch)
			Else
				r = rate
			End If

			Dim stdev As Double = Math.Sqrt(r / (1.0 - r))

			noise = workspaceMgr.createUninitialized(ArrayType.INPUT, output.dataType(), inputActivations.shape(), inputActivations.ordering())
			Nd4j.Executioner.exec(New GaussianDistribution(noise, 1.0, stdev))

			Return Nd4j.Executioner.exec(New MulOp(inputActivations, noise, output))(0)
		End Function

		Public Overridable Function backprop(ByVal gradAtOutput As INDArray, ByVal gradAtInput As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As INDArray Implements IDropout.backprop
			Preconditions.checkState(noise IsNot Nothing, "Cannot perform backprop: GaussianDropout noise array is absent (already cleared?)")
			'out = in*y, where y ~ N(1, stdev)
			'dL/dIn = dL/dOut * dOut/dIn = y * dL/dOut
			Nd4j.Executioner.exec(New MulOp(gradAtOutput, noise, gradAtInput))
			noise = Nothing
			Return gradAtInput
		End Function

		Public Overridable Sub clear() Implements IDropout.clear
			noise = Nothing
		End Sub

		Public Overridable Function clone() As GaussianDropout
			Return New GaussianDropout(rate,If(rateSchedule Is Nothing, Nothing, rateSchedule.clone()))
		End Function
	End Class

End Namespace