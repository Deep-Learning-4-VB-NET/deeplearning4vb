Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports RecordListener = DeepLearning4VB.org.datavec.api.records.listener.RecordListener
Imports LogRecordListener = DeepLearning4VB.org.datavec.api.records.listener.impl.LogRecordListener
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports FileRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.FileRecordReader
Imports CollectionRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports CollectionSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.CollectionSequenceRecordReader
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports InMemoryRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.inmemory.InMemoryRecordReader
Imports RecordWriter = DeepLearning4VB.org.datavec.api.records.writer.RecordWriter
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports StringSplit = DeepLearning4VB.org.datavec.api.split.StringSplit
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.listener

    ''' <summary>The record listeners: <see cref="LogRecordListener"/> and the listener hooks of the readers.</summary>
    <TestClass>
    Public Class RecordListenersTest

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

        ''' <summary>Records every notification.</summary>
        Private Class CountingListener
            Implements RecordListener

            Public ReadOnly readRecords As New List(Of Object)()
            Public ReadOnly readers As New List(Of RecordReader)()
            Public ReadOnly written As New List(Of Object)()
            Private invokedFlag As Boolean

            Public Function invoked() As Boolean Implements RecordListener.invoked
                Return invokedFlag
            End Function

            Public Sub invoke() Implements RecordListener.invoke
                invokedFlag = True
            End Sub

            Public Sub recordRead(ByVal reader As RecordReader, ByVal record As Object) Implements RecordListener.recordRead
                invoke()
                readers.Add(reader)
                readRecords.Add(record)
            End Sub

            Public Sub recordWrite(ByVal writer As RecordWriter, ByVal record As Object) Implements RecordListener.recordWrite
                invoke()
                written.Add(record)
            End Sub
        End Class

        <TestMethod>
        Public Sub testLogRecordListener()
            Dim listener As New LogRecordListener()
            assertFalse(listener.invoked())
            listener.recordRead(Nothing, "a record")
            assertTrue(listener.invoked())

            Dim other As New LogRecordListener()
            other.recordWrite(Nothing, Arrays.asList(Of WritableT)(New IntWritable(1)))
            assertTrue(other.invoked())

            Dim third As New LogRecordListener()
            third.invoke()
            assertTrue(third.invoked())
        End Sub

        <TestMethod>
        Public Sub testLineReaderNotifiesWithTheLine()
            Dim f As String = Path.Combine(tmpDir, "lines.csv")
            File.WriteAllText(f, "1,2" & vbLf & "3,4" & vbLf & "5,6", New UTF8Encoding(False))

            Dim listener As New CountingListener()
            Using rr As New CSVRecordReader()
                rr.setListeners(listener)
                assertEquals(1, rr.Listeners.Count)
                assertTrue(rr.Listeners(0) Is listener)
                rr.initialize(New FileSplit(f))
                Dim count As Integer = 0
                Do While rr.hasNext()
                    rr.next()
                    count += 1
                Loop
                assertEquals(3, count)
            End Using
            assertTrue(listener.invoked())
            'the line readers pass the raw line
            assertEquals(Arrays.asList(Of Object)("1,2", "3,4", "5,6"), listener.readRecords)
            assertEquals(3, listener.readers.Count)
            assertTrue(TypeOf listener.readers(0) Is CSVRecordReader)
        End Sub

        <TestMethod>
        Public Sub testCollectionReadersNotifyWithTheRecord()
            Dim records As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            records.Add(Arrays.asList(Of WritableT)(New IntWritable(1)))
            records.Add(Arrays.asList(Of WritableT)(New IntWritable(2)))

            Dim listener As New CountingListener()
            Dim rr As New CollectionRecordReader(records)
            rr.setListeners(DirectCast(New List(Of RecordListener)(New RecordListener() {listener}), ICollection(Of RecordListener)))
            rr.initialize(New StringSplit("ignored"))
            assertEquals(records(0), rr.next())
            assertEquals(records(1), rr.nextRecord().getRecord())
            assertEquals(Arrays.asList(Of Object)(records(0), records(1)), listener.readRecords)

            'the sequence reader passes the whole sequence
            Dim seqListener As New CountingListener()
            Dim sequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            sequences.Add(records)
            Dim srr As New CollectionSequenceRecordReader(sequences)
            srr.Listeners = Arrays.asList(Of RecordListener)(seqListener)
            srr.sequenceRecord()
            assertEquals(1, seqListener.readRecords.Count)
            assertEquals(records, seqListener.readRecords(0))

            'the in-memory reader keeps its listeners but does not notify them
            Dim quiet As New CountingListener()
            Dim mem As New InMemoryRecordReader(records)
            mem.setListeners(quiet)
            assertEquals(Arrays.asList(Of RecordListener)(quiet), mem.Listeners)
            mem.next()
            assertFalse(quiet.invoked())
        End Sub

        <TestMethod>
        Public Sub testFileReaderNotifiesWithTheUri()
            Dim f As String = Path.Combine(tmpDir, "whole.txt")
            File.WriteAllText(f, "content", New UTF8Encoding(False))

            Dim listener As New CountingListener()
            Dim rr As New FileRecordReader()
            rr.setListeners(listener)
            rr.initialize(New FileSplit(f))
            Dim r = rr.nextRecord()
            assertEquals(1, listener.readRecords.Count)
            assertEquals(r.getMetaData().URI, listener.readRecords(0))

            'replacing the listeners
            Dim second As New CountingListener()
            rr.Listeners = Arrays.asList(Of RecordListener)(second)
            rr.reset()
            rr.next()
            assertEquals(1, listener.readRecords.Count)
            assertEquals(1, second.readRecords.Count)
        End Sub

    End Class

End Namespace
