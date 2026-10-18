Imports System
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration

Namespace org.nd4vb.linalg.api.memory

    ''' <summary>
    ''' A memory workspace: a scope in which arrays are allocated and reused across iterations.
    ''' <para>
    ''' Differences from the Java interface: Java's <c>AutoCloseable</c> is <see cref="IDisposable"/> (<c>Dispose</c>
    ''' must behave like <see cref="close"/>); the constant <c>DEFAULT_ID</c> is
    ''' <see cref="MemoryWorkspace.Constants.DEFAULT_ID"/>; the native-only <c>alloc(...)</c> overloads, which returned
    ''' JavaCPP <c>PagedPointer</c>s into off-heap memory, are deleted (the managed backend allocates from the
    ''' garbage-collected heap and its workspaces only track scope and statistics, see docs/PHASE2_DESIGN.md).
    ''' </para>
    ''' </summary>
    Public Interface MemoryWorkspace
        Inherits IDisposable, Deallocatable

        ''' <summary>
        ''' The constants of the Java interface (VB interfaces cannot hold fields): Java
        ''' <c>MemoryWorkspace.DEFAULT_ID</c> is <c>MemoryWorkspace.Constants.DEFAULT_ID</c>.
        ''' </summary>
        NotInheritable Class Constants
            Public Const DEFAULT_ID As String = "DefaultWorkspace"

            Private Sub New()
            End Sub
        End Class

        Enum Type
            ''' <summary>
            ''' This mode means you have dummy workspace here. It doesn't provide any functionality.
            ''' </summary>
            DUMMY

            ''' <summary>
            ''' Most regular workspace. It starts somewhere, and ends somewhere. It has limits, aka scope of use.
            ''' </summary>
            SCOPED

            ''' <summary>
            ''' Special workspace mode: circular buffer. Workspace is never closed, and gets reset only once end reached.
            ''' </summary>
            CIRCULAR
        End Enum

        ''' <summary>
        ''' This method returns WorkspaceConfiguration bean that was used for given Workspace instance
        ''' </summary>
        ReadOnly Property WorkspaceConfiguration As WorkspaceConfiguration

        ''' <summary>
        ''' This method returns Type of this workspace
        ''' </summary>
        ReadOnly Property WorkspaceType As Type

        ''' <summary>
        ''' This method returns Id of this workspace
        ''' </summary>
        ReadOnly Property Id As String

        ''' <summary>
        ''' Returns deviceId for this workspace
        ''' </summary>
        ReadOnly Property DeviceId As Integer

        ''' <summary>
        ''' This method returns threadId where this workspace was created
        ''' </summary>
        ReadOnly Property ThreadId As Long?

        ''' <summary>
        ''' This method returns current generation Id
        ''' </summary>
        ReadOnly Property GenerationId As Long

        ''' <summary>
        ''' The offset of the primary (host) allocation within the workspace.
        ''' </summary>
        ReadOnly Property PrimaryOffset As Long

        ''' <summary>
        ''' This method notifies given Workspace that new use cycle is starting now
        ''' </summary>
        Function notifyScopeEntered() As MemoryWorkspace

        ''' <summary>
        ''' This method TEMPORARY enters this workspace, without reset applied
        ''' </summary>
        Function notifyScopeBorrowed() As MemoryWorkspace

        ''' <summary>
        ''' This method notifies given Workspace that use cycle just ended
        ''' </summary>
        Function notifyScopeLeft() As MemoryWorkspace

        ''' <summary>
        ''' This method returns True if scope was opened, and not closed yet.
        ''' </summary>
        ReadOnly Property ScopeActive As Boolean

        ''' <summary>
        ''' This method causes Workspace initialization
        '''
        ''' PLEASE NOTE: This call will have no effect on previously initialized Workspace
        ''' </summary>
        Sub initializeWorkspace()

        ''' <summary>
        ''' This method causes Workspace destruction: all memory allocations are released after this call.
        ''' </summary>
        Sub destroyWorkspace()

        Sub destroyWorkspace(ByVal extended As Boolean)

        ''' <summary>
        ''' This method allows you to temporary disable/enable given Workspace use.
        ''' If turned off - direct memory allocations will be used.
        ''' </summary>
        ''' <param name="isEnabled"> </param>
        Sub toggleWorkspaceUse(ByVal isEnabled As Boolean)

        ''' <summary>
        ''' This method returns amount of memory consumed in current cycle, in bytes
        ''' </summary>
        ReadOnly Property ThisCycleAllocations As Long

        ''' <summary>
        ''' This method enabled debugging mode for this workspace
        ''' </summary>
        ''' <param name="reallyEnable"> </param>
        Sub enableDebug(ByVal reallyEnable As Boolean)

        ''' <summary>
        ''' This method returns amount of memory consumed in last successful cycle, in bytes
        ''' </summary>
        ReadOnly Property LastCycleAllocations As Long

        ''' <summary>
        ''' This method returns amount of memory consumed by largest successful cycle, in bytes
        ''' </summary>
        ReadOnly Property MaxCycleAllocations As Long

        ''' <summary>
        ''' This methos returns current allocated size of this workspace
        ''' </summary>
        ReadOnly Property CurrentSize As Long

        ''' <summary>
        ''' This method is for compatibility with "try-with-resources" java blocks (in VB: <c>Using</c>).
        ''' Internally it should be equal to notifyScopeLeft() method
        ''' </summary>
        Sub close()

        ''' <summary>
        ''' This method returns parent Workspace, if any. Null if there's none.
        ''' </summary>
        ReadOnly Property ParentWorkspace As MemoryWorkspace

        ''' <summary>
        ''' This method temporary disables this workspace
        ''' </summary>
        Function tagOutOfScopeUse() As MemoryWorkspace

        ''' <summary>
        ''' Set the previous workspace, if any<br>
        ''' NOTE: this method should only be used if you are fully aware of the consequences of doing so. Incorrect use
        ''' of this method may leave workspace management in an invalid/indeterminant state!
        ''' </summary>
        ''' <param name="memoryWorkspace"> Workspace to set as the previous workspace. This is the workspace that will become active
        '''                        when this workspace is closed. </param>
        WriteOnly Property PreviousWorkspace As MemoryWorkspace

        ''' <summary>
        ''' This mehtod returns current offset within buffer
        ''' </summary>
        ReadOnly Property CurrentOffset As Long

    End Interface

End Namespace
