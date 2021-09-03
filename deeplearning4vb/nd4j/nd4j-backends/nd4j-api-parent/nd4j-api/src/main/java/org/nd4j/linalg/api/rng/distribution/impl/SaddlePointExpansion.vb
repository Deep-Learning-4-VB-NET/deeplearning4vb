Imports System
Imports Gamma = org.apache.commons.math3.special.Gamma
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports MathUtils = org.apache.commons.math3.util.MathUtils

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

Namespace org.nd4j.linalg.api.rng.distribution.impl

	Public Class SaddlePointExpansion

		''' <summary>
		''' 1/2 * log(2 &#960;).
		''' </summary>
		Private Shared ReadOnly HALF_LOG_2_PI As Double = 0.5 * FastMath.log(MathUtils.TWO_PI)

		''' <summary>
		''' exact Stirling expansion error for certain values.
		''' </summary>
		Private Shared ReadOnly EXACT_STIRLING_ERRORS() As Double = {0.0, 0.1534264097200273452913848, 0.0810614667953272582196702, 0.0548141210519176538961390, 0.0413406959554092940938221, 0.03316287351993628748511048, 0.02767792568499833914878929, 0.02374616365629749597132920, 0.02079067210376509311152277, 0.01848845053267318523077934, 0.01664469118982119216319487, 0.01513497322191737887351255, 0.01387612882307074799874573, 0.01281046524292022692424986, 0.01189670994589177009505572, 0.01110455975820691732662991, 0.010411265261972096497478567, 0.009799416126158803298389475, 0.009255462182712732917728637, 0.008768700134139385462952823, 0.008330563433362871256469318, 0.007934114564314020547248100, 0.007573675487951840794972024, 0.007244554301320383179543912, 0.006942840107209529865664152, 0.006665247032707682442354394, 0.006408994188004207068439631, 0.006171712263039457647532867, 0.005951370112758847735624416, 0.005746216513010115682023589, 0.005554733551962801371038690 }

		''' <summary>
		''' Default constructor.
		''' </summary>
		Private Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Compute the error of Stirling's series at the given value.
		''' <para>
		''' References:
		''' <ol>
		''' <li>Eric W. Weisstein. "Stirling's Series." From MathWorld--A Wolfram Web
		''' Resource. <a target="_blank"
		''' href="http://mathworld.wolfram.com/StirlingsSeries.html">
		''' http://mathworld.wolfram.com/StirlingsSeries.html</a></li>
		''' </ol>
		''' </para>
		''' </summary>
		''' <param name="z"> the value. </param>
		''' <returns> the Striling's series error. </returns>
		Public Shared Function getStirlingError(ByVal z As Double) As Double
			Dim ret As Double
			If z < 15.0 Then
				Dim z2 As Double = 2.0 * z
				If FastMath.floor(z2) = z2 Then
					ret = EXACT_STIRLING_ERRORS(CInt(Math.Truncate(z2)))
				Else
					ret = Gamma.logGamma(z + 1.0) - (z + 0.5) * FastMath.log(z) + z - HALF_LOG_2_PI
				End If
			Else
				Dim z2 As Double = z * z
				ret = (0.083333333333333333333 - (0.00277777777777777777778 - (0.00079365079365079365079365 - (0.000595238095238095238095238 - 0.0008417508417508417508417508 / z2) / z2) / z2) / z2) / z
			End If
			Return ret
		End Function

		''' <summary>
		''' A part of the deviance portion of the saddle point approximation.
		''' <para>
		''' References:
		''' <ol>
		''' <li>Catherine Loader (2000). "Fast and Accurate Computation of Binomial
		''' Probabilities.". <a target="_blank"
		''' href="http://www.herine.net/stat/papers/dbinom.pdf">
		''' http://www.herine.net/stat/papers/dbinom.pdf</a></li>
		''' </ol>
		''' </para>
		''' </summary>
		''' <param name="x">  the x value. </param>
		''' <param name="mu"> the average. </param>
		''' <returns> a part of the deviance. </returns>
		Public Shared Function getDeviancePart(ByVal x As Double, ByVal mu As Double) As Double
			Dim ret As Double
			If FastMath.abs(x - mu) < 0.1 * (x + mu) Then
				Dim d As Double = x - mu
				Dim v As Double = d / (x + mu)
				Dim s1 As Double = v * d
				Dim s As Double = Double.NaN
				Dim ej As Double = 2.0 * x * v
				v = v * v
				Dim j As Integer = 1
				Do While s1 <> s
					s = s1
					ej *= v
					s1 = s + ej / ((j * 2) + 1)
					j += 1
				Loop
				ret = s1
			Else
				ret = x * FastMath.log(x / mu) + mu - x
			End If
			Return ret
		End Function

		''' <summary>
		''' Compute the logarithm of the PMF for a binomial distribution
		''' using the saddle point expansion.
		''' </summary>
		''' <param name="x"> the value at which the probability is evaluated. </param>
		''' <param name="n"> the number of trials. </param>
		''' <param name="p"> the probability of success. </param>
		''' <param name="q"> the probability of failure (1 - p). </param>
		''' <returns> log(p(x)). </returns>
		Public Shared Function logBinomialProbability(ByVal x As Integer, ByVal n As Integer, ByVal p As Double, ByVal q As Double) As Double
			Dim ret As Double
			If x = 0 Then
				If p < 0.1 Then
					ret = -getDeviancePart(n, n * q) - n * p
				Else
					ret = n * FastMath.log(q)
				End If
			ElseIf x = n Then
				If q < 0.1 Then
					ret = -getDeviancePart(n, n * p) - n * q
				Else
					ret = n * FastMath.log(p)
				End If
			Else
				ret = getStirlingError(n) - getStirlingError(x) - getStirlingError(n - x) - getDeviancePart(x, n * p) - getDeviancePart(n - x, n * q)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Dim f As Double = (MathUtils.TWO_PI * x * (n - x)) / n
				ret = -0.5 * FastMath.log(f) + ret
			End If
			Return ret
		End Function
	End Class

End Namespace