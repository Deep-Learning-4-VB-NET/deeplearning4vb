Imports System
Imports val = lombok.val
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports SimpleRnnParamInitializer = org.deeplearning4j.nn.params.SimpleRnnParamInitializer
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports BroadcastCopyOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastCopyOp
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports LayerNorm = org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNorm
Imports LayerNormBp = org.nd4j.linalg.api.ops.impl.transforms.custom.LayerNormBp
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports org.nd4j.common.primitives
Imports org.nd4j.linalg.indexing.NDArrayIndex

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
	Public Class SimpleRnn
		Inherits BaseRecurrentLayer(Of org.deeplearning4j.nn.conf.layers.recurrent.SimpleRnn)

		Public Const STATE_KEY_PREV_ACTIVATION As String = "prevAct"


		Public Sub New(ByVal conf As NeuralNetConfiguration, ByVal dataType As DataType)
			MyBase.New(conf, dataType)
		End Sub

		Public Overrides Function rnnTimeStep(ByVal input As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Dim last As INDArray = stateMap(STATE_KEY_PREV_ACTIVATION)
			Dim [out] As INDArray = activateHelper(last, False, False, workspaceMgr).getFirst()
			Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
				stateMap(STATE_KEY_PREV_ACTIVATION) = [out].get(all(), all(), point([out].size(2)-1)).dup()
			End Using
			Return [out]
		End Function

		Public Overrides Function rnnActivateUsingStoredState(ByVal input As INDArray, ByVal training As Boolean, ByVal storeLastForTBPTT As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			setInput(input, workspaceMgr)
			Dim last As INDArray = tBpttStateMap(STATE_KEY_PREV_ACTIVATION)
			Dim [out] As INDArray = activateHelper(last, training, False, workspaceMgr).getFirst()
			If storeLastForTBPTT Then
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.scopeOutOfWorkspaces()
					tBpttStateMap(STATE_KEY_PREV_ACTIVATION) = [out].get(all(), all(), point([out].size(2)-1)).dup()
				End Using
			End If
			Return [out]
		End Function

		Public Overrides Function backpropGradient(ByVal epsilon As INDArray, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			Return tbpttBackpropGradient(epsilon, -1, workspaceMgr)
		End Function

		Public Overrides Function tbpttBackpropGradient(ByVal epsilon As INDArray, ByVal tbpttBackLength As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)
			assertInputSet(True)
			If epsilon.ordering() <> "f"c OrElse Not Shape.hasDefaultStridesForShape(epsilon) Then
				epsilon = epsilon.dup("f"c)
			End If

			Dim nOut As val = layerConf().getNOut()

			Dim input As INDArray = Me.input_Conflict.castTo(dataType) 'No-op if correct type
			input = permuteIfNWC(input)

			'First: Do forward pass to get gate activations and Zs
			Dim p As Quad(Of INDArray, INDArray, INDArray, INDArray) = activateHelper(Nothing, True, True, workspaceMgr)

			Dim w As INDArray = getParamWithNoise(SimpleRnnParamInitializer.WEIGHT_KEY, True, workspaceMgr)
			Dim rw As INDArray = getParamWithNoise(SimpleRnnParamInitializer.RECURRENT_WEIGHT_KEY, True, workspaceMgr)
			Dim b As INDArray = getParamWithNoise(SimpleRnnParamInitializer.BIAS_KEY, True, workspaceMgr)
			Dim g As INDArray = (If(hasLayerNorm(), getParamWithNoise(SimpleRnnParamInitializer.GAIN_KEY, True, workspaceMgr), Nothing))
			Dim gx As INDArray = (If(g IsNot Nothing, g.get(interval(0, 0, True), interval(0, nOut)), Nothing))
			Dim gr As INDArray = (If(g IsNot Nothing, g.get(interval(0, 0, True), interval(nOut, nOut * 2)), Nothing))

			Dim wg As INDArray = gradientViews(SimpleRnnParamInitializer.WEIGHT_KEY)
			Dim rwg As INDArray = gradientViews(SimpleRnnParamInitializer.RECURRENT_WEIGHT_KEY)
			Dim bg As INDArray = gradientViews(SimpleRnnParamInitializer.BIAS_KEY)
			Dim gg As INDArray = (If(hasLayerNorm(), gradientViews(SimpleRnnParamInitializer.GAIN_KEY), Nothing))
			Dim gxg As INDArray = (If(gg IsNot Nothing, gg.get(interval(0, 0, True), interval(0, nOut)), Nothing))
			Dim grg As INDArray = (If(gg IsNot Nothing, gg.get(interval(0, 0, True), interval(nOut, nOut * 2)), Nothing))

			gradientsFlattened.assign(0)

			Dim a As IActivation = layerConf().getActivationFn()

			Dim tsLength As val = input.size(2)

			Dim epsOut As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, input.dataType(), input.shape(), "f"c)

			Dim dldzNext As INDArray = Nothing
			Dim [end] As Long
			If tbpttBackLength > 0 Then
				[end] = Math.Max(0, tsLength-tbpttBackLength)
			Else
				[end] = 0
			End If
			epsilon = permuteIfNWC(epsilon)
			For i As Long = tsLength - 1 To [end] Step -1
				Dim dldaCurrent As INDArray = epsilon.get(all(), all(), point(i)).dup()
				Dim aCurrent As INDArray = p.getFirst().get(all(), all(), point(i))
				Dim zCurrent As INDArray = p.getSecond().get(all(), all(), point(i))
				Dim nCurrent As INDArray = (If(hasLayerNorm(), p.getThird().get(all(), all(), point(i)), Nothing))
				Dim rCurrent As INDArray = (If(hasLayerNorm(), p.getFourth().get(all(), all(), point(i)), Nothing))
				Dim inCurrent As INDArray = input.get(all(), all(), point(i))
				Dim epsOutCurrent As INDArray = epsOut.get(all(), all(), point(i))

				If dldzNext IsNot Nothing Then
					'Backprop the component of dL/da (for current time step) from the recurrent connections
					Nd4j.gemm(dldzNext, rw, dldaCurrent, False, True, 1.0, 1.0)

					'Recurrent weight gradients:
					Nd4j.gemm(aCurrent, dldzNext, rwg, True, False, 1.0, 1.0)
				End If
				Dim dldzCurrent As INDArray = a.backprop(zCurrent.dup(), dldaCurrent).First

				'Handle masking
				Dim maskCol As INDArray = Nothing
				If maskArray_Conflict IsNot Nothing Then
					'Mask array: shape [minibatch, tsLength]
					'If mask array is present (for example, with bidirectional RNN) -> need to zero out these errors to
					' avoid using errors from a masked time step to calculate the parameter gradients
					maskCol = maskArray_Conflict.getColumn(i, True).castTo(dataType)
					dldzCurrent.muliColumnVector(maskCol)
				End If

				Dim dldnCurrent As INDArray
				If hasLayerNorm() Then
					dldnCurrent = workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, dldzCurrent.dataType(), dldzCurrent.shape())
					Dim ggCur As INDArray = workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, gg.dataType(), gxg.shape())
					Dim bgCur As INDArray = workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, bg.dataType(), bg.shape())
					Nd4j.Executioner.exec(New LayerNormBp(nCurrent, gx, b, dldzCurrent, dldnCurrent, ggCur, bgCur, True, 1))
					gxg.addi(ggCur)
					bg.addi(bgCur)
				Else
					dldnCurrent = dldzCurrent
					'Bias gradients
					bg.addi(dldzCurrent.sum(0))
				End If

				'weight gradients:
				Nd4j.gemm(inCurrent, dldnCurrent, wg, True, False, 1.0, 1.0)

				'Epsilon out to layer below (i.e., dL/dIn)
				Nd4j.gemm(dldnCurrent, w, epsOutCurrent, False, True, 1.0, 0.0)

				' propagate epsilon to previous iteration
				If hasLayerNorm() AndAlso i > [end] Then
					dldzNext = workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, dldzCurrent.dataType(), dldzCurrent.shape())
					Dim ggCur As INDArray = workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, gg.dataType(), grg.shape())
					Nd4j.Executioner.exec(New LayerNormBp(rCurrent, gr, dldzCurrent, dldzNext, ggCur, True, 1))
					grg.addi(ggCur)
				Else
					dldzNext = dldzCurrent
				End If

				If maskArray_Conflict IsNot Nothing Then
					'If mask array is present: Also need to zero out errors to avoid sending anything but 0s to layer below for masked steps
					epsOutCurrent.muliColumnVector(maskCol)
				End If
			Next i

			weightNoiseParams.Clear()

			Dim grad As Gradient = New DefaultGradient(gradientsFlattened)
			grad.gradientForVariable()(SimpleRnnParamInitializer.WEIGHT_KEY) = wg
			grad.gradientForVariable()(SimpleRnnParamInitializer.RECURRENT_WEIGHT_KEY) = rwg
			grad.gradientForVariable()(SimpleRnnParamInitializer.BIAS_KEY) = bg
			If hasLayerNorm() Then
				grad.gradientForVariable()(SimpleRnnParamInitializer.GAIN_KEY) = gg
			End If

			epsOut = backpropDropOutIfPresent(epsOut)
			epsOut = permuteIfNWC(epsOut)
			Return New Pair(Of Gradient, INDArray)(grad, epsOut)
		End Function

		Public Overrides ReadOnly Property PretrainLayer As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Function activate(ByVal training As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray
			Return activateHelper(Nothing, training, False, workspaceMgr).getFirst()
		End Function

		Private Function activateHelper(ByVal prevStepOut As INDArray, ByVal training As Boolean, ByVal forBackprop As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Quad(Of INDArray, INDArray, INDArray, INDArray)
			assertInputSet(False)
			Preconditions.checkState(input_Conflict.rank() = 3, "3D input expected to RNN layer expected, got " & input_Conflict.rank())
			Preconditions.checkState(prevStepOut Is Nothing OrElse prevStepOut.size(0) = input_Conflict.size(0), "Invalid RNN previous state (last time step activations/initialization): rnnTimeStep with different minibatch size, or forgot to call rnnClearPreviousState between batches?" & " Previous step output = [batch, nIn] = %ndShape, current input = [batch, nIn, seqLength] = %ndShape", prevStepOut, input_Conflict)

			applyDropOutIfNecessary(training, workspaceMgr)

			Dim input As INDArray = Me.input_Conflict.castTo(dataType) 'No-op if correct type
			input = permuteIfNWC(input)
			Dim m As val = input.size(0)
			Dim tsLength As val = input.size(2)
			Dim nOut As val = layerConf().getNOut()

			Dim w As INDArray = getParamWithNoise(SimpleRnnParamInitializer.WEIGHT_KEY, training, workspaceMgr)
			Dim rw As INDArray = getParamWithNoise(SimpleRnnParamInitializer.RECURRENT_WEIGHT_KEY, training, workspaceMgr)
			Dim b As INDArray = getParamWithNoise(SimpleRnnParamInitializer.BIAS_KEY, training, workspaceMgr)
			Dim g As INDArray = (If(hasLayerNorm(), getParamWithNoise(SimpleRnnParamInitializer.GAIN_KEY, training, workspaceMgr), Nothing))
			Dim gx As INDArray = (If(g IsNot Nothing, g.get(interval(0, 0, True), interval(0, nOut)), Nothing))
			Dim gr As INDArray = (If(g IsNot Nothing, g.get(interval(0, 0, True), interval(nOut, nOut * 2)), Nothing))

			Dim [out] As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, w.dataType(), New Long(){m, nOut, tsLength}, "f"c)
			Dim outZ As INDArray = (If(forBackprop, workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, w.dataType(), [out].shape()), Nothing))
			Dim outPreNorm As INDArray = (If(forBackprop AndAlso hasLayerNorm(), workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, w.dataType(), [out].shape(), "f"c), Nothing))
			Dim recPreNorm As INDArray = (If(forBackprop AndAlso hasLayerNorm(), workspaceMgr.createUninitialized(ArrayType.BP_WORKING_MEM, w.dataType(), [out].shape(), "f"c), Nothing))

			If input.ordering() <> "f"c OrElse Shape.strideDescendingCAscendingF(input) Then
				input = workspaceMgr.dup(ArrayType.ACTIVATIONS, input, "f"c)
			End If

			'TODO implement 'mmul across time' optimization

			If Not hasLayerNorm() Then
				'Minor performance optimization: do the "add bias" first:
				Nd4j.Executioner.exec(New BroadcastCopyOp([out], b, [out], 1))
			End If

			Dim a As IActivation = layerConf().getActivationFn()

			For i As Integer = 0 To tsLength - 1
				'out = activationFn(in*w + last*rw + bias)
				Dim currOut As INDArray = [out].get(all(), all(), point(i)) 'F order
				Dim currIn As INDArray = input.get(all(), all(), point(i))
				If hasLayerNorm() Then
					Dim currOutPreNorm As INDArray = (If(forBackprop, outPreNorm, [out])).get(all(), all(), point(i))
					Nd4j.gemm(currIn, w, currOutPreNorm, False, False, 1.0, 0.0)
					Nd4j.Executioner.exec(New LayerNorm(currOutPreNorm, gx, b, currOut, True, 1))
				Else
					Nd4j.gemm(currIn, w, currOut, False, False, 1.0, 1.0) 'beta = 1.0 to keep previous contents (bias)
				End If

				If i > 0 OrElse prevStepOut IsNot Nothing Then
					If hasLayerNorm() Then
						Dim currRecPreNorm As INDArray = If(forBackprop, recPreNorm.get(all(), all(), point(i)), workspaceMgr.createUninitialized(ArrayType.FF_WORKING_MEM, currOut.dataType(), currOut.shape(), "f"c))
						Nd4j.gemm(prevStepOut, rw, currRecPreNorm, False, False, 1.0, 0.0)
						Dim recNorm As INDArray = workspaceMgr.createUninitialized(ArrayType.FF_WORKING_MEM, currOut.dataType(), currOut.shape(), "f"c)
						Nd4j.Executioner.exec(New LayerNorm(currRecPreNorm, gr, recNorm, True, 1))
						currOut.addi(recNorm)
					Else
						Nd4j.gemm(prevStepOut, rw, currOut, False, False, 1.0, 1.0) 'beta = 1.0 to keep previous contents
					End If
				End If

				If forBackprop Then
					outZ.get(all(), all(), point(i)).assign(currOut)
				End If

				a.getActivation(currOut, training)

				If maskArray_Conflict IsNot Nothing Then
					'If mask array is present: Also need to zero out errors to avoid sending anything but 0s to layer below for masked steps
					Dim maskCol As INDArray = maskArray_Conflict.getColumn(i, True).castTo(dataType)
					currOut.muliColumnVector(maskCol)
				End If

				prevStepOut = currOut
			Next i

			'Apply mask, if present:
			If maskArray_Conflict IsNot Nothing Then
				'Mask should be shape [minibatch, tsLength]
				Dim mask As INDArray = maskArray_Conflict.castTo(dataType)
				Nd4j.Executioner.exec(New BroadcastMulOp([out], mask, [out], 0, 2))
				If forBackprop Then
					Nd4j.Executioner.exec(New BroadcastMulOp(outZ, mask, outZ, 0, 2))
				End If
			End If
			If Not forBackprop Then
				[out] = permuteIfNWC([out])
				outZ = permuteIfNWC(outZ)
				outPreNorm = permuteIfNWC(outPreNorm)
				recPreNorm = permuteIfNWC(recPreNorm)
			End If
			Return New Quad(Of INDArray, INDArray, INDArray, INDArray)([out], outZ, outPreNorm, recPreNorm)
		End Function

		Public Overrides Function hasLayerNorm() As Boolean
			Return layerConf().hasLayerNorm()
		End Function
	End Class

End Namespace