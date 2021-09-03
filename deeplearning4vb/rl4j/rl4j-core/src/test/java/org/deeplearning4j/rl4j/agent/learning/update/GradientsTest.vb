Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports RunWith = org.junit.runner.RunWith
Imports MockitoJUnitRunner = org.mockito.junit.MockitoJUnitRunner
Imports MockitoExtension = org.mockito.junit.jupiter.MockitoExtension
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertSame
import static org.mockito.Mockito.mock

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
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class GradientsTest
	Public Class GradientsTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_getBatchSizeIsCalled_expect_batchSizeIsReturned()
		Public Overridable Sub when_getBatchSizeIsCalled_expect_batchSizeIsReturned()
			' Arrange
			Dim sut As New Gradients(5)

			' Act
			Dim batchSize As Long = sut.getBatchSize()

			' Assert
			assertEquals(5, batchSize)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_puttingLabels_expect_getLabelReturnsLabels()
		Public Overridable Sub when_puttingLabels_expect_getLabelReturnsLabels()
			' Arrange
			Dim gradient As Gradient = mock(GetType(Gradient))
			Dim sut As New Gradients(5)
			sut.putGradient("test", gradient)

			' Act
			Dim result As Gradient = sut.getGradient("test")

			' Assert
			assertSame(gradient, result)
		End Sub
	End Class

End Namespace