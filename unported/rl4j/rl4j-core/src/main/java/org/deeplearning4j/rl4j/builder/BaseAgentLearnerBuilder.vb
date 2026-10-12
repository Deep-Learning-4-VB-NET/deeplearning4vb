Imports System.Collections.Generic
Imports AccessLevel = lombok.AccessLevel
Imports Data = lombok.Data
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports SuperBuilder = lombok.experimental.SuperBuilder
Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent
Imports org.deeplearning4j.rl4j.agent
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports org.deeplearning4j.rl4j.agent.learning.behavior
Imports org.deeplearning4j.rl4j.agent.learning.behavior
Imports org.deeplearning4j.rl4j.agent.learning.update
Imports org.deeplearning4j.rl4j.agent.learning.update
Imports org.deeplearning4j.rl4j.agent.learning.update.updater
Imports org.deeplearning4j.rl4j.agent.listener
Imports org.deeplearning4j.rl4j.environment
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


	Public MustInherit Class BaseAgentLearnerBuilder(Of ACTION, EXPERIENCE_TYPE, ALGORITHM_RESULT_TYPE, CONFIGURATION_TYPE As BaseAgentLearnerBuilder.Configuration(Of ACTION))
		Implements Builder(Of IAgentLearner(Of ACTION))

		Protected Friend ReadOnly configuration As CONFIGURATION_TYPE
		Private ReadOnly environmentBuilder As Builder(Of Environment(Of ACTION))
		Private ReadOnly transformProcessBuilder As Builder(Of TransformProcess)
		Protected Friend ReadOnly networks As INetworksHandler

		Protected Friend createdAgentLearnerCount As Integer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BaseAgentLearnerBuilder(@NonNull CONFIGURATION_TYPE configuration, @NonNull ITrainableNeuralNet neuralNet, @NonNull Builder<org.deeplearning4j.rl4j.environment.Environment<ACTION>> environmentBuilder, @NonNull Builder<org.deeplearning4j.rl4j.observation.transform.TransformProcess> transformProcessBuilder)
		Public Sub New(ByVal configuration As CONFIGURATION_TYPE, ByVal neuralNet As ITrainableNeuralNet, ByVal environmentBuilder As Builder(Of Environment(Of ACTION)), ByVal transformProcessBuilder As Builder(Of TransformProcess))
			Me.configuration = configuration
			Me.environmentBuilder = environmentBuilder
			Me.transformProcessBuilder = transformProcessBuilder

			Me.networks = If(configuration.isAsynchronous(), New AsyncNetworkHandler(neuralNet), New SyncNetworkHandler(neuralNet))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private org.deeplearning4j.rl4j.environment.Environment<ACTION> environment;
		Private environment As Environment(Of ACTION)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private org.deeplearning4j.rl4j.observation.transform.TransformProcess transformProcess;
		Private transformProcess As TransformProcess

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private org.deeplearning4j.rl4j.policy.IPolicy<ACTION> policy;
		Private policy As IPolicy(Of ACTION)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private org.deeplearning4j.rl4j.experience.ExperienceHandler<ACTION, EXPERIENCE_TYPE> experienceHandler;
		Private experienceHandler As ExperienceHandler(Of ACTION, EXPERIENCE_TYPE)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private org.deeplearning4j.rl4j.agent.learning.algorithm.IUpdateAlgorithm<ALGORITHM_RESULT_TYPE, EXPERIENCE_TYPE> updateAlgorithm;
		Private updateAlgorithm As IUpdateAlgorithm(Of ALGORITHM_RESULT_TYPE, EXPERIENCE_TYPE)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private org.deeplearning4j.rl4j.agent.learning.update.updater.INeuralNetUpdater<ALGORITHM_RESULT_TYPE> neuralNetUpdater;
		Private neuralNetUpdater As INeuralNetUpdater(Of ALGORITHM_RESULT_TYPE)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private org.deeplearning4j.rl4j.agent.learning.update.IUpdateRule<EXPERIENCE_TYPE> updateRule;
		Private updateRule As IUpdateRule(Of EXPERIENCE_TYPE)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter(lombok.AccessLevel.@PROTECTED) private org.deeplearning4j.rl4j.agent.learning.behavior.ILearningBehavior<ACTION> learningBehavior;
		Private learningBehavior As ILearningBehavior(Of ACTION)

		Protected Friend MustOverride Function buildPolicy() As IPolicy(Of ACTION)
		Protected Friend MustOverride Function buildExperienceHandler() As ExperienceHandler(Of ACTION, EXPERIENCE_TYPE)
		Protected Friend MustOverride Function buildUpdateAlgorithm() As IUpdateAlgorithm(Of ALGORITHM_RESULT_TYPE, EXPERIENCE_TYPE)
		Protected Friend MustOverride Function buildNeuralNetUpdater() As INeuralNetUpdater(Of ALGORITHM_RESULT_TYPE)
		Protected Friend Overridable Function buildUpdateRule() As IUpdateRule(Of EXPERIENCE_TYPE)
			Return New UpdateRule(Of ALGORITHM_RESULT_TYPE, EXPERIENCE_TYPE)(getUpdateAlgorithm(), getNeuralNetUpdater())
		End Function
		Protected Friend Overridable Function buildLearningBehavior() As ILearningBehavior(Of ACTION)
			Return LearningBehavior.builder(Of ACTION, EXPERIENCE_TYPE)().experienceHandler(getExperienceHandler()).updateRule(getUpdateRule()).build()
		End Function

		Protected Friend Overridable Sub resetForNewBuild()
			networks.resetForNewBuild()
			environment = environmentBuilder.build()
			transformProcess = transformProcessBuilder.build()
			policy = buildPolicy()
			experienceHandler = buildExperienceHandler()
			updateAlgorithm = buildUpdateAlgorithm()
			neuralNetUpdater = buildNeuralNetUpdater()
			updateRule = buildUpdateRule()
			learningBehavior = buildLearningBehavior()

			createdAgentLearnerCount += 1
		End Sub

		Protected Friend Overridable ReadOnly Property ThreadId As String
			Get
				Return "AgentLearner-" & createdAgentLearnerCount
			End Get
		End Property

		Protected Friend Overridable Function buildAgentLearner() As IAgentLearner(Of ACTION)
			Dim result As AgentLearner(Of ACTION) = New AgentLearner(getEnvironment(), getTransformProcess(), getPolicy(), configuration.getAgentLearnerConfiguration(), ThreadId, getLearningBehavior())
			If configuration.getAgentLearnerListeners() IsNot Nothing Then
				For Each listener As AgentListener(Of ACTION) In configuration.getAgentLearnerListeners()
					result.addListener(listener)
				Next listener
			End If

			Return result
		End Function

		''' <summary>
		''' Build a properly assembled / configured IAgentLearner. </summary>
		''' <returns> a <seealso cref="IAgentLearner"/> </returns>
		Public Overrides Function build() As IAgentLearner(Of ACTION)
			resetForNewBuild()
			Return buildAgentLearner()
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SuperBuilder @Data public static class Configuration<ACTION>
		Public Class Configuration(Of ACTION)
			''' <summary>
			''' The configuration that will be used to build the <seealso cref="AgentLearner"/>
			''' </summary>
			Friend agentLearnerConfiguration As AgentLearner.Configuration

			''' <summary>
			''' A list of <seealso cref="AgentListener AgentListeners"/> that will be added to the AgentLearner. (default = null; no listeners)
			''' </summary>
			Friend agentLearnerListeners As IList(Of AgentListener(Of ACTION))

			''' <summary>
			''' Tell the builder that the AgentLearners will be used in an asynchronous setup
			''' </summary>
			Friend asynchronous As Boolean
		End Class
	End Class

End Namespace