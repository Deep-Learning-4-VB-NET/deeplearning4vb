Imports System
Imports System.Collections.Generic
Imports BinaryByteUnit = com.jakewharton.byteunits.BinaryByteUnit
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports MemoryWorkspaceManager = org.nd4j.linalg.api.memory.MemoryWorkspaceManager
Imports Nd4jWorkspace = org.nd4j.linalg.api.memory.abstracts.Nd4jWorkspace
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports org.nd4j.linalg.api.memory.enums
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DummyWorkspace = org.nd4j.linalg.api.memory.abstracts.DummyWorkspace
Imports org.nd4j.common.primitives

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

Namespace org.nd4j.linalg.api.memory.provider



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BasicWorkspaceManager implements org.nd4j.linalg.api.memory.MemoryWorkspaceManager
	Public MustInherit Class BasicWorkspaceManager
		Implements MemoryWorkspaceManager

		Public MustOverride Function getWorkspaceForCurrentThread(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace
		Public MustOverride Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String, ByVal deviceId As Integer?) As MemoryWorkspace
		Public MustOverride Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace
		Public MustOverride Function createNewWorkspace() As MemoryWorkspace
		Public MustOverride Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration) As MemoryWorkspace

		Protected Friend counter As New AtomicLong()
		Protected Friend defaultConfiguration As WorkspaceConfiguration
		Protected Friend backingMap As New ThreadLocal(Of IDictionary(Of String, MemoryWorkspace))()
		'private ReferenceQueue<MemoryWorkspace> queue;
		'private WorkspaceDeallocatorThread thread;
		'private Map<String, Nd4jWorkspace.GarbageWorkspaceReference> referenceMap = new ConcurrentHashMap<>();

		' default mode is DISABLED, as in: production mode
