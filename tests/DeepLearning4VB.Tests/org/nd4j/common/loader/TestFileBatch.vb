Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4j.common.loader

Namespace org.nd4j.common.loader

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class TestFileBatch

        Private testDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            testDir = Path.Combine(Path.GetTempPath(), "dl4vb-filebatch-" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(testDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(testDir) Then
                Directory.Delete(testDir, True)
            End If
        End Sub

        <TestMethod>
        Public Sub testFileBatch()
            Dim fileList As IList(Of String) = New List(Of String)()
            For i As Integer = 0 To 9
                Dim s As String = "File contents - file " & i
                Dim f As String = Path.Combine(testDir, "origFile" & i & ".txt")
                File.WriteAllText(f, s, New UTF8Encoding(False))
                fileList.Add(f)
            Next i

            Dim fb As FileBatch = FileBatch.forFiles(fileList)

            assertEquals(10, fb.getFileBytes().Count)
            assertEquals(10, fb.getOriginalUris().Count)
            For i As Integer = 0 To 9
                Dim expBytes() As Byte = Encoding.UTF8.GetBytes("File contents - file " & i)
                Dim actBytes() As Byte = fb.getFileBytes()(i)
                assertArrayEquals(expBytes, actBytes)

                Dim expPath As String = New Uri(Path.GetFullPath(fileList(i))).AbsoluteUri
                Dim actPath As String = fb.getOriginalUris()(i)
                assertEquals(expPath, actPath)
            Next i

            'Save and load:
            Dim baos As New MemoryStream()
            fb.writeAsZip(baos)
            Dim asBytes() As Byte = baos.ToArray()

            Dim fb2 As FileBatch
            Using bais As New MemoryStream(asBytes)
                fb2 = FileBatch.readFromZip(bais)
            End Using

            assertEquals(fb.getOriginalUris(), fb2.getOriginalUris())
            assertEquals(10, fb2.getFileBytes().Count)
            For i As Integer = 0 To 9
                assertArrayEquals(fb.getFileBytes()(i), fb2.getFileBytes()(i))
            Next i
            assertEquals(fb, fb2)
            assertEquals(fb.GetHashCode(), fb2.GetHashCode())

            'Check that it is indeed a valid zip file:
            Dim f2 As String = Path.Combine(testDir, "testfile.zip")
            fb.writeAsZip(f2)

            Dim names As ISet(Of String) = New HashSet(Of String)()
            Using zf As ZipArchive = ZipFile.OpenRead(f2)
                For Each entry As ZipArchiveEntry In zf.Entries
                    names.Add(entry.FullName)
                Next entry
            End Using

            assertEquals(11, names.Count) '10 files, 1 "original file names" file
            assertTrue(names.Contains(FileBatch.ORIGINAL_PATHS_FILENAME))
            For i As Integer = 0 To 9
                Dim n As String = "file_" & i & ".txt"
                assertTrue(names.Contains(n), n)
            Next i

            ' and it can be read back from the file as well
            Dim fb3 As FileBatch = FileBatch.readFromZip(f2)
            assertEquals(fb, fb3)
        End Sub

        <TestMethod>
        Public Sub testFilesWithoutExtensionAreStoredAsBin()
            Dim f As String = Path.Combine(testDir, "noextension")
            File.WriteAllBytes(f, New Byte() {1, 2, 3})
            Dim fb As FileBatch = FileBatch.forFiles(f)
            Using ms As New MemoryStream()
                fb.writeAsZip(ms)
                Using zf As New ZipArchive(New MemoryStream(ms.ToArray()), ZipArchiveMode.Read)
                    assertNotNull(zf.GetEntry("file_0.bin"))
                End Using
            End Using
        End Sub

        <TestMethod>
        Public Sub testReadFromZipRejectsForeignZip()
            Using ms As New MemoryStream()
                Using zip As New ZipArchive(ms, ZipArchiveMode.Create, True)
                    Using es = zip.CreateEntry("file_0.txt").Open()
                        es.WriteByte(65)
                    End Using
                End Using
                assertThrows(Of IOException)(Sub() FileBatch.readFromZip(New MemoryStream(ms.ToArray())))
            End Using
        End Sub

    End Class

End Namespace
