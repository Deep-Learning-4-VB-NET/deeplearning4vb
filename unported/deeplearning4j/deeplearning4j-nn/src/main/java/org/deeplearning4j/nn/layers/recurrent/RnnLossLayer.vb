Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports Setter = lombok.Setter
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports IOutputLayer = org.deeplearning4j.nn.api.layers.IOutputLayer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.common.primitives
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr

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

Namespace org.deeplearning4j.nn.layers.recurrent


	<Serializable>
	Public Class RnnLossLayer
		Inherits BaseLayer(Of org.deeplearning4j.nn.conf.layers.RnnLossLayer)
		Implements IOutputLayer

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter @Getter protected org.nd4j.linalg.api.ndarray.INDArray labels;
		Protected Friend labels As INDArray

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			Dim input As INDArray = Me.input_Conflict
			Dim labels As INDArray = Me.labels
			If input.rank() <> 3 Then
				Throw New System.NotSupportedException("Input is not rank 3. Expected rank 3 input of shape [minibatch, size, sequenceLength]. Got input with rank " & input.rank() & " with shape " & Arrays.toString(input.shape()) & " for layer " & layerId())
			End If
			If labels Is Nothing Then
				Throw New System.InvalidOperationException("Labels are not set (null)")
			End If

			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				input = input.permute(0, 2, 1)
				labels = labels.permute(0, 2, 1)
			End If
			Preconditions.checkState(labels.rank() = 3, "Expected rank 3 labels array, got label array with shape %ndShape", labels)
			Preconditions.checkState(input.size(2) = labels.size(2), "Sequence lengths do not match for RnnOutputLayer input and labels:" & "Arrays should be rank 3 with shape [minibatch, size, sequenceLength] - mismatch on dimension 2 (sequence length) - input=%ndShape vs. label=%ndShape", input, labels)


			Dim input2d As INDArray = TimeSeriesUtils.reshape3dTo2d(input, workspaceMgr, ArrayType.BP_WORKING_MEM)
			Dim labels2d As INDArray = TimeSeriesUtils.reshape3dTo2d(labels, workspaceMgr, ArrayType.BP_WORKING_MEM)
			Dim maskReshaped As INDArray
			If Me.maskArray_Conflict IsNot Nothing Then
				If Me.maskArray_Conflict.rank() = 3 Then
					maskReshaped = TimeSeriesUtils.reshapePerOutputTimeSeriesMaskTo2d(Me.maskArray_Conflict, workspaceMgr, ArrayType.BP_WORKING_MEM)
				Else
					maskReshaped = TimeSeriesUtils.reshapeTimeSeriesMaskToVector(Me.maskArray_Conflict, workspaceMgr, ArrayType.BP_WORKING_MEM)
				End If
			Else
				maskReshaped = Nothing
			End If

			' delta calculation
			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim delta2d As INDArray = lossFunction.computeGradient(labels2d, input2d.dup(input2d.ordering()), layerConf().getActivationFn(), maskReshaped)

			Dim delta3d As INDArray = TimeSeriesUtils.reshape2dTo3d(delta2d, input.size(0), workspaceMgr, ArrayType.ACTIVATION_GRAD)
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				delta3d = delta3d.permute(0, 2, 1)
			End If
			' grab the empty gradient
			Dim gradient As Gradient = New DefaultGradient()
			Return New Pair(Of Gradient, INDArray)(gradient, delta3d)
		End Function

		Public Overrides Function calcRegularizationScore(ByVal backpropParamsOnly As Boolean) As Double
			Return 0
		End Function

		Public Overridable Function f1Score(ByVal data As DataSet) As Double
			Return 0
		End Function

		''' <summary>
		'''{@inheritDoc}
		''' </summary>
		Public Overridable Function f1Score(ByVal examples As INDArray, ByVal labels As INDArray) As Double
			Dim [out] As INDArray = activate(examples, False, Nothing)
			Dim eval As New Evaluation()
			eval.evalTimeSeries(labels, [out], maskArray_Conflict)
			Return eval.f1()
		End Function

		Public Overridable Function numLabels() As Integer
			Return CInt(labels.size(1))
		End Function

		Public Overridable Overloads Sub fit(ByVal iter As DataSetIterator)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Function predict(ByVal examples As INDArray) As Integer()
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Function predict(ByVal dataSet As DataSet) As IList(Of String)
			Throw New System.NotSupportedException("Not supported")
		End Function

		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels As INDArray)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Overloads Sub fit(ByVal data As DataSet)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overridable Overloads Sub fit(ByVal examples As INDArray, ByVal labels() As Integer)
			Throw New System.NotSupportedException("Not supported")
		End Sub

		Public Overrides Function type() As Type
			Return Type.RECURRENT
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			Dim input As INDArray = Me.input_Conflict
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				input = input.permute(0, 2, 1)
			End If
			If input.rank() <> 3 Then
				Throw New System.NotSupportedException("Input must be rank 3. Got input with rank " & input.rank() & " " & layerId())
			End If

			Dim as2d As INDArray = TimeSeriesUtils.reshape3dTo2d(input)
			Dim out2d As INDArray = layerConf().getActivationFn().getActivation(workspaceMgr.dup(ArrayType.ACTIVATIONS, as2d, as2d.ordering()), training)
			Dim ret As INDArray = workspaceMgr.leverageTo(ArrayType.ACTIVATIONS, TimeSeriesUtils.reshape2dTo3d(out2d, input.size(0), workspaceMgr, ArrayType.ACTIVATIONS))
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				ret = ret.permute(0, 2, 1)
			End If
			Return ret
		End Function

		Public Overrides WriteOnly Property MaskArray As INDArray
			Set(ByVal maskArray As INDArray)
				Me.maskArray_Conflict = maskArray
			End Set
		End Property

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			If maskArray Is Nothing Then
				Return Nothing
			End If
			Me.maskArray_Conflict = TimeSeriesUtils.reshapeTimeSeriesMaskToVector(maskArray, LayerWorkspaceMgr.noWorkspaces(), ArrayType.INPUT) 'TODO
			Me.maskState = currentMaskState

			Return Nothing 'Last layer in network
		End Function

		Public Overridable Function needsLabels() As Boolean Implements IOutputLayer.needsLabels
			Return True
		End Function

		Public Overridable Function computeScore(ByVal fullNetRegTerm As Double, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Double Implements IOutputLayer.computeScore
			Dim input As INDArray = Me.input_Conflict
			Dim labels As INDArray = Me.labels
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				input = input.permute(0, 2, 1)
				labels = input.permute(0, 2, 1)
			End If
			Dim input2d As INDArray = TimeSeriesUtils.reshape3dTo2d(input, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim labels2d As INDArray = TimeSeriesUtils.reshape3dTo2d(labels, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim maskReshaped As INDArray
			If Me.maskArray_Conflict IsNot Nothing Then
				If Me.maskArray_Conflict.rank() = 3 Then
					maskReshaped = TimeSeriesUtils.reshapePerOutputTimeSeriesMaskTo2d(Me.maskArray_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)
				Else
					maskReshaped = TimeSeriesUtils.reshapeTimeSeriesMaskToVector(Me.maskArray_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)
				End If
			Else
				maskReshaped = Nothing
			End If

			Dim lossFunction As ILossFunction = layerConf().getLossFn()

			Dim score As Double = lossFunction.computeScore(labels2d, input2d.dup(), layerConf().getActivationFn(), maskReshaped,False)
			score /= InputMiniBatchSize
			score += fullNetRegTerm

			Me.score_Conflict = score

			Return score
		End Function

		''' <summary>
		'''Compute the score for each example individually, after labels and input have been set.
		''' </summary>
		''' <param name="fullNetRegTerm"> Regularization score term for the entire network (or, 0.0 to not include regularization) </param>
		''' <returns> A column INDArray of shape [numExamples,1], where entry i is the score of the ith example </returns>
		Public Overridable Function computeScoreForExamples(ByVal fullNetRegTerm As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IOutputLayer.computeScoreForExamples
			'For RNN: need to sum up the score over each time step before returning.
			Dim input As INDArray = Me.input_Conflict
			Dim labels As INDArray = Me.labels
			If input Is Nothing OrElse labels Is Nothing Then
				Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
			End If
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				input = input.permute(0, 2, 1)
				labels = input.permute(0, 2, 1)
			End If
			Dim input2d As INDArray = TimeSeriesUtils.reshape3dTo2d(input, workspaceMgr, ArrayType.FF_WORKING_MEM)
			Dim labels2d As INDArray = TimeSeriesUtils.reshape3dTo2d(labels, workspaceMgr, ArrayType.FF_WORKING_MEM)

			Dim maskReshaped As INDArray
			If Me.maskArray_Conflict IsNot Nothing Then
				If Me.maskArray_Conflict.rank() = 3 Then
					maskReshaped = TimeSeriesUtils.reshapePerOutputTimeSeriesMaskTo2d(Me.maskArray_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)
				Else
					maskReshaped = TimeSeriesUtils.reshapeTimeSeriesMaskToVector(Me.maskArray_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)
				End If
			Else
				maskReshaped = Nothing
			End If

			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim scoreArray As INDArray = lossFunction.computeScoreArray(labels2d, input2d, layerConf().getActivationFn(), maskReshaped)
			'scoreArray: shape [minibatch*timeSeriesLength, 1]
			'Reshape it to [minibatch, timeSeriesLength] then sum over time step

			Dim scoreArrayTs As INDArray = TimeSeriesUtils.reshapeVectorToTimeSeriesMask(scoreArray, CInt(input.size(0)))
			Dim summedScores As INDArray = scoreArrayTs.sum(1)

			If fullNetRegTerm <> 0.0 Then
				summedScores.addi(fullNetRegTerm)
			End If

			Return summedScores
		End Function
	End Class

End Namespace