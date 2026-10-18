Imports System

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

Namespace org.nd4vb.linalg.util

    ''' <summary>
    ''' A 64-bit hash over bytes or strings (a table-driven multiplicative hash). The results are bit-identical to Java's:
    ''' Java <c>byte[]</c> is <c>SByte()</c>, and a string is hashed by the low and high byte of each UTF-16 char.
    ''' </summary>
    ''' @author raver119@gmail.com
    Public Class HashUtil

        Private Shared ReadOnly byteTable() As Long = createLookupTable()
        Private Const HSTART As Long = &HBB40E64DA205B064L
        Private Const HMULT As Long = 7664345821815920749L

        Private Shared Function createLookupTable() As Long()
            Dim table(255) As Long
            Dim h As Long = &H544B2FBACAAF1684L
            For i As Integer = 0 To 255
                For j As Integer = 0 To 30
                    h = CLng(CULng(h) >> 7) Xor h
                    h = (h << 11) Xor h
                    h = CLng(CULng(h) >> 10) Xor h
                Next j
                table(i) = h
            Next i
            Return table
        End Function

        ''' <summary>The 64-bit hash of a byte array.</summary>
        Public Shared Function getLongHash(ByVal data() As SByte) As Long
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If
            Dim h As Long = HSTART
            Dim ht() As Long = byteTable
            Dim len As Integer = data.Length
            For i As Integer = 0 To len - 1
                h = (h * HMULT) Xor ht(data(i) And &HFF)
            Next i
            Return h
        End Function

        ''' <summary>The 64-bit hash of a string (two table lookups per UTF-16 char).</summary>
        Public Shared Function getLongHash(ByVal [string] As String) As Long
            If [string] Is Nothing Then
                Throw New ArgumentNullException(NameOf([string]))
            End If
            Dim h As Long = HSTART
            Dim ht() As Long = byteTable
            Dim len As Integer = [string].Length
            For i As Integer = 0 To len - 1
                Dim ch As Integer = AscW([string].Chars(i))
                h = (h * HMULT) Xor ht(ch And &HFF)
                h = (h * HMULT) Xor ht((ch >> 8) And &HFF)
            Next i
            Return h
        End Function

    End Class

End Namespace
