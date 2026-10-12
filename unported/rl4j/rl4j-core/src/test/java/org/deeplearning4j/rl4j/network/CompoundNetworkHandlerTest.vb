Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
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
Imports org.junit.jupiter.api.Assertions
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
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class CompoundNetworkHandlerTest
	Public Class CompoundNetworkHandlerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock INetworkHandler handler1;
		Friend handler1 As INetworkHandler

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock INetworkHandler handler2;
		Friend handler2 As INetworkHandler

		Private sut As CompoundNetworkHandler

		Public Overridable Sub setup(ByVal setupRecurrent As Boolean)
			[when](handler1.Recurrent).thenReturn(setupRecurrent)
			[when](handler2.Recurrent).thenReturn(False)

			sut = New CompoundNetworkHandler(handler1, handler2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingNotifyGradientCalculation_expect_listenersNotified()
		Public Overridable Sub when_callingNotifyGradientCalculation_expect_listenersNotified()
			' Arrange
			setup(False)

			' Act
			sut.notifyGradientCalculation()

			' Assert
			verify(handler1, times(1)).notifyGradientCalculation()
			verify(handler2, times(1)).notifyGradientCalculation()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingNotifyIterationDone_expect_listenersNotified()
		Public Overridable Sub when_callingNotifyIterationDone_expect_listenersNotified()
			' Arrange
			setup(False)

			' Act
			sut.notifyIterationDone()

			' Assert
			verify(handler1, times(1)).notifyIterationDone()
			verify(handler2, times(1)).notifyIterationDone()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingPerformFit_expect_performFitIsCalledOnHandlders()
		Public Overridable Sub when_callingPerformFit_expect_performFitIsCalledOnHandlders()
			' Arrange
			setup(False)
			Dim featuresLabels As New FeaturesLabels(Nothing)

			' Act
			sut.performFit(featuresLabels)

			' Assert
			verify(handler1, times(1)).performFit(featuresLabels)
			verify(handler2, times(1)).performFit(featuresLabels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingPerformGradientsComputation_expect_performGradientsComputationIsCalledOnHandlers()
		Public Overridable Sub when_callingPerformGradientsComputation_expect_performGradientsComputationIsCalledOnHandlers()
			' Arrange
			setup(False)
			Dim featuresLabels As New FeaturesLabels(Nothing)

			' Act
			sut.performGradientsComputation(featuresLabels)

			' Assert
			verify(handler1, times(1)).performGradientsComputation(featuresLabels)
			verify(handler2, times(1)).performGradientsComputation(featuresLabels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingFillGradientsResponse_expect_fillGradientsResponseIsCalledOnHandlers()
		Public Overridable Sub when_callingFillGradientsResponse_expect_fillGradientsResponseIsCalledOnHandlers()
			' Arrange
			setup(False)
			Dim gradientsMock As Gradients = mock(GetType(Gradients))

			' Act
			sut.fillGradientsResponse(gradientsMock)

			' Assert
			verify(handler1, times(1)).fillGradientsResponse(gradientsMock)
			verify(handler2, times(1)).fillGradientsResponse(gradientsMock)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingApplyGradient_expect_correctGradientAppliedAndIterationUpdated()
		Public Overridable Sub when_callingApplyGradient_expect_correctGradientAppliedAndIterationUpdated()
			' Arrange
			setup(False)
			Dim gradientsMock As Gradients = mock(GetType(Gradients))

			' Act
			sut.applyGradient(gradientsMock, 345)

			' Assert
			verify(handler1, times(1)).applyGradient(gradientsMock, 345)
			verify(handler2, times(1)).applyGradient(gradientsMock, 345)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingRecurrentStepOutput_expect_recurrentStepCalledWithObservationData()
		Public Overridable Sub when_callingRecurrentStepOutput_expect_recurrentStepCalledWithObservationData()
			' Arrange
			setup(False)
			Dim observationMock As Observation = mock(GetType(Observation))
			Dim recurrentStepOutput1() As Double = { 1.0, 2.0, 3.0}
			Dim recurrentStepOutput2() As Double = { 10.0, 20.0, 30.0}
			[when](handler1.recurrentStepOutput(observationMock)).thenReturn(New INDArray() { Nd4j.create(recurrentStepOutput1) })
			[when](handler2.recurrentStepOutput(observationMock)).thenReturn(New INDArray() { Nd4j.create(recurrentStepOutput2) })

			' Act
			Dim results() As INDArray = sut.recurrentStepOutput(observationMock)

			' Assert
			verify(handler1, times(1)).recurrentStepOutput(observationMock)
			verify(handler2, times(1)).recurrentStepOutput(observationMock)
			assertEquals(2, results.Length)
			assertArrayEquals(results(0).toDoubleVector(), recurrentStepOutput1, 0.00001)
			assertArrayEquals(results(1).toDoubleVector(), recurrentStepOutput2, 0.00001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingFeaturesBatchOutput_expect_outputCalledWithBatch()
		Public Overridable Sub when_callingFeaturesBatchOutput_expect_outputCalledWithBatch()
			' Arrange
			setup(False)
			Dim batch As INDArray = Nd4j.rand(1, 2)
			Dim features As New Features(New INDArray() { batch })
			[when](handler1.batchOutput(features)).thenReturn(New INDArray() { batch.mul(2.0) })
			[when](handler2.batchOutput(features)).thenReturn(New INDArray() { batch.div(2.0) })

			' Act
			Dim results() As INDArray = sut.batchOutput(features)

			' Assert
			verify(handler1, times(1)).batchOutput(features)
			verify(handler2, times(1)).batchOutput(features)
			assertEquals(2, results.Length)
			assertArrayEquals(results(0).toDoubleVector(), batch.mul(2.0).toDoubleVector(), 0.00001)
			assertArrayEquals(results(1).toDoubleVector(), batch.div(2.0).toDoubleVector(), 0.00001)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingResetState_expect_recurrentHandlersAreReset()
		Public Overridable Sub when_callingResetState_expect_recurrentHandlersAreReset()
			' Arrange
			setup(True)

			' Act
			sut.resetState()

			' Assert
			verify(handler1, times(1)).resetState()
			verify(handler2, never()).resetState()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingClone_expect_handlersAreCloned() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub when_callingClone_expect_handlersAreCloned()
			' Arrange
			setup(False)
			[when](handler1.clone()).thenReturn(handler1)
			[when](handler2.clone()).thenReturn(handler2)


			' Act
			Dim result As CompoundNetworkHandler = DirectCast(sut.clone(), CompoundNetworkHandler)

			' Assert
			assertNotSame(sut, result)

			verify(handler1, times(1)).clone()
			verify(handler2, times(1)).clone()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCopyFrom_expect_handlersParamsAreCopied()
		Public Overridable Sub when_callingCopyFrom_expect_handlersParamsAreCopied()
			' Arrange
			setup(False)
			Dim from As New CompoundNetworkHandler(handler1, handler2)

			' Act
			sut.copyFrom(from)

			' Assert
			verify(handler1, times(1)).copyFrom(handler1)
			verify(handler2, times(1)).copyFrom(handler2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_noHandlerIsRecurrent_expect_isRecurrentFalse()
		Public Overridable Sub when_noHandlerIsRecurrent_expect_isRecurrentFalse()
			' Arrange
			setup(False)

			' Act
			Dim isRecurrent As Boolean = sut.Recurrent

			' Assert
			assertFalse(isRecurrent)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_aHandlerIsRecurrent_expect_isRecurrentTrue()
		Public Overridable Sub when_aHandlerIsRecurrent_expect_isRecurrentTrue()
			' Arrange
			setup(True)

			' Act
			Dim isRecurrent As Boolean = sut.Recurrent

			' Assert
			assertTrue(isRecurrent)
		End Sub
	End Class
End Namespace