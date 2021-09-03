Imports System
Imports Data = lombok.Data
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports AddOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp
Imports GaussianDistribution = org.nd4j.linalg.api.ops.random.impl.GaussianDistribution
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.nn.conf.dropout

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class GaussianNoise implements IDropout
	<Serializable>
	Public Class GaussianNoise
		Implements IDropout

		Private stddev As Double
		Private stddevSchedule As ISchedule

		''' <param name="stddev"> Standard deviation for the mean 0 Gaussian noise </param>
		Public Sub New(ByVal stddev As Double)
			Me.New(stddev, Nothing)
		End Sub

		''' <param name="stddevSchedule"> Schedule for standard deviation for the mean 0 Gaussian noise </param>
		Public Sub New(ByVal stddevSchedule As ISchedule)
			Me.New(Double.NaN, stddevSchedule)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected GaussianNoise(@JsonProperty("stddev") double stddev, @JsonProperty("stddevSchedule") org.nd4j.linalg.schedule.ISchedule stddevSchedule)
		Protected Friend Sub New(ByVal stddev As Double, ByVal stddevSchedule As ISchedule)
			Me.stddev = stddev
			Me.stddevSchedule = stddevSchedule
		End Sub

		Public Overridable Function applyDropout(ByVal inputActivations As INDArray, ByVal output As INDArray, ByVal iteration As Integer, ByVal epoch As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IDropout.applyDropout
			Dim currS As Double
			If stddevSchedule IsNot Nothing Then
				currS = stddevSchedule.valueAt(iteration, epoch)
			Else
				currS = stddev
			End If

			Dim noise As INDArray = Nd4j.createUninitialized(output.dataType(), inputActivations.shape(), inputActivations.ordering())
			Nd4j.Executioner.exec(New GaussianDistribution(noise, 0, currS))

			Nd4j.Executioner.exec(New AddOp(inputActivations, noise, output))
			Return output
		End Function

		Public Overridable Function backprop(ByVal gradAtOutput As INDArray, ByVal gradAtInput As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As INDArray Implements IDropout.backprop
			'dL/dIn = dL/dOut * dOut/dIn, with dOut/dIn = 1
			If gradAtInput Is gradAtOutput Then
				'Same array (in-place result)
				Return gradAtInput
			Else
				Return gradAtInput.assign(gradAtOutput)
			End If
		End Function

		Public Overridable Sub clear() Implements IDropout.clear
			'No op
		End Sub

		Public Overridable Function clone() As IDropout Implements IDropout.clone
			Return New GaussianNoise(stddev, stddevSchedule)
		End Function
	End Class

End Namespace