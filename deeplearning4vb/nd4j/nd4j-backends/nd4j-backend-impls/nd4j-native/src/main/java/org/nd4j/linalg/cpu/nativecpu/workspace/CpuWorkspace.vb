Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports LongPointer = org.bytedeco.javacpp.LongPointer
Imports AllocationsTracker = org.nd4j.linalg.api.memory.AllocationsTracker
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationKind = org.nd4j.linalg.api.memory.enums.AllocationKind
Imports LocationPolicy = org.nd4j.linalg.api.memory.enums.LocationPolicy
Imports MemoryKind = org.nd4j.linalg.api.memory.enums.MemoryKind
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
Imports PointersPair = org.nd4j.linalg.api.memory.pointers.PointersPair
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jWorkspace = org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace
Imports Deallocatable = org.nd4j.linalg.api.memory.Deallocatable
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder

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

Namespace org.nd4j.linalg.cpu.nativecpu.workspace


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CpuWorkspace extends org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace implements org.nd4j.linalg.api.memory.Deallocatable
	Public Class CpuWorkspace
		Inherits Nd4jWorkspace
		Implements Deallocatable

		Protected Friend mmap As LongPointer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CpuWorkspace(@NonNull WorkspaceConfiguration configuration)
		Public Sub New(ByVal configuration As WorkspaceConfiguration)
			MyBase.New(configuration)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CpuWorkspace(@NonNull WorkspaceConfiguration configuration, @NonNull String workspaceId)
		Public Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String)
			MyBase.New(configuration, workspaceId)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CpuWorkspace(@NonNull WorkspaceConfiguration configuration, @NonNull String workspaceId, System.Nullable<Integer> deviceId)
		Public Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String, ByVal deviceId As Integer?)
			MyBase.New(configuration, workspaceId)
			Me.deviceId = deviceId
		End Sub


		Public Overrides ReadOnly Property UniqueId As String Implements Deallocatable.getUniqueId
			Get
				Return "Workspace_" & Id & "_" & Nd4j.DeallocatorService.nextValue()
			End Get
		End Property

		Public Overrides Function deallocator() As Deallocator
	'        
	'        return new Deallocator() {
	'            @Override
	'            public void deallocate() {
	'                log.info("Deallocator invoked!");
	'            }
	'        };
	'        
			 Return New CpuWorkspaceDeallocator(Me)
		End Function

		Public Overrides Function targetDevice() As Integer Implements Deallocatable.targetDevice
			Return 0
		End Function

		Protected Friend Overrides Sub init()
			MyBase.init()

			If workspaceConfiguration.getPolicyLocation() = LocationPolicy.RAM Then

				If currentSize_Conflict.get() > 0 Then
					isInit.set(True)

					If isDebug.get() Then
						log.info("Allocating [{}] workspace of {} bytes...", id, currentSize_Conflict.get())
					End If

					workspace.setHostPointer(New PagedPointer(memoryManager.allocate(currentSize_Conflict.get() + SAFETY_OFFSET, MemoryKind.HOST, True)))
					AllocationsTracker.Instance.markAllocated(AllocationKind.WORKSPACE, 0, currentSize_Conflict.get() + SAFETY_OFFSET)
				End If
			ElseIf workspaceConfiguration.getPolicyLocation() = LocationPolicy.MMAP Then
				Dim flen As Long = tempFile.length()
				mmap = NativeOpsHolder.Instance.getDeviceNativeOps().mmapFile(Nothing, tempFile.getAbsolutePath(), flen)

				If mmap Is Nothing Then
					Throw New Exception("MMAP failed")
				End If

				workspace.setHostPointer(New PagedPointer(mmap.get(0)))
			End If
		End Sub

		Protected Friend Overrides Sub clearPinnedAllocations(ByVal extended As Boolean)
			If isDebug.get() Then
				log.info("Workspace [{}] device_{} threadId {} cycle {}: clearing pinned allocations...", id, Nd4j.AffinityManager.getDeviceForCurrentThread(), Thread.CurrentThread.getId(), cyclesCount_Conflict.get())
			End If

			Do While pinnedAllocations.Count > 0
				Dim pair As PointersPair = pinnedAllocations.First.Value
				If pair Is Nothing Then
					Throw New Exception()
				End If

				Dim stepNumber As Long = pair.getAllocationCycle()
				Dim stepCurrent As Long = stepsCount.get()

				If isDebug.get() Then
					log.info("Allocation step: {}; Current step: {}", stepNumber, stepCurrent)
				End If

				If stepNumber + 2 < stepCurrent OrElse extended Then
					pinnedAllocations.RemoveFirst()

					NativeOpsHolder.Instance.getDeviceNativeOps().freeHost(pair.getHostPointer())

					pinnedCount.decrementAndGet()
					pinnedAllocationsSize.addAndGet(pair.getRequiredMemory() * -1)
				Else
					Exit Do
				End If
			Loop
		End Sub

		Protected Friend Overridable Function mappedFileSize() As Long
			If workspaceConfiguration.getPolicyLocation() <> LocationPolicy.MMAP Then
				Return 0
			End If

			Return tempFile.length()
		End Function

		Protected Friend Overrides Sub clearExternalAllocations()
			If isDebug.get() Then
				log.info("Workspace [{}] device_{} threadId {} guid [{}]: clearing external allocations...", id, Nd4j.AffinityManager.getDeviceForCurrentThread(), Thread.CurrentThread.getId(), guid)
			End If

			Dim nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
			For Each pair As PointersPair In externalAllocations
				If pair.getHostPointer() IsNot Nothing Then
					nativeOps.freeHost(pair.getHostPointer())
				End If
			Next pair
			externalAllocations.Clear()
			externalCount.set(0)
			spilledAllocationsSize.set(0)
		End Sub

		Public Overrides Sub destroyWorkspace(ByVal extended As Boolean)
			SyncLock Me
				If isDebug.get() Then
					log.info("Destroying workspace...")
				End If
        
				Dim sizez As val = currentSize_Conflict.getAndSet(0)
				hostOffset_Conflict.set(0)
				deviceOffset_Conflict.set(0)
        
				If extended Then
					clearExternalAllocations()
				End If
        
				clearPinnedAllocations(extended)
        
				If workspaceConfiguration.getPolicyLocation() = LocationPolicy.RAM Then
					If workspace.getHostPointer() IsNot Nothing Then
						NativeOpsHolder.Instance.getDeviceNativeOps().freeHost(workspace.getHostPointer())
        
						AllocationsTracker.Instance.markReleased(AllocationKind.WORKSPACE, 0, sizez)
					End If
				ElseIf workspaceConfiguration.getPolicyLocation() = LocationPolicy.MMAP Then
					If workspace.getHostPointer() IsNot Nothing Then
						NativeOpsHolder.Instance.getDeviceNativeOps().munmapFile(Nothing, mmap, tempFile.length())
					End If
				End If
        
				workspace.setDevicePointer(Nothing)
				workspace.setHostPointer(Nothing)
			End SyncLock
		End Sub

		Protected Friend Overrides Sub resetWorkspace()
			'Pointer.memset(workspace.getHostPointer(), 0, currentSize.get() + SAFETY_OFFSET);
		End Sub

		Protected Friend Overridable Function workspace() As PointersPair
			Return workspace
		End Function

		Protected Friend Overridable Function pinnedPointers() As LinkedList(Of PointersPair)
			Return pinnedAllocations
		End Function

		Protected Friend Overridable Function externalPointers() As IList(Of PointersPair)
			Return externalAllocations
		End Function

		Public Overrides ReadOnly Property PrimaryOffset As Long
			Get
				Return HostOffset
			End Get
		End Property
	End Class

End Namespace