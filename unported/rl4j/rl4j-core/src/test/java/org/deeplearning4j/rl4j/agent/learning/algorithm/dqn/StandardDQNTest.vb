Imports System.Collections.Generic
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports org.deeplearning4j.rl4j.experience
Imports CommonLabelNames = org.deeplearning4j.rl4j.network.CommonLabelNames
Imports CommonOutputNames = org.deeplearning4j.rl4j.network.CommonOutputNames
Imports IOutputNeuralNet = org.deeplearning4j.rl4j.network.IOutputNeuralNet
Imports NeuralNetOutput = org.deeplearning4j.rl4j.network.NeuralNetOutput
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports Mock = org.mockito.Mock
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.mockito.ArgumentMatchers.any
import static org.mockito.Mockito.when

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

Namespace org.deeplearning4j.rl4j.agent.learning.algorithm.dqn


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class StandardDQNTest
	Public Class StandardDQNTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IOutputNeuralNet qNetworkMock;
		Friend qNetworkMock As IOutputNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock IOutputNeuralNet targetQNetworkMock;
		Friend targetQNetworkMock As IOutputNeuralNet

		Private ReadOnly configuration As BaseTransitionTDAlgorithm.Configuration = BaseTransitionTDAlgorithm.Configuration.builder().gamma(0.5).build()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void setup()
		Public Overridable Sub setup()
			[when](qNetworkMock.output(any(GetType(Features)))).thenAnswer(Function(i)
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.QValues, i.getArgument(0, GetType(Features)).get(0))
			Return result
			End Function)
			[when](targetQNetworkMock.output(any(GetType(Features)))).thenAnswer(Function(i)
			Dim result As New NeuralNetOutput()
			result.put(CommonOutputNames.QValues, i.getArgument(0, GetType(Features)).get(0))
			Return result
			End Function)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_isTerminal_expect_rewardValueAtIdx0()
		Public Overridable Sub when_isTerminal_expect_rewardValueAtIdx0()

			' Assemble
			Dim stateActionRewardStates As IList(Of StateActionRewardState(Of Integer)) = New ArrayListAnonymousInnerClass(Me)

			Dim sut As New StandardDQN(qNetworkMock, targetQNetworkMock, configuration)

			' Act
			Dim result As FeaturesLabels = sut.compute(stateActionRewardStates)

			' Assert
			Dim evaluatedQValues As INDArray = result.getLabels(CommonLabelNames.QValues)
			assertEquals(1.0, evaluatedQValues.getDouble(0, 0), 0.0001)
			assertEquals(2.2, evaluatedQValues.getDouble(0, 1), 0.0001)
		End Sub

		Private Class ArrayListAnonymousInnerClass
			Inherits List(Of StateActionRewardState(Of Integer))

			Private ReadOnly outerInstance As StandardDQNTest

			Public Sub New(ByVal outerInstance As StandardDQNTest)
				Me.outerInstance = outerInstance

				Me.add(outerInstance.buildTransition(outerInstance.buildObservation(New Double(){1.1, 2.2}), 0, 1.0, True, outerInstance.buildObservation(New Double(){11.0, 22.0})))
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_isNotTerminal_expect_rewardPlusEstimatedQValue()
		Public Overridable Sub when_isNotTerminal_expect_rewardPlusEstimatedQValue()

			' Assemble
			Dim stateActionRewardStates As IList(Of StateActionRewardState(Of Integer)) = New ArrayListAnonymousInnerClass2(Me)

			Dim sut As New org.deeplearning4j.rl4j.agent.learning.algorithm.dqn.StandardDQN(qNetworkMock, targetQNetworkMock, configuration)

			' Act
			Dim result As FeaturesLabels = sut.compute(stateActionRewardStates)

			' Assert
			Dim evaluatedQValues As INDArray = result.getLabels(CommonLabelNames.QValues)
			assertEquals(1.0 + 0.5 * 22.0, evaluatedQValues.getDouble(0, 0), 0.0001)
			assertEquals(2.2, evaluatedQValues.getDouble(0, 1), 0.0001)
		End Sub

		Private Class ArrayListAnonymousInnerClass2
			Inherits List(Of StateActionRewardState(Of Integer))

			Private ReadOnly outerInstance As StandardDQNTest

			Public Sub New(ByVal outerInstance As StandardDQNTest)
				Me.outerInstance = outerInstance

				Me.add(outerInstance.buildTransition(outerInstance.buildObservation(New Double(){1.1, 2.2}), 0, 1.0, False, outerInstance.buildObservation(New Double(){11.0, 22.0})))
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_batchHasMoreThanOne_expect_everySampleEvaluated()
		Public Overridable Sub when_batchHasMoreThanOne_expect_everySampleEvaluated()

			' Assemble
			Dim stateActionRewardStates As IList(Of StateActionRewardState(Of Integer)) = New ArrayListAnonymousInnerClass3(Me)

			Dim sut As New StandardDQN(qNetworkMock, targetQNetworkMock, configuration)

			' Act
			Dim result As FeaturesLabels = sut.compute(stateActionRewardStates)

			' Assert
			Dim evaluatedQValues As INDArray = result.getLabels(CommonLabelNames.QValues)
			assertEquals((1.0 + 0.5 * 22.0), evaluatedQValues.getDouble(0, 0), 0.0001)
			assertEquals(2.2, evaluatedQValues.getDouble(0, 1), 0.0001)

			assertEquals(3.3, evaluatedQValues.getDouble(1, 0), 0.0001)
			assertEquals((2.0 + 0.5 * 44.0), evaluatedQValues.getDouble(1, 1), 0.0001)

			assertEquals(3.0, evaluatedQValues.getDouble(2, 0), 0.0001) ' terminal: reward only
			assertEquals(6.6, evaluatedQValues.getDouble(2, 1), 0.0001)

		End Sub

		Private Class ArrayListAnonymousInnerClass3
			Inherits List(Of StateActionRewardState(Of Integer))

			Private ReadOnly outerInstance As StandardDQNTest

			Public Sub New(ByVal outerInstance As StandardDQNTest)
				Me.outerInstance = outerInstance

				Me.add(outerInstance.buildTransition(outerInstance.buildObservation(New Double(){1.1, 2.2}), 0, 1.0, False, outerInstance.buildObservation(New Double(){11.0, 22.0})))
				Me.add(outerInstance.buildTransition(outerInstance.buildObservation(New Double(){3.3, 4.4}), 1, 2.0, False, outerInstance.buildObservation(New Double(){33.0, 44.0})))
				Me.add(outerInstance.buildTransition(outerInstance.buildObservation(New Double(){5.5, 6.6}), 0, 3.0, True, outerInstance.buildObservation(New Double(){55.0, 66.0})))
			End Sub

		End Class

		Private Function buildObservation(ByVal data() As Double) As Observation
			Return New Observation(Nd4j.create(data).reshape(ChrW(1), 2))
		End Function

		Private Function buildTransition(ByVal observation As Observation, ByVal action As Integer?, ByVal reward As Double, ByVal isTerminal As Boolean, ByVal nextObservation As Observation) As StateActionRewardState(Of Integer)
			Dim result As New StateActionRewardState(Of Integer)(observation, action, reward, isTerminal)
			result.setNextObservation(nextObservation)

			Return result
		End Function
	End Class

End Namespace