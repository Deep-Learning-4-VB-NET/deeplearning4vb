Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BytePointer = org.bytedeco.javacpp.BytePointer
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports org.nd4j.linalg.api.memory.enums
Imports PagedPointer = org.nd4j.linalg.api.memory.pointers.PagedPointer
Imports PointersPair = org.nd4j.linalg.api.memory.pointers.PointersPair
Imports ND4JIllegalStateException = org.nd4j.linalg.exception.ND4JIllegalStateException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports MemoryManager = org.nd4j.linalg.api.memory.MemoryManager
Imports ND4JFileUtils = org.nd4j.common.util.ND4JFileUtils

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

Namespace org.nd4j.linalg.api.memory.abstracts


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class Nd4jWorkspace implements org.nd4j.linalg.api.memory.MemoryWorkspace
	Public MustInherit Class Nd4jWorkspace
		Implements MemoryWorkspace

		Public MustOverride Function targetDevice() As Integer Implements org.nd4j.linalg.api.memory.Deallocatable.targetDevice
		Public MustOverride Function deallocator() As Deallocator Implements org.nd4j.linalg.api.memory.Deallocatable.deallocator
		Public MustOverride ReadOnly Property UniqueId As String Implements org.nd4j.linalg.api.memory.Deallocatable.getUniqueId
		Public MustOverride WriteOnly Property PreviousWorkspace As MemoryWorkspace
		Public MustOverride ReadOnly Property PrimaryOffset As Long Implements MemoryWorkspace.getPrimaryOffset
		Public MustOverride ReadOnly Property ThreadId As Long? Implements MemoryWorkspace.getThreadId
		Public MustOverride ReadOnly Property DeviceId As Integer Implements MemoryWorkspace.getDeviceId
		Public MustOverride ReadOnly Property Id As String Implements MemoryWorkspace.getId
		Public MustOverride ReadOnly Property WorkspaceConfiguration As WorkspaceConfiguration Implements MemoryWorkspace.getWorkspaceConfiguration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected int deviceId;
		Protected Friend deviceId As Integer
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected System.Nullable<Long> threadId;
		Protected Friend threadId As Long?

'JAVA TO VB CONVERTER NOTE: The field workspaceType was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend workspaceType_Conflict As Type = Type.SCOPED

		Protected Friend Const SAFETY_OFFSET As Long = 1024L

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected String id;
		Protected Friend id As String

'JAVA TO VB CONVERTER NOTE: The field currentSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend currentSize_Conflict As New AtomicLong(0)
'JAVA TO VB CONVERTER NOTE: The field hostOffset was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend hostOffset_Conflict As New AtomicLong(0)
'JAVA TO VB CONVERTER NOTE: The field deviceOffset was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend deviceOffset_Conflict As New AtomicLong(0)

		Protected Friend workspace As New PointersPair()

		Protected Friend memoryManager As MemoryManager

		Protected Friend isLearning As New AtomicBoolean(True)
		Protected Friend isUsed As New AtomicBoolean(True)

		Protected Friend disabledCounter As New AtomicLong(0)


'JAVA TO VB CONVERTER NOTE: The field cyclesCount was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend cyclesCount_Conflict As New AtomicLong(0)
		Protected Friend stepsCount As New AtomicLong(0)
		Protected Friend stepsNumber As Integer = 1

