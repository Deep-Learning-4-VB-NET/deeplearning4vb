Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SysMath = System.Math
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>Value checks for every scalar and scalar bool op (z = f(x, scalar)), edge cases and the derivative scalar ops.</summary>
    <TestClass>
    Public Class ScalarMathTest

        Private Const D As Double = 0.000000000001
        Private Shared ReadOnly NaN As Double = Double.NaN
        Private Shared ReadOnly PosInf As Double = Double.PositiveInfinity

        Private Shared Function S(ByVal name As String, ByVal x As Double, ByVal scalar As Double, ByVal expected As Double) As OpCase
            Return New OpCase(name, x, scalar, Nothing, expected, D)
        End Function

        Private Shared Function Cases() As List(Of OpCase)
            Return New List(Of OpCase) From {
                S("add_scalar", 1.0, 2.0, 3.0), S("sub_scalar", 1.0, 2.0, -1.0), S("mul_scalar", 3.0, 2.0, 6.0),
                S("div_scalar", 1.0, 4.0, 0.25), S("div_scalar", 1.0, 0.0, PosInf), S("div_scalar", 0.0, 0.0, NaN),
                S("rdiv_scalar", 4.0, 1.0, 0.25), S("rdiv_scalar", 0.0, 1.0, PosInf), S("rsub_scalar", 1.0, 4.0, 3.0),
                S("max_scalar", 1.0, 2.0, 2.0), S("max_scalar", NaN, 0.0, 0.0), S("max_scalar", 3.0, 2.0, 3.0),
                S("scalar_min", 1.0, 2.0, 1.0), S("scalar_min", NaN, 0.0, 0.0),
                S("fmod_scalar", 5.5, 2.0, 1.5), S("fmod_scalar", -5.5, 2.0, -1.5), S("fmod_scalar", 1.0, 0.0, NaN),
                S("remainder_scalar", 5.5, 2.0, -0.5), S("remainder_scalar", -7.5, 2.0, 0.5),
                S("set_scalar", 1.0, 7.0, 7.0), S("set_scalar", NaN, 7.0, 7.0),
                S("setvalorless_scalar", 1.0, 3.0, 3.0), S("setvalorless_scalar", 5.0, 3.0, 5.0), S("setvalorless_scalar", NaN, 3.0, 3.0),
                S("pow", 2.0, 0.5, 1.4142135623730951), S("pow", -8.0, 1.0 / 3.0, NaN), S("pow", 0.0, -1.0, PosInf), S("pow", NaN, 0.0, 1.0),
                S("pow", -2.0, 3.0, -8.0),
                S("_powderivative", 3.0, 2.5, 12.99038105676658), S("_powderivative", 2.0, 1.0, 1.0),
                S("log_x", 8.0, 2.0, 3.0), S("log_x", 100.0, 10.0, 2.0), S("log_x", -1.0, 10.0, NaN),
                S("relu", -1.0, 0.0, 0.0), S("relu", 2.0, 0.0, 2.0), S("relu", 0.5, 1.0, 1.0), S("relu", NaN, 0.0, NaN),
                S("relu6", 7.0, 0.0, 6.0), S("relu6", -1.0, 0.0, 0.0), S("relu6", 3.0, 0.0, 3.0),
                S("leakyrelu", -2.0, 0.01, -0.02), S("leakyrelu", 3.0, 0.01, 3.0),
                S("leakyreluderivative", -2.0, 0.01, 0.01), S("leakyreluderivative", 0.0, 0.01, 1.0), S("leakyreluderivative", 3.0, 0.01, 1.0),
                S("step", 0.5, 0.0, 1.0), S("step", -0.5, 0.0, 0.0), S("step", 0.0, 0.0, 0.0), S("step", 1.0, 2.0, 0.0),
                S("replace_nans", NaN, 5.0, 5.0), S("replace_nans", 1.0, 5.0, 1.0),
                S("equals_scalar", 2.0, 2.0, 1.0), S("equals_scalar", NaN, NaN, 0.0),
                S("notequals_scalar", 2.0, 3.0, 1.0), S("notequals_scalar", NaN, 1.0, 1.0),
                S("greaterthan_scalar", 3.0, 2.0, 1.0), S("greaterthanorequal_scalar", 2.0, 2.0, 1.0),
                S("lessthan_scalar", 3.0, 2.0, 0.0), S("lessthanorequal_scalar", 2.0, 2.0, 1.0),
                S("eps_scalar", 1.0, 1.0, 1.0), S("eps_scalar", 1.0, 1.0001, 0.0),
                S("and_scalar", 1.0, 1.0, 1.0), S("and_scalar", 1.0, 0.0, 0.0),
                S("or_scalar", 0.0, 2.0, 1.0), S("or_scalar", 0.0, 0.0, 0.0),
                S("xor_scalar", 1.0, 1.0, 0.0), S("xor_scalar", 0.0, 1.0, 1.0),
                S("not_scalar", 0.0, 0.0, 1.0), S("not_scalar", 3.0, 0.0, 0.0)
            }
        End Function

        <TestMethod>
        Public Sub EveryScalarOpMatchesItsReferenceValues()
            CheckCases(Cases(), Function(c) ManagedOpMath.Scalar(c.Name)(c.X, c.Y, c.Args))
        End Sub

        <TestMethod>
        Public Sub EveryRegisteredScalarOpHasAValueCheck()
            Dim names As New List(Of String)(ManagedOpMath.Names(ManagedOpKind.Scalar))
            names.AddRange(ManagedOpMath.Names(ManagedOpKind.ScalarBool))
            CheckCoverage(names, Cases(), "Scalar ops")
        End Sub

        <TestMethod>
        Public Sub ScalarIsTheParameter_NotTheExtraArgument()
            ' LeakyReLU passes alpha both as the scalar and in extraArgs; the scalar counts
            assertEquals(-0.4, ManagedOpMath.Scalar("leakyrelu")(-2.0, 0.2, New Double() {0.5}), D)
            assertEquals(0.0, ManagedOpMath.Scalar("step")(0.5, 1.0, New Double() {0.0}), 0.0)
        End Sub

        <TestMethod>
        Public Sub DerivativeScalarOps_MatchNumericDerivatives()
            For Each alpha As Double In New Double() {0.01, 0.2}
                Dim a As Double = alpha
                For Each x As Double In New Double() {-2.0, -0.3, 0.4, 3.0}
                    Dim numeric As Double = NumericDerivative(Function(v) ManagedOpMath.Scalar("leakyrelu")(v, a, NoArgs), x)
                    assertEquals(numeric, ManagedOpMath.Scalar("leakyreluderivative")(x, a, NoArgs), 0.000001)
                Next
            Next
            For Each p As Double In New Double() {2.5, 3.0, -1.0}
                Dim e As Double = p
                For Each x As Double In New Double() {0.5, 1.3, 2.0}
                    Dim numeric As Double = NumericDerivative(Function(v) ManagedOpMath.Scalar("pow")(v, e, NoArgs), x)
                    assertEquals(numeric, ManagedOpMath.Scalar("_powderivative")(x, e, NoArgs), 0.000001 * SysMath.Max(1.0, SysMath.Abs(numeric)))
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub ScalarBoolOps_AreRegisteredAsScalarBool()
            For Each name As String In New String() {"equals_scalar", "notequals_scalar", "greaterthan_scalar", "eps_scalar", "and_scalar", "not_scalar"}
                assertTrue(ManagedOpMath.Supports(ManagedOpKind.ScalarBool, name), name)
                assertFalse(ManagedOpMath.Supports(ManagedOpKind.Scalar, name), name)
            Next
            Dim fn As PairwiseFunction = Nothing
            assertTrue(ManagedOpMath.TryGetScalar("relu", fn))
            assertFalse(ManagedOpMath.TryGetScalar("relu_bp", fn))
            assertNull(fn)
        End Sub

    End Class

End Namespace
