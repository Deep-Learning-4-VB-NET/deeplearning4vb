Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossUtil = org.nd4j.linalg.lossfunctions.LossUtil
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

Namespace org.nd4j.linalg.lossfunctions.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode @JsonInclude(JsonInclude.Include.NON_NULL) @Getter public class LossMultiLabel implements org.nd4j.linalg.lossfunctions.ILossFunction
	<Serializable>
	Public Class LossMultiLabel
		Implements ILossFunction


		Public Sub New()
		End Sub

		Private Sub calculate(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal scoreOutput As INDArray, ByVal gradientOutput As INDArray)
			If scoreOutput Is Nothing AndAlso gradientOutput Is Nothing Then
				Throw New System.ArgumentException("You have to provide at least one of scoreOutput or gradientOutput!")
			End If
			If labels.size(1) <> preOutput.size(1) Then
				Throw New System.ArgumentException("Labels array numColumns (size(1) = " & labels.size(1) & ") does not match output layer" & " number of outputs (nOut = " & preOutput.size(1) & ") ")

			End If
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray postOutput = activationFn.getActivation(preOutput.dup(), true);
			Dim postOutput As INDArray = activationFn.getActivation(preOutput.dup(), True)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray positive = labels;
			Dim positive As INDArray = labels
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray negative = labels.eq(0.0).castTo(org.nd4j.linalg.factory.Nd4j.defaultFloatingPointType());
			Dim negative As INDArray = labels.eq(0.0).castTo(Nd4j.defaultFloatingPointType())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray normFactor = negative.sum(true,1).castTo(org.nd4j.linalg.factory.Nd4j.defaultFloatingPointType()).muli(positive.sum(true,1));
			Dim normFactor As INDArray = negative.sum(True,1).castTo(Nd4j.defaultFloatingPointType()).muli(positive.sum(True,1))


			Dim examples As Long = positive.size(0)
			For i As Integer = 0 To examples - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray locCfn = postOutput.getRow(i, true);
				Dim locCfn As INDArray = postOutput.getRow(i, True)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final long[] shape = locCfn.shape();
				Dim shape() As Long = locCfn.shape()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray locPositive = positive.getRow(i, true);
				Dim locPositive As INDArray = positive.getRow(i, True)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray locNegative = negative.getRow(i, true);
				Dim locNegative As INDArray = negative.getRow(i, True)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final System.Nullable<Double> locNormFactor = normFactor.getDouble(i);
				Dim locNormFactor As Double? = normFactor.getDouble(i)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int outSetSize = locNegative.sumNumber().intValue();
				Dim outSetSize As Integer = locNegative.sumNumber().intValue()
				If outSetSize = 0 OrElse outSetSize = locNegative.columns() Then
					If scoreOutput IsNot Nothing Then
						scoreOutput.getRow(i, True).assign(0)
					End If

					If gradientOutput IsNot Nothing Then
						gradientOutput.getRow(i, True).assign(0)
					End If
				Else
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray operandA = org.nd4j.linalg.factory.Nd4j.ones(shape[1], shape[0]).mmul(locCfn);
					Dim operandA As INDArray = Nd4j.ones(shape(1), shape(0)).mmul(locCfn)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray operandB = operandA.transpose();
					Dim operandB As INDArray = operandA.transpose()

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray pairwiseSub = org.nd4j.linalg.ops.transforms.Transforms.exp(operandA.sub(operandB));
					Dim pairwiseSub As INDArray = Transforms.exp(operandA.sub(operandB))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray selection = locPositive.transpose().mmul(locNegative);
					Dim selection As INDArray = locPositive.transpose().mmul(locNegative)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray classificationDifferences = pairwiseSub.muli(selection).divi(locNormFactor);
					Dim classificationDifferences As INDArray = pairwiseSub.muli(selection).divi(locNormFactor)

					If scoreOutput IsNot Nothing Then
						If mask IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray perLabel = classificationDifferences.sum(0);
							Dim perLabel As INDArray = classificationDifferences.sum(0)
							LossUtil.applyMask(perLabel, mask.getRow(i, True))
							perLabel.sum(scoreOutput.getRow(i, True), 0)
						Else
							classificationDifferences.sum(scoreOutput.getRow(i, True), 0, 1)
						End If
					End If

					If gradientOutput IsNot Nothing Then
						gradientOutput.getRow(i, True).assign(classificationDifferences.sum(True, 0).addi(classificationDifferences.sum(True,1).transposei().negi()))
					End If
				End If
			Next i

			If gradientOutput IsNot Nothing Then
				gradientOutput.assign(activationFn.backprop(preOutput.dup(), gradientOutput).First)
				'multiply with masks, always
				If mask IsNot Nothing Then
					LossUtil.applyMask(gradientOutput, mask)
				End If
			End If
		End Sub

		Public Overridable Function scoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray scoreArr = org.nd4j.linalg.factory.Nd4j.create(labels.size(0), 1);
			Dim scoreArr As INDArray = Nd4j.create(labels.size(0), 1)
			calculate(labels, preOutput, activationFn, mask, scoreArr, Nothing)
			Return scoreArr
		End Function

		Public Overridable Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double Implements ILossFunction.computeScore
			Dim scoreArr As INDArray = scoreArray(labels, preOutput, activationFn, mask)

			Dim score As Double = scoreArr.sumNumber().doubleValue()

			If average Then
				score /= scoreArr.size(0)
			End If

			Return score
		End Function

		Public Overridable Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeScoreArray
			Dim scoreArr As INDArray = scoreArray(labels, preOutput, activationFn, mask)
			Return scoreArr.sum(True,1)
		End Function

		Public Overridable Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeGradient
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
			If labels.size(1) <> preOutput.size(1) Then
				Throw New System.ArgumentException("Labels array numColumns (size(1) = " & labels.size(1) & ") does not match output layer" & " number of outputs (nOut = " & preOutput.size(1) & ") ")

			End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray grad = org.nd4j.linalg.factory.Nd4j.ones(labels.shape());
			Dim grad As INDArray = Nd4j.ones(labels.shape())
			calculate(labels, preOutput, activationFn, mask, Nothing, grad)
			Return grad
		End Function

		Public Overridable Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray) Implements ILossFunction.computeGradientAndScore
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray scoreArr = org.nd4j.linalg.factory.Nd4j.create(labels.size(0), 1);
			Dim scoreArr As INDArray = Nd4j.create(labels.size(0), 1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray grad = org.nd4j.linalg.factory.Nd4j.ones(labels.shape());
			Dim grad As INDArray = Nd4j.ones(labels.shape())

			calculate(labels, preOutput, activationFn, mask, scoreArr, grad)

			Dim score As Double = scoreArr.sumNumber().doubleValue()

			If average Then
				score /= scoreArr.size(0)
			End If

			Return New Pair(Of Double, INDArray)(score, grad)
		End Function

		Public Overridable Function name() As String Implements ILossFunction.name
			Return ToString()
		End Function


		Public Overrides Function ToString() As String
			Return "LossMultiLabel"
		End Function
	End Class

End Namespace