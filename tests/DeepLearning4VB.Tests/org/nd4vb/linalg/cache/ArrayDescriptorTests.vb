Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.cache
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports CustomOpDescriptor = DeepLearning4VB.org.nd4vb.linalg.api.ops.CustomOpDescriptor

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.cache

    ''' <summary>
    ''' Tests for the cache keys (<c>ArrayDescriptor</c>) and the custom op descriptor, which are compared by value.
    ''' </summary>
    <TestClass>
    Public Class ArrayDescriptorTests

        <TestMethod>
        Public Sub testEqualityByContentAndType()
            assertEquals(New ArrayDescriptor(New Integer() {1, 2, 3}, DataType.INT), New ArrayDescriptor(New Integer() {1, 2, 3}, DataType.INT))
            assertEquals(New ArrayDescriptor(New Integer() {1, 2, 3}, DataType.INT).GetHashCode(), New ArrayDescriptor(New Integer() {1, 2, 3}, DataType.INT).GetHashCode())
            assertNotEquals(New ArrayDescriptor(New Integer() {1, 2, 3}, DataType.INT), New ArrayDescriptor(New Integer() {1, 2, 4}, DataType.INT))
            assertNotEquals(New ArrayDescriptor(New Integer() {1, 2, 3}, DataType.INT), New ArrayDescriptor(New Integer() {1, 2, 3}, DataType.LONG))

            assertEquals(New ArrayDescriptor(New Single() {1.5F}, DataType.FLOAT), New ArrayDescriptor(New Single() {1.5F}, DataType.FLOAT))
            assertEquals(New ArrayDescriptor(New Double() {1.5, 2}, DataType.DOUBLE), New ArrayDescriptor(New Double() {1.5, 2}, DataType.DOUBLE))
            assertEquals(New ArrayDescriptor(New Long() {7}, DataType.LONG), New ArrayDescriptor(New Long() {7}, DataType.LONG))
            ' different element types never match, even with equal values
            assertNotEquals(New ArrayDescriptor(New Long() {7}, DataType.LONG), New ArrayDescriptor(New Integer() {7}, DataType.LONG))
            assertFalse(New ArrayDescriptor(New Long() {7}, DataType.LONG).Equals(Nothing))
        End Sub

        <TestMethod>
        Public Sub testBooleanDescriptorsCompareTheirBooleans()
            ' Java compared the (null) integer arrays here and threw; the port compares the booleans
            Dim a As New ArrayDescriptor(New Boolean() {True, False}, DataType.BOOL)
            assertEquals(a, New ArrayDescriptor(New Boolean() {True, False}, DataType.BOOL))
            assertEquals(a.GetHashCode(), New ArrayDescriptor(New Boolean() {True, False}, DataType.BOOL).GetHashCode())
            assertNotEquals(a, New ArrayDescriptor(New Boolean() {True, True}, DataType.BOOL))
        End Sub

        <TestMethod>
        Public Sub testCustomOpDescriptorBuilder()
            Dim d As CustomOpDescriptor = CustomOpDescriptor.builder().hash(123456789012L).numInputs(2).numOutputs(1).allowsInplace(True).numTArgs(3).numIArgs(4).build()
            assertEquals(123456789012L, d.getHash())
            assertEquals(2, d.getNumInputs())
            assertEquals(1, d.getNumOutputs())
            assertTrue(d.isAllowsInplace())
            assertEquals(3, d.getNumTArgs())
            assertEquals(4, d.getNumIArgs())
            Dim empty As CustomOpDescriptor = CustomOpDescriptor.builder().build()
            assertEquals(0L, empty.getHash())
            assertFalse(empty.isAllowsInplace())
        End Sub

    End Class

End Namespace
