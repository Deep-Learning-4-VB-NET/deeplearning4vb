Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports SysMath = System.Math
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' The reduce3 ops (two-input reductions): folded in two halves with private states merged by MergeReduceState, compared
    ''' with direct computations and hand-computed values; the accumulator-slot protocol.
    ''' </summary>
    <TestClass>
    Public Class Reduce3MathTest

        Private Shared ReadOnly X As Double() = {1.0, 2.0, 3.0, -1.0}
        Private Shared ReadOnly Y As Double() = {2.0, 0.0, 3.0, 1.0}
        Private Shared ReadOnly JavaArgs As Double() = {0.0, 0.0}

        Private Shared Function Direct() As Dictionary(Of String, Func(Of Double(), Double(), Double))
            Return New Dictionary(Of String, Func(Of Double(), Double(), Double)) From {
                {"manhattan", Function(a, b) a.Zip(b, Function(p, q) SysMath.Abs(p - q)).Sum()},
                {"euclidean", Function(a, b) SysMath.Sqrt(a.Zip(b, Function(p, q) (p - q) * (p - q)).Sum())},
                {"dot", Function(a, b) a.Zip(b, Function(p, q) p * q).Sum()},
                {"cosinesimilarity", Function(a, b) a.Zip(b, Function(p, q) p * q).Sum() / (SysMath.Sqrt(a.Sum(Function(p) p * p)) * SysMath.Sqrt(b.Sum(Function(q) q * q)))},
                {"cosinedistance", Function(a, b) 1.0 - a.Zip(b, Function(p, q) p * q).Sum() / (SysMath.Sqrt(a.Sum(Function(p) p * p)) * SysMath.Sqrt(b.Sum(Function(q) q * q)))},
                {"jaccarddistance", Function(a, b) 1.0 - a.Zip(b, Function(p, q) SysMath.Min(p, q)).Sum() / a.Zip(b, Function(p, q) SysMath.Max(p, q)).Sum()},
                {"hammingdistance", Function(a, b) a.Zip(b, Function(p, q) If(p <> q, 1.0, 0.0)).Sum() / a.Length},
                {"equals_with_eps", Function(a, b) a.Zip(b, Function(p, q) If(SysMath.Abs(p - q) <= 0.00001, 0.0, 1.0)).Sum()}
            }
        End Function

        Private Shared Function ArgsFor(ByVal name As String) As Double()
            Return If(name = "equals_with_eps", New Double() {0.0, 0.0, 0.00001}, JavaArgs)
        End Function

        <TestMethod>
        Public Sub EveryReduce3Op_FoldsToItsDirectValue()
            Dim directs As Dictionary(Of String, Func(Of Double(), Double(), Double)) = Direct()
            Dim pairs As (A As Double(), B As Double())() = {
                (X, Y),
                (New Double() {0.5, 1.5, 2.5, 3.5, 4.5}, New Double() {4.0, 3.0, 2.0, 1.0, 0.25}),
                (New Double() {3.0, 4.0}, New Double() {4.0, 3.0})
            }
            Dim failures As New List(Of String)()
            For Each name As String In ManagedOpMath.Names(ManagedOpKind.Reduce3)
                If Not directs.ContainsKey(name) Then
                    failures.Add(name & ": no direct computation in the test")
                    Continue For
                End If
                Dim definition As ReduceDefinition = ManagedOpMath.Reduce(name)
                assertTrue(definition.Map Is Nothing AndAlso definition.MapPair IsNot Nothing, name & " sets MapPair")
                For Each pair In pairs
                    Dim expected As Double = directs(name)(pair.A, pair.B)
                    Dim actual As Double = FoldPair(definition, pair.A, pair.B, ArgsFor(name))
                    If SysMath.Abs(actual - expected) > 0.000000000001 * SysMath.Max(1.0, SysMath.Abs(expected)) Then
                        failures.Add(name & ": " & actual & " <> " & expected)
                    End If
                Next
            Next
            If failures.Count > 0 Then fail(String.Join(Environment.NewLine, failures))
        End Sub

        <TestMethod>
        Public Sub HandComputedValues()
            assertEquals(10.0, FoldPair(ManagedOpMath.Reduce("dot"), X, Y, NoArgs), 0.0)
            assertEquals(5.0, FoldPair(ManagedOpMath.Reduce("manhattan"), X, Y, JavaArgs), 0.0)
            assertEquals(3.0, FoldPair(ManagedOpMath.Reduce("euclidean"), X, Y, JavaArgs), 0.0)
            assertEquals(10.0 / (SysMath.Sqrt(15.0) * SysMath.Sqrt(14.0)), FoldPair(ManagedOpMath.Reduce("cosinesimilarity"), X, Y, JavaArgs), 0.000000000000001)
            assertEquals(1.0 - 10.0 / (SysMath.Sqrt(15.0) * SysMath.Sqrt(14.0)), FoldPair(ManagedOpMath.Reduce("cosinedistance"), X, Y, JavaArgs), 0.000000000000001)
            ' mins 1, 0, 3, -1 sum to 3; maxes 2, 2, 3, 1 sum to 8
            assertEquals(0.625, FoldPair(ManagedOpMath.Reduce("jaccarddistance"), X, Y, JavaArgs), 0.000000000000001)
            assertEquals(0.75, FoldPair(ManagedOpMath.Reduce("hammingdistance"), X, Y, JavaArgs), 0.0)
            assertEquals(3.0, FoldPair(ManagedOpMath.Reduce("equals_with_eps"), X, Y, New Double() {0.0, 0.0, 0.00001}), 0.0)
            ' differences are 1, 2, 0, 2: within eps 2.5 everything is equal
            assertEquals(0.0, FoldPair(ManagedOpMath.Reduce("equals_with_eps"), X, Y, New Double() {0.0, 0.0, 2.5}), 0.0)
            assertEquals(1.0, FoldPair(ManagedOpMath.Reduce("cosinesimilarity"), New Double() {1.0, 2.0}, New Double() {2.0, 4.0}, JavaArgs), 0.000000000000001)
        End Sub

        <TestMethod>
        Public Sub EqualsWithEps_UsesTheRelativeAndInfinityRulesOfNd4jEq()
            Dim definition As ReduceDefinition = ManagedOpMath.Reduce("equals_with_eps")
            Dim args As Double() = {0.0, 0.0, 0.001}
            ' relative: 1000 vs 1000.5 differ by 0.5 <= 1000.5 * 0.001
            assertEquals(0.0, FoldPair(definition, New Double() {1000.0}, New Double() {1000.5}, args), 0.0)
            assertEquals(0.0, FoldPair(definition, New Double() {Double.PositiveInfinity}, New Double() {Double.PositiveInfinity}, args), 0.0)
            assertEquals(1.0, FoldPair(definition, New Double() {Double.PositiveInfinity}, New Double() {Double.NegativeInfinity}, args), 0.0)
            assertEquals(1.0, FoldPair(definition, New Double() {Double.NaN}, New Double() {Double.NaN}, args), 0.0)
            ' default eps (Nd4j.EPS_THRESHOLD) without arguments, and a lone eps argument
            assertEquals(1.0, FoldPair(definition, New Double() {1.0}, New Double() {1.001}, NoArgs), 0.0)
            assertEquals(0.0, FoldPair(definition, New Double() {1.0}, New Double() {1.001}, New Double() {0.01}), 0.0)
        End Sub

        <TestMethod>
        Public Sub ZeroVectors_GiveNaNLikeLibnd4j()
            Dim zeros As Double() = {0.0, 0.0}
            assertTrue(Double.IsNaN(FoldPair(ManagedOpMath.Reduce("cosinesimilarity"), zeros, zeros, JavaArgs)))
            assertTrue(Double.IsNaN(FoldPair(ManagedOpMath.Reduce("jaccarddistance"), zeros, zeros, JavaArgs)))
        End Sub

        <TestMethod>
        Public Sub AccumulatorSlots_AreTheReduceStateProtocol()
            assertEquals(2, ManagedOpMath.AccumulatorSlotCount("cosinesimilarity"))
            assertEquals(2, ManagedOpMath.AccumulatorSlotCount("cosinedistance"))
            assertEquals(2, ManagedOpMath.AccumulatorSlotCount("jaccarddistance"))
            assertEquals(0, ManagedOpMath.AccumulatorSlotCount("dot"))
            assertEquals(0, ManagedOpMath.AccumulatorSlotCount("reduce_sum"))
            assertEquals(0, ManagedOpMath.AccumulatorSlotCount(Nothing))

            ' the state is a copy, at least as long as the slots, with the slots zeroed
            Dim stale As Double() = {7.0, 8.0}
            Dim state As Double() = ManagedOpMath.CreateReduceState("cosinesimilarity", stale)
            assertArrayEquals(New Double() {0.0, 0.0}, state, 0.0)
            assertArrayEquals(New Double() {7.0, 8.0}, stale, 0.0)
            assertArrayEquals(New Double() {0.0, 0.0}, ManagedOpMath.CreateReduceState("jaccarddistance", NoArgs), 0.0)
            assertArrayEquals(New Double() {0.0, 0.0, 0.5}, ManagedOpMath.CreateReduceState("equals_with_eps", New Double() {0.0, 0.0, 0.5}), 0.0)
            assertArrayEquals(New Double() {}, ManagedOpMath.CreateReduceState("dot", Nothing), 0.0)

            Dim total As Double() = {1.0, 2.0, 9.0}
            ManagedOpMath.MergeReduceState("cosinesimilarity", total, New Double() {10.0, 20.0, 90.0})
            assertArrayEquals(New Double() {11.0, 22.0, 9.0}, total, 0.0)
            ManagedOpMath.MergeReduceState("dot", total, New Double() {10.0, 20.0, 90.0})
            assertArrayEquals(New Double() {11.0, 22.0, 9.0}, total, 0.0)
        End Sub

        <TestMethod>
        Public Sub AccumulatingOps_RejectAStateWithoutSlots()
            For Each name As String In New String() {"cosinesimilarity", "cosinedistance", "jaccarddistance"}
                Dim definition As ReduceDefinition = ManagedOpMath.Reduce(name)
                assertThrows(Of ArgumentException)(Sub() definition.MapPair.Invoke(1.0, 2.0, NoArgs))
            Next
        End Sub

        <TestMethod>
        Public Sub SequentialAndSplitFolds_Agree()
            ' one state through a sequential fold gives the same as two merged halves
            Dim xs As Double() = {0.3, -1.2, 2.2, 4.0, -0.7, 1.1, 0.0}
            Dim ys As Double() = {1.0, 0.4, -2.0, 3.3, 0.9, 1.1, 5.0}
            For Each name As String In ManagedOpMath.Names(ManagedOpKind.Reduce3)
                Dim definition As ReduceDefinition = ManagedOpMath.Reduce(name)
                Dim args As Double() = If(name = "equals_with_eps", New Double() {0.0, 0.0, 0.00001}, JavaArgs)
                Dim state As Double() = ManagedOpMath.CreateReduceState(name, args)
                Dim acc As Double = definition.StartingValue(state)
                For i As Integer = 0 To xs.Length - 1
                    acc = definition.Update(acc, definition.MapPair(xs(i), ys(i), state), state)
                Next
                Dim sequential As Double = definition.PostProcess(acc, xs.Length, state)
                assertEquals(sequential, FoldPair(definition, xs, ys, args), 0.000000000001, name)
            Next
        End Sub

    End Class

End Namespace
