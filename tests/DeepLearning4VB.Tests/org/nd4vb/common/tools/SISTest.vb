Imports System
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.tools

Namespace org.nd4vb.common.tools

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class SISTest
        Private sis As SIS
        Private tmpFld As String

        <TestInitialize>
        Public Sub CreateTempDir()
            tmpFld = Path.Combine(Path.GetTempPath(), "dl4vb-sis-" & Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tmpFld)
        End Sub

        Private console As StringWriter

        <TestMethod>
        Public Sub testAll()
            sis = New SIS()
            console = New StringWriter()
            Dim mtLv As Integer = 0
            sis.initValues(mtLv, "TEST", console, console, tmpFld, "Test", "ABC", True, True)
            Dim fFName As String = sis.getfullFileName()
            sis.info(fFName)
            sis.info("aaabbbcccdddeefff")
            assertEquals(33, fFName.Length)
            assertEquals("Z", fFName.Substring(0, 1))
            Dim tempVar = fFName.Length - 13
            assertEquals("_Test_ABC.txt", fFName.Substring(tempVar, fFName.Length - tempVar))
            ' initValues with ShowFullInfo also logs a line, so the count is at least the two explicit messages
            assertTrue(sis.getcharsCount() >= fFName.Length + "aaabbbcccdddeefff".Length)

            sis.onStop(mtLv)
            sis = Nothing

            ' the file was written, time-stamped, and the console echo contains the same lines
            Dim logFile As String = Path.Combine(tmpFld, fFName)
            assertTrue(File.Exists(logFile), logFile)
            Dim lines() As String = File.ReadAllLines(logFile)
            assertTrue(lines.Length >= 3, "expected at least 3 log lines, got " & lines.Length)
            assertTrue(Array.Exists(lines, Function(l) l.EndsWith(": aaabbbcccdddeefff")), String.Join(" / ", lines))
            assertTrue(console.ToString().Contains("aaabbbcccdddeefff"))
        End Sub

        <TestMethod>
        Public Sub testInfoAfterStopIsReportedNotWritten()
            sis = New SIS()
            console = New StringWriter()
            sis.initValues(0, "TEST", console, console, tmpFld, "Test", "X", False, False)
            sis.onStop(0)
            sis.info("late message")
            sis = Nothing
            assertTrue(console.ToString().Contains("File is closed !!!"))
        End Sub

        <TestCleanup>
        Public Sub after()
            Dim mtLv As Integer = 0
            If sis IsNot Nothing Then
                sis.onStop(mtLv)
                sis = Nothing
            End If
            If console IsNot Nothing Then
                console.Dispose()
                console = Nothing
            End If
            If Directory.Exists(tmpFld) Then
                Directory.Delete(tmpFld, True)
            End If
        End Sub

    End Class

End Namespace
