Imports System
Imports Data = lombok.Data
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
'ORIGINAL LINE: @Data public class ExponentialReconstructionDistribution implements ReconstructionDistribution
	<Serializable>
	Public Class ExponentialReconstructionDistribution
		Implements ReconstructionDistribution

		Private ReadOnly activationFn As IActivation

		Public Sub New()
			Me.New("identity")
		End Sub

		''' @deprecated Use <seealso cref="ExponentialReconstructionDistribution(Activation)"/> 
		<Obsolete("Use <seealso cref=""ExponentialReconstructionDistribution(Activation)""/>")>
		Public Sub New(ByVal activationFn As String)
			Me.New(Activation.fromString(activationFn).getActivationFunction())
		End Sub

		Public Sub New(ByVal activation As Activation)
			Me.New(activation.getActivationFunction())
		End Sub

		Public Sub New(ByVal activationFn As IActivation)
			Me.activationFn = activationFn
		End Sub

		Public Overridable Function hasLossFunction() As Boolean Implements ReconstructionDistribution.hasLossFunction
			Return False
		End Function

		Public Overridable Function distributionInputSize(ByVal dataSize As Integer) As Integer Implements ReconstructionDistribution.distributionInputSize
			Return dataSize
		End Function

		Public Overridable Function negLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray, ByVal average As Boolean) As Double Implements ReconstructionDistribution.negLogProbability
			'p(x) = lambda * exp( -lambda * x)
			'logp(x) = log(lambda) - lambda * x = gamma - lambda * x

			Dim gamma As INDArray = preOutDistributionParams.dup()
			activationFn.getActivation(gamma, False)

			Dim lambda As INDArray = Transforms.exp(gamma, True)
			Dim negLogProbSum As Double = -lambda.muli(x).rsubi(gamma).sumNumber().doubleValue()
			If average Then
				Return negLogProbSum / x.size(0)
			Else
				Return negLogProbSum
			End If

		End Function

		Public Overridable Function exampleNegLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.exampleNegLogProbability

			Dim gamma As INDArray = preOutDistributionParams.dup()
			activationFn.getActivation(gamma, False)

			Dim lambda As INDArray = Transforms.exp(gamma, True)
			Return lambda.muli(x).rsubi(gamma).sum(True, 1).negi()
		End Function

		Public Overridable Function gradient(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.gradient
			'p(x) = lambda * exp( -lambda * x)
			'logp(x) = log(lambda) - lambda * x = gamma - lambda * x
			'dlogp(x)/dgamma = 1 - lambda * x      (or negative of this for d(-logp(x))/dgamma

			Dim gamma As INDArray = activationFn.getActivation(preOutDistributionParams.dup(), True)

			Dim lambda As INDArray = Transforms.exp(gamma, True)
			Dim dLdx As INDArray = x.mul(lambda).subi(1.0)

			'dL/dz
			Return activationFn.backprop(preOutDistributionParams.dup(), dLdx).First
		End Function

		Public Overridable Function generateRandom(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateRandom
			Dim gamma As INDArray = activationFn.getActivation(preOutDistributionParams.dup(), False)

			Dim lambda As INDArray = Transforms.exp(gamma, True)

			'Inverse cumulative distribution function: -log(1-p)/lambda

			Dim u As INDArray = Nd4j.rand(preOutDistributionParams.shape())

			'Note here: if u ~ U(0,1) then 1-u ~ U(0,1)
			Return Transforms.log(u, False).divi(lambda).negi()
		End Function

		Public Overridable Function generateAtMean(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateAtMean
			'Input: gamma = log(lambda)    ->  lambda = exp(gamma)
			'Mean for exponential distribution: 1/lambda

			Dim gamma As INDArray = activationFn.getActivation(preOutDistributionParams.dup(), False)

			Dim lambda As INDArray = Transforms.exp(gamma, True)
			Return lambda.rdivi(1.0) 'mean = 1.0 / lambda
		End Function

		Public Overrides Function ToString() As String
			Return "ExponentialReconstructionDistribution(afn=" & activationFn & ")"
		End Function
	End Class

End Namespace