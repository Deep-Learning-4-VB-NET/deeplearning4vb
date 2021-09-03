Imports System.Threading
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend

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

Namespace org.nd4j.linalg.workspace

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.WORKSPACES) @NativeTag public class CyclicWorkspaceTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CyclicWorkspaceTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasicMechanics_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasicMechanics_1(ByVal backend As Nd4jBackend)
			Dim fShape As val = New Long(){128, 784}
			Dim lShape As val = New Long() {128, 10}
			Dim prefetchSize As val = 24
			Dim configuration As val = WorkspaceConfiguration.builder().minSize(10 * 1024L * 1024L).overallocationLimit(prefetchSize + 1).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyLearning(LearningPolicy.FIRST_LOOP).policyAllocation(AllocationPolicy.OVERALLOCATE).policySpill(SpillPolicy.REALLOCATE).build()

			For e As Integer = 0 To 99
				Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "randomNameHere" & 119)
					Dim fArray As val = Nd4j.create(fShape).assign(e)
					Dim lArray As val = Nd4j.create(lShape).assign(e)

	'                log.info("Current offset: {}; Current size: {};", ws.getCurrentOffset(), ws.getCurrentSize());
				End Using
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @SneakyThrows @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.LONG_TEST) @Tag(org.nd4j.common.tests.tags.TagNames.LARGE_RESOURCES) public void testGc(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testGc(ByVal backend As Nd4jBackend)
			Dim indArray As val = Nd4j.create(4, 4)
			indArray.putRow(0, Nd4j.create(New Single(){0, 2, -2, 0}))
			indArray.putRow(1, Nd4j.create(New Single(){0, 1, -1, 0}))
			indArray.putRow(2, Nd4j.create(New Single(){0, -1, 1, 0}))
			indArray.putRow(3, Nd4j.create(New Single(){0, -2, 2, 0}))

			For i As Integer = 0 To 99999999
				indArray.getRow(i Mod 3)
				Thread.Sleep(1)
			Next i
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace