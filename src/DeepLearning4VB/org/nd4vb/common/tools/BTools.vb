Imports System
Imports System.Globalization
Imports System.Text

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

Namespace org.nd4vb.common.tools

    ''' <summary>
    ''' Base ("B") text formatting helpers used by the diagnostic tools.
    ''' </summary>
    Public Class BTools

        ''' <summary>Method Level External Shift String: one dot per level.</summary>
        Public Shared Function getMtLvESS(ByVal mtLv As Integer) As String
            If mtLv < 0 Then
                Return "?"
            End If
            Return New String("."c, mtLv)
        End Function

        ''' <summary>Method Level Intern Shift String.</summary>
        Public Shared ReadOnly Property MtLvISS As String
            Get
                Return " "
            End Get
        End Property

        Public Shared Function getMtLvISS() As String
            Return MtLvISS
        End Function

        Public Shared Function getSpaces(ByVal SpacesCount As Integer) As String
            If SpacesCount < 0 Then
                Return "?"
            End If
            Return New String(" "c, SpacesCount)
        End Function

        ''' <summary>Booleans as a string of T/F characters.</summary>
        Public Shared Function getSBln(ByVal ParamArray blnA() As Boolean) As String
            If blnA Is Nothing OrElse blnA.Length = 0 Then
                Return "?"
            End If
            Dim sb As New StringBuilder(blnA.Length)
            For K As Integer = 0 To blnA.Length - 1
                sb.Append(If(blnA(K), "T"c, "F"c))
            Next K
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Formats a double with the given number of decimals, "." as decimal separator and " " as
        ''' thousands separator, rounding half to even (the Java DecimalFormat default).
        ''' </summary>
        Public Shared Function getSDbl(ByVal Value As Double, ByVal DecPrec As Integer) As String
            If Double.IsNaN(Value) Then
                Return "NaN"
            End If
            If Double.IsPositiveInfinity(Value) Then
                Return "∞"
            End If
            If Double.IsNegativeInfinity(Value) Then
                Return "-∞"
            End If
            If DecPrec < 0 Then
                DecPrec = 0
            End If
            If DecPrec > 100 Then
                DecPrec = 100
            End If
            Dim rounded As Double = Value
            If DecPrec <= 15 Then
                rounded = Math.Round(Value, DecPrec, MidpointRounding.ToEven)
            End If
            Dim nfi As New NumberFormatInfo()
            nfi.NumberDecimalSeparator = "."
            nfi.NumberGroupSeparator = " "
            nfi.NumberGroupSizes = New Integer() {3}
            Dim pattern As String = "#,##0" & If(DecPrec > 0, "." & New String("0"c, DecPrec), "")
            Dim result As String = rounded.ToString(pattern, nfi)
            ' DecimalFormat keeps the sign of negative values that round to zero ("-0.00"); .NET Framework drops it.
            Dim negative As Boolean = BitConverter.DoubleToInt64Bits(Value) < 0
            If rounded = 0 AndAlso negative AndAlso Not result.StartsWith("-", StringComparison.Ordinal) Then
                result = "-" & result
            End If
            Return result
        End Function

        Public Shared Function getSDbl(ByVal Value As Double, ByVal DecPrec As Integer, ByVal ShowPlusSign As Boolean) As String
            Dim PlusSign As String = ""
            If ShowPlusSign AndAlso Value > 0 Then
                PlusSign = "+"
            End If
            If ShowPlusSign AndAlso Value = 0 Then
                PlusSign = " "
            End If
            Return PlusSign & getSDbl(Value, DecPrec)
        End Function

        ''' <summary>As <see cref="getSDbl(Double, Integer, Boolean)"/>, left-padded with spaces to the given length.</summary>
        Public Shared Function getSDbl(ByVal Value As Double, ByVal DecPrec As Integer, ByVal ShowPlusSign As Boolean, ByVal StringLength As Integer) As String
            Dim SDbl As String = getSDbl(Value, DecPrec, ShowPlusSign)
            If SDbl.Length >= StringLength Then
                Return SDbl
            End If
            Return SDbl.PadLeft(StringLength)
        End Function

        Public Shared Function getSInt(ByVal Value As Integer, ByVal CharsCount As Integer) As String
            Return getSInt(Value, CharsCount, " "c)
        End Function

        ''' <summary>
        ''' Formats an integer left-padded to <paramref name="CharsCount"/> characters. A leading char of
        ''' '0' zero-pads after the sign (Java's <c>%0Nd</c>); any other leading char pads with spaces (<c>%Nd</c>).
        ''' </summary>
        Public Shared Function getSInt(ByVal Value As Integer, ByVal CharsCount As Integer, ByVal LeadingChar As Char) As String
            If CharsCount <= 0 Then
                Return getSInt(Value)
            End If
            If LeadingChar = "0"c Then
                Return JavaString.format("%0" & CharsCount.ToString(CultureInfo.InvariantCulture) & "d", Value)
            End If
            Return JavaString.format("%" & CharsCount.ToString(CultureInfo.InvariantCulture) & "d", Value)
        End Function

        Public Shared Function getSInt(ByVal Value As Integer) As String
            Return Value.ToString("D", CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Integers joined with ", ".</summary>
        Public Shared Function getSIntA(ByVal ParamArray intA() As Integer) As String
            If intA Is Nothing OrElse intA.Length = 0 Then
                Return "?"
            End If
            Dim sb As New StringBuilder()
            For K As Integer = 0 To intA.Length - 1
                If sb.Length > 0 Then
                    sb.Append(", ")
                End If
                sb.Append(getSInt(intA(K)))
            Next K
            Return sb.ToString()
        End Function

        ''' <summary>Number of characters needed to print indices up to <paramref name="MaxIndex"/>.</summary>
        Public Shared Function getIndexCharsCount(ByVal MaxIndex As Integer) As Integer
            If MaxIndex <= 0 Then
                Return 1
            End If
            Return CInt(Math.Truncate(Math.Log10(MaxIndex))) + 1
        End Function

        ''' <summary>Local date/time as "LDTm: mm:ss.SSS".</summary>
        Public Shared ReadOnly Property SLcDtTm As String
            Get
                Return getSLcDtTm("mm:ss.SSS")
            End Get
        End Property

        ''' <summary>Local date/time formatted with a Java date pattern, prefixed with "LDTm: ".</summary>
        Public Shared Function getSLcDtTm(ByVal FormatS As String) As String
            Return "LDTm: " & DateTime.Now.ToString(JavaString.ConvertJavaDatePattern(FormatS), CultureInfo.InvariantCulture)
        End Function

    End Class

End Namespace