'JAVA TO VB CONVERTER NOTE: The field lastCycleAllocations was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend lastCycleAllocations_Conflict As New AtomicLong(0)
		Protected Friend cycleAllocations As New AtomicLong(0)
		Protected Friend spilledAllocationsSize As New AtomicLong(0)
		Protected Friend pinnedAllocationsSize As New AtomicLong(0)
		Protected Friend maxCycle As New AtomicLong(0)
		Protected Friend resetPlanned As New AtomicBoolean(False)
		Protected Friend isOpen As New AtomicBoolean(False)
		Protected Friend isInit As New AtomicBoolean(False)
		Protected Friend isOver As New AtomicBoolean(False)
		Protected Friend isBorrowed As New AtomicBoolean(False)

		Protected Friend tagScope As New AtomicInteger(0)

		Protected Friend isDebug As New AtomicBoolean(False)
		Protected Friend externalCount As New AtomicInteger(0)
		Protected Friend pinnedCount As New AtomicInteger(0)

		Protected Friend trimmedMode As New AtomicBoolean(False)
		Protected Friend trimmedStep As New AtomicLong(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected final org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration workspaceConfiguration;
		Protected Friend ReadOnly workspaceConfiguration As WorkspaceConfiguration

		' external allocations are purged at the end of loop
		Protected Friend externalAllocations As IList(Of PointersPair) = New List(Of PointersPair)()

		' pinned allocations are purged with delay, used for circular mode only
		Protected Friend pinnedAllocations As LinkedList(Of PointersPair) = New LinkedTransferQueue(Of PointersPair)()

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter protected org.nd4j.linalg.api.memory.MemoryWorkspace previousWorkspace;
		Protected Friend previousWorkspace As MemoryWorkspace
		Protected Friend borrowingWorkspace As MemoryWorkspace

'JAVA TO VB CONVERTER NOTE: The field initialBlockSize was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend initialBlockSize_Conflict As New AtomicLong(0)

		Protected Friend guid As String

		Protected Friend tempFile As File

'JAVA TO VB CONVERTER NOTE: The field generationId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend generationId_Conflict As New AtomicLong(0)

		' this field is used as alignment base for all allocations within this workspace
		Public Const alignmentBase As Integer = 32

		' this memory manager implementation will be used to allocate real memory for this workspace

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Nd4jWorkspace(@NonNull WorkspaceConfiguration configuration)
		Public Sub New(ByVal configuration As WorkspaceConfiguration)
			Me.New(configuration, DEFAULT_ID)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Nd4jWorkspace(@NonNull WorkspaceConfiguration configuration, @NonNull String workspaceId)
		Public Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String)
			Me.workspaceConfiguration = configuration
			Me.id = workspaceId
			Me.threadId = Thread.CurrentThread.getId()
			Me.guid = Nd4j.WorkspaceManager.UUID
			Me.memoryManager = Nd4j.MemoryManager
			Me.deviceId = Nd4j.AffinityManager.getDeviceForCurrentThread()

			' and actual workspace allocation
			currentSize_Conflict.set(workspaceConfiguration.getInitialSize())

			If workspaceConfiguration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED Then
				workspaceType_Conflict = Type.CIRCULAR
			Else
				workspaceType_Conflict = Type.SCOPED
			End If

			If workspaceConfiguration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso workspaceConfiguration.getPolicyAllocation() = AllocationPolicy.OVERALLOCATE Then
				If workspaceConfiguration.getOverallocationLimit() < 1.0 Then
					Throw New ND4JIllegalStateException("For cyclic workspace overallocation should be positive integral value.")
				End If

				stepsNumber = CInt(Math.Truncate(workspaceConfiguration.getOverallocationLimit() + 1))
				log.trace("Steps: {}", stepsNumber)
			End If

			'if (workspaceConfiguration.getPolicyLearning() == LearningPolicy.OVER_TIME && workspaceConfiguration.getCyclesBeforeInitialization() < 1)
			'log.warn("Workspace [{}]: initialization OVER_TIME was selected, but number of cycles isn't positive value!", id);

			' validate mmap option
			If configuration.getPolicyLocation() = LocationPolicy.MMAP Then
				' file path should be either non-null
				If configuration.getTempFilePath() IsNot Nothing Then
						tempFile = New File(configuration.getTempFilePath())

						If tempFile.length() = 0 OrElse tempFile.length() < configuration.getInitialSize() Then
							If configuration.getInitialSize() > 0 Then
								Try
									fillFile(tempFile, configuration.getInitialSize())
								Catch e As Exception
									Throw New Exception(e)
								End Try
							Else
								Throw New ND4JIllegalStateException("Memory-mapped file should have positive length.")
							End If
						Else
							configuration.setInitialSize(tempFile.length())
						End If
				ElseIf configuration.getInitialSize() > 0 Then
					Try
						tempFile = ND4JFileUtils.createTempFile("workspace", "tempMMAP")
						tempFile.deleteOnExit()

						' fill temp file with zeroes, up to initialSize bytes
						fillFile(tempFile, configuration.getInitialSize())

					Catch e As Exception
						Throw New Exception(e)
					End Try
				Else
					Throw New ND4JIllegalStateException("MMAP target file path should be non-null or workspace initialSize should be >0 for temp file")
				End If
			End If

			init()
		End Sub

		Public Overridable ReadOnly Property WorkspaceType As Type Implements MemoryWorkspace.getWorkspaceType
			Get
				Return Me.workspaceType_Conflict
			End Get
		End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void fillFile(java.io.File file, long length) throws Exception
		Public Shared Sub fillFile(ByVal file As File, ByVal length As Long)
			Dim buffer(16383) As SByte
			For i As Integer = 0 To buffer.Length - 1
				buffer(i) = CSByte(0)
			Next i

			Using fos As New FileStream(file, FileMode.Create, FileAccess.Write), bos As New java.io.BufferedOutputStream(fos)
				Dim written As Long = 0
				Do While written < length
					fos.Write(buffer, 0, buffer.Length)
					written += buffer.Length
				Loop
			End Using
		End Sub

		Public Overridable ReadOnly Property GenerationId As Long Implements MemoryWorkspace.getGenerationId
			Get
				Return generationId_Conflict.get()
			End Get
		End Property

		''' <summary>
		''' This method returns step number. Viable only in circular mode.
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property StepNumber As Long
			Get
				Return stepsCount.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of bytes in spilled allocations.
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property SpilledSize As Long
			Get
				Return spilledAllocationsSize.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of bytes in pinned allocations.
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property PinnedSize As Long
			Get
				Return pinnedAllocationsSize.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of bytes for first block of circular workspace.
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property InitialBlockSize As Long
			Get
				Return initialBlockSize_Conflict.get()
			End Get
		End Property

		''' <summary>
		''' This method returns parent Workspace, if any. Null if there's none.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ParentWorkspace As MemoryWorkspace
			Get
				Return previousWorkspace
			End Get
		End Property

		''' <summary>
		''' This method returns current device memory offset within workspace
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property DeviceOffset As Long
			Get
				Return deviceOffset_Conflict.get()
			End Get
		End Property

		''' <summary>
		''' This method returns current host memory offset within workspace
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property HostOffset As Long
			Get
				Return hostOffset_Conflict.get()
			End Get
		End Property

		''' <summary>
		''' This method returns current amount of memory allocated for workspace.
		''' 
		''' PLEASE NOTE: It shows only amount of HOST memory.
		''' If current backend assumes DEVICE/HOST memory pair,
		''' DEVICE memory will probably have the same size, but won't be accounted in this value.
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property CurrentSize As Long Implements MemoryWorkspace.getCurrentSize
			Get
				Return currentSize_Conflict.get()
			End Get
		End Property

		Public Overridable ReadOnly Property CurrentOffset As Long Implements MemoryWorkspace.getCurrentOffset
			Get
				Return hostOffset_Conflict.get()
			End Get
		End Property

		Protected Friend Overridable Sub init()
			' in case of MMAP we don't want any learning applied
			If workspaceConfiguration.getPolicyLocation() = LocationPolicy.MMAP AndAlso workspaceConfiguration.getPolicyLearning() <> LearningPolicy.NONE Then
				Throw New System.ArgumentException("Workspace backed by memory-mapped file can't have LearningPolicy defined")
			End If

			' we don't want overallocation in case of MMAP
			If currentSize_Conflict.get() > 0 AndAlso workspaceConfiguration.getPolicyLocation() <> LocationPolicy.MMAP Then
				If Not isOver.get() Then
					If workspaceConfiguration.getPolicyAllocation() = AllocationPolicy.OVERALLOCATE AndAlso workspaceConfiguration.getOverallocationLimit() > 0 Then
						currentSize_Conflict.addAndGet(CLng(Math.Truncate(currentSize_Conflict.get() * workspaceConfiguration.getOverallocationLimit())))
						isOver.set(True)
					End If
				End If

				If workspaceConfiguration.getMaxSize() > 0 AndAlso currentSize_Conflict.get() > workspaceConfiguration.getMaxSize() Then
					currentSize_Conflict.set(workspaceConfiguration.getMaxSize())
				End If
			End If
		End Sub

		Public Overridable Function alloc(ByVal requiredMemory As Long, ByVal type As DataType, ByVal initialize As Boolean) As PagedPointer Implements MemoryWorkspace.alloc
			Return alloc(requiredMemory, MemoryKind.HOST, type, initialize)
		End Function

		''' <summary>
		''' This method enabled debugging mode for this workspace
		''' </summary>
		''' <param name="reallyEnable"> </param>
		Public Overridable Sub enableDebug(ByVal reallyEnable As Boolean) Implements MemoryWorkspace.enableDebug
			Me.isDebug.set(reallyEnable)
		End Sub

		Public Overridable Function alloc(ByVal requiredMemory As Long, ByVal kind As MemoryKind, ByVal type As DataType, ByVal initialize As Boolean) As PagedPointer
	'        
	'            just two options here:
	'            1) reqMem + hostOffset < totalSize, we just return pointer + offset
	'            2) go for either external spilled, or pinned allocation
	'         

			Dim numElements As Long = requiredMemory \ Nd4j.sizeOfDataType(type)

			' we enforce 8 byte alignment to ensure CUDA doesn't blame us
			Dim div As Long = requiredMemory Mod alignmentBase
			If div <> 0 Then
				requiredMemory += (alignmentBase - div)
			End If

			' shortcut made to skip workspace
			If Not isUsed.get() Then
				If disabledCounter.incrementAndGet() Mod 10 = 0 Then
					log.warn("Workspace was turned off, and wasn't enabled after {} allocations", disabledCounter.get())
				End If

				Dim pointer As New PagedPointer(memoryManager.allocate(requiredMemory, MemoryKind.HOST, initialize), numElements)

				externalAllocations.Add(New PointersPair(pointer, Nothing))

				Return pointer
			End If

	'        
	'            Trimmed mode is possible for cyclic workspace mode. Used in AsyncDataSetIterator, MQ, etc.
	'            Basically idea is simple: if one of datasets coming out of iterator has size higher then expected - we should reallocate workspace to match this size.
	'            So, we switch to trimmed mode, and all allocations will be "pinned", and eventually workspace will be reallocated.
	'         
			Dim trimmer As Boolean = (workspaceConfiguration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso requiredMemory + cycleAllocations.get() > initialBlockSize_Conflict.get() AndAlso initialBlockSize_Conflict.get() > 0) OrElse trimmedMode.get()

			If trimmer AndAlso workspaceConfiguration.getPolicySpill() = SpillPolicy.REALLOCATE AndAlso Not trimmedMode.get() Then
				trimmedMode.set(True)
				trimmedStep.set(stepsCount.get())
			End If

			' if size is enough - allocate from workspace
			If hostOffset_Conflict.get() + requiredMemory <= currentSize_Conflict.get() AndAlso Not trimmer AndAlso Nd4j.WorkspaceManager.DebugMode <> DebugMode.SPILL_EVERYTHING Then
				' just alignment to 8 bytes

				cycleAllocations.addAndGet(requiredMemory)
				Dim prevOffset As Long = hostOffset_Conflict.getAndAdd(requiredMemory)
				deviceOffset_Conflict.set(hostOffset_Conflict.get())

				Dim ptr As PagedPointer = workspace.getHostPointer().withOffset(prevOffset, numElements)

				If isDebug.get() Then
					log.info("Workspace [{}]: Allocating array of {} bytes, capacity of {} elements, prevOffset: {}; currentOffset: {}; address: {}", id, requiredMemory, numElements, prevOffset, hostOffset_Conflict.get(), ptr.address())
				End If

				If initialize Then
					Pointer.memset(ptr, 0, requiredMemory)
				End If

				Return ptr
			Else
				' if current workspace isn't enough - we allocate it separately as spilled (or pinned, in case of circular mode)

				' in case of circular mode - we just reset offsets, and start from the beginning of the workspace
				If workspaceConfiguration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso currentSize_Conflict.get() > 0 AndAlso Not trimmer AndAlso Nd4j.WorkspaceManager.DebugMode <> DebugMode.SPILL_EVERYTHING Then
					reset()
					resetPlanned.set(True)
					Return alloc(requiredMemory, kind, type, initialize)
				End If

				' updating respective counters
				If Not trimmer Then
					spilledAllocationsSize.addAndGet(requiredMemory)
				Else
					pinnedAllocationsSize.addAndGet(requiredMemory)
				End If

				If isDebug.get() Then
					log.info("Workspace [{}]: step: {}, spilled  {} bytes, capacity of {} elements", id, stepsCount.get(), requiredMemory, numElements)
				End If

				Select Case workspaceConfiguration.getPolicySpill()
					Case REALLOCATE, EXTERNAL
						cycleAllocations.addAndGet(requiredMemory)
						If Not trimmer Then
							externalCount.incrementAndGet()

							Dim pointer As New PagedPointer(memoryManager.allocate(requiredMemory, MemoryKind.HOST, initialize), numElements)

							externalAllocations.Add(New PointersPair(pointer, Nothing))

							Return pointer
						Else
							pinnedCount.incrementAndGet()
							Dim pointer As New PagedPointer(memoryManager.allocate(requiredMemory, MemoryKind.HOST, initialize), numElements)

							pinnedAllocations.AddLast(New PointersPair(stepsCount.get(), requiredMemory, pointer, Nothing))


							Return pointer
						End If
					Case Else
						Throw New ND4JIllegalStateException("Can't allocate memory: Workspace is full")
				End Select
			End If
		End Function

		Public Overridable Sub free(ByVal pointer As Pointer)
			' no-op for main page(s), purge for external stuff
		End Sub

		Public Overridable Sub initializeWorkspace() Implements MemoryWorkspace.initializeWorkspace
			' we can reallocate this workspace to larger size if that's needed and allowed by configuration
			If (currentSize_Conflict.get() < maxCycle.get() OrElse currentSize_Conflict.get() < cycleAllocations.get()) AndAlso workspaceConfiguration.getPolicySpill() = SpillPolicy.REALLOCATE AndAlso (workspaceConfiguration.getMaxSize() = 0 OrElse (maxCycle.get() < workspaceConfiguration.getMaxSize())) Then
				If workspaceConfiguration.getPolicyReset() <> ResetPolicy.ENDOFBUFFER_REACHED Then
					destroyWorkspace(True)
					isInit.set(False)
				End If
			End If

			' if we're in cyclic mode, we do reallocations only after 2 full cycles, to avoid race conditions
			If trimmedMode.get() AndAlso trimmedStep.get() + 2 < stepsCount.get() Then
				destroyWorkspace(False)
				isInit.set(False)
				isOver.set(False)
			End If

			If Not isInit.get() Then
				If workspaceConfiguration.getPolicyLearning() <> LearningPolicy.NONE Then
					If workspaceConfiguration.getMaxSize() > 0 Then
						currentSize_Conflict.set(Math.Min(maxCycle.get(), workspaceConfiguration.getMaxSize()))
					Else
						currentSize_Conflict.set(maxCycle.get())
					End If

					' if we're on cyclic mode, let's add 30% to size, just to reduce number of reallocations
					If workspaceConfiguration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED Then
						currentSize_Conflict.set(CLng(Math.Truncate(currentSize_Conflict.get() * 1.3)))
						currentSize_Conflict.addAndGet(8 - (currentSize_Conflict.get() Mod 8))
						maxCycle.set(currentSize_Conflict.get())
					End If

					' we're updating single block size for circular mode, will be used for alignment later
					initialBlockSize_Conflict.set(currentSize_Conflict.get())

					' handliong optional overallocation here, however it's usually good idea to use it everywhere, to avoid frequent realloc calls
					If Not isOver.get() Then
						If workspaceConfiguration.getPolicyAllocation() = AllocationPolicy.OVERALLOCATE AndAlso workspaceConfiguration.getOverallocationLimit() > 0 AndAlso currentSize_Conflict.get() > 0 Then
							currentSize_Conflict.set(currentSize_Conflict.get() + CLng(Math.Truncate(currentSize_Conflict.get() * workspaceConfiguration.getOverallocationLimit())))
							isOver.set(True)
						End If
					End If

					If workspaceConfiguration.getMinSize() > 0 AndAlso currentSize_Conflict.get() < workspaceConfiguration.getMinSize() Then
						currentSize_Conflict.set(workspaceConfiguration.getMinSize())
					End If

					' purge spilled allocations
					If externalCount.get() > 0 AndAlso (workspaceConfiguration.getPolicyReset() = ResetPolicy.BLOCK_LEFT OrElse resetPlanned.get()) Then
						clearExternalAllocations()
						resetPlanned.set(False)
					End If

					' calling for implementation-specific workspace initialization. basically allocation happens there
					init()
				End If
			End If
		End Sub

		''' <summary>
		''' This method returns number of spilled allocations, that can be purged at the end of block
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property NumberOfExternalAllocations As Integer
			Get
				Return externalCount.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of pinned allocations, they can be purged after 2 steps.
		''' 
		''' PLEASE NOTE: This method can return non-zero calues only for circular workspace mode
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property NumberOfPinnedAllocations As Integer
			Get
				Return pinnedCount.get()
			End Get
		End Property

		Public Overridable Sub destroyWorkspace() Implements MemoryWorkspace.destroyWorkspace
			destroyWorkspace(True)
		End Sub


		''' <summary>
		''' This method basically deallocates workspace memory
		''' </summary>
		''' <param name="extended"> </param>
		Public Overridable Sub destroyWorkspace(ByVal extended As Boolean) Implements MemoryWorkspace.destroyWorkspace
			If workspace.getHostPointer() IsNot Nothing AndAlso workspace.getHostPointer().getOriginalPointer() IsNot Nothing AndAlso TypeOf workspace.getHostPointer().getOriginalPointer() Is BytePointer Then
				workspace.getHostPointer().getOriginalPointer().deallocate()
			End If

			workspace.setHostPointer(Nothing)
			currentSize_Conflict.set(0)
			reset()

			If extended Then
				clearExternalAllocations()
			End If

			'cycleAllocations.set(0);
			'maxCycle.set(0);
		End Sub

		''' <summary>
		''' This method TEMPORARY enters this workspace, without reset applied
		''' 
		''' @return
		''' </summary>
		Public Overridable Function notifyScopeBorrowed() As MemoryWorkspace
			If isBorrowed.get() Then
				Throw New ND4JIllegalStateException("Workspace [" & id & "]: Can't borrow from borrowed workspace")
			End If

			borrowingWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			isBorrowed.set(True)

			Nd4j.MemoryManager.CurrentWorkspace = Me

			Return Me
		End Function

		Public Overridable ReadOnly Property CyclesCount As Long
			Get
				Return cyclesCount_Conflict.get()
			End Get
		End Property

		Public Overridable Sub close() Implements MemoryWorkspace.close
			' first we check if this workspace was borrowed. if yes - just close without reset.
			If isBorrowed.get() Then
				If tagScope.get() > 0 Then
					If tagScope.decrementAndGet() = 0 Then
						Nd4j.MemoryManager.CurrentWorkspace = Me
					End If
					Return
				End If

				isBorrowed.set(False)
				Nd4j.MemoryManager.CurrentWorkspace = borrowingWorkspace
				Return
			End If

			' next we check, if the same workspace was opened multiple times sequentially. then we just decrement counter, without reset
			If tagScope.get() > 0 Then
				If tagScope.decrementAndGet() = 0 Then
					Nd4j.MemoryManager.CurrentWorkspace = Me
				End If
				Return
			End If

			' this is for safety. We have to be sure that no ops were left non-processed
			'Furthermore, need to commit before marking workspace as closed, to avoid (incorrectly) hitting scope panic
			Nd4j.Executioner.commit()

			' since this workspace block is finished, we restore previous one. Even if it's null
			Nd4j.MemoryManager.CurrentWorkspace = previousWorkspace
			isOpen.set(False)

			' just counter for cycles/blocks
			cyclesCount_Conflict.incrementAndGet()
			If cyclesCount_Conflict.get() > 1 And (cyclesCount_Conflict.get() - 1) Mod stepsNumber = 0 Then
				' this counter is for cyclic mode, it counts generations, full loops over buffer
				stepsCount.incrementAndGet()
			End If
	'        
	'            Basically all we want here, is:
	'            1) memset primary page(s)
	'            2) purge external allocations
	'         

			If Not isUsed.get() Then
				log.warn("Workspace was turned off, and wasn't ever turned on back again")
				isUsed.set(True)
			End If

			' if during this cycle we've used more memory then before - increase max count. we'll use it in future for optional reallocation
			If cycleAllocations.get() > maxCycle.get() Then
				If isDebug.get() Then
					log.info("Workspace [{}] device_{}, current cycle: {}; max cycle: {}", id, Nd4j.AffinityManager.getDeviceForCurrentThread(), cycleAllocations.get(), maxCycle.get())
				End If

				maxCycle.set(cycleAllocations.get())
			End If

			' checking, if we should reallocate this workspace to higher amount of memory
			If workspaceConfiguration.getPolicyLearning() <> LearningPolicy.NONE AndAlso maxCycle.get() > 0 Then
				'log.info("Delayed workspace {}, device_{} initialization starts...", id, Nd4j.getAffinityManager().getDeviceForCurrentThread());

				' if we're going to resize - we're probably safe to purge spilled allocations
				If externalCount.get() > 0 AndAlso (workspaceConfiguration.getPolicyReset() = ResetPolicy.BLOCK_LEFT OrElse resetPlanned.get()) Then
					clearExternalAllocations()
					resetPlanned.set(False)
				End If

				If (workspaceConfiguration.getPolicyLearning() = LearningPolicy.OVER_TIME AndAlso workspaceConfiguration.getCyclesBeforeInitialization() = cyclesCount_Conflict.intValue()) OrElse (workspaceConfiguration.getPolicyLearning() = LearningPolicy.FIRST_LOOP AndAlso currentSize_Conflict.get() = 0) Then
					'log.info("Initializing on cycle {}", cyclesCount.get());

					If Nd4j.WorkspaceManager.DebugMode <> DebugMode.SPILL_EVERYTHING Then
						initializeWorkspace()
					End If
				ElseIf currentSize_Conflict.get() > 0 AndAlso cycleAllocations.get() > 0 AndAlso workspaceConfiguration.getPolicySpill() = SpillPolicy.REALLOCATE AndAlso workspaceConfiguration.getPolicyReset() <> ResetPolicy.ENDOFBUFFER_REACHED Then
					'log.debug("Reinit on cycle {}; step: {}", cyclesCount.get(), stepsCount.get());

					If Nd4j.WorkspaceManager.DebugMode <> DebugMode.SPILL_EVERYTHING Then
						initializeWorkspace()
					End If
				End If
			End If


			' clearing pinned allocations that are old enough
			If pinnedCount.get() > 0 Then
				clearPinnedAllocations(False)
			End If

			' if we're in trimmed mode (preparing for reallocation of circular buffer) - we can do it 2 generations after
			If trimmedMode.get() AndAlso trimmedStep.get() + 2 < stepsCount.get() Then
				initialBlockSize_Conflict.set(maxCycle.get())
				initializeWorkspace()
				trimmedMode.set(False)
				trimmedStep.set(0)

				reset()
			End If

			lastCycleAllocations_Conflict.set(cycleAllocations.get())

			disabledCounter.set(0)


			If workspaceConfiguration.getPolicyReset() = ResetPolicy.BLOCK_LEFT Then
				reset()
			ElseIf workspaceConfiguration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso currentSize_Conflict.get() > 0 Then

				' for variable input we want to ensure alignment to max block, to avoid accidental buffer overruns
				Dim diff As Long = initialBlockSize_Conflict.get() - cycleAllocations.get()

				' we don't care about offsets if that's trimmed mode, offsets will be reset anyway upon reallocation
				If diff > 0 AndAlso Not trimmedMode.get() AndAlso deviceOffset_Conflict.get() > 0 Then

					If isDebug.get() Then
						log.info("Worskpace [{}]: Align to [{}]; diff: [{}]; block size: [{}]; currentOffset: [{}]; workspaceSize: [{}]; trimmedMode: {}", id, initialBlockSize_Conflict.get(), diff, cycleAllocations.get(), deviceOffset_Conflict.get(), currentSize_Conflict.get(), trimmedMode.get())
					End If

					deviceOffset_Conflict.getAndAdd(diff)
					hostOffset_Conflict.getAndAdd(diff)
				End If
			End If

			cycleAllocations.set(0)
		End Sub

		Protected Friend MustOverride Sub clearPinnedAllocations(ByVal extended As Boolean)

		Protected Friend MustOverride Sub clearExternalAllocations()

		Public Overridable Function notifyScopeEntered() As MemoryWorkspace
			' we should block stuff since we're going to invalidate spilled allocations
			' TODO: block on spilled allocations probably?

			Dim prev As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace

			' if we're opening the same workspace - just increase counter, and skip everything else
			If prev Is Me AndAlso isOpen.get() Then
				tagScope.incrementAndGet()
				Return Me
			End If

			' we'll need this in close() call, to restore previous workspace (if any)
			previousWorkspace = prev

			Nd4j.MemoryManager.CurrentWorkspace = Me
			isOpen.set(True)

			' resetting workspace to 0 offset (if anything), not applicable to circular mode, sure
			If workspaceConfiguration.getPolicyReset() = ResetPolicy.BLOCK_LEFT Then
				reset()
			End If

			' if we have any spilled allocations left from last cycle - purge them.
			If externalCount.get() > 0 AndAlso (workspaceConfiguration.getPolicyReset() = ResetPolicy.BLOCK_LEFT OrElse resetPlanned.get()) Then
				clearExternalAllocations()
				resetPlanned.set(False)
			End If

			cycleAllocations.set(0)
			disabledCounter.set(0)

			generationId_Conflict.incrementAndGet()

			Return Me
		End Function

		''' <summary>
		''' This method reset host/device offsets within workspace
		''' 
		''' PLEASE NOTE: Never call this method unless you realize all consequences
		''' </summary>
		Public Overridable Sub reset()
			'log.info("Resetting at device: {}; host: {};", deviceOffset.get(), hostOffset.get());
			hostOffset_Conflict.set(0)
			deviceOffset_Conflict.set(0)
		End Sub

		Protected Friend MustOverride Sub resetWorkspace()

		''' <summary>
		''' This method is shortcut to close() method
		''' 
		''' @return
		''' </summary>
		Public Overridable Function notifyScopeLeft() As MemoryWorkspace
			close()
			Return Me
		End Function

		''' <summary>
		''' This method allows to temporary disable this workspace, and issue allocations directly. </summary>
		''' <param name="isEnabled"> </param>
		Public Overridable Sub toggleWorkspaceUse(ByVal isEnabled As Boolean) Implements MemoryWorkspace.toggleWorkspaceUse
			isUsed.set(isEnabled)
		End Sub

		''' <summary>
		''' This method returns number of bytes allocated during last full cycle
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property LastCycleAllocations As Long Implements MemoryWorkspace.getLastCycleAllocations
			Get
				Return lastCycleAllocations_Conflict.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of bytes allocated during THIS cycle
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ThisCycleAllocations As Long Implements MemoryWorkspace.getThisCycleAllocations
			Get
				Return cycleAllocations.get()
			End Get
		End Property

		''' <summary>
		''' This method returns number of bytes of biggest cycle
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property MaxCycleAllocations As Long Implements MemoryWorkspace.getMaxCycleAllocations
			Get
				Return maxCycle.get()
			End Get
		End Property

		''' <summary>
		''' This method returns True if scope was opened, and not closed yet.
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property ScopeActive As Boolean Implements MemoryWorkspace.isScopeActive
			Get
				Return isOpen.get()
			End Get
		End Property

		Public Overridable Function tagOutOfScopeUse() As MemoryWorkspace
			tagScope.incrementAndGet()
			Return Me
		End Function

		Public Overrides Function ToString() As String
			Return "Nd4jWorkspace{" & "id='" & id & "'"c & ", currentSize=" & currentSize_Conflict.get() & "}"c
		End Function
	'
	'    @Data
	'    public static class GarbageWorkspaceReference extends WeakReference<MemoryWorkspace> {
	'        private PointersPair pointersPair;
	'        private String id;
	'        private Long threadId;
	'        private Queue<PointersPair> pinnedPointers;
	'        private List<PointersPair> externalPointers;
	'        private String key;
	'
	'        public GarbageWorkspaceReference(MemoryWorkspace referent, ReferenceQueue<? super MemoryWorkspace> queue) {
	'            super(referent, queue);
	'            this.pointersPair = ((Nd4jWorkspace) referent).workspace;
	'
	'            this.id = referent.getId();
	'            this.threadId = referent.getThreadId();
	'            this.pinnedPointers = ((Nd4jWorkspace) referent).pinnedAllocations;
	'            this.externalPointers = ((Nd4jWorkspace) referent).externalAllocations;
	'
	'            this.key = id + "_" + threadId;
	'        }
	'    }
	'    
	End Class

End Namespace