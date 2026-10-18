Imports System
Imports MemoryWorkspace = DeepLearning4VB.org.nd4vb.linalg.api.memory.MemoryWorkspace

Namespace org.nd4vb.linalg.workspace

    ''' <summary>
    ''' A simple utility class for closing several workspaces at once: <see cref="close"/> (and <c>Dispose</c>, so it
    ''' can be used in a <c>Using</c> block; Java <c>AutoCloseable</c>) closes them in reverse order.
    ''' Lombok's <c>@Data</c> members are written out.
    ''' </summary>
    Public Class WorkspacesCloseable
        Implements IDisposable

        Private workspaces() As MemoryWorkspace

        Public Sub New(ParamArray ByVal workspaces() As MemoryWorkspace)
            If workspaces Is Nothing Then Throw New ArgumentNullException(NameOf(workspaces), "workspaces is marked non-null but is null")
            Me.workspaces = workspaces
        End Sub

        Public Overridable Function getWorkspaces() As MemoryWorkspace()
            Return workspaces
        End Function

        Public Overridable Sub setWorkspaces(ByVal workspaces() As MemoryWorkspace)
            Me.workspaces = workspaces
        End Sub

        Public Overridable Sub close()
            For i As Integer = workspaces.Length - 1 To 0 Step -1
                workspaces(i).close()
            Next i
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            close()
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As WorkspacesCloseable = TryCast(obj, WorkspacesCloseable)
            If o Is Nothing Then Return False
            Return Arrays.equals(workspaces, o.workspaces)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return 59 + Arrays.hashCode(workspaces)
        End Function

        Public Overrides Function ToString() As String
            Return "WorkspacesCloseable(workspaces=" & Arrays.toString(workspaces) & ")"
        End Function

    End Class

End Namespace
