Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer

'
' *  ******************************************************************************
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.buffer

    ''' <summary>Tests for the <c>DataType</c> enum helpers (Java's enum methods), for every data type.</summary>
    <TestClass>
    Public Class DataTypeTests

        ' name, ordinal, toInt (-1: throws), width, precision, isFPType, isIntType, isNumerical, isSigned
        Private Shared ReadOnly Expected As Object()() = {
            New Object() {DataType.DOUBLE, "DOUBLE", 0, 6, 8, 17, True, False, True, True},
            New Object() {DataType.FLOAT, "FLOAT", 1, 5, 4, 9, True, False, True, True},
            New Object() {DataType.HALF, "HALF", 2, 3, 2, 5, True, False, True, True},
            New Object() {DataType.LONG, "LONG", 3, 10, 8, -1, False, True, True, True},
            New Object() {DataType.INT, "INT", 4, 9, 4, -1, False, True, True, True},
            New Object() {DataType.SHORT, "SHORT", 5, 8, 2, -1, False, True, True, True},
            New Object() {DataType.UBYTE, "UBYTE", 6, 11, 1, -1, False, True, True, False},
            New Object() {DataType.BYTE, "BYTE", 7, 7, 1, -1, False, True, True, True},
            New Object() {DataType.BOOL, "BOOL", 8, 1, 1, -1, False, False, False, False},
            New Object() {DataType.UTF8, "UTF8", 9, 50, -1, -1, False, False, False, False},
            New Object() {DataType.COMPRESSED, "COMPRESSED", 10, -1, -1, -1, False, False, False, False},
            New Object() {DataType.BFLOAT16, "BFLOAT16", 11, 17, 2, 4, True, False, True, True},
            New Object() {DataType.UINT16, "UINT16", 12, 12, 2, -1, False, True, True, False},
            New Object() {DataType.UINT32, "UINT32", 13, 13, 4, -1, False, True, True, False},
            New Object() {DataType.UINT64, "UINT64", 14, 14, 8, -1, False, True, True, False},
            New Object() {DataType.UNKNOWN, "UNKNOWN", 15, -1, -1, -1, False, False, False, False}
        }

        <TestMethod>
        Public Sub testHelpersForEveryDataType()
            assertEquals(16, Expected.Length)
            For Each row As Object() In Expected
                Dim dt As DataType = CType(row(0), DataType)
                Dim label As String = CStr(row(1))
                assertEquals(label, dt.name())
                assertEquals(CInt(row(2)), dt.ordinal(), label)
                If CInt(row(3)) = -1 Then
                    Dim ex = assertThrows(Of NotSupportedException)(Sub() dt.toInt())
                    assertEquals("Non-covered data type: [" & label & "]", ex.Message)
                Else
                    assertEquals(CInt(row(3)), dt.toInt(), label)
                End If
                assertEquals(CInt(row(4)), dt.width(), label)
                assertEquals(CInt(row(5)), dt.precision(), label)
                assertEquals(CBool(row(6)), dt.isFPType(), label)
                assertEquals(CBool(row(7)), dt.isIntType(), label)
                assertEquals(CBool(row(8)), dt.isNumerical(), label)
                assertEquals(CBool(row(9)), dt.isSigned(), label)
                assertEquals(dt, DataTypeValueOf(label))
            Next
        End Sub

        <TestMethod>
        Public Sub testAliasesShareValues()
            assertEquals(DataType.HALF, DataType.FLOAT16)
            assertEquals(DataType.INT, DataType.INT32)
            assertEquals(DataType.LONG, DataType.INT64)
            assertEquals(DataType.SHORT, DataType.INT16)
            assertEquals(DataType.BYTE, DataType.INT8)
            assertEquals(DataType.UBYTE, DataType.UINT8)
            ' the Java name is the canonical one, whatever ToString returns
            assertEquals("INT", DataType.INT32.name())
            assertEquals("UBYTE", DataType.UINT8.name())
            assertEquals("HALF", DataType.FLOAT16.name())
            assertEquals(DataType.INT64, DataTypeValueOf("INT64"))
            assertEquals(DataType.UBYTE, DataTypeValueOf("UINT8"))
            ' ToString always parses back
            For Each dt As DataType In [Enum].GetValues(GetType(DataType))
                assertEquals(dt, DataTypeValueOf(dt.ToString()))
            Next
        End Sub

        <TestMethod>
        Public Sub testValues()
            Dim values As DataType() = DataTypeValues()
            assertEquals(16, values.Length)
            For i As Integer = 0 To values.Length - 1
                assertEquals(i, values(i).ordinal())
            Next
            assertEquals(DataType.DOUBLE, values(0))
            assertEquals(DataType.UNKNOWN, values(15))
        End Sub

        <TestMethod>
        Public Sub testValueOfRejectsUnknownAndNumericNames()
            assertEquals("No enum constant DataType.float", assertThrows(Of ArgumentException)(Sub() DataTypeValueOf("float")).Message)
            assertThrows(Of ArgumentException)(Sub() DataTypeValueOf("1"))
            assertThrows(Of ArgumentException)(Sub() DataTypeValueOf(""))
            assertThrows(Of ArgumentNullException)(Sub() DataTypeValueOf(Nothing))
        End Sub

        <TestMethod>
        Public Sub testFromIntRoundTrip()
            For Each dt As DataType In DataTypeValues()
                If dt = DataType.COMPRESSED OrElse dt = DataType.UNKNOWN OrElse dt = DataType.UTF8 Then Continue For
                assertEquals(dt, DataTypeFromInt(dt.toInt()))
            Next
            ' the extra libnd4j codes
            assertEquals(DataType.FLOAT, DataTypeFromInt(2))
            assertEquals(DataType.HALF, DataTypeFromInt(4))
            assertEquals(DataType.UNKNOWN, DataTypeFromInt(100))
            assertEquals("Unknown data type: [50]", assertThrows(Of NotSupportedException)(Sub() DataTypeFromInt(50)).Message)
            assertThrows(Of NotSupportedException)(Sub() DataTypeFromInt(0))
        End Sub

        <TestMethod>
        Public Sub testFromNumpy()
            assertEquals(DataType.BOOL, DataTypeFromNumpy("bool"))
            assertEquals(DataType.INT8, DataTypeFromNumpy("byte"))
            assertEquals(DataType.INT8, DataTypeFromNumpy("int8"))
            assertEquals(DataType.INT16, DataTypeFromNumpy("INT16"))
            assertEquals(DataType.INT32, DataTypeFromNumpy("int32"))
            assertEquals(DataType.INT64, DataTypeFromNumpy("int64"))
            assertEquals(DataType.UINT8, DataTypeFromNumpy("uint8"))
            assertEquals(DataType.UINT16, DataTypeFromNumpy("uint16"))
            assertEquals(DataType.UINT32, DataTypeFromNumpy("uint32"))
            assertEquals(DataType.UINT64, DataTypeFromNumpy("uint64"))
            assertEquals(DataType.FLOAT16, DataTypeFromNumpy("float16"))
            assertEquals(DataType.FLOAT, DataTypeFromNumpy("float32"))
            assertEquals(DataType.DOUBLE, DataTypeFromNumpy("Float64"))
            assertEquals("Unknown datatype or no ND4VB equivalent datatype exists: complex64", assertThrows(Of InvalidOperationException)(Sub() DataTypeFromNumpy("complex64")).Message)
        End Sub

        <TestMethod>
        Public Sub testDataTypeEx()
            assertEquals(10, [Enum].GetValues(GetType(DataTypeEx)).Length)
            assertEquals(0, CInt(DataTypeEx.FLOAT8))
            assertEquals(9, CInt(DataTypeEx.FTHRESHOLD))
        End Sub

    End Class

End Namespace
