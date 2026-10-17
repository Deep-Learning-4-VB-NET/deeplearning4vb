Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.records.metadata

Namespace org.datavec.api.records.metadata

    ''' <summary>
    ''' The record metadata classes (Lombok <c>@Data</c> in Java): locations, URIs, reader classes,
    ''' value equality, hash codes and the Lombok string forms.
    ''' </summary>
    <TestClass>
    Public Class RecordMetaDataTest

        Private Shared ReadOnly FileUri As New Uri("file:///C:/data/file.csv")
        Private Shared ReadOnly OtherUri As New Uri("file:///C:/data/other.csv")

        <TestMethod>
        Public Sub testRecordMetaDataLine()
            Dim line As New RecordMetaDataLine(3, FileUri, GetType(String))
            assertEquals(3, line.getLineNumber())
            assertEquals(3, line.LineNumber)
            assertEquals(FileUri, line.getURI())
            assertEquals(FileUri, line.URI)
            assertEquals(GetType(String), line.getReaderClass())
            assertEquals(GetType(String), line.ReaderClass)
            'location: base name + "." + extension, then the line number
            assertEquals("file.csv line 3", line.getLocation())
            assertEquals(line.getLocation(), line.Location)
            assertEquals("RecordMetaDataLine(lineNumber=3, uri=file:///C:/data/file.csv, readerClass=System.String)", line.ToString())

            assertEquals(New RecordMetaDataLine(3, FileUri, GetType(String)), line)
            assertEquals(New RecordMetaDataLine(3, FileUri, GetType(String)).GetHashCode(), line.GetHashCode())
            assertNotEquals(New RecordMetaDataLine(4, FileUri, GetType(String)), line)
            assertNotEquals(New RecordMetaDataLine(3, OtherUri, GetType(String)), line)
            assertNotEquals(New RecordMetaDataLine(3, FileUri, GetType(Integer)), line)
            assertNotEquals(New RecordMetaDataLine(3, FileUri, Nothing), line)
            assertFalse(line.Equals(Nothing))
            assertFalse(line.Equals("file.csv line 3"))

            'setters
            line.setLineNumber(10)
            line.setUri(OtherUri)
            line.setReaderClass(Nothing)
            assertEquals("other.csv line 10", line.getLocation())
            assertNull(line.getReaderClass())
            assertEquals("RecordMetaDataLine(lineNumber=10, uri=file:///C:/data/other.csv, readerClass=null)", line.ToString())
            line.LineNumber = 11
            line.URI = Nothing
            line.ReaderClass = GetType(Double)
            assertEquals(11, line.getLineNumber())
            assertEquals(GetType(Double), line.getReaderClass())
        End Sub

        <TestMethod>
        Public Sub testRecordMetaDataLineLocationForms()
            'no URI: only the line number
            assertEquals("line 7", New RecordMetaDataLine(7, Nothing, Nothing).getLocation())
            'a name without extension keeps the trailing dot (FilenameUtils.getBaseName + "." + getExtension)
            assertEquals("noext. line 1", New RecordMetaDataLine(1, New Uri("file:///C:/data/noext"), Nothing).getLocation())
            'only the last path segment is used, for non-file URIs as well
            assertEquals("part-00000.csv line 2", New RecordMetaDataLine(2, New Uri("hdfs://host/data/part-00000.csv"), Nothing).getLocation())
            assertEquals("archive.tar.gz line 0", New RecordMetaDataLine(0, New Uri("file:///C:/x/archive.tar.gz"), Nothing).getLocation())
        End Sub

        <TestMethod>
        Public Sub testRecordMetaDataLineInterval()
            Dim interval As New RecordMetaDataLineInterval(2, 5, FileUri, GetType(String))
            assertEquals(2, interval.getLineNumberStart())
            assertEquals(5, interval.getLineNumberEnd())
            assertEquals(2, interval.LineNumberStart)
            assertEquals(5, interval.LineNumberEnd)
            assertEquals(FileUri, interval.getURI())
            assertEquals(GetType(String), interval.getReaderClass())
            assertEquals("file.csv lines 2-5", interval.getLocation())
            assertEquals("lines 2-5", New RecordMetaDataLineInterval(2, 5, Nothing, Nothing).getLocation())
            assertEquals("RecordMetaDataLineInterval(lineNumberStart=2, lineNumberEnd=5, uri=file:///C:/data/file.csv, readerClass=System.String)", interval.ToString())

            assertEquals(New RecordMetaDataLineInterval(2, 5, FileUri, GetType(String)), interval)
            assertEquals(New RecordMetaDataLineInterval(2, 5, FileUri, GetType(String)).GetHashCode(), interval.GetHashCode())
            assertNotEquals(New RecordMetaDataLineInterval(2, 6, FileUri, GetType(String)), interval)
            assertNotEquals(New RecordMetaDataLineInterval(1, 5, FileUri, GetType(String)), interval)
            assertNotEquals(New RecordMetaDataLineInterval(2, 5, OtherUri, GetType(String)), interval)
            'a line and a line interval never compare equal
            assertNotEquals(New RecordMetaDataLine(2, FileUri, GetType(String)), interval)

            interval.setLineNumberStart(20)
            interval.setLineNumberEnd(30)
            interval.setUri(OtherUri)
            interval.setReaderClass(Nothing)
            assertEquals("other.csv lines 20-30", interval.Location)
            assertNull(interval.ReaderClass)
            interval.LineNumberStart = 1
            interval.LineNumberEnd = 2
            interval.URI = FileUri
            interval.ReaderClass = GetType(Integer)
            assertEquals(New RecordMetaDataLineInterval(1, 2, FileUri, GetType(Integer)), interval)
        End Sub

        <TestMethod>
        Public Sub testRecordMetaDataIndex()
            Dim index As New RecordMetaDataIndex(7L, FileUri, GetType(String))
            assertEquals(7L, index.getIndex())
            assertEquals(7L, index.Index)
            assertEquals("index=7", index.getLocation())
            assertEquals("index=7", index.Location)
            assertEquals(FileUri, index.getURI())
            assertEquals(FileUri, index.URI)
            assertEquals(GetType(String), index.getReaderClass())
            assertEquals(GetType(String), index.ReaderClass)
            assertEquals("RecordMetaDataIndex(index=7, uri=file:///C:/data/file.csv, readerClass=System.String)", index.ToString())
            assertEquals("RecordMetaDataIndex(index=0, uri=null, readerClass=null)", New RecordMetaDataIndex(0L, Nothing, Nothing).ToString())

            assertEquals(New RecordMetaDataIndex(7L, FileUri, GetType(String)), index)
            assertEquals(New RecordMetaDataIndex(7L, FileUri, GetType(String)).GetHashCode(), index.GetHashCode())
            assertNotEquals(New RecordMetaDataIndex(8L, FileUri, GetType(String)), index)
            assertNotEquals(New RecordMetaDataIndex(7L, OtherUri, GetType(String)), index)
            assertNotEquals(New RecordMetaDataIndex(7L, FileUri, Nothing), index)
            assertEquals(New RecordMetaDataIndex(7L, Nothing, Nothing), New RecordMetaDataIndex(7L, Nothing, Nothing))
            'a large index is a long, not an int
            assertEquals("index=" & Long.MaxValue, New RecordMetaDataIndex(Long.MaxValue, Nothing, Nothing).getLocation())
        End Sub

        <TestMethod>
        Public Sub testRecordMetaDataInterval()
            Dim interval As New RecordMetaDataInterval(1L, 5L, FileUri)
            assertEquals(1L, interval.getFrom())
            assertEquals(5L, interval.getTo())
            assertEquals(1L, interval.From)
            assertEquals(5L, interval.To)
            assertEquals("interval(1,5)", interval.getLocation())
            assertEquals(FileUri, interval.getURI())
            'the three-argument constructor leaves the reader class unset
            assertNull(interval.getReaderClass())
            assertEquals("RecordMetaDataInterval(from=1, to=5, uri=file:///C:/data/file.csv, readerClass=null)", interval.ToString())

            Dim withReader As New RecordMetaDataInterval(1L, 5L, FileUri, GetType(String))
            assertEquals(GetType(String), withReader.getReaderClass())
            assertNotEquals(withReader, interval)
            interval.setReaderClass(GetType(String))
            assertEquals(withReader, interval)
            assertEquals(withReader.GetHashCode(), interval.GetHashCode())
            interval.ReaderClass = Nothing
            assertNotEquals(withReader, interval)

            assertNotEquals(New RecordMetaDataInterval(0L, 5L, FileUri), New RecordMetaDataInterval(1L, 5L, FileUri))
            assertNotEquals(New RecordMetaDataInterval(1L, 6L, FileUri), New RecordMetaDataInterval(1L, 5L, FileUri))
            assertNotEquals(New RecordMetaDataInterval(1L, 5L, OtherUri), New RecordMetaDataInterval(1L, 5L, FileUri))
            assertNotEquals(New RecordMetaDataIndex(1L, FileUri, Nothing), New RecordMetaDataInterval(1L, 1L, FileUri))
        End Sub

        <TestMethod>
        Public Sub testRecordMetaDataURI()
            Dim meta As New RecordMetaDataURI(FileUri)
            assertEquals(FileUri, meta.getURI())
            assertEquals(FileUri, meta.URI)
            assertNull(meta.getReaderClass())
            assertNull(meta.ReaderClass)
            'the location is the URI itself
            assertEquals(FileUri.ToString(), meta.getLocation())
            assertEquals("file:///C:/data/file.csv", meta.Location)
            assertEquals("RecordMetaDataURI(uri=file:///C:/data/file.csv, readerClass=null)", meta.ToString())

            Dim withReader As New RecordMetaDataURI(FileUri, GetType(String))
            assertEquals(GetType(String), withReader.getReaderClass())
            assertEquals("RecordMetaDataURI(uri=file:///C:/data/file.csv, readerClass=System.String)", withReader.ToString())
            assertEquals(New RecordMetaDataURI(FileUri, GetType(String)), withReader)
            assertEquals(New RecordMetaDataURI(FileUri, GetType(String)).GetHashCode(), withReader.GetHashCode())
            assertNotEquals(meta, withReader)
            assertNotEquals(New RecordMetaDataURI(OtherUri), meta)
            assertEquals(New RecordMetaDataURI(Nothing), New RecordMetaDataURI(Nothing))
            assertNotEquals(New RecordMetaDataURI(Nothing), meta)
        End Sub

        <TestMethod>
        Public Sub testRecordMetaDataImageURI()
            Dim image As New RecordMetaDataImageURI(FileUri, GetType(String), 3, 4, 5)
            assertEquals(3, image.getOrigC())
            assertEquals(4, image.getOrigH())
            assertEquals(5, image.getOrigW())
            assertEquals(3, image.OrigC)
            assertEquals(4, image.OrigH)
            assertEquals(5, image.OrigW)
            'the URI and reader class are inherited from RecordMetaDataURI
            assertTrue(TypeOf image Is RecordMetaDataURI)
            assertEquals(FileUri, image.getURI())
            assertEquals(GetType(String), image.getReaderClass())
            assertEquals("file:///C:/data/file.csv", image.getLocation())
            'Lombok @Data without callSuper: only the image fields take part in equals/hashCode/toString
            assertEquals("RecordMetaDataImageURI(origC=3, origH=4, origW=5)", image.ToString())
            assertEquals(New RecordMetaDataImageURI(OtherUri, Nothing, 3, 4, 5), image)
            assertEquals(New RecordMetaDataImageURI(OtherUri, Nothing, 3, 4, 5).GetHashCode(), image.GetHashCode())
            assertNotEquals(New RecordMetaDataImageURI(FileUri, GetType(String), 1, 4, 5), image)
            assertNotEquals(New RecordMetaDataImageURI(FileUri, GetType(String), 3, 1, 5), image)
            assertNotEquals(New RecordMetaDataImageURI(FileUri, GetType(String), 3, 4, 1), image)
            'an image URI is never equal to a plain URI metadata (and vice versa)
            assertNotEquals(New RecordMetaDataURI(FileUri, GetType(String)), image)
            assertFalse(image.Equals(New RecordMetaDataURI(FileUri, GetType(String))))

            image.setOrigC(1)
            image.setOrigH(2)
            image.setOrigW(3)
            assertEquals("RecordMetaDataImageURI(origC=1, origH=2, origW=3)", image.ToString())
            image.OrigC = 7
            image.OrigH = 8
            image.OrigW = 9
            assertEquals(New RecordMetaDataImageURI(Nothing, Nothing, 7, 8, 9), image)
        End Sub

        <TestMethod>
        Public Sub testRecordMetaDataComposable()
            Dim first As New RecordMetaDataIndex(1L, FileUri, GetType(String))
            Dim second As New RecordMetaDataInterval(2L, 3L, OtherUri)
            Dim composable As New RecordMetaDataComposable(first, second)
            assertNull(composable.getReaderClass())
            assertEquals("locations(index=1,interval(2,3))", composable.getLocation())
            assertEquals(composable.getLocation(), composable.Location)
            'the URI of the composite is the URI of its first element
            assertEquals(FileUri, composable.getURI())
            assertEquals(FileUri, composable.URI)
            assertEquals(2, composable.getMeta().Length)
            assertTrue(composable.getMeta()(0) Is first)
            assertTrue(composable.Meta(1) Is second)

            Dim typed As New RecordMetaDataComposable(GetType(String), first, second)
            assertEquals(GetType(String), typed.getReaderClass())
            assertEquals(GetType(String), typed.ReaderClass)
            assertNotEquals(typed, composable)
            composable.setReaderClass(GetType(String))
            assertEquals(typed, composable)
            assertEquals(typed.GetHashCode(), composable.GetHashCode())

            'equality is by content of the metadata array, not by identity
            Dim same As New RecordMetaDataComposable(GetType(String), New RecordMetaDataIndex(1L, FileUri, GetType(String)), New RecordMetaDataInterval(2L, 3L, OtherUri))
            assertEquals(same, composable)
            assertEquals(same.GetHashCode(), composable.GetHashCode())
            assertNotEquals(New RecordMetaDataComposable(GetType(String), second, first), composable)
            assertNotEquals(New RecordMetaDataComposable(GetType(String), first), composable)

            Dim text As String = composable.ToString()
            assertEquals("RecordMetaDataComposable(readerClass=System.String, meta=[RecordMetaDataIndex(index=1, uri=file:///C:/data/file.csv, readerClass=System.String), RecordMetaDataInterval(from=2, to=3, uri=file:///C:/data/other.csv, readerClass=null)])", text)

            composable.setMeta(New RecordMetaData() {second})
            assertEquals("locations(interval(2,3))", composable.getLocation())
            assertEquals(OtherUri, composable.getURI())
            composable.Meta = New RecordMetaData() {}
            assertEquals("locations()", composable.getLocation())
            composable.ReaderClass = Nothing
            assertEquals("RecordMetaDataComposable(readerClass=null, meta=[])", composable.ToString())
        End Sub

        <TestMethod>
        Public Sub testRecordMetaDataComposableMap()
            Dim first As New RecordMetaDataIndex(1L, FileUri, GetType(String))
            Dim second As New RecordMetaDataLine(4, OtherUri, GetType(String))
            'a sorted map, so that the iteration order is defined
            Dim map As IDictionary(Of String, RecordMetaData) = New SortedDictionary(Of String, RecordMetaData)(StringComparer.Ordinal)
            map("a") = first
            map("b") = second
            Dim composable As New RecordMetaDataComposableMap(map)
            assertNull(composable.getReaderClass())
            assertEquals("locations(a=index=1,b=other.csv line 4)", composable.getLocation())
            assertEquals(composable.getLocation(), composable.Location)
            'the URI is that of the first entry
            assertEquals(FileUri, composable.getURI())
            assertEquals(FileUri, composable.URI)
            assertTrue(composable.getMeta() Is map)
            assertTrue(composable.Meta Is map)

            Dim typed As New RecordMetaDataComposableMap(GetType(String), map)
            assertEquals(GetType(String), typed.getReaderClass())
            assertNotEquals(typed, composable)
            composable.setReaderClass(GetType(String))
            assertEquals(typed, composable)
            assertEquals(typed.GetHashCode(), composable.GetHashCode())

            'equality is by map content (any IDictionary with equal entries)
            Dim other As IDictionary(Of String, RecordMetaData) = New Dictionary(Of String, RecordMetaData)()
            other("b") = New RecordMetaDataLine(4, OtherUri, GetType(String))
            other("a") = New RecordMetaDataIndex(1L, FileUri, GetType(String))
            assertEquals(New RecordMetaDataComposableMap(GetType(String), other), composable)
            assertEquals(New RecordMetaDataComposableMap(GetType(String), other).GetHashCode(), composable.GetHashCode())
            other("c") = first
            assertNotEquals(New RecordMetaDataComposableMap(GetType(String), other), composable)
            Dim renamed As IDictionary(Of String, RecordMetaData) = New Dictionary(Of String, RecordMetaData)()
            renamed("x") = first
            renamed("b") = second
            assertNotEquals(New RecordMetaDataComposableMap(GetType(String), renamed), composable)

            assertEquals("RecordMetaDataComposableMap(readerClass=System.String, meta={a=RecordMetaDataIndex(index=1, uri=file:///C:/data/file.csv, readerClass=System.String), b=RecordMetaDataLine(lineNumber=4, uri=file:///C:/data/other.csv, readerClass=System.String)})", composable.ToString())

            Dim onlyOne As IDictionary(Of String, RecordMetaData) = New Dictionary(Of String, RecordMetaData)()
            onlyOne("only") = second
            composable.setMeta(onlyOne)
            assertEquals("locations(only=other.csv line 4)", composable.getLocation())
            assertEquals(OtherUri, composable.getURI())
            composable.Meta = Nothing
            composable.ReaderClass = Nothing
            assertEquals("RecordMetaDataComposableMap(readerClass=null, meta=null)", composable.ToString())
            assertEquals(New RecordMetaDataComposableMap(Nothing), composable)
        End Sub

        ''' <summary>All metadata classes are serializable (they travel with records in Spark jobs in Java).</summary>
        <TestMethod>
        Public Sub testSerializable()
            For Each t As Type In New Type() {GetType(RecordMetaDataLine), GetType(RecordMetaDataLineInterval), GetType(RecordMetaDataIndex), GetType(RecordMetaDataInterval), GetType(RecordMetaDataURI), GetType(RecordMetaDataImageURI), GetType(RecordMetaDataComposable), GetType(RecordMetaDataComposableMap)}
                assertTrue(isSerializableType(t), t.Name & " should be serializable")
            Next t

            Dim original As New RecordMetaDataComposable(GetType(String), New RecordMetaDataLine(3, FileUri, GetType(String)), New RecordMetaDataImageURI(OtherUri, Nothing, 3, 4, 5))
            Dim formatter As New ObjectFormatter()
            Dim copy As RecordMetaDataComposable
            Using ms As New MemoryStream()
                formatter.Serialize(ms, original)
                ms.Position = 0
                copy = DirectCast(formatter.Deserialize(ms), RecordMetaDataComposable)
            End Using
            assertFalse(copy Is original)
            assertEquals(original, copy)
            assertEquals(original.getLocation(), copy.getLocation())
            assertEquals(FileUri, copy.getURI())
            assertEquals(GetType(String), copy.getReaderClass())
            assertEquals(5, DirectCast(copy.getMeta()(1), RecordMetaDataImageURI).getOrigW())
        End Sub

    End Class

End Namespace
