Imports System.Collections.Generic
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports RnnOutputLayer = org.deeplearning4j.nn.layers.recurrent.RnnOutputLayer
Imports ComputationGraphUpdater = org.deeplearning4j.nn.updater.graph.ComputationGraphUpdater
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports ArgumentCaptor = org.mockito.ArgumentCaptor
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
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class ComputationGraphHandlerTest
	Public Class ComputationGraphHandlerTest

		Private Shared ReadOnly LABEL_NAMES() As String = {"TEST_LABEL"}
		Private Const GRADIENT_NAME As String = "TEST_GRADIENT"

		Private modelMock As ComputationGraph
		Private trainingListenerMock As TrainingListener
		Private configurationMock As ComputationGraphConfiguration

		Private sut As ComputationGraphHandler

		Public Overridable Sub setup(ByVal setupRecurrent As Boolean)
			modelMock = mock(GetType(ComputationGraph))
			trainingListenerMock = mock(GetType(TrainingListener))

			configurationMock = mock(GetType(ComputationGraphConfiguration))
			[when](configurationMock.getIterationCount()).thenReturn(123)
			[when](configurationMock.getEpochCount()).thenReturn(234)
			[when](modelMock.Configuration).thenReturn(configurationMock)

			If setupRecurrent Then
				[when](modelMock.getOutputLayer(0)).thenReturn(New RnnOutputLayer(Nothing, Nothing))
			End If

			sut = New ComputationGraphHandler(modelMock, LABEL_NAMES, GRADIENT_NAME, 1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingNotifyGradientCalculation_expect_listenersNotified()
		Public Overridable Sub when_callingNotifyGradientCalculation_expect_listenersNotified()
			' Arrange
			setup(False)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.Collection<org.deeplearning4j.optimize.api.TrainingListener> listeners = new java.util.ArrayList<org.deeplearning4j.optimize.api.TrainingListener>()
			Dim listeners As ICollection(Of TrainingListener) = New ArrayListAnonymousInnerClass(Me)
			[when](modelMock.getListeners()).thenReturn(listeners)

			' Act
			sut.notifyGradientCalculation()

			' Assert
			verify(trainingListenerMock, times(1)).onGradientCalculation(modelMock)
		End Sub

		Private Class ArrayListAnonymousInnerClass
			Inherits List(Of TrainingListener)

			Private ReadOnly outerInstance As ComputationGraphHandlerTest

			Public Sub New(ByVal outerInstance As ComputationGraphHandlerTest)
				Me.outerInstance = outerInstance

				Me.add(outerInstance.trainingListenerMock)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingNotifyIterationDone_expect_listenersNotified()
		Public Overridable Sub when_callingNotifyIterationDone_expect_listenersNotified()
			' Arrange
			setup(False)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.Collection<org.deeplearning4j.optimize.api.TrainingListener> listeners = new java.util.ArrayList<org.deeplearning4j.optimize.api.TrainingListener>()
			Dim listeners As ICollection(Of TrainingListener) = New ArrayListAnonymousInnerClass2(Me)
			[when](modelMock.getListeners()).thenReturn(listeners)

			' Act
			sut.notifyIterationDone()

			' Assert
			verify(trainingListenerMock, times(1)).iterationDone(modelMock, 123, 234)
		End Sub

		Private Class ArrayListAnonymousInnerClass2
			Inherits List(Of TrainingListener)

			Private ReadOnly outerInstance As ComputationGraphHandlerTest

			Public Sub New(ByVal outerInstance As ComputationGraphHandlerTest)
				Me.outerInstance = outerInstance

				Me.add(outerInstance.trainingListenerMock)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingPerformFit_expect_fitCalledOnModelWithCorrectLabels()
		Public Overridable Sub when_callingPerformFit_expect_fitCalledOnModelWithCorrectLabels()
			' Arrange
			setup(False)
			Dim featuresData As INDArray = Nd4j.rand(1, 2)
			Dim features As New Features(New INDArray() { Nd4j.rand(1, 2), featuresData })
			Dim labels As INDArray = Nd4j.rand(1, 2)
			Dim featuresLabels As New FeaturesLabels(features)
			featuresLabels.putLabels("TEST_LABEL", labels)

			' Act
			sut.performFit(featuresLabels)

			' Assert
			Dim featuresCaptor As ArgumentCaptor(Of INDArray()) = ArgumentCaptor.forClass(GetType(INDArray()))
			Dim labelsCaptor As ArgumentCaptor(Of INDArray()) = ArgumentCaptor.forClass(GetType(INDArray()))
			verify(modelMock, times(1)).fit(featuresCaptor.capture(), labelsCaptor.capture())
			Dim featuresArg As INDArray = featuresCaptor.getValue()(0)
			assertSame(featuresArg, featuresData)
			Dim labelsArg As INDArray = labelsCaptor.getValue()(0)
			assertSame(labelsArg, labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingperformGradientsComputation_expect_modelCalledWithCorrectFeaturesLabels()
		Public Overridable Sub when_callingperformGradientsComputation_expect_modelCalledWithCorrectFeaturesLabels()
			' Arrange
			setup(False)
			Dim featuresData As INDArray = Nd4j.rand(1, 2)
			Dim labels As INDArray = Nd4j.rand(1, 2)
			Dim features As New Features(New INDArray() { Nd4j.rand(1, 2), featuresData })
			Dim featuresLabels As New FeaturesLabels(features)
			featuresLabels.putLabels("TEST_LABEL", labels)

			' Act
			sut.performGradientsComputation(featuresLabels)

			' Assert
			Dim inputsCaptor As ArgumentCaptor(Of INDArray) = ArgumentCaptor.forClass(GetType(INDArray))
			verify(modelMock, times(1)).setInputs(inputsCaptor.capture())
			Dim inputsArg As INDArray = inputsCaptor.getValue()
			assertSame(featuresData, inputsArg)

			Dim labelsCaptor As ArgumentCaptor(Of INDArray) = ArgumentCaptor.forClass(GetType(INDArray))
			verify(modelMock, times(1)).setLabels(labelsCaptor.capture())
			Dim labelsArg As INDArray = labelsCaptor.getValue()
			assertSame(labels, labelsArg)

			verify(modelMock, times(1)).computeGradientAndScore()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingFillGradientsResponse_expect_gradientIsCorrectlyFilled()
		Public Overridable Sub when_callingFillGradientsResponse_expect_gradientIsCorrectlyFilled()
			' Arrange
			setup(False)
			Dim gradientsMock As Gradients = mock(GetType(Gradients))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.gradient.Gradient gradient = mock(org.deeplearning4j.nn.gradient.Gradient.class);
			Dim gradient As Gradient = mock(GetType(Gradient))
			[when](modelMock.gradient()).thenReturn(gradient)

			' Act
			sut.fillGradientsResponse(gradientsMock)

			' Assert
			verify(gradientsMock, times(1)).putGradient(GRADIENT_NAME, gradient)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingApplyGradient_expect_correctGradientAppliedAndIterationUpdated()
		Public Overridable Sub when_callingApplyGradient_expect_correctGradientAppliedAndIterationUpdated()
			' Arrange
			setup(False)
			Dim gradientsMock As Gradients = mock(GetType(Gradients))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.gradient.Gradient gradient = mock(org.deeplearning4j.nn.gradient.Gradient.class);
			Dim gradient As Gradient = mock(GetType(Gradient))
			Dim gradientGradient As INDArray = Nd4j.rand(1, 2)
			[when](gradient.gradient()).thenReturn(gradientGradient)
			[when](gradientsMock.getGradient(GRADIENT_NAME)).thenReturn(gradient)
			Dim updaterMock As ComputationGraphUpdater = mock(GetType(ComputationGraphUpdater))
			[when](modelMock.Updater).thenReturn(updaterMock)
			Dim paramsMock As INDArray = mock(GetType(INDArray))
			[when](modelMock.params()).thenReturn(paramsMock)

			' Act
			sut.applyGradient(gradientsMock, 345)

			' Assert
			verify(gradientsMock, times(1)).getGradient(GRADIENT_NAME)
			verify(updaterMock, times(1)).update(eq(gradient), eq(123), eq(234), eq(345), any())
			verify(paramsMock, times(1)).subi(gradientGradient)
			verify(configurationMock, times(1)).setIterationCount(124)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingRecurrentStepOutput_expect_recurrentStepCalledWithObservationData()
		Public Overridable Sub when_callingRecurrentStepOutput_expect_recurrentStepCalledWithObservationData()
			' Arrange
			setup(False)
			Dim observationMock As Observation = mock(GetType(Observation))
			Dim observationData As INDArray = Nd4j.rand(1, 2)
			[when](observationMock.getChannelData(1)).thenReturn(observationData)

			' Act
			sut.recurrentStepOutput(observationMock)

			' Assert
			verify(modelMock, times(1)).rnnTimeStep(observationData)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingFeaturesBatchOutput_expect_outputCalledWithBatch()
		Public Overridable Sub when_callingFeaturesBatchOutput_expect_outputCalledWithBatch()
			' Arrange
			setup(False)
			Dim channelData As INDArray = Nd4j.rand(1, 2)
			Dim features As New Features(New INDArray() { Nd4j.rand(1, 2), channelData })

			' Act
			sut.batchOutput(features)

			' Assert
			verify(modelMock, times(1)).output(New INDArray() { channelData })
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingResetState_expect_modelStateIsCleared()
		Public Overridable Sub when_callingResetState_expect_modelStateIsCleared()
			' Arrange
			setup(False)

			' Act
			sut.resetState()

			' Assert
			verify(modelMock, times(1)).rnnClearPreviousState()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingClone_expect_handlerAndModelIsCloned() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub when_callingClone_expect_handlerAndModelIsCloned()
			' Arrange
			setup(False)
			[when](modelMock.clone()).thenReturn(modelMock)

			' Act
			Dim result As ComputationGraphHandler = DirectCast(sut.clone(), ComputationGraphHandler)

			' Assert
			assertNotSame(sut, result)

			verify(modelMock, times(1)).clone()

			Dim privateField As System.Reflection.FieldInfo = GetType(ComputationGraphHandler).getDeclaredField("labelNames")
			privateField.setAccessible(True)
			Dim cloneLabelNames() As String = CType(privateField.get(sut), String())
			assertArrayEquals(cloneLabelNames, LABEL_NAMES)

			privateField = GetType(ComputationGraphHandler).getDeclaredField("gradientName")
			privateField.setAccessible(True)
			Dim cloneGradientName As String = CStr(privateField.get(sut))
			assertEquals(cloneGradientName, GRADIENT_NAME)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCopyFrom_expect_modelParamsAreCopiedToModel()
		Public Overridable Sub when_callingCopyFrom_expect_modelParamsAreCopiedToModel()
			' Arrange
			setup(False)
			Dim params As INDArray = Nd4j.rand(1, 2)
			[when](modelMock.params()).thenReturn(params)
			Dim from As New ComputationGraphHandler(modelMock, Nothing, Nothing, 0)

			' Act
			sut.copyFrom(from)

			' Assert
			verify(modelMock, times(1)).setParams(params)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_modelIsNotRecurrent_expect_isRecurrentFalse()
		Public Overridable Sub when_modelIsNotRecurrent_expect_isRecurrentFalse()
			' Arrange
			setup(False)

			' Act
			Dim isRecurrent As Boolean = sut.Recurrent

			' Assert
			assertFalse(isRecurrent)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_modelIsRecurrent_expect_isRecurrentTrue()
		Public Overridable Sub when_modelIsRecurrent_expect_isRecurrentTrue()
			' Arrange
			setup(True)

			' Act
			Dim isRecurrent As Boolean = sut.Recurrent

			' Assert
			assertTrue(isRecurrent)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_creatingWithMapper_expect_computationsUseTheMapper()
		Public Overridable Sub when_creatingWithMapper_expect_computationsUseTheMapper()
			' Arrange
			Dim channelToNetworkInputMapperMock As ChannelToNetworkInputMapper = mock(GetType(ChannelToNetworkInputMapper))
			modelMock = mock(GetType(ComputationGraph))
			trainingListenerMock = mock(GetType(TrainingListener))
			Dim featuresData As INDArray = Nd4j.rand(1, 2)
			Dim features As New Features(New INDArray() { Nd4j.rand(1, 2), featuresData })
			Dim labels As INDArray = Nd4j.rand(1, 2)
			Dim featuresLabels As New FeaturesLabels(features)
			featuresLabels.putLabels("TEST_LABEL", labels)
			sut = New ComputationGraphHandler(modelMock, LABEL_NAMES, GRADIENT_NAME, channelToNetworkInputMapperMock)

			' Act
			sut.performGradientsComputation(featuresLabels)

			' Assert
			verify(channelToNetworkInputMapperMock, times(1)).getNetworkInputs(features)
		End Sub

	End Class
End Namespace