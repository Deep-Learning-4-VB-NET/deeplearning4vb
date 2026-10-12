Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException
Imports OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ConstantDistribution extends org.nd4j.linalg.api.rng.distribution.BaseDistribution
	Public Class ConstantDistribution
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
		''' Mean of this distribution.
		''' </summary>
		Private value As Double

		Public Sub New(ByVal value As Double)
			Me.value = value
			Me.random = Nd4j.Random
		End Sub
	'
	'    max doesn't want this distripution
	'    public OrthogonalDistribution(@NonNull INDArray gains) {
	'        this.gains = gains;
	'        this.random = Nd4j.getRandom();
	'    }
	'
		''' <summary>
		''' Access the mean.
		''' </summary>
		''' <returns> the mean for this distribution. </returns>
		Public Overridable ReadOnly Property Mean As Double
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

		''' <summary>
		''' Access the standard deviation.
		''' </summary>
		''' <returns> the standard deviation for this distribution. </returns>
		Public Overridable ReadOnly Property StandardDeviation As Double
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function density(ByVal x As Double) As Double
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' If {@code x} is more than 40 standard deviations from the mean, 0 or 1
		''' is returned, as in these cases the actual value is within
		''' {@code Double.MIN_VALUE} of 0 or 1.
		''' </summary>
		Public Overrides Function cumulativeProbability(ByVal x As Double) As Double
			Throw New System.NotSupportedException()
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
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		''' @deprecated See <seealso cref="org.apache.commons.math3.distribution.RealDistribution.cumulativeProbability(Double, Double)"/> 
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override @Deprecated public double cumulativeProbability(double x0, double x1) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		<Obsolete>
		Public Overrides Function cumulativeProbability(ByVal x0 As Double, ByVal x1 As Double) As Double
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public double probability(double x0, double x1) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		Public Overrides Function probability(ByVal x0 As Double, ByVal x1 As Double) As Double
			Throw New System.NotSupportedException()
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Protected Friend Overrides ReadOnly Property SolverAbsoluteAccuracy As Double
			Get
				Throw New System.NotSupportedException()
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
			Throw New System.NotSupportedException()
		End Function

		Public Overrides Function sample(ByVal shape() As Integer) As INDArray
			Return sample(Nd4j.createUninitialized(shape))
		End Function


		Public Overrides Function sample(ByVal target As INDArray) As INDArray
			Return target.assign(value)
		End Function
	End Class

End Namespace