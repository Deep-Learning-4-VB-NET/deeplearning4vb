Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports SoftMax = org.nd4j.linalg.api.ops.impl.transforms.custom.SoftMax
Imports TimesOneMinus = org.nd4j.linalg.api.ops.impl.transforms.same.TimesOneMinus
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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
'ORIGINAL LINE: @EqualsAndHashCode @JsonInclude(JsonInclude.Include.NON_NULL) @Getter @Setter public class LossBinaryXENT implements org.nd4j.linalg.lossfunctions.ILossFunction
	<Serializable>
	Public Class LossBinaryXENT
		Implements ILossFunction

		Public Const DEFAULT_CLIPPING_EPSILON As Double = 1e-5

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private final org.nd4j.linalg.api.ndarray.INDArray weights;
		Private ReadOnly weights As INDArray

		Private clipEps As Double

		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Binary cross entropy where each the output is
		''' (optionally) weighted/scaled by a fixed scalar value.
		''' Note that the weights array must be a row vector, of length equal to
		''' the labels/output dimension 1 size.
		''' A weight vector of 1s should give identical results to no weight vector.
		''' </summary>
		''' <param name="weights"> Weights array (row vector). May be null. </param>
		Public Sub New(ByVal weights As INDArray)
			Me.New(DEFAULT_CLIPPING_EPSILON, weights)
		End Sub

		''' <summary>
		''' Binary cross entropy where each the output is
		''' (optionally) weighted/scaled by a fixed scalar value.
		''' Note that the weights array must be a row vector, of length equal to
		''' the labels/output dimension 1 size.
		''' A weight vector of 1s should give identical results to no weight vector.
		''' </summary>
		''' <param name="clipEps"> Epsilon value for clipping. Probabilities are clipped in range of [eps, 1-eps]. Default eps: 1e-5 </param>
		Public Sub New(ByVal clipEps As Double)
			Me.New(clipEps, Nothing)
		End Sub

		''' <summary>
		''' Binary cross entropy where each the output is
		''' (optionally) weighted/scaled by a fixed scalar value.
		''' Note that the weights array must be a row vector, of length equal to
		''' the labels/output dimension 1 size.
		''' A weight vector of 1s should give identical results to no weight vector.
		''' </summary>
		''' <param name="clipEps"> Epsilon value for clipping. Probabilities are clipped in range of [eps, 1-eps]. Default eps: 1e-5 </param>
		''' <param name="weights"> Weights array (row vector). May be null. </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public LossBinaryXENT(@JsonProperty("clipEps") double clipEps, @JsonProperty("weights") org.nd4j.linalg.api.ndarray.INDArray weights)
		Public Sub New(ByVal clipEps As Double, ByVal weights As INDArray)
			If weights IsNot Nothing AndAlso Not weights.RowVector Then
				Throw New System.ArgumentException("Weights array must be a row vector")
			End If
			If clipEps < 0 OrElse clipEps > 0.5 Then
				Throw New System.ArgumentException("Invalid clipping epsilon value: epsilon should be >= 0 (but near zero)." & "Got: " & clipEps)
			End If

			Me.clipEps = clipEps
			Me.weights = weights
		End Sub

		Private Function scoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			If Not labels.equalShapes(preOutput) Then
				Preconditions.throwEx("Labels and preOutput must have equal shapes: got shapes %s vs %s", labels.shape(), preOutput.shape())
			End If
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype

			Dim scoreArr As INDArray
			If TypeOf activationFn Is ActivationSoftmax Then
				'TODO Post GPU support for custom ops: Use LogSoftMax op to avoid numerical issues when calculating score
				Dim logsoftmax As INDArray = Nd4j.exec(DirectCast(New SoftMax(preOutput, preOutput.ulike(), -1), CustomOp))(0)
				Transforms.log(logsoftmax, False)
				scoreArr = logsoftmax.muli(labels)

			Else
				Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)
				If clipEps > 0.0 Then
					Dim op As CustomOp = DynamicCustomOp.builder("clipbyvalue").addInputs(output).callInplace(True).addFloatingPointArguments(clipEps, 1.0-clipEps).build()
					Nd4j.Executioner.execAndReturn(op)
				End If
				scoreArr = Transforms.log(output, True).muli(labels)
				Dim secondTerm As INDArray = output.rsubi(1)
				Transforms.log(secondTerm, False)
				secondTerm.muli(labels.rsub(1))
				scoreArr.addi(secondTerm)
			End If

			'Weighted loss function
			If weights IsNot Nothing Then
				If weights.length() <> preOutput.size(1) Then
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
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype

			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)
			If clipEps > 0.0 Then
				Dim op As CustomOp = DynamicCustomOp.builder("clipbyvalue").addInputs(output).callInplace(True).addFloatingPointArguments(clipEps, 1.0-clipEps).build()
				Nd4j.Executioner.execAndReturn(op)
			End If

			Dim numerator As INDArray = output.sub(labels)
			Dim denominator As INDArray = Nd4j.Executioner.exec(New TimesOneMinus(output)) ' output * (1-output)
			Dim dLda As INDArray = numerator.divi(denominator)

			If mask IsNot Nothing AndAlso LossUtil.isPerOutputMasking(dLda, mask) Then
				'For *most* activation functions: we don't actually need to mask dL/da in addition to masking dL/dz later
				'but: some, like softmax, require both (due to dL/dz_i being a function of dL/da_j, for i != j)
				'We could add a special case for softmax (activationFn instanceof ActivationSoftmax) but that would be
				' error prone - but buy us a tiny bit of performance
				LossUtil.applyMask(dLda, mask)
			End If

			Dim grad As INDArray = activationFn.backprop(preOutput, dLda).First 'TODO activation functions with weights

			'Weighted loss function
			If weights IsNot Nothing Then
				If weights.length() <> output.size(1) Then
					Throw New System.InvalidOperationException("Weights vector (length " & weights.length() & ") does not match output.size(1)=" & output.size(1))
				End If

				grad.muliRowVector(weights.castTo(grad.dataType()))
			End If

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
				Return "LossBinaryXENT()"
			End If
			Return "LossBinaryXENT(weights=" & weights & ")"
		End Function
	End Class

End Namespace