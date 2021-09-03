Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports BinaryByteUnit = com.jakewharton.byteunits.BinaryByteUnit
Imports Pointer = org.bytedeco.javacpp.Pointer
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DefaultGradient = org.deeplearning4j.nn.gradient.DefaultGradient
Imports Gradient = org.deeplearning4j.nn.gradient.Gradient
Imports BaseCudnnHelper = org.deeplearning4j.cuda.BaseCudnnHelper
Imports FwdPassReturn = org.deeplearning4j.nn.layers.recurrent.FwdPassReturn
Imports LSTMHelper = org.deeplearning4j.nn.layers.recurrent.LSTMHelper
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports IActivation = org.nd4j.linalg.activations.IActivation
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports ActivationTanH = org.nd4j.linalg.activations.impl.ActivationTanH
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Shape = org.nd4j.linalg.api.shape.Shape
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports org.nd4j.common.primitives
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports ArrayType = org.deeplearning4j.nn.workspace.ArrayType
Imports org.bytedeco.cuda.cudart
Imports org.bytedeco.cuda.cudnn
Imports org.bytedeco.cuda.global.cudart
Imports org.bytedeco.cuda.global.cudnn

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  * See the NOTICE file distributed with this work for additional
' *  * information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.deeplearning4j.cuda.recurrent


	''' <summary>
	''' cuDNN-based helper for the recurrent LSTM layer (no peephole connections).
	''' 
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class CudnnLSTMHelper extends org.deeplearning4j.cuda.BaseCudnnHelper implements org.deeplearning4j.nn.layers.recurrent.LSTMHelper
	Public Class CudnnLSTMHelper
		Inherits BaseCudnnHelper
		Implements LSTMHelper

		Public Sub New(ByVal dataType As DataType)
			MyBase.New(dataType)
		End Sub

		Private Class CudnnLSTMContext
			Inherits CudnnContext

			Private Class Deallocator
				Inherits CudnnLSTMContext
				Implements Pointer.Deallocator

				Friend Sub New(ByVal c As CudnnLSTMContext)
					MyBase.New(c)
				End Sub

				Public Overrides Sub deallocate()
					destroyHandles()
				End Sub
			End Class

			Friend hxDesc As New cudnnTensorStruct(), cxDesc As New cudnnTensorStruct()
			Friend hyDesc As New cudnnTensorStruct(), cyDesc As New cudnnTensorStruct()
			Friend dhxDesc As New cudnnTensorStruct(), dcxDesc As New cudnnTensorStruct()
			Friend dhyDesc As New cudnnTensorStruct(), dcyDesc As New cudnnTensorStruct()

			Friend wDesc As New cudnnFilterStruct(), dwDesc As New cudnnFilterStruct()
			Friend linLayerMatDesc As New cudnnFilterStruct(), linLayerBiasDesc As New cudnnFilterStruct()

			Friend rnnDesc As New cudnnRNNStruct()
			Friend dropoutDesc As New cudnnDropoutStruct()
			Friend activationDesc As New cudnnActivationStruct()

			Public Sub New()
				createHandles()
				deallocator(New Deallocator(Me))
			End Sub

			Public Sub New(ByVal c As CudnnLSTMContext)
				MyBase.New(c)
				hxDesc = New cudnnTensorStruct(c.hxDesc)
				cxDesc = New cudnnTensorStruct(c.cxDesc)
				hyDesc = New cudnnTensorStruct(c.hyDesc)
				cyDesc = New cudnnTensorStruct(c.cyDesc)
				dhxDesc = New cudnnTensorStruct(c.dhxDesc)
				dcxDesc = New cudnnTensorStruct(c.dcxDesc)
				dhyDesc = New cudnnTensorStruct(c.dhyDesc)
				dcyDesc = New cudnnTensorStruct(c.dcyDesc)

				wDesc = New cudnnFilterStruct(c.wDesc)
				dwDesc = New cudnnFilterStruct(c.dwDesc)
				linLayerMatDesc = New cudnnFilterStruct(c.linLayerMatDesc)
				linLayerBiasDesc = New cudnnFilterStruct(c.linLayerBiasDesc)

				rnnDesc = New cudnnRNNStruct(c.rnnDesc)
				dropoutDesc = New cudnnDropoutStruct(c.dropoutDesc)
				activationDesc = New cudnnActivationStruct(c.activationDesc)
			End Sub

			Protected Friend Overrides Sub createHandles()
				MyBase.createHandles()

				checkCudnn(cudnnCreateTensorDescriptor(hxDesc))
				checkCudnn(cudnnCreateTensorDescriptor(cxDesc))
				checkCudnn(cudnnCreateTensorDescriptor(hyDesc))
				checkCudnn(cudnnCreateTensorDescriptor(cyDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dhxDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dcxDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dhyDesc))
				checkCudnn(cudnnCreateTensorDescriptor(dcyDesc))

				checkCudnn(cudnnCreateFilterDescriptor(wDesc))
				checkCudnn(cudnnCreateFilterDescriptor(dwDesc))
				checkCudnn(cudnnCreateFilterDescriptor(linLayerMatDesc))
				checkCudnn(cudnnCreateFilterDescriptor(linLayerBiasDesc))

				checkCudnn(cudnnCreateRNNDescriptor(rnnDesc))
				checkCudnn(cudnnCreateDropoutDescriptor(dropoutDesc))
				checkCudnn(cudnnCreateActivationDescriptor(activationDesc))
			End Sub

			Protected Friend Overrides Sub destroyHandles()
				checkCudnn(cudnnDestroyActivationDescriptor(activationDesc))
				checkCudnn(cudnnDestroyDropoutDescriptor(dropoutDesc))
				checkCudnn(cudnnDestroyRNNDescriptor(rnnDesc))

				checkCudnn(cudnnDestroyFilterDescriptor(wDesc))
				checkCudnn(cudnnDestroyFilterDescriptor(dwDesc))
				checkCudnn(cudnnDestroyFilterDescriptor(linLayerMatDesc))
				checkCudnn(cudnnDestroyFilterDescriptor(linLayerBiasDesc))

				checkCudnn(cudnnDestroyTensorDescriptor(hxDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(cxDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(hyDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(cyDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dhxDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dcxDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dhyDesc))
				checkCudnn(cudnnDestroyTensorDescriptor(dcyDesc))

				MyBase.destroyHandles()
			End Sub
		End Class

		' These constants might eventually become variable parameters...
		Protected Friend Const NUM_LAYERS As Integer = 1
		Protected Friend Const DROPOUT As Single = 0
		Protected Friend Const BIDIRECTIONAL As Boolean = False
		Protected Friend Shared ReadOnly RNN_MODE As Integer = CUDNN_LSTM
		Protected Friend Const NUM_LINEAR_LAYERS As Integer = 8 ' CUDNN_LSTM

		Private cudnnContext As New CudnnLSTMContext()
		Private xDesc As New TensorArray()
		Private yDesc As New TensorArray()
		Private dxDesc As New TensorArray()
		Private dyDesc As New TensorArray()
		Private stateSpace As New DataCache()
		Private reserveSpace As New DataCache()
		Private weightsSpace As New DataCache()

		Private initializedDropoutDescriptor As Boolean = False

		Private Shared Function toCOrder(ByVal arr As INDArray) As INDArray
			If arr.View OrElse arr.ordering() <> "c"c OrElse Not Shape.strideDescendingCAscendingF(arr) Then
				arr = arr.dup("c"c)
			End If
			Return arr
		End Function

		Public Overridable Overloads Function checkSupported(ByVal gateActivationFn As IActivation, ByVal activationFn As IActivation, ByVal hasPeepholeConnections As Boolean) As Boolean Implements LSTMHelper.checkSupported
			Dim supported As Boolean = checkSupported()
			If Not (TypeOf gateActivationFn Is ActivationSigmoid) Then
				supported = False
				log.warn("Not supported: Gate activation functions != ActivationSigmoid")
			End If
			If Not (TypeOf activationFn Is ActivationTanH) Then
				supported = False
				log.warn("Not supported: Layer activation functions != ActivationTanH")
			End If
			If hasPeepholeConnections Then
				supported = False
				log.warn("Not supported: LSTM layers with peephole connections")
			End If
			Return supported
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.common.primitives.Pair<org.deeplearning4j.nn.gradient.Gradient, org.nd4j.linalg.api.ndarray.INDArray> backpropGradient(final org.deeplearning4j.nn.conf.NeuralNetConfiguration conf, final org.nd4j.linalg.activations.IActivation gateActivationFn, final org.nd4j.linalg.api.ndarray.INDArray input, final org.nd4j.linalg.api.ndarray.INDArray recurrentWeights, final org.nd4j.linalg.api.ndarray.INDArray inputWeights, final org.nd4j.linalg.api.ndarray.INDArray epsilon, final boolean truncatedBPTT, final int tbpttBackwardLength, final org.deeplearning4j.nn.layers.recurrent.FwdPassReturn fwdPass, final boolean forwards, final String inputWeightKey, final String recurrentWeightKey, final String biasWeightKey, final java.util.Map<String, org.nd4j.linalg.api.ndarray.INDArray> gradientViews, org.nd4j.linalg.api.ndarray.INDArray maskArray, final boolean hasPeepholeConnections, final org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr)
		Public Overridable Function backpropGradient(ByVal conf As NeuralNetConfiguration, ByVal gateActivationFn As IActivation, ByVal input As INDArray, ByVal recurrentWeights As INDArray, ByVal inputWeights As INDArray, ByVal epsilon As INDArray, ByVal truncatedBPTT As Boolean, ByVal tbpttBackwardLength As Integer, ByVal fwdPass As FwdPassReturn, ByVal forwards As Boolean, ByVal inputWeightKey As String, ByVal recurrentWeightKey As String, ByVal biasWeightKey As String, ByVal gradientViews As IDictionary(Of String, INDArray), ByVal maskArray As INDArray, ByVal hasPeepholeConnections As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As Pair(Of Gradient, INDArray)

			'Expect errors to have shape: [miniBatchSize,n^(L+1),timeSeriesLength]
			Dim hiddenLayerSize As val = recurrentWeights.size(0) 'i.e., n^L
			Dim prevLayerSize As val = inputWeights.size(0) 'n^(L-1)
			Dim inputLayerSize As val = input.size(1)
			Dim miniBatchSize As val = epsilon.size(0)
			Dim is2dInput As Boolean = epsilon.rank() < 3 'Edge case: T=1 may have shape [miniBatchSize,n^(L+1)], equiv. to [miniBatchSize,n^(L+1),1]
			Dim timeSeriesLength As Long = (If(is2dInput, 1, epsilon.size(2)))

			Dim x As INDArray = toCOrder(input.permute(2, 0, 1))
			Dim dy As INDArray = toCOrder(epsilon.permute(2, 0, 1))
			Dim dx As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATION_GRAD, inputWeights.dataType(), New Long() {timeSeriesLength, miniBatchSize, prevLayerSize}, "c"c)

			Dim iwGradientsOut As INDArray = gradientViews(inputWeightKey)
			Dim rwGradientsOut As INDArray = gradientViews(recurrentWeightKey) 'Order: {I,F,O,G}
			Dim bGradientsOut As INDArray = gradientViews(biasWeightKey)

			Dim outputActivations As INDArray = toCOrder(fwdPass.fwdPassOutput.permute(2, 0, 1))
			Dim prevStepMemCellState As INDArray = toCOrder(fwdPass.prevMemCell)
			Dim prevStepActivations As INDArray = toCOrder(fwdPass.prevAct)

			Nd4j.Executioner.commit()

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareActionAllWrite(x, dy, dx, outputActivations, prevStepMemCellState, prevStepActivations, iwGradientsOut, rwGradientsOut, bGradientsOut)
			Dim xData As Pointer = allocator.getPointer(x, context)
			Dim dyData As Pointer = allocator.getPointer(dy, context)
			Dim dxData As Pointer = allocator.getPointer(dx, context)
			Dim outputActivationsData As Pointer = allocator.getPointer(outputActivations, context)
			Dim prevMemCellStateData As Pointer = allocator.getPointer(prevStepMemCellState, context)
			Dim prevStepActivationsData As Pointer = allocator.getPointer(prevStepActivations, context)
			Dim iwGradientsOutData As Pointer = allocator.getPointer(iwGradientsOut, context)
			Dim rwGradientsOutData As Pointer = allocator.getPointer(rwGradientsOut, context)
			Dim bGradientsOutData As Pointer = allocator.getPointer(bGradientsOut, context)

			Dim stream As New CUstream_st(context.CublasStream)
			checkCudnn(cudnnSetStream(cudnnContext, stream))

			If truncatedBPTT Then
				Dim endIdx As val = Math.Max(0, timeSeriesLength - tbpttBackwardLength) * miniBatchSize * hiddenLayerSize
				xData.position(endIdx * dataTypeSize)
				dyData.position(endIdx * (If(BIDIRECTIONAL, 2, 1)) * dataTypeSize)
				outputActivationsData.position(endIdx * (If(BIDIRECTIONAL, 2, 1)) * dataTypeSize)
				timeSeriesLength = CInt(Math.Min(timeSeriesLength, tbpttBackwardLength))
			End If

			Dim xDesc0 As cudnnTensorStruct = xDesc.get(GetType(cudnnTensorStruct), 0)

			Dim workSpace As DataCache = workspaceMgr.getHelperWorkspace(LayerWorkspaceMgr.CUDNN_WORKSPACE_KEY)
			checkCudnn(cudnnRNNBackwardData(cudnnContext, cudnnContext.rnnDesc, CInt(timeSeriesLength), yDesc, outputActivationsData, dyDesc, dyData, cudnnContext.dhyDesc, Nothing, cudnnContext.dcyDesc, Nothing, cudnnContext.wDesc, weightsSpace, cudnnContext.hxDesc, prevStepActivationsData, cudnnContext.cxDesc, prevMemCellStateData, dxDesc, dxData, cudnnContext.dhxDesc, Nothing, cudnnContext.dcxDesc, Nothing, workSpace, workSpace.limit(), reserveSpace, reserveSpace.limit()))

			' cudnnRNNBackwardWeights adds to the data in dW.
			checkCuda(cudaMemsetAsync(weightsSpace, 0, weightsSpace.limit(), stream))

			checkCudnn(cudnnRNNBackwardWeights(cudnnContext, cudnnContext.rnnDesc, CInt(timeSeriesLength), xDesc, xData, cudnnContext.hxDesc, prevStepActivationsData, yDesc, outputActivationsData, workSpace, workSpace.limit(), cudnnContext.dwDesc, weightsSpace, reserveSpace, reserveSpace.limit()))

			Dim dataType(0) As Integer
			Dim format(0) As Integer
			Dim nbDims(0) As Integer
			Dim filterDimA(2) As Integer
			Dim linLayerMat As New Pointer()
			Dim linLayerBias As New Pointer()

			Dim layer As Integer = 0
			Do While layer < NUM_LAYERS * (If(BIDIRECTIONAL, 2, 1))
				For linLayerID As Integer = 0 To NUM_LINEAR_LAYERS - 1
					checkCudnn(cudnnGetRNNLinLayerMatrixParams(cudnnContext, cudnnContext.rnnDesc, layer, xDesc0, cudnnContext.wDesc, weightsSpace, linLayerID, cudnnContext.linLayerMatDesc, linLayerMat))

					checkCudnn(cudnnGetFilterNdDescriptor(cudnnContext.linLayerMatDesc, 3, dataType, format, nbDims, filterDimA))

					checkCudnn(cudnnGetRNNLinLayerBiasParams(cudnnContext, cudnnContext.rnnDesc, layer, xDesc0, cudnnContext.wDesc, weightsSpace, linLayerID, cudnnContext.linLayerBiasDesc, linLayerBias))

					checkCudnn(cudnnGetFilterNdDescriptor(cudnnContext.linLayerBiasDesc, 3, dataType, format, nbDims, filterDimA))

					' our data is in "new, forget, output, and input gates" order (aka IFOG), each kind of weight packed together
					Dim position As Integer = 0
					Dim size As Long = 0
					Dim data As Pointer = Nothing
					Select Case linLayerID
						Case 0
							data = iwGradientsOutData
							position = 3
							size = inputLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 1
							data = iwGradientsOutData
							position = 1
							size = inputLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 2
							data = iwGradientsOutData
							position = 0
							size = inputLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 3
							data = iwGradientsOutData
							position = 2
							size = inputLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 4
							data = rwGradientsOutData
							position = 3
							size = hiddenLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 5
							data = rwGradientsOutData
							position = 1
							size = hiddenLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 6
							data = rwGradientsOutData
							position = 0
							size = hiddenLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 7
							data = rwGradientsOutData
							position = 2
							size = hiddenLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case Else
							Throw New Exception()
					End Select
					checkCuda(cudaMemcpyAsync(data.position(position * size * hiddenLayerSize * dataTypeSize), linLayerMat, size * hiddenLayerSize * dataTypeSize, cudaMemcpyDeviceToDevice, stream))
					If linLayerID < 4 Then
						checkCuda(cudaMemcpyAsync(bGradientsOutData.position(position * hiddenLayerSize * dataTypeSize), linLayerBias, hiddenLayerSize * dataTypeSize, cudaMemcpyDeviceToDevice, stream))
					End If
				Next linLayerID
				layer += 1
			Loop

			allocator.FlowController.registerActionAllWrite(context, x, dy, dx, outputActivations, prevStepMemCellState, prevStepActivations, iwGradientsOut, rwGradientsOut, bGradientsOut)

			Dim retGradient As Gradient = New DefaultGradient()
			retGradient.gradientForVariable()(inputWeightKey) = iwGradientsOut
			retGradient.gradientForVariable()(recurrentWeightKey) = rwGradientsOut
			retGradient.gradientForVariable()(biasWeightKey) = bGradientsOut

			Dim epsilonNext As INDArray = dx.permute(1, 2, 0) 'i.e., what would be W^L*(delta^L)^T. Shape: [m,n^(L-1),T]

			Return New Pair(Of Gradient, INDArray)(retGradient, epsilonNext)
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.nn.layers.recurrent.FwdPassReturn activate(final org.deeplearning4j.nn.api.Layer layer, final org.deeplearning4j.nn.conf.NeuralNetConfiguration conf, final org.nd4j.linalg.activations.IActivation gateActivationFn, org.nd4j.linalg.api.ndarray.INDArray input, final org.nd4j.linalg.api.ndarray.INDArray recurrentWeights, final org.nd4j.linalg.api.ndarray.INDArray inputWeights, final org.nd4j.linalg.api.ndarray.INDArray biases, final boolean training, final org.nd4j.linalg.api.ndarray.INDArray prevOutputActivations, final org.nd4j.linalg.api.ndarray.INDArray prevMemCellState, boolean forBackprop, boolean forwards, final String inputWeightKey, org.nd4j.linalg.api.ndarray.INDArray maskArray, final boolean hasPeepholeConnections, final org.deeplearning4j.nn.workspace.LayerWorkspaceMgr workspaceMgr)
		Public Overridable Function activate(ByVal layer As Layer, ByVal conf As NeuralNetConfiguration, ByVal gateActivationFn As IActivation, ByVal input As INDArray, ByVal recurrentWeights As INDArray, ByVal inputWeights As INDArray, ByVal biases As INDArray, ByVal training As Boolean, ByVal prevOutputActivations As INDArray, ByVal prevMemCellState As INDArray, ByVal forBackprop As Boolean, ByVal forwards As Boolean, ByVal inputWeightKey As String, ByVal maskArray As INDArray, ByVal hasPeepholeConnections As Boolean, ByVal workspaceMgr As LayerWorkspaceMgr) As FwdPassReturn

			Dim is2dInput As Boolean = input.rank() < 3 'Edge case of T=1, may have shape [m,nIn], equiv. to [m,nIn,1]
			Dim timeSeriesLength As val = (If(is2dInput, 1, input.size(2)))
			Dim hiddenLayerSize As val = recurrentWeights.size(0)
			Dim miniBatchSize As val = input.size(0)
			Dim inputLayerSize As val = input.size(1)

			Dim x As INDArray = toCOrder(input.permute(2, 0, 1))
			Dim linInputWeights As INDArray = inputWeights
			Dim linRecurrentWeights As INDArray = recurrentWeights
			Dim linBiases As INDArray = biases

			Dim prevAct As INDArray = toCOrder(prevOutputActivations)
			Dim prevMemCell As INDArray = toCOrder(prevMemCellState)

			Dim outputActivations As INDArray = workspaceMgr.createUninitialized(ArrayType.ACTIVATIONS, inputWeights.dataType(), New Long() {timeSeriesLength, miniBatchSize, hiddenLayerSize * (If(BIDIRECTIONAL, 2, 1))}, "c"c)
			Dim finalMemCellState As INDArray = Nd4j.createUninitialized(inputWeights.dataType(), New Long() { miniBatchSize, hiddenLayerSize}, "c"c)
			Dim finalStepActivations As INDArray = Nd4j.createUninitialized(inputWeights.dataType(), New Long() { miniBatchSize, hiddenLayerSize}, "c"c)

			Dim toReturn As New FwdPassReturn()
			toReturn.prevAct = prevAct
			toReturn.prevMemCell = prevMemCell

			Nd4j.Executioner.commit()



			If timeSeriesLength > xDesc.capacity() Then
				xDesc.deallocate()
				xDesc = New TensorArray(timeSeriesLength)
			End If
			If timeSeriesLength > yDesc.capacity() Then
				yDesc.deallocate()
				yDesc = New TensorArray(timeSeriesLength)
			End If
			If timeSeriesLength > dxDesc.capacity() Then
				dxDesc.deallocate()
				dxDesc = New TensorArray(timeSeriesLength)
			End If
			If timeSeriesLength > dyDesc.capacity() Then
				dyDesc.deallocate()
				dyDesc = New TensorArray(timeSeriesLength)
			End If

			For i As Integer = 0 To timeSeriesLength - 1
				Dim dimA() As Integer = {CInt(miniBatchSize), CInt(inputLayerSize), 1}
				Dim strideA() As Integer = {CInt(dimA(2)) * dimA(1), dimA(2), 1}

				checkCudnn(cudnnSetTensorNdDescriptor(xDesc.get(GetType(cudnnTensorStruct), i), dataType, 3, dimA, strideA))
				checkCudnn(cudnnSetTensorNdDescriptor(dxDesc.get(GetType(cudnnTensorStruct), i), dataType, 3, dimA, strideA))

				Dim dimB() As Integer = {CInt(miniBatchSize), CInt(hiddenLayerSize) * (If(BIDIRECTIONAL, 2, 1)), 1}
				Dim strideB() As Integer = {dimB(2) * dimB(1), dimB(2), 1}

				checkCudnn(cudnnSetTensorNdDescriptor(yDesc.get(GetType(cudnnTensorStruct), i), dataType, 3, dimB, strideB))
				checkCudnn(cudnnSetTensorNdDescriptor(dyDesc.get(GetType(cudnnTensorStruct), i), dataType, 3, dimB, strideB))
			Next i

			Dim dimC() As Integer = {NUM_LAYERS * (If(BIDIRECTIONAL, 2, 1)), CInt(miniBatchSize), CInt(hiddenLayerSize)}
			Dim strideC() As Integer = {dimC(2) * dimC(1), dimC(2), 1}

			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.hxDesc, dataType, 3, dimC, strideC))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.cxDesc, dataType, 3, dimC, strideC))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.hyDesc, dataType, 3, dimC, strideC))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.cyDesc, dataType, 3, dimC, strideC))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.dhxDesc, dataType, 3, dimC, strideC))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.dcxDesc, dataType, 3, dimC, strideC))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.dhyDesc, dataType, 3, dimC, strideC))
			checkCudnn(cudnnSetTensorNdDescriptor(cudnnContext.dcyDesc, dataType, 3, dimC, strideC))

			checkCudnn(cudnnDropoutGetStatesSize(cudnnContext, sizeInBytes))
			Dim stateSize As Long = sizeInBytes.get(0)
			If stateSize > stateSpace.capacity() Then
				stateSpace.deallocate()
				stateSpace = New DataCache(stateSize)
			End If
			stateSpace.limit(stateSize)

			If Not initializedDropoutDescriptor Then
				checkCudnn(cudnnSetDropoutDescriptor(cudnnContext.dropoutDesc, cudnnContext, DROPOUT, stateSpace, stateSize, Nd4j.Random.Seed))
			End If

			checkCudnn(cudnnSetRNNDescriptor_v6(cudnnContext, cudnnContext.rnnDesc, CInt(hiddenLayerSize), NUM_LAYERS, cudnnContext.dropoutDesc, CUDNN_LINEAR_INPUT,If(BIDIRECTIONAL, CUDNN_BIDIRECTIONAL, CUDNN_UNIDIRECTIONAL), RNN_MODE, CUDNN_RNN_ALGO_STANDARD, dataType))

			Dim xDesc0 As cudnnTensorStruct = xDesc.get(GetType(cudnnTensorStruct), 0)
			checkCudnn(cudnnGetRNNParamsSize(cudnnContext, cudnnContext.rnnDesc, xDesc0, sizeInBytes, dataType))
			Dim weightsSize As Long = sizeInBytes.get(0)
			If weightsSize > weightsSpace.capacity() Then
				weightsSpace.deallocate()
				weightsSpace = New DataCache(weightsSize)
			End If
			weightsSpace.limit(weightsSize)

			Dim dimW() As Integer = {CInt(weightsSize) \ dataTypeSize, 1, 1}

			checkCudnn(cudnnSetFilterNdDescriptor(cudnnContext.wDesc, dataType, CUDNN_TENSOR_NCHW, 3, dimW))
			checkCudnn(cudnnSetFilterNdDescriptor(cudnnContext.dwDesc, dataType, CUDNN_TENSOR_NCHW, 3, dimW))

			checkCudnn(cudnnGetRNNWorkspaceSize(cudnnContext, cudnnContext.rnnDesc, CInt(timeSeriesLength), xDesc, sizeInBytes))
			Dim workSize As Long = sizeInBytes.get(0)
			Dim workSpace As DataCache = workspaceMgr.getHelperWorkspace(LayerWorkspaceMgr.CUDNN_WORKSPACE_KEY)
			If workSpace Is Nothing OrElse workSize > workSpace.capacity() Then
				If log.isTraceEnabled() Then
					If workSpace Is Nothing Then
						log.trace("CudnnLSTMHelper activate: Allocating initial workspace of size {} ({})", workSize, BinaryByteUnit.format(workSize, "#.00"))
					Else
						log.trace("CudnnLSTMHelper activate: Deallocating workspace of size {} ({}), allocating new workspace of size {} ({})", workSpace.capacity(), BinaryByteUnit.format(workSpace.capacity(), "#.00"), workSize, BinaryByteUnit.format(workSize, "#.00"))
					End If
				End If
				If workSpace IsNot Nothing Then
					workSpace.deallocate()
				End If
				workSpace = New DataCache(workSize)
				workspaceMgr.setHelperWorkspace(LayerWorkspaceMgr.CUDNN_WORKSPACE_KEY, workSpace)
			End If
			workSpace.limit(workSize)

			checkCudnn(cudnnGetRNNTrainingReserveSize(cudnnContext, cudnnContext.rnnDesc, CInt(timeSeriesLength), xDesc, sizeInBytes))
			Dim reserveSize As Long = sizeInBytes.get(0)
			If reserveSize > reserveSpace.capacity() Then
				reserveSpace.deallocate()
				reserveSpace = New DataCache(reserveSize)
			End If
			reserveSpace.limit(reserveSize)

			Dim allocator As Allocator = AtomicAllocator.Instance
			Dim context As CudaContext = allocator.FlowController.prepareActionAllWrite(x, linInputWeights, linRecurrentWeights, linBiases, prevAct, prevMemCell, outputActivations, finalMemCellState, finalStepActivations)
			Dim xData As Pointer = allocator.getPointer(x, context)
			Dim linInputWeightsData As Pointer = allocator.getPointer(linInputWeights, context)
			Dim linRecurrentWeightsData As Pointer = allocator.getPointer(linRecurrentWeights, context)
			Dim linBiasesData As Pointer = allocator.getPointer(linBiases, context)
			Dim prevActData As Pointer = allocator.getPointer(prevAct, context)
			Dim prevMemCellData As Pointer = allocator.getPointer(prevMemCell, context)
			Dim outputActivationsData As Pointer = allocator.getPointer(outputActivations, context)
			Dim finalMemCellStateData As Pointer = allocator.getPointer(finalMemCellState, context)
			Dim finalTimeStepActivationsData As Pointer = allocator.getPointer(finalStepActivations, context)

			Dim stream As New CUstream_st(context.CublasStream)
			checkCudnn(cudnnSetStream(cudnnContext, stream))

			checkCuda(cudaMemsetAsync(weightsSpace, 0, weightsSpace.limit(), stream))

			Dim dataType(0) As Integer
			Dim format(0) As Integer
			Dim nbDims(0) As Integer
			Dim filterDimA(2) As Integer
			Dim linLayerMat As New Pointer()
			Dim linLayerBias As New Pointer()

			Dim layerNum As Integer = 0
			Do While layerNum < NUM_LAYERS * (If(BIDIRECTIONAL, 2, 1))
				For linLayerID As Integer = 0 To NUM_LINEAR_LAYERS - 1
					checkCudnn(cudnnGetRNNLinLayerMatrixParams(cudnnContext, cudnnContext.rnnDesc, layerNum, xDesc0, cudnnContext.wDesc, weightsSpace, linLayerID, cudnnContext.linLayerMatDesc, linLayerMat))

					checkCudnn(cudnnGetFilterNdDescriptor(cudnnContext.linLayerMatDesc, 3, dataType, format, nbDims, filterDimA))

					checkCudnn(cudnnGetRNNLinLayerBiasParams(cudnnContext, cudnnContext.rnnDesc, layerNum, xDesc0, cudnnContext.wDesc, weightsSpace, linLayerID, cudnnContext.linLayerBiasDesc, linLayerBias))

					checkCudnn(cudnnGetFilterNdDescriptor(cudnnContext.linLayerBiasDesc, 3, dataType, format, nbDims, filterDimA))

					' our data is in "new, forget, output, and input gates" order (aka IFOG), each kind of weight packed together
					Dim position As Integer = 0
					Dim size As Long = 0
					Dim data As Pointer = Nothing
					Select Case linLayerID
						Case 0
							data = linInputWeightsData
							position = 3
							size = inputLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 1
							data = linInputWeightsData
							position = 1
							size = inputLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 2
							data = linInputWeightsData
							position = 0
							size = inputLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 3
							data = linInputWeightsData
							position = 2
							size = inputLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 4
							data = linRecurrentWeightsData
							position = 3
							size = hiddenLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 5
							data = linRecurrentWeightsData
							position = 1
							size = hiddenLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 6
							data = linRecurrentWeightsData
							position = 0
							size = hiddenLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case 7
							data = linRecurrentWeightsData
							position = 2
							size = hiddenLayerSize
'JAVA TO VB CONVERTER TODO TASK: VB does not allow fall-through from a non-empty 'case':
						Case Else
							Throw New Exception()
					End Select
					checkCuda(cudaMemcpyAsync(linLayerMat, data.position(position * size * hiddenLayerSize * dataTypeSize), size * hiddenLayerSize * dataTypeSize, cudaMemcpyDeviceToDevice, stream))
					If linLayerID < 4 Then
						checkCuda(cudaMemcpyAsync(linLayerBias, linBiasesData.position(position * hiddenLayerSize * dataTypeSize), hiddenLayerSize * dataTypeSize, cudaMemcpyDeviceToDevice, stream))
					End If
				Next linLayerID
				layerNum += 1
			Loop

			If training Then
				checkCudnn(cudnnRNNForwardTraining(cudnnContext, cudnnContext.rnnDesc, CInt(timeSeriesLength), xDesc, xData, cudnnContext.hxDesc, prevActData, cudnnContext.cxDesc, prevMemCellData, cudnnContext.wDesc, weightsSpace, yDesc, outputActivationsData, cudnnContext.hyDesc, finalTimeStepActivationsData, cudnnContext.cyDesc, finalMemCellStateData, workSpace, workSpace.limit(), reserveSpace, reserveSpace.limit()))
			Else
				checkCudnn(cudnnRNNForwardInference(cudnnContext, cudnnContext.rnnDesc, CInt(timeSeriesLength), xDesc, xData, cudnnContext.hxDesc, prevActData, cudnnContext.cxDesc, prevMemCellData, cudnnContext.wDesc, weightsSpace, yDesc, outputActivationsData, cudnnContext.hyDesc, finalTimeStepActivationsData, cudnnContext.cyDesc, finalMemCellStateData, workSpace, workSpace.limit()))
			End If

			allocator.FlowController.registerActionAllWrite(context, x, linInputWeights, linRecurrentWeights, linBiases, prevAct, prevMemCell, outputActivations, finalMemCellState, finalStepActivations)

			toReturn.fwdPassOutput = outputActivations.permute(1, 2, 0)
			toReturn.lastAct = finalStepActivations
			toReturn.lastMemCell = finalMemCellState
			toReturn.prevAct = prevAct
			toReturn.prevMemCell = prevMemCell

			Return toReturn
		End Function

		Public Overridable Function helperMemoryUse() As IDictionary(Of String, Long)
			Dim memUse As IDictionary(Of String, Long) = New Dictionary(Of String, Long)()
			memUse("stateStace") = stateSpace.capacity()
			memUse("reserveSpace") = reserveSpace.capacity()
			memUse("weightsSpace") = weightsSpace.capacity()
			Return memUse
		End Function
	End Class

End Namespace