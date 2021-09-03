Imports System.Collections.Generic
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
	''' Bernoulli numbers.
	''' </summary>

	Friend Class Bernoulli
	'    
	'     * The list of all Bernoulli numbers as a vector, n=0,2,4,....
	'     

		Friend Shared a As IList(Of Rational) = New List(Of Rational)()

		Private Class ComparatorAnonymousInnerClass2
			Implements IComparer(Of KeyValuePair(Of K, V))

			Private ReadOnly outerInstance As ArrayUtil

			Public Sub New(ByVal outerInstance As ArrayUtil)
				Me.outerInstance = outerInstance
			End Sub

			Public Function Compare(ByVal o1 As KeyValuePair(Of K, V), ByVal o2 As KeyValuePair(Of K, V)) As Integer Implements IComparer(Of KeyValuePair(Of K, V)).Compare
				Return (o1.Value).compareTo(o2.Value)
			End Function
		End Class

		Public Sub New()
			If a.Count = 0 Then
				a.Add(Rational.ONE)
				a.Add(New Rational(1, 6))
			End If
		End Sub

		''' <summary>
		''' Set a coefficient in the internal table.
		''' </summary>
		''' <param name="n">     the zero-based index of the coefficient. n=0 for the constant term. </param>
		''' <param name="value"> the new value of the coefficient. </param>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: protected void set(final int n, final Rational value)
		Protected Friend Overridable Sub set(ByVal n As Integer, ByVal value As Rational)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int nindx = n / 2;
			Dim nindx As Integer = n \ 2
			If nindx < a.Count Then
				a(nindx) = value
			Else
				Do While a.Count < nindx
					a.Add(Rational.ZERO)
				Loop
				a.Add(value)
			End If
		End Sub

		''' <summary>
		''' The Bernoulli number at the index provided.
		''' </summary>
		''' <param name="n"> the index, non-negative. </param>
		''' <returns> the B_0=1 for n=0, B_1=-1/2 for n=1, B_2=1/6 for n=2 etc </returns>
		Public Overridable Function at(ByVal n As Integer) As Rational
			If n = 1 Then
				Return (New Rational(-1, 2))
			ElseIf n Mod 2 <> 0 Then
				Return Rational.ZERO
			Else
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int nindx = n / 2;
				Dim nindx As Integer = n \ 2
				If a.Count <= nindx Then
					For i As Integer = 2 * a.Count To n Step 2
						set(i, doubleSum(i))
					Next i
				End If
				Return a(nindx)
			End If
		End Function
	'     Generate a new B_n by a standard double sum.
	'     * @param n The index of the Bernoulli number.
	'     * @return The Bernoulli number at n.
	'     

		Private Function doubleSum(ByVal n As Integer) As Rational
			Dim resul As Rational = Rational.ZERO
			Dim k As Integer = 0
			Do While k <= n
				Dim jsum As Rational = Rational.ZERO
				Dim bin As BigInteger = BigInteger.One
				For j As Integer = 0 To k
					Dim jpown As BigInteger = BigInteger.valueOf(j).pow(n)
					If j Mod 2 = 0 Then
						jsum = jsum.add(bin * jpown)
					Else
						jsum = jsum.subtract(bin * jpown)
					End If
	'                 update binomial(k,j) recursively
	'                 
					bin = bin * BigInteger.valueOf(k - j).divide(BigInteger.valueOf(j + 1))
				Next j
				resul = resul.add(jsum.divide(BigInteger.valueOf(k + 1)))
				k += 1
			Loop
			Return resul
		End Function
	End Class ' Bernoulli

End Namespace