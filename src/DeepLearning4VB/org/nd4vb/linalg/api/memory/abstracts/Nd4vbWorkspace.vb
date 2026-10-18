Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.enums
Imports DeepLearning4VB.org.slf4j
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports Deallocatable = DeepLearning4VB.org.nd4vb.linalg.api.memory.Deallocatable
Imports Deallocator = DeepLearning4VB.org.nd4vb.linalg.api.memory.Deallocator
Imports MemoryManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryManager
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports MemoryWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspaceManager
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports SysMath = System.Math

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

Namespace org.nd4vb.linalg.api.memory.abstracts

    ''' <summary>
    ''' The shared logic of workspaces (Java <c>Nd4jWorkspace</c>): scopes (entered, borrowed, left, tagged out-of-scope
    ''' use), generations, cycles, the learning, overallocation, spill and reset policies, and the allocation statistics.
    ''' </summary>
    ''' <remarks>
    ''' <para>In ND4VB memory always comes from the garbage-collected heap (docs/PHASE2_DESIGN.md): a workspace hands out
    ''' no memory. Data buffers created in a workspace allocate their own arrays and report the allocation through
    ''' <see cref="alloc(Long, MemoryKind, DataType, Boolean)"/>, which applies Java's bookkeeping exactly (32-byte
    ''' alignment, host offset, cycle allocations, spilled and pinned allocations, circular resets, the "Workspace is full"
    ''' error) and returns the allocation's offset within the workspace. Learning, overallocation and spill policies only
    ''' affect these statistics.</para>
    ''' <para>Like Java, a workspace belongs to the thread that created it; it is not thread-safe.</para>
    ''' <para>Differences from Java:
    ''' <list type="bullet">
    ''' <item>The memory manager, workspace manager and device come from the constructor that takes them; the Java
    ''' constructors read them from <c>Nd4vb</c> (Java: <c>Nd4j</c>). The current workspace is read and set through that
    ''' memory manager, and the debug mode through that workspace manager.</item>
    ''' <item><c>alloc</c> returns the offset within the workspace (or -1 for spilled and pinned allocations) instead of a
    ''' pointer; <c>free(Pointer)</c> and the <c>PointersPair</c> bookkeeping are native-only and not ported (spilled and
    ''' pinned allocations are kept as <see cref="AllocationRecord"/>s).</item>
    ''' <item><c>close()</c> does not call <c>Nd4j.getExecutioner().commit()</c>: the managed executioner runs ops
    ''' synchronously, so there is nothing to commit (the CPU backend's commit was a no-op too).</item>
    ''' <item>MMAP workspaces map no file. A <c>tempFilePath</c> that names a file shorter than the initial size is filled
    ''' with zeros as in Java (a longer file sets the initial size), but no temporary file is created when the path is not
    ''' given.</item>
    ''' <item>A circular workspace resets and retries an allocation only when that can help (its offset is not 0); Java
    ''' recursed without end when a single allocation was larger than the whole workspace.</item>
    ''' </list></para>
    ''' </remarks>
    Public MustInherit Class Nd4vbWorkspace
        Implements MemoryWorkspace

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(Nd4vbWorkspace))

        ''' <summary>A spilled (external) or pinned allocation (Java kept the pointers in <c>PointersPair</c>s).</summary>
        Protected Friend NotInheritable Class AllocationRecord
            ''' <summary>Bytes allocated (aligned).</summary>
            Public ReadOnly RequiredMemory As Long
            ''' <summary>The workspace step (circular generation) of the allocation.</summary>
            Public ReadOnly AllocationCycle As Long

            Public Sub New(ByVal requiredMemory As Long, ByVal allocationCycle As Long)
                Me.RequiredMemory = requiredMemory
                Me.AllocationCycle = allocationCycle
            End Sub
        End Class

        ''' <summary>Id of a resource for the deallocator service.</summary>
        Public MustOverride ReadOnly Property UniqueId As String Implements Deallocatable.UniqueId
        ''' <summary>The deallocator that releases this workspace's bookkeeping after it is garbage collected.</summary>
        Public MustOverride Function deallocator() As Deallocator Implements Deallocatable.deallocator
        ''' <summary>The device of this workspace.</summary>
        Public MustOverride Function targetDevice() As Integer Implements Deallocatable.targetDevice
        ''' <summary>The offset of the primary (host) allocation within the workspace.</summary>
        Public MustOverride ReadOnly Property PrimaryOffset As Long Implements MemoryWorkspace.PrimaryOffset

        Protected Friend deviceIdField As Integer
        Protected Friend threadIdField As Long?
        Protected Friend workspaceTypeField As MemoryWorkspace.Type = MemoryWorkspace.Type.SCOPED

        ''' <summary>Extra bytes Java allocated beyond the workspace size (counted in the allocation tracker).</summary>
        Protected Friend Const SAFETY_OFFSET As Long = 1024L

        Protected Friend idField As String

        Protected Friend currentSizeField As Long = 0
        Protected Friend hostOffsetField As Long = 0
        Protected Friend deviceOffsetField As Long = 0

        ''' <summary>The memory manager whose current workspace this workspace sets.</summary>
        Protected Friend memoryManager As MemoryManager
        ''' <summary>The workspace manager that created this workspace (its debug mode applies), or Nothing.</summary>
        Protected Friend workspaceManager As MemoryWorkspaceManager

        Protected Friend isLearning As Boolean = True
        Protected Friend isUsed As Boolean = True

        Protected Friend disabledCounter As Long = 0

        Protected Friend cyclesCountField As Long = 0
        Protected Friend stepsCount As Long = 0
        Protected Friend stepsNumber As Integer = 1

        Protected Friend lastCycleAllocationsField As Long = 0
        Protected Friend cycleAllocations As Long = 0
        Protected Friend spilledAllocationsSize As Long = 0
        Protected Friend pinnedAllocationsSize As Long = 0
        Protected Friend maxCycle As Long = 0
        Protected Friend resetPlanned As Boolean = False
        Protected Friend isOpen As Boolean = False
        Protected Friend isInit As Boolean = False
        Protected Friend isOver As Boolean = False
        Protected Friend isBorrowed As Boolean = False

        Protected Friend tagScope As Integer = 0

        Protected Friend isDebug As Boolean = False
        Protected Friend externalCount As Integer = 0
        Protected Friend pinnedCount As Integer = 0

        Protected Friend trimmedMode As Boolean = False
        Protected Friend trimmedStep As Long = 0

        Protected Friend ReadOnly workspaceConfigurationField As WorkspaceConfiguration

        ''' <summary>External (spilled) allocations are purged at the end of loop.</summary>
        Protected Friend externalAllocations As New List(Of AllocationRecord)()

        ''' <summary>Pinned allocations are purged with delay, used for circular mode only.</summary>
        Protected Friend pinnedAllocations As New LinkedList(Of AllocationRecord)()

        Protected Friend previousWorkspaceField As MemoryWorkspace
        Protected Friend borrowingWorkspace As MemoryWorkspace

        Protected Friend initialBlockSizeField As Long = 0

        Protected Friend guid As String

        ''' <summary>The memory-mapped file of an MMAP workspace (a path), or Nothing.</summary>
        Protected Friend tempFile As String

        Protected Friend generationIdField As Long = 0

        ''' <summary>This field is used as alignment base for all allocations within this workspace.</summary>
        Public Const alignmentBase As Integer = 32

        ''' <summary>A workspace with the default id; the managers come from <c>Nd4vb</c>.</summary>
        Public Sub New(ByVal configuration As WorkspaceConfiguration)
            Me.New(configuration, MemoryWorkspace.Constants.DEFAULT_ID)
        End Sub

        ''' <summary>A workspace with the given id; the memory manager, workspace manager and device come from <c>Nd4vb</c>.</summary>
        Public Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String)
            Me.New(configuration, workspaceId, Nd4vbT.WorkspaceManager, Nd4vbT.MemoryManager, Nd4vbT.AffinityManager.DeviceForCurrentThread.GetValueOrDefault())
        End Sub

        ''' <summary>
        ''' A workspace that uses the given memory manager (for the current workspace) and workspace manager (for the debug
        ''' mode and the UUID; may be Nothing) on device <paramref name="deviceId"/>.
        ''' </summary>
        Protected Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String, ByVal workspaceManager As MemoryWorkspaceManager, ByVal memoryManager As MemoryManager, ByVal deviceId As Integer)
            If configuration Is Nothing Then
                Throw New ArgumentNullException(NameOf(configuration))
            End If
            If workspaceId Is Nothing Then
                Throw New ArgumentNullException(NameOf(workspaceId))
            End If
            If memoryManager Is Nothing Then
                Throw New ArgumentNullException(NameOf(memoryManager))
            End If
            Me.workspaceConfigurationField = configuration
            Me.idField = workspaceId
            Me.threadIdField = Thread.CurrentThread.ManagedThreadId
            Me.workspaceManager = workspaceManager
            Me.guid = If(workspaceManager Is Nothing, Nothing, workspaceManager.UUID)
            Me.memoryManager = memoryManager
            Me.deviceIdField = deviceId

            ' and actual workspace allocation
            currentSizeField = configuration.getInitialSize()

            If configuration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED Then
                workspaceTypeField = MemoryWorkspace.Type.CIRCULAR
            Else
                workspaceTypeField = MemoryWorkspace.Type.SCOPED
            End If

            If configuration.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso configuration.getPolicyAllocation() = AllocationPolicy.OVERALLOCATE Then
                If configuration.getOverallocationLimit() < 1.0 Then
                    Throw New ND4VBIllegalStateException("For cyclic workspace overallocation should be positive integral value.")
                End If

                stepsNumber = CInt(SysMath.Truncate(configuration.getOverallocationLimit() + 1))
                log.trace("Steps: {}", stepsNumber)
            End If

            ' validate mmap option
            If configuration.getPolicyLocation() = LocationPolicy.MMAP Then
                ' file path should be either non-null
                If configuration.getTempFilePath() IsNot Nothing Then
                    tempFile = configuration.getTempFilePath()
                    Dim fileLength As Long = If(File.Exists(tempFile), New FileInfo(tempFile).Length, 0L)
                    If fileLength = 0 OrElse fileLength < configuration.getInitialSize() Then
                        If configuration.getInitialSize() > 0 Then
                            fillFile(tempFile, configuration.getInitialSize())
                        Else
                            Throw New ND4VBIllegalStateException("Memory-mapped file should have positive length.")
                        End If
                    Else
                        configuration.setInitialSize(fileLength)
                        currentSizeField = fileLength
                    End If
                ElseIf configuration.getInitialSize() <= 0 Then
                    Throw New ND4VBIllegalStateException("MMAP target file path should be non-null or workspace initialSize should be >0 for temp file")
                End If
            End If

            init()
        End Sub

        ''' <summary>This method returns Type of this workspace.</summary>
        Public Overridable ReadOnly Property WorkspaceType As MemoryWorkspace.Type Implements MemoryWorkspace.WorkspaceType
            Get
                Return Me.workspaceTypeField
            End Get
        End Property

        ''' <summary>Writes <paramref name="length"/> zero bytes (rounded up to 16 KiB blocks) to the file at <paramref name="path"/>.</summary>
        Public Shared Sub fillFile(ByVal path As String, ByVal length As Long)
            Dim buffer(16383) As Byte
            Using fos As New FileStream(path, FileMode.Create, FileAccess.Write)
                Dim written As Long = 0
                Do While written < length
                    fos.Write(buffer, 0, buffer.Length)
                    written += buffer.Length
                Loop
            End Using
        End Sub

        ''' <summary>The configuration this workspace was created with.</summary>
        Public Overridable ReadOnly Property WorkspaceConfiguration As WorkspaceConfiguration Implements MemoryWorkspace.WorkspaceConfiguration
            Get
                Return workspaceConfigurationField
            End Get
        End Property

        ''' <summary>The id of this workspace.</summary>
        Public Overridable ReadOnly Property Id As String Implements MemoryWorkspace.Id
            Get
                Return idField
            End Get
        End Property

        ''' <summary>The device of this workspace.</summary>
        Public Overridable ReadOnly Property DeviceId As Integer Implements MemoryWorkspace.DeviceId
            Get
                Return deviceIdField
            End Get
        End Property

        ''' <summary>The managed id of the thread that created this workspace.</summary>
        Public Overridable ReadOnly Property ThreadId As Long? Implements MemoryWorkspace.ThreadId
            Get
                Return threadIdField
            End Get
        End Property

        ''' <summary>The current generation: incremented each time a new scope is entered.</summary>
        Public Overridable ReadOnly Property GenerationId As Long Implements MemoryWorkspace.GenerationId
            Get
                Return generationIdField
            End Get
        End Property

        ''' <summary>This method returns step number. Viable only in circular mode.</summary>
        Public Overridable ReadOnly Property StepNumber As Long
            Get
                Return stepsCount
            End Get
        End Property

        ''' <summary>This method returns number of bytes in spilled allocations.</summary>
        Public Overridable ReadOnly Property SpilledSize As Long
            Get
                Return spilledAllocationsSize
            End Get
        End Property

        ''' <summary>This method returns number of bytes in pinned allocations.</summary>
        Public Overridable ReadOnly Property PinnedSize As Long
            Get
                Return pinnedAllocationsSize
            End Get
        End Property

        ''' <summary>This method returns number of bytes for first block of circular workspace.</summary>
        Public Overridable ReadOnly Property InitialBlockSize As Long
            Get
                Return initialBlockSizeField
            End Get
        End Property

        ''' <summary>This method returns parent Workspace, if any. Null if there's none.</summary>
        Public Overridable ReadOnly Property ParentWorkspace As MemoryWorkspace Implements MemoryWorkspace.ParentWorkspace
            Get
                Return previousWorkspaceField
            End Get
        End Property

        ''' <summary>
        ''' Sets the previous workspace, the one that becomes current when this workspace is closed. Use only if you are
        ''' fully aware of the consequences.
        ''' </summary>
        Public Overridable WriteOnly Property PreviousWorkspace As MemoryWorkspace Implements MemoryWorkspace.PreviousWorkspace
            Set(ByVal value As MemoryWorkspace)
                previousWorkspaceField = value
            End Set
        End Property

        ''' <summary>This method returns current device memory offset within workspace.</summary>
        Public Overridable ReadOnly Property DeviceOffset As Long
            Get
                Return deviceOffsetField
            End Get
        End Property

        ''' <summary>This method returns current host memory offset within workspace.</summary>
        Public Overridable ReadOnly Property HostOffset As Long
            Get
                Return hostOffsetField
            End Get
        End Property

        ''' <summary>This method returns current amount of memory allocated for workspace (host memory).</summary>
        Public Overridable ReadOnly Property CurrentSize As Long Implements MemoryWorkspace.CurrentSize
            Get
                Return currentSizeField
            End Get
        End Property

        ''' <summary>The current offset within the workspace (the host offset).</summary>
        Public Overridable ReadOnly Property CurrentOffset As Long Implements MemoryWorkspace.CurrentOffset
            Get
                Return hostOffsetField
            End Get
        End Property

        ''' <summary>Applies the overallocation policy and the maximum size to the current size (the implementation then "allocates" it).</summary>
        Protected Friend Overridable Sub init()
            ' in case of MMAP we don't want any learning applied
            If workspaceConfigurationField.getPolicyLocation() = LocationPolicy.MMAP AndAlso workspaceConfigurationField.getPolicyLearning() <> LearningPolicy.NONE Then
                Throw New ArgumentException("Workspace backed by memory-mapped file can't have LearningPolicy defined")
            End If

            ' we don't want overallocation in case of MMAP
            If currentSizeField > 0 AndAlso workspaceConfigurationField.getPolicyLocation() <> LocationPolicy.MMAP Then
                If Not isOver Then
                    If workspaceConfigurationField.getPolicyAllocation() = AllocationPolicy.OVERALLOCATE AndAlso workspaceConfigurationField.getOverallocationLimit() > 0 Then
                        currentSizeField += CLng(SysMath.Truncate(currentSizeField * workspaceConfigurationField.getOverallocationLimit()))
                        isOver = True
                    End If
                End If

                If workspaceConfigurationField.getMaxSize() > 0 AndAlso currentSizeField > workspaceConfigurationField.getMaxSize() Then
                    currentSizeField = workspaceConfigurationField.getMaxSize()
                End If
            End If
        End Sub

        ''' <summary>The debug mode of the workspace manager (DISABLED without one).</summary>
        Protected Friend Function currentDebugMode() As DebugMode
            If workspaceManager Is Nothing Then
                Return DebugMode.DISABLED
            End If
            Return workspaceManager.DebugMode
        End Function

        ''' <summary>Same as <c>alloc(requiredMemory, MemoryKind.HOST, type, initialize)</c>.</summary>
        Public Overridable Function alloc(ByVal requiredMemory As Long, ByVal type As DataType, ByVal initialize As Boolean) As Long
            Return alloc(requiredMemory, MemoryKind.HOST, type, initialize)
        End Function

        ''' <summary>This method enabled debugging mode for this workspace.</summary>
        Public Overridable Sub enableDebug(ByVal reallyEnable As Boolean) Implements MemoryWorkspace.enableDebug
            Me.isDebug = reallyEnable
        End Sub

        ''' <summary>
        ''' Records an allocation of <paramref name="requiredMemory"/> bytes (aligned up to <see cref="alignmentBase"/>) the way
        ''' Java's workspace allocates: from the workspace when it fits (the result is the offset within the workspace),
        ''' otherwise as a spilled (EXTERNAL/REALLOCATE spill policy) or pinned (circular trimmed mode) allocation (the result
        ''' is -1); a circular workspace resets to offset 0 first. The memory itself comes from the managed heap.
        ''' </summary>
        ''' <exception cref="ND4VBIllegalStateException">The workspace is full and its spill policy is FAIL.</exception>
        Public Overridable Function alloc(ByVal requiredMemory As Long, ByVal kind As MemoryKind, ByVal type As DataType, ByVal initialize As Boolean) As Long
            ' we enforce alignment to ensure CUDA doesn't blame us
            Dim div As Long = requiredMemory Mod alignmentBase
            If div <> 0 Then
                requiredMemory += (alignmentBase - div)
            End If

            ' shortcut made to skip workspace
            If Not isUsed Then
                disabledCounter += 1
                If disabledCounter Mod 10 = 0 Then
                    log.warn("Workspace was turned off, and wasn't enabled after {} allocations", disabledCounter)
                End If

                externalAllocations.Add(New AllocationRecord(requiredMemory, stepsCount))
                Return -1
            End If

            ' Trimmed mode is possible for cyclic workspace mode. Used in AsyncDataSetIterator, MQ, etc.
            ' Basically idea is simple: if one of datasets coming out of iterator has size higher then expected - we should reallocate workspace to match this size.
            ' So, we switch to trimmed mode, and all allocations will be "pinned", and eventually workspace will be reallocated.
            Dim trimmer As Boolean = (workspaceConfigurationField.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso requiredMemory + cycleAllocations > initialBlockSizeField AndAlso initialBlockSizeField > 0) OrElse trimmedMode

            If trimmer AndAlso workspaceConfigurationField.getPolicySpill() = SpillPolicy.REALLOCATE AndAlso Not trimmedMode Then
                trimmedMode = True
                trimmedStep = stepsCount
            End If

            ' if size is enough - allocate from workspace
            If hostOffsetField + requiredMemory <= currentSizeField AndAlso Not trimmer AndAlso currentDebugMode() <> DebugMode.SPILL_EVERYTHING Then
                cycleAllocations += requiredMemory
                Dim prevOffset As Long = hostOffsetField
                hostOffsetField += requiredMemory
                deviceOffsetField = hostOffsetField

                If isDebug Then
                    log.info("Workspace [{}]: Allocating array of {} bytes, prevOffset: {}; currentOffset: {}", idField, requiredMemory, prevOffset, hostOffsetField)
                End If

                Return prevOffset
            Else
                ' if current workspace isn't enough - we allocate it separately as spilled (or pinned, in case of circular mode)

                ' in case of circular mode - we just reset offsets, and start from the beginning of the workspace
                If workspaceConfigurationField.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso currentSizeField > 0 AndAlso Not trimmer AndAlso currentDebugMode() <> DebugMode.SPILL_EVERYTHING AndAlso hostOffsetField > 0 Then
                    reset()
                    resetPlanned = True
                    Return alloc(requiredMemory, kind, type, initialize)
                End If

                ' updating respective counters
                If Not trimmer Then
                    spilledAllocationsSize += requiredMemory
                Else
                    pinnedAllocationsSize += requiredMemory
                End If

                If isDebug Then
                    log.info("Workspace [{}]: step: {}, spilled  {} bytes", idField, stepsCount, requiredMemory)
                End If

                Select Case workspaceConfigurationField.getPolicySpill()
                    Case SpillPolicy.REALLOCATE, SpillPolicy.EXTERNAL
                        cycleAllocations += requiredMemory
                        If Not trimmer Then
                            externalCount += 1
                            externalAllocations.Add(New AllocationRecord(requiredMemory, stepsCount))
                        Else
                            pinnedCount += 1
                            pinnedAllocations.AddLast(New AllocationRecord(requiredMemory, stepsCount))
                        End If
                        Return -1
                    Case Else
                        Throw New ND4VBIllegalStateException("Can't allocate memory: Workspace is full")
                End Select
            End If
        End Function

        ''' <summary>
        ''' Re-sizes the workspace from what it learned (the largest cycle), within the maximum and minimum sizes, applying
        ''' overallocation; has no effect on an initialized workspace unless reallocation is due.
        ''' </summary>
        Public Overridable Sub initializeWorkspace() Implements MemoryWorkspace.initializeWorkspace
            ' we can reallocate this workspace to larger size if that's needed and allowed by configuration
            If (currentSizeField < maxCycle OrElse currentSizeField < cycleAllocations) AndAlso workspaceConfigurationField.getPolicySpill() = SpillPolicy.REALLOCATE AndAlso (workspaceConfigurationField.getMaxSize() = 0 OrElse (maxCycle < workspaceConfigurationField.getMaxSize())) Then
                If workspaceConfigurationField.getPolicyReset() <> ResetPolicy.ENDOFBUFFER_REACHED Then
                    destroyWorkspace(True)
                    isInit = False
                End If
            End If

            ' if we're in cyclic mode, we do reallocations only after 2 full cycles, to avoid race conditions
            If trimmedMode AndAlso trimmedStep + 2 < stepsCount Then
                destroyWorkspace(False)
                isInit = False
                isOver = False
            End If

            If Not isInit Then
                If workspaceConfigurationField.getPolicyLearning() <> LearningPolicy.NONE Then
                    If workspaceConfigurationField.getMaxSize() > 0 Then
                        currentSizeField = SysMath.Min(maxCycle, workspaceConfigurationField.getMaxSize())
                    Else
                        currentSizeField = maxCycle
                    End If

                    ' if we're on cyclic mode, let's add 30% to size, just to reduce number of reallocations
                    If workspaceConfigurationField.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED Then
                        currentSizeField = CLng(SysMath.Truncate(currentSizeField * 1.3))
                        currentSizeField += 8 - (currentSizeField Mod 8)
                        maxCycle = currentSizeField
                    End If

                    ' we're updating single block size for circular mode, will be used for alignment later
                    initialBlockSizeField = currentSizeField

                    ' handling optional overallocation here, however it's usually good idea to use it everywhere, to avoid frequent realloc calls
                    If Not isOver Then
                        If workspaceConfigurationField.getPolicyAllocation() = AllocationPolicy.OVERALLOCATE AndAlso workspaceConfigurationField.getOverallocationLimit() > 0 AndAlso currentSizeField > 0 Then
                            currentSizeField = currentSizeField + CLng(SysMath.Truncate(currentSizeField * workspaceConfigurationField.getOverallocationLimit()))
                            isOver = True
                        End If
                    End If

                    If workspaceConfigurationField.getMinSize() > 0 AndAlso currentSizeField < workspaceConfigurationField.getMinSize() Then
                        currentSizeField = workspaceConfigurationField.getMinSize()
                    End If

                    ' purge spilled allocations
                    If externalCount > 0 AndAlso (workspaceConfigurationField.getPolicyReset() = ResetPolicy.BLOCK_LEFT OrElse resetPlanned) Then
                        clearExternalAllocations()
                        resetPlanned = False
                    End If

                    ' calling for implementation-specific workspace initialization. basically allocation happens there
                    init()
                End If
            End If
        End Sub

        ''' <summary>This method returns number of spilled allocations, that can be purged at the end of block.</summary>
        Public Overridable ReadOnly Property NumberOfExternalAllocations As Integer
            Get
                Return externalCount
            End Get
        End Property

        ''' <summary>This method returns number of pinned allocations, they can be purged after 2 steps (circular mode only).</summary>
        Public Overridable ReadOnly Property NumberOfPinnedAllocations As Integer
            Get
                Return pinnedCount
            End Get
        End Property

        ''' <summary>Same as <c>destroyWorkspace(True)</c>.</summary>
        Public Overridable Sub destroyWorkspace() Implements MemoryWorkspace.destroyWorkspace
            destroyWorkspace(True)
        End Sub

        ''' <summary>Releases the workspace: its size becomes 0 and the offsets are reset; <paramref name="extended"/> also clears spilled allocations.</summary>
        Public Overridable Sub destroyWorkspace(ByVal extended As Boolean) Implements MemoryWorkspace.destroyWorkspace
            currentSizeField = 0
            reset()

            If extended Then
                clearExternalAllocations()
            End If
        End Sub

        ''' <summary>
        ''' TEMPORARY enters this workspace, without reset applied: it becomes the current workspace until <see cref="close"/>,
        ''' which restores the workspace that was current before.
        ''' </summary>
        ''' <exception cref="ND4VBIllegalStateException">The workspace is already borrowed.</exception>
        Public Overridable Function notifyScopeBorrowed() As MemoryWorkspace Implements MemoryWorkspace.notifyScopeBorrowed
            If isBorrowed Then
                Throw New ND4VBIllegalStateException("Workspace [" & idField & "]: Can't borrow from borrowed workspace")
            End If

            borrowingWorkspace = memoryManager.CurrentWorkspace
            isBorrowed = True

            memoryManager.CurrentWorkspace = Me

            Return Me
        End Function

        ''' <summary>The number of closed cycles (scopes).</summary>
        Public Overridable ReadOnly Property CyclesCount As Long
            Get
                Return cyclesCountField
            End Get
        End Property

        ''' <summary>
        ''' Leaves the scope: a borrowed workspace restores the borrowing workspace; a nested re-entry (or tagged out-of-scope
        ''' use) only decrements its counter; otherwise the previous workspace becomes current, the cycle statistics are
        ''' updated, learning and reallocation happen when due, and a BLOCK_LEFT workspace resets.
        ''' </summary>
        Public Overridable Sub close() Implements MemoryWorkspace.close
            ' first we check if this workspace was borrowed. if yes - just close without reset.
            If isBorrowed Then
                If tagScope > 0 Then
                    tagScope -= 1
                    If tagScope = 0 Then
                        memoryManager.CurrentWorkspace = Me
                    End If
                    Return
                End If

                isBorrowed = False
                memoryManager.CurrentWorkspace = borrowingWorkspace
                Return
            End If

            ' next we check, if the same workspace was opened multiple times sequentially. then we just decrement counter, without reset
            If tagScope > 0 Then
                tagScope -= 1
                If tagScope = 0 Then
                    memoryManager.CurrentWorkspace = Me
                End If
                Return
            End If

            ' Java committed pending ops here (Nd4j.getExecutioner().commit()); the managed executioner runs synchronously.

            ' since this workspace block is finished, we restore previous one. Even if it's null
            memoryManager.CurrentWorkspace = previousWorkspaceField
            isOpen = False

            ' just counter for cycles/blocks
            cyclesCountField += 1
            If cyclesCountField > 1 And (cyclesCountField - 1) Mod stepsNumber = 0 Then
                ' this counter is for cyclic mode, it counts generations, full loops over buffer
                stepsCount += 1
            End If

            If Not isUsed Then
                log.warn("Workspace was turned off, and wasn't ever turned on back again")
                isUsed = True
            End If

            ' if during this cycle we've used more memory then before - increase max count. we'll use it in future for optional reallocation
            If cycleAllocations > maxCycle Then
                If isDebug Then
                    log.info("Workspace [{}] device_{}, current cycle: {}; max cycle: {}", idField, deviceIdField, cycleAllocations, maxCycle)
                End If

                maxCycle = cycleAllocations
            End If

            ' checking, if we should reallocate this workspace to higher amount of memory
            If workspaceConfigurationField.getPolicyLearning() <> LearningPolicy.NONE AndAlso maxCycle > 0 Then
                ' if we're going to resize - we're probably safe to purge spilled allocations
                If externalCount > 0 AndAlso (workspaceConfigurationField.getPolicyReset() = ResetPolicy.BLOCK_LEFT OrElse resetPlanned) Then
                    clearExternalAllocations()
                    resetPlanned = False
                End If

                If (workspaceConfigurationField.getPolicyLearning() = LearningPolicy.OVER_TIME AndAlso workspaceConfigurationField.getCyclesBeforeInitialization() = CInt(cyclesCountField)) OrElse (workspaceConfigurationField.getPolicyLearning() = LearningPolicy.FIRST_LOOP AndAlso currentSizeField = 0) Then
                    If currentDebugMode() <> DebugMode.SPILL_EVERYTHING Then
                        initializeWorkspace()
                    End If
                ElseIf currentSizeField > 0 AndAlso cycleAllocations > 0 AndAlso workspaceConfigurationField.getPolicySpill() = SpillPolicy.REALLOCATE AndAlso workspaceConfigurationField.getPolicyReset() <> ResetPolicy.ENDOFBUFFER_REACHED Then
                    If currentDebugMode() <> DebugMode.SPILL_EVERYTHING Then
                        initializeWorkspace()
                    End If
                End If
            End If

            ' clearing pinned allocations that are old enough
            If pinnedCount > 0 Then
                clearPinnedAllocations(False)
            End If

            ' if we're in trimmed mode (preparing for reallocation of circular buffer) - we can do it 2 generations after
            If trimmedMode AndAlso trimmedStep + 2 < stepsCount Then
                initialBlockSizeField = maxCycle
                initializeWorkspace()
                trimmedMode = False
                trimmedStep = 0

                reset()
            End If

            lastCycleAllocationsField = cycleAllocations

            disabledCounter = 0

            If workspaceConfigurationField.getPolicyReset() = ResetPolicy.BLOCK_LEFT Then
                reset()
            ElseIf workspaceConfigurationField.getPolicyReset() = ResetPolicy.ENDOFBUFFER_REACHED AndAlso currentSizeField > 0 Then
                ' for variable input we want to ensure alignment to max block, to avoid accidental buffer overruns
                Dim diff As Long = initialBlockSizeField - cycleAllocations

                ' we don't care about offsets if that's trimmed mode, offsets will be reset anyway upon reallocation
                If diff > 0 AndAlso Not trimmedMode AndAlso deviceOffsetField > 0 Then
                    If isDebug Then
                        log.info("Workspace [{}]: Align to [{}]; diff: [{}]; block size: [{}]; currentOffset: [{}]; workspaceSize: [{}]", idField, initialBlockSizeField, diff, cycleAllocations, deviceOffsetField, currentSizeField)
                    End If

                    deviceOffsetField += diff
                    hostOffsetField += diff
                End If
            End If

            cycleAllocations = 0
        End Sub

        ''' <summary>Same as <see cref="close"/> (Java <c>AutoCloseable</c>; VB <c>Using</c> blocks).</summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            close()
        End Sub

        ''' <summary>Releases pinned allocations that are at least two steps old (all of them when <paramref name="extended"/>).</summary>
        Protected Friend MustOverride Sub clearPinnedAllocations(ByVal extended As Boolean)

        ''' <summary>Releases the spilled allocations.</summary>
        Protected Friend MustOverride Sub clearExternalAllocations()

        ''' <summary>
        ''' Starts a new scope: this workspace becomes current (the previous one is remembered for <see cref="close"/>), a
        ''' BLOCK_LEFT workspace resets, spilled allocations of the last cycle are purged and the generation is incremented.
        ''' Re-entering the current open workspace only increments a counter.
        ''' </summary>
        Public Overridable Function notifyScopeEntered() As MemoryWorkspace Implements MemoryWorkspace.notifyScopeEntered
            Dim prev As MemoryWorkspace = memoryManager.CurrentWorkspace

            ' if we're opening the same workspace - just increase counter, and skip everything else
            If prev Is Me AndAlso isOpen Then
                tagScope += 1
                Return Me
            End If

            ' we'll need this in close() call, to restore previous workspace (if any)
            previousWorkspaceField = prev

            memoryManager.CurrentWorkspace = Me
            isOpen = True

            ' resetting workspace to 0 offset (if anything), not applicable to circular mode, sure
            If workspaceConfigurationField.getPolicyReset() = ResetPolicy.BLOCK_LEFT Then
                reset()
            End If

            ' if we have any spilled allocations left from last cycle - purge them.
            If externalCount > 0 AndAlso (workspaceConfigurationField.getPolicyReset() = ResetPolicy.BLOCK_LEFT OrElse resetPlanned) Then
                clearExternalAllocations()
                resetPlanned = False
            End If

            cycleAllocations = 0
            disabledCounter = 0

            generationIdField += 1

            Return Me
        End Function

        ''' <summary>Resets the host/device offsets within the workspace. Never call this method unless you realize all consequences.</summary>
        Public Overridable Sub reset()
            hostOffsetField = 0
            deviceOffsetField = 0
        End Sub

        ''' <summary>Implementation-specific reset of the workspace memory.</summary>
        Protected Friend MustOverride Sub resetWorkspace()

        ''' <summary>Shortcut to <see cref="close"/>.</summary>
        Public Overridable Function notifyScopeLeft() As MemoryWorkspace Implements MemoryWorkspace.notifyScopeLeft
            close()
            Return Me
        End Function

        ''' <summary>Temporarily disables (or re-enables) this workspace: allocations then go directly to memory (and are recorded as external).</summary>
        Public Overridable Sub toggleWorkspaceUse(ByVal isEnabled As Boolean) Implements MemoryWorkspace.toggleWorkspaceUse
            isUsed = isEnabled
        End Sub

        ''' <summary>This method returns number of bytes allocated during last full cycle.</summary>
        Public Overridable ReadOnly Property LastCycleAllocations As Long Implements MemoryWorkspace.LastCycleAllocations
            Get
                Return lastCycleAllocationsField
            End Get
        End Property

        ''' <summary>This method returns number of bytes allocated during THIS cycle.</summary>
        Public Overridable ReadOnly Property ThisCycleAllocations As Long Implements MemoryWorkspace.ThisCycleAllocations
            Get
                Return cycleAllocations
            End Get
        End Property

        ''' <summary>This method returns number of bytes of biggest cycle.</summary>
        Public Overridable ReadOnly Property MaxCycleAllocations As Long Implements MemoryWorkspace.MaxCycleAllocations
            Get
                Return maxCycle
            End Get
        End Property

        ''' <summary>True if a scope was opened and not closed yet.</summary>
        Public Overridable ReadOnly Property ScopeActive As Boolean Implements MemoryWorkspace.ScopeActive
            Get
                Return isOpen
            End Get
        End Property

        ''' <summary>Marks a temporary use out of this workspace's scope: the next <see cref="close"/> only undoes the mark.</summary>
        Public Overridable Function tagOutOfScopeUse() As MemoryWorkspace Implements MemoryWorkspace.tagOutOfScopeUse
            tagScope += 1
            Return Me
        End Function

        ''' <summary>Java's rendering: <c>Nd4jWorkspace{id='...', currentSize=...}</c> with the ND4VB class name.</summary>
        Public Overrides Function ToString() As String
            Return "Nd4vbWorkspace{" & "id='" & idField & "'" & ", currentSize=" & currentSizeField & "}"
        End Function

    End Class

End Namespace
