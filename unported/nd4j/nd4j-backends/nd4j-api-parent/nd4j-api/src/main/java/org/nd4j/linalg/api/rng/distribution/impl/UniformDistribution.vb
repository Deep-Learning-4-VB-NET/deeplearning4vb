Imports val = lombok.val
Imports NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException
Imports OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException
Imports LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
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

	Public Class UniformDistribution
		Inherits BaseDistribution

		Private upper, lower As Double

		''' <summary>
		''' Create a uniform real distribution using the given lower and upper
		''' bounds.
		''' </summary>
		''' <param name="lower"> Lower bound of this distribution (inclusive). </param>
		''' <param name="upper"> Upper bound of this distribution (exclusive). </param>
		''' <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}. </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public UniformDistribution(double lower, double upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		Public Sub New(ByVal lower As Double, ByVal upper As Double)
			Me.New(Nd4j.Random, lower, upper)
		End Sub


		''' <summary>
		''' Creates a uniform distribution.
		''' </summary>
		''' <param name="rng">   Random number generator. </param>
		''' <param name="lower"> Lower bound of this distribution (inclusive). </param>
		''' <param name="upper"> Upper bound of this distribution (exclusive). </param>
		''' <exception cref="NumberIsTooLargeException"> if {@code lower >= upper}.
		''' @since 3.1 </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public UniformDistribution(org.nd4j.linalg.api.rng.Random rng, double lower, double upper) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		Public Sub New(ByVal rng As org.nd4j.linalg.api.rng.Random, ByVal lower As Double, ByVal upper As Double)
			MyBase.New(rng)
			If lower >= upper Then
				Throw New NumberIsTooLargeException(LocalizedFormats.LOWER_BOUND_NOT_BELOW_UPPER_BOUND, lower, upper, False)
			End If

			Me.lower = lower
			Me.upper = upper
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function density(ByVal x As Double) As Double
			If x < lower OrElse x > upper Then
				Return 0.0
			End If
			Return 1 / (upper - lower)
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Function cumulativeProbability(ByVal x As Double) As Double
			If x <= lower Then
				Return 0
			End If
			If x >= upper Then
				Return 1
			End If
			Return (x - lower) / (upper - lower)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public double cumulativeProbability(double x0, double x1) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		Public Overrides Function cumulativeProbability(ByVal x0 As Double, ByVal x1 As Double) As Double
			Return 0
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public double inverseCumulativeProbability(final double p) throws org.apache.commons.math3.exception.OutOfRangeException
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Overrides Function inverseCumulativeProbability(ByVal p As Double) As Double
			If p < 0.0 OrElse p > 1.0 Then
				Throw New OutOfRangeException(p, 0, 1)
			End If
			Return p * (upper - lower) + lower
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' For lower bound {@code lower} and upper bound {@code upper}, the mean is
		''' {@code 0.5 * (lower + upper)}.
		''' </summary>
		Public Overrides ReadOnly Property NumericalMean As Double
			Get
				Return 0.5 * (lower + upper)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' For lower bound {@code lower} and upper bound {@code upper}, the
		''' variance is {@code (upper - lower)^2 / 12}.
		''' </summary>
		Public Overrides ReadOnly Property NumericalVariance As Double
			Get
				Dim ul As Double = upper - lower
				Return ul * ul / 12
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The lower bound of the support is equal to the lower bound parameter
		''' of the distribution.
		''' </summary>
		''' <returns> lower bound of the support </returns>
		Public Overrides ReadOnly Property SupportLowerBound As Double
			Get
				Return lower
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The upper bound of the support is equal to the upper bound parameter
		''' of the distribution.
		''' </summary>
		''' <returns> upper bound of the support </returns>
		Public Overrides ReadOnly Property SupportUpperBound As Double
			Get
				Return upper
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides ReadOnly Property SupportLowerBoundInclusive As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides ReadOnly Property SupportUpperBoundInclusive As Boolean
			Get
				Return True
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
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double u = random.nextDouble();
			Dim u As Double = random.nextDouble()
			Return u * upper + (1 - u) * lower
		End Function


		Public Overrides Function sample(ByVal shape() As Integer) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray ret = org.nd4j.linalg.factory.Nd4j.createUninitialized(shape, org.nd4j.linalg.factory.Nd4j.order());
			Dim ret As INDArray = Nd4j.createUninitialized(shape, Nd4j.order())
			Return sample(ret)
		End Function

		Public Overrides Function sample(ByVal ret As INDArray) As INDArray
			If random.StatePointer IsNot Nothing Then
				Return Nd4j.Executioner.exec(New org.nd4j.linalg.api.ops.random.impl.UniformDistribution(ret, lower, upper), random)
			Else
				Dim idxIter As val = New NdIndexIterator(ret.shape()) 'For consistent values irrespective of c vs. fortran ordering
				Dim len As Long = ret.length()
				For i As Integer = 0 To len - 1
					ret.putScalar(idxIter.next(), sample())
				Next i
				Return ret
			End If
		End Function
	End Class

End Namespace