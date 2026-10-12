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
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AllocationsTracker = org.nd4j.linalg.api.memory.AllocationsTracker
Imports DeviceAllocationsTracker = org.nd4j.linalg.api.memory.DeviceAllocationsTracker
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationKind = org.nd4j.linalg.api.memory.enums.AllocationKind
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.linalg.memory

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @NativeTag @Tag(TagNames.WORKSPACES) public class AccountingTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class AccountingTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDetached_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDetached_1(ByVal backend As Nd4jBackend)
			Dim array As val = Nd4j.createFromArray(1, 2, 3, 4, 5)
			assertEquals(DataType.INT, array.dataType())

			assertTrue(Nd4j.MemoryManager.allocatedMemory(0) > 0L)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDetached_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDetached_2(ByVal backend As Nd4jBackend)
			Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()

			Dim before As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			Dim array As val = Nd4j.createFromArray(1, 2, 3, 4, 5, 6, 7)
			assertEquals(DataType.INT, array.dataType())

			Dim after As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			assertTrue(after > before)
			assertTrue(AllocationsTracker.Instance.bytesOnDevice(AllocationKind.CONSTANT, Nd4j.AffinityManager.getDeviceForCurrentThread()) > 0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWorkspaceAccounting_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWorkspaceAccounting_1(ByVal backend As Nd4jBackend)
			Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()
			Dim wsConf As val = WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.FIRST_LOOP).build()

			Dim before As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			Dim workspace As val = Nd4j.WorkspaceManager.createNewWorkspace(wsConf, "random_name_here")

			Dim middle As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			workspace.destroyWorkspace(True)

			Dim after As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			log.info("Before: {}; Middle: {}; After: {}", before, middle, after)
			assertTrue(middle > before)
			assertTrue(after < middle)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWorkspaceAccounting_2(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testWorkspaceAccounting_2(ByVal backend As Nd4jBackend)
			Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()
			Dim wsConf As val = WorkspaceConfiguration.builder().initialSize(0).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.OVER_TIME).cyclesBeforeInitialization(3).build()

			Dim before As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			Dim middle1 As Long = 0
			Using workspace As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(wsConf, "random_name_here")
				Dim array As val = Nd4j.create(DataType.DOUBLE, 5, 5)
				middle1 = Nd4j.MemoryManager.allocatedMemory(deviceId)
			End Using

			Dim middle2 As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()

			Dim after As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			log.info("Before: {}; Middle1: {}; Middle2: {}; After: {}", before, middle1, middle2, after)
			assertTrue(middle1 > before)
			assertTrue(after < middle1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testManualDeallocation_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testManualDeallocation_1(ByVal backend As Nd4jBackend)
			Dim deviceId As val = Nd4j.AffinityManager.getDeviceForCurrentThread()
			Dim before As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			Dim array As val = Nd4j.createFromArray(New SByte() {1, 2, 3})

			Dim middle As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			array.close()

			Dim after As val = Nd4j.MemoryManager.allocatedMemory(deviceId)

			assertTrue(middle > before)

			' <= here just because possible cache activation
			assertTrue(after <= middle)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testTracker_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testTracker_1(ByVal backend As Nd4jBackend)
			Dim tracker As val = New DeviceAllocationsTracker()

			For Each e As val In System.Enum.GetValues(GetType(AllocationKind))
				For v As Integer = 1 To 100
					tracker.updateState(e, v)
				Next v

				assertNotEquals(0, tracker.getState(e))
			Next e

			For Each e As val In System.Enum.GetValues(GetType(AllocationKind))
				For v As Integer = 1 To 100
					tracker.updateState(e, -v)
				Next v

				assertEquals(0, tracker.getState(e))
			Next e
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace