Imports System
Imports Data = lombok.Data
Imports Activation = org.nd4j.linalg.activations.Activation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty

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
'ORIGINAL LINE: @Data public class LossFunctionWrapper implements ReconstructionDistribution
	<Serializable>
	Public Class LossFunctionWrapper
		Implements ReconstructionDistribution

		Private ReadOnly activationFn As IActivation
		Private ReadOnly lossFunction As ILossFunction

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LossFunctionWrapper(@JsonProperty("activationFn") org.nd4j.linalg.activations.IActivation activationFn, @JsonProperty("lossFunction") org.nd4j.linalg.lossfunctions.ILossFunction lossFunction)
		Public Sub New(ByVal activationFn As IActivation, ByVal lossFunction As ILossFunction)
			Me.activationFn = activationFn
			Me.lossFunction = lossFunction
		End Sub

		Public Sub New(ByVal activation As Activation, ByVal lossFunction As ILossFunction)
			Me.New(activation.getActivationFunction(), lossFunction)
		End Sub

		Public Overridable Function hasLossFunction() As Boolean Implements ReconstructionDistribution.hasLossFunction
			Return True
		End Function

		Public Overridable Function distributionInputSize(ByVal dataSize As Integer) As Integer Implements ReconstructionDistribution.distributionInputSize
			Return dataSize
		End Function

		Public Overridable Function negLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray, ByVal average As Boolean) As Double Implements ReconstructionDistribution.negLogProbability

			'NOTE: The returned value here is NOT negative log probability, but it (the loss function value)
			' is equivalent, in terms of being something we want to minimize...

			Return lossFunction.computeScore(x, preOutDistributionParams, activationFn, Nothing, average)
		End Function

		Public Overridable Function exampleNegLogProbability(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.exampleNegLogProbability
			Return lossFunction.computeScoreArray(x, preOutDistributionParams, activationFn, Nothing)
		End Function

		Public Overridable Function gradient(ByVal x As INDArray, ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.gradient
			Return lossFunction.computeGradient(x, preOutDistributionParams, activationFn, Nothing)
		End Function

		Public Overridable Function generateRandom(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateRandom
			'Loss functions: not probabilistic -> deterministic output
			Return generateAtMean(preOutDistributionParams)
		End Function

		Public Overridable Function generateAtMean(ByVal preOutDistributionParams As INDArray) As INDArray Implements ReconstructionDistribution.generateAtMean
			'Loss functions: not probabilistic -> not random
			Dim [out] As INDArray = preOutDistributionParams.dup()
			Return activationFn.getActivation([out], True)
		End Function

		Public Overrides Function ToString() As String
			Return "LossFunctionWrapper(afn=" & activationFn & "," & lossFunction & ")"
		End Function
	End Class

End Namespace