Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4j.common.tools

Namespace org.nd4j.common.tools

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class BToolsTest

        <TestMethod>
        Public Sub testgetMtLvESS()
            assertEquals("?", BTools.getMtLvESS(-5))
            assertEquals("", BTools.getMtLvESS(0))
            assertEquals("...", BTools.getMtLvESS(3))
        End Sub

        <TestMethod>
        Public Sub testgetMtLvISS()
            assertEquals(" ", BTools.MtLvISS)
        End Sub

        <TestMethod>
        Public Sub testgetSpaces()
            assertEquals("?", BTools.getSpaces(-3))
            assertEquals("", BTools.getSpaces(0))
            assertEquals("    ", BTools.getSpaces(4))
        End Sub

        <TestMethod>
        Public Sub testgetSBln()
            assertEquals("?", BTools.getSBln())
            assertEquals("?", BTools.getSBln(Nothing))
            assertEquals("T", BTools.getSBln(True))
            assertEquals("F", BTools.getSBln(False))
            assertEquals("TFFT", BTools.getSBln(True, False, False, True))
            assertEquals("FTFFT", BTools.getSBln(False, True, False, False, True))
        End Sub

        <TestMethod>
        Public Sub testgetSDbl()
            assertEquals("NaN", BTools.getSDbl(Double.NaN, 0))
            assertEquals("-6", BTools.getSDbl(-5.5R, 0))
            assertEquals("-5.50", BTools.getSDbl(-5.5R, 2))
            assertEquals("-5.30", BTools.getSDbl(-5.3R, 2))
            assertEquals("-5", BTools.getSDbl(-5.3R, 0))
            assertEquals("0.00", BTools.getSDbl(0R, 2))
            assertEquals("0", BTools.getSDbl(0R, 0))
            assertEquals("0.30", BTools.getSDbl(0.3R, 2))
            assertEquals("4.50", BTools.getSDbl(4.5R, 2))
            assertEquals("4", BTools.getSDbl(4.5R, 0))
            assertEquals("6", BTools.getSDbl(5.5R, 0))
            assertEquals("12 345 678", BTools.getSDbl(12345678R, 0))
            assertEquals("-456", BTools.getSDbl(-456R, 0, False))
            assertEquals("-456", BTools.getSDbl(-456R, 0, True))
            assertEquals("+456", BTools.getSDbl(456R, 0, True))
            assertEquals("456", BTools.getSDbl(456R, 0, False))
            assertEquals(" 0", BTools.getSDbl(0R, 0, True))
            assertEquals("0", BTools.getSDbl(0R, 0, False))
            assertEquals("  4.50", BTools.getSDbl(4.5R, 2, False, 6))
            assertEquals(" +4.50", BTools.getSDbl(4.5R, 2, True, 6))
            assertEquals("   +456", BTools.getSDbl(456R, 0, True, 7))
            assertEquals("    456", BTools.getSDbl(456R, 0, False, 7))
        End Sub

        <TestMethod>
        Public Sub testgetSInt()
            assertEquals("23", BTools.getSInt(23, 1))
            assertEquals("23", BTools.getSInt(23, 2))
            assertEquals(" 23", BTools.getSInt(23, 3))
            assertEquals("0000056", BTools.getSInt(56, 7, "0"c))
            assertEquals("-000056", BTools.getSInt(-56, 7, "0"c))
        End Sub

        <TestMethod>
        Public Sub testgetSIntA()
            assertEquals("?", BTools.getSIntA(Nothing))
            assertEquals("?", BTools.getSIntA())
            assertEquals("0", BTools.getSIntA(0))
            assertEquals("5, 6, 7", BTools.getSIntA(5, 6, 7))
            Dim intA() As Integer = {2, 3, 4, 5, 6}
            assertEquals("2, 3, 4, 5, 6", BTools.getSIntA(intA))
        End Sub

        <TestMethod>
        Public Sub testgetIndexCharsCount()
            assertEquals(1, BTools.getIndexCharsCount(-5))
            assertEquals(1, BTools.getIndexCharsCount(5))
            assertEquals(3, BTools.getIndexCharsCount(345))
        End Sub

        <TestMethod>
        Public Sub testgetSLcDtTm()
            assertEquals(15, BTools.SLcDtTm.Length)
            assertEquals("LDTm: ", BTools.SLcDtTm.Substring(0, 6))
            Dim dateOnly As String = BTools.getSLcDtTm("yyyy.MM.dd")
            assertEquals("LDTm: " & Date.Now.ToString("yyyy.MM.dd"), dateOnly)
        End Sub

    End Class

End Namespace
