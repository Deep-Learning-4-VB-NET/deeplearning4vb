Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SysMath = System.Math
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' The index reductions: argmax/argmin/argamax/argamin (and the legacy imax... names), first_index and last_index with
    ''' conditions; ties, NaN, infinities, empty input, and a split scan merged out of order.
    ''' </summary>
    <TestClass>
    Public Class IndexReduceMathTest

        Private Shared ReadOnly Values As Double() = {1.0, 5.0, -7.0, 5.0, 3.0, -7.0, 0.5}

        Private Shared Sub CheckIndex(ByVal name As String, ByVal values As Double(), ByVal args As Double(), ByVal expected As Long)
            Dim definition As IndexReduceDefinition = ManagedOpMath.IndexReduce(name)
            assertEquals(expected, IndexFold(definition, values, args), name & " sequential")
            assertEquals(expected, IndexFoldSplit(definition, values, args), name & " split")
        End Sub

        <TestMethod>
        Public Sub ArgMaxFamily_FindsTheFirstExtremum()
            For Each prefix As String() In New String()() {New String() {"argmax", "imax"}, New String() {"argmin", "imin"},
                                                            New String() {"argamax", "iamax"}, New String() {"argamin", "iamin"}}
                For Each name As String In prefix
                    Select Case prefix(0)
                        Case "argmax" : CheckIndex(name, Values, NoArgs, 1L)
                        Case "argmin" : CheckIndex(name, Values, NoArgs, 2L)
                        Case "argamax" : CheckIndex(name, Values, NoArgs, 2L)
                        Case "argamin" : CheckIndex(name, Values, NoArgs, 6L)
                    End Select
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub Ties_KeepTheSmallerIndex_EvenWhenMergedOutOfOrder()
            Dim flat As Double() = {2.0, 2.0, 2.0, 2.0, 2.0, 2.0}
            For Each name As String In New String() {"argmax", "argmin", "argamax", "argamin", "imax", "imin", "iamax", "iamin"}
                CheckIndex(name, flat, NoArgs, 0L)
            Next
            ' the tie sits in the second half, the winner in the first
            CheckIndex("argmax", New Double() {0.0, 9.0, 1.0, 9.0, 9.0, 2.0}, NoArgs, 1L)
            CheckIndex("argamin", New Double() {3.0, -1.0, 2.0, 1.0, -1.0, 5.0}, NoArgs, 1L)
        End Sub

        <TestMethod>
        Public Sub NaN_NeverWins_AndDegenerateInputsGiveIndexZero()
            CheckIndex("argmax", New Double() {Double.NaN, 2.0, Double.NaN, 3.0}, NoArgs, 3L)
            CheckIndex("argmin", New Double() {Double.NaN, 2.0, Double.NaN, 1.0}, NoArgs, 3L)
            CheckIndex("argmax", New Double() {Double.NaN, Double.NaN}, NoArgs, 0L)
            CheckIndex("argmax", New Double() {Double.NegativeInfinity, Double.NegativeInfinity}, NoArgs, 0L)
            CheckIndex("argmin", New Double() {Double.PositiveInfinity, 1.0, Double.PositiveInfinity}, NoArgs, 1L)
            CheckIndex("argmax", New Double() {1.0, Double.PositiveInfinity}, NoArgs, 1L)
            CheckIndex("argamax", New Double() {0.0, -0.0, 0.0}, NoArgs, 0L)
            CheckIndex("argmax", New Double() {}, NoArgs, 0L)
            assertEquals(0L, ManagedOpMath.IndexReduce("argmax").EmptyIndex)
        End Sub

        <TestMethod>
        Public Sub FirstAndLastIndex_UseTheCondition()
            ' {compare, eps, mode}: mode 3 = greater than 2
            Dim greaterThan2 As Double() = {2.0, 0.00001, 3.0}
            CheckIndex("first_index", Values, greaterThan2, 1L)
            CheckIndex("last_index", Values, greaterThan2, 4L)
            ' mode 6 = |x| < 1
            Dim absBelow1 As Double() = {1.0, 0.00001, 6.0}
            CheckIndex("first_index", Values, absBelow1, 6L)
            CheckIndex("last_index", Values, absBelow1, 6L)
            ' mode 0 = eps-equals -7
            Dim equalsMinus7 As Double() = {-7.0, 0.00001, 0.0}
            CheckIndex("first_index", Values, equalsMinus7, 2L)
            CheckIndex("last_index", Values, equalsMinus7, 5L)
            ' no match: -1
            Dim greaterThan100 As Double() = {100.0, 0.00001, 3.0}
            CheckIndex("first_index", Values, greaterThan100, -1L)
            CheckIndex("last_index", Values, greaterThan100, -1L)
            CheckIndex("first_index", New Double() {}, greaterThan100, -1L)
            ' NaN with mode 9
            CheckIndex("first_index", New Double() {1.0, Double.NaN, 2.0, Double.NaN}, New Double() {0.0, 0.0, 9.0}, 1L)
            CheckIndex("last_index", New Double() {1.0, Double.NaN, 2.0, Double.NaN}, New Double() {0.0, 0.0, 9.0}, 3L)
            assertEquals(-1L, ManagedOpMath.IndexReduce("first_index").EmptyIndex)
            assertThrows(Of ArgumentException)(Sub() IndexFold(ManagedOpMath.IndexReduce("first_index"), Values, NoArgs))
        End Sub

        <TestMethod>
        Public Sub Prefer_TreatsANegativeBestIndexAsNothingChosen()
            Dim argmax As IndexReduceDefinition = ManagedOpMath.IndexReduce("argmax")
            assertTrue(argmax.Prefer(-5.0, 3, Double.NegativeInfinity, -1, NoArgs))
            assertFalse(argmax.Prefer(Double.NegativeInfinity, 3, Double.NegativeInfinity, -1, NoArgs))
            assertTrue(argmax.Prefer(4.0, 7, 4.0, 9, NoArgs))
            assertFalse(argmax.Prefer(4.0, 9, 4.0, 7, NoArgs))
            Dim first As IndexReduceDefinition = ManagedOpMath.IndexReduce("first_index")
            Dim args As Double() = {0.0, 0.0, 3.0}
            assertTrue(first.Prefer(1.0, 5, Double.NegativeInfinity, -1, args))
            assertTrue(first.Prefer(1.0, 2, 1.0, 5, args))
            assertFalse(first.Prefer(1.0, 8, 1.0, 5, args))
            assertFalse(first.Prefer(-1.0, 0, 1.0, 5, args))
        End Sub

        <TestMethod>
        Public Sub Registration_SeparatesLegacyAndCustomNames()
            For Each name As String In New String() {"first_index", "last_index", "imax", "imin", "iamax", "iamin"}
                assertTrue(ManagedOpMath.Supports(ManagedOpKind.IndexReduce, name), name)
            Next
            For Each name As String In New String() {"argmax", "argmin", "argamax", "argamin"}
                assertTrue(ManagedOpMath.Supports(ManagedOpKind.Custom, name), name)
                Dim family As ManagedOpKind
                assertTrue(ManagedOpMath.TryGetCustomFamily(name, family))
                assertEquals(ManagedOpKind.IndexReduce, family)
            Next
            Dim definition As IndexReduceDefinition = Nothing
            assertFalse(ManagedOpMath.TryGetIndexReduce("argmedian", definition))
            assertNull(definition)
        End Sub

    End Class

End Namespace
