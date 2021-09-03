Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossUtil = org.nd4j.linalg.lossfunctions.LossUtil
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports NDArrayTextDeSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer
Imports NDArrayTextSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonProperty = org.nd4j.shade.jackson.annotation.JsonProperty
Imports JsonDeserialize = org.nd4j.shade.jackson.databind.annotation.JsonDeserialize
Imports JsonSerialize = org.nd4j.shade.jackson.databind.annotation.JsonSerialize

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
'ORIGINAL LINE: @EqualsAndHashCode @JsonInclude(JsonInclude.Include.NON_NULL) @Getter @Setter public class LossMCXENT implements org.nd4j.linalg.lossfunctions.ILossFunction
	<Serializable>
	Public Class LossMCXENT
		Implements ILossFunction

		Private Const DEFAULT_SOFTMAX_CLIPPING_EPSILON As Double = 1e-10

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) protected org.nd4j.linalg.api.ndarray.INDArray weights;
		Protected Friend weights As INDArray

		Protected Friend softmaxClipEps As Double

		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Multi-Class Cross Entropy loss function where each the output is (optionally) weighted/scaled by a flags scalar value.
		''' Note that the weights array must be a row vector, of length equal to the labels/output dimension 1 size.
		''' A weight vector of 1s should give identical results to no weight vector.
		''' </summary>
		''' <param name="weights"> Weights array (row vector). May be null. </param>
		Public Sub New(ByVal weights As INDArray)
			Me.New(DEFAULT_SOFTMAX_CLIPPING_EPSILON, weights)
		End Sub

		''' <summary>
		''' Multi-Class Cross Entropy loss function where each the output is (optionally) weighted/scaled by a fixed scalar value.
		''' Note that the weights array must be a row vector, of length equal to the labels/output dimension 1 size.
		''' A weight vector of 1s should give identical results to no weight vector.
		''' </summary>
		''' <param name="weights"> Weights array (row vector). May be null. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LossMCXENT(@JsonProperty("softmaxClipEps") double softmaxClipEps, @JsonProperty("weights") org.nd4j.linalg.api.ndarray.INDArray weights)
		Public Sub New(ByVal softmaxClipEps As Double, ByVal weights As INDArray)
			If weights IsNot Nothing AndAlso Not weights.RowVector Then
				Throw New System.ArgumentException("Weights array must be a row vector")
			End If
			If softmaxClipEps < 0 OrElse softmaxClipEps > 0.5 Then
				Throw New System.ArgumentException("Invalid clipping epsilon: epsilon should be >= 0 (but near zero). Got: " & softmaxClipEps)
			End If
			Me.weights = weights
			Me.softmaxClipEps = softmaxClipEps
		End Sub

		Protected Friend Overridable Function scoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			If Not labels.equalShapes(preOutput) Then
				Preconditions.throwEx("Labels and preOutput must have equal shapes: got shapes %s vs %s", labels.shape(), preOutput.shape())
			End If
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype

			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)
			If TypeOf activationFn Is ActivationSoftmax AndAlso softmaxClipEps > 0.0 Then
				BooleanIndexing.replaceWhere(output, softmaxClipEps, Conditions.lessThan(softmaxClipEps))
				BooleanIndexing.replaceWhere(output, 1.0-softmaxClipEps, Conditions.greaterThan(1.0-softmaxClipEps))
			End If
			Dim scoreArr As INDArray = Transforms.log(output, False).muli(labels)

			'Weighted loss function
			If weights IsNot Nothing Then
				If weights.length() <> scoreArr.size(1) Then
					Throw New System.InvalidOperationException("Weights vector (length " & weights.length() & ") does not match output.size(1)=" & preOutput.size(1))
				End If
				scoreArr.muliRowVector(weights.castTo(scoreArr.dataType()))
			End If

			If mask IsNot Nothing Then
				LossUtil.applyMask(scoreArr, mask)
			End If
			Return scoreArr
		End Function

		Public Overridable Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double Implements ILossFunction.computeScore
			Dim scoreArr As INDArray = scoreArray(labels, preOutput, activationFn, mask)

			Dim score As Double = -scoreArr.sumNumber().doubleValue()

			If average Then
				score /= scoreArr.size(0)
			End If

			Return score
		End Function

		Public Overridable Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeScoreArray
			Dim scoreArr As INDArray = scoreArray(labels, preOutput, activationFn, mask)
			Return scoreArr.sum(True,1).muli(-1)
		End Function

		Public Overridable Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeGradient
			If Not labels.equalShapes(preOutput) Then
				Preconditions.throwEx("Labels and preOutput must have equal shapes: got shapes %s vs %s", labels.shape(), preOutput.shape())
			End If
			Dim grad As INDArray
			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype

			If TypeOf activationFn Is ActivationSoftmax Then

				If mask IsNot Nothing AndAlso LossUtil.isPerOutputMasking(output, mask) Then
					Throw New System.NotSupportedException("Per output masking for MCXENT + softmax: not supported")
				End If

				'Weighted loss function
				If weights IsNot Nothing Then
					If weights.length() <> output.size(1) Then
						Throw New System.InvalidOperationException("Weights vector (length " & weights.length() & ") does not match output.size(1)=" & output.size(1))
					End If
					Dim temp As INDArray = labels.mulRowVector(weights.castTo(labels.dataType()))
					Dim col As INDArray = temp.sum(True,1)
					grad = output.mulColumnVector(col).sub(temp)
				Else
					grad = output.subi(labels)
				End If
			Else
				Dim dLda As INDArray = output.rdivi(labels).negi()

				grad = activationFn.backprop(preOutput, dLda).First 'TODO activation function with weights

				'Weighted loss function
				If weights IsNot Nothing Then
					If weights.length() <> output.size(1) Then
						Throw New System.InvalidOperationException("Weights vector (length " & weights.length() & ") does not match output.size(1)=" & output.size(1))
					End If
					grad.muliRowVector(weights.castTo(grad.dataType()))
				End If
			End If

			'Loss function with masking
			If mask IsNot Nothing Then
				LossUtil.applyMask(grad, mask)
			End If

			Return grad
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
			If weights Is Nothing Then
				Return "LossMCXENT()"
			End If
			Return "LossMCXENT(weights=" & weights & ")"
		End Function
	End Class

End Namespace