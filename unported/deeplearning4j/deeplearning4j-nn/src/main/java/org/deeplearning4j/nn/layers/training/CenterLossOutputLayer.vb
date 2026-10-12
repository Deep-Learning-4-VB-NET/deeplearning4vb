Imports System
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports CenterLossParamInitializer = org.deeplearning4j.nn.params.CenterLossParamInitializer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType

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

Namespace org.deeplearning4j.nn.layers.training


	<Serializable>
	Public Class CenterLossOutputLayer
		Inherits BaseOutputLayer(Of org.deeplearning4j.nn.conf.layers.CenterLossOutputLayer)

		Private fullNetRegTerm As Double

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		''' <summary>
		''' Compute score after labels and input have been set. </summary>
		''' <param name="fullNetRegTerm"> Regularization score term for the entire network </param>
		''' <param name="training"> whether score should be calculated at train or test time (this affects things like application of
		'''                 dropout, etc) </param>
		''' <returns> score (loss function) </returns>
		Public Overrides Function computeScore(ByVal fullNetRegTerm As Double, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Double
			If input_Conflict Is Nothing OrElse labels_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
			End If
			Me.fullNetRegTerm = fullNetRegTerm
			Dim preOut As INDArray = preOutput2d(training, workspaceMgr)

			' center loss has two components
			' the first enforces inter-class dissimilarity, the second intra-class dissimilarity (squared l2 norm of differences)
			Dim interClassLoss As ILossFunction = layerConf().getLossFn()

			' calculate the intra-class score component
			Dim centers As INDArray = params(CenterLossParamInitializer.CENTER_KEY)
			Dim l As INDArray = labels_Conflict.castTo(centers.dataType()) 'Ensure correct dtype (same as params); no-op if already correct dtype
			Dim centersForExamples As INDArray = l.mmul(centers)

			'        double intraClassScore = intraClassLoss.computeScore(centersForExamples, input, Activation.IDENTITY.getActivationFunction(), maskArray, false);
			Dim norm2DifferenceSquared As INDArray = input_Conflict.sub(centersForExamples).norm2(1)
			norm2DifferenceSquared.muli(norm2DifferenceSquared)

			Dim sum As Double = norm2DifferenceSquared.sumNumber().doubleValue()
			Dim lambda As Double = layerConf().getLambda()
			Dim intraClassScore As Double = lambda / 2.0 * sum

			'        intraClassScore = intraClassScore * layerConf().getLambda() / 2;

			' now calculate the inter-class score component
			Dim interClassScore As Double = interClassLoss.computeScore(getLabels2d(workspaceMgr, ArrayType.FF_WORKING_MEM), preOut, layerConf().getActivationFn(), maskArray_Conflict, False)

			Dim score As Double = interClassScore + intraClassScore

			score /= InputMiniBatchSize
			score += fullNetRegTerm

			Me.score_Conflict = score
			Return score
		End Function

		''' <summary>
		'''Compute the score for each example individually, after labels and input have been set.
		''' </summary>
		''' <param name="fullNetRegTerm"> Regularization term for the entire network (or, 0.0 to not include regularization) </param>
		''' <returns> A column INDArray of shape [numExamples,1], where entry i is the score of the ith example </returns>
		Public Overrides Function computeScoreForExamples(ByVal fullNetRegTerm As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			If input_Conflict Is Nothing OrElse labels_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
			End If
			Dim preOut As INDArray = preOutput2d(False, workspaceMgr)

			' calculate the intra-class score component
			Dim centers As INDArray = params(CenterLossParamInitializer.CENTER_KEY)
			Dim centersForExamples As INDArray = labels_Conflict.mmul(centers)
			Dim intraClassScoreArray As INDArray = input_Conflict.sub(centersForExamples)

			' calculate the inter-class score component
			Dim interClassLoss As ILossFunction = layerConf().getLossFn()
			Dim scoreArray As INDArray = interClassLoss.computeScoreArray(getLabels2d(workspaceMgr, ArrayType.FF_WORKING_MEM), preOut, layerConf().getActivationFn(), maskArray_Conflict)
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
			scoreArray.addi(intraClassScoreArray.muli(layerConf().getLambda() / 2))

			If fullNetRegTerm <> 0.0 Then
				scoreArray.addi(fullNetRegTerm)
			End If
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, scoreArray)
		End Function

		Public Overrides Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
			If input_Conflict Is Nothing OrElse labels_Conflict Is Nothing Then
				Return
			End If

			Dim preOut As INDArray = preOutput2d(True, workspaceMgr)
			Dim pair As Pair(Of Gradient, INDArray) = getGradientsAndDelta(preOut, workspaceMgr)
			Me.gradient_Conflict = pair.First

			score_Conflict = computeScore(fullNetRegTerm, True, workspaceMgr)
		End Sub

		Protected Friend Overrides WriteOnly Property ScoreWithZ As INDArray
			Set(ByVal z As INDArray)
				Throw New Exception("Not supported " & layerId())
			End Set
		End Property

		Public Overrides Function gradientAndScore() As Pair(Of Gradient, Double)
			Return New Pair(Of Gradient, Double)(gradient(), score())
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim pair As Pair(Of Gradient, INDArray) = getGradientsAndDelta(preOutput2d(True, workspaceMgr), workspaceMgr) 'Returns Gradient and delta^(this), not Gradient and epsilon^(this-1)
			Dim delta As INDArray = pair.Second

			' centers
			Dim centers As INDArray = params(CenterLossParamInitializer.CENTER_KEY)
			Dim l As INDArray = labels_Conflict.castTo(centers.dataType()) 'Ensure correct dtype (same as params); no-op if already correct dtype
			Dim centersForExamples As INDArray = l.mmul(centers)
			Dim dLcdai As INDArray = input_Conflict.sub(centersForExamples)

			Dim w As INDArray = getParamWithNoise(CenterLossParamInitializer.WEIGHT_KEY, True, workspaceMgr)

			Dim epsilonNext As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, w.dataType(), New Long(){w.size(0), delta.size(0)}, "f"c)
			epsilonNext = w.mmuli(delta.transpose(), epsilonNext).transpose()
			Dim lambda As Double = layerConf().getLambda()
			epsilonNext.addi(dLcdai.muli(lambda)) ' add center loss here

			weightNoiseParams.Clear()

			Return New Pair(Of Gradient, INDArray)(pair.First, epsilonNext)
		End Function

		''' <summary>
		''' Gets the gradient from one training iteration </summary>
		''' <returns> the gradient (bias and weight matrix) </returns>
		Public Overrides Function gradient() As Gradient
			Return gradient_Conflict
		End Function

		''' <summary>
		''' Returns tuple: {Gradient,Delta,Output} given preOut </summary>
		Private Function getGradientsAndDelta(ByVal preOut As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim labels2d As INDArray = getLabels2d(workspaceMgr, ArrayType.BP_WORKING_MEM)
			If labels2d.size(1) <> preOut.size(1) Then
				Throw New DL4JInvalidInputException("Labels array numColumns (size(1) = " & labels2d.size(1) & ") does not match output layer" & " number of outputs (nOut = " & preOut.size(1) & ") " & layerId())
			End If

			Dim delta As INDArray = lossFunction.computeGradient(labels2d, preOut, layerConf().getActivationFn(), maskArray_Conflict)

			Dim gradient As Gradient = New DefaultGradient()

			Dim weightGradView As INDArray = gradientViews(CenterLossParamInitializer.WEIGHT_KEY)
			Dim biasGradView As INDArray = gradientViews(CenterLossParamInitializer.BIAS_KEY)
			Dim centersGradView As INDArray = gradientViews(CenterLossParamInitializer.CENTER_KEY)

			' centers delta
			Dim alpha As Double = layerConf().getAlpha()

			Dim centers As INDArray = params(CenterLossParamInitializer.CENTER_KEY)
			Dim l As INDArray = labels_Conflict.castTo(centers.dataType()) 'Ensure correct dtype (same as params); no-op if already correct dtype
			Dim centersForExamples As INDArray = l.mmul(centers)
			Dim diff As INDArray = centersForExamples.sub(input_Conflict).muli(alpha)
			Dim numerator As INDArray = l.transpose().mmul(diff)
			Dim denominator As INDArray = l.sum(0).reshape(ChrW(l.size(1)), 1).addi(1.0)

			Dim deltaC As INDArray
			If layerConf().getGradientCheck() Then
				Dim lambda As Double = layerConf().getLambda()
				'For gradient checks: need to multiply dLc/dcj by lambda to get dL/dcj
				deltaC = numerator.muli(lambda)
			Else
				deltaC = numerator.diviColumnVector(denominator)
			End If
			centersGradView.assign(deltaC)



			' other standard calculations
			Nd4j.gemm(input_Conflict, delta, weightGradView, True, False, 1.0, 0.0) 'Equivalent to:  weightGradView.assign(input.transpose().mmul(delta));
			delta.sum(biasGradView, 0) 'biasGradView is initialized/zeroed first in sum op

			gradient.gradientForVariable()(CenterLossParamInitializer.WEIGHT_KEY) = weightGradView
			gradient.gradientForVariable()(CenterLossParamInitializer.BIAS_KEY) = biasGradView
			gradient.gradientForVariable()(CenterLossParamInitializer.CENTER_KEY) = centersGradView

			Return New Pair(Of Gradient, INDArray)(gradient, delta)
		End Function

		Protected Friend Overrides Function getLabels2d(ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			Return labels_Conflict
		End Function
	End Class

End Namespace