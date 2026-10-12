Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports org.deeplearning4j.rl4j.agent.learning.update.updater
Imports NeuralNetUpdaterConfiguration = org.deeplearning4j.rl4j.agent.learning.update.updater.NeuralNetUpdaterConfiguration
Imports AsyncGradientsNeuralNetUpdater = org.deeplearning4j.rl4j.agent.learning.update.updater.async.AsyncGradientsNeuralNetUpdater
Imports AsyncSharedNetworksUpdateHandler = org.deeplearning4j.rl4j.agent.learning.update.updater.async.AsyncSharedNetworksUpdateHandler
Imports org.deeplearning4j.rl4j.environment
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.network
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports org.deeplearning4j.rl4j.policy

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

	Public MustInherit Class BaseAsyncAgentLearnerBuilder(Of CONFIGURATION_TYPE As BaseAsyncAgentLearnerBuilder.Configuration)
		Inherits BaseAgentLearnerBuilder(Of Integer, StateActionReward(Of Integer), Gradients, CONFIGURATION_TYPE)

		Private ReadOnly asyncSharedNetworksUpdateHandler As AsyncSharedNetworksUpdateHandler

		Public Sub New(ByVal configuration As CONFIGURATION_TYPE, ByVal neuralNet As ITrainableNeuralNet, ByVal environmentBuilder As Builder(Of Environment(Of Integer)), ByVal transformProcessBuilder As Builder(Of TransformProcess))
			MyBase.New(configuration, neuralNet, environmentBuilder, transformProcessBuilder)

			asyncSharedNetworksUpdateHandler = buildAsyncSharedNetworksUpdateHandler()
		End Sub

		Protected Friend Overrides Function buildExperienceHandler() As ExperienceHandler(Of Integer, StateActionReward(Of Integer))
			Return New StateActionExperienceHandler(Of Integer)(configuration.getExperienceHandlerConfiguration())
		End Function

		Protected Friend Overrides Function buildNeuralNetUpdater() As INeuralNetUpdater(Of Gradients)
			Return New AsyncGradientsNeuralNetUpdater(networks.ThreadCurrentNetwork, asyncSharedNetworksUpdateHandler)
		End Function

		Protected Friend MustOverride Function buildAsyncSharedNetworksUpdateHandler() As AsyncSharedNetworksUpdateHandler

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @SuperBuilder @Data public static class Configuration extends BaseAgentLearnerBuilder.Configuration<Integer>
		Public Class Configuration
			Inherits BaseAgentLearnerBuilder.Configuration(Of Integer)

			Friend policyConfiguration As EpsGreedy.Configuration
			Friend neuralNetUpdaterConfiguration As NeuralNetUpdaterConfiguration
			Friend experienceHandlerConfiguration As StateActionExperienceHandler.Configuration
		End Class
	End Class

End Namespace