Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVRegexRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRegexRecordReader
Imports FileRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.FileRecordWriter
Imports CSVRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.csv.CSVRecordWriter
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports InputStreamInputSplit = DeepLearning4VB.org.datavec.api.split.InputStreamInputSplit
Imports StringSplit = DeepLearning4VB.org.datavec.api.split.StringSplit
Imports NumberOfRecordsPartitioner = DeepLearning4VB.org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4j.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class CSVRecordReaderTest

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

        <TestMethod>
        Public Sub testNext()
            Dim reader As New CSVRecordReader()
            reader.initialize(New StringSplit("1,1,8.0,,,,14.0,,,,15.0,,,,,,,,,,,,1"))
            Dim count As Integer = 0
            Do While reader.hasNext()
                Dim vals As IList(Of WritableT) = reader.next()
                Dim arr As IList(Of WritableT) = New List(Of WritableT)(vals)
                assertEquals(23, vals.Count, "Entry count")
                Dim lastEntry As Text = DirectCast(arr(arr.Count - 1), Text)
                assertEquals(1, lastEntry.Length, "Last entry garbage")
                count += 1
            Loop
            assertEquals(1, count)
        End Sub

        <TestMethod>
        Public Sub testEmptyEntries()
            Dim reader As New CSVRecordReader()
            reader.initialize(New StringSplit("1,1,8.0,,,,14.0,,,,15.0,,,,,,,,,,,,"))
            Dim count As Integer = 0
            Do While reader.hasNext()
                Dim vals As IList(Of WritableT) = reader.next()
                assertEquals(23, vals.Count, "Entry count")
                count += 1
            Loop
            assertEquals(1, count)
        End Sub

        <TestMethod>
        Public Sub testReset()
            Using rr As New CSVRecordReader(0, ","c)
                rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))
                Dim nResets As Integer = 5
                For i As Integer = 0 To nResets - 1
                    Dim lineCount As Integer = 0
                    Do While rr.hasNext()
                        Dim line As IList(Of WritableT) = rr.next()
                        assertEquals(5, line.Count)
                        lineCount += 1
                    Loop
                    assertFalse(rr.hasNext())
                    assertEquals(150, lineCount)
                    rr.reset()
                Next i
            End Using
        End Sub

        <TestMethod>
        Public Sub testResetWithSkipLines()
            Using rr As New CSVRecordReader(10, ","c)
                rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))
                Dim lineCount As Integer = 0
                Do While rr.hasNext()
                    rr.next()
                    lineCount += 1
                Loop
                assertEquals(140, lineCount)
                rr.reset()
                lineCount = 0
                Do While rr.hasNext()
                    rr.next()
                    lineCount += 1
                Loop
                assertEquals(140, lineCount)
            End Using
        End Sub

        <TestMethod>
        Public Sub testWrite()
            Dim list As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim sb As New StringBuilder()
            For i As Integer = 0 To 9
                Dim temp As IList(Of WritableT) = New List(Of WritableT)()
                For j As Integer = 0 To 2
                    Dim v As Integer = 100 * i + j
                    temp.Add(New IntWritable(v))
                    sb.Append(v)
                    If j < 2 Then
                        sb.Append(",")
                    ElseIf i <> 9 Then
                        sb.Append(vbLf)
                    End If
                Next j
                list.Add(temp)
            Next i
            Dim expected As String = sb.ToString()

            Dim p As String = Path.Combine(tmpDir, "csvwritetest.csv")
            File.WriteAllText(p, "")
            Dim writer As FileRecordWriter = New CSVRecordWriter()
            Dim fileSplit As New FileSplit(p)
            writer.initialize(fileSplit, New NumberOfRecordsPartitioner())
            For Each c As IList(Of WritableT) In list
                writer.write(c)
            Next c
            writer.close()

            'Read file back in; compare
            Dim fileContents As String = File.ReadAllText(p, FileRecordWriter.DEFAULT_CHARSET)
            assertEquals(expected, fileContents)
        End Sub

        <TestMethod>
        Public Sub testTabsAsSplit1()
            Using reader As New CSVRecordReader(0, ControlChars.Tab)
                reader.initialize(New FileSplit(New ClassPathResource("datavec-api/tabbed.txt").File))
                Dim count As Integer = 0
                Do While reader.hasNext()
                    Dim list As IList(Of WritableT) = New List(Of WritableT)(reader.next())
                    assertEquals(2, list.Count)
                    count += 1
                Loop
                assertEquals(5, count)
            End Using
        End Sub

        <TestMethod>
        Public Sub testPipesAsSplit()
            Using reader As New CSVRecordReader(0, "|"c)
                reader.initialize(New FileSplit(New ClassPathResource("datavec-api/issue414.csv").File))
                Dim lineidx As Integer = 0
                Dim sixthColumn As IList(Of Integer) = Arrays.asList(13, 95, 15, 25)
                Do While reader.hasNext()
                    Dim list As IList(Of WritableT) = New List(Of WritableT)(reader.next())
                    assertEquals(10, list.Count)
                    assertEquals(sixthColumn(lineidx), list(5).toInt())
                    lineidx += 1
                Loop
                assertEquals(4, lineidx)
            End Using
        End Sub

        <TestMethod>
        Public Sub testWithQuotes()
            Dim reader As New CSVRecordReader(0, ","c, """"c)
            reader.initialize(New StringSplit("1,0,3,""Braund, Mr. Owen Harris"",male,"""""""""))
            Dim count As Integer = 0
            Do While reader.hasNext()
                Dim vals As IList(Of WritableT) = reader.next()
                assertEquals(6, vals.Count, "Entry count")
                assertEquals("1", vals(0).ToString())
                assertEquals("0", vals(1).ToString())
                assertEquals("3", vals(2).ToString())
                assertEquals("Braund, Mr. Owen Harris", vals(3).ToString())
                assertEquals("male", vals(4).ToString())
                assertEquals("""", vals(5).ToString())
                count += 1
            Loop
            assertEquals(1, count)
        End Sub

        <TestMethod>
        Public Sub testMeta()
            Using rr As New CSVRecordReader(0, ","c)
                rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

                Dim lineCount As Integer = 0
                Dim metaList As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
                Dim writables As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                Do While rr.hasNext()
                    Dim r As Record = rr.nextRecord()
                    assertEquals(5, r.getRecord().Count)
                    lineCount += 1
                    Dim meta As RecordMetaData = r.MetaData
                    metaList.Add(meta)
                    writables.Add(r.getRecord())
                Loop
                assertFalse(rr.hasNext())
                assertEquals(150, lineCount)
                rr.reset()

                Dim contents As IList(Of Record) = rr.loadFromMetaData(metaList)
                assertEquals(150, contents.Count)
                For i As Integer = 0 To 149
                    assertEquals(writables(i), contents(i).getRecord())
                    assertEquals(metaList(i), contents(i).getMetaData())
                Next i

                Dim meta2 As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
                meta2.Add(metaList(100))
                meta2.Add(metaList(90))
                meta2.Add(metaList(80))
                meta2.Add(metaList(70))
                meta2.Add(metaList(60))

                Dim contents2 As IList(Of Record) = rr.loadFromMetaData(meta2)
                assertEquals(writables(100), contents2(0).getRecord())
                assertEquals(writables(90), contents2(1).getRecord())
                assertEquals(writables(80), contents2(2).getRecord())
                assertEquals(writables(70), contents2(3).getRecord())
                assertEquals(writables(60), contents2(4).getRecord())
            End Using
        End Sub

        <TestMethod>
        Public Sub testRegex()
            Dim reader As CSVRecordReader = New CSVRegexRecordReader(0, ",", Nothing, New String() {Nothing, "(.+) (.+) (.+)"})
            reader.initialize(New StringSplit("normal,1.2.3.4 space separator"))
            Dim count As Integer = 0
            Do While reader.hasNext()
                Dim vals As IList(Of WritableT) = reader.next()
                assertEquals(4, vals.Count, "Entry count")
                assertEquals("normal", vals(0).ToString())
                assertEquals("1.2.3.4", vals(1).ToString())
                assertEquals("space", vals(2).ToString())
                assertEquals("separator", vals(3).ToString())
                count += 1
            Loop
            assertEquals(1, count)
        End Sub

        ''' <summary>Java: NoSuchElementException; the port throws InvalidOperationException.</summary>
        <TestMethod>
        Public Sub testCsvSkipAllLines()
            Const numLines As Integer = 4
            Dim header As String = ",one,two,three"
            Dim lines As IList(Of String) = New List(Of String)()
            For i As Integer = 0 To numLines - 1
                lines.Add(Convert.ToString(i) & header)
            Next i
            Dim tempFile As String = Path.Combine(tmpDir, "csvSkipLines.csv")
            File.WriteAllLines(tempFile, lines)

            Using rr As New CSVRecordReader(numLines, ","c)
                rr.initialize(New FileSplit(tempFile))
                rr.reset()
                assertTrue(Not rr.hasNext())
                assertThrows(Of InvalidOperationException)(Sub() rr.next())
            End Using
        End Sub

        <TestMethod>
        Public Sub testCsvSkipAllButOneLine()
            Const numLines As Integer = 4
            Dim lineList As IList(Of WritableT) = Arrays.asList(Of WritableT)(New Text(Convert.ToString(numLines - 1)), New Text("one"), New Text("two"), New Text("three"))
            Dim header As String = ",one,two,three"
            Dim lines As IList(Of String) = New List(Of String)()
            For i As Integer = 0 To numLines - 1
                lines.Add(Convert.ToString(i) & header)
            Next i
            Dim tempFile As String = Path.Combine(tmpDir, "csvSkipLines.csv")
            File.WriteAllLines(tempFile, lines)

            Using rr As New CSVRecordReader(numLines - 1, ","c)
                rr.initialize(New FileSplit(tempFile))
                rr.reset()
                assertTrue(rr.hasNext())
                assertEquals(lineList, rr.next())
                assertFalse(rr.hasNext())
            End Using
        End Sub

        <TestMethod>
        Public Sub testStreamReset()
            Using rr As New CSVRecordReader(0, ","c)
                rr.initialize(New InputStreamInputSplit(New ClassPathResource("datavec-api/iris.dat").InputStream))

                Dim count As Integer = 0
                Do While rr.hasNext()
                    assertNotNull(rr.next())
                    count += 1
                Loop
                assertEquals(150, count)

                assertFalse(rr.resetSupported())

                Try
                    rr.reset()
                    fail("Expected exception")
                Catch e As Exception
                    Dim msg As String = e.Message
                    Dim msg2 As String = e.InnerException.Message
                    assertTrue(msg.Contains("Error during LineRecordReader reset"), msg)
                    assertTrue(msg2.Contains("Reset not supported from streams"), msg2)
                End Try
            End Using
        End Sub

        <TestMethod>
        Public Sub testUsefulExceptionNoInit()
            Dim rr As New CSVRecordReader(0, ","c)

            Try
                rr.hasNext()
                fail("Expected exception")
            Catch e As Exception
                assertTrue(e.Message.Contains("initialized"), e.Message)
            End Try

            Try
                rr.next()
                fail("Expected exception")
            Catch e As Exception
                assertTrue(e.Message.Contains("initialized"), e.Message)
            End Try
        End Sub

        ''' <summary>A header line is skipped once per file, not once per split (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testSkipLinesPerFile()
            Dim a As String = Path.Combine(tmpDir, "a.csv")
            Dim b As String = Path.Combine(tmpDir, "b.csv")
            File.WriteAllText(a, "h1,h2" & vbLf & "1,2" & vbLf & "3,4")
            File.WriteAllText(b, "h1,h2" & vbLf & "5,6")
            Using rr As New CSVRecordReader(1, ","c)
                rr.initialize(New FileSplit(tmpDir))
                Dim all As New List(Of String)()
                Do While rr.hasNext()
                    Dim r As IList(Of WritableT) = rr.next()
                    assertEquals(2, r.Count)
                    all.Add(r(0).ToString() & r(1).ToString())
                Loop
                all.Sort(StringComparer.Ordinal)
                assertEquals(Arrays.asList("12", "34", "56"), all)
            End Using
        End Sub

        ''' <summary>Batches via next(n) (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testBatches()
            Using rr As New CSVRecordReader(0, ","c)
                rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))
                assertTrue(rr.batchesSupported())
                Dim first As IList(Of IList(Of WritableT)) = rr.next(100)
                assertEquals(100, first.Count)
                assertEquals(5, first(0).Count)
                assertEquals("5.1", first(0)(0).ToString())
                Dim rest As IList(Of IList(Of WritableT)) = rr.next(100)
                assertEquals(50, rest.Count)
                assertFalse(rr.hasNext())
            End Using
        End Sub

    End Class

End Namespace
