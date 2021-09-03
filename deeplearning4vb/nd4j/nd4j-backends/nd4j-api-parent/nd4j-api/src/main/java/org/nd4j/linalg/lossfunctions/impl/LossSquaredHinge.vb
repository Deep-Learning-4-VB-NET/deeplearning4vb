Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossUtil = org.nd4j.linalg.lossfunctions.LossUtil
Imports org.nd4j.common.primitives

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
'ORIGINAL LINE: @EqualsAndHashCode public class LossSquaredHinge implements org.nd4j.linalg.lossfunctions.ILossFunction
	<Serializable>
	Public Class LossSquaredHinge
		Implements ILossFunction

		Public Overridable Function scoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			If Not labels.equalShapes(preOutput) Then
				Preconditions.throwEx("Labels and preOutput must have equal shapes: got shapes %s vs %s", labels.shape(), preOutput.shape())
			End If
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
	'         y_hat is -1 or 1
	'        hinge loss is max(0,1-y_hat*y)
	'         
			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)

			Dim scoreArr As INDArray = output.muli(labels) 'y*yhat
			scoreArr.rsubi(1.0) '1 - y*yhat

			If mask IsNot Nothing Then
				LossUtil.applyMask(scoreArr, mask)
			End If
			Return scoreArr ' 1 - y*yhat
		End Function

		Public Overridable Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double Implements ILossFunction.computeScore
			Dim scoreArr As INDArray = computeScoreArray(labels, preOutput, activationFn, mask)
			Dim score As Double = scoreArr.sumNumber().doubleValue()
			If average Then
				score /= scoreArr.size(0)
			End If
			Return score
		End Function

		Public Overridable Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeScoreArray
			Dim scoreArr As INDArray = scoreArray(labels, preOutput, activationFn, mask)
			BooleanIndexing.replaceWhere(scoreArr, 0.0, Conditions.lessThan(0.0)) 'max(0,1-y*yhat)
			scoreArr.muli(scoreArr)
			Return scoreArr.sum(True,1)
		End Function

		Public Overridable Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeGradient
			If Not labels.equalShapes(preOutput) Then
				Preconditions.throwEx("Labels and preOutput must have equal shapes: got shapes %s vs %s", labels.shape(), preOutput.shape())
			End If
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
			Dim scoreArr As INDArray = scoreArray(labels, preOutput, activationFn, mask)

			Dim bitMaskRowCol As INDArray = scoreArr.dup()
	'        
	'            bit mask is 0 if 1-sigma(y*yhat) is neg, bit mask is 1 if 1-sigma(y*yhat) is +ve
	'         
			BooleanIndexing.replaceWhere(bitMaskRowCol, 0.0, Conditions.lessThan(0.0))
			BooleanIndexing.replaceWhere(bitMaskRowCol, 1.0, Conditions.greaterThan(0.0))

			Dim dLda As INDArray = scoreArr.muli(2).muli(labels.neg())
			dLda.muli(bitMaskRowCol)

			If mask IsNot Nothing AndAlso LossUtil.isPerOutputMasking(dLda, mask) Then
				'For *most* activation functions: we don't actually need to mask dL/da in addition to masking dL/dz later
				'but: some, like softmax, require both (due to dL/dz_i being a function of dL/da_j, for i != j)
				'We could add a special case for softmax (activationFn instanceof ActivationSoftmax) but that would be
				' error prone - though buy us a tiny bit of performance
				LossUtil.applyMask(dLda, mask)
			End If

			Dim gradients As INDArray = activationFn.backprop(preOutput, dLda).First 'TODO activation functions with params

			If mask IsNot Nothing Then
				LossUtil.applyMask(gradients, mask)
			End If

			Return gradients
		End Function

		Public Overridable Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray) Implements ILossFunction.computeGradientAndScore
			'TODO: probably a more efficient way to do this...

			Return New Pair(Of Double, INDArray)(computeScore(labels, preOutput, activationFn, mask, average), computeGradient(labels, preOutput, activationFn, mask))
		End Function

		''' <summary>
		''' The opName of this function
		''' 
		''' @return
		''' </summary>
		Public Overridable Function name() As String Implements ILossFunction.name
			Return ToString()
		End Function


		Public Overrides Function ToString() As String
			Return "LossSquaredHinge()"
		End Function
	End Class

End Namespace