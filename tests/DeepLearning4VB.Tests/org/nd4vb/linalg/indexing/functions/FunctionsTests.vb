Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.indexing.functions
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

Namespace org.nd4vb.linalg.indexing.functions

    ''' <summary>
    ''' The number functions of <c>indexing.functions</c>: <c>Identity</c>, <c>Zero</c>, <c>Value</c> and
    ''' <c>StableNumber</c>.
    ''' </summary>
    <TestClass>
    Public Class FunctionsTests

        <TestMethod>
        Public Sub testIdentityZeroAndValue()
            assertEquals(2.5, New Identity().apply(2.5))
            assertEquals(7L, New Identity().apply(7L))
            assertNull(New Identity().apply(Nothing))
            assertEquals(0, New Zero().apply(5.5))
            assertTrue(TypeOf New Zero().apply(1) Is Integer)
            assertEquals(3.0F, New Value(3.0F).apply(10))
        End Sub

        <TestMethod>
        Public Sub testStableNumber()
            Dim d As New StableNumber(StableNumber.Type.DOUBLE)
            assertEquals(-Double.MaxValue, d.apply(Double.PositiveInfinity))
            assertEquals(-Double.MaxValue, d.apply(Double.NegativeInfinity))
            assertEquals(Nd4vbT.EPS_THRESHOLD, d.apply(Double.NaN))
            assertEquals(2.5, d.apply(2.5))
            assertEquals(4, d.apply(4))

            Dim f As New StableNumber(StableNumber.Type.FLOAT)
            assertEquals(-Single.MaxValue, f.apply(Single.PositiveInfinity))
            ' a double too large for single precision is infinite in single precision
            assertEquals(-Single.MaxValue, f.apply(1.0E+300))
            assertEquals(Nd4vbT.EPS_THRESHOLD, f.apply(Single.NaN))
            assertEquals(1.5F, f.apply(1.5F))
        End Sub

    End Class

End Namespace
