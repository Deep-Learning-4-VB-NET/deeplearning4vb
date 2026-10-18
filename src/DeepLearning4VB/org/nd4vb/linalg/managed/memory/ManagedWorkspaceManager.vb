' DeepLearning4VB: the managed backend's workspace manager (phase 2, see docs/PHASE2_DESIGN.md).
' Structural model: nd4j-native org.nd4j.linalg.cpu.nativecpu.workspace.CpuWorkspaceManager.

Imports System
Imports DebugModeT = DeepLearning4VB.org.nd4vb.linalg.api.memory.enums.DebugMode
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory
Imports DeepLearning4VB.org.nd4vb.linalg.api.memory.enums
Imports BasicWorkspaceManager = DeepLearning4VB.org.nd4vb.linalg.api.memory.provider.BasicWorkspaceManager
Imports DeallocatorService = DeepLearning4VB.org.nd4vb.linalg.api.memory.deallocation.DeallocatorService
Imports DummyWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.abstracts.DummyWorkspace
Imports WorkspaceConfiguration = DeepLearning4VB.org.nd4vb.linalg.api.memory.conf.WorkspaceConfiguration
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

Namespace org.nd4vb.linalg.managed.memory

    ''' <summary>
    ''' The workspace manager of the managed backend (Java <c>CpuWorkspaceManager</c>): creates <see cref="ManagedWorkspace"/>s
    ''' (or <see cref="DummyWorkspace"/>s in the BYPASS_EVERYTHING debug mode), keeps them per thread and registers them
    ''' with the deallocator service.
    ''' </summary>
    ''' <remarks>
    ''' Difference from Java: the memory manager and the deallocator service are the ones given to the constructor; the
    ''' parameterless constructor uses <c>Nd4vb.MemoryManager</c> and <c>Nd4vb.DeallocatorService</c> (Java:
    ''' <c>Nd4j.getMemoryManager()</c> and <c>Nd4j.getDeallocatorService()</c>), looked up when first needed. The debug mode
    ''' checked is this manager's own (Java read <c>Nd4j.getWorkspaceManager().getDebugMode()</c>, the same object).
    ''' </remarks>
    Public Class ManagedWorkspaceManager
        Inherits BasicWorkspaceManager

        Private ReadOnly deallocatorServiceField As DeallocatorService
        Private ReadOnly useNd4vbDeallocator As Boolean

        ''' <summary>A manager that uses <c>Nd4vb.MemoryManager</c> and <c>Nd4vb.DeallocatorService</c>.</summary>
        Public Sub New()
            MyBase.New()
            useNd4vbDeallocator = True
        End Sub

        ''' <summary>
        ''' A manager that uses <paramref name="memoryManager"/> for the current workspace and registers workspaces with
        ''' <paramref name="deallocatorService"/> (Nothing: no registration).
        ''' </summary>
        Public Sub New(ByVal memoryManager As MemoryManager, ByVal deallocatorService As DeallocatorService)
            MyBase.New(javaDefaultConfiguration(), memoryManager)
            If memoryManager Is Nothing Then
                Throw New ArgumentNullException(NameOf(memoryManager))
            End If
            Me.deallocatorServiceField = deallocatorService
        End Sub

        ''' <summary>A new workspace with the default id, or a dummy workspace in the BYPASS_EVERYTHING debug mode.</summary>
        Protected Friend Overridable Function newWorkspace(ByVal configuration As WorkspaceConfiguration) As MemoryWorkspace
            Return newWorkspace(configuration, MemoryWorkspace.Constants.DEFAULT_ID)
        End Function

        ''' <summary>A new workspace with the given id, or a dummy workspace in the BYPASS_EVERYTHING debug mode.</summary>
        Protected Friend Overridable Function newWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace
            Return newWorkspace(configuration, id, 0)
        End Function

        ''' <summary>A new workspace with the given id on the given device, or a dummy workspace in the BYPASS_EVERYTHING debug mode.</summary>
        Protected Friend Overridable Function newWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String, ByVal deviceId As Integer) As MemoryWorkspace
            If DebugMode = DebugModeT.BYPASS_EVERYTHING Then
                Return New DummyWorkspace(currentMemoryManager())
            End If
            Return New ManagedWorkspace(configuration, id, deviceId, Me, currentMemoryManager())
        End Function

        Private Function register(ByVal id As String, ByVal workspace As MemoryWorkspace) As MemoryWorkspace
            ensureThreadExistense()
            backingMap.Value(id) = workspace
            If DebugMode <> DebugModeT.BYPASS_EVERYTHING Then
                pickReference(workspace)
            End If
            Return workspace
        End Function

        ''' <inheritdoc/>
        Public Overrides Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration) As MemoryWorkspace
            If configuration Is Nothing Then
                Throw New ArgumentNullException(NameOf(configuration))
            End If
            Dim workspace As MemoryWorkspace = newWorkspace(configuration)
            Return register(If(workspace.Id, MemoryWorkspace.Constants.DEFAULT_ID), workspace)
        End Function

        ''' <inheritdoc/>
        Public Overrides Function createNewWorkspace() As MemoryWorkspace
            Dim workspace As MemoryWorkspace = newWorkspace(defaultConfiguration)
            Return register(If(workspace.Id, MemoryWorkspace.Constants.DEFAULT_ID), workspace)
        End Function

        ''' <inheritdoc/>
        Public Overrides Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace
            If configuration Is Nothing Then
                Throw New ArgumentNullException(NameOf(configuration))
            End If
            If id Is Nothing Then
                Throw New ArgumentNullException(NameOf(id))
            End If
            Return register(id, newWorkspace(configuration, id))
        End Function

        ''' <inheritdoc/>
        Public Overrides Function createNewWorkspace(ByVal configuration As WorkspaceConfiguration, ByVal id As String, ByVal deviceId As Integer?) As MemoryWorkspace
            If configuration Is Nothing Then
                Throw New ArgumentNullException(NameOf(configuration))
            End If
            If id Is Nothing Then
                Throw New ArgumentNullException(NameOf(id))
            End If
            Return register(id, newWorkspace(configuration, id, deviceId.GetValueOrDefault()))
        End Function

        ''' <summary>The workspace with the given id for the current thread, created with <paramref name="configuration"/> on first use.</summary>
        Public Overrides Function getWorkspaceForCurrentThread(ByVal configuration As WorkspaceConfiguration, ByVal id As String) As MemoryWorkspace
            If configuration Is Nothing Then
                Throw New ArgumentNullException(NameOf(configuration))
            End If
            If id Is Nothing Then
                Throw New ArgumentNullException(NameOf(id))
            End If
            ensureThreadExistense()

            Dim workspace As MemoryWorkspace = Nothing
            If Not backingMap.Value.TryGetValue(id, workspace) OrElse workspace Is Nothing Then
                workspace = register(id, newWorkspace(configuration, id))
            End If

            Return workspace
        End Function

        ''' <summary>Registers the workspace with the deallocator service.</summary>
        Protected Friend Overrides Sub pickReference(ByVal w As MemoryWorkspace)
            Dim service As DeallocatorService = If(useNd4vbDeallocator, Nd4vbT.DeallocatorService, deallocatorServiceField)
            If service IsNot Nothing Then
                service.pickObject(w)
            End If
        End Sub

    End Class

End Namespace
