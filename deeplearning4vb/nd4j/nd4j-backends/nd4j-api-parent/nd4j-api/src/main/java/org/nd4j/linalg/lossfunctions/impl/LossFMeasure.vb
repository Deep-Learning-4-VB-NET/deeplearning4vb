Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.common.primitives
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

Namespace org.nd4j.linalg.lossfunctions.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @EqualsAndHashCode public class LossFMeasure implements org.nd4j.linalg.lossfunctions.ILossFunction
	<Serializable>
	Public Class LossFMeasure
		Implements ILossFunction

		Public Const DEFAULT_BETA As Double = 1.0

		Private ReadOnly beta As Double

		Public Sub New()
			Me.New(DEFAULT_BETA)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LossFMeasure(@JsonProperty("beta") double beta)
		Public Sub New(ByVal beta As Double)
			If beta <= 0 Then
				Throw New System.NotSupportedException("Invalid value: beta must be > 0. Got: " & beta)
			End If
			Me.beta = beta
		End Sub


		Public Overridable Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double Implements ILossFunction.computeScore
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
			Dim d() As Double = computeScoreNumDenom(labels, preOutput, activationFn, mask, average)
			Dim numerator As Double = d(0)
			Dim denominator As Double = d(1)

			If numerator = 0.0 AndAlso denominator = 0.0 Then
				Return 0.0
			End If

			Return 1.0 - numerator / denominator
		End Function

		Private Function computeScoreNumDenom(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double()
			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)

			Dim n As Long = labels.size(1)
			If n <> 1 AndAlso n <> 2 Then
				Throw New System.NotSupportedException("For binary classification: expect output size of 1 or 2. Got: " & n)
			End If

			'First: determine positives and negatives
			Dim isPositiveLabel As INDArray
			Dim isNegativeLabel As INDArray
			Dim pClass0 As INDArray
			Dim pClass1 As INDArray
			If n = 1 Then
				isPositiveLabel = labels
				isNegativeLabel = isPositiveLabel.rsub(1.0)
				pClass0 = output.rsub(1.0)
				pClass1 = output
			Else
				isPositiveLabel = labels.getColumn(1)
				isNegativeLabel = labels.getColumn(0)
				pClass0 = output.getColumn(0)
				pClass1 = output.getColumn(1)
			End If

			If mask IsNot Nothing Then
				isPositiveLabel = isPositiveLabel.mulColumnVector(mask)
				isNegativeLabel = isNegativeLabel.mulColumnVector(mask)
			End If

			Dim tp As Double = isPositiveLabel.mul(pClass1).sumNumber().doubleValue()
			Dim fp As Double = isNegativeLabel.mul(pClass1).sumNumber().doubleValue()
			Dim fn As Double = isPositiveLabel.mul(pClass0).sumNumber().doubleValue()

			Dim numerator As Double = (1.0 + beta * beta) * tp
			Dim denominator As Double = (1.0 + beta * beta) * tp + beta * beta * fn + fp

			Return New Double() {numerator, denominator}
		End Function

		Public Overridable Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeScoreArray
			Throw New System.NotSupportedException("Cannot compute score array for FMeasure loss function: loss is only " & "defined for minibatches")
		End Function

		Public Overridable Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeGradient
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
			Dim d() As Double = computeScoreNumDenom(labels, preOutput, activationFn, mask, False)
			Dim numerator As Double = d(0)
			Dim denominator As Double = d(1)

			If numerator = 0.0 AndAlso denominator = 0.0 Then
				'Zero score -> zero gradient
				Return Nd4j.create(preOutput.shape())
			End If

			Dim secondTerm As Double = numerator / (denominator * denominator)

			Dim dLdOut As INDArray
			If labels.size(1) = 1 Then
				'Single binary output case
				dLdOut = labels.mul(1 + beta * beta).divi(denominator).subi(secondTerm)
			Else
				'Softmax case: the getColumn(1) here is to account for the fact that we're using prob(class1)
				' only in the score function; column(1) is equivalent to output for the single output case
				dLdOut = Nd4j.create(labels.shape())
				dLdOut.getColumn(1).assign(labels.getColumn(1).mul(1 + beta * beta).divi(denominator).subi(secondTerm))
			End If

			'Negate relative to description in paper, as we want to *minimize* 1.0-fMeasure, which is equivalent to
			' maximizing fMeasure
			dLdOut.negi()

			Dim dLdPreOut As INDArray = activationFn.backprop(preOutput, dLdOut).First

			If mask IsNot Nothing Then
				dLdPreOut.muliColumnVector(mask)
			End If

			Return dLdPreOut
		End Function

		Public Overridable Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray) Implements ILossFunction.computeGradientAndScore
			'TODO optimize
			Return New Pair(Of Double, INDArray)(computeScore(labels, preOutput, activationFn, mask, average), computeGradient(labels, preOutput, activationFn, mask))
		End Function

		''' <summary>
		''' The opName of this function
		''' 
		''' @return
		''' </summary>
		Public Overridable Function name() As String Implements ILossFunction.name
			Return "floss"
		End Function

		Public Overrides Function ToString() As String
			Return "LossFMeasure(beta=" & beta & ")"
		End Function
	End Class

End Namespace