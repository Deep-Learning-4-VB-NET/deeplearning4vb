Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.managed.kernels

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>Tensor-along-dimension shape infos and offsets against a brute-force enumeration of coordinates (ranks 2 to 4, both orders, strided).</summary>
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
