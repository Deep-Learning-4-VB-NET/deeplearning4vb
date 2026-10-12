Imports Features = org.deeplearning4j.rl4j.agent.learning.update.Features
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
Imports org.junit.jupiter.api.Assertions

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
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class ChannelToNetworkInputMapperTest
	Public Class ChannelToNetworkInputMapperTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_mapIsEmpty_expect_exception()
		Public Overridable Sub when_mapIsEmpty_expect_exception()
			Try
				Dim tempVar As New ChannelToNetworkInputMapper(New ChannelToNetworkInputMapper.NetworkInputToChannelBinding(){}, New String() { "TEST" }, New String () { "TEST" })
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "networkInputsToChannelNameMap is empty."
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_networkInputNamesIsEmpty_expect_exception()
		Public Overridable Sub when_networkInputNamesIsEmpty_expect_exception()
			Try
				Dim map() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST", "TEST") }
				Dim tempVar As New ChannelToNetworkInputMapper(map, New String(){}, New String () { "TEST" })
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "networkInputNames is empty."
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_channelNamesIsEmpty_expect_exception()
		Public Overridable Sub when_channelNamesIsEmpty_expect_exception()
			Try
				Dim map() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST", "TEST") }
				Dim tempVar As New ChannelToNetworkInputMapper(map, New String () { "TEST" }, New String(){})
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "channelNames is empty."
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_notAllInputsAreMapped_expect_exception()
		Public Overridable Sub when_notAllInputsAreMapped_expect_exception()
			Try
				Dim map() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST", "TEST") }
				Dim tempVar As New ChannelToNetworkInputMapper(map, New String () { "TEST", "NOT-MAPPED" }, New String () { "TEST" })
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "All network inputs must be mapped exactly once. Input 'NOT-MAPPED' is mapped 0 times."
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_anInputIsMappedMultipleTimes_expect_exception()
		Public Overridable Sub when_anInputIsMappedMultipleTimes_expect_exception()
			Try
				Dim map() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST", "TEST"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST1", "TEST"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST1", "TEST") }
				Dim tempVar As New ChannelToNetworkInputMapper(map, New String () { "TEST", "TEST1" }, New String () { "TEST" })
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "All network inputs must be mapped exactly once. Input 'TEST1' is mapped 2 times."
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_aMapInputDoesNotExist_expect_exception()
		Public Overridable Sub when_aMapInputDoesNotExist_expect_exception()
			Try
				Dim map() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST", "TEST"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST1", "TEST")}
				Dim tempVar As New ChannelToNetworkInputMapper(map, New String () { "TEST" }, New String () { "TEST" })
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "'TEST1' not found in networkInputNames"
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_aMapFeatureDoesNotExist_expect_exception()
		Public Overridable Sub when_aMapFeatureDoesNotExist_expect_exception()
			Try
				Dim map() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST", "TEST"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("TEST1", "TEST1")}
				Dim tempVar As New ChannelToNetworkInputMapper(map, New String () { "TEST", "TEST1" }, New String () { "TEST" })
				fail("IllegalArgumentException should have been thrown")
			Catch exception As System.ArgumentException
				Dim expectedMessage As String = "'TEST1' not found in channelNames"
				Dim actualMessage As String = exception.Message

				assertTrue(actualMessage.Contains(expectedMessage))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingObservationGetNetworkInputs_expect_aCorrectlyOrderedINDArrayArray()
		Public Overridable Sub when_callingObservationGetNetworkInputs_expect_aCorrectlyOrderedINDArrayArray()
			' ARRANGE
			Dim map() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("IN-1", "FEATURE-1"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("IN-2", "FEATURE-2"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("IN-3", "FEATURE-3")}
			Dim networkInputs() As String = { "IN-1", "IN-2", "IN-3" }
			Dim channelNames() As String = { "FEATURE-1", "FEATURE-2", "FEATURE-UNUSED", "FEATURE-3" }
			Dim sut As New ChannelToNetworkInputMapper(map, networkInputs, channelNames)
			Dim feature1 As INDArray = Nd4j.rand(1, 2)
			Dim feature2 As INDArray = Nd4j.rand(1, 2)
			Dim featureUnused As INDArray = Nd4j.rand(1, 2)
			Dim feature3 As INDArray = Nd4j.rand(1, 2)
			Dim observation As New Observation(New INDArray() { feature1, feature2, featureUnused, feature3 })

			' ACT
			Dim results() As INDArray = sut.getNetworkInputs(observation)

			' ASSERT
			assertSame(feature1, results(0))
			assertSame(feature2, results(1))
			assertSame(feature3, results(2))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingFeaturesGetNetworkInputs_expect_aCorrectlyOrderedINDArrayArray()
		Public Overridable Sub when_callingFeaturesGetNetworkInputs_expect_aCorrectlyOrderedINDArrayArray()
			' ARRANGE
			Dim map() As ChannelToNetworkInputMapper.NetworkInputToChannelBinding = { ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("IN-1", "FEATURE-1"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("IN-2", "FEATURE-2"), ChannelToNetworkInputMapper.NetworkInputToChannelBinding.map("IN-3", "FEATURE-3")}
			Dim networkInputs() As String = { "IN-1", "IN-2", "IN-3" }
			Dim channelNames() As String = { "FEATURE-1", "FEATURE-2", "FEATURE-UNUSED", "FEATURE-3" }
			Dim sut As New ChannelToNetworkInputMapper(map, networkInputs, channelNames)
			Dim feature1 As INDArray = Nd4j.rand(1, 2)
			Dim feature2 As INDArray = Nd4j.rand(1, 2)
			Dim featureUnused As INDArray = Nd4j.rand(1, 2)
			Dim feature3 As INDArray = Nd4j.rand(1, 2)
			Dim features As New Features(New INDArray() { feature1, feature2, featureUnused, feature3 })

			' ACT
			Dim results() As INDArray = sut.getNetworkInputs(features)

			' ASSERT
			assertSame(feature1, results(0))
			assertSame(feature2, results(1))
			assertSame(feature3, results(2))
		End Sub

	End Class

End Namespace