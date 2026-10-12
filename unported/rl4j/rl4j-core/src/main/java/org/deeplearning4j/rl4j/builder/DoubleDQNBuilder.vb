Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports DoubleDQN = org.deeplearning4j.rl4j.agent.learning.algorithm.dqn.DoubleDQN
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports org.deeplearning4j.rl4j.environment
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.network
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
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

	''' <summary>
	''' A <seealso cref="IAgentLearner"/> builder that will setup a <seealso cref="DoubleDQN Double-DQN"/> algorithm in addition to the setup done by <seealso cref="BaseDQNAgentLearnerBuilder"/>.
	''' </summary>
	Public Class DoubleDQNBuilder
		Inherits BaseDQNAgentLearnerBuilder(Of DoubleDQNBuilder.Configuration)


		Public Sub New(ByVal configuration As Configuration, ByVal neuralNet As ITrainableNeuralNet, ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess), ByVal rnd As Random)
			MyBase.New(configuration, neuralNet, environmentBuilder, transformProcessBuilder, rnd)
		End Sub

		Protected Friend Overrides Function buildUpdateAlgorithm() As IUpdateAlgorithm(Of FeaturesLabels, StateActionRewardState(Of Integer))
			Return New DoubleDQN(networks.ThreadCurrentNetwork, networks.TargetNetwork, configuration.getUpdateAlgorithmConfiguration())
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @SuperBuilder @Data public static class Configuration extends BaseDQNAgentLearnerBuilder.Configuration
		Public Class Configuration
			Inherits BaseDQNAgentLearnerBuilder.Configuration

		End Class
	End Class

End Namespace