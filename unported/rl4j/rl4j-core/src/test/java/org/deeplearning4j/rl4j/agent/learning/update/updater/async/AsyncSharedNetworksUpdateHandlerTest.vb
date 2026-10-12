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
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.junit.jupiter.api.Assertions.fail
import static org.mockito.ArgumentMatchers.any
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
'ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) @Tag(TagNames.FILE_IO) @NativeTag public class AsyncSharedNetworksUpdateHandlerTest
	Public Class AsyncSharedNetworksUpdateHandlerTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ITrainableNeuralNet globalCurrentMock;
		Friend globalCurrentMock As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ITrainableNeuralNet targetMock;
		Friend targetMock As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_handleGradientsIsCalledWithoutTarget_expect_gradientsAppliedOnGlobalCurrent()
		Public Overridable Sub when_handleGradientsIsCalledWithoutTarget_expect_gradientsAppliedOnGlobalCurrent()
			' Arrange
			Dim configuration As NeuralNetUpdaterConfiguration = NeuralNetUpdaterConfiguration.builder().build()
			Dim sut As New AsyncSharedNetworksUpdateHandler(globalCurrentMock, configuration)
			Dim gradients As New Gradients(10)

			' Act
			sut.handleGradients(gradients)

			' Assert
			verify(globalCurrentMock, times(1)).applyGradients(gradients)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_handleGradientsIsCalledWithTarget_expect_gradientsAppliedOnGlobalCurrentAndTargetUpdated()
		Public Overridable Sub when_handleGradientsIsCalledWithTarget_expect_gradientsAppliedOnGlobalCurrentAndTargetUpdated()
			' Arrange
			Dim configuration As NeuralNetUpdaterConfiguration = NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(2).build()
			Dim sut As New AsyncSharedNetworksUpdateHandler(globalCurrentMock, targetMock, configuration)
			Dim gradients As New Gradients(10)

			' Act
			sut.handleGradients(gradients)
			sut.handleGradients(gradients)

			' Assert
			verify(globalCurrentMock, times(2)).applyGradients(gradients)
			verify(targetMock, times(1)).copyFrom(globalCurrentMock)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_configurationHasInvalidFrequency_expect_Exception()
		Public Overridable Sub when_configurationHasInvalidFrequency_expect_Exception()
			Try
				Dim configuration As NeuralNetUpdaterConfiguration = NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(0).build()
				Dim sut As New AsyncSharedNetworksUpdateHandler(globalCurrentMock, targetMock, configuration)

				fail("NullPointerException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "Configuration: targetUpdateFrequency must be greater than 0, got:  [0]"
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

	End Class

End Namespace