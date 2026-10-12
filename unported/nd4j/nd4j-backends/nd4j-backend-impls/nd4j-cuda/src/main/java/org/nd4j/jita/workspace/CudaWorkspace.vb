Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AllocationsTracker = org.nd4j.linalg.api.memory.AllocationsTracker
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports org.nd4j.linalg.api.memory.enums
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
Imports PointersPair = org.nd4j.linalg.api.memory.pointers.PointersPair
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jWorkspace = org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator
Imports MemoryTracker = org.nd4j.jita.allocator.impl.MemoryTracker

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

Namespace org.nd4j.jita.workspace


	''' <summary>
	''' CUDA-aware MemoryWorkspace implementation
	''' 
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudaWorkspace extends org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace
	Public Class CudaWorkspace
		Inherits Nd4jWorkspace


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CudaWorkspace(@NonNull WorkspaceConfiguration configuration)
		Public Sub New(ByVal configuration As WorkspaceConfiguration)
			MyBase.New(configuration)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CudaWorkspace(@NonNull WorkspaceConfiguration configuration, @NonNull String workspaceId)
		Public Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String)
			MyBase.New(configuration, workspaceId)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CudaWorkspace(@NonNull WorkspaceConfiguration configuration, @NonNull String workspaceId, System.Nullable<Integer> deviceId)
		Public Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String, ByVal deviceId As Integer?)
			MyBase.New(configuration, workspaceId)
			Me.deviceId = deviceId
		End Sub

		Protected Friend Overrides Sub init()
			If workspaceConfiguration.getPolicyLocation() = LocationPolicy.MMAP Then
				Throw New ND4JIllegalStateException("CUDA do not support MMAP workspaces yet")
			End If

			MyBase.init()

			If currentSize_Conflict.get() > 0 Then
				'log.info("Allocating {} bytes at DEVICE & HOST space...", currentSize.get());
				isInit.set(True)

				Dim bytes As Long = currentSize_Conflict.get()

				If isDebug.get() Then
					log.info("Allocating [{}] workspace on device_{}, {} bytes...", id, Nd4j.AffinityManager.getDeviceForCurrentThread(), bytes)
				End If

				If isDebug.get() Then
					Nd4j.WorkspaceManager.printAllocationStatisticsForCurrentThread()
				End If

				Dim ptr As Pointer = memoryManager.allocate((bytes + SAFETY_OFFSET), MemoryKind.HOST, False)
				If ptr Is Nothing Then
					Throw New ND4JIllegalStateException("Can't allocate memory for workspace")
				End If

				workspace.setHostPointer(New PagedPointer(ptr))

				If workspaceConfiguration.getPolicyMirroring() <> MirroringPolicy.HOST_ONLY Then
					workspace.setDevicePointer(New PagedPointer(memoryManager.allocate((bytes + SAFETY_OFFSET), MemoryKind.DEVICE, False)))
					AllocationsTracker.Instance.markAllocated(AllocationKind.GENERAL, Nd4j.AffinityManager.getDeviceForCurrentThread(), bytes + SAFETY_OFFSET)

					MemoryTracker.Instance.incrementWorkspaceAllocatedAmount(Nd4j.AffinityManager.getDeviceForCurrentThread(), bytes + SAFETY_OFFSET)

					' if base pointer isn't aligned to 16 bytes (128 bits) - adjust the offfset then
					Dim addr As val = workspace.getDevicePointer().address()
					Dim div As val = addr Mod alignmentBase
					If div <> 0 Then
						deviceOffset_Conflict.set(alignmentBase - div)
						hostOffset_Conflict.set(alignmentBase - div)
					End If
				End If
			End If
		End Sub

		Public Overrides Function alloc(ByVal requiredMemory As Long, ByVal type As DataType, ByVal initialize As Boolean) As PagedPointer
			Return Me.alloc(requiredMemory, MemoryKind.DEVICE, type, initialize)
		End Function


		Public Overrides Sub destroyWorkspace(ByVal extended As Boolean)
			SyncLock Me
				Dim size As val = currentSize_Conflict.getAndSet(0)
				reset()
        
				If extended Then
					clearExternalAllocations()
				End If
        
				clearPinnedAllocations(extended)
        
				If workspace.getHostPointer() IsNot Nothing Then
					NativeOpsHolder.Instance.getDeviceNativeOps().freeHost(workspace.getHostPointer())
				End If
        
				If workspace.getDevicePointer() IsNot Nothing Then
					NativeOpsHolder.Instance.getDeviceNativeOps().freeDevice(workspace.getDevicePointer(), 0)
					AllocationsTracker.Instance.markReleased(AllocationKind.GENERAL, Nd4j.AffinityManager.getDeviceForCurrentThread(), size + SAFETY_OFFSET)
        
					MemoryTracker.Instance.decrementWorkspaceAmount(Nd4j.AffinityManager.getDeviceForCurrentThread(), size + SAFETY_OFFSET)
				End If
        
				workspace.setDevicePointer(Nothing)
				workspace.setHostPointer(Nothing)
        
			End SyncLock
		End Sub


		Public Overrides Function alloc(ByVal requiredMemory As Long, ByVal kind As MemoryKind, ByVal type As DataType, ByVal initialize As Boolean) As PagedPointer
			Dim numElements As Long = requiredMemory \ Nd4j.sizeOfDataType(type)

			' alignment
			If requiredMemory Mod alignmentBase <> 0 Then
				requiredMemory += alignmentBase - (requiredMemory Mod alignmentBase)
			End If

			If Not isUsed.get() Then
				If disabledCounter.incrementAndGet() Mod 10 = 0 Then
					log.warn("Worskpace was turned off, and wasn't enabled after {} allocations", disabledCounter.get())
				End If

				If kind = MemoryKind.DEVICE Then
					Dim pointer As val = New PagedPointer(memoryManager.allocate(requiredMemory, MemoryKind.DEVICE, initialize), numElements)
					externalAllocations.Add(New PointersPair(Nothing, pointer))
					MemoryTracker.Instance.incrementWorkspaceAllocatedAmount(Nd4j.AffinityManager.getDeviceForCurrentThread(), requiredMemory)
					Return pointer
				Else
					Dim pointer As val = New PagedPointer(memoryManager.allocate(requiredMemory, MemoryKind.HOST, initialize), numElements)
					externalAllocations.Add(New PointersPair(pointer, Nothing))
					Return pointer
				End If


			End If

			Dim trimmer As Boolean = (workspaceConfiguration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso requiredMemory + cycleAllocations.get() > initialBlockSize_Conflict.get() AndAlso initialBlockSize_Conflict.get() > 0 AndAlso kind = MemoryKind.DEVICE) OrElse trimmedMode.get()

			If trimmer AndAlso workspaceConfiguration.getPolicySpill() = SpillPolicy.REALLOCATE AndAlso Not trimmedMode.get() Then
				trimmedMode.set(True)
				trimmedStep.set(stepsCount.get())
			End If

			If kind = MemoryKind.DEVICE Then
				If deviceOffset_Conflict.get() + requiredMemory <= currentSize_Conflict.get() AndAlso Not trimmer AndAlso Nd4j.WorkspaceManager.DebugMode <> DebugMode.SPILL_EVERYTHING Then
					cycleAllocations.addAndGet(requiredMemory)
					Dim prevOffset As Long = deviceOffset_Conflict.getAndAdd(requiredMemory)

					If workspaceConfiguration.getPolicyMirroring() = MirroringPolicy.HOST_ONLY Then
						Return Nothing
					End If

					Dim ptr As val = workspace.getDevicePointer().withOffset(prevOffset, numElements)

					If isDebug.get() Then
						log.info("Workspace [{}] device_{}: alloc array of {} bytes, capacity of {} elements; prevOffset: {}; newOffset: {}; size: {}; address: {}", id, Nd4j.AffinityManager.getDeviceForCurrentThread(), requiredMemory, numElements, prevOffset, deviceOffset_Conflict.get(), currentSize_Conflict.get(), ptr.address())
					End If

					If initialize Then
						Dim context As val = AtomicAllocator.Instance.DeviceContext

						Dim ret As Integer = NativeOpsHolder.Instance.getDeviceNativeOps().memsetAsync(ptr, 0, requiredMemory, 0, context.getSpecialStream())
						If ret = 0 Then
							Throw New ND4JIllegalStateException("memset failed device_" & Nd4j.AffinityManager.getDeviceForCurrentThread())
						End If

						context.syncSpecialStream()
					End If

					Return ptr
				Else

					' spill
					If workspaceConfiguration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso currentSize_Conflict.get() > 0 AndAlso Not trimmer AndAlso Nd4j.WorkspaceManager.DebugMode <> DebugMode.SPILL_EVERYTHING Then
						'log.info("End of space reached. Current offset: {}; requiredMemory: {}", deviceOffset.get(), requiredMemory);
						deviceOffset_Conflict.set(0)
						resetPlanned.set(True)
						Return alloc(requiredMemory, kind, type, initialize)
					End If

					If Not trimmer Then
						spilledAllocationsSize.addAndGet(requiredMemory)
					Else
						pinnedAllocationsSize.addAndGet(requiredMemory)
					End If

					If isDebug.get() Then
						log.info("Workspace [{}] device_{}: spilled DEVICE array of {} bytes, capacity of {} elements", id, Nd4j.AffinityManager.getDeviceForCurrentThread(), requiredMemory, numElements)
					End If

					Dim shape As val = New AllocationShape(requiredMemory \ Nd4j.sizeOfDataType(type), Nd4j.sizeOfDataType(type), type)

					cycleAllocations.addAndGet(requiredMemory)

					If workspaceConfiguration.getPolicyMirroring() = MirroringPolicy.HOST_ONLY Then
						Return Nothing
					End If

					Select Case workspaceConfiguration.getPolicySpill()
						Case REALLOCATE, EXTERNAL
							If Not trimmer Then
								externalCount.incrementAndGet()
								'
								'AtomicAllocator.getInstance().getMemoryHandler().getMemoryProvider().malloc(shape, null, AllocationStatus.DEVICE).getDevicePointer()
								Dim pointer As val = New PagedPointer(memoryManager.allocate(requiredMemory, MemoryKind.DEVICE, initialize), numElements)
								pointer.isLeaked()

								Dim pp As val = New PointersPair(Nothing, pointer)
								pp.setRequiredMemory(requiredMemory)
								externalAllocations.Add(pp)

								MemoryTracker.Instance.incrementWorkspaceAllocatedAmount(Nd4j.AffinityManager.getDeviceForCurrentThread(), requiredMemory)
								Return pointer
							Else
								pinnedCount.incrementAndGet()

								Dim pointer As val = New PagedPointer(memoryManager.allocate(requiredMemory, MemoryKind.DEVICE, initialize), numElements)
								pointer.isLeaked()

								pinnedAllocations.AddLast(New PointersPair(stepsCount.get(), requiredMemory, Nothing, pointer))
								MemoryTracker.Instance.incrementWorkspaceAllocatedAmount(Nd4j.AffinityManager.getDeviceForCurrentThread(), requiredMemory)
								Return pointer
							End If
						Case Else
							Throw New ND4JIllegalStateException("Can't allocate memory: Workspace is full")
					End Select
				End If
			ElseIf kind = MemoryKind.HOST Then
				If hostOffset_Conflict.get() + requiredMemory <= currentSize_Conflict.get() AndAlso Not trimmer AndAlso Nd4j.WorkspaceManager.DebugMode <> DebugMode.SPILL_EVERYTHING Then

					Dim prevOffset As Long = hostOffset_Conflict.getAndAdd(requiredMemory)

					Dim ptr As val = workspace.getHostPointer().withOffset(prevOffset, numElements)

					' && workspaceConfiguration.getPolicyMirroring() == MirroringPolicy.HOST_ONLY
					If initialize Then
						Pointer.memset(ptr, 0, requiredMemory)
					End If
					Return ptr
				Else
			   '     log.info("Spilled HOST array of {} bytes, capacity of {} elements", requiredMemory, numElements);
					If workspaceConfiguration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso currentSize_Conflict.get() > 0 AndAlso Not trimmer AndAlso Nd4j.WorkspaceManager.DebugMode <> DebugMode.SPILL_EVERYTHING Then
						'log.info("End of space reached. Current offset: {}; requiredMemory: {}", deviceOffset.get(), requiredMemory);
						hostOffset_Conflict.set(0)
						'resetPlanned.set(true);
						Return alloc(requiredMemory, kind, type, initialize)
					End If

					Dim shape As val = New AllocationShape(requiredMemory \ Nd4j.sizeOfDataType(type), Nd4j.sizeOfDataType(type), type)

					Select Case workspaceConfiguration.getPolicySpill()
						Case REALLOCATE, EXTERNAL
							If Not trimmer Then
								'memoryManager.allocate(requiredMemory, MemoryKind.HOST, true)
								'AtomicAllocator.getInstance().getMemoryHandler().getMemoryProvider().malloc(shape, null, AllocationStatus.DEVICE).getDevicePointer()
								Dim pointer As New PagedPointer(memoryManager.allocate(requiredMemory, MemoryKind.HOST, initialize), numElements)

								externalAllocations.Add(New PointersPair(pointer, Nothing))
								Return pointer
							Else
								'AtomicAllocator.getInstance().getMemoryHandler().getMemoryProvider().malloc(shape, null, AllocationStatus.DEVICE).getDevicePointer()
								Dim pointer As New PagedPointer(memoryManager.allocate(requiredMemory, MemoryKind.HOST, initialize), numElements)
								pointer.isLeaked()

								pinnedAllocations.AddLast(New PointersPair(stepsCount.get(), 0L, pointer, Nothing))
								Return pointer
							End If
						Case Else
							Throw New ND4JIllegalStateException("Can't allocate memory: Workspace is full")
					End Select
				End If
			Else
				Throw New ND4JIllegalStateException("Unknown MemoryKind was passed in: " & kind)
			End If

			'throw new ND4JIllegalStateException("Shouldn't ever reach this line");
		End Function

		Protected Friend Overrides Sub clearPinnedAllocations(ByVal extended As Boolean)
			If isDebug.get() Then
				log.info("Workspace [{}] device_{} threadId {} cycle {}: clearing pinned allocations...", id, Nd4j.AffinityManager.getDeviceForCurrentThread(), Thread.CurrentThread.getId(), cyclesCount_Conflict.get())
			End If

			Do While pinnedAllocations.Count > 0
				Dim pair As val = pinnedAllocations.First.Value
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

					If pair.getDevicePointer() IsNot Nothing Then
						NativeOpsHolder.Instance.getDeviceNativeOps().freeDevice(pair.getDevicePointer(), 0)
						MemoryTracker.Instance.decrementWorkspaceAmount(Nd4j.AffinityManager.getDeviceForCurrentThread(), pair.getRequiredMemory())
						pinnedCount.decrementAndGet()

						If isDebug.get() Then
							log.info("deleting external device allocation ")
						End If
					End If

					If pair.getHostPointer() IsNot Nothing Then
						NativeOpsHolder.Instance.getDeviceNativeOps().freeHost(pair.getHostPointer())

						If isDebug.get() Then
							log.info("deleting external host allocation ")
						End If
					End If

					Dim sizez As val = pair.getRequiredMemory() * -1
					pinnedAllocationsSize.addAndGet(sizez)
				Else
					Exit Do
				End If
			Loop
		End Sub

		Protected Friend Overrides Sub clearExternalAllocations()
			If isDebug.get() Then
				log.info("Workspace [{}] device_{} threadId {} guid [{}]: clearing external allocations...", id, Nd4j.AffinityManager.getDeviceForCurrentThread(), Thread.CurrentThread.getId(), guid)
			End If

			Nd4j.Executioner.commit()

			Try
				For Each pair As PointersPair In externalAllocations
					If pair.getHostPointer() IsNot Nothing Then
						NativeOpsHolder.Instance.getDeviceNativeOps().freeHost(pair.getHostPointer())

						If isDebug.get() Then
							log.info("deleting external host allocation... ")
						End If
					End If

					If pair.getDevicePointer() IsNot Nothing Then
						NativeOpsHolder.Instance.getDeviceNativeOps().freeDevice(pair.getDevicePointer(), 0)

						If isDebug.get() Then
							log.info("deleting external device allocation... ")
						End If

						Dim sizez As val = pair.getRequiredMemory()
						If sizez IsNot Nothing Then
							AllocationsTracker.Instance.markReleased(AllocationKind.GENERAL, Nd4j.AffinityManager.getDeviceForCurrentThread(), sizez)
							MemoryTracker.Instance.decrementWorkspaceAmount(Nd4j.AffinityManager.getDeviceForCurrentThread(), sizez)
						End If
					End If
				Next pair
			Catch e As Exception
				log.error("RC: Workspace [{}] device_{} threadId {} guid [{}]: clearing external allocations...", id, Nd4j.AffinityManager.getDeviceForCurrentThread(), Thread.CurrentThread.getId(), guid)
				Throw New Exception(e)
			End Try

			spilledAllocationsSize.set(0)
			externalCount.set(0)
			externalAllocations.Clear()
		End Sub

		Protected Friend Overrides Sub resetWorkspace()
			If currentSize_Conflict.get() < 1 Then
				Return
			End If


	'
	'        if (Nd4j.getExecutioner() instanceof GridExecutioner)
	'            ((GridExecutioner) Nd4j.getExecutioner()).flushQueueBlocking();
	'
	'        CudaContext context = (CudaContext) AtomicAllocator.getInstance().getDeviceContext().getContext();
	'
	'        //log.info("workspace: {}, size: {}", workspace.getDevicePointer().address(), currentSize.get());
	'
	'        NativeOpsHolder.getInstance().getDeviceNativeOps().memsetAsync(workspace.getDevicePointer(), 0, currentSize.get() + SAFETY_OFFSET, 0, context.getSpecialStream());
	'
	'        Pointer.memset(workspace.getHostPointer(), 0, currentSize.get() + SAFETY_OFFSET);
	'
	'        context.getSpecialStream().synchronize();
	'        
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

		Public Overrides Function deallocator() As Deallocator
			Return New CudaWorkspaceDeallocator(Me)
		End Function

		Public Overrides ReadOnly Property UniqueId As String
			Get
				Return "Workspace_" & Id & "_" & Nd4j.DeallocatorService.nextValue()
			End Get
		End Property

		Public Overrides Function targetDevice() As Integer
			Return deviceId
		End Function

		Public Overrides ReadOnly Property PrimaryOffset As Long
			Get
				Return DeviceOffset
			End Get
		End Property
	End Class

End Namespace