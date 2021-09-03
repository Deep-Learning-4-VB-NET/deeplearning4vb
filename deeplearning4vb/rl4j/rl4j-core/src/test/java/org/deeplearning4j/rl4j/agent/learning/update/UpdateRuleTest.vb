Imports System
Imports System.Collections.Generic
Imports org.deeplearning4j.rl4j.agent.learning.algorithm
Imports org.deeplearning4j.rl4j.agent.learning.update.updater
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
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
import static org.junit.jupiter.api.Assertions.assertEquals
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

Namespace org.deeplearning4j.rl4j.agent.learning.update


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ExtendWith(MockitoExtension.class) @Tag(TagNames.FILE_IO) @NativeTag public class UpdateRuleTest
	Public Class UpdateRuleTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock private org.deeplearning4j.rl4j.agent.learning.algorithm.IUpdateAlgorithm<FeaturesLabels, Integer> updateAlgorithm;
		Private updateAlgorithm As IUpdateAlgorithm(Of FeaturesLabels, Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Mock private org.deeplearning4j.rl4j.agent.learning.update.updater.INeuralNetUpdater<FeaturesLabels> updater;
		Private updater As INeuralNetUpdater(Of FeaturesLabels)

		Private sut As UpdateRule(Of FeaturesLabels, Integer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void init()
		Public Overridable Sub init()
			sut = New UpdateRule(Of FeaturesLabels, Integer)(updateAlgorithm, updater)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingUpdate_expect_computeAndUpdateNetwork()
		Public Overridable Sub when_callingUpdate_expect_computeAndUpdateNetwork()
			' Arrange
			Dim trainingBatch As IList(Of Integer) = New ArrayListAnonymousInnerClass(Me)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final FeaturesLabels computeResult = new FeaturesLabels(null);
			Dim computeResult As New FeaturesLabels(Nothing)
			[when](updateAlgorithm.compute(any())).thenReturn(computeResult)

			' Act
			sut.update(trainingBatch)

			' Assert
			verify(updateAlgorithm, times(1)).compute(trainingBatch)
			verify(updater, times(1)).update(computeResult)
		End Sub

		Private Class ArrayListAnonymousInnerClass
			Inherits List(Of Integer)

			Private ReadOnly outerInstance As UpdateRuleTest

			Public Sub New(ByVal outerInstance As UpdateRuleTest)
				Me.outerInstance = outerInstance

				Convert.ToInt32(1)
				Convert.ToInt32(2)
			End Sub

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingUpdate_expect_updateCountIncremented()
		Public Overridable Sub when_callingUpdate_expect_updateCountIncremented()
			' Arrange

			' Act
			sut.update(Nothing)
			Dim updateCountBefore As Integer = sut.UpdateCount
			sut.update(Nothing)
			Dim updateCountAfter As Integer = sut.UpdateCount

			' Assert
			assertEquals(updateCountBefore + 1, updateCountAfter)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void when_callingNotifyNewBatchStarted_expect_synchronizeCurrentCalled()
		Public Overridable Sub when_callingNotifyNewBatchStarted_expect_synchronizeCurrentCalled()
			' Arrange

			' Act
			sut.notifyNewBatchStarted()

			' Assert
			verify(updater, times(1)).synchronizeCurrent()
		End Sub

	End Class

End Namespace