Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4j.common.tools

Namespace org.nd4j.common.tools

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class InfoLineTest

        <TestMethod>
        Public Sub testAll()
            Dim iv0 As New InfoValues(" A", " B")
            Dim iv1 As New InfoValues(" C", " D")
            Dim iv2 As New InfoValues(" E", " F", " G", " H")
            iv0.vsL.Add(" ab ")
            iv1.vsL.Add(" cd ")
            iv2.vsL.Add(" ef ")
            Dim il As New InfoLine()
            il.ivL.Add(iv0)
            il.ivL.Add(iv1)
            il.ivL.Add(iv2)
            Dim mtLv As Integer = 2
            assertEquals(".. | A  | C  | E  |", il.getTitleLine(mtLv, 0))
            assertEquals(".. | B  | D  | F  |", il.getTitleLine(mtLv, 1))
            assertEquals(".. |    |    | G  |", il.getTitleLine(mtLv, 2))
            assertEquals(".. |    |    | H  |", il.getTitleLine(mtLv, 3))
            assertEquals(".. | ab | cd | ef |", il.getValuesLine(mtLv))
        End Sub

    End Class

End Namespace
