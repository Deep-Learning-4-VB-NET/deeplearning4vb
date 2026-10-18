Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SysMath = System.Math
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' The scalar helpers behind the registry: erf/erfc against reference values (Python's math.erf/erfc, correctly rounded
    ''' to about 1 ulp), log1p/expm1, the inverse hyperbolics, rounding, C99 pow/fmod/remainder, libnd4j's NaN conventions,
    ''' and every condition mode.
    ''' </summary>
    <TestClass>
    Public Class NumericFunctionsTest

        ''' <summary>x, erf(x), erfc(x).</summary>
        Private Shared ReadOnly ErfTable As Double(,) = {
                {-5.5, -0.9999999999999927, 1.9999999999999927},
                {-3.7, -0.9999998328489421, 1.9999998328489421},
                {-2.2, -0.998137153702018, 1.998137153702018},
                {-1.0, -0.8427007929497149, 1.842700792949715},
                {-0.6, -0.6038560908479259, 1.603856090847926},
                {-0.49, -0.511668261188523, 1.511668261188523},
                {-0.1, -0.1124629160182849, 1.1124629160182848},
                {-0.00000001, -0.000000011283791670955126, 1.0000000112837917},
                {0.0, 0.0, 1.0},
                {0.0000000003, 0.0000000003385137501286538, 0.9999999996614862},
                {0.05, 0.05637197779701663, 0.9436280222029834},
                {0.25, 0.27632639016823696, 0.7236736098317631},
                {0.4999, 0.5204119951613866, 0.47958800483861347},
                {0.5, 0.5204998778130465, 0.4795001221869534},
                {0.5001, 0.5205877516768808, 0.47941224832311924},
                {0.56, 0.5716157638237683, 0.42838423617623156},
                {0.75, 0.7111556336535152, 0.28884436634648486},
                {0.9, 0.796908212422832, 0.20309178757716786},
                {1.0, 0.8427007929497149, 0.1572992070502851},
                {1.23, 0.9180501041267612, 0.08194989587323863},
                {1.5, 0.9661051464753108, 0.033894853524689274},
                {1.937, 0.9938435768459388, 0.006156423154061206},
                {2.0, 0.9953222650189527, 0.004677734981047265},
                {2.5, 0.999593047982555, 0.000406952017444959},
                {3.0, 0.9999779095030014, 0.00002209049699858544},
                {3.3, 0.9999969422902035, 0.0000030577097964381654},
                {4.0, 0.9999999845827421, 0.000000015417257900280017},
                {4.06, 0.9999999906273142, 0.00000000937268585186749},
                {4.07, 0.9999999913792682, 0.000000008620731799869473},
                {4.5, 0.9999999998033839, 0.00000000019661604415428876},
                {5.0, 0.9999999999984626, 0.0000000000015374597944280351},
                {6.0, 1.0, 2.1519736712498913E-17},
                {8.0, 1.0, 1.1224297172982929E-29},
                {10.0, 1.0, 2.088487583762545E-45},
                {15.3, 1.0, 7.976560125420701E-104},
                {20.0, 1.0, 5.395865611607901E-176},
                {26.0, 1.0, 5.663192408856143E-296},
                {27.0, 1.0, 5.23705E-319}
            }

        <TestMethod>
        Public Sub Erf_And_Erfc_MatchReferenceValues()
            For i As Integer = 0 To ErfTable.GetLength(0) - 1
                Dim x As Double = ErfTable(i, 0)
                Dim erf As Double = ErfTable(i, 1)
                Dim erfc As Double = ErfTable(i, 2)
                assertEquals(erf, NumericFunctions.Erf(x), SysMath.Abs(erf) * 0.00000000000002 + 1.0E-300, "erf(" & x & ")")
                Dim relative As Double = If(erfc < 1.0E-300, 0.001, 0.00000000000002)
                assertEquals(erfc, NumericFunctions.Erfc(x), erfc * relative, "erfc(" & x & ")")
            Next
        End Sub

        <TestMethod>
        Public Sub Erf_IsOddAndBounded_ErfcComplementsIt()
            For x As Double = -7.0 To 7.0 Step 0.0625
                Dim e As Double = NumericFunctions.Erf(x)
                assertEquals(-e, NumericFunctions.Erf(-x), 0.0, "odd at " & x)
                assertTrue(e >= -1.0 AndAlso e <= 1.0)
                assertEquals(1.0, e + NumericFunctions.Erfc(x), 0.000000000000001, "erf + erfc at " & x)
            Next
            assertEquals(0.0, NumericFunctions.Erfc(30.0), 0.0)
            assertEquals(2.0, NumericFunctions.Erfc(-30.0), 0.0)
            assertTrue(Double.IsNaN(NumericFunctions.Erf(Double.NaN)))
            assertTrue(Double.IsNaN(NumericFunctions.Erfc(Double.NaN)))
            ' monotonic across the table/continued-fraction seams
            Dim previous As Double = 2.0
            For x As Double = 0.0 To 6.0 Step 0.001
                Dim c As Double = NumericFunctions.Erfc(x)
                assertTrue(c <= previous, "erfc not decreasing at " & x)
                previous = c
            Next
        End Sub

        <TestMethod>
        Public Sub Log1p_Expm1_AreAccurateNearZero()
            assertEquals(9.999999999500001E-11, NumericFunctions.Log1p(0.0000000001), 1.0E-25)
            assertEquals(-0.000009999950000166668, NumericFunctions.Expm1(-0.00001), 1.0E-20)
            assertEquals(0.3498588075760031, NumericFunctions.Expm1(0.3), 0.000000000000001)
            assertEquals(-0.6931471805599453, NumericFunctions.Log1p(-0.5), 0.000000000000001)
            assertEquals(1.0E-300, NumericFunctions.Log1p(1.0E-300), 0.0)
            assertEquals(1.0E-300, NumericFunctions.Expm1(1.0E-300), 0.0)
            assertTrue(Double.IsPositiveInfinity(NumericFunctions.Log1p(Double.PositiveInfinity)))
            assertTrue(Double.IsPositiveInfinity(NumericFunctions.Expm1(Double.PositiveInfinity)))
            assertEquals(-1.0, NumericFunctions.Expm1(-100.0), 0.0)
            assertTrue(Double.IsPositiveInfinity(NumericFunctions.Expm1(1000.0)))
            ' consistency: log1p(expm1(x)) = x
            For x As Double = -5.0 To 5.0 Step 0.37
                assertEquals(x, NumericFunctions.Log1p(NumericFunctions.Expm1(x)), 0.000000000000005 * SysMath.Max(1.0, SysMath.Abs(x)))
            Next
        End Sub

        <TestMethod>
        Public Sub InverseHyperbolics_InvertTheHyperbolics()
            For x As Double = -3.0 To 3.0 Step 0.25
                assertEquals(x, NumericFunctions.Asinh(SysMath.Sinh(x)), 0.000000000000005, "asinh at " & x)
                assertEquals(x, NumericFunctions.Atanh(SysMath.Tanh(x)), 0.00000000001, "atanh at " & x)
                assertEquals(SysMath.Abs(x), NumericFunctions.Acosh(SysMath.Cosh(x)), 0.0000001, "acosh at " & x)
            Next
            assertEquals(23.7189981105004, NumericFunctions.Asinh(10000000000.0), 0.00000000000001)
            assertEquals(-0.000000001, NumericFunctions.Asinh(-0.000000001), 1.0E-24)
            assertEquals(0.000014142136208675862, NumericFunctions.Acosh(1.0000000001), 0.0000000000000001)
            assertTrue(Double.IsNaN(NumericFunctions.Acosh(0.999)))
            assertTrue(Double.IsNaN(NumericFunctions.Atanh(1.001)))
            assertTrue(Double.IsNegativeInfinity(NumericFunctions.Asinh(Double.NegativeInfinity)))
            assertTrue(Double.IsPositiveInfinity(NumericFunctions.Acosh(Double.PositiveInfinity)))
        End Sub

        <TestMethod>
        Public Sub Rounding_MatchesC()
            assertEquals(3.0, NumericFunctions.RoundHalfAwayFromZero(2.5), 0.0)
            assertEquals(-3.0, NumericFunctions.RoundHalfAwayFromZero(-2.5), 0.0)
            assertEquals(0.0, NumericFunctions.RoundHalfAwayFromZero(0.49999999999999994), 0.0)
            assertEquals(2.0, NumericFunctions.RoundHalfToEven(2.5), 0.0)
            assertEquals(4.0, NumericFunctions.RoundHalfToEven(3.5), 0.0)
            assertEquals(-4.0, NumericFunctions.RoundHalfToEven(-3.5), 0.0)
            assertEquals(1.0, NumericFunctions.RoundHalfToEven(0.5000000000000001), 0.0)
            assertTrue(Double.IsPositiveInfinity(NumericFunctions.RoundHalfToEven(Double.PositiveInfinity)))
            assertEquals(0.0, NumericFunctions.Sign(Double.NaN), 0.0)
            assertEquals(-1.0, NumericFunctions.Sign(Double.NegativeInfinity), 0.0)
        End Sub

        <TestMethod>
        Public Sub Pow_FollowsC99OnBothFrameworks()
            assertEquals(1.0, NumericFunctions.Pow(Double.NaN, 0.0), 0.0)
            assertEquals(1.0, NumericFunctions.Pow(1.0, Double.NaN), 0.0)
            assertEquals(1.0, NumericFunctions.Pow(-1.0, Double.PositiveInfinity), 0.0)
            assertEquals(1.0, NumericFunctions.Pow(-1.0, Double.NegativeInfinity), 0.0)
            assertTrue(Double.IsNaN(NumericFunctions.Pow(Double.NaN, 1.0)))
            assertTrue(Double.IsNaN(NumericFunctions.Pow(-2.0, 0.5)))
            assertEquals(-8.0, NumericFunctions.Pow(-2.0, 3.0), 0.0)
            assertEquals(0.0, NumericFunctions.Pow(0.5, Double.PositiveInfinity), 0.0)
            assertTrue(Double.IsPositiveInfinity(NumericFunctions.Pow(0.0, -2.0)))
        End Sub

        <TestMethod>
        Public Sub Remainders_FollowC()
            assertEquals(1.5, NumericFunctions.FMod(5.5, 2.0), 0.0)
            assertEquals(-1.5, NumericFunctions.FMod(-5.5, 2.0), 0.0)
            assertEquals(2.0, NumericFunctions.FloorMod(-1.0, 3.0), 0.0)
            assertEquals(-2.0, NumericFunctions.FloorMod(1.0, -3.0), 0.0)
            assertEquals(-0.5, NumericFunctions.IeeeRemainder(5.5, 2.0), 0.0)
            assertEquals(-1.0, NumericFunctions.IeeeRemainder(7.0, 2.0), 0.0)
            assertEquals(1.0, NumericFunctions.IeeeRemainder(5.0, 2.0), 0.0)
            assertTrue(Double.IsNaN(NumericFunctions.FMod(Double.PositiveInfinity, 1.0)))
        End Sub

        <TestMethod>
        Public Sub Libnd4jMaxMin_KeepTheirNaNAsymmetry()
            assertEquals(1.0, NumericFunctions.NdMax(Double.NaN, 1.0), 0.0)
            assertTrue(Double.IsNaN(NumericFunctions.NdMax(1.0, Double.NaN)))
            assertEquals(1.0, NumericFunctions.NdMin(Double.NaN, 1.0), 0.0)
            assertTrue(Double.IsNaN(NumericFunctions.NdMin(1.0, Double.NaN)))
            assertTrue(Double.IsNaN(NumericFunctions.NanMax(Double.NaN, 1.0)))
            assertTrue(Double.IsNaN(NumericFunctions.NanMin(1.0, Double.NaN)))
            assertEquals(3.0, NumericFunctions.NanMax(3.0, -1.0), 0.0)
        End Sub

        <TestMethod>
        Public Sub EqualsWithEps_And_RelativeError()
            assertTrue(NumericFunctions.EqualsWithEps(1.0, 1.000001, 0.00001))
            assertFalse(NumericFunctions.EqualsWithEps(1.0, 1.1, 0.00001))
            assertTrue(NumericFunctions.EqualsWithEps(1000000.0, 1000001.0, 0.00001))
            assertTrue(NumericFunctions.EqualsWithEps(Double.NegativeInfinity, Double.NegativeInfinity, 0.0))
            assertFalse(NumericFunctions.EqualsWithEps(Double.NegativeInfinity, Double.PositiveInfinity, 0.1))
            assertFalse(NumericFunctions.EqualsWithEps(Double.NaN, Double.NaN, 0.1))
            assertEquals(0.0, NumericFunctions.RelativeError(0.0, 0.0), 0.0)
            assertEquals(0.5, NumericFunctions.RelativeError(1.0, 3.0), 0.0)
        End Sub

        <TestMethod>
        Public Sub LogAddExp_And_Atan2()
            assertEquals(SysMath.Log(SysMath.Exp(1.0) + SysMath.Exp(2.0)), NumericFunctions.LogAddExp(1.0, 2.0), 0.000000000000001)
            assertEquals(5.0, NumericFunctions.LogAddExp(Double.NegativeInfinity, 5.0), 0.0)
            assertTrue(Double.IsNegativeInfinity(NumericFunctions.LogAddExp(Double.NegativeInfinity, Double.NegativeInfinity)))
            assertTrue(Double.IsPositiveInfinity(NumericFunctions.LogAddExp(Double.PositiveInfinity, Double.PositiveInfinity)))
            assertEquals(SysMath.PI / 4.0, NumericFunctions.Atan2(Double.PositiveInfinity, Double.PositiveInfinity), 0.0)
            assertEquals(3.0 * SysMath.PI / 4.0, NumericFunctions.Atan2(Double.PositiveInfinity, Double.NegativeInfinity), 0.0)
            assertEquals(-SysMath.PI / 4.0, NumericFunctions.Atan2(Double.NegativeInfinity, Double.PositiveInfinity), 0.0)
            assertEquals(SysMath.Atan2(1.0, 2.0), NumericFunctions.Atan2(1.0, 2.0), 0.0)
        End Sub

        <TestMethod>
        Public Sub Conditions_EveryModeIncludingNaN()
            ' value, compare 1, eps 0.1 -> expected for modes 0..15
            Dim cases As (Value As Double, Expected As Boolean())() = {
                (1.05, New Boolean() {True, False, False, True, False, True, False, True, False, False, False, True, True, False, True, False}),
                (-3.0, New Boolean() {False, True, True, False, True, False, False, True, False, False, False, True, True, False, True, False}),
                (1.0, New Boolean() {True, False, False, False, True, True, False, False, False, False, True, False, True, True, True, False}),
                (Double.NaN, New Boolean() {False, False, False, False, False, False, False, False, False, True, False, True, False, False, False, True}),
                (Double.NegativeInfinity, New Boolean() {False, True, True, False, True, False, False, True, True, False, False, True, True, False, False, True})
            }
            For Each c In cases
                For mode As Integer = 0 To 15
                    assertEquals(c.Expected(mode), ConditionMath.Matches(mode, c.Value, 1.0, 0.1), "mode " & mode & " value " & c.Value)
                Next
            Next
            assertThrows(Of ArgumentOutOfRangeException)(Sub() ConditionMath.Matches(16, 0.0, 0.0, 0.0))
            assertThrows(Of ArgumentOutOfRangeException)(Sub() ConditionMath.Matches(-1, 0.0, 0.0, 0.0))
            assertEquals(3, ConditionMath.ModeOf(3.9))
            assertEquals(-1, ConditionMath.ModeOf(Double.NaN))
            assertEquals(-1, ConditionMath.ModeOf(1.0E+20))
        End Sub

    End Class

End Namespace
