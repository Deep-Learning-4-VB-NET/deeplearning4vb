Imports System
Imports System.Runtime.CompilerServices

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Namespace org.nd4vb.linalg.api.buffer

    ''' <summary>
    ''' The data types an array can hold. The values are the Java ordinals. The Java instance methods
    ''' (<c>width()</c>, <c>isFPType()</c>, <c>toInt()</c>, ...) are the extension methods in
    ''' <see cref="DataTypeExtensions"/>, the static ones are <see cref="DataTypeExtensions.DataTypeFromInt"/>,
    ''' <see cref="DataTypeExtensions.DataTypeFromNumpy"/>, <see cref="DataTypeExtensions.DataTypeValues"/> and
    ''' <see cref="DataTypeExtensions.DataTypeValueOf"/>.
    ''' <para>
    ''' <c>FLOAT16</c>, <c>INT32</c>, <c>INT64</c>, <c>INT16</c>, <c>INT8</c> and <c>UINT8</c> are aliases of
    ''' <c>HALF</c>, <c>INT</c>, <c>LONG</c>, <c>SHORT</c>, <c>BYTE</c> and <c>UBYTE</c> (the same values, as in
    ''' Java, where they are static constants). Java deprecated the old names; they are not marked obsolete here
    ''' because the ported code uses them throughout. Because the aliases share values, <c>ToString()</c> may
    ''' return either name depending on the runtime: use <see cref="DataTypeExtensions.name"/> for the Java name.
    ''' </para>
    ''' </summary>
    Public Enum DataType
        [DOUBLE] = 0
        FLOAT = 1
        ''' <summary>16-bit IEEE floating point (Java deprecated this name in favour of <see cref="FLOAT16"/>).</summary>
        HALF = 2
        ''' <summary>64-bit signed integer (Java deprecated this name in favour of <see cref="INT64"/>).</summary>
        [LONG] = 3
        ''' <summary>32-bit signed integer (Java deprecated this name in favour of <see cref="INT32"/>).</summary>
        INT = 4
        ''' <summary>16-bit signed integer (Java deprecated this name in favour of <see cref="INT16"/>).</summary>
        [SHORT] = 5
        ''' <summary>8-bit unsigned integer (Java deprecated this name in favour of <see cref="UINT8"/>).</summary>
        UBYTE = 6
        ''' <summary>8-bit signed integer (Java deprecated this name in favour of <see cref="INT8"/>).</summary>
        [BYTE] = 7
        BOOL = 8
        UTF8 = 9
        COMPRESSED = 10
        BFLOAT16 = 11
        UINT16 = 12
        UINT32 = 13
        UINT64 = 14
        UNKNOWN = 15

        ''' <summary>Alias of <see cref="HALF"/>.</summary>
        FLOAT16 = HALF
        ''' <summary>Alias of <see cref="INT"/>.</summary>
        INT32 = INT
        ''' <summary>Alias of <c>LONG</c>.</summary>
        INT64 = [LONG]
        ''' <summary>Alias of <c>SHORT</c>.</summary>
        INT16 = [SHORT]
        ''' <summary>Alias of <c>BYTE</c>.</summary>
        INT8 = [BYTE]
        ''' <summary>Alias of <see cref="UBYTE"/>.</summary>
        UINT8 = UBYTE
    End Enum

    ''' <summary>The Java instance and static methods of the <c>DataType</c> enum.</summary>
    Public Module DataTypeExtensions

        Private ReadOnly Names As String() = {"DOUBLE", "FLOAT", "HALF", "LONG", "INT", "SHORT", "UBYTE", "BYTE", "BOOL", "UTF8", "COMPRESSED", "BFLOAT16", "UINT16", "UINT32", "UINT64", "UNKNOWN"}
        Private ReadOnly AliasNames As String() = {"FLOAT16", "INT32", "INT64", "INT16", "INT8", "UINT8"}
        Private ReadOnly AliasValues As DataType() = {DataType.HALF, DataType.INT, DataType.LONG, DataType.SHORT, DataType.BYTE, DataType.UBYTE}

        ''' <summary>
        ''' Values inherited from https://github.com/eclipse/deeplearning4j/blob/master/libnd4j/include/array/DataType.h
        ''' (Java <c>DataType.fromInt</c>).
        ''' </summary>
        ''' <param name="type"> the input int type </param>
        ''' <returns> the appropriate data type </returns>
        Public Function DataTypeFromInt(ByVal type As Integer) As DataType
            Select Case type
                Case 1
                    Return DataType.BOOL
                Case 2
                    Return DataType.FLOAT
                Case 3
                    Return DataType.HALF
                Case 4
                    Return DataType.HALF
                Case 5
                    Return DataType.FLOAT
                Case 6
                    Return DataType.DOUBLE
                Case 7
                    Return DataType.BYTE
                Case 8
                    Return DataType.SHORT
                Case 9
                    Return DataType.INT
                Case 10
                    Return DataType.LONG
                Case 11
                    Return DataType.UBYTE
                Case 12
                    Return DataType.UINT16
                Case 13
                    Return DataType.UINT32
                Case 14
                    Return DataType.UINT64
                Case 17
                    Return DataType.BFLOAT16
                Case 100
                    Return DataType.UNKNOWN
                Case Else
                    Throw New NotSupportedException("Unknown data type: [" & type & "]")
            End Select
        End Function

        ''' <summary>The libnd4j type number of this data type (Java <c>toInt()</c>).</summary>
        <Extension>
        Public Function toInt(ByVal dataType As DataType) As Integer
            Select Case dataType
                Case DataType.BOOL
                    Return 1
                Case DataType.HALF
                    Return 3
                Case DataType.FLOAT
                    Return 5
                Case DataType.DOUBLE
                    Return 6
                Case DataType.BYTE
                    Return 7
                Case DataType.SHORT
                    Return 8
                Case DataType.INT
                    Return 9
                Case DataType.LONG
                    Return 10
                Case DataType.UBYTE
                    Return 11
                Case DataType.UINT16
                    Return 12
                Case DataType.UINT32
                    Return 13
                Case DataType.UINT64
                    Return 14
                Case DataType.BFLOAT16
                    Return 17
                Case DataType.UTF8
                    Return 50
                Case Else
                    Throw New NotSupportedException("Non-covered data type: [" & dataType.name() & "]")
            End Select
        End Function

        ''' <returns> True if the datatype is a floating point type (double, float or half precision) </returns>
        <Extension>
        Public Function isFPType(ByVal dataType As DataType) As Boolean
            Return dataType = DataType.FLOAT OrElse dataType = DataType.DOUBLE OrElse dataType = DataType.HALF OrElse dataType = DataType.BFLOAT16
        End Function

        ''' <returns> True if the datatype is an integer type (long, integer, short, ubyte or byte, and the unsigned types) </returns>
        <Extension>
        Public Function isIntType(ByVal dataType As DataType) As Boolean
            Select Case dataType
                Case DataType.LONG, DataType.INT, DataType.SHORT, DataType.UBYTE, DataType.BYTE, DataType.UINT16, DataType.UINT32, DataType.UINT64
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        ''' <summary>
        ''' True if the value is numerical: <c>this != UTF8 &amp;&amp; this != BOOL &amp;&amp; this != COMPRESSED &amp;&amp; this != UNKNOWN</c>
        ''' (Java's comment says booleans are numerical, but its code, kept here, excludes BOOL).
        ''' </summary>
        <Extension>
        Public Function isNumerical(ByVal dataType As DataType) As Boolean
            Return dataType <> DataType.UTF8 AndAlso dataType <> DataType.BOOL AndAlso dataType <> DataType.COMPRESSED AndAlso dataType <> DataType.UNKNOWN
        End Function

        ''' <returns> True if the datatype is a numerical type and is signed (supports negative values) </returns>
        <Extension>
        Public Function isSigned(ByVal dataType As DataType) As Boolean
            Select Case dataType
                Case DataType.DOUBLE, DataType.FLOAT, DataType.HALF, DataType.LONG, DataType.INT, DataType.SHORT, DataType.BYTE, DataType.BFLOAT16
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        ''' <returns> the max number of significant decimal digits (-1 for the non floating point types) </returns>
        <Extension>
        Public Function precision(ByVal dataType As DataType) As Integer
            Select Case dataType
                Case DataType.DOUBLE
                    Return 17
                Case DataType.FLOAT
                    Return 9
                Case DataType.HALF
                    Return 5
                Case DataType.BFLOAT16
                    Return 4
                Case Else
                    Return -1
            End Select
        End Function

        ''' <returns> For fixed-width types, the number of bytes per array element; -1 otherwise </returns>
        <Extension>
        Public Function width(ByVal dataType As DataType) As Integer
            Select Case dataType
                Case DataType.DOUBLE, DataType.LONG, DataType.UINT64
                    Return 8
                Case DataType.FLOAT, DataType.INT, DataType.UINT32
                    Return 4
                Case DataType.HALF, DataType.SHORT, DataType.BFLOAT16, DataType.UINT16
                    Return 2
                Case DataType.UBYTE, DataType.BYTE, DataType.BOOL
                    Return 1
                Case Else
                    Return -1
            End Select
        End Function

        ''' <summary>The data type for a numpy dtype name (Java <c>DataType.fromNumpy</c>).</summary>
        Public Function DataTypeFromNumpy(ByVal numpyDtypeName As String) As DataType
            Select Case numpyDtypeName.ToLowerInvariant()
                Case "bool"
                    Return DataType.BOOL
                Case "byte", "int8"
                    Return DataType.INT8
                Case "int16"
                    Return DataType.INT16
                Case "int32"
                    Return DataType.INT32
                Case "int64"
                    Return DataType.INT64
                Case "uint8"
                    Return DataType.UINT8
                Case "float16"
                    Return DataType.FLOAT16
                Case "float32"
                    Return DataType.FLOAT
                Case "float64"
                    Return DataType.DOUBLE
                Case "uint16"
                    Return DataType.UINT16
                Case "uint32"
                    Return DataType.UINT32
                Case "uint64"
                    Return DataType.UINT64
                Case Else
                    Throw New InvalidOperationException("Unknown datatype or no ND4VB equivalent datatype exists: " & numpyDtypeName)
            End Select
        End Function

        ''' <summary>The Java ordinal of the constant (the enum value).</summary>
        <Extension>
        Public Function ordinal(ByVal dataType As DataType) As Integer
            Return CInt(dataType)
        End Function

        ''' <summary>
        ''' The Java constant name (Java <c>name()</c>/<c>toString()</c>): an alias such as <c>INT32</c> gives its
        ''' canonical name <c>INT</c>, whatever <c>ToString()</c> returns on the current runtime.
        ''' </summary>
        <Extension>
        Public Function name(ByVal dataType As DataType) As String
            Dim i As Integer = CInt(dataType)
            If i >= 0 AndAlso i < Names.Length Then Return Names(i)
            Return i.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function

        ''' <summary>The 16 constants in Java ordinal order (Java <c>DataType.values()</c>; the aliases are not repeated).</summary>
        Public Function DataTypeValues() As DataType()
            Dim result(Names.Length - 1) As DataType
            For i As Integer = 0 To result.Length - 1
                result(i) = CType(i, DataType)
            Next
            Return result
        End Function

        ''' <summary>
        ''' Parses a constant name (Java <c>DataType.valueOf</c>). The alias names (<c>FLOAT16</c>, <c>INT32</c>,
        ''' <c>INT64</c>, <c>INT16</c>, <c>INT8</c>, <c>UINT8</c>) are accepted as well, because <c>ToString()</c> may
        ''' produce them; Java only accepts the canonical names. Matching is case-sensitive like Java.
        ''' </summary>
        Public Function DataTypeValueOf(ByVal name As String) As DataType
            If name Is Nothing Then Throw New ArgumentNullException(NameOf(name), "Name is null")
            Dim i As Integer = Array.IndexOf(Names, name)
            If i >= 0 Then Return CType(i, DataType)
            i = Array.IndexOf(AliasNames, name)
            If i >= 0 Then Return AliasValues(i)
            Throw New ArgumentException("No enum constant DataType." & name)
        End Function

    End Module

End Namespace
