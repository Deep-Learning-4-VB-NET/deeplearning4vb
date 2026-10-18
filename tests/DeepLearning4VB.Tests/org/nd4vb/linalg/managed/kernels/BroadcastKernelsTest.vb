Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    <TestClass>
    Public Class BroadcastKernelsTest

        Private Shared ReadOnly Subtract As PairwiseFunction = Function(a As Double, b As Double, e As Double()) a - b
        Private Shared ReadOnly Add As PairwiseFunction = Function(a As Double, b As Double, e As Double()) a + b

        <TestMethod>
        Public Sub RowAndColumnBroadcastOnAMatrix()
            For Each order As Char In {"c"c, "f"c}
                Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, New Long() {3, 4}, order, 1, 2)
                Dim row As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {4}, "c"c, New Double() {10, 20, 30, 40}, 2, 3)
                Dim z As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, New Long() {3, 4}, "c"c, 0, 2)
                BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, row.Storage, row.Offset, row.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, Add, Nothing)
                z.AssertValues(New Double() {11, 22, 33, 44, 15, 26, 37, 48, 19, 30, 41, 52}, 0.0, "row " & order)
                z.AssertOutsideUntouched()
                ' a column vector given as [3, 1] along dimension 0 (negative dimension -2)
                Dim col As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {3, 1}, "f"c, New Double() {100, 200, 300})
                Dim z2 As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3, 4}, "f"c, 3)
                BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, col.Storage, col.Offset, col.Info, z2.Storage, z2.Offset, z2.Info, New Integer() {-2}, Add, Nothing)
                z2.AssertValues(New Double() {101, 102, 103, 104, 205, 206, 207, 208, 309, 310, 311, 312}, 0.0, "column " & order)
                z2.AssertOutsideUntouched()
                ' in place on x
                BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, row.Storage, row.Offset, row.Info, x.Storage, x.Offset, x.Info, New Integer() {1}, Subtract, Nothing)
                x.AssertValues(New Double() {-9, -18, -27, -36, -5, -14, -23, -32, -1, -10, -19, -28}, 0.0, "in place " & order)
                x.AssertOutsideUntouched()
            Next
        End Sub

        <TestMethod>
        Public Sub Rank3EveryDimensionSetAndLayout()
            Dim shape As Long() = {2, 3, 4}
            Dim layouts As (Order As Char, Offset As Long, Spacing As Long)() = {("c"c, 0L, 1L), ("f"c, 2L, 1L), ("c"c, 1L, 2L)}
            For Each dims As Integer() In KernelTestUtil.AllDimensionSets(3)
                If dims.Length = 0 Then
                    Continue For
                End If
                Dim tadShape As Long() = New Long(dims.Length - 1) {}
                For i As Integer = 0 To dims.Length - 1
                    tadShape(i) = shape(dims(i))
                Next
                For Each xl In layouts
                    For Each yl In layouts
                        Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, shape, xl.Order, xl.Offset, xl.Spacing)
                        Dim y As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, tadShape, yl.Order, yl.Offset, yl.Spacing, 1000.0)
                        Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, shape, "f"c, 1, 2)
                        BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, dims, Subtract, Nothing)
                        For i As Long = 0 To 23
                            Dim coords As Long() = KernelTestUtil.Coordinates(shape, i)
                            Dim j As Long = KernelTestUtil.LinearIndex(shape, coords, dims)
                            Dim expected As Double = (i + 1) - 1000.0 * (j + 1)
                            Assert.AreEqual(expected, z.GetValue(i), 0.0, "dims " & KernelTestUtil.Describe(dims) & " x " & xl.ToString() & " y " & yl.ToString() & " element " & i)
                        Next
                        z.AssertOutsideUntouched()
                    Next
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub YOfMatchingLengthButOtherShapeIsReadInLogicalOrder()
            ' TAD of [2,3,4] along (0, 2) is [2, 4]; y is an f-ordered [4, 2] with gaps (its shape does not map)
            Dim shape As Long() = {2, 3, 4}
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, shape, "c"c)
            Dim y As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, New Long() {4, 2}, "f"c, 1, 2, 100.0)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, shape, "c"c)
            BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, New Integer() {0, 2}, Add, Nothing)
            For i As Long = 0 To 23
                Dim coords As Long() = KernelTestUtil.Coordinates(shape, i)
                Dim j As Long = coords(0) * 4 + coords(2)
                Assert.AreEqual((i + 1) + 100.0 * (j + 1), z.GetValue(i), 0.0, "element " & i)
            Next
            y.AssertOutsideUntouched()
        End Sub

        <TestMethod>
        Public Sub BroadcastIntoBooleanOutput()
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.INT, New Long() {2, 3}, "c"c)
            Dim y As KernelTestArray = KernelTestArray.FromValues(StorageType.INT, New Long() {3}, "c"c, New Double() {1, 5, 4})
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.BOOL, New Long() {2, 3}, "c"c)
            BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, Function(a As Double, b As Double, e As Double()) If(a > b, 1.0, 0.0), Nothing)
            z.AssertValues(New Double() {0, 0, 0, 1, 0, 1})
        End Sub

        <TestMethod>
        Public Sub AlongDimensionsValidation()
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, New Long() {3, 4}, "c"c)
            Dim y As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, New Long() {3}, "c"c)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, New Long() {3, 4}, "c"c)
            Assert.ThrowsException(Of ArgumentException)(Sub() BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, Add, Nothing))
            Dim wrongZ As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, New Long() {4, 3}, "c"c)
            Assert.ThrowsException(Of ArgumentException)(Sub() BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, wrongZ.Storage, wrongZ.Offset, wrongZ.Info, New Integer() {0}, Add, Nothing))
            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Sub() BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, New Integer() {2}, Add, Nothing))
            ' no dimensions: y covers all of x (plain pairwise)
            Dim full As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, New Long() {12}, "c"c)
            BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, full.Storage, full.Offset, full.Info, z.Storage, z.Offset, z.Info, New Integer() {}, Add, Nothing)
            z.AssertValues(New Double() {2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24})
        End Sub

        <TestMethod>
        Public Sub NumPyBroadcastShapes()
            CollectionAssert.AreEqual(New Long() {3, 4}, BroadcastKernels.BroadcastShape(New Long() {3, 1}, New Long() {1, 4}))
            CollectionAssert.AreEqual(New Long() {2, 3, 4}, BroadcastKernels.BroadcastShape(New Long() {2, 3, 4}, New Long() {4}))
            CollectionAssert.AreEqual(New Long() {5, 4, 3}, BroadcastKernels.BroadcastShape(New Long() {5, 1, 3}, New Long() {4, 1}))
            CollectionAssert.AreEqual(New Long() {0}, BroadcastKernels.BroadcastShape(New Long() {0}, New Long() {1}))
            CollectionAssert.AreEqual(New Long() {2, 3}, BroadcastKernels.BroadcastShape(New Long() {}, New Long() {2, 3}))
            Assert.ThrowsException(Of ArgumentException)(Function() BroadcastKernels.BroadcastShape(New Long() {3}, New Long() {4}))
            Assert.IsTrue(BroadcastKernels.CanBroadcast(New Long() {8, 1, 6, 1}, New Long() {7, 1, 5}))
            Assert.IsFalse(BroadcastKernels.CanBroadcast(New Long() {2, 1}, New Long() {8, 4, 3}))
            CollectionAssert.AreEqual(New Long() {0, 0, 1}, BroadcastKernels.BroadcastStrides(ShapeInfoHelper.Create(New Long() {1, 4}, "c"c), New Long() {2, 3, 4}))
            Assert.ThrowsException(Of ArgumentException)(Function() BroadcastKernels.BroadcastStrides(ShapeInfoHelper.Create(New Long() {3}, "c"c), New Long() {2, 4}))
        End Sub

        <TestMethod>
        Public Sub NumPyBroadcastKernel()
            ' [3,1] - [1,4] -> [3,4]
            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {3, 1}, "c"c, New Double() {10, 20, 30}, 1, 2)
            Dim y As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {1, 4}, "f"c, New Double() {1, 2, 3, 4}, 2, 3)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3, 4}, "f"c, 1)
            BroadcastKernels.BroadcastPairwise(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, Subtract, Nothing)
            z.AssertValues(New Double() {9, 8, 7, 6, 19, 18, 17, 16, 29, 28, 27, 26})
            z.AssertOutsideUntouched()
            ' rank-3 x with a vector y, and the reverse
            Dim shape As Long() = {2, 3, 4}
            Dim x3 As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, shape, "f"c, 3, 2)
            Dim v As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {4}, "c"c, New Double() {100, 200, 300, 400})
            Dim z3 As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, shape, "c"c)
            BroadcastKernels.BroadcastPairwise(v.Storage, v.Offset, v.Info, x3.Storage, x3.Offset, x3.Info, z3.Storage, z3.Offset, z3.Info, Subtract, Nothing)
            For i As Long = 0 To 23
                Assert.AreEqual(100.0 * ((i Mod 4) + 1) - (i + 1), z3.GetValue(i), 0.0)
            Next
            ' in place: x3 += [3,1] column
            Dim c As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {3, 1}, "c"c, New Double() {1000, 2000, 3000})
            BroadcastKernels.BroadcastPairwise(x3.Storage, x3.Offset, x3.Info, c.Storage, c.Offset, c.Info, x3.Storage, x3.Offset, x3.Info, Add, Nothing)
            For i As Long = 0 To 23
                Dim coords As Long() = KernelTestUtil.Coordinates(shape, i)
                Assert.AreEqual((i + 1) + 1000.0 * (coords(1) + 1), x3.GetValue(i), 0.0)
            Next
            x3.AssertOutsideUntouched()
            ' y does not broadcast to z
            Dim bad As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, New Long() {3, 5}, "c"c)
            Assert.ThrowsException(Of ArgumentException)(Sub() BroadcastKernels.BroadcastPairwise(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, bad.Storage, bad.Offset, bad.Info, Subtract, Nothing))
        End Sub

        <TestMethod>
        Public Sub NumPyBroadcastCopy()
            Dim x As KernelTestArray = KernelTestArray.FromValues(StorageType.INT, New Long() {1, 3}, "c"c, New Double() {1, 2, 3})
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, New Long() {2, 3}, "f"c, 1, 2)
            BroadcastKernels.BroadcastCopy(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info)
            z.AssertValues(New Double() {1, 2, 3, 1, 2, 3})
            z.AssertOutsideUntouched()
            Dim s As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {}, "c"c, New Double() {7.5})
            Dim z2 As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {2, 2}, "c"c)
            BroadcastKernels.BroadcastCopy(s.Storage, s.Offset, s.Info, z2.Storage, z2.Offset, z2.Info)
            z2.AssertValues(New Double() {7.5, 7.5, 7.5, 7.5})
        End Sub

        <TestMethod>
        Public Sub LargeBroadcastRunsInParallel()
            Dim shape As Long() = {123, 457}
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, shape, "f"c)
            Dim y As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {457}, "c"c)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, shape, "c"c)
            BroadcastKernels.BroadcastAlongDimensions(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, New Integer() {1}, Subtract, Nothing)
            For i As Long = 0 To z.Length - 1 Step 13
                Assert.AreEqual((i + 1) - ((i Mod 457) + 1), z.GetValue(i), 0.0)
            Next
        End Sub

    End Class

End Namespace
