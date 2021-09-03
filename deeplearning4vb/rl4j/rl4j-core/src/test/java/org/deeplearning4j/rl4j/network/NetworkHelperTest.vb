Imports System.Collections.Generic
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Observation = org.deeplearning4j.rl4j.observation.Observation
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
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.junit.jupiter.api.Assertions.fail
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
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class NetworkHelperTest
	Public Class NetworkHelperTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithMapper_expect_correctlyBuiltNetworkHandler()
		Public Overridable Sub when_callingBuildHandlerWithMapper_expect_correctlyBuiltNetworkHandler()
			' Arrange
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<String> networkInputs = java.util.Arrays.asList("INPUT1", "INPUT2", "INPUT3");
			Dim networkInputs As IList(Of String) = New List(Of String) From {"INPUT1", "INPUT2", "INPUT3"}
			Dim configurationMock As ComputationGraphConfiguration = mock(GetType(ComputationGraphConfiguration))
			[when](configurationMock.getNetworkInputs()).thenReturn(networkInputs)

			Dim modelMock As ComputationGraph = mock(GetType(ComputationGraph))
			[when](modelMock.Configuration).thenReturn(configurationMock)

			Dim networkInputsToChannelNameMap() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("INPUT1", "CN2"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("INPUT2", "CN3"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("INPUT3", "CN1")}
			Dim channelNames() As String = { "CN1", "CN2", "CN3" }
			Dim labelNames() As String = { "LN1", "LN2", "LN3" }
			Dim sut As New NetworkHelper()

			' Act
			Dim handler As INetworkHandler = sut.buildHandler(modelMock, networkInputsToChannelNameMap, channelNames, labelNames, "GRADIENT")

			' Assert
			Dim channel1 As INDArray = Nd4j.rand(1, 2)
			Dim channel2 As INDArray = Nd4j.rand(1, 2)
			Dim channel3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
			handler.stepOutput(observation)

			verify(modelMock, times(1)).output(channel2, channel3, channel1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithComputationGraphAndEmptyChannelName_expect_networkHandlerWithFirstInputBoundToFirstChannel()
		Public Overridable Sub when_callingBuildHandlerWithComputationGraphAndEmptyChannelName_expect_networkHandlerWithFirstInputBoundToFirstChannel()
			' Arrange
			Dim modelMock As ComputationGraph = mock(GetType(ComputationGraph))

			Dim channelNames() As String = { "CN1", "CN2", "CN3" }
			Dim labelNames() As String = { "LN1", "LN2", "LN3" }
			Dim sut As New NetworkHelper()

			' Act
			Dim handler As INetworkHandler = sut.buildHandler(modelMock, "", channelNames, labelNames, "GRADIENT")

			' Assert
			Dim channel1 As INDArray = Nd4j.rand(1, 2)
			Dim channel2 As INDArray = Nd4j.rand(1, 2)
			Dim channel3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
			handler.stepOutput(observation)

			verify(modelMock, times(1)).output(channel1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithMLNAndEmptyChannelName_expect_networkHandlerWithFirstInputBoundToFirstChannel()
		Public Overridable Sub when_callingBuildHandlerWithMLNAndEmptyChannelName_expect_networkHandlerWithFirstInputBoundToFirstChannel()
			' Arrange
			Dim modelMock As MultiLayerNetwork = mock(GetType(MultiLayerNetwork))

			Dim channelNames() As String = { "CN1", "CN2", "CN3" }
			Dim sut As New NetworkHelper()

			' Act
			Dim handler As INetworkHandler = sut.buildHandler(modelMock, "", channelNames, "LABEL", "GRADIENT")

			' Assert
			Dim channel1 As INDArray = Nd4j.rand(1, 2)
			Dim channel2 As INDArray = Nd4j.rand(1, 2)
			Dim channel3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
			handler.stepOutput(observation)

			verify(modelMock, times(1)).output(channel1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithComputationGraphAndNullChannelNames_expect_networkHandlerWithFirstInputBoundToFirstChannel()
		Public Overridable Sub when_callingBuildHandlerWithComputationGraphAndNullChannelNames_expect_networkHandlerWithFirstInputBoundToFirstChannel()
			' Arrange
			Dim modelMock As ComputationGraph = mock(GetType(ComputationGraph))

			Dim labelNames() As String = { "LN1", "LN2", "LN3" }
			Dim sut As New NetworkHelper()

			' Act
			Dim handler As INetworkHandler = sut.buildHandler(modelMock, "CN2", Nothing, labelNames, "GRADIENT")

			' Assert
			Dim channel1 As INDArray = Nd4j.rand(1, 2)
			Dim channel2 As INDArray = Nd4j.rand(1, 2)
			Dim channel3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
			handler.stepOutput(observation)

			verify(modelMock, times(1)).output(channel1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithMLNAndNullChannelNames_expect_networkHandlerWithFirstInputBoundToFirstChannel()
		Public Overridable Sub when_callingBuildHandlerWithMLNAndNullChannelNames_expect_networkHandlerWithFirstInputBoundToFirstChannel()
			' Arrange
			Dim modelMock As MultiLayerNetwork = mock(GetType(MultiLayerNetwork))

			Dim sut As New NetworkHelper()

			' Act
			Dim handler As INetworkHandler = sut.buildHandler(modelMock, "CN2", Nothing, "LABEL", "GRADIENT")

			' Assert
			Dim channel1 As INDArray = Nd4j.rand(1, 2)
			Dim channel2 As INDArray = Nd4j.rand(1, 2)
			Dim channel3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
			handler.stepOutput(observation)

			verify(modelMock, times(1)).output(channel1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithComputationGraphAndEmptyChannelNames_expect_networkHandlerWithFirstInputBoundToFirstChannel()
		Public Overridable Sub when_callingBuildHandlerWithComputationGraphAndEmptyChannelNames_expect_networkHandlerWithFirstInputBoundToFirstChannel()
			' Arrange
			Dim modelMock As ComputationGraph = mock(GetType(ComputationGraph))

			Dim labelNames() As String = { "LN1", "LN2", "LN3" }
			Dim sut As New NetworkHelper()

			' Act
			Dim handler As INetworkHandler = sut.buildHandler(modelMock, "CN2", New String(){}, labelNames, "GRADIENT")

			' Assert
			Dim channel1 As INDArray = Nd4j.rand(1, 2)
			Dim channel2 As INDArray = Nd4j.rand(1, 2)
			Dim channel3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
			handler.stepOutput(observation)

			verify(modelMock, times(1)).output(channel1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithMLNAndEmptyChannelNames_expect_networkHandlerWithFirstInputBoundToFirstChannel()
		Public Overridable Sub when_callingBuildHandlerWithMLNAndEmptyChannelNames_expect_networkHandlerWithFirstInputBoundToFirstChannel()
			' Arrange
			Dim modelMock As MultiLayerNetwork = mock(GetType(MultiLayerNetwork))

			Dim sut As New NetworkHelper()

			' Act
			Dim handler As INetworkHandler = sut.buildHandler(modelMock, "CN2", New String(){}, "LABEL", "GRADIENT")

			' Assert
			Dim channel1 As INDArray = Nd4j.rand(1, 2)
			Dim channel2 As INDArray = Nd4j.rand(1, 2)
			Dim channel3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
			handler.stepOutput(observation)

			verify(modelMock, times(1)).output(channel1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithComputationGraphAndSpecificChannelName_expect_networkHandlerWithFirstInputBoundToThatChannel()
		Public Overridable Sub when_callingBuildHandlerWithComputationGraphAndSpecificChannelName_expect_networkHandlerWithFirstInputBoundToThatChannel()
			' Arrange
			Dim modelMock As ComputationGraph = mock(GetType(ComputationGraph))

			Dim channelNames() As String = { "CN1", "CN2", "CN3" }
			Dim labelNames() As String = { "LN1", "LN2", "LN3" }
			Dim sut As New NetworkHelper()

			' Act
			Dim handler As INetworkHandler = sut.buildHandler(modelMock, "CN2", channelNames, labelNames, "GRADIENT")

			' Assert
			Dim channel1 As INDArray = Nd4j.rand(1, 2)
			Dim channel2 As INDArray = Nd4j.rand(1, 2)
			Dim channel3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
			handler.stepOutput(observation)

			verify(modelMock, times(1)).output(channel2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithMLNAndSpecificChannelName_expect_networkHandlerWithFirstInputBoundToThatChannel()
		Public Overridable Sub when_callingBuildHandlerWithMLNAndSpecificChannelName_expect_networkHandlerWithFirstInputBoundToThatChannel()
			' Arrange
			Dim modelMock As MultiLayerNetwork = mock(GetType(MultiLayerNetwork))

			Dim channelNames() As String = { "CN1", "CN2", "CN3" }
			Dim sut As New NetworkHelper()

			' Act
			Dim handler As INetworkHandler = sut.buildHandler(modelMock, "CN2", channelNames, "LABEL", "GRADIENT")

			' Assert
			Dim channel1 As INDArray = Nd4j.rand(1, 2)
			Dim channel2 As INDArray = Nd4j.rand(1, 2)
			Dim channel3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
			handler.stepOutput(observation)

			verify(modelMock, times(1)).output(channel2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithComputationGraphAndUnknownChannelName_expect_networkHandlerWithFirstInputBoundToThatChannel()
		Public Overridable Sub when_callingBuildHandlerWithComputationGraphAndUnknownChannelName_expect_networkHandlerWithFirstInputBoundToThatChannel()
			Try
				' Arrange
				Dim modelMock As ComputationGraph = mock(GetType(ComputationGraph))

				Dim channelNames() As String = {"CN1", "CN2", "CN3"}
				Dim labelNames() As String = {"LN1", "LN2", "LN3"}
				Dim sut As New NetworkHelper()

				' Act
				Dim handler As INetworkHandler = sut.buildHandler(modelMock, "UNKNOWN", channelNames, labelNames, "GRADIENT")

				' Assert
				Dim channel1 As INDArray = Nd4j.rand(1, 2)
				Dim channel2 As INDArray = Nd4j.rand(1, 2)
				Dim channel3 As INDArray = Nd4j.rand(1, 2)
				Dim observation As New Observation(New INDArray(){channel1, channel2, channel3})
				handler.stepOutput(observation)
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "The channel 'UNKNOWN' was not found in channelNames."
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingBuildHandlerWithMLNAndUnknownChannelName_expect_networkHandlerWithFirstInputBoundToThatChannel()
		Public Overridable Sub when_callingBuildHandlerWithMLNAndUnknownChannelName_expect_networkHandlerWithFirstInputBoundToThatChannel()
			Try
				' Arrange
				Dim modelMock As MultiLayerNetwork = mock(GetType(MultiLayerNetwork))

				Dim channelNames() As String = { "CN1", "CN2", "CN3" }
				Dim sut As New NetworkHelper()

				' Act
				Dim handler As INetworkHandler = sut.buildHandler(modelMock, "UNKNOWN", channelNames, "LABEL", "GRADIENT")

				' Assert
				Dim channel1 As INDArray = Nd4j.rand(1, 2)
				Dim channel2 As INDArray = Nd4j.rand(1, 2)
				Dim channel3 As INDArray = Nd4j.rand(1, 2)
				Dim observation As New Observation(New INDArray() { channel1, channel2, channel3})
				handler.stepOutput(observation)
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "The channel 'UNKNOWN' was not found in channelNames."
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

	End Class

End Namespace