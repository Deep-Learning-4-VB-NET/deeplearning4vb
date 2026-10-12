Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Solver = org.deeplearning4j.optimize.Solver
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Evaluation = org.nd4j.evaluation.classification.Evaluation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
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

Namespace org.deeplearning4j.nn.layers



	<Serializable>
	Public MustInherit Class BaseOutputLayer(Of LayerConfT As org.deeplearning4j.nn.conf.layers.BaseOutputLayer)
		Inherits BaseLayer(Of LayerConfT)
		Implements IOutputLayer

		Public Overrides MustOverride Property IterationCount As Integer

		'current input and label matrices
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend labels_Conflict As INDArray

		<NonSerialized>
		Private Shadows solver As Solver

		Private fullNetRegTerm As Double

		Protected Friend inputMaskArray As INDArray
		Protected Friend inputMaskArrayState As MaskState

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		''' <summary>
		''' Compute score after labels and input have been set. </summary>
		''' <param name="fullNetRegTerm"> Regularization score term for the entire network </param>
		''' <param name="training"> whether score should be calculated at train or test time (this affects things like application of
		'''                 dropout, etc) </param>
		''' <returns> score (loss function) </returns>
		Public Overridable Function computeScore(ByVal fullNetRegTerm As Double, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Double Implements IOutputLayer.computeScore
			If input_Conflict Is Nothing OrElse labels_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
			End If
			Me.fullNetRegTerm = fullNetRegTerm
			Dim preOut As INDArray = preOutput2d(training, workspaceMgr)

			Dim lossFunction As ILossFunction = layerConf().getLossFn()

			Dim labels2d As INDArray = getLabels2d(workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim score As Double = lossFunction.computeScore(labels2d, preOut, layerConf().getActivationFn(), maskArray_Conflict,False)

			If conf().isMiniBatch() Then
				score /= InputMiniBatchSize
			End If

			score += fullNetRegTerm

			Me.score_Conflict = score
			Return score
		End Function

		Public Overridable Function needsLabels() As Boolean Implements IOutputLayer.needsLabels
			Return True
		End Function

		''' <summary>
		'''Compute the score for each example individually, after labels and input have been set.
		''' </summary>
		''' <param name="fullNetRegTerm"> Regularization score term for the entire network (or, 0.0 to not include regularization) </param>
		''' <returns> A column INDArray of shape [numExamples,1], where entry i is the score of the ith example </returns>
		Public Overridable Function computeScoreForExamples(ByVal fullNetRegTerm As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IOutputLayer.computeScoreForExamples
			If input_Conflict Is Nothing OrElse labels_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
			End If
			Dim preOut As INDArray = preOutput2d(False, workspaceMgr)

			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim scoreArray As INDArray = lossFunction.computeScoreArray(getLabels2d(workspaceMgr, ArrayType.FF_WORKING_MEM), preOut, layerConf().getActivationFn(), maskArray_Conflict)
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
				Throw New Exception("Not supported - " & layerId())
			End Set
		End Property

		Public Overrides Function gradientAndScore() As Pair(Of Gradient, Double)
			Return New Pair(Of Gradient, Double)(gradient(), score())
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim pair As Pair(Of Gradient, INDArray) = getGradientsAndDelta(preOutput2d(True, workspaceMgr), workspaceMgr) 'Returns Gradient and delta^(this), not Gradient and epsilon^(this-1)
			Dim delta As INDArray = pair.Second

			Dim w As INDArray = getParamWithNoise(DefaultParamInitializer.WEIGHT_KEY, True, workspaceMgr)
			Dim epsilonNext As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, delta.dataType(), New Long(){w.size(0), delta.size(0)}, "f"c)
			epsilonNext = w.mmuli(delta.transpose(), epsilonNext).transpose()

			'Normally we would clear weightNoiseParams here - but we want to reuse them for forward + backward + score
			' So this is instead done in MultiLayerNetwork/CompGraph backprop methods

			epsilonNext = backpropDropOutIfPresent(epsilonNext)
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
			'INDArray delta = lossFunction.computeGradient(labels2d, preOut, layerConf().getActivationFunction(), maskArray);
			Dim delta As INDArray = lossFunction.computeGradient(labels2d, preOut, layerConf().getActivationFn(), maskArray_Conflict)

			Dim gradient As Gradient = New DefaultGradient()

			Dim weightGradView As INDArray = gradientViews(DefaultParamInitializer.WEIGHT_KEY)
			Nd4j.gemm(input_Conflict.castTo(weightGradView.dataType()), delta, weightGradView, True, False, 1.0, 0.0) 'Equivalent to:  weightGradView.assign(input.transpose().mmul(delta));         //TODO can we avoid cast?
			gradient.gradientForVariable()(DefaultParamInitializer.WEIGHT_KEY) = weightGradView

			If hasBias() Then
				Dim biasGradView As INDArray = gradientViews(DefaultParamInitializer.BIAS_KEY)
				delta.sum(biasGradView, 0) 'biasGradView is initialized/zeroed first in sum op
				gradient.gradientForVariable()(DefaultParamInitializer.BIAS_KEY) = biasGradView
			End If

			delta = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, delta)
			Return New Pair(Of Gradient, INDArray)(gradient, delta)
		End Function


		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Return activate(training, workspaceMgr)
		End Function


		''' <summary>
		''' Sets the input and labels and returns a score for the prediction
		''' wrt true labels
		''' </summary>
		''' <param name="data"> the data to score </param>
		''' <returns> the score for the given input,label pairs </returns>
		Public Overridable Function f1Score(ByVal data As DataSet) As Double
			Return f1Score(data.Features, data.Labels)
		End Function

		''' <summary>
		''' Returns the f1 score for the given examples.
		''' </summary>
		''' <param name="examples"> te the examples to classify (one example in each row) </param>
		''' <param name="labels">   the true labels </param>
		''' <returns> the scores for each ndarray </returns>
		Public Overridable Function f1Score(ByVal examples As INDArray, ByVal labels As INDArray) As Double
			Dim eval As New Evaluation()
			eval.eval(labels, activate(examples, False, LayerWorkspaceMgr.noWorkspacesImmutable()))
			Return eval.f1()
		End Function

		''' <summary>
		''' Returns the number of possible labels
		''' </summary>
		''' <returns> the number of possible labels for this classifier </returns>
		Public Overridable Function numLabels() As Integer
			Return CInt(labels_Conflict.size(1))
		End Function

		Public Overridable Overloads Sub fit(ByVal iter As DataSetIterator)
			Do While iter.MoveNext()
				fit(iter.Current)
			Loop
		End Sub

		''' <summary>
		''' Returns the predictions for each example in the dataset </summary>
		''' <param name="input"> the matrix to predict </param>
		''' <returns> the prediction for the dataset </returns>
		Public Overridable Function predict(ByVal input As INDArray) As Integer()
			Dim output As INDArray = activate(input, False, LayerWorkspaceMgr.noWorkspacesImmutable())
			Preconditions.checkState(output.rank() = 2, "predict(INDArray) method can only be used on rank 2 output - got array with rank %s", output.rank())
			Return output.argMax(1).toIntVector()
		End Function

		''' <summary>
		''' Return predicted label names
		''' </summary>
		''' <param name="dataSet"> to predict </param>
		''' <returns> the predicted labels for the dataSet </returns>
		Public Overridable Function predict(ByVal dataSet As DataSet) As IList(Of String)
			Dim intRet() As Integer = predict(dataSet.Features)
			Dim ret As IList(Of String) = New List(Of String)()
			For Each i As Integer In intRet
				ret.Insert(i, dataSet.getLabelName(i))
			Next i
			Return ret
		End Function

		''' <summary>
		''' Fit the model
		''' </summary>
		''' <param name="input"> the examples to classify (one example in each row) </param>
		''' <param name="labels">   the example labels(a binary outcome matrix) </param>
		Public Overridable Overloads Sub fit(ByVal input As INDArray, ByVal labels As INDArray)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		''' <summary>
		''' Fit the model
		''' </summary>
		''' <param name="data"> the data to train on </param>
		Public Overridable Overloads Sub fit(ByVal data As DataSet)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		''' <summary>
		''' Fit the model
		''' </summary>
		''' <param name="examples"> the examples to classify (one example in each row) </param>
		''' <param name="labels">   the labels for each example (the number of labels must match </param>
		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels() As Integer)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides Sub clear()
			MyBase.clear()
			labels_Conflict = Nothing
			solver = Nothing
			inputMaskArrayState = Nothing
			inputMaskArray = Nothing
			fullNetRegTerm = 0.0
		End Sub

		Public Overrides Sub fit(ByVal data As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Property Labels As INDArray Implements IOutputLayer.getLabels
			Get
				Return labels_Conflict
			End Get
			Set(ByVal labels As INDArray)
				Me.labels_Conflict = labels
			End Set
		End Property


		Protected Friend Overridable Function preOutput2d(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return preOutput(training, workspaceMgr)
		End Function

		Protected Friend Overrides Sub applyMask(ByVal [to] As INDArray)
			'For output layers: can be either per-example masking, or per-
			If maskArray_Conflict.ColumnVectorOrScalar Then
				[to].muliColumnVector(maskArray_Conflict.castTo([to].dataType()))
			ElseIf [to].shape().SequenceEqual(maskArray_Conflict.shape()) Then
				[to].muli(maskArray_Conflict.castTo([to].dataType()))
			Else
				Throw New System.InvalidOperationException("Invalid mask array: per-example masking should be a column vector, " & "per output masking arrays should be the same shape as the output/labels arrays. Mask shape: " & Arrays.toString(maskArray_Conflict.shape()) & ", output shape: " & Arrays.toString([to].shape()) & layerId())
			End If
		End Sub

		Protected Friend MustOverride Function getLabels2d(ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function hasBias() As Boolean
			Return layerConf().hasBias()
		End Function
	End Class

End Namespace