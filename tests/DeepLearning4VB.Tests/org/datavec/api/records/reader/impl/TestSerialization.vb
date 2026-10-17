Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports LineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.LineRecordReader
Imports CSVLineSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVLineSequenceRecordReader
Imports CSVMultiSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVMultiSequenceRecordReader
Imports CSVNLinesSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVNLinesSequenceRecordReader
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVRegexRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRegexRecordReader
Imports CSVSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports CSVVariableSlidingWindowRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVVariableSlidingWindowRecordReader
Imports FieldSelection = DeepLearning4VB.org.datavec.api.records.reader.impl.jackson.FieldSelection
Imports JacksonLineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.jackson.JacksonLineRecordReader
Imports JacksonRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.jackson.JacksonRecordReader
Imports LibSvmRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.misc.LibSvmRecordReader
Imports SVMLightRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.misc.SVMLightRecordReader
Imports RegexLineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.regex.RegexLineRecordReader
Imports RegexSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.regex.RegexSequenceRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>
    ''' Ported from the DL4J JUnit test of the same name: Java serialization is SerializationUtils here.
    ''' The TransformProcessRecordReader / TransformProcessSequenceRecordReader entries are not ported
    ''' (they need TransformProcess).
    ''' </summary>
    <TestClass>
    Public Class TestSerialization

        Private Shared Function roundTrip(Of T)(ByVal value As T) As T
            Dim formatter As New ObjectFormatter()
            Using ms As New MemoryStream()
                formatter.Serialize(ms, value)
                ms.Position = 0
                Return DirectCast(formatter.Deserialize(ms), T)
            End Using
        End Function

        <TestMethod>
        Public Sub testRR()
            Dim rrs As IList(Of RecordReader) = New List(Of RecordReader)()
            rrs.Add(New CSVNLinesSequenceRecordReader(10))
            rrs.Add(New CSVRecordReader(10, ","c))
            rrs.Add(New CSVSequenceRecordReader(1, ","))
            rrs.Add(New CSVVariableSlidingWindowRecordReader(5))
            rrs.Add(New CSVRegexRecordReader(0, ",", Nothing, New String() {Nothing, "(.+) (.+) (.+)"}))
            rrs.Add(New CSVLineSequenceRecordReader())
            rrs.Add(New CSVMultiSequenceRecordReader("^$", CSVMultiSequenceRecordReader.Mode.PAD, New Text("PAD")))
            rrs.Add(New JacksonRecordReader(New FieldSelection.Builder().addField("a").addField(New Text("MISSING_B"), "b").addField(New Text("MISSING_CX"), "c", "x").build()))
            rrs.Add(New JacksonLineRecordReader(New FieldSelection.Builder().addField("value1").addField("value2").build()))
            rrs.Add(New LibSvmRecordReader())
            rrs.Add(New SVMLightRecordReader())
            rrs.Add(New RegexLineRecordReader("(.+) (.+) (.+)", 0))
            rrs.Add(New RegexSequenceRecordReader("(.+) (.+) (.+)", 0))
            rrs.Add(New LineRecordReader())

            For Each r As RecordReader In rrs
                Dim r2 As RecordReader = roundTrip(r)
                assertNotNull(r2)
                assertEquals(r.GetType(), r2.GetType())
                assertFalse(r2 Is r)
            Next r
        End Sub

        <TestMethod>
        Public Sub testCsvRRSerializationResults()
            Dim skipLines As Integer = 3
            Dim r1 As RecordReader = New CSVRecordReader(skipLines, ControlChars.Tab)
            Dim r2 As RecordReader = roundTrip(r1)

            Dim f As String = New ClassPathResource("datavec-api/iris_tab_delim.txt").File

            r1.initialize(New FileSplit(f))
            r2.initialize(New FileSplit(f))

            Dim count As Integer = 0
            Do While r1.hasNext()
                Dim n1 As IList(Of WritableT) = r1.next()
                Dim n2 As IList(Of WritableT) = r2.next()
                assertEquals(n1, n2)
                assertEquals(5, n1.Count)
                count += 1
            Loop
            assertFalse(r2.hasNext())
            assertEquals(150 - skipLines, count)
            r1.Dispose()
            r2.Dispose()
        End Sub

        ''' <summary>A deserialized reader is fully usable after initialization (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testDeserializedReadersWork()
            Dim regex As RegexLineRecordReader = roundTrip(New RegexLineRecordReader("(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}) (\d+) ([A-Z]+) (.*)", 1))
            regex.initialize(New FileSplit(New ClassPathResource("datavec-api/logtestdata/logtestfile0.txt").File))
            assertEquals("DEBUG", regex.next()(2).ToString())
            regex.close()

            Dim multi As CSVMultiSequenceRecordReader = roundTrip(New CSVMultiSequenceRecordReader("^$", CSVMultiSequenceRecordReader.Mode.CONCAT))
            multi.initialize(New Global.DeepLearning4VB.org.datavec.api.split.StringSplit("a,b"))
            assertEquals(2, multi.sequenceRecord().Count)

            Dim svm As SVMLightRecordReader = roundTrip(New SVMLightRecordReader())
            Dim config As New Global.DeepLearning4VB.org.datavec.api.conf.Configuration()
            config.setInt(SVMLightRecordReader.NUM_FEATURES, 11)
            svm.initialize(config, New FileSplit(New ClassPathResource("datavec-api/svmlight/basic.txt").File))
            assertEquals(12, svm.next().Count)
            svm.close()
        End Sub

    End Class

End Namespace
