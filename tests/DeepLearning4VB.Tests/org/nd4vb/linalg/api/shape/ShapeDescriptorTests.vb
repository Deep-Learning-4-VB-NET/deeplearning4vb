Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports DeepLearning4VB.org.nd4vb.linalg.api.shape
Imports DeepLearning4VB.org.nd4vb.linalg.api.shape.options
Imports ND4VBUnknownDataTypeException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBUnknownDataTypeException
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.shape

    ''' <summary>
    ''' Tests for <c>LongShapeDescriptor</c>, <c>ShapeDescriptor</c>, <c>ArrayOptionsHelper</c> and <c>TadPack</c>:
    ''' value equality, Java's hash codes, the string form and the extras (data type and array type bits).
    ''' </summary>
    <TestClass>
    Public Class ShapeDescriptorTests

        Private Shared ReadOnly FloatExtras As Long = ArrayOptionsHelper.DTYPE_FLOAT_BIT

        <TestMethod>
        Public Sub testLongShapeDescriptorEqualityIsByValue()
            Dim a As New LongShapeDescriptor(New Long() {2, 3}, New Long() {3, 1}, 0, 1, "c"c, FloatExtras)
            Dim b As New LongShapeDescriptor(New Long() {2, 3}, New Long() {3, 1}, 0, 1, "c"c, FloatExtras)
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertTrue(a.Equals(a))
            assertFalse(a.Equals(Nothing))
            assertFalse(a.Equals("not a descriptor"))

            ' every field takes part in the comparison
            assertFalse(a.Equals(New LongShapeDescriptor(New Long() {3, 2}, New Long() {3, 1}, 0, 1, "c"c, FloatExtras)))
            assertFalse(a.Equals(New LongShapeDescriptor(New Long() {2, 3}, New Long() {1, 2}, 0, 1, "c"c, FloatExtras)))
            assertFalse(a.Equals(New LongShapeDescriptor(New Long() {2, 3}, New Long() {3, 1}, 1, 1, "c"c, FloatExtras)))
            assertFalse(a.Equals(New LongShapeDescriptor(New Long() {2, 3}, New Long() {3, 1}, 0, 2, "c"c, FloatExtras)))
            assertFalse(a.Equals(New LongShapeDescriptor(New Long() {2, 3}, New Long() {3, 1}, 0, 1, "f"c, FloatExtras)))
            assertFalse(a.Equals(New LongShapeDescriptor(New Long() {2, 3}, New Long() {3, 1}, 0, 1, "c"c, ArrayOptionsHelper.DTYPE_DOUBLE_BIT)))
        End Sub

        <TestMethod>
        Public Sub testLongShapeDescriptorCopiesItsArrays()
            Dim shape As Long() = {2, 3}
            Dim stride As Long() = {3, 1}
            Dim d As New LongShapeDescriptor(shape, stride, 0, 1, "c"c, FloatExtras)
            shape(0) = 7
            stride(0) = 9
            assertArrayEquals(New Long() {2, 3}, d.getShape())
            assertArrayEquals(New Long() {3, 1}, d.getStride())
        End Sub

        <TestMethod>
        Public Sub testHashCodesAreJavas()
            ' expected values computed with Java's formulas (Long.hashCode, Arrays.hashCode, 31 * result + ...)
            assertEquals(-1452746183, New LongShapeDescriptor(New Long() {2, 3}, New Long() {3, 1}, 0, 1, "c"c, 8192).GetHashCode())
            assertEquals(-1368844013, New LongShapeDescriptor(New Long() {4, 5, 6}, New Long() {1, 4, 20}, 5, -1, "f"c, (1L << 40) Or 8L).GetHashCode())
            assertEquals(-1216599653, New ShapeDescriptor(New Integer() {2, 3}, New Integer() {3, 1}, 0, 1, "c"c, 8192).GetHashCode())
        End Sub

        <TestMethod>
        Public Sub testToStringLikeJava()
            assertEquals("[2,2, 3,3, 1,8192,1,c]", New LongShapeDescriptor(New Long() {2, 3}, New Long() {3, 1}, 0, 1, "c"c, 8192).ToString())
            ' ShapeDescriptor prints the offset where LongShapeDescriptor prints the extras
            assertEquals("[2,2, 3,3, 1,4,1,c]", New ShapeDescriptor(New Integer() {2, 3}, New Integer() {3, 1}, 4, 1, "c"c, 8192).ToString())
        End Sub

        <TestMethod>
        Public Sub testShapeDescriptorEqualityAndGetters()
            Dim a As New ShapeDescriptor(New Integer() {4, 5}, New Integer() {1, 4}, 2, 1, "f"c, FloatExtras)
            Dim b As New ShapeDescriptor(New Integer() {4, 5}, New Integer() {1, 4}, 2, 1, "f"c, FloatExtras)
            assertEquals(a, b)
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertFalse(a.Equals(New ShapeDescriptor(New Integer() {4, 5}, New Integer() {1, 4}, 2, 1, "c"c, FloatExtras)))
            assertFalse(a.Equals(New ShapeDescriptor(New Integer() {4, 5}, New Integer() {1, 4}, 0, 1, "f"c, FloatExtras)))
            assertArrayEquals(New Integer() {4, 5}, a.getShape())
            assertArrayEquals(New Integer() {1, 4}, a.getStride())
            assertEquals(2L, a.getOffset())
            assertEquals(1, a.getEws())
            assertEquals("f"c, a.getOrder())
            assertEquals(FloatExtras, a.getExtras())

            ' a ShapeDescriptor never equals a LongShapeDescriptor with the same values
            Dim l As LongShapeDescriptor = LongShapeDescriptor.fromShapeDescriptor(a)
            assertFalse(a.Equals(l))
            assertArrayEquals(New Long() {4, 5}, l.getShape())
            assertArrayEquals(New Long() {1, 4}, l.getStride())
            assertEquals("f"c, l.getOrder())
            assertEquals(FloatExtras, l.getExtras())
            assertEquals(New LongShapeDescriptor(New Long() {4, 5}, New Long() {1, 4}, 2, 1, "f"c, FloatExtras), l)
            assertThrows(Of ArgumentNullException)(Sub() LongShapeDescriptor.fromShapeDescriptor(Nothing))
        End Sub

        <TestMethod>
        Public Sub testFromShapeUsesCOrderStrides()
            Dim d As LongShapeDescriptor = LongShapeDescriptor.fromShape(New Long() {2, 3, 4}, DataType.DOUBLE)
            assertArrayEquals(New Long() {12, 4, 1}, d.getStride())
            assertEquals("c"c, d.getOrder())
            assertEquals(3, d.rank())
            assertEquals(DataType.DOUBLE, d.dataType())
            assertFalse(d.isEmpty())

            Dim fromInts As LongShapeDescriptor = LongShapeDescriptor.fromShape(New Integer() {2, 3, 4}, DataType.DOUBLE)
            assertEquals(d, fromInts)

            Dim withExtras As LongShapeDescriptor = LongShapeDescriptor.fromShape(New Long() {5, 2}, ArrayOptionsHelper.DTYPE_INT_BIT)
            assertArrayEquals(New Long() {2, 1}, withExtras.getStride())
            assertEquals(DataType.INT, withExtras.dataType())

            Dim f As LongShapeDescriptor = LongShapeDescriptor.fromShape(New Long() {2, 3}, New Long() {1, 2}, 1, "f"c, DataType.HALF, False)
            assertEquals("f"c, f.getOrder())
            assertEquals(DataType.HALF, f.dataType())
            assertThrows(Of ArgumentNullException)(Sub() LongShapeDescriptor.fromShape(Nothing, New Long() {}, 1, "c"c, DataType.FLOAT, False))
            assertThrows(Of ArgumentNullException)(Sub() LongShapeDescriptor.fromShape(New Long() {}, Nothing, 1, "c"c, DataType.FLOAT, False))
        End Sub

        <TestMethod>
        Public Sub testEmptyAndAsDataType()
            Dim e As LongShapeDescriptor = LongShapeDescriptor.empty(DataType.FLOAT)
            assertTrue(e.isEmpty())
            assertEquals(0, e.rank())
            assertEquals(DataType.FLOAT, e.dataType())

            Dim asInt As LongShapeDescriptor = e.asDataType(DataType.INT)
            assertTrue(asInt.isEmpty())
            assertEquals(DataType.INT, asInt.dataType())
            assertFalse(e.Equals(asInt))

            Dim d As LongShapeDescriptor = LongShapeDescriptor.fromShape(New Long() {3}, DataType.FLOAT)
            Dim asLong As LongShapeDescriptor = d.asDataType(DataType.LONG)
            assertFalse(asLong.isEmpty())
            assertEquals(DataType.LONG, asLong.dataType())
            assertArrayEquals(d.getShape(), asLong.getShape())

            d.setExtras(ArrayOptionsHelper.DTYPE_BOOL_BIT)
            assertEquals(DataType.BOOL, d.dataType())
        End Sub

        <TestMethod>
        Public Sub testDataTypeBitsRoundTripForEveryDataType()
            For Each dt As DataType In DataTypeExtensions.DataTypeValues()
                If dt = DataType.UNKNOWN Then
                    assertThrows(Of NotSupportedException)(Sub() ArrayOptionsHelper.setOptionBit(0L, dt))
                    Continue For
                End If
                Dim extras As Long = ArrayOptionsHelper.setOptionBit(0L, dt)
                assertEquals(dt, ArrayOptionsHelper.dataType(extras), dt.name())
                ' the array type bits do not disturb the data type
                assertEquals(dt, ArrayOptionsHelper.dataType(ArrayOptionsHelper.setOptionBit(extras, ArrayType.EMPTY)), dt.name())
            Next dt
            assertEquals(ArrayOptionsHelper.DTYPE_UNSIGNED_BIT Or ArrayOptionsHelper.DTYPE_BYTE_BIT, ArrayOptionsHelper.setOptionBit(0L, DataType.UBYTE))
            assertEquals(ArrayOptionsHelper.DTYPE_BYTE_BIT, ArrayOptionsHelper.setOptionBit(0L, DataType.BYTE))
            Dim ex = assertThrows(Of ND4VBUnknownDataTypeException)(Sub() ArrayOptionsHelper.dataType(0L))
            assertEquals("Unknown extras set: [0]", ex.Message)
        End Sub

        <TestMethod>
        Public Sub testArrayTypeBits()
            ' shape information of a 2x3 float array: [rank, shape, stride, extras, ews, order]
            Dim info As Long() = {2, 2, 3, 3, 1, ArrayOptionsHelper.DTYPE_FLOAT_BIT, 1, AscW("c"c)}
            assertEquals(ArrayType.DENSE, ArrayOptionsHelper.arrayType(info))
            assertEquals(DataType.FLOAT, ArrayOptionsHelper.dataType(info))
            ArrayOptionsHelper.setOptionBit(info, ArrayType.EMPTY)
            assertEquals(ArrayType.EMPTY, ArrayOptionsHelper.arrayType(info))
            assertTrue(ArrayOptionsHelper.hasBitSet(info, ArrayOptionsHelper.ATYPE_EMPTY_BIT))
            assertEquals(ArrayOptionsHelper.DTYPE_FLOAT_BIT Or ArrayOptionsHelper.ATYPE_EMPTY_BIT, info(5))

            assertEquals(ArrayType.SPARSE, ArrayOptionsHelper.arrayType(New Long() {0, ArrayOptionsHelper.ATYPE_SPARSE_BIT, 1, AscW("c"c)}))
            assertEquals(ArrayType.COMPRESSED, ArrayOptionsHelper.arrayType(New Long() {0, ArrayOptionsHelper.ATYPE_COMPRESSED_BIT, 1, AscW("c"c)}))
            ' DENSE sets no bit
            assertEquals(5L, ArrayOptionsHelper.setOptionBit(5L, ArrayType.DENSE))
            assertTrue(ArrayOptionsHelper.hasBitSet(12L, 4L))
            assertFalse(ArrayOptionsHelper.hasBitSet(12L, 3L))
        End Sub

        <TestMethod>
        Public Sub testDataTypeFromName()
            assertEquals(DataType.FLOAT, ArrayOptionsHelper.dataType("float32"))
            assertEquals(DataType.FLOAT, ArrayOptionsHelper.dataType("resource"))
            assertEquals(DataType.DOUBLE, ArrayOptionsHelper.dataType("double"))
            assertEquals(DataType.LONG, ArrayOptionsHelper.dataType("int64"))
            assertEquals(DataType.UBYTE, ArrayOptionsHelper.dataType("uint8"))
            assertEquals(DataType.HALF, ArrayOptionsHelper.dataType("float16"))
            assertEquals(DataType.UTF8, ArrayOptionsHelper.dataType("string"))
            Dim ex = assertThrows(Of ND4VBIllegalStateException)(Sub() ArrayOptionsHelper.dataType("complex64"))
            assertEquals("Unknown data type used: [complex64]", ex.Message)
        End Sub

        <TestMethod>
        Public Sub testTadPack()
            Dim p As TadPack = TadPack.builder().build()
            assertNull(p.getTadShapeInfo())
            assertNull(p.getTadOffsets())
            assertEquals(p, New TadPack())
            assertEquals(p.GetHashCode(), New TadPack().GetHashCode())
        End Sub

    End Class

End Namespace
