' DeepLearning4VB: the managed backend's workspace (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.workspace.CpuWorkspace.

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.enums
Imports DeepLearning4VB.org.slf4j
Imports Nd4vbWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.Nd4vbWorkspace
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration

Namespace org.nd4vb.linalg.managed.memory

    ''' <summary>
    ''' The workspace of the managed backend (Java <c>CpuWorkspace</c>). It keeps Java's scoping rules and statistics (see
    ''' <see cref="Nd4vbWorkspace"/>); its memory "block" is bookkeeping only: initializing a RAM workspace of
    ''' <c>n</c> bytes records <c>n + SAFETY_OFFSET</c> workspace bytes in the <see cref="AllocationsTracker"/> (as Java did),
    ''' and destroying it releases them.
    ''' </summary>
    ''' <remarks>
    ''' Differences from Java: the tracker is released by exactly the amount it was charged (Java released 1024 bytes less);
    ''' <see cref="UniqueId"/> is fixed when first read (Java returned a new value on every call); an MMAP workspace maps no
    ''' file. Native-only members not ported: <c>mmap</c>, <c>workspace()</c>, <c>pinnedPointers()</c>,
    ''' <c>externalPointers()</c> (the pointer lists).
    ''' </remarks>
    Public Class ManagedWorkspace
        Inherits Nd4vbWorkspace

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(ManagedWorkspace))
        Private Shared idCounter As Long = 0

        ' no initializers: init() runs from the base constructor, before field initializers would run
        Private trackedState As ManagedWorkspaceDeallocator.TrackedBytes
        Private uniqueIdField As String

        ''' <summary>A workspace with the default id; the managers come from <c>Nd4vb</c>.</summary>
        Public Sub New(ByVal configuration As WorkspaceConfiguration)
            MyBase.New(configuration)
        End Sub

        ''' <summary>A workspace with the given id; the managers come from <c>Nd4vb</c>.</summary>
        Public Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String)
            MyBase.New(configuration, workspaceId)
        End Sub

        ''' <summary>A workspace with the given id on the given device; the managers come from <c>Nd4vb</c>.</summary>
        Public Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String, ByVal deviceId As Integer?)
            MyBase.New(configuration, workspaceId)
            If deviceId.HasValue Then
                Me.deviceIdField = deviceId.Value
            End If
        End Sub

        ''' <summary>
        ''' A workspace created by <paramref name="workspaceManager"/> (its debug mode applies; may be Nothing) that sets the
        ''' current workspace of <paramref name="memoryManager"/>.
        ''' </summary>
        Public Sub New(ByVal configuration As WorkspaceConfiguration, ByVal workspaceId As String, ByVal deviceId As Integer, ByVal workspaceManager As MemoryWorkspaceManager, ByVal memoryManager As MemoryManager)
            MyBase.New(configuration, workspaceId, workspaceManager, memoryManager, deviceId)
        End Sub

        Private Function tracked() As ManagedWorkspaceDeallocator.TrackedBytes
            If trackedState Is Nothing Then
                trackedState = New ManagedWorkspaceDeallocator.TrackedBytes()
            End If
            Return trackedState
        End Function

        ''' <summary>An id for the deallocator service: <c>Workspace_</c>, the workspace id and a counter value.</summary>
        Public Overrides ReadOnly Property UniqueId As String
            Get
                If uniqueIdField Is Nothing Then
                    uniqueIdField = "Workspace_" & Id & "_" & Interlocked.Increment(idCounter).ToString(Globalization.CultureInfo.InvariantCulture)
                End If
                Return uniqueIdField
            End Get
        End Property

        ''' <summary>A deallocator that releases the tracker bytes of this workspace (it holds no reference to the workspace).</summary>
        Public Overrides Function deallocator() As Deallocator
            Return New ManagedWorkspaceDeallocator(tracked(), deviceIdField)
        End Function

        ''' <summary>The single managed device.</summary>
        Public Overrides Function targetDevice() As Integer
            Return 0
        End Function

        ''' <summary>Applies the size policies, then "allocates" a RAM workspace: marks it initialized and charges the tracker.</summary>
        Protected Friend Overrides Sub init()
            MyBase.init()

            If workspaceConfigurationField.getPolicyLocation() = LocationPolicy.RAM Then
                If currentSizeField > 0 Then
                    isInit = True

                    If isDebug Then
                        log.info("Allocating [{}] workspace of {} bytes...", idField, currentSizeField)
                    End If

                    tracked().charge(deviceIdField, currentSizeField + SAFETY_OFFSET)
                End If
            End If
        End Sub

        ''' <summary>Releases pinned allocations at least two steps old (all of them when <paramref name="extended"/>).</summary>
        Protected Friend Overrides Sub clearPinnedAllocations(ByVal extended As Boolean)
            If isDebug Then
                log.info("Workspace [{}] device_{} threadId {} cycle {}: clearing pinned allocations...", idField, deviceIdField, Thread.CurrentThread.ManagedThreadId, cyclesCountField)
            End If

            Do While pinnedAllocations.Count > 0
                Dim pair As AllocationRecord = pinnedAllocations.First.Value

                Dim stepNumber As Long = pair.AllocationCycle
                Dim stepCurrent As Long = stepsCount

                If isDebug Then
                    log.info("Allocation step: {}; Current step: {}", stepNumber, stepCurrent)
                End If

                If stepNumber + 2 < stepCurrent OrElse extended Then
                    pinnedAllocations.RemoveFirst()
                    pinnedCount -= 1
                    pinnedAllocationsSize -= pair.RequiredMemory
                Else
                    Exit Do
                End If
            Loop
        End Sub

        ''' <summary>The size of the mapped file of an MMAP workspace (0 otherwise).</summary>
        Protected Friend Overridable Function mappedFileSize() As Long
            If workspaceConfigurationField.getPolicyLocation() <> LocationPolicy.MMAP OrElse tempFile Is Nothing OrElse Not File.Exists(tempFile) Then
                Return 0
            End If

            Return New FileInfo(tempFile).Length
        End Function

        ''' <summary>Releases the spilled allocations.</summary>
        Protected Friend Overrides Sub clearExternalAllocations()
            If isDebug Then
                log.info("Workspace [{}] device_{} threadId {} guid [{}]: clearing external allocations...", idField, deviceIdField, Thread.CurrentThread.ManagedThreadId, guid)
            End If

            externalAllocations.Clear()
            externalCount = 0
            spilledAllocationsSize = 0
        End Sub

        ''' <summary>Destroys the workspace: size 0, offsets reset, pinned (and with <paramref name="extended"/> spilled) allocations released, tracker bytes released.</summary>
        Public Overrides Sub destroyWorkspace(ByVal extended As Boolean)
            SyncLock Me
                If isDebug Then
                    log.info("Destroying workspace...")
                End If

                currentSizeField = 0
                hostOffsetField = 0
                deviceOffsetField = 0

                If extended Then
                    clearExternalAllocations()
                End If

                clearPinnedAllocations(extended)

                tracked().release()
            End SyncLock
        End Sub

        ''' <summary>Nothing to reset in managed memory.</summary>
        Protected Friend Overrides Sub resetWorkspace()
        End Sub

        ''' <summary>The host offset.</summary>
        Public Overrides ReadOnly Property PrimaryOffset As Long
            Get
                Return HostOffset
            End Get
        End Property

        ''' <summary>The bytes this workspace currently charges to the allocation tracker.</summary>
        Public ReadOnly Property TrackedBytes As Long
            Get
                Return tracked().Bytes
            End Get
        End Property

    End Class

End Namespace
