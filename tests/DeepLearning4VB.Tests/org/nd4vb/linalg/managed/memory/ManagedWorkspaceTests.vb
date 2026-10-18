Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.enums
Imports DeepLearning4VB.org.nd4vb.linalg.managed.memory
Imports AllocationsTracker = DeepLearning4VB.org.nd4vb.linalg.api.memory.AllocationsTracker
Imports FakeArray = DeepLearning4VB.Tests.org.nd4vb.linalg.cache.FakeArray
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports Nd4vbWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.Nd4vbWorkspace
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports ND4VBWorkspaceException = DeepLearning4VB.org.nd4vb.linalg.workspace.ND4VBWorkspaceException
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration
Imports WorkspaceUtils = DeepLearning4VB.org.nd4vb.linalg.workspace.WorkspaceUtils

Namespace org.nd4vb.linalg.managed.memory

    ''' <summary>
    ''' Workspace scoping, borrowing, nesting, learning and the exceptions, ported from the upstream
    ''' <c>BasicWorkspaceTests</c>, <c>WorkspaceProviderTests</c>, <c>DebugModeTests</c> and <c>SpecialWorkspaceTests</c>
    ''' with data buffers in place of arrays: an array created in a workspace allocates its buffer there, so
    ''' <see cref="create"/> does what <c>Nd4j.create</c> does. Expected sizes follow the 32-byte alignment of the ported
    ''' workspace (some upstream tests still assume 16 bytes).
    ''' </summary>
    <TestClass>
    Public Class ManagedWorkspaceTests

        Private mm As ManagedMemoryManager
        Private wm As ManagedWorkspaceManager

        ' BasicWorkspaceTests configurations
        Private Shared ReadOnly basicConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).maxSize(10 * 1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()
        Private Shared ReadOnly loopOverTimeConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()
        Private Shared ReadOnly loopFirstConfig As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(0.1).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()
        ' WorkspaceProviderTests configurations
        Private Shared ReadOnly basicConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(81920).overallocationLimit(0.1).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.NONE).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.OVERALLOCATE).build()
        Private Shared ReadOnly loopConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.1).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.STRICT).build()
        Private Shared ReadOnly reallocateConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.1).policySpill(SpillPolicy.REALLOCATE).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.STRICT).build()
        Private Shared ReadOnly reallocateDelayedConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).overallocationLimit(0.1).policySpill(SpillPolicy.REALLOCATE).cyclesBeforeInitialization(3).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.STRICT).build()
        Private Shared ReadOnly circularConfiguration As WorkspaceConfiguration = WorkspaceConfiguration.builder().minSize(10 * 1024L * 1024L).overallocationLimit(1.0).policySpill(SpillPolicy.EXTERNAL).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policyAllocation(AllocationPolicy.STRICT).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).build()

        <TestInitialize>
        Public Sub setUp()
            mm = New ManagedMemoryManager()
            wm = New ManagedWorkspaceManager(mm, Nothing)
        End Sub

        ''' <summary>A buffer allocated like an array: in the current workspace of the memory manager, if any.</summary>
        Private Function create(ByVal type As DataType, ByVal length As Long) As ManagedDataBuffer
            Dim ws As MemoryWorkspace = mm.CurrentWorkspace
            If ws Is Nothing Then
                Return New ManagedDataBuffer(type, length)
            End If
            Return New ManagedDataBuffer(type, length, True, ws)
        End Function

        Private Shared Function aligned(ByVal bytes As Long) As Long
            Return ((bytes + 31) \ 32) * 32
        End Function

        Private Function ws(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As Nd4vbWorkspace
            Return DirectCast(wm.getAndActivateWorkspace(configuration, id), Nd4vbWorkspace)
        End Function

        <TestMethod>
        Public Sub testAlignment()
            Using w As Nd4vbWorkspace = ws(basicConfig, "ALIGN")
                assertEquals(0L, w.PrimaryOffset)
                create(DataType.DOUBLE, 5)
                assertEquals(64L, w.PrimaryOffset)
                create(DataType.FLOAT, 8)
                assertEquals(96L, w.PrimaryOffset)
                assertEquals(96L, w.ThisCycleAllocations)
                assertEquals(96L, w.HostOffset)
                assertEquals(96L, w.DeviceOffset)
            End Using
        End Sub

        <TestMethod>
        Public Sub testOverallocation1()
            Dim config As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1024).maxSize(10 * 1024 * 1024).overallocationLimit(1.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.NONE).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()
            Dim workspace As Nd4vbWorkspace = DirectCast(wm.createNewWorkspace(config), Nd4vbWorkspace)
            assertEquals(2048L, workspace.CurrentSize)
        End Sub

        <TestMethod>
        Public Sub testOverallocation2()
            Dim config As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(1.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()
            Dim workspace As Nd4vbWorkspace = DirectCast(wm.createNewWorkspace(config), Nd4vbWorkspace)
            assertEquals(0L, workspace.CurrentSize)
            Using cW As MemoryWorkspace = workspace.notifyScopeEntered()
                create(DataType.DOUBLE, 100)
            End Using
            ' 800 bytes * 2 as overallocation coefficient
            assertEquals(200L * 8, workspace.CurrentSize)
        End Sub

        <TestMethod>
        Public Sub testOverallocation3()
            Dim config As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).maxSize(10 * 1024 * 1024).overallocationLimit(1.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.OVER_TIME).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()
            Dim workspace As Nd4vbWorkspace = DirectCast(wm.createNewWorkspace(config), Nd4vbWorkspace)
            For x As Integer = 10 To 100 Step 10
                Using cW As MemoryWorkspace = workspace.notifyScopeEntered()
                    create(DataType.DOUBLE, x)
                End Using
            Next x
            assertEquals(0L, workspace.CurrentSize)
            workspace.initializeWorkspace()
            assertEquals(200L * 8, workspace.CurrentSize)
        End Sub

        <TestMethod>
        Public Sub testMinSize1()
            Dim conf As WorkspaceConfiguration = WorkspaceConfiguration.builder().minSize(10 * 1024 * 1024).overallocationLimit(1.0).policyAllocation(AllocationPolicy.OVERALLOCATE).policyLearning(LearningPolicy.FIRST_LOOP).policyMirroring(MirroringPolicy.FULL).policySpill(SpillPolicy.EXTERNAL).build()
            Using workspace As Nd4vbWorkspace = ws(conf, "WT")
                create(DataType.FLOAT, 100)
                assertEquals(0L, workspace.CurrentSize)
            End Using
            Using workspace As Nd4vbWorkspace = ws(conf, "WT")
                create(DataType.FLOAT, 100)
                assertEquals(10L * 1024 * 1024, workspace.CurrentSize)
            End Using
        End Sub

        <TestMethod>
        Public Sub testScope1And2()
            Dim array As ManagedDataBuffer
            Using wsI As Nd4vbWorkspace = ws(basicConfig, "ITER")
                array = create(DataType.DOUBLE, 100)
                assertTrue(array.InScope)
            End Using
            assertFalse(array.InScope)

            Using wsI As Nd4vbWorkspace = ws(loopFirstConfig, "ITER2")
                array = create(DataType.DOUBLE, 100)
                assertTrue(array.InScope)
                assertEquals(0L, wsI.CurrentSize)
            End Using
            Using wsI As Nd4vbWorkspace = ws(loopFirstConfig, "ITER2")
                array = create(DataType.DOUBLE, 100)
                assertTrue(array.InScope)
                assertEquals(800L, wsI.PrimaryOffset)
            End Using
            assertFalse(array.InScope)
        End Sub

        <TestMethod>
        Public Sub testToggle1()
            Dim workspace As Nd4vbWorkspace = DirectCast(wm.createNewWorkspace(loopFirstConfig), Nd4vbWorkspace)
            assertEquals(0L, workspace.PrimaryOffset)
            Using cW As MemoryWorkspace = workspace.notifyScopeEntered()
                create(DataType.DOUBLE, 100)
                cW.toggleWorkspaceUse(False)
                Dim detachedLike As ManagedDataBuffer = create(DataType.DOUBLE, 100)
                ' the allocation goes around the workspace, but the buffer still belongs to it (Java)
                assertTrue(detachedLike.Attached)
                cW.toggleWorkspaceUse(True)
                create(DataType.DOUBLE, 100)
            End Using
            assertEquals(0L, workspace.PrimaryOffset)
            assertEquals(200L * 8, workspace.CurrentSize)

            Using cW As MemoryWorkspace = workspace.notifyScopeEntered()
                create(DataType.DOUBLE, 100)
                cW.toggleWorkspaceUse(False)
                create(DataType.DOUBLE, 100)
                cW.toggleWorkspaceUse(True)
                assertEquals(100L * 8, workspace.PrimaryOffset)
                create(DataType.DOUBLE, 100)
                assertEquals(200L * 8, workspace.PrimaryOffset)
            End Using
        End Sub

        <TestMethod>
        Public Sub testLoops1()
            Dim workspace As Nd4vbWorkspace = DirectCast(wm.createNewWorkspace(loopOverTimeConfig), Nd4vbWorkspace)
            workspace.notifyScopeEntered()
            create(DataType.DOUBLE, 10)
            ' the workspace has no memory yet: the allocation spilled
            assertEquals(0L, workspace.PrimaryOffset)
            assertEquals(0L, workspace.CurrentSize)
            assertEquals(1, workspace.NumberOfExternalAllocations)
            assertEquals(aligned(80), workspace.SpilledSize)
            workspace.notifyScopeLeft()
            workspace.initializeWorkspace()
            assertEquals(aligned(80), workspace.CurrentSize)

            For x As Integer = 0 To 9
                assertEquals(0L, workspace.PrimaryOffset)
                workspace.notifyScopeEntered()
                create(DataType.DOUBLE, 10)
                assertEquals(aligned(80), workspace.PrimaryOffset, "Failed on iteration " & x)
                workspace.notifyScopeLeft()
                assertEquals(0L, workspace.PrimaryOffset)
            Next x
            assertEquals(11L, workspace.CyclesCount)
            assertEquals(aligned(80), workspace.LastCycleAllocations)
            assertEquals(aligned(80), workspace.MaxCycleAllocations)
        End Sub

        <TestMethod>
        Public Sub testIsAttachedAndScopeOut()
            Dim outside As ManagedDataBuffer = create(DataType.FLOAT, 10)
            assertFalse(outside.Attached)
            Using w As Nd4vbWorkspace = ws(basicConfig, "ATTACH")
                Dim inside As ManagedDataBuffer = create(DataType.FLOAT, 10)
                assertTrue(inside.Attached)
                Using scoped As MemoryWorkspace = wm.scopeOutOfWorkspaces()
                    assertNull(mm.CurrentWorkspace)
                    Dim detached As ManagedDataBuffer = create(DataType.FLOAT, 10)
                    assertFalse(detached.Attached)
                    ' scoping out tags the workspace: its scope stays open
                    assertTrue(w.ScopeActive)
                End Using
                Assert.AreSame(w, mm.CurrentWorkspace)
                assertTrue(create(DataType.FLOAT, 10).Attached)
            End Using
            assertNull(mm.CurrentWorkspace)
        End Sub

        <TestMethod>
        Public Sub testNestedWorkspaces1()
            wm.DefaultWorkspaceConfiguration = basicConfiguration
            Using ws1 As Nd4vbWorkspace = DirectCast(wm.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), Nd4vbWorkspace)
                create(DataType.FLOAT, 100)
                assertEquals(aligned(400), ws1.PrimaryOffset)
                Using ws2 As Nd4vbWorkspace = DirectCast(wm.getWorkspaceForCurrentThread("WS2").notifyScopeEntered(), Nd4vbWorkspace)
                    assertEquals(0L, ws2.PrimaryOffset)
                    create(DataType.FLOAT, 100)
                    assertEquals(aligned(400), ws1.PrimaryOffset)
                    assertEquals(aligned(400), ws2.PrimaryOffset)
                    Assert.AreSame(ws1, ws2.ParentWorkspace)
                End Using
                Assert.AreSame(ws1, mm.CurrentWorkspace)
            End Using
            assertNull(mm.CurrentWorkspace)
            wm.destroyAllWorkspacesForCurrentThread()
            assertEquals(0, wm.AllWorkspacesForCurrentThread.Count)
        End Sub

        <TestMethod>
        Public Sub testNestedWorkspaces2()
            wm.DefaultWorkspaceConfiguration = basicConfiguration
            Using ws1 As Nd4vbWorkspace = DirectCast(wm.getWorkspaceForCurrentThread("WS1").notifyScopeEntered(), Nd4vbWorkspace)
                create(DataType.FLOAT, 100)
                For x As Integer = 1 To 100
                    Using ws2 As MemoryWorkspace = wm.getWorkspaceForCurrentThread(loopConfiguration, "WS2").notifyScopeEntered()
                        create(DataType.FLOAT, x)
                    End Using
                    Dim ws2b As Nd4vbWorkspace = DirectCast(wm.getWorkspaceForCurrentThread("WS2"), Nd4vbWorkspace)
                    assertEquals(aligned(x * 4L), ws2b.LastCycleAllocations)
                Next x
                wm.getWorkspaceForCurrentThread("WS2").initializeWorkspace()
                assertEquals(aligned(400), wm.getWorkspaceForCurrentThread("WS2").CurrentSize)
            End Using
            assertNull(mm.CurrentWorkspace)
        End Sub

        <TestMethod>
        Public Sub testReallocate1()
            Using w As MemoryWorkspace = wm.getAndActivateWorkspace(reallocateConfiguration, "WS_1")
                create(DataType.DOUBLE, 100)
            End Using
            Dim workspace As Nd4vbWorkspace = DirectCast(wm.getWorkspaceForCurrentThread(reallocateConfiguration, "WS_1"), Nd4vbWorkspace)
            workspace.initializeWorkspace()
            assertEquals(800L, workspace.CurrentSize)
            Using w As MemoryWorkspace = wm.getAndActivateWorkspace(reallocateConfiguration, "WS_1")
                create(DataType.DOUBLE, 1000)
            End Using
            assertEquals(8000L, workspace.MaxCycleAllocations)
            workspace.initializeWorkspace()
            assertEquals(8000L, workspace.CurrentSize)
        End Sub

        <TestMethod>
        Public Sub testReallocate2()
            Dim workspace As MemoryWorkspace = wm.getWorkspaceForCurrentThread(reallocateDelayedConfiguration, "WS_1")
            For i As Integer = 1 To 10
                Using w As MemoryWorkspace = wm.getAndActivateWorkspace(reallocateDelayedConfiguration, "WS_1")
                    create(DataType.FLOAT, 100 * i)
                End Using
                If i >= 3 Then
                    assertEquals(aligned(400L * i), workspace.CurrentSize, "Failed on iteration " & i)
                Else
                    assertEquals(0L, workspace.CurrentSize)
                End If
            Next i
        End Sub

        <TestMethod>
        Public Sub testCircularLearning1()
            For i As Integer = 0 To 1
                Using w As MemoryWorkspace = wm.getAndActivateWorkspace(circularConfiguration, "WSX")
                    create(DataType.FLOAT, 10)
                End Using
                Dim workspace As Nd4vbWorkspace = DirectCast(wm.getWorkspaceForCurrentThread(circularConfiguration, "WSX"), Nd4vbWorkspace)
                assertEquals(MemoryWorkspace.Type.CIRCULAR, workspace.WorkspaceType)
                assertEquals(10 * 1024 * 1024L, workspace.CurrentSize)
                If i = 0 Then
                    assertEquals(0L, workspace.PrimaryOffset)
                Else
                    ' the block is aligned to the learned block size (1.3 x 64, rounded up to 8)
                    assertEquals(workspace.InitialBlockSize, workspace.PrimaryOffset)
                    assertEquals(88L, workspace.InitialBlockSize)
                End If
            Next i
        End Sub

        <TestMethod>
        Public Sub testCircularBufferReset1()
            Dim workspace As Nd4vbWorkspace = DirectCast(wm.getWorkspaceForCurrentThread(circularConfiguration, "WSR_1"), Nd4vbWorkspace)
            Using w As MemoryWorkspace = wm.getAndActivateWorkspace("WSR_1")
                create(DataType.FLOAT, 10000)
                assertEquals(0L, workspace.CurrentSize)
                assertEquals(1, workspace.NumberOfExternalAllocations)
            End Using
            assertEquals(10 * 1024L * 1024L, workspace.CurrentSize)
            assertEquals(0L, workspace.PrimaryOffset)
            assertEquals(1, workspace.NumberOfExternalAllocations)
            For i As Integer = 0 To (11 * 1024 * 1024) - 1 Step 10000 * 4
                Using w As MemoryWorkspace = wm.getAndActivateWorkspace("WSR_1")
                    create(DataType.FLOAT, 10000)
                End Using
            Next i
            ' the buffer end was reached: the circular reset purged the spilled allocation
            assertEquals(0, workspace.NumberOfExternalAllocations)
        End Sub

        <TestMethod>
        Public Sub testWorkspaceOrder_1()
            Dim conf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1000000).overallocationLimit(0.05).policyLearning(LearningPolicy.NONE).build()
            Dim exp As New List(Of String) From {"outer", Nothing, "outer", "inner", "outer", Nothing}
            Dim res As New List(Of String)()
            Dim current As Func(Of String) = Function() If(mm.CurrentWorkspace Is Nothing, Nothing, mm.CurrentWorkspace.Id)
            Using w1 As MemoryWorkspace = wm.getAndActivateWorkspace(conf, "outer")
                Using w2 As MemoryWorkspace = wm.getAndActivateWorkspace(conf, "inner")
                    Using w3 As MemoryWorkspace = w1.notifyScopeBorrowed()
                        res.Add(current())
                        Using w4 As MemoryWorkspace = wm.scopeOutOfWorkspaces()
                            res.Add(current())
                        End Using
                        res.Add(current())
                    End Using
                    res.Add(current())
                End Using
                res.Add(current())
            End Using
            res.Add(current())
            assertEquals(exp, res)
        End Sub

        <TestMethod>
        Public Sub testBorrowing()
            Using outer As Nd4vbWorkspace = ws(basicConfig, "OUTER")
                Using inner As Nd4vbWorkspace = ws(basicConfig, "INNER")
                    Dim borrowed As MemoryWorkspace = outer.notifyScopeBorrowed()
                    Assert.AreSame(outer, borrowed)
                    Assert.AreSame(outer, mm.CurrentWorkspace)
                    Dim b As ManagedDataBuffer = create(DataType.FLOAT, 4)
                    Assert.AreSame(outer, b.ParentWorkspace)
                    Dim ex = assertThrows(Of ND4VBIllegalStateException)(Sub() outer.notifyScopeBorrowed())
                    assertEquals("Workspace [OUTER]: Can't borrow from borrowed workspace", ex.Message)
                    borrowed.close()
                    ' closing a borrowed workspace restores the borrowing one and keeps the scope open
                    Assert.AreSame(inner, mm.CurrentWorkspace)
                    assertTrue(outer.ScopeActive)
                End Using
                Assert.AreSame(outer, mm.CurrentWorkspace)
            End Using
        End Sub

        <TestMethod>
        Public Sub testReenteringIncrementsTheTagScope()
            Using w As Nd4vbWorkspace = ws(basicConfig, "TAG")
                Dim generation As Long = w.GenerationId
                Dim again As MemoryWorkspace = wm.getAndActivateWorkspace(basicConfig, "TAG")
                Assert.AreSame(w, again)
                assertEquals(generation, w.GenerationId)
                again.close()
                ' the inner close only undid the re-entry
                assertTrue(w.ScopeActive)
                Assert.AreSame(w, mm.CurrentWorkspace)
                assertEquals(0L, w.CyclesCount)
            End Using
            assertFalse(DirectCast(wm.getWorkspaceForCurrentThread("TAG"), Nd4vbWorkspace).ScopeActive)
        End Sub

        <TestMethod>
        Public Sub testGenerationsAndOutOfScopeChecks()
            Dim w As Nd4vbWorkspace = DirectCast(wm.getWorkspaceForCurrentThread(basicConfig, "GEN"), Nd4vbWorkspace)
            assertEquals(0L, w.GenerationId)
            Dim b As ManagedDataBuffer
            Using s As MemoryWorkspace = w.notifyScopeEntered()
                b = create(DataType.DOUBLE, 3)
                assertEquals(1L, b.GenerationId)
                WorkspaceUtils.assertValidArray(New FakeArray(b), "ok", wm)
                WorkspaceUtils.assertOpenAndActive("GEN", "not open", wm)
                WorkspaceUtils.assertOpenActiveAndCurrent("GEN", "not current", wm, mm)
                assertThrows(Of ND4VBWorkspaceException)(Sub() WorkspaceUtils.assertNoWorkspacesOpen("open", False, wm, mm))
                Using scoped As MemoryWorkspace = wm.scopeOutOfWorkspaces()
                    WorkspaceUtils.assertNoWorkspacesOpen("scoped out", True, wm, mm)
                    assertThrows(Of ND4VBWorkspaceException)(Sub() WorkspaceUtils.assertOpenActiveAndCurrent("GEN", "not current", wm, mm))
                End Using
            End Using
            WorkspaceUtils.assertNoWorkspacesOpen("none open", False, wm, mm)
            assertThrows(Of ND4VBWorkspaceException)(Sub() WorkspaceUtils.assertOpenAndActive("GEN", "not open", wm))
            Dim leaked = assertThrows(Of ND4VBWorkspaceException)(Sub() WorkspaceUtils.assertValidArray(New FakeArray(b), "leak", wm))
            assertTrue(leaked.Message.StartsWith("leak: Array uses leaked workspace pointer from workspace GEN"))
            Using s As MemoryWorkspace = w.notifyScopeEntered()
                Dim outdated = assertThrows(Of ND4VBWorkspaceException)(Sub() WorkspaceUtils.assertValidArray(New FakeArray(b), Nothing, wm))
                assertTrue(outdated.Message.StartsWith("Array outdated workspace pointer from workspace GEN (array generation 1, current workspace generation 2)"))
            End Using
            ' detached arrays are always valid
            WorkspaceUtils.assertValidArray(New FakeArray(New ManagedDataBuffer(DataType.INT, 1)), Nothing, wm)
            WorkspaceUtils.assertValidArray(Nothing, Nothing, wm)
        End Sub

        <TestMethod>
        Public Sub testSpillPolicyFail()
            Dim conf As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(64).policyAllocation(AllocationPolicy.STRICT).policyLearning(LearningPolicy.NONE).policySpill(SpillPolicy.FAIL).build()
            Using w As Nd4vbWorkspace = ws(conf, "FULL")
                create(DataType.DOUBLE, 8)
                Dim ex = assertThrows(Of ND4VBIllegalStateException)(Sub() create(DataType.DOUBLE, 1))
                assertEquals("Can't allocate memory: Workspace is full", ex.Message)
            End Using
        End Sub

        <TestMethod>
        Public Sub testConfigurationErrors()
            Dim circularOverallocate As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1024).policyAllocation(AllocationPolicy.OVERALLOCATE).overallocationLimit(0.5).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).build()
            assertThrows(Of ND4VBIllegalStateException)(Sub() wm.createNewWorkspace(circularOverallocate))
            Dim mmapLearning As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1024).policyLocation(LocationPolicy.MMAP).policyLearning(LearningPolicy.FIRST_LOOP).build()
            assertThrows(Of ArgumentException)(Sub() wm.createNewWorkspace(mmapLearning))
            Dim mmapNoSize As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(0).policyLocation(LocationPolicy.MMAP).policyLearning(LearningPolicy.NONE).build()
            assertThrows(Of ND4VBIllegalStateException)(Sub() wm.createNewWorkspace(mmapNoSize))
            ' a circular workspace with overallocation counts steps of (limit + 1) cycles
            Dim circular As WorkspaceConfiguration = WorkspaceConfiguration.builder().initialSize(1024).policyAllocation(AllocationPolicy.OVERALLOCATE).overallocationLimit(2.0).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyLearning(LearningPolicy.NONE).build()
            Dim cw As Nd4vbWorkspace = DirectCast(wm.createNewWorkspace(circular, "STEPS"), Nd4vbWorkspace)
            For i As Integer = 1 To 7
                cw.notifyScopeEntered()
                cw.close()
            Next
            ' cycles 4 and 7 complete a step of 3 cycles
            assertEquals(2L, cw.StepNumber)
        End Sub

        <TestMethod>
        Public Sub testDebugModes()
            assertEquals(DebugMode.DISABLED, wm.DebugMode)
            wm.DebugMode = DebugMode.SPILL_EVERYTHING
            assertEquals(DebugMode.SPILL_EVERYTHING, wm.DebugMode)
            Using w As Nd4vbWorkspace = ws(basicConfig, "R_119_1993")
                assertEquals(10 * 1024 * 1024L, w.CurrentSize)
                Dim array As ManagedDataBuffer = create(DataType.DOUBLE, 100)
                assertTrue(array.Attached)
                ' nothing should get into workspace
                assertEquals(0L, w.PrimaryOffset)
                assertEquals(0L, w.DeviceOffset)
                ' array buffer should be spilled now
                assertEquals(100L * 8, w.SpilledSize)
            End Using

            wm.DebugMode = DebugMode.BYPASS_EVERYTHING
            Using w As MemoryWorkspace = wm.getAndActivateWorkspace(loopFirstConfig, "R_119_1994")
                assertEquals(MemoryWorkspace.Type.DUMMY, w.WorkspaceType)
                assertFalse(create(DataType.DOUBLE, 100).Attached)
            End Using
        End Sub

        <TestMethod>
        Public Sub testDestroyAndTracker()
            Dim before As Long = AllocationsTracker.Instance.bytesOnDevice(AllocationKind.WORKSPACE, 0)
            Dim w As ManagedWorkspace = DirectCast(wm.createNewWorkspace(basicConfiguration, "TRACKED"), ManagedWorkspace)
            ' 81920 bytes + 10% overallocation, plus Java's safety offset
            assertEquals(90112L, w.CurrentSize)
            assertEquals(90112L + 1024, w.TrackedBytes)
            assertEquals(before + 90112L + 1024, AllocationsTracker.Instance.bytesOnDevice(AllocationKind.WORKSPACE, 0))
            w.deallocator().deallocate()
            assertEquals(0L, w.TrackedBytes)
            assertEquals(before, AllocationsTracker.Instance.bytesOnDevice(AllocationKind.WORKSPACE, 0))
            w.notifyScopeEntered()
            create(DataType.FLOAT, 10)
            w.close()
            w.destroyWorkspace()
            assertEquals(0L, w.CurrentSize)
            assertEquals(0L, w.PrimaryOffset)
            assertTrue(w.UniqueId.StartsWith("Workspace_TRACKED_"))
            assertEquals(w.UniqueId, w.UniqueId)
            assertEquals(0, w.targetDevice())
            assertTrue(w.ToString().Contains("id='TRACKED'"))
        End Sub

    End Class

End Namespace
