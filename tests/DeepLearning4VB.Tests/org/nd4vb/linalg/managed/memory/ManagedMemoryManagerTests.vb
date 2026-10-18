Imports System
Imports System.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.managed.memory
Imports DummyWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.DummyWorkspace
Imports FakeArray = DeepLearning4VB.Tests.org.nd4vb.linalg.cache.FakeArray
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace

Namespace org.nd4vb.linalg.managed.memory

    ''' <summary>Tests for <see cref="ManagedMemoryManager"/> (and <c>BasicMemoryManager</c>).</summary>
    <TestClass>
    Public Class ManagedMemoryManagerTests

        <TestMethod>
        Public Sub testCurrentWorkspaceIsPerThread()
            Dim mm As New ManagedMemoryManager()
            assertNull(mm.CurrentWorkspace)
            Dim ws As New DummyWorkspace(mm)
            mm.CurrentWorkspace = ws
            Assert.AreSame(ws, mm.CurrentWorkspace)
            Dim seen As MemoryWorkspace = ws
            Dim t As New Thread(Sub() seen = mm.CurrentWorkspace)
            t.Start()
            t.Join()
            assertNull(seen)
            mm.CurrentWorkspace = Nothing
            assertNull(mm.CurrentWorkspace)
        End Sub

        <TestMethod>
        Public Sub testScopeOutOfWorkspaces()
            Dim mm As New ManagedMemoryManager()
            ' outside any workspace: a dummy workspace that does nothing
            Dim outside As MemoryWorkspace = mm.scopeOutOfWorkspaces()
            assertEquals(MemoryWorkspace.Type.DUMMY, outside.WorkspaceType)
            Dim outer As New DummyWorkspace(mm)
            mm.CurrentWorkspace = outer
            Using scoped As MemoryWorkspace = mm.scopeOutOfWorkspaces()
                assertNull(mm.CurrentWorkspace)
            End Using
            Assert.AreSame(outer, mm.CurrentWorkspace)
        End Sub

        <TestMethod>
        Public Sub testMemcpy()
            Dim mm As New ManagedMemoryManager()
            Dim src As New ManagedDataBuffer(DataType.LONG, New Long() {9007199254740993L, -1})
            Dim dst As New ManagedDataBuffer(DataType.LONG, 3)
            mm.memcpy(dst, src)
            assertArrayEquals(New Long() {9007199254740993L, -1, 0}, dst.asLong())
            ' views copy from and to their own offsets
            Dim view As New ManagedDataBuffer(dst, 2, 1)
            mm.memcpy(view, New ManagedDataBuffer(DataType.LONG, New Long() {5, 6}))
            assertArrayEquals(New Long() {9007199254740993L, 5, 6}, dst.asLong())
            assertThrows(Of NotSupportedException)(Sub() mm.memcpy(dst, New ManagedDataBuffer(DataType.INT, 3)))
            assertThrows(Of ArgumentNullException)(Sub() mm.memcpy(Nothing, src))
        End Sub

        <TestMethod>
        Public Sub testMemset()
            Dim mm As New ManagedMemoryManager()
            Dim root As New ManagedDataBuffer(DataType.FLOAT, New Single() {1, 2, 3, 4})
            Dim viewBuffer As New ManagedDataBuffer(root, 2, 1)
            mm.memset(New FakeArray(viewBuffer))
            assertArrayEquals(New Single() {1, 0, 0, 4}, root.asFloat(), 0.0F)
            ' a view array is assigned 0.0 instead
            Dim viewArray As New FakeArray(root) With {.IsViewValue = True}
            mm.memset(viewArray)
            assertEquals(0.0, CDbl(viewArray.AssignedValue), 0.0)
            assertArrayEquals(New Single() {1, 0, 0, 4}, root.asFloat(), 0.0F)
            Dim strings As New ManagedDataBuffer(New Collections.Generic.List(Of String) From {"a", "b"})
            mm.memset(New FakeArray(strings))
            assertEquals("", strings.getString(1))
            assertThrows(Of ArgumentNullException)(Sub() mm.memset(Nothing))
        End Sub

        <TestMethod>
        Public Sub testGcSettings()
            Dim mm As New ManagedMemoryManager()
            Dim before As Long = mm.LastGcTime
            Thread.Sleep(5)
            mm.invokeGc()
            assertTrue(mm.LastGcTime >= before)
            mm.togglePeriodicGc(True)
            ' the managed backend never uses periodic GC (as Java's CPU backend)
            assertFalse(mm.PeriodicGcActive)
            mm.OccasionalGcFrequency = 3
            assertEquals(3, mm.OccasionalGcFrequency)
            mm.AutoGcWindow = 0
            assertEquals(0, mm.AutoGcWindow)
            mm.toggleAveraging(True)
            For i As Integer = 1 To 5
                mm.invokeGcOccasionally()
            Next
            assertTrue(mm.AverageLoopTime >= 0)
            mm.toggleAveraging(False)
            assertEquals(0, mm.AverageLoopTime)
            assertNull(mm.BandwidthUse)
            assertTrue(mm.allocatedMemory(0) > 0)
            assertThrows(Of NotSupportedException)(Sub() mm.collect())
            mm.purgeCaches()
            mm.notifyScopeEntered()
            mm.notifyScopeLeft()
            mm.releaseCurrentContext()
        End Sub

    End Class

End Namespace
