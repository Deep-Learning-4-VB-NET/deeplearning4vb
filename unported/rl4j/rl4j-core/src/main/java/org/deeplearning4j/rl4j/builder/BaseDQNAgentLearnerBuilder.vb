Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent
Imports BaseTransitionTDAlgorithm = org.deeplearning4j.rl4j.agent.learning.algorithm.dqn.BaseTransitionTDAlgorithm
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports org.deeplearning4j.rl4j.agent.learning.update.updater
Imports NeuralNetUpdaterConfiguration = org.deeplearning4j.rl4j.agent.learning.update.updater.NeuralNetUpdaterConfiguration
Imports SyncLabelsNeuralNetUpdater = org.deeplearning4j.rl4j.agent.learning.update.updater.sync.SyncLabelsNeuralNetUpdater
Imports org.deeplearning4j.rl4j.environment
Imports org.deeplearning4j.rl4j.environment
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.network
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.policy
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Random = org.nd4j.linalg.api.rng.Random

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
Namespace org.deeplearning4j.rl4j.builder

	Public MustInherit Class BaseDQNAgentLearnerBuilder(Of CONFIGURATION_TYPE As BaseDQNAgentLearnerBuilder.Configuration)
		Inherits BaseAgentLearnerBuilder(Of Integer, StateActionRewardState(Of Integer), FeaturesLabels, CONFIGURATION_TYPE)

		Private ReadOnly rnd As Random

		Public Sub New(ByVal configuration As CONFIGURATION_TYPE, ByVal neuralNet As ITrainableNeuralNet, ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal rnd As Random)
			MyBase.New(configuration, neuralNet, environmentBuilder, transformProcessBuilder)

			' TODO: remove once RNN networks states are supported with DQN
			Preconditions.checkArgument(Not neuralNet.isRecurrent(), "Recurrent networks are not yet supported with DQN.")
			Me.rnd = rnd
		End Sub

		Protected Friend Overrides Function buildPolicy() As IPolicy(Of Integer)
			Dim greedyPolicy As INeuralNetPolicy(Of Integer) = New DQNPolicy(Of Integer)(networks.ThreadCurrentNetwork)
			Dim actionSchema As IActionSchema(Of Integer) = getEnvironment().getSchema().getActionSchema()
			Return New EpsGreedy(greedyPolicy, actionSchema, configuration.getPolicyConfiguration(), rnd)
		End Function

		Protected Friend Overrides Function buildExperienceHandler() As ExperienceHandler(Of Integer, StateActionRewardState(Of Integer))
			Return New ReplayMemoryExperienceHandler(configuration.getExperienceHandlerConfiguration(), rnd)
		End Function

		Protected Friend Overrides Function buildNeuralNetUpdater() As INeuralNetUpdater(Of FeaturesLabels)
			If configuration.isAsynchronous() Then
				Throw New System.NotSupportedException("Only synchronized use is currently supported")
			End If

			Return New SyncLabelsNeuralNetUpdater(networks.ThreadCurrentNetwork, networks.TargetNetwork, configuration.getNeuralNetUpdaterConfiguration())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @SuperBuilder @Data public static class Configuration extends BaseAgentLearnerBuilder.Configuration<Integer>
		Public Class Configuration
			Inherits BaseAgentLearnerBuilder.Configuration(Of Integer)

			Friend policyConfiguration As EpsGreedy.Configuration
			Friend experienceHandlerConfiguration As ReplayMemoryExperienceHandler.Configuration
			Friend neuralNetUpdaterConfiguration As NeuralNetUpdaterConfiguration
			Friend updateAlgorithmConfiguration As BaseTransitionTDAlgorithm.Configuration
		End Class
	End Class

End Namespace