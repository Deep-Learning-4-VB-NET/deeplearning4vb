Imports System.Collections.Generic
Imports NotPositiveException = org.apache.commons.math3.exception.NotPositiveException
Imports NumberIsTooLargeException = org.apache.commons.math3.exception.NumberIsTooLargeException
Imports OutOfRangeException = org.apache.commons.math3.exception.OutOfRangeException
Imports LocalizedFormats = org.apache.commons.math3.exception.util.LocalizedFormats
Imports RandomGenerator = org.apache.commons.math3.random.RandomGenerator
Imports Beta = org.apache.commons.math3.special.Beta
Imports FastMath = org.apache.commons.math3.util.FastMath
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
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


	Public Class BinomialDistribution
		Inherits BaseDistribution

		''' <summary>
		''' The number of trials.
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field numberOfTrials was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly numberOfTrials_Conflict As Integer
		''' <summary>
		''' The probability of success.
		''' </summary>
'JAVA TO VB CONVERTER NOTE: The field probabilityOfSuccess was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private probabilityOfSuccess_Conflict As Double

		Private p As INDArray

		''' <summary>
		''' Create a binomial distribution with the given number of trials and
		''' probability of success.
		''' </summary>
		''' <param name="trials"> Number of trials. </param>
		''' <param name="p">      Probability of success. </param>
		''' <exception cref="org.apache.commons.math3.exception.NotPositiveException"> if {@code trials < 0}. </exception>
		''' <exception cref="org.apache.commons.math3.exception.OutOfRangeException">  if {@code p < 0} or {@code p > 1}. </exception>
		Public Sub New(ByVal trials As Integer, ByVal p As Double)
			Me.New(Nd4j.Random, trials, p)
		End Sub

		''' <summary>
		''' Creates a binomial distribution.
		''' </summary>
		''' <param name="rng">    Random number generator. </param>
		''' <param name="trials"> Number of trials. </param>
		''' <param name="p">      Probability of success. </param>
		''' <exception cref="org.apache.commons.math3.exception.NotPositiveException"> if {@code trials < 0}. </exception>
		''' <exception cref="org.apache.commons.math3.exception.OutOfRangeException">  if {@code p < 0} or {@code p > 1}.
		''' @since 3.1 </exception>
		Public Sub New(ByVal rng As Random, ByVal trials As Integer, ByVal p As Double)
			MyBase.New(rng)

			If trials < 0 Then
				Throw New NotPositiveException(LocalizedFormats.NUMBER_OF_TRIALS, trials)
			End If
			If p < 0 OrElse p > 1 Then
				Throw New OutOfRangeException(p, 0, 1)
			End If

			probabilityOfSuccess_Conflict = p
			numberOfTrials_Conflict = trials
		End Sub

		Public Sub New(ByVal n As Integer, ByVal p As INDArray)
			Me.random = Nd4j.Random
			Me.numberOfTrials_Conflict = n
			Me.p = p
		End Sub

		''' <summary>
		''' Access the number of trials for this distribution.
		''' </summary>
		''' <returns> the number of trials. </returns>
		Public Overridable ReadOnly Property NumberOfTrials As Integer
			Get
				Return numberOfTrials_Conflict
			End Get
		End Property

		''' <summary>
		''' Access the probability of success for this distribution.
		''' </summary>
		''' <returns> the probability of success. </returns>
		Public Overridable ReadOnly Property ProbabilityOfSuccess As Double
			Get
				Return probabilityOfSuccess_Conflict
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Overloads Function probability(ByVal x As Integer) As Double

			Dim ret As Double
			If x < 0 OrElse x > numberOfTrials_Conflict Then
				ret = 0.0
			Else
				ret = FastMath.exp(SaddlePointExpansion.logBinomialProbability(x, numberOfTrials_Conflict, probabilityOfSuccess_Conflict, 1.0 - probabilityOfSuccess_Conflict))
			End If
			Return ret
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overridable Overloads Function cumulativeProbability(ByVal x As Integer) As Double

			Dim ret As Double
			If x < 0 Then
				ret = 0.0
			ElseIf x >= numberOfTrials_Conflict Then
				ret = 1.0
			Else
				ret = 1.0 - Beta.regularizedBeta(probabilityOfSuccess_Conflict, x + 1.0, numberOfTrials_Conflict - x)
			End If
			Return ret
		End Function

		Public Overrides Function density(ByVal x As Double) As Double
			Return 0
		End Function

		Public Overrides Function cumulativeProbability(ByVal x As Double) As Double

			Dim ret As Double
			If x < 0 Then
				ret = 0.0R
			ElseIf x >= Me.numberOfTrials_Conflict Then
				ret = 1.0R
			Else
				ret = 1.0R - Beta.regularizedBeta(Me.probabilityOfSuccess_Conflict, x + 1.0R, (Me.numberOfTrials_Conflict - x))
			End If

			Return ret
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public double cumulativeProbability(double x0, double x1) throws org.apache.commons.math3.exception.NumberIsTooLargeException
		Public Overrides Function cumulativeProbability(ByVal x0 As Double, ByVal x1 As Double) As Double
			Return 0
		End Function

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' For {@code n} trials and probability parameter {@code p}, the mean is
		''' {@code n * p}.
		''' </summary>
		Public Overrides ReadOnly Property NumericalMean As Double
			Get
    
				Return numberOfTrials_Conflict * probabilityOfSuccess_Conflict
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' For {@code n} trials and probability parameter {@code p}, the variance is
		''' {@code n * p * (1 - p)}.
		''' </summary>
		Public Overrides ReadOnly Property NumericalVariance As Double
			Get
    
	'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
	'ORIGINAL LINE: final double p = probabilityOfSuccess;
				Dim p As Double = probabilityOfSuccess_Conflict
				Return numberOfTrials_Conflict * p * (1 - p)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The lower bound of the support is always 0 except for the probability
		''' parameter {@code p = 1}.
		''' </summary>
		''' <returns> lower bound of the support (0 or the number of trials) </returns>
		Public Overrides ReadOnly Property SupportLowerBound As Double
			Get
    
				Return If(probabilityOfSuccess_Conflict < 1.0, 0, numberOfTrials_Conflict)
			End Get
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' <p/>
		''' The upper bound of the support is the number of trials except for the
		''' probability parameter {@code p = 0}.
		''' </summary>
		''' <returns> upper bound of the support (number of trials or 0) </returns>
		Public Overrides ReadOnly Property SupportUpperBound As Double
			Get
    
				Return If(probabilityOfSuccess_Conflict > 0.0, numberOfTrials_Conflict, 0)
			End Get
		End Property

		Public Overrides ReadOnly Property SupportLowerBoundInclusive As Boolean
			Get
				Return False
			End Get
		End Property

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


		Private Sub ensureConsistent(ByVal i As Integer)
			probabilityOfSuccess_Conflict = p.reshape(ChrW(-1)).getDouble(i)
		End Sub

		Public Overrides Function sample(ByVal shape() As Integer) As INDArray
			Dim ret As INDArray = Nd4j.createUninitialized(shape, Nd4j.order())
			Return sample(ret)
		End Function

		Public Overrides Function sample(ByVal ret As INDArray) As INDArray
			If random.StatePointer IsNot Nothing Then
				If p IsNot Nothing Then
					Return Nd4j.Executioner.exec(New org.nd4j.linalg.api.ops.random.impl.BinomialDistributionEx(ret, numberOfTrials_Conflict, p), random)
				Else
					Return Nd4j.Executioner.exec(New org.nd4j.linalg.api.ops.random.impl.BinomialDistributionEx(ret, numberOfTrials_Conflict, probabilityOfSuccess_Conflict), random)
				End If
			Else
				Dim idxIter As IEnumerator(Of Long()) = New NdIndexIterator(ret.shape()) 'For consistent values irrespective of c vs. fortran ordering
				Dim len As Long = ret.length()
				If p IsNot Nothing Then
					For i As Integer = 0 To len - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						Dim idx() As Long = idxIter.next()
						Dim binomialDistribution As New org.apache.commons.math3.distribution.BinomialDistribution(DirectCast(Nd4j.Random, RandomGenerator), numberOfTrials_Conflict, p.getDouble(idx))
						ret.putScalar(idx, binomialDistribution.sample())
					Next i
				Else
					Dim binomialDistribution As New org.apache.commons.math3.distribution.BinomialDistribution(DirectCast(Nd4j.Random, RandomGenerator), numberOfTrials_Conflict, probabilityOfSuccess_Conflict)
					For i As Integer = 0 To len - 1
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
						ret.putScalar(idxIter.next(), binomialDistribution.sample())
					Next i
				End If
				Return ret
			End If

		End Function
	End Class

End Namespace