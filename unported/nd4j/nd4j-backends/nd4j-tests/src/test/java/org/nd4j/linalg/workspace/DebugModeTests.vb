Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports DebugMode = org.nd4j.linalg.api.memory.enums.DebugMode
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports MirroringPolicy = org.nd4j.linalg.api.memory.enums.MirroringPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Nd4jWorkspace = org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace
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

Namespace org.nd4j.linalg.workspace

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.WORKSPACES) @NativeTag @Execution(ExecutionMode.SAME_THREAD) public class DebugModeTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class DebugModeTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void turnMeUp()
		Public Overridable Sub turnMeUp()
			Nd4j.WorkspaceManager.DebugMode = DebugMode.DISABLED
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void turnMeDown()
		Public Overridable Sub turnMeDown()
			Nd4j.WorkspaceManager.DebugMode = DebugMode.DISABLED
			Nd4j.MemoryManager.CurrentWorkspace = Nothing
			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDebugMode_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDebugMode_1(ByVal backend As Nd4jBackend)
			assertEquals(DebugMode.DISABLED, Nd4j.WorkspaceManager.DebugMode)

			Nd4j.WorkspaceManager.DebugMode = DebugMode.SPILL_EVERYTHING

			assertEquals(DebugMode.SPILL_EVERYTHING, Nd4j.WorkspaceManager.DebugMode)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpillMode_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpillMode_1(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DebugMode = DebugMode.SPILL_EVERYTHING

			Dim basicConfig As val = WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).maxSize(10 * 1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()

			Using ws As lombok.val = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "R_119_1993"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				assertEquals(10 * 1024 * 1024L, ws.getCurrentSize())
				assertEquals(0, ws.getDeviceOffset())
				assertEquals(0, ws.getPrimaryOffset())

				Dim array As val = Nd4j.create(DataType.DOUBLE, 10, 10).assign(1.0f)
				assertTrue(array.isAttached())

				' nothing should get into workspace
				assertEquals(0, ws.getPrimaryOffset())
				assertEquals(0, ws.getDeviceOffset())

				' array buffer should be spilled now
				assertEquals(10 * 10 * Nd4j.sizeOfDataType(DataType.DOUBLE), ws.getSpilledSize())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSpillMode_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSpillMode_2(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DebugMode = DebugMode.SPILL_EVERYTHING

			Dim basicConfig As val = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()

			Using ws As lombok.val = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "R_119_1992"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				assertEquals(0L, ws.getCurrentSize())
				assertEquals(0, ws.getDeviceOffset())
				assertEquals(0, ws.getPrimaryOffset())

				Dim array As val = Nd4j.create(DataType.DOUBLE, 10, 10).assign(1.0f)

				assertTrue(array.isAttached())

				' nothing should get into workspace
				assertEquals(0, ws.getPrimaryOffset())
				assertEquals(0, ws.getDeviceOffset())

				' array buffer should be spilled now
				assertEquals(10 * 10 * Nd4j.sizeOfDataType(DataType.DOUBLE), ws.getSpilledSize())
			End Using

			Using ws As lombok.val = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "R_119_1992"), org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace)
				assertEquals(0L, ws.getCurrentSize())
				assertEquals(0, ws.getDeviceOffset())
				assertEquals(0, ws.getPrimaryOffset())
				assertEquals(0, ws.getSpilledSize())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBypassMode_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBypassMode_1(ByVal backend As Nd4jBackend)
			Nd4j.WorkspaceManager.DebugMode = DebugMode.BYPASS_EVERYTHING

			Dim basicConfig As val = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()

			Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(basicConfig, "R_119_1994")

				Dim array As val = Nd4j.create(10, 10).assign(1.0f)
				assertFalse(array.isAttached())
			End Using
		End Sub
	End Class

End Namespace