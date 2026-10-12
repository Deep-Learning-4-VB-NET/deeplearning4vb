Imports System
Imports System.Collections.Generic
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports Solver = org.deeplearning4j.optimize.Solver
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.common.primitives
Imports FeatureUtil = org.nd4j.linalg.util.FeatureUtil
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

Namespace org.deeplearning4j.nn.layers




	<Serializable>
	Public Class LossLayer
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.LossLayer)
		Implements IOutputLayer

		'current input and label matrices
'JAVA TO VB CONVERTER NOTE: The field labels was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend labels_Conflict As INDArray

		<NonSerialized>
		Private Shadows solver As Solver

		Private fullNetworkRegularizationScore As Double

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
			Me.fullNetworkRegularizationScore = fullNetRegTerm
			Dim preOut As INDArray = input_Conflict

			Dim lossFunction As ILossFunction = layerConf().getLossFn()

			'double score = lossFunction.computeScore(getLabels2d(), preOut, layerConf().getActivationFunction(), maskArray, false);
			Dim score As Double = lossFunction.computeScore(Labels2d, preOut, layerConf().getActivationFn(), maskArray_Conflict, False)
			score /= InputMiniBatchSize
			score += fullNetworkRegularizationScore

			Me.score_Conflict = score
			Return score
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
			Dim preOut As INDArray = input_Conflict

			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim scoreArray As INDArray = lossFunction.computeScoreArray(Labels2d, preOut, layerConf().getActivationFn(), maskArray_Conflict)
			If fullNetRegTerm <> 0.0 Then
				scoreArray.addi(fullNetRegTerm)
			End If
			Return workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, scoreArray)
		End Function

		Public Overrides Sub computeGradientAndScore(ByVal workspaceMgr As LayerWorkspaceMgr)
			If input_Conflict Is Nothing OrElse labels_Conflict Is Nothing Then
				Return
			End If

			Dim preOut As INDArray = input_Conflict
			Dim pair As Pair(Of Gradient, INDArray) = getGradientsAndDelta(preOut, workspaceMgr)
			Me.gradient_Conflict = pair.First

			score_Conflict = computeScore(fullNetworkRegularizationScore, True, workspaceMgr)
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
			Return getGradientsAndDelta(input_Conflict, workspaceMgr)
		End Function


		''' <summary>
		''' Returns tuple: {Gradient,Delta,Output} given preOut </summary>
		Private Function getGradientsAndDelta(ByVal preOut As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			' delta calculation
			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim delta As INDArray = lossFunction.computeGradient(Labels2d, preOut, layerConf().getActivationFn(), maskArray_Conflict)

			' grab the empty gradient
			Dim gradient As Gradient = New DefaultGradient()

			delta = workspaceMgr.leverageTo(ArrayType.ACTIVATION_GRAD, delta)
			Return New Pair(Of Gradient, INDArray)(gradient, delta)
		End Function

		''' <summary>
		''' Gets the gradient from one training iteration </summary>
		''' <returns> the gradient (bias and weight matrix) </returns>
		Public Overrides Function gradient() As Gradient
			Return gradient_Conflict
		End Function

		Public Overrides Function calcRegularizationScore(ByVal backpropOnlyParams As Boolean) As Double
			Return 0
		End Function

		Public Overrides Function type() As Type
			Return Type.FEED_FORWARD
		End Function

		Public Overrides Sub fit(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim z As INDArray = input_Conflict
			Dim ret As INDArray = layerConf().getActivationFn().getActivation(z.dup(), training)

			If maskArray_Conflict IsNot Nothing Then
				ret.muliColumnVector(maskArray_Conflict)
			End If

			Dim [out] As INDArray = workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, ret)
			Return [out]
		End Function

		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Return activate(training, workspaceMgr)
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function params() As INDArray
			Return Nothing
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
		''' Think of this to be like a percentage right.
		''' The higher the number the more it got right.
		''' This is on a scale from 0 to 1.
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
			' no-op
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
			Throw New System.NotSupportedException("LossLayer has no parameters and cannot be fit")
		End Sub

		''' <summary>
		''' Fit the model
		''' </summary>
		''' <param name="data"> the data to train on </param>
		Public Overridable Overloads Sub fit(ByVal data As DataSet)
			fit(data.Features, data.Labels)
		End Sub

		''' <summary>
		''' Fit the model
		''' </summary>
		''' <param name="examples"> the examples to classify (one example in each row) </param>
		''' <param name="labels">   the labels for each example (the number of labels must match </param>
		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels() As Integer)
			Dim outcomeMatrix As INDArray = FeatureUtil.toOutcomeMatrix(labels, numLabels())
			fit(examples, outcomeMatrix)

		End Sub

		Public Overrides Sub clear()
			MyBase.clear()
			If labels_Conflict IsNot Nothing Then
				labels_Conflict.data().destroy()
				labels_Conflict = Nothing
			End If
			solver = Nothing
		End Sub

		Public Overridable Property Labels As INDArray Implements IOutputLayer.getLabels
			Get
				Return labels_Conflict
			End Get
			Set(ByVal labels As INDArray)
				Me.labels_Conflict = labels
			End Set
		End Property

		Public Overridable Function needsLabels() As Boolean Implements IOutputLayer.needsLabels
			Return True
		End Function


		Protected Friend Overridable ReadOnly Property Labels2d As INDArray
			Get
				If labels_Conflict.rank() > 2 Then
					Return labels_Conflict.reshape(ChrW(labels_Conflict.size(2)), labels_Conflict.size(1))
				End If
				Return labels_Conflict
			End Get
		End Property

	End Class

End Namespace