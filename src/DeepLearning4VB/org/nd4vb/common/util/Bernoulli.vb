Imports System
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

Namespace org.nd4vb.common.util

    ''' <summary>
    ''' Bernoulli numbers B_n as exact rationals, computed on demand and cached.
    ''' </summary>
    Public Class Bernoulli

        ''' <summary>Cache of the even-indexed Bernoulli numbers: a(k) = B_(2k).</summary>
        Private Shared ReadOnly a As New List(Of Rational)()
        Private Shared ReadOnly cacheLock As New Object()

        Public Sub New()
            SyncLock cacheLock
                If a.Count = 0 Then
                    a.Add(Rational.ONE)
                    a.Add(New Rational(1, 6))
                End If
            End SyncLock
        End Sub

        ''' <summary>Sets a coefficient in the internal table.</summary>
        ''' <param name="n">The zero-based index of the coefficient (must be even).</param>
        ''' <param name="value">The new value of the coefficient.</param>
        Protected Friend Overridable Sub [set](ByVal n As Integer, ByVal value As Rational)
            Dim nindx As Integer = n \ 2
            SyncLock cacheLock
                If nindx < a.Count Then
                    a(nindx) = value
                Else
                    Do While a.Count < nindx
                        a.Add(Rational.ZERO)
                    Loop
                    a.Add(value)
                End If
            End SyncLock
        End Sub

        ''' <summary>The Bernoulli number at the index provided.</summary>
        ''' <param name="n">The index, non-negative.</param>
        ''' <returns>B_0 = 1 for n = 0, B_1 = -1/2 for n = 1, B_2 = 1/6 for n = 2, etc.</returns>
        Public Overridable Function at(ByVal n As Integer) As Rational
            If n < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(n), "Index must be non-negative")
            End If
            If n = 1 Then
                Return New Rational(-1, 2)
            ElseIf n Mod 2 <> 0 Then
                Return Rational.ZERO
            Else
                Dim nindx As Integer = n \ 2
                SyncLock cacheLock
                    If a.Count <= nindx Then
                        For i As Integer = 2 * a.Count To n Step 2
                            [set](i, doubleSum(i))
                        Next i
                    End If
                    Return a(nindx)
                End SyncLock
            End If
        End Function

        ''' <summary>Generates a new B_n by the standard double sum.</summary>
        Private Function doubleSum(ByVal n As Integer) As Rational
            Dim resul As Rational = Rational.ZERO
            For k As Integer = 0 To n
                Dim jsum As Rational = Rational.ZERO
                Dim bin As BigInteger = BigInteger.One
                For j As Integer = 0 To k
                    Dim jpown As BigInteger = BigInteger.Pow(New BigInteger(j), n)
                    If j Mod 2 = 0 Then
                        jsum = jsum.add(bin * jpown)
                    Else
                        jsum = jsum.subtract(bin * jpown)
                    End If
                    ' update binomial(k, j) recursively
                    bin = BigInteger.Divide(bin * New BigInteger(k - j), New BigInteger(j + 1))
                Next j
                resul = resul.add(jsum.divide(New BigInteger(k + 1)))
            Next k
            Return resul
        End Function
    End Class

End Namespace
