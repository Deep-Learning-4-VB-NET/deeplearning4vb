Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossUtil = org.nd4j.linalg.lossfunctions.LossUtil
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude

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

Namespace org.deeplearning4j.rl4j.network.ac

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @JsonInclude(JsonInclude.Include.NON_NULL) public class ActorCriticLoss implements org.nd4j.linalg.lossfunctions.ILossFunction
	<Serializable>
	Public Class ActorCriticLoss
		Implements ILossFunction

		Public Const BETA As Double = 0.01

		Private Function scoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True).addi(1e-5)
			Dim logOutput As INDArray = Transforms.log(output, True)
			Dim entropy As INDArray = output.muli(logOutput)
			Dim scoreArr As INDArray = logOutput.muli(labels).subi(entropy.muli(BETA))

			If mask IsNot Nothing Then
				LossUtil.applyMask(scoreArr, mask)
			End If
			Return scoreArr
		End Function

		Public Overridable Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double Implements ILossFunction.computeScore
			Dim scoreArr As INDArray = scoreArray(labels, preOutput, activationFn, mask)
			Dim score As Double = -scoreArr.sumNumber().doubleValue()
			Return If(average, score / scoreArr.size(0), score)
		End Function

		Public Overridable Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeScoreArray
			Dim scoreArr As INDArray = scoreArray(labels, preOutput, activationFn, mask)
			Return scoreArr.sum(1).muli(-1)
		End Function

		Public Overridable Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeGradient
			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True).addi(1e-5)
			Dim logOutput As INDArray = Transforms.log(output, True)
			Dim entropyDev As INDArray = logOutput.addi(1)
			Dim dLda As INDArray = output.rdivi(labels).subi(entropyDev.muli(BETA)).negi()
			Dim grad As INDArray = activationFn.backprop(preOutput, dLda).First

			If mask IsNot Nothing Then
				LossUtil.applyMask(grad, mask)
			End If
			Return grad
		End Function

		Public Overridable Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray) Implements ILossFunction.computeGradientAndScore
			Return New Pair(Of Double, INDArray)(computeScore(labels, preOutput, activationFn, mask, average), computeGradient(labels, preOutput, activationFn, mask))
		End Function

		Public Overrides Function ToString() As String
			Return "ActorCriticLoss()"
		End Function

		Public Overridable Function name() As String Implements ILossFunction.name
			Return ToString()
		End Function
	End Class

End Namespace