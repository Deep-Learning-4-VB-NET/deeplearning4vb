Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLineInterval = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLineInterval
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports CSVNLinesSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVNLinesSequenceRecordReader
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class CSVNLinesSequenceRecordReaderTest

        <TestMethod>
        Public Sub testCSVNLinesSequenceRecordReader()
            Dim nLinesPerSequence As Integer = 10

            Dim seqRR As SequenceRecordReader = New CSVNLinesSequenceRecordReader(nLinesPerSequence)
            seqRR.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Dim rr As New CSVRecordReader()
            rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Dim count As Integer = 0
            Do While seqRR.hasNext()
                Dim [next] As IList(Of IList(Of WritableT)) = seqRR.sequenceRecord()

                Dim expected As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                For i As Integer = 0 To nLinesPerSequence - 1
                    expected.Add(rr.next())
                Next i

                assertEquals(10, [next].Count)
                assertEquals(expected, [next])

                count += 1
            Loop

            assertEquals(150 \ nLinesPerSequence, count)
            seqRR.Dispose()
            rr.close()
        End Sub

        <TestMethod>
        Public Sub testCSVNlinesSequenceRecordReaderMetaData()
            Dim nLinesPerSequence As Integer = 10

            Dim seqRR As SequenceRecordReader = New CSVNLinesSequenceRecordReader(nLinesPerSequence)
            seqRR.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Dim [out] As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Do While seqRR.hasNext()
                Dim [next] As IList(Of IList(Of WritableT)) = seqRR.sequenceRecord()
                [out].Add([next])
            Loop

            seqRR.reset()
            Dim out2 As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Dim out3 As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
            Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
            Do While seqRR.hasNext()
                Dim seq As SequenceRecord = seqRR.nextSequence()
                out2.Add(seq.getSequenceRecord())
                meta.Add(seq.MetaData)
                out3.Add(seq)
            Loop

            assertEquals([out], out2)
            'each sequence covers ten consecutive lines
            For i As Integer = 0 To meta.Count - 1
                Dim interval As RecordMetaDataLineInterval = DirectCast(meta(i), RecordMetaDataLineInterval)
                assertEquals(10 * i, interval.getLineNumberStart())
                assertEquals(10 * i + 9, interval.getLineNumberEnd())
            Next i

            Dim out4 As IList(Of SequenceRecord) = seqRR.loadSequenceFromMetaData(meta)
            assertEquals(out3, out4)

            'a subset, in a different order
            Dim subset As IList(Of SequenceRecord) = seqRR.loadSequenceFromMetaData(Arrays.asList(meta(14), meta(3)))
            assertEquals(2, subset.Count)
            assertEquals(out3(14), subset(0))
            assertEquals(out3(3), subset(1))
            seqRR.Dispose()
        End Sub

        ''' <summary>A trailing, shorter sequence and the unsupported record methods (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testPartialLastSequence()
            Dim seqRR As New CSVNLinesSequenceRecordReader(100)
            seqRR.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))
            assertEquals(100, seqRR.sequenceRecord().Count)
            assertEquals(50, seqRR.sequenceRecord().Count)
            assertFalse(seqRR.hasNext())
            assertThrows(Of InvalidOperationException)(Sub() seqRR.sequenceRecord())
            assertThrows(Of NotSupportedException)(Sub() seqRR.loadFromMetaData(New List(Of RecordMetaData)()))
            seqRR.close()
        End Sub

    End Class

End Namespace
