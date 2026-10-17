Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.util

Namespace org.nd4vb.common.resources

    ''' <summary>Ported from the DL4J JUnit test of the same name, plus tar/tar.gz/gz coverage of the built-in tar reader.</summary>
    <TestClass>
    Public Class TestArchiveUtils

        Private testDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            testDir = Path.Combine(Path.GetTempPath(), "dl4vb-archive-" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(testDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(testDir) Then Directory.Delete(testDir, True)
        End Sub

        <TestMethod>
        Public Sub testUnzipFileTo()
            'random txt file
            Dim content As String = "test file content"
            Dim testFile As String = Path.Combine(testDir, "myDir", "myTestFile.txt")
            Directory.CreateDirectory(Path.GetDirectoryName(testFile))
            File.WriteAllText(testFile, content, New UTF8Encoding(False))

            'zip it as test.zip
            Dim zipPath As String = Path.Combine(Path.GetDirectoryName(testFile), "test.zip")
            Using zip As New ZipArchive(New FileStream(zipPath, FileMode.Create), ZipArchiveMode.Create)
                zip.CreateEntryFromFile(testFile, Path.GetFileName(testFile))
            End Using

            'now unzip to a directory that doesn't previously exist
            Dim unzipDir As String = Path.Combine(Path.GetDirectoryName(testFile), "unzipTo")
            ArchiveUtils.unzipFileTo(zipPath, unzipDir)
            assertEquals(content, File.ReadAllText(Path.Combine(unzipDir, "myTestFile.txt")))
            ' the Java implementation deletes the archive once extracted
            assertFalse(File.Exists(zipPath))
        End Sub

        <TestMethod>
        Public Sub testZipListAndExtractSingle()
            Dim zipPath As String = Path.Combine(testDir, "list.zip")
            Using zip As New ZipArchive(New FileStream(zipPath, FileMode.Create), ZipArchiveMode.Create)
                WriteEntry(zip, "a.txt", "A")
                WriteEntry(zip, "sub/b.txt", "B")
            End Using
            Dim names As IList(Of String) = ArchiveUtils.zipListFiles(zipPath)
            assertEquals(New String() {"a.txt", "sub/b.txt"}, names)
            Dim dest As String = Path.Combine(testDir, "b-only.txt")
            ArchiveUtils.zipExtractSingleFile(zipPath, dest, "sub/b.txt")
            assertEquals("B", File.ReadAllText(dest))
            assertThrows(Of FileNotFoundException)(Sub() ArchiveUtils.zipExtractSingleFile(zipPath, dest, "missing.txt"))
        End Sub

        <TestMethod>
        Public Sub testZipSlipIsRejected()
            Dim zipPath As String = Path.Combine(testDir, "evil.zip")
            Using zip As New ZipArchive(New FileStream(zipPath, FileMode.Create), ZipArchiveMode.Create)
                WriteEntry(zip, "../outside.txt", "X")
            End Using
            Dim dest As String = Path.Combine(testDir, "out")
            assertThrows(Of IOException)(Sub() ArchiveUtils.unzipFileTo(zipPath, dest, False))
            assertFalse(File.Exists(Path.Combine(testDir, "outside.txt")))
        End Sub

        <TestMethod>
        Public Sub testTarAndTarGz()
            Dim tarPath As String = Path.Combine(testDir, "data.tar")
            Using fs As New FileStream(tarPath, FileMode.Create)
                WriteTarEntry(fs, "dir/", "")
                WriteTarEntry(fs, "dir/hello.txt", "hello tar")
                WriteTarEntry(fs, "root.txt", New String("z"c, 1000)) ' spans two 512-byte blocks
                fs.Write(New Byte(1023) {}, 0, 1024) ' end-of-archive marker
            End Using
            assertEquals(New String() {"dir/", "dir/hello.txt", "root.txt"}, ArchiveUtils.tarListFiles(tarPath))

            ' gzip the tar and verify the .tar.gz paths too
            Dim tgzPath As String = Path.Combine(testDir, "data.tar.gz")
            Using input As New FileStream(tarPath, FileMode.Open)
                Using output As New GZipStream(New FileStream(tgzPath, FileMode.Create), CompressionMode.Compress)
                    input.CopyTo(output)
                End Using
            End Using
            assertEquals(3, ArchiveUtils.tarGzListFiles(tgzPath).Count)
            assertThrows(Of InvalidOperationException)(Function() ArchiveUtils.tarListFiles(tgzPath))

            Dim singleFile As String = Path.Combine(testDir, "hello-extracted.txt")
            ArchiveUtils.tarGzExtractSingleFile(tgzPath, singleFile, "dir/hello.txt")
            assertEquals("hello tar", File.ReadAllText(singleFile))
            assertThrows(Of InvalidOperationException)(Sub() ArchiveUtils.tarGzExtractSingleFile(tgzPath, singleFile, "nope"))

            Dim dest As String = Path.Combine(testDir, "untar")
            ArchiveUtils.unzipFileTo(tgzPath, dest, False)
            assertEquals("hello tar", File.ReadAllText(Path.Combine(dest, "dir", "hello.txt")))
            assertEquals(1000, File.ReadAllText(Path.Combine(dest, "root.txt")).Length)
            assertFalse(File.Exists(tgzPath))

            Dim dest2 As String = Path.Combine(testDir, "untar2")
            ArchiveUtils.unzipFileTo(tarPath, dest2, True)
            assertTrue(File.Exists(Path.Combine(dest2, "root.txt")))
        End Sub

        <TestMethod>
        Public Sub testGzSingleFile()
            Dim gzPath As String = Path.Combine(testDir, "note.txt.gz")
            Using output As New GZipStream(New FileStream(gzPath, FileMode.Create), CompressionMode.Compress)
                Dim bytes = Encoding.UTF8.GetBytes("gzipped note")
                output.Write(bytes, 0, bytes.Length)
            End Using
            ArchiveUtils.unzipFileTo(gzPath, testDir, False)
            assertEquals("gzipped note", File.ReadAllText(Path.Combine(testDir, "note.txt")))
            assertFalse(File.Exists(gzPath))
            assertThrows(Of InvalidOperationException)(Sub() ArchiveUtils.unzipFileTo(Path.Combine(testDir, "note.txt"), testDir, False))
            assertThrows(Of ArgumentException)(Sub() ArchiveUtils.unzipFileTo(Path.Combine(testDir, "missing.zip"), testDir, False))
        End Sub

        Private Shared Sub WriteEntry(ByVal zip As ZipArchive, ByVal name As String, ByVal content As String)
            Using s As Stream = zip.CreateEntry(name).Open()
                Dim bytes = Encoding.UTF8.GetBytes(content)
                s.Write(bytes, 0, bytes.Length)
            End Using
        End Sub

        ''' <summary>Writes a minimal ustar entry (header block + content padded to 512 bytes).</summary>
        Private Shared Sub WriteTarEntry(ByVal fs As Stream, ByVal name As String, ByVal content As String)
            Dim data = Encoding.UTF8.GetBytes(content)
            Dim header(511) As Byte
            Encoding.ASCII.GetBytes(name).CopyTo(header, 0)
            Encoding.ASCII.GetBytes("0000644" & ChrW(0)).CopyTo(header, 100)
            Encoding.ASCII.GetBytes("0000000" & ChrW(0)).CopyTo(header, 108)
            Encoding.ASCII.GetBytes("0000000" & ChrW(0)).CopyTo(header, 116)
            Encoding.ASCII.GetBytes(Convert.ToString(data.Length, 8).PadLeft(11, "0"c) & ChrW(0)).CopyTo(header, 124)
            Encoding.ASCII.GetBytes("00000000000" & ChrW(0)).CopyTo(header, 136)
            header(156) = If(name.EndsWith("/"), AscW("5"c), AscW("0"c))
            Encoding.ASCII.GetBytes("ustar" & ChrW(0) & "00").CopyTo(header, 257)
            ' checksum: sum of header bytes with the checksum field treated as spaces
            For i As Integer = 148 To 155
                header(i) = 32
            Next
            Dim sum As Integer = 0
            For Each b In header
                sum += b
            Next
            Encoding.ASCII.GetBytes(Convert.ToString(sum, 8).PadLeft(6, "0"c) & ChrW(0) & " ").CopyTo(header, 148)
            fs.Write(header, 0, 512)
            fs.Write(data, 0, data.Length)
            Dim pad As Integer = (512 - data.Length Mod 512) Mod 512
            fs.Write(New Byte(pad - 1 + If(pad = 0, 1, 0)) {}, 0, pad)
        End Sub

    End Class

End Namespace
