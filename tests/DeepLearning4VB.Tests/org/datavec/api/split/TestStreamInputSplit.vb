Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.function
Imports CSVRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports CSVSequenceRecordReader = DeepLearning4VB.org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports InputSplit = DeepLearning4VB.org.datavec.api.split.InputSplit
Imports StreamInputSplit = DeepLearning4VB.org.datavec.api.split.StreamInputSplit
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil
Imports Text = DeepLearning4VB.org.datavec.api.writable.Text
Imports WritableT = DeepLearning4VB.org.datavec.api.writable.Writable

Namespace org.datavec.api.split

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class TestStreamInputSplit

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

        Private Function writeFile(ByVal name As String, ByVal content As String) As Uri
            Dim f As String = Path.Combine(tmpDir, name)
            File.WriteAllText(f, content, New UTF8Encoding(False))
            Return URIUtil.fileToURI(f)
        End Function

        <TestMethod>
        Public Sub testCsvSimple()
            Dim f1 As Uri = writeFile("file1.txt", "a,b,c" & vbLf & "d,e,f")
            Dim f2 As Uri = writeFile("file2.txt", "1,2,3")

            Dim uris As IList(Of Uri) = Arrays.asList(f1, f2)

            Using rr As New CSVRecordReader()
                Dim fn As New TestStreamFunction()
                Dim [is] As InputSplit = New StreamInputSplit(uris, fn)
                rr.initialize([is])

                Dim exp As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                exp.Add(Arrays.asList(Of WritableT)(New Text("a"), New Text("b"), New Text("c")))
                exp.Add(Arrays.asList(Of WritableT)(New Text("d"), New Text("e"), New Text("f")))
                exp.Add(Arrays.asList(Of WritableT)(New Text("1"), New Text("2"), New Text("3")))

                Dim act As IList(Of IList(Of WritableT)) = New List(Of IList(Of WritableT))()
                Do While rr.hasNext()
                    act.Add(rr.next())
                Loop

                assertEquals(exp, act)

                'Check that the specified stream loading function was used, not the default:
                assertEquals(uris, fn.calledWithUris)

                rr.reset()
                Dim count As Integer = 0
                Do While rr.hasNext()
                    count += 1
                    rr.next()
                Loop
                assertEquals(3, count)
            End Using
        End Sub

        <TestMethod>
        Public Sub testCsvSequenceSimple()
            Dim f1 As Uri = writeFile("file1.txt", "a,b,c" & vbLf & "d,e,f")
            Dim f2 As Uri = writeFile("file2.txt", "1,2,3")

            Dim uris As IList(Of Uri) = Arrays.asList(f1, f2)

            Dim rr As New CSVSequenceRecordReader()
            Dim fn As New TestStreamFunction()
            Dim [is] As InputSplit = New StreamInputSplit(uris, fn)
            rr.initialize([is])

            Dim exp As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            exp.Add(Arrays.asList(Of IList(Of WritableT))(Arrays.asList(Of WritableT)(New Text("a"), New Text("b"), New Text("c")), Arrays.asList(Of WritableT)(New Text("d"), New Text("e"), New Text("f"))))
            exp.Add(Arrays.asList(Of IList(Of WritableT))(Arrays.asList(Of WritableT)(New Text("1"), New Text("2"), New Text("3"))))

            Dim act As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Do While rr.hasNext()
                act.Add(rr.sequenceRecord())
            Loop

            assertEquals(exp, act)

            'Check that the specified stream loading function was used, not the default:
            assertEquals(uris, fn.calledWithUris)

            rr.reset()
            Dim count As Integer = 0
            Do While rr.hasNext()
                count += 1
                rr.sequenceRecord()
            Loop
            assertEquals(2, count)
        End Sub

        <TestMethod>
        Public Sub testShuffle()
            Dim f1 As Uri = writeFile("file1.txt", "a,b,c")
            Dim f2 As Uri = writeFile("file2.txt", "1,2,3")
            Dim f3 As Uri = writeFile("file3.txt", "x,y,z")

            Dim uris As IList(Of Uri) = Arrays.asList(f1, f2, f3)

            Dim rr As New CSVSequenceRecordReader()
            Dim fn As New TestStreamFunction()
            Dim [is] As InputSplit = New StreamInputSplit(uris, fn, New Random(12345))
            rr.initialize([is])

            Dim act As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Do While rr.hasNext()
                act.Add(rr.sequenceRecord())
            Loop

            rr.reset()
            Dim act2 As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Do While rr.hasNext()
                act2.Add(rr.sequenceRecord())
            Loop

            rr.reset()
            Dim act3 As IList(Of IList(Of IList(Of WritableT))) = New List(Of IList(Of IList(Of WritableT)))()
            Do While rr.hasNext()
                act3.Add(rr.sequenceRecord())
            Loop

            assertEquals(3, act.Count)
            assertEquals(3, act2.Count)
            assertEquals(3, act3.Count)
            'every pass reads all three files
            Dim expectedSet As New HashSet(Of String)(New String() {"a", "1", "x"})
            For Each pass As IList(Of IList(Of IList(Of WritableT))) In New IList(Of IList(Of IList(Of WritableT)))() {act, act2, act3}
                Dim firstValues As New HashSet(Of String)()
                For Each seq As IList(Of IList(Of WritableT)) In pass
                    firstValues.Add(seq(0)(0).ToString())
                Next seq
                assertEquals(expectedSet, firstValues)
            Next pass

            'The Java test relied on java.util.Random(12345) giving three different orders in its first three
            'passes; System.Random produces a different sequence for the same seed. Check instead that the order
            'changes between resets: across ten passes at least two different orders must appear.
            Dim orders As New HashSet(Of String)()
            For Each pass As IList(Of IList(Of IList(Of WritableT))) In New IList(Of IList(Of IList(Of WritableT)))() {act, act2, act3}
                orders.Add(String.Join("", pass.Select(Function(s) s(0)(0).ToString())))
            Next pass
            For i As Integer = 1 To 7
                rr.reset()
                Dim order As New System.Text.StringBuilder()
                Do While rr.hasNext()
                    order.Append(rr.sequenceRecord()(0)(0).ToString())
                Loop
                orders.Add(order.ToString())
            Next i
            assertTrue(orders.Count > 1, "the stream order never changed across 10 passes")
        End Sub

        Public Class TestStreamFunction
            Implements [Function](Of Uri, Stream)

            Public calledWithUris As IList(Of Uri) = New List(Of Uri)()

            Public Function apply(ByVal uri As Uri) As Stream Implements [Function](Of Uri, Stream).apply
                calledWithUris.Add(uri) 'Just for testing to ensure function is used
                Return New FileStream(URIUtil.toFilePath(uri), FileMode.Open, FileAccess.Read)
            End Function
        End Class

    End Class

End Namespace
