Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports NeuralNetUpdaterConfiguration = org.deeplearning4j.rl4j.agent.learning.update.updater.NeuralNetUpdaterConfiguration
Imports org.deeplearning4j.rl4j.network
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports RunWith = org.junit.runner.RunWith
Imports Mock = org.mockito.Mock
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
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

Namespace org.deeplearning4j.rl4j.agent.learning.update.updater.async

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) @Tag(TagNames.FILE_IO) @NativeTag public class AsyncGradientsNeuralNetUpdaterTest
	Public Class AsyncGradientsNeuralNetUpdaterTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ITrainableNeuralNet threadCurrentMock;
		Friend threadCurrentMock As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ITrainableNeuralNet globalCurrentMock;
		Friend globalCurrentMock As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock AsyncSharedNetworksUpdateHandler asyncSharedNetworksUpdateHandlerMock;
		Friend asyncSharedNetworksUpdateHandlerMock As AsyncSharedNetworksUpdateHandler

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingUpdate_expect_handlerCalledAndThreadCurrentUpdated()
		Public Overridable Sub when_callingUpdate_expect_handlerCalledAndThreadCurrentUpdated()
			' Arrange
			Dim configuration As NeuralNetUpdaterConfiguration = NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(2).build()
			Dim sut As New AsyncGradientsNeuralNetUpdater(threadCurrentMock, asyncSharedNetworksUpdateHandlerMock)
			Dim gradients As New Gradients(10)

			' Act
			sut.update(gradients)

			' Assert
			verify(asyncSharedNetworksUpdateHandlerMock, times(1)).handleGradients(gradients)
			verify(threadCurrentMock, never()).copyFrom(globalCurrentMock)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_synchronizeCurrentIsCalled_expect_synchronizeThreadCurrentWithGlobal()
		Public Overridable Sub when_synchronizeCurrentIsCalled_expect_synchronizeThreadCurrentWithGlobal()
			' Arrange
			Dim configuration As NeuralNetUpdaterConfiguration = NeuralNetUpdaterConfiguration.builder().build()
			Dim sut As New AsyncGradientsNeuralNetUpdater(threadCurrentMock, asyncSharedNetworksUpdateHandlerMock)
			[when](asyncSharedNetworksUpdateHandlerMock.getGlobalCurrent()).thenReturn(globalCurrentMock)

			' Act
			sut.synchronizeCurrent()

			' Assert
			verify(threadCurrentMock, times(1)).copyFrom(globalCurrentMock)
		End Sub
	End Class

End Namespace