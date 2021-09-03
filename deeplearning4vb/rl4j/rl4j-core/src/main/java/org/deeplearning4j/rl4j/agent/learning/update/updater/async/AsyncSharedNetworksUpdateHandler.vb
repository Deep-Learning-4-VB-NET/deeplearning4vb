Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports NeuralNetUpdaterConfiguration = org.deeplearning4j.rl4j.agent.learning.update.updater.NeuralNetUpdaterConfiguration
Imports org.deeplearning4j.rl4j.network
Imports Preconditions = org.nd4j.common.base.Preconditions

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
Namespace org.deeplearning4j.rl4j.agent.learning.update.updater.async

	Public Class AsyncSharedNetworksUpdateHandler

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final org.deeplearning4j.rl4j.network.ITrainableNeuralNet globalCurrent;
		Private ReadOnly globalCurrent As ITrainableNeuralNet

		Private ReadOnly target As ITrainableNeuralNet
		Private ReadOnly targetUpdateFrequency As Integer

		Private updateCount As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AsyncSharedNetworksUpdateHandler(@NonNull ITrainableNeuralNet globalCurrent, @NonNull NeuralNetUpdaterConfiguration configuration)
		Public Sub New(ByVal globalCurrent As ITrainableNeuralNet, ByVal configuration As NeuralNetUpdaterConfiguration)
			Me.globalCurrent = globalCurrent
			Me.target = Nothing
			Me.targetUpdateFrequency = 0
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AsyncSharedNetworksUpdateHandler(@NonNull ITrainableNeuralNet globalCurrent, @NonNull ITrainableNeuralNet target, @NonNull NeuralNetUpdaterConfiguration configuration)
		Public Sub New(ByVal globalCurrent As ITrainableNeuralNet, ByVal target As ITrainableNeuralNet, ByVal configuration As NeuralNetUpdaterConfiguration)
			Preconditions.checkArgument(configuration.getTargetUpdateFrequency() > 0, "Configuration: targetUpdateFrequency must be greater than 0, got: ", configuration.getTargetUpdateFrequency())

			Me.globalCurrent = globalCurrent
			Me.target = target
			Me.targetUpdateFrequency = configuration.getTargetUpdateFrequency()
		End Sub

		''' <summary>
		''' Applies the gradients to the global current and synchronize the target network if necessary </summary>
		''' <param name="gradients"> </param>
		Public Overridable Sub handleGradients(ByVal gradients As Gradients)
			globalCurrent.applyGradients(gradients)
			updateCount += 1

			If target IsNot Nothing Then
				syncTargetNetwork()
			End If
		End Sub

		Private Sub syncTargetNetwork()
			If updateCount Mod targetUpdateFrequency = 0 Then
				target.copyFrom(globalCurrent)
			End If
		End Sub
	End Class

End Namespace