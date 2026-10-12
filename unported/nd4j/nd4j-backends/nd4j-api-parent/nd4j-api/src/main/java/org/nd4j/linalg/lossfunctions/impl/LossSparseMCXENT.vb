Imports System
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OneHot = org.nd4j.linalg.api.ops.impl.shape.OneHot
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
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
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @JsonInclude(JsonInclude.Include.NON_NULL) @Getter @Setter public class LossSparseMCXENT extends LossMCXENT
	<Serializable>
	Public Class LossSparseMCXENT
		Inherits LossMCXENT

		Private Const DEFAULT_SOFTMAX_CLIPPING_EPSILON As Double = 1e-10

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
'ORIGINAL LINE: public LossSparseMCXENT(@JsonProperty("softmaxClipEps") double softmaxClipEps, @JsonProperty("weights") org.nd4j.linalg.api.ndarray.INDArray weights)
		Public Sub New(ByVal softmaxClipEps As Double, ByVal weights As INDArray)
			MyBase.New(softmaxClipEps, weights)
		End Sub

		Protected Friend Overridable Function sparseScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			Dim oneHotLabels As INDArray = toOneHot(labels, preOutput)
			Return MyBase.scoreArray(oneHotLabels, preOutput, activationFn, mask)
		End Function

		Public Overrides Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double
			Dim oneHotLabels As INDArray = toOneHot(labels, preOutput)
			Return MyBase.computeScore(oneHotLabels, preOutput, activationFn, mask, average)
		End Function

		Public Overrides Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			Dim scoreArr As INDArray = sparseScoreArray(labels, preOutput, activationFn, mask)
			Return scoreArr.sum(True,1).muli(-1)
		End Function

		Public Overrides Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray
			Dim oneHotLabels As INDArray = toOneHot(labels, preOutput)
			Return MyBase.computeGradient(oneHotLabels, preOutput, activationFn, mask)
		End Function

		Public Overrides Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray)
			Dim oneHotLabels As INDArray = toOneHot(labels, preOutput)
			Return New Pair(Of Double, INDArray)(MyBase.computeScore(oneHotLabels, preOutput, activationFn, mask, average), MyBase.computeGradient(oneHotLabels, preOutput, activationFn, mask))
		End Function

		Private Function toOneHot(ByVal labels As INDArray, ByVal preOutput As INDArray) As INDArray
			Preconditions.checkState(labels.size(-1) = 1, "Labels for LossSparseMCXENT should be an array of integers " & "with first dimension equal to minibatch size, and last dimension having size 1. Got labels array with shape %ndShape", labels)
			Dim oneHotLabels As INDArray = preOutput.ulike()
			Nd4j.exec(New OneHot(labels.reshape(ChrW(labels.length())), oneHotLabels, CInt(preOutput.size(-1))))
			Return oneHotLabels
		End Function


		Public Overrides Function ToString() As String
			If weights Is Nothing Then
				Return "LossSparseMCXENT()"
			End If
			Return "LossSparseMCXENT(weights=" & weights & ")"
		End Function
	End Class

End Namespace