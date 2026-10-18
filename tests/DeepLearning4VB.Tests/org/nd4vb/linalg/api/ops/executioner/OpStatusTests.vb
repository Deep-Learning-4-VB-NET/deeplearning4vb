Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops.executioner
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

Namespace org.nd4vb.linalg.api.ops.executioner

    ''' <summary>Tests for the <c>OpStatus</c> enum helpers (Java <c>byNumber</c>, <c>values</c>, <c>valueOf</c>).</summary>
    <TestClass>
    Public Class OpStatusTests

        <TestMethod>
        Public Sub testByNumberMapsTheLibnd4jCodes()
            Dim expected As New Dictionary(Of Integer, OpStatus) From {
                {0, OpStatus.ND4VB_STATUS_OK},
                {1, OpStatus.ND4VB_STATUS_BAD_INPUT},
                {2, OpStatus.ND4VB_STATUS_BAD_SHAPE},
                {3, OpStatus.ND4VB_STATUS_BAD_RANK},
                {4, OpStatus.ND4VB_STATUS_BAD_PARAMS},
                {5, OpStatus.ND4VB_STATUS_BAD_OUTPUT},
                {6, OpStatus.ND4VB_STATUS_BAD_RNG},
                {7, OpStatus.ND4VB_STATUS_BAD_EPSILON},
                {8, OpStatus.ND4VB_STATUS_BAD_GRADIENTS},
                {9, OpStatus.ND4VB_STATUS_BAD_BIAS},
                {20, OpStatus.ND4VB_STATUS_VALIDATION},
                {30, OpStatus.ND4VB_STATUS_BAD_GRAPH},
                {31, OpStatus.ND4VB_STATUS_BAD_LENGTH},
                {32, OpStatus.ND4VB_STATUS_BAD_DIMENSIONS},
                {33, OpStatus.ND4VB_STATUS_BAD_ORDER},
                {34, OpStatus.ND4VB_STATUS_BAD_ARGUMENTS}
            }
            For Each kv As KeyValuePair(Of Integer, OpStatus) In expected
                assertEquals(kv.Value, OpStatusExtensions.OpStatusByNumber(kv.Key), kv.Key.ToString())
            Next kv
            ' every constant has a number
            assertEquals(OpStatusExtensions.OpStatusValues().Length, expected.Count)
            For Each bad As Integer In New Integer() {-1, 10, 19, 21, 35, 100}
                Dim n As Integer = bad
                Dim ex = assertThrows(Of ND4VBIllegalStateException)(Sub() OpStatusExtensions.OpStatusByNumber(n))
                assertEquals("Unknown status given: " & n, ex.Message)
            Next bad
        End Sub

        <TestMethod>
        Public Sub testValuesAreInJavaOrder()
            Dim values As OpStatus() = OpStatusExtensions.OpStatusValues()
            assertEquals(16, values.Length)
            assertEquals(OpStatus.ND4VB_STATUS_OK, values(0))
            assertEquals(OpStatus.ND4VB_STATUS_BAD_GRAPH, values(10))
            assertEquals(OpStatus.ND4VB_STATUS_VALIDATION, values(15))
            For i As Integer = 0 To values.Length - 1
                assertEquals(i, CInt(values(i)))
            Next i
        End Sub

        <TestMethod>
        Public Sub testValueOf()
            For Each s As OpStatus In OpStatusExtensions.OpStatusValues()
                assertEquals(s, OpStatusExtensions.OpStatusValueOf(s.ToString()))
            Next s
            For Each bad As String In New String() {"0", "3", "nd4vb_status_ok", "ND4J_STATUS_OK", "", "UNKNOWN"}
                Dim name As String = bad
                assertThrows(Of ArgumentException)(Sub() OpStatusExtensions.OpStatusValueOf(name))
            Next bad
            assertThrows(Of ArgumentException)(Sub() OpStatusExtensions.OpStatusValueOf(Nothing))
        End Sub

    End Class

End Namespace
