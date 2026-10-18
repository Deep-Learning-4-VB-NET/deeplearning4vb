Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    <TestClass>
    Public Class IndexReduceKernelsTest

        Private Shared ReadOnly Layouts As (Order As Char, Offset As Long, Spacing As Long)() = {
            ("c"c, 0L, 1L), ("f"c, 0L, 1L), ("f"c, 2L, 3L)}

        <TestMethod>
        Public Sub TiesKeepTheFirstIndex()
            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {6}, "c"c, New Double() {1, 5, 3, 5, 0, 0})
            Assert.AreEqual(1L, IndexReduceKernels.IndexReduceToScalar(x.Storage, x.Offset, x.Info, KernelTestUtil.ArgMaxDefinition(), Nothing))
            Assert.AreEqual(4L, IndexReduceKernels.IndexReduceToScalar(x.Storage, x.Offset, x.Info, KernelTestUtil.ArgMinDefinition(), Nothing))
            Dim same As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {4}, "c"c, New Double() {2, 2, 2, 2})
            Assert.AreEqual(0L, IndexReduceKernels.IndexReduceToScalar(same.Storage, same.Offset, same.Info, KernelTestUtil.ArgMaxDefinition(), Nothing))
            ' all -Infinity: nothing beats the starting value, EmptyIndex (0) comes back
            Dim inf As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {3}, "c"c, New Double() {Double.NegativeInfinity, Double.NegativeInfinity, Double.NegativeInfinity})
            Assert.AreEqual(0L, IndexReduceKernels.IndexReduceToScalar(inf.Storage, inf.Offset, inf.Info, KernelTestUtil.ArgMaxDefinition(), Nothing))
        End Sub

        <TestMethod>
        Public Sub IndexesAreRowMajorLogicalNotMemoryOrder()
            ' f-ordered 2x3 with logical values [[1,2,9],[9,0,3]]: the first 9 in row-major order is element 2,
            ' although element 3 comes first in memory
            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {2, 3}, "f"c, New Double() {1, 2, 9, 9, 0, 3})
            Assert.AreEqual(2L, IndexReduceKernels.IndexReduceToScalar(x.Storage, x.Offset, x.Info, KernelTestUtil.ArgMaxDefinition(), Nothing))
            Assert.AreEqual(4L, IndexReduceKernels.IndexReduceToScalar(x.Storage, x.Offset, x.Info, KernelTestUtil.ArgMinDefinition(), Nothing))
        End Sub

        <TestMethod>
        Public Sub AlongEveryDimensionSet()
            Dim shape As Long() = {3, 2, 4}
            Dim values As Double() = New Double(23) {}
            For i As Integer = 0 To 23
                values(i) = (i * 5) Mod 7   ' many ties
            Next
            For Each dims As Integer() In KernelTestUtil.AllDimensionSets(3)
                Dim tadDims As Integer() = If(dims.Length = 0, New Integer() {0, 1, 2}, dims)
                Dim outer As Integer() = KernelTestUtil.Outer(3, dims)
                Dim groups As Integer = 1
                For Each d As Integer In outer
                    groups *= CInt(shape(d))
                Next
                Dim bestValue As Double() = New Double(groups - 1) {}
                Dim bestIndex As Long() = New Long(groups - 1) {}
                For g As Integer = 0 To groups - 1
                    bestValue(g) = Double.NegativeInfinity
                    bestIndex(g) = -1
                Next
                For i As Long = 0 To 23
                    Dim coords As Long() = KernelTestUtil.Coordinates(shape, i)
                    Dim g As Integer = CInt(KernelTestUtil.LinearIndex(shape, coords, outer))
                    Dim j As Long = KernelTestUtil.LinearIndex(shape, coords, tadDims)
                    If values(CInt(i)) > bestValue(g) OrElse (values(CInt(i)) = bestValue(g) AndAlso j < bestIndex(g)) Then
                        bestValue(g) = values(CInt(i))
                        bestIndex(g) = j
                    End If
                Next
                For Each xl In Layouts
                    Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, xl.Order, values, xl.Offset, xl.Spacing)
                    Dim outerShape As Long() = TadPack.OuterShape(x.Info, dims)
                    Dim z As KernelTestArray = KernelTestArray.Create(StorageType.LONG, outerShape, "f"c, 1)
                    IndexReduceKernels.IndexReduceAlongDimensions(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, dims, KernelTestUtil.ArgMaxDefinition(), Nothing)
                    Dim exp As Double() = New Double(groups - 1) {}
                    For g As Integer = 0 To groups - 1
                        exp(g) = bestIndex(g)
                    Next
                    z.AssertValues(exp, 0.0, "argmax dims " & KernelTestUtil.Describe(dims) & " " & xl.ToString())
                    z.AssertOutsideUntouched()
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub ChunkMergesKeepSequentialSemantics()
            Dim n As Integer = 50000
            Dim data As Double() = New Double(n - 1) {}
            For i As Integer = 0 To n - 1
                data(i) = Math.Sin(i * 0.01)
            Next
            ' the maximum 2.0 appears in several chunks; the first one wins
            data(31000) = 2.0
            data(17003) = 2.0
            data(44000) = 2.0
            data(2) = -3.0
            data(49999) = -3.0
            Dim s As ManagedStorage = ManagedStorage.FromArray(data)
            Dim info As Long() = ShapeInfoHelper.Create(New Long() {n}, "c"c)
            Dim check As Action =
                Sub()
                    Assert.AreEqual(17003L, IndexReduceKernels.IndexReduceToScalar(s, 0, info, KernelTestUtil.ArgMaxDefinition(), Nothing))
                    Assert.AreEqual(2L, IndexReduceKernels.IndexReduceToScalar(s, 0, info, KernelTestUtil.ArgMinDefinition(), Nothing))
                    Assert.AreEqual(17003L, IndexReduceKernels.IndexReduceToScalar(s, 0, info, KernelTestUtil.FirstIndexAboveDefinition(), New Double() {1.5}))
                    Assert.AreEqual(44000L, IndexReduceKernels.IndexReduceToScalar(s, 0, info, KernelTestUtil.LastIndexAboveDefinition(), New Double() {1.5}))
                    Assert.AreEqual(-1L, IndexReduceKernels.IndexReduceToScalar(s, 0, info, KernelTestUtil.FirstIndexAboveDefinition(), New Double() {5.0}))
                End Sub
            check()
            KernelTestUtil.WithLowThresholds(check)
            ' one long TAD per row of a 2 x 25000 view, reduced with parallel chunks
            Dim m As ManagedStorage = ManagedStorage.FromArray(data)
            Dim mInfo As Long() = ShapeInfoHelper.Create(New Long() {2, 25000}, "c"c)
            Dim z As ManagedStorage = ManagedStorage.Create(StorageType.INT, 2)
            KernelTestUtil.WithLowThresholds(
                Sub() IndexReduceKernels.IndexReduceAlongDimensions(m, 0, mInfo, z, 0, ShapeInfoHelper.Create(New Long() {2}, "c"c), New Integer() {1}, KernelTestUtil.ArgMaxDefinition(), Nothing))
            CollectionAssert.AreEqual(New Integer() {17003, 31000 - 25000}, z.Ints)
        End Sub

        <TestMethod>
        Public Sub EmptyInputsAndValidation()
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {2, 3}, "c"c)
            Dim empty As Long() = ShapeInfoHelper.Create(New Long() {0}, "c"c)
            Assert.AreEqual(-1L, IndexReduceKernels.IndexReduceToScalar(x.Storage, 0, empty, KernelTestUtil.FirstIndexAboveDefinition(), New Double() {0.0}))
            Assert.AreEqual(0L, IndexReduceKernels.IndexReduceToScalar(x.Storage, 0, empty, KernelTestUtil.ArgMaxDefinition(), Nothing))
            Dim emptyTads As Long() = ShapeInfoHelper.Create(New Long() {2, 0}, "c"c)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.LONG, New Long() {2}, "c"c)
            IndexReduceKernels.IndexReduceAlongDimensions(x.Storage, 0, emptyTads, z.Storage, z.Offset, z.Info, New Integer() {1}, KernelTestUtil.FirstIndexAboveDefinition(), New Double() {0.0})
            z.AssertValues(New Double() {-1, -1})
            Dim noPrefer As New IndexReduceDefinition()
            Assert.ThrowsException(Of ArgumentException)(Function() IndexReduceKernels.IndexReduceToScalar(x.Storage, x.Offset, x.Info, noPrefer, Nothing))
            Dim wrong As KernelTestArray = KernelTestArray.Create(StorageType.LONG, New Long() {3}, "c"c)
            Assert.ThrowsException(Of ArgumentException)(Sub() IndexReduceKernels.IndexReduceAlongDimensions(x.Storage, x.Offset, x.Info, wrong.Storage, wrong.Offset, wrong.Info, New Integer() {1}, KernelTestUtil.ArgMaxDefinition(), Nothing))
        End Sub

    End Class

    <TestClass>
    Public Class SummaryStatsKernelsTest

        Private Shared Function TwoPassVariance(ByVal values As Double(), ByVal biasCorrected As Boolean) As Double
            If values.Length <= 1 Then
                Return 0.0
            End If
            Dim mean As Double = 0
            For Each v As Double In values
                mean += v
            Next
            mean /= values.Length
            Dim ss As Double = 0
            For Each v As Double In values
                ss += (v - mean) * (v - mean)
            Next
            Return ss / If(biasCorrected, values.Length - 1, values.Length)
        End Function

        <TestMethod>
        Public Sub VarianceWithAndWithoutBiasCorrection()
            Dim values As Double() = {2, 4, 4, 4, 5, 5, 7, 9}
            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {2, 4}, "f"c, values, 1, 2)
            Assert.AreEqual(4.0, SummaryStatsKernels.Variance(x.Storage, x.Offset, x.Info, False), 1.0E-12)
            Assert.AreEqual(32.0 / 7.0, SummaryStatsKernels.Variance(x.Storage, x.Offset, x.Info, True), 1.0E-12)
            Assert.AreEqual(2.0, SummaryStatsKernels.StandardDeviation(x.Storage, x.Offset, x.Info, False), 1.0E-12)
            Assert.AreEqual(Math.Sqrt(32.0 / 7.0), SummaryStatsKernels.StandardDeviation(x.Storage, x.Offset, x.Info, True), 1.0E-12)
            ' a single element and an empty array give 0, as libnd4j does
            Dim one As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {1}, "c"c, New Double() {3})
            Assert.AreEqual(0.0, SummaryStatsKernels.Variance(one.Storage, one.Offset, one.Info, True), 0.0)
            Assert.AreEqual(0.0, SummaryStatsKernels.Variance(one.Storage, 0, ShapeInfoHelper.Create(New Long() {0}, "c"c), False), 0.0)
        End Sub

        <TestMethod>
        Public Sub VarianceAlongDimensions()
            Dim shape As Long() = {3, 4, 5}
            Dim values As Double() = New Double(59) {}
            For i As Integer = 0 To 59
                values(i) = ((i * 37) Mod 11) * 0.5 - 2.0
            Next
            For Each order As Char In {"c"c, "f"c}
                Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, order, values, 2, 2)
                For Each dims As Integer() In KernelTestUtil.AllDimensionSets(3)
                    Dim outer As Integer() = KernelTestUtil.Outer(3, dims)
                    Dim groups As Integer = 1
                    For Each d As Integer In outer
                        groups *= CInt(shape(d))
                    Next
                    Dim members As New System.Collections.Generic.List(Of Double)()
                    For Each bias As Boolean In {False, True}
                        Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, TadPack.OuterShape(x.Info, dims), "c"c, 1)
                        Dim zs As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, TadPack.OuterShape(x.Info, dims), "f"c)
                        SummaryStatsKernels.VarianceAlongDimensions(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, dims, bias, False)
                        SummaryStatsKernels.VarianceAlongDimensions(x.Storage, x.Offset, x.Info, zs.Storage, zs.Offset, zs.Info, dims, bias, True)
                        For g As Integer = 0 To groups - 1
                            members.Clear()
                            For i As Long = 0 To 59
                                If KernelTestUtil.LinearIndex(shape, KernelTestUtil.Coordinates(shape, i), outer) = g Then
                                    members.Add(values(CInt(i)))
                                End If
                            Next
                            Dim expected As Double = TwoPassVariance(members.ToArray(), bias)
                            Dim label As String = order & " dims " & KernelTestUtil.Describe(dims) & " bias " & bias & " group " & g
                            Assert.AreEqual(expected, z.GetValue(g), 1.0E-12, label)
                            Assert.AreEqual(Math.Sqrt(expected), zs.GetValue(g), 1.0E-5, label & " std")
                        Next
                        z.AssertOutsideUntouched()
                    Next
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub ParallelWelfordMatchesTwoPass()
            Dim n As Integer = 123457
            Dim data As Double() = New Double(n - 1) {}
            Dim rng As New Random(5)
            For i As Integer = 0 To n - 1
                data(i) = 1000.0 + rng.NextDouble() * 3.0   ' large mean, small spread: hard for naive formulas
            Next
            Dim s As ManagedStorage = ManagedStorage.FromArray(data)
            Dim info As Long() = ShapeInfoHelper.Create(New Long() {n}, "c"c)
            Dim expected As Double = TwoPassVariance(data, True)
            Assert.AreEqual(expected, SummaryStatsKernels.Variance(s, 0, info, True), expected * 1.0E-10)
            KernelTestUtil.WithLowThresholds(
                Sub()
                    Assert.AreEqual(expected, SummaryStatsKernels.Variance(s, 0, info, True), expected * 1.0E-10)
                    Dim z As ManagedStorage = ManagedStorage.Create(StorageType.DOUBLE, 1)
                    SummaryStatsKernels.VarianceAlongDimensions(s, 0, info, z, 0, ShapeInfoHelper.Create(New Long() {1}, "c"c), New Integer() {0}, True, False)
                    Assert.AreEqual(expected, z.GetDouble(0), expected * 1.0E-10)
                End Sub)
        End Sub

    End Class

End Namespace
