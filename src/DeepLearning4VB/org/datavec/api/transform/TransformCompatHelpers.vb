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

Imports System
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports DeepLearning4VB.JavaCompat

Namespace org.datavec.api.transform

    ''' <summary>
    ''' Java <c>java.lang.Number</c> and <c>Integer/Long/Float/Double.parseXxx</c> semantics for the transform
    ''' packages (conditions, metadata, aggregators). Candidate for hoisting into JavaCompat.
    ''' </summary>
    Friend Module JavaNumbers

        ''' <summary>True for the boxed numeric types a Java <c>Number</c> can be.</summary>
        Public Function isNumber(ByVal o As Object) As Boolean
            Return TypeOf o Is Integer OrElse TypeOf o Is Long OrElse TypeOf o Is Double OrElse TypeOf o Is Single OrElse
                TypeOf o Is Short OrElse TypeOf o Is SByte OrElse TypeOf o Is Byte OrElse TypeOf o Is Decimal OrElse
                TypeOf o Is UInteger OrElse TypeOf o Is ULong OrElse TypeOf o Is UShort
        End Function

        Private Function requireNumber(ByVal o As Object) As Object
            If o Is Nothing Then Throw New NullReferenceException("null cannot be used as a java.lang.Number")
            If Not isNumber(o) Then Throw New InvalidCastException(o.GetType().FullName & " cannot be cast to java.lang.Number")
            Return o
        End Function

        ''' <summary>Java <c>Number.doubleValue()</c>.</summary>
        Public Function doubleValue(ByVal o As Object) As Double
            Return Convert.ToDouble(requireNumber(o), CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Java <c>Number.floatValue()</c>.</summary>
        Public Function floatValue(ByVal o As Object) As Single
            Return Convert.ToSingle(requireNumber(o), CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Java <c>Number.longValue()</c> (floating point values are truncated towards zero).</summary>
        Public Function longValue(ByVal o As Object) As Long
            requireNumber(o)
            If TypeOf o Is Double OrElse TypeOf o Is Single OrElse TypeOf o Is Decimal Then
                Dim d As Double = Convert.ToDouble(o, CultureInfo.InvariantCulture)
                If Double.IsNaN(d) Then Return 0L
                If d >= 9.2233720368547758E+18 Then Return Long.MaxValue
                If d <= -9.2233720368547758E+18 Then Return Long.MinValue
                Return CLng(Math.Truncate(d))
            End If
            Return Convert.ToInt64(o, CultureInfo.InvariantCulture)
        End Function

        ''' <summary>Java <c>Number.intValue()</c> (floating point values are truncated towards zero, long values wrap).</summary>
        Public Function intValue(ByVal o As Object) As Integer
            requireNumber(o)
            If TypeOf o Is Double OrElse TypeOf o Is Single OrElse TypeOf o Is Decimal Then
                Dim d As Double = Convert.ToDouble(o, CultureInfo.InvariantCulture)
                If Double.IsNaN(d) Then Return 0
                If d >= 2147483647.0 Then Return Integer.MaxValue
                If d <= -2147483648.0 Then Return Integer.MinValue
                Return CInt(Math.Truncate(d))
            End If
            Return Arrays.WrapToInt32(Convert.ToInt64(o, CultureInfo.InvariantCulture))
        End Function

        ''' <summary>Java <c>Integer.parseInt</c> as a Try-method: optional sign and decimal digits only, no whitespace.</summary>
        Public Function tryParseInt(ByVal s As String, ByRef value As Integer) As Boolean
            value = 0
            If s Is Nothing OrElse s.Length = 0 Then Return False
            Return Integer.TryParse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, value)
        End Function

        ''' <summary>Java <c>Long.parseLong</c> as a Try-method.</summary>
        Public Function tryParseLong(ByVal s As String, ByRef value As Long) As Boolean
            value = 0
            If s Is Nothing OrElse s.Length = 0 Then Return False
            Return Long.TryParse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, value)
        End Function

        ''' <summary>
        ''' Java <c>Double.parseDouble</c> as a Try-method: surrounding whitespace is ignored, "NaN", "Infinity",
        ''' "+Infinity", "-Infinity" and the Java type suffixes (1.5d, 1.5f) are accepted.
        ''' </summary>
        Public Function tryParseDouble(ByVal s As String, ByRef value As Double) As Boolean
            value = 0
            If s Is Nothing Then Return False
            Dim t As String = s.Trim()
            If t.Length = 0 Then Return False
            Select Case t
                Case "NaN"
                    value = Double.NaN
                    Return True
                Case "Infinity", "+Infinity"
                    value = Double.PositiveInfinity
                    Return True
                Case "-Infinity"
                    value = Double.NegativeInfinity
                    Return True
            End Select
            Dim last As Char = t(t.Length - 1)
            If last = "d"c OrElse last = "D"c OrElse last = "f"c OrElse last = "F"c Then
                t = t.Substring(0, t.Length - 1)
            End If
            Return Double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, value)
        End Function

        ''' <summary>Java <c>Float.parseFloat</c> as a Try-method.</summary>
        Public Function tryParseFloat(ByVal s As String, ByRef value As Single) As Boolean
            Dim d As Double
            If Not tryParseDouble(s, d) Then
                value = 0
                Return False
            End If
            value = CSng(d)
            Return True
        End Function

        ''' <summary>Java <c>Integer.parseInt</c>; throws <see cref="NumberFormatException"/>.</summary>
        Public Function parseInt(ByVal s As String) As Integer
            Dim v As Integer
            If Not tryParseInt(s, v) Then Throw New NumberFormatException("For input string: """ & s & """")
            Return v
        End Function

        ''' <summary>Java <c>Long.parseLong</c>; throws <see cref="NumberFormatException"/>.</summary>
        Public Function parseLong(ByVal s As String) As Long
            Dim v As Long
            If Not tryParseLong(s, v) Then Throw New NumberFormatException("For input string: """ & s & """")
            Return v
        End Function

        ''' <summary>Java <c>Double.parseDouble</c>; throws <see cref="NumberFormatException"/>.</summary>
        Public Function parseDouble(ByVal s As String) As Double
            Dim v As Double
            If Not tryParseDouble(s, v) Then Throw New NumberFormatException("For input string: """ & s & """")
            Return v
        End Function

        ''' <summary>Java <c>Float.parseFloat</c>; throws <see cref="NumberFormatException"/>.</summary>
        Public Function parseFloat(ByVal s As String) As Single
            Dim v As Single
            If Not tryParseFloat(s, v) Then Throw New NumberFormatException("For input string: """ & s & """")
            Return v
        End Function

    End Module

    ''' <summary>Java <c>String</c> helpers for the transform packages. Candidate for hoisting into JavaCompat.</summary>
    Friend Module JavaStrings

        ''' <summary>Java <c>String.matches(regex)</c>: the whole string must match the (Java-syntax) regular expression.</summary>
        Public Function matches(ByVal s As String, ByVal pattern As String) As Boolean
            Return Regex.IsMatch(s, "\A(?:" & pattern & ")\z")
        End Function

        ''' <summary>Java <c>StringBuilder.reverse()</c> as a function on a string.</summary>
        Public Function reverse(ByVal s As String) As String
            Dim chars() As Char = s.ToCharArray()
            Array.Reverse(chars)
            Return New String(chars)
        End Function

    End Module

End Namespace
