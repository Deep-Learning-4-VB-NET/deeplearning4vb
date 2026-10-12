Imports System
Imports Data = lombok.Data
Imports val = lombok.val
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports INDArrayIndex = org.nd4j.linalg.indexing.INDArrayIndex
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms

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

Namespace org.deeplearning4j.nn.conf.layers.variational

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class GaussianReconstructionDistribution implements ReconstructionDistribution
	<Serializable>
	Public Class GaussianReconstructionDistribution
		Implements ReconstructionDistribution

		Private Shared ReadOnly NEG_HALF_LOG_2PI As Double = -0.5 * Math.Log(2 * Math.PI)

		Private ReadOnly activationFn As IActivation

		''' <summary>
		''' Create a GaussianReconstructionDistribution with the default identity activation function.
		''' </summary>
		Public Sub New()
			Me.New(Activation.IDENTITY)
		End Sub

		''' <param name="activationFn">    Activation function for the reconstruction distribution. Typically identity or tanh. </param>
		Public Sub New(ByVal activationFn As Activation)
			Me.New(activationFn.getActivationFunction())
		End Sub

		''' <param name="activationFn">    Activation function for the reconstruction distribution. Typically identity or tanh. </param>
		Public Sub New(ByVal activationFn As IActivation)
			Me.activationFn = activationFn
		End Sub

		Public Overridable Function hasLossFunction() As Boolean Implements ReconstructionDistribution.hasLossFunction
			Return False
		End Function

		Public Overridable Function distributionInputSize(ByVal dataSize As Integer) As Integer Implements ReconstructionDistribution.distributionInputSize
			Return 2 * dataSize
		End Function

		Public Overridable Function negLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray, ByVal average As Boolean) As Double Implements ReconstructionDistribution.negLogProbability
			Dim size As val = preOutDistributionParams.size(1) \ 2

			Dim logProbArrays() As INDArray = calcLogProbArrayExConstants(x, preOutDistributionParams)
			Dim logProb As Double = x.size(0) * size * NEG_HALF_LOG_2PI - 0.5 * logProbArrays(0).sumNumber().doubleValue() - logProbArrays(1).sumNumber().doubleValue()

			If average Then
				Return -logProb / x.size(0)
			Else
				Return -logProb
			End If
		End Function

		Public Overridable Function exampleNegLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.exampleNegLogProbability
			Dim size As val = preOutDistributionParams.size(1) \ 2

			Dim logProbArrays() As INDArray = calcLogProbArrayExConstants(x, preOutDistributionParams)

			Return logProbArrays(0).sum(True, 1).muli(0.5).subi(size * NEG_HALF_LOG_2PI).addi(logProbArrays(1).sum(True, 1))
		End Function

		Private Function calcLogProbArrayExConstants(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray()
			Dim output As INDArray = preOutDistributionParams.dup()
			activationFn.getActivation(output, False)

			Dim size As val = output.size(1) \ 2
			Dim mean As INDArray = output.get(NDArrayIndex.all(), NDArrayIndex.interval(0, size))
			Dim logStdevSquared As INDArray = output.get(NDArrayIndex.all(), NDArrayIndex.interval(size, 2 * size))

			Dim sigmaSquared As INDArray = Transforms.exp(logStdevSquared, True)
			Dim lastTerm As INDArray = x.sub(mean.castTo(x.dataType()))
			lastTerm.muli(lastTerm)
			lastTerm.divi(sigmaSquared.castTo(lastTerm.dataType())).divi(2)

			Return New INDArray() {logStdevSquared, lastTerm}
		End Function

		Public Overridable Function gradient(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.gradient
			Dim output As INDArray = preOutDistributionParams.dup()
			activationFn.getActivation(output, True)

			Dim size As val = output.size(1) \ 2
			Dim mean As INDArray = output.get(NDArrayIndex.all(), NDArrayIndex.interval(0, size))
			Dim logStdevSquared As INDArray = output.get(NDArrayIndex.all(), NDArrayIndex.interval(size, 2 * size))

			Dim sigmaSquared As INDArray = Transforms.exp(logStdevSquared, True).castTo(x.dataType())

			Dim xSubMean As INDArray = x.sub(mean.castTo(x.dataType()))
			Dim xSubMeanSq As INDArray = xSubMean.mul(xSubMean)

			Dim dLdmu As INDArray = xSubMean.divi(sigmaSquared)

			Dim sigma As INDArray = Transforms.sqrt(sigmaSquared, True)
			Dim sigma3 As INDArray = Transforms.pow(sigmaSquared, 3.0 / 2)

			Dim dLdsigma As INDArray = sigma.rdiv(-1).addi(xSubMeanSq.divi(sigma3))
			Dim dLdlogSigma2 As INDArray = sigma.divi(2).muli(dLdsigma)

			Dim dLdx As INDArray = Nd4j.createUninitialized(preOutDistributionParams.dataType(), output.shape())
			dLdx.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(0, size)}, dLdmu)
			dLdx.put(New INDArrayIndex() {NDArrayIndex.all(), NDArrayIndex.interval(size, 2 * size)}, dLdlogSigma2)
			dLdx.negi()

			'dL/dz
			Return activationFn.backprop(preOutDistributionParams.dup(), dLdx).First
		End Function

		Public Overridable Function generateRandom(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateRandom
			Dim output As INDArray = preOutDistributionParams.dup()
			activationFn.getActivation(output, True)

			Dim size As val = output.size(1) \ 2
			Dim mean As INDArray = output.get(NDArrayIndex.all(), NDArrayIndex.interval(0, size))
			Dim logStdevSquared As INDArray = output.get(NDArrayIndex.all(), NDArrayIndex.interval(size, 2 * size))

			Dim sigma As INDArray = Transforms.exp(logStdevSquared, True)
			Transforms.sqrt(sigma, False)

			Dim e As INDArray = Nd4j.randn(sigma.shape())
			Return e.muli(sigma).addi(mean) 'mu + sigma * N(0,1) ~ N(mu,sigma^2)
		End Function

		Public Overridable Function generateAtMean(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateAtMean
			Dim size As val = preOutDistributionParams.size(1) \ 2
			Dim mean As INDArray = preOutDistributionParams.get(NDArrayIndex.all(), NDArrayIndex.interval(0, size)).dup()
			activationFn.getActivation(mean, False)

			Return mean
		End Function

		Public Overrides Function ToString() As String
			Return "GaussianReconstructionDistribution(afn=" & activationFn & ")"
		End Function
	End Class

End Namespace