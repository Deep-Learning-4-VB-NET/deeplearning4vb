Imports System
Imports System.IO
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.datavec.api.split.streams
Imports URIUtil = DeepLearning4VB.org.datavec.api.util.files.URIUtil

Namespace org.datavec.api.split.streams

    ''' <summary>The default stream creator of a <c>StreamInputSplit</c>: it opens <c>file:</c> URIs for reading.</summary>
    <TestClass>
    Public Class FileStreamCreatorFunctionTest

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
        Public Sub testOpensFileUris()
            Dim target As String = Path.Combine(tmpDir, "with space.txt")
            File.WriteAllText(target, "line one" & vbLf & "line two", New UTF8Encoding(False))
            Dim fn As New FileStreamCreatorFunction()
            Dim f As DeepLearning4VB.org.nd4j.common.function.[Function](Of Uri, Stream) = fn
            Using s As Stream = f.apply(URIUtil.fileToURI(target))
                assertTrue(s.CanRead)
                assertFalse(s.CanWrite)
                Using r As New StreamReader(s, Encoding.UTF8)
                    assertEquals("line one", r.ReadLine())
                    assertEquals("line two", r.ReadLine())
                    assertNull(r.ReadLine())
                End Using
            End Using
            'each call opens a fresh stream positioned at the start
            Using s As Stream = fn.apply(URIUtil.fileToURI(target))
                assertEquals(0L, s.Position)
                assertEquals(AscW("l"c), s.ReadByte())
            End Using
            'the scheme is matched case-insensitively
            Using s As Stream = fn.apply(New Uri("FILE:///" & target.Replace("\"c, "/"c)))
                assertEquals(New FileInfo(target).Length, s.Length)
            End Using
        End Sub

        <TestMethod>
        Public Sub testRejectsNonFileUris()
            Dim fn As New FileStreamCreatorFunction()
            Dim ex As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() fn.apply(New Uri("hdfs://host/data/file.csv")))
            assertTrue(ex.Message.Contains("not a File URI"), ex.Message)
            assertTrue(ex.Message.Contains("hdfs://host/data/file.csv"), ex.Message)
            assertThrows(Of InvalidOperationException)(Sub() fn.apply(New Uri("http://example.com/x.csv")))
            'a relative URI has no scheme and so is treated as a file, but has no file path
            assertThrows(Of ArgumentException)(Sub() fn.apply(URIUtil.createURI("relative/file.csv")))
        End Sub

        <TestMethod>
        Public Sub testMissingFile()
            Dim fn As New FileStreamCreatorFunction()
            Dim missing As Uri = URIUtil.fileToURI(Path.Combine(tmpDir, "missing.csv"))
            Dim ex As InvalidOperationException = assertThrows(Of InvalidOperationException)(Sub() fn.apply(missing))
            assertTrue(ex.Message.StartsWith("Error loading stream for file: ", StringComparison.Ordinal), ex.Message)
            assertTrue(TypeOf ex.InnerException Is FileNotFoundException)
            'the function is serializable, as the split that carries it
            assertTrue(GetType(FileStreamCreatorFunction).IsSerializable)
        End Sub

    End Class

End Namespace
