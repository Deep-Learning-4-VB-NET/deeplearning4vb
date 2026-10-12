Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB
Imports DeepLearning4VB.JavaCompat
Imports DeepLearning4VB.org.deeplearning4j.common.config
Imports DeepLearning4VB.org.deeplearning4j.common.resources
Imports DeepLearning4VB.org.deeplearning4j.common.util
Imports DeepLearning4VB.org.nd4j.common.collections
Imports DeepLearning4VB.org.nd4j.common.primitives
Imports DeepLearning4VB.org.nd4j.common.resources
Imports DeepLearning4VB.org.nd4j.common.util

Namespace org.nd4j.common.util

    <TestClass>
    Public Class SerializationUtilsTest

        <Serializable>
        Public Class Payload
            Public Name As String
            Public Values As Integer()
        End Class

        <TestMethod>
        Public Sub RoundTripsThroughBytesStreamsAndFiles()
            Dim p As New Payload With {.Name = "x", .Values = New Integer() {1, 2, 3}}
            Dim bytes As Byte() = SerializationUtils.toByteArray(p)
            Dim back As Payload = SerializationUtils.fromByteArray(Of Payload)(bytes)
            assertEquals("x", back.Name)
            assertArrayEquals(p.Values, back.Values)
            assertEquals("x", SerializationUtils.deserialize(Of Payload)(bytes).Name)

            Using ms As New MemoryStream()
                SerializationUtils.serialize(p, ms)
                ms.Position = 0
                assertEquals("x", SerializationUtils.deserialize(Of Payload)(ms).Name)
            End Using

            Dim file As String = Path.Combine(Path.GetTempPath(), "dl4vb-ser-" & Guid.NewGuid().ToString("N"), "p.bin")
            Try
                SerializationUtils.saveObject(p, file)
                assertEquals("x", SerializationUtils.readObject(Of Payload)(file).Name)
                ' an Atomic survives serialization and is usable afterwards
                SerializationUtils.saveObject(New Atomic(Of String)("v"), file)
                assertEquals("v", SerializationUtils.readObject(Of Atomic(Of String))(file).[get]())
            Finally
                Directory.Delete(Path.GetDirectoryName(file), True)
            End Try
            assertThrows(Of InvalidOperationException)(Function() SerializationUtils.readObject(Of Payload)("Z:\does\not\exist.bin"))
            assertThrows(Of InvalidOperationException)(Function() SerializationUtils.fromByteArray(Of Payload)(New Byte() {1, 2, 3}))
        End Sub

    End Class

    <TestClass>
    Public Class NioUtilTest

        <TestMethod>
        Public Sub CopiesStridedElements()
            Dim from(4 * 6 - 1) As Byte
            For i As Integer = 0 To 5
                BitConverter.GetBytes(i * 10).CopyTo(from, i * 4)
            Next
            Dim [to](4 * 6 - 1) As Byte
            ' every second int from offset 1 -> consecutive ints from offset 0
            NioUtil.copyAtStride(3, NioUtil.BufferType.INT, from, 1, 2, [to], 0, 1)
            assertEquals(10, BitConverter.ToInt32([to], 0))
            assertEquals(30, BitConverter.ToInt32([to], 4))
            assertEquals(50, BitConverter.ToInt32([to], 8))

            Dim d(8 * 3 - 1) As Byte
            BitConverter.GetBytes(1.5).CopyTo(d, 0)
            BitConverter.GetBytes(2.5).CopyTo(d, 8)
            Dim d2(8 * 4 - 1) As Byte
            NioUtil.copyAtStride(2, NioUtil.BufferType.DOUBLE, d, 0, 1, d2, 1, 2)
            assertEquals(1.5, BitConverter.ToDouble(d2, 8), 0.0)
            assertEquals(2.5, BitConverter.ToDouble(d2, 24), 0.0)
            assertThrows(Of ArgumentNullException)(Sub() NioUtil.copyAtStride(1, NioUtil.BufferType.FLOAT, Nothing, 0, 1, d2, 0, 1))
        End Sub

    End Class

    <TestClass>
    Public Class WeakIdentityHashMapTest

        <TestMethod>
        Public Sub KeysAreComparedByIdentity()
            Dim m As New WeakIdentityHashMap(Of String, Integer)()
            Dim a As New String("key".ToCharArray())
            Dim b As New String("key".ToCharArray())
            assertFalse(a Is b)
            assertEquals(0, m.put(a, 1))
            assertEquals(1, m.put(a, 2))
            m(b) = 3
            assertEquals(2, m.size())
            assertEquals(2, m.[get](a))
            assertEquals(3, m(b))
            assertEquals(0, m.[get]("other"))
            assertTrue(m.ContainsKey(a))
            assertTrue(m.containsValue(3))
            assertTrue(m.keySet().Contains(a) AndAlso m.keySet().Contains(b))
            assertEquals(2, m.entrySet().Count)
            Dim v As Integer
            assertTrue(m.TryGetValue(b, v))
            assertEquals(3, v)
            assertEquals(2, m.removeAndGet(a))
            assertFalse(m.Remove(a))
            assertEquals(1, m.Count)
            m.Clear()
            assertTrue(m.isEmpty())
            assertThrows(Of ArgumentNullException)(Function() m.put(Nothing, 1))
        End Sub

        <TestMethod>
        Public Sub CollectedKeysDisappear()
            Dim m As New WeakIdentityHashMap(Of Object, String)()
            Dim strong As New Object()
            m.put(strong, "kept")
            AddTemporaryKey(m)
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            assertEquals(1, m.size())
            assertEquals("kept", m.[get](strong))
            GC.KeepAlive(strong)
        End Sub

        Private Shared Sub AddTemporaryKey(ByVal m As WeakIdentityHashMap(Of Object, String))
            m.put(New Object(), "temporary")
        End Sub

    End Class

    <TestClass>
    Public Class TableTest

        <TestMethod>
        Public Sub HashBasedTable_Operations()
            Dim t = HashBasedTable(Of String, Integer, String).create()
            assertTrue(t.isEmpty())
            assertNull(t.put("r1", 1, "a"))
            assertEquals("a", t.put("r1", 1, "A"))
            t.put("r1", 2, "b")
            t.put("r2", 1, "c")
            assertEquals(3, t.size())
            assertEquals("A", t.[get]("r1", 1))
            assertNull(t.[get]("r9", 9))
            assertTrue(t.contains("r2", 1))
            assertFalse(t.contains("r2", 2))
            assertTrue(t.containsRow("r1") AndAlso t.containsColumn(2) AndAlso t.containsValue("c"))
            assertEquals(New Integer() {1, 2}, t.row("r1").Keys.ToArray())
            assertEquals(New String() {"r1", "r2"}, t.column(1).Keys.ToArray())
            assertEquals(New String() {"r1", "r2"}, t.rowKeySet().ToArray())
            assertEquals(New Integer() {1, 2}, t.columnKeySet().ToArray())
            assertEquals(New String() {"A", "b", "c"}, t.values().ToArray())
            assertEquals(3, t.cellSet().Count)
            assertEquals(2, t.rowMap().Count)
            assertEquals(2, t.columnMap().Count)
            assertEquals("c", t.remove("r2", 1))
            assertFalse(t.containsRow("r2"))
            assertNull(t.remove("r2", 1))

            Dim copy = HashBasedTable(Of String, Integer, String).create()
            copy.putAll(t)
            assertEquals(t.cellSet(), copy.cellSet())
            Dim sync As New SynchronizedTable(Of String, Integer, String)(copy)
            assertEquals("A", sync.[get]("r1", 1))
            assertEquals(2, sync.size())
            sync.clear()
            assertTrue(copy.isEmpty())
            assertEquals("(r1,1)=A", New Cell(Of String, Integer, String)("r1", 1, "A").ToString())
        End Sub

    End Class

    <TestClass>
    Public Class DownloaderTest

        Private testDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            testDir = Path.Combine(Path.GetTempPath(), "dl4vb-dl-" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(testDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(testDir) Then Directory.Delete(testDir, True)
        End Sub

        <TestMethod>
        Public Sub Md5AndLocalDownload()
            Dim source As String = Path.Combine(testDir, "source.txt")
            File.WriteAllText(source, "hello", New UTF8Encoding(False))
            ' MD5("hello")
            assertEquals("5d41402abc4b2a76b9719d911017c592", Downloader.md5Hex(source))
            assertTrue(Downloader.checkMD5OfFile("5D41402ABC4B2A76B9719D911017C592", source))
            assertFalse(Downloader.checkMD5OfFile("0000", source))

            Dim target As String = Path.Combine(testDir, "sub", "target.txt")
            ' as in Java, the target directory must already exist
            assertThrows(Of IOException)(Sub() Downloader.download("test", New Uri(source), target, "5d41402abc4b2a76b9719d911017c592", 3))
            Directory.CreateDirectory(Path.GetDirectoryName(target))
            Downloader.download("test", New Uri(source), target, "5d41402abc4b2a76b9719d911017c592", 3)
            assertEquals("hello", File.ReadAllText(target))
            assertFalse(File.Exists(Path.Combine(testDir, "sub", "inProgress.lock")))

            ' a wrong hash is retried and finally fails, leaving no file behind
            Dim bad As String = Path.Combine(testDir, "sub", "bad.txt")
            assertThrows(Of IOException)(Sub() Downloader.download("test", New Uri(source), bad, "ffff", 2))
            assertFalse(File.Exists(bad))
        End Sub

        <TestMethod>
        Public Sub DownloadAndExtractZip()
            Dim zipPath As String = Path.Combine(testDir, "archive.zip")
            Using zip As New System.IO.Compression.ZipArchive(New FileStream(zipPath, FileMode.Create), System.IO.Compression.ZipArchiveMode.Create)
                Using s = zip.CreateEntry("inner/file.txt").Open()
                    Dim bytes = Encoding.UTF8.GetBytes("zipped")
                    s.Write(bytes, 0, bytes.Length)
                End Using
            End Using
            Dim md5 As String = Downloader.md5Hex(zipPath)
            Dim downloaded As String = Path.Combine(testDir, "dl", "archive.zip")
            Dim extractTo As String = Path.Combine(testDir, "extracted")
            Directory.CreateDirectory(Path.GetDirectoryName(downloaded))
            Downloader.downloadAndExtract("archive", New Uri(zipPath), downloaded, extractTo, md5, 3)
            assertEquals("zipped", File.ReadAllText(Path.Combine(extractTo, "inner", "file.txt")))
        End Sub

    End Class

    <TestClass>
    Public Class DL4JCommonTest

        <TestCleanup>
        Public Sub Cleanup()
            JavaSystem.clearProperty(DL4JSystemProperties.DL4J_TEMP_DIR_PROPERTY)
            JavaSystem.clearProperty(DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY)
            DL4JResources.resetBaseDirectoryLocation()
            DL4JResources.BaseDownloadURL = "https://dl4jdata.blob.core.windows.net/"
        End Sub

        <TestMethod>
        Public Sub ResourceTypeNames()
            assertEquals("data", ResourceType.DATASET.resourceName())
            assertEquals("models", ResourceType.ZOO_MODEL.resourceName())
            assertEquals("resources", ResourceType.RESOURCE.resourceName())
            assertEquals(ResourceType.ZOO_MODEL, ResourceTypeValueOf("ZOO_MODEL"))
            assertThrows(Of ArgumentException)(Function() ResourceTypeValueOf("nope"))
        End Sub

        <TestMethod>
        Public Sub DL4JResources_UrlsAndDirectories()
            assertEquals("https://dl4jdata.blob.core.windows.net/", DL4JResources.BaseDownloadURL)
            assertEquals("https://dl4jdata.blob.core.windows.net/datasets/x.zip", DL4JResources.getURLString("/datasets/x.zip"))
            assertEquals("https://dl4jdata.blob.core.windows.net/datasets/x.zip", DL4JResources.getURL("datasets/x.zip").ToString())
            DL4JResources.setBaseDownloadURL("http://localhost:8080/")
            assertEquals("http://localhost:8080/a", DL4JResources.getURLString("a"))

            Dim defaultDir As String = DL4JResources.BaseDirectory
            assertTrue(defaultDir.EndsWith(".deeplearning4j"), defaultDir)
            assertTrue(Directory.Exists(defaultDir))

            Dim custom As String = Path.Combine(Path.GetTempPath(), "dl4vb-res-" & Guid.NewGuid().ToString("N"))
            Try
                JavaSystem.setProperty(DL4JSystemProperties.DL4J_RESOURCES_DIR_PROPERTY, custom)
                DL4JResources.resetBaseDirectoryLocation()
                assertEquals(custom, DL4JResources.BaseDirectory)
                Dim d As String = DL4JResources.getDirectory(ResourceType.ZOO_MODEL, "lenet")
                assertEquals(Path.Combine(custom, "models", "lenet"), d)
                assertTrue(Directory.Exists(d))
                assertThrows(Of InvalidOperationException)(Sub() DL4JResources.setBaseDirectory(Path.Combine(custom, "missing")))
            Finally
                Directory.Delete(custom, True)
            End Try
        End Sub

        <TestMethod>
        Public Sub DL4JFileUtils_HonoursTempDirProperty()
            Dim custom As String = Path.Combine(Path.GetTempPath(), "dl4vb-tmp2-" & Guid.NewGuid().ToString("N"))
            JavaSystem.setProperty(DL4JSystemProperties.DL4J_TEMP_DIR_PROPERTY, custom)
            Try
                Dim f As String = DL4JFileUtils.createTempFile("dl4j", ".tmp")
                assertTrue(File.Exists(f))
                assertTrue(f.StartsWith(custom))
            Finally
                Directory.Delete(custom, True)
            End Try
            JavaSystem.clearProperty(DL4JSystemProperties.DL4J_TEMP_DIR_PROPERTY)
            Dim g As String = DL4JFileUtils.createTempFile("dl4j", Nothing)
            assertTrue(g.StartsWith(Path.GetTempPath()))
            File.Delete(g)
        End Sub

    End Class

End Namespace
