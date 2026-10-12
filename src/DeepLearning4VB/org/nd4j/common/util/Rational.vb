Imports System
Imports System.Globalization
Imports System.Numerics

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

Namespace org.nd4j.common.util

    ''' <summary>
    ''' An arbitrary-precision rational number a/b, always kept in lowest terms with a positive denominator.
    ''' </summary>
    Public Class Rational
        Implements ICloneable, IComparable(Of Rational)

        ''' <summary>The maximum and minimum value of a standard 32-bit integer.</summary>
        Friend Shared ReadOnly MAX_INT As BigInteger = New BigInteger(Integer.MaxValue)
        Friend Shared ReadOnly MIN_INT As BigInteger = New BigInteger(Integer.MinValue)
        Public Shared ReadOnly ONE As New Rational(1, 1)
        Public Shared ReadOnly ZERO As New Rational()

        ''' <summary>Numerator.</summary>
        Friend a As BigInteger
        ''' <summary>Denominator.</summary>
        Friend b As BigInteger

        ''' <summary>Creates the value zero.</summary>
        Public Sub New()
            a = BigInteger.Zero
            b = BigInteger.One
        End Sub

        ''' <summary>Creates a/b.</summary>
        Public Sub New(ByVal a As BigInteger, ByVal b As BigInteger)
            If b.IsZero Then
                Throw New DivideByZeroException("Rational denominator must not be zero")
            End If
            Me.a = a
            Me.b = b
            normalize()
        End Sub

        ''' <summary>Creates the integer a.</summary>
        Public Sub New(ByVal a As BigInteger)
            Me.a = a
            b = BigInteger.One
        End Sub

        ''' <summary>Creates a/b.</summary>
        Public Sub New(ByVal a As Integer, ByVal b As Integer)
            Me.New(New BigInteger(a), New BigInteger(b))
        End Sub

        ''' <summary>
        ''' Parses a string: either "numerator/denominator" or a plain integer.
        ''' </summary>
        Public Sub New(ByVal str As String)
            Me.New(str, 10)
        End Sub

        ''' <summary>
        ''' Parses a string in the given radix: either "numerator/denominator" or a plain integer.
        ''' </summary>
        Public Sub New(ByVal str As String, ByVal radix As Integer)
            Dim hasslah As Integer = str.IndexOf("/", StringComparison.Ordinal)
            If hasslah = -1 Then
                a = ParseBigInteger(str, radix)
                b = BigInteger.One
            Else
                a = ParseBigInteger(str.Substring(0, hasslah), radix)
                b = ParseBigInteger(str.Substring(hasslah + 1), radix)
                If b.IsZero Then
                    Throw New DivideByZeroException("Rational denominator must not be zero")
                End If
                normalize()
            End If
        End Sub

        ''' <summary>Parses an integer in any radix from 2 to 36 (java.math.BigInteger(String, int)).</summary>
        Friend Shared Function ParseBigInteger(ByVal str As String, ByVal radix As Integer) As BigInteger
            If str Is Nothing Then Throw New ArgumentNullException(NameOf(str))
            If radix < 2 OrElse radix > 36 Then Throw New ArgumentOutOfRangeException(NameOf(radix))
            Dim s As String = str.Trim()
            If s.Length = 0 Then Throw New FormatException("Zero length BigInteger")
            If radix = 10 Then
                Return BigInteger.Parse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture)
            End If
            Dim negative As Boolean = False
            Dim start As Integer = 0
            If s(0) = "-"c Then
                negative = True
                start = 1
            ElseIf s(0) = "+"c Then
                start = 1
            End If
            If start >= s.Length Then Throw New FormatException("Invalid number: " & str)
            Dim result As BigInteger = BigInteger.Zero
            Dim bigRadix As New BigInteger(radix)
            For i As Integer = start To s.Length - 1
                Dim c As Char = Char.ToLowerInvariant(s(i))
                Dim digit As Integer
                If c >= "0"c AndAlso c <= "9"c Then
                    digit = AscW(c) - AscW("0"c)
                ElseIf c >= "a"c AndAlso c <= "z"c Then
                    digit = AscW(c) - AscW("a"c) + 10
                Else
                    Throw New FormatException("Invalid digit '" & s(i) & "' for radix " & radix)
                End If
                If digit >= radix Then Throw New FormatException("Invalid digit '" & s(i) & "' for radix " & radix)
                result = result * bigRadix + digit
            Next i
            Return If(negative, -result, result)
        End Function

        ''' <summary>Binomial coefficient (n choose m).</summary>
        Public Shared Function binomial(ByVal n As Rational, ByVal m As BigInteger) As Rational
            If m.CompareTo(BigInteger.Zero) = 0 Then
                Return Rational.ONE
            End If
            Dim bin As Rational = n
            Dim i As BigInteger = New BigInteger(2)
            Do While i.CompareTo(m) <> 1
                bin = bin.multiply(n.subtract(i - BigInteger.One)).divide(i)
                i = i + BigInteger.One
            Loop
            Return bin
        End Function

        ''' <summary>Binomial coefficient (n choose m).</summary>
        Public Shared Function binomial(ByVal n As Rational, ByVal m As Integer) As Rational
            If m = 0 Then
                Return Rational.ONE
            End If
            Dim bin As Rational = n
            For i As Integer = 2 To m
                bin = bin.multiply(n.subtract(i - 1)).divide(i)
            Next i
            Return bin
        End Function

        ''' <summary>Creates a copy.</summary>
        Public Function clone() As Rational
            Return New Rational(a, b)
        End Function

        Private Function CloneObject() As Object Implements ICloneable.Clone
            Return clone()
        End Function

        Public Overridable Function multiply(ByVal val As Rational) As Rational
            Return New Rational(a * val.a, b * val.b)
        End Function

        Public Overridable Function multiply(ByVal val As BigInteger) As Rational
            Return multiply(New Rational(val, BigInteger.One))
        End Function

        Public Overridable Function multiply(ByVal val As Integer) As Rational
            Return multiply(New BigInteger(val))
        End Function

        ''' <summary>This value raised to an integer power (1 for exponent 0; negative exponents invert).</summary>
        Public Overridable Function pow(ByVal exponent As Integer) As Rational
            If exponent = 0 Then
                Return New Rational(1, 1)
            End If
            Dim num As BigInteger = BigInteger.Pow(a, Math.Abs(exponent))
            Dim deno As BigInteger = BigInteger.Pow(b, Math.Abs(exponent))
            If exponent > 0 Then
                Return New Rational(num, deno)
            Else
                Return New Rational(deno, num)
            End If
        End Function

        ''' <summary>This value raised to an integer power.</summary>
        Public Overridable Function pow(ByVal exponent As BigInteger) As Rational
            If exponent.CompareTo(MAX_INT) = 1 Then
                Throw New FormatException("Exponent " & exponent.ToString() & " too large.")
            End If
            If exponent.CompareTo(MIN_INT) = -1 Then
                Throw New FormatException("Exponent " & exponent.ToString() & " too small.")
            End If
            Return pow(CInt(exponent))
        End Function

        Public Overridable Function divide(ByVal val As Rational) As Rational
            Return New Rational(a * val.b, b * val.a)
        End Function

        Public Overridable Function divide(ByVal val As BigInteger) As Rational
            Return divide(New Rational(val, BigInteger.One))
        End Function

        Public Overridable Function divide(ByVal val As Integer) As Rational
            Return divide(New Rational(val, 1))
        End Function

        Public Overridable Function add(ByVal val As Rational) As Rational
            Return New Rational(a * val.b + b * val.a, b * val.b)
        End Function

        Public Overridable Function add(ByVal val As BigInteger) As Rational
            Return add(New Rational(val, BigInteger.One))
        End Function

        Public Overridable Function add(ByVal val As Integer) As Rational
            Return add(New Rational(val, 1))
        End Function

        Public Overridable Function negate() As Rational
            Return New Rational(-a, b)
        End Function

        Public Overridable Function subtract(ByVal val As Rational) As Rational
            Return add(val.negate())
        End Function

        Public Overridable Function subtract(ByVal val As BigInteger) As Rational
            Return subtract(New Rational(val, BigInteger.One))
        End Function

        Public Overridable Function subtract(ByVal val As Integer) As Rational
            Return subtract(New Rational(val, 1))
        End Function

        ''' <summary>The numerator of the reduced fraction.</summary>
        Public Overridable Function numer() As BigInteger
            Return a
        End Function

        ''' <summary>The denominator of the reduced fraction.</summary>
        Public Overridable Function denom() As BigInteger
            Return b
        End Function

        Public Overridable Function abs() As Rational
            Return New Rational(BigInteger.Abs(a), BigInteger.Abs(b))
        End Function

        ''' <summary>The nearest integer not greater than this value.</summary>
        Public Overridable Function floor() As BigInteger
            If b.CompareTo(BigInteger.One) = 0 Then
                Return a
            ElseIf a.CompareTo(BigInteger.Zero) > 0 Then
                Return BigInteger.Divide(a, b)
            Else
                Return BigInteger.Divide(a, b) - BigInteger.One
            End If
        End Function

        ''' <summary>The integer part, rounded towards zero.</summary>
        Public Overridable Function trunc() As BigInteger
            If b.CompareTo(BigInteger.One) = 0 Then
                Return a
            Else
                Return BigInteger.Divide(a, b)
            End If
        End Function

        ''' <summary>-1, 0 or 1 if this number is numerically less than, equal to, or greater than val.</summary>
        Public Overridable Function compareTo(ByVal val As Rational) As Integer Implements IComparable(Of Rational).CompareTo
            ' Denominators are always positive, so cross-multiplication preserves the order.
            Dim left As BigInteger = a * val.b
            Dim right As BigInteger = val.a * b
            Return left.CompareTo(right)
        End Function

        Public Overridable Function compareTo(ByVal val As BigInteger) As Integer
            Return compareTo(New Rational(val, BigInteger.One))
        End Function

        ''' <summary>"numerator/denominator", or just the numerator when the denominator is 1.</summary>
        Public Overrides Function ToString() As String
            If b.CompareTo(BigInteger.One) <> 0 Then
                Return a.ToString() & "/" & b.ToString()
            Else
                Return a.ToString()
            End If
        End Function

        ''' <summary>The value with double precision.</summary>
        Public Overridable Function doubleValue() As Double
            If b.IsOne Then
                Return CDbl(a)
            End If
            ' Scale so that both parts fit comfortably in a double before dividing, which avoids
            ' intermediate overflow for very large numerators or denominators.
            Dim shift As Integer = Math.Max(0, CInt(Math.Max(BigInteger.Log(BigInteger.Abs(a), 2), BigInteger.Log(b, 2))) - 1000)
            If shift > 0 Then
                Return CDbl(a >> shift) / CDbl(b >> shift)
            End If
            Return CDbl(a) / CDbl(b)
        End Function

        ''' <summary>The value with single precision.</summary>
        Public Overridable Function floatValue() As Single
            Return CSng(doubleValue())
        End Function

        ''' <summary>The value as a decimal (28-29 significant digits).</summary>
        Public Overridable Function BigDecimalValue() As Decimal
            Return CDec(a) / CDec(b)
        End Function

        ''' <summary>The value in floating point format with the given number of significant digits, truncated.</summary>
        Public Overridable Function toFString(ByVal digits As Integer) As String
            If b.CompareTo(BigInteger.One) <> 0 Then
                Return RoundDown(digits).ToString(CultureInfo.InvariantCulture)
            Else
                Return a.ToString()
            End If
        End Function

        Private Function RoundDown(ByVal digits As Integer) As Decimal
            Dim value As Decimal = BigDecimalValue()
            If value = 0D OrElse digits <= 0 Then Return value
            Dim magnitude As Integer = CInt(Math.Floor(Math.Log10(CDbl(Math.Abs(value))))) + 1
            Dim decimals As Integer = digits - magnitude
            If decimals <= 0 Then
                Dim factor As Decimal = CDec(Math.Pow(10, -decimals))
                Return Math.Truncate(value / factor) * factor
            End If
            If decimals > 28 Then decimals = 28
            Dim scale As Decimal = CDec(Math.Pow(10, decimals))
            Return Math.Truncate(value * scale) / scale
        End Function

        Public Overridable Function max(ByVal val As Rational) As Rational
            If compareTo(val) > 0 Then
                Return Me
            Else
                Return val
            End If
        End Function

        Public Overridable Function min(ByVal val As Rational) As Rational
            If compareTo(val) < 0 Then
                Return Me
            Else
                Return val
            End If
        End Function

        ''' <summary>Pochhammer's symbol (this)_n = this*(this+1)*...*(this+n-1); Nothing for negative n.</summary>
        Public Overridable Function Pochhammer(ByVal n As BigInteger) As Rational
            If n.CompareTo(BigInteger.Zero) < 0 Then
                Return Nothing
            ElseIf n.CompareTo(BigInteger.Zero) = 0 Then
                Return Rational.ONE
            Else
                Dim res As New Rational(a, b)
                Dim i As BigInteger = BigInteger.One
                Do While i.CompareTo(n) < 0
                    res = res.multiply(add(i))
                    i = i + BigInteger.One
                Loop
                Return res
            End If
        End Function

        Public Overridable Function Pochhammer(ByVal n As Integer) As Rational
            Return Pochhammer(New BigInteger(n))
        End Function

        ''' <summary>Reduces to lowest terms and moves any negative sign to the numerator.</summary>
        Protected Friend Overridable Sub normalize()
            Dim g As BigInteger = BigInteger.GreatestCommonDivisor(a, b)
            If g.CompareTo(BigInteger.One) > 0 Then
                a = BigInteger.Divide(a, g)
                b = BigInteger.Divide(b, g)
            End If
            If b.CompareTo(BigInteger.Zero) = -1 Then
                a = -a
                b = -b
            End If
        End Sub

        Public Overrides Function Equals(ByVal o As Object) As Boolean
            Dim other = TryCast(o, Rational)
            If other Is Nothing Then Return False
            Return a.Equals(other.a) AndAlso b.Equals(other.b)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return a.GetHashCode() Xor (b.GetHashCode() * 31)
        End Function

    End Class

End Namespace
