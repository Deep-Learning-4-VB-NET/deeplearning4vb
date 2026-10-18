Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' The registry API (lookups, TryGet forms, Supports, Names, custom families) and the inventory check: every legacy op
    ''' of docs/phase2/op-inventory.md is registered under its kind.
    ''' </summary>
    <TestClass>
    Public Class ManagedOpMathRegistryTest

        <TestMethod>
        Public Sub EveryLegacyOpOfTheInventory_IsRegistered()
            Dim missing As New List(Of String)()
            For Each op In LegacyOps()
                If Not ManagedOpMath.Supports(op.Kind, op.Name) Then missing.Add(op.Kind.ToString() & " " & op.Name)
            Next
            If missing.Count > 0 Then fail("Inventory ops not registered: " & String.Join(", ", missing))
            assertTrue(LegacyOps().Length >= 170)
        End Sub

        <TestMethod>
        Public Sub EveryLegacyOp_IsReachableThroughItsLookup()
            For Each op In LegacyOps()
                Select Case op.Kind
                    Case ManagedOpKind.TransformSame, ManagedOpKind.TransformFloat, ManagedOpKind.TransformStrict, ManagedOpKind.TransformBool, ManagedOpKind.TransformAny
                        assertNotNull(ManagedOpMath.Transform(op.Name))
                    Case ManagedOpKind.Pairwise, ManagedOpKind.PairwiseBool, ManagedOpKind.Broadcast, ManagedOpKind.BroadcastBool
                        assertNotNull(ManagedOpMath.Pairwise(op.Name))
                    Case ManagedOpKind.Scalar, ManagedOpKind.ScalarBool
                        assertNotNull(ManagedOpMath.Scalar(op.Name))
                    Case ManagedOpKind.ReduceFloat, ManagedOpKind.ReduceSame, ManagedOpKind.ReduceLong, ManagedOpKind.ReduceBool, ManagedOpKind.Reduce3
                        Dim definition As ReduceDefinition = ManagedOpMath.Reduce(op.Name)
                        assertEquals(op.Name, definition.Name)
                        assertNotNull(definition.StartingValue)
                        assertNotNull(definition.Update)
                        assertNotNull(definition.Merge)
                        assertNotNull(definition.PostProcess)
                        assertTrue((op.Kind = ManagedOpKind.Reduce3) = (definition.MapPair IsNot Nothing), op.Name & ": MapPair only for reduce3")
                        assertTrue((op.Kind = ManagedOpKind.Reduce3) = (definition.Map Is Nothing), op.Name & ": Map for single-input reductions")
                    Case ManagedOpKind.IndexReduce
                        Dim definition As IndexReduceDefinition = ManagedOpMath.IndexReduce(op.Name)
                        assertEquals(op.Name, definition.Name)
                        assertNotNull(definition.Prefer)
                    Case ManagedOpKind.SummaryStats
                        assertEquals(op.Name, ManagedOpMath.SummaryStats(op.Name).Name)
                    Case Else
                        fail("unexpected kind " & op.Kind.ToString() & " for " & op.Name)
                End Select
            Next
        End Sub

        <TestMethod>
        Public Sub CustomMath_IsOnlyForCustomOpsOfTheInventory()
            Dim customs As New HashSet(Of String)(CustomOps(), StringComparer.Ordinal)
            For Each name As String In ManagedOpMath.Names(ManagedOpKind.Custom)
                assertTrue(customs.Contains(name), name & " is not a custom op of the Java API")
                Dim family As ManagedOpKind
                assertTrue(ManagedOpMath.TryGetCustomFamily(name, family), name)
                Select Case family
                    Case ManagedOpKind.TransformSame, ManagedOpKind.TransformFloat, ManagedOpKind.TransformStrict, ManagedOpKind.TransformBool, ManagedOpKind.TransformAny
                        assertNotNull(ManagedOpMath.Transform(name))
                    Case ManagedOpKind.Pairwise, ManagedOpKind.PairwiseBool
                        assertNotNull(ManagedOpMath.Pairwise(name))
                    Case ManagedOpKind.ReduceFloat, ManagedOpKind.ReduceSame, ManagedOpKind.ReduceLong, ManagedOpKind.ReduceBool
                        assertNotNull(ManagedOpMath.Reduce(name))
                    Case ManagedOpKind.IndexReduce
                        assertNotNull(ManagedOpMath.IndexReduce(name))
                    Case Else
                        fail("unexpected family " & family.ToString() & " for " & name)
                End Select
            Next
            assertTrue(ManagedOpMath.Names(ManagedOpKind.Custom).Length >= 50)
            Dim none As ManagedOpKind
            assertFalse(ManagedOpMath.TryGetCustomFamily("matmul", none))
            assertFalse(ManagedOpMath.TryGetCustomFamily(Nothing, none))
        End Sub

        <TestMethod>
        Public Sub NotImplementedLegacyOps_AreAbsent()
            For Each name As String In NotImplemented()
                Dim t As TransformFunction = Nothing
                Dim p As PairwiseFunction = Nothing
                assertFalse(ManagedOpMath.TryGetTransform(name, t), name)
                assertFalse(ManagedOpMath.TryGetPairwise(name, p), name)
            Next
        End Sub

        <TestMethod>
        Public Sub UnknownNames_ThrowNotSupported_TryGetReturnsFalse()
            assertThrows(Of NotSupportedException)(Sub() ManagedOpMath.Transform("no_such_op"))
            assertThrows(Of NotSupportedException)(Sub() ManagedOpMath.Pairwise("no_such_op"))
            assertThrows(Of NotSupportedException)(Sub() ManagedOpMath.Scalar("no_such_op"))
            assertThrows(Of NotSupportedException)(Sub() ManagedOpMath.Reduce("no_such_op"))
            assertThrows(Of NotSupportedException)(Sub() ManagedOpMath.IndexReduce("no_such_op"))
            assertThrows(Of NotSupportedException)(Sub() ManagedOpMath.SummaryStats("no_such_op"))
            assertThrows(Of NotSupportedException)(Sub() ManagedOpMath.Transform(Nothing))
            Dim ex As NotSupportedException = assertThrows(Of NotSupportedException)(Sub() ManagedOpMath.Reduce("reduce_median"))
            assertTrue(ex.Message.Contains("reduce_median"))

            Dim t As TransformFunction = Nothing
            assertFalse(ManagedOpMath.TryGetTransform("no_such_op", t))
            assertNull(t)
            assertFalse(ManagedOpMath.TryGetTransform(Nothing, t))
            Dim r As ReduceDefinition = Nothing
            assertFalse(ManagedOpMath.TryGetReduce("no_such_op", r))
            assertNull(r)
            Dim s As SummaryStatsDefinition = Nothing
            assertFalse(ManagedOpMath.TryGetSummaryStats("no_such_op", s))
            assertFalse(ManagedOpMath.Supports(ManagedOpKind.TransformSame, Nothing))
        End Sub

        <TestMethod>
        Public Sub Lookups_AreOrdinal_LikeJavaOpNames()
            Dim t As TransformFunction = Nothing
            assertTrue(ManagedOpMath.TryGetTransform("Reciprocal", t))
            assertFalse(ManagedOpMath.TryGetTransform("reciprocal", t))
            assertFalse(ManagedOpMath.TryGetTransform("ABS", t))
            Dim p As PairwiseFunction = Nothing
            assertTrue(ManagedOpMath.TryGetPairwise("Pow", p))
            assertFalse(ManagedOpMath.TryGetPairwise("pow", p))
            assertTrue(ManagedOpMath.TryGetScalar("pow", p))
            assertTrue(ManagedOpMath.Supports(ManagedOpKind.ReduceLong, "countNonZero"))
            assertFalse(ManagedOpMath.Supports(ManagedOpKind.ReduceLong, "countnonzero"))
        End Sub

        <TestMethod>
        Public Sub Names_AreSortedDistinctAndPerKind()
            Dim total As Integer = 0
            For Each kind As ManagedOpKind In [Enum].GetValues(GetType(ManagedOpKind))
                Dim names As String() = ManagedOpMath.Names(kind)
                Dim sorted As String() = names.OrderBy(Function(n) n, StringComparer.Ordinal).ToArray()
                assertEquals(sorted, names)
                assertEquals(names.Length, names.Distinct(StringComparer.Ordinal).Count())
                For Each n As String In names
                    assertTrue(ManagedOpMath.Supports(kind, n), kind.ToString() & " " & n)
                Next
                total += names.Length
                ' a copy each time
                If names.Length > 0 Then
                    names(0) = "changed"
                    assertFalse(ManagedOpMath.Names(kind).Contains("changed"))
                End If
            Next
            assertEquals(0, ManagedOpMath.Names(ManagedOpKind.Random).Length)
            assertTrue(total > 220)
        End Sub

        <TestMethod>
        Public Sub Lookups_ReturnTheSameDelegateEachTime()
            assertTrue(ManagedOpMath.Transform("sigmoid") Is ManagedOpMath.Transform("sigmoid"))
            assertTrue(ManagedOpMath.Reduce("reduce_sum") Is ManagedOpMath.Reduce("reduce_sum"))
            ' scalar and broadcast ops share the element math of their pairwise counterparts
            assertTrue(ManagedOpMath.Scalar("add_scalar") Is ManagedOpMath.Pairwise("broadcastadd"))
        End Sub

    End Class

End Namespace
