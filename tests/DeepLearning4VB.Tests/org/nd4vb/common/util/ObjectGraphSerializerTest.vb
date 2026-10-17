Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Numerics
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ObjectGraphSerializer = DeepLearning4VB.org.nd4vb.common.util.ObjectGraphSerializer
Imports SerializationUtils = DeepLearning4VB.org.nd4vb.common.util.SerializationUtils

Namespace org.nd4vb.common.util

    ''' <summary>
    ''' Tests for the binary object-graph serializer behind SerializationUtils (the BinaryFormatter replacement
    ''' that works on both .NET Framework 4.8.1 and .NET 10).
    ''' </summary>
    <TestClass>
    Public Class ObjectGraphSerializerTest

        <Serializable>
        Public Class Node
            Public Name As String
            Public NextNode As Node
            Public Other As Node
        End Class

        <Serializable>
        Public Class Callbacks
            Implements IDeserializationCallback

            Public Value As Integer
            <NonSerialized> Public Cache As String = "transient"
            <NonSerialized> Public OnDeserializedCalled As Boolean
            <NonSerialized> Public CallbackCalled As Boolean
            Public ReadOnly Fixed As Integer
            Public Handler As EventHandler

            Public Sub New(ByVal fixed As Integer)
                Me.Fixed = fixed
            End Sub

            <OnDeserialized>
            Private Sub AfterRead(ByVal ctx As StreamingContext)
                OnDeserializedCalled = True
                Cache = "rebuilt " & Value
            End Sub

            Public Sub OnDeserialization(ByVal sender As Object) Implements IDeserializationCallback.OnDeserialization
                CallbackCalled = True
            End Sub
        End Class

        <Serializable>
        Public Class ReverseComparer
            Implements IComparer(Of String)

            Public Function Compare(ByVal x As String, ByVal y As String) As Integer Implements IComparer(Of String).Compare
                Return String.CompareOrdinal(y, x)
            End Function
        End Class

        Public Enum Colour
            Red = 1
            Green = 2
            Blue = 4
        End Enum

        <Serializable>
        Public Class Mixed
            Public Colour As Colour
            Public MaybeInt As Integer?
            Public MaybeNot As Integer?
            Public Pair As KeyValuePair(Of String, Integer)
            Public Tuple As Tuple(Of Integer, String)
            Public Moment As Date
            Public Span As TimeSpan
            Public Id As Guid
            Public Money As Decimal
            Public Big As BigInteger
            Public Offset As DateTimeOffset
        End Class

        <Serializable>
        Public Class GoodType
            Public Payload As Integer
        End Class

        Public Class EvilType
            Public Payload As Integer
        End Class

        Public Class NotMarked
            Public Payload As Integer
        End Class

        <Serializable>
        Public Class HoldsAnything
            Public Value As Object
        End Class

        Private Shared Function RoundTrip(Of T)(ByVal value As T) As T
            Return SerializationUtils.clone(value)
        End Function

        <TestMethod>
        Public Sub Primitives_And_Strings_RoundTrip()
            Dim values As Object() = {True, CByte(200), CSByte(-5), CShort(-300), CUShort(60000), -123456, CUInt(4000000000UI), Long.MinValue, ULong.MaxValue, 1.5F, Math.PI, 12.345D, "x"c, "", "héllo wörld", ChrW(&HD83D) & ChrW(&HDE00), ChrW(&HD800), Nothing, Double.NaN, Double.NegativeInfinity}
            Dim back As Object() = RoundTrip(values)
            assertEquals(values.Length, back.Length)
            For i As Integer = 0 To values.Length - 1
                If values(i) Is Nothing Then
                    assertNull(back(i))
                Else
                    assertEquals(values(i).GetType(), back(i).GetType())
                    assertTrue(values(i).Equals(back(i)), "element " & i)
                End If
            Next
        End Sub

        <TestMethod>
        Public Sub Identity_And_Cycles_ArePreserved()
            Dim a As New Node With {.Name = "a"}
            Dim b As New Node With {.Name = "b"}
            a.NextNode = b
            b.NextNode = a
            a.Other = b
            Dim back As Node = RoundTrip(a)
            assertEquals("a", back.Name)
            assertEquals("b", back.NextNode.Name)
            assertTrue(back.NextNode.NextNode Is back, "cycle")
            assertTrue(back.Other Is back.NextNode, "shared reference")
            Dim self As New Node With {.Name = "self"}
            self.NextNode = self
            Dim selfBack As Node = RoundTrip(self)
            assertTrue(selfBack.NextNode Is selfBack)
        End Sub

        <TestMethod>
        Public Sub Collections_KeepContentOrderAndComparers()
            Dim list As New List(Of String) From {"b", "a", "c"}
            Dim dict As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase) From {{"One", 1}, {"two", 2}}
            Dim sorted As New SortedDictionary(Of String, Integer)(New ReverseComparer()) From {{"a", 1}, {"c", 3}, {"b", 2}}
            Dim hs As New HashSet(Of Integer) From {3, 1, 2}
            Dim ll As New LinkedList(Of String)(New String() {"x", "y", "z"})
            Dim q As New Queue(Of Integer)(New Integer() {1, 2, 3})
            Dim st As New Stack(Of Integer)(New Integer() {1, 2, 3})
            Dim al As New ArrayList From {1, "two", 3.0}
            Dim ht As New Hashtable From {{"k", "v"}}
            Dim graph As Object() = {list, dict, sorted, hs, ll, q, st, al, ht}
            Dim back As Object() = RoundTrip(graph)

            assertEquals(list, back(0))
            Dim dictBack = DirectCast(back(1), Dictionary(Of String, Integer))
            assertEquals(1, dictBack("ONE")) ' the case-insensitive comparer survived
            Dim sortedBack = DirectCast(back(2), SortedDictionary(Of String, Integer))
            assertEquals(New List(Of String) From {"c", "b", "a"}, New List(Of String)(sortedBack.Keys))
            assertTrue(DirectCast(back(3), HashSet(Of Integer)).SetEquals(hs))
            assertEquals(New List(Of String) From {"x", "y", "z"}, New List(Of String)(DirectCast(back(4), LinkedList(Of String))))
            assertEquals(1, DirectCast(back(5), Queue(Of Integer)).Dequeue())
            assertEquals(3, DirectCast(back(6), Stack(Of Integer)).Pop())
            assertEquals("two", DirectCast(back(7), ArrayList)(1))
            assertEquals("v", DirectCast(back(8), Hashtable)("k"))
        End Sub

        <TestMethod>
        Public Sub Arrays_JaggedMultiDimensionalAndPrimitive()
            Dim jagged As Integer()() = {New Integer() {1, 2}, New Integer() {}, New Integer() {3}}
            Dim grid(,) As Integer = {{1, 2, 3}, {4, 5, 6}}
            Dim bytes As Byte() = {0, 1, 255}
            Dim chars As Char() = "abc".ToCharArray()
            Dim colours As Colour() = {Colour.Blue, Colour.Red}
            Dim back As Object() = RoundTrip(New Object() {jagged, grid, bytes, chars, colours})
            assertArrayEquals(jagged, DirectCast(back(0), Integer()()))
            Dim g = DirectCast(back(1), Integer(,))
            assertEquals(2, g.GetLength(0))
            assertEquals(3, g.GetLength(1))
            assertEquals(6, g(1, 2))
            assertEquals(2, g(0, 1))
            assertArrayEquals(bytes, DirectCast(back(2), Byte()))
            assertArrayEquals(chars, DirectCast(back(3), Char()))
            assertArrayEquals(colours, DirectCast(back(4), Colour()))
        End Sub

        <TestMethod>
        Public Sub ValueTypes_Enums_And_Nullables()
            Dim m As New Mixed With {
                .Colour = Colour.Green, .MaybeInt = 7, .MaybeNot = Nothing,
                .Pair = New KeyValuePair(Of String, Integer)("k", 9), .Tuple = Tuple.Create(1, "one"),
                .Moment = New Date(2016, 1, 1, 12, 30, 45, DateTimeKind.Utc), .Span = TimeSpan.FromMinutes(90),
                .Id = Guid.NewGuid(), .Money = 1234.5678D, .Big = BigInteger.Pow(2, 100),
                .Offset = New DateTimeOffset(2020, 2, 29, 23, 59, 0, TimeSpan.FromHours(5.5))}
            Dim back As Mixed = RoundTrip(m)
            assertEquals(Colour.Green, back.Colour)
            assertEquals(7, back.MaybeInt.Value)
            assertFalse(back.MaybeNot.HasValue)
            assertEquals("k", back.Pair.Key)
            assertEquals(9, back.Pair.Value)
            assertEquals(Tuple.Create(1, "one"), back.Tuple)
            assertEquals(m.Moment, back.Moment)
            assertEquals(DateTimeKind.Utc, back.Moment.Kind)
            assertEquals(m.Span, back.Span)
            assertEquals(m.Id, back.Id)
            assertEquals(m.Money, back.Money)
            assertEquals(m.Big, back.Big)
            assertEquals(m.Offset, back.Offset)
            assertEquals(m.Offset.Offset, back.Offset.Offset)
        End Sub

        <TestMethod>
        Public Sub FrameworkTypes_AreWrittenByValue()
            Dim abs As New Uri("https://example.org/a%20b?x=1")
            Dim rel As New Uri("dir/file.txt", UriKind.Relative)
            Dim de As CultureInfo = CultureInfo.GetCultureInfo("de-DE")
            Dim utf8 As New UTF8Encoding(False)
            Dim re As New Regex("^a+b$", RegexOptions.IgnoreCase Or RegexOptions.Multiline)
            Dim tz As TimeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Test+0530", TimeSpan.FromMinutes(330), "Test", "Test")
            Dim back As Object() = RoundTrip(New Object() {abs, rel, de, utf8, re, TimeZoneInfo.Utc, tz, GetType(Dictionary(Of String, Integer()))})
            assertEquals(abs, back(0))
            assertEquals(rel, back(1))
            assertFalse(DirectCast(back(1), Uri).IsAbsoluteUri)
            assertEquals("de-DE", DirectCast(back(2), CultureInfo).Name)
            assertEquals(0, DirectCast(back(3), Encoding).GetPreamble().Length)
            Dim reBack = DirectCast(back(4), Regex)
            assertTrue(reBack.IsMatch("AAB"))
            assertEquals(re.Options, reBack.Options)
            assertTrue(DirectCast(back(5), TimeZoneInfo) Is TimeZoneInfo.Utc)
            assertEquals(TimeSpan.FromMinutes(330), DirectCast(back(6), TimeZoneInfo).BaseUtcOffset)
            assertEquals(GetType(Dictionary(Of String, Integer())), back(7))
        End Sub

        <TestMethod>
        Public Sub SeededRandom_ContinuesTheSameSequence()
            Dim rnd As New Random(12345)
            rnd.Next()
            Dim copy As Random = RoundTrip(rnd)
            For i As Integer = 1 To 5
                assertEquals(rnd.Next(), copy.Next())
            Next
        End Sub

        <TestMethod>
        Public Sub NonSerializedFields_Callbacks_ReadOnlyFields_And_Delegates()
            Dim c As New Callbacks(42) With {.Value = 7}
            c.Handler = New EventHandler(Sub(s, e)
                                         End Sub)
            Dim back As Callbacks = RoundTrip(c)
            assertEquals(7, back.Value)
            assertEquals(42, back.Fixed)
            assertTrue(back.OnDeserializedCalled)
            assertTrue(back.CallbackCalled)
            assertEquals("rebuilt 7", back.Cache)
            assertNull(back.Handler) ' delegate fields are not written
        End Sub

        <TestMethod>
        Public Sub NonSerializableTypes_AreRefused()
            Dim ex = assertThrows(Of InvalidOperationException)(Sub() SerializationUtils.toByteArray(New NotMarked()))
            assertTrue(TypeOf ex.InnerException Is SerializationException, ex.ToString())
            assertThrows(Of InvalidOperationException)(Sub() SerializationUtils.toByteArray(New HoldsAnything With {.Value = New NotMarked()}))
            assertThrows(Of InvalidOperationException)(Sub() SerializationUtils.toByteArray(New HoldsAnything With {.Value = New Action(Sub()
                                                                                                                                              End Sub)}))
        End Sub

        <TestMethod>
        Public Sub Deserialization_RefusesTypesThatAreNotSerializable()
            ' Write a GoodType, then rename it in the stream to EvilType (same length, same fields, not [Serializable]).
            Dim bytes As Byte() = SerializationUtils.toByteArray(New GoodType With {.Payload = 1})
            Dim good As Byte() = Encoding.Unicode.GetBytes("GoodType")
            Dim evil As Byte() = Encoding.Unicode.GetBytes("EvilType")
            Dim pos As Integer = IndexOf(bytes, good)
            assertTrue(pos > 0, "type name not found in the stream")
            Array.Copy(evil, 0, bytes, pos, evil.Length)
            Dim ex = assertThrows(Of InvalidOperationException)(Sub() SerializationUtils.fromByteArray(Of Object)(bytes))
            assertTrue(ex.InnerException.Message.Contains("not marked as serializable"), ex.InnerException.Message)
        End Sub

        <TestMethod>
        Public Sub ForeignOrCorruptData_IsRejected()
            assertThrows(Of InvalidOperationException)(Sub() SerializationUtils.fromByteArray(Of Object)(New Byte() {1, 2, 3, 4, 5, 6, 7, 8, 9}))
            assertThrows(Of InvalidOperationException)(Sub() SerializationUtils.fromByteArray(Of Object)(New Byte() {}))
            Dim bytes As Byte() = SerializationUtils.toByteArray("hello")
            bytes(8) = 99 ' unsupported format version
            assertThrows(Of InvalidOperationException)(Sub() SerializationUtils.fromByteArray(Of Object)(bytes))
        End Sub

        <TestMethod>
        Public Sub FileAndStreamApis()
            Dim dir As String = Path.Combine(Path.GetTempPath(), "dl4vb-tests", Guid.NewGuid().ToString("N"))
            Try
                Dim file As String = Path.Combine(dir, "sub", "graph.bin")
                SerializationUtils.saveObject(New List(Of Integer) From {1, 2, 3}, file)
                assertEquals(New List(Of Integer) From {1, 2, 3}, SerializationUtils.readObject(Of List(Of Integer))(file))
                Using ms As New MemoryStream()
                    SerializationUtils.writeObject("first", ms)
                    SerializationUtils.writeObject(2, ms)
                    ms.Position = 0
                    assertEquals("first", SerializationUtils.readObject(Of String)(ms))
                    assertEquals(2, SerializationUtils.deserialize(Of Integer)(ms)) ' streams are left open and positioned after each graph
                End Using
            Finally
                If Directory.Exists(dir) Then Directory.Delete(dir, True)
            End Try
        End Sub

        Private Shared Function IndexOf(ByVal haystack As Byte(), ByVal needle As Byte()) As Integer
            For i As Integer = 0 To haystack.Length - needle.Length
                Dim match As Boolean = True
                For j As Integer = 0 To needle.Length - 1
                    If haystack(i + j) <> needle(j) Then
                        match = False
                        Exit For
                    End If
                Next
                If match Then Return i
            Next
            Return -1
        End Function

    End Class

End Namespace
