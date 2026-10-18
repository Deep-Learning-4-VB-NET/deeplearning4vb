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

End Namespace
