Imports System
Imports NotStrictlyPositiveException = org.apache.commons.math3.exception.NotStrictlyPositiveException
Imports NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException
Imports OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException
Imports LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats
Imports Erf = org.apache.commons.math3.special.Erf
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Random = org.nd4j.linalg.api.rng.Random
Imports BaseDistribution = org.nd4j.linalg.api.rng.distribution.BaseDistribution
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

Namespace org.nd4j.linalg.api.rng.distribution.impl

	Public Class LogNormalDistribution
		Inherits BaseDistribution

		''' <summary>
		''' Default inverse cumulative probability accuracy.
		''' 
		''' @since 2.1
		''' </summary>
		Public Const DEFAULT_INVERSE_ABSOLUTE_ACCURACY As Double = 1e-9
		''' <summary>
		''' Serializable version identifier.
		''' </summary>
		Private Const serialVersionUID As Long = 8589540077390120676L
		''' <summary>
		''' &radic;(2 &pi;)
		''' </summary>
		Private Shared ReadOnly SQRT2PI As Double = FastMath.sqrt(2 * FastMath.PI)
		''' <summary>
		''' &radic;(2)
		''' </summary>
		Private Shared ReadOnly SQRT2 As Double = FastMath.sqrt(2.0)
		''' <summary>
		''' Standard deviation of this distribution.
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field standardDeviation was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly standardDeviation_Conflict As Double
		''' <summary>
		''' Mean of this distribution.
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field mean was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private mean_Conflict As Double
		Private means As INDArray
		''' <summary>
		''' Inverse cumulative probability accuracy.
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field solverAbsoluteAccuracy was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shadows solverAbsoluteAccuracy_Conflict As Double

		Public Sub New(ByVal rng As Random, ByVal standardDeviation As Double, ByVal means As INDArray)
			MyBase.New(rng)
			Me.standardDeviation_Conflict = standardDeviation
			Me.means = means
		End Sub

		Public Sub New(ByVal standardDeviation As Double, ByVal means As INDArray)
			Me.standardDeviation_Conflict = standardDeviation
			Me.means = means
		End Sub

		''' <summary>
		''' Create a normal distribution with mean equal to zero and standard
		''' deviation equal to one.
		''' </summary>
		Public Sub New()
			Me.New(0, 1)
		End Sub

		''' <summary>
		''' Create a normal distribution using the given mean and standard deviation.
		''' </summary>
		''' <param name="mean"> Mean for this distribution. </param>
		''' <param name="sd">   Standard deviation for this distribution. </param>
		''' <exception cref="NotStrictlyPositiveException"> if {@code sd <= 0}. </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public LogNormalDistribution(double mean, double sd) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		Public Sub New(ByVal mean As Double, ByVal sd As Double)
			Me.New(mean, sd, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public LogNormalDistribution(org.nd4j.linalg.api.rng.Random rng, double mean, double sd) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		Public Sub New(ByVal rng As Random, ByVal mean As Double, ByVal sd As Double)
			Me.New(rng, mean, sd, DEFAULT_INVERSE_ABSOLUTE_ACCURACY)
		End Sub

		''' <summary>
		''' Create a normal distribution using the given mean, standard deviation and
		''' inverse cumulative distribution accuracy.
		''' </summary>
		''' <param name="mean">               Mean for this distribution. </param>
		''' <param name="sd">                 Standard deviation for this distribution. </param>
		''' <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
		''' <exception cref="NotStrictlyPositiveException"> if {@code sd <= 0}.
		''' @since 2.1 </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public LogNormalDistribution(double mean, double sd, double inverseCumAccuracy) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		Public Sub New(ByVal mean As Double, ByVal sd As Double, ByVal inverseCumAccuracy As Double)
			Me.New(Nd4j.Random, mean, sd, inverseCumAccuracy)
		End Sub

		''' <summary>
		''' Creates a normal distribution.
		''' </summary>
		''' <param name="rng">                Random number generator. </param>
		''' <param name="mean">               Mean for this distribution. </param>
		''' <param name="sd">                 Standard deviation for this distribution. </param>
		''' <param name="inverseCumAccuracy"> Inverse cumulative probability accuracy. </param>
		''' <exception cref="NotStrictlyPositiveException"> if {@code sd <= 0}.
		''' @since 3.1 </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public LogNormalDistribution(org.nd4j.linalg.api.rng.Random rng, double mean, double sd, double inverseCumAccuracy) throws org.apache.commons.math3.exception.NotStrictlyPositiveException
		Public Sub New(ByVal rng As Random, ByVal mean As Double, ByVal sd As Double, ByVal inverseCumAccuracy As Double)
			MyBase.New(rng)

			If sd <= 0 Then
				Throw New NotStrictlyPositiveException(LocalizedFormats.STANDARD_DEVIATION, sd)
			End If

			Me.mean_Conflict = mean
			standardDeviation_Conflict = sd
			solverAbsoluteAccuracy_Conflict = inverseCumAccuracy
		End Sub

		Public Sub New(ByVal mean As INDArray, ByVal std As Double)
			Me.means = mean
			Me.standardDeviation_Conflict = std
			Me.random = Nd4j.Random
		End Sub

		''' <summary>
		''' Access the mean.
		''' </summary>
		''' <returns> the mean for this distribution. </returns>
		Public Overridable ReadOnly Property Mean As Double
			Get
				Return mean_Conflict
			End Get
		End Property

		''' <summary>
		''' Access the standard deviation.
		''' </summary>
		''' <returns> the standard deviation for this distribution. </returns>
		Public Overridable ReadOnly Property StandardDeviation As Double
			Get
				Return standardDeviation_Conflict
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function density(ByVal x As Double) As Double
			If means IsNot Nothing Then
				Throw New System.InvalidOperationException("Unable to sample from more than one mean")
			End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double x0 = x - mean;
			Dim x0 As Double = x - mean_Conflict
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double x1 = x0 / standardDeviation;
			Dim x1 As Double = x0 / standardDeviation_Conflict
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			Return FastMath.exp(-0.5 * x1 * x1) / (standardDeviation_Conflict * SQRT2PI)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' If {@code x} is more than 40 standard deviations from the mean, 0 or 1
		''' is returned, as in these cases the actual value is within
		''' {@code Double.MIN_VALUE} of 0 or 1.
		''' </summary>
		Public Overrides Function cumulativeProbability(ByVal x As Double) As Double
			If means IsNot Nothing Then
				Throw New System.InvalidOperationException("Unable to sample from more than one mean")
			End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double dev = x - mean;
			Dim dev As Double = x - mean_Conflict
			If FastMath.abs(dev) > 40 * standardDeviation_Conflict Then
				Return If(dev < 0, 0.0R, 1.0R)
			End If
			Return 0.5 * (1 + Erf.erf(dev / (standardDeviation_Conflict * SQRT2)))
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 3.2
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public double inverseCumulativeProbability(final double p) throws org.apache.commons.math3.exception.OutOfRangeException
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Overrides Function inverseCumulativeProbability(ByVal p As Double) As Double
			If p < 0.0 OrElse p > 1.0 Then
				Throw New OutOfRangeException(p, 0, 1)
			End If
			If means IsNot Nothing Then
				Throw New System.InvalidOperationException("Unable to sample from more than one mean")
			End If

			Return mean_Conflict + standardDeviation_Conflict * SQRT2 * Erf.erfInv(2 * p - 1)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' @deprecated See <seealso cref="org.apache.commons.math3.distribution.RealDistribution.cumulativeProbability(Double, Double)"/> 
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override @Deprecated public double cumulativeProbability(double x0, double x1) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		<Obsolete>
		Public Overrides Function cumulativeProbability(ByVal x0 As Double, ByVal x1 As Double) As Double
			Return probability(x0, x1)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public double probability(double x0, double x1) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		Public Overrides Function probability(ByVal x0 As Double, ByVal x1 As Double) As Double
			If x0 > x1 Then
				Throw New NumberIsTooLargeException(LocalizedFormats.LOWER_ENDPOINT_ABOVE_UPPER_ENDPOINT, x0, x1, True)
			End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double denom = standardDeviation * SQRT2;
			Dim denom As Double = standardDeviation_Conflict * SQRT2
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double v0 = (x0 - mean) / denom;
			Dim v0 As Double = (x0 - mean_Conflict) / denom
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double v1 = (x1 - mean) / denom;
			Dim v1 As Double = (x1 - mean_Conflict) / denom
			Return 0.5 * Erf.erf(v0, v1)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides ReadOnly Property SolverAbsoluteAccuracy As Double
			Get
				Return solverAbsoluteAccuracy_Conflict
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' For mean parameter {@code mu}, the mean is {@code mu}.
		''' </summary>
		Public Overrides ReadOnly Property NumericalMean As Double
			Get
				Return Mean
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' For standard deviation parameter {@code s}, the variance is {@code s^2}.
		''' </summary>
		Public Overrides ReadOnly Property NumericalVariance As Double
			Get
	'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
	'ORIGINAL LINE: final double s = getStandardDeviation();
				Dim s As Double = StandardDeviation
				Return s * s
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The lower bound of the support is always negative infinity
		''' no matter the parameters.
		''' </summary>
		''' <returns> lower bound of the support (always
		''' {@code Double.NEGATIVE_INFINITY}) </returns>
		Public Overrides ReadOnly Property SupportLowerBound As Double
			Get
				Return Double.NegativeInfinity
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The upper bound of the support is always positive infinity
		''' no matter the parameters.
		''' </summary>
		''' <returns> upper bound of the support (always
		''' {@code Double.POSITIVE_INFINITY}) </returns>
		Public Overrides ReadOnly Property SupportUpperBound As Double
			Get
				Return Double.PositiveInfinity
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides ReadOnly Property SupportLowerBoundInclusive As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides ReadOnly Property SupportUpperBoundInclusive As Boolean
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The support of this distribution is connected.
		''' </summary>
		''' <returns> {@code true} </returns>
		Public Overrides ReadOnly Property SupportConnected As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function sample() As Double
			If means IsNot Nothing Then
				Throw New System.InvalidOperationException("Unable to sample from more than one mean")
			End If
			Return standardDeviation_Conflict * random.nextGaussian() + mean_Conflict
		End Function

		Public Overrides Function sample(ByVal shape() As Integer) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray ret = org.nd4j.linalg.factory.Nd4j.createUninitialized(shape, org.nd4j.linalg.factory.Nd4j.order());
			Dim ret As INDArray = Nd4j.createUninitialized(shape, Nd4j.order())
			Return sample(ret)
		End Function

		Public Overrides Function sample(ByVal ret As INDArray) As INDArray
			If means IsNot Nothing Then
				Return Nd4j.Executioner.exec(New org.nd4j.linalg.api.ops.random.impl.LogNormalDistribution(ret, means, standardDeviation_Conflict), random)
			Else
				Return Nd4j.Executioner.exec(New org.nd4j.linalg.api.ops.random.impl.LogNormalDistribution(ret, mean_Conflict, standardDeviation_Conflict), random)
			End If
		End Function
	End Class

End Namespace