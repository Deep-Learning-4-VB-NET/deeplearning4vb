Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SysMath = System.Math
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Value checks for every pairwise, pairwise bool, broadcast and broadcast bool op and for the broadcastable custom ops
    ''' (arithmetic, comparisons, logic, the activation backprop ops), including NaN, infinities, zero divisors and the
    ''' extra arguments; the backprop ops are checked against numeric derivatives of their forward ops.
    ''' </summary>
    <TestClass>
    Public Class PairwiseMathTest

        Private Const D As Double = 0.000000000001
        Private Shared ReadOnly NaN As Double = Double.NaN
        Private Shared ReadOnly PosInf As Double = Double.PositiveInfinity
        Private Shared ReadOnly NegInf As Double = Double.NegativeInfinity

        Private Shared Function P(ByVal name As String, ByVal x As Double, ByVal y As Double, ByVal expected As Double) As OpCase
            Return New OpCase(name, x, y, Nothing, expected, D)
        End Function

        Private Shared Function PA(ByVal name As String, ByVal x As Double, ByVal y As Double, ByVal args As Double(), ByVal expected As Double) As OpCase
            Return New OpCase(name, x, y, args, expected, D)
        End Function

        Private Shared Function Cases() As List(Of OpCase)
            Dim lambda As Double = 1.0507009873554805
            Return New List(Of OpCase) From {
                P("copy", 1.0, 2.0, 2.0), P("set", 1.0, 2.0, 2.0), P("old_assign", 1.0, NaN, NaN),
                P("fmod", 5.5, 2.0, 1.5), P("fmod", -5.5, 2.0, -1.5), P("fmod", 5.5, -2.0, 1.5), P("fmod", 1.0, 0.0, NaN),
                P("fmod", PosInf, 2.0, NaN), P("fmod", 3.0, PosInf, 3.0),
                P("remainder", 5.5, 2.0, -0.5), P("remainder", 7.0, 2.0, -1.0), P("remainder", -7.5, 2.0, 0.5), P("remainder", 3.0, PosInf, 3.0),
                P("remainder", 1.0, 0.0, NaN),
                P("pow_pairwise", 2.0, 10.0, 1024.0), P("pow_pairwise", NaN, 0.0, 1.0), P("pow_pairwise", 1.0, NaN, 1.0),
                P("pow_pairwise", -8.0, 1.0 / 3.0, NaN), P("pow_pairwise", -1.0, PosInf, 1.0), P("pow_pairwise", 0.0, -1.0, PosInf),
                PA("axpy", 2.0, 3.0, New Double() {0.5, 10.0}, 4.0), P("axpy", 2.0, 3.0, 5.0),
                P("max_pairwise", 1.0, 2.0, 2.0), P("max_pairwise", NaN, 1.0, 1.0), P("max_pairwise", 1.0, NaN, NaN),
                P("min_pairwise", 1.0, 2.0, 1.0), P("min_pairwise", NaN, 1.0, 1.0), P("min_pairwise", 1.0, NaN, NaN),
                P("amax_pairwise", -3.0, 2.0, -3.0), P("amax_pairwise", 1.0, -2.0, -2.0),
                P("amin_pairwise", -3.0, 2.0, 2.0), P("amin_pairwise", -1.0, 2.0, -1.0),
                P("RelativeError", 1.0, 3.0, 0.5), P("RelativeError", 0.0, 0.0, 0.0), P("RelativeError", -1.0, 1.0, 1.0),
                PA("BinaryRelativeError", 1.0, 3.0, New Double() {0.4}, 1.0), PA("BinaryRelativeError", 1.0, 3.0, New Double() {0.6}, 0.0),
                PA("BinaryMinimalRelativeError", 1.0, 3.0, New Double() {0.4, 0.1}, 1.0),
                PA("BinaryMinimalRelativeError", 1.0, 3.0, New Double() {0.4, 5.0}, 0.0),
                PA("BinaryMinimalRelativeError", 1.0, 1.0, New Double() {0.0, 0.0}, 0.0),
                PA("BinaryMinimalRelativeError", NaN, 1.0, New Double() {0.4, 0.1}, 1.0),
                PA("cas", 10.0, 3.0, New Double() {0.0, 0.0, 0.00001, 3.0}, 3.0), PA("cas", 10.0, -3.0, New Double() {0.0, 0.0, 0.00001, 3.0}, 10.0),
                PA("car", 10.0, 3.0, New Double() {5.0, 0.0, 0.00001, 3.0}, 3.0), PA("car", 1.0, 3.0, New Double() {5.0, 0.0, 0.00001, 3.0}, 1.0),
                PA("car", NaN, 0.0, New Double() {0.0, 0.0, 0.00001, 9.0}, 0.0),
                P("and", 1.0, 2.0, 1.0), P("and", 0.0, 2.0, 0.0), P("and", NaN, 1.0, 1.0), PA("and", 5.0, 3.0, New Double() {5.0}, 0.0),
                P("or", 0.0, 0.0, 0.0), P("or", 0.0, 3.0, 1.0), PA("or", 5.0, 5.0, New Double() {5.0}, 0.0),
                P("xor", 1.0, 0.0, 1.0), P("xor", 1.0, 1.0, 0.0), P("xor", 0.0, 0.0, 0.0),
                P("not", 0.0, 7.0, 1.0), P("not", 3.0, 7.0, 0.0), PA("not", 3.0, 0.0, New Double() {3.0}, 1.0),
                P("eps", 1.0, 1.0 + 0.0000000000001, 1.0), P("eps", 1.0, 1.001, 0.0), P("eps", NaN, NaN, 0.0), P("eps", PosInf, PosInf, 0.0),
                P("broadcastadd", 1.0, 2.0, 3.0), P("broadcastsub", 1.0, 2.0, -1.0), P("broadcastmul", 3.0, 4.0, 12.0),
                P("broadcastdiv", 1.0, 4.0, 0.25), P("broadcastrdiv", 4.0, 1.0, 0.25), P("broadcastrsub", 1.0, 4.0, 3.0),
                P("broadcastcopy", 1.0, 9.0, 9.0), P("broadcast_max", 1.0, 2.0, 2.0), P("broadcast_min", 1.0, 2.0, 1.0),
                P("broadcast_amax", -3.0, 2.0, -3.0), P("broadcast_amin", -3.0, 2.0, 2.0),
                P("broadcast_equalto", 2.0, 2.0, 1.0), P("broadcast_equalto", NaN, NaN, 0.0),
                P("broadcast_notequal", NaN, NaN, 1.0), P("broadcast_notequal", 2.0, 2.0, 0.0),
                P("broadcast_greaterthan", 3.0, 2.0, 1.0), P("broadcast_greaterthan", NaN, 2.0, 0.0),
                P("broadcast_greaterthanorequal", 2.0, 2.0, 1.0),
                P("broadcast_lessthan", 1.0, 2.0, 1.0), P("broadcast_lessthanorequal", 3.0, 2.0, 0.0),
                P("add", 1.5, 2.25, 3.75), P("add", PosInf, NegInf, NaN),
                P("subtract", 1.0, 3.0, -2.0),
                P("multiply", -2.0, 3.0, -6.0), P("multiply", 0.0, PosInf, NaN),
                P("divide", 1.0, 0.0, PosInf), P("divide", -1.0, 0.0, NegInf), P("divide", 0.0, 0.0, NaN), P("divide", 3.0, 4.0, 0.75),
                P("realdiv", 3.0, 2.0, 1.5),
                P("reversedivide", 2.0, 3.0, 1.5), P("reversesubtract", 2.0, 3.0, 1.0),
                P("maximum", 1.0, 3.0, 3.0), P("minimum", 1.0, 3.0, 1.0),
                P("Pow", 3.0, 2.0, 9.0), P("Pow", 4.0, -0.5, 0.5),
                P("assign", 1.0, 5.0, 5.0),
                P("floordiv", -7.0, 2.0, -4.0), P("floordiv", 7.0, 2.0, 3.0), P("floordiv", 1.0, 0.0, PosInf),
                P("floormod", -1.0, 3.0, 2.0), P("floormod", 1.0, -3.0, -2.0), P("floormod", 5.0, 3.0, 2.0), P("floormod", -4.0, 2.0, 0.0),
                P("floormod", 5.5, -2.0, -0.5),
                P("mod", 7.9, 3.2, 1.0), P("mod", -7.0, 3.0, -1.0), P("mod", 5.0, 0.5, NaN),
                P("truncatediv", 7.0, 2.0, 3.0), P("truncatediv", -7.0, 2.0, -3.0), P("truncatediv", 7.9, 2.9, 3.0),
                P("squaredsubtract", 1.0, 4.0, 9.0),
                P("tf_atan2", 1.0, -1.0, 2.356194490192345), P("tf_atan2", -1.0, 0.0, -SysMath.PI / 2.0), P("tf_atan2", 0.0, 1.0, 0.0),
                P("tf_atan2", PosInf, PosInf, SysMath.PI / 4.0), P("tf_atan2", NegInf, NegInf, -3.0 * SysMath.PI / 4.0),
                P("prelu", -2.0, 0.25, -0.5), P("prelu", 3.0, 0.25, 3.0),
                P("equals", 2.0, 2.0, 1.0), P("not_equals", 1.0, 2.0, 1.0), P("greater", 3.0, 2.0, 1.0), P("greater_equal", 1.0, 2.0, 0.0),
                P("less", 1.0, 2.0, 1.0), P("less_equal", 2.0, 2.0, 1.0),
                P("boolean_and", 1.0, 0.0, 0.0), P("boolean_and", NaN, 2.0, 1.0), P("boolean_or", 1.0, 0.0, 1.0), P("boolean_or", 0.0, 0.0, 0.0),
                P("boolean_xor", 1.0, 1.0, 0.0), P("boolean_xor", -1.0, 0.0, 1.0),
                P("sigmoid_bp", 0.3, 2.0, 0.4889166233814917),
                P("tanh_bp", 0.5, 2.0, 1.5728954659318548),
                P("softplus_bp", 0.0, 3.0, 1.5),
                P("softsign_bp", 1.0, 4.0, 1.0),
                P("cube_bp", 2.0, 0.5, 6.0),
                P("selu_bp", 1.0, 2.0, 2.0 * lambda),
                P("rationaltanh_bp", 1.0, 1.0, 0.802873417918128),
                P("rectifiedtanh_bp", -1.0, 5.0, 0.0), P("rectifiedtanh_bp", 0.5, 1.0, 0.7864477329659274),
                P("hardsigmoid_bp", 0.0, 1.0, 0.2), P("hardsigmoid_bp", 3.0, 1.0, 0.0),
                P("hardtanh_bp", 0.5, 3.0, 3.0), P("hardtanh_bp", 2.0, 3.0, 0.0),
                P("elu_bp", -1.0, 1.0, 0.36787944117144233), PA("elu_bp", -1.0, 1.0, New Double() {2.0}, 0.7357588823428847), P("elu_bp", 1.0, 5.0, 5.0),
                P("lrelu_bp", -1.0, 2.0, 0.02), PA("lrelu_bp", -1.0, 2.0, New Double() {0.5}, 1.0), P("lrelu_bp", 1.0, 2.0, 2.0),
                P("relu_bp", 1.0, 2.0, 2.0), P("relu_bp", -1.0, 2.0, 0.0), PA("relu_bp", 0.5, 2.0, New Double() {1.0}, 0.0),
                P("relu6_bp", 3.0, 2.0, 2.0), P("relu6_bp", 7.0, 2.0, 0.0), P("relu6_bp", -1.0, 2.0, 0.0),
                P("thresholdedrelu_bp", 2.0, 2.0, 2.0), PA("thresholdedrelu_bp", 0.5, 2.0, New Double() {1.0}, 0.0)
            }
        End Function

        <TestMethod>
        Public Sub EveryPairwiseOpMatchesItsReferenceValues()
            CheckCases(Cases(), Function(c) ManagedOpMath.Pairwise(c.Name)(c.X, c.Y, c.Args))
        End Sub

        <TestMethod>
        Public Sub EveryRegisteredPairwiseOpHasAValueCheck()
            CheckCoverage(AllPairwiseNames(), Cases(), "Pairwise ops")
        End Sub

        <TestMethod>
        Public Sub Kinds_SeparatePairwiseBroadcastAndBool()
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.Pairwise, "max_pairwise"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.PairwiseBool, "and"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.Broadcast, "broadcastadd"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.BroadcastBool, "broadcast_equalto"))
            assertFalse(ManagedOpMath.Supports(ManagedOpKind.Broadcast, "max_pairwise"))
            assertFalse(ManagedOpMath.Supports(ManagedOpKind.Pairwise, "add"))
            Dim family As ManagedOpKind
            assertTrue(ManagedOpMath.TryGetCustomFamily("add", family))
            assertEquals(ManagedOpKind.Pairwise, family)
            assertTrue(ManagedOpMath.TryGetCustomFamily("equals", family))
            assertEquals(ManagedOpKind.PairwiseBool, family)
            ' the same math backs the legacy, broadcast and custom names
            assertTrue(ManagedOpMath.Pairwise("broadcastadd") Is ManagedOpMath.Pairwise("add"))
        End Sub

        <TestMethod>
        Public Sub CompareAndReplace_TestsXAndCompareAndSet_TestsY()
            ' {compare, set, eps, mode}; mode 2 = less than 0
            Dim args As Double() = {0.0, 0.0, 0.00001, 2.0}
            assertEquals(9.0, ManagedOpMath.Pairwise("car")(-1.0, 9.0, args), 0.0)
            assertEquals(1.0, ManagedOpMath.Pairwise("car")(1.0, -9.0, args), 0.0)
            assertEquals(-9.0, ManagedOpMath.Pairwise("cas")(1.0, -9.0, args), 0.0)
            assertEquals(-1.0, ManagedOpMath.Pairwise("cas")(-1.0, 9.0, args), 0.0)
            assertThrows(Of ArgumentException)(Sub() ManagedOpMath.Pairwise("car")(1.0, 2.0, New Double() {0.0}))
        End Sub

        <TestMethod>
        Public Sub FloorMod_TakesTheSignOfTheDivisor()
            Dim floormod As PairwiseFunction = ManagedOpMath.Pairwise("floormod")
            For Each x As Double In New Double() {-7.5, -3.0, -1.0, 0.0, 1.0, 2.5, 7.0}
                For Each y As Double In New Double() {-3.0, -2.0, 2.0, 3.0}
                    Dim r As Double = floormod(x, y, NoArgs)
                    Dim expected As Double = x - SysMath.Floor(x / y) * y
                    assertEquals(expected, r, 0.0000000001, "floormod(" & x & ", " & y & ")")
                    If r <> 0.0 Then assertEquals(SysMath.Sign(y), SysMath.Sign(r))
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub Atan2_UsesTensorFlowArgumentOrder()
            ' tf_atan2(y, x) = angle of the point (x, y), whatever the quadrant
            Dim atan2 As PairwiseFunction = ManagedOpMath.Pairwise("tf_atan2")
            For Each angle As Double In New Double() {-3.0, -2.0, -0.5, 0.0, 0.7, 1.9, 3.1}
                assertEquals(angle, atan2(SysMath.Sin(angle) * 2.0, SysMath.Cos(angle) * 2.0, NoArgs), 0.000000000001)
            Next
        End Sub

        Private Shared Sub CheckBackprop(ByVal bp As String, ByVal forward As Func(Of Double, Double), ByVal points As Double(), Optional ByVal args As Double() = Nothing)
            Dim a As Double() = If(args, NoArgs)
            Dim f As PairwiseFunction = ManagedOpMath.Pairwise(bp)
            Const g As Double = 1.7
            For Each x As Double In points
                Dim expected As Double = g * NumericDerivative(forward, x)
                assertEquals(expected, f(x, g, a), 0.000001 * SysMath.Max(1.0, SysMath.Abs(expected)), bp & " at " & x)
            Next
        End Sub

        <TestMethod>
        Public Sub BackpropOps_MatchNumericDerivativesOfTheirForwardOps()
            Dim smooth As Double() = {-2.5, -0.7, 0.0, 0.3, 1.1, 2.9}
            Dim noKink As Double() = {-2.5, -0.7, 0.3, 1.1, 2.9}
            CheckBackprop("sigmoid_bp", Function(x) ManagedOpMath.Transform("sigmoid")(x, NoArgs), smooth)
            CheckBackprop("tanh_bp", Function(x) ManagedOpMath.Transform("tanh")(x, NoArgs), smooth)
            CheckBackprop("softplus_bp", Function(x) ManagedOpMath.Transform("softplus")(x, NoArgs), smooth)
            CheckBackprop("softsign_bp", Function(x) ManagedOpMath.Transform("softsign")(x, NoArgs), noKink)
            CheckBackprop("cube_bp", Function(x) ManagedOpMath.Transform("cube")(x, NoArgs), smooth)
            CheckBackprop("selu_bp", Function(x) ManagedOpMath.Transform("selu")(x, NoArgs), noKink)
            CheckBackprop("rationaltanh_bp", Function(x) ManagedOpMath.Transform("rational_tanh")(x, NoArgs), noKink)
            CheckBackprop("rectifiedtanh_bp", Function(x) ManagedOpMath.Transform("rectified_tanh")(x, NoArgs), noKink)
            CheckBackprop("hardsigmoid_bp", Function(x) ManagedOpMath.Transform("hard_sigmoid")(x, NoArgs), New Double() {-3.0, -1.0, 0.0, 2.0, 3.0})
            CheckBackprop("hardtanh_bp", Function(x) ManagedOpMath.Transform("hardtanh")(x, NoArgs), New Double() {-3.0, -0.5, 0.0, 0.7, 1.5})
            Dim alpha As Double() = {0.7}
            CheckBackprop("elu_bp", Function(x) ManagedOpMath.Transform("elu")(x, alpha), noKink, alpha)
            CheckBackprop("lrelu_bp", Function(x) ManagedOpMath.Scalar("leakyrelu")(x, 0.7, NoArgs), noKink, alpha)
            Dim cutoff As Double() = {0.5}
            CheckBackprop("relu_bp", Function(x) ManagedOpMath.Scalar("relu")(x, 0.5, NoArgs), noKink, cutoff)
            CheckBackprop("relu6_bp", Function(x) ManagedOpMath.Scalar("relu6")(x, 0.5, NoArgs), New Double() {-1.0, 0.3, 1.1, 5.0, 7.0}, cutoff)
            CheckBackprop("thresholdedrelu_bp", Function(x) ManagedOpMath.Transform("thresholdedrelu")(x, cutoff), New Double() {-1.0, 0.3, 1.1, 5.0}, cutoff)
        End Sub

        <TestMethod>
        Public Sub Logic_ComparableReplacesZero()
            ' the Java And/Or/Xor/Not ops carry a "comparable" (default 0): a value is false when it equals it
            Dim args As Double() = {2.0}
            assertEquals(1.0, ManagedOpMath.Pairwise("and")(0.0, 1.0, args), 0.0)
            assertEquals(0.0, ManagedOpMath.Pairwise("and")(2.0, 1.0, args), 0.0)
            assertEquals(1.0, ManagedOpMath.Pairwise("or")(2.0, 0.0, args), 0.0)
            assertEquals(0.0, ManagedOpMath.Pairwise("or")(2.0, 2.0, args), 0.0)
            assertEquals(1.0, ManagedOpMath.Pairwise("xor")(2.0, 0.0, args), 0.0)
            assertEquals(0.0, ManagedOpMath.Pairwise("xor")(0.0, 1.0, args), 0.0)
        End Sub

    End Class

End Namespace
