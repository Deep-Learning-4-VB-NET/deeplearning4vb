Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Text.Json
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4j.common.holder
Imports DeepLearning4VB.org.nd4j.common.primitives
Imports DeepLearning4VB.org.nd4j.common.validation

Namespace org.nd4j.common.validation

    <TestClass>
    Public Class ValidationTests

        Private testDir As String

        <TestInitialize>
        Public Sub CreateTempDir()
            testDir = Path.Combine(Path.GetTempPath(), "dl4vb-validation-" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(testDir)
        End Sub

        <TestCleanup>
        Public Sub DeleteTempDir()
            If Directory.Exists(testDir) Then Directory.Delete(testDir, True)
        End Sub

        <TestMethod>
        Public Sub ValidationResult_BuilderAndToString()
            Dim vr = ValidationResult.builder().valid(False).formatType("JSON").path("c:\x.json").issues("first", "second").build()
            assertFalse(vr.Valid)
            assertEquals("JSON", vr.getFormatType())
            assertEquals(2, vr.getIssues().Count)
            Dim text As String = vr.ToString()
            assertTrue(text.StartsWith("Format type: JSON" & ControlChars.Lf & "Path: c:\x.json" & ControlChars.Lf & "Format valid: false" & ControlChars.Lf & "Issues:" & ControlChars.Lf & "- first" & ControlChars.Lf & "- second"), text)

            Dim singleIssue = ValidationResult.builder().valid(True).issues("only" & ControlChars.Lf & "two lines").formatClass(GetType(String)).exception(New InvalidOperationException("boom")).build()
            text = singleIssue.ToString()
            assertTrue(text.Contains("Format class: System.String"))
            assertTrue(text.Contains("Issue: only" & ControlChars.Lf & "       two lines"))
            assertTrue(text.Contains("Stack Trace:"))
            assertTrue(text.Contains("boom"))
            assertEquals(vr, ValidationResult.builder().valid(False).formatType("JSON").path("c:\x.json").issues("first", "second").build())
        End Sub

        <TestMethod>
        Public Sub Validator_Files()
            Dim missing As String = Path.Combine(testDir, "missing.bin")
            Dim vr = Nd4jCommonValidator.isValidFile(missing)
            assertFalse(vr.Valid)
            assertEquals("File does not exist", vr.Issues(0))
            assertEquals(Path.GetFullPath(missing), vr.Path)

            Dim empty As String = Path.Combine(testDir, "empty.bin")
            File.WriteAllBytes(empty, New Byte() {})
            assertFalse(Nd4jCommonValidator.isValidFile(empty).Valid)
            assertEquals("File is empty (length 0)", Nd4jCommonValidator.isValidFile(empty).Issues(0))
            assertNull(Nd4jCommonValidator.isValidFile(empty, "Any", True))

            assertFalse(Nd4jCommonValidator.isValidFile(testDir).Valid)
            assertEquals("Specified path is a directory", Nd4jCommonValidator.isValidFile(testDir).Issues(0))

            Dim ok As String = Path.Combine(testDir, "ok.bin")
            File.WriteAllBytes(ok, New Byte() {1})
            Dim good = Nd4jCommonValidator.isValidFile(ok)
            assertTrue(good.Valid)
            assertEquals("File", good.FormatType)
            assertNull(Nd4jCommonValidator.getPath(Nothing))
        End Sub

        <TestMethod>
        Public Sub Validator_Json()
            assertTrue(Nd4jCommonValidator.isValidJSON("{""a"": [1, 2, {""b"": null}]}").Valid)
            Dim bad = Nd4jCommonValidator.isValidJSON("{""a"": ")
            assertFalse(bad.Valid)
            assertEquals("File does not appear to be valid JSON", bad.Issues(0))
            assertNotNull(bad.Exception)
            ' the Java version parsed into a Map, so a JSON array is not accepted either
            assertFalse(Nd4jCommonValidator.isValidJSON("[1, 2]").Valid)

            Dim f As String = Path.Combine(testDir, "doc.json")
            File.WriteAllText(f, "{""x"": 1}", New UTF8Encoding(False))
            Dim vr = Nd4jCommonValidator.isValidJsonUTF8(f)
            assertTrue(vr.Valid)
            assertEquals("JSON", vr.FormatType)
            assertEquals(Path.GetFullPath(f), vr.Path)
            File.WriteAllText(f, "not json")
            assertFalse(Nd4jCommonValidator.isValidJson(f, Encoding.UTF8).Valid)
            assertFalse(Nd4jCommonValidator.isValidJsonUTF8(Path.Combine(testDir, "nope.json")).Valid)
        End Sub

        <TestMethod>
        Public Sub Validator_Zip()
            Dim zipPath As String = Path.Combine(testDir, "a.zip")
            Using zip As New ZipArchive(New FileStream(zipPath, FileMode.Create), ZipArchiveMode.Create)
                Using s = zip.CreateEntry("one.txt").Open()
                    s.WriteByte(65)
                End Using
                zip.CreateEntry("dir/two.txt")
            End Using
            assertTrue(Nd4jCommonValidator.isValidZipFile(zipPath, False).Valid)
            assertTrue(Nd4jCommonValidator.isValidZipFile(zipPath, False, "one.txt", "dir/two.txt").Valid)
            Dim missing = Nd4jCommonValidator.isValidZipFile(zipPath, False, "one.txt", "three.txt", "four.txt")
            assertFalse(missing.Valid)
            assertEquals("Zip file is missing 2 of 3 required entries: [three.txt, four.txt]", missing.Issues(0))

            Dim emptyZip As String = Path.Combine(testDir, "empty.zip")
            Using zip As New ZipArchive(New FileStream(emptyZip, FileMode.Create), ZipArchiveMode.Create)
            End Using
            assertFalse(Nd4jCommonValidator.isValidZipFile(emptyZip, False).Valid)
            assertEquals("Zip file is empty", Nd4jCommonValidator.isValidZipFile(emptyZip, False).Issues(0))
            assertTrue(Nd4jCommonValidator.isValidZipFile(emptyZip, True).Valid)

            Dim notZip As String = Path.Combine(testDir, "text.zip")
            File.WriteAllText(notZip, "this is not a zip file")
            Dim corrupt = Nd4jCommonValidator.isValidZipFile(notZip, False)
            assertFalse(corrupt.Valid)
            assertTrue(corrupt.Issues(0).StartsWith("File does not appear to be valid zip file"))
        End Sub

        Public Class Sample
            Public Name As String
            Public Count As Integer
            Public Missing As String
            Public Property Ratio As Double
            Public Property Score As AtomicDouble
            Public Property Flag As AtomicBoolean
        End Class

        <TestMethod>
        Public Sub ObjectMapperHolder_SerializesFieldsAndSkipsNulls()
            Dim s As New Sample With {.Name = "n", .Count = 3, .Ratio = 0.5, .Score = New AtomicDouble(2.5), .Flag = New AtomicBoolean(True)}
            Dim json As String = JsonSerializer.Serialize(s, ObjectMapperHolder.JsonMapper)
            assertTrue(json.Contains("""Name"":""n"""), json)
            assertTrue(json.Contains("""Count"":3"), json)
            assertFalse(json.Contains("Missing"), json)
            ' AtomicDouble/AtomicBoolean serialise through their Value property (VB cannot implement STJ converters)
            assertTrue(json.Contains("""Score"":{""Value"":2.5}"), json)
            assertTrue(json.Contains("""Flag"":{""Value"":true}"), json)
            Dim back = JsonSerializer.Deserialize(Of Sample)(json, ObjectMapperHolder.JsonMapper)
            assertEquals("n", back.Name)
            assertEquals(3, back.Count)
            assertEquals(2.5, back.Score.[get](), 0.0)
            assertTrue(back.Flag.[get]())
            Dim lower = JsonSerializer.Deserialize(Of Sample)("{""name"":""x"",""count"":7}", ObjectMapperHolder.JsonMapper)
            assertEquals("x", lower.Name)
            assertEquals(7, lower.Count)
        End Sub

    End Class

End Namespace
