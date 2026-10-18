Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports WorkspacesCloseable = DeepLearning4VB.org.nd4vb.linalg.workspace.WorkspacesCloseable

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.memory.abstracts

    ''' <summary>
    ''' Tests for <c>DummyWorkspace</c> scoping (entering detaches the thread from its current workspace, closing restores
    ''' it) and for <c>WorkspacesCloseable</c>, which closes workspaces in reverse order. The memory manager is a fake
    ''' passed to the <c>DummyWorkspace(MemoryManager)</c> constructor (Java: <c>Nd4j.getMemoryManager()</c>).
    ''' </summary>
    <TestClass>
    Public Class DummyWorkspaceTests

        ''' <summary>A memory manager that only keeps the current workspace and records its changes.</summary>
        Private Class FakeMemoryManager
            Implements MemoryManager

            Private current As MemoryWorkspace
            Public ReadOnly Changes As New List(Of MemoryWorkspace)()

            Public Property CurrentWorkspace As MemoryWorkspace Implements MemoryManager.CurrentWorkspace
                Get
                    Return current
                End Get
                Set(ByVal value As MemoryWorkspace)
                    current = value
                    Changes.Add(value)
                End Set
            End Property

            Public Sub notifyScopeEntered() Implements MemoryManager.notifyScopeEntered
            End Sub

            Public Sub notifyScopeLeft() Implements MemoryManager.notifyScopeLeft
            End Sub

            Public Sub invokeGcOccasionally() Implements MemoryManager.invokeGcOccasionally
            End Sub

            Public Sub invokeGc() Implements MemoryManager.invokeGc
            End Sub

            Public Sub togglePeriodicGc(ByVal enabled As Boolean) Implements MemoryManager.togglePeriodicGc
            End Sub

            Public Sub toggleAveraging(ByVal enabled As Boolean) Implements MemoryManager.toggleAveraging
            End Sub

            Public ReadOnly Property PeriodicGcActive As Boolean Implements MemoryManager.PeriodicGcActive
                Get
                    Return False
                End Get
            End Property

            Public ReadOnly Property LastGcTime As Long Implements MemoryManager.LastGcTime
                Get
                    Return 0
                End Get
            End Property

            Public Property OccasionalGcFrequency As Integer Implements MemoryManager.OccasionalGcFrequency

            Public ReadOnly Property AverageLoopTime As Integer Implements MemoryManager.AverageLoopTime
                Get
                    Return 0
                End Get
            End Property

            Public Property AutoGcWindow As Integer Implements MemoryManager.AutoGcWindow

            Public Sub collect(ParamArray ByVal arrays() As INDArray) Implements MemoryManager.collect
            End Sub

            Public Sub purgeCaches() Implements MemoryManager.purgeCaches
            End Sub

            Public Sub memcpy(ByVal dstBuffer As DataBuffer, ByVal srcBuffer As DataBuffer) Implements MemoryManager.memcpy
                Throw New NotSupportedException()
            End Sub

            Public Sub memset(ByVal array As INDArray) Implements MemoryManager.memset
                Throw New NotSupportedException()
            End Sub

            Public Function scopeOutOfWorkspaces() As MemoryWorkspace Implements MemoryManager.scopeOutOfWorkspaces
                Return New DummyWorkspace().notifyScopeEntered()
            End Function

            Public ReadOnly Property BandwidthUse As IDictionary(Of Integer, Long) Implements MemoryManager.BandwidthUse
                Get
                    Return Nothing
                End Get
            End Property

            Public Function allocatedMemory(ByVal deviceId As Integer?) As Long Implements MemoryManager.allocatedMemory
                Return 0
            End Function

            Public Sub releaseCurrentContext() Implements MemoryManager.releaseCurrentContext
            End Sub
        End Class

        <TestMethod>
        Public Sub testScopeOutAndRestore()
            Dim manager As New FakeMemoryManager()
            Dim outer As New DummyWorkspace(manager)
            manager.CurrentWorkspace = outer
            Using ws As MemoryWorkspace = New DummyWorkspace(manager).notifyScopeEntered()
                ' inside the scope the thread is outside any workspace
                assertNull(manager.CurrentWorkspace)
            End Using
            ' Dispose (VB Using) closes it, which restores the previous workspace
            Assert.AreSame(outer, manager.CurrentWorkspace)
        End Sub

        <TestMethod>
        Public Sub testNestedScopes()
            Dim manager As New FakeMemoryManager()
            Dim outer As New DummyWorkspace(manager)
            manager.CurrentWorkspace = outer
            Dim first As New DummyWorkspace(manager)
            first.notifyScopeEntered()
            assertNull(manager.CurrentWorkspace)
            Dim second As New DummyWorkspace(manager)
            second.PreviousWorkspace = first
            manager.CurrentWorkspace = second
            Dim third As New DummyWorkspace(manager)
            third.notifyScopeEntered()
            assertNull(manager.CurrentWorkspace)
            third.notifyScopeLeft()
            Assert.AreSame(second, manager.CurrentWorkspace)
            first.close()
            Assert.AreSame(outer, manager.CurrentWorkspace)
        End Sub

        <TestMethod>
        Public Sub testMemoryManagerIsRequired()
            assertThrows(Of ArgumentNullException)(Sub() Call New DummyWorkspace(DirectCast(Nothing, MemoryManager)).close())
        End Sub

        <TestMethod>
        Public Sub testDummyProperties()
            Dim ws As New DummyWorkspace()
            assertEquals(MemoryWorkspace.Type.DUMMY, ws.WorkspaceType)
            assertNull(ws.WorkspaceConfiguration)
            assertNull(ws.Id)
            assertNull(ws.ParentWorkspace)
            assertNull(ws.notifyScopeBorrowed())
            assertFalse(ws.ScopeActive)
            assertEquals(-1L, ws.ThreadId)
            assertEquals(0, ws.DeviceId)
            assertEquals(0L, ws.GenerationId)
            assertEquals(0L, ws.ThisCycleAllocations)
            assertEquals(0L, ws.LastCycleAllocations)
            assertEquals(0L, ws.MaxCycleAllocations)
            assertEquals(0L, ws.CurrentSize)
            assertEquals(0L, ws.CurrentOffset)
            assertEquals(0L, ws.PrimaryOffset)
            assertEquals(0, ws.targetDevice())
            Assert.AreSame(ws, ws.tagOutOfScopeUse())
            ' a fresh random id per call, as in Java (UUID.randomUUID())
            assertNotEquals(ws.UniqueId, ws.UniqueId)
            Guid.Parse(ws.UniqueId)
            ws.deallocator().deallocate()
            ws.initializeWorkspace()
            ws.destroyWorkspace()
            ws.destroyWorkspace(True)
            ws.toggleWorkspaceUse(False)
            ws.enableDebug(True)
        End Sub

        <TestMethod>
        Public Sub testWorkspacesCloseableClosesInReverseOrder()
            Dim manager As New FakeMemoryManager()
            Dim a As New DummyWorkspace(manager)
            Dim b As New DummyWorkspace(manager)
            Dim c As New DummyWorkspace(manager)
            Dim parentA As New DummyWorkspace(manager)
            Dim parentB As New DummyWorkspace(manager)
            Dim parentC As New DummyWorkspace(manager)
            a.PreviousWorkspace = parentA
            b.PreviousWorkspace = parentB
            c.PreviousWorkspace = parentC
            Using closeable As New WorkspacesCloseable(a, b, c)
                assertEquals(3, closeable.getWorkspaces().Length)
            End Using
            ' c is closed first, a last
            assertEquals(3, manager.Changes.Count)
            Assert.AreSame(parentC, manager.Changes(0))
            Assert.AreSame(parentB, manager.Changes(1))
            Assert.AreSame(parentA, manager.Changes(2))
            assertThrows(Of ArgumentNullException)(Sub() Call New WorkspacesCloseable(DirectCast(Nothing, MemoryWorkspace())).close())
            assertEquals(New WorkspacesCloseable(a, b), New WorkspacesCloseable(a, b))
            assertNotEquals(New WorkspacesCloseable(a, b), New WorkspacesCloseable(b, a))
        End Sub

    End Class

End Namespace
