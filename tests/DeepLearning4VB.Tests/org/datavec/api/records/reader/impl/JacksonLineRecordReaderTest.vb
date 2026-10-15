Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports FieldSelection = DeepLearning4VB.org.datavec.api.records.reader.impl.jackson.FieldSelection
Imports JacksonLineRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.jackson.JacksonLineRecordReader
Imports JacksonLineSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.jackson.JacksonLineSequenceRecordReader
Imports CollectionInputSplit = DeepLearning4VB.org.datavec.api.split.CollectionInputSplit
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4j.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name (JSON only: the port has no YAML/XML ObjectMapper).</summary>
    <TestClass>
    Public Class JacksonLineRecordReaderTest

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

        Private Shared Function getFieldSelection() As FieldSelection
            Return New FieldSelection.Builder().addField("value1").addField("value2").addField("value3").addField("value4").addField("value5").addField("value6").addField("value7").addField("value8").addField("value9").addField("value10").build()
        End Function

        <TestMethod>
        Public Sub testReadJSON()
            Using rr As RecordReader = New JacksonLineRecordReader(getFieldSelection())
                rr.initialize(New FileSplit(New ClassPathResource("datavec-api/json/json_test_3.txt").File))
                testJacksonRecordReader(rr)
            End Using
        End Sub

        Private Shared Sub testJacksonRecordReader(ByVal rr As RecordReader)
            Dim records As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Do While rr.hasNext()
                Dim json0 As IList(Of WritableT) = rr.next()
                assertTrue(json0.Count > 0)
                records.Add(json0)
            Loop
            'the fixture has three documents of ten values each: strings, numbers (returned as written) and an empty string
            assertEquals(3, records.Count)
            assertEquals(Arrays.asList(Of WritableT)(New Text("a"), New Text("b"), New Text("c"), New Text("d"), New Text("e"), New Text("f"), New Text("g"), New Text("h"), New Text("i"), New Text("j")), records(0))
            assertEquals(10, records(1).Count)
            assertEquals("1", records(1)(0).ToString())
            assertEquals("10", records(1)(9).ToString())
            assertEquals(10, records(1)(9).toInt())
            assertEquals("", records(2)(3).ToString())
            assertEquals("5.5", records(2)(4).ToString())

            rr.reset()
            assertEquals(records(0), rr.next())
        End Sub

        <TestMethod>
        Public Sub testJacksonLineSequenceRecordReader()
            Call New ClassPathResource("datavec-api/JacksonLineSequenceRecordReaderTest/").copyDirectory(tmpDir)

            Dim f As FieldSelection = New FieldSelection.Builder().addField("a").addField(New Text("MISSING_B"), "b").addField(New Text("MISSING_CX"), "c", "x").build()
            Dim rr As New JacksonLineSequenceRecordReader(f)
            Dim files() As String = Directory.GetFiles(tmpDir)
            Array.Sort(files, StringComparer.Ordinal)
            Dim u(files.Length - 1) As Uri
            For i As Integer = 0 To files.Length - 1
                u(i) = URIUtil.fileToURI(files(i))
            Next i
            rr.initialize(New CollectionInputSplit(u))

            Dim expSeq0 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expSeq0.Add(Arrays.asList(Of WritableT)(New Text("aValue0"), New Text("bValue0"), New Text("cxValue0")))
            expSeq0.Add(Arrays.asList(Of WritableT)(New Text("aValue1"), New Text("MISSING_B"), New Text("cxValue1")))
            expSeq0.Add(Arrays.asList(Of WritableT)(New Text("aValue2"), New Text("bValue2"), New Text("MISSING_CX")))

            Dim expSeq1 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            expSeq1.Add(Arrays.asList(Of WritableT)(New Text("aValue3"), New Text("bValue3"), New Text("cxValue3")))

            Dim count As Integer = 0
            Do While rr.hasNext()
                Dim [next] As IList(Of IList(Of WritableT)) = rr.sequenceRecord()
                If count = 0 Then
                    assertEquals(expSeq0, [next])
                Else
                    assertEquals(expSeq1, [next])
                End If
                count += 1
            Loop

            assertEquals(2, count)

            'reset, metadata and the stream form
            rr.reset()
            Dim sr = rr.nextSequence()
            assertEquals(expSeq0, sr.getSequenceRecord())
            assertEquals(u(0), sr.getMetaData().URI)
            Using dis As New DataInputStream(New FileStream(files(1), FileMode.Open, FileAccess.Read))
                assertEquals(expSeq1, rr.sequenceRecord(u(1), dis))
            End Using
        End Sub

    End Class

End Namespace
