Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertSame

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

Namespace org.deeplearning4j.rl4j.agent.learning.update

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class FeaturesTest
	Public Class FeaturesTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_creatingFeatureWithBatchSize10_expectGetBatchSizeReturn10()
		Public Overridable Sub when_creatingFeatureWithBatchSize10_expectGetBatchSizeReturn10()
			' Arrange
			Dim featuresData() As INDArray = {Nd4j.rand(10, 1)}

			' Act
			Dim sut As New Features(featuresData)

			' Assert
			assertEquals(10, sut.getBatchSize())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingGetWithAChannelIndex_expectGetReturnsThatChannelData()
		Public Overridable Sub when_callingGetWithAChannelIndex_expectGetReturnsThatChannelData()
			' Arrange
			Dim channel0Data As INDArray = Nd4j.rand(10, 1)
			Dim channel1Data As INDArray = Nd4j.rand(10, 1)
			Dim featuresData() As INDArray = { channel0Data, channel1Data }

			' Act
			Dim sut As New Features(featuresData)

			' Assert
			assertSame(channel1Data, sut.get(1))
		End Sub

	End Class

End Namespace