Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossUtil = org.nd4j.linalg.lossfunctions.LossUtil
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports NDArrayTextDeSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer
Imports NDArrayTextSerializer = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
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
'ORIGINAL LINE: @EqualsAndHashCode @JsonInclude(JsonInclude.Include.NON_NULL) @Getter public class LossMSLE implements org.nd4j.linalg.lossfunctions.ILossFunction
	<Serializable>
	Public Class LossMSLE
		Implements ILossFunction

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonSerialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextSerializer.class) @JsonDeserialize(using = org.nd4j.serde.jackson.shaded.NDArrayTextDeSerializer.class) private final org.nd4j.linalg.api.ndarray.INDArray weights;
		Private ReadOnly weights As INDArray

		Public Sub New()
			Me.New(Nothing)
		End Sub

		''' <summary>
		''' Mean Squared Logarithmic Error loss function where each the output is (optionally) weighted/scaled by a flags scalar value.
		''' Note that the weights array must be a row vector, of length equal to the labels/output dimension 1 size.
		''' A weight vector of 1s should give identical results to no weight vector.
		''' </summary>
		''' <param name="weights"> Weights array (row vector). May be null. </param>
		Public Sub New(ByVal weights As INDArray)
			If weights IsNot Nothing AndAlso Not weights.RowVector Then
				Throw New System.ArgumentException("Weights array must be a row vector")
			End If
			Me.weights = weights
		End Sub

		Public Overridable Function scoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			If Not labels.equalShapes(preOutput) Then
				Preconditions.throwEx("Labels and preOutput must have equal shapes: got shapes %s vs %s", labels.shape(), preOutput.shape())
			End If
			labels = labels.castTo(preOutput.dataType()) 'No-op if already correct dtype
			Dim scoreArr As INDArray
			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)
			scoreArr = Transforms.log(output.addi(1.0).divi(labels.add(1.0)), False)
			scoreArr = scoreArr.muli(scoreArr).divi(labels.size(1))

			'Weighted loss function
			If weights IsNot Nothing Then
				If weights.length() <> output.size(1) Then
					Throw New System.InvalidOperationException("Weights vector (length " & weights.length() & ") does not match output.size(1)=" & output.size(1))
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
			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)

			Dim p1 As INDArray = output.add(1.0)
			Dim dlda As INDArray = p1.rdiv(2.0 / labels.size(1))
			Dim logRatio As INDArray = Transforms.log(p1.divi(labels.add(1.0)), False)
			dlda.muli(logRatio)

			If weights IsNot Nothing Then
				dlda.muliRowVector(weights.castTo(dlda.dataType()))
			End If

			If mask IsNot Nothing AndAlso LossUtil.isPerOutputMasking(dlda, mask) Then
				'For *most* activation functions: we don't actually need to mask dL/da in addition to masking dL/dz later
				'but: some, like softmax, require both (due to dL/dz_i being a function of dL/da_j, for i != j)
				'We could add a special case for softmax (activationFn instanceof ActivationSoftmax) but that would be
				' error prone - though buy us a tiny bit of performance
				LossUtil.applyMask(dlda, mask)
			End If

			'dL/dz
			Dim gradients As INDArray = activationFn.backprop(preOutput, dlda).First 'TODO activation functions with weights

			If mask IsNot Nothing Then
				LossUtil.applyMask(gradients, mask)
			End If

			Return gradients
		End Function

		Public Overridable Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray) Implements ILossFunction.computeGradientAndScore
			'TODO: probably a more efficient way to do this...
			'Yes - will implement in round two. Just want to get done now.

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
				Return "LossMSLE()"
			End If
			Return "LossMSLE(weights=" & weights & ")"
		End Function
	End Class

End Namespace