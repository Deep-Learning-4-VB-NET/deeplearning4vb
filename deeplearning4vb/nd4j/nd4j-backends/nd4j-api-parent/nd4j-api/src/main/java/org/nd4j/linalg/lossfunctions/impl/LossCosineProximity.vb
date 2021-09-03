Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
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
'ORIGINAL LINE: @EqualsAndHashCode public class LossCosineProximity implements org.nd4j.linalg.lossfunctions.ILossFunction
	<Serializable>
	Public Class LossCosineProximity
		Implements ILossFunction

		''' 
		''' <param name="labels"> </param>
		''' <param name="preOutput"> </param>
		''' <param name="activationFn"> </param>
		''' <param name="mask">
		''' @return </param>
		Public Overridable Function scoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			If Not labels.equalShapes(preOutput) Then
				Preconditions.throwEx("Labels and preOutput must have equal shapes: got shapes %s vs %s", labels.shape(), preOutput.shape())
			End If
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype

	'        
	'         mean of -(y.dot(yhat)/||y||*||yhat||)
	'         
			Dim postOutput As INDArray = activationFn.getActivation(preOutput.dup(), True)

			Dim yhatmag As INDArray = postOutput.norm2(1)
			Dim ymag As INDArray = labels.norm2(1)
			yhatmag = Transforms.max(yhatmag, Nd4j.EPS_THRESHOLD, False)
			ymag = Transforms.max(ymag, Nd4j.EPS_THRESHOLD, False)

			Dim scoreArr As INDArray = postOutput.mul(labels)
			scoreArr.diviColumnVector(yhatmag)
			scoreArr.diviColumnVector(ymag)

			If mask IsNot Nothing Then
				If Not mask.ColumnVector Then
					'Per-output masking doesn't really make sense for cosine proximity
					Throw New System.NotSupportedException("Expected column vector mask array for LossCosineProximity." & " Got mask array with shape " & Arrays.toString(mask.shape()) & "; per-output masking is not " & "supported for LossCosineProximity")
				End If
				scoreArr.muliColumnVector(mask)
			End If
			Return scoreArr.muli(-1)
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
			If Not labels.equalShapes(preOutput) Then
				Preconditions.throwEx("Labels and preOutput must have equal shapes: got shapes %s vs %s", labels.shape(), preOutput.shape())
			End If
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
			Dim yhat As INDArray = activationFn.getActivation(preOutput.dup(), True)
			Dim yL2norm As INDArray = labels.norm2(1)

			Dim yhatL2norm As INDArray = yhat.norm2(1)
			Dim yhatL2normSq As INDArray = yhatL2norm.mul(yhatL2norm)

			'Note: This is not really the L1 norm since I am not taking abs values
			Dim yhatDotyL1norm As INDArray = labels.mul(yhat).sum(True,1)

			Dim dLda As INDArray = labels.mulColumnVector(yhatL2normSq)
			dLda.subi(yhat.mulColumnVector(yhatDotyL1norm))

			' transform vals to avoid nans before div
			yL2norm = Transforms.max(yL2norm, Nd4j.EPS_THRESHOLD, False)
			yhatL2norm = Transforms.max(yhatL2norm, Nd4j.EPS_THRESHOLD, False)
			yhatL2normSq = Transforms.max(yhatL2normSq, Nd4j.EPS_THRESHOLD, False)

			dLda.diviColumnVector(yL2norm)
			dLda.diviColumnVector(yhatL2norm.mul(yhatL2normSq))
			dLda.muli(-1)

			'dL/dz
			Dim gradients As INDArray = activationFn.backprop(preOutput, dLda).First 'TODO loss functions with params

			If mask IsNot Nothing Then
				gradients.muliColumnVector(mask)
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
			Return "LossCosineProximity()"
		End Function
	End Class

End Namespace