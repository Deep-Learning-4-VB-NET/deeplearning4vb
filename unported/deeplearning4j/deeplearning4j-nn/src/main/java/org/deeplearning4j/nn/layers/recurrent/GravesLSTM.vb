Imports System
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports GravesLSTMParamInitializer = org.deeplearning4j.nn.params.GravesLSTMParamInitializer
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Deprecated @Slf4j public class GravesLSTM extends BaseRecurrentLayer<org.deeplearning4j.nn.conf.layers.GravesLSTM>
	<Obsolete, Serializable>
	Public Class GravesLSTM
		Inherits BaseRecurrentLayer(Of org.deeplearning4j.nn.conf.layers.GravesLSTM)

		Public Const STATE_KEY_PREV_ACTIVATION As String = "prevAct"
		Public Const STATE_KEY_PREV_MEMCELL As String = "prevMem"

		Protected Friend cachedFwdPass As FwdPassReturn

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function gradient() As Gradient
			Throw New System.NotSupportedException("gradient() method for layerwise pretraining: not supported for LSTMs (pretraining not possible)" & layerId())
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Return backpropGradientHelper(epsilon, False, -1, workspaceMgr)
		End Function

		Public Overrides Function tbpttBackpropGradient(ByVal epsilon As INDArray, ByVal tbpttBackwardLength As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Return backpropGradientHelper(epsilon, True, tbpttBackwardLength, workspaceMgr)
		End Function


'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient, org.nd4j.linalg.api.ndarray.INDArray> backpropGradientHelper(final org.nd4j.linalg.api.ndarray.INDArray epsilon, final boolean truncatedBPTT, final int tbpttBackwardLength, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr)
		Private Function backpropGradientHelper(ByVal epsilon As INDArray, ByVal truncatedBPTT As Boolean, ByVal tbpttBackwardLength As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inputWeights = getParamWithNoise(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.INPUT_WEIGHT_KEY, true, workspaceMgr);
			Dim inputWeights As INDArray = getParamWithNoise(GravesLSTMParamInitializer.INPUT_WEIGHT_KEY, True, workspaceMgr)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray recurrentWeights = getParamWithNoise(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY, true, workspaceMgr);
			Dim recurrentWeights As INDArray = getParamWithNoise(GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY, True, workspaceMgr) 'Shape: [hiddenLayerSize,4*hiddenLayerSize+3]; order: [wI,wF,wO,wG,wFF,wOO,wGG]

			'First: Do forward pass to get gate activations, zs etc.
			Dim fwdPass As FwdPassReturn
			If truncatedBPTT Then
				fwdPass = activateHelper(True, stateMap(STATE_KEY_PREV_ACTIVATION), stateMap(STATE_KEY_PREV_MEMCELL), True, workspaceMgr)
				'Store last time step of output activations and memory cell state in tBpttStateMap
				tBpttStateMap(STATE_KEY_PREV_ACTIVATION) = fwdPass.lastAct.detach()
				tBpttStateMap(STATE_KEY_PREV_MEMCELL) = fwdPass.lastMemCell.detach()
			Else
				fwdPass = activateHelper(True, Nothing, Nothing, True, workspaceMgr)
			End If
			fwdPass.fwdPassOutput = permuteIfNWC(fwdPass.fwdPassOutput)

			Dim p As Pair(Of Gradient, INDArray) = LSTMHelpers.backpropGradientHelper(Me, Me.conf_Conflict, Me.layerConf().getGateActivationFn(), permuteIfNWC(Me.input_Conflict), recurrentWeights, inputWeights, permuteIfNWC(epsilon), truncatedBPTT, tbpttBackwardLength, fwdPass, True, GravesLSTMParamInitializer.INPUT_WEIGHT_KEY, GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY, GravesLSTMParamInitializer.BIAS_KEY, gradientViews, maskArray_Conflict, True, Nothing, workspaceMgr, layerConf().isHelperAllowFallback())

			weightNoiseParams.Clear()
			p.Second = permuteIfNWC(backpropDropOutIfPresent(p.Second))
			Return p
		End Function

		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Return activateHelper(training, Nothing, Nothing, False, workspaceMgr).fwdPassOutput
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return activateHelper(training, Nothing, Nothing, False, workspaceMgr).fwdPassOutput
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private FwdPassReturn activateHelper(final boolean training, final org.nd4j.linalg.api.ndarray.INDArray prevOutputActivations, final org.nd4j.linalg.api.ndarray.INDArray prevMemCellState, boolean forBackprop, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr)
		Private Function activateHelper(ByVal training As Boolean, ByVal prevOutputActivations As INDArray, ByVal prevMemCellState As INDArray, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As FwdPassReturn
			assertInputSet(False)
			Preconditions.checkState(Me.input_Conflict.rank() = 3, "3D input expected to RNN layer expected, got " & Me.input_Conflict.rank())
			applyDropOutIfNecessary(training, workspaceMgr)

	'        if (cacheMode == null)
	'            cacheMode = CacheMode.NONE;

			'TODO LSTM cache mode is disabled for now - not passing all tests
			cacheMode_Conflict = CacheMode.NONE

			If forBackprop AndAlso cachedFwdPass IsNot Nothing Then
				Dim ret As FwdPassReturn = cachedFwdPass
				cachedFwdPass = Nothing
				Return ret
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray recurrentWeights = getParamWithNoise(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY, training, workspaceMgr);
			Dim recurrentWeights As INDArray = getParamWithNoise(GravesLSTMParamInitializer.RECURRENT_WEIGHT_KEY, training, workspaceMgr) 'Shape: [hiddenLayerSize,4*hiddenLayerSize+3]; order: [wI,wF,wO,wG,wFF,wOO,wGG]
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inputWeights = getParamWithNoise(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.INPUT_WEIGHT_KEY, training, workspaceMgr);
			Dim inputWeights As INDArray = getParamWithNoise(GravesLSTMParamInitializer.INPUT_WEIGHT_KEY, training, workspaceMgr) 'Shape: [n^(L-1),4*hiddenLayerSize]; order: [wi,wf,wo,wg]
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray biases = getParamWithNoise(org.deeplearning4j.nn.params.GravesLSTMParamInitializer.BIAS_KEY, training, workspaceMgr);
			Dim biases As INDArray = getParamWithNoise(GravesLSTMParamInitializer.BIAS_KEY, training, workspaceMgr) 'by row: IFOG            //Shape: [4,hiddenLayerSize]; order: [bi,bf,bo,bg]^T
			Dim input As INDArray = permuteIfNWC(Me.input_Conflict)
			Dim fwd As FwdPassReturn = LSTMHelpers.activateHelper(Me, Me.conf_Conflict, Me.layerConf().getGateActivationFn(), input, recurrentWeights, inputWeights, biases, training, prevOutputActivations, prevMemCellState, forBackprop OrElse (cacheMode_Conflict <> CacheMode.NONE AndAlso training), True, GravesLSTMParamInitializer.INPUT_WEIGHT_KEY, maskArray_Conflict, True, Nothing, cacheMode_Conflict, workspaceMgr, layerConf().isHelperAllowFallback())

			fwd.fwdPassOutput = permuteIfNWC(fwd.fwdPassOutput)
			If training AndAlso cacheMode_Conflict <> CacheMode.NONE Then
				cachedFwdPass = fwd
			End If
			Return fwd
		End Function

		Public Overrides Function type() As Type
			Return Type.RECURRENT
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			'LSTM (standard, not bi-directional) don't make any changes to the data OR the mask arrays
			'Any relevant masking occurs during backprop
			'They also set the current mask array as inactive: this is for situations like the following:
			' in -> dense -> lstm -> dense -> lstm
			' The first dense should be masked using the input array, but the second shouldn't. If necessary, the second
			' dense will be masked via the output layer mask

			Return New Pair(Of INDArray, MaskState)(maskArray, MaskState.Passthrough)
		End Function

		Public Overrides Function rnnTimeStep(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Dim fwdPass As FwdPassReturn = activateHelper(False, stateMap(STATE_KEY_PREV_ACTIVATION), stateMap(STATE_KEY_PREV_MEMCELL), False, workspaceMgr)
			Dim outAct As INDArray = fwdPass.fwdPassOutput
			'Store last time step of output activations and memory cell state for later use:
			stateMap(STATE_KEY_PREV_ACTIVATION) = fwdPass.lastAct.detach()
			stateMap(STATE_KEY_PREV_MEMCELL) = fwdPass.lastMemCell.detach()

			Return outAct
		End Function



		Public Overrides Function rnnActivateUsingStoredState(ByVal input As INDArray, ByVal training As Boolean, ByVal storeLastForTBPTT As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Dim fwdPass As FwdPassReturn = activateHelper(training, stateMap(STATE_KEY_PREV_ACTIVATION), stateMap(STATE_KEY_PREV_MEMCELL), False, workspaceMgr)
			Dim outAct As INDArray = fwdPass.fwdPassOutput
			If storeLastForTBPTT Then
				'Store last time step of output activations and memory cell state in tBpttStateMap
				tBpttStateMap(STATE_KEY_PREV_ACTIVATION) = fwdPass.lastAct.detach()
				tBpttStateMap(STATE_KEY_PREV_MEMCELL) = fwdPass.lastMemCell.detach()
			End If

			Return outAct
		End Function
	End Class

End Namespace