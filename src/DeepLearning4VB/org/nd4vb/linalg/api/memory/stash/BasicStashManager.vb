Imports System

Namespace org.nd4vb.linalg.api.memory.stash

    ''' <summary>The placeholder stash manager of ND4J: every method returns False or Nothing.</summary>
    Public Class BasicStashManager
        Implements StashManager

        Public Overridable Function checkIfStashExists(Of T)(ByVal stashId As T) As Boolean Implements StashManager.checkIfStashExists
            Return False
        End Function

        Public Overridable Function getStash(Of T)(ByVal stashId As T) As Stash(Of T) Implements StashManager.getStash
            Return Nothing
        End Function

        Public Overridable Function createStashIfNotExists(Of T)(ByVal stashId As T) As Stash(Of T) Implements StashManager.createStashIfNotExists
            Return Nothing
        End Function

    End Class

End Namespace
