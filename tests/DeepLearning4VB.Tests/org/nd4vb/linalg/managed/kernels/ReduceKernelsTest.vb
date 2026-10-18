Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    <TestClass>
    Public Class ReduceKernelsTest

        Private Shared ReadOnly Shape3 As Long() = {2, 3, 4}
        Private Shared ReadOnly Layouts As (Order As Char, Offset As Long, Spacing As Long)() = {
            ("c"c, 0L, 1L), ("f"c, 0L, 1L), ("c"c, 2L, 3L), ("f"c, 1L, 2L)}

        ''' <summary>Deterministic integer test values (sums are exact in double).</summary>
        Private Shared Function TestValues(ByVal n As Long, Optional ByVal seed As Integer = 0) As Double()
            Dim v As Double() = New Double(CInt(n) - 1) {}
            For i As Integer = 0 To v.Length - 1
                v(i) = ((CLng(i + seed) * 7919L) Mod 23L) - 11
            Next
            Return v
        End Function

        ''' <summary>Brute-force reduction along dims: groups elements by their outer coordinates (row-major).</summary>
        Private Shared Function Expected(ByVal shape As Long(), ByVal values As Double(), ByVal dims As Integer(), ByVal start As Double,
                                         ByVal update As Func(Of Double, Double, Double), ByVal post As Func(Of Double, Long, Double)) As Double()
            Dim outer As Integer() = KernelTestUtil.Outer(shape.Length, dims)
            Dim groups As Long = 1
            For Each d As Integer In outer
                groups *= shape(d)
            Next
            Dim acc As Double() = New Double(CInt(groups) - 1) {}
            Dim counts As Long() = New Long(CInt(groups) - 1) {}
            For g As Integer = 0 To acc.Length - 1
                acc(g) = start
            Next
            For i As Long = 0 To values.Length - 1
                Dim g As Long = KernelTestUtil.LinearIndex(shape, KernelTestUtil.Coordinates(shape, i), outer)
                acc(CInt(g)) = update(acc(CInt(g)), values(CInt(i)))
                counts(CInt(g)) += 1
            Next
            For g As Integer = 0 To acc.Length - 1
                acc(g) = post(acc(g), counts(g))
            Next
            Return acc
        End Function

        Private Shared Function OuterShape(ByVal shape As Long(), ByVal dims As Integer(), ByVal keepDims As Boolean) As Long()
            Dim result As New List(Of Long)()
            For d As Integer = 0 To shape.Length - 1
                Dim reduced As Boolean = dims.Length = 0 OrElse Array.IndexOf(dims, d) >= 0
                If Not reduced Then
                    result.Add(shape(d))
                ElseIf keepDims Then
                    result.Add(1)
                End If
            Next
            Return result.ToArray()
        End Function

        Private Sub CheckAllDimensionSets(ByVal type As StorageType)
            Dim values As Double() = TestValues(24)
            Dim defs As (Def As ReduceDefinition, Start As Double, Update As Func(Of Double, Double, Double), Post As Func(Of Double, Long, Double))() = {
                (KernelTestUtil.SumDefinition(), 0.0, Function(a As Double, b As Double) a + b, Function(a As Double, n As Long) a),
                (KernelTestUtil.MaxDefinition(), Double.NegativeInfinity, Function(a As Double, b As Double) Math.Max(a, b), Function(a As Double, n As Long) a),
                (KernelTestUtil.MeanDefinition(), 0.0, Function(a As Double, b As Double) a + b, Function(a As Double, n As Long) a / n)}
            For Each def In defs
                For Each dims As Integer() In KernelTestUtil.AllDimensionSets(3)
                    Dim exp As Double() = Expected(Shape3, values, dims, def.Start, def.Update, def.Post)
                    For Each xl In Layouts
                        Dim x As KernelTestArray = KernelTestArray.FromValues(type, Shape3, xl.Order, values, xl.Offset, xl.Spacing)
                        For Each keep As Boolean In {False, True}
                            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, OuterShape(Shape3, dims, keep), If(keep, "f"c, "c"c), 1, 2)
                            ReduceKernels.ReduceAlongDimensions(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, dims, def.Def, Nothing)
                            z.AssertValues(exp, 1.0E-12, def.Def.Name & " " & type.ToString() & " dims " & KernelTestUtil.Describe(dims) & " " & xl.ToString())
                            z.AssertOutsideUntouched()
                        Next
                        If dims.Length = 0 Then
                            Assert.AreEqual(exp(0), ReduceKernels.ReduceToScalar(x.Storage, x.Offset, x.Info, def.Def, Nothing), 1.0E-12, def.Def.Name & " scalar")
                        End If
                        x.AssertOutsideUntouched()
                    Next
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub SumMaxMeanAlongEveryDimensionSetDouble()
            CheckAllDimensionSets(StorageType.DOUBLE)
        End Sub

        <TestMethod>
        Public Sub SumMaxMeanAlongEveryDimensionSetFloatAndInt()
            CheckAllDimensionSets(StorageType.FLOAT)
            CheckAllDimensionSets(StorageType.INT)
        End Sub

        <TestMethod>
        Public Sub SameResultsWithParallelChunks()
            KernelTestUtil.WithLowThresholds(
                Sub()
                    CheckAllDimensionSets(StorageType.DOUBLE)
                    ' one long TAD, reduced in parallel chunks
                    Dim values As Double() = TestValues(5000, 3)
                    Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {2, 2500}, "f"c, values, 1, 2)
                    Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {2}, "c"c)
                    ReduceKernels.ReduceAlongDimensions(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, KernelTestUtil.SumDefinition(), Nothing)
                    Dim e As Double() = Expected(New Long() {2, 2500}, values, New Integer() {1}, 0.0, Function(a As Double, b As Double) a + b, Function(a As Double, n As Long) a)
                    z.AssertValues(e)
                End Sub)
        End Sub

        <TestMethod>
        Public Sub LargeFullReductionMatchesSequential()
            Dim n As Integer = 200003
            Dim data As Double() = New Double(n - 1) {}
            Dim rng As New Random(17)
            For i As Integer = 0 To n - 1
                data(i) = rng.NextDouble() - 0.3
            Next
            Dim s As ManagedStorage = ManagedStorage.FromArray(data)
            Dim info As Long() = ShapeInfoHelper.Create(New Long() {n}, "c"c)
            Dim parallel As Double = ReduceKernels.ReduceToScalar(s, 0, info, KernelTestUtil.SumDefinition(), Nothing)
            Dim again As Double = ReduceKernels.ReduceToScalar(s, 0, info, KernelTestUtil.SumDefinition(), Nothing)
            Assert.AreEqual(parallel, again, 0.0, "parallel reductions are deterministic")
            Dim dop As Integer = KernelOptions.MaxDegreeOfParallelism
            Dim sequential As Double
            Try
                KernelOptions.MaxDegreeOfParallelism = 1
                sequential = ReduceKernels.ReduceToScalar(s, 0, info, KernelTestUtil.SumDefinition(), Nothing)
            Finally
                KernelOptions.MaxDegreeOfParallelism = dop
            End Try
            Dim plain As Double = 0
            For Each v As Double In data
                plain += v
            Next
            Assert.AreEqual(plain, sequential, 1.0E-9)
            Assert.AreEqual(sequential, parallel, 1.0E-9)
            ' float storage, mean, without Merge (stays sequential)
            Dim noMerge As ReduceDefinition = KernelTestUtil.MeanDefinition()
            noMerge.Merge = Nothing
            Dim fs As ManagedStorage = ManagedStorage.Create(StorageType.FLOAT, n)
            fs.CopyFrom(data)
            Dim mean As Double = ReduceKernels.ReduceToScalar(fs, 0, info, noMerge, Nothing)
            Assert.AreEqual(plain / n, mean, 1.0E-6)
        End Sub

        <TestMethod>
        Public Sub DelegateDefaultsAndExtraArgs()
            Dim d As New ReduceDefinition()
            d.Name = "minimal"
            d.Update = Function(a As Double, b As Double, e As Double()) a + b * e(0)
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {4}, "c"c)
            Assert.AreEqual(20.0, ReduceKernels.ReduceToScalar(x.Storage, x.Offset, x.Info, d, New Double() {2.0}), 0.0)
            ' empty input: PostProcess(StartingValue, 0)
            Dim empty As Long() = ShapeInfoHelper.Create(New Long() {0}, "c"c)
            Assert.IsTrue(Double.IsNaN(ReduceKernels.ReduceToScalar(x.Storage, 0, empty, KernelTestUtil.MeanDefinition(), Nothing)))
            Assert.AreEqual(Double.NegativeInfinity, ReduceKernels.ReduceToScalar(x.Storage, 0, empty, KernelTestUtil.MaxDefinition(), Nothing))
            Dim emptyTads As Long() = ShapeInfoHelper.Create(New Long() {3, 0}, "c"c)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3}, "c"c)
            ReduceKernels.ReduceAlongDimensions(x.Storage, 0, emptyTads, z.Storage, z.Offset, z.Info, New Integer() {1}, KernelTestUtil.SumDefinition(), Nothing)
            z.AssertValues(New Double() {0, 0, 0})
        End Sub

        <TestMethod>
        Public Sub ReductionResultTypes()
            ' LONG result of an INT sum, BOOL result of an "any" reduction
            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.INT, New Long() {2, 3}, "c"c, New Double() {0, 0, 0, 0, 5, 0})
            Dim zl As KernelTestArray = KernelTestArray.Create(StorageType.LONG, New Long() {2}, "c"c)
            ReduceKernels.ReduceAlongDimensions(x.Storage, x.Offset, x.Info, zl.Storage, zl.Offset, zl.Info, New Integer() {1}, KernelTestUtil.SumDefinition(), Nothing)
            zl.AssertValues(New Double() {0, 5})
            Dim any As ReduceDefinition = KernelTestUtil.SumDefinition()
            any.Map = Function(v As Double, e As Double()) If(v <> 0.0, 1.0, 0.0)
            Dim zb As KernelTestArray = KernelTestArray.Create(StorageType.BOOL, New Long() {2}, "c"c)
            ReduceKernels.ReduceAlongDimensions(x.Storage, x.Offset, x.Info, zb.Storage, zb.Offset, zb.Info, New Integer() {1}, any, Nothing)
            zb.AssertValues(New Double() {0, 1})
        End Sub

        <TestMethod>
        Public Sub Reduce3DotAlongEveryDimensionSet()
            Dim xv As Double() = TestValues(24)
            Dim yv As Double() = TestValues(24, 5)
            Dim prod As Double() = New Double(23) {}
            For i As Integer = 0 To 23
                prod(i) = xv(i) * yv(i)
            Next
            For Each dims As Integer() In KernelTestUtil.AllDimensionSets(3)
                Dim exp As Double() = Expected(Shape3, prod, dims, 0.0, Function(a As Double, b As Double) a + b, Function(a As Double, n As Long) a)
                For Each xl In Layouts
                    For Each yl In Layouts
                        Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, Shape3, xl.Order, xv, xl.Offset, xl.Spacing)
                        Dim y As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, Shape3, yl.Order, yv, yl.Offset, yl.Spacing)
                        Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, OuterShape(Shape3, dims, False), "c"c, 2)
                        ReduceKernels.Reduce3AlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, dims, KernelTestUtil.DotDefinition(), Nothing)
                        z.AssertValues(exp, 1.0E-12, "dot dims " & KernelTestUtil.Describe(dims) & " " & xl.ToString() & " " & yl.ToString())
                        z.AssertOutsideUntouched()
                    Next
                Next
            Next
            Dim xs As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, Shape3, "f"c, xv, 3, 2)
            Dim ys As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, Shape3, "c"c, yv)
            Dim total As Double = 0
            For Each p As Double In prod
                total += p
            Next
            Assert.AreEqual(total, ReduceKernels.Reduce3ToScalar(xs.Storage, xs.Offset, xs.Info, ys.Storage, ys.Offset, ys.Info, KernelTestUtil.DotDefinition(), Nothing), 1.0E-9)
            ' different shapes of equal length (neither linear) still pair by logical index
            Dim yFlat As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {4, 6}, "f"c, yv, 1, 2)
            Assert.AreEqual(total, ReduceKernels.Reduce3ToScalar(xs.Storage, xs.Offset, xs.Info, yFlat.Storage, yFlat.Offset, yFlat.Info, KernelTestUtil.DotDefinition(), Nothing), 1.0E-9)
        End Sub

        <TestMethod>
        Public Sub Reduce3AgainstOneTadAndAllPairs()
            ' rows of a 3x4 matrix dotted with one 4-vector
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {3, 4}, "f"c, 1, 2)
            Dim v As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {1, 4}, "c"c, New Double() {1, 0, -1, 2})
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3}, "c"c)
            ReduceKernels.Reduce3AlongDimensions(x.Storage, x.Offset, x.Info, v.Storage, v.Offset, v.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, KernelTestUtil.DotDefinition(), Nothing)
            ' row r = (4r+1, 4r+2, 4r+3, 4r+4): (4r+1) - (4r+3) + 2(4r+4) = 8r + 6
            z.AssertValues(New Double() {6, 14, 22})
            ' all pairs of rows of x (3x4) and y (2x4)
            Dim y As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {2, 4}, "c"c, New Double() {1, 1, 1, 1, 1, -1, 1, -1}, 2)
            Dim zz As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3, 2}, "f"c, 1)
            ReduceKernels.Reduce3All(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, zz.Storage, zz.Offset, zz.Info, New Integer() {1}, KernelTestUtil.DotDefinition(), Nothing)
            ' row sums 10, 26, 42; alternating sums -2 for every row
            zz.AssertValues(New Double() {10, -2, 26, -2, 42, -2})
            zz.AssertOutsideUntouched()
            ' along dimension 0 of 3x4 and 3x2: 4 x 2 column pairs
            Dim y2 As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {3, 2}, "c"c, New Double() {1, 0, 0, 1, 0, 0})
            Dim z2 As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {4, 2}, "c"c)
            ReduceKernels.Reduce3All(x.Storage, x.Offset, x.Info, y2.Storage, y2.Offset, y2.Info, z2.Storage, z2.Offset, z2.Info, New Integer() {0}, KernelTestUtil.DotDefinition(), Nothing)
            z2.AssertValues(New Double() {1, 5, 2, 6, 3, 7, 4, 8})
            KernelTestUtil.WithLowThresholds(
                Sub()
                    Dim zp As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {4, 2}, "c"c)
                    ReduceKernels.Reduce3All(x.Storage, x.Offset, x.Info, y2.Storage, y2.Offset, y2.Info, zp.Storage, zp.Offset, zp.Info, New Integer() {0}, KernelTestUtil.DotDefinition(), Nothing)
                    zp.AssertValues(New Double() {1, 5, 2, 6, 3, 7, 4, 8})
                End Sub)
        End Sub

        <TestMethod>
        Public Sub Validation()
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {3, 4}, "c"c)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {4}, "c"c)
            Assert.ThrowsException(Of ArgumentException)(Sub() ReduceKernels.ReduceAlongDimensions(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, KernelTestUtil.SumDefinition(), Nothing))
            Dim noUpdate As New ReduceDefinition()
            Assert.ThrowsException(Of ArgumentException)(Function() ReduceKernels.ReduceToScalar(x.Storage, x.Offset, x.Info, noUpdate, Nothing))
            Assert.ThrowsException(Of ArgumentNullException)(Function() ReduceKernels.ReduceToScalar(x.Storage, x.Offset, x.Info, Nothing, Nothing))
            Assert.ThrowsException(Of ArgumentException)(Function() ReduceKernels.Reduce3ToScalar(x.Storage, x.Offset, x.Info, x.Storage, x.Offset, x.Info, KernelTestUtil.SumDefinition(), Nothing))
            Dim y As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {4, 3}, "c"c)
            Dim z3 As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3}, "c"c)
            Assert.ThrowsException(Of ArgumentException)(Sub() ReduceKernels.Reduce3AlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z3.Storage, z3.Offset, z3.Info, New Integer() {1}, KernelTestUtil.DotDefinition(), Nothing))
        End Sub

    End Class

End Namespace
