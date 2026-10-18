Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SysMath = System.Math
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Value checks for every transform op of the registry (legacy transforms and the element-wise custom ops), edge cases,
    ''' extra arguments, and the derivative ops against a numeric derivative of their forward op. Reference values were
    ''' computed independently (Python's math module, or by hand).
    ''' </summary>
    <TestClass>
    Public Class TransformMathTest

        Private Const D As Double = 0.000000000001
        Private Shared ReadOnly NaN As Double = Double.NaN
        Private Shared ReadOnly PosInf As Double = Double.PositiveInfinity
        Private Shared ReadOnly NegInf As Double = Double.NegativeInfinity

        Private Shared Function T(ByVal name As String, ByVal x As Double, ByVal expected As Double) As OpCase
            Return New OpCase(name, x, 0.0, Nothing, expected, D)
        End Function

        Private Shared Function TD(ByVal name As String, ByVal x As Double, ByVal expected As Double, ByVal delta As Double) As OpCase
            Return New OpCase(name, x, 0.0, Nothing, expected, delta)
        End Function

        Private Shared Function TA(ByVal name As String, ByVal x As Double, ByVal args As Double(), ByVal expected As Double) As OpCase
            Return New OpCase(name, x, 0.0, args, expected, D)
        End Function

        Private Shared Function Cases() As List(Of OpCase)
            Dim stab As Double = -CDbl(-3.79297773665F)
            Return New List(Of OpCase) From {
                T("abs", -2.5, 2.5), T("abs", NegInf, PosInf), T("abs", NaN, NaN),
                T("ceil", -1.5, -1.0), T("ceil", 1.2, 2.0),
                T("cube", -2.0, -8.0), T("cube", 1.5, 3.375),
                T("floor", -1.5, -2.0), T("floor", 1.9, 1.0),
                T("neg", 3.0, -3.0),
                T("oneminus", 0.25, 0.75),
                T("Reciprocal", 4.0, 0.25), T("Reciprocal", 0.0, PosInf),
                T("round", 2.5, 3.0), T("round", -2.5, -3.0), T("round", 1.4, 1.0), T("round", 0.49999999999999994, 0.0), T("round", NaN, NaN),
                T("sign", -3.0, -1.0), T("sign", 0.0, 0.0), T("sign", 2.0, 1.0), T("sign", NaN, 0.0),
                T("square", -3.0, 9.0),
                T("timesoneminus", 0.25, 0.1875),
                TA("cas", 5.0, New Double() {5.0, 9.0, 0.00001, 0.0}, 9.0), TA("cas", 4.0, New Double() {5.0, 9.0, 0.00001, 0.0}, 4.0),
                TA("cas", -3.0, New Double() {0.0, 7.0, 0.00001, 2.0}, 7.0),
                T("copy", 7.0, 7.0),
                T("sqrt", 2.0, 1.4142135623730951), T("sqrt", -1.0, NaN), T("sqrt", PosInf, PosInf),
                T("rsqrt", 4.0, 0.5), T("rsqrt", 0.0, PosInf), T("rsqrt", -4.0, NaN),
                T("sin", 1.0, 0.8414709848078965), T("cos", 1.0, 0.5403023058681398), T("tan", 1.0, 1.5574077246549023),
                T("asin", 0.5, 0.5235987755982989), T("asin", 2.0, NaN),
                T("acos", 0.5, 1.0471975511965979), T("acos", -1.5, NaN),
                T("atan", 1.0, 0.7853981633974483), T("atan", PosInf, SysMath.PI / 2.0),
                T("sinh", 1.0, 1.1752011936438014), T("cosh", 1.0, 1.5430806348152437),
                T("tanh", 0.5, 0.46211715726000974), T("tanh", 1000.0, 1.0),
                T("asinh", 1.0, 0.881373587019543), T("asinh", -0.000000001, -0.000000001), T("asinh", 10000000000.0, 23.7189981105004),
                T("acosh", 2.0, 1.3169578969248166), T("acosh", 0.5, NaN), T("acosh", 1.0, 0.0),
                T("atanh", 0.5, 0.5493061443340549), T("atanh", 1.0, PosInf), T("atanh", -1.0, NegInf), T("atanh", 1.5, NaN),
                T("tanderivative", 1.0, 3.425518820814759), T("tanderivative", 0.0, 1.0),
                T("_tanhderivative", 0.5, 0.7864477329659274), T("_tanhderivative", 0.0, 1.0),
                T("exp", 1.0, 2.718281828459045), T("exp", NegInf, 0.0), T("exp", 1000.0, PosInf),
                TD("expm1", 0.0000000001, 0.000000000100000000005, 0.000000000000000000001), T("expm1", 1.0, 1.718281828459045), T("expm1", NegInf, -1.0),
                T("log", SysMath.E, 1.0), T("log", 0.0, NegInf), T("log", -1.0, NaN),
                TD("log1p", 0.0000000001, 0.000000000099999999995, 0.000000000000000000001), T("log1p", 3.0, 1.3862943611198906),
                T("log1p", -1.0, NegInf), T("log1p", -2.0, NaN),
                T("erf", 0.5, 0.5204998778130465), T("erf", 1.0, 0.8427007929497149), T("erf", -2.0, -0.9953222650189527),
                T("erf", PosInf, 1.0), T("erf", NegInf, -1.0), T("erf", NaN, NaN),
                T("erfc", 0.5, 0.4795001221869534), T("erfc", -1.0, 1.842700792949715), T("erfc", PosInf, 0.0), T("erfc", NegInf, 2.0),
                TD("erfc", 3.0, 0.00002209049699858544, 0.0000000000000000001),
                T("rint", 2.5, 2.0), T("rint", 3.5, 4.0), T("rint", -2.5, -2.0), T("rint", 1.6, 2.0), T("rint", -0.4, 0.0),
                T("sigmoid", 0.0, 0.5), T("sigmoid", 2.0, 0.8807970779778823), T("sigmoid", -800.0, 0.0), T("sigmoid", 800.0, 1.0),
                T("_sigmoidderivative", 1.0, 0.19661193324148185),
                T("logsigmoid", -1.0, -1.3132616875182228), T("logsigmoid", -1000.0, -1000.0), T("logsigmoid", 1000.0, 0.0),
                T("softplus", 1.0, 1.3132616875182228), T("softplus", 1000.0, 1000.0), T("softplus", -1000.0, 0.0), T("softplus", 0.0, SysMath.Log(2.0)),
                T("softsign", 3.0, 0.75), T("softsign", -1.0, -0.5),
                T("softsignderivative", 3.0, 0.0625),
                T("swish", 1.0, 0.7310585786300049), T("swish", -1000.0, 0.0),
                T("_swishderivative", 1.0, 0.9276705118714867),
                T("gelu", 1.0, 0.8457957687936359), T("gelu", -2.0, -0.0643413741240036),
                T("_geluderivative", 1.0, 1.0677796089105305),
                T("precise_gelu", 1.0, 0.8411919906082768), T("precise_gelu", -0.5, -0.15428599017485606),
                T("_precise_geluderivative", 1.0, 1.0829640838457826),
                T("mish", 1.0, 0.8650983882673103), T("mish", -2.0, -0.2525014826957089),
                T("_mishderivative", 1.0, 1.0490362200997922),
                T("hardtanh", 2.0, 1.0), T("hardtanh", -3.0, -1.0), T("hardtanh", 0.5, 0.5), T("hardtanh", NaN, NaN),
                T("hardtanhderivative", 1.0, 1.0), T("hardtanhderivative", -1.0, 1.0), T("hardtanhderivative", 1.5, 0.0),
                T("hard_sigmoid", 0.0, 0.5), T("hard_sigmoid", 3.0, 1.0), T("hard_sigmoid", -3.0, 0.0), T("hard_sigmoid", 1.0, 0.7), T("hard_sigmoid", NaN, NaN),
                T("hard_sigmoidderivative", 0.0, 0.2), T("hard_sigmoidderivative", 2.5, 0.2), T("hard_sigmoidderivative", 3.0, 0.0),
                T("selu", 1.0, 1.0507009873554805), T("selu", -1.0, -1.1113307378125625), T("selu", 0.0, 0.0),
                T("seluderivative", -1.0, 0.646768603034814), T("seluderivative", 2.0, 1.0507009873554805),
                T("rectified_tanh", -1.0, 0.0), T("rectified_tanh", 0.5, 0.46211715726000974),
                T("rectified_tanh_derivative", 0.5, 0.7864477329659274), T("rectified_tanh_derivative", -0.5, 0.0), T("rectified_tanh_derivative", 0.0, 0.0),
                T("rational_tanh", 1.0, 0.9982215458427454), T("rational_tanh", -2.0, -1.5160930776549535), T("rational_tanh", 0.0, 0.0),
                T("rational_tanh_derivative", 1.0, 0.802873417918128), T("rational_tanh_derivative", -2.0, 0.26518574712345483),
                T("rational_tanh_derivative", 0.0, 1.1439332962036133),
                T("cubederivative", 2.0, 12.0),
                T("stabilize", 10.0, stab), T("stabilize", -10.0, -stab), T("stabilize", 1.0, 1.0),
                TA("stabilize", 10.0, New Double() {2.0}, stab / 2.0), TA("stabilize", 1.0, New Double() {2.0}, 1.0),
                TA("setrange", 0.5, New Double() {0.0, 1.0}, 0.5), TA("setrange", 2.0, New Double() {0.0, 1.0}, 0.0),
                TA("setrange", 5.0, New Double() {-1.0, 1.0}, 9.0), TA("setrange", 0.25, New Double() {-1.0, 1.0}, 0.25),
                T("setrange", 0.75, 0.75),
                T("isnan", NaN, 1.0), T("isnan", 1.0, 0.0), T("isnan", PosInf, 0.0),
                T("isinf", NegInf, 1.0), T("isinf", 1.0, 0.0), T("isinf", NaN, 0.0),
                T("isfinite", 1.0, 1.0), T("isfinite", NaN, 0.0), T("isfinite", PosInf, 0.0),
                T("bool_not", 0.0, 1.0), T("bool_not", 2.0, 0.0), T("bool_not", NaN, 0.0),
                TA("match_condition_transform", 3.0, New Double() {2.0, 0.00001, 3.0}, 1.0),
                TA("match_condition_transform", 1.0, New Double() {2.0, 0.00001, 3.0}, 0.0),
                TA("match_condition_transform", NaN, New Double() {0.0, 0.00001, 9.0}, 1.0),
                T("not", 0.0, 1.0), T("not", 5.0, 0.0), TA("not", 5.0, New Double() {5.0}, 1.0),
                T("old_assign", 3.0, 3.0), T("set", -3.0, -3.0),
                T("elu", 1.0, 1.0), T("elu", -1.0, -0.6321205588285577), TA("elu", -1.0, New Double() {2.0}, -1.2642411176571153),
                T("thresholdedrelu", 0.5, 0.5), T("thresholdedrelu", -1.0, 0.0), TA("thresholdedrelu", 0.5, New Double() {1.0}, 0.0),
                TA("ClipByValue", 2.0, New Double() {-1.0, 1.0}, 1.0), TA("ClipByValue", -2.0, New Double() {-1.0, 1.0}, -1.0),
                TA("ClipByValue", 0.3, New Double() {-1.0, 1.0}, 0.3),
                T("boolean_not", 0.0, 1.0), T("boolean_not", -1.0, 0.0),
                T("identity", 4.0, 4.0)
            }
        End Function

        <TestMethod>
        Public Sub EveryTransformMatchesItsReferenceValues()
            CheckCases(Cases(), Function(c) ManagedOpMath.Transform(c.Name)(c.X, c.Args))
        End Sub

        <TestMethod>
        Public Sub EveryRegisteredTransformHasAValueCheck()
            CheckCoverage(AllTransformNames(), Cases(), "Transform ops")
        End Sub

        <TestMethod>
        Public Sub Kinds_FollowTheJavaBaseClasses()
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.TransformSame, "abs"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.TransformFloat, "sqrt"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.TransformStrict, "sigmoid"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.TransformBool, "isnan"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.TransformAny, "old_assign"))
            assertFalse(ManagedOpMath.Supports(ManagedOpKind.TransformSame, "sigmoid"))
            assertFalse(ManagedOpMath.Supports(ManagedOpKind.TransformStrict, "elu"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.Custom, "elu"))
        End Sub

        <TestMethod>
        Public Sub Round_IsHalfAwayFromZero_Rint_IsHalfToEven()
            Dim round As TransformFunction = ManagedOpMath.Transform("round")
            Dim rint As TransformFunction = ManagedOpMath.Transform("rint")
            For Each v As Double In New Double() {-3.5, -2.5, -1.5, -0.5, 0.5, 1.5, 2.5, 3.5}
                assertEquals(SysMath.Round(v, MidpointRounding.AwayFromZero), round(v, NoArgs), 0.0)
                assertEquals(SysMath.Round(v, MidpointRounding.ToEven), rint(v, NoArgs), 0.0)
            Next
            ' the sign of zero survives (C's round(-0.4) is -0.0)
            assertTrue(1.0 / round(-0.4, NoArgs) < 0.0)
            assertEquals(4503599627370497.0, round(4503599627370497.0, NoArgs), 0.0)
            assertEquals(1.0E+300, rint(1.0E+300, NoArgs), 0.0)
        End Sub

        <TestMethod>
        Public Sub ExtraArguments_RequiredOnesThrowWhenMissing()
            assertThrows(Of ArgumentException)(Sub() ManagedOpMath.Transform("ClipByValue")(1.0, NoArgs))
            assertThrows(Of ArgumentException)(Sub() ManagedOpMath.Transform("ClipByValue")(1.0, New Double() {0.0}))
            assertThrows(Of ArgumentException)(Sub() ManagedOpMath.Transform("match_condition_transform")(1.0, New Double() {0.0, 0.1}))
            assertThrows(Of ArgumentException)(Sub() ManagedOpMath.Transform("cas")(1.0, New Double() {0.0, 1.0, 0.1}))
            assertThrows(Of ArgumentOutOfRangeException)(Sub() ManagedOpMath.Transform("match_condition_transform")(1.0, New Double() {0.0, 0.1, 99.0}))
        End Sub

        <TestMethod>
        Public Sub CompareAndSet_UsesEveryConditionMode()
            Dim cas As TransformFunction = ManagedOpMath.Transform("cas")
            ' {compare, set, eps, mode}: x = -2 against compare 1
            Dim expectedMatch As Boolean() = {False, True, True, False, True, False, False, True, False, False, False, True, True, False, True, False}
            For mode As Integer = 0 To 15
                Dim result As Double = cas(-2.0, New Double() {1.0, 42.0, 0.00001, mode})
                assertEquals(If(expectedMatch(mode), 42.0, -2.0), result, 0.0, "mode " & mode)
            Next
        End Sub

        Private Shared Sub CheckDerivative(ByVal derivative As String, ByVal forward As String, ByVal points As Double(), Optional ByVal args As Double() = Nothing)
            Dim a As Double() = If(args, NoArgs)
            Dim f As TransformFunction = ManagedOpMath.Transform(forward)
            Dim df As TransformFunction = ManagedOpMath.Transform(derivative)
            For Each x As Double In points
                Dim numeric As Double = NumericDerivative(Function(v) f(v, a), x)
                assertEquals(numeric, df(x, a), 0.000001 * SysMath.Max(1.0, SysMath.Abs(numeric)), derivative & " at " & x)
            Next
        End Sub

        <TestMethod>
        Public Sub DerivativeOps_MatchNumericDerivativesOfTheirForwardOps()
            Dim smooth As Double() = {-3.0, -1.2, -0.3, 0.0, 0.4, 1.0, 2.7}
            CheckDerivative("_sigmoidderivative", "sigmoid", smooth)
            CheckDerivative("_tanhderivative", "tanh", smooth)
            CheckDerivative("tanderivative", "tan", New Double() {-1.2, -0.3, 0.0, 0.4, 1.0})
            CheckDerivative("softsignderivative", "softsign", New Double() {-3.0, -1.2, -0.3, 0.4, 1.0, 2.7})
            CheckDerivative("cubederivative", "cube", smooth)
            CheckDerivative("_swishderivative", "swish", smooth)
            CheckDerivative("_geluderivative", "gelu", smooth)
            CheckDerivative("_precise_geluderivative", "precise_gelu", smooth)
            CheckDerivative("_mishderivative", "mish", smooth)
            CheckDerivative("rational_tanh_derivative", "rational_tanh", New Double() {-3.0, -1.2, -0.3, 0.4, 1.0, 2.7})
            ' piecewise ops: away from the kinks
            CheckDerivative("hardtanhderivative", "hardtanh", New Double() {-3.0, -0.5, 0.0, 0.7, 1.5})
            CheckDerivative("hard_sigmoidderivative", "hard_sigmoid", New Double() {-4.0, -1.0, 0.0, 2.0, 3.0})
            CheckDerivative("rectified_tanh_derivative", "rectified_tanh", New Double() {-2.0, -0.5, 0.3, 1.5})
            CheckDerivative("seluderivative", "selu", New Double() {-3.0, -0.5, 0.3, 1.5})
        End Sub

        <TestMethod>
        Public Sub StableForms_DoNotOverflow()
            For Each name As String In New String() {"sigmoid", "softplus", "logsigmoid", "swish", "mish", "gelu", "_sigmoidderivative", "_swishderivative", "_mishderivative", "_geluderivative"}
                Dim f As TransformFunction = ManagedOpMath.Transform(name)
                For Each x As Double In New Double() {-1000.0, -50.0, 50.0, 1000.0}
                    Dim v As Double = f(x, NoArgs)
                    assertFalse(Double.IsNaN(v), name & "(" & x & ") is NaN")
                Next
            Next
            assertEquals(1000.0, ManagedOpMath.Transform("mish")(1000.0, NoArgs), 0.0)
            assertEquals(1.0, ManagedOpMath.Transform("_mishderivative")(1000.0, NoArgs), 0.0)
        End Sub

        <TestMethod>
        Public Sub NaN_PropagatesThroughTheArithmeticTransforms()
            For Each name As String In New String() {"abs", "ceil", "cube", "floor", "neg", "oneminus", "Reciprocal", "round", "square", "timesoneminus",
                                                      "sqrt", "rsqrt", "sin", "cos", "exp", "expm1", "log", "log1p", "erf", "erfc", "rint", "sigmoid",
                                                      "softplus", "logsigmoid", "tanh", "asinh", "acosh", "atanh", "gelu", "precise_gelu", "mish", "swish",
                                                      "rational_tanh", "rectified_tanh", "selu", "elu", "copy", "identity"}
                assertTrue(Double.IsNaN(ManagedOpMath.Transform(name)(Double.NaN, NoArgs)), name & "(NaN)")
            Next
        End Sub

    End Class

End Namespace
