Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB
Imports DeepLearning4VB.org.nd4j.common.config
Imports ResourcesApi = DeepLearning4VB.org.nd4j.common.resources.Resources
Imports DeepLearning4VB.org.nd4j.common.resources.strumpf

Namespace org.nd4j.common.resources

    ''' <summary>
    ''' Ported from the DL4J JUnit test of the same name (which was disabled upstream because it needed
    ''' network access). Here the "remote" file is a local file:// URL, so everything runs offline.
    ''' </summary>
    <TestClass>
    Public Class TestStrumpf

        Private testDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            testDir = Path.Combine(Path.GetTempPath(), "dl4vb-strumpf-" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(testDir)
            ResourcesApi.reset()
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            JavaSystem.clearProperty(ND4JSystemProperties.RESOURCES_LOCAL_DIRS)
            JavaSystem.clearProperty(ND4JSystemProperties.RESOURCES_CACHE_DIR)
            ResourcesApi.reset()
            If Directory.Exists(testDir) Then Directory.Delete(testDir, True)
        End Sub

        <TestMethod>
        Public Sub testResolvingActual()
            ' data/irisSvmLight.txt is a test resource copied next to the test assembly
            Dim f As String = ResourcesApi.asFile("data/irisSvmLight.txt")
            assertTrue(File.Exists(f))
            Dim count As Integer = 0
            Using r As New StreamReader(f)
                Do While r.ReadLine() IsNot Nothing
                    count += 1
                Loop
            End Using
            assertEquals(12, count) 'Iris normally has 150 examples; this is subset with 12
            assertTrue(ResourcesApi.exists("data/irisSvmLight.txt"))
            assertFalse(ResourcesApi.exists("data/nope.txt"))
            Using s As Stream = ResourcesApi.asStream("data/irisSvmLight.txt")
                assertTrue(s.ReadByte() >= 0)
            End Using
            assertThrows(Of InvalidOperationException)(Function() ResourcesApi.asFile("data/nope.txt"))
            assertEquals("x/y", ResourcesApi.normalizePath("x/y.resource_reference"))
        End Sub

        <TestMethod>
        Public Sub testResolveLocal()
            Dim content As String = "test file content"
            Dim relPath As String = "myDir/myTestFile.txt"
            Dim testFile As String = Path.Combine(testDir, "myDir", "myTestFile.txt")
            Directory.CreateDirectory(Path.GetDirectoryName(testFile))
            File.WriteAllText(testFile, content, New UTF8Encoding(False))

            JavaSystem.setProperty(ND4JSystemProperties.RESOURCES_LOCAL_DIRS, testDir)
            Dim r As New StrumpfResolver()
            assertTrue(r.exists(relPath))
            assertTrue(r.directoryExists("myDir"))
            Dim f As String = r.asFile(relPath)
            assertTrue(File.Exists(f))
            assertEquals(Path.GetFullPath(testFile), Path.GetFullPath(f))
            assertEquals(content, File.ReadAllText(f, Encoding.UTF8))
            assertTrue(r.hasLocalCache())
            assertEquals(100, r.priority())

            Dim dest As String = Path.Combine(testDir, "copy")
            Directory.CreateDirectory(dest)
            r.copyDirectory("myDir", dest)
            assertEquals(content, File.ReadAllText(Path.Combine(dest, "myTestFile.txt")))
            assertThrows(Of InvalidOperationException)(Function() r.asFile("missing.txt"))
        End Sub

        ''' <summary>A reference file pointing at a gzipped local "remote" file is downloaded, verified and cached.</summary>
        <TestMethod>
        Public Sub testResourceReferenceDownloadAndCache()
            Dim content As String = "the real resource content"
            Dim remoteGz As String = Path.Combine(testDir, "remote", "big.txt.gz")
            Directory.CreateDirectory(Path.GetDirectoryName(remoteGz))
            Using output As New GZipStream(New FileStream(remoteGz, FileMode.Create), CompressionMode.Compress)
                Dim bytes = Encoding.UTF8.GetBytes(content)
                output.Write(bytes, 0, bytes.Length)
            End Using
            Dim plain As String = Path.Combine(testDir, "remote", "big.txt")
            File.WriteAllText(plain, content, New UTF8Encoding(False))

            Dim localDir As String = Path.Combine(testDir, "local")
            Directory.CreateDirectory(Path.Combine(localDir, "data"))
            Dim refPath As String = Path.Combine(localDir, "data", "big.txt" & StrumpfResolver.REF)
            Dim json As String = "{""current_version"": 2, ""v1"": {" &
                """data/big.txt_hash"": """ & ResourceFile.sha256(plain) & """, " &
                """data/big.txt_compressed_hash"": """ & ResourceFile.sha256(remoteGz) & """, " &
                """full_remote_path"": """ & New Uri(remoteGz).AbsoluteUri & """ } }"
            File.WriteAllText(refPath, json)

            Dim rf As ResourceFile = ResourceFile.fromFile(refPath)
            assertEquals(2, rf.current_version)
            assertEquals("data/big.txt", rf.relativePath())
            assertEquals(refPath, rf.filePath)

            Dim cache As String = Path.Combine(testDir, "cache")
            JavaSystem.setProperty(ND4JSystemProperties.RESOURCES_LOCAL_DIRS, localDir)
            JavaSystem.setProperty(ND4JSystemProperties.RESOURCES_CACHE_DIR, cache)
            Dim r As New StrumpfResolver()
            assertTrue(r.exists("data/big.txt"))
            assertEquals(cache, r.localCacheRoot())

            Dim resolved As String = r.asFile("data/big.txt")
            assertEquals(content, File.ReadAllText(resolved))
            assertTrue(resolved.StartsWith(cache), resolved)
            assertTrue(resolved.Replace("\"c, "/"c).EndsWith("data/big.txt__v2/big.txt"), resolved)
            assertTrue(rf.localFileExistsAndValid(cache))

            ' a corrupted cache entry is detected and re-fetched
            File.WriteAllText(resolved, "corrupted")
            assertFalse(rf.localFileExistsAndValid(cache))
            assertEquals(content, File.ReadAllText(r.asFile("data/big.txt")))

            ' copyDirectory replaces reference files with the real files
            Dim dest As String = Path.Combine(testDir, "copied")
            Directory.CreateDirectory(dest)
            r.copyDirectory("data", dest)
            assertEquals(content, File.ReadAllText(Path.Combine(dest, "big.txt")))
            assertFalse(File.Exists(Path.Combine(dest, "big.txt" & StrumpfResolver.REF)))

            ' and the static Resources facade sees the same thing after a reset
            ResourcesApi.reset()
            assertEquals(content, File.ReadAllText(ResourcesApi.asFile("data/big.txt")))
        End Sub

        <TestMethod>
        Public Sub testResourceFileErrors()
            Dim refPath As String = Path.Combine(testDir, "bad.resource_reference")
            File.WriteAllText(refPath, "{""current_version"": 1, ""v1"": {""full_remote_path"": ""file:///nowhere""}}")
            Dim rf As ResourceFile = ResourceFile.fromFile(refPath)
            assertThrows(Of InvalidOperationException)(Function() rf.relativePath())
            assertThrows(Of InvalidOperationException)(Function() ResourceFile.fromFile(Path.Combine(testDir, "missing.resource_reference")))
        End Sub

    End Class

End Namespace
