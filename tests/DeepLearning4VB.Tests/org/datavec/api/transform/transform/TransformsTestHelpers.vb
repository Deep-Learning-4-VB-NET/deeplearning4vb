Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.transform.transforms

    ''' <summary>Shared helpers of the transform implementation tests.</summary>
    Friend Module TransformsTestHelpers

        ''' <summary>Java <c>Arrays.&lt;Writable&gt;asList(...)</c>.</summary>
        Public Function row(ByVal ParamArray ws() As WritableT) As IList(Of WritableT)
            Return New List(Of WritableT)(ws)
        End Function

        ''' <summary>A sequence of records.</summary>
        Public Function seqOf(ByVal ParamArray rows() As IList(Of WritableT)) As IList(Of IList(Of WritableT))
            Return New List(Of IList(Of WritableT))(rows)
        End Function

        ''' <summary>A list of objects (the input of <c>map(Object)</c> / <c>mapSequence(Object)</c>).</summary>
        Public Function objList(ByVal ParamArray values() As Object) As IList(Of Object)
            Return New List(Of Object)(values)
        End Function

        ''' <summary>Java serialization round trip (ObjectOutputStream/ObjectInputStream): the BinaryFormatter.</summary>
        Public Function serializeRoundTrip(Of T)(ByVal obj As T) As T
            Dim bf As New BinaryFormatter()
            Using ms As New MemoryStream()
                bf.Serialize(ms, obj)
                ms.Position = 0
                Return DirectCast(bf.Deserialize(ms), T)
            End Using
        End Function

    End Module

End Namespace
