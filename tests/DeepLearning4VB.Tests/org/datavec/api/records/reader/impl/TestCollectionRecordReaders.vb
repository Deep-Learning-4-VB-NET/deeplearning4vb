Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataIndex = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataIndex
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports CollectionRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.CollectionRecordReader
Imports CollectionSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.CollectionSequenceRecordReader
Imports ListStringRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.collection.ListStringRecordReader
Imports InMemoryRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.inmemory.InMemoryRecordReader
Imports InMemorySequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.inmemory.InMemorySequenceRecordReader
Imports ListStringSplit = DeepLearning4VB.org.datavec.api.split.ListStringSplit
Imports StringSplit = DeepLearning4VB.org.datavec.api.split.StringSplit
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name, plus the other in-memory readers.</summary>
    <TestClass>
    Public Class TestCollectionRecordReaders

        <TestMethod>
        Public Sub testCollectionSequenceRecordReader()
            Dim listOfSequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()

            Dim sequence1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            sequence1.Add(Arrays.asList(Of WritableT)(New IntWritable(0), New IntWritable(1)))
            sequence1.Add(Arrays.asList(Of WritableT)(New IntWritable(2), New IntWritable(3)))
            listOfSequences.Add(sequence1)

            Dim sequence2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            sequence2.Add(Arrays.asList(Of WritableT)(New IntWritable(4), New IntWritable(5)))
            sequence2.Add(Arrays.asList(Of WritableT)(New IntWritable(6), New IntWritable(7)))
            listOfSequences.Add(sequence2)

            Dim seqRR As SequenceRecordReader = New CollectionSequenceRecordReader(listOfSequences)
            assertTrue(seqRR.hasNext())

            assertEquals(sequence1, seqRR.sequenceRecord())
            assertEquals(sequence2, seqRR.sequenceRecord())
            assertFalse(seqRR.hasNext())

            seqRR.reset()
            assertEquals(sequence1, seqRR.sequenceRecord())
            assertEquals(sequence2, seqRR.sequenceRecord())
            assertFalse(seqRR.hasNext())

            'Test metadata:
            seqRR.reset()
            Dim out2 As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Dim seq As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
            Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
            Do While seqRR.hasNext()
                Dim r As SequenceRecord = seqRR.nextSequence()
                out2.Add(r.getSequenceRecord())
                seq.Add(r)
                meta.Add(r.MetaData)
            Loop

            assertEquals(listOfSequences, out2)
            assertEquals(0L, DirectCast(meta(0), RecordMetaDataIndex).getIndex())
            assertEquals(1L, DirectCast(meta(1), RecordMetaDataIndex).getIndex())

            Dim fromMeta As IList(Of SequenceRecord) = seqRR.loadSequenceFromMetaData(meta)
            assertEquals(seq, fromMeta)
            assertEquals(seq(1), seqRR.loadSequenceFromMetaData(meta(1)))

            assertThrows(Of NotSupportedException)(Sub() seqRR.next())
            assertThrows(Of NotSupportedException)(Sub() seqRR.loadFromMetaData(meta))
        End Sub

        ''' <summary>The (non-sequence) CollectionRecordReader (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testCollectionRecordReader()
            Dim records As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            records.Add(Arrays.asList(Of WritableT)(New IntWritable(0), New Text("a")))
            records.Add(Arrays.asList(Of WritableT)(New IntWritable(1), New Text("b")))
            records.Add(Arrays.asList(Of WritableT)(New IntWritable(2), New Text("c")))

            Dim rr As RecordReader = New CollectionRecordReader(records)
            rr.initialize(New StringSplit("ignored"))
            assertTrue(rr.hasNext())
            assertEquals(records(0), rr.next())
            assertEquals(records(1), rr.next())
            assertEquals(records(2), rr.next())
            assertFalse(rr.hasNext())
            assertThrows(Of InvalidOperationException)(Sub() rr.next())

            rr.reset()
            Dim out2 As IList(Of Record) = New List(Of Record)()
            Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
            Do While rr.hasNext()
                Dim r As Record = rr.nextRecord()
                out2.Add(r)
                meta.Add(r.MetaData)
            Loop
            assertEquals(3, out2.Count)
            assertEquals(2L, DirectCast(meta(2), RecordMetaDataIndex).getIndex())
            assertEquals(GetType(CollectionRecordReader), meta(2).getReaderClass())
            assertEquals(out2, rr.loadFromMetaData(meta))
            assertEquals(out2(1), rr.loadFromMetaData(meta(1)))
            'a subset keeps the requested records
            Dim subset As IList(Of Record) = rr.loadFromMetaData(Arrays.asList(meta(2), meta(0)))
            assertEquals(2, subset.Count)
            assertTrue(subset.Contains(out2(0)))
            assertTrue(subset.Contains(out2(2)))
            assertThrows(Of InvalidOperationException)(Sub() rr.loadFromMetaData(New RecordMetaDataIndex(7L, Nothing, Nothing)))
            assertThrows(Of NotSupportedException)(Sub() rr.record(Nothing, Nothing))
            assertNull(rr.Labels)
            assertTrue(rr.resetSupported())
        End Sub

        ''' <summary>ListStringRecordReader over a ListStringSplit (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testListStringRecordReader()
            Dim data As IList(Of IList(Of String)) = New List(Of IList(Of String))()
            data.Add(Arrays.asList("a", "b"))
            data.Add(Arrays.asList("c"))

            Dim rr As New ListStringRecordReader()
            rr.initialize(New ListStringSplit(data))
            assertTrue(rr.hasNext())
            assertEquals(Arrays.asList(Of WritableT)(New Text("a"), New Text("b")), rr.next())
            Dim r As Record = rr.nextRecord()
            assertEquals(Arrays.asList(Of WritableT)(New Text("c")), r.getRecord())
            assertNull(r.getMetaData())
            assertFalse(rr.hasNext())
            rr.reset()
            assertEquals(2, rr.next().Count)
            assertThrows(Of ArgumentException)(Sub() rr.initialize(New StringSplit("not a list split")))
            assertThrows(Of NotSupportedException)(Sub() rr.loadFromMetaData(New List(Of RecordMetaData)()))
        End Sub

        ''' <summary>The in-memory readers (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testInMemoryRecordReaders()
            Dim records As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            records.Add(Arrays.asList(Of WritableT)(New IntWritable(1)))
            records.Add(Arrays.asList(Of WritableT)(New IntWritable(2)))

            Dim rr As New InMemoryRecordReader(records)
            assertTrue(rr.hasNext())
            assertEquals(records(0), rr.next())
            assertEquals(records(1), rr.nextRecord().getRecord())
            assertFalse(rr.hasNext())
            rr.reset()
            assertEquals(records(0), rr.next())
            rr.setLabels(Arrays.asList("one", "two"))
            assertEquals(Arrays.asList("one", "two"), rr.Labels)
            assertEquals(New InMemoryRecordReader(records), New InMemoryRecordReader(records))
            assertTrue(rr.ToString().StartsWith("InMemoryRecordReader(records=[[1], [2]]", StringComparison.Ordinal))
            assertThrows(Of NotSupportedException)(Sub() rr.loadFromMetaData(New List(Of RecordMetaData)()))

            Dim sequences As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            sequences.Add(records)
            Dim srr As New InMemorySequenceRecordReader(sequences)
            assertTrue(srr.hasNext())
            assertEquals(records, srr.sequenceRecord())
            assertFalse(srr.hasNext())
            srr.reset()
            Dim sr As SequenceRecord = srr.nextSequence()
            assertEquals(records, sr.getSequenceRecord())
            assertNull(sr.getMetaData())
            assertEquals(New InMemorySequenceRecordReader(sequences), srr)
            assertThrows(Of NotSupportedException)(Sub() srr.next())
        End Sub

    End Class

End Namespace
