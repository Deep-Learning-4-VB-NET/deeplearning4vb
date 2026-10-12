Imports NonNull = lombok.NonNull
Imports org.deeplearning4j.rl4j.agent.learning.update.updater
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
Namespace org.deeplearning4j.rl4j.agent.learning.update.updater.sync

	Public MustInherit Class BaseSyncNeuralNetUpdater(Of DATA_TYPE)
		Implements INeuralNetUpdater(Of DATA_TYPE)

		Protected Friend ReadOnly current As ITrainableNeuralNet
		Private ReadOnly target As ITrainableNeuralNet

		Private ReadOnly targetUpdateFrequency As Integer
		Private updateCount As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseSyncNeuralNetUpdater(@NonNull ITrainableNeuralNet current, @NonNull ITrainableNeuralNet target, @NonNull NeuralNetUpdaterConfiguration configuration)
		Protected Friend Sub New(ByVal current As ITrainableNeuralNet, ByVal target As ITrainableNeuralNet, ByVal configuration As NeuralNetUpdaterConfiguration)
			Preconditions.checkArgument(configuration.getTargetUpdateFrequency() > 0, "Configuration: targetUpdateFrequency must be greater than 0, got: ", configuration.getTargetUpdateFrequency())

			Me.current = current
			Me.target = target
			Me.targetUpdateFrequency = configuration.getTargetUpdateFrequency()
		End Sub

		Public MustOverride Overrides Sub update(ByVal dataType As DATA_TYPE) Implements INeuralNetUpdater(Of DATA_TYPE).update

		Protected Friend Overridable Sub syncTargetNetwork()
			updateCount += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if(++updateCount % targetUpdateFrequency == 0)
			If updateCount Mod targetUpdateFrequency = 0 Then
				target.copyFrom(current)
			End If
		End Sub

		Public Overridable Sub synchronizeCurrent() Implements INeuralNetUpdater(Of DATA_TYPE).synchronizeCurrent
			' Do nothing; there is only one current network in the sync setup.
		End Sub
	End Class

End Namespace