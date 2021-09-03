Imports System.Collections.Generic
Imports UnivariateFunction = org.apache.commons.math3.analysis.UnivariateFunction
Imports UnivariateSolverUtils = org.apache.commons.math3.analysis.solvers.UnivariateSolverUtils
Imports NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException
Imports NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException
Imports OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException
Imports LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.linalg.api.rng.distribution


	Public MustInherit Class BaseDistribution
		Implements Distribution

		Public MustOverride ReadOnly Property SupportConnected As Boolean Implements Distribution.isSupportConnected
		Public MustOverride ReadOnly Property SupportUpperBoundInclusive As Boolean Implements Distribution.isSupportUpperBoundInclusive
		Public MustOverride ReadOnly Property SupportLowerBoundInclusive As Boolean Implements Distribution.isSupportLowerBoundInclusive
		Public MustOverride ReadOnly Property SupportUpperBound As Double Implements Distribution.getSupportUpperBound
		Public MustOverride ReadOnly Property SupportLowerBound As Double Implements Distribution.getSupportLowerBound
		Public MustOverride ReadOnly Property NumericalVariance As Double Implements Distribution.getNumericalVariance
		Public MustOverride ReadOnly Property NumericalMean As Double Implements Distribution.getNumericalMean
		Public MustOverride Function cumulativeProbability(ByVal x0 As Double, ByVal x1 As Double) As Double
		Public MustOverride Function cumulativeProbability(ByVal x As Double) As Double
		Public MustOverride Function density(ByVal x As Double) As Double
		Protected Friend random As Random
'JAVA TO VB CONVERTER NOTE: The field solverAbsoluteAccuracy was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend solverAbsoluteAccuracy_Conflict As Double


		Public Sub New(ByVal rng As Random)
			Me.random = rng
		End Sub


		Public Sub New()
			Me.New(Nd4j.Random)
		End Sub

		''' <summary>
		''' For a random variable {@code X} whose values are distributed according
		''' to this distribution, this method returns {@code P(x0 < X <= x1)}.
		''' </summary>
		''' <param name="x0"> Lower bound (excluded). </param>
		''' <param name="x1"> Upper bound (included). </param>
		''' <returns> the probability that a random variable with this distribution
		''' takes a value between {@code x0} and {@code x1}, excluding the lower
		''' and including the upper endpoint. </returns>
		''' <exception cref="org.apache.commons.math3.exception.NumberIsTooLargeException"> if {@code x0 > x1}.
		'''                                                                      <p/>
		'''                                                                      The default implementation uses the identity
		'''                                                                      {@code P(x0 < X <= x1) = P(X <= x1) - P(X <= x0)}
		''' @since 3.1 </exception>

		Public Overridable Function probability(ByVal x0 As Double, ByVal x1 As Double) As Double
			If x0 > x1 Then
				Throw New NumberIsTooLargeException(LocalizedFormats.LOWER_ENDPOINT_ABOVE_UPPER_ENDPOINT, x0, x1, True)
			End If
			Return cumulativeProbability(x1) - cumulativeProbability(x0)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The default implementation returns
		''' <ul>
		''' <li><seealso cref="getSupportLowerBound()"/> for {@code p = 0},</li>
		''' <li><seealso cref="getSupportUpperBound()"/> for {@code p = 1}.</li>
		''' </ul>
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public double inverseCumulativeProbability(final double p) throws org.apache.commons.math3.exception.OutOfRangeException
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Overridable Function inverseCumulativeProbability(ByVal p As Double) As Double Implements Distribution.inverseCumulativeProbability
	'        
	'         * IMPLEMENTATION NOTES
	'         * --------------------
	'         * Where applicable, use is made of the one-sided Chebyshev inequality
	'         * to bracket the root. This inequality states that
	'         * P(X - mu >= k * sig) <= 1 / (1 + k^2),
	'         * mu: mean, sig: standard deviation. Equivalently
	'         * 1 - P(X < mu + k * sig) <= 1 / (1 + k^2),
	'         * F(mu + k * sig) >= k^2 / (1 + k^2).
	'         *
	'         * For k = sqrt(p / (1 - p)), we find
	'         * F(mu + k * sig) >= p,
	'         * and (mu + k * sig) is an upper-bound for the root.
	'         *
	'         * Then, introducing Y = -X, mean(Y) = -mu, sd(Y) = sig, and
	'         * P(Y >= -mu + k * sig) <= 1 / (1 + k^2),
	'         * P(-X >= -mu + k * sig) <= 1 / (1 + k^2),
	'         * P(X <= mu - k * sig) <= 1 / (1 + k^2),
	'         * F(mu - k * sig) <= 1 / (1 + k^2).
	'         *
	'         * For k = sqrt((1 - p) / p), we find
	'         * F(mu - k * sig) <= p,
	'         * and (mu - k * sig) is a lower-bound for the root.
	'         *
	'         * In cases where the Chebyshev inequality does not apply, geometric
	'         * progressions 1, 2, 4, ... and -1, -2, -4, ... are used to bracket
	'         * the root.
	'         
			If p < 0.0 OrElse p > 1.0 Then
				Throw New OutOfRangeException(p, 0, 1)
			End If

			Dim lowerBound As Double = SupportLowerBound
			If p = 0.0 Then
				Return lowerBound
			End If

			Dim upperBound As Double = SupportUpperBound
			If p = 1.0 Then
				Return upperBound
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double mu = getNumericalMean();
			Dim mu As Double = NumericalMean
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double sig = org.apache.commons.math3.util.FastMath.sqrt(getNumericalVariance());
			Dim sig As Double = FastMath.sqrt(NumericalVariance)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final boolean chebyshevApplies;
			Dim chebyshevApplies As Boolean
			chebyshevApplies = Not (Double.IsInfinity(mu) OrElse Double.IsNaN(mu) OrElse Double.IsInfinity(sig) OrElse Double.IsNaN(sig))

			If lowerBound = Double.NegativeInfinity Then
				If chebyshevApplies Then
					lowerBound = mu - sig * FastMath.sqrt((1.0 - p) / p)
				Else
					lowerBound = -1.0
					Do While cumulativeProbability(lowerBound) >= p
						lowerBound *= 2.0
					Loop
				End If
			End If

			If upperBound = Double.PositiveInfinity Then
				If chebyshevApplies Then
					upperBound = mu + sig * FastMath.sqrt(p / (1.0 - p))
				Else
					upperBound = 1.0
					Do While cumulativeProbability(upperBound) < p
						upperBound *= 2.0
					Loop
				End If
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.commons.math3.analysis.UnivariateFunction toSolve = new org.apache.commons.math3.analysis.UnivariateFunction()
			Dim toSolve As UnivariateFunction = New UnivariateFunctionAnonymousInnerClass(Me, p)

			Dim x As Double = UnivariateSolverUtils.solve(toSolve, lowerBound, upperBound, SolverAbsoluteAccuracy)

			If Not SupportConnected Then
				' Test for plateau. 
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double dx = getSolverAbsoluteAccuracy();
				Dim dx As Double = SolverAbsoluteAccuracy
				If x - dx >= SupportLowerBound Then
					Dim px As Double = cumulativeProbability(x)
					If cumulativeProbability(x - dx) = px Then
						upperBound = x
						Do While upperBound - lowerBound > dx
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double midPoint = 0.5 * (lowerBound + upperBound);
							Dim midPoint As Double = 0.5 * (lowerBound + upperBound)
							If cumulativeProbability(midPoint) < px Then
								lowerBound = midPoint
							Else
								upperBound = midPoint
							End If
						Loop
						Return upperBound
					End If
				End If
			End If
			Return x
		End Function

		Private Class UnivariateFunctionAnonymousInnerClass
			Inherits UnivariateFunction

			Private ReadOnly outerInstance As BaseDistribution

			Private p As Double

			Public Sub New(ByVal outerInstance As BaseDistribution, ByVal p As Double)
				Me.outerInstance = outerInstance
				Me.p = p
			End Sub


