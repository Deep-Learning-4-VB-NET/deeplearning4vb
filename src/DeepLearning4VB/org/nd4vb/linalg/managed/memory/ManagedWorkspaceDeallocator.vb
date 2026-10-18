' DeepLearning4VB: releases a collected workspace's bookkeeping (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.workspace.CpuWorkspaceDeallocator.

Imports System
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory
Imports AllocationKind = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.AllocationKind

Namespace org.nd4vb.linalg.managed.memory

    ''' <summary>
    ''' The deallocator of a <see cref="ManagedWorkspace"/> (Java <c>CpuWorkspaceDeallocator</c>). Java freed the workspace's
    ''' native blocks after the workspace was collected; managed memory needs no freeing, so this releases the workspace bytes
    ''' the workspace still charges to the <see cref="AllocationsTracker"/>. It shares only a <see cref="TrackedBytes"/>
    ''' counter with the workspace, so it does not keep the workspace alive.
    ''' </summary>
    Public Class ManagedWorkspaceDeallocator
        Implements Deallocator

        ''' <summary>The workspace bytes charged to the allocation tracker, shared by a workspace and its deallocator.</summary>
        Public NotInheritable Class TrackedBytes
            Private bytesField As Long
            Private deviceField As Integer

            ''' <summary>The bytes currently charged.</summary>
            Public ReadOnly Property Bytes As Long
                Get
                    SyncLock Me
                        Return bytesField
                    End SyncLock
                End Get
            End Property

            ''' <summary>Charges <paramref name="bytes"/> workspace bytes on <paramref name="device"/>, releasing any previous charge first.</summary>
            Public Sub charge(ByVal device As Integer, ByVal bytes As Long)
                SyncLock Me
                    release()
                    AllocationsTracker.Instance.markAllocated(AllocationKind.WORKSPACE, device, bytes)
                    bytesField = bytes
                    deviceField = device
                End SyncLock
            End Sub

            ''' <summary>Releases the current charge (if any).</summary>
            Public Sub release()
                SyncLock Me
                    If bytesField <> 0 Then
                        AllocationsTracker.Instance.markReleased(AllocationKind.WORKSPACE, deviceField, bytesField)
                        bytesField = 0
                    End If
                End SyncLock
            End Sub
        End Class

        Private ReadOnly state As TrackedBytes
        Private ReadOnly deviceId As Integer

        ''' <summary>A deallocator for the bytes tracked in <paramref name="state"/>.</summary>
        Public Sub New(ByVal state As TrackedBytes, ByVal deviceId As Integer)
            If state Is Nothing Then
                Throw New ArgumentNullException(NameOf(state))
            End If
            Me.state = state
            Me.deviceId = deviceId
        End Sub

        ''' <summary>Releases the workspace bytes still charged to the allocation tracker.</summary>
        Public Overridable Sub deallocate() Implements Deallocator.deallocate
            state.release()
        End Sub

    End Class

End Namespace
