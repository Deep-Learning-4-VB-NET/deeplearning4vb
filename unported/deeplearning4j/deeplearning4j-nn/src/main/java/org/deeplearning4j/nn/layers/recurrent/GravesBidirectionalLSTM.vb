Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports MaskState = org.deeplearning4j.nn.api.MaskState
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports GravesBidirectionalLSTMParamInitializer = org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer
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
'ORIGINAL LINE: @Slf4j public class GravesBidirectionalLSTM extends BaseRecurrentLayer<org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM>
	<Serializable>
	Public Class GravesBidirectionalLSTM
		Inherits BaseRecurrentLayer(Of org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM)

		Protected Friend cachedPassForward As FwdPassReturn
		Protected Friend cachedPassBackward As FwdPassReturn

		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function gradient() As Gradient
			Throw New System.NotSupportedException("Not supported " & layerId())
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

			If truncatedBPTT Then
				Throw New System.NotSupportedException("Time step for bidirectional RNN not supported: it has to run on a batch of data all at once " & layerId())
			End If

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final FwdPassReturn fwdPass = activateHelperDirectional(true, null, null, true, true, workspaceMgr);
			Dim fwdPass As FwdPassReturn = activateHelperDirectional(True, Nothing, Nothing, True, True, workspaceMgr)
			fwdPass.fwdPassOutput = permuteIfNWC(fwdPass.fwdPassOutput)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient, org.nd4j.linalg.api.ndarray.INDArray> forwardsGradient = LSTMHelpers.backpropGradientHelper(this, this.conf, this.layerConf().getGateActivationFn(), permuteIfNWC(this.input), getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS), getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS), permuteIfNWC(epsilon), truncatedBPTT, tbpttBackwardLength, fwdPass, true, org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS, org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS, org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS, gradientViews, maskArray, true, null, workspaceMgr, layerConf().isHelperAllowFallback());
			Dim forwardsGradient As Pair(Of Gradient, INDArray) = LSTMHelpers.backpropGradientHelper(Me, Me.conf_Conflict, Me.layerConf().getGateActivationFn(), permuteIfNWC(Me.input_Conflict), getParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS), getParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS), permuteIfNWC(epsilon), truncatedBPTT, tbpttBackwardLength, fwdPass, True, GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS, GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS, GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS, gradientViews, maskArray_Conflict, True, Nothing, workspaceMgr, layerConf().isHelperAllowFallback())



