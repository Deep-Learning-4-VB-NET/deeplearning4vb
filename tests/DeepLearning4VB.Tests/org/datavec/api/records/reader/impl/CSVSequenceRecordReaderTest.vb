Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataURI = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataURI
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports CSVSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports NumberedFileInputSplit = DeepLearning4VB.org.datavec.api.split.NumberedFileInputSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable
Imports ClassPathResource = DeepLearning4VB.org.nd4j.common.io.ClassPathResource

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class CSVSequenceRecordReaderTest

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
        Public Sub test()
            Dim seqReader As New CSVSequenceRecordReader(1, ",")
            seqReader.initialize(New TestInputSplit())

            Dim sequenceCount As Integer = 0
            Do While seqReader.hasNext()
                Dim sequence As IList(Of IList(Of WritableT)) = seqReader.sequenceRecord()
                ' 4 lines, plus 1 header line
                assertEquals(4, sequence.Count)

                Dim lineCount As Integer = 0
                For Each timeStep As IList(Of WritableT) In sequence
                    assertEquals(3, timeStep.Count)
                    Dim countInLine As Integer = 0
                    For Each entry As WritableT In timeStep
                        Dim expValue As Integer = 100 * sequenceCount + 10 * lineCount + countInLine
                        assertEquals(expValue.ToString(), entry.ToString())
                        countInLine += 1
                    Next entry
                    lineCount += 1
                Next timeStep
                sequenceCount += 1
            Loop
            assertEquals(3, sequenceCount)
        End Sub

        <TestMethod>
        Public Sub testReset()
            Dim seqReader As New CSVSequenceRecordReader(1, ",")
            seqReader.initialize(New TestInputSplit())

            Dim nTests As Integer = 5
            For i As Integer = 0 To nTests - 1
                seqReader.reset()

                Dim sequenceCount As Integer = 0
                Do While seqReader.hasNext()
                    Dim sequence As IList(Of IList(Of WritableT)) = seqReader.sequenceRecord()
                    ' 4 lines, plus 1 header line
                    assertEquals(4, sequence.Count)

                    Dim lineCount As Integer = 0
                    For Each timeStep As IList(Of WritableT) In sequence
                        lineCount += 1
                    Next timeStep
                    sequenceCount += 1
                    assertEquals(4, lineCount)
                Loop
                assertEquals(3, sequenceCount)
            Next i
        End Sub

        <TestMethod>
        Public Sub testMetaData()
            Dim seqReader As New CSVSequenceRecordReader(1, ",")
            seqReader.initialize(New TestInputSplit())

            Dim l As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Do While seqReader.hasNext()
                Dim sequence As IList(Of IList(Of WritableT)) = seqReader.sequenceRecord()
                ' 4 lines, plus 1 header line
                assertEquals(4, sequence.Count)

                Dim lineCount As Integer = 0
                For Each timeStep As IList(Of WritableT) In sequence
                    lineCount += 1
                Next timeStep
                assertEquals(4, lineCount)

                l.Add(sequence)
            Loop

            Dim l2 As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
            Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
            seqReader.reset()
            Do While seqReader.hasNext()
                Dim sr As SequenceRecord = seqReader.nextSequence()
                l2.Add(sr)
                meta.Add(sr.MetaData)
            Loop
            assertEquals(3, l2.Count)

            Dim fromMeta As IList(Of SequenceRecord) = seqReader.loadSequenceFromMetaData(meta)
            For i As Integer = 0 To 2
                assertEquals(l(i), l2(i).getSequenceRecord())
                assertEquals(l(i), fromMeta(i).getSequenceRecord())
                assertTrue(TypeOf meta(i) Is RecordMetaDataURI)
                assertEquals(TestInputSplit.uris()(i), meta(i).URI)
                assertEquals(l2(i), fromMeta(i))
            Next i
        End Sub

        ''' <summary>The three CSV sequence files, as a hand-written split (as in the JUnit test).</summary>
        Private Class TestInputSplit
            Implements InputSplit

            Public Shared Function uris() As Uri()
                Dim arr(2) As Uri
                arr(0) = URIUtil.fileToURI(New ClassPathResource("datavec-api/csvsequence_0.txt").File)
                arr(1) = URIUtil.fileToURI(New ClassPathResource("datavec-api/csvsequence_1.txt").File)
                arr(2) = URIUtil.fileToURI(New ClassPathResource("datavec-api/csvsequence_2.txt").File)
                Return arr
            End Function

            Public Function canWriteToLocation(ByVal location As Uri) As Boolean Implements InputSplit.canWriteToLocation
                Return False
            End Function

            Public Function addNewLocation() As String Implements InputSplit.addNewLocation
                Return Nothing
            End Function

            Public Function addNewLocation(ByVal location As String) As String Implements InputSplit.addNewLocation
                Return Nothing
            End Function

            Public Sub updateSplitLocations(ByVal reset As Boolean) Implements InputSplit.updateSplitLocations
            End Sub

            Public Function needsBootstrapForWrite() As Boolean Implements InputSplit.needsBootstrapForWrite
                Return False
            End Function

            Public Sub bootStrapForWrite() Implements InputSplit.bootStrapForWrite
            End Sub

            Public Function openOutputStreamFor(ByVal location As String) As Stream Implements InputSplit.openOutputStreamFor
                Return Nothing
            End Function

            Public Function openInputStreamFor(ByVal location As String) As Stream Implements InputSplit.openInputStreamFor
                Return Nothing
            End Function

            Public Function length() As Long Implements InputSplit.length
                Return 3
            End Function

            Public Function locations() As Uri() Implements InputSplit.locations
                Return uris()
            End Function

            Public Function locationsIterator() As IEnumerator(Of Uri) Implements InputSplit.locationsIterator
                Return DirectCast(Arrays.asList(locations()), IEnumerable(Of Uri)).GetEnumerator()
            End Function

            Public Function locationsPathIterator() As IEnumerator(Of String) Implements InputSplit.locationsPathIterator
                Dim loc() As Uri = locations()
                Dim arr(loc.Length - 1) As String
                For i As Integer = 0 To loc.Length - 1
                    arr(i) = loc(i).ToString()
                Next i
                Return DirectCast(Arrays.asList(arr), IEnumerable(Of String)).GetEnumerator()
            End Function

            Public Sub reset() Implements InputSplit.reset
                ' No op
            End Sub

            Public Function resetSupported() As Boolean Implements InputSplit.resetSupported
                Return True
            End Function
        End Class

        ''' <summary>Java copied the resources with <c>ClassPathResource("csvsequence_%d.txt")</c>; here they live under <c>datavec-api/</c>.</summary>
        <TestMethod>
        Public Sub testCsvSeqAndNumberedFileSplit()
            'Simple sanity check unit test
            For i As Integer = 0 To 2
                Call New ClassPathResource("datavec-api/csvsequence_" & i & ".txt").getTempFileFromArchive(tmpDir)
            Next i

            'Load time series from CSV sequence files; compare to SequenceRecordReaderDataSetIterator
            Dim featuresPath As String = Path.Combine(tmpDir, "csvsequence_%d.txt")
            Dim featureReader As SequenceRecordReader = New CSVSequenceRecordReader(1, ",")
            featureReader.initialize(New NumberedFileInputSplit(featuresPath, 0, 2))

            Dim count As Integer = 0
            Do While featureReader.hasNext()
                Dim sr As SequenceRecord = featureReader.nextSequence()
                assertEquals(4, sr.getSequenceLength())
                assertEquals((100 * count).ToString(), sr.getTimeStep(0)(0).ToString())
                count += 1
            Loop
            assertEquals(3, count)
        End Sub

    End Class

End Namespace
