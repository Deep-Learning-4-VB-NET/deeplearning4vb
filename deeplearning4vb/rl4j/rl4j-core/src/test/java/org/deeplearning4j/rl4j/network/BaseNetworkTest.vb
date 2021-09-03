Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Test = org.junit.jupiter.api.Test
Imports RunWith = org.junit.runner.RunWith
Imports ArgumentCaptor = org.mockito.ArgumentCaptor
Imports Mock = org.mockito.Mock
Imports Mockito = org.mockito.Mockito
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertSame
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

Namespace org.deeplearning4j.rl4j.network

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) public class BaseNetworkTest
	Public Class BaseNetworkTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock INetworkHandler handlerMock;
		Friend handlerMock As INetworkHandler

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock NeuralNetOutput neuralNetOutputMock;
		Friend neuralNetOutputMock As NeuralNetOutput

		Private sut As BaseNetwork(Of BaseNetwork)

		Public Overridable Sub setup(ByVal setupRecurrent As Boolean)
			[when](handlerMock.Recurrent).thenReturn(setupRecurrent)
			sut = mock(GetType(BaseNetwork), Mockito.withSettings().useConstructor(handlerMock).defaultAnswer(Mockito.CALLS_REAL_METHODS))
			[when](sut.packageResult(any())).thenReturn(neuralNetOutputMock)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingIsRecurrent_expect_handlerIsCalled()
		Public Overridable Sub when_callingIsRecurrent_expect_handlerIsCalled()
			' Arrange
			setup(False)

			' Act
			sut.Recurrent

			' Assert
			verify(handlerMock, times(1)).isRecurrent()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingFit_expect_handlerIsCalled()
		Public Overridable Sub when_callingFit_expect_handlerIsCalled()
			' Arrange
			setup(False)
			Dim featuresLabels As New FeaturesLabels(Nothing)

			' Act
			sut.fit(featuresLabels)

			' Assert
			verify(handlerMock, times(1)).performFit(featuresLabels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingComputeGradients_expect_handlerComputeGradientsIsNotifiedAndResponseIsFilled()
		Public Overridable Sub when_callingComputeGradients_expect_handlerComputeGradientsIsNotifiedAndResponseIsFilled()
			' Arrange
			setup(False)
			Dim featuresMock As Features = mock(GetType(Features))
			[when](featuresMock.getBatchSize()).thenReturn(12L)
			Dim featuresLabels As New FeaturesLabels(featuresMock)

			' Act
			Dim response As Gradients = sut.computeGradients(featuresLabels)

			' Assert
			verify(handlerMock, times(1)).performGradientsComputation(featuresLabels)
			verify(handlerMock, times(1)).notifyGradientCalculation()
			verify(handlerMock, times(1)).fillGradientsResponse(response)
			assertEquals(12, response.getBatchSize())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingApplyGradients_expect_handlerAppliesGradientAndIsNotified()
		Public Overridable Sub when_callingApplyGradients_expect_handlerAppliesGradientAndIsNotified()
			' Arrange
			setup(False)
			Dim gradientsMock As Gradients = mock(GetType(Gradients))
			[when](gradientsMock.getBatchSize()).thenReturn(12L)

			' Act
			sut.applyGradients(gradientsMock)

			' Assert
			verify(handlerMock, times(1)).applyGradient(gradientsMock, 12L)
			verify(handlerMock, times(1)).notifyIterationDone()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOutputOnNonRecurrentNetworkAndNotInCache_expect_nonRecurrentOutputIsReturned()
		Public Overridable Sub when_callingOutputOnNonRecurrentNetworkAndNotInCache_expect_nonRecurrentOutputIsReturned()
			' Arrange
			setup(False)
			Dim observation As New Observation(Nd4j.rand(1, 2))
			Dim batchOutputResult() As INDArray = { Nd4j.rand(1, 2) }
			[when](handlerMock.stepOutput(observation)).thenReturn(batchOutputResult)

			' Act
			sut.output(observation)

			' Assert
			verify(handlerMock, times(1)).stepOutput(observation)
			verify(sut, times(1)).packageResult(batchOutputResult)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOutputOnRecurrentNetworkAndNotInCache_expect_nonRecurrentOutputIsReturned()
		Public Overridable Sub when_callingOutputOnRecurrentNetworkAndNotInCache_expect_nonRecurrentOutputIsReturned()
			' Arrange
			setup(True)
			Dim observation As New Observation(Nd4j.rand(1, 2))
			Dim batchOutputResult() As INDArray = { Nd4j.rand(1, 2) }
			[when](handlerMock.recurrentStepOutput(observation)).thenReturn(batchOutputResult)

			' Act
			sut.output(observation)

			' Assert
			verify(handlerMock, times(1)).recurrentStepOutput(observation)
			verify(sut, times(1)).packageResult(batchOutputResult)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOutput_expect_nonRecurrentOutputIsReturned()
		Public Overridable Sub when_callingOutput_expect_nonRecurrentOutputIsReturned()
			' Arrange
			setup(False)
			Dim featuresData As INDArray = Nd4j.rand(1, 2)
			Dim features As New Features(New INDArray() { featuresData })
			Dim batchOutputResult() As INDArray = { Nd4j.rand(1, 2) }
			[when](handlerMock.batchOutput(features)).thenReturn(batchOutputResult)

			' Act
			sut.output(features)

			' Assert
			Dim captor As ArgumentCaptor(Of Features) = ArgumentCaptor.forClass(GetType(Features))
			verify(handlerMock, times(1)).batchOutput(captor.capture())
			Dim resultData As INDArray = captor.getValue().get(0)
			assertSame(featuresData, resultData)

			verify(sut, times(1)).packageResult(batchOutputResult)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingResetOnNonRecurrent_expect_handlerNotCalled()
		Public Overridable Sub when_callingResetOnNonRecurrent_expect_handlerNotCalled()
			' Arrange
			setup(False)

			' Act
			sut.reset()

			' Assert
			verify(handlerMock, never()).resetState()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingResetOnRecurrent_expect_handlerIsCalled()
		Public Overridable Sub when_callingResetOnRecurrent_expect_handlerIsCalled()
			' Arrange
			setup(True)

			' Act
			sut.reset()

			' Assert
			verify(handlerMock, times(1)).resetState()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCopyFrom_expect_handlerIsCalled()
		Public Overridable Sub when_callingCopyFrom_expect_handlerIsCalled()
			' Arrange
			setup(False)

			' Act
			sut.copyFrom(sut)

			' Assert
			verify(handlerMock, times(1)).copyFrom(handlerMock)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingFit_expect_CacheInvalidated()
		Public Overridable Sub when_callingFit_expect_CacheInvalidated()
			' Arrange
			setup(False)
			Dim observation As New Observation(Nd4j.rand(1, 2))
			Dim batchOutputResult() As INDArray = { Nd4j.rand(1, 2) }

			' Act
			sut.output(observation)
			sut.fit(Nothing)
			sut.output(observation)

			' Assert
			' Note: calling batchOutput twice means BaseNetwork.fit() has cleared the cache
			verify(handlerMock, times(2)).stepOutput(observation)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingApplyGradients_expect_CacheInvalidated()
		Public Overridable Sub when_callingApplyGradients_expect_CacheInvalidated()
			' Arrange
			setup(False)
			Dim observation As New Observation(Nd4j.rand(1, 2))
			Dim batchOutputResult() As INDArray = { Nd4j.rand(1, 2) }

			' Act
			sut.output(observation)
			sut.fit(Nothing)
			sut.output(observation)

			' Assert
			' Note: calling batchOutput twice means BaseNetwork.fit() has cleared the cache
			verify(handlerMock, times(2)).stepOutput(observation)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOutputWithoutClearingCache_expect_CacheInvalidated()
		Public Overridable Sub when_callingOutputWithoutClearingCache_expect_CacheInvalidated()
			' Arrange
			setup(False)
			Dim gradientsMock As Gradients = mock(GetType(Gradients))
			[when](gradientsMock.getBatchSize()).thenReturn(12L)
			Dim observation As New Observation(Nd4j.rand(1, 2))
			Dim batchOutputResult() As INDArray = { Nd4j.rand(1, 2) }

			' Act
			sut.output(observation)
			sut.applyGradients(gradientsMock)
			sut.output(observation)

			' Assert
			' Note: calling batchOutput twice means BaseNetwork.applyGradients() has cleared the cache
			verify(handlerMock, times(2)).stepOutput(observation)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingReset_expect_CacheInvalidated()
		Public Overridable Sub when_callingReset_expect_CacheInvalidated()
			' Arrange
			setup(False)
			Dim observation As New Observation(Nd4j.rand(1, 2))
			Dim batchOutputResult() As INDArray = { Nd4j.rand(1, 2) }

			' Act
			sut.output(observation)
			sut.reset()
			sut.output(observation)

			' Assert
			' Note: calling batchOutput twice means BaseNetwork.reset() has cleared the cache
			verify(handlerMock, times(2)).stepOutput(observation)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCopyFrom_expect_CacheInvalidated()
		Public Overridable Sub when_callingCopyFrom_expect_CacheInvalidated()
			' Arrange
			setup(False)
			Dim observation As New Observation(Nd4j.rand(1, 2))
			Dim batchOutputResult() As INDArray = { Nd4j.rand(1, 2) }


			' Act
			sut.output(observation)
			sut.copyFrom(sut)
			sut.output(observation)

			' Assert
			' Note: calling batchOutput twice means BaseNetwork.reset() has cleared the cache
			verify(handlerMock, times(2)).stepOutput(observation)
		End Sub

	End Class

End Namespace