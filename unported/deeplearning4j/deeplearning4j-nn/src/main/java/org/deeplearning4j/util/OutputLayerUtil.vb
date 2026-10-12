Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports org.deeplearning4j.nn.conf.layers
Imports Yolo2OutputLayer = org.deeplearning4j.nn.conf.layers.objdetect.Yolo2OutputLayer
Imports OCNNOutputLayer = org.deeplearning4j.nn.conf.ocnn.OCNNOutputLayer
Imports org.nd4j.evaluation
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports org.nd4j.linalg.activations.impl
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports LossBinaryXENT = org.nd4j.linalg.lossfunctions.impl.LossBinaryXENT
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT

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

Namespace org.deeplearning4j.util


	Public Class OutputLayerUtil

		Private Sub New()
		End Sub

		Private Shared ReadOnly OUTSIDE_ZERO_ONE_RANGE As ISet(Of Type) = New HashSet(Of Type)()
		Shared Sub New()
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationCube))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationELU))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationHardTanH))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationIdentity))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationLReLU))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationPReLU))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationRationalTanh))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationReLU))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationReLU6))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationRReLU))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationSELU))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationSoftPlus))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationSoftSign))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationSwish))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationTanH))
			OUTSIDE_ZERO_ONE_RANGE.Add(GetType(ActivationThresholdedReLU))
		End Sub

		Private Shared ReadOnly COMMON_MSG As String = vbLf & "This configuration validation check can be disabled for MultiLayerConfiguration" & " and ComputationGraphConfiguration using validateOutputLayerConfig(false), however this is not recommended."


		''' <summary>
		''' Validate the output layer (or loss layer) configuration, to detect invalid consfiugrations. A DL4JInvalidConfigException
		''' will be thrown for invalid configurations (like softmax + nOut=1).<br>
		''' 
		''' If the specified layer is not an output layer, this is a no-op </summary>
		''' <param name="layerName"> Name of the layer </param>
		''' <param name="layer">         Layer </param>
		Public Shared Sub validateOutputLayer(ByVal layerName As String, ByVal layer As Layer)
			Dim activation As IActivation
			Dim loss As ILossFunction
			Dim nOut As Long
			Dim isLossLayer As Boolean = False
			If TypeOf layer Is BaseOutputLayer AndAlso Not (TypeOf layer Is OCNNOutputLayer) Then
				activation = DirectCast(layer, BaseOutputLayer).getActivationFn()
				loss = DirectCast(layer, BaseOutputLayer).getLossFn()
				nOut = DirectCast(layer, BaseOutputLayer).getNOut()
			ElseIf TypeOf layer Is LossLayer Then
				activation = DirectCast(layer, LossLayer).getActivationFn()
				loss = DirectCast(layer, LossLayer).getLossFn()
				nOut = DirectCast(layer, LossLayer).getNOut()
				isLossLayer = True
			ElseIf TypeOf layer Is RnnLossLayer Then
				activation = DirectCast(layer, RnnLossLayer).getActivationFn()
				loss = DirectCast(layer, RnnLossLayer).getLossFn()
				nOut = DirectCast(layer, RnnLossLayer).getNOut()
				isLossLayer = True
			ElseIf TypeOf layer Is CnnLossLayer Then
				activation = DirectCast(layer, CnnLossLayer).getActivationFn()
				loss = DirectCast(layer, CnnLossLayer).getLossFn()
				nOut = DirectCast(layer, CnnLossLayer).getNOut()
				isLossLayer = True
			Else
				'Not an output layer
				Return
			End If
			OutputLayerUtil.validateOutputLayerConfiguration(layerName, nOut, isLossLayer, activation, loss)
		End Sub

		''' <summary>
		''' Validate the output layer (or loss layer) configuration, to detect invalid consfiugrations. A DL4JInvalidConfigException
		''' will be thrown for invalid configurations (like softmax + nOut=1).<br>
		''' <para>
		''' If the specified layer is not an output layer, this is a no-op
		''' 
		''' </para>
		''' </summary>
		''' <param name="layerName">    Name of the layer </param>
		''' <param name="nOut">         Number of outputs for the layer </param>
		''' <param name="isLossLayer">  Should be true for loss layers (no params), false for output layers </param>
		''' <param name="activation">   Activation function </param>
		''' <param name="lossFunction"> Loss function </param>
		Public Shared Sub validateOutputLayerConfiguration(ByVal layerName As String, ByVal nOut As Long, ByVal isLossLayer As Boolean, ByVal activation As IActivation, ByVal lossFunction As ILossFunction)
			'nOut = 1 + softmax
			If Not isLossLayer AndAlso nOut = 1 AndAlso TypeOf activation Is ActivationSoftmax Then 'May not have valid nOut for LossLayer
				Throw New DL4JInvalidConfigException("Invalid output layer configuration for layer """ & layerName & """: Softmax + nOut=1 networks " & "are not supported. Softmax cannot be used with nOut=1 as the output will always be exactly 1.0 " & "regardless of the input. " & COMMON_MSG)
			End If

			'loss function required probability, but activation is outside 0-1 range
			If lossFunctionExpectsProbability(lossFunction) AndAlso activationExceedsZeroOneRange(activation, isLossLayer) Then
				Throw New DL4JInvalidConfigException("Invalid output layer configuration for layer """ & layerName & """: loss function " & lossFunction & " expects activations to be in the range 0 to 1 (probabilities) but activation function " & activation & " does not bound values to this 0 to 1 range. This indicates a likely invalid network configuration. " & COMMON_MSG)
			End If

			'Common mistake: softmax + xent
			If TypeOf activation Is ActivationSoftmax AndAlso TypeOf lossFunction Is LossBinaryXENT Then
				Throw New DL4JInvalidConfigException("Invalid output layer configuration for layer """ & layerName & """: softmax activation function in combination " & "with LossBinaryXENT (binary cross entropy loss function). For multi-class classification, use softmax + " & "MCXENT (multi-class cross entropy); for binary multi-label classification, use sigmoid + XENT. " & COMMON_MSG)
			End If

			'Common mistake: sigmoid + mcxent
			If TypeOf activation Is ActivationSigmoid AndAlso TypeOf lossFunction Is LossMCXENT Then
				Throw New DL4JInvalidConfigException("Invalid output layer configuration for layer """ & layerName & """: sigmoid activation function in combination " & "with LossMCXENT (multi-class cross entropy loss function). For multi-class classification, use softmax + " & "MCXENT (multi-class cross entropy); for binary multi-label classification, use sigmoid + XENT. " & COMMON_MSG)
			End If
		End Sub

		Public Shared Function lossFunctionExpectsProbability(ByVal lf As ILossFunction) As Boolean
			'Note LossNegativeLogLikelihood extends LossMCXENT
			Return TypeOf lf Is LossMCXENT OrElse TypeOf lf Is LossBinaryXENT
		End Function

		Public Shared Function activationExceedsZeroOneRange(ByVal activation As IActivation, ByVal isLossLayer As Boolean) As Boolean

			If OUTSIDE_ZERO_ONE_RANGE.Contains(activation.GetType()) Then
				If isLossLayer AndAlso TypeOf activation Is ActivationIdentity Then
					'Note: we're intentionally excluding identity here, for situations like dense(softmax) -> loss(identity)
					'However, we might miss a few invalid configs like dense(relu) -> loss(identity)
					Return False
				End If
				Return True
			End If
			Return False
		End Function

		''' <summary>
		''' Validates if the output layer configuration is valid for classifier evaluation.
		''' This is used to try and catch invalid evaluation - i.e., trying to use classifier evaluation on a regression model.
		''' This method won't catch all possible invalid cases, but should catch some common problems.
		''' </summary>
		''' <param name="outputLayer">          Output layer </param>
		''' <param name="classifierEval">       Class for the classifier evaluation </param>
		Public Shared Sub validateOutputLayerForClassifierEvaluation(ByVal outputLayer As Layer, ByVal classifierEval As Type)
			If TypeOf outputLayer Is Yolo2OutputLayer Then
				Throw New System.InvalidOperationException("Classifier evaluation using " & classifierEval.Name & " class cannot be applied for object" & " detection evaluation using Yolo2OutputLayer: " & classifierEval.Name & "  class is for classifier evaluation only.")
			End If

			'Check that the activation function provides probabilities. This can't catch everything, but should catch a few
			' of the common mistakes users make
			If TypeOf outputLayer Is BaseLayer Then
				Dim bl As BaseLayer = DirectCast(outputLayer, BaseLayer)
				Dim isOutputLayer As Boolean = TypeOf outputLayer Is OutputLayer OrElse TypeOf outputLayer Is RnnOutputLayer OrElse TypeOf outputLayer Is CenterLossOutputLayer

				If activationExceedsZeroOneRange(bl.getActivationFn(), Not isOutputLayer) Then
					Throw New System.InvalidOperationException("Classifier evaluation using " & classifierEval.Name & " class cannot be applied to output" & " layers with activation functions that are not probabilities (in range 0 to 1). Output layer type: " & outputLayer.GetType().Name & " has activation function " & bl.getActivationFn().GetType().Name & ". This check can be disabled using MultiLayerNetwork.getLayerWiseConfigurations().setValidateOutputLayerConfig(false)" & " or ComputationGraph.getConfiguration().setValidateOutputLayerConfig(false)")
				End If
			End If
		End Sub
	End Class

End Namespace