Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports NonNull = lombok.NonNull
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent
Imports AdvantageActorCritic = org.deeplearning4j.rl4j.agent.learning.algorithm.actorcritic.AdvantageActorCritic
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports AsyncSharedNetworksUpdateHandler = org.deeplearning4j.rl4j.agent.learning.update.updater.async.AsyncSharedNetworksUpdateHandler
Imports org.deeplearning4j.rl4j.environment
Imports org.deeplearning4j.rl4j.environment
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.network
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports org.deeplearning4j.rl4j.policy
Imports org.deeplearning4j.rl4j.policy
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

	Public Class AdvantageActorCriticBuilder
		Inherits BaseAsyncAgentLearnerBuilder(Of AdvantageActorCriticBuilder.Configuration)

		Private ReadOnly rnd As Random

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public AdvantageActorCriticBuilder(@NonNull Configuration configuration, @NonNull ITrainableNeuralNet neuralNet, @NonNull Builder<org.deeplearning4j.rl4j.environment.Environment<Integer>> environmentBuilder, @NonNull Builder<org.deeplearning4j.rl4j.observation.transform.TransformProcess> transformProcessBuilder, org.nd4j.linalg.api.rng.Random rnd)
		Public Sub New(ByVal configuration As Configuration, ByVal neuralNet As ITrainableNeuralNet, ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal rnd As Random)
			MyBase.New(configuration, neuralNet, environmentBuilder, transformProcessBuilder)
			Me.rnd = rnd
		End Sub

		Protected Friend Overrides Function buildPolicy() As IPolicy(Of Integer)
			Return ACPolicy.builder().neuralNet(networks.getThreadCurrentNetwork()).isTraining(True).rnd(rnd).build()
		End Function

		Protected Friend Overrides Function buildUpdateAlgorithm() As IUpdateAlgorithm(Of Gradients, StateActionReward(Of Integer))
			Dim actionSchema As IActionSchema(Of Integer) = getEnvironment().getSchema().getActionSchema()
			Return New AdvantageActorCritic(networks.getThreadCurrentNetwork(), actionSchema.ActionSpaceSize, configuration.getAdvantageActorCriticConfiguration())
		End Function

		Protected Friend Overrides Function buildAsyncSharedNetworksUpdateHandler() As AsyncSharedNetworksUpdateHandler
			Return New AsyncSharedNetworksUpdateHandler(networks.getGlobalCurrentNetwork(), configuration.getNeuralNetUpdaterConfiguration())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @SuperBuilder @Data public static class Configuration extends BaseAsyncAgentLearnerBuilder.Configuration
		Public Class Configuration
			Inherits BaseAsyncAgentLearnerBuilder.Configuration

			Friend advantageActorCriticConfiguration As AdvantageActorCritic.Configuration
		End Class

	End Class

End Namespace