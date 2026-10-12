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

Namespace org.deeplearning4j.rl4j.agent.learning.update.updater.sync

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) @Tag(TagNames.FILE_IO) @NativeTag public class SyncGradientsNeuralNetUpdaterTest
	Public Class SyncGradientsNeuralNetUpdaterTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ITrainableNeuralNet currentMock;
		Friend currentMock As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock ITrainableNeuralNet targetMock;
		Friend targetMock As ITrainableNeuralNet

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingUpdate_expect_currentUpdatedAndtargetNotChanged()
		Public Overridable Sub when_callingUpdate_expect_currentUpdatedAndtargetNotChanged()
			' Arrange
			Dim configuration As NeuralNetUpdaterConfiguration = NeuralNetUpdaterConfiguration.builder().build()
			Dim sut As New SyncGradientsNeuralNetUpdater(currentMock, targetMock, configuration)
			Dim gradients As New Gradients(10)

			' Act
			sut.update(gradients)

			' Assert
			verify(currentMock, times(1)).applyGradients(gradients)
			verify(targetMock, never()).applyGradients(any())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingUpdate_expect_targetUpdatedFromCurrentAtFrequency()
		Public Overridable Sub when_callingUpdate_expect_targetUpdatedFromCurrentAtFrequency()
			' Arrange
			Dim configuration As NeuralNetUpdaterConfiguration = NeuralNetUpdaterConfiguration.builder().targetUpdateFrequency(3).build()
			Dim sut As New SyncGradientsNeuralNetUpdater(currentMock, targetMock, configuration)
			Dim gradients As New Gradients(10)

			' Act
			sut.update(gradients)
			sut.update(gradients)
			sut.update(gradients)

			' Assert
			verify(currentMock, never()).copyFrom(any())
			verify(targetMock, times(1)).copyFrom(currentMock)
		End Sub
	End Class

End Namespace