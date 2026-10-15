Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports RecordMetaDataLineInterval = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLineInterval
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVVariableSlidingWindowRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVVariableSlidingWindowRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4j.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class CSVVariableSlidingWindowRecordReaderTest

        <TestMethod>
        Public Sub testCSVVariableSlidingWindowRecordReader()
            Dim maxLinesPerSequence As Integer = 3

            Dim seqRR As SequenceRecordReader = New CSVVariableSlidingWindowRecordReader(maxLinesPerSequence)
            seqRR.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Dim rr As New CSVRecordReader()
            rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Dim count As Integer = 0
            Do While seqRR.hasNext()
                Dim [next] As IList(Of IList(Of WritableT)) = seqRR.sequenceRecord()

                If count = maxLinesPerSequence - 1 Then
                    Dim expected As New LinkedList(Of IList(Of WritableT))()
                    For i As Integer = 0 To maxLinesPerSequence - 1
                        expected.AddFirst(rr.next())
                    Next i
                    assertEquals(expected, [next])
                End If
                If count = maxLinesPerSequence Then
                    assertEquals(maxLinesPerSequence, [next].Count)
                End If
                If count = 0 Then
                    ' first seq should be length 1
                    assertEquals(1, [next].Count)
                End If
                If count > 150 Then
                    ' last seq should be length 1
                    assertEquals(1, [next].Count)
                End If

                count += 1
            Loop

            assertEquals(152, count)
            seqRR.Dispose()
            rr.close()
        End Sub

        <TestMethod>
        Public Sub testCSVVariableSlidingWindowRecordReaderStride()
            Dim maxLinesPerSequence As Integer = 3
            Dim stride As Integer = 2

            Dim seqRR As SequenceRecordReader = New CSVVariableSlidingWindowRecordReader(maxLinesPerSequence, stride)
            seqRR.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Dim rr As New CSVRecordReader()
            rr.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))

            Dim count As Integer = 0
            Do While seqRR.hasNext()
                Dim [next] As IList(Of IList(Of WritableT)) = seqRR.sequenceRecord()

                If count = maxLinesPerSequence - 1 Then
                    Dim expected As New LinkedList(Of IList(Of WritableT))()
                    For s As Integer = 0 To stride - 1
                        expected = New LinkedList(Of IList(Of WritableT))()
                        For i As Integer = 0 To maxLinesPerSequence - 1
                            expected.AddFirst(rr.next())
                        Next i
                    Next s
                    assertEquals(expected, [next])
                End If
                If count = maxLinesPerSequence Then
                    assertEquals(maxLinesPerSequence, [next].Count)
                End If
                If count = 0 Then
                    ' first seq should be length 2
                    assertEquals(2, [next].Count)
                End If
                If count > 74 Then
                    ' last seq should be length 1
                    assertEquals(1, [next].Count)
                End If

                count += 1
            Loop

            assertEquals(76, count)
            seqRR.Dispose()
            rr.close()
        End Sub

        ''' <summary>Reset and the line-interval metadata of each window (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testResetAndMetaData()
            Dim seqRR As New CSVVariableSlidingWindowRecordReader(3)
            seqRR.initialize(New FileSplit(New ClassPathResource("datavec-api/iris.dat").File))
            Dim first As IList(Of IList(Of WritableT)) = seqRR.sequenceRecord()
            assertEquals(1, first.Count)
            assertEquals("5.1", first(0)(0).ToString())

            seqRR.reset()
            Dim sr As SequenceRecord = seqRR.nextSequence()
            assertEquals(first, sr.getSequenceRecord())
            Dim meta As RecordMetaDataLineInterval = DirectCast(sr.getMetaData(), RecordMetaDataLineInterval)
            assertEquals(0, meta.getLineNumberStart())
            assertEquals(1, meta.getLineNumberEnd()) 'Java: end = lineIndex + queue.size() - 1 = 1 + 1 - 1
            assertEquals(GetType(CSVVariableSlidingWindowRecordReader), meta.getReaderClass())

            sr = seqRR.nextSequence()
            assertEquals(2, sr.getSequenceLength())
            'the window is ordered newest line first
            assertEquals("4.9", sr.getTimeStep(0)(0).ToString())
            assertEquals("5.1", sr.getTimeStep(1)(0).ToString())

            assertThrows(Of ArgumentException)(Sub()
                                                   Dim ignored As New CSVVariableSlidingWindowRecordReader(3, 0)
                                               End Sub)
            seqRR.close()
        End Sub

    End Class

End Namespace
