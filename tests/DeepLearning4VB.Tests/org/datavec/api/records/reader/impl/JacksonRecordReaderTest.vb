Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports PathLabelGenerator = DeepLearning4VB.org.datavec.api.io.labels.PathLabelGenerator
Imports Record = DeepLearning4VB.org.datavec.api.records.Record
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataURI
Imports RecordReader = DeepLearning4VB.org.datavec.api.records.reader.RecordReader
Imports FieldSelection = DeepLearning4VB.org.datavec.api.records.reader.impl.jackson.FieldSelection
Imports JacksonRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.jackson.JacksonRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports NumberedFileInputSplit = DeepLearning4VB.org.datavec.api.split.NumberedFileInputSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports IntWritable = DeepLearning4VB.org.datavec.api.writable.IntWritable
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4vb.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>
    ''' Ported from the DL4J JUnit test of the same name. The YAML and XML variants (testReadingYaml,
    ''' testReadingXml) are not ported: the port reads JSON only (System.Text.Json; no YAML/XML ObjectMapper).
    ''' </summary>
    <TestClass>
    Public Class JacksonRecordReaderTest

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

        ''' <summary>Copies the three JSON test files to the temp dir and returns a numbered split over them.</summary>
        Private Function jsonSplit() As InputSplit
            Dim cpr As New ClassPathResource("datavec-api/json/")
            cpr.copyDirectory(tmpDir)
            Dim path As String = System.IO.Path.Combine(tmpDir, "json_test_%d.txt")
            Return New NumberedFileInputSplit(path, 0, 2)
        End Function

        <TestMethod>
        Public Sub testReadingJson()
            'Load 3 values from 3 JSON files
            'stricture: a:value, b:value, c:x:value, c:y:value
            'And we want to load only a:value, b:value and c:x:value
            'For first JSON file: all values are present
            'For second JSON file: b:value is missing
            'For third JSON file: c:x:value is missing
            Dim [is] As InputSplit = jsonSplit()

            Dim rr As RecordReader = New JacksonRecordReader(getFieldSelection())
            rr.initialize([is])

            testJacksonRecordReader(rr)
        End Sub

        Private Shared Function getFieldSelection() As FieldSelection
            Return New FieldSelection.Builder().addField("a").addField(New Text("MISSING_B"), "b").addField(New Text("MISSING_CX"), "c", "x").build()
        End Function

        Private Shared Sub testJacksonRecordReader(ByVal rr As RecordReader)
            Dim json0 As IList(Of WritableT) = rr.next()
            Dim exp0 As IList(Of WritableT) = Arrays.asList(Of WritableT)(New Text("aValue0"), New Text("bValue0"), New Text("cxValue0"))
            assertEquals(exp0, json0)

            Dim json1 As IList(Of WritableT) = rr.next()
            Dim exp1 As IList(Of WritableT) = Arrays.asList(Of WritableT)(New Text("aValue1"), New Text("MISSING_B"), New Text("cxValue1"))
            assertEquals(exp1, json1)

            Dim json2 As IList(Of WritableT) = rr.next()
            Dim exp2 As IList(Of WritableT) = Arrays.asList(Of WritableT)(New Text("aValue2"), New Text("bValue2"), New Text("MISSING_CX"))
            assertEquals(exp2, json2)

            assertFalse(rr.hasNext())

            'Test reset
            rr.reset()
            assertEquals(exp0, rr.next())
            assertEquals(exp1, rr.next())
            assertEquals(exp2, rr.next())
            assertFalse(rr.hasNext())
        End Sub

        <TestMethod>
        Public Sub testAppendingLabels()
            Dim [is] As InputSplit = jsonSplit()

            'Insert at the end:
            Dim rr As RecordReader = New JacksonRecordReader(getFieldSelection(), False, -1, New LabelGen())
            rr.initialize([is])

            Dim exp0 As IList(Of WritableT) = Arrays.asList(Of WritableT)(New Text("aValue0"), New Text("bValue0"), New Text("cxValue0"), New IntWritable(0))
            assertEquals(exp0, rr.next())

            Dim exp1 As IList(Of WritableT) = Arrays.asList(Of WritableT)(New Text("aValue1"), New Text("MISSING_B"), New Text("cxValue1"), New IntWritable(1))
            assertEquals(exp1, rr.next())

            Dim exp2 As IList(Of WritableT) = Arrays.asList(Of WritableT)(New Text("aValue2"), New Text("bValue2"), New Text("MISSING_CX"), New IntWritable(2))
            assertEquals(exp2, rr.next())

            'Insert at position 0:
            rr = New JacksonRecordReader(getFieldSelection(), False, -1, New LabelGen(), 0)
            rr.initialize([is])

            exp0 = Arrays.asList(Of WritableT)(New IntWritable(0), New Text("aValue0"), New Text("bValue0"), New Text("cxValue0"))
            assertEquals(exp0, rr.next())

            exp1 = Arrays.asList(Of WritableT)(New IntWritable(1), New Text("aValue1"), New Text("MISSING_B"), New Text("cxValue1"))
            assertEquals(exp1, rr.next())

            exp2 = Arrays.asList(Of WritableT)(New IntWritable(2), New Text("aValue2"), New Text("bValue2"), New Text("MISSING_CX"))
            assertEquals(exp2, rr.next())
        End Sub

        <TestMethod>
        Public Sub testAppendingLabelsMetaData()
            Dim [is] As InputSplit = jsonSplit()

            'Insert at the end:
            Dim rr As RecordReader = New JacksonRecordReader(getFieldSelection(), False, -1, New LabelGen())
            rr.initialize([is])

            Dim [out] As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Do While rr.hasNext()
                [out].Add(rr.next())
            Loop
            assertEquals(3, [out].Count)

            rr.reset()

            Dim out2 As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            Dim outRecord As IList(Of Record) = New List(Of Record)()
            Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
            Do While rr.hasNext()
                Dim r As Record = rr.nextRecord()
                out2.Add(r.getRecord())
                outRecord.Add(r)
                meta.Add(r.MetaData)
            Loop

            assertEquals([out], out2)
            For i As Integer = 0 To 2
                assertEquals([is].locations()(i), meta(i).URI)
                assertEquals(GetType(JacksonRecordReader), DirectCast(meta(i), RecordMetaDataURI).getReaderClass())
            Next i

            Dim fromMeta As IList(Of Record) = rr.loadFromMetaData(meta)
            assertEquals(outRecord, fromMeta)
            assertEquals(outRecord(2), rr.loadFromMetaData(meta(2)))
        End Sub

        ''' <summary>Shuffling with a seed, a FileSplit is rejected, and the stream form (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testShuffleAndErrors()
            Dim [is] As InputSplit = jsonSplit()
            Dim all As New HashSet(Of String)(New String() {"aValue0", "aValue1", "aValue2"})

            Dim rr As New JacksonRecordReader(getFieldSelection(), True, 12345, Nothing)
            rr.initialize([is])
            Dim seen As New HashSet(Of String)()
            Do While rr.hasNext()
                seen.Add(rr.next()(0).ToString())
            Loop
            assertEquals(all, seen)
            'the same seed gives the same order
            Dim first As New List(Of String)()
            Dim second As New List(Of String)()
            Dim a As New JacksonRecordReader(getFieldSelection(), True, 99, Nothing)
            Dim b As New JacksonRecordReader(getFieldSelection(), True, 99, Nothing)
            a.initialize([is])
            b.initialize([is])
            Do While a.hasNext()
                first.Add(a.next()(0).ToString())
                second.Add(b.next()(0).ToString())
            Loop
            assertEquals(first, second)

            assertThrows(Of NotSupportedException)(Sub() Call New JacksonRecordReader(getFieldSelection()).initialize(New FileSplit(tmpDir)))
            assertThrows(Of InvalidOperationException)(Sub() Call New JacksonRecordReader(getFieldSelection()).next())
            assertThrows(Of NotSupportedException)(Sub()
                                                       Dim ignored = rr.Labels
                                                   End Sub)

            'a single document read from a stream
            Dim uri As Uri = [is].locations()(1)
            Using dis As New DataInputStream(New FileStream(URIUtil.toFilePath(uri), FileMode.Open, FileAccess.Read))
                assertEquals(Arrays.asList(Of WritableT)(New Text("aValue1"), New Text("MISSING_B"), New Text("cxValue1")), rr.record(uri, dis))
            End Using
        End Sub

        <Serializable>
        Private Class LabelGen
            Implements PathLabelGenerator

            Public Function getLabelForPath(ByVal path As String) As WritableT Implements PathLabelGenerator.getLabelForPath
                If path.EndsWith("0.txt", StringComparison.Ordinal) Then
                    Return New IntWritable(0)
                ElseIf path.EndsWith("1.txt", StringComparison.Ordinal) Then
                    Return New IntWritable(1)
                Else
                    Return New IntWritable(2)
                End If
            End Function

            Public Function getLabelForPath(ByVal uri As Uri) As WritableT Implements PathLabelGenerator.getLabelForPath
                Return getLabelForPath(URIUtil.getPath(uri))
            End Function

            Public Function inferLabelClasses() As Boolean Implements PathLabelGenerator.inferLabelClasses
                Return True
            End Function
        End Class

    End Class

End Namespace
