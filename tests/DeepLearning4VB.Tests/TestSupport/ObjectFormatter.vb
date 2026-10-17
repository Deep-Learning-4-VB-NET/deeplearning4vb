Imports System.IO
Imports SerializationUtils = DeepLearning4VB.org.nd4vb.common.util.SerializationUtils

''' <summary>
''' Test helper with the shape of the old BinaryFormatter (<c>Serialize</c>/<c>Deserialize</c> on a stream),
''' backed by <see cref="SerializationUtils"/>: the .NET counterpart of Java's ObjectOutputStream and
''' ObjectInputStream on both .NET Framework 4.8.1 and .NET 10.
''' </summary>
Friend NotInheritable Class ObjectFormatter

    Public Sub Serialize(ByVal stream As Stream, ByVal graph As Object)
        SerializationUtils.writeObject(graph, stream)
    End Sub

    Public Function Deserialize(ByVal stream As Stream) As Object
        Return SerializationUtils.readObject(Of Object)(stream)
    End Function

End Class

''' <summary>Serialization checks that work the same on .NET Framework 4.8.1 and .NET 10.</summary>
Friend Module SerializationTestHelpers

    ''' <summary>True when the type is marked <c>[Serializable]</c> (Type.IsSerializable is obsolete on .NET 10).</summary>
    Public Function isSerializableType(ByVal t As Type) As Boolean
#Disable Warning SYSLIB0050 ' the [Serializable] metadata flag is what these tests check
        Return (t.Attributes And System.Reflection.TypeAttributes.Serializable) <> 0
#Enable Warning SYSLIB0050
    End Function

End Module
