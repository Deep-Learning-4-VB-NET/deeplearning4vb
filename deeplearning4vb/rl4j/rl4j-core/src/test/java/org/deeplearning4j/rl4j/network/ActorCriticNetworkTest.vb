Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
Imports FeaturesLabels = org.deeplearning4j.rl4j.agent.learning.update.FeaturesLabels
Imports Gradients = org.deeplearning4j.rl4j.agent.learning.update.Gradients
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertNotSame
import static org.junit.jupiter.api.Assertions.assertSame
Imports org.mockito.Mockito
import static org.mockito.Mockito.times

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
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class ActorCriticNetworkTest
	Public Class ActorCriticNetworkTest

		Private Function createFeaturesLabelsMock() As FeaturesLabels
			Dim featuresLabelsMock As FeaturesLabels = mock(GetType(FeaturesLabels))
			Dim features As New Features(New INDArray() { Nd4j.rand(1, 2) })
			[when](featuresLabelsMock.getFeatures()).thenReturn(features)

			Return featuresLabelsMock
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCtorWithCG_expect_handlerUsesCorrectLabelAndGradientNames()
		Public Overridable Sub when_callingCtorWithCG_expect_handlerUsesCorrectLabelAndGradientNames()
			' Arrange
			Dim modelMock As ComputationGraph = mock(GetType(ComputationGraph))
			Dim featuresLabelsMock As FeaturesLabels = createFeaturesLabelsMock()
			Dim gradientMock As Gradient = mock(GetType(Gradient))
			[when](modelMock.gradient()).thenReturn(gradientMock)

			' Act
			Dim sut As ActorCriticNetwork = ActorCriticNetwork.builder().withCombinedNetwork(modelMock).build()
			Dim results As Gradients = sut.computeGradients(featuresLabelsMock)

			' Assert
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Value)
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Policy)
			assertSame(gradientMock, results.getGradient(CommonGradientNames.ActorCritic.Combined))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCtorWithSeparateMLN_expect_handlerUsesCorrectLabelAndGradientNames()
		Public Overridable Sub when_callingCtorWithSeparateMLN_expect_handlerUsesCorrectLabelAndGradientNames()
			' Arrange
			Dim valueMock As MultiLayerNetwork = mock(GetType(MultiLayerNetwork))
			Dim valueGradientMock As Gradient = mock(GetType(Gradient))
			[when](valueMock.gradient()).thenReturn(valueGradientMock)

			Dim policyMock As MultiLayerNetwork = mock(GetType(MultiLayerNetwork))
			Dim policyGradientMock As Gradient = mock(GetType(Gradient))
			[when](policyMock.gradient()).thenReturn(policyGradientMock)

			Dim featuresLabelsMock As FeaturesLabels = createFeaturesLabelsMock()

			' Act
			Dim sut As ActorCriticNetwork = ActorCriticNetwork.builder().withSeparateNetworks(valueMock, policyMock).build()
			Dim results As Gradients = sut.computeGradients(featuresLabelsMock)

			' Assert
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Value)
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Policy)
			assertSame(valueGradientMock, results.getGradient(CommonGradientNames.ActorCritic.Value))
			assertSame(policyGradientMock, results.getGradient(CommonGradientNames.ActorCritic.Policy))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCtorWithSeparateMLNAndCG_expect_handlerUsesCorrectLabelAndGradientNames()
		Public Overridable Sub when_callingCtorWithSeparateMLNAndCG_expect_handlerUsesCorrectLabelAndGradientNames()
			' Arrange
			Dim valueMock As MultiLayerNetwork = mock(GetType(MultiLayerNetwork))
			Dim valueGradientMock As Gradient = mock(GetType(Gradient))
			[when](valueMock.gradient()).thenReturn(valueGradientMock)

			Dim policyMock As ComputationGraph = mock(GetType(ComputationGraph))
			Dim policyGradientMock As Gradient = mock(GetType(Gradient))
			[when](policyMock.gradient()).thenReturn(policyGradientMock)

			Dim featuresLabelsMock As FeaturesLabels = createFeaturesLabelsMock()

			' Act
			Dim sut As ActorCriticNetwork = ActorCriticNetwork.builder().withSeparateNetworks(valueMock, policyMock).build()
			Dim results As Gradients = sut.computeGradients(featuresLabelsMock)

			' Assert
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Value)
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Policy)
			assertSame(valueGradientMock, results.getGradient(CommonGradientNames.ActorCritic.Value))
			assertSame(policyGradientMock, results.getGradient(CommonGradientNames.ActorCritic.Policy))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCtorWithSeparateCGAndMLN_expect_handlerUsesCorrectLabelAndGradientNames()
		Public Overridable Sub when_callingCtorWithSeparateCGAndMLN_expect_handlerUsesCorrectLabelAndGradientNames()
			' Arrange
			Dim valueMock As ComputationGraph = mock(GetType(ComputationGraph))
			Dim valueGradientMock As Gradient = mock(GetType(Gradient))
			[when](valueMock.gradient()).thenReturn(valueGradientMock)

			Dim policyMock As MultiLayerNetwork = mock(GetType(MultiLayerNetwork))
			Dim policyGradientMock As Gradient = mock(GetType(Gradient))
			[when](policyMock.gradient()).thenReturn(policyGradientMock)

			Dim featuresLabelsMock As FeaturesLabels = createFeaturesLabelsMock()

			' Act
			Dim sut As ActorCriticNetwork = ActorCriticNetwork.builder().withSeparateNetworks(valueMock, policyMock).build()
			Dim results As Gradients = sut.computeGradients(featuresLabelsMock)

			' Assert
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Value)
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Policy)
			assertSame(valueGradientMock, results.getGradient(CommonGradientNames.ActorCritic.Value))
			assertSame(policyGradientMock, results.getGradient(CommonGradientNames.ActorCritic.Policy))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingCtorWithSeparateCG_expect_handlerUsesCorrectLabelAndGradientNames()
		Public Overridable Sub when_callingCtorWithSeparateCG_expect_handlerUsesCorrectLabelAndGradientNames()
			' Arrange
			Dim valueMock As ComputationGraph = mock(GetType(ComputationGraph))
			Dim valueGradientMock As Gradient = mock(GetType(Gradient))
			[when](valueMock.gradient()).thenReturn(valueGradientMock)

			Dim policyMock As ComputationGraph = mock(GetType(ComputationGraph))
			Dim policyGradientMock As Gradient = mock(GetType(Gradient))
			[when](policyMock.gradient()).thenReturn(policyGradientMock)

			Dim featuresLabelsMock As FeaturesLabels = createFeaturesLabelsMock()

			' Act
			Dim sut As ActorCriticNetwork = ActorCriticNetwork.builder().withSeparateNetworks(valueMock, policyMock).build()
			Dim results As Gradients = sut.computeGradients(featuresLabelsMock)

			' Assert
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Value)
			verify(featuresLabelsMock, times(1)).getLabels(CommonLabelNames.ActorCritic.Policy)
			assertSame(valueGradientMock, results.getGradient(CommonGradientNames.ActorCritic.Value))
			assertSame(policyGradientMock, results.getGradient(CommonGradientNames.ActorCritic.Policy))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingOutput_expect_resultHasCorrectNames()
		Public Overridable Sub when_callingOutput_expect_resultHasCorrectNames()
			' Arrange
			Dim modelMock As ComputationGraph = mock(GetType(ComputationGraph))
			Dim featuresData As INDArray = Nd4j.rand(1, 2)
			Dim features As New Features(New INDArray() { featuresData })
			Dim outputValue As INDArray = Nd4j.rand(1, 2)
			Dim outputPolicy As INDArray = Nd4j.rand(1, 2)
			[when](modelMock.output(featuresData)).thenReturn(New INDArray() { outputValue, outputPolicy })

			' Act
			Dim sut As ActorCriticNetwork = ActorCriticNetwork.builder().withCombinedNetwork(modelMock).build()
			Dim result As NeuralNetOutput = sut.output(features)

			' Assert
			assertSame(outputValue, result.get(CommonOutputNames.ActorCritic.Value))
			assertSame(outputPolicy, result.get(CommonOutputNames.ActorCritic.Policy))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingClone_expect_clonedActorCriticNetwork()
		Public Overridable Sub when_callingClone_expect_clonedActorCriticNetwork()
			' Arrange
			Dim modelMock As ComputationGraph = mock(GetType(ComputationGraph))
			[when](modelMock.clone()).thenReturn(modelMock)

			' Act
			Dim sut As ActorCriticNetwork = ActorCriticNetwork.builder().withCombinedNetwork(modelMock).build()
			Dim clone As ActorCriticNetwork = sut.clone()

			' Assert
			assertNotSame(sut, clone)
			assertNotSame(sut.getNetworkHandler(), clone.getNetworkHandler())
		End Sub

	End Class

End Namespace