Imports System

' DeepLearning4VB shim for stream-lib's com.clearspring.analytics.stream.cardinality package
' (see HyperLogLogPlus.vb).

Namespace com.clearspring.analytics.stream.cardinality

    ''' <summary>Thrown when cardinality estimators that cannot be merged (different precision) are merged.</summary>
    <Serializable>
    Public Class CardinalityMergeException
        Inherits Exception

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

    End Class

End Namespace
