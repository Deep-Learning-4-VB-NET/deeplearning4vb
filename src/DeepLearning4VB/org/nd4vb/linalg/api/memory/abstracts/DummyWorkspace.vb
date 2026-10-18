Imports System
Imports Deallocatable = DeepLearning4VB.org.nd4vb.linalg.api.memory.Deallocatable
Imports Deallocator = DeepLearning4VB.org.nd4vb.linalg.api.memory.Deallocator
Imports MemoryManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryManager
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration

Namespace org.nd4vb.linalg.api.memory.abstracts

    ''' <summary>
    ''' A workspace that does nothing: entering it detaches the current thread from its current workspace (arrays are
    ''' allocated outside any workspace) and closing it restores the previous one. Used by
    ''' <c>scopeOutOfWorkspaces()</c>.
    ''' <para>
    ''' Java reads and sets the current workspace through <c>Nd4j.getMemoryManager()</c>. The <c>Nd4j</c> factory is
    ''' ported in a later wave, so until then the memory manager comes from <see cref="MemoryManagerProvider"/>; when it
    ''' is Nothing, entering and closing have no effect. Java's <c>alloc(...)</c> overloads are deleted with the
    ''' interface's native-only members.
    ''' </para>
    ''' </summary>
    Public Class DummyWorkspace
        Implements MemoryWorkspace

        ''' <summary>
        ''' Supplies the memory manager whose current workspace this workspace saves and restores (Java:
        ''' <c>Nd4j.getMemoryManager()</c>). Replace with <c>Nd4vb.getMemoryManager()</c> when the factory is ported.
        ''' </summary>
        Friend Shared MemoryManagerProvider As Func(Of MemoryManager)

        Protected Friend parentWorkspaceField As MemoryWorkspace

        Private Shared Function currentMemoryManager() As MemoryManager
            Dim provider As Func(Of MemoryManager) = MemoryManagerProvider
            Return If(provider Is Nothing, Nothing, provider())
        End Function

        ''' <summary>
        ''' This method returns WorkspaceConfiguration bean that was used for given Workspace instance
        ''' </summary>
        Public Overridable ReadOnly Property WorkspaceConfiguration As WorkspaceConfiguration Implements MemoryWorkspace.WorkspaceConfiguration
            Get
                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' This method returns Id of this workspace
        ''' </summary>
        Public Overridable ReadOnly Property Id As String Implements MemoryWorkspace.Id
            Get
                Return Nothing
            End Get
        End Property

        Public Overridable ReadOnly Property ThreadId As Long? Implements MemoryWorkspace.ThreadId
            Get
                Return -1L
            End Get
        End Property

        Public Overridable ReadOnly Property DeviceId As Integer Implements MemoryWorkspace.DeviceId
            Get
                Return 0
            End Get
        End Property

        Public Overridable ReadOnly Property WorkspaceType As MemoryWorkspace.Type Implements MemoryWorkspace.WorkspaceType
            Get
                Return MemoryWorkspace.Type.DUMMY
            End Get
        End Property

        Public Overridable ReadOnly Property GenerationId As Long Implements MemoryWorkspace.GenerationId
            Get
                Return 0L
            End Get
        End Property

        ''' <summary>
        ''' This method notifies given Workspace that new use cycle is starting now: the current workspace is
        ''' remembered and the thread leaves it.
        ''' </summary>
        Public Overridable Function notifyScopeEntered() As MemoryWorkspace Implements MemoryWorkspace.notifyScopeEntered
            Dim manager As MemoryManager = currentMemoryManager()
            If manager IsNot Nothing Then
                parentWorkspaceField = manager.CurrentWorkspace
                manager.CurrentWorkspace = Nothing
            End If
            Return Me
        End Function

        ''' <summary>
        ''' This method TEMPORARY enters this workspace, without reset applied
        ''' </summary>
        Public Overridable Function notifyScopeBorrowed() As MemoryWorkspace Implements MemoryWorkspace.notifyScopeBorrowed
            Return Nothing
        End Function

        ''' <summary>
        ''' This method notifies given Workspace that use cycle just ended
        ''' </summary>
        Public Overridable Function notifyScopeLeft() As MemoryWorkspace Implements MemoryWorkspace.notifyScopeLeft
            close()
            Return Me
        End Function

        ''' <summary>
        ''' This method returns True if scope was opened, and not closed yet.
        ''' </summary>
        Public Overridable ReadOnly Property ScopeActive As Boolean Implements MemoryWorkspace.ScopeActive
            Get
                Return False
            End Get
        End Property

        Public Overridable Sub initializeWorkspace() Implements MemoryWorkspace.initializeWorkspace
        End Sub

        Public Overridable Sub destroyWorkspace() Implements MemoryWorkspace.destroyWorkspace
        End Sub

        Public Overridable Sub destroyWorkspace(ByVal extended As Boolean) Implements MemoryWorkspace.destroyWorkspace
        End Sub

        Public Overridable Sub toggleWorkspaceUse(ByVal isEnabled As Boolean) Implements MemoryWorkspace.toggleWorkspaceUse
        End Sub

        Public Overridable ReadOnly Property ThisCycleAllocations As Long Implements MemoryWorkspace.ThisCycleAllocations
            Get
                Return 0
            End Get
        End Property

        Public Overridable Sub enableDebug(ByVal reallyEnable As Boolean) Implements MemoryWorkspace.enableDebug
        End Sub

        Public Overridable ReadOnly Property LastCycleAllocations As Long Implements MemoryWorkspace.LastCycleAllocations
            Get
                Return 0
            End Get
        End Property

        Public Overridable ReadOnly Property MaxCycleAllocations As Long Implements MemoryWorkspace.MaxCycleAllocations
            Get
                Return 0
            End Get
        End Property

        Public Overridable ReadOnly Property CurrentSize As Long Implements MemoryWorkspace.CurrentSize
            Get
                Return 0
            End Get
        End Property

        ''' <summary>Restores the workspace that was current when this workspace was entered.</summary>
        Public Overridable Sub close() Implements MemoryWorkspace.close
            Dim manager As MemoryManager = currentMemoryManager()
            If manager IsNot Nothing Then
                manager.CurrentWorkspace = parentWorkspaceField
            End If
        End Sub

        ''' <summary>Same as <see cref="close"/> (Java <c>AutoCloseable</c>; VB <c>Using</c> blocks).</summary>
        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            close()
        End Sub

        ''' <summary>
        ''' This method returns parent Workspace, if any. Like Java, always Nothing for the dummy workspace.
        ''' </summary>
        Public Overridable ReadOnly Property ParentWorkspace As MemoryWorkspace Implements MemoryWorkspace.ParentWorkspace
            Get
                Return Nothing
            End Get
        End Property

        Public Overridable Function tagOutOfScopeUse() As MemoryWorkspace Implements MemoryWorkspace.tagOutOfScopeUse
            Return Me
        End Function

        Public Overridable WriteOnly Property PreviousWorkspace As MemoryWorkspace Implements MemoryWorkspace.PreviousWorkspace
            Set(ByVal value As MemoryWorkspace)
                parentWorkspaceField = value
            End Set
        End Property

        Public Overridable ReadOnly Property CurrentOffset As Long Implements MemoryWorkspace.CurrentOffset
            Get
                Return 0
            End Get
        End Property

        Public Overridable ReadOnly Property UniqueId As String Implements Deallocatable.UniqueId
            Get
                Return Guid.NewGuid().ToString()
            End Get
        End Property

        Public Overridable Function deallocator() As Deallocator Implements Deallocatable.deallocator
            Return New NoOpDeallocator()
        End Function

        Private Class NoOpDeallocator
            Implements Deallocator

            Public Sub deallocate() Implements Deallocator.deallocate
                ' no-op
            End Sub
        End Class

        Public Overridable Function targetDevice() As Integer Implements Deallocatable.targetDevice
            Return 0
        End Function

        Public Overridable ReadOnly Property PrimaryOffset As Long Implements MemoryWorkspace.PrimaryOffset
            Get
                Return 0
            End Get
        End Property

    End Class

End Namespace
