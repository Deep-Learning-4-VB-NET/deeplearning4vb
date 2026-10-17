Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.common.tools

Namespace org.nd4vb.common.tools

    ''' <summary>Ported from the DL4J JUnit test of the same name.</summary>
    <TestClass>
    Public Class InfoValuesTest
        Private ReadOnly t1_titleA() As String = {"T0", "T1", "T2", "T3", "T4", "T5"}
        Private ReadOnly t2_titleA() As String = {"", "T1", "T2"}

        <TestMethod>
        Public Sub testconstructor()
            Dim iv As InfoValues
            iv = New InfoValues(t1_titleA)
            assertEquals("T0", iv.titleA(0))
            assertEquals("T1", iv.titleA(1))
            assertEquals("T2", iv.titleA(2))
            assertEquals("T3", iv.titleA(3))
            assertEquals("T4", iv.titleA(4))
            assertEquals("T5", iv.titleA(5))
            iv = New InfoValues(t2_titleA)
            assertEquals("", iv.titleA(0))
            assertEquals("T1", iv.titleA(1))
            assertEquals("T2", iv.titleA(2))
            assertEquals("", iv.titleA(3))
            assertEquals("", iv.titleA(4))
            assertEquals("", iv.titleA(5))
        End Sub

        <TestMethod>
        Public Sub testgetValues()
            Dim iv As InfoValues
            iv = New InfoValues("Test")
            iv.vsL.Add(" AB ")
            iv.vsL.Add(" CD ")
            assertEquals(" AB | CD |", iv.Values)
        End Sub

    End Class

End Namespace
