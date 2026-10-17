Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports FileRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.FileRecordReader
Imports CollectionInputSplit = DeepLearning4VB.org.datavec.api.split.CollectionInputSplit
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class FileRecordReaderTest

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
        Public Sub testReset()
            Dim rr As New FileRecordReader()
            rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Dim nResets As Integer = 5
            For i As Integer = 0 To nResets - 1
                Dim lineCount As Integer = 0
                Do While rr.hasNext()
                    Dim line As IList(Of WritableT) = rr.next()
                    assertEquals(1, line.Count)
                    'the whole file is one Text
                    assertTrue(line(0).ToString().StartsWith("5.1,3.5,1.4,0.2,0", StringComparison.Ordinal))
                    lineCount += 1
                Loop
                assertFalse(rr.hasNext())
                assertEquals(1, lineCount)
                rr.reset()
            Next i
        End Sub

        <TestMethod>
        Public Sub testMeta()
            Dim rr As New FileRecordReader()

            Dim arr(2) As Uri
            arr(0) = URIUtil.fileToURI(New ClassPathResource("datavec-api/csvsequence_0.txt").File)
            arr(1) = URIUtil.fileToURI(New ClassPathResource("datavec-api/csvsequence_1.txt").File)
            arr(2) = URIUtil.fileToURI(New ClassPathResource("datavec-api/csvsequence_2.txt").File)

            Dim [is] As InputSplit = New CollectionInputSplit(Arrays.asList(arr))
            rr.initialize([is])

            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Do While rr.hasNext()
                [out].Add(rr.next())
            Loop

            assertEquals(3, [out].Count)

            rr.reset()
            Dim out2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim out3 As IList(Of Record) = New List(Of Record)()
            Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
            Dim count As Integer = 0
            Do While rr.hasNext()
                Dim r As Record = rr.nextRecord()
                out2.Add(r.getRecord())
                out3.Add(r)
                meta.Add(r.MetaData)
                assertEquals(arr(count), r.MetaData.URI)
                count += 1
            Loop

            assertEquals([out], out2)

            Dim fromMeta As IList(Of Record) = rr.loadFromMetaData(meta)
            assertEquals(out3, fromMeta)
            assertEquals(out3(1), rr.loadFromMetaData(meta(1)))
        End Sub

        ''' <summary>Labels from the parent directory name, appended when configured (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testLabelsFromParentDirectory()
            Dim a As String = Path.Combine(tmpDir, "cats", "one.txt")
            Dim b As String = Path.Combine(tmpDir, "dogs", "two.txt")
            Directory.CreateDirectory(Path.GetDirectoryName(a))
            Directory.CreateDirectory(Path.GetDirectoryName(b))
            File.WriteAllText(a, "meow", New UTF8Encoding(False))
            File.WriteAllText(b, "woof", New UTF8Encoding(False))

            'initialize(conf, split) appends the label by default
            Dim rr As New FileRecordReader()
            rr.initialize(New Configuration(), New FileSplit(tmpDir))
            assertEquals(Arrays.asList("cats", "dogs"), rr.Labels)
            Dim seen As Integer = 0
            Do While rr.hasNext()
                Dim r As Record = rr.nextRecord()
                assertEquals(2, r.getRecord().Count)
                Dim content As String = r.getRecord()(0).ToString()
                Dim label As Integer = DirectCast(r.getRecord()(1), IntWritable).get()
                If content = "meow" Then
                    assertEquals(0, label)
                    assertEquals(0, rr.getLabel(r.MetaData.URI))
                Else
                    assertEquals("woof", content)
                    assertEquals(1, label)
                End If
                assertEquals(label, rr.getCurrentLabel())
                seen += 1
            Loop
            assertEquals(2, seen)
            assertTrue(rr.resetSupported())

            'without a configuration there are no labels
            Dim plain As New FileRecordReader()
            plain.initialize(New FileSplit(tmpDir))
            assertNull(plain.Labels)
            assertEquals(1, plain.next().Count)
            plain.setLabels(Arrays.asList("x"))
            assertEquals(Arrays.asList("x"), plain.Labels)

            'batches and the stream form
            Dim batched As New FileRecordReader()
            batched.initialize(New FileSplit(tmpDir))
            assertEquals(2, batched.next(5).Count)
            assertFalse(batched.hasNext())
            Using dis As New DataInputStream(New MemoryStream(Encoding.UTF8.GetBytes("line1" & vbLf & "line2")))
                assertEquals(JavaCollections.singletonList(Of WritableT)(New Text("line1" & vbLf & "line2" & vbLf)), batched.record(URIUtil.fileToURI(a), dis))
            End Using
        End Sub

    End Class

End Namespace