'JAVA TO VB CONVERTER NOTE: The field debugMode was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend debugMode_Conflict As New SynchronizedObject(Of DebugMode)(DebugMode.DISABLED)

		Public Sub New()
			Me.New(WorkspaceConfiguration.builder().initialSize(0).maxSize(0).overallocationLimit(0.3).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public BasicWorkspaceManager(@NonNull WorkspaceConfiguration defaultConfiguration)
		Public Sub New(ByVal defaultConfiguration As WorkspaceConfiguration)
			Me.defaultConfiguration = defaultConfiguration
			'this.queue = new ReferenceQueue<>();

			'thread = new WorkspaceDeallocatorThread(this.queue);
			'thread.start();
		End Sub

		''' <summary>
		''' Returns globally unique ID
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property UUID As String Implements MemoryWorkspaceManager.getUUID
			Get
				Return "Workspace_" & counter.incrementAndGet().ToString()
			End Get
		End Property

		''' <summary>
		''' This method allows to specify "Default" configuration, that will be used in signatures which do not have WorkspaceConfiguration argument </summary>
		''' <param name="configuration"> </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setDefaultWorkspaceConfiguration(@NonNull WorkspaceConfiguration configuration)
		Public Overridable WriteOnly Property DefaultWorkspaceConfiguration As WorkspaceConfiguration
			Set(ByVal configuration As WorkspaceConfiguration)
				Me.defaultConfiguration = configuration
			End Set
		End Property

		''' <summary>
		''' This method will return workspace with default configuration and default id.
		''' @return
		''' </summary>
		Public Overridable Property WorkspaceForCurrentThread As MemoryWorkspace
			Get
				Return getWorkspaceForCurrentThread(MemoryWorkspace.DEFAULT_ID)
			End Get
			Set(ByVal workspace As MemoryWorkspace)
				setWorkspaceForCurrentThread(workspace, MemoryWorkspace.DEFAULT_ID)
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.memory.MemoryWorkspace getWorkspaceForCurrentThread(@NonNull String id)
		Public Overridable Function getWorkspaceForCurrentThread(ByVal id As String) As MemoryWorkspace
			Return getWorkspaceForCurrentThread(defaultConfiguration, id)
		End Function

		Public Overridable Property DebugMode As DebugMode
			Get
				Return debugMode_Conflict.get()
			End Get
			Set(ByVal mode As DebugMode)
				If mode = Nothing Then
					mode = DebugMode.DISABLED
				End If
    
				debugMode_Conflict.set(mode)
			End Set
		End Property


	'    
	'    @Override
	'    public MemoryWorkspace getWorkspaceForCurrentThread(@NonNull WorkspaceConfiguration configuration, @NonNull String id) {
	'        ensureThreadExistense();
	'    
	'        MemoryWorkspace workspace = backingMap.get().get(id);
	'        if (workspace == null) {
	'            workspace = new Nd4jWorkspace(configuration, id);
	'            backingMap.get().put(id, workspace);
	'        }
	'    
	'        return workspace;
	'    }
	'    

		Protected Friend MustOverride Sub pickReference(ByVal workspace As MemoryWorkspace)
	'    
	'    protected void pickReference(MemoryWorkspace workspace) {
	'        Nd4jWorkspace.GarbageWorkspaceReference reference = new Nd4jWorkspace.GarbageWorkspaceReference(workspace, queue);
	'        referenceMap.put(reference.getKey(), reference);
	'    }
	'    




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void setWorkspaceForCurrentThread(@NonNull MemoryWorkspace workspace, @NonNull String id)
		Public Overridable Sub setWorkspaceForCurrentThread(ByVal workspace As MemoryWorkspace, ByVal id As String) Implements MemoryWorkspaceManager.setWorkspaceForCurrentThread
			ensureThreadExistense()

			backingMap.get().put(id, workspace)
		End Sub

		''' <summary>
		''' This method destroys given workspace
		''' </summary>
		''' <param name="workspace"> </param>
		Public Overridable Sub destroyWorkspace(ByVal workspace As MemoryWorkspace)
			If workspace Is Nothing OrElse TypeOf workspace Is DummyWorkspace Then
				Return
			End If

			workspace.destroyWorkspace(True)
			backingMap.get().remove(workspace.Id)
		End Sub

		''' <summary>
		''' This method destroy default workspace, if any
		''' </summary>
		Public Overridable Sub destroyWorkspace() Implements MemoryWorkspaceManager.destroyWorkspace
			ensureThreadExistense()

			Dim workspace As MemoryWorkspace = backingMap.get().get(MemoryWorkspace.DEFAULT_ID)
			'if (workspace != null)
			'workspace.destroyWorkspace();

			backingMap.get().remove(MemoryWorkspace.DEFAULT_ID)
		End Sub

		''' <summary>
		''' This method destroys all workspaces allocated in current thread
		''' </summary>
		Public Overridable Sub destroyAllWorkspacesForCurrentThread() Implements MemoryWorkspaceManager.destroyAllWorkspacesForCurrentThread
			ensureThreadExistense()

			Dim workspaces As IList(Of MemoryWorkspace) = New List(Of MemoryWorkspace)()
			CType(workspaces, List(Of MemoryWorkspace)).AddRange(backingMap.get().values())

			For Each workspace As MemoryWorkspace In workspaces
				destroyWorkspace(workspace)
			Next workspace

			Nd4j.MemoryManager.invokeGc()
		End Sub

		Protected Friend Overridable Sub ensureThreadExistense()
			If backingMap.get() Is Nothing Then
				backingMap.set(New Dictionary(Of String, MemoryWorkspace)())
			End If
		End Sub

		''' <summary>
		''' This method gets & activates default workspace
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property AndActivateWorkspace As MemoryWorkspace
			Get
				Return WorkspaceForCurrentThread.notifyScopeEntered()
			End Get
		End Property

		''' <summary>
		''' This method gets & activates workspace with a given Id
		''' </summary>
		''' <param name="id">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.memory.MemoryWorkspace getAndActivateWorkspace(@NonNull String id)
		Public Overridable Function getAndActivateWorkspace(ByVal id As String) As MemoryWorkspace
			Return getWorkspaceForCurrentThread(id).notifyScopeEntered()
		End Function

		''' <summary>
		''' This method gets & activates default with a given configuration and Id
		''' </summary>
		''' <param name="configuration"> </param>
		''' <param name="id">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.memory.MemoryWorkspace getAndActivateWorkspace(@NonNull WorkspaceConfiguration configuration, @NonNull String id)
		Public Overridable Function getAndActivateWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace
			Return getWorkspaceForCurrentThread(configuration, id).notifyScopeEntered()
		End Function

		''' <summary>
		''' This method checks, if Workspace with a given Id was created before this call
		''' </summary>
		''' <param name="id">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public boolean checkIfWorkspaceExists(@NonNull String id)
		Public Overridable Function checkIfWorkspaceExists(ByVal id As String) As Boolean Implements MemoryWorkspaceManager.checkIfWorkspaceExists
			ensureThreadExistense()
			Return backingMap.get().containsKey(id)
		End Function


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public boolean checkIfWorkspaceExistsAndActive(@NonNull String id)
		Public Overridable Function checkIfWorkspaceExistsAndActive(ByVal id As String) As Boolean Implements MemoryWorkspaceManager.checkIfWorkspaceExistsAndActive
			Dim exists As Boolean = checkIfWorkspaceExists(id)
			If Not exists Then
				Return False
			End If

			Return backingMap.get().get(id).isScopeActive()
		End Function

		''' <summary>
		''' This method temporary opens block out of any workspace scope.
		''' <para>
		''' PLEASE NOTE: Do not forget to close this block.
		''' 
		''' @return
		''' </para>
		''' </summary>
		Public Overridable Function scopeOutOfWorkspaces() As MemoryWorkspace
			Dim workspace As MemoryWorkspace = Nd4j.MemoryManager.CurrentWorkspace
			If workspace Is Nothing Then
				Return New DummyWorkspace()
			Else
				Nd4j.MemoryManager.CurrentWorkspace = Nothing
				Return workspace.tagOutOfScopeUse()
			End If
		End Function


		<Obsolete>
		Public Const WorkspaceDeallocatorThreadName As String = "Workspace deallocator thread"

	'    
	'    protected class WorkspaceDeallocatorThread extends Thread implements Runnable {
	'        private final ReferenceQueue<MemoryWorkspace> queue;
	'
	'        protected WorkspaceDeallocatorThread(ReferenceQueue<MemoryWorkspace> queue) {
	'            this.queue = queue;
	'            this.setDaemon(true);
	'            this.setName(WorkspaceDeallocatorThreadName);
	'        }
	'
	'        @Override
	'        public void run() {
	'            while (true) {
	'                try {
	'                    Nd4jWorkspace.GarbageWorkspaceReference reference =
	'                                    (Nd4jWorkspace.GarbageWorkspaceReference) queue.remove();
	'                    if (reference != null) {
	'                        //                      log.info("Releasing reference for Workspace [{}]", reference.getId());
	'                        PointersPair pair = reference.getPointersPair();
	'                        // purging workspace planes
	'                        if (pair != null) {
	'                            if (pair.getDevicePointer() != null) {
	'                                //log.info("Deallocating device...");
	'                                Nd4j.getMemoryManager().release(pair.getDevicePointer(), MemoryKind.DEVICE);
	'                            }
	'
	'
	'                            if (pair.getHostPointer() != null) {
	'                                //                                log.info("Deallocating host...");
	'                                Nd4j.getMemoryManager().release(pair.getHostPointer(), MemoryKind.HOST);
	'                            }
	'                        }
	'
	'                        // purging all spilled pointers
	'                        for (PointersPair pair2 : reference.getExternalPointers()) {
	'                            if (pair2 != null) {
	'                                if (pair2.getHostPointer() != null)
	'                                    Nd4j.getMemoryManager().release(pair2.getHostPointer(), MemoryKind.HOST);
	'
	'                                if (pair2.getDevicePointer() != null)
	'                                    Nd4j.getMemoryManager().release(pair2.getDevicePointer(), MemoryKind.DEVICE);
	'                            }
	'                        }
	'
	'                        // purging all pinned pointers
	'                        while ((pair = reference.getPinnedPointers().poll()) != null) {
	'                            if (pair.getHostPointer() != null)
	'                                Nd4j.getMemoryManager().release(pair.getHostPointer(), MemoryKind.HOST);
	'
	'                            if (pair.getDevicePointer() != null)
	'                                Nd4j.getMemoryManager().release(pair.getDevicePointer(), MemoryKind.DEVICE);
	'                        }
	'
	'                        referenceMap.remove(reference.getKey());
	'                    }
	'                } catch (InterruptedException e) {
	'                    return; // terminate thread when being interrupted
	'                } catch (Exception e) {
	'                    //
	'                }
	'            }
	'        }
	'    }
	'    

		''' <summary>
		''' This method prints out basic statistics for workspaces allocated in current thread
		''' </summary>
		Public Overridable Sub printAllocationStatisticsForCurrentThread() Implements MemoryWorkspaceManager.printAllocationStatisticsForCurrentThread
			SyncLock Me
				ensureThreadExistense()
				Dim map As IDictionary(Of String, MemoryWorkspace) = backingMap.get()
				log.info("Workspace statistics: ---------------------------------")
				log.info("Number of workspaces in current thread: {}", map.Count)
				log.info("Workspace name: Allocated / external (spilled) / external (pinned)")
				For Each key As String In map.Keys
					Dim current As Long = DirectCast(map(key), Nd4jWorkspace).CurrentSize
					Dim spilled As Long = DirectCast(map(key), Nd4jWorkspace).SpilledSize
					Dim pinned As Long = DirectCast(map(key), Nd4jWorkspace).PinnedSize
					log.info(String.Format("{0,-26} {1,8} / {2,8} / {3,8} ({4,11:D} / {5,11:D} / {6,11:D})", (key & ":"), BinaryByteUnit.format(current, "#.00"), BinaryByteUnit.format(spilled, "#.00"), BinaryByteUnit.format(pinned, "#.00"), current, spilled, pinned))
				Next key
			End SyncLock
		End Sub


		Public Overridable ReadOnly Property AllWorkspacesIdsForCurrentThread As IList(Of String) Implements MemoryWorkspaceManager.getAllWorkspacesIdsForCurrentThread
			Get
				ensureThreadExistense()
				Return New List(Of String)(backingMap.get().keySet())
			End Get
		End Property

		Public Overridable ReadOnly Property AllWorkspacesForCurrentThread As IList(Of MemoryWorkspace) Implements MemoryWorkspaceManager.getAllWorkspacesForCurrentThread
			Get
				ensureThreadExistense()
				Return New List(Of MemoryWorkspace)(backingMap.get().values())
			End Get
		End Property

		Public Overridable Function anyWorkspaceActiveForCurrentThread() As Boolean Implements MemoryWorkspaceManager.anyWorkspaceActiveForCurrentThread
			ensureThreadExistense()
			Dim anyActive As Boolean = False
			For Each ws As MemoryWorkspace In backingMap.get().values()
				If ws.ScopeActive Then
					anyActive = True
					Exit For
				End If
			Next ws
			Return anyActive
		End Function
	End Class

End Namespace