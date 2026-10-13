Imports System
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
#Disable Warning BC40000 ' the DataVec ClassPathResource is kept for compatibility and marked obsolete
Imports ClassPathResource = DeepLearning4VB.org.datavec.api.util.ClassPathResource
Imports Resource = DeepLearning4VB.org.nd4j.common.io.Resource

Namespace org.datavec.api.util

    ''' <summary>
    ''' Ported from the DataVec JUnit test of the same name. The Java test read the files
    ''' <c>datavec-api/iris.dat</c> (2700 bytes), <c>datavec-api/csvsequence test.txt</c> and
    ''' <c>datavec-api/csvsequence_1.txt</c> (5 lines, 60 bytes) from the test class path; here the
    ''' equivalent fixtures live under <c>resources/datavec/util</c> and are found through the
    ''' <c>resources</c> search directory next to the test assembly. Git may check the fixtures out
    ''' with CRLF line endings, so (as the Java test did on Windows) both sizes are accepted.
    ''' </summary>
    <TestClass>
    Public Class ClassPathResourceTest

        Private Shared Sub assertSizeWithEitherLineEnding(ByVal lfSize As Long, ByVal lines As Integer, ByVal actual As Long)
            assertTrue(actual = lfSize OrElse actual = lfSize + lines, "size " & actual & " is neither " & lfSize & " (LF) nor " & (lfSize + lines) & " (CRLF)")
        End Sub

        <TestMethod>
        Public Sub testGetFile1()
            Dim intFile As String = (New ClassPathResource("datavec/util/iris.dat")).File
            assertTrue(File.Exists(intFile))
            assertSizeWithEitherLineEnding(2700L, 150, New FileInfo(intFile).Length)
            assertEquals(150, File.ReadAllLines(intFile).Length)
            assertEquals("5.1,3.5,1.4,0.2,0", File.ReadAllLines(intFile)(0))
        End Sub

        <TestMethod>
        Public Sub testGetFileSlash1()
            'a leading slash is tolerated, as on the Java class path
            Dim intFile As String = (New ClassPathResource("/datavec/util/iris.dat")).File
            assertTrue(File.Exists(intFile))
            assertSizeWithEitherLineEnding(2700L, 150, New FileInfo(intFile).Length)
            assertEquals(intFile, (New ClassPathResource("datavec/util/iris.dat")).File)
        End Sub

        <TestMethod>
        Public Sub testGetFileWithSpace1()
            Dim intFile As String = (New ClassPathResource("datavec/util/csvsequence test.txt")).File
            assertTrue(File.Exists(intFile))
            assertEquals("csvsequence test.txt", Path.GetFileName(intFile))
            assertSizeWithEitherLineEnding(60L, 5, New FileInfo(intFile).Length)
        End Sub

        <TestMethod>
        Public Sub testInputStream()
            Dim resource As New ClassPathResource("datavec/util/csvsequence_1.txt")
            Dim intFile As String = resource.File
            assertSizeWithEitherLineEnding(60L, 5, New FileInfo(intFile).Length)
            assertEquals(5, countLines(resource))
        End Sub

        <TestMethod>
        Public Sub testInputStreamSlash()
            Dim resource As New ClassPathResource("/datavec/util/csvsequence_1.txt")
            Dim intFile As String = resource.File
            assertSizeWithEitherLineEnding(60L, 5, New FileInfo(intFile).Length)
            assertEquals(5, countLines(resource))
            Using reader As New StreamReader(resource.InputStream)
                assertEquals("0.0,1.0,2.0", reader.ReadLine())
                assertEquals("1.0,2.0,3.0", reader.ReadLine())
            End Using
        End Sub

        ''' <summary>The DataVec class is a thin, obsolete subclass of the ND4J resource.</summary>
        <TestMethod>
        Public Sub testIsAnNd4jClassPathResource()
            Dim resource As New ClassPathResource("datavec/util/iris.dat")
            assertTrue(TypeOf resource Is DeepLearning4VB.org.nd4j.common.io.ClassPathResource)
            assertTrue(TypeOf resource Is Resource)
            assertTrue(resource.exists())
            assertFalse(resource.isDirectory())
            assertEquals("datavec/util/iris.dat", resource.getPath())
            assertEquals("iris.dat", resource.Filename)
            assertEquals(New FileInfo(resource.File).Length, resource.contentLength())
            assertTrue(resource.URI.IsFile)

            Dim missing As New ClassPathResource("datavec/util/no-such-file.dat")
            assertFalse(missing.exists())
            assertThrows(Of FileNotFoundException)(Function() missing.File)
            assertThrows(Of FileNotFoundException)(Function() missing.InputStream)
            assertThrows(Of ArgumentNullException)(Sub()
                                                       Dim ignored As New ClassPathResource(Nothing)
                                                   End Sub)
        End Sub

        Private Shared Function countLines(ByVal resource As ClassPathResource) As Integer
            Dim stream As Stream = resource.InputStream
            Dim reader As New StreamReader(stream)
            Dim cnt As Integer = 0
            Dim line As String = reader.ReadLine()
            Do While line IsNot Nothing
                cnt += 1
                line = reader.ReadLine()
            Loop
            reader.Close()
            Return cnt
        End Function

    End Class

End Namespace
#Enable Warning BC40000
