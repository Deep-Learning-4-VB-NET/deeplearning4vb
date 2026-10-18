Imports System

Namespace org.nd4vb.linalg.api.memory.stash

    ''' <summary>Creates and finds <see cref="Stash(Of T)"/> instances (Java <c>StashManager</c>).</summary>
    Public Interface StashManager

        Function checkIfStashExists(Of T)(ByVal stashId As T) As Boolean

        Function getStash(Of T)(ByVal stashId As T) As Stash(Of T)

        Function createStashIfNotExists(Of T)(ByVal stashId As T) As Stash(Of T)

    End Interface

End Namespace
