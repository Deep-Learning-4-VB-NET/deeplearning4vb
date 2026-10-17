Imports System
Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.io

Namespace org.nd4vb.common.io

    ''' <summary>
    ''' Ported from the DL4J JUnit test of the same name, plus coverage of the .NET resolution rules
    ''' (search directories and embedded manifest resources).
    ''' </summary>
    <TestClass>
    Public Class ClassPathResourceTest

        Private testDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            testDir = Path.Combine(Path.GetTempPath(), "dl4vb-cpr-" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(testDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(testDir) Then Directory.Delete(testDir, True)
        End Sub

        ''' <summary>https://github.com/eclipse/deeplearning4j/issues/6483</summary>
        <TestMethod>
        Public Sub testDirExtractingIntelliJ()
            Dim cpr As New ClassPathResource("somedir")
            assertTrue(cpr.exists())
            assertTrue(cpr.isDirectory())

            cpr.copyDirectory(testDir)

            Dim files() As String = Directory.GetFiles(testDir)
            assertEquals(1, files.Length)
            assertEquals("afile.txt", Path.GetFileName(files(0)))
        End Sub

        <TestMethod>
        Public Sub testFileFromSearchDirectory()
            Dim cpr As New ClassPathResource("somedir/afile.txt")
            assertTrue(cpr.exists())
            assertFalse(cpr.isDirectory())
            assertEquals("afile.txt", cpr.Filename)
            assertEquals("class path resource [somedir/afile.txt]", cpr.Description)
            assertTrue(File.Exists(cpr.File))
            assertTrue(cpr.URI.IsFile)
            assertEquals(New FileInfo(cpr.File).Length, cpr.contentLength())
            assertTrue(cpr.lastModified() > 0)
            Using r As New StreamReader(cpr.InputStream)
                assertEquals("a file inside somedir", r.ReadLine())
            End Using
            ' leading slash and backslashes are tolerated
            assertTrue(New ClassPathResource("/somedir\afile.txt").exists())
            assertEquals(cpr, New ClassPathResource("somedir/afile.txt"))
            assertEquals(cpr.GetHashCode(), New ClassPathResource("somedir/afile.txt").GetHashCode())
        End Sub

        <TestMethod>
        Public Sub testEmbeddedResource()
            Dim cpr As New ClassPathResource("embedded/hello.txt", Assembly.GetExecutingAssembly())
            assertTrue(cpr.exists())
            Using r As New StreamReader(cpr.InputStream)
                assertEquals("embedded hello", r.ReadLine())
            End Using
            ' no file on disk: the content is extracted to a temporary file on demand
            Dim extracted As String = cpr.File
            assertTrue(File.Exists(extracted))
            assertEquals("hello.txt", Path.GetFileName(extracted))
            assertEquals("embedded hello" & ControlChars.Lf, File.ReadAllText(extracted))
            Dim inDir As String = cpr.getTempFileFromArchive(testDir)
            assertEquals(Path.Combine(testDir, "hello.txt"), inDir)
            assertTrue(cpr.URI.ToString().StartsWith("resource://"))
            ' also found without naming the assembly, through the assemblies known to ND4VBClassLoading
            assertTrue(New ClassPathResource("embedded/hello.txt").exists())
        End Sub

        <TestMethod>
        Public Sub testMissingResource()
            Dim cpr As New ClassPathResource("does/not/exist.txt")
            assertFalse(cpr.exists())
            assertThrows(Of FileNotFoundException)(Function() cpr.InputStream)
            assertThrows(Of FileNotFoundException)(Function() cpr.File)
            assertThrows(Of FileNotFoundException)(Function() cpr.URI)
            assertThrows(Of InvalidOperationException)(Sub() cpr.copyDirectory(testDir))
        End Sub

        <TestMethod>
        Public Sub testAddedSearchDirectoryAndRelative()
            Dim sub1 As String = Path.Combine(testDir, "extra", "nested")
            Directory.CreateDirectory(sub1)
            File.WriteAllText(Path.Combine(sub1, "one.txt"), "1")
            File.WriteAllText(Path.Combine(sub1, "two.txt"), "2")
            ClassPathResource.addSearchDirectory(testDir)
            Try
                Dim one As New ClassPathResource("extra/nested/one.txt")
                assertTrue(one.exists())
                Dim two = one.createRelative("two.txt")
                assertEquals("extra/nested/two.txt", DirectCast(two, ClassPathResource).Path)
                assertTrue(two.exists())
                assertEquals("2", File.ReadAllText(two.File))
            Finally
                ClassPathResource.removeSearchDirectory(testDir)
            End Try
            assertFalse(New ClassPathResource("extra/nested/one.txt").exists())
        End Sub

        <TestMethod>
        Public Sub testCleanPath()
            assertEquals("a/b/c.txt", ClassPathResource.CleanPath("/a/./b/../b/c.txt"))
            assertEquals("a/b/", ClassPathResource.CleanPath("\a\b\"))
            assertEquals("", ClassPathResource.CleanPath("/"))
        End Sub

    End Class

End Namespace
