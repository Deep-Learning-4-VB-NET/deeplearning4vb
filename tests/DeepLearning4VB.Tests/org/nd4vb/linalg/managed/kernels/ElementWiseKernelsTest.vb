Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    <TestClass>
    Public Class ElementWiseKernelsTest

        Private Shared ReadOnly TwicePlusOne As TransformFunction = Function(v As Double, e As Double()) 2.0 * v + 1.0
        Private Shared ReadOnly Subtract As PairwiseFunction = Function(a As Double, b As Double, e As Double()) a - b
        Private Shared ReadOnly WeightedSum As PairwiseFunction = Function(a As Double, b As Double, e As Double()) a + e(0) * b

        Private Shared ReadOnly Layouts As (Order As Char, Offset As Long, Spacing As Long)() = {
            ("c"c, 0L, 1L), ("f"c, 0L, 1L), ("c"c, 3L, 2L), ("f"c, 5L, 3L)}

        <TestMethod>
        Public Sub TransformOverEveryLayoutCombination()
            Dim shape As Long() = {3, 4, 2}
            For Each t As StorageType In {StorageType.FLOAT, StorageType.DOUBLE, StorageType.INT}
                For Each xl In Layouts
                    For Each zl In Layouts
                        Dim x As KernelTestArray = KernelTestArray.Sequence(t, shape, xl.Order, xl.Offset, xl.Spacing)
                        Dim z As KernelTestArray = KernelTestArray.Create(t, shape, zl.Order, zl.Offset, zl.Spacing)
                        ElementWiseKernels.Transform(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, TwicePlusOne, Nothing)
                        Dim expected As Double() = New Double(23) {}
                        For i As Integer = 0 To 23
                            expected(i) = 2.0 * (i + 1) + 1.0
                        Next
                        Dim label As String = t.ToString() & " x " & xl.ToString() & " z " & zl.ToString()
                        z.AssertValues(expected, 0.0, label)
                        z.AssertOutsideUntouched()
                        x.AssertOutsideUntouched()
                    Next
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub TransformInPlaceAndMixedTypes()
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, New Long() {2, 5}, "f"c, 1, 2)
            ElementWiseKernels.Transform(x.Storage, x.Offset, x.Info, x.Storage, x.Offset, x.Info, TwicePlusOne, Nothing)
            x.AssertValues(New Double() {3, 5, 7, 9, 11, 13, 15, 17, 19, 21})
            x.AssertOutsideUntouched()

            ' DOUBLE -> INT truncates like Java's (int) cast; FLOAT -> BOOL stores f() <> 0
            Dim d As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {4}, "c"c, New Double() {0.4, -1.7, 2.0E+10, Double.NaN})
            Dim i As KernelTestArray = KernelTestArray.Create(StorageType.INT, New Long() {4}, "c"c, 2)
            ElementWiseKernels.Transform(d.Storage, d.Offset, d.Info, i.Storage, i.Offset, i.Info, Function(v As Double, e As Double()) v, Nothing)
            i.AssertValues(New Double() {0, -1, Integer.MaxValue, 0})
            Dim b As KernelTestArray = KernelTestArray.Create(StorageType.BOOL, New Long() {4}, "c"c)
            ElementWiseKernels.Transform(d.Storage, d.Offset, d.Info, b.Storage, b.Offset, b.Info, Function(v As Double, e As Double()) Math.Round(v), Nothing)
            b.AssertValues(New Double() {0, 1, 1, 1})
            Dim h As KernelTestArray = KernelTestArray.Sequence(StorageType.HALF, New Long() {2, 2}, "c"c)
            Dim hz As KernelTestArray = KernelTestArray.Create(StorageType.HALF, New Long() {2, 2}, "f"c, 1)
            ElementWiseKernels.Transform(h.Storage, h.Offset, h.Info, hz.Storage, hz.Offset, hz.Info, Function(v As Double, e As Double()) v / 4.0, Nothing)
            hz.AssertValues(New Double() {0.25, 0.5, 0.75, 1.0})
            Dim u As KernelTestArray = KernelTestArray.FromValues(StorageType.UBYTE, New Long() {3}, "c"c, New Double() {0, 1, 2})
            ElementWiseKernels.Transform(u.Storage, u.Offset, u.Info, u.Storage, u.Offset, u.Info, Function(v As Double, e As Double()) v - 1.0, Nothing)
            u.AssertValues(New Double() {255, 0, 1})
        End Sub

        <TestMethod>
        Public Sub ScalarUsesTheScalarAndExtraArgs()
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {2, 3}, "f"c, 2, 2)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {2, 3}, "c"c, 1)
            ElementWiseKernels.Scalar(x.Storage, x.Offset, x.Info, 10.0, z.Storage, z.Offset, z.Info, WeightedSum, New Double() {0.5})
            z.AssertValues(New Double() {6, 7, 8, 9, 10, 11})
            z.AssertOutsideUntouched()
            Dim fz As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, New Long() {2, 3}, "c"c)
            ElementWiseKernels.Scalar(x.Storage, x.Offset, x.Info, 1.0, fz.Storage, fz.Offset, fz.Info, Subtract, Nothing)
            fz.AssertValues(New Double() {0, 1, 2, 3, 4, 5})
            Dim xf As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, New Long() {5}, "c"c, 0, 3)
            ElementWiseKernels.Scalar(xf.Storage, xf.Offset, xf.Info, 2.0, xf.Storage, xf.Offset, xf.Info, Function(a As Double, s As Double, e As Double()) a * s, Nothing)
            xf.AssertValues(New Double() {2, 4, 6, 8, 10})
            xf.AssertOutsideUntouched()
        End Sub

        <TestMethod>
        Public Sub PairwiseOverMixedLayoutsAndTypes()
            Dim shape As Long() = {2, 3, 2}
            For Each t As StorageType In {StorageType.FLOAT, StorageType.DOUBLE, StorageType.LONG}
                For Each xl In Layouts
                    For Each yl In Layouts
                        Dim x As KernelTestArray = KernelTestArray.Sequence(t, shape, xl.Order, xl.Offset, xl.Spacing, 10.0)
                        Dim y As KernelTestArray = KernelTestArray.Sequence(t, shape, yl.Order, yl.Offset, yl.Spacing)
                        Dim z As KernelTestArray = KernelTestArray.Create(t, shape, "f"c, 4, 2)
                        ElementWiseKernels.Pairwise(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, Subtract, Nothing)
                        Dim expected As Double() = New Double(11) {}
                        For i As Integer = 0 To 11
                            expected(i) = 9.0 * (i + 1)
                        Next
                        z.AssertValues(expected, 0.0, t.ToString() & " " & xl.ToString() & " " & yl.ToString())
                        z.AssertOutsideUntouched()
                    Next
                Next
            Next
            ' mixed storage types: INT - FLOAT into DOUBLE, and a comparison into BOOL
            Dim xi As KernelTestArray = KernelTestArray.Sequence(StorageType.INT, New Long() {4}, "c"c)
            Dim yf As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {4}, "c"c, New Double() {0.5, 3.0, 2.5, 5.0})
            Dim zd As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {4}, "c"c)
            ElementWiseKernels.Pairwise(xi.Storage, xi.Offset, xi.Info, yf.Storage, yf.Offset, yf.Info, zd.Storage, zd.Offset, zd.Info, Subtract, Nothing)
            zd.AssertValues(New Double() {0.5, -1.0, 0.5, -1.0})
            Dim zb As KernelTestArray = KernelTestArray.Create(StorageType.BOOL, New Long() {4}, "c"c)
            ElementWiseKernels.Pairwise(xi.Storage, xi.Offset, xi.Info, yf.Storage, yf.Offset, yf.Info, zb.Storage, zb.Offset, zb.Info,
                                        Function(a As Double, c As Double, e As Double()) If(a > c, 1.0, 0.0), Nothing)
            zb.AssertValues(New Double() {1, 0, 1, 0})
        End Sub

        <TestMethod>
        Public Sub DifferentShapesOfEqualLengthMatchByLogicalIndex()
            ' x is a c-ordered 2x3, z an f-ordered 3x2 with gaps (neither shape nor linear stride agree)
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {2, 3}, "c"c)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {3, 2}, "f"c, 1, 2)
            ElementWiseKernels.Transform(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, TwicePlusOne, Nothing)
            z.AssertValues(New Double() {3, 5, 7, 9, 11, 13})
            z.AssertOutsideUntouched()
            ' a non-linear x view (2x2 block of a 4x4) paired with a vector y and a 4-element z
            Dim block As KernelTestArray = KernelTestArray.Strided(StorageType.FLOAT, New Long() {2, 2}, New Long() {4, 1}, "c"c, 5)
            block.SetAll(New Double() {1, 2, 3, 4})
            Dim y As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, New Long() {4}, "c"c, 0, 1, 100.0)
            Dim z2 As KernelTestArray = KernelTestArray.Strided(StorageType.FLOAT, New Long() {1, 4}, New Long() {8, 2}, "c"c, 1)
            ElementWiseKernels.Pairwise(y.Storage, y.Offset, y.Info, block.Storage, block.Offset, block.Info, z2.Storage, z2.Offset, z2.Info, Subtract, Nothing)
            z2.AssertValues(New Double() {99, 198, 297, 396})
            z2.AssertOutsideUntouched()
            ' copy between different shapes, neither linear
            Dim zc As KernelTestArray = KernelTestArray.Strided(StorageType.DOUBLE, New Long() {4, 1}, New Long() {1, 1}, "f"c, 0)
            ElementWiseKernels.Copy(block.Storage, block.Offset, block.Info, zc.Storage, zc.Offset, zc.Info)
            zc.AssertValues(New Double() {1, 2, 3, 4})
            Dim scalar As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {}, "c"c, 2)
            Dim one As KernelTestArray = KernelTestArray.FromValues(StorageType.FLOAT, New Long() {1, 1}, "c"c, New Double() {4.5})
            ElementWiseKernels.Transform(one.Storage, one.Offset, one.Info, scalar.Storage, scalar.Offset, scalar.Info, TwicePlusOne, Nothing)
            scalar.AssertValues(New Double() {10.0})
        End Sub

        <TestMethod>
        Public Sub ParallelChunksGiveTheSameResult()
            Dim shape As Long() = {37, 41, 3}
            KernelTestUtil.WithLowThresholds(
                Sub()
                    For Each xl In Layouts
                        Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, shape, xl.Order, xl.Offset, xl.Spacing)
                        Dim y As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, shape, "f"c, 0, 1)
                        Dim z As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, shape, "c"c, 2, 2)
                        ElementWiseKernels.Pairwise(x.Storage, x.Offset, x.Info, y.Storage, y.Offset, y.Info, z.Storage, z.Offset, z.Info, Subtract, Nothing)
                        For i As Integer = 0 To CInt(z.Length) - 1
                            Assert.AreEqual(0.0, z.GetValue(i), 0.0)
                        Next
                        z.AssertOutsideUntouched()
                        Dim g As KernelTestArray = KernelTestArray.Create(StorageType.INT, shape, "f"c, 1)
                        ElementWiseKernels.Transform(x.Storage, x.Offset, x.Info, g.Storage, g.Offset, g.Info, TwicePlusOne, Nothing)
                        For i As Integer = 0 To CInt(g.Length) - 1
                            Assert.AreEqual(2.0 * (i + 1) + 1.0, g.GetValue(i), 0.0)
                        Next
                    Next
                End Sub)
            ' and above the default threshold
            Dim big As KernelTestArray = KernelTestArray.Sequence(StorageType.DOUBLE, New Long() {300, 301}, "f"c)
            Dim bz As KernelTestArray = KernelTestArray.Create(StorageType.DOUBLE, New Long() {300, 301}, "c"c)
            ElementWiseKernels.Transform(big.Storage, big.Offset, big.Info, bz.Storage, bz.Offset, bz.Info, TwicePlusOne, Nothing)
            For i As Integer = 0 To CInt(bz.Length) - 1 Step 97
                Assert.AreEqual(2.0 * (i + 1) + 1.0, bz.GetValue(i), 0.0)
            Next
        End Sub

        <TestMethod>
        Public Sub CopyAndFill()
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.LONG, New Long() {3, 4}, "c"c, 2, 2)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.LONG, New Long() {3, 4}, "f"c, 1)
            ElementWiseKernels.Copy(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info)
            z.AssertValues(x.Values())
            z.AssertOutsideUntouched()
            Dim zh As KernelTestArray = KernelTestArray.Create(StorageType.HALF, New Long() {3, 4}, "c"c, 0, 3)
            ElementWiseKernels.Copy(x.Storage, x.Offset, x.Info, zh.Storage, zh.Offset, zh.Info)
            zh.AssertValues(x.Values())
            Dim strings As ManagedStorage = ManagedStorage.FromArray(New String() {"a", "b", "c", "d"})
            Dim stringsOut As ManagedStorage = ManagedStorage.Create(StorageType.UTF8, 4)
            ElementWiseKernels.Copy(strings, 0, ShapeInfoHelper.Create(New Long() {2, 2}, "c"c), stringsOut, 0, ShapeInfoHelper.Create(New Long() {2, 2}, "f"c))
            CollectionAssert.AreEqual(New String() {"a", "c", "b", "d"}, stringsOut.Strings)
            Dim f As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, New Long() {2, 3}, "f"c, 1, 2)
            ElementWiseKernels.Fill(f.Storage, f.Offset, f.Info, 2.5)
            f.AssertValues(New Double() {2.5, 2.5, 2.5, 2.5, 2.5, 2.5})
            f.AssertOutsideUntouched()
            Dim fc As KernelTestArray = KernelTestArray.Create(StorageType.INT, New Long() {2, 3}, "c"c, 1)
            ElementWiseKernels.Fill(fc.Storage, fc.Offset, fc.Info, -3.7)
            fc.AssertValues(New Double() {-3, -3, -3, -3, -3, -3})
            fc.AssertOutsideUntouched()
        End Sub

        <TestMethod>
        Public Sub IntegerOpsWrapAroundAndCastsSaturate()
            Dim add As PairwiseFunction = Function(a As Double, b As Double, e As Double()) a + b
            ' BYTE + BYTE -> BYTE wraps like Java's (byte) arithmetic
            Dim bx As KernelTestArray = KernelTestArray.FromValues(StorageType.BYTE, New Long() {3}, "c"c, New Double() {127, -128, 100}, 1, 2)
            Dim by As KernelTestArray = KernelTestArray.FromValues(StorageType.BYTE, New Long() {3}, "c"c, New Double() {1, -1, 27})
            Dim bz As KernelTestArray = KernelTestArray.Create(StorageType.BYTE, New Long() {3}, "f"c, 2)
            ElementWiseKernels.Pairwise(bx.Storage, bx.Offset, bx.Info, by.Storage, by.Offset, by.Info, bz.Storage, bz.Offset, bz.Info, add, Nothing)
            bz.AssertValues(New Double() {-128, 127, 127})
            bz.AssertOutsideUntouched()
            ' UBYTE + UBYTE -> UBYTE: 250 + 10 = 4 (mod 256)
            Dim ux As KernelTestArray = KernelTestArray.FromValues(StorageType.UBYTE, New Long() {2}, "c"c, New Double() {250, 255})
            Dim uz As KernelTestArray = KernelTestArray.Create(StorageType.UBYTE, New Long() {2}, "c"c)
            ElementWiseKernels.Scalar(ux.Storage, ux.Offset, ux.Info, 10.0, uz.Storage, uz.Offset, uz.Info, add, Nothing)
            uz.AssertValues(New Double() {4, 9})
            ' INT: MaxValue + 1 = MinValue; -MinValue = MinValue; SHORT 20000 * 2 = -25536
            Dim ix As KernelTestArray = KernelTestArray.FromValues(StorageType.INT, New Long() {2, 2}, "f"c, New Double() {Integer.MaxValue, Integer.MinValue, 5, -7}, 1)
            Dim iz As KernelTestArray = KernelTestArray.Create(StorageType.INT, New Long() {2, 2}, "c"c)
            ElementWiseKernels.Scalar(ix.Storage, ix.Offset, ix.Info, 1.0, iz.Storage, iz.Offset, iz.Info, add, Nothing)
            iz.AssertValues(New Double() {Integer.MinValue, Integer.MinValue + 1.0, 6, -6})
            ElementWiseKernels.Transform(ix.Storage, ix.Offset, ix.Info, iz.Storage, iz.Offset, iz.Info, Function(v As Double, e As Double()) -v, Nothing)
            iz.AssertValues(New Double() {-Integer.MaxValue, Integer.MinValue, -5, 7})
            Dim sx As KernelTestArray = KernelTestArray.FromValues(StorageType.SHORT, New Long() {2}, "c"c, New Double() {20000, -3})
            Dim sz As KernelTestArray = KernelTestArray.Create(StorageType.SHORT, New Long() {2}, "c"c)
            ElementWiseKernels.Scalar(sx.Storage, sx.Offset, sx.Info, 2.0, sz.Storage, sz.Offset, sz.Info, Function(a As Double, b As Double, e As Double()) a * b, Nothing)
            sz.AssertValues(New Double() {-25536, -6})
            ' INT result into a LONG target keeps the exact value; into UINT32 it wraps modulo 2^32
            Dim lz As KernelTestArray = KernelTestArray.Create(StorageType.LONG, New Long() {2, 2}, "c"c)
            ElementWiseKernels.Scalar(ix.Storage, ix.Offset, ix.Info, 1.0, lz.Storage, lz.Offset, lz.Info, add, Nothing)
            lz.AssertValues(New Double() {2147483648.0, -2147483647.0, 6, -6})
            Dim u32 As KernelTestArray = KernelTestArray.Create(StorageType.UINT32, New Long() {2, 2}, "c"c)
            ElementWiseKernels.Scalar(ix.Storage, ix.Offset, ix.Info, -6.0, u32.Storage, u32.Offset, u32.Info, add, Nothing)
            u32.AssertValues(New Double() {2147483641.0, 2147483642.0, 4294967295.0, 4294967283.0})
            ' with a floating-point operand the store is a cast and saturates, as Java's (int) cast does
            Dim fx As KernelTestArray = KernelTestArray.FromValues(StorageType.DOUBLE, New Long() {2}, "c"c, New Double() {2147483647.0, -3.0E+10})
            Dim one As KernelTestArray = KernelTestArray.FromValues(StorageType.INT, New Long() {2}, "c"c, New Double() {1, -1})
            Dim mz As KernelTestArray = KernelTestArray.Create(StorageType.INT, New Long() {2}, "c"c)
            ElementWiseKernels.Pairwise(fx.Storage, fx.Offset, fx.Info, one.Storage, one.Offset, one.Info, mz.Storage, mz.Offset, mz.Info, add, Nothing)
            mz.AssertValues(New Double() {Integer.MaxValue, Integer.MinValue})
            ' integer division by zero gives an infinite double, which saturates
            Dim dz As KernelTestArray = KernelTestArray.Create(StorageType.INT, New Long() {2}, "c"c)
            ElementWiseKernels.Scalar(one.Storage, one.Offset, one.Info, 0.0, dz.Storage, dz.Offset, dz.Info, Function(a As Double, b As Double, e As Double()) a / b, Nothing)
            dz.AssertValues(New Double() {Integer.MaxValue, Integer.MinValue})
            ' broadcasting uses the same rule
            Dim row As KernelTestArray = KernelTestArray.FromValues(StorageType.BYTE, New Long() {2}, "c"c, New Double() {1, 100})
            Dim mat As KernelTestArray = KernelTestArray.FromValues(StorageType.BYTE, New Long() {2, 2}, "c"c, New Double() {127, 27, -128, 28})
            Dim bc As KernelTestArray = KernelTestArray.Create(StorageType.BYTE, New Long() {2, 2}, "c"c)
            BroadcastKernels.BroadcastAlongDimensions(mat.Storage, mat.Offset, mat.Info, row.Storage, row.Offset, row.Info, bc.Storage, bc.Offset, bc.Info, New Integer() {1}, add, Nothing)
            bc.AssertValues(New Double() {-128, 127, -127, -128})
        End Sub

        <TestMethod>
        Public Sub CopyConvertsEveryTypePairOnStridedViews()
            Dim edge As Double() = Global.DeepLearning4VB.Tests.org.nd4vb.linalg.managed.storage.ManagedStorageTest.EdgeValues
            Dim values As Double() = New Double(edge.Length) {}
            Array.Copy(edge, values, edge.Length)
            values(edge.Length) = 42.0
            Dim n As Integer = values.Length
            Dim shape As Long() = {2, n \ 2}
            Dim otherShape As Long() = {n \ 2, 2}
            For Each st As StorageType In [Enum].GetValues(GetType(StorageType))
                If st = StorageType.UTF8 Then
                    Continue For
                End If
                Dim x As KernelTestArray = KernelTestArray.FromValues(st, shape, "f"c, values, 3, 2)
                For Each dt As StorageType In [Enum].GetValues(GetType(StorageType))
                    If dt = StorageType.UTF8 Then
                        Continue For
                    End If
                    ' expected: the element rule applied to x's stored values
                    Dim expected As ManagedStorage = ManagedStorage.Create(dt, n)
                    For i As Integer = 0 To n - 1
                        Global.DeepLearning4VB.Tests.org.nd4vb.linalg.managed.storage.ManagedStorageTest.ConvertByRule(x.Storage, x.Offset + ShapeInfoHelper.OffsetOf(x.Info, i), expected, i)
                    Next
                    For Each target In {(Dims:=shape, Order:="c"c, Offset:=1L, Spacing:=1L), (Dims:=shape, Order:="f"c, Offset:=0L, Spacing:=3L), (Dims:=otherShape, Order:="c"c, Offset:=2L, Spacing:=2L)}
                        Dim z As KernelTestArray = KernelTestArray.Create(dt, target.Dims, target.Order, target.Offset, target.Spacing)
                        ElementWiseKernels.Copy(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info)
                        Dim label As String = st.ToString() & " -> " & dt.ToString() & " into " & String.Join("x", target.Dims) & target.Order
                        For i As Integer = 0 To n - 1
                            Assert.AreEqual(expected.GetString(i), z.Storage.GetString(z.Offset + ShapeInfoHelper.OffsetOf(z.Info, i)), label & " element " & i)
                        Next
                        z.AssertOutsideUntouched()
                    Next
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub ArgumentValidation()
            Dim x As KernelTestArray = KernelTestArray.Sequence(StorageType.FLOAT, New Long() {2, 3}, "c"c)
            Dim z As KernelTestArray = KernelTestArray.Create(StorageType.FLOAT, New Long() {7}, "c"c)
            Assert.ThrowsException(Of ArgumentException)(Sub() ElementWiseKernels.Transform(x.Storage, x.Offset, x.Info, z.Storage, z.Offset, z.Info, TwicePlusOne, Nothing))
            ' a view running past the end of its storage
            Assert.ThrowsException(Of ArgumentException)(Sub() ElementWiseKernels.Transform(x.Storage, 5, x.Info, x.Storage, 0, x.Info, TwicePlusOne, Nothing))
            Assert.ThrowsException(Of ArgumentNullException)(Sub() ElementWiseKernels.Transform(x.Storage, 0, x.Info, x.Storage, 0, x.Info, Nothing, Nothing))
            Dim s As ManagedStorage = ManagedStorage.FromArray(New String() {"a", "b", "c", "d", "e", "f"})
            Assert.ThrowsException(Of NotSupportedException)(Sub() ElementWiseKernels.Transform(s, 0, x.Info, x.Storage, 0, x.Info, TwicePlusOne, Nothing))
            Assert.ThrowsException(Of ArgumentException)(Sub() ElementWiseKernels.Transform(x.Storage, 0, New Long() {2, 3}, x.Storage, 0, x.Info, TwicePlusOne, Nothing))
            ' empty arrays are a no-op
            Dim e As Long() = ShapeInfoHelper.Create(New Long() {0, 3}, "c"c)
            ElementWiseKernels.Transform(x.Storage, 0, e, x.Storage, 0, e, TwicePlusOne, Nothing)
            x.AssertValues(New Double() {1, 2, 3, 4, 5, 6})
        End Sub

    End Class

End Namespace
