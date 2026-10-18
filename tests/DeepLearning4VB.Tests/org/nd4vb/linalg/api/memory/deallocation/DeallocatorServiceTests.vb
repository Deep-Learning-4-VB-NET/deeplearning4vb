Imports System
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.deallocation

Namespace org.nd4vb.linalg.api.memory.deallocation

    ''' <summary>Tests for the thread-less <see cref="DeallocatorService"/>: deallocators run once their objects are collected.</summary>
    <TestClass>
    Public Class DeallocatorServiceTests

        Private Class Counter
            Public Count As Integer
        End Class

        Private Class CountingDeallocator
            Implements Deallocator

            Private ReadOnly counter As Counter

            Public Sub New(ByVal counter As Counter)
                Me.counter = counter
            End Sub

            Public Sub deallocate() Implements Deallocator.deallocate
                counter.Count += 1
            End Sub
        End Class

        Private Class Resource
            Implements Deallocatable

            Private ReadOnly id As String
            Private ReadOnly counter As Counter

            Public Sub New(ByVal id As String, ByVal counter As Counter)
                Me.id = id
                Me.counter = counter
            End Sub

            Public ReadOnly Property UniqueId As String Implements Deallocatable.UniqueId
                Get
                    Return id
                End Get
            End Property

            Public Function deallocator() As Deallocator Implements Deallocatable.deallocator
                Return New CountingDeallocator(counter)
            End Function

            Public Function targetDevice() As Integer Implements Deallocatable.targetDevice
                Return 0
            End Function
        End Class

        <MethodImpl(MethodImplOptions.NoInlining)>
        Private Shared Sub registerGarbage(ByVal service As DeallocatorService, ByVal counter As Counter, ByVal count As Integer)
            For i As Integer = 1 To count
                service.pickObject(New Resource("garbage_" & i, counter))
            Next
        End Sub

        <TestMethod>
        Public Sub testCollectedObjectsAreDeallocated()
            Dim service As New DeallocatorService()
            Dim counter As New Counter()
            Dim alive As New Resource("alive", counter)
            service.pickObject(alive)
            registerGarbage(service, counter, 3)
            assertEquals(4, service.TrackedObjects)
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            assertEquals(3, service.purge())
            assertEquals(3, counter.Count)
            assertEquals(1, service.TrackedObjects)
            ' the live object stays tracked
            assertEquals(0, service.purge())
            GC.KeepAlive(alive)
        End Sub

        <TestMethod>
        Public Sub testNextValueAndArguments()
            Dim service As New DeallocatorService()
            Dim a As Long = service.nextValue()
            assertEquals(a + 1, service.nextValue())
            assertThrows(Of ArgumentNullException)(Sub() service.pickObject(Nothing))
        End Sub

    End Class

End Namespace
