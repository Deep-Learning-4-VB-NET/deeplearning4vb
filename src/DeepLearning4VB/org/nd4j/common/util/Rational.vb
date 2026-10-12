Imports System
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

	Friend Class Rational
		Implements ICloneable

	'     The maximum and minimum value of a standard Java integer, 2^31.
	'     
		Friend Shared MAX_INT As BigInteger = BigInteger.valueOf(Integer.MaxValue)
		Friend Shared MIN_INT As BigInteger = BigInteger.valueOf(Integer.MinValue)
		Friend Shared ONE As New Rational(1, 1)
		Friend Shared ZERO As New Rational()
		''' <summary>
		''' numerator
		''' </summary>
		Friend a As BigInteger
		''' <summary>
		''' denominator
		''' </summary>
		Friend b As BigInteger

		''' <summary>
		''' Default ctor, which represents the zero.
		''' </summary>
		Public Sub New()
			a = BigInteger.Zero
			b = BigInteger.One
		End Sub

		''' <summary>
		''' ctor from a numerator and denominator.
		''' </summary>
		''' <param name="a"> the numerator. </param>
		''' <param name="b"> the denominator. </param>
		Public Sub New(ByVal a As BigInteger, ByVal b As BigInteger)
			Me.a = a
			Me.b = b
			normalize()
		End Sub

		''' <summary>
		''' ctor from a numerator.
		''' </summary>
		''' <param name="a"> the BigInteger. </param>
		Public Sub New(ByVal a As BigInteger)
			Me.a = a
			b = BigInteger.valueOf(1)
		End Sub

		''' <summary>
		''' ctor from a numerator and denominator.
		''' </summary>
		''' <param name="a"> the numerator. </param>
		''' <param name="b"> the denominator. </param>
		Public Sub New(ByVal a As Integer, ByVal b As Integer)
			Me.New(BigInteger.valueOf(a), BigInteger.valueOf(b))
		End Sub

		''' <summary>
		''' ctor from a string representation.
		''' </summary>
		''' <param name="str"> the string.
		'''            This either has a slash in it, separating two integers, or, if there is no slash,
		'''            is representing the numerator with implicit denominator equal to 1.
		''' @warning this does not yet test for a denominator equal to zero </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Rational(String str) throws NumberFormatException
		Public Sub New(ByVal str As String)
			Me.New(str, 10)
		End Sub

		''' <summary>
		''' ctor from a string representation in a specified base.
		''' </summary>
		''' <param name="str">   the string.
		'''              This either has a slash in it, separating two integers, or, if there is no slash,
		'''              is just representing the numerator. </param>
		''' <param name="radix"> the number base for numerator and denominator
		''' @warning this does not yet test for a denominator equal to zero
		''' 5 </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Rational(String str, int radix) throws NumberFormatException
		Public Sub New(ByVal str As String, ByVal radix As Integer)
			Dim hasslah As Integer = str.IndexOf("/", StringComparison.Ordinal)
			If hasslah = -1 Then
				a = New BigInteger(str, radix)
				b = New BigInteger("1", radix)
				' no normalization necessary here 
			Else
	'             create numerator and denominator separately
	'             
				a = New BigInteger(str.Substring(0, hasslah), radix)
				b = New BigInteger(str.Substring(hasslah + 1), radix)
				normalize()
			End If
		End Sub

		''' <summary>
		''' binomial (n choose m).
		''' </summary>
		''' <param name="n"> the numerator. Equals the size of the set to choose from. </param>
		''' <param name="m"> the denominator. Equals the number of elements to select. </param>
		''' <returns> the binomial coefficient. </returns>
		Public Shared Function binomial(ByVal n As Rational, ByVal m As BigInteger) As Rational
			If m.compareTo(BigInteger.Zero) = 0 Then
				Return Rational.ONE
			End If
			Dim bin As Rational = n
			Dim i As BigInteger = BigInteger.valueOf(2)
			Do While i.compareTo(m) <> 1
				bin = bin.multiply(n.subtract(i - BigInteger.One)).divide(i)
				i = i + System.Numerics.BigInteger.One
			Loop
			Return bin
		End Function ' Rational.binomial

		''' <summary>
		''' binomial (n choose m).
		''' </summary>
		''' <param name="n"> the numerator. Equals the size of the set to choose from. </param>
		''' <param name="m"> the denominator. Equals the number of elements to select. </param>
		''' <returns> the binomial coefficient. </returns>
		Public Shared Function binomial(ByVal n As Rational, ByVal m As Integer) As Rational
			If m = 0 Then
				Return Rational.ONE
			End If
			Dim bin As Rational = n
			For i As Integer = 2 To m
				bin = bin.multiply(n.subtract(i - 1)).divide(i)
			Next i
			Return bin
		End Function ' Rational.binomial

		''' <summary>
		''' Create a copy.
		''' </summary>
		Public Overrides Function clone() As Rational
	'         protected access means this does not work
	'         * return new Rational(a.clone(), b.clone()) ;
	'         
			Dim aclon As New BigInteger("" & a)
			Dim bclon As New BigInteger("" & b)
			Return New Rational(aclon, bclon)
		End Function ' Rational.clone

		''' <summary>
		''' Multiply by another fraction.
		''' </summary>
		''' <param name="val"> a second rational number. </param>
		''' <returns> the product of this with the val. </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public Rational multiply(final Rational val)
		Public Overridable Function multiply(ByVal val As Rational) As Rational
			Dim num As BigInteger = a * val.a
			Dim deno As BigInteger = b * val.b
	'         Normalization to an coprime format will be done inside
	'         * the ctor() and is not duplicated here.
	'         
			Return (New Rational(num, deno))
		End Function ' Rational.multiply

		''' <summary>
		''' Multiply by a BigInteger.
		''' </summary>
		''' <param name="val"> a second number. </param>
		''' <returns> the product of this with the value. </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public Rational multiply(final java.math.BigInteger val)
		Public Overridable Function multiply(ByVal val As BigInteger) As Rational
			Dim val2 As New Rational(val, BigInteger.One)
			Return (multiply(val2))
		End Function ' Rational.multiply

		''' <summary>
		''' Multiply by an integer.
		''' </summary>
		''' <param name="val"> a second number. </param>
		''' <returns> the product of this with the value. </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public Rational multiply(final int val)
		Public Overridable Function multiply(ByVal val As Integer) As Rational
			Dim tmp As BigInteger = BigInteger.valueOf(val)
			Return multiply(tmp)
		End Function ' Rational.multiply

		''' <summary>
		''' Power to an integer.
		''' </summary>
		''' <param name="exponent"> the exponent. </param>
		''' <returns> this value raised to the power given by the exponent.
		''' If the exponent is 0, the value 1 is returned. </returns>
		Public Overridable Function pow(ByVal exponent As Integer) As Rational
			If exponent = 0 Then
				Return New Rational(1, 1)
			End If
			Dim num As BigInteger = a.pow(Math.Abs(exponent))
			Dim deno As BigInteger = b.pow(Math.Abs(exponent))
			If exponent > 0 Then
				Return (New Rational(num, deno))
			Else
				Return (New Rational(deno, num))
			End If
		End Function ' Rational.pow

		''' <summary>
		''' Power to an integer.
		''' </summary>
		''' <param name="exponent"> the exponent. </param>
		''' <returns> this value raised to the power given by the exponent.
		''' If the exponent is 0, the value 1 is returned. </returns>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Rational pow(java.math.BigInteger exponent) throws NumberFormatException
		Public Overridable Function pow(ByVal exponent As BigInteger) As Rational
			' test for overflow 
			If exponent.compareTo(MAX_INT) = 1 Then
				Throw New System.FormatException("Exponent " & exponent.ToString() & " too large.")
			End If
			If exponent.compareTo(MIN_INT) = -1 Then
				Throw New System.FormatException("Exponent " & exponent.ToString() & " too small.")
			End If
			' promote to the simpler interface above 
			Return pow(CInt(CUInt(exponent And UInteger.MaxValue)))
		End Function ' Rational.pow

		''' <summary>
		''' Divide by another fraction.
		''' </summary>
		''' <param name="val"> A second rational number. </param>
		''' <returns> The value of this/val </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public Rational divide(final Rational val)
		Public Overridable Function divide(ByVal val As Rational) As Rational
			Dim num As BigInteger = a * val.b
			Dim deno As BigInteger = b * val.a
	'         Reduction to a coprime format is done inside the ctor,
	'         * and not repeated here.
	'         
			Return (New Rational(num, deno))
		End Function ' Rational.divide

		''' <summary>
		''' Divide by an integer.
		''' </summary>
		''' <param name="val"> a second number. </param>
		''' <returns> the value of this/val </returns>
		Public Overridable Function divide(ByVal val As BigInteger) As Rational
			Dim val2 As New Rational(val, BigInteger.One)
			Return (divide(val2))
		End Function ' Rational.divide

		''' <summary>
		''' Divide by an integer.
		''' </summary>
		''' <param name="val"> A second number. </param>
		''' <returns> The value of this/val </returns>
		Public Overridable Function divide(ByVal val As Integer) As Rational
			Dim val2 As New Rational(val, 1)
			Return (divide(val2))
		End Function ' Rational.divide

		''' <summary>
		''' Add another fraction.
		''' </summary>
		''' <param name="val"> The number to be added </param>
		''' <returns> this+val. </returns>
		Public Overridable Function add(ByVal val As Rational) As Rational
			Dim num As BigInteger = a * val.b + b * val.a
			Dim deno As BigInteger = b * val.b
			Return (New Rational(num, deno))
		End Function ' Rational.add

		''' <summary>
		''' Add another integer.
		''' </summary>
		''' <param name="val"> The number to be added </param>
		''' <returns> this+val. </returns>
		Public Overridable Function add(ByVal val As BigInteger) As Rational
			Dim val2 As New Rational(val, BigInteger.One)
			Return (add(val2))
		End Function ' Rational.add

		''' <summary>
		''' Compute the negative.
		''' </summary>
		''' <returns> -this. </returns>
		Public Overridable Function negate() As Rational
			Return (New Rational(-a, b))
		End Function ' Rational.negate

		''' <summary>
		''' Subtract another fraction.
		''' 7
		''' </summary>
		''' <param name="val"> the number to be subtracted from this </param>
		''' <returns> this - val. </returns>
		Public Overridable Function subtract(ByVal val As Rational) As Rational
			Dim val2 As Rational = val.negate()
			Return (add(val2))
		End Function ' Rational.subtract

		''' <summary>
		''' Subtract an integer.
		''' </summary>
		''' <param name="val"> the number to be subtracted from this </param>
		''' <returns> this - val. </returns>
		Public Overridable Function subtract(ByVal val As BigInteger) As Rational
			Dim val2 As New Rational(val, BigInteger.One)
			Return (subtract(val2))
		End Function ' Rational.subtract

		''' <summary>
		''' Subtract an integer.
		''' </summary>
		''' <param name="val"> the number to be subtracted from this </param>
		''' <returns> this - val. </returns>
		Public Overridable Function subtract(ByVal val As Integer) As Rational
			Dim val2 As New Rational(val, 1)
			Return (subtract(val2))
		End Function ' Rational.subtract

		''' <summary>
		''' Get the numerator.
		''' </summary>
		''' <returns> The numerator of the reduced fraction. </returns>
		Public Overridable Function numer() As BigInteger
			Return a
		End Function

		''' <summary>
		''' Get the denominator.
		''' </summary>
		''' <returns> The denominator of the reduced fraction. </returns>
		Public Overridable Function denom() As BigInteger
			Return b
		End Function

		''' <summary>
		''' Absolute value.
		''' </summary>
		''' <returns> The absolute (non-negative) value of this. </returns>
		Public Overridable Function abs() As Rational
			Return (New Rational(a.abs(), b.abs()))
		End Function

		''' <summary>
		''' floor(): the nearest integer not greater than this.
		''' </summary>
		''' <returns> The integer rounded towards negative infinity. </returns>
		Public Overridable Function floor() As BigInteger
	'         is already integer: return the numerator
	'         
			If b.compareTo(BigInteger.One) = 0 Then
				Return a
			ElseIf a.compareTo(BigInteger.Zero) > 0 Then
				Return a / b
			Else
				Return a / b - BigInteger.One
			End If
		End Function ' Rational.floor


		''' <summary>
		''' Remove the fractional part.
		''' </summary>
		''' <returns> The integer rounded towards zero. </returns>
		Public Overridable Function trunc() As BigInteger
	'         is already integer: return the numerator
	'         
			If b.compareTo(BigInteger.One) = 0 Then
				Return a
			Else
				Return a / b
			End If
		End Function ' Rational.trunc


		''' <summary>
		''' Compares the value of this with another constant.
		''' </summary>
		''' <param name="val"> the other constant to compare with </param>
		''' <returns> -1, 0 or 1 if this number is numerically less than, equal to,
		''' or greater than val. </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int compareTo(final Rational val)
		Public Overridable Function compareTo(ByVal val As Rational) As Integer
	'         Since we have always kept the denominators positive,
	'         * simple cross-multiplying works without changing the sign.
	'         
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.math.BigInteger left = a.multiply(val.b);
			Dim left As BigInteger = a * val.b
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.math.BigInteger right = val.a.multiply(b);
			Dim right As BigInteger = val.a * b
			Return left.compareTo(right)
		End Function ' Rational.compareTo


		''' <summary>
		''' Compares the value of this with another constant.
		''' </summary>
		''' <param name="val"> the other constant to compare with </param>
		''' <returns> -1, 0 or 1 if this number is numerically less than, equal to,
		''' or greater than val. </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public int compareTo(final java.math.BigInteger val)
		Public Overridable Function compareTo(ByVal val As BigInteger) As Integer
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Rational val2 = new Rational(val, java.math.BigInteger.ONE);
			Dim val2 As New Rational(val, BigInteger.One)
			Return (compareTo(val2))
		End Function ' Rational.compareTo


		''' <summary>
		''' Return a string in the format number/denom.
		''' If the denominator equals 1, print just the numerator without a slash.
		''' </summary>
		''' <returns> the human-readable version in base 10 </returns>
		Public Overrides Function ToString() As String
			If b.compareTo(BigInteger.One) <> 0 Then
				Return (a.ToString() & "/" & b.ToString())
			Else
				Return a.ToString()
			End If
		End Function ' Rational.toString


		''' <summary>
		''' Return a double value representation.
		''' </summary>
		''' <returns> The value with double precision. </returns>
		Public Overridable Function doubleValue() As Double
	'         To meet the risk of individual overflows of the exponents of
	'         * a separate invocation a.doubleValue() or b.doubleValue(), we divide first
	'         * in a BigDecimal environment and converst the result.
	'         
			Dim adivb As Decimal = (New Decimal(a)).divide(New Decimal(b), MathContext.DECIMAL128)
			Return adivb.doubleValue()
		End Function ' Rational.doubleValue


		''' <summary>
		''' Return a float value representation.
		''' </summary>
		''' <returns> The value with single precision. </returns>
		Public Overridable Function floatValue() As Single
			Dim adivb As Decimal = (New Decimal(a)).divide(New Decimal(b), MathContext.DECIMAL128)
			Return adivb.floatValue()
		End Function ' Rational.floatValue


		''' <summary>
		''' Return a representation as BigDecimal.
		''' </summary>
		''' <param name="mc"> the mathematical context which determines precision, rounding mode etc </param>
		''' <returns> A representation as a BigDecimal floating point number. </returns>
		Public Overridable Function BigDecimalValue(ByVal mc As MathContext) As Decimal
	'         numerator and denominator individually rephrased
	'         
			Dim n As New Decimal(a)
			Dim d As New Decimal(b)
			Return n.divide(d, mc)
		End Function ' Rational.BigDecimnalValue


		''' <summary>
		''' Return a string in floating point format.
		''' </summary>
		''' <param name="digits"> The precision (number of digits) </param>
		''' <returns> The human-readable version in base 10. </returns>
		Public Overridable Function toFString(ByVal digits As Integer) As String
			If b.compareTo(BigInteger.One) <> 0 Then
				Dim mc As New MathContext(digits, RoundingMode.DOWN)
				Dim f As Decimal = (New Decimal(a)).divide(New Decimal(b), mc)
				Return (f.ToString())
			Else
				Return a.ToString()
			End If
		End Function ' Rational.toFString


		''' <summary>
		''' Compares the value of this with another constant.
		''' </summary>
		''' <param name="val"> The other constant to compare with </param>
		''' <returns> The arithmetic maximum of this and val. </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public Rational max(final Rational val)
		Public Overridable Function max(ByVal val As Rational) As Rational
			If compareTo(val) > 0 Then
				Return Me
			Else
				Return val
			End If
		End Function ' Rational.max


		''' <summary>
		''' Compares the value of this with another constant.
		''' </summary>
		''' <param name="val"> The other constant to compare with </param>
		''' <returns> The arithmetic minimum of this and val. </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public Rational min(final Rational val)
		Public Overridable Function min(ByVal val As Rational) As Rational
			If compareTo(val) < 0 Then
				Return Me
			Else
				Return val
			End If
		End Function ' Rational.min


		''' <summary>
		''' Compute Pochhammer's symbol (this)_n.
		''' </summary>
		''' <param name="n"> The number of product terms in the evaluation. </param>
		''' <returns> Gamma(this+n)/Gamma(this) = this*(this+1)*...*(this+n-1). </returns>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public Rational Pochhammer(final java.math.BigInteger n)
		Public Overridable Function Pochhammer(ByVal n As BigInteger) As Rational
			If n.compareTo(BigInteger.Zero) < 0 Then
				Return Nothing
			ElseIf n.compareTo(BigInteger.Zero) = 0 Then
				Return Rational.ONE
			Else
	'             initialize results with the current value
	'             
				Dim res As New Rational(a, b)
				Dim i As BigInteger = BigInteger.One
				Do While i.compareTo(n) < 0
					res = res.multiply(add(i))
					i = i + System.Numerics.BigInteger.One
				Loop
				Return res
			End If
		End Function ' Rational.pochhammer


		''' <summary>
		''' Compute pochhammer's symbol (this)_n.
		''' </summary>
		''' <param name="n"> The number of product terms in the evaluation. </param>
		''' <returns> Gamma(this+n)/GAMMA(this). </returns>
		Public Overridable Function Pochhammer(ByVal n As Integer) As Rational
			Return Pochhammer(BigInteger.valueOf(n))
		End Function ' Rational.pochhammer


		''' <summary>
		''' Normalize to coprime numerator and denominator.
		''' Also copy a negative sign of the denominator to the numerator.
		''' </summary>
		Protected Friend Overridable Sub normalize()
	'         compute greatest common divisor of numerator and denominator
	'         
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.math.BigInteger g = a.gcd(b);
			Dim g As BigInteger = a.gcd(b)
			If g.compareTo(BigInteger.One) > 0 Then
				a = a / g
				b = b / g
			End If
			If b.compareTo(BigInteger.Zero) = -1 Then
				a = -a
				b = -b
			End If
		End Sub ' Rational.normalize

	End Class ' Rational

End Namespace