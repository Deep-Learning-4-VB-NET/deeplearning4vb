Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.blas.params

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.blas.params

    ''' <summary>
    ''' Tests for <c>MMulTranspose</c>'s Lombok members and property map (<c>exec</c> needs arrays and is tested with
    ''' the backend).
    ''' </summary>
    <TestClass>
    Public Class MMulTransposeTests

        <TestMethod>
        Public Sub testBuilderAndAllFalse()
            Dim none As MMulTranspose = MMulTranspose.allFalse()
            assertFalse(none.isTransposeA())
            assertFalse(none.isTransposeB())
            assertFalse(none.isTransposeResult())
            Assert.AreSame(none, MMulTranspose.allFalse())
            assertEquals(none, MMulTranspose.builder().build())

            Dim t As MMulTranspose = MMulTranspose.builder().transposeA(True).transposeResult(True).build()
            assertTrue(t.isTransposeA())
            assertFalse(t.isTransposeB())
            assertTrue(t.isTransposeResult())
            assertEquals("MMulTranspose(transposeA=true, transposeB=false, transposeResult=true)", t.ToString())
        End Sub

        <TestMethod>
        Public Sub testEqualsAndLombokHashCode()
            Dim a As New MMulTranspose(True, False, True)
            Dim b As New MMulTranspose(True, False, True)
            assertEquals(a, b)
            ' Lombok: ((59 + 79) * 59 + 97) * 59 + 79
            assertEquals(((59 + 79) * 59 + 97) * 59 + 79, a.GetHashCode())
            assertEquals(a.GetHashCode(), b.GetHashCode())
            assertNotEquals(a, New MMulTranspose(True, True, True))
        End Sub

        <TestMethod>
        Public Sub testPropertiesRoundTrip()
            Dim a As New MMulTranspose(False, True, False)
            Dim props As IDictionary(Of String, Object) = a.toProperties()
            assertEquals(3, props.Count)
            assertEquals(True, props("transposeB"))
            Dim b As New MMulTranspose(False, False, False)
            b.setProperties(props)
            assertEquals(a, b)
            ' missing keys are left alone
            b.setProperties(New Dictionary(Of String, Object) From {{"transposeA", True}})
            assertTrue(b.isTransposeA())
            assertTrue(b.isTransposeB())
            assertEquals(False, a.getValue(GetType(MMulTranspose).GetField("transposeA", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)))
        End Sub

    End Class

End Namespace
