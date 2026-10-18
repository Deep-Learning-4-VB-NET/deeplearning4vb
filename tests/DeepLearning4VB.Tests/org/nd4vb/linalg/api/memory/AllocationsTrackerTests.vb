Imports System
Imports System.Threading.Tasks
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.enums
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.stash
Imports DummyWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.DummyWorkspace
Imports DeallocatableReference = DeepLearning4VB.org.nd4vb.linalg.api.memory.deallocation.DeallocatableReference

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.memory

    ''' <summary>
    ''' Tests for the allocation trackers, <c>DeallocatableReference</c> and the placeholder stash manager.
    ''' </summary>
    <TestClass>
    Public Class AllocationsTrackerTests

        <TestMethod>
        Public Sub testDeviceAllocationsTrackerCountsPerKind()
            Dim t As New DeviceAllocationsTracker()
            t.updateState(AllocationKind.GENERAL, 100)
            t.updateState(AllocationKind.WORKSPACE, 30)
            t.updateState(AllocationKind.GENERAL, -40)
            assertEquals(60L, t.getState(AllocationKind.GENERAL))
            assertEquals(30L, t.getState(AllocationKind.WORKSPACE))
            assertEquals(0L, t.getState(AllocationKind.CONSTANT))
        End Sub

        <TestMethod>
        Public Sub testDeviceAllocationsTrackerIsThreadSafe()
            Dim t As New DeviceAllocationsTracker()
            Parallel.For(0, 1000, Sub(i) t.updateState(AllocationKind.CONSTANT, 3))
            assertEquals(3000L, t.getState(AllocationKind.CONSTANT))
        End Sub

        <TestMethod>
        Public Sub testAllocationsTrackerSingleton()
            Dim tracker As AllocationsTracker = AllocationsTracker.Instance
            Assert.AreSame(tracker, AllocationsTracker.getInstance())
            ' a device id nobody else uses, so other tests cannot interfere
            Dim device As Integer = 987654
            Dim before As Long = tracker.bytesOnDevice(device)
            tracker.markAllocated(AllocationKind.GENERAL, device, 512)
            tracker.markAllocated(AllocationKind.WORKSPACE, device, 64)
            assertEquals(before + 512, tracker.bytesOnDevice(device))
            assertEquals(before + 512, tracker.bytesOnDevice(AllocationKind.GENERAL, device))
            assertEquals(64L, tracker.bytesOnDevice(AllocationKind.WORKSPACE, device))
            tracker.markReleased(AllocationKind.GENERAL, device, 512)
            tracker.markReleased(AllocationKind.WORKSPACE, device, 64)
            assertEquals(before, tracker.bytesOnDevice(device))
            assertEquals(0L, tracker.bytesOnDevice(AllocationKind.WORKSPACE, device))
        End Sub

        <TestMethod>
        Public Sub testDeallocatableReference()
            Dim ws As New DummyWorkspace()
            Dim reference As New DeallocatableReference(ws)
            Assert.AreSame(ws, reference.get())
            assertNotNull(reference.getId())
            assertNotNull(reference.getDeallocator())
            reference.setId("id-1")
            assertEquals("id-1", reference.getId())

            Dim other As New DeallocatableReference(ws)
            other.setId("id-1")
            other.setDeallocator(reference.getDeallocator())
            assertEquals(reference, other)
            assertEquals(reference.GetHashCode(), other.GetHashCode())
            other.setId("id-2")
            assertFalse(reference.Equals(other))
            assertTrue(reference.ToString().StartsWith("DeallocatableReference(id=id-1, deallocator="), reference.ToString())
            GC.KeepAlive(ws)
        End Sub

        <TestMethod>
        Public Sub testBasicStashManagerIsAPlaceholder()
            Dim manager As New BasicStashManager()
            assertFalse(manager.checkIfStashExists("a"))
            assertNull(manager.getStash("a"))
            assertNull(manager.createStashIfNotExists(1))
        End Sub

    End Class

End Namespace
