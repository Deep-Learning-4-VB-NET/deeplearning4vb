' DeepLearning4VB: the tables behind ManagedOpMath (phase 2, see docs/PHASE2_DESIGN.md). New code, no Java original.

Imports System
Imports System.Collections.Generic

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' The registry's storage: one dictionary per lookup (transforms, pairwise/broadcast, scalar, reductions, index
    ''' reductions, summary statistics), keyed by the Java <c>opName()</c> with ordinal comparison, plus the set of
    ''' names registered under each <see cref="ManagedOpKind"/>. Filled once by the family classes and read-only after.
    ''' </summary>
    Friend NotInheritable Class OpTable

        Friend ReadOnly TransformTable As New Dictionary(Of String, TransformFunction)(StringComparer.Ordinal)
        Friend ReadOnly PairwiseTable As New Dictionary(Of String, PairwiseFunction)(StringComparer.Ordinal)
        Friend ReadOnly ScalarTable As New Dictionary(Of String, PairwiseFunction)(StringComparer.Ordinal)
        Friend ReadOnly ReduceTable As New Dictionary(Of String, ReduceDefinition)(StringComparer.Ordinal)
        Friend ReadOnly IndexReduceTable As New Dictionary(Of String, IndexReduceDefinition)(StringComparer.Ordinal)
        Friend ReadOnly SummaryStatsTable As New Dictionary(Of String, SummaryStatsDefinition)(StringComparer.Ordinal)
        Friend ReadOnly KindNames As New Dictionary(Of ManagedOpKind, SortedSet(Of String))()
        ''' <summary>Custom (DynamicCustomOp) names whose element-wise math is registered, and the lookup family that holds it.</summary>
        Friend ReadOnly CustomFamilies As New Dictionary(Of String, ManagedOpKind)(StringComparer.Ordinal)
        ''' <summary>libnd4j's <c>extraParamsLen</c>: leading extra-argument slots a reduce3 op uses as accumulators.</summary>
        Friend ReadOnly AccumulatorSlots As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

        Friend Shared Function Build() As OpTable
            Dim t As New OpTable()
            TransformMath.Register(t)
            PairwiseMath.Register(t)
            ScalarMath.Register(t)
            ReduceMath.Register(t)
            Reduce3Math.Register(t)
            IndexReduceMath.Register(t)
            SummaryStatsMath.Register(t)
            Return t
        End Function

        Private Sub MarkKind(ByVal kind As ManagedOpKind, ByVal name As String)
            Dim names As SortedSet(Of String) = Nothing
            If Not KindNames.TryGetValue(kind, names) Then
                names = New SortedSet(Of String)(StringComparer.Ordinal)
                KindNames(kind) = names
            End If
            names.Add(name)
        End Sub

        Private Shared Sub AddUnique(Of T As Class)(ByVal table As Dictionary(Of String, T), ByVal name As String, ByVal value As T, ByVal what As String)
            Dim existing As T = Nothing
            If table.TryGetValue(name, existing) Then
                If Not Object.ReferenceEquals(existing, value) Then
                    Throw New InvalidOperationException("Two different " & what & " definitions for op '" & name & "'")
                End If
                Return
            End If
            table.Add(name, value)
        End Sub

        Private Shared Sub CheckKind(ByVal kind As ManagedOpKind, ByVal name As String, ParamArray allowed As ManagedOpKind())
            If Array.IndexOf(allowed, kind) < 0 Then
                Throw New ArgumentException("Op '" & name & "' cannot be registered as " & kind.ToString())
            End If
        End Sub

        ''' <summary>A legacy transform (same, float, strict, bool or any).</summary>
        Friend Sub AddTransform(ByVal kind As ManagedOpKind, ByVal name As String, ByVal fn As TransformFunction)
            CheckKind(kind, name, ManagedOpKind.TransformSame, ManagedOpKind.TransformFloat, ManagedOpKind.TransformStrict,
                      ManagedOpKind.TransformBool, ManagedOpKind.TransformAny)
            AddUnique(TransformTable, name, fn, "transform")
            MarkKind(kind, name)
        End Sub

        ''' <summary>A legacy pairwise or broadcast op (plain or bool).</summary>
        Friend Sub AddPairwise(ByVal kind As ManagedOpKind, ByVal name As String, ByVal fn As PairwiseFunction)
            CheckKind(kind, name, ManagedOpKind.Pairwise, ManagedOpKind.PairwiseBool, ManagedOpKind.Broadcast, ManagedOpKind.BroadcastBool)
            AddUnique(PairwiseTable, name, fn, "pairwise")
            MarkKind(kind, name)
        End Sub

        ''' <summary>A legacy scalar op (plain or bool).</summary>
        Friend Sub AddScalar(ByVal kind As ManagedOpKind, ByVal name As String, ByVal fn As PairwiseFunction)
            CheckKind(kind, name, ManagedOpKind.Scalar, ManagedOpKind.ScalarBool)
            AddUnique(ScalarTable, name, fn, "scalar")
            MarkKind(kind, name)
        End Sub

        ''' <summary>A legacy reduction (float, same, long, bool) or reduce3 op.</summary>
        Friend Sub AddReduce(ByVal kind As ManagedOpKind, ByVal definition As ReduceDefinition)
            CheckKind(kind, definition.Name, ManagedOpKind.ReduceFloat, ManagedOpKind.ReduceSame, ManagedOpKind.ReduceLong,
                      ManagedOpKind.ReduceBool, ManagedOpKind.Reduce3)
            AddUnique(ReduceTable, definition.Name, definition, "reduce")
            MarkKind(kind, definition.Name)
        End Sub

        ''' <summary>A legacy index reduction.</summary>
        Friend Sub AddIndexReduce(ByVal definition As IndexReduceDefinition)
            AddUnique(IndexReduceTable, definition.Name, definition, "index reduce")
            MarkKind(ManagedOpKind.IndexReduce, definition.Name)
        End Sub

        ''' <summary>A summary-statistics op (variance, standard deviation).</summary>
        Friend Sub AddSummaryStats(ByVal definition As SummaryStatsDefinition)
            AddUnique(SummaryStatsTable, definition.Name, definition, "summary stats")
            MarkKind(ManagedOpKind.SummaryStats, definition.Name)
        End Sub

        ''' <summary>The element-wise math of a custom op that applies one function per element.</summary>
        Friend Sub AddCustomTransform(ByVal family As ManagedOpKind, ByVal name As String, ByVal fn As TransformFunction)
            CheckKind(family, name, ManagedOpKind.TransformSame, ManagedOpKind.TransformFloat, ManagedOpKind.TransformStrict,
                      ManagedOpKind.TransformBool, ManagedOpKind.TransformAny)
            AddUnique(TransformTable, name, fn, "transform")
            AddCustom(family, name)
        End Sub

        ''' <summary>The element-wise math of a broadcastable custom op with two inputs (z = f(in0, in1)).</summary>
        Friend Sub AddCustomPairwise(ByVal family As ManagedOpKind, ByVal name As String, ByVal fn As PairwiseFunction)
            CheckKind(family, name, ManagedOpKind.Pairwise, ManagedOpKind.PairwiseBool)
            AddUnique(PairwiseTable, name, fn, "pairwise")
            AddCustom(family, name)
        End Sub

        ''' <summary>The fold of a custom reduction op.</summary>
        Friend Sub AddCustomReduce(ByVal family As ManagedOpKind, ByVal definition As ReduceDefinition)
            CheckKind(family, definition.Name, ManagedOpKind.ReduceFloat, ManagedOpKind.ReduceSame, ManagedOpKind.ReduceLong, ManagedOpKind.ReduceBool)
            AddUnique(ReduceTable, definition.Name, definition, "reduce")
            AddCustom(family, definition.Name)
        End Sub

        ''' <summary>The comparison of a custom index-reduction op (argmax and friends).</summary>
        Friend Sub AddCustomIndexReduce(ByVal definition As IndexReduceDefinition)
            AddUnique(IndexReduceTable, definition.Name, definition, "index reduce")
            AddCustom(ManagedOpKind.IndexReduce, definition.Name)
        End Sub

        Private Sub AddCustom(ByVal family As ManagedOpKind, ByVal name As String)
            Dim existing As ManagedOpKind
            If CustomFamilies.TryGetValue(name, existing) AndAlso existing <> family Then
                Throw New InvalidOperationException("Custom op '" & name & "' registered in two families")
            End If
            CustomFamilies(name) = family
            MarkKind(ManagedOpKind.Custom, name)
        End Sub

    End Class

    ''' <summary>Reads an op's extra arguments with defaults (the arrays may be shorter than the op's full argument list).</summary>
    Friend NotInheritable Class OpArgs

        Private Sub New()
        End Sub

        ''' <summary>Extra argument <paramref name="index"/>, or <paramref name="defaultValue"/> when there are not that many.</summary>
        Friend Shared Function [Get](ByVal extraArgs As Double(), ByVal index As Integer, ByVal defaultValue As Double) As Double
            If extraArgs Is Nothing OrElse extraArgs.Length <= index Then Return defaultValue
            Return extraArgs(index)
        End Function

        ''' <summary>Extra argument <paramref name="index"/>; throws when the op was not given it.</summary>
        Friend Shared Function Require(ByVal extraArgs As Double(), ByVal index As Integer, ByVal opName As String, ByVal argName As String) As Double
            If extraArgs Is Nothing OrElse extraArgs.Length <= index Then
                Throw New ArgumentException("Op '" & opName & "' needs extra argument " & index & " (" & argName & ")", NameOf(extraArgs))
            End If
            Return extraArgs(index)
        End Function

    End Class

End Namespace
