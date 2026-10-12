Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports NStepQLearning = org.deeplearning4j.rl4j.agent.learning.algorithm.nstepqlearning.NStepQLearning
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports AsyncSharedNetworksUpdateHandler = org.deeplearning4j.rl4j.agent.learning.update.updater.async.AsyncSharedNetworksUpdateHandler
Imports org.deeplearning4j.rl4j.environment
Imports org.deeplearning4j.rl4j.environment
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

	Public Class NStepQLearningBuilder
		Inherits BaseAsyncAgentLearnerBuilder(Of NStepQLearningBuilder.Configuration)

		Private ReadOnly rnd As Random

		Public Sub New(ByVal configuration As Configuration, ByVal neuralNet As ITrainableNeuralNet, ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal rnd As Random)
			MyBase.New(configuration, neuralNet, environmentBuilder, transformProcessBuilder)

			' TODO: remove once RNN networks states are stored in the experience elements
			Preconditions.checkArgument(Not neuralNet.isRecurrent() OrElse configuration.getExperienceHandlerConfiguration().getBatchSize() = Integer.MaxValue, "RL with a recurrent network currently only works with whole-trajectory updates. Until RNN are fully supported, please set the batch size of your experience handler to Integer.MAX_VALUE")

			Me.rnd = rnd
		End Sub

		Protected Friend Overrides Function buildPolicy() As IPolicy(Of Integer)
			Dim greedyPolicy As INeuralNetPolicy(Of Integer) = New DQNPolicy(Of Integer)(networks.ThreadCurrentNetwork)
			Dim actionSchema As IActionSchema(Of Integer) = getEnvironment().getSchema().getActionSchema()
			Return New EpsGreedy(greedyPolicy, actionSchema, configuration.getPolicyConfiguration(), rnd)
		End Function

		Protected Friend Overrides Function buildUpdateAlgorithm() As IUpdateAlgorithm(Of Gradients, StateActionReward(Of Integer))
			Dim actionSchema As IActionSchema(Of Integer) = getEnvironment().getSchema().getActionSchema()
			Return New NStepQLearning(networks.ThreadCurrentNetwork, networks.TargetNetwork, actionSchema.ActionSpaceSize, configuration.getNstepQLearningConfiguration())
		End Function

		Protected Friend Overrides Function buildAsyncSharedNetworksUpdateHandler() As AsyncSharedNetworksUpdateHandler
			Return New AsyncSharedNetworksUpdateHandler(networks.GlobalCurrentNetwork, networks.TargetNetwork, configuration.getNeuralNetUpdaterConfiguration())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @SuperBuilder @Data public static class Configuration extends BaseAsyncAgentLearnerBuilder.Configuration
		Public Class Configuration
			Inherits BaseAsyncAgentLearnerBuilder.Configuration

			Friend nstepQLearningConfiguration As NStepQLearning.Configuration
		End Class
	End Class

End Namespace