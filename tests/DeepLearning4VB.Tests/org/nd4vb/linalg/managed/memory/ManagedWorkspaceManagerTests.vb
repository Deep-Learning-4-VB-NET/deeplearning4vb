Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.enums
Imports DeepLearning4VB.org.nd4vb.linalg.managed.memory
Imports DeallocatorService = DeepLearning4VB.org.nd4vb.linalg.api.memory.deallocation.DeallocatorService
Imports DummyWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.DummyWorkspace
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration

Namespace org.nd4vb.linalg.managed.memory

    ''' <summary>Tests for <see cref="ManagedWorkspaceManager"/> (and <c>BasicWorkspaceManager</c>).</summary>
    <TestClass>
    Public Class ManagedWorkspaceManagerTests

        Private Shared ReadOnly config As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1024).policyLearning(LearningPolicy.NONE).build()

        <TestMethod>
        Public Sub testNewWorkspace1()
            Dim wm As New ManagedWorkspaceManager(New ManagedMemoryManager(), Nothing)
            Dim workspace1 As MemoryWorkspace = wm.WorkspaceForCurrentThread
            assertNotNull(workspace1)
            Assert.AreSame(workspace1, wm.WorkspaceForCurrentThread)
            assertEquals(MemoryWorkspace.Constants.DEFAULT_ID, workspace1.Id)
            assertTrue(TypeOf workspace1 Is ManagedWorkspace)
        End Sub

        <TestMethod>
        Public Sub testMultithreading1()
            Dim wm As New ManagedWorkspaceManager(New ManagedMemoryManager(), Nothing)
            wm.DefaultWorkspaceConfiguration = config
            Dim workspaces As New ConcurrentBag(Of MemoryWorkspace)()
            Dim threads(19) As Thread
            For x As Integer = 0 To threads.Length - 1
                threads(x) = New Thread(Sub() workspaces.Add(wm.WorkspaceForCurrentThread))
                threads(x).Start()
            Next
            For Each t As Thread In threads
                t.Join()
            Next
            Dim distinct As New HashSet(Of MemoryWorkspace)(workspaces)
            assertEquals(20, distinct.Count)
            For Each w As MemoryWorkspace In distinct
                assertNotNull(w)
                assertEquals(1024L + CLng(1024 * 0.3), w.CurrentSize)
            Next
        End Sub

        <TestMethod>
        Public Sub testRegistryAndIds()
            Dim mm As New ManagedMemoryManager()
            Dim service As New DeallocatorService()
            Dim wm As New ManagedWorkspaceManager(mm, service)
            Dim first As String = wm.UUID
            Dim second As String = wm.UUID
            assertTrue(first.StartsWith("Workspace_"))
            assertNotEquals(first, second)

            Dim a As MemoryWorkspace = wm.createNewWorkspace(config, "A")
            Dim b As MemoryWorkspace = wm.createNewWorkspace(config, "B", 0)
            Dim d As MemoryWorkspace = wm.createNewWorkspace(config)
            assertEquals("A", a.Id)
            assertEquals(MemoryWorkspace.Constants.DEFAULT_ID, d.Id)
            assertEquals(3, service.TrackedObjects)
            assertTrue(wm.checkIfWorkspaceExists("A"))
            assertFalse(wm.checkIfWorkspaceExists("Z"))
            assertFalse(wm.checkIfWorkspaceExistsAndActive("A"))
            assertFalse(wm.anyWorkspaceActiveForCurrentThread())
            Using w As MemoryWorkspace = wm.getAndActivateWorkspace("A")
                Assert.AreSame(a, w)
                assertTrue(wm.checkIfWorkspaceExistsAndActive("A"))
                assertTrue(wm.anyWorkspaceActiveForCurrentThread())
            End Using
            Dim ids As New HashSet(Of String)(wm.AllWorkspacesIdsForCurrentThread)
            assertTrue(ids.SetEquals({"A", "B", MemoryWorkspace.Constants.DEFAULT_ID}))
            assertEquals(3, wm.getAllWorkspacesForCurrentThread().Count)
            wm.printAllocationStatisticsForCurrentThread()

            wm.destroyWorkspace(b)
            assertFalse(wm.checkIfWorkspaceExists("B"))
            assertEquals(0L, b.CurrentSize)
            wm.destroyWorkspace()
            assertFalse(wm.checkIfWorkspaceExists(MemoryWorkspace.Constants.DEFAULT_ID))
            wm.destroyWorkspace(DirectCast(Nothing, MemoryWorkspace))
            wm.destroyWorkspace(New DummyWorkspace(mm))
            wm.setWorkspaceForCurrentThread(b, "B2")
            Assert.AreSame(b, wm.getWorkspaceForCurrentThread("B2"))
            wm.destroyAllWorkspacesForCurrentThread()
            ' as in Java, a workspace is forgotten under its own id: the extra "B2" entry stays
            assertEquals(New List(Of String) From {"B2"}, New List(Of String)(wm.AllWorkspacesIdsForCurrentThread))
            assertThrows(Of ArgumentNullException)(Sub() wm.getWorkspaceForCurrentThread(config, Nothing))
        End Sub

        <TestMethod>
        Public Sub testScopeOutOfWorkspacesTagsTheCurrentWorkspace()
            Dim mm As New ManagedMemoryManager()
            Dim wm As New ManagedWorkspaceManager(mm, Nothing)
            Dim outside As MemoryWorkspace = wm.scopeOutOfWorkspaces()
            assertTrue(TypeOf outside Is DummyWorkspace)
            Using w As MemoryWorkspace = wm.getAndActivateWorkspace(config, "OUT")
                Dim scoped As MemoryWorkspace = wm.scopeOutOfWorkspaces()
                Assert.AreSame(w, scoped)
                assertNull(mm.CurrentWorkspace)
                scoped.close()
                Assert.AreSame(w, mm.CurrentWorkspace)
                assertTrue(w.ScopeActive)
            End Using
            assertNull(mm.CurrentWorkspace)
        End Sub

        <TestMethod>
        Public Sub testDefaultsAndDebugMode()
            Dim wm As New ManagedWorkspaceManager(New ManagedMemoryManager(), Nothing)
            Dim d As WorkspaceConfiguration = Global.DeepLearning4VB.org.nd4vb.linalg.api.memory.provider.BasicWorkspaceManager.javaDefaultConfiguration()
            assertEquals(AllocationPolicy.OVERALLOCATE, d.getPolicyAllocation())
            assertEquals(LearningPolicy.FIRST_LOOP, d.getPolicyLearning())
            assertEquals(SpillPolicy.EXTERNAL, d.getPolicySpill())
            assertEquals(0.3, d.getOverallocationLimit(), 0.0)
            assertEquals(DebugMode.DISABLED, wm.DebugMode)
            wm.DebugMode = DebugMode.BYPASS_EVERYTHING
            assertTrue(TypeOf wm.createNewWorkspace(config, "BYPASS") Is DummyWorkspace)
            assertTrue(TypeOf wm.createNewWorkspace() Is DummyWorkspace)
            wm.DebugMode = DebugMode.DISABLED
            assertTrue(TypeOf wm.createNewWorkspace(config, "REAL") Is ManagedWorkspace)
            assertThrows(Of ArgumentNullException)(Sub() Call New ManagedWorkspaceManager(Nothing, Nothing).createNewWorkspace())
        End Sub

    End Class

End Namespace