'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public double value(final double x)
			Public Function value(ByVal x As Double) As Double
				Return outerInstance.cumulativeProbability(x) - p
			End Function
		End Class

		''' <summary>
		''' Returns the solver absolute accuracy for inverse cumulative computation.
		''' You can override this method in order to use a Brent solver with an
		''' absolute accuracy different from the default.
		''' </summary>
		''' <returns> the maximum absolute error in inverse cumulative probability estimates </returns>
		Protected Friend Overridable ReadOnly Property SolverAbsoluteAccuracy As Double
			Get
				Return solverAbsoluteAccuracy_Conflict
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Sub reseedRandomGenerator(ByVal seed As Long) Implements Distribution.reseedRandomGenerator
			random.Seed = seed
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The default implementation uses the
		''' <a href="http://en.wikipedia.org/wiki/Inverse_transform_sampling">
		''' inversion method.
		''' </a>
		''' </summary>
		Public Overridable Function sample() As Double Implements Distribution.sample
			Return inverseCumulativeProbability(random.nextDouble())
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The default implementation generates the sample by calling
		''' <seealso cref="sample()"/> in a loop.
		''' </summary>
		Public Overridable Function sample(ByVal sampleSize As Long) As Double() Implements Distribution.sample
			If sampleSize <= 0 Then
				Throw New NotStrictlyPositiveException(LocalizedFormats.NUMBER_OF_SAMPLES, sampleSize)
			End If
			Dim [out](CInt(sampleSize) - 1) As Double
			For i As Integer = 0 To sampleSize - 1
				[out](i) = sample()
			Next i
			Return [out]
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' <returns> zero.
		''' @since 3.1 </returns>
		Public Overridable Function probability(ByVal x As Double) As Double Implements Distribution.probability
			Return 0R
		End Function

		Public Overridable Function sample(ByVal shape() As Integer) As INDArray Implements Distribution.sample
			Dim ret As INDArray = Nd4j.create(shape)
			Return sample(ret)
		End Function

		Public Overridable Function sample(ByVal shape() As Long) As INDArray Implements Distribution.sample
			Dim ret As INDArray = Nd4j.create(shape)
			Return sample(ret)
		End Function

		Public Overridable Function sample(ByVal target As INDArray) As INDArray Implements Distribution.sample
			Dim idxIter As IEnumerator(Of Long()) = New NdIndexIterator(target.shape()) 'For consistent values irrespective of c vs. fortran ordering
			Dim len As Long = target.length()
			For i As Long = 0 To len - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				target.putScalar(idxIter.next(), sample())
			Next i
			Return target
		End Function
	End Class

End Namespace