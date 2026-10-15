Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLine
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports LineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.LineRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports InputStreamInputSplit = DeepLearning4VB.org.datavec.api.split.InputStreamInputSplit
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class LineReaderTest

        Private tmpDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            tmpDir = Path.Combine(Path.GetTempPath(), "dl4vb-tests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tmpDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(tmpDir) Then Directory.Delete(tmpDir, True)
        End Sub

        ''' <summary>commons-io <c>FileUtils.writeLines</c>: one line per entry, each followed by the line separator.</summary>
        Private Sub writeLines(ByVal name As String, ByVal ParamArray lines() As String)
            File.WriteAllLines(Path.Combine(tmpDir, name), lines, New UTF8Encoding(False))
        End Sub

        <TestMethod>
        Public Sub testLineReader()
            writeLines("tmp1.txt", "1", "2", "3")
            writeLines("tmp2.txt", "4", "5", "6")
            writeLines("tmp3.txt", "7", "8", "9")

            Dim split As InputSplit = New FileSplit(tmpDir)

            Using reader As RecordReader = New LineRecordReader()
                reader.initialize(split)

                Dim count As Integer = 0
                Dim list As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                Do While reader.hasNext()
                    Dim l As IList(Of WritableT) = reader.next()
                    assertEquals(1, l.Count)
                    list.Add(l)
                    count += 1
                Loop

                assertEquals(9, count)
                'every line is read exactly once, in file order
                Dim values As New List(Of String)()
                For Each l As IList(Of WritableT) In list
                    values.Add(l(0).ToString())
                Next l
                values.Sort(StringComparer.Ordinal)
                assertEquals(Arrays.asList("1", "2", "3", "4", "5", "6", "7", "8", "9"), values)
            End Using
        End Sub

        <TestMethod>
        Public Sub testLineReaderMetaData()
            writeLines("tmp1.txt", "1", "2", "3")
            writeLines("tmp2.txt", "4", "5", "6")
            writeLines("tmp3.txt", "7", "8", "9")

            Dim split As InputSplit = New FileSplit(tmpDir)

            Using reader As RecordReader = New LineRecordReader()
                reader.initialize(split)

                Dim list As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                Do While reader.hasNext()
                    list.Add(reader.next())
                Loop
                assertEquals(9, list.Count)

                Dim out2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                Dim out3 As IList(Of Record) = New List(Of Record)()
                Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
                reader.reset()
                Dim count As Integer = 0
                Do While reader.hasNext()
                    Dim r As Record = reader.nextRecord()
                    out2.Add(r.getRecord())
                    out3.Add(r)
                    meta.Add(r.MetaData)
                    Dim fileIdx As Integer = count \ 3
                    Dim uri As Uri = r.MetaData.URI
                    assertEquals(split.locations()(fileIdx), uri)
                    assertEquals(count Mod 3, DirectCast(r.MetaData, RecordMetaDataLine).getLineNumber())
                    count += 1
                Loop

                assertEquals(list, out2)

                Dim fromMeta As IList(Of Record) = reader.loadFromMetaData(meta)
                assertEquals(out3, fromMeta)

                'try: second line of second and third files only...
                Dim subsetMeta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
                subsetMeta.Add(meta(4))
                subsetMeta.Add(meta(7))

                Dim subset As IList(Of Record) = reader.loadFromMetaData(subsetMeta)
                assertEquals(2, subset.Count)
                assertEquals(out3(4), subset(0))
                assertEquals(out3(7), subset(1))
            End Using
        End Sub

        <TestMethod>
        Public Sub testLineReaderWithInputStreamInputSplit()
            Dim tmp1 As String = Path.Combine(tmpDir, "tmp1.txt.gz")
            Using os As New GZipStream(New FileStream(tmp1, FileMode.Create, FileAccess.Write), CompressionMode.Compress)
                Using w As New StreamWriter(os, New UTF8Encoding(False))
                    For Each line As String In Arrays.asList("1", "2", "3", "4", "5", "6", "7", "8", "9")
                        w.Write(line)
                        w.Write(Environment.NewLine)
                    Next line
                End Using
            End Using

            Using gz As New GZipStream(New FileStream(tmp1, FileMode.Open, FileAccess.Read), CompressionMode.Decompress)
                Dim split As InputSplit = New InputStreamInputSplit(gz)

                Using reader As RecordReader = New LineRecordReader()
                    reader.initialize(split)

                    Dim count As Integer = 0
                    Do While reader.hasNext()
                        Dim l As IList(Of WritableT) = reader.next()
                        assertEquals(1, l.Count)
                        count += 1
                        assertEquals(count.ToString(), l(0).ToString())
                    Loop

                    assertEquals(9, count)
                End Using
            End Using
        End Sub

        ''' <summary>Reading single lines from a stream, and the listener hook (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testRecordFromStream()
            Dim reader As New LineRecordReader()
            Using dis As New DataInputStream(New MemoryStream(Encoding.UTF8.GetBytes("first line" & vbLf & "second line")))
                Dim l As IList(Of WritableT) = reader.record(New Uri("file:///C:/data/x.txt"), dis)
                assertEquals(JavaCollections.singletonList(Of WritableT)(New Text("first line")), l)
            End Using
            assertNull(reader.Labels)
            assertTrue(reader.resetSupported())
            'reset before initialization is an error
            assertThrows(Of NotSupportedException)(Sub() reader.reset())
        End Sub

    End Class

End Namespace
