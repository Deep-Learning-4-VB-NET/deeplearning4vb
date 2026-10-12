Imports System
Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
Namespace org.nd4j.linalg.lossfunctions


	<Serializable>
	Public MustInherit Class SameDiffLoss
		Implements ILossFunction

		<NonSerialized>
		Protected Friend sd As SameDiff
		<NonSerialized>
		Protected Friend scorePerExampleVariable As SDVariable

		Protected Friend Sub New()

		End Sub

		''' <summary>
		''' Define the loss function.<br>
		''' <b>NOTE</b>: The score on a *per example* basis - should return a SDVariable with shape [minibatch], where out[i]
		''' is the score for the ith minibatch
		''' </summary>
		''' <param name="sd">         SameDiff instance to define the loss on </param>
		''' <param name="layerInput"> Input to the SameDiff loss function </param>
		''' <param name="labels">     Labels placeholder </param>
		''' <returns> The score on a per example basis (SDVariable with shape [minibatch]) </returns>
		Public MustOverride Function defineLoss(ByVal sd As SameDiff, ByVal layerInput As SDVariable, ByVal labels As SDVariable) As SDVariable

		Protected Friend Overridable Sub createSameDiffInstance(ByVal dataType As DataType)
			sd = SameDiff.create()
			Dim layerInput As SDVariable = sd.placeHolder("layerInput", dataType, -1)
			Dim labels As SDVariable = sd.placeHolder("labels", dataType, -1)
			scorePerExampleVariable = Me.defineLoss(sd, layerInput, labels)
			scorePerExampleVariable.markAsLoss()
			sd.createGradFunction("layerInput")
		End Sub

		''' <summary>
		''' Compute the score (loss function value) for the given inputs.
		''' </summary>
		''' <param name="labels">       Label/expected preOutput </param>
		''' <param name="preOutput">    Output of the model (neural network) </param>
		''' <param name="activationFn"> Activation function that should be applied to preOutput </param>
		''' <param name="mask">         Mask array; may be null </param> </param>
		''' <param name="average">      Whether the score should be averaged (divided by number of rows in labels/preOutput) or not   <returns> Loss function value </returns>
		Public Overridable Function computeScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Double Implements ILossFunction.computeScore
			If sd Is Nothing Then
				createSameDiffInstance(preOutput.dataType())
			End If

			Dim scoreArr As INDArray = computeScoreArray(labels, preOutput, activationFn, mask)

			Dim score As Double = scoreArr.sumNumber().doubleValue()
			If average Then
				score /= scoreArr.size(0)
			End If
			Return score
		End Function


		''' <summary>
		''' Compute the score (loss function value) for each example individually.
		''' For input [numExamples,nOut] returns scores as a column vector: [numExamples,1]
		''' </summary>
		''' <param name="labels">       Labels/expected output </param>
		''' <param name="preOutput">    Output of the model (neural network) </param>
		''' <param name="activationFn"> Activation function that should be applied to preOutput </param> </param>
		''' <param name="mask">         <returns> Loss function value for each example; column vector </returns>
		Public Overridable Function computeScoreArray(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeScoreArray
			If sd Is Nothing Then
				createSameDiffInstance(preOutput.dataType())
			End If

			Preconditions.checkArgument((labels.size(1) = preOutput.size(1)), "Labels array numColumns (size(1) = %s) does not match output layer number of outputs (nOut = %s)", labels.size(1), preOutput.size(1))

			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)

			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			m("labels") = labels
			m("layerInput") = output

			Dim scoreArr As INDArray = sd.outputSingle(m, scorePerExampleVariable.name())

			If mask IsNot Nothing Then
				LossUtil.applyMask(scoreArr, mask)
			End If
			Return scoreArr
		End Function


		''' <summary>
		''' Compute the gradient of the loss function with respect to the inputs: dL/dOutput
		''' </summary>
		''' <param name="labels">       Label/expected output </param>
		''' <param name="preOutput">    Output of the model (neural network), before the activation function is applied </param>
		''' <param name="activationFn"> Activation function that should be applied to preOutput </param>
		''' <param name="mask">         Mask array; may be null </param>
		''' <returns> Gradient dL/dPreOut </returns>
		Public Overridable Function computeGradient(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray) As INDArray Implements ILossFunction.computeGradient
			If sd Is Nothing Then
				createSameDiffInstance(preOutput.dataType())
			End If


			Dim m As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			Dim output As INDArray = activationFn.getActivation(preOutput.dup(), True)
			m("labels") = labels
			m("layerInput") = output

			Dim grads As IDictionary(Of String, INDArray) = sd.calculateGradients(m, "layerInput")

			Dim gradAtActivationOutput As INDArray = grads("layerInput")
			Dim gradAtInput As INDArray = activationFn.backprop(preOutput.dup(), gradAtActivationOutput).First

			If mask IsNot Nothing Then
				LossUtil.applyMask(gradAtInput, mask)
			End If
			Return gradAtInput
		End Function

		''' <summary>
		''' Compute both the score (loss function value) and gradient. This is equivalent to calling <seealso cref="computeScore(INDArray, INDArray, IActivation, INDArray, Boolean)"/>
		''' and <seealso cref="computeGradient(INDArray, INDArray, IActivation, INDArray)"/> individually
		''' </summary>
		''' <param name="labels">       Label/expected output </param>
		''' <param name="preOutput">    Output of the model (neural network) </param>
		''' <param name="activationFn"> Activation function that should be applied to preOutput </param>
		''' <param name="mask">         Mask array; may be null </param>
		''' <param name="average">      Whether the score should be averaged (divided by number of rows in labels/output) or not </param>
		''' <returns> The score (loss function value) and gradient </returns>
		Public Overridable Function computeGradientAndScore(ByVal labels As INDArray, ByVal preOutput As INDArray, ByVal activationFn As IActivation, ByVal mask As INDArray, ByVal average As Boolean) As Pair(Of Double, INDArray) Implements ILossFunction.computeGradientAndScore

			Dim GradientAndScore As New Pair(Of Double, INDArray)()
			GradientAndScore.First = Me.computeScore(labels, preOutput, activationFn, mask, average)
			GradientAndScore.Second = Me.computeGradient(labels, preOutput, activationFn, mask)

			Return GradientAndScore
		End Function

		Public Overridable Function name() As String Implements ILossFunction.name
			Return Me.GetType().Name
		End Function
	End Class





End Namespace