Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLine
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports RegexLineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.regex.RegexLineRecordReader
Imports RegexSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.regex.RegexSequenceRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports NumberedFileInputSplit = DeepLearning4VB.org.datavec.api.split.NumberedFileInputSplit
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4j.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class RegexRecordReaderTest

        Private Const REGEX As String = "(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}) (\d+) ([A-Z]+) (.*)"

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

        Private Shared Function line(ByVal ParamArray values() As String) As IList(Of WritableT)
            Dim result As IList(Of WritableT) = New List(Of WritableT)()
            For Each s As String In values
                result.Add(New Text(s))
            Next s
            Return result
        End Function

        <TestMethod>
        Public Sub testRegexLineRecordReader()
            Using rr As RecordReader = New RegexLineRecordReader(REGEX, 1)
                rr.initialize(New FileSplit(New ClassPathResource("datavec-api/logtestdata/logtestfile0.txt").File))

                Dim exp0 As IList(Of WritableT) = line("2016-01-01 23:59:59.001", "1", "DEBUG", "First entry message!")
                Dim exp1 As IList(Of WritableT) = line("2016-01-01 23:59:59.002", "2", "INFO", "Second entry message!")
                Dim exp2 As IList(Of WritableT) = line("2016-01-01 23:59:59.003", "3", "WARN", "Third entry message!")
                assertEquals(exp0, rr.next())
                assertEquals(exp1, rr.next())
                assertEquals(exp2, rr.next())
                assertFalse(rr.hasNext())

                'Test reset:
                rr.reset()
                assertEquals(exp0, rr.next())
                assertEquals(exp1, rr.next())
                assertEquals(exp2, rr.next())
                assertFalse(rr.hasNext())
            End Using
        End Sub

        <TestMethod>
        Public Sub testRegexLineRecordReaderMeta()
            Using rr As RecordReader = New RegexLineRecordReader(REGEX, 1)
                rr.initialize(New FileSplit(New ClassPathResource("datavec-api/logtestdata/logtestfile0.txt").File))

                Dim list As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                Do While rr.hasNext()
                    list.Add(rr.next())
                Loop
                assertEquals(3, list.Count)

                Dim list2 As IList(Of Record) = New List(Of Record)()
                Dim list3 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
                rr.reset()
                'Start by skipping 1 line
                Dim count As Integer = 1
                Do While rr.hasNext()
                    Dim r As Record = rr.nextRecord()
                    list2.Add(r)
                    list3.Add(r.getRecord())
                    meta.Add(r.MetaData)
                    assertEquals(count, DirectCast(r.MetaData, RecordMetaDataLine).getLineNumber())
                    count += 1
                Loop

                Dim fromMeta As IList(Of Record) = rr.loadFromMetaData(meta)

                assertEquals(list, list3)
                assertEquals(list2, fromMeta)
            End Using
        End Sub

        <TestMethod>
        Public Sub testRegexSequenceRecordReader()
            Dim cpr As New ClassPathResource("datavec-api/logtestdata/")
            cpr.copyDirectory(tmpDir)
            Dim path As String = System.IO.Path.Combine(tmpDir, "logtestfile%d.txt")

            Dim [is] As InputSplit = New NumberedFileInputSplit(path, 0, 1)

            Dim rr As SequenceRecordReader = New RegexSequenceRecordReader(REGEX, 1)
            rr.initialize([is])

            Dim exp0 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp0.Add(line("2016-01-01 23:59:59.001", "1", "DEBUG", "First entry message!"))
            exp0.Add(line("2016-01-01 23:59:59.002", "2", "INFO", "Second entry message!"))
            exp0.Add(line("2016-01-01 23:59:59.003", "3", "WARN", "Third entry message!"))

            Dim exp1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            exp1.Add(line("2016-01-01 23:59:59.011", "11", "DEBUG", "First entry message!"))
            exp1.Add(line("2016-01-01 23:59:59.012", "12", "INFO", "Second entry message!"))
            exp1.Add(line("2016-01-01 23:59:59.013", "13", "WARN", "Third entry message!"))

            assertEquals(exp0, rr.sequenceRecord())
            assertEquals(exp1, rr.sequenceRecord())
            assertFalse(rr.hasNext())

            'Test resetting:
            rr.reset()
            assertEquals(exp0, rr.sequenceRecord())
            assertEquals(exp1, rr.sequenceRecord())
            assertFalse(rr.hasNext())
        End Sub

        <TestMethod>
        Public Sub testRegexSequenceRecordReaderMeta()
            Dim cpr As New ClassPathResource("datavec-api/logtestdata/")
            cpr.copyDirectory(tmpDir)
            Dim path As String = System.IO.Path.Combine(tmpDir, "logtestfile%d.txt")

            Dim [is] As InputSplit = New NumberedFileInputSplit(path, 0, 1)

            Dim rr As SequenceRecordReader = New RegexSequenceRecordReader(REGEX, 1)
            rr.initialize([is])

            Dim [out] As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Do While rr.hasNext()
                [out].Add(rr.sequenceRecord())
            Loop

            assertEquals(2, [out].Count)
            Dim out2 As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Dim out3 As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
            Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
            rr.reset()
            Do While rr.hasNext()
                Dim seqr As SequenceRecord = rr.nextSequence()
                out2.Add(seqr.getSequenceRecord())
                out3.Add(seqr)
                meta.Add(seqr.MetaData)
            Loop

            Dim fromMeta As IList(Of SequenceRecord) = rr.loadSequenceFromMetaData(meta)

            assertEquals([out], out2)
            assertEquals(out3, fromMeta)
        End Sub

        ''' <summary>Invalid lines: failing, skipping, and the single-record stream path (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testInvalidLineHandling()
            Dim f As String = System.IO.Path.Combine(tmpDir, "mixed.txt")
            File.WriteAllText(f, "2016-01-01 23:59:59.001 1 DEBUG ok" & vbLf & "this line does not match" & vbLf & "2016-01-01 23:59:59.002 2 INFO ok too", New UTF8Encoding(False))

            Dim failing As New RegexSequenceRecordReader(REGEX, 0)
            failing.initialize(New FileSplit(f))
            assertThrows(Of InvalidOperationException)(Sub() failing.sequenceRecord())

            Dim skipping As New RegexSequenceRecordReader(REGEX, 0, Encoding.UTF8, RegexSequenceRecordReader.LineErrorHandling.SkipInvalid)
            skipping.initialize(New FileSplit(f))
            Dim seq As IList(Of IList(Of WritableT)) = skipping.sequenceRecord()
            assertEquals(2, seq.Count)
            assertEquals("ok too", seq(1)(3).ToString())

            Dim warning As New RegexSequenceRecordReader(REGEX, 0, Encoding.UTF8, RegexSequenceRecordReader.LineErrorHandling.SkipInvalidWithWarning)
            warning.initialize(New FileSplit(f))
            assertEquals(seq, warning.sequenceRecord())

            'a line reader on the same file fails on the second line
            Using lineReader As New RegexLineRecordReader(REGEX, 0)
                lineReader.initialize(New FileSplit(f))
                assertEquals(line("2016-01-01 23:59:59.001", "1", "DEBUG", "ok"), lineReader.next())
                assertThrows(Of InvalidOperationException)(Sub() lineReader.next())
            End Using

            'a single record from a stream
            Dim fromStream As New RegexLineRecordReader(REGEX, 0)
            Using dis As New DataInputStream(New MemoryStream(Encoding.UTF8.GetBytes("2016-01-01 23:59:59.003 3 WARN streamed")))
                assertEquals(line("2016-01-01 23:59:59.003", "3", "WARN", "streamed"), fromStream.record(New Uri(f), dis))
            End Using
        End Sub

    End Class

End Namespace
