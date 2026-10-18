Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.workspace
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports FakeArray = DeepLearning4VB.Tests.org.nd4vb.linalg.cache.FakeArray
Imports LearningPolicy = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.LearningPolicy
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer
Imports ManagedMemoryManager = DeepLearning4VB.org.nd4vb.linalg.managed.memory.ManagedMemoryManager
Imports ManagedWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.managed.memory.ManagedWorkspaceManager
Imports MemoryManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryManager
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports MemoryWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspaceManager
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration

Namespace org.nd4vb.linalg.workspace

    ''' <summary>Tests for <see cref="BaseWorkspaceMgr(Of T)"/> with the managed workspace and memory managers.</summary>
    <TestClass>
    Public Class BaseWorkspaceMgrTests

        Public Enum ArrayKind
            INPUT
            ACTIVATIONS
            TEMP
        End Enum

        Private Class TestMgr
            Inherits BaseWorkspaceMgr(Of ArrayKind)

            Private ReadOnly wm As MemoryWorkspaceManager
            Private ReadOnly mm As MemoryManager

            Public Sub New(ByVal wm As MemoryWorkspaceManager, ByVal mm As MemoryManager)
                Me.wm = wm
                Me.mm = mm
            End Sub

            Protected Overrides Function currentWorkspaceManager() As MemoryWorkspaceManager
                Return wm
            End Function

            Protected Overrides Function currentMemoryManager() As MemoryManager
                Return mm
            End Function
        End Class

        Private mm As ManagedMemoryManager
        Private wm As ManagedWorkspaceManager
        Private mgr As TestMgr
        Private ReadOnly conf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(4096).policyLearning(LearningPolicy.NONE).build()

        <TestInitialize>
        Public Sub setUp()
            mm = New ManagedMemoryManager()
            wm = New ManagedWorkspaceManager(mm, Nothing)
            mgr = New TestMgr(wm, mm)
            mgr.setWorkspace(ArrayKind.ACTIVATIONS, "WS_ACT", conf)
            mgr.setScopedOutFor(ArrayKind.INPUT)
        End Sub

        <TestMethod>
        Public Sub testConfiguration()
            assertTrue(mgr.isScopedOut(ArrayKind.INPUT))
            assertTrue(mgr.hasConfiguration(ArrayKind.INPUT))
            assertTrue(mgr.hasConfiguration(ArrayKind.ACTIVATIONS))
            assertFalse(mgr.hasConfiguration(ArrayKind.TEMP))
            assertEquals("WS_ACT", mgr.getWorkspaceName(ArrayKind.ACTIVATIONS))
            Assert.AreSame(conf, mgr.getConfiguration(ArrayKind.ACTIVATIONS))
            assertNull(mgr.getWorkspaceName(ArrayKind.TEMP))
            assertNull(mgr.getConfiguration(ArrayKind.TEMP))
            Dim ex = assertThrows(Of ND4VBWorkspaceException)(Sub() mgr.notifyScopeEntered(ArrayKind.TEMP))
            assertEquals("No workspace configuration has been provided for arrayType: TEMP", ex.Message)
            mgr.setConfiguration(ArrayKind.TEMP, conf)
            ex = assertThrows(Of ND4VBWorkspaceException)(Sub() mgr.notifyScopeEntered(ArrayKind.TEMP))
            assertEquals("No workspace name has been provided for arrayType: TEMP", ex.Message)
            ' setting a workspace un-scopes an array type; scoping out forgets the name
            mgr.setWorkspace(ArrayKind.INPUT, "WS_IN", conf)
            assertFalse(mgr.isScopedOut(ArrayKind.INPUT))
            mgr.ScopedOutFor = ArrayKind.INPUT
            assertNull(mgr.getWorkspaceName(ArrayKind.INPUT))
        End Sub

        <TestMethod>
        Public Sub testScopes()
            assertFalse(mgr.isWorkspaceOpen(ArrayKind.ACTIVATIONS))
            assertTrue(mgr.isWorkspaceOpen(ArrayKind.INPUT))
            mgr.assertNotOpen(ArrayKind.ACTIVATIONS, "closed")
            Dim ex = assertThrows(Of ND4VBWorkspaceException)(Sub() mgr.assertOpen(ArrayKind.ACTIVATIONS, "msg"))
            assertEquals("Assertion failed: expected workspace for array type ACTIVATIONS to be open: msg", ex.Message)
            ex = assertThrows(Of ND4VBWorkspaceException)(Sub() mgr.notifyScopeBorrowed(ArrayKind.ACTIVATIONS))
            assertEquals("Workspace ""WS_ACT"" for array type ACTIVATIONS is not open", ex.Message)

            Using ws As MemoryWorkspace = mgr.notifyScopeEntered(ArrayKind.ACTIVATIONS)
                assertEquals("WS_ACT", ws.Id)
                assertTrue(mgr.isWorkspaceOpen(ArrayKind.ACTIVATIONS))
                mgr.assertOpen(ArrayKind.ACTIVATIONS, "open")
                mgr.assertCurrentWorkspace(ArrayKind.ACTIVATIONS, "current")
                assertThrows(Of ND4VBWorkspaceException)(Sub() mgr.assertNotOpen(ArrayKind.ACTIVATIONS, "open"))
                Using scoped As MemoryWorkspace = mgr.notifyScopeEntered(ArrayKind.INPUT)
                    assertNull(mm.CurrentWorkspace)
                    ex = assertThrows(Of ND4VBWorkspaceException)(Sub() mgr.assertCurrentWorkspace(ArrayKind.ACTIVATIONS, "x"))
                    assertEquals("Assertion failed: expected current workspace to be ""WS_ACT"" (for array type ACTIVATIONS) - actual current workspace is null: x", ex.Message)
                End Using
                Using borrowed As MemoryWorkspace = mgr.notifyScopeBorrowed(ArrayKind.ACTIVATIONS)
                    Assert.AreSame(ws, borrowed)
                End Using
            End Using
            assertNull(mm.CurrentWorkspace)

            Using all As WorkspacesCloseable = mgr.notifyScopeEntered(ArrayKind.ACTIVATIONS, ArrayKind.INPUT)
                assertEquals(2, all.getWorkspaces().Length)
            End Using
            assertNull(mm.CurrentWorkspace)
        End Sub

        <TestMethod>
        Public Sub testLeverageAndValidation()
            Dim detached As New FakeArray(New ManagedDataBuffer(DataType.FLOAT, 2))
            Assert.AreSame(detached, mgr.leverageTo(ArrayKind.ACTIVATIONS, detached))
            assertNull(detached.LeveragedTo)
            Assert.AreSame(detached, mgr.validateArrayLocation(ArrayKind.ACTIVATIONS, detached, False, False))
            Dim ex = assertThrows(Of ND4VBWorkspaceException)(Sub() mgr.validateArrayLocation(ArrayKind.ACTIVATIONS, detached, False, True))
            assertEquals("Array workspace validation failed: Array of type ACTIVATIONS should be in workspace ""WS_ACT"" but is detached", ex.Message)

            Dim other As MemoryWorkspace = wm.getAndActivateWorkspace(conf, "OTHER")
            Using act As MemoryWorkspace = mgr.notifyScopeEntered(ArrayKind.ACTIVATIONS)
                Dim inOther As New FakeArray(New ManagedDataBuffer(DataType.FLOAT, 2, True, other))
                ex = assertThrows(Of ND4VBWorkspaceException)(Sub() mgr.validateArrayLocation(ArrayKind.ACTIVATIONS, inOther, False, True))
                assertEquals("Array workspace validation failed: Array of type ACTIVATIONS should be in workspace ""WS_ACT"" but is in workspace ""OTHER""", ex.Message)
                ' migrating leverages to the expected workspace
                mgr.validateArrayLocation(ArrayKind.ACTIVATIONS, inOther, True, True)
                assertEquals("WS_ACT", inOther.LeveragedTo)
                ' a scoped-out type detaches
                mgr.leverageTo(ArrayKind.INPUT, inOther)
                assertTrue(inOther.Detached)
                ex = assertThrows(Of ND4VBWorkspaceException)(Sub() mgr.validateArrayLocation(ArrayKind.INPUT, inOther, False, True))
                assertEquals("Array workspace validation failed: Array of type INPUT should be detached (no workspace) but is in workspace: OTHER", ex.Message)
                Dim inAct As New FakeArray(New ManagedDataBuffer(DataType.FLOAT, 2, True, act))
                Assert.AreSame(inAct, mgr.validateArrayLocation(ArrayKind.ACTIVATIONS, inAct, False, True))
            End Using
            other.close()
        End Sub

    End Class

End Namespace
