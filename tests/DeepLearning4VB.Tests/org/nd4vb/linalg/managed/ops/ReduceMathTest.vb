Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SysMath = System.Math
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' The single-input reductions: every definition is folded by a small loop (start, map, update, two halves merged,
    ''' post-process) and compared with a direct computation, plus hand-computed values, empty input and NaN handling.
    ''' </summary>
    <TestClass>
    Public Class ReduceMathTest

        Private Shared ReadOnly MatchArgs As Double() = {0.5, 0.00001, 3.0}   ' x > 0.5

        Private Shared ReadOnly Mixed As Double() = {1.5, -2.0, 3.25, 0.0, -0.5, 4.0, 2.0}
        Private Shared ReadOnly Probabilities As Double() = {0.1, 0.2, 0.3, 0.4}
        Private Shared ReadOnly Odd As Double() = {-7.0, 0.25, 9.5}

        Private Shared Function Ln(ByVal x As Double) As Double
            Return SysMath.Log(x)
        End Function

        ''' <summary>Direct computations, independent of the registry, for every registered reduction.</summary>
        Private Shared Function Direct() As Dictionary(Of String, Func(Of Double(), Double))
            Return New Dictionary(Of String, Func(Of Double(), Double)) From {
                {"reduce_sum", Function(v) v.Sum()},
                {"reduce_prod", Function(v) v.Aggregate(1.0, Function(acc, x) acc * x)},
                {"reduce_max", Function(v) v.Max()},
                {"reduce_min", Function(v) v.Min()},
                {"asum", Function(v) v.Sum(Function(x) SysMath.Abs(x))},
                {"amax", Function(v) v.Max(Function(x) SysMath.Abs(x))},
                {"amin", Function(v) v.Min(Function(x) SysMath.Abs(x))},
                {"reduce_mean", Function(v) v.Average()},
                {"amean", Function(v) v.Average(Function(x) SysMath.Abs(x))},
                {"reduce_norm1", Function(v) v.Sum(Function(x) SysMath.Abs(x))},
                {"reduce_norm2", Function(v) SysMath.Sqrt(v.Sum(Function(x) x * x))},
                {"reduce_sqnorm", Function(v) v.Sum(Function(x) x * x)},
                {"reduce_normmax", Function(v) v.Max(Function(x) SysMath.Abs(x))},
                {"entropy", Function(v) -v.Sum(Function(x) x * Ln(x))},
                {"logentropy", Function(v) Ln(-v.Sum(Function(x) x * Ln(x)))},
                {"shannonentropy", Function(v) -v.Sum(Function(x) x * Ln(x) / Ln(2.0))},
                {"countNonZero", Function(v) v.Count(Function(x) x <> 0.0)},
                {"countZero", Function(v) v.Count(Function(x) x = 0.0)},
                {"match_condition", Function(v) v.Count(Function(x) x > 0.5)},
                {"any", Function(v) If(v.Any(Function(x) x <> 0.0), 1.0, 0.0)},
                {"all", Function(v) If(v.All(Function(x) x <> 0.0), 1.0, 0.0)},
                {"hasnan", Function(v) If(v.Any(Function(x) Double.IsNaN(x)), 1.0, 0.0)},
                {"hasinf", Function(v) If(v.Any(Function(x) Double.IsInfinity(x)), 1.0, 0.0)},
                {"reduce_logsumexp", Function(v)
                                         Dim m As Double = v.Max()
                                         Return m + Ln(v.Sum(Function(x) SysMath.Exp(x - m)))
                                     End Function}
            }
        End Function

        Private Shared Function ArgsFor(ByVal name As String) As Double()
            Return If(name = "match_condition", MatchArgs, NoArgs)
        End Function

        Private Shared Function RegisteredReductions() As List(Of String)
            Dim names As New List(Of String)()
            For Each kind As ManagedOpKind In New ManagedOpKind() {ManagedOpKind.ReduceFloat, ManagedOpKind.ReduceSame, ManagedOpKind.ReduceLong, ManagedOpKind.ReduceBool}
                names.AddRange(ManagedOpMath.Names(kind))
            Next
            For Each name As String In ManagedOpMath.Names(ManagedOpKind.Custom)
                Dim family As ManagedOpKind
                If ManagedOpMath.TryGetCustomFamily(name, family) AndAlso family = ManagedOpKind.ReduceFloat Then names.Add(name)
            Next
            Return names
        End Function

        <TestMethod>
        Public Sub EveryReduction_FoldsToItsDirectValue()
            Dim directs As Dictionary(Of String, Func(Of Double(), Double)) = Direct()
            Dim failures As New List(Of String)()
            For Each name As String In RegisteredReductions()
                If Not directs.ContainsKey(name) Then
                    failures.Add(name & ": no direct computation in the test")
                    Continue For
                End If
                Dim definition As ReduceDefinition = ManagedOpMath.Reduce(name)
                assertTrue(definition.MapPair Is Nothing, name & " is not a reduce3 op")
                Dim inputs As Double()() = If(name.Contains("entropy"), New Double()() {Probabilities, New Double() {0.5, 0.5}, New Double() {0.9}},
                                              New Double()() {Mixed, Probabilities, Odd, New Double() {-3.0}})
                For Each values As Double() In inputs
                    Dim expected As Double = directs(name)(values)
                    Dim tolerance As Double = 0.000000000001 * SysMath.Max(1.0, SysMath.Abs(expected))
                    Dim split As Double = Fold(definition, values, ArgsFor(name))
                    Dim sequential As Double = FoldSequential(definition, values, ArgsFor(name))
                    If SysMath.Abs(split - expected) > tolerance Then failures.Add(name & " (halves) on {" & String.Join(", ", values) & "}: " & split & " <> " & expected)
                    If SysMath.Abs(sequential - expected) > tolerance Then failures.Add(name & " (sequential) on {" & String.Join(", ", values) & "}: " & sequential & " <> " & expected)
                Next
            Next
            If failures.Count > 0 Then fail(String.Join(Environment.NewLine, failures))
        End Sub

        <TestMethod>
        Public Sub HandComputedValues()
            assertEquals(6.0, FoldSequential(ManagedOpMath.Reduce("reduce_sum"), New Double() {1.0, 2.0, 3.0}, NoArgs), 0.0)
            assertEquals(5.0, Fold(ManagedOpMath.Reduce("reduce_norm2"), New Double() {3.0, 4.0}, NoArgs), 0.0)
            assertEquals(25.0, Fold(ManagedOpMath.Reduce("reduce_sqnorm"), New Double() {3.0, 4.0}, NoArgs), 0.0)
            assertEquals(2.5, Fold(ManagedOpMath.Reduce("reduce_mean"), New Double() {1.0, 2.0, 3.0, 4.0}, NoArgs), 0.0)
            assertEquals(24.0, Fold(ManagedOpMath.Reduce("reduce_prod"), New Double() {1.0, 2.0, 3.0, 4.0}, NoArgs), 0.0)
            assertEquals(SysMath.Log(2.0), Fold(ManagedOpMath.Reduce("entropy"), New Double() {0.5, 0.5}, NoArgs), 0.000000000000001)
            assertEquals(1.0, Fold(ManagedOpMath.Reduce("shannonentropy"), New Double() {0.5, 0.5}, NoArgs), 0.000000000000001)
            assertEquals(2.0, Fold(ManagedOpMath.Reduce("shannonentropy"), New Double() {0.25, 0.25, 0.25, 0.25}, NoArgs), 0.000000000000001)
            assertEquals(3.40760596444438, Fold(ManagedOpMath.Reduce("reduce_logsumexp"), New Double() {1.0, 2.0, 3.0}, NoArgs), 0.00000000000001)
            assertEquals(4.0, Fold(ManagedOpMath.Reduce("amax"), New Double() {1.0, -4.0, 3.0}, NoArgs), 0.0)
            assertEquals(0.5, Fold(ManagedOpMath.Reduce("amin"), New Double() {1.0, -0.5, 3.0}, NoArgs), 0.0)
            assertEquals(2.0, Fold(ManagedOpMath.Reduce("countZero"), New Double() {0.0, 1.0, -0.0, Double.NaN}, NoArgs), 0.0)
            assertEquals(2.0, Fold(ManagedOpMath.Reduce("countNonZero"), New Double() {0.0, 1.0, -0.0, Double.NaN}, NoArgs), 0.0)
        End Sub

        <TestMethod>
        Public Sub EmptyInput_GivesTheStartingValuePostProcessed()
            Dim empty As Double() = New Double() {}
            assertEquals(0.0, FoldSequential(ManagedOpMath.Reduce("reduce_sum"), empty, NoArgs), 0.0)
            assertEquals(1.0, FoldSequential(ManagedOpMath.Reduce("reduce_prod"), empty, NoArgs), 0.0)
            assertTrue(Double.IsNaN(FoldSequential(ManagedOpMath.Reduce("reduce_mean"), empty, NoArgs)))
            assertTrue(Double.IsNegativeInfinity(FoldSequential(ManagedOpMath.Reduce("reduce_max"), empty, NoArgs)))
            assertTrue(Double.IsPositiveInfinity(FoldSequential(ManagedOpMath.Reduce("reduce_min"), empty, NoArgs)))
            assertTrue(Double.IsNegativeInfinity(FoldSequential(ManagedOpMath.Reduce("reduce_logsumexp"), empty, NoArgs)))
            assertEquals(0.0, FoldSequential(ManagedOpMath.Reduce("any"), empty, NoArgs), 0.0)
            assertEquals(1.0, FoldSequential(ManagedOpMath.Reduce("all"), empty, NoArgs), 0.0)
            assertEquals(0.0, FoldSequential(ManagedOpMath.Reduce("countNonZero"), empty, NoArgs), 0.0)
            assertEquals(0.0, FoldSequential(ManagedOpMath.Reduce("reduce_norm2"), empty, NoArgs), 0.0)
        End Sub

        <TestMethod>
        Public Sub NaN_PropagatesThroughMaxAndMin_WhereverItSits()
            For Each name As String In New String() {"reduce_max", "reduce_min", "amax", "amin", "reduce_normmax", "reduce_sum", "reduce_mean"}
                For position As Integer = 0 To 3
                    Dim values As Double() = {1.0, -5.0, 3.0, 2.0}
                    values(position) = Double.NaN
                    assertTrue(Double.IsNaN(Fold(ManagedOpMath.Reduce(name), values, NoArgs)), name & " with NaN at " & position)
                    assertTrue(Double.IsNaN(FoldSequential(ManagedOpMath.Reduce(name), values, NoArgs)), name & " with NaN at " & position)
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub Entropy_OfAZeroElementIsNaN_AsInLibnd4j()
            assertTrue(Double.IsNaN(Fold(ManagedOpMath.Reduce("entropy"), New Double() {0.0, 1.0}, NoArgs)))
            assertTrue(Double.IsNaN(Fold(ManagedOpMath.Reduce("shannonentropy"), New Double() {0.0, 1.0}, NoArgs)))
        End Sub

        <TestMethod>
        Public Sub AnyAndAll_TreatNaNAsTrueAndIgnoreSigns()
            assertEquals(1.0, Fold(ManagedOpMath.Reduce("any"), New Double() {-1.0, 1.0}, NoArgs), 0.0)
            assertEquals(1.0, Fold(ManagedOpMath.Reduce("all"), New Double() {-1.0, -1.0}, NoArgs), 0.0)
            assertEquals(0.0, Fold(ManagedOpMath.Reduce("all"), New Double() {-1.0, 0.0, 2.0}, NoArgs), 0.0)
            assertEquals(1.0, Fold(ManagedOpMath.Reduce("any"), New Double() {0.0, Double.NaN}, NoArgs), 0.0)
            assertEquals(1.0, Fold(ManagedOpMath.Reduce("hasinf"), New Double() {0.0, Double.NegativeInfinity}, NoArgs), 0.0)
            assertEquals(0.0, Fold(ManagedOpMath.Reduce("hasnan"), New Double() {0.0, Double.PositiveInfinity}, NoArgs), 0.0)
        End Sub

        <TestMethod>
        Public Sub MatchCondition_CountsEveryMode()
            Dim values As Double() = {-2.0, -0.5, 0.0, 0.5, 2.0, Double.PositiveInfinity, Double.NaN}
            ' compare 0.5, eps 1e-5: expected counts for modes 0..15
            Dim expected As Integer() = {1, 5, 3, 2, 4, 3, 1, 3, 1, 1, 1, 6, 5, 3, 5, 2}
            Dim definition As ReduceDefinition = ManagedOpMath.Reduce("match_condition")
            For mode As Integer = 0 To 15
                Dim args As Double() = {0.5, 0.00001, mode}
                assertEquals(CDbl(expected(mode)), Fold(definition, values, args), 0.0, "mode " & mode)
            Next
            assertThrows(Of ArgumentException)(Sub() Fold(definition, values, New Double() {0.5}))
        End Sub

        <TestMethod>
        Public Sub LogSumExp_IsStableForLargeValues()
            Dim definition As ReduceDefinition = ManagedOpMath.Reduce("reduce_logsumexp")
            assertEquals(1000.0 + SysMath.Log(2.0), Fold(definition, New Double() {1000.0, 1000.0}, NoArgs), 0.000000000001)
            assertEquals(-1000.0 + SysMath.Log(3.0), Fold(definition, New Double() {-1000.0, -1000.0, -1000.0}, NoArgs), 0.000000000001)
            assertTrue(Double.IsPositiveInfinity(Fold(definition, New Double() {1.0, Double.PositiveInfinity}, NoArgs)))
            assertTrue(Double.IsNaN(Fold(definition, New Double() {1.0, Double.NaN}, NoArgs)))
            Dim family As ManagedOpKind
            assertTrue(ManagedOpMath.TryGetCustomFamily("reduce_logsumexp", family))
            assertEquals(ManagedOpKind.ReduceFloat, family)
        End Sub

        <TestMethod>
        Public Sub Kinds_FollowTheJavaBaseClasses()
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.ReduceSame, "reduce_sum"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.ReduceFloat, "reduce_mean"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.ReduceLong, "countNonZero"))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.ReduceBool, "any"))
            assertFalse(ManagedOpMath.Supports(ManagedOpKind.ReduceFloat, "reduce_sum"))
            assertFalse(ManagedOpMath.Supports(ManagedOpKind.Reduce3, "reduce_sum"))
        End Sub

    End Class

End Namespace
