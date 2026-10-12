Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports BinomialDistribution = org.apache.commons.math3.distribution.BinomialDistribution
Imports ExponentialDistribution = org.apache.commons.math3.distribution.ExponentialDistribution
Imports NormalDistribution = org.apache.commons.math3.distribution.NormalDistribution
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports BernoulliReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.BernoulliReconstructionDistribution
Imports ExponentialReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.ExponentialReconstructionDistribution
Imports GaussianReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.GaussianReconstructionDistribution
Imports ReconstructionDistribution = org.deeplearning4j.nn.conf.layers.variational.ReconstructionDistribution
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.nn.layers.variational



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag @Tag(TagNames.RNG) @Tag(TagNames.CUSTOM_FUNCTIONALITY) @Tag(TagNames.DL4J_OLD_API) public class TestReconstructionDistributions extends org.deeplearning4j.BaseDL4JTest
	Public Class TestReconstructionDistributions
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGaussianLogProb()
		Public Overridable Sub testGaussianLogProb()
			Nd4j.Random.setSeed(12345)

			Dim inputSize As Integer = 4
			Dim mbs() As Integer = {1, 2, 5}

			For Each average As Boolean In New Boolean() {True, False}
				For Each minibatch As Integer In mbs

					Dim x As INDArray = Nd4j.rand(minibatch, inputSize)
					Dim mean As INDArray = Nd4j.randn(minibatch, inputSize)
					Dim logStdevSquared As INDArray = Nd4j.rand(minibatch, inputSize).subi(0.5)

					Dim distributionParams As INDArray = Nd4j.createUninitialized(New Integer() {minibatch, 2 * inputSize})
					distributionParams.get(NDArrayIndex.all(), NDArrayIndex.interval(0, inputSize)).assign(mean)
					distributionParams.get(NDArrayIndex.all(), NDArrayIndex.interval(inputSize, 2 * inputSize)).assign(logStdevSquared)

					Dim dist As ReconstructionDistribution = New GaussianReconstructionDistribution(Activation.IDENTITY)

					Dim negLogProb As Double = dist.negLogProbability(x, distributionParams, average)

					Dim exampleNegLogProb As INDArray = dist.exampleNegLogProbability(x, distributionParams)
					assertArrayEquals(New Long() {minibatch, 1}, exampleNegLogProb.shape())

					'Calculate the same thing, but using Apache Commons math

					Dim logProbSum As Double = 0.0
					For i As Integer = 0 To minibatch - 1
						Dim exampleSum As Double = 0.0
						For j As Integer = 0 To inputSize - 1
							Dim mu As Double = mean.getDouble(i, j)
							Dim logSigma2 As Double = logStdevSquared.getDouble(i, j)
							Dim sigma As Double = Math.Sqrt(Math.Exp(logSigma2))
							Dim nd As New NormalDistribution(mu, sigma)

							Dim xVal As Double = x.getDouble(i, j)
							Dim thisLogProb As Double = nd.logDensity(xVal)
							logProbSum += thisLogProb
							exampleSum += thisLogProb
						Next j
						assertEquals(-exampleNegLogProb.getDouble(i), exampleSum, 1e-6)
					Next i

					Dim expNegLogProb As Double
					If average Then
						expNegLogProb = -logProbSum / minibatch
					Else
						expNegLogProb = -logProbSum
					End If


					'                System.out.println(expLogProb + "\t" + logProb + "\t" + (logProb / expLogProb));
					assertEquals(expNegLogProb, negLogProb, 1e-6)


					'Also: check random sampling...
					Dim count As Integer = minibatch * inputSize
					Dim arr As INDArray = Nd4j.linspace(-3, 3, count, Nd4j.dataType()).reshape(ChrW(minibatch), inputSize)
					Dim sampleMean As INDArray = dist.generateAtMean(arr)
					Dim sampleRandom As INDArray = dist.generateRandom(arr)
				Next minibatch
			Next average
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBernoulliLogProb()
		Public Overridable Sub testBernoulliLogProb()
			Nd4j.Random.setSeed(12345)

			Dim inputSize As Integer = 4
			Dim mbs() As Integer = {1, 2, 5}

			Dim r As New Random(12345)

			For Each average As Boolean In New Boolean() {True, False}
				For Each minibatch As Integer In mbs

					Dim x As INDArray = Nd4j.zeros(minibatch, inputSize)
					For i As Integer = 0 To minibatch - 1
						For j As Integer = 0 To inputSize - 1
							x.putScalar(i, j, r.Next(2))
						Next j
					Next i

					Dim distributionParams As INDArray = Nd4j.rand(minibatch, inputSize).muli(2).subi(1) 'i.e., pre-sigmoid prob
					Dim prob As INDArray = Transforms.sigmoid(distributionParams, True)

					Dim dist As ReconstructionDistribution = New BernoulliReconstructionDistribution(Activation.SIGMOID)

					Dim negLogProb As Double = dist.negLogProbability(x, distributionParams, average)

					Dim exampleNegLogProb As INDArray = dist.exampleNegLogProbability(x, distributionParams)
					assertArrayEquals(New Long() {minibatch, 1}, exampleNegLogProb.shape())

					'Calculate the same thing, but using Apache Commons math

					Dim logProbSum As Double = 0.0
					For i As Integer = 0 To minibatch - 1
						Dim exampleSum As Double = 0.0
						For j As Integer = 0 To inputSize - 1
							Dim p As Double = prob.getDouble(i, j)

							Dim binomial As New BinomialDistribution(1, p) 'Bernoulli is a special case of binomial

							Dim xVal As Double = x.getDouble(i, j)
							Dim thisLogProb As Double = binomial.logProbability(CInt(Math.Truncate(xVal)))
							logProbSum += thisLogProb
							exampleSum += thisLogProb
						Next j
						assertEquals(-exampleNegLogProb.getDouble(i), exampleSum, 1e-6)
					Next i

					Dim expNegLogProb As Double
					If average Then
						expNegLogProb = -logProbSum / minibatch
					Else
						expNegLogProb = -logProbSum
					End If

					'                System.out.println(x);

					'                System.out.println(expNegLogProb + "\t" + logProb + "\t" + (logProb / expNegLogProb));
					assertEquals(expNegLogProb, negLogProb, 1e-6)

					'Also: check random sampling...
					Dim count As Integer = minibatch * inputSize
					Dim arr As INDArray = Nd4j.linspace(-3, 3, count, Nd4j.dataType()).reshape(ChrW(minibatch), inputSize)
					Dim sampleMean As INDArray = dist.generateAtMean(arr)
					Dim sampleRandom As INDArray = dist.generateRandom(arr)

					For i As Integer = 0 To minibatch - 1
						For j As Integer = 0 To inputSize - 1
							Dim d1 As Double = sampleMean.getDouble(i, j)
							Dim d2 As Double = sampleRandom.getDouble(i, j)
							assertTrue(d1 >= 0.0 OrElse d1 <= 1.0) 'Mean value - probability... could do 0 or 1 (based on most likely) but that isn't very useful...
							assertTrue(d2 = 0.0 OrElse d2 = 1.0)
						Next j
					Next i
				Next minibatch
			Next average
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testExponentialLogProb()
		Public Overridable Sub testExponentialLogProb()
			Nd4j.Random.setSeed(12345)

			Dim inputSize As Integer = 4
			Dim mbs() As Integer = {1, 2, 5}

			Dim r As New Random(12345)

			For Each average As Boolean In New Boolean() {True, False}
				For Each minibatch As Integer In mbs

					Dim x As INDArray = Nd4j.zeros(minibatch, inputSize)
					For i As Integer = 0 To minibatch - 1
						For j As Integer = 0 To inputSize - 1
							x.putScalar(i, j, r.Next(2))
						Next j
					Next i

					Dim distributionParams As INDArray = Nd4j.rand(minibatch, inputSize).muli(2).subi(1) 'i.e., pre-afn gamma
					Dim gammas As INDArray = Transforms.tanh(distributionParams, True)

					Dim dist As ReconstructionDistribution = New ExponentialReconstructionDistribution(Activation.TANH)

					Dim negLogProb As Double = dist.negLogProbability(x, distributionParams, average)

					Dim exampleNegLogProb As INDArray = dist.exampleNegLogProbability(x, distributionParams)
					assertArrayEquals(New Long() {minibatch, 1}, exampleNegLogProb.shape())

					'Calculate the same thing, but using Apache Commons math

					Dim logProbSum As Double = 0.0
					For i As Integer = 0 To minibatch - 1
						Dim exampleSum As Double = 0.0
						For j As Integer = 0 To inputSize - 1
							Dim gamma As Double = gammas.getDouble(i, j)
							Dim lambda As Double = Math.Exp(gamma)
							Dim mean As Double = 1.0 / lambda

							Dim exp As New ExponentialDistribution(mean) 'Commons math uses mean = 1/lambda

							Dim xVal As Double = x.getDouble(i, j)
							Dim thisLogProb As Double = exp.logDensity(xVal)
							logProbSum += thisLogProb
							exampleSum += thisLogProb
						Next j
						assertEquals(-exampleNegLogProb.getDouble(i), exampleSum, 1e-6)
					Next i

					Dim expNegLogProb As Double
					If average Then
						expNegLogProb = -logProbSum / minibatch
					Else
						expNegLogProb = -logProbSum
					End If

					'                System.out.println(x);

					'                System.out.println(expNegLogProb + "\t" + logProb + "\t" + (logProb / expNegLogProb));
					assertEquals(expNegLogProb, negLogProb, 1e-6)

					'Also: check random sampling...
					Dim count As Integer = minibatch * inputSize
					Dim arr As INDArray = Nd4j.linspace(-3, 3, count, Nd4j.dataType()).reshape(ChrW(minibatch), inputSize)
					Dim sampleMean As INDArray = dist.generateAtMean(arr)
					Dim sampleRandom As INDArray = dist.generateRandom(arr)

					For i As Integer = 0 To minibatch - 1
						For j As Integer = 0 To inputSize - 1
							Dim d1 As Double = sampleMean.getDouble(i, j)
							Dim d2 As Double = sampleRandom.getDouble(i, j)
							assertTrue(d1 >= 0.0)
							assertTrue(d2 >= 0.0)
						Next j
					Next i
				Next minibatch
			Next average
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void gradientCheckReconstructionDistributions()
		Public Overridable Sub gradientCheckReconstructionDistributions()
			Dim eps As Double = 1e-6
			Dim maxRelError As Double = 1e-6
			Dim minAbsoluteError As Double = 1e-9

			Nd4j.Random.setSeed(12345)

			Dim inputSize As Integer = 4
			Dim mbs() As Integer = {1, 3}

			Dim r As New Random(12345)

			Dim distributions() As ReconstructionDistribution = {
				New GaussianReconstructionDistribution(Activation.IDENTITY),
				New GaussianReconstructionDistribution(Activation.TANH),
				New BernoulliReconstructionDistribution(Activation.SIGMOID),
				New ExponentialReconstructionDistribution(Activation.IDENTITY),
				New ExponentialReconstructionDistribution(Activation.TANH)
			}


			Dim passes As IList(Of String) = New List(Of String)()
			Dim failures As IList(Of String) = New List(Of String)()
			For Each rd As ReconstructionDistribution In distributions
				For Each minibatch As Integer In mbs


					Dim x As INDArray
					Dim distributionParams As INDArray
					If TypeOf rd Is GaussianReconstructionDistribution Then
						distributionParams = Nd4j.rand(minibatch, inputSize * 2).muli(2).subi(1)
						x = Nd4j.rand(minibatch, inputSize)
					ElseIf TypeOf rd Is BernoulliReconstructionDistribution Then
						distributionParams = Nd4j.rand(minibatch, inputSize).muli(2).subi(1)
						x = Nd4j.zeros(minibatch, inputSize)
						For i As Integer = 0 To minibatch - 1
							For j As Integer = 0 To inputSize - 1
								x.putScalar(i, j, r.Next(2))
							Next j
						Next i
					ElseIf TypeOf rd Is ExponentialReconstructionDistribution Then
						distributionParams = Nd4j.rand(minibatch, inputSize).muli(2).subi(1)
						x = Nd4j.rand(minibatch, inputSize)
					Else
						Throw New Exception()
					End If

					Dim gradient As INDArray = rd.gradient(x, distributionParams)

					Dim testName As String = "minibatch = " & minibatch & ", size = " & inputSize & ", Distribution = " & rd
					Console.WriteLine("***** Starting test: " & testName & "*****")

					Dim totalFailureCount As Integer = 0
					Dim i As Integer = 0
					Do While i < distributionParams.size(1)
						Dim j As Integer = 0
						Do While j < distributionParams.size(0)
							Dim initial As Double = distributionParams.getDouble(j, i)
							distributionParams.putScalar(j, i, initial + eps)
							Dim scorePlus As Double = rd.negLogProbability(x, distributionParams, False)
							distributionParams.putScalar(j, i, initial - eps)
							Dim scoreMinus As Double = rd.negLogProbability(x, distributionParams, False)
							distributionParams.putScalar(j, i, initial)

							Dim numericalGrad As Double = (scorePlus - scoreMinus) / (2.0 * eps)
							Dim backpropGrad As Double = gradient.getDouble(j, i)

							Dim relError As Double = Math.Abs(numericalGrad - backpropGrad) / (Math.Abs(numericalGrad) + Math.Abs(backpropGrad))
							Dim absError As Double = Math.Abs(backpropGrad - numericalGrad)

							If relError > maxRelError OrElse Double.IsNaN(relError) Then
								If absError < minAbsoluteError Then
									log.info("Input (" & j & "," & i & ") passed: grad= " & backpropGrad & ", numericalGrad= " & numericalGrad & ", relError= " & relError & "; absolute error = " & absError & " < minAbsoluteError = " & minAbsoluteError)
								Else
									log.info("Input (" & j & "," & i & ") FAILED: grad= " & backpropGrad & ", numericalGrad= " & numericalGrad & ", relError= " & relError & ", scorePlus=" & scorePlus & ", scoreMinus= " & scoreMinus)
									totalFailureCount += 1
								End If
							Else
								log.trace("Input (" & j & "," & i & ") passed: grad= " & backpropGrad & ", numericalGrad= " & numericalGrad & ", relError= " & relError)
							End If
							j += 1
						Loop
						i += 1
					Loop


					If totalFailureCount > 0 Then
						failures.Add(testName)
					Else
						passes.Add(testName)
					End If

				Next minibatch
			Next rd

			Console.WriteLine(vbLf & vbLf & vbLf & " +++++ Test Passes +++++")
			For Each s As String In passes
				Console.WriteLine(s)
			Next s

			Console.WriteLine(vbLf & vbLf & vbLf & " +++++ Test Faliures +++++")
			For Each s As String In failures
				Console.WriteLine(s)
			Next s

			assertEquals(0, failures.Count)
		End Sub
	End Class

End Namespace