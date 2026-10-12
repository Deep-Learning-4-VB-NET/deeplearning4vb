Imports System
Imports Data = lombok.Data
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationHardSigmoid = org.nd4j.linalg.activations.impl.ActivationHardSigmoid
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports LessThan = org.nd4j.linalg.api.ops.impl.transforms.custom.LessThan
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
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
'ORIGINAL LINE: @Slf4j @Data public class BernoulliReconstructionDistribution implements ReconstructionDistribution
	<Serializable>
	Public Class BernoulliReconstructionDistribution
		Implements ReconstructionDistribution

		Private ReadOnly activationFn As IActivation

		''' <summary>
		''' Create a BernoulliReconstructionDistribution with the default Sigmoid activation function
		''' </summary>
		Public Sub New()
			Me.New(Activation.SIGMOID)
		End Sub

		''' <param name="activationFn">    Activation function. Sigmoid generally; must be bounded in range 0 to 1 </param>
		Public Sub New(ByVal activationFn As Activation)
			Me.New(activationFn.getActivationFunction())
		End Sub

		''' <param name="activationFn">    Activation function. Sigmoid generally; must be bounded in range 0 to 1 </param>
		Public Sub New(ByVal activationFn As IActivation)
			Me.activationFn = activationFn
			If Not (TypeOf activationFn Is ActivationSigmoid) AndAlso Not (TypeOf activationFn Is ActivationHardSigmoid) Then
				log.warn("Using BernoulliRecontructionDistribution with activation function """ & activationFn & """." & " Using sigmoid/hard sigmoid is recommended to bound probabilities in range 0 to 1")
			End If
		End Sub

		Public Overridable Function hasLossFunction() As Boolean Implements ReconstructionDistribution.hasLossFunction
			Return False
		End Function

		Public Overridable Function distributionInputSize(ByVal dataSize As Integer) As Integer Implements ReconstructionDistribution.distributionInputSize
			Return dataSize
		End Function

		Public Overridable Function negLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray, ByVal average As Boolean) As Double Implements ReconstructionDistribution.negLogProbability
			Dim logProb As INDArray = calcLogProbArray(x, preOutDistributionParams)

			If average Then
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				Return -logProb.sumNumber().doubleValue() / x.size(0)
			Else
				Return -logProb.sumNumber().doubleValue()
			End If
		End Function

		Public Overridable Function exampleNegLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.exampleNegLogProbability
			Dim logProb As INDArray = calcLogProbArray(x, preOutDistributionParams)

			Return logProb.sum(True, 1).negi()
		End Function

		Private Function calcLogProbArray(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray
			x = x.castTo(preOutDistributionParams.dataType())
			Dim output As INDArray = preOutDistributionParams.dup()
			activationFn.getActivation(output, False)

			Dim logOutput As INDArray = Transforms.log(output, True)
			Dim log1SubOut As INDArray = Transforms.log(output.rsubi(1.0), False)

			'For numerical stability: if output = 0, then log(output) == -infinity
			'then x * log(output) = NaN, but lim(x->0, output->0)[ x * log(output) ] == 0
			' therefore: want 0*log(0) = 0, NOT 0*log(0) = NaN by default
			BooleanIndexing.replaceWhere(logOutput, 0.0, Conditions.Infinite) 'log(out)= +/- inf -> x == 0.0 -> 0 * log(0) = 0
			BooleanIndexing.replaceWhere(log1SubOut, 0.0, Conditions.Infinite) 'log(out)= +/- inf -> x == 0.0 -> 0 * log(0) = 0
			Return logOutput.muli(x).addi(x.rsub(1.0).muli(log1SubOut))
		End Function

		Public Overridable Function gradient(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.gradient
			Dim output As INDArray = preOutDistributionParams.dup()
			activationFn.getActivation(output, True)
			x = x.castTo(preOutDistributionParams.dataType())

			Dim diff As INDArray = x.sub(output)
			Dim outOneMinusOut As INDArray = output.rsub(1.0).muli(output)

			Dim grad As INDArray = diff.divi(outOneMinusOut)
			grad = activationFn.backprop(preOutDistributionParams.dup(), grad).First

			'Issue: if output == 0 or output == 1, then (assuming sigmoid output or similar)
			'sigmaPrime == 0, sigmaPrime * (x-out) / (out*(1-out)) == 0 * (x-out) / 0 -> 0/0 -> NaN. But taking limit, we want
			'0*(x-out)/0 == 0 -> implies 0 gradient at the far extremes (0 or 1) of the output
			BooleanIndexing.replaceWhere(grad, 0.0, Conditions.Nan)
			Return grad.negi()
		End Function

		Public Overridable Function generateRandom(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateRandom
			Dim p As INDArray = preOutDistributionParams.dup()
			activationFn.getActivation(p, False)

			Dim rand As INDArray = Nd4j.rand(p.shape())
			'Can simply randomly sample by looking where values are < p...
			'i.e., sample = 1 if randNum < p, 0 otherwise

			Dim [out] As INDArray = Nd4j.createUninitialized(DataType.BOOL, p.shape())

			Nd4j.Executioner.execAndReturn(New LessThan(rand, p, [out]))
			Return [out].castTo(DataType.FLOAT)
		End Function

		Public Overridable Function generateAtMean(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateAtMean
			'mean value for bernoulli: same as probability parameter...
			'Obviously we can't produce exactly the mean value - bernoulli should produce only {0,1} values
			'but returning the actual mean value is more useful
			Dim p As INDArray = preOutDistributionParams.dup()
			activationFn.getActivation(p, False)

			Return p
		End Function

		Public Overrides Function ToString() As String
			Return "BernoulliReconstructionDistribution(afn=" & activationFn & ")"
		End Function
	End Class

End Namespace