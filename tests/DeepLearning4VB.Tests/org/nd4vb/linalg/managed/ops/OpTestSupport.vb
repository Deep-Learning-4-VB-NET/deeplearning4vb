Imports System
Imports System.Collections.Generic
Imports SysMath = System.Math
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>One value check of an element-wise op: f(X[, Y], Args) = Expected (within Delta).</summary>
    Friend NotInheritable Class OpCase
        Public ReadOnly Name As String
        Public ReadOnly X As Double
        Public ReadOnly Y As Double
        Public ReadOnly Args As Double()
        Public ReadOnly Expected As Double
        Public ReadOnly Delta As Double

        Public Sub New(ByVal name As String, ByVal x As Double, ByVal y As Double, ByVal args As Double(), ByVal expected As Double, ByVal delta As Double)
            Me.Name = name
            Me.X = x
            Me.Y = y
            Me.Args = If(args, OpTestSupport.NoArgs)
            Me.Expected = expected
            Me.Delta = delta
        End Sub

        Public Overrides Function ToString() As String
            Return Name & "(" & X.ToString("R") & ", " & Y.ToString("R") & ", {" & String.Join(", ", Args) & "})"
        End Function
    End Class

    ''' <summary>Shared helpers of the op math tests: table-driven value checks, numeric derivatives, and tiny reference folds.</summary>
    Friend Module OpTestSupport

        Public ReadOnly NoArgs As Double() = New Double() {}

        ''' <summary>Central difference of f at x (step 1e-5; error around 1e-10 for smooth f).</summary>
        Public Function NumericDerivative(ByVal f As Func(Of Double, Double), ByVal x As Double) As Double
            Const h As Double = 0.00001
            Return (f(x + h) - f(x - h)) / (2.0 * h)
        End Function

        ''' <summary>Runs a reduce definition the way a parallel kernel would: two halves folded separately, merged, post-processed.</summary>
        Public Function Fold(ByVal definition As ReduceDefinition, ByVal values As Double(), ByVal extraArgs As Double()) As Double
            Dim mid As Integer = values.Length \ 2
            Dim first As Double = FoldRange(definition, values, 0, mid, extraArgs)
            Dim second As Double = FoldRange(definition, values, mid, values.Length, extraArgs)
            Dim acc As Double = definition.Merge(first, second, extraArgs)
            Return definition.PostProcess(acc, values.LongLength, extraArgs)
        End Function

        ''' <summary>Runs a reduce definition sequentially over all values.</summary>
        Public Function FoldSequential(ByVal definition As ReduceDefinition, ByVal values As Double(), ByVal extraArgs As Double()) As Double
            Dim acc As Double = FoldRange(definition, values, 0, values.Length, extraArgs)
            Return definition.PostProcess(acc, values.LongLength, extraArgs)
        End Function

        Private Function FoldRange(ByVal definition As ReduceDefinition, ByVal values As Double(), ByVal start As Integer, ByVal [end] As Integer, ByVal extraArgs As Double()) As Double
            Dim acc As Double = definition.StartingValue(extraArgs)
            For i As Integer = start To [end] - 1
                acc = definition.Update(acc, definition.Map(values(i), extraArgs), extraArgs)
            Next
            Return acc
        End Function

        ''' <summary>
        ''' Runs a reduce3 definition like a parallel kernel: each half gets its own state from CreateReduceState, the halves
        ''' are merged with Merge and MergeReduceState, and PostProcess sees the merged state.
        ''' </summary>
        Public Function FoldPair(ByVal definition As ReduceDefinition, ByVal xs As Double(), ByVal ys As Double(), ByVal extraArgs As Double()) As Double
            Dim mid As Integer = xs.Length \ 2
            Dim state1 As Double() = ManagedOpMath.CreateReduceState(definition.Name, extraArgs)
            Dim state2 As Double() = ManagedOpMath.CreateReduceState(definition.Name, extraArgs)
            Dim first As Double = FoldPairRange(definition, xs, ys, 0, mid, state1)
            Dim second As Double = FoldPairRange(definition, xs, ys, mid, xs.Length, state2)
            Dim acc As Double = definition.Merge(first, second, state1)
            ManagedOpMath.MergeReduceState(definition.Name, state1, state2)
            Return definition.PostProcess(acc, xs.LongLength, state1)
        End Function

        Private Function FoldPairRange(ByVal definition As ReduceDefinition, ByVal xs As Double(), ByVal ys As Double(), ByVal start As Integer, ByVal [end] As Integer, ByVal state As Double()) As Double
            Dim acc As Double = definition.StartingValue(state)
            For i As Integer = start To [end] - 1
                acc = definition.Update(acc, definition.MapPair(xs(i), ys(i), state), state)
            Next
            Return acc
        End Function

        ''' <summary>A sequential index reduction: starts from (StartingValue, -1) and maps a final -1 to EmptyIndex.</summary>
        Public Function IndexFold(ByVal definition As IndexReduceDefinition, ByVal values As Double(), ByVal extraArgs As Double()) As Long
            Dim best As Double = definition.StartingValue(extraArgs)
            Dim bestIndex As Long = -1
            IndexFoldRange(definition, values, 0, values.Length, extraArgs, best, bestIndex)
            Return If(bestIndex < 0, definition.EmptyIndex, bestIndex)
        End Function

        ''' <summary>
        ''' An index reduction in two halves, the second half folded first and offered as the candidate to the first
        ''' (the order a parallel merge may see), each half starting from (StartingValue, -1).
        ''' </summary>
        Public Function IndexFoldSplit(ByVal definition As IndexReduceDefinition, ByVal values As Double(), ByVal extraArgs As Double()) As Long
            Dim mid As Integer = values.Length \ 2
            Dim best1 As Double = definition.StartingValue(extraArgs)
            Dim index1 As Long = -1
            Dim best2 As Double = definition.StartingValue(extraArgs)
            Dim index2 As Long = -1
            IndexFoldRange(definition, values, mid, values.Length, extraArgs, best2, index2)
            IndexFoldRange(definition, values, 0, mid, extraArgs, best1, index1)
            ' merge: offer the first half's result to the second's (a smaller index arriving late)
            If index1 >= 0 AndAlso definition.Prefer(best1, index1, best2, index2, extraArgs) Then
                best2 = best1
                index2 = index1
            End If
            Return If(index2 < 0, definition.EmptyIndex, index2)
        End Function

        Private Sub IndexFoldRange(ByVal definition As IndexReduceDefinition, ByVal values As Double(), ByVal start As Integer, ByVal [end] As Integer,
                                   ByVal extraArgs As Double(), ByRef best As Double, ByRef bestIndex As Long)
            For i As Integer = start To [end] - 1
                If definition.Prefer(values(i), i, best, bestIndex, extraArgs) Then
                    best = values(i)
                    bestIndex = i
                End If
            Next
        End Sub

        ''' <summary>The transform-kind names, and the custom ops whose math lives in the transform table.</summary>
        Public Function AllTransformNames() As SortedSet(Of String)
            Dim result As New SortedSet(Of String)(StringComparer.Ordinal)
            For Each kind As ManagedOpKind In New ManagedOpKind() {ManagedOpKind.TransformSame, ManagedOpKind.TransformFloat,
                                                                    ManagedOpKind.TransformStrict, ManagedOpKind.TransformBool, ManagedOpKind.TransformAny}
                result.UnionWith(ManagedOpMath.Names(kind))
            Next
            AddCustom(result, ManagedOpKind.TransformSame, ManagedOpKind.TransformFloat, ManagedOpKind.TransformStrict, ManagedOpKind.TransformBool, ManagedOpKind.TransformAny)
            Return result
        End Function

        ''' <summary>The pairwise/broadcast names, and the custom ops whose math lives in the pairwise table.</summary>
        Public Function AllPairwiseNames() As SortedSet(Of String)
            Dim result As New SortedSet(Of String)(StringComparer.Ordinal)
            For Each kind As ManagedOpKind In New ManagedOpKind() {ManagedOpKind.Pairwise, ManagedOpKind.PairwiseBool,
                                                                    ManagedOpKind.Broadcast, ManagedOpKind.BroadcastBool}
                result.UnionWith(ManagedOpMath.Names(kind))
            Next
            AddCustom(result, ManagedOpKind.Pairwise, ManagedOpKind.PairwiseBool)
            Return result
        End Function

        Private Sub AddCustom(ByVal result As SortedSet(Of String), ParamArray families As ManagedOpKind())
            For Each name As String In ManagedOpMath.Names(ManagedOpKind.Custom)
                Dim family As ManagedOpKind
                If ManagedOpMath.TryGetCustomFamily(name, family) AndAlso Array.IndexOf(families, family) >= 0 Then result.Add(name)
            Next
        End Sub

        ''' <summary>Evaluates every case with the given lookup and reports all failures at once.</summary>
        Public Sub CheckCases(ByVal cases As IEnumerable(Of OpCase), ByVal evaluate As Func(Of OpCase, Double))
            Dim failures As New List(Of String)()
            For Each c As OpCase In cases
                Dim actual As Double
                Try
                    actual = evaluate(c)
                Catch ex As Exception
                    failures.Add(c.ToString() & " threw " & ex.GetType().Name & ": " & ex.Message)
                    Continue For
                End Try
                Dim ok As Boolean
                If Double.IsNaN(c.Expected) Then
                    ok = Double.IsNaN(actual)
                ElseIf Double.IsInfinity(c.Expected) Then
                    ok = (c.Expected = actual)
                Else
                    ok = Not Double.IsNaN(actual) AndAlso SysMath.Abs(c.Expected - actual) <= c.Delta
                End If
                If Not ok Then failures.Add(c.ToString() & ": expected " & c.Expected.ToString("R") & " but was " & actual.ToString("R"))
            Next
            If failures.Count > 0 Then fail(failures.Count & " case(s) failed:" & Environment.NewLine & String.Join(Environment.NewLine, failures))
        End Sub

        ''' <summary>Fails listing every name that has no case.</summary>
        Public Sub CheckCoverage(ByVal names As IEnumerable(Of String), ByVal cases As IEnumerable(Of OpCase), ByVal what As String)
            Dim covered As New HashSet(Of String)(StringComparer.Ordinal)
            For Each c As OpCase In cases
                covered.Add(c.Name)
            Next
            Dim missing As New List(Of String)()
            For Each n As String In names
                If Not covered.Contains(n) Then missing.Add(n)
            Next
            If missing.Count > 0 Then fail(what & " without a value check: " & String.Join(", ", missing))
        End Sub

    End Module

End Namespace
