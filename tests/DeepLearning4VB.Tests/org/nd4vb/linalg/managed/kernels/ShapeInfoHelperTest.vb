Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    <TestClass>
    Public Class ShapeInfoHelperTest

        <TestMethod>
        Public Sub CreateLaysOutNd4jShapeInfo()
            Dim info As Long() = ShapeInfoHelper.Create(New Long() {2, 3, 4}, "c"c, 12345L)
            CollectionAssert.AreEqual(New Long() {3, 2, 3, 4, 12, 4, 1, 12345, 1, 99}, info)
            Assert.AreEqual(3, ShapeInfoHelper.Rank(info))
            Assert.AreEqual(24L, ShapeInfoHelper.Length(info))
            Assert.AreEqual("c"c, ShapeInfoHelper.GetOrder(info))
            Assert.AreEqual(12345L, ShapeInfoHelper.Extras(info))
            Assert.AreEqual(1L, ShapeInfoHelper.ElementWiseStride(info))
            Assert.AreEqual(3L, ShapeInfoHelper.Size(info, 1))
            Assert.AreEqual(4L, ShapeInfoHelper.Stride(info, 1))
            CollectionAssert.AreEqual(New Long() {2, 3, 4}, ShapeInfoHelper.GetShape(info))
            CollectionAssert.AreEqual(New Long() {12, 4, 1}, ShapeInfoHelper.GetStrides(info))

            Dim f As Long() = ShapeInfoHelper.Create(New Long() {2, 3, 4}, "f"c)
            CollectionAssert.AreEqual(New Long() {3, 2, 3, 4, 1, 2, 6, 0, 1, 102}, f)
            Dim scalar As Long() = ShapeInfoHelper.Create(New Long() {}, "c"c)
            CollectionAssert.AreEqual(New Long() {0, 0, 1, 99}, scalar)
            Assert.AreEqual(1L, ShapeInfoHelper.Length(scalar))
            Assert.AreEqual(0L, ShapeInfoHelper.Length(ShapeInfoHelper.Create(New Long() {3, 0}, "c"c)))
            Assert.AreEqual(4, ShapeInfoHelper.InfoLength(0))
        End Sub

        <TestMethod>
        Public Sub ElementWiseStride()
            ' a column of a c-ordered 3x4 matrix: shape [3, 1], strides [4, 1]
            Assert.AreEqual(4L, ShapeInfoHelper.ComputeElementWiseStride(New Long() {3, 1}, New Long() {4, 1}, "c"c))
            ' every second element
            Assert.AreEqual(2L, ShapeInfoHelper.ComputeElementWiseStride(New Long() {2, 3}, New Long() {6, 2}, "c"c))
            ' a 2x2 block of a 4x4 matrix is not equally spaced
            Assert.AreEqual(0L, ShapeInfoHelper.ComputeElementWiseStride(New Long() {2, 2}, New Long() {4, 1}, "c"c))
            ' f-contiguous data: ews 1 in f order, none in c order
            Assert.AreEqual(1L, ShapeInfoHelper.ComputeElementWiseStride(New Long() {2, 3}, New Long() {1, 2}, "f"c))
            Assert.AreEqual(0L, ShapeInfoHelper.ComputeElementWiseStride(New Long() {2, 3}, New Long() {1, 2}, "c"c))
            Assert.AreEqual(1L, ShapeInfoHelper.ComputeElementWiseStride(New Long() {1, 1}, New Long() {7, 9}, "c"c))
        End Sub

        <TestMethod>
        Public Sub LinearStrideAndContiguity()
            Assert.AreEqual(1L, ShapeInfoHelper.LinearStride(ShapeInfoHelper.Create(New Long() {2, 3}, "c"c)))
            Assert.AreEqual(0L, ShapeInfoHelper.LinearStride(ShapeInfoHelper.Create(New Long() {2, 3}, "f"c)))
            Assert.AreEqual(3L, ShapeInfoHelper.LinearStride(ShapeInfoHelper.Create(New Long() {2, 1}, New Long() {3, 1}, "c"c)))
            Assert.IsTrue(ShapeInfoHelper.IsContiguous(ShapeInfoHelper.Create(New Long() {2, 3}, "f"c)))
            Assert.IsTrue(ShapeInfoHelper.IsContiguous(ShapeInfoHelper.Create(New Long() {2, 3}, "f"c), "f"c))
            Assert.IsFalse(ShapeInfoHelper.IsContiguous(ShapeInfoHelper.Create(New Long() {2, 3}, "f"c), "c"c))
            Assert.IsFalse(ShapeInfoHelper.IsContiguous(ShapeInfoHelper.Create(New Long() {2, 2}, New Long() {4, 1}, "c"c)))
        End Sub

        <TestMethod>
        Public Sub OffsetsFollowRowMajorLogicalOrder()
            Dim f As Long() = ShapeInfoHelper.Create(New Long() {2, 3}, "f"c)
            ' logical (0,0),(0,1),(0,2),(1,0),(1,1),(1,2) at f offsets 0,2,4,1,3,5
            CollectionAssert.AreEqual(New Long() {0, 2, 4, 1, 3, 5}, ShapeInfoHelper.Offsets(f))
            Assert.AreEqual(3L, ShapeInfoHelper.OffsetOf(f, 4L))
            Assert.AreEqual(5L, ShapeInfoHelper.OffsetOf(f, New Long() {1, 2}))
            CollectionAssert.AreEqual(New Long() {1, 2}, ShapeInfoHelper.IndexToCoordinates(New Long() {2, 3}, 5))
            Dim view As Long() = ShapeInfoHelper.Create(New Long() {2, 2, 2}, New Long() {20, 5, 2}, "c"c)
            CollectionAssert.AreEqual(New Long() {0, 2, 5, 7, 20, 22, 25, 27}, ShapeInfoHelper.Offsets(view))
            For i As Long = 0 To 7
                Assert.AreEqual(ShapeInfoHelper.Offsets(view)(CInt(i)), ShapeInfoHelper.OffsetOf(view, i))
            Next
            CollectionAssert.AreEqual(New Long() {0}, ShapeInfoHelper.Offsets(ShapeInfoHelper.Create(New Long() {}, "c"c)))
            Assert.AreEqual(0, ShapeInfoHelper.Offsets(ShapeInfoHelper.Create(New Long() {0, 3}, "c"c)).Length)
        End Sub

        <TestMethod>
        Public Sub SqueezedShapes()
            Assert.IsTrue(ShapeInfoHelper.SqueezedShapesEqual(New Long() {1, 3, 1, 4}, New Long() {3, 4, 1}))
            Assert.IsFalse(ShapeInfoHelper.SqueezedShapesEqual(New Long() {3, 4}, New Long() {4, 3}))
            Assert.IsTrue(ShapeInfoHelper.SqueezedShapesEqual(New Long() {}, New Long() {1, 1}))
            Assert.IsFalse(ShapeInfoHelper.SqueezedShapesEqual(New Long() {12}, New Long() {3, 4}))
        End Sub

    End Class

    <TestClass>
    Public Class TadPackTest

        ''' <summary>Checks every element of every TAD against a brute-force enumeration of the array's coordinates.</summary>
        Private Shared Sub CheckTads(ByVal shape As Long(), ByVal order As Char, ByVal dims As Integer(), Optional ByVal spacing As Long = 1)
            Dim strides As Long() = ShapeInfoHelper.CalculateStrides(shape, order)
            For d As Integer = 0 To strides.Length - 1
                strides(d) *= spacing
            Next
            Dim info As Long() = ShapeInfoHelper.Create(shape, strides, order)
            Dim tad As TadPack = TadPack.Create(info, dims)
            Dim rank As Integer = shape.Length
            Dim tadDims As Integer() = If(dims.Length = 0, KernelTestUtil.AllDimensionSets(rank)((1 << rank) - 1), dims)
            Dim outer As Integer() = KernelTestUtil.Outer(rank, dims)
            Dim label As String = "shape [" & String.Join(",", shape) & "] " & order & " dims " & KernelTestUtil.Describe(dims)
            Dim expectedTads As Long = 1
            For Each d As Integer In outer
                expectedTads *= shape(d)
            Next
            Assert.AreEqual(expectedTads, tad.NumTads, label & " numTads")
            Dim tadLength As Long = ShapeInfoHelper.Length(info) \ Math.Max(1L, expectedTads)
            Assert.AreEqual(tadLength, tad.TadLength, label & " tadLength")
            CollectionAssert.AreEqual(tadDims, tad.Dimensions, label & " dims")
            CollectionAssert.AreEqual(outer, tad.OuterDimensions, label & " outer")
            ' TAD shape = shape along the TAD dimensions, with the array's strides and order
            Dim tadInfo As Long() = tad.TadShapeInfo
            Assert.AreEqual(tadDims.Length, ShapeInfoHelper.Rank(tadInfo), label & " tad rank")
            For i As Integer = 0 To tadDims.Length - 1
                Assert.AreEqual(shape(tadDims(i)), ShapeInfoHelper.Size(tadInfo, i), label & " tad shape")
                Assert.AreEqual(strides(tadDims(i)), ShapeInfoHelper.Stride(tadInfo, i), label & " tad stride")
            Next
            Assert.AreEqual(order, ShapeInfoHelper.GetOrder(tadInfo), label & " tad order")
            ' every element: TAD (row-major over outer dims) and position (row-major over TAD dims)
            Dim n As Long = ShapeInfoHelper.Length(info)
            For i As Long = 0 To n - 1
                Dim coords As Long() = KernelTestUtil.Coordinates(shape, i)
                Dim t As Long = KernelTestUtil.LinearIndex(shape, coords, outer)
                Dim j As Long = KernelTestUtil.LinearIndex(shape, coords, tadDims)
                Dim expectedOffset As Long = ShapeInfoHelper.OffsetOf(info, coords)
                Dim actual As Long = tad.TadOffsets(CInt(t)) + ShapeInfoHelper.OffsetOf(tadInfo, j)
                Assert.AreEqual(expectedOffset, actual, label & " element " & i)
            Next
        End Sub

        <TestMethod>
        Public Sub Rank2AllDimensionSetsBothOrders()
            For Each order As Char In {"c"c, "f"c}
                For Each dims As Integer() In KernelTestUtil.AllDimensionSets(2)
                    CheckTads(New Long() {3, 4}, order, dims)
                    CheckTads(New Long() {3, 4}, order, dims, 3)
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub Rank3AllDimensionSetsBothOrders()
            For Each order As Char In {"c"c, "f"c}
                For Each dims As Integer() In KernelTestUtil.AllDimensionSets(3)
                    CheckTads(New Long() {2, 3, 4}, order, dims)
                    CheckTads(New Long() {3, 1, 5}, order, dims, 2)
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub Rank4AllDimensionSetsBothOrders()
            For Each order As Char In {"c"c, "f"c}
                For Each dims As Integer() In KernelTestUtil.AllDimensionSets(4)
                    CheckTads(New Long() {2, 3, 2, 4}, order, dims)
                Next
            Next
        End Sub

        <TestMethod>
        Public Sub KnownOffsets()
            ' c-ordered 2x3x4, TADs along dimension 2 are the 6 rows, 4 apart
            Dim tad As TadPack = TadPack.Create(ShapeInfoHelper.Create(New Long() {2, 3, 4}, "c"c), 2)
            CollectionAssert.AreEqual(New Long() {0, 4, 8, 12, 16, 20}, tad.TadOffsets)
            Assert.AreEqual(1L, ShapeInfoHelper.ElementWiseStride(tad.TadShapeInfo))
            ' along dimension 0: 12 TADs of length 2, offsets 0..11, stride 12
            Dim tad0 As TadPack = TadPack.Create(ShapeInfoHelper.Create(New Long() {2, 3, 4}, "c"c), 0)
            Assert.AreEqual(12L, tad0.NumTads)
            CollectionAssert.AreEqual(New Long() {12}, ShapeInfoHelper.GetStrides(tad0.TadShapeInfo))
            Assert.AreEqual(12L, ShapeInfoHelper.ElementWiseStride(tad0.TadShapeInfo))
            ' f-ordered 2x3x4 along dims (0, 2): TADs over dimension 1 at offsets 0, 2, 4; elements not equally spaced in f order
            Dim tadF As TadPack = TadPack.Create(ShapeInfoHelper.Create(New Long() {2, 3, 4}, "f"c), 2, 0)
            CollectionAssert.AreEqual(New Long() {0, 2, 4}, tadF.TadOffsets)
            CollectionAssert.AreEqual(New Long() {2, 4}, ShapeInfoHelper.GetShape(tadF.TadShapeInfo))
            CollectionAssert.AreEqual(New Long() {1, 6}, ShapeInfoHelper.GetStrides(tadF.TadShapeInfo))
            Assert.AreEqual(0L, ShapeInfoHelper.ElementWiseStride(tadF.TadShapeInfo))
        End Sub

        <TestMethod>
        Public Sub DimensionNormalization()
            CollectionAssert.AreEqual(New Integer() {0, 2}, TadPack.NormalizeDimensions(3, New Integer() {-1, 0, 2, 0}))
            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Function() TadPack.NormalizeDimensions(3, New Integer() {3}))
            Assert.ThrowsException(Of ArgumentOutOfRangeException)(Function() TadPack.NormalizeDimensions(3, New Integer() {-4}))
            Dim whole As TadPack = TadPack.Create(ShapeInfoHelper.Create(New Long() {2, 3}, "c"c))
            Assert.AreEqual(1L, whole.NumTads)
            Assert.AreEqual(6L, whole.TadLength)
            Dim scalar As TadPack = TadPack.Create(ShapeInfoHelper.Create(New Long() {}, "c"c))
            Assert.AreEqual(1L, scalar.NumTads)
            Assert.AreEqual(1L, scalar.TadLength)
            CollectionAssert.AreEqual(New Long() {2}, TadPack.OuterShape(ShapeInfoHelper.Create(New Long() {2, 3}, "c"c), 1))
            Dim empty As TadPack = TadPack.Create(ShapeInfoHelper.Create(New Long() {0, 3}, "c"c), 1)
            Assert.AreEqual(0L, empty.NumTads)
            Assert.AreEqual(3L, empty.TadLength)
        End Sub

    End Class

End Namespace
