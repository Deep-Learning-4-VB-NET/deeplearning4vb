Imports Builder = org.apache.commons.lang3.builder.Builder
Imports org.deeplearning4j.rl4j.agent
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports org.deeplearning4j.rl4j.agent.learning.update.updater
Imports org.deeplearning4j.rl4j.environment
Imports org.deeplearning4j.rl4j.experience
Imports org.deeplearning4j.rl4j.network
Imports TransformProcess = org.deeplearning4j.rl4j.observation.transform.TransformProcess
Imports org.deeplearning4j.rl4j.policy
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports Mock = org.mockito.Mock
Imports Mockito = org.mockito.Mockito
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports org.mockito.Mockito

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) public class BaseAgentLearnerBuilderTest
	Public Class BaseAgentLearnerBuilderTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock BaseAgentLearnerBuilder.Configuration configuration;
		Friend configuration As BaseAgentLearnerBuilder.Configuration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ITrainableNeuralNet neuralNet;
		Friend neuralNet As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Builder<org.deeplearning4j.rl4j.environment.Environment<Integer>> environmentBuilder;
		Friend environmentBuilder As Builder(Of Environment(Of Integer))

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Builder<org.deeplearning4j.rl4j.observation.transform.TransformProcess> transformProcessBuilder;
		Friend transformProcessBuilder As Builder(Of TransformProcess)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IUpdateAlgorithm updateAlgorithmMock;
		Friend updateAlgorithmMock As IUpdateAlgorithm

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock INeuralNetUpdater neuralNetUpdaterMock;
		Friend neuralNetUpdaterMock As INeuralNetUpdater

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ExperienceHandler experienceHandlerMock;
		Friend experienceHandlerMock As ExperienceHandler

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock Environment environmentMock;
		Friend environmentMock As Environment

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IPolicy policyMock;
		Friend policyMock As IPolicy

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock TransformProcess transformProcessMock;
		Friend transformProcessMock As TransformProcess

		Friend sut As BaseAgentLearnerBuilder

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()
			sut = mock(GetType(BaseAgentLearnerBuilder), Mockito.withSettings().useConstructor(configuration, neuralNet, environmentBuilder, transformProcessBuilder).defaultAnswer(Mockito.CALLS_REAL_METHODS))

			Dim agentLearnerConfiguration As AgentLearner.Configuration = AgentLearner.Configuration.builder().maxEpisodeSteps(200).build()

			[when](sut.buildUpdateAlgorithm()).thenReturn(updateAlgorithmMock)
			[when](sut.buildNeuralNetUpdater()).thenReturn(neuralNetUpdaterMock)
			[when](sut.buildExperienceHandler()).thenReturn(experienceHandlerMock)
			[when](environmentBuilder.build()).thenReturn(environmentMock)
			[when](transformProcessBuilder.build()).thenReturn(transformProcessMock)
			[when](sut.buildPolicy()).thenReturn(policyMock)
			[when](configuration.getAgentLearnerConfiguration()).thenReturn(agentLearnerConfiguration)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_buildingAgentLearner_expect_dependenciesAndAgentLearnerIsBuilt()
		Public Overridable Sub when_buildingAgentLearner_expect_dependenciesAndAgentLearnerIsBuilt()
			' Arrange

			' Act
			sut.build()

			' Assert
			verify(environmentBuilder, times(1)).build()
			verify(transformProcessBuilder, times(1)).build()
			verify(sut, times(1)).buildPolicy()
			verify(sut, times(1)).buildExperienceHandler()
			verify(sut, times(1)).buildUpdateAlgorithm()
			verify(sut, times(1)).buildNeuralNetUpdater()
			verify(sut, times(1)).buildAgentLearner()
		End Sub

	End Class

End Namespace