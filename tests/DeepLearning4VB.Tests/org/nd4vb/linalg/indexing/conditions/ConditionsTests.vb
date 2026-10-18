Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.indexing.conditions
Imports ConditionsT = DeepLearning4VB.org.nd4vb.linalg.indexing.conditions.Conditions
Imports ConditionMath = DeepLearning4VB.org.nd4vb.linalg.managed.ops.ConditionMath
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

Namespace org.nd4vb.linalg.indexing.conditions

    ''' <summary>
    ''' Every condition's <c>apply</c> and <c>condtionNum</c>, the agreement of each mode with the op math registry
    ''' (<c>managed.ops.ConditionMath</c>), the composite conditions, <c>ConditionBuilder</c> and the <c>Conditions</c>
    ''' factory.
    ''' </summary>
    <TestClass>
    Public Class ConditionsTests

        Private Shared ReadOnly Samples As Double() = {-7.0, -3.0, -1.5, 0.0, 0.5, 1.0, 2.0, 2.5, 3.0, 7.0, Double.NaN, Double.PositiveInfinity, Double.NegativeInfinity}

        <TestMethod>
        Public Sub testConditionNumbers()
            assertEquals(0, New EpsilonEquals(1).condtionNum())
            assertEquals(0, New EqualsCondition(1).condtionNum())
            assertEquals(1, New EpsilonNotEquals(1).condtionNum())
            assertEquals(2, New LessThan(1).condtionNum())
            assertEquals(3, New GreaterThan(1).condtionNum())
            assertEquals(4, New LessThanOrEqual(1).condtionNum())
            assertEquals(5, New GreaterThanOrEqual(1).condtionNum())
            assertEquals(6, New AbsValueLessThan(1).condtionNum())
            assertEquals(7, New AbsValueGreaterThan(1).condtionNum())
            assertEquals(8, New IsInfinite().condtionNum())
            assertEquals(9, New IsNaN().condtionNum())
            assertEquals(11, New NotEqualsCondition(1).condtionNum())
            assertEquals(12, New AbsValueGreaterOrEqualsThan(1).condtionNum())
            assertEquals(13, New AbsValueLessOrEqualsThan(1).condtionNum())
            assertEquals(14, New IsFinite().condtionNum())
            assertEquals(15, New NotFinite().condtionNum())
            assertEquals(-1, New [And](New IsNaN()).condtionNum())
            assertEquals(-1, New [Or](New IsNaN()).condtionNum())
            assertEquals(-1, New [Not](New IsNaN()).condtionNum())
            assertEquals(-1, New ConditionEquals(New IsNaN()).condtionNum())

            ' the mode constants of the op math registry
            assertEquals(ConditionMath.ModeEpsEquals, New EpsilonEquals(1).condtionNum())
            assertEquals(ConditionMath.ModeEpsNotEquals, New EpsilonNotEquals(1).condtionNum())
            assertEquals(ConditionMath.ModeLessThan, New LessThan(1).condtionNum())
            assertEquals(ConditionMath.ModeGreaterThan, New GreaterThan(1).condtionNum())
            assertEquals(ConditionMath.ModeLessThanOrEqual, New LessThanOrEqual(1).condtionNum())
            assertEquals(ConditionMath.ModeGreaterThanOrEqual, New GreaterThanOrEqual(1).condtionNum())
            assertEquals(ConditionMath.ModeAbsLessThan, New AbsValueLessThan(1).condtionNum())
            assertEquals(ConditionMath.ModeAbsGreaterThan, New AbsValueGreaterThan(1).condtionNum())
            assertEquals(ConditionMath.ModeIsInfinite, New IsInfinite().condtionNum())
            assertEquals(ConditionMath.ModeIsNaN, New IsNaN().condtionNum())
            assertEquals(ConditionMath.ModeNotEquals, New NotEqualsCondition(1).condtionNum())
            assertEquals(ConditionMath.ModeAbsGreaterThanOrEqual, New AbsValueGreaterOrEqualsThan(1).condtionNum())
            assertEquals(ConditionMath.ModeAbsLessThanOrEqual, New AbsValueLessOrEqualsThan(1).condtionNum())
            assertEquals(ConditionMath.ModeIsFinite, New IsFinite().condtionNum())
            assertEquals(ConditionMath.ModeNotFinite, New NotFinite().condtionNum())
        End Sub

        <TestMethod>
        Public Sub testApplyAgreesWithOpMathRegistry()
            ' the values are exactly representable in single precision, and away from the epsilon boundary
            Dim conditions As Condition() = {
                New GreaterThan(2.5), New GreaterThanOrEqual(2.5), New LessThan(2.5), New LessThanOrEqual(2.5),
                New AbsValueGreaterThan(2.5), New AbsValueGreaterOrEqualsThan(2.5), New AbsValueLessThan(2.5), New AbsValueLessOrEqualsThan(2.5),
                New EpsilonEquals(2.5), New EpsilonNotEquals(2.5), New EpsilonEquals(2.5, 0.75), New EpsilonNotEquals(2.5, 0.75),
                New IsNaN(), New IsInfinite(), New IsFinite(), New NotFinite()}
            For Each c As Condition In conditions
                For Each x As Double In Samples
                    Dim expected As Boolean = ConditionMath.Matches(c.condtionNum(), x, c.Value, c.epsThreshold())
                    assertEquals(expected, c.apply(x), CObj(c).GetType().Name & " mode " & c.condtionNum() & " at " & x)
                Next x
            Next c
        End Sub

        <TestMethod>
        Public Sub testValueConditions()
            assertTrue(New GreaterThan(3).apply(4))
            assertFalse(New GreaterThan(3).apply(3))
            assertTrue(New GreaterThanOrEqual(3).apply(3L))
            assertFalse(New GreaterThanOrEqual(3).apply(2.5F))
            assertTrue(New LessThan(3).apply(CShort(2)))
            assertFalse(New LessThan(3).apply(3.0))
            assertTrue(New LessThanOrEqual(3).apply(3))
            assertFalse(New LessThanOrEqual(3).apply(3.5))
            assertTrue(New AbsValueGreaterThan(2).apply(-3))
            assertFalse(New AbsValueGreaterThan(2).apply(-2))
            assertTrue(New AbsValueGreaterOrEqualsThan(2).apply(-2))
            assertTrue(New AbsValueLessThan(2).apply(-1.5))
            assertFalse(New AbsValueLessThan(2).apply(-2))
            assertTrue(New AbsValueLessOrEqualsThan(2).apply(-2))
            assertFalse(New AbsValueLessOrEqualsThan(2).apply(2.5))

            ' single precision, as in Java: 2.0000001 rounds to 2.0f
            assertTrue(New GreaterThanOrEqual(2.0000001).apply(2.0))
            assertFalse(New GreaterThan(2.0000001).apply(2.0000001))

            ' the default value is 0
            assertTrue(New GreaterThan().apply(0.1))
            assertEquals(0.0, New LessThan().Value, 0.0)
        End Sub

        <TestMethod>
        Public Sub testEpsilonConditions()
            Dim e As New EpsilonEquals(1.0)
            assertEquals(Nd4vbT.EPS_THRESHOLD, e.epsThreshold(), 0.0)
            assertTrue(e.apply(1.000001))
            assertFalse(e.apply(1.1))
            assertFalse(New EpsilonNotEquals(1.0).apply(1.000001))
            assertTrue(New EpsilonNotEquals(1.0).apply(1.1))

            ' the epsilon given to the constructor is used
            Dim wide As New EpsilonEquals(1.0, 0.5)
            assertEquals(0.5, wide.epsThreshold(), 0.0)
            assertTrue(wide.apply(1.25))
            assertFalse(wide.apply(1.75))
            assertFalse(New EpsilonNotEquals(1.0, 0.5).apply(1.25))
            assertTrue(New EpsilonNotEquals(1.0, 0.5).apply(1.75))
            assertTrue(New EpsilonEquals().apply(0.0))
        End Sub

        <TestMethod>
        Public Sub testEqualityConditionsFollowTheDefaultDataType()
            runWithNd4vb(
                Sub()
                    assertTrue(New EqualsCondition(2).apply(2.0))
                    assertFalse(New EqualsCondition(2).apply(2.5))
                    assertTrue(New NotEqualsCondition(2).apply(2.5))
                    assertFalse(New NotEqualsCondition(2).apply(2L))
                    assertTrue(New EqualsCondition().apply(0))
                    ' 1.00000001 equals 1 in single precision only
                    Dim isDouble As Boolean = Nd4vbT.dataType() = DataType.DOUBLE
                    assertEquals(Not isDouble, New EqualsCondition(1.0).apply(1.00000001))
                    assertEquals(isDouble, New NotEqualsCondition(1.0).apply(1.00000001))
                End Sub)
        End Sub

        <TestMethod>
        Public Sub testValuelessConditions()
            assertTrue(New IsNaN().apply(Double.NaN))
            assertTrue(New IsNaN().apply(Single.NaN))
            assertFalse(New IsNaN().apply(1))
            assertTrue(New IsInfinite().apply(Double.NegativeInfinity))
            assertFalse(New IsInfinite().apply(Double.NaN))
            ' tested in single precision, as in Java
            assertTrue(New IsInfinite().apply(1.0E+300))
            assertTrue(New IsFinite().apply(1.5))
            assertFalse(New IsFinite().apply(Double.PositiveInfinity))
            assertFalse(New IsFinite().apply(Double.NaN))
            assertTrue(New NotFinite().apply(Double.NaN))
            assertTrue(New NotFinite().apply(Single.NegativeInfinity))
            assertFalse(New NotFinite().apply(0))
            assertEquals(-1.0, New IsNaN().Value, 0.0)
        End Sub

        <TestMethod>
        Public Sub testSetValue()
            ' the comparison conditions ignore setValue, as in Java
            Dim gt As New GreaterThan(1)
            gt.setValue(5)
            assertEquals(1.0, gt.Value, 0.0)
            Dim eps As New EpsilonEquals(1)
            eps.setValue(5)
            assertEquals(1.0, eps.Value, 0.0)
            ' the value-less conditions inherit BaseCondition.setValue
            Dim nan As New IsNaN()
            nan.setValue(3)
            assertEquals(3.0, nan.Value, 0.0)
            ' composites ignore it
            Dim a As New [And](New IsNaN())
            a.setValue(3)
            assertEquals(-1.0, a.Value, 0.0)
            assertEquals(0.0, a.epsThreshold(), 0.0)
        End Sub

        <TestMethod>
        Public Sub testCompositeConditions()
            Dim inRange As Condition = New [And](New GreaterThan(0), New LessThan(10))
            assertTrue(inRange.apply(5))
            assertFalse(inRange.apply(15))
            assertFalse(inRange.apply(-1))

            Dim outside As Condition = New [Or](New LessThan(0), New GreaterThan(10))
            assertTrue(outside.apply(-1))
            assertTrue(outside.apply(11))
            assertFalse(outside.apply(5))

            assertFalse(New [Not](inRange).apply(5))
            assertTrue(New [Not](inRange).apply(15))

            ' ((c0 = c1) = c2)
            Dim eq As Condition = New ConditionEquals(New GreaterThan(0), New LessThan(0))
            assertFalse(eq.apply(5))
            assertTrue(eq.apply(0))
            Dim eq3 As Condition = New ConditionEquals(New GreaterThan(0), New GreaterThan(1), New GreaterThan(2))
            assertTrue(eq3.apply(5))
            assertFalse(eq3.apply(1.5))
            assertEquals(0.0, eq.epsThreshold(), 0.0)
        End Sub

        <TestMethod>
        Public Sub testConditionBuilder()
            assertNull(New ConditionBuilder().build())
            assertThrows(Of InvalidOperationException)(Sub() Call New ConditionBuilder().[not]())

            ' each step combines the new conditions with the condition built so far
            Dim c As Condition = New ConditionBuilder().[and](New GreaterThan(0)).[or](New LessThan(-10)).build()
            assertTrue(TypeOf c Is [Or])
            assertTrue(c.apply(5))
            assertTrue(c.apply(-20))
            assertFalse(c.apply(-5))

            Dim n As Condition = New ConditionBuilder().[and](New GreaterThan(0), New LessThan(10)).[not]().build()
            assertTrue(TypeOf n Is [Not])
            assertFalse(n.apply(5))
            assertTrue(n.apply(20))

            Dim e As Condition = New ConditionBuilder().eq(New GreaterThan(0)).eq(New LessThan(10)).build()
            assertTrue(TypeOf e Is ConditionEquals)
            ' (x < 10) = (x > 0)
            assertTrue(e.apply(5))
            assertFalse(e.apply(20))
            ' NaN fails both comparisons, so they are equal
            assertTrue(e.apply(Double.NaN))
        End Sub

        <TestMethod>
        Public Sub testConditionsFactory()
            assertTrue(TypeOf ConditionsT.Infinite Is IsInfinite)
            assertTrue(TypeOf ConditionsT.Nan Is IsNaN)
            assertTrue(TypeOf ConditionsT.Finite Is IsFinite)
            assertTrue(TypeOf ConditionsT.notFinite() Is NotFinite)

            assertTrue(TypeOf ConditionsT.greaterThan(3) Is GreaterThan)
            assertEquals(3.0, ConditionsT.greaterThan(3).Value, 0.0)
            assertEquals(0.0, ConditionsT.greaterThan().Value, 0.0)
            assertTrue(TypeOf ConditionsT.lessThan(3) Is LessThan)
            assertTrue(TypeOf ConditionsT.lessThanOrEqual() Is LessThanOrEqual)
            assertTrue(TypeOf ConditionsT.greaterThanOrEqual(1) Is GreaterThanOrEqual)
            assertTrue(TypeOf ConditionsT.absGreaterThan(1) Is AbsValueGreaterThan)
            assertTrue(TypeOf ConditionsT.absGreaterThanOrEqual() Is AbsValueGreaterOrEqualsThan)
            assertTrue(TypeOf ConditionsT.absLessThan(1) Is AbsValueLessThan)
            assertTrue(TypeOf ConditionsT.absLessThanOrEqual(1) Is AbsValueLessOrEqualsThan)
            assertTrue(TypeOf ConditionsT.equals(1) Is EqualsCondition)
            assertTrue(TypeOf ConditionsT.equals() Is EqualsCondition)
            assertTrue(TypeOf ConditionsT.notEquals(1) Is NotEqualsCondition)
            assertTrue(TypeOf ConditionsT.notEquals() Is NotEqualsCondition)
            assertTrue(TypeOf ConditionsT.epsNotEquals() Is EpsilonNotEquals)
            assertTrue(TypeOf ConditionsT.epsEquals() Is EpsilonEquals)
            assertEquals(Nd4vbT.EPS_THRESHOLD, ConditionsT.epsEquals(2).epsThreshold(), 0.0)
            assertEquals(0.25, ConditionsT.epsEquals(2, 0.25).epsThreshold(), 0.0)
            assertEquals(2.0, ConditionsT.epsEquals(2, 0.25).Value, 0.0)
            assertEquals(5, ConditionsT.greaterThanOrEqual(1).condtionNum())
        End Sub

    End Class

End Namespace
