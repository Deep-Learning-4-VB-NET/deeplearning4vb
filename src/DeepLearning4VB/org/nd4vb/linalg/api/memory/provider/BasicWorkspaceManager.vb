Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Threading
Imports DeepLearning4VB.org.nd4vb.common.primitives
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.enums
Imports DeepLearning4VB.org.slf4j
Imports DummyWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.DummyWorkspace
Imports MemoryManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryManager
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports MemoryWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspaceManager
Imports Nd4vbWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.Nd4vbWorkspace
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

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

Namespace org.nd4vb.linalg.api.memory.provider

    ''' <summary>
    ''' The shared part of workspace managers (Java <c>BasicWorkspaceManager</c>): the per-thread map of workspaces by id,
    ''' the default configuration, the debug mode, scoping out of workspaces and allocation statistics.
    ''' </summary>
    ''' <remarks>
    ''' Differences from Java: the memory manager (whose current workspace is used) is the one given to the constructor,
    ''' or <c>Nd4vb.MemoryManager</c> when none was given (Java always used <c>Nd4j.getMemoryManager()</c>). Java's
    ''' commented-out workspace deallocator thread is not ported.
    ''' </remarks>
    Public MustInherit Class BasicWorkspaceManager
        Implements MemoryWorkspaceManager

        Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(GetType(BasicWorkspaceManager))

        ''' <summary>The workspace for the configuration and id, created on first use (implementation-specific).</summary>
        Public MustOverride Function getWorkspaceForCurrentThread(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace Implements MemoryWorkspaceManager.getWorkspaceForCurrentThread
        ''' <summary>Creates a new workspace on the given device.</summary>
        Public MustOverride Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String, ByVal deviceId As Integer?) As MemoryWorkspace Implements MemoryWorkspaceManager.createNewWorkspace
        ''' <summary>Creates a new workspace with the given id.</summary>
        Public MustOverride Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace Implements MemoryWorkspaceManager.createNewWorkspace
        ''' <summary>Creates a new workspace with the default configuration and id.</summary>
        Public MustOverride Function createNewWorkspace() As MemoryWorkspace Implements MemoryWorkspaceManager.createNewWorkspace
        ''' <summary>Creates a new workspace with the default id.</summary>
        Public MustOverride Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration) As MemoryWorkspace Implements MemoryWorkspaceManager.createNewWorkspace

        Protected Friend counter As Long = 0
        Protected Friend defaultConfiguration As WorkspaceConfiguration
        Protected Friend backingMap As New ThreadLocal(Of IDictionary(Of String, MemoryWorkspace))()

        ''' <summary>Default mode is DISABLED, as in: production mode.</summary>
        Protected Friend debugModeField As New SynchronizedObject(Of DebugMode)(DebugMode.DISABLED)

        Private ReadOnly memoryManagerField As MemoryManager

        ''' <summary>
        ''' A manager with Java's default configuration: no initial size, overallocation 0.3, OVERALLOCATE, FIRST_LOOP learning,
        ''' FULL mirroring, EXTERNAL spill.
        ''' </summary>
        Public Sub New()
            Me.New(javaDefaultConfiguration(), Nothing)
        End Sub

        ''' <summary>A manager with the given default configuration.</summary>
        Public Sub New(ByVal defaultConfiguration As WorkspaceConfiguration)
            Me.New(defaultConfiguration, Nothing)
        End Sub

        ''' <summary>
        ''' A manager with the given default configuration that uses <paramref name="memoryManager"/> for the current
        ''' workspace (Nothing: <c>Nd4vb.MemoryManager</c>).
        ''' </summary>
        Public Sub New(ByVal defaultConfiguration As WorkspaceConfiguration, ByVal memoryManager As MemoryManager)
            If defaultConfiguration Is Nothing Then
                Throw New ArgumentNullException(NameOf(defaultConfiguration))
            End If
            Me.defaultConfiguration = defaultConfiguration
            Me.memoryManagerField = memoryManager
        End Sub

        ''' <summary>Java's default workspace configuration of a workspace manager.</summary>
        Public Shared Function javaDefaultConfiguration() As WorkspaceConfiguration
            Return WorkspaceConfiguration.builder().initialSize(0).maxSize(0).overallocationLimit(0.3).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()
        End Function

        ''' <summary>The memory manager whose current workspace this manager reads and sets.</summary>
        Protected Friend Overridable Function currentMemoryManager() As MemoryManager
            If memoryManagerField IsNot Nothing Then
                Return memoryManagerField
            End If
            Return Nd4vbT.MemoryManager
        End Function

        ''' <summary>Returns a new globally unique id (<c>Workspace_</c> followed by a counter).</summary>
        Public Overridable ReadOnly Property UUID As String Implements MemoryWorkspaceManager.UUID
            Get
                Return "Workspace_" & Interlocked.Increment(counter).ToString(CultureInfo.InvariantCulture)
            End Get
        End Property

        ''' <summary>The "default" configuration, used by the members without a configuration argument.</summary>
        Public Overridable WriteOnly Property DefaultWorkspaceConfiguration As WorkspaceConfiguration Implements MemoryWorkspaceManager.DefaultWorkspaceConfiguration
            Set(ByVal configuration As WorkspaceConfiguration)
                If configuration Is Nothing Then
                    Throw New ArgumentNullException(NameOf(configuration))
                End If
                Me.defaultConfiguration = configuration
            End Set
        End Property

        ''' <summary>The workspace with the default configuration and default id.</summary>
        Public Overridable Property WorkspaceForCurrentThread As MemoryWorkspace Implements MemoryWorkspaceManager.WorkspaceForCurrentThread
            Get
                Return getWorkspaceForCurrentThread(MemoryWorkspace.Constants.DEFAULT_ID)
            End Get
            Set(ByVal workspace As MemoryWorkspace)
                setWorkspaceForCurrentThread(workspace, MemoryWorkspace.Constants.DEFAULT_ID)
            End Set
        End Property

        ''' <summary>The workspace with the given id, created with the default configuration on first use.</summary>
        Public Overridable Function getWorkspaceForCurrentThread(ByVal id As String) As MemoryWorkspace Implements MemoryWorkspaceManager.getWorkspaceForCurrentThread
            Return getWorkspaceForCurrentThread(defaultConfiguration, id)
        End Function

        ''' <summary>The debug mode (DISABLED, SPILL_EVERYTHING or BYPASS_EVERYTHING) for the workspaces of this manager.</summary>
        Public Overridable Property DebugMode As DebugMode Implements MemoryWorkspaceManager.DebugMode
            Get
                Return debugModeField.get()
            End Get
            Set(ByVal mode As DebugMode)
                debugModeField.set(mode)
            End Set
        End Property

        ''' <summary>Registers a workspace for clean-up after it is garbage collected (implementation-specific).</summary>
        Protected Friend MustOverride Sub pickReference(ByVal workspace As MemoryWorkspace)

        ''' <summary>Stores a workspace under the given id for the current thread.</summary>
        Public Overridable Sub setWorkspaceForCurrentThread(ByVal workspace As MemoryWorkspace, ByVal id As String) Implements MemoryWorkspaceManager.setWorkspaceForCurrentThread
            If workspace Is Nothing Then
                Throw New ArgumentNullException(NameOf(workspace))
            End If
            If id Is Nothing Then
                Throw New ArgumentNullException(NameOf(id))
            End If
            ensureThreadExistense()
            backingMap.Value(id) = workspace
        End Sub

        ''' <summary>Destroys the given workspace and forgets it (Nothing and dummy workspaces are ignored).</summary>
        Public Overridable Sub destroyWorkspace(ByVal workspace As MemoryWorkspace) Implements MemoryWorkspaceManager.destroyWorkspace
            If workspace Is Nothing OrElse TypeOf workspace Is DummyWorkspace Then
                Return
            End If

            workspace.destroyWorkspace(True)
            ensureThreadExistense()
            backingMap.Value.Remove(workspace.Id)
        End Sub

        ''' <summary>Forgets the default workspace, if any (as in Java, without destroying it).</summary>
        Public Overridable Sub destroyWorkspace() Implements MemoryWorkspaceManager.destroyWorkspace
            ensureThreadExistense()
            backingMap.Value.Remove(MemoryWorkspace.Constants.DEFAULT_ID)
        End Sub

        ''' <summary>Destroys all workspaces of the current thread, then runs a garbage collection.</summary>
        Public Overridable Sub destroyAllWorkspacesForCurrentThread() Implements MemoryWorkspaceManager.destroyAllWorkspacesForCurrentThread
            ensureThreadExistense()
            Dim workspaces As New List(Of MemoryWorkspace)(backingMap.Value.Values)
            For Each workspace As MemoryWorkspace In workspaces
                destroyWorkspace(workspace)
            Next workspace

            currentMemoryManager().invokeGc()
        End Sub

        ''' <summary>Creates the current thread's workspace map if needed.</summary>
        Protected Friend Overridable Sub ensureThreadExistense()
            If backingMap.Value Is Nothing Then
                backingMap.Value = New Dictionary(Of String, MemoryWorkspace)()
            End If
        End Sub

        ''' <summary>Gets and activates (enters) the default workspace.</summary>
        Public Overridable ReadOnly Property AndActivateWorkspace As MemoryWorkspace Implements MemoryWorkspaceManager.AndActivateWorkspace
            Get
                Return WorkspaceForCurrentThread.notifyScopeEntered()
            End Get
        End Property

        ''' <summary>Java <c>getAndActivateWorkspace()</c>.</summary>
        Public Overridable Function getAndActivateWorkspace() As MemoryWorkspace
            Return AndActivateWorkspace
        End Function

        ''' <summary>Gets and activates the workspace with the given id.</summary>
        Public Overridable Function getAndActivateWorkspace(ByVal id As String) As MemoryWorkspace Implements MemoryWorkspaceManager.getAndActivateWorkspace
            Return getWorkspaceForCurrentThread(id).notifyScopeEntered()
        End Function

        ''' <summary>Gets and activates the workspace with the given configuration and id.</summary>
        Public Overridable Function getAndActivateWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace Implements MemoryWorkspaceManager.getAndActivateWorkspace
            Return getWorkspaceForCurrentThread(configuration, id).notifyScopeEntered()
        End Function

        ''' <summary>True if a workspace with the given id was created in the current thread.</summary>
        Public Overridable Function checkIfWorkspaceExists(ByVal id As String) As Boolean Implements MemoryWorkspaceManager.checkIfWorkspaceExists
            If id Is Nothing Then
                Throw New ArgumentNullException(NameOf(id))
            End If
            ensureThreadExistense()
            Return backingMap.Value.ContainsKey(id)
        End Function

        ''' <summary>True if a workspace with the given id exists in the current thread and its scope is open.</summary>
        Public Overridable Function checkIfWorkspaceExistsAndActive(ByVal id As String) As Boolean Implements MemoryWorkspaceManager.checkIfWorkspaceExistsAndActive
            Dim exists As Boolean = checkIfWorkspaceExists(id)
            If Not exists Then
                Return False
            End If

            Return backingMap.Value(id).ScopeActive
        End Function

        ''' <summary>
        ''' Temporarily opens a block outside any workspace scope (do not forget to close it): a dummy workspace when none is
        ''' current, otherwise the current workspace, detached from the thread and tagged for out-of-scope use.
        ''' </summary>
        Public Overridable Function scopeOutOfWorkspaces() As MemoryWorkspace Implements MemoryWorkspaceManager.scopeOutOfWorkspaces
            Dim manager As MemoryManager = currentMemoryManager()
            Dim workspace As MemoryWorkspace = manager.CurrentWorkspace
            If workspace Is Nothing Then
                Return New DummyWorkspace(manager)
            Else
                manager.CurrentWorkspace = Nothing
                Return workspace.tagOutOfScopeUse()
            End If
        End Function

        ''' <summary>Deprecated in Java.</summary>
        Public Const WorkspaceDeallocatorThreadName As String = "Workspace deallocator thread"

        ''' <summary>Logs the size, spilled and pinned bytes of every workspace of the current thread.</summary>
        Public Overridable Sub printAllocationStatisticsForCurrentThread() Implements MemoryWorkspaceManager.printAllocationStatisticsForCurrentThread
            SyncLock Me
                ensureThreadExistense()
                Dim map As IDictionary(Of String, MemoryWorkspace) = backingMap.Value
                log.info("Workspace statistics: ---------------------------------")
                log.info("Number of workspaces in current thread: {}", map.Count)
                log.info("Workspace name: Allocated / external (spilled) / external (pinned)")
                For Each key As String In map.Keys
                    Dim ws As Nd4vbWorkspace = TryCast(map(key), Nd4vbWorkspace)
                    Dim current As Long = map(key).CurrentSize
                    Dim spilled As Long = If(ws Is Nothing, 0L, ws.SpilledSize)
                    Dim pinned As Long = If(ws Is Nothing, 0L, ws.PinnedSize)
                    log.info(String.Format(CultureInfo.InvariantCulture, "{0,-26} {1,8} / {2,8} / {3,8} ({4,11:D} / {5,11:D} / {6,11:D})", key & ":", formatBytes(current), formatBytes(spilled), formatBytes(pinned), current, spilled, pinned))
                Next key
            End SyncLock
        End Sub

        ''' <summary>Binary byte units with two decimals, like Java's <c>BinaryByteUnit.format(bytes, "#.00")</c>.</summary>
        Friend Shared Function formatBytes(ByVal bytes As Long) As String
            Dim units As String() = {"B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB"}
            Dim value As Double = bytes
            Dim u As Integer = 0
            Do While Math.Abs(value) >= 1024.0 AndAlso u < units.Length - 1
                value /= 1024.0
                u += 1
            Loop
            Return value.ToString("0.00", CultureInfo.InvariantCulture) & " " & units(u)
        End Function

        ''' <summary>The ids of the workspaces of the current thread.</summary>
        Public Overridable ReadOnly Property AllWorkspacesIdsForCurrentThread As IList(Of String) Implements MemoryWorkspaceManager.AllWorkspacesIdsForCurrentThread
            Get
                ensureThreadExistense()
                Return New List(Of String)(backingMap.Value.Keys)
            End Get
        End Property

        ''' <summary>Java <c>getAllWorkspacesIdsForCurrentThread()</c>.</summary>
        Public Overridable Function getAllWorkspacesIdsForCurrentThread() As IList(Of String)
            Return AllWorkspacesIdsForCurrentThread
        End Function

        ''' <summary>The workspaces of the current thread.</summary>
        Public Overridable ReadOnly Property AllWorkspacesForCurrentThread As IList(Of MemoryWorkspace) Implements MemoryWorkspaceManager.AllWorkspacesForCurrentThread
            Get
                ensureThreadExistense()
                Return New List(Of MemoryWorkspace)(backingMap.Value.Values)
            End Get
        End Property

        ''' <summary>Java <c>getAllWorkspacesForCurrentThread()</c>.</summary>
        Public Overridable Function getAllWorkspacesForCurrentThread() As IList(Of MemoryWorkspace)
            Return AllWorkspacesForCurrentThread
        End Function

        ''' <summary>True if any workspace of the current thread has an open scope.</summary>
        Public Overridable Function anyWorkspaceActiveForCurrentThread() As Boolean Implements MemoryWorkspaceManager.anyWorkspaceActiveForCurrentThread
            ensureThreadExistense()
            For Each ws As MemoryWorkspace In backingMap.Value.Values
                If ws.ScopeActive Then
                    Return True
                End If
            Next ws
            Return False
        End Function

    End Class

End Namespace
