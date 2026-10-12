Imports System
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports val = lombok.val
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DropOutInverted = org.nd4j.linalg.api.ops.random.impl.DropOutInverted
Imports Broadcast = org.nd4j.linalg.factory.Broadcast
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
'ORIGINAL LINE: @Data @JsonIgnoreProperties({"mask"}) @EqualsAndHashCode(exclude = {"mask"}) public class SpatialDropout implements IDropout
	<Serializable>
	Public Class SpatialDropout
		Implements IDropout

		Private p As Double
		Private pSchedule As ISchedule
		<NonSerialized>
		Private mask As INDArray

		''' <param name="activationRetainProbability"> Probability of retaining an activation - see <seealso cref="Dropout"/> javadoc </param>
		Public Sub New(ByVal activationRetainProbability As Double)
			Me.New(activationRetainProbability, Nothing)
			If activationRetainProbability < 0.0 Then
				Throw New System.ArgumentException("Activation retain probability must be > 0. Got: " & activationRetainProbability)
			End If
			If activationRetainProbability = 0.0 Then
				Throw New System.ArgumentException("Invalid probability value: Dropout with 0.0 probability of retaining " & "activations is not supported")
			End If
		End Sub

		''' <param name="activationRetainProbabilitySchedule"> Schedule for probability of retaining an activation - see <seealso cref="Dropout"/> javadoc </param>
		Public Sub New(ByVal activationRetainProbabilitySchedule As ISchedule)
			Me.New(Double.NaN, activationRetainProbabilitySchedule)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected SpatialDropout(@JsonProperty("p") double activationRetainProbability, @JsonProperty("pSchedule") org.nd4j.linalg.schedule.ISchedule activationRetainProbabilitySchedule)
		Protected Friend Sub New(ByVal activationRetainProbability As Double, ByVal activationRetainProbabilitySchedule As ISchedule)
			Me.p = activationRetainProbability
			Me.pSchedule = activationRetainProbabilitySchedule
		End Sub


		Public Overridable Function applyDropout(ByVal inputActivations As INDArray, ByVal output As INDArray, ByVal iteration As Integer, ByVal epoch As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IDropout.applyDropout
			Preconditions.checkArgument(inputActivations.rank() = 5 OrElse inputActivations.rank() = 4 OrElse inputActivations.rank() = 3, "Cannot apply spatial dropout to activations of rank %s: " & "spatial dropout can only be used for rank 3, 4 or 5 activations (input activations shape: %s)", inputActivations.rank(), inputActivations.shape())

			Dim currP As Double
			If pSchedule IsNot Nothing Then
				currP = pSchedule.valueAt(iteration, epoch)
			Else
				currP = p
			End If

			Dim minibatch As val = inputActivations.size(0)
			Dim dim1 As val = inputActivations.size(1)
			mask = workspaceMgr.createUninitialized(ArrayType.INPUT, output.dataType(), minibatch, dim1).assign(1.0)
			Nd4j.Executioner.exec(New DropOutInverted(mask, currP))

			Broadcast.mul(inputActivations, mask, output, 0, 1)
			Return output
		End Function

		Public Overridable Function backprop(ByVal gradAtOutput As INDArray, ByVal gradAtInput As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As INDArray Implements IDropout.backprop
			Preconditions.checkState(mask IsNot Nothing, "Cannot perform backprop: Dropout mask array is absent (already cleared?)")
			'Mask has values 0 or 1/p
			'dL/dIn = dL/dOut * dOut/dIn = dL/dOut * (0 if dropped, or 1/p otherwise)
			Broadcast.mul(gradAtOutput, mask, gradAtInput, 0, 1)
			mask = Nothing
			Return gradAtInput
		End Function

		Public Overridable Sub clear() Implements IDropout.clear
			mask = Nothing
		End Sub

		Public Overridable Function clone() As IDropout Implements IDropout.clone
			Return New SpatialDropout(p, pSchedule)
		End Function
	End Class

End Namespace