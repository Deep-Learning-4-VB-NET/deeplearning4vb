Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports FileBatchRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.filebatch.FileBatchRecordReader
Imports FileBatchSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.filebatch.FileBatchSequenceRecordReader
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports FileBatch = DeepLearning4VB.org.nd4vb.common.loader.FileBatch

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name (the Nd4vb backend parameter of the JUnit tests is not used by the tests).</summary>
    <TestClass>
    Public Class FileBatchRecordReaderTest

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
        Public Sub testCsv()
            'This is an unrealistic use case - one line/record per CSV
            Dim fileList As IList(Of String) = New List(Of String)()
            For i As Integer = 0 To 9
                Dim s As String = "file_" & i & "," & i & "," & i
                Dim f As String = Path.Combine(tmpDir, "origFile" & i & ".csv")
                File.WriteAllText(f, s, New UTF8Encoding(False))
                fileList.Add(f)
            Next i

            Dim fb As FileBatch = FileBatch.forFiles(fileList)

            Dim rr As RecordReader = New CSVRecordReader()
            Dim fbrr As New FileBatchRecordReader(rr, fb)

            For test As Integer = 0 To 2
                For i As Integer = 0 To 9
                    assertTrue(fbrr.hasNext())
                    Dim [next] As IList(Of WritableT) = fbrr.next()
                    assertEquals(3, [next].Count)
                    Dim s1 As String = "file_" & i
                    assertEquals(s1, [next](0).ToString())
                    assertEquals(i.ToString(), [next](1).ToString())
                    assertEquals(i.ToString(), [next](2).ToString())
                Next i
                assertFalse(fbrr.hasNext())
                assertTrue(fbrr.resetSupported())
                fbrr.reset()
            Next test

            'batches, records and the unsupported stream form
            assertEquals(4, fbrr.next(4).Count)
            assertEquals("file_4", fbrr.nextRecord().getRecord()(0).ToString())
            assertEquals(5, fbrr.next(100).Count)
            assertFalse(fbrr.hasNext())
            assertThrows(Of InvalidOperationException)(Sub() fbrr.next())
            assertThrows(Of NotSupportedException)(Sub() fbrr.record(Nothing, Nothing))
            assertNull(fbrr.Labels)
            fbrr.Dispose()
        End Sub

        <TestMethod>
        Public Sub testCsvSequence()
            'CSV sequence - 3 lines per file, 10 files
            Dim fileList As IList(Of String) = New List(Of String)()
            For i As Integer = 0 To 9
                Dim sb As New StringBuilder()
                For j As Integer = 0 To 2
                    If j > 0 Then
                        sb.Append(vbLf)
                    End If
                    sb.Append("file_" & i & "," & i & "," & j)
                Next j
                Dim f As String = Path.Combine(tmpDir, "origFile" & i & ".csv")
                File.WriteAllText(f, sb.ToString(), New UTF8Encoding(False))
                fileList.Add(f)
            Next i

            Dim fb As FileBatch = FileBatch.forFiles(fileList)
            Dim rr As SequenceRecordReader = New CSVSequenceRecordReader()
            Dim fbrr As New FileBatchSequenceRecordReader(rr, fb)

            For test As Integer = 0 To 2
                For i As Integer = 0 To 9
                    assertTrue(fbrr.hasNext())
                    Dim [next] As IList(Of IList(Of WritableT)) = fbrr.sequenceRecord()
                    assertEquals(3, [next].Count)
                    Dim count As Integer = 0
                    For Each [step] As IList(Of WritableT) In [next]
                        Dim s1 As String = "file_" & i
                        assertEquals(s1, [step](0).ToString())
                        assertEquals(i.ToString(), [step](1).ToString())
                        assertEquals(count.ToString(), [step](2).ToString())
                        count += 1
                    Next [step]
                Next i
                assertFalse(fbrr.hasNext())
                assertTrue(fbrr.resetSupported())
                fbrr.reset()
            Next test

            'a sequence record carries no metadata; the record methods are unsupported
            Dim sr = fbrr.nextSequence()
            assertEquals(3, sr.getSequenceLength())
            assertNull(sr.getMetaData())
            assertThrows(Of NotSupportedException)(Sub() fbrr.next())
            assertThrows(Of NotSupportedException)(Sub() fbrr.nextRecord())
            fbrr.Dispose()
        End Sub

    End Class

End Namespace
