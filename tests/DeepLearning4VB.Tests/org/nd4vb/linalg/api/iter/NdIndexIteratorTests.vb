Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.iter

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.iter

    ''' <summary>
    ''' Tests for the shape iterators that need no arrays: <c>NdIndexIterator</c> (C and Fortran order, with and
    ''' without the shared cache), <c>LinearIndexLookup</c> and Java's <c>FlatIterator</c>.
    ''' </summary>
    <TestClass>
    Public Class NdIndexIteratorTests

        Private Shared Function drain(ByVal it As NdIndexIterator) As List(Of Long())
            Dim result As New List(Of Long())()
            Do While it.hasNext()
                result.Add(DirectCast(it.next().Clone(), Long()))
            Loop
            Return result
        End Function

        <TestMethod>
        Public Sub testCOrder()
            Dim all As List(Of Long()) = drain(New NdIndexIterator(2, 3))
            assertEquals(6, all.Count)
            assertArrayEquals(New Long() {0, 0}, all(0))
            assertArrayEquals(New Long() {0, 1}, all(1))
            assertArrayEquals(New Long() {0, 2}, all(2))
            assertArrayEquals(New Long() {1, 0}, all(3))
            assertArrayEquals(New Long() {1, 2}, all(5))
        End Sub

        <TestMethod>
        Public Sub testFortranOrder()
            Dim all As List(Of Long()) = drain(New NdIndexIterator("f"c, 2, 3))
            assertEquals(6, all.Count)
            assertArrayEquals(New Long() {0, 0}, all(0))
            assertArrayEquals(New Long() {1, 0}, all(1))
            assertArrayEquals(New Long() {0, 1}, all(2))
            assertArrayEquals(New Long() {1, 2}, all(5))
        End Sub

        <TestMethod>
        Public Sub testCachedMatchesUncached()
            For Each order As Char In New Char() {"c"c, "f"c}
                Dim plain As List(Of Long()) = drain(New NdIndexIterator(order, False, 3, 1, 4))
                Dim cached As List(Of Long()) = drain(New NdIndexIterator(order, True, 3, 1, 4))
                Dim cachedAgain As List(Of Long()) = drain(New NdIndexIterator(order, True, 3, 1, 4))
                assertEquals(12, plain.Count)
                For i As Integer = 0 To plain.Count - 1
                    assertArrayEquals(plain(i), cached(i))
                    assertArrayEquals(plain(i), cachedAgain(i))
                Next i
            Next order
        End Sub

        <TestMethod>
        Public Sub testEnumeratorAndShapes()
            Dim it As New NdIndexIterator(New Long() {2, 2})
            Dim count As Integer = 0
            Do While it.MoveNext()
                assertEquals(2, it.Current.Length)
                count += 1
            Loop
            assertEquals(4, count)
            assertFalse(it.MoveNext())
            it.Reset()
            assertTrue(it.MoveNext())
            assertArrayEquals(New Long() {0, 0}, it.Current)

            ' like Java, an empty shape has no subscripts (ArrayUtil.prodLong of an empty shape is 0), and neither
            ' has a shape with a zero
            assertEquals(0, drain(New NdIndexIterator(New Long() {})).Count)
            assertEquals(0, drain(New NdIndexIterator(3, 0)).Count)

            assertThrows(Of ArgumentException)(Sub() Call New NdIndexIterator("x"c, 2, 2).next())
        End Sub

        <TestMethod>
        Public Sub testLinearIndexLookup()
            Dim c As New LinearIndexLookup(New Integer() {2, 3}, "c"c)
            assertArrayEquals(New Long() {1, 1}, c.lookup(4))
            ' cached: the same array comes back
            Assert.AreSame(c.lookup(4), c.lookup(4))
            Dim f As New LinearIndexLookup(New Long() {2, 3}, "f"c)
            assertArrayEquals(New Long() {0, 2}, f.lookup(4))
            assertArrayEquals(New Long() {1, 2}, f.lookup(5))
        End Sub

        <TestMethod>
        Public Sub testFlatIteratorKeepsJavasBehaviour()
            ' Java's FlatIterator counts prod(shape) steps but only advances the first coordinate
            Dim it As New FlatIterator(New Integer() {3})
            Dim seen As New List(Of Integer())()
            Do While it.hasNext()
                seen.Add(DirectCast(it.next().Clone(), Integer()))
            Loop
            assertEquals(3, seen.Count)
            assertArrayEquals(New Integer() {1}, seen(0))
            assertArrayEquals(New Integer() {3}, seen(2))
            assertThrows(Of NotSupportedException)(Sub() it.Reset())

            ' past the end of the first dimension it fails like Java (ArrayIndexOutOfBoundsException there)
            Dim it2 As New FlatIterator(New Integer() {2, 2})
            assertArrayEquals(New Integer() {1, 0}, it2.next())
            assertArrayEquals(New Integer() {2, 0}, it2.next())
            assertThrows(Of IndexOutOfRangeException)(Sub() it2.next())
        End Sub

    End Class

End Namespace