'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final FwdPassReturn backPass = activateHelperDirectional(true, null, null, true, false, workspaceMgr);
			Dim backPass As FwdPassReturn = activateHelperDirectional(True, Nothing, Nothing, True, False, workspaceMgr)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient, org.nd4j.linalg.api.ndarray.INDArray> backwardsGradient = LSTMHelpers.backpropGradientHelper(this, this.conf, this.layerConf().getGateActivationFn(), permuteIfNWC(this.input), getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS), getParam(org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS), permuteIfNWC(epsilon), truncatedBPTT, tbpttBackwardLength, backPass, false, org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS, org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS, org.deeplearning4j.nn.params.GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS, gradientViews, maskArray, true, null, workspaceMgr, layerConf().isHelperAllowFallback());
			Dim backwardsGradient As Pair(Of Gradient, INDArray) = LSTMHelpers.backpropGradientHelper(Me, Me.conf_Conflict, Me.layerConf().getGateActivationFn(), permuteIfNWC(Me.input_Conflict), getParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS), getParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS), permuteIfNWC(epsilon), truncatedBPTT, tbpttBackwardLength, backPass, False, GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS, GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS, GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS, gradientViews, maskArray_Conflict, True, Nothing, workspaceMgr, layerConf().isHelperAllowFallback())

			forwardsGradient.Second = permuteIfNWC(forwardsGradient.Second)
			backwardsGradient.Second = permuteIfNWC(backwardsGradient.Second)
			'merge the gradient, which is key value pair of String,INDArray
			'the keys for forwards and backwards should be different

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.gradient.Gradient combinedGradient = new org.deeplearning4j.nn.gradient.DefaultGradient();
			Dim combinedGradient As Gradient = New DefaultGradient()


			For Each entry As KeyValuePair(Of String, INDArray) In forwardsGradient.First.gradientForVariable().SetOfKeyValuePairs()
				combinedGradient.setGradientFor(entry.Key, entry.Value)
			Next entry

			For Each entry As KeyValuePair(Of String, INDArray) In backwardsGradient.First.gradientForVariable().SetOfKeyValuePairs()
				combinedGradient.setGradientFor(entry.Key, entry.Value)
			Next entry

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.gradient.Gradient correctOrderedGradient = new org.deeplearning4j.nn.gradient.DefaultGradient();
			Dim correctOrderedGradient As Gradient = New DefaultGradient()

			For Each key As String In params_Conflict.Keys
				correctOrderedGradient.setGradientFor(key, combinedGradient.getGradientFor(key))
			Next key

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray forwardEpsilon = forwardsGradient.getSecond();
			Dim forwardEpsilon As INDArray = forwardsGradient.Second
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray backwardsEpsilon = backwardsGradient.getSecond();
			Dim backwardsEpsilon As INDArray = backwardsGradient.Second
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray combinedEpsilon = forwardEpsilon.addi(backwardsEpsilon);
			Dim combinedEpsilon As INDArray = forwardEpsilon.addi(backwardsEpsilon)

			'sum the errors that were back-propagated
			Return New Pair(Of Gradient, INDArray)(correctOrderedGradient, combinedEpsilon)

		End Function

		Public Overrides Function activate(ByVal input As INDArray, ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Return activateOutput(training, False, workspaceMgr)
		End Function

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return activateOutput(training, False, workspaceMgr)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private org.nd4j.linalg.api.ndarray.INDArray activateOutput(final boolean training, boolean forBackprop, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr)
		Private Function activateOutput(ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			assertInputSet(False)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final FwdPassReturn forwardsEval;
			Dim forwardsEval As FwdPassReturn
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final FwdPassReturn backwardsEval;
			Dim backwardsEval As FwdPassReturn

			If cacheMode_Conflict <> CacheMode.NONE AndAlso cachedPassForward IsNot Nothing AndAlso cachedPassBackward IsNot Nothing Then
				' restore from cache. but this coll will probably never happen
				forwardsEval = cachedPassForward
				backwardsEval = cachedPassBackward

				cachedPassBackward = Nothing
				cachedPassForward = Nothing
			Else

				forwardsEval = LSTMHelpers.activateHelper(Me, Me.conf_Conflict, Me.layerConf().getGateActivationFn(), permuteIfNWC(Me.input_Conflict), getParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS), getParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS), getParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS), training, Nothing, Nothing, forBackprop OrElse (cacheMode_Conflict <> CacheMode.NONE AndAlso training), True, GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS, maskArray_Conflict, True, Nothing,If(forBackprop, cacheMode_Conflict, CacheMode.NONE), workspaceMgr, layerConf().isHelperAllowFallback())

				backwardsEval = LSTMHelpers.activateHelper(Me, Me.conf_Conflict, Me.layerConf().getGateActivationFn(), permuteIfNWC(Me.input_Conflict), getParam(GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS), getParam(GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS), getParam(GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS), training, Nothing, Nothing, forBackprop OrElse (cacheMode_Conflict <> CacheMode.NONE AndAlso training), False, GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS, maskArray_Conflict, True, Nothing,If(forBackprop, cacheMode_Conflict, CacheMode.NONE), workspaceMgr, layerConf().isHelperAllowFallback())

				forwardsEval.fwdPassOutput = permuteIfNWC(forwardsEval.fwdPassOutput)
				backwardsEval.fwdPassOutput = permuteIfNWC(backwardsEval.fwdPassOutput)
				cachedPassForward = forwardsEval
				cachedPassBackward = backwardsEval
			End If

			'sum outputs
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray fwdOutput = forwardsEval.fwdPassOutput;
			Dim fwdOutput As INDArray = forwardsEval.fwdPassOutput
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray backOutput = backwardsEval.fwdPassOutput;
			Dim backOutput As INDArray = backwardsEval.fwdPassOutput

			' if we're on ff pass & cache enabled - we should not modify fwdOutput, and for backprop pass - we don't care
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray totalOutput = training && cacheMode != org.deeplearning4j.nn.conf.CacheMode.NONE && !forBackprop ? fwdOutput.add(backOutput) : fwdOutput.addi(backOutput);
			Dim totalOutput As INDArray = If(training AndAlso cacheMode_Conflict <> CacheMode.NONE AndAlso Not forBackprop, fwdOutput.add(backOutput), fwdOutput.addi(backOutput))

			Return totalOutput
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private FwdPassReturn activateHelperDirectional(final boolean training, final org.nd4j.linalg.api.ndarray.INDArray prevOutputActivations, final org.nd4j.linalg.api.ndarray.INDArray prevMemCellState, boolean forBackprop, boolean forwards, org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr)
		Private Function activateHelperDirectional(ByVal training As Boolean, ByVal prevOutputActivations As INDArray, ByVal prevMemCellState As INDArray, ByVal forBackprop As Boolean, ByVal forwards As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As FwdPassReturn

			If cacheMode_Conflict = Nothing Then
				cacheMode_Conflict = CacheMode.NONE
			End If

			If cacheMode_Conflict <> CacheMode.NONE AndAlso forwards AndAlso forBackprop AndAlso cachedPassForward IsNot Nothing Then
				Dim ret As FwdPassReturn = cachedPassForward
				cachedPassForward = Nothing
				Return ret
			ElseIf cacheMode_Conflict <> CacheMode.NONE AndAlso Not forwards AndAlso forBackprop Then
				Dim ret As FwdPassReturn = cachedPassBackward
				cachedPassBackward = Nothing
				Return ret
			Else

				Dim recurrentKey As String = GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_FORWARDS
				Dim inputKey As String = GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_FORWARDS
				Dim biasKey As String = GravesBidirectionalLSTMParamInitializer.BIAS_KEY_FORWARDS

				If Not forwards Then
					recurrentKey = GravesBidirectionalLSTMParamInitializer.RECURRENT_WEIGHT_KEY_BACKWARDS
					inputKey = GravesBidirectionalLSTMParamInitializer.INPUT_WEIGHT_KEY_BACKWARDS
					biasKey = GravesBidirectionalLSTMParamInitializer.BIAS_KEY_BACKWARDS
				End If

				Dim ret As FwdPassReturn = LSTMHelpers.activateHelper(Me, Me.conf_Conflict, Me.layerConf().getGateActivationFn(), permuteIfNWC(Me.input_Conflict), getParam(recurrentKey), getParam(inputKey), getParam(biasKey), training, prevOutputActivations, prevMemCellState, forBackprop, forwards, inputKey, maskArray_Conflict, True, Nothing,If(forBackprop, cacheMode_Conflict, CacheMode.NONE), workspaceMgr, layerConf().isHelperAllowFallback())
				ret.fwdPassOutput = permuteIfNWC(ret.fwdPassOutput)
				Return ret
			End If
		End Function

		Public Overrides Function type() As Type
			Return Type.RECURRENT
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function rnnTimeStep(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Throw New System.NotSupportedException("you can not time step a bidirectional RNN, it has to run on a batch of data all at once " & layerId())
		End Function



		Public Overrides Function rnnActivateUsingStoredState(ByVal input As INDArray, ByVal training As Boolean, ByVal storeLastForTBPTT As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Throw New System.NotSupportedException("Cannot set stored state: bidirectional RNNs don't have stored state " & layerId())
		End Function


		Public Overrides Function feedForwardMaskArray(ByVal maskArray As INDArray, ByVal currentMaskState As MaskState, ByVal minibatchSize As Integer) As Pair(Of INDArray, MaskState)
			'Bidirectional RNNs operate differently to standard RNNs from a masking perspective
			'Specifically, the masks are applied regardless of the mask state
			'For example, input -> RNN -> Bidirectional-RNN: we should still mask the activations and errors in the bi-RNN
			' even though the normal RNN has marked the current mask state as 'passthrough'
			'Consequently, the mask is marked as active again

			Me.maskArray_Conflict = maskArray
			Me.maskState = currentMaskState

			Return New Pair(Of INDArray, MaskState)(maskArray, MaskState.Active)
		End Function
	End Class

End Namespace