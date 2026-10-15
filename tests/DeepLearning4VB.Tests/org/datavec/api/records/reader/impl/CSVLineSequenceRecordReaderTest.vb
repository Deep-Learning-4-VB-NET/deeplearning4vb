Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.JavaCompat
Imports SequenceRecord = DeepLearning4VB.org.datavec.api.records.SequenceRecord
Imports RecordMetaData = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataLine = DeepLearning4VB.org.datavec.api.records.metadata.RecordMetaDataLine
Imports SequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.SequenceRecordReader
Imports CSVLineSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVLineSequenceRecordReader
Imports FileSplit = DeepLearning4VB.org.datavec.api.split.FileSplit
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.records.reader.impl

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class CSVLineSequenceRecordReaderTest

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

        Private Shared Function univariate(ByVal ParamArray values() As String) As IList(Of IList(Of WritableT))
            Dim result As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
            For Each s As String In values
                result.Add(JavaCollections.singletonList(Of WritableT)(New Text(s)))
            Next s
            Return result
        End Function

        <TestMethod>
        Public Sub test()
            Dim source As String = Path.Combine(tmpDir, "temp.csv")
            Dim str As String = "a,b,c" & vbLf & "1,2,3,4"
            File.WriteAllText(source, str, New UTF8Encoding(False))

            Using rr As SequenceRecordReader = New CSVLineSequenceRecordReader()
                rr.initialize(New FileSplit(source))

                Dim exp0 As IList(Of IList(Of WritableT)) = univariate("a", "b", "c")
                Dim exp1 As IList(Of IList(Of WritableT)) = univariate("1", "2", "3", "4")

                For i As Integer = 0 To 2
                    Dim count As Integer = 0
                    Do While rr.hasNext()
                        Dim [next] As IList(Of IList(Of WritableT)) = rr.sequenceRecord()
                        If count = 0 Then
                            assertEquals(exp0, [next])
                        Else
                            assertEquals(exp1, [next])
                        End If
                        count += 1
                    Loop

                    assertEquals(2, count)
                    rr.reset()
                Next i
            End Using
        End Sub

        ''' <summary>Metadata of each line sequence, and loading from it (not in the JUnit test).</summary>
        <TestMethod>
        Public Sub testMetaData()
            Dim source As String = Path.Combine(tmpDir, "temp.csv")
            File.WriteAllText(source, "a,b,c" & vbLf & "1,2,3,4", New UTF8Encoding(False))

            Using rr As New CSVLineSequenceRecordReader()
                rr.initialize(New FileSplit(source))
                Dim records As IList(Of SequenceRecord) = New List(Of SequenceRecord)()
                Dim meta As IList(Of RecordMetaData) = New List(Of RecordMetaData)()
                Do While rr.hasNext()
                    Dim sr As SequenceRecord = rr.nextSequence()
                    records.Add(sr)
                    meta.Add(sr.getMetaData())
                Loop
                assertEquals(2, records.Count)
                assertEquals(0, DirectCast(meta(0), RecordMetaDataLine).getLineNumber())
                assertEquals(1, DirectCast(meta(1), RecordMetaDataLine).getLineNumber())
                assertEquals(univariate("1", "2", "3", "4"), records(1).getSequenceRecord())

                Dim fromMeta As IList(Of SequenceRecord) = rr.loadSequenceFromMetaData(meta)
                assertEquals(records, fromMeta)
                assertEquals(records(1), rr.loadSequenceFromMetaData(meta(1)))

                'the sequence form of a single line read from a stream
                Using dis As New DataInputStream(New MemoryStream(Encoding.UTF8.GetBytes("x,y")))
                    assertEquals(univariate("x", "y"), rr.sequenceRecord(New Uri(source), dis))
                End Using
            End Using
        End Sub

    End Class

End Namespace
