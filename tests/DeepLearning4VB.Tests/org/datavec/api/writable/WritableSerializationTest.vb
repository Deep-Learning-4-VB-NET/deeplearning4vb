Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.datavec.api.io
Imports DeepLearning4VB.org.datavec.api.writable
Imports DeepLearning4VB.org.nd4vb.common.holder
Imports DVW = DeepLearning4VB.org.datavec.api.writable

Namespace org.datavec.api.writable

    ''' <summary>
    ''' Binary round trips of every core writable through the Java-compatible data streams, the
    ''' zero-compressed integer encoding of <c>WritableUtils</c>, the reusable buffers, the raw
    ''' (byte-level) comparators and the JSON form of the writables.
    ''' </summary>
    <TestClass>
    Public Class WritableSerializationTest

        Private Shared Function AllCoreWritables() As DVW.Writable()
            Return New DVW.Writable() {
                New BooleanWritable(True), New BooleanWritable(False),
                New ByteWritable(-7), New ByteWritable(127),
                New DoubleWritable(-1.25), New DoubleWritable(Double.MaxValue),
                New FloatWritable(3.5F), New FloatWritable(Single.Epsilon),
                New IntWritable(Integer.MinValue), New IntWritable(42),
                New LongWritable(Long.MaxValue), New LongWritable(-1),
                NullWritable.INSTANCE,
                New Text(""), New Text("hello"), New Text("größe – 日本語"), New Text(New String("x"c, 300))}
        End Function

        Private Shared Function Serialize(ByVal w As DVW.Writable, ByVal withType As Boolean) As Byte()
            Using ms As New MemoryStream()
                Using [out] As New DataOutputStream(ms, True)
                    If withType Then
                        WritableFactory.getInstance().writeWithType(w, [out])
                    Else
                        w.write([out])
                    End If
                End Using
                Return ms.ToArray()
            End Using
        End Function

        <TestMethod>
        Public Sub testRoundTripWithType()
            For Each w As DVW.Writable In AllCoreWritables()
                Dim bytes() As Byte = Serialize(w, True)
                Using [in] As New DataInputStream(New MemoryStream(bytes))
                    Dim back As DVW.Writable = WritableFactory.getInstance().readWithType([in])
                    assertEquals(w, back)
                    assertEquals(DirectCast(w, Object).GetType(), DirectCast(back, Object).GetType())
                    assertEquals(w.getType(), back.getType())
                    assertEquals(-1, [in].read(), "stream must be fully consumed for " & w.ToString())
                End Using
            Next w
        End Sub

        <TestMethod>
        Public Sub testRoundTripReadFields()
            For Each w As DVW.Writable In AllCoreWritables()
                Dim bytes() As Byte = Serialize(w, False)
                Dim fresh As DVW.Writable = WritableFactory.getInstance().newWritable(w.getType().typeIdx())
                Using [in] As New DataInputStream(New MemoryStream(bytes))
                    fresh.readFields([in])
                End Using
                assertEquals(w, fresh)
            Next w
        End Sub

        <TestMethod>
        Public Sub testWireFormatMatchesHadoop()
            ' type index as a short, then the value in big-endian
            assertArrayEquals(New Byte() {0, 4, 0, 0, 1, 0}, Serialize(New IntWritable(256), True))
            assertArrayEquals(New Byte() {0, 0, 1}, Serialize(New BooleanWritable(True), True))
            assertArrayEquals(New Byte() {0, 1, 254}, Serialize(New ByteWritable(-2), True))
            assertArrayEquals(New Byte() {0, 6}, Serialize(NullWritable.INSTANCE, True))
            assertArrayEquals(New Byte() {&H3F, &HF0, 0, 0, 0, 0, 0, 0}, Serialize(New DoubleWritable(1.0), False))
            assertArrayEquals(New Byte() {&H3F, &H80, 0, 0}, Serialize(New FloatWritable(1.0F), False))
            assertArrayEquals(New Byte() {0, 0, 0, 0, 0, 0, 0, 5}, Serialize(New LongWritable(5), False))
            ' Text: zero-compressed length then UTF-8 bytes
            assertArrayEquals(New Byte() {2, 97, 98}, Serialize(New Text("ab"), False))
            assertArrayEquals(New Byte() {0, 7, 2, 97, 98}, Serialize(New Text("ab"), True))
            ' 200 bytes: length needs the 2-byte vint form (-113, 200)
            Dim long200 As Byte() = Serialize(New Text(New String("a"c, 200)), False)
            assertEquals(202, long200.Length)
            assertEquals(CByte(143), long200(0)) ' (byte) -113
            assertEquals(CByte(200), long200(1))
            ' BytesWritable writes its raw content without a length
            assertArrayEquals(New Byte() {9, 8, 7}, Serialize(New BytesWritable(New Byte() {9, 8, 7}), False))
        End Sub

        <TestMethod>
        Public Sub testBytesWritableReadFieldsNeedsPreallocatedContent()
            Dim target As New BytesWritable(New Byte(2) {})
            Using [in] As New DataInputStream(New MemoryStream(New Byte() {9, 8, 7}))
                target.readFields([in])
            End Using
            assertArrayEquals(New Byte() {9, 8, 7}, target.getContent())
        End Sub

        <TestMethod>
        Public Sub testVIntRoundTrip()
            Dim values() As Long = {0, 1, -1, 127, 128, -112, -113, 255, 256, 65535, 65536, -65536, 1L << 20, -(1L << 20), 1L << 40, Integer.MaxValue, Integer.MinValue, Long.MaxValue, Long.MinValue}
            For Each v As Long In values
                Dim bytes() As Byte
                Using ms As New MemoryStream()
                    Using [out] As New DataOutputStream(ms, True)
                        WritableUtils.writeVLong([out], v)
                    End Using
                    bytes = ms.ToArray()
                End Using
                assertEquals(WritableUtils.getVIntSize(v), bytes.Length, "encoded size of " & v)
                assertEquals(WritableUtils.decodeVIntSize(bytes(0)), bytes.Length, "decoded size of " & v)
                Using [in] As New DataInputStream(New MemoryStream(bytes))
                    assertEquals(v, WritableUtils.readVLong([in]))
                End Using
                assertEquals(v, WritableComparator.readVLong(bytes, 0))
                assertEquals(v < 0, WritableUtils.isNegativeVInt(CSByte(If(bytes(0) > 127, CInt(bytes(0)) - 256, CInt(bytes(0))))))
                If v >= Integer.MinValue AndAlso v <= Integer.MaxValue Then
                    Using [in] As New DataInputStream(New MemoryStream(bytes))
                        assertEquals(CInt(v), WritableUtils.readVInt([in]))
                    End Using
                    assertEquals(CInt(v), WritableComparator.readVInt(bytes, 0))
                End If
            Next v
        End Sub

        <TestMethod>
        Public Sub testVIntEncodingMatchesHadoop()
            Dim enc = Function(v As Long) As Byte()
                          Using ms As New MemoryStream()
                              Using [out] As New DataOutputStream(ms, True)
                                  WritableUtils.writeVLong([out], v)
                              End Using
                              Return ms.ToArray()
                          End Using
                      End Function
            assertArrayEquals(New Byte() {127}, enc(127))
            assertArrayEquals(New Byte() {144}, enc(-112)) ' (byte) -112
            assertArrayEquals(New Byte() {143, 128}, enc(128)) ' -113, 0x80
            assertArrayEquals(New Byte() {142, 1, 0}, enc(256)) ' -114, 0x01 0x00
            assertArrayEquals(New Byte() {135, 112}, enc(-113)) ' -121, then ~(-113) = 112
        End Sub

        <TestMethod>
        Public Sub testStringsAndCompressedStrings()
            Dim bytes() As Byte
            Using ms As New MemoryStream()
                Using [out] As New DataOutputStream(ms, True)
                    WritableUtils.writeString([out], "héllo")
                    WritableUtils.writeString([out], Nothing)
                    WritableUtils.writeStringArray([out], New String() {"a", "", "bc"})
                    WritableUtils.writeCompressedString([out], New String("z"c, 1000))
                    WritableUtils.writeCompressedString([out], Nothing)
                    WritableUtils.writeCompressedStringArray([out], New String() {"x", Nothing})
                    WritableUtils.writeCompressedStringArray([out], Nothing)
                    WritableUtils.writeEnum([out], WritableType.Float)
                    Text.writeString([out], "text-string")
                    Text.writeString([out], "skip me")
                    WritableUtils.writeCompressedByteArray([out], New Byte() {1, 2, 3})
                    WritableUtils.writeCompressedByteArray([out], Nothing)
                End Using
                bytes = ms.ToArray()
            End Using
            Using [in] As New DataInputStream(New MemoryStream(bytes))
                assertEquals("héllo", WritableUtils.readString([in]))
                assertNull(WritableUtils.readString([in]))
                assertArrayEquals(New String() {"a", "", "bc"}, WritableUtils.readStringArray([in]))
                assertEquals(New String("z"c, 1000), WritableUtils.readCompressedString([in]))
                assertNull(WritableUtils.readCompressedString([in]))
                assertArrayEquals(New String() {"x", Nothing}, WritableUtils.readCompressedStringArray([in]))
                assertNull(WritableUtils.readCompressedStringArray([in]))
                assertEquals(WritableType.Float, WritableUtils.readEnum(Of WritableType)([in], GetType(WritableType)))
                assertEquals("text-string", Text.readString([in]))
                Text.skip([in])
                assertArrayEquals(New Byte() {1, 2, 3}, WritableUtils.readCompressedByteArray([in]))
                assertNull(WritableUtils.readCompressedByteArray([in]))
                assertEquals(-1, [in].read())
            End Using
        End Sub

        <TestMethod>
        Public Sub testCompressedByteArrayIsGzip()
            Dim bytes() As Byte
            Using ms As New MemoryStream()
                Using [out] As New DataOutputStream(ms, True)
                    Dim pct As Integer = WritableUtils.writeCompressedByteArray([out], New Byte(9999) {})
                    assertTrue(pct < 10, "10000 zero bytes should compress well, got " & pct & "%")
                End Using
                bytes = ms.ToArray()
            End Using
            ' int length prefix, then the gzip magic number
            assertEquals(CByte(&H1F), bytes(4))
            assertEquals(CByte(&H8B), bytes(5))
        End Sub

        <TestMethod>
        Public Sub testSkipFully()
            Using [in] As New DataInputStream(New MemoryStream(New Byte() {1, 2, 3, 4, 5}))
                WritableUtils.skipFully([in], 3)
                assertEquals(4, [in].read())
                assertThrows(Of IOException)(Sub() WritableUtils.skipFully([in], 5))
            End Using
        End Sub

        <TestMethod>
        Public Sub testDataOutputAndInputBuffer()
            Dim outBuf As New DataOutputBuffer(4)
            outBuf.writeInt(258)
            outBuf.writeUTF("ab")
            outBuf.writeLong(-1)
            assertEquals(4 + 4 + 8, outBuf.getLength())
            assertTrue(outBuf.getData().Length >= outBuf.Length) ' backing array, valid up to Length
            assertEquals(CByte(1), outBuf.Data(2))

            Dim inBuf As New DataInputBuffer()
            inBuf.reset(outBuf.getData(), outBuf.getLength())
            assertEquals(258, inBuf.readInt())
            assertEquals(4, inBuf.getPosition())
            assertEquals("ab", inBuf.readUTF())
            assertEquals(-1L, inBuf.readLong())
            assertEquals(outBuf.Length, inBuf.getLength())
            assertEquals(-1, inBuf.read())

            ' reset to a sub-range
            inBuf.reset(New Byte() {9, 9, 0, 0, 0, 7, 9}, 2, 4)
            assertEquals(7, inBuf.readInt())
            assertEquals(-1, inBuf.read())
            assertEquals(6, inBuf.Position)

            ' DataOutputBuffer.write(DataInput, len) copies raw bytes; reset empties it
            outBuf.reset()
            assertEquals(0, outBuf.Length)
            inBuf.reset(New Byte() {1, 2, 3, 4}, 4)
            outBuf.write(inBuf, 3)
            assertEquals(3, outBuf.Length)
            Using ms As New MemoryStream()
                outBuf.writeTo(ms)
                assertArrayEquals(New Byte() {1, 2, 3}, ms.ToArray())
            End Using
        End Sub

        <TestMethod>
        Public Sub testToByteArray()
            Dim bytes() As Byte = WritableUtils.toByteArray(New IntWritable(1), New Text("ab"), New BooleanWritable(True))
            assertArrayEquals(New Byte() {0, 0, 0, 1, 2, 97, 98, 1}, bytes)
        End Sub

        <TestMethod>
        Public Sub testRawComparators()
            Dim intCmp As WritableComparator = WritableComparator.get(GetType(IntWritable))
            assertTrue(TypeOf intCmp Is IntWritable.Comparator)
            assertEquals(GetType(IntWritable), intCmp.getKeyClass())
            Dim b1() As Byte = WritableUtils.toByteArray(New IntWritable(-5))
            Dim b2() As Byte = WritableUtils.toByteArray(New IntWritable(3))
            assertTrue(intCmp.compare(b1, 0, 4, b2, 0, 4) < 0)
            assertTrue(intCmp.compare(b2, 0, 4, b1, 0, 4) > 0)
            assertEquals(0, intCmp.compare(b1, 0, 4, b1, 0, 4))
            assertTrue(intCmp.compare(New IntWritable(1), New IntWritable(2)) < 0)
            assertTrue(DirectCast(intCmp, IComparer(Of Object)).Compare(New IntWritable(2), New IntWritable(1)) > 0)

            Dim longCmp As WritableComparator = WritableComparator.get(GetType(LongWritable))
            Dim l1() As Byte = WritableUtils.toByteArray(New LongWritable(Long.MinValue))
            Dim l2() As Byte = WritableUtils.toByteArray(New LongWritable(0))
            assertTrue(longCmp.compare(l1, 0, 8, l2, 0, 8) < 0)
            Dim dec As New LongWritable.DecreasingComparator()
            assertTrue(dec.compare(l1, 0, 8, l2, 0, 8) > 0)
            assertTrue(dec.compare(New LongWritable(1), New LongWritable(2)) > 0)

            Dim dblCmp As WritableComparator = WritableComparator.get(GetType(DoubleWritable))
            Dim d1() As Byte = WritableUtils.toByteArray(New DoubleWritable(-0.5))
            Dim d2() As Byte = WritableUtils.toByteArray(New DoubleWritable(0.25))
            assertTrue(dblCmp.compare(d1, 0, 8, d2, 0, 8) < 0)
            Dim fltCmp As WritableComparator = WritableComparator.get(GetType(FloatWritable))
            Dim f1() As Byte = WritableUtils.toByteArray(New FloatWritable(2))
            Dim f2() As Byte = WritableUtils.toByteArray(New FloatWritable(1))
            assertTrue(fltCmp.compare(f1, 0, 4, f2, 0, 4) > 0)
            Dim byteCmp As WritableComparator = WritableComparator.get(GetType(ByteWritable))
            assertTrue(byteCmp.compare(New Byte() {255}, 0, 1, New Byte() {1}, 0, 1) < 0) ' -1 < 1 (signed)
            Dim boolCmp As WritableComparator = WritableComparator.get(GetType(BooleanWritable))
            assertTrue(boolCmp.compare(New Byte() {0}, 0, 1, New Byte() {1}, 0, 1) < 0)

            ' Text: the comparator skips the vint length prefix and compares bytes lexicographically
            Dim textCmp As WritableComparator = WritableComparator.get(GetType(Text))
            Dim t1() As Byte = WritableUtils.toByteArray(New Text("abc"))
            Dim t2() As Byte = WritableUtils.toByteArray(New Text("abd"))
            Dim t3() As Byte = WritableUtils.toByteArray(New Text("ab"))
            assertTrue(textCmp.compare(t1, 0, t1.Length, t2, 0, t2.Length) < 0)
            assertTrue(textCmp.compare(t1, 0, t1.Length, t3, 0, t3.Length) > 0)
            assertEquals(0, textCmp.compare(t1, 0, t1.Length, t1, 0, t1.Length))
        End Sub

        <TestMethod>
        Public Sub testGenericComparatorDeserializesKeys()
            Dim cmp As WritableComparator = WritableComparator.get(GetType(PairKey))
            assertTrue(cmp Is WritableComparator.get(GetType(PairKey)), "comparators are cached")
            Dim a() As Byte = WritableUtils.toByteArray(New PairKey(1, 9))
            Dim b() As Byte = WritableUtils.toByteArray(New PairKey(2, 0))
            assertTrue(cmp.compare(a, 0, a.Length, b, 0, b.Length) < 0)
            assertTrue(cmp.compare(b, 0, b.Length, a, 0, a.Length) > 0)
            assertEquals(0, cmp.compare(a, 0, a.Length, a, 0, a.Length))
            assertTrue(TypeOf cmp.newKey() Is PairKey)
        End Sub

        ''' <summary>A custom key without an optimized comparator: ordered by the first value.</summary>
        Public Class PairKey
            Implements WritableComparable

            Public first As Integer
            Public second As Integer

            Public Sub New()
            End Sub

            Public Sub New(ByVal first As Integer, ByVal second As Integer)
                Me.first = first
                Me.second = second
            End Sub

            Public Sub write(ByVal [out] As DataOutput) Implements DVW.Writable.write
                [out].writeInt(first)
                [out].writeInt(second)
            End Sub

            Public Sub readFields(ByVal [in] As DataInput) Implements DVW.Writable.readFields
                first = [in].readInt()
                second = [in].readInt()
            End Sub

            Public Sub writeType(ByVal [out] As DataOutput) Implements DVW.Writable.writeType
                [out].writeShort(999)
            End Sub

            Public Function toDouble() As Double Implements DVW.Writable.toDouble
                Return first
            End Function

            Public Function toFloat() As Single Implements DVW.Writable.toFloat
                Return first
            End Function

            Public Function toInt() As Integer Implements DVW.Writable.toInt
                Return first
            End Function

            Public Function toLong() As Long Implements DVW.Writable.toLong
                Return first
            End Function

            Public Shadows Function [getType]() As WritableType Implements DVW.Writable.getType
                Return WritableType.Int
            End Function

            Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
                Return first.CompareTo(DirectCast(obj, PairKey).first)
            End Function
        End Class

        <TestMethod>
        Public Sub testTextUtilities()
            Dim t As New Text("hello wörld")
            assertEquals(12, t.getLength()) ' ö is two bytes
            assertEquals(12, Text.utf8Length("hello wörld"))
            assertEquals(4, Text.utf8Length(ChrW(&HD83D) & ChrW(&HDE00))) ' surrogate pair
            assertEquals(3, Text.utf8Length(ChrW(&HD83D))) ' unpaired surrogate
            assertEquals(6, t.find("wörld"))
            assertEquals(-1, t.find("world"))
            assertEquals(2, t.find("l"))
            assertEquals(3, t.find("l", 3))
            assertEquals(AscW("h"c), t.charAt(0))
            assertEquals(&HF6, t.charAt(7)) ' ö
            assertEquals(-1, t.charAt(8)) ' trailing byte
            assertEquals(-1, t.charAt(12))
            assertEquals(-1, t.charAt(-1))

            Text.validateUTF8(t.getBytes(), 0, t.getLength())
            Dim ex As Text.MalformedInputException = assertThrows(Of Text.MalformedInputException)(Sub() Text.validateUTF8(New Byte() {97, 255, 98}))
            assertEquals(1, ex.getInputLength())
            assertThrows(Of Text.MalformedInputException)(Sub() Text.validateUTF8(New Byte() {&HC0, &H80})) ' overlong
            assertThrows(Of Text.MalformedInputException)(Sub() Text.validateUTF8(New Byte() {&HED, &HA0, &H80})) ' surrogate
            Text.validateUTF8(New Byte() {&HF0, &H9F, &H98, &H80}) ' 4-byte emoji is fine

            ' mutation: set/append/clear reuse the buffer
            Dim u As New Text("abc")
            u.append(Encoding.UTF8.GetBytes("def"), 0, 3)
            assertEquals("abcdef", u.ToString())
            u.set(New Text("xy"))
            assertEquals("xy", u.ToString())
            u.set(Encoding.UTF8.GetBytes("12345"), 1, 3)
            assertEquals("234", u.ToString())
            u.clear()
            assertEquals("", u.ToString())
            assertEquals(0, u.getLength())
            assertEquals(New Text(""), u)

            ' replacement vs reporting decoders
            assertEquals("a" & ChrW(&HFFFD) & "b", Text.decode(New Byte() {97, 255, 98}))
            assertThrows(Of DecoderFallbackException)(Sub() Text.decode(New Byte() {97, 255, 98}, 0, 3, False))
            assertArrayEquals(New Byte() {97, 63}, Text.encode("a" & ChrW(&HD800))) ' unpaired surrogate -> '?'
            assertThrows(Of EncoderFallbackException)(Sub() Text.encode("a" & ChrW(&HD800), False))

            Dim pos As Integer = 0
            assertEquals(&H1F600, Text.bytesToCodePoint(New Byte() {&HF0, &H9F, &H98, &H80}, pos))
            assertEquals(4, pos)
        End Sub

        <TestMethod>
        Public Sub testBinaryComparableEquality()
            Dim a As New Text("same")
            Dim b As New Text("same")
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertEquals(0, a.CompareTo(b))
            assertTrue(a.CompareTo(Encoding.UTF8.GetBytes("samf"), 0, 4) < 0)
            assertTrue(a.CompareTo(DirectCast(New Text("sam"), Object)) > 0)
            assertNotEquals(a, New Text("Same"))
            ' a Text with a larger backing buffer than its length still equals a fresh one
            Dim c As New Text("something longer")
            c.set(Encoding.UTF8.GetBytes("same"), 0, 4)
            assertTrue(c.getBytes().Length > c.getLength())
            assertEquals(a, c)
            assertEquals(a.GetHashCode(), c.GetHashCode())
        End Sub

        <TestMethod>
        Public Sub testJsonRoundTrip()
            Dim opts As JsonSerializerOptions = ObjectMapperHolder.JsonMapper
            Dim json As String = JsonSerializer.Serialize(Of DVW.Writable)(New IntWritable(5), opts)
            assertTrue(json.Contains("""@class"":""org.datavec.api.writable.IntWritable"""), json)
            assertTrue(json.Contains("""value"":5"), json)
            assertEquals(New IntWritable(5), JsonSerializer.Deserialize(Of DVW.Writable)(json, opts))

            Dim list As New List(Of DVW.Writable) From {New IntWritable(1), New LongWritable(-2), New DoubleWritable(2.5), New FloatWritable(0.5F), New BooleanWritable(True), New ByteWritable(-3), New Text("tëxt"), NullWritable.INSTANCE, New BytesWritable(New Byte() {1, 2, 255})}
            Dim listJson As String = JsonSerializer.Serialize(list, opts)
            Dim back As List(Of DVW.Writable) = JsonSerializer.Deserialize(Of List(Of DVW.Writable))(listJson, opts)
            assertEquals(list, back)
            For i As Integer = 0 To list.Count - 1
                assertEquals(DirectCast(list(i), Object).GetType(), DirectCast(back(i), Object).GetType())
            Next i

            ' JSON written by DataVec (Java) for a Text: full backing array plus valid length
            Dim javaText As String = "{""@class"":""org.datavec.api.writable.Text"",""bytes"":""YWJjZGVm"",""length"":3}"
            assertEquals(New Text("abc"), JsonSerializer.Deserialize(Of DVW.Writable)(javaText, opts))
        End Sub

    End Class

End Namespace
