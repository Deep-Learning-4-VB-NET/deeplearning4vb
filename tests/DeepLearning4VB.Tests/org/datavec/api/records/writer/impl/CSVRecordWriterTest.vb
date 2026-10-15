Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.FileRecordWriter
Imports CSVRecordWriter = DeepLearning4VB.org.datavec.api.records.writer.impl.csv.CSVRecordWriter
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports NumberOfRecordsPartitioner = DeepLearning4VB.org.datavec.api.split.partition.NumberOfRecordsPartitioner
Imports PartitionMetaData = DeepLearning4VB.org.datavec.api.split.partition.PartitionMetaData
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.writer.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class CSVRecordWriterTest

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
        Public Sub testWrite()
            Dim tempFile As String = Path.Combine(tmpDir, "datavec.writer")
            File.WriteAllText(tempFile, "")
            Dim fileSplit As New FileSplit(tempFile)

            Dim writer As New CSVRecordWriter()
            writer.initialize(fileSplit, New NumberOfRecordsPartitioner())
            Dim collection As IList(Of WritableT) = New List(Of WritableT)()
            collection.Add(New Text("12"))
            collection.Add(New Text("13"))
            collection.Add(New Text("14"))

            Dim meta As PartitionMetaData = writer.write(collection)
            assertEquals(1, meta.getNumRecordsUpdated())
            'the writer holds the file open until it is closed
            writer.close()

            Using reader As New CSVRecordReader(0)
                reader.initialize(New FileSplit(tempFile))
                Dim cnt As Integer = 0
                Do While reader.hasNext()
                    Dim line As IList(Of WritableT) = New List(Of WritableT)(reader.next())
                    assertEquals(3, line.Count)

                    assertEquals(12, line(0).toInt())
                    assertEquals(13, line(1).toInt())
                    assertEquals(14, line(2).toInt())
                    cnt += 1
                Loop
                assertEquals(1, cnt)
            End Using
        End Sub

        ''' <summary>Batch writes, empty records and the text form (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testWriteBatch()
            Dim tempFile As String = Path.Combine(tmpDir, "batch.csv")
            Using writer As New CSVRecordWriter()
                assertTrue(writer.supportsBatch())
                writer.initialize(New FileSplit(tempFile), New NumberOfRecordsPartitioner())
                Dim batch As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                batch.Add(Arrays.asList(Of WritableT)(New IntWritable(1), New Text("a,b")))
                batch.Add(New List(Of WritableT)())
                batch.Add(Arrays.asList(Of WritableT)(New IntWritable(2), New Text("c")))
                Dim meta As PartitionMetaData = writer.writeBatch(batch)
                assertEquals(3, meta.getNumRecordsUpdated())
                writer.write(Arrays.asList(Of WritableT)(New IntWritable(3)))
            End Using
            'no new line after the last record, empty records are skipped, values are not quoted
            assertEquals("1,a,b" & vbLf & "2,c" & vbLf & "3", File.ReadAllText(tempFile, FileRecordWriter.DEFAULT_CHARSET))
        End Sub

    End Class

End Namespace
