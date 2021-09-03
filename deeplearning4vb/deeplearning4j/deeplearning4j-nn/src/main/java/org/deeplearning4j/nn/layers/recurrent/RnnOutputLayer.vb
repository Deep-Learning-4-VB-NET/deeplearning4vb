Imports System
Imports System.Linq
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports RNNFormat = org.deeplearning4j.nn.conf.RNNFormat
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports org.deeplearning4j.nn.layers
Imports DefaultParamInitializer = org.deeplearning4j.nn.params.DefaultParamInitializer
Imports TimeSeriesUtils = org.deeplearning4j.util.TimeSeriesUtils
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
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
	Public Class RnnOutputLayer
		Inherits BaseOutputLayer(Of org.deeplearning4j.nn.conf.layers.RnnOutputLayer)

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			If input_Conflict.rank() <> 3 Then
				Throw New System.NotSupportedException("Input is not rank 3. RnnOutputLayer expects rank 3 input with shape [minibatch, layerInSize, sequenceLength]." & " Got input with rank " & input_Conflict.rank() & " and shape " & Arrays.toString(input_Conflict.shape()) & " - " & layerId())
			End If

			Dim format As RNNFormat = layerConf().getRnnDataFormat()
			Dim td As Integer = If(format = RNNFormat.NCW, 2, 1)
			Preconditions.checkState(labels_Conflict.rank() = 3, "Expected rank 3 labels array, got label array with shape %ndShape", labels_Conflict)
			Preconditions.checkState(input_Conflict.size(td) = labels_Conflict.size(td), "Sequence lengths do not match for RnnOutputLayer input and labels:" & "Arrays should be rank 3 with shape [minibatch, size, sequenceLength] - mismatch on dimension 2 (sequence length) - input=%ndShape vs. label=%ndShape", input_Conflict, labels_Conflict)


			Dim inputTemp As INDArray = input_Conflict
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				Me.input_Conflict = input_Conflict.permute(0, 2, 1)
			End If

			Me.input_Conflict = TimeSeriesUtils.reshape3dTo2d(input_Conflict, workspaceMgr, ArrayType.BP_WORKING_MEM)

			applyDropOutIfNecessary(True, workspaceMgr) 'Edge case: we skip OutputLayer forward pass during training as this isn't required to calculate gradients

			Dim gradAndEpsilonNext As Pair(Of Gradient, INDArray) = MyBase.backpropGradient(epsilon, workspaceMgr) 'Also applies dropout
			Me.input_Conflict = inputTemp
			Dim epsilon2d As INDArray = gradAndEpsilonNext.Second

			Dim epsilon3d As INDArray = TimeSeriesUtils.reshape2dTo3d(epsilon2d, input_Conflict.size(0), workspaceMgr, ArrayType.ACTIVATION_GRAD)
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				epsilon3d = epsilon3d.permute(0, 2, 1)
			End If
			weightNoiseParams.Clear()

			'epsilon3d = backpropDropOutIfPresent(epsilon3d);
			Return New Pair(Of Gradient, INDArray)(gradAndEpsilonNext.First, epsilon3d)
		End Function

		''' <summary>
		'''{@inheritDoc}
		''' </summary>
		Public Overrides Function f1Score(ByVal examples As INDArray, ByVal labels As INDArray) As Double
			If examples.rank() = 3 Then
				examples = TimeSeriesUtils.reshape3dTo2d(examples, LayerWorkspaceMgr.noWorkspaces(), ArrayType.ACTIVATIONS)
			End If
			If labels.rank() = 3 Then
				labels = TimeSeriesUtils.reshape3dTo2d(labels, LayerWorkspaceMgr.noWorkspaces(), ArrayType.ACTIVATIONS)
			End If
			Return MyBase.f1Score(examples, labels)
		End Function

		Public Overrides ReadOnly Property Input As INDArray
			Get
				Return input_Conflict
			End Get
		End Property

		Public Overrides Function type() As Type
			Return Type.RECURRENT
		End Function

		Protected Friend Overrides Function preOutput2d(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
			If input_Conflict.rank() = 3 Then
				'Case when called from RnnOutputLayer
				Dim inputTemp As INDArray = input_Conflict
				input_Conflict = If(layerConf().getRnnDataFormat() = RNNFormat.NWC, input_Conflict.permute(0, 2, 1), input_Conflict)
				input_Conflict = TimeSeriesUtils.reshape3dTo2d(input_Conflict, workspaceMgr, ArrayType.FF_WORKING_MEM)
				Dim [out] As INDArray = MyBase.preOutput(training, workspaceMgr)
				Me.input_Conflict = inputTemp
				Return [out]
			Else
				'Case when called from BaseOutputLayer
				Dim [out] As INDArray = MyBase.preOutput(training, workspaceMgr)
				Return [out]
			End If
		End Function

		Protected Friend Overrides Function getLabels2d(ByVal workspaceMgr As LayerWorkspaceMgr, ByVal arrayType As ArrayType) As INDArray
			Dim labels As INDArray = Me.labels_Conflict
			If labels.rank() = 3 Then
				labels = If(layerConf().getRnnDataFormat() = RNNFormat.NWC, labels.permute(0, 2, 1), labels)
				Return TimeSeriesUtils.reshape3dTo2d(labels, workspaceMgr, arrayType)
			End If
			Return labels
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Dim input As INDArray = Me.input_Conflict
			If input.rank() <> 3 Then
				Throw New System.NotSupportedException("Input must be rank 3. Got input with rank " & input.rank() & " " & layerId())
			End If
			Dim b As INDArray = getParamWithNoise(DefaultParamInitializer.BIAS_KEY, training, workspaceMgr)
			Dim W As INDArray = getParamWithNoise(DefaultParamInitializer.WEIGHT_KEY, training, workspaceMgr)

			applyDropOutIfNecessary(training, workspaceMgr)
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				input = input.permute(0, 2, 1)
			End If
			Dim input2d As INDArray = TimeSeriesUtils.reshape3dTo2d(input.castTo(W.dataType()), workspaceMgr, ArrayType.FF_WORKING_MEM)

			Dim act2d As INDArray = layerConf().getActivationFn().getActivation(input2d.mmul(W).addiRowVector(b), training)
			If maskArray_Conflict IsNot Nothing Then
				If Not maskArray_Conflict.ColumnVectorOrScalar OrElse maskArray_Conflict.shape().SequenceEqual(act2d.shape()) Then
					'Per output masking
					act2d.muli(maskArray_Conflict.castTo(act2d.dataType()))
				Else
					'Per time step masking
					act2d.muliColumnVector(maskArray_Conflict.castTo(act2d.dataType()))
				End If
			End If

			Dim ret As INDArray = TimeSeriesUtils.reshape2dTo3d(act2d, input.size(0), workspaceMgr, ArrayType.ACTIVATIONS)
			If layerConf().getRnnDataFormat() = RNNFormat.NWC Then
				ret = ret.permute(0, 2, 1)
			End If
			Return ret
		End Function

		Public Overrides WriteOnly Property MaskArray As INDArray
			Set(ByVal maskArray As INDArray)
				If maskArray IsNot Nothing Then
					'Two possible cases:
					'(a) per time step masking - rank 2 mask array -> reshape to rank 1 (column vector)
					'(b) per output masking - rank 3 mask array  -> reshape to rank 2 (
					If maskArray.rank() = 2 Then
						Me.maskArray_Conflict = TimeSeriesUtils.reshapeTimeSeriesMaskToVector(maskArray, LayerWorkspaceMgr.noWorkspacesImmutable(), ArrayType.INPUT)
					ElseIf maskArray.rank() = 3 Then
						Me.maskArray_Conflict = TimeSeriesUtils.reshape3dTo2d(maskArray, LayerWorkspaceMgr.noWorkspacesImmutable(), ArrayType.INPUT)
					Else
						Throw New System.NotSupportedException("Invalid mask array: must be rank 2 or 3 (got: rank " & maskArray.rank() & ", shape = " & Arrays.toString(maskArray.shape()) & ") " & layerId())
					End If
				Else
					Me.maskArray_Conflict = Nothing
				End If
			End Set
		End Property

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)

			'If the *input* mask array is present and active, we should use it to mask the output
			If maskArray IsNot Nothing AndAlso currentMaskState = MaskState.Active Then
				Me.inputMaskArray = TimeSeriesUtils.reshapeTimeSeriesMaskToVector(maskArray, LayerWorkspaceMgr.noWorkspacesImmutable(), ArrayType.INPUT)
				Me.inputMaskArrayState = currentMaskState
			Else
				Me.inputMaskArray = Nothing
				Me.inputMaskArrayState = Nothing
			End If

			Return Nothing 'Last layer in network
		End Function

		''' <summary>
		'''Compute the score for each example individually, after labels and input have been set.
		''' </summary>
		''' <param name="fullNetRegTerm"> Regularization score term for the entire network (or, 0.0 to not include regularization) </param>
		''' <returns> A column INDArray of shape [numExamples,1], where entry i is the score of the ith example </returns>
		Public Overrides Function computeScoreForExamples(ByVal fullNetRegTerm As Double, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			'For RNN: need to sum up the score over each time step before returning.

			If input_Conflict Is Nothing OrElse labels_Conflict Is Nothing Then
				Throw New System.InvalidOperationException("Cannot calculate score without input and labels " & layerId())
			End If
			Dim preOut As INDArray = preOutput2d(False, workspaceMgr)

			Dim lossFunction As ILossFunction = layerConf().getLossFn()
			Dim scoreArray As INDArray = lossFunction.computeScoreArray(getLabels2d(workspaceMgr, ArrayType.FF_WORKING_MEM), preOut, layerConf().getActivationFn(), maskArray_Conflict)
			'scoreArray: shape [minibatch*timeSeriesLength, 1]
			'Reshape it to [minibatch, timeSeriesLength] then sum over time step

			Dim scoreArrayTs As INDArray = TimeSeriesUtils.reshapeVectorToTimeSeriesMask(scoreArray, CInt(input_Conflict.size(0)))
			Dim summedScores As INDArray = scoreArrayTs.sum(True, 1)

			If fullNetRegTerm <> 0.0 Then
				summedScores.addi(fullNetRegTerm)
			End If

			Return summedScores
		End Function
	End Class

End Namespace