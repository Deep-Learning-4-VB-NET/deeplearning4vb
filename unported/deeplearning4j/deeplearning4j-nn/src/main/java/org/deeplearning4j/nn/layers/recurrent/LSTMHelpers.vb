Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DL4JInvalidInputException = org.deeplearning4j.exception.DL4JInvalidInputException
Imports CacheMode = org.deeplearning4j.nn.conf.CacheMode
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports AbstractLSTM = org.deeplearning4j.nn.conf.layers.AbstractLSTM
Imports GravesBidirectionalLSTM = org.deeplearning4j.nn.conf.layers.GravesBidirectionalLSTM
Imports LayerMemoryReport = org.deeplearning4j.nn.conf.memory.LayerMemoryReport
Imports MemoryReport = org.deeplearning4j.nn.conf.memory.MemoryReport
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports MemoryWorkspace = org.nd4j.linalg.api.memory.MemoryWorkspace
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MulOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.MulOp
Imports TimesOneMinus = org.nd4j.linalg.api.ops.impl.transforms.same.TimesOneMinus
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports ND4JArraySizeException = org.nd4j.linalg.exception.ND4JArraySizeException
Imports ND4JOpProfilerException = org.nd4j.linalg.exception.ND4JOpProfilerException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LSTMHelpers
	Public Class LSTMHelpers

		'    public static final String SIGMOID = "sigmoid";

		Private Sub New()
		End Sub

		''' <summary>
		''' Returns FwdPassReturn object with activations/INDArrays. Allows activateHelper to be used for forward pass, backward pass
		''' and rnnTimeStep whilst being reasonably efficient for all
		''' </summary>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static FwdPassReturn activateHelper(final BaseRecurrentLayer layer, final org.deeplearning4j.nn.conf.NeuralNetConfiguration conf, final org.nd4j.linalg.activations.IActivation gateActivationFn, org.nd4j.linalg.api.ndarray.INDArray input, final org.nd4j.linalg.api.ndarray.INDArray recurrentWeights, final org.nd4j.linalg.api.ndarray.INDArray originalInputWeights, final org.nd4j.linalg.api.ndarray.INDArray biases, final boolean training, final org.nd4j.linalg.api.ndarray.INDArray originalPrevOutputActivations, final org.nd4j.linalg.api.ndarray.INDArray originalPrevMemCellState, boolean forBackprop, boolean forwards, final String inputWeightKey, org.nd4j.linalg.api.ndarray.INDArray maskArray, final boolean hasPeepholeConnections, final LSTMHelper helper, final org.deeplearning4j.nn.conf.CacheMode cacheMode, final org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr, boolean isHelperAllowFallback)
		Public Shared Function activateHelper(ByVal layer As BaseRecurrentLayer, ByVal conf As NeuralNetConfiguration, ByVal gateActivationFn As IActivation, ByVal input As INDArray, ByVal recurrentWeights As INDArray, ByVal originalInputWeights As INDArray, ByVal biases As INDArray, ByVal training As Boolean, ByVal originalPrevOutputActivations As INDArray, ByVal originalPrevMemCellState As INDArray, ByVal forBackprop As Boolean, ByVal forwards As Boolean, ByVal inputWeightKey As String, ByVal maskArray As INDArray, ByVal hasPeepholeConnections As Boolean, ByVal helper As LSTMHelper, ByVal cacheMode As CacheMode, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal isHelperAllowFallback As Boolean) As FwdPassReturn

			'Mini-batch data format: for mini-batch size m, nIn inputs, and T time series length
			'Data has shape [m,nIn,T]. Layer activations/output has shape [m,nHiddenUnits,T]
			If input Is Nothing OrElse input.length() = 0 Then
				Throw New System.ArgumentException("Invalid input: not set or 0 length")
			End If

			Dim inputWeights As INDArray = originalInputWeights
			Dim prevOutputActivations As INDArray = originalPrevOutputActivations

			If maskArray IsNot Nothing Then
				maskArray = maskArray.castTo(recurrentWeights.dataType())
			End If

			Dim is2dInput As Boolean = input.rank() < 3 'Edge case of T=1, may have shape [m,nIn], equiv. to [m,nIn,1]

			input = input.castTo(inputWeights.dataType()) 'No-op if already correct dtype

			If (Not is2dInput AndAlso (input.size(2) > Integer.MaxValue)) OrElse recurrentWeights.size(0) > Integer.MaxValue OrElse input.size(0) > Integer.MaxValue Then
				Throw New ND4JArraySizeException()
			End If
			Dim timeSeriesLength As Integer = CInt(If(is2dInput, 1, input.size(2)))
			Dim hiddenLayerSize As Integer = CInt(recurrentWeights.size(0))
			Dim miniBatchSize As Integer = CInt(input.size(0))

			Dim prevMemCellState As INDArray
			If originalPrevMemCellState Is Nothing Then
				prevMemCellState = Nd4j.create(inputWeights.dataType(), New Long() {miniBatchSize, hiddenLayerSize}, "f"c)
			Else
				prevMemCellState = originalPrevMemCellState.dup("f"c)
			End If


			Dim recurrentWeightsIFOG As INDArray = recurrentWeights.get(all(), interval(0, 4 * hiddenLayerSize)).dup("f"c)

			Dim wFFTranspose As INDArray = Nothing
			Dim wOOTranspose As INDArray = Nothing
			Dim wGGTranspose As INDArray = Nothing

			If hasPeepholeConnections Then
				wFFTranspose = recurrentWeights.get(all(), interval(4 * hiddenLayerSize, 4 * hiddenLayerSize + 1)).reshape(1, recurrentWeights.size(0)) 'current
				wOOTranspose = recurrentWeights.get(all(), interval(4 * hiddenLayerSize + 1, 4 * hiddenLayerSize + 2)).reshape(1, recurrentWeights.size(0)) 'current
				wGGTranspose = recurrentWeights.get(all(), interval(4 * hiddenLayerSize + 2, 4 * hiddenLayerSize + 3)).reshape(1, recurrentWeights.size(0)) 'previous

				If timeSeriesLength > 1 OrElse forBackprop Then
					wFFTranspose = Shape.toMmulCompatible(wFFTranspose)
					wOOTranspose = Shape.toMmulCompatible(wOOTranspose)
					wGGTranspose = Shape.toMmulCompatible(wGGTranspose)
				End If
			End If

			'Allocate arrays for activations:
			Dim sigmoidGates As Boolean = TypeOf gateActivationFn Is ActivationSigmoid
			Dim afn As IActivation = layer.layerConf().getActivationFn()
			Dim outputActivations As INDArray = Nothing

			Dim toReturn As New FwdPassReturn()
			If forBackprop Then
				toReturn.fwdPassOutputAsArrays = New INDArray(timeSeriesLength - 1){}
				toReturn.memCellState = New INDArray(timeSeriesLength - 1){}
				toReturn.memCellActivations = New INDArray(timeSeriesLength - 1){}
				toReturn.iz = New INDArray(timeSeriesLength - 1){}
				toReturn.ia = New INDArray(timeSeriesLength - 1){}
				toReturn.fa = New INDArray(timeSeriesLength - 1){}
				toReturn.oa = New INDArray(timeSeriesLength - 1){}
				toReturn.ga = New INDArray(timeSeriesLength - 1){}
				If Not sigmoidGates Then
					toReturn.fz = New INDArray(timeSeriesLength - 1){}
					toReturn.oz = New INDArray(timeSeriesLength - 1){}
					toReturn.gz = New INDArray(timeSeriesLength - 1){}
				End If

				If training AndAlso cacheMode <> CacheMode.NONE AndAlso workspaceMgr.hasConfiguration(ArrayType.FF_CACHE) AndAlso workspaceMgr.isWorkspaceOpen(ArrayType.FF_CACHE) Then
					Using wsB As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeBorrowed(org.deeplearning4j.nn.workspace.ArrayType.FF_CACHE)
						outputActivations = Nd4j.create(inputWeights.dataType(), New Long() {miniBatchSize, hiddenLayerSize, timeSeriesLength}, "f"c) 'F order to keep time steps together
						toReturn.fwdPassOutput = outputActivations
					End Using
				Else
					outputActivations = workspaceMgr.create(ArrayType.ACTIVATIONS, input.dataType(), New Long() {miniBatchSize, hiddenLayerSize, timeSeriesLength}, "f"c) 'F order to keep time steps together
					toReturn.fwdPassOutput = outputActivations
				End If
			Else
				outputActivations = workspaceMgr.create(ArrayType.ACTIVATIONS, input.dataType(), New Long() {miniBatchSize, hiddenLayerSize, timeSeriesLength}, "f"c) 'F order to keep time steps together
				toReturn.fwdPassOutput = outputActivations
			End If

			'Level1 l1BLAS = Nd4j.getBlasWrapper().level1();

			'Input validation: check input data matches nIn
			If input.size(1) <> inputWeights.size(0) Then
				Throw New DL4JInvalidInputException("Received input with size(1) = " & input.size(1) & " (input array shape = " & Arrays.toString(input.shape()) & "); input.size(1) must match layer nIn size (nIn = " & inputWeights.size(0) & ")")
			End If
			'Input validation: check that if past state is provided, that it has same
			'These can be different if user forgets to call rnnClearPreviousState() between calls of rnnTimeStep
			Preconditions.checkState(prevOutputActivations Is Nothing OrElse prevOutputActivations.size(0) = input.size(0), "Invalid RNN previous state (last time step activations/initialization): rnnTimeStep with different minibatch size, or forgot to call rnnClearPreviousState between batches?" & " Previous step output = [batch, nIn] = %ndShape, current input = [batch, nIn, seqLength] = %ndShape", prevOutputActivations, input)

			'initialize prevOutputActivations to zeroes
			If prevOutputActivations Is Nothing Then
				prevOutputActivations = Nd4j.zeros(input.dataType(), New Long() {miniBatchSize, hiddenLayerSize})
			End If

			If helper IsNot Nothing AndAlso (layer.helperCountFail = 0 OrElse Not isHelperAllowFallback) Then
				Dim ret As FwdPassReturn = Nothing
				Try
					ret = helper.activate(layer, conf, gateActivationFn, input, recurrentWeights, inputWeights, biases, training, prevOutputActivations, prevMemCellState, forBackprop, forwards, inputWeightKey, maskArray, hasPeepholeConnections, workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch e As Exception
					If e.Message.contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw e
					End If

					If isHelperAllowFallback Then
						layer.helperCountFail += 1
						log.warn("MKL/CuDNN execution failed - falling back on built-in implementation",e)
					Else
						Throw New Exception("Error during LSTM MKL/CuDNN helper forward pass - helperAllowFallback() is set to false", e)
					End If
				End Try

				If ret IsNot Nothing Then
					Return ret
				End If
			End If

			For iTimeIndex As Integer = 0 To timeSeriesLength - 1
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.RNN_FF_LOOP_WORKING_MEM)
					Dim time As Integer = iTimeIndex

					If Not forwards Then
						time = timeSeriesLength - iTimeIndex - 1
					End If


					Dim miniBatchData As INDArray = (If(is2dInput, input, input.tensorAlongDimension(time, 1, 0))) '[Expected shape: [m,nIn]. Also deals with edge case of T=1, with 'time series' data of shape [m,nIn], equiv. to [m,nIn,1]
					miniBatchData = Shape.toMmulCompatible(miniBatchData)

					' if we're using cache here - let's create ifogActivations within cache workspace, so all views from this array will be valid in cache
					cacheEnter(training, cacheMode, workspaceMgr)

					'Calculate activations for: network input + forget, output, input modulation gates. Next 3 lines are first part of those
					Dim ifogActivations As INDArray = miniBatchData.mmul(inputWeights) 'Shape: [miniBatch,4*layerSize]
					cacheExit(training, cacheMode, workspaceMgr)

					Nd4j.gemm(prevOutputActivations, recurrentWeightsIFOG, ifogActivations, False, False, 1.0, 1.0)
					ifogActivations.addiRowVector(biases)

					Dim inputActivations As INDArray = ifogActivations.get(all(), interval(0, hiddenLayerSize))
					If forBackprop Then
						If shouldCache(training, cacheMode, workspaceMgr) Then
							cacheEnter(training, cacheMode, workspaceMgr)
							toReturn.iz(time) = inputActivations.dup("f"c)
							cacheExit(training, cacheMode, workspaceMgr)
						Else
							toReturn.iz(time) = workspaceMgr.dup(ArrayType.BP_WORKING_MEM, inputActivations, "f"c)
						End If
					End If
					layer.layerConf().getActivationFn().getActivation(inputActivations, training)
					If forBackprop Then
						If shouldCache(training, cacheMode, workspaceMgr) Then
							cacheEnter(training, cacheMode, workspaceMgr)
							toReturn.ia(time) = inputActivations.dup("f"c)
							cacheExit(training, cacheMode, workspaceMgr)
						Else
							toReturn.ia(time) = workspaceMgr.leverageTo(ArrayType.BP_WORKING_MEM, inputActivations)
						End If
					End If

					Dim forgetGateActivations As INDArray = ifogActivations.get(all(), interval(hiddenLayerSize, 2 * hiddenLayerSize))
					If hasPeepholeConnections Then
						Dim pmcellWFF As INDArray = prevMemCellState.dup("f"c).muliRowVector(wFFTranspose)
						forgetGateActivations.addi(pmcellWFF)
					End If
					'Above line: treats matrix as a vector. Can only do this because we're sure both pwcelWFF and forgetGateACtivations are f order, offset 0 and have same strides
					If forBackprop AndAlso Not sigmoidGates Then
						If shouldCache(training, cacheMode, workspaceMgr) Then
							cacheEnter(training, cacheMode, workspaceMgr)
							toReturn.fz(time) = forgetGateActivations.dup("f"c) 'Forget gate pre-out (z)
							cacheExit(training, cacheMode, workspaceMgr)
						Else
							toReturn.fz(time) = workspaceMgr.dup(ArrayType.BP_WORKING_MEM, forgetGateActivations, "f"c) 'Forget gate pre-out (z)
						End If
					End If
					gateActivationFn.getActivation(forgetGateActivations, training)

					If forBackprop Then
						If shouldCache(training, cacheMode, workspaceMgr) Then
							cacheEnter(training, cacheMode, workspaceMgr)
							toReturn.fa(time) = forgetGateActivations.dup("f"c)
							cacheExit(training, cacheMode, workspaceMgr)
						Else
							toReturn.fa(time) = workspaceMgr.leverageTo(ArrayType.BP_WORKING_MEM, forgetGateActivations)
						End If
					End If


					Dim inputModGateActivations As INDArray = ifogActivations.get(all(), interval(3 * hiddenLayerSize, 4 * hiddenLayerSize))
					If hasPeepholeConnections Then
						Dim pmcellWGG As INDArray = prevMemCellState.dup("f"c).muliRowVector(wGGTranspose)
						inputModGateActivations.addi(pmcellWGG)
					End If
					If forBackprop AndAlso Not sigmoidGates Then
						cacheEnter(training, cacheMode, workspaceMgr)
						toReturn.gz(time) = workspaceMgr.dup(ArrayType.BP_WORKING_MEM, inputModGateActivations, "f"c) 'Input modulation gate pre-out (z)
						cacheExit(training, cacheMode, workspaceMgr)
					End If
					gateActivationFn.getActivation(inputModGateActivations, training)
					If forBackprop Then
						If shouldCache(training, cacheMode, workspaceMgr) Then
							cacheEnter(training, cacheMode, workspaceMgr)
							toReturn.ga(time) = inputModGateActivations.dup("f"c)
							cacheExit(training, cacheMode, workspaceMgr)
						Else
							toReturn.ga(time) = workspaceMgr.leverageTo(ArrayType.BP_WORKING_MEM, inputModGateActivations)
						End If
					End If

					'Memory cell state
					Dim currentMemoryCellState As INDArray
					Dim inputModMulInput As INDArray
					If forBackprop Then
						cacheEnter(training, cacheMode, workspaceMgr)
						currentMemoryCellState = workspaceMgr.dup(ArrayType.BP_WORKING_MEM, prevMemCellState, "f"c).muli(forgetGateActivations)
						cacheExit(training, cacheMode, workspaceMgr)
						' this variable isn't stored in cache
						inputModMulInput = inputModGateActivations.dup("f"c).muli(inputActivations)
					Else
						currentMemoryCellState = workspaceMgr.leverageTo(ArrayType.FF_WORKING_MEM, forgetGateActivations.muli(prevMemCellState)) 'TODO optimize without the copy
						inputModMulInput = inputModGateActivations.muli(inputActivations)
					End If
					currentMemoryCellState.addi(inputModMulInput)

					Dim outputGateActivations As INDArray = ifogActivations.get(all(), interval(2 * hiddenLayerSize, 3 * hiddenLayerSize))
					If hasPeepholeConnections Then
						Dim pmcellWOO As INDArray = currentMemoryCellState.dup("f"c).muliRowVector(wOOTranspose)
						outputGateActivations.addi(pmcellWOO)
					End If
					If forBackprop AndAlso Not sigmoidGates Then
						cacheEnter(training, cacheMode, workspaceMgr)
						toReturn.oz(time) = workspaceMgr.dup(ArrayType.BP_WORKING_MEM, outputGateActivations, "f"c) 'Output gate activations
						cacheExit(training, cacheMode, workspaceMgr)
					End If
					gateActivationFn.getActivation(outputGateActivations, training)
					If forBackprop Then
						If shouldCache(training, cacheMode, workspaceMgr) Then
							cacheEnter(training, cacheMode, workspaceMgr)
							toReturn.oa(time) = outputGateActivations.dup("f"c)
							cacheExit(training, cacheMode, workspaceMgr)
						Else
							toReturn.oa(time) = workspaceMgr.leverageTo(ArrayType.BP_WORKING_MEM, outputGateActivations) 'TODO optimize without leverage
						End If
					End If


					'//////////// same as with iFogActivations - if we use cache, let's create this array right there
					cacheEnter(training, cacheMode, workspaceMgr)
					'LSTM unit outputs:
					Dim currMemoryCellActivation As INDArray
					currMemoryCellActivation = workspaceMgr.dup(ArrayType.FF_WORKING_MEM, currentMemoryCellState, "f"c)
					currMemoryCellActivation = afn.getActivation(currMemoryCellActivation, training)
					cacheExit(training, cacheMode, workspaceMgr)
					'/////////////////

					Dim currHiddenUnitActivations As INDArray
					If forBackprop Then
						cacheEnter(training, cacheMode, workspaceMgr)
						currHiddenUnitActivations = workspaceMgr.dup(ArrayType.BP_WORKING_MEM, currMemoryCellActivation, "f"c).muli(outputGateActivations) 'Expected shape: [m,hiddenLayerSize]
						cacheExit(training, cacheMode, workspaceMgr)
					Else
						currHiddenUnitActivations = currMemoryCellActivation.muli(outputGateActivations) 'Expected shape: [m,hiddenLayerSize]
					End If

					If maskArray IsNot Nothing Then
						'Mask array is present: bidirectional RNN -> need to zero out these activations to avoid
						' incorrectly using activations from masked time steps (i.e., want 0 initialization in both directions)
						'We *also* need to apply this to the memory cells, as they are carried forward
						'Mask array has shape [minibatch, timeSeriesLength] -> get column
						Dim timeStepMaskColumn As INDArray = maskArray.getColumn(time, True)
						currHiddenUnitActivations.muliColumnVector(timeStepMaskColumn)
						currentMemoryCellState.muliColumnVector(timeStepMaskColumn)
					End If

					currentMemoryCellState = workspaceMgr.leverageTo(ArrayType.FF_WORKING_MEM, currentMemoryCellState) 'TODO optimize, without the leverage
					If forBackprop Then
						toReturn.fwdPassOutputAsArrays(time) = currHiddenUnitActivations
						toReturn.memCellState(time) = currentMemoryCellState
						toReturn.memCellActivations(time) = currMemoryCellActivation

						If training AndAlso cacheMode <> CacheMode.NONE AndAlso workspaceMgr.hasConfiguration(ArrayType.FF_CACHE) AndAlso workspaceMgr.isWorkspaceOpen(ArrayType.FF_CACHE) Then
							toReturn.memCellActivations(time) = workspaceMgr.leverageTo(ArrayType.FF_CACHE, toReturn.memCellActivations(time))
							toReturn.memCellState(time) = workspaceMgr.leverageTo(ArrayType.FF_CACHE, toReturn.memCellState(time))
						End If

						If cacheMode <> CacheMode.NONE Then
							outputActivations.tensorAlongDimension(time, 1, 0).assign(currHiddenUnitActivations)
						End If
					Else
						outputActivations.tensorAlongDimension(time, 1, 0).assign(currHiddenUnitActivations)
					End If

					prevOutputActivations = currHiddenUnitActivations
					prevMemCellState = currentMemoryCellState

					' no need to dup here, if that's cache - it's already within Cache workspace
					toReturn.lastAct = currHiddenUnitActivations

					' the same as above, already in cache
					toReturn.lastMemCell = currentMemoryCellState
				End Using
			Next iTimeIndex



			'toReturn.leverageTo(ComputationGraph.workspaceExternal);

			toReturn.prevAct = originalPrevOutputActivations
			toReturn.prevMemCell = originalPrevMemCellState

			Return toReturn
		End Function

		Private Shared Function shouldCache(ByVal training As Boolean, ByVal cacheMode As CacheMode, ByVal workspaceMgr As LayerWorkspaceMgr) As Boolean
			Return training AndAlso cacheMode <> CacheMode.NONE AndAlso workspaceMgr.hasConfiguration(ArrayType.FF_CACHE) AndAlso workspaceMgr.isWorkspaceOpen(ArrayType.FF_CACHE)
		End Function

		Private Shared Sub cacheEnter(ByVal training As Boolean, ByVal cacheMode As CacheMode, ByVal workspaceMgr As LayerWorkspaceMgr)
			If shouldCache(training, cacheMode, workspaceMgr) Then
				workspaceMgr.notifyScopeBorrowed(ArrayType.FF_CACHE)
			End If
		End Sub

		Private Shared Sub cacheExit(ByVal training As Boolean, ByVal cacheMode As CacheMode, ByVal workspaceMgr As LayerWorkspaceMgr)
			If shouldCache(training, cacheMode, workspaceMgr) Then
				Nd4j.WorkspaceManager.getWorkspaceForCurrentThread(workspaceMgr.getWorkspaceName(ArrayType.FF_CACHE)).notifyScopeLeft()
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient, org.nd4j.linalg.api.ndarray.INDArray> backpropGradientHelper(final BaseRecurrentLayer layer, final org.deeplearning4j.nn.conf.NeuralNetConfiguration conf, final org.nd4j.linalg.activations.IActivation gateActivationFn, org.nd4j.linalg.api.ndarray.INDArray input, final org.nd4j.linalg.api.ndarray.INDArray recurrentWeights, final org.nd4j.linalg.api.ndarray.INDArray inputWeights, final org.nd4j.linalg.api.ndarray.INDArray epsilon, final boolean truncatedBPTT, final int tbpttBackwardLength, final FwdPassReturn fwdPass, final boolean forwards, final String inputWeightKey, final String recurrentWeightKey, final String biasWeightKey, final java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> gradientViews, org.nd4j.linalg.api.ndarray.INDArray maskArray, final boolean hasPeepholeConnections, final LSTMHelper helper, final org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr, final boolean isHelperAllowFallback)
		Public Shared Function backpropGradientHelper(ByVal layer As BaseRecurrentLayer, ByVal conf As NeuralNetConfiguration, ByVal gateActivationFn As IActivation, ByVal input As INDArray, ByVal recurrentWeights As INDArray, ByVal inputWeights As INDArray, ByVal epsilon As INDArray, ByVal truncatedBPTT As Boolean, ByVal tbpttBackwardLength As Integer, ByVal fwdPass As FwdPassReturn, ByVal forwards As Boolean, ByVal inputWeightKey As String, ByVal recurrentWeightKey As String, ByVal biasWeightKey As String, ByVal gradientViews As IDictionary(Of String, INDArray), ByVal maskArray As INDArray, ByVal hasPeepholeConnections As Boolean, ByVal helper As LSTMHelper, ByVal workspaceMgr As LayerWorkspaceMgr, ByVal isHelperAllowFallback As Boolean) As Pair(Of Gradient, INDArray)

			input = input.castTo(inputWeights.dataType()) 'No-op if

			'Expect errors to have shape: [miniBatchSize,n^(L+1),timeSeriesLength]
			Dim hiddenLayerSize As val = recurrentWeights.size(0) 'i.e., n^L
			Dim prevLayerSize As val = inputWeights.size(0) 'n^(L-1)
			Dim miniBatchSize As val = epsilon.size(0)
			Dim is2dInput As Boolean = epsilon.rank() < 3 'Edge case: T=1 may have shape [miniBatchSize,n^(L+1)], equiv. to [miniBatchSize,n^(L+1),1]
			Dim timeSeriesLength As val = (If(is2dInput, 1, epsilon.size(2)))
			Dim wFFTranspose As INDArray = Nothing
			Dim wOOTranspose As INDArray = Nothing
			Dim wGGTranspose As INDArray = Nothing
			If hasPeepholeConnections Then
				wFFTranspose = recurrentWeights.get(all(), point(4 * hiddenLayerSize)).reshape(1, recurrentWeights.size(0))
				wOOTranspose = recurrentWeights.get(all(), point(4 * hiddenLayerSize + 1)).reshape(1, recurrentWeights.size(0))
				wGGTranspose = recurrentWeights.get(all(), point(4 * hiddenLayerSize + 2)).reshape(1, recurrentWeights.size(0))
			End If


			Dim wIFOG As INDArray = recurrentWeights.get(all(), interval(0, 4 * hiddenLayerSize))
			'F order here so that content for time steps are together
			Dim epsilonNext As INDArray = workspaceMgr.create(ArrayType.ACTIVATION_GRAD, input.dataType(), New Long() {miniBatchSize, prevLayerSize, timeSeriesLength}, "f"c) 'i.e., what would be W^L*(delta^L)^T. Shape: [m,n^(L-1),T]

			Dim nablaCellStateNext As INDArray = Nothing

			Dim deltaifogNext As INDArray = Nd4j.create(inputWeights.dataType(), New Long() {miniBatchSize, 4 * hiddenLayerSize}, "f"c)
			Dim deltaiNext As INDArray = deltaifogNext.get(all(), interval(0, hiddenLayerSize))
			Dim deltafNext As INDArray = deltaifogNext.get(all(), interval(hiddenLayerSize, 2 * hiddenLayerSize))
			Dim deltaoNext As INDArray = deltaifogNext.get(all(), interval(2 * hiddenLayerSize, 3 * hiddenLayerSize))
			Dim deltagNext As INDArray = deltaifogNext.get(all(), interval(3 * hiddenLayerSize, 4 * hiddenLayerSize))

	'        Level1 l1BLAS = Nd4j.getBlasWrapper().level1();
			Dim endIdx As Long = 0

			If truncatedBPTT Then
				endIdx = Math.Max(0, timeSeriesLength - tbpttBackwardLength)
			End If

			'Get gradients. Note that we have to manually zero these, as they might not be initialized (or still has data from last iteration)
			'Also note that they are in f order (as per param initializer) so can be used in gemm etc
			Dim iwGradientsOut As INDArray = gradientViews(inputWeightKey)
			Dim rwGradientsOut As INDArray = gradientViews(recurrentWeightKey) 'Order: {I,F,O,G,FF,OO,GG}
			Dim bGradientsOut As INDArray = gradientViews(biasWeightKey)
			iwGradientsOut.assign(0)
			rwGradientsOut.assign(0)
			bGradientsOut.assign(0)

			Dim rwGradientsIFOG As INDArray = rwGradientsOut.get(all(), interval(0, 4 * hiddenLayerSize))
			Dim rwGradientsFF As INDArray = Nothing
			Dim rwGradientsOO As INDArray = Nothing
			Dim rwGradientsGG As INDArray = Nothing
			If hasPeepholeConnections Then
				rwGradientsFF = rwGradientsOut.get(all(), point(4 * hiddenLayerSize)).reshape(1, recurrentWeights.size(0))
				rwGradientsOO = rwGradientsOut.get(all(), point(4 * hiddenLayerSize + 1)).reshape(1, recurrentWeights.size(0))
				rwGradientsGG = rwGradientsOut.get(all(), point(4 * hiddenLayerSize + 2)).reshape(1, recurrentWeights.size(0))
			End If

			If helper IsNot Nothing AndAlso (layer.helperCountFail = 0 OrElse Not isHelperAllowFallback) Then
				Dim ret As Pair(Of Gradient, INDArray) = Nothing
				Try
					ret = helper.backpropGradient(conf, gateActivationFn, input, recurrentWeights, inputWeights, epsilon, truncatedBPTT, tbpttBackwardLength, fwdPass, forwards, inputWeightKey, recurrentWeightKey, biasWeightKey, gradientViews, maskArray, hasPeepholeConnections, workspaceMgr)
				Catch e As ND4JOpProfilerException
					Throw e 'NaN panic etc for debugging
				Catch e As Exception
					If e.Message.contains("Failed to allocate") Then
						'This is a memory exception - don't fallback to built-in implementation
						Throw e
					End If

					If isHelperAllowFallback Then
						layer.helperCountFail += 1
						log.warn("MKL/CuDNN execution failed - falling back on built-in implementation",e)
					Else
						Throw New Exception("Error during LSTM MKL/CuDNN helper backprop - helperAllowFallback() is set to false", e)
					End If
				End Try

				If ret IsNot Nothing Then
					Return ret
				End If
			End If

			Dim sigmoidGates As Boolean = TypeOf gateActivationFn Is ActivationSigmoid
			Dim afn As IActivation = CType(conf.getLayer(), org.deeplearning4j.nn.conf.layers.BaseLayer).getActivationFn()

			Dim timeStepMaskColumn As INDArray = Nothing
			For iTimeIndex As Long = timeSeriesLength - 1 To endIdx Step -1
				Using ws As org.nd4j.linalg.api.memory.MemoryWorkspace = workspaceMgr.notifyScopeEntered(org.deeplearning4j.nn.workspace.ArrayType.RNN_BP_LOOP_WORKING_MEM)

					If iTimeIndex > Integer.MaxValue Then
						Throw New ND4JArraySizeException()
					End If
					Dim time As Integer = CInt(iTimeIndex)
					Dim inext As Integer = 1

					If Not forwards Then
						time = CInt(timeSeriesLength - iTimeIndex - 1)
						inext = -1
					End If


					'First: calclate the components of nablaCellState that relies on the next time step deltas, so we can overwrite the deltas
					Dim nablaCellState As INDArray
					If iTimeIndex <> timeSeriesLength - 1 AndAlso hasPeepholeConnections Then
						nablaCellState = deltafNext.dup("f"c).muliRowVector(wFFTranspose)
						nablaCellState.addi(deltagNext.dup("f"c).muliRowVector(wGGTranspose))
					Else
						nablaCellState = Nd4j.create(inputWeights.dataType(), New Long(){miniBatchSize, hiddenLayerSize}, "f"c)
					End If

					Dim prevMemCellState As INDArray = (If(iTimeIndex = 0, fwdPass.prevMemCell, fwdPass.memCellState((time - inext))))
					Dim prevHiddenUnitActivation As INDArray = (If(iTimeIndex = 0, fwdPass.prevAct, fwdPass.fwdPassOutputAsArrays((time - inext))))
					Dim currMemCellState As INDArray = fwdPass.memCellState(time)

					'LSTM unit output errors (dL/d(a_out)); not to be confused with \delta=dL/d(z_out)

					Dim epsilonSlice As INDArray = (If(is2dInput, epsilon, epsilon.tensorAlongDimension(time, 1, 0))) '(w^{L+1}*(delta^{(L+1)t})^T)^T or equiv.
					Dim nablaOut As INDArray = Shape.toOffsetZeroCopy(epsilonSlice, "f"c) 'Shape: [m,n^L]
					If iTimeIndex <> timeSeriesLength - 1 Then
						'if t == timeSeriesLength-1 then deltaiNext etc are zeros
						Nd4j.gemm(deltaifogNext, wIFOG, nablaOut, False, True, 1.0, 1.0)
					End If

					'Output gate deltas:
					Dim sigmahOfS As INDArray = fwdPass.memCellActivations(time)
					Dim ao As INDArray = fwdPass.oa(time)

					'Normally would use zo.dup() in above line, but won't be using zo again (for this time step). Ditto for zf, zg, zi
					Dim deltao As INDArray = deltaoNext
					Nd4j.Executioner.exec(New MulOp(nablaOut, sigmahOfS, deltao))
					If sigmoidGates Then
						Dim sigmaoPrimeOfZo As INDArray = Nd4j.Executioner.exec(New TimesOneMinus(ao.dup("f"c))) 'Equivalent to sigmoid deriv on zo
						deltao.muli(sigmaoPrimeOfZo)
					Else
						deltao.assign(gateActivationFn.backprop(fwdPass.oz(time), deltao).First) 'Deltao needs to be modified in-place
						'TODO: optimize (no assign)
					End If

					'Memory cell error:
					Dim temp As INDArray = afn.backprop(currMemCellState.dup("f"c), ao.muli(nablaOut)).First 'TODO activation functions with params
					nablaCellState.addi(temp)
					If hasPeepholeConnections Then
						Dim deltaMulRowWOO As INDArray = deltao.dup("f"c).muliRowVector(wOOTranspose)
						nablaCellState.addi(deltaMulRowWOO)
					End If
					If iTimeIndex <> timeSeriesLength - 1 Then
						Dim nextForgetGateAs As INDArray = fwdPass.fa(time + inext)
						nablaCellState.addi(nextForgetGateAs.muli(nablaCellStateNext))
					End If


					'Store for use in next iteration, and IF we're in workspace, we need to push it out of current workspace
					nablaCellStateNext = workspaceMgr.leverageTo(ArrayType.BP_WORKING_MEM, nablaCellState) 'TODO optimize without leverage


					'Forget gate delta:
					Dim af As INDArray = fwdPass.fa(time)
					Dim deltaf As INDArray = Nothing
					If iTimeIndex > 0 OrElse prevMemCellState IsNot Nothing Then 'For time == 0 && no prevMemCellState, equivalent to muli by 0
						'Note that prevMemCellState may be non-null at t=0 for TBPTT
						deltaf = deltafNext
						If sigmoidGates Then
							Nd4j.Executioner.exec(New TimesOneMinus(af, deltaf))
							deltaf.muli(nablaCellState)
							deltaf.muli(prevMemCellState)
						Else
							Dim temp2 As INDArray = nablaCellState.mul(prevMemCellState)
							deltaf.assign(gateActivationFn.backprop(fwdPass.fz(time).dup("f"c), temp2).First) 'deltaf needs to be modified in-place
							'TODO activation functions with params
						End If
					End If
					'Shape: [m,n^L]

					'Input modulation gate delta:
					Dim ag As INDArray = fwdPass.ga(time)
					Dim ai As INDArray = fwdPass.ia(time)
					Dim deltag As INDArray = deltagNext
					If sigmoidGates Then
						Nd4j.Executioner.exec(New TimesOneMinus(ag, deltag)) 'Equivalent to sigmoid deriv on zg
						deltag.muli(ai)
						deltag.muli(nablaCellState)
					Else
						Dim temp2 As INDArray = Nd4j.Executioner.exec(New MulOp(ai, nablaCellState, Nd4j.createUninitialized(inputWeights.dataType(), ai.shape(), "f"c)))(0)
						deltag.assign(gateActivationFn.backprop(fwdPass.gz(time), temp2).First)
						'TODO activation functions with params; optimize (no assign)
					End If
					'Shape: [m,n^L]

					'Network input delta:
					Dim zi As INDArray = fwdPass.iz(time)
					Dim deltai As INDArray = deltaiNext
					temp = Nd4j.Executioner.exec(New MulOp(ag, nablaCellState, Nd4j.createUninitialized(inputWeights.dataType(), deltai.shape(), "f"c)))(0)
					deltai.assign(afn.backprop(zi, temp).First)
					'TODO activation functions with params; also: optimize this (no assign)
					'Shape: [m,n^L]


					'Handle masking
					If maskArray IsNot Nothing Then
						'Mask array is present: bidirectional RNN -> need to zero out these errors to avoid using errors from a masked time step
						' to calculate the parameter gradients.  Mask array has shape [minibatch, timeSeriesLength] -> get column(this time step)
						timeStepMaskColumn = maskArray.getColumn(time, True)
						deltaifogNext.muli(timeStepMaskColumn)
						'Later, the deltaifogNext is used to calculate: input weight gradients, recurrent weight gradients, bias gradients
					End If

					Dim prevLayerActivationSlice As INDArray = Shape.toMmulCompatible(If(is2dInput, input, input.tensorAlongDimension(time, 1, 0)))
					If iTimeIndex > 0 OrElse prevHiddenUnitActivation IsNot Nothing Then 'For time == 0 && no prevMemCellState, equivalent to muli by 0
						'Note that prevHiddenUnitActivations may be non-null at t=0 for TBPTT
						'Again, deltaifog_current == deltaifogNext at this point... same array
						Nd4j.gemm(prevLayerActivationSlice, deltaifogNext, iwGradientsOut, True, False, 1.0, 1.0)
					Else
						Dim iwGradients_i As INDArray = iwGradientsOut.get(all(), interval(0, hiddenLayerSize))
						Nd4j.gemm(prevLayerActivationSlice, deltai, iwGradients_i, True, False, 1.0, 1.0)
						Dim iwGradients_og As INDArray = iwGradientsOut.get(all(), interval(2 * hiddenLayerSize, 4 * hiddenLayerSize))
						Dim deltaog As INDArray = deltaifogNext.get(all(), interval(2 * hiddenLayerSize, 4 * hiddenLayerSize))
						Nd4j.gemm(prevLayerActivationSlice, deltaog, iwGradients_og, True, False, 1.0, 1.0)
					End If

					If iTimeIndex > 0 OrElse prevHiddenUnitActivation IsNot Nothing Then
						'If t==0 and prevHiddenUnitActivation==null, equiv. to zeros(n^L,n^L), so dL/dW for recurrent weights
						' will end up as 0 anyway
						'At this point: deltaifog and deltaifogNext are the same thing...
						'So what we are actually doing here is sum of (prevAct^transpose * deltaifog_current)
						Nd4j.gemm(prevHiddenUnitActivation, deltaifogNext, rwGradientsIFOG, True, False, 1.0, 1.0)

						'Shape: [1,n^L]. sum(0) is sum over examples in mini-batch.
						'Can use axpy here because result of sum and rwGradients[4 to 6] have order Nd4j.order(), via Nd4j.create()
						If hasPeepholeConnections Then
							Dim dLdwFF As INDArray = deltaf.dup("f"c).muli(prevMemCellState).sum(True, 0) 'mul not mmul because these weights are from unit j->j only (whereas other recurrent weights are i->j for all i,j)
							rwGradientsFF.addi(dLdwFF)
							Dim dLdwGG As INDArray = deltag.dup("f"c).muli(prevMemCellState).sum(True, 0)
							rwGradientsGG.addi(dLdwGG)
						End If
					End If

					If hasPeepholeConnections Then
						Dim dLdwOO As INDArray = deltao.dup("f"c).muli(currMemCellState).sum(True, 0) 'Expected shape: [n^L,1]. sum(0) is sum over examples in mini-batch.
						rwGradientsOO.addi(dLdwOO)
					End If

					If iTimeIndex > 0 OrElse prevHiddenUnitActivation IsNot Nothing Then 'For time == 0 && no prevMemCellState, equivalent to muli by 0
						'Note that prevHiddenUnitActivation may be non-null at t=0 for TBPTT
						bGradientsOut.addi(deltaifogNext.sum(True, 0))
					Else
						bGradientsOut.get(interval(0,0,True), interval(0, hiddenLayerSize)).addi(deltai.sum(True, 0))
						Dim ogBiasToAdd As INDArray = deltaifogNext.get(all(), interval(2 * hiddenLayerSize, 4 * hiddenLayerSize)).sum(True, 0)
						Dim ogBiasGrad As INDArray = bGradientsOut.get(interval(0,0,True), interval(2 * hiddenLayerSize, 4 * hiddenLayerSize))
						ogBiasGrad.addi(ogBiasToAdd)
					End If

					'Calculate epsilonNext - i.e., equiv. to what would be (w^L*(d^(Lt))^T)^T in a normal network
					'But here, need to add 4 weights * deltas for the IFOG gates
					Dim epsilonNextSlice As INDArray = epsilonNext.tensorAlongDimension(time, 1, 0) 'This slice: f order and contiguous, due to epsilonNext being defined as f order.
					If iTimeIndex > 0 OrElse prevHiddenUnitActivation IsNot Nothing Then
						'Note that prevHiddenUnitActivation may be non-null at t=0 for TBPTT
						Nd4j.gemm(deltaifogNext, inputWeights, epsilonNextSlice, False, True, 1.0, 1.0)
					Else
						'No contribution from forget gate at t=0
						Dim wi As INDArray = inputWeights.get(all(), interval(0, hiddenLayerSize))
						Nd4j.gemm(deltai, wi, epsilonNextSlice, False, True, 1.0, 1.0)
						Dim deltaog As INDArray = deltaifogNext.get(all(), interval(2 * hiddenLayerSize, 4 * hiddenLayerSize))
						Dim wog As INDArray = inputWeights.get(all(), interval(2 * hiddenLayerSize, 4 * hiddenLayerSize))
						Nd4j.gemm(deltaog, wog, epsilonNextSlice, False, True, 1.0, 1.0) 'epsilonNextSlice.addi(deltao.mmul(woTranspose)).addi(deltag.mmul(wgTranspose));
					End If

					If maskArray IsNot Nothing Then
						'Mask array is present: bidirectional RNN -> need to zero out these errors to avoid sending anything
						' but 0s to the layer below at this time step (for the given example)
						epsilonNextSlice.muli(timeStepMaskColumn)
					End If
				End Using
			Next iTimeIndex

			Dim retGradient As Gradient = New DefaultGradient()
			retGradient.gradientForVariable()(inputWeightKey) = iwGradientsOut
			retGradient.gradientForVariable()(recurrentWeightKey) = rwGradientsOut
			retGradient.gradientForVariable()(biasWeightKey) = bGradientsOut

			Return New Pair(Of Gradient, INDArray)(retGradient, epsilonNext)
		End Function


		Public Shared Function getMemoryReport(ByVal lstmLayer As AbstractLSTM, ByVal inputType As InputType) As LayerMemoryReport
			Dim isGraves As Boolean = TypeOf lstmLayer Is org.deeplearning4j.nn.conf.layers.GravesLSTM
			Return getMemoryReport(isGraves, lstmLayer, inputType)
		End Function

		Public Shared Function getMemoryReport(ByVal lstmLayer As GravesBidirectionalLSTM, ByVal inputType As InputType) As LayerMemoryReport
			Dim r As LayerMemoryReport = getMemoryReport(True, lstmLayer, inputType)

			'Double everything for bidirectional
			Dim fixedTrain As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
			Dim varTrain As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
			Dim cacheFixed As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
			Dim cacheVar As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
			For Each cm As CacheMode In System.Enum.GetValues(GetType(CacheMode))
				fixedTrain(cm) = 2 * r.getWorkingMemoryFixedTrain().get(cm)
				varTrain(cm) = 2 * r.getWorkingMemoryVariableTrain().get(cm)
				cacheFixed(cm) = 2 * r.getCacheModeMemFixed().get(cm)
				cacheVar(cm) = 2 * r.getCacheModeMemVariablePerEx().get(cm)
			Next cm

			Return (New LayerMemoryReport.Builder(r.getLayerName(), r.GetType(), r.getInputType(), r.getOutputType())).standardMemory(2 * r.getParameterSize(), 2 * r.getUpdaterStateSize()).workingMemory(2 * r.getWorkingMemoryFixedInference(), 2 * r.getWorkingMemoryVariableInference(), fixedTrain, varTrain).cacheMemory(cacheFixed, cacheVar).build()
		End Function

		Public Shared Function getMemoryReport(ByVal isGraves As Boolean, ByVal lstmLayer As org.deeplearning4j.nn.conf.layers.FeedForwardLayer, ByVal inputType As InputType) As LayerMemoryReport


			Dim itr As InputType.InputTypeRecurrent = DirectCast(inputType, InputType.InputTypeRecurrent)
			Dim tsLength As val = itr.getTimeSeriesLength()

			Dim outputType As InputType = lstmLayer.getOutputType(-1, inputType)

			Dim numParams As val = lstmLayer.initializer().numParams(lstmLayer)
			Dim updaterSize As Integer = CInt(Math.Truncate(lstmLayer.getIUpdater().stateSize(numParams)))

			'Memory use during forward pass:
			'ifogActivations: nTimeSteps * [minibatch,4*layerSize] (not cached during inference fwd pass)
			Dim workingMemInferencePerEx As val = tsLength * 4 * lstmLayer.getNOut() 'Reduced by factor of tsLength if using workspace

			'For training, we also have
			'nTimeSteps * 5 * [minibatch, nOut] - 4 x gate pre-outs, memory cell state - may be cached
			'nTimeSteps * [minibatch, nOut] - peephole conneciton activations, graves LSTM only - may be cached
			'Total: 4 + 5 + 1 = 10xnOut per time step (training) or 4x (inference)
			Dim fwdPassPerTimeStepTrainCache As val = tsLength * 6 * lstmLayer.getNOut()

			'During backprop:
			'2 dups of size [minibatch, nOut] for nablaCellState (1 alloc only for no peephole)
			'1 [minibatch, nOut] for deltao
			'2 for memory cell error
			'1 allocation for input modulation gate
			'1 for layer input
			'3 dups [minibatch, nOut] for peephole (Graves only)
			' 5xnOut (independent of minibatch size) - deltaiFog, peephole etc. Only 2 if no peephole TODO
			'6 for non-graves, 9 for graves

			Dim backpropWorkingSpace As val = (If(isGraves, 9, 6)) * tsLength * lstmLayer.getNOut()

			'TODO NO WAY TO TAKE LSTM WORKSPACE INTO ACCOUNT HERE :(


			Dim trainVariable As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
			Dim cacheVariable As IDictionary(Of CacheMode, Long) = New Dictionary(Of CacheMode, Long)()
			For Each cm As CacheMode In System.Enum.GetValues(GetType(CacheMode))
				Dim trainWorking As Long
				Dim cacheMem As Long

				If cm = CacheMode.NONE Then
					trainWorking = workingMemInferencePerEx + fwdPassPerTimeStepTrainCache + backpropWorkingSpace
					cacheMem = 0
				Else
					trainWorking = workingMemInferencePerEx + backpropWorkingSpace
					cacheMem = fwdPassPerTimeStepTrainCache
				End If

				trainVariable(cm) = trainWorking
				cacheVariable(cm) = cacheMem
			Next cm

			Return (New LayerMemoryReport.Builder(Nothing, lstmLayer.GetType(), inputType, outputType)).standardMemory(numParams, updaterSize).workingMemory(0, workingMemInferencePerEx, MemoryReport.CACHE_MODE_ALL_ZEROS, trainVariable).cacheMemory(MemoryReport.CACHE_MODE_ALL_ZEROS, cacheVariable).build()
		End Function
	End Class

End Namespace