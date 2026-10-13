Imports System
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.io.serializers
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Configured = DeepLearning4VB.org.datavec.api.conf.Configured
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text

Namespace org.datavec.api.io.serializers

    ''' <summary>
    ''' <see cref="SerializationFactory"/>: serializations are registered by class name through the
    ''' <c>io.serializations</c> property and looked up by the class they accept.
    ''' </summary>
    <TestClass>
    Public Class SerializationFactoryTest

        <TestMethod>
        Public Sub testFactoryWithoutConfiguredSerializations()
            'the Java default (Hadoop's WritableSerialization) does not exist, so nothing is registered
            Dim conf As New Configuration(False)
            Dim factory As New SerializationFactory(conf)
            assertTrue(TypeOf factory Is Configured)
            assertTrue(factory.getConf() Is conf)
            assertNull(factory.getSerialization(Of Text)(GetType(Text)))
            assertNull(factory.getSerializer(Of Text)(GetType(Text)))
            assertNull(factory.getDeserializer(Of Text)(GetType(Text)))
        End Sub

        <TestMethod>
        Public Sub testSerializationFoundByClassName()
            Dim conf As New Configuration(False)
            conf.setStrings("io.serializations", GetType(TextSerialization).FullName)
            Dim factory As New SerializationFactory(conf)

            Dim serialization As Serialization(Of Text) = factory.getSerialization(Of Text)(GetType(Text))
            assertNotNull(serialization)
            assertTrue(TypeOf serialization Is TextSerialization)
            'the serialization was created through ReflectionUtils, so it is configured with the factory's configuration
            assertTrue(DirectCast(serialization, TextSerialization).getConf() Is conf)
            'classes the serialization does not accept are not served
            assertNull(factory.getSerialization(Of String)(GetType(String)))
            assertNull(factory.getSerializer(Of String)(GetType(String)))
            'the serialization accepts subclasses of Text as well
            assertNotNull(factory.getSerialization(Of Text)(GetType(SubText)))
            'a serialization that accepts the class but is typed differently is skipped
            assertNull(factory.getSerialization(Of String)(GetType(Text)))
        End Sub

        <TestMethod>
        Public Sub testSerializeAndDeserializeRoundTrip()
            Dim conf As New Configuration(False)
            conf.setStrings("io.serializations", GetType(TextSerialization).FullName)
            Dim factory As New SerializationFactory(conf)

            Dim bytes() As Byte
            Using ms As New MemoryStream()
                Dim serializer As Serializer(Of Text) = factory.getSerializer(Of Text)(GetType(Text))
                assertNotNull(serializer)
                serializer.open(ms)
                serializer.serialize(New Text("hello"))
                serializer.serialize(New Text("wörld"))
                serializer.close()
                bytes = ms.ToArray()
            End Using
            assertTrue(bytes.Length > 0)

            Using ms As New MemoryStream(bytes)
                Dim deserializer As Deserializer(Of Text) = factory.getDeserializer(Of Text)(GetType(Text))
                assertNotNull(deserializer)
                deserializer.open(ms)
                'with no instance to reuse a new one is created
                Dim first As Text = deserializer.deserialize(Nothing)
                assertEquals(New Text("hello"), first)
                'an existing instance is filled in and returned
                Dim reused As New Text("old")
                Dim second As Text = deserializer.deserialize(reused)
                assertTrue(second Is reused)
                assertEquals(New Text("wörld"), second)
                deserializer.close()
            End Using
        End Sub

        <TestMethod>
        Public Sub testUnknownAndMultipleSerializationNames()
            Dim conf As New Configuration(False)
            'an unresolvable class name is skipped with a warning; the others are still registered
            conf.setStrings("io.serializations", "no.such.Serialization", GetType(StringSerialization).FullName, GetType(TextSerialization).FullName)
            Dim factory As New SerializationFactory(conf)
            assertTrue(TypeOf factory.getSerialization(Of String)(GetType(String)) Is StringSerialization)
            assertTrue(TypeOf factory.getSerialization(Of Text)(GetType(Text)) Is TextSerialization)
            assertNull(factory.getSerialization(Of Integer)(GetType(Integer)))

            'the first serialization accepting the class wins
            conf.setStrings("io.serializations", GetType(TextSerialization).FullName, GetType(OtherTextSerialization).FullName)
            assertTrue(TypeOf New SerializationFactory(conf).getSerialization(Of Text)(GetType(Text)) Is TextSerialization)
            conf.setStrings("io.serializations", GetType(OtherTextSerialization).FullName, GetType(TextSerialization).FullName)
            assertTrue(TypeOf New SerializationFactory(conf).getSerialization(Of Text)(GetType(Text)) Is OtherTextSerialization)

            'a class that is not a Serialization is registered but never matches; one without a
            'parameterless constructor cannot be created and is skipped
            conf.setStrings("io.serializations", GetType(Object).FullName, GetType(String).FullName)
            assertNull(New SerializationFactory(conf).getSerialization(Of Text)(GetType(Text)))
            assertNull(New SerializationFactory(conf).getSerialization(Of Object)(GetType(Object)))
        End Sub

        Public Class SubText
            Inherits Text

            Public Sub New()
            End Sub

        End Class

    End Class

    ''' <summary>A length-prefixed UTF-8 serialization of <see cref="Text"/>, configured by the factory.</summary>
    Public Class TextSerialization
        Inherits Configured
        Implements Serialization(Of Text)

        Public Sub New()
        End Sub

        Public Overridable Function accept(ByVal c As Type) As Boolean Implements Serialization(Of Text).accept
            Return GetType(Text).IsAssignableFrom(c)
        End Function

        Public Function getSerializer(ByVal c As Type) As Serializer(Of Text) Implements Serialization(Of Text).getSerializer
            Return New TextSerializer()
        End Function

        Public Function getDeserializer(ByVal c As Type) As Deserializer(Of Text) Implements Serialization(Of Text).getDeserializer
            Return New TextDeserializer()
        End Function

        Private Class TextSerializer
            Implements Serializer(Of Text)

            Private writer As BinaryWriter

            Public Sub open(ByVal [out] As Stream) Implements Serializer(Of Text).open
                writer = New BinaryWriter([out], Encoding.UTF8, True)
            End Sub

            Public Sub serialize(ByVal t As Text) Implements Serializer(Of Text).serialize
                writer.Write(t.ToString())
            End Sub

            Public Sub close() Implements Serializer(Of Text).close
                writer.Flush()
                writer.Close()
            End Sub

        End Class

        Private Class TextDeserializer
            Implements Deserializer(Of Text)

            Private reader As BinaryReader

            Public Sub open(ByVal [in] As Stream) Implements Deserializer(Of Text).open
                reader = New BinaryReader([in], Encoding.UTF8, True)
            End Sub

            Public Function deserialize(ByVal t As Text) As Text Implements Deserializer(Of Text).deserialize
                Dim s As String = reader.ReadString()
                If t Is Nothing Then Return New Text(s)
                t.[set](s)
                Return t
            End Function

            Public Sub close() Implements Deserializer(Of Text).close
                reader.Close()
            End Sub

        End Class

    End Class

    ''' <summary>A second Text serialization, to check registration order.</summary>
    Public Class OtherTextSerialization
        Inherits TextSerialization

        Public Sub New()
        End Sub

    End Class

    ''' <summary>Accepts strings only; the stream methods are not needed by the tests.</summary>
    Public Class StringSerialization
        Implements Serialization(Of String)

        Public Sub New()
        End Sub

        Public Function accept(ByVal c As Type) As Boolean Implements Serialization(Of String).accept
            Return c Is GetType(String)
        End Function

        Public Function getSerializer(ByVal c As Type) As Serializer(Of String) Implements Serialization(Of String).getSerializer
            Throw New NotSupportedException()
        End Function

        Public Function getDeserializer(ByVal c As Type) As Deserializer(Of String) Implements Serialization(Of String).getDeserializer
            Throw New NotSupportedException()
        End Function

    End Class

End Namespace
