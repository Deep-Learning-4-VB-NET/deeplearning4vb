Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports RecordMetaDataInterval = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataInterval
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports CSVMultiSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVMultiSequenceRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class CSVMultiSequenceRecordReaderTest

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

        Private Shared Sub separatorFor(ByVal i As Integer, ByRef seqSep As String, ByRef seqSepRegex As String)
            Select Case i
                Case 0
                    seqSep = ""
                    seqSepRegex = "^$"
                Case 1
                    seqSep = "---"
                    seqSepRegex = seqSep
                Case 2
                    seqSep = "&"
                    seqSepRegex = seqSep
                Case Else
                    Throw New Exception()
            End Select
        End Sub

        Private Function writeFile(ByVal name As String, ByVal content As String) As String
            Dim f As String = Path.Combine(tmpDir, name)
            File.WriteAllText(f, content, New UTF8Encoding(False))
            Return f
        End Function

        Private Shared Function univariate(ByVal csv As String) As IList(Of IList(Of WritableT))
            Dim result As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For Each s As String In csv.Split(","c)
                result.Add(JavaCollections.singletonList(Of WritableT)(New Text(s)))
            Next s
            Return result
        End Function

        <TestMethod>
        Public Sub testConcatMode()
            For i As Integer = 0 To 2
                Dim seqSep As String = Nothing
                Dim seqSepRegex As String = Nothing
                separatorFor(i, seqSep, seqSepRegex)

                Dim str As String = "a,b,c" & vbLf & "1,2,3,4" & vbLf & "x,y" & vbLf & seqSep & vbLf & "A,B,C"
                Dim f As String = writeFile("concat" & i & ".csv", str)

                Using seqRR As SequenceRecordReader = New CSVMultiSequenceRecordReader(seqSepRegex, CSVMultiSequenceRecordReader.Mode.CONCAT)
                    seqRR.initialize(New FileSplit(f))

                    Dim exp0 As IList(Of IList(Of WritableT)) = univariate("a,b,c,1,2,3,4,x,y")
                    Dim exp1 As IList(Of IList(Of WritableT)) = univariate("A,B,C")

                    assertEquals(exp0, seqRR.sequenceRecord())
                    assertEquals(exp1, seqRR.sequenceRecord())
                    assertFalse(seqRR.hasNext())

                    seqRR.reset()

                    assertEquals(exp0, seqRR.sequenceRecord())
                    assertEquals(exp1, seqRR.sequenceRecord())
                    assertFalse(seqRR.hasNext())
                End Using
            Next i
        End Sub

        <TestMethod>
        Public Sub testEqualLength()
            For i As Integer = 0 To 2
                Dim seqSep As String = Nothing
                Dim seqSepRegex As String = Nothing
                separatorFor(i, seqSep, seqSepRegex)

                Dim str As String = "a,b" & vbLf & "1,2" & vbLf & "x,y" & vbLf & seqSep & vbLf & "A" & vbLf & "B" & vbLf & "C"
                Dim f As String = writeFile("equal" & i & ".csv", str)

                Using seqRR As SequenceRecordReader = New CSVMultiSequenceRecordReader(seqSepRegex, CSVMultiSequenceRecordReader.Mode.EQUAL_LENGTH)
                    seqRR.initialize(New FileSplit(f))

                    Dim exp0 As IList(Of IList(Of WritableT)) = Arrays.asList(Of IList(Of WritableT))(Arrays.asList(Of WritableT)(New Text("a"), New Text("1"), New Text("x")), Arrays.asList(Of WritableT)(New Text("b"), New Text("2"), New Text("y")))
                    Dim exp1 As IList(Of IList(Of WritableT)) = JavaCollections.singletonList(Of IList(Of WritableT))(Arrays.asList(Of WritableT)(New Text("A"), New Text("B"), New Text("C")))

                    assertEquals(exp0, seqRR.sequenceRecord())
                    assertEquals(exp1, seqRR.sequenceRecord())
                    assertFalse(seqRR.hasNext())

                    seqRR.reset()

                    assertEquals(exp0, seqRR.sequenceRecord())
                    assertEquals(exp1, seqRR.sequenceRecord())
                    assertFalse(seqRR.hasNext())
                End Using
            Next i
        End Sub

        <TestMethod>
        Public Sub testPadding()
            For i As Integer = 0 To 2
                Dim seqSep As String = Nothing
                Dim seqSepRegex As String = Nothing
                separatorFor(i, seqSep, seqSepRegex)

                Dim str As String = "a,b" & vbLf & "1" & vbLf & "x" & vbLf & seqSep & vbLf & "A" & vbLf & "B" & vbLf & "C"
                Dim f As String = writeFile("pad" & i & ".csv", str)

                Using seqRR As SequenceRecordReader = New CSVMultiSequenceRecordReader(seqSepRegex, CSVMultiSequenceRecordReader.Mode.PAD, New Text("PAD"))
                    seqRR.initialize(New FileSplit(f))

                    Dim exp0 As IList(Of IList(Of WritableT)) = Arrays.asList(Of IList(Of WritableT))(Arrays.asList(Of WritableT)(New Text("a"), New Text("1"), New Text("x")), Arrays.asList(Of WritableT)(New Text("b"), New Text("PAD"), New Text("PAD")))
                    Dim exp1 As IList(Of IList(Of WritableT)) = JavaCollections.singletonList(Of IList(Of WritableT))(Arrays.asList(Of WritableT)(New Text("A"), New Text("B"), New Text("C")))

                    assertEquals(exp0, seqRR.sequenceRecord())
                    assertEquals(exp1, seqRR.sequenceRecord())
                    assertFalse(seqRR.hasNext())

                    seqRR.reset()

                    assertEquals(exp0, seqRR.sequenceRecord())
                    assertEquals(exp1, seqRR.sequenceRecord())
                    assertFalse(seqRR.hasNext())
                End Using
            Next i
        End Sub

        ''' <summary>Metadata, the mode checks and the DataInputStream path (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testMetaDataAndErrors()
            Dim f As String = writeFile("meta.csv", "a,b" & vbLf & "1,2" & vbLf & "---" & vbLf & "x,y,z")
            Using seqRR As New CSVMultiSequenceRecordReader("---", CSVMultiSequenceRecordReader.Mode.EQUAL_LENGTH)
                seqRR.initialize(New FileSplit(f))
                Dim sr As SequenceRecord = seqRR.nextSequence()
                assertEquals(2, sr.getSequenceLength())
                Dim meta As RecordMetaDataInterval = DirectCast(sr.getMetaData(), RecordMetaDataInterval)
                assertEquals(0L, meta.getFrom())
                assertEquals(3L, meta.getTo())
                'the second "sequence" has a single line, so any length is equal
                assertEquals(3, seqRR.nextSequence().getSequenceLength())
                assertFalse(seqRR.batchesSupported())
                assertThrows(Of NotSupportedException)(Sub() seqRR.loadSequenceFromMetaData(meta))
            End Using

            'unequal column lengths are an error in EQUAL_LENGTH mode
            Dim bad As String = writeFile("bad.csv", "a,b" & vbLf & "1")
            Using seqRR As New CSVMultiSequenceRecordReader("---", CSVMultiSequenceRecordReader.Mode.EQUAL_LENGTH)
                seqRR.initialize(New FileSplit(bad))
                assertThrows(Of InvalidOperationException)(Sub() seqRR.sequenceRecord())
            End Using

            'PAD mode needs a padding value
            assertThrows(Of InvalidOperationException)(Sub()
                                                           Dim ignored As New CSVMultiSequenceRecordReader("---", CSVMultiSequenceRecordReader.Mode.PAD)
                                                       End Sub)

            'reading a sequence from a stream (as the file batch readers do)
            Dim seqRR2 As New CSVMultiSequenceRecordReader("---", CSVMultiSequenceRecordReader.Mode.CONCAT)
            Using dis As New DataInputStream(New MemoryStream(Encoding.UTF8.GetBytes("p,q" & vbLf & "r" & vbLf & "---" & vbLf & "ignored")))
                assertEquals(univariate("p,q,r"), seqRR2.sequenceRecord(New Uri(f), dis))
            End Using
        End Sub

    End Class

End Namespace
