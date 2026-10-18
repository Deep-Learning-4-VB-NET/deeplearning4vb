Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.api.shape.options
Imports ShapeT = DeepLearning4VB.org.nd4vb.linalg.api.shape.Shape
Imports ShapeInfoHelper = DeepLearning4VB.org.nd4vb.linalg.managed.kernels.ShapeInfoHelper
Imports JvmShapeInfo = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.JvmShapeInfo
Imports CoordinateFunction = DeepLearning4VB.org.nd4vb.linalg.api.shape.loop.coordinatefunction.CoordinateFunction
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb

Namespace org.nd4vb.linalg.api.shape

    ''' <summary>
    ''' Tests for the pure functions of <c>Shape</c> (no arrays): shape information creation and readers, element-wise
    ''' strides, order detection, offsets, linear index conversions, broadcasting, reductions, matrix multiplication
    ''' shapes, vector/matrix/scalar checks and the data-type flags in the extras. The expected values follow the Java
    ''' semantics, including its quirks.
    ''' </summary>
    <TestClass>
    Public Class ShapeTests

        Private Shared ReadOnly FloatBit As Long = ArrayOptionsHelper.DTYPE_FLOAT_BIT

        ' ---------------------------------------------------------------- shape information creation and readers

        <TestMethod>
        Public Sub testCreateShapeInfoArrayLayout()
            ' upstream LongShapeTests.testLongShape_1: a 5x3 DOUBLE c-order array
            assertArrayEquals(New Long() {2, 5, 3, 3, 1, 16384, 1, 99}, ShapeT.createShapeInfoArray(New Long() {5, 3}, New Long() {3, 1}, 1, "c"c, DataType.DOUBLE, False))
            assertArrayEquals(New Long() {2, 2, 3, 3, 1, FloatBit, 1, 99}, ShapeT.createShapeInfoArray(New Long() {2, 3}, New Long() {3, 1}, 1, "c"c, FloatBit))
            assertArrayEquals(New Long() {3, 2, 3, 4, 1, 2, 6, FloatBit, 1, 102}, ShapeT.createShapeInfoArray(New Long() {2, 3, 4}, New Long() {1, 2, 6}, 1, "f"c, FloatBit))
            ' scalar: [0, extras, ews, order]
            assertArrayEquals(New Long() {0, FloatBit, 1, 99}, ShapeT.createShapeInfoArray(New Long() {}, New Long() {}, 1, "c"c, DataType.FLOAT, False))
            ' the element-wise stride is stored as given
            assertArrayEquals(New Long() {1, 4, 2, FloatBit, 2, 99}, ShapeT.createShapeInfoArray(New Long() {4}, New Long() {2}, 2, "c"c, FloatBit))
        End Sub

        <TestMethod>
        Public Sub testCreateShapeInfoArrayValidation()
            assertThrows(Of ND4VBIllegalStateException)(Sub() ShapeT.createShapeInfoArray(New Long() {2, 3}, New Long() {1}, 1, "c"c, FloatBit))
            assertThrows(Of ArgumentNullException)(Sub() ShapeT.createShapeInfoArray(Nothing, New Long() {1}, 1, "c"c, FloatBit))
            assertThrows(Of ArgumentNullException)(Sub() ShapeT.createShapeInfoArray(New Long() {1}, Nothing, 1, "c"c, FloatBit))
        End Sub

        <TestMethod>
        Public Sub testShapeInfoAgreesWithKernelHelper()
            Dim shapes As Long()() = {New Long() {}, New Long() {7}, New Long() {2, 3}, New Long() {1, 4}, New Long() {4, 1}, New Long() {2, 3, 4}, New Long() {3, 1, 5, 2}}
            For Each order As Char In New Char() {"c"c, "f"c}
                For Each s As Long() In shapes
                    Dim strides As Long() = ShapeInfoHelper.CalculateStrides(s, order)
                    Dim expected As Long() = ShapeInfoHelper.Create(s, order, FloatBit)
                    Dim ews As Long = ShapeInfoHelper.ComputeElementWiseStride(s, strides, order)
                    Dim actual As Long() = ShapeT.createShapeInfoArray(s, strides, ews, order, FloatBit)
                    assertArrayEquals(expected, actual)
                    ' the readers of both sides agree
                    assertEquals(ShapeInfoHelper.Rank(expected), ShapeT.rank(actual))
                    assertArrayEquals(ShapeInfoHelper.GetShape(expected), ShapeT.shape(actual))
                    assertArrayEquals(ShapeInfoHelper.GetStrides(expected), ShapeT.stride(actual))
                    assertEquals(ShapeInfoHelper.Extras(expected), ShapeT.extras(actual))
                    assertEquals(ShapeInfoHelper.ElementWiseStride(expected), ShapeT.elementWiseStride(actual))
                    assertEquals(ShapeInfoHelper.GetOrder(expected), ShapeT.order(actual))
                    assertEquals(ShapeInfoHelper.Length(expected), ShapeT.length(actual))
                    assertEquals(ShapeInfoHelper.InfoLength(s.Length), ShapeT.shapeInfoLength(actual))
                    ' dense layouts have element-wise stride 1 in their own order
                    If s.Length > 0 Then
                        assertEquals(1L, ShapeT.elementWiseStride(s, strides, order = "f"c))
                    End If
                Next s
            Next order
        End Sub

        <TestMethod>
        Public Sub testShapeInfoReaders()
            Dim info As Long() = ShapeT.createShapeInfoArray(New Long() {2, 3, 4}, New Long() {12, 4, 1}, 1, "c"c, DataType.DOUBLE, False)
            assertEquals(3, ShapeT.rank(info))
            assertArrayEquals(New Long() {2, 3, 4}, ShapeT.shape(info))
            assertArrayEquals(New Long() {2, 3, 4}, ShapeT.shapeOf(info))
            assertArrayEquals(New Long() {12, 4, 1}, ShapeT.stride(info))
            assertArrayEquals(New Long() {12, 4, 1}, ShapeT.stridesOf(info))
            assertEquals(3L, ShapeT.size(info, 1))
            assertEquals(4L, ShapeT.stride(info, 1))
            assertEquals(4L, ShapeT.sizeUnsafe(info, 2))
            assertEquals(1L, ShapeT.strideUnsafe(info, 2, 3))
            assertEquals(24L, ShapeT.length(info))
            assertEquals(10, ShapeT.shapeInfoLength(3L))
            assertEquals(10, ShapeT.shapeInfoLength(info))
            assertEquals(ArrayOptionsHelper.DTYPE_DOUBLE_BIT, ShapeT.options(info))
            assertEquals(ArrayOptionsHelper.DTYPE_DOUBLE_BIT, ShapeT.extras(info))
            assertEquals(1L, ShapeT.elementWiseStride(info))
            assertEquals("c"c, ShapeT.order(info))
            assertFalse(ShapeT.isEmpty(info))
            assertThrows(Of ArgumentException)(Sub() ShapeT.size(info, 3))
            assertThrows(Of ArgumentException)(Sub() ShapeT.stride(info, 3))

            ' Integer() shape information
            Dim intInfo As Integer() = {2, 2, 3, 3, 1, 0, 1, 102}
            assertEquals(2, ShapeT.rank(intInfo))
            assertArrayEquals(New Integer() {2, 3}, ShapeT.shape(intInfo))
            assertArrayEquals(New Integer() {3, 1}, ShapeT.stride(intInfo))
            assertArrayEquals(New Integer() {3, 1}, ShapeT.stridesOf(intInfo))
            assertEquals(3, ShapeT.size(intInfo, 1))
            assertEquals(1, ShapeT.stride(intInfo, 1))
            assertEquals(6L, ShapeT.length(intInfo))
            assertEquals("f"c, ShapeT.order(intInfo))

            assertTrue(ShapeT.contentEquals(New Long() {1, 2}, New Long() {1, 2, 3}))
            assertFalse(ShapeT.contentEquals(New Long() {1, 2}, New Long() {1, 3}))
        End Sub

        <TestMethod>
        Public Sub testDataTypeFlagsInExtras()
            For Each dt As DataType In New DataType() {DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.BFLOAT16, DataType.LONG, DataType.INT, DataType.SHORT, DataType.BYTE, DataType.UBYTE, DataType.BOOL, DataType.UTF8, DataType.UINT16, DataType.UINT32, DataType.UINT64}
                Dim info As Long() = ShapeT.createShapeInfoArray(New Long() {2, 2}, New Long() {2, 1}, 1, "c"c, dt, False)
                assertEquals(dt, ArrayOptionsHelper.dataType(info))
                assertEquals(ArrayType.DENSE, ArrayOptionsHelper.arrayType(info))
                assertFalse(ShapeT.isEmpty(info))

                Dim emptyInfo As Long() = ShapeT.createShapeInfoArray(New Long() {2, 2}, New Long() {2, 1}, 1, "c"c, dt, True)
                assertEquals(dt, ArrayOptionsHelper.dataType(emptyInfo))
                assertTrue(ShapeT.isEmpty(emptyInfo))
            Next dt

            ' a zero dimension makes the array empty
            Dim zero As Long() = ShapeT.createShapeInfoArray(New Long() {0, 3}, New Long() {3, 1}, 1, "c"c, DataType.FLOAT, False)
            assertEquals(FloatBit Or ArrayOptionsHelper.ATYPE_EMPTY_BIT, ShapeT.extras(zero))
            assertTrue(ShapeT.isEmpty(zero))
            ' JvmShapeInfo reads the same fields; an empty array has length 0
            Dim jvm As New JvmShapeInfo(zero)
            assertEquals(0L, jvm.getLength())
            assertEquals(2, jvm.getRank())
            assertArrayEquals(New Long() {0, 3}, jvm.getShape())
            assertArrayEquals(New Long() {3, 1}, jvm.getStride())
            assertEquals(FloatBit Or ArrayOptionsHelper.ATYPE_EMPTY_BIT, jvm.getExtras())
            assertEquals(1L, jvm.getEws())
            assertEquals("c"c, jvm.getOrder())
        End Sub

        <TestMethod>
        Public Sub testJvmShapeInfo()
            Dim info As Long() = ShapeT.createShapeInfoArray(New Long() {3, 4}, New Long() {1, 3}, 1, "f"c, DataType.INT, False)
            Dim jvm As New JvmShapeInfo(info)
            assertArrayEquals(info, jvm.getJavaShapeInformation())
            assertArrayEquals(New Long() {3, 4}, jvm.getShape())
            assertArrayEquals(New Long() {1, 3}, jvm.getStride())
            assertEquals(12L, jvm.getLength())
            assertEquals(1L, jvm.getEws())
            assertEquals(ArrayOptionsHelper.DTYPE_INT_BIT, jvm.getExtras())
            assertEquals("f"c, jvm.getOrder())
            assertEquals(2, jvm.getRank())
            assertThrows(Of ArgumentNullException)(Sub()
                                                         Dim unused As New JvmShapeInfo(Nothing)
                                                     End Sub)
        End Sub

        <TestMethod>
        Public Sub testShapeToString()
            Dim info As Long() = ShapeT.createShapeInfoArray(New Long() {2, 3}, New Long() {3, 1}, 1, "c"c, DataType.FLOAT, False)
            assertEquals("Rank: 2, DataType: FLOAT, Offset: 0, Order: c, Shape: [2,3],  Stride: [3,1]", ShapeT.shapeToString(info))
        End Sub

        ' ---------------------------------------------------------------- element-wise stride and order

        <TestMethod>
        Public Sub testElementWiseStride()
            assertEquals(1L, ShapeT.elementWiseStride(New Long() {2, 3}, New Long() {3, 1}, False))
            assertEquals(1L, ShapeT.elementWiseStride(New Long() {2, 3}, New Long() {1, 2}, True))
            ' F layout read in C order, and a sub-matrix view: not equally spaced
            assertEquals(0L, ShapeT.elementWiseStride(New Long() {2, 3}, New Long() {1, 2}, False))
            assertEquals(0L, ShapeT.elementWiseStride(New Long() {2, 2}, New Long() {6, 1}, False))
            ' a strided row vector
            assertEquals(2L, ShapeT.elementWiseStride(New Long() {1, 4}, New Long() {8, 2}, False))
            assertEquals(2L, ShapeInfoHelper.ComputeElementWiseStride(New Long() {1, 4}, New Long() {8, 2}, "c"c))
            ' every second row of a 4x3 matrix
            assertEquals(0L, ShapeT.elementWiseStride(New Long() {2, 3}, New Long() {6, 1}, False))
            ' rank 1 gives the stride, rank 0 gives 1
            assertEquals(3L, ShapeT.elementWiseStride(New Long() {5}, New Long() {3}, False))
            assertEquals(1L, ShapeT.elementWiseStride(New Long() {}, New Long() {}, False))
            ' Java quirk: all dimensions 1 gives the last stride (the kernels' helper gives 1)
            assertEquals(7L, ShapeT.elementWiseStride(New Long() {1, 1}, New Long() {5, 7}, False))
            ' zero length gives 0
            assertEquals(0L, ShapeT.elementWiseStride(New Long() {0, 3}, New Long() {3, 1}, False))

            ' Integer overloads (rank 1 gives 1 here, as in Java)
            assertEquals(1, ShapeT.elementWiseStride(New Integer() {2, 3}, New Integer() {3, 1}, False))
            assertEquals(1, ShapeT.elementWiseStride(New Integer() {5}, New Integer() {3}, False))
            assertEquals(0, ShapeT.elementWiseStride(New Integer() {2, 2}, New Integer() {6, 1}, False))
        End Sub

        <TestMethod>
        Public Sub testOrderDetection()
            assertEquals("c"c, ShapeT.getOrder(New Long() {2, 3}, New Long() {3, 1}, 1L))
            assertEquals("f"c, ShapeT.getOrder(New Long() {2, 3}, New Long() {1, 2}, 1L))
            assertEquals("a"c, ShapeT.getOrder(New Long() {3}, New Long() {1}, 1L))
            assertEquals("c"c, ShapeT.getOrder(New Long() {1, 3}, New Long() {3, 1}, 1L))
            ' not contiguous: ascending strides read as 'f', anything else as 'c'
            assertEquals("f"c, ShapeT.getOrder(New Long() {2, 2}, New Long() {2, 10}, 1L))
            assertEquals("c"c, ShapeT.getOrder(New Long() {2, 2}, New Long() {10, 2}, 1L))
            ' the Integer overload has no ascending-strides rule
            assertEquals("c"c, ShapeT.getOrder(New Integer() {2, 2}, New Integer() {2, 10}, 1))
            assertEquals("f"c, ShapeT.getOrder(New Integer() {2, 3}, New Integer() {1, 2}, 1))

            assertTrue(ShapeT.cOrFortranOrder(New Long() {2, 3}, New Long() {3, 1}, 1L))
            assertTrue(ShapeT.cOrFortranOrder(New Long() {2, 3}, New Long() {1, 2}, 1L))
            assertFalse(ShapeT.cOrFortranOrder(New Long() {2, 2}, New Long() {2, 10}, 1L))

            ShapeT.assertValidOrder("c"c)
            ShapeT.assertValidOrder("f"c)
            ShapeT.assertValidOrder("a"c)
            assertThrows(Of ArgumentException)(Sub() ShapeT.assertValidOrder("x"c))
        End Sub

        ' ---------------------------------------------------------------- offsets

        <TestMethod>
        Public Sub testOffsets()
            Dim info As Long() = ShapeT.createShapeInfoArray(New Long() {2, 3}, New Long() {3, 1}, 1, "c"c, FloatBit)
            assertEquals(5L, ShapeT.getOffset(info, 1L, 2L))
            assertEquals(5L, ShapeT.getOffset(info, 1, 2))
            assertEquals(5L, ShapeT.getOffsetUnsafe(info, 1L, 2L))
            assertThrows(Of ArgumentException)(Sub() ShapeT.getOffset(info, 3L, 0L))
            assertThrows(Of ArgumentException)(Sub() ShapeT.getOffsetUnsafe(info, 2L, 0L))

            ' dimensions of size 1 are skipped
            Dim row As Long() = ShapeT.createShapeInfoArray(New Long() {1, 3}, New Long() {99, 1}, 1, "c"c, FloatBit)
            assertEquals(2L, ShapeT.getOffset(row, 0L, 2L))

            Dim info4 As Long() = ShapeT.createShapeInfoArray(New Long() {2, 3, 4, 5}, New Long() {60, 20, 5, 1}, 1, "c"c, FloatBit)
            assertEquals(119L, ShapeT.getOffsetUnsafe(info4, 1L, 2L, 3L, 4L))
            assertEquals(119L, ShapeT.getOffset(info4, 1L, 2L, 3L, 4L))
            assertThrows(Of ArgumentException)(Sub() ShapeT.getOffsetUnsafe(info4, 2L, 0L, 0L, 0L))

            Dim fInfo As Long() = ShapeT.createShapeInfoArray(New Long() {2, 3}, New Long() {1, 2}, 1, "f"c, FloatBit)
            assertEquals(5L, ShapeT.getOffset(fInfo, 1L, 2L))

            ' Integer() shape information
            Dim intInfo As Integer() = {3, 2, 3, 4, 12, 4, 1, 0, 1, 99}
            assertEquals(23L, ShapeT.getOffset(intInfo, 1, 2, 3))
            assertEquals(23L, ShapeT.getOffsetUnsafe(intInfo, 1, 2, 3))

            ' with a base offset; an index equal to the size throws
            assertEquals(14L, ShapeT.getOffset(10L, New Integer() {2, 3}, New Integer() {3, 1}, 1, 1))
            assertThrows(Of ArgumentException)(Sub() ShapeT.getOffset(10L, New Integer() {2, 3}, New Integer() {3, 1}, 2, 0))
            assertThrows(Of ArgumentException)(Sub() ShapeT.getOffset(10L, New Integer() {2, 3}, New Integer() {3, 1}, 1))

            ' the buffer length a layout needs
            assertEquals(6L, ShapeT.lengthOfBuffer(New Long() {2, 3}, New Long() {3, 1}))
            assertEquals(8L, ShapeT.lengthOfBuffer(New Long() {2, 2}, New Long() {6, 1}))
            assertEquals(8L, ShapeT.lengthOfBuffer(New Integer() {2, 2}, New Integer() {6, 1}))
            assertThrows(Of ArgumentException)(Sub() ShapeT.lengthOfBuffer(New Long() {2, 2}, New Long() {1}))
        End Sub

        ' ---------------------------------------------------------------- linear index conversions

        <TestMethod>
        Public Sub testInd2SubAndSub2Ind()
            assertArrayEquals(New Long() {1, 1}, ShapeT.ind2subC(New Long() {2, 3}, 4L))
            assertArrayEquals(New Long() {0, 2}, ShapeT.ind2sub(New Long() {2, 3}, 4L))
            assertArrayEquals(New Integer() {1, 1}, ShapeT.ind2subC(New Integer() {2, 3}, 4L))
            assertArrayEquals(New Integer() {0, 2}, ShapeT.ind2sub(New Integer() {2, 3}, 4L))
            ' upstream StaticShapeTests.testShapeInd2Sub
            assertArrayEquals(New Integer() {0, 1}, ShapeT.ind2subC(New Integer() {2, 2}, 1L))
            assertArrayEquals(New Integer() {1, 0}, ShapeT.ind2sub(New Integer() {2, 2}, 1L))

            ' every linear index round-trips; sub2Ind is the Fortran-order index
            Dim shape As Integer() = {2, 3, 4}
            For i As Integer = 0 To 23
                Dim f As Integer() = ShapeT.ind2sub(shape, CLng(i))
                assertEquals(CLng(i), ShapeT.sub2Ind(shape, f))
                Dim c As Long() = ShapeT.ind2subC(New Long() {2, 3, 4}, CLng(i))
                assertEquals(CLng(i), c(0) * 12 + c(1) * 4 + c(2))
            Next i

            ' an explicit number of indices
            assertArrayEquals(New Long() {0, 1}, ShapeT.ind2subC(New Long() {2, 3}, 1L, 6L))
            assertArrayEquals(New Long() {1, 0}, ShapeT.ind2sub(New Long() {2, 3}, 1L, 6L))
        End Sub

        ' ---------------------------------------------------------------- broadcasting

        <TestMethod>
        Public Sub testBroadcastHelpers()
            assertNull(ShapeT.getBroadcastDimensions(New Long() {3, 4}, New Long() {3, 4}))
            assertArrayEquals(New Integer() {0}, ShapeT.getBroadcastDimensions(New Long() {3, 4}, New Long() {1, 4}))
            assertArrayEquals(New Integer() {0}, ShapeT.getBroadcastDimensions(New Long() {1, 4}, New Long() {3, 4}))
            assertArrayEquals(New Integer() {1}, ShapeT.getBroadcastDimensions(New Integer() {3, 4}, New Integer() {3, 1}))
            assertThrows(Of ArgumentException)(Sub() ShapeT.getBroadcastDimensions(New Long() {3, 4}, New Long() {2, 4}))

            assertArrayEquals(New Integer() {3, 4}, ShapeT.broadcastOutputShape(New Integer() {3, 1}, New Integer() {1, 4}))
            assertArrayEquals(New Integer() {2, 3, 4}, ShapeT.broadcastOutputShape(New Integer() {3, 4}, New Integer() {2, 1, 1}))
            assertThrows(Of ND4VBIllegalStateException)(Sub() ShapeT.broadcastOutputShape(New Integer() {3, 4}, New Integer() {2, 4}))
            ' a shape with a zero dimension wins
            assertArrayEquals(New Long() {0, 3}, ShapeT.broadcastOutputShape(New Long() {0, 3}, New Long() {2, 3}))
            assertArrayEquals(New Long() {2, 0}, ShapeT.broadcastOutputShape(New Long() {2, 3}, New Long() {2, 0}))

            assertTrue(ShapeT.areShapesBroadcastable(New Long() {3, 1, 5}, New Long() {4, 1}))
            assertTrue(ShapeT.areShapesBroadcastable(New Integer() {3, 1, 5}, New Integer() {4, 1}))
            assertFalse(ShapeT.areShapesBroadcastable(New Long() {3, 4}, New Long() {2, 4}))
            assertFalse(ShapeT.areShapesBroadcastable(New Integer() {3, 4}, New Integer() {2, 4}))
            ShapeT.assertBroadcastable(New Long() {3, 1}, New Long() {1, 4})
            Dim e As ND4VBIllegalStateException = assertThrows(Of ND4VBIllegalStateException)(Sub() ShapeT.assertBroadcastable(New Long() {3, 4}, New Long() {2, 4}, GetType(ShapeTests)))
            assertTrue(e.Message.Contains("op: " & GetType(ShapeTests).FullName))
            assertTrue(e.Message.Contains("[3, 4]"))

            assertEquals(4L, ShapeT.sizeAt(New Long() {2, 3, 4}, -1))
            assertEquals(2L, ShapeT.sizeAt(New Long() {2, 3, 4}, 0))
            assertTrue(ShapeT.containsZeros(New Long() {2, 0}))
            assertFalse(ShapeT.containsZeros(New Long() {2, 1}))
        End Sub

        ' ---------------------------------------------------------------- reductions and axes

        <TestMethod>
        Public Sub testReducedShapes()
            assertArrayEquals(New Long() {3, 1}, ShapeT.getReducedShape(New Long() {3, 4}, New Integer() {1}))
            assertArrayEquals(New Long() {1, 4}, ShapeT.getReducedShape(New Long() {3, 4}, New Integer() {0}))
            assertArrayEquals(New Long() {}, ShapeT.getReducedShape(New Long() {3, 4}, New Integer() {0, 1}))
            assertArrayEquals(New Long() {2, 4}, ShapeT.getReducedShape(New Long() {2, 3, 4}, New Integer() {1}))
            assertArrayEquals(New Long() {3, 1}, ShapeT.getReducedShape(New Integer() {3, 4}, New Integer() {1}))

            ' keepDims and the new format
            assertArrayEquals(New Long() {2, 1, 4}, ShapeT.getReducedShape(New Long() {2, 3, 4}, New Integer() {1}, True))
            assertArrayEquals(New Long() {2, 3, 1}, ShapeT.getReducedShape(New Long() {2, 3, 4}, New Integer() {-1}, True))
            assertArrayEquals(New Long() {1, 1}, ShapeT.getReducedShape(New Long() {3, 4}, New Integer() {0, 1}, True))
            assertArrayEquals(New Long() {3}, ShapeT.getReducedShape(New Long() {3, 4}, New Integer() {1}, False))
            assertArrayEquals(New Long() {4}, ShapeT.getReducedShape(New Long() {3, 4}, New Integer() {0}, False))
            assertArrayEquals(New Long() {}, ShapeT.getReducedShape(New Long() {3, 4}, New Integer() {}, False))
            assertArrayEquals(New Long() {3, 1}, ShapeT.getReducedShape(New Long() {3, 4}, New Integer() {1}, False, False))
            assertArrayEquals(New Long() {2, 1, 4}, ShapeT.getReducedShape(New Integer() {2, 3, 4}, New Integer() {1}, True, True))
            assertArrayEquals(New Long() {2, 4}, ShapeT.getReducedShape(New Integer() {2, 3, 4}, New Integer() {1}, False, True))
        End Sub

        <TestMethod>
        Public Sub testAxisHelpers()
            assertArrayEquals(New Integer() {0, 2}, ShapeT.normalizeAxis(3, -1, 0, 0))
            assertArrayEquals(New Integer() {Integer.MaxValue}, ShapeT.normalizeAxis(3))
            assertArrayEquals(New Integer() {Integer.MaxValue}, ShapeT.normalizeAxis(0, 0))
            assertThrows(Of ND4VBIllegalStateException)(Sub() ShapeT.normalizeAxis(2, 5))
            assertThrows(Of ND4VBIllegalStateException)(Sub() ShapeT.normalizeAxis(2, -3))
            assertThrows(Of ND4VBIllegalStateException)(Sub() ShapeT.normalizeAxis(0, 1))
            assertArrayEquals(New Integer() {3, 1, 2}, ShapeT.uniquify(New Integer() {3, 1, 3, 2}))

            assertTrue(ShapeT.isWholeArray(3, 0, 1, 2))
            assertFalse(ShapeT.isWholeArray(3, 0))
            assertTrue(ShapeT.isWholeArray(0, 1))
            assertTrue(ShapeT.isWholeArray(3, Integer.MaxValue))
            assertTrue(ShapeT.isWholeArray(New Long() {2, 3}))
            assertTrue(ShapeT.isWholeArray(New Integer() {2, 3}, 0, 1))
            assertTrue(ShapeT.wholeArrayDimension())
            assertTrue(ShapeT.wholeArrayDimension(Integer.MaxValue))
            assertFalse(ShapeT.wholeArrayDimension(0))

            assertEquals(12L, ShapeT.getTADLength(New Long() {2, 3, 4}, 1, 2))
            assertEquals(8L, ShapeT.getTADLength(New Integer() {2, 3, 4}, 0, 2))
            assertArrayEquals(New Integer() {3, 2}, ShapeT.sizeForAxes(New Integer() {1, 0}, New Integer() {2, 3}))
            assertArrayEquals(New Integer() {1}, ShapeT.newStrides(New Integer() {3, 1}, 1, Nothing))
            assertArrayEquals(New Integer() {3, 1}, ShapeT.newStrides(New Integer() {3, 1}, 2, Nothing))
        End Sub

        <TestMethod>
        Public Sub testMatrixMultiplyShape()
            assertArrayEquals(New Long() {2, 4}, ShapeT.getMatrixMultiplyShape(New Long() {2, 3}, New Long() {3, 4}))
            assertArrayEquals(New Long() {5, 2, 4}, ShapeT.getMatrixMultiplyShape(New Long() {5, 2, 3}, New Long() {5, 3, 4}))
            assertArrayEquals(New Long() {1, 4}, ShapeT.getMatrixMultiplyShape(New Long() {3}, New Long() {3, 4}))
            assertArrayEquals(New Long() {3, 4}, ShapeT.getMatrixMultiplyShape(New Long() {1}, New Long() {3, 4}))
            assertThrows(Of ArgumentException)(Sub() ShapeT.getMatrixMultiplyShape(New Long() {2, 3}, New Long() {4, 5}))
            assertThrows(Of ArgumentException)(Sub() ShapeT.getMatrixMultiplyShape(New Long() {5, 2, 3}, New Long() {4, 3, 4}))
            assertThrows(Of ND4VBIllegalStateException)(Sub() ShapeT.getMatrixMultiplyShape(New Long() {2, 0}, New Long() {0, 3}))

            assertArrayEquals(New Integer() {2, 4}, ShapeT.getMatrixMultiplyShape(New Integer() {2, 3}, New Integer() {3, 4}))
            assertArrayEquals(New Integer() {1, 5}, ShapeT.getMatrixMultiplyShape(New Integer() {4}, New Integer() {4, 5}))
            assertThrows(Of ArgumentException)(Sub() ShapeT.getMatrixMultiplyShape(New Integer() {2, 3}, New Integer() {4, 5}))
        End Sub

        ' ---------------------------------------------------------------- vector, matrix, scalar and shape equality

        <TestMethod>
        Public Sub testVectorMatrixScalar()
            assertTrue(ShapeT.isVector(New Long() {3}))
            assertTrue(ShapeT.isVector(New Long() {1, 3}))
            assertTrue(ShapeT.isVector(New Long() {3, 1}))
            assertTrue(ShapeT.isVector(New Long() {1, 1}))
            assertFalse(ShapeT.isVector(New Long() {2, 3}))
            assertFalse(ShapeT.isVector(New Long() {}))
            assertFalse(ShapeT.isVector(New Long() {1, 1, 3}))
            assertTrue(ShapeT.isVector(New Integer() {1, 3}))

            assertTrue(ShapeT.isMatrix(New Long() {2, 3}))
            assertFalse(ShapeT.isMatrix(New Long() {1, 3}))
            assertFalse(ShapeT.isMatrix(New Long() {3}))
            assertTrue(ShapeT.isMatrix(New Integer() {2, 3}))

            assertTrue(ShapeT.shapeIsScalar(New Long() {}))
            assertTrue(ShapeT.shapeIsScalar(New Long() {1}))
            assertTrue(ShapeT.shapeIsScalar(New Long() {1, 1}))
            assertFalse(ShapeT.shapeIsScalar(New Long() {2}))
            assertTrue(ShapeT.shapeIsScalar(New Integer() {1, 1}))

            assertTrue(ShapeT.isRowVectorShape(New Long() {1, 3}))
            assertTrue(ShapeT.isRowVectorShape(New Long() {3}))
            assertFalse(ShapeT.isRowVectorShape(New Long() {3, 1}))
            assertTrue(ShapeT.isColumnVectorShape(New Long() {3, 1}))
            assertFalse(ShapeT.isColumnVectorShape(New Integer() {1, 3}))

            assertTrue(ShapeT.isPlaceholderShape(New Long() {-1, 3}))
            assertFalse(ShapeT.isPlaceholderShape(New Long() {2, 3}))
            assertTrue(ShapeT.isPlaceholderShape(DirectCast(Nothing, Long())))
            assertFalse(ShapeT.isPlaceholderShape(New Long() {Long.MinValue}))
            assertTrue(ShapeT.isPlaceholderShape(New Integer() {-1}))

            assertTrue(ShapeT.shapeMatchesPlaceholder(New Long() {-1, 3}, New Long() {5, 3}))
            assertFalse(ShapeT.shapeMatchesPlaceholder(New Long() {-1, 3}, New Long() {5, 4}))
            assertFalse(ShapeT.shapeMatchesPlaceholder(New Long() {-1}, New Long() {5, 4}))
            assertTrue(ShapeT.shapeMatchesPlaceholder(Nothing, Nothing))
            assertFalse(ShapeT.shapeMatchesPlaceholder(Nothing, New Long() {1}))

            assertEquals(1L, ShapeT.lengthOf(New Long() {}))
            assertEquals(6L, ShapeT.lengthOf(New Long() {2, 3}))
            assertEquals(0L, ShapeT.lengthOf(New Integer() {2, 0}))
            assertEquals(2, ShapeT.rankFromShape(New Long() {2, 3}))
            assertThrows(Of ND4VBIllegalStateException)(Sub() ShapeT.rankFromShape(DirectCast(Nothing, Long())))
            assertArrayEquals(New Integer() {1, 3}, ShapeT.ensureAtMinRowVector(3))
            assertArrayEquals(New Integer() {2, 3}, ShapeT.ensureAtMinRowVector(2, 3))
        End Sub

        <TestMethod>
        Public Sub testSqueezeAndShapeEquality()
            assertArrayEquals(New Long() {3, 4}, ShapeT.squeeze(New Long() {1, 3, 1, 4}))
            assertArrayEquals(New Long() {3, 1}, ShapeT.squeeze(New Long() {3, 1}))
            assertArrayEquals(New Long() {1, 1}, ShapeT.squeeze(New Long() {1, 1}))
            assertArrayEquals(New Long() {}, ShapeT.squeeze(New Long() {1, 1, 1}))
            assertArrayEquals(New Integer() {3, 4}, ShapeT.squeeze(New Integer() {1, 3, 1, 4}))

            assertTrue(ShapeT.shapeEquals(New Long() {1, 3}, New Long() {3}))
            assertTrue(ShapeT.shapeEquals(New Long() {3, 1}, New Long() {3, 1}))
            assertFalse(ShapeT.shapeEquals(New Long() {3, 1}, New Long() {1, 3}))
            assertTrue(ShapeT.shapeEquals(New Long() {}, New Long() {1}))
            assertTrue(ShapeT.shapeEquals(New Long() {2, 1, 3}, New Long() {2, 3}))
            assertFalse(ShapeT.shapeEquals(New Long() {2, 3}, New Long() {3, 2}))
            assertTrue(ShapeT.shapeEquals(New Integer() {1, 3}, New Integer() {3}))

            assertTrue(ShapeT.shapeEqualWithSqueeze(New Long() {1, 3, 4}, New Long() {3, 4}))
            assertTrue(ShapeT.shapeEqualWithSqueeze(New Long() {2, 1, 1}, New Long() {1, 2}))
            assertFalse(ShapeT.shapeEqualWithSqueeze(New Long() {2}, New Long() {2, 3}))
            assertTrue(ShapeT.shapeEqualWithSqueeze(Nothing, Nothing))
            assertFalse(ShapeT.shapeEqualWithSqueeze(New Long() {1}, Nothing))

            assertTrue(ShapeT.scalarEquals(New Long() {}, New Long() {1}))
            assertTrue(ShapeT.scalarEquals(New Integer() {1}, New Integer() {}))
            assertFalse(ShapeT.scalarEquals(New Long() {2}, New Long() {}))

            ShapeT.assertShapeLessThan(New Long() {1, 2}, New Long() {2, 3})
            assertThrows(Of InvalidOperationException)(Sub() ShapeT.assertShapeLessThan(New Long() {2, 2}, New Long() {2, 3}))
            assertThrows(Of ArgumentException)(Sub() ShapeT.assertShapeLessThan(New Integer() {1}, New Integer() {2, 3}))
        End Sub

        <TestMethod>
        Public Sub testReshapeHelpers()
            ' a -1 dimension takes the remaining length; zeros become 1
            assertArrayEquals(New Integer() {3, 4}, ShapeT.resolveNegativeShapeIfNeccessary(New Integer() {2, 6}, New Integer() {3, -1}))
            assertArrayEquals(New Integer() {1, 6}, ShapeT.resolveNegativeShapeIfNeccessary(New Integer() {2, 3}, New Integer() {0, 6}))
        End Sub

        <TestMethod>
        Public Sub testIterateVisitsEveryCoordinate()
            Dim f As New RecordingFunction()
            ShapeT.iterate(0, 2, New Long() {2, 3}, New Long(1) {}, f)
            assertEquals(6, f.Visited.Count)
            assertEquals("0,0;0,1;0,2;1,0;1,1;1,2", String.Join(";", f.Visited))

            Dim g As New RecordingFunction()
            ShapeT.iterate(0, 2, New Integer() {2, 2}, New Integer(1) {}, g)
            assertEquals("0,0;0,1;1,0;1,1", String.Join(";", g.Visited))
        End Sub

        ''' <summary>Records the coordinates a <see cref="CoordinateFunction"/> sees.</summary>
        Private Class RecordingFunction
            Implements CoordinateFunction

            Public ReadOnly Visited As New List(Of String)()

            Public Sub process(ParamArray ByVal coord()() As Long) Implements CoordinateFunction.process
                Visited.Add(String.Join(",", coord(0)))
            End Sub
        End Class

        ' ---------------------------------------------------------------- data types

        <TestMethod>
        Public Sub testDataTypeClasses()
            assertTrue(ShapeT.isR(DataType.FLOAT))
            assertTrue(ShapeT.isR(DataType.BFLOAT16))
            assertFalse(ShapeT.isR(DataType.INT))
            assertTrue(ShapeT.isZ(DataType.INT))
            assertTrue(ShapeT.isZ(DataType.UINT64))
            assertFalse(ShapeT.isZ(DataType.BOOL))
            assertTrue(ShapeT.isB(DataType.BOOL))
            assertTrue(ShapeT.isS(DataType.UTF8))
        End Sub

        <TestMethod>
        Public Sub testPickPairwiseDataType()
            Dim saved As Boolean = Nd4vbT.PrecisionBoostAllowed
            Try
                Nd4vbT.PrecisionBoostAllowed = False
                assertEquals(DataType.FLOAT, ShapeT.pickPairwiseDataType(DataType.FLOAT, DataType.FLOAT))
                assertEquals(DataType.FLOAT, ShapeT.pickPairwiseDataType(DataType.FLOAT, DataType.INT))
                assertEquals(DataType.DOUBLE, ShapeT.pickPairwiseDataType(DataType.INT, DataType.DOUBLE))
                assertEquals(DataType.FLOAT, ShapeT.pickPairwiseDataType(DataType.FLOAT, DataType.DOUBLE))
                assertEquals(DataType.INT, ShapeT.pickPairwiseDataType(DataType.INT, DataType.LONG))
                assertEquals(DataType.FLOAT, ShapeT.pickPairwiseDataType(DataType.FLOAT, 1.5))
                assertEquals(DataType.DOUBLE, ShapeT.pickPairwiseDataType(DataType.INT, 2.5))
                assertEquals(DataType.FLOAT, ShapeT.pickPairwiseDataType(DataType.INT, 2.5F))
                assertEquals(DataType.INT, ShapeT.pickPairwiseDataType(DataType.INT, 3L))
                assertEquals(DataType.SHORT, ShapeT.pickPairwiseDataType(DataType.SHORT, 3))
                assertEquals(DataType.LONG, ShapeT.pickPairwiseDataType(DataType.LONG, CShort(3)))
                assertEquals(DataType.LONG, ShapeT.pickPairwiseDataType(DataType.LONG, CSByte(3)))
                assertThrows(Of NotSupportedException)(Sub() ShapeT.pickPairwiseDataType(DataType.LONG, 3D))

                ' Java quirk: with precision boost the "bigger" type is the larger ordinal
                Nd4vbT.PrecisionBoostAllowed = True
                assertEquals(DataType.HALF, ShapeT.pickPairwiseDataType(DataType.HALF, DataType.DOUBLE))
                assertEquals(DataType.INT, ShapeT.pickPairwiseDataType(DataType.LONG, DataType.INT))
            Finally
                Nd4vbT.PrecisionBoostAllowed = saved
            End Try
        End Sub

    End Class

End Namespace
