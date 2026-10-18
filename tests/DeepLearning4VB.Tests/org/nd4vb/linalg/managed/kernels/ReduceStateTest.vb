Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>
    ''' Reductions whose definitions keep per-reduction state (<see cref="ReduceDefinition.CreateState"/> and
    ''' <see cref="ReduceDefinition.MergeState"/>): every TAD and every parallel partial gets its own state,
    ''' partial states are merged, and PostProcess sees the merged state.
    ''' </summary>
    <TestClass>
    Public Class ReduceStateTest

        ''' <summary>Counts the state callbacks (thread-safe).</summary>
        Private NotInheritable Class StateCounter
            Public Creates As Integer
            Public Merges As Integer
        End Class

        ''' <summary>
        ''' Cosine similarity the libnd4j way: MapPair adds x*x and y*y to state slots 0 and 1 and returns x*y,
        ''' PostProcess divides the folded sum by sqrt(slot0 * slot1). CreateState copies the extra args and
        ''' zeroes the two slots.
        ''' </summary>
        Private Shared Function CosineDefinition(ByVal counter As StateCounter, Optional ByVal withMergeState As Boolean = True) As ReduceDefinition
            Dim d As New ReduceDefinition()
            d.Name = "cosinesimilarity"
            d.StartingValue = Function(st As Double()) 0.0
            d.MapPair = Function(x As Double, y As Double, st As Double()) As Double
                            st(0) += x * x
                            st(1) += y * y
                            Return x * y
                        End Function
            d.Update = Function(acc As Double, v As Double, st As Double()) acc + v
            d.Merge = Function(a As Double, b As Double, st As Double()) a + b
            d.PostProcess = Function(acc As Double, n As Long, st As Double()) acc / Math.Sqrt(st(0) * st(1))
            d.CreateState = Function(args As Double()) As Double()
                                Interlocked.Increment(counter.Creates)
                                Dim s As Double() = New Double(Math.Max(2, args.Length) - 1) {}
                                Array.Copy(args, s, args.Length)
                                s(0) = 0.0
                                s(1) = 0.0
                                Return s
                            End Function
            If withMergeState Then
                d.MergeState = Sub(total As Double(), partialState As Double())
                                   Interlocked.Increment(counter.Merges)
                                   total(0) += partialState(0)
                                   total(1) += partialState(1)
                               End Sub
            End If
            Return d
        End Function

        ''' <summary>
        ''' A single-input stateful reduction: root mean square where Map counts the elements in state slot 0
        ''' (instead of using n) and PostProcess reads the count.
        ''' </summary>
        Private Shared Function CountingRmsDefinition(ByVal counter As StateCounter) As ReduceDefinition
            Dim d As New ReduceDefinition()
            d.Name = "counting_rms"
            d.Map = Function(v As Double, st As Double()) As Double
                        st(0) += 1.0
                        Return v * v
                    End Function
            d.Update = Function(acc As Double, v As Double, st As Double()) acc + v
            d.Merge = Function(a As Double, b As Double, st As Double()) a + b
            d.PostProcess = Function(acc As Double, n As Long, st As Double()) As Double
                                Assert.AreEqual(CDbl(n), st(0), 0.0, "every element was counted in the reduction's own state")
                                Return Math.Sqrt(acc / st(0))
                            End Function
            d.CreateState = Function(args As Double()) As Double()
                                Interlocked.Increment(counter.Creates)
                                Return New Double() {0.0}
                            End Function
            d.MergeState = Sub(total As Double(), partialState As Double())
                               Interlocked.Increment(counter.Merges)
                               total(0) += partialState(0)
                           End Sub
            Return d
        End Function

        Private Shared Function MakeValues(ByVal n As Integer, ByVal seed As Integer) As Double()
            Dim v As Double() = New Double(n - 1) {}
            For i As Integer = 0 To n - 1
                v(i) = ((CLng(i + seed) * 7919L) Mod 23L) - 11.0 + 0.25
            Next
            Return v
        End Function

        ''' <summary>Direct cosine similarity of the elements of each group (groups by the outer coordinates, row-major).</summary>
        Private Shared Function DirectCosine(ByVal shape As Long(), ByVal xv As Double(), ByVal yv As Double(), ByVal dims As Integer()) As Double()
            Dim outer As Integer() = KernelTestUtil.Outer(shape.Length, dims)
            Dim groups As Integer = 1
            For Each d As Integer In outer
                groups *= CInt(shape(d))
            Next
            Dim dot As Double() = New Double(groups - 1) {}
            Dim xx As Double() = New Double(groups - 1) {}
            Dim yy As Double() = New Double(groups - 1) {}
            For i As Long = 0 To xv.Length - 1
                Dim g As Integer = CInt(KernelTestUtil.LinearIndex(shape, KernelTestUtil.Coordinates(shape, i), outer))
                dot(g) += xv(CInt(i)) * yv(CInt(i))
                xx(g) += xv(CInt(i)) * xv(CInt(i))
                yy(g) += yv(CInt(i)) * yv(CInt(i))
            Next
            Dim r As Double() = New Double(groups - 1) {}
            For g As Integer = 0 To groups - 1
                r(g) = dot(g) / Math.Sqrt(xx(g) * yy(g))
            Next
            Return r
        End Function

        Private Shared Sub WithParallelism(ByVal degree As Integer, ByVal action As Action)
            Dim dop As Integer = KernelOptions.MaxDegreeOfParallelism
            Try
                KernelOptions.MaxDegreeOfParallelism = degree
                action()
            Finally
                KernelOptions.MaxDegreeOfParallelism = dop
            End Try
        End Sub

        <TestMethod>
        Public Sub CosineSimilarityAlongEveryDimensionSet()
            Dim shape As Long() = {2, 3, 4}
            Dim xv As Double() = MakeValues(24, 0)
            Dim yv As Double() = MakeValues(24, 5)
            Dim layouts As (Order As Char, Offset As Long, Spacing As Long)() = {("c"c, 0L, 1L), ("f"c, 2L, 1L), ("c"c, 1L, 2L), ("f"c, 0L, 3L)}
            Dim extra As Double() = {0.0, 0.0, 42.0}
            For Each dims As Integer() In KernelTestUtil.AllDimensionSets(3)
                Dim expected As Double() = DirectCosine(shape, xv, yv, dims)
                For Each xl In layouts
                    For Each yl In layouts
                        Dim counter As New StateCounter()
                        Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, xl.Order, xv, xl.Offset, xl.Spacing)
                        Dim y As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, shape, yl.Order, yv, yl.Offset, yl.Spacing)
                        Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, TadPack.OuterShape(x.Info, dims), "f"c, 1, 2)
                        WithParallelism(1,
                            Sub()
                                ReduceKernels.Reduce3AlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, dims, CosineDefinition(counter), extra)
                            End Sub)
                        Dim label As String = "dims " & KernelTestUtil.Describe(dims) & " " & xl.ToString() & " " & yl.ToString()
                        z.AssertValues(expected, 1.0E-12, label)
                        z.AssertOutsideUntouched()
                        Assert.AreEqual(expected.Length, counter.Creates, label & ": one state per TAD")
                        Assert.AreEqual(0, counter.Merges, label & ": sequential reductions merge nothing")
                    Next
                Next
                If dims.Length = 0 Then
                    Dim counter As New StateCounter()
                    Dim xs As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, shape, "f"c, xv, 3, 2)
                    Dim ys As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, "c"c, yv)
                    Assert.AreEqual(expected(0), ReduceKernels.Reduce3ToScalar(xs.Storage, xs.Offset, xs.Info, ys.Storage, ys.Offset, ys.Info, CosineDefinition(counter), extra), 1.0E-12)
                    Assert.AreEqual(1, counter.Creates)
                End If
            Next
            CollectionAssert.AreEqual(New Double() {0.0, 0.0, 42.0}, extra, "the op's extra arguments are never written")
        End Sub

        <TestMethod>
        Public Sub CosineSimilarityWithParallelPartials()
            KernelTestUtil.WithLowThresholds(
                Sub()
                    WithParallelism(4,
                        Sub()
                            ' three long TADs: reduced one after the other, each in ceil(1000 / 64) = 16 parallel partials
                            Dim shape As Long() = {3, 1000}
                            Dim xv As Double() = MakeValues(3000, 1)
                            Dim yv As Double() = MakeValues(3000, 9)
                            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, "f"c, xv, 1, 2)
                            Dim y As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, "c"c, yv, 2)
                            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3}, "c"c, 1)
                            Dim counter As New StateCounter()
                            ReduceKernels.Reduce3AlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, CosineDefinition(counter), New Double() {0.0, 0.0})
                            z.AssertValues(DirectCosine(shape, xv, yv, New Integer() {1}), 1.0E-12, "long TADs")
                            Assert.AreEqual(3 * 16, counter.Creates, "one state per partial")
                            Assert.AreEqual(3 * 15, counter.Merges, "partials merged in chunk order")

                            ' many short TADs: parallel over the TADs, one state each, nothing to merge
                            Dim shape2 As Long() = {64, 50}
                            Dim xv2 As Double() = MakeValues(3200, 4)
                            Dim yv2 As Double() = MakeValues(3200, 7)
                            Dim x2 As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, shape2, "c"c, xv2)
                            Dim y2 As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape2, "f"c, yv2, 3)
                            Dim z2 As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {64, 1}, "f"c)
                            Dim counter2 As New StateCounter()
                            ReduceKernels.Reduce3AlongDimensions(x2.Storage, x2.Offset, x2.Info, y2.Storage, y2.Offset, y2.Info, z2.Storage, z2.Offset, z2.Info, New Integer() {1}, CosineDefinition(counter2), Nothing)
                            z2.AssertValues(DirectCosine(shape2, xv2, yv2, New Integer() {1}), 1.0E-6, "short TADs")
                            Assert.AreEqual(64, counter2.Creates)
                            Assert.AreEqual(0, counter2.Merges)

                            ' one full reduction of 5000 pairs: ceil(5000 / 64) = 79 partials
                            Dim xv3 As Double() = MakeValues(5000, 2)
                            Dim yv3 As Double() = MakeValues(5000, 3)
                            Dim x3 As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {50, 100}, "f"c, xv3)
                            Dim y3 As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {50, 100}, "c"c, yv3, 1, 2)
                            Dim counter3 As New StateCounter()
                            Dim full As Double = ReduceKernels.Reduce3ToScalar(x3.Storage, x3.Offset, x3.Info, y3.Storage, y3.Offset, y3.Info, CosineDefinition(counter3), Nothing)
                            Assert.AreEqual(DirectCosine(New Long() {5000}, xv3, yv3, Array.Empty(Of Integer)())(0), full, 1.0E-12)
                            Assert.AreEqual(79, counter3.Creates)
                            Assert.AreEqual(78, counter3.Merges)

                            ' without MergeState a stateful reduction is never split
                            Dim counter4 As New StateCounter()
                            Dim unsplit As Double = ReduceKernels.Reduce3ToScalar(x3.Storage, x3.Offset, x3.Info, y3.Storage, y3.Offset, y3.Info, CosineDefinition(counter4, False), Nothing)
                            Assert.AreEqual(full, unsplit, 1.0E-12)
                            Assert.AreEqual(1, counter4.Creates)
                            Assert.AreEqual(0, counter4.Merges)
                        End Sub)
                End Sub)
        End Sub

        <TestMethod>
        Public Sub ParallelStatefulResultsAreRepeatable()
            ' two TADs longer than the parallel threshold: each is split into ceil(40000 / 16384) = 3 partials
            Dim shape As Long() = {2, 40000}
            Dim rng As New Global.System.Random(29)
            Dim xv As Double() = New Double(79999) {}
            Dim yv As Double() = New Double(79999) {}
            For i As Integer = 0 To xv.Length - 1
                xv(i) = rng.NextDouble() - 0.4
                yv(i) = rng.NextDouble() - 0.6
            Next
            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, "c"c, xv)
            Dim y As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, "f"c, yv)
            Dim first As Double() = Nothing
            For run As Integer = 0 To 2
                Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {2}, "c"c)
                Dim counter As New StateCounter()
                ReduceKernels.Reduce3AlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, CosineDefinition(counter), Nothing)
                If KernelOptions.MaxDegreeOfParallelism > 1 Then
                    Assert.AreEqual(6, counter.Creates, "three partials per TAD")
                    Assert.AreEqual(4, counter.Merges)
                End If
                If first Is Nothing Then
                    first = z.Values()
                    z.AssertValues(DirectCosine(shape, xv, yv, New Integer() {1}), 1.0E-9, "default thresholds")
                Else
                    z.AssertValues(first, 0.0, "run " & run)
                End If
            Next
        End Sub

        <TestMethod>
        Public Sub CosineSimilarityOverAllTadPairs()
            ' rows of x (3x4) against rows of y (2x4)
            Dim xv As Double() = {1, 2, 3, 4, -1, 0, 1, 0, 2, 2, 2, 2}
            Dim yv As Double() = {1, 1, 1, 1, 0, 1, 0, -1}
            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {3, 4}, "f"c, xv, 1)
            Dim y As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {2, 4}, "c"c, yv, 2, 2)
            Dim expected As New List(Of Double)()
            For i As Integer = 0 To 2
                For j As Integer = 0 To 1
                    Dim dot As Double = 0, xx As Double = 0, yy As Double = 0
                    For k As Integer = 0 To 3
                        dot += xv(i * 4 + k) * yv(j * 4 + k)
                        xx += xv(i * 4 + k) * xv(i * 4 + k)
                        yy += yv(j * 4 + k) * yv(j * 4 + k)
                    Next
                    expected.Add(dot / Math.Sqrt(xx * yy))
                Next
            Next
            For Each lowThresholds As Boolean In {False, True}
                Dim counter As New StateCounter()
                Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3, 2}, "c"c, 1)
                Dim run As Action = Sub() ReduceKernels.Reduce3All(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, CosineDefinition(counter), Nothing)
                If lowThresholds Then
                    KernelTestUtil.WithLowThresholds(run)
                Else
                    run()
                End If
                z.AssertValues(expected.ToArray(), 1.0E-12, "low thresholds " & lowThresholds)
                Assert.AreEqual(6, counter.Creates)
                z.AssertOutsideUntouched()
            Next
            ' every row of x against the whole of a single vector
            Dim v As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {4}, "c"c, New Double() {1, 1, 1, 1})
            Dim zv As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3}, "c"c)
            Dim counterV As New StateCounter()
            ReduceKernels.Reduce3AlongDimensions(x.Storage, x.Offset, x.Info, v.Storage, v.Offset, v.Info, zv.Storage, zv.Offset, zv.Info, New Integer() {1}, CosineDefinition(counterV), Nothing)
            zv.AssertValues(New Double() {expected(0), expected(2), expected(4)}, 1.0E-12)
            Assert.AreEqual(3, counterV.Creates)
        End Sub

        <TestMethod>
        Public Sub StatefulSingleInputReduction()
            Dim shape As Long() = {3, 5, 7}
            Dim values As Double() = MakeValues(105, 3)
            For Each lowThresholds As Boolean In {False, True}
                For Each dims As Integer() In KernelTestUtil.AllDimensionSets(3)
                    Dim outer As Integer() = KernelTestUtil.Outer(3, dims)
                    Dim groups As Integer = 1
                    For Each d As Integer In outer
                        groups *= CInt(shape(d))
                    Next
                    Dim sums As Double() = New Double(groups - 1) {}
                    Dim counts As Double() = New Double(groups - 1) {}
                    For i As Long = 0 To values.Length - 1
                        Dim g As Integer = CInt(KernelTestUtil.LinearIndex(shape, KernelTestUtil.Coordinates(shape, i), outer))
                        sums(g) += values(CInt(i)) * values(CInt(i))
                        counts(g) += 1
                    Next
                    Dim expected As Double() = New Double(groups - 1) {}
                    For g As Integer = 0 To groups - 1
                        expected(g) = Math.Sqrt(sums(g) / counts(g))
                    Next
                    Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, shape, "f"c, values, 2, 2)
                    Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, TadPack.OuterShape(x.Info, dims), "c"c)
                    Dim counter As New StateCounter()
                    Dim run As Action = Sub() ReduceKernels.ReduceAlongDimensions(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, dims, CountingRmsDefinition(counter), Nothing)
                    If lowThresholds Then
                        KernelTestUtil.WithLowThresholds(Sub() WithParallelism(4, run))
                    Else
                        run()
                    End If
                    z.AssertValues(expected, 1.0E-12, "dims " & KernelTestUtil.Describe(dims) & " low thresholds " & lowThresholds)
                    Assert.IsTrue(counter.Creates >= groups, "at least one state per TAD")
                    Assert.AreEqual(counter.Creates - groups, counter.Merges, "every extra state is a partial that was merged")
                Next
            Next
            ' the full reduction in parallel partials: 105 elements in chunks of 64 = 2 partials
            KernelTestUtil.WithLowThresholds(
                Sub()
                    WithParallelism(4,
                        Sub()
                            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, shape, "c"c, values)
                            Dim counter As New StateCounter()
                            Dim total As Double = 0
                            For Each v As Double In values
                                total += v * v
                            Next
                            Assert.AreEqual(Math.Sqrt(total / 105.0), ReduceKernels.ReduceToScalar(x.Storage, x.Offset, x.Info, CountingRmsDefinition(counter), Nothing), 1.0E-12)
                            Assert.AreEqual(2, counter.Creates)
                            Assert.AreEqual(1, counter.Merges)
                        End Sub)
                End Sub)
        End Sub

        <TestMethod>
        Public Sub StatelessDefinitionsShareTheExtraArguments()
            Dim extra As Double() = {3.0}
            Dim foreign As Integer = 0
            Dim d As New ReduceDefinition()
            d.Name = "scaled_sum"
            d.Map = Function(v As Double, e As Double()) As Double
                        If e IsNot extra Then
                            Interlocked.Increment(foreign)
                        End If
                        Return v * e(0)
                    End Function
            d.Update = Function(a As Double, b As Double, e As Double()) a + b
            d.Merge = Function(a As Double, b As Double, e As Double()) a + b
            d.PostProcess = Function(a As Double, n As Long, e As Double()) As Double
                                If e IsNot extra Then
                                    Interlocked.Increment(foreign)
                                End If
                                Return a
                            End Function
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {4, 50}, "f"c)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {4}, "c"c)
            KernelTestUtil.WithLowThresholds(
                Sub()
                    ReduceKernels.ReduceAlongDimensions(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, d, extra)
                    Assert.AreEqual(3.0 * 20100.0, ReduceKernels.ReduceToScalar(x.Storage, x.Offset, x.Info, d, extra), 0.0)
                End Sub)
            ' row r holds the logical values 50r + 1 ... 50r + 50 (the f layout does not matter): sum = 2500r + 1275
            For r As Integer = 0 To 3
                Assert.AreEqual(3.0 * (2500.0 * r + 1275.0), z.GetValue(r), 0.0)
            Next
            Assert.AreEqual(0, foreign, "every delegate saw the op's own extra-argument array")
        End Sub

        <TestMethod>
        Public Sub EmptyReductionsGetAFreshState()
            Dim counter As New StateCounter()
            Dim def As ReduceDefinition = CosineDefinition(counter)
            Dim x As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3, 2}, "c"c)
            Dim emptyTads As Long() = ShapeInfoHelper.Create(New Long() {3, 0}, "c"c)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3}, "c"c)
            ReduceKernels.Reduce3AlongDimensions(x.Storage, 0, emptyTads, x.Storage, 0, emptyTads, z.Storage, z.Offset, z.Info, New Integer() {1}, def, Nothing)
            For i As Integer = 0 To 2
                Assert.IsTrue(Double.IsNaN(z.GetValue(i)), "0 / sqrt(0 * 0)")
            Next
            Assert.AreEqual(3, counter.Creates)
            Dim empty As Long() = ShapeInfoHelper.Create(New Long() {0}, "c"c)
            Assert.IsTrue(Double.IsNaN(ReduceKernels.Reduce3ToScalar(x.Storage, 0, empty, x.Storage, 0, empty, def, Nothing)))
            Assert.AreEqual(4, counter.Creates)
            ' a CreateState that returns Nothing is reported
            Dim broken As ReduceDefinition = CosineDefinition(New StateCounter())
            broken.CreateState = Function(args As Double()) Nothing
            Assert.ThrowsException(Of InvalidOperationException)(Function() ReduceKernels.Reduce3ToScalar(x.Storage, x.Offset, x.Info, x.Storage, x.Offset, x.Info, broken, Nothing))
        End Sub

    End Class

End Namespace
